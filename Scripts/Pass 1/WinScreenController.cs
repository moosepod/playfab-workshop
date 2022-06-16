using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using PlayFab;
using PlayFab.ClientModels;

using UnityEngine.SceneManagement;
public class WinScreenController : MonoBehaviour
{
    public TMP_InputField nameInput;
    public TextMeshProUGUI messageLabel;

    public int score = 100;

    public void Start() {
        messageLabel.text = "Please enter your name.";
    }

    public void SubmitScore() {
        if (nameInput.text.Length == 0) {        
            messageLabel.text = "Please enter a name!";
        } else {
            //SceneManager.LoadScene(Constants.SPLASH_SCENE);
            messageLabel.text = "Submitting score...";

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
    }

    private void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("Login was successful.");
        UpdateDisplayName();
    }

    private void FailureCallback(PlayFabError error)
    {
        Debug.LogWarning("Login failed.");
        Debug.LogError(error.GenerateErrorReport());
        messageLabel.text = "Error submitting score";
    }

    public void SubmitScoreToApi() {
        PlayFabClientAPI.UpdatePlayerStatistics(new UpdatePlayerStatisticsRequest {
            Statistics = new List<StatisticUpdate> {
                new StatisticUpdate {
                    StatisticName = "HighScore",
                    Value = score
                }
            }
        }, result=> OnStatisticsUpdated(result), FailureCallback);
    }

    private void OnStatisticsUpdated(UpdatePlayerStatisticsResult updateResult) {
        Debug.Log("Successfully submitted high score");
        messageLabel.text = "Score submitted!";
    }

    public void UpdateDisplayName() {
        PlayFabClientAPI.UpdateUserTitleDisplayName(new UpdateUserTitleDisplayNameRequest {
            DisplayName=nameInput.text
            
        }, result=> OnDisplayNameUpdated(result), FailureCallback);
    }

    private void OnDisplayNameUpdated(UpdateUserTitleDisplayNameResult updateResult) {
        SubmitScoreToApi();
    }
}
