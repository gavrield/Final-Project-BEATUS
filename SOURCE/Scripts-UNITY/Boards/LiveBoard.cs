using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LiveBoard : MonoBehaviour
{
    private Text scoreText;
    private Text combosText;
    private Text timeText;
    private Text timeTotalText;
    private Text missText;
    private Text difficultyText;
    private Text accuracyText;
    private Text boostTimerText;

    // Use this for init
    void Awake()
    {
        scoreText = GameObject.Find("scoreLiveValue").GetComponent<Text>();
        missText = GameObject.Find("missLiveValue").GetComponent<Text>();
        timeText = GameObject.Find("timeLiveValue").GetComponent<Text>();
        timeTotalText = GameObject.Find("timeLiveLabel").GetComponent<Text>();
        combosText = GameObject.Find("combosLiveValue").GetComponent<Text>();
        accuracyText = GameObject.Find("accuracyLiveValue").GetComponent<Text>();
        difficultyText = GameObject.Find("difficultyLiveValue").GetComponent<Text>();
        boostTimerText = GameObject.Find("boostTimerText").GetComponent<Text>();
    }

    //INIT LABELS:
    public void InitLabels(string difficultyName, float totalTime)
    {
        UpdateTimeLiveLabel((int)totalTime);
        UpdateDifficultyLive(difficultyName);
    }

    //UPDATES:
    public void UpdateStatistics(int missesChange, int score, float accuracyRate)
    {
        UpdateMissSign(missesChange);
        UpdateScoreLive(score);
        UpdateAccurateLive(accuracyRate);
    }
    private void UpdateScoreLive(int score)
    {
        scoreText.text = score.ToString();
    }
    public void UpdateCombosLive(int combos)
    {
        combosText.text = combos.ToString();
    }
    public void UpdateDifficultyLive(string difficulty)
    {
        difficultyText.text = difficulty.ToString();
    }
    private void UpdateAccurateLive(float rate)
    {
        accuracyText.text = rate.ToString("n2") + "%";
    }
    public void UpdateTimeLive(int time)
    {
        timeText.text = time.ToString();
    }
    public void UpdateTimeLiveLabel(float time)
    {
        timeTotalText.text = "/" + time.ToString() + " Seconds";
    }
    public void UpdateTimerLiveLabel(float timeElpased,float doubleTime)
    {
        int timer = (int)(doubleTime - timeElpased);
        boostTimerText.text = timer.ToString();
    }
    private void UpdateMissSign(int x)
    {
        if (x == 0)
        {
            missText.text = "";
        }
        else if(x == 1)
        {
            missText.text = missText.text + "X";
        }
    }

    //KILL THIS LIVE BOARD
    public void TurnOffLiveBoard()
    {
        gameObject.SetActive(false);
    }
}