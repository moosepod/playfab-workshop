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

    public void Start() {
        messageLabel.text = "Please enter your name.";
    }

    public void SubmitScore() {
        if (nameInput.text.Length == 0) {        
            messageLabel.text = "Please enter a name!";
        } else {
            messageLabel.text = "Publishing score...";
            ScoreAPI.Instance.PublishCurrentScoreWithName(nameInput.text, SuccessCallback, FailureCallback);
        }
    }

    private void SuccessCallback()
    {    
        SceneManager.LoadScene(Constants.SPLASH_SCENE);
    }

    private void FailureCallback(string message)
    {
        messageLabel.text = message;
    }
}
