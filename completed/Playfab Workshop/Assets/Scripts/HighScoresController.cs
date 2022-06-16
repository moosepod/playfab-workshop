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

        messageLabel.text = "Loading...";

        RequestLeaderboard();
    }

    public void ReturnToSplash() {
        SceneManager.LoadScene(Constants.SPLASH_SCENE);
    }

    public void RequestLeaderboard() {
        ScoreAPI.Instance.FetchHighScores(DisplayLeaderboardCallback, FailureCallback);
    }

    private void DisplayLeaderboardCallback(HighScore[] highScores ) {
        messageLabel.gameObject.SetActive(false);
        for (int i=0; i < highScores.Length; i++ ) {
            if (i < scoreRows.Length) {
                HighScore entry = highScores[i]; 
                scoreRows[i].displayNameLabel.text = entry.DisplayName;
                scoreRows[i].scoreLabel.text = entry.Score.ToString();
                
                scoreRows[i].displayNameLabel.gameObject.transform.parent.gameObject.SetActive(true);
            }
        }
    }

    private void FailureCallback(string mesage){
        messageLabel.gameObject.SetActive(true);
        messageLabel.text = "Error fetching high scores...";
    }
}
