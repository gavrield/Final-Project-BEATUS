
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class FinishBoard : MonoBehaviour
{
    public Text scoreText;
    public Text hitsText;
    public Text combosText;
    public Text accuracyRateText;
    public Text difficultyText;
    public GameObject KeyBoard;
    public GameObject NameLabel;
    public GameObject NameTitle;
    public GameObject SaveHighscore;
    public GameObject GameFinishTitle;
    public GameObject GameOverTitle;
    public GameObject GameOverMsg;
    public GameObject HighScoreLabel;

    public void ActiveFinishBoard()
    {
        gameObject.SetActive(true);
    }

    public void ShowFinishScreen(bool newHighScore,bool isGameOver,int score,int hits,int combos,string difficultyName,float accurayRate,int numOfObjects)
    {
        if(newHighScore)
        {
            NewHighScore();
        }
        else
        {
            ActivateLabels(isGameOver);
        }
        //Take all Details from the gameManager and update in this screen
        UpdateScores(score,hits,combos,difficultyName,accurayRate,numOfObjects);
    }

    private void ActivateLabels(bool isGameOver)
    {
        SaveHighscore.SetActive(false);
        HighScoreLabel.SetActive(false);
        NameLabel.SetActive(false);
        SelectTitleToFinishScreen(isGameOver);
    }

    private void SelectTitleToFinishScreen(bool isGameOver)
    {
        if (isGameOver)
        {
            GameOverMsg.SetActive(true);
            NameTitle.SetActive(false);
            GameOverTitle.SetActive(true);
        }
        else
        {
            GameFinishTitle.SetActive(true);
        }
    }

    private void UpdateScores(int newScore, int hits, int combos, string difficultyName, float accurayRate, int numOfObjects)
    {
        print("sdsd");
        print(newScore.ToString());
        scoreText.text = newScore.ToString();
        hitsText.text = hits.ToString() + "/" + numOfObjects.ToString();
        combosText.text = combos.ToString();
        difficultyText.text = difficultyName;
        accuracyRateText.text = accurayRate.ToString("n2") + "%";
    }

    private void NewHighScore()
    {
        KeyBoard.SetActive(true);
        SaveHighscore.SetActive(true);
        HighScoreLabel.SetActive(true);
        GameFinishTitle.SetActive(false);
        NameLabel.SetActive(true); //And keyboard for writing name
    }
}