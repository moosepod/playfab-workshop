using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    }

    public void ReturnToSplash() {
        SceneManager.LoadScene(Constants.SPLASH_SCENE);
    }
}
