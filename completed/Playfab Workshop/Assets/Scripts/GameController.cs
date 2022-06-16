using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using UnityEngine.SceneManagement;
public class GameController : MonoBehaviour
{
    public TMP_InputField guessInput;
    public TextMeshProUGUI messageLabel;
    public TextMeshProUGUI scoreLabel;
    public int startCounter = 100;
    public int scoreMultiplier = 100;
    public int maxNumber;
    public int minNumber;

    private int targetNumber = 0;
    private float counter = 0;
    private int score;

    public void Start() {
        GenerateTargetNumber();
        messageLabel.gameObject.SetActive(false);
        counter = startCounter;
        score =  scoreMultiplier * (int) counter;
    }

    public void Update() {
        counter -= Time.deltaTime;
        score = scoreMultiplier * (int) counter;
        scoreLabel.text = String.Format("{0}",score);
        if (score <= 0) {
           SceneManager.LoadScene(Constants.LOSE_GAME_SCENE);
        }
    }

    public void Guess() {
        try {
            int guess = int.Parse(guessInput.text);
            if (guess == targetNumber) {
                ScoreAPI.Instance.SetLatestScore(score);
                 SceneManager.LoadScene(Constants.WIN_GAME_SCENE);
            } else if (guess > targetNumber) {
                ShowMessage(String.Format("{0} is too high!", guess));
            } else {
                ShowMessage(String.Format("{0} is too low!", guess));
            }
            guessInput.text = "";
        } 
         catch (FormatException)
          {
              ShowMessage("Invalid number");
          }
       
    }

    public void GenerateTargetNumber() {
        targetNumber = UnityEngine.Random.Range(minNumber,maxNumber);
        Debug.LogFormat("Target is {0}",targetNumber);
    }

    void ShowMessage(string message) {
        messageLabel.text = message;
        messageLabel.gameObject.SetActive(true);
    }
}
