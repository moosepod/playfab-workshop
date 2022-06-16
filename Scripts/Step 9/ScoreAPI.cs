using System;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;

public struct HighScore {
    public HighScore(string displayName, int score) {
        DisplayName = displayName;
        Score = score;
    }
    
    public string DisplayName { get; }
    public int Score {get; }
}

public class ScoreAPI : MonoBehaviour
{
    const string HIGH_SCORE_STATISTIC = "HighScore";
    const string PLAYER_ID_PREF = "PlayerId";
    const int HIGH_SCORE_START_POSITION = 0;
    const int HIGH_SCORE_FETCH_COUNT = 10;

    public string playerIdOverride;

    private int latestScore = 250;

    private static ScoreAPI instance;
	public static ScoreAPI Instance
	{
		get { return instance; }
	}

	private void Awake()
	{
		if (instance != null && instance != this)
		{
			Destroy(this.gameObject);
			return;
		}

		instance = this;

		DontDestroyOnLoad(this.gameObject);
	}

    public void SetLatestScore(int score) {
        latestScore = score;
    }

    public void FetchHighScores(Action<HighScore[]>  successCallback, Action<string> errorCallback) {
        // Fetch top high scores from the API
        LoginThen(() => {

                Debug.Log("API: Fetching scores...");
                PlayFabClientAPI.GetLeaderboard(new GetLeaderboardRequest {
                StatisticName = HIGH_SCORE_STATISTIC,
                StartPosition = HIGH_SCORE_START_POSITION,
                MaxResultsCount = HIGH_SCORE_FETCH_COUNT
                }, 
                result=> {
                    HighScore[] scores = new HighScore[result.Leaderboard.Count];
                    for (int i=0; i < result.Leaderboard.Count; i++ ) {                        
                    PlayerLeaderboardEntry entry = result.Leaderboard[i];
                        scores[i] = new HighScore(entry.DisplayName, entry.StatValue);
                    }
                    successCallback(scores);
                }, 
                errorResult => {
                    Debug.LogWarning("Something went wrong with fetching high scores:");
                    Debug.LogError(errorResult.GenerateErrorReport());
                    errorCallback("Error fetching high scores");
                });
            },
            errorMessage => {
                errorCallback(errorMessage);
            });
    }

    public void PublishCurrentScoreWithName(string displayName, Action successCallback, Action<string> errorCallback) {
        // Make two api calls, first to set the name of the current user, then to set their latest score.
        // Always call login first -- less efficient, but avoids having to deal with re-logging in on session timeout
        LoginThen(() => {
            SetDisplayName(displayName, 
                () => {
                    PublishScore(latestScore, 
                        successCallback,
                        errorCallback);
                },
                errorCallback);
            }, 
            errorMessage => {
                errorCallback(errorMessage);
            }
        );
    }

    void SetDisplayName(string displayName, Action successCallback, Action<string> errorCallback) {
        // Make an API call to set the logged-in users current name
        Debug.Log("API: Setting display name...");
        PlayFabClientAPI.UpdateUserTitleDisplayName(new UpdateUserTitleDisplayNameRequest {
            DisplayName=displayName
            
            }, 
        result=> {
            successCallback();
        }, 
        errorResult => {
            Debug.LogWarning("Something went wrong with setting display name:");
            Debug.LogError(errorResult.GenerateErrorReport());
            errorCallback("Error updating high score");
        });
    }

    void PublishScore(int score, Action successCallback, Action<string> errorCallback) {
        // Make an API call to set the latest high score for the logged in user
        Debug.Log("API: Publishing score...");
        PlayFabClientAPI.UpdatePlayerStatistics(new UpdatePlayerStatisticsRequest {
            Statistics = new List<StatisticUpdate> {
                new StatisticUpdate {
                    StatisticName = HIGH_SCORE_STATISTIC,
                    Value = score
                }
            }
        }, 
        result=> {
            successCallback();
        }, 
        errorResult => {
            Debug.LogWarning("Something went wrong with setting high score:");
            Debug.LogError(errorResult.GenerateErrorReport());
            errorCallback("Error updating high score");
        });
    }

    string GetPlayerID() {
        // Return the persistent ID for the current player. This will be auto-generated and
        // stored in preferences. Of course this means if the preferences are removed/editing, this 
        // ID will change

        // To make testing easier, allow overriding this ID from the editor
        if (this.playerIdOverride.Length > 0) {
            return this.playerIdOverride;
        }

        string playerId = PlayerPrefs.GetString(PLAYER_ID_PREF);
        if (playerId == "") {
            playerId = System.Guid.NewGuid().ToString();
            PlayerPrefs.SetString(PLAYER_ID_PREF,playerId);
        }

        return playerId;
    }

    void LoginThen( Action successCallback, Action<string> errorCallback) {
        // Login the user to the API, then call either success or error. Error will be 
        // passed an error message as a string
        Debug.Log("API: Logging in...");
        var request = new LoginWithCustomIDRequest { CustomId = GetPlayerID(), CreateAccount = true};
        PlayFabClientAPI.LoginWithCustomID(request, 
            result => {
                successCallback();
            }, 
            errorResult => {
                Debug.LogWarning("Something went wrong with login:");
                Debug.LogError(errorResult.GenerateErrorReport());
                errorCallback("Error logging into high score server");
            });
    }
}
