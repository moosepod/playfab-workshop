using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine.SceneManagement;

[System.Serializable]
public class HighScoreRow {
    public TextMeshProUGUI displayNameLabel;
    public TextMeshProUGUI scoreLabel;
}

public class HighScoresController : MonoBehaviour
{
    public HighScoreRow[] scoreRows;
    public TextMeshProUGUI messageLabel;

    public void Start() {
        for (int i=0; i<scoreRows.Length;i++) {
            scoreRows[i].displayNameLabel.gameObject.transform.parent.gameObject.SetActive(false);
        }
        messageLabel.gameObject.SetActive(false);

        messageLabel.text = "Loading...";

        // Fetch the player's unique ID from the preferences. Note that this will change if
        // the preferences is ever cleared, and can be manually changed, so it's not secure!
        string playerId = PlayerPrefs.GetString("PlayerId");
        if (playerId == "") {
            playerId = System.Guid.NewGuid().ToString();
            PlayerPrefs.SetString("PlayerId",playerId);
        }
        
        var request = new LoginWithCustomIDRequest { CustomId = playerId, CreateAccount = true};
        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, FailureCallback);

    }

     private void OnLoginSuccess(LoginResult result)
    {
        RequestLeaderboard();
    }

    public void ReturnToSplash() {
        SceneManager.LoadScene(Constants.SPLASH_SCENE);
    }

    public void RequestLeaderboard() {
        PlayFabClientAPI.GetLeaderboard(new GetLeaderboardRequest {
                StatisticName = "HighScore",
                StartPosition = 0,
                MaxResultsCount = 10
        }, result=> DisplayLeaderboard(result), FailureCallback);
    }

    private void DisplayLeaderboard(GetLeaderboardResult result ) {
        messageLabel.gameObject.SetActive(false);
        for (int i=0; i < result.Leaderboard.Count; i++ ) {
            if (i < scoreRows.Length) {
                PlayerLeaderboardEntry entry = result.Leaderboard[i];
                scoreRows[i].displayNameLabel.text = entry.DisplayName;
                scoreRows[i].scoreLabel.text = entry.StatValue.ToString();
                
                scoreRows[i].displayNameLabel.gameObject.transform.parent.gameObject.SetActive(true);
            }
        }
    }

    private void FailureCallback(PlayFabError error){
        Debug.LogWarning("Something went wrong with your API call. Here's some debug information:");
        Debug.LogError(error.GenerateErrorReport());
        messageLabel.gameObject.SetActive(true);
        messageLabel.text = "Error fetching high scores...";
    }
}
