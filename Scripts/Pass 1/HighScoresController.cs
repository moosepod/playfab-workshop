using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine.SceneManagement;
public class HighScoresController : MonoBehaviour
{
    public TextMeshProUGUI[] scoreLabels;
    public TextMeshProUGUI messageLabel;

    public void Start() {
        for (int i=0; i<scoreLabels.Length;i++) {
            scoreLabels[i].gameObject.SetActive(false);
        }

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
            if (i < scoreLabels.Length) {
                PlayerLeaderboardEntry entry = result.Leaderboard[i];
                scoreLabels[i].text = string.Format("{0}) {1} - {2}",i+1,entry.DisplayName, entry.StatValue);
                scoreLabels[i].gameObject.SetActive(true);
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
