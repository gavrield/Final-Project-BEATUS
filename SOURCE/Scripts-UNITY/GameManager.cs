using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    const int RATE_TO_RAISE = 80;
    const int RATE_TO_DECREASE = 50;
    private int hits = 0;
    private int directHit = 0;
    private int misses = 0;
    private int missInRow = 0;
    private int combo = 0;
    private int score = 0;
    private float accuracyRate = 100.00f;
    private int totalCombos = 0;
    private int lowestScore = 0;
    private int topScore = 0;
    private int scoreFactor = 1;
    private GameObject scoreLive;
    private LiveBoard liveBoard;
    private bool gameOver;
    private float musicLength;
    private bool isAfekaBoost = false;
    private HighscoreTable highScoreTable;
    private float difficulty;
    private int objectTimeToArrive;
    private int timerLeft = 0;
    private bool isFinishScreenActive = false;
    public FinishBoard finishBoard;
    public GameObject laserPointer;
    public GameObject Slicer;
    private Component sideLights;
    private Component comboUpdateSign;
    private Component goodLuckLabel;
    private Component RaiseLevelLabel;
    private Component DecreaseLevelLabel;
    private Component NewHighScoreLabel;
    private Component TopScoreLabel;
    private Component boostTimer;
    private bool specialBoostLocked = false;
    public AudioSource gameOverSound;
    public AudioSource successSound;
    public AudioSource losingSound;
    public AudioSource TimeUp;
    public AudioSource music;
    public AudioSource comboSound;
    public AudioSource goodBoostSound;
    public AudioSource badBoostSound;
    public Component TopScoreFireWorks;
    public Component finishFireworks;
    private string song;
    private string bpmFolder;
    [SerializeField] int scorePerArrowHit = 120;//hit with the arrow
    [SerializeField] int scorePerUnArrowHit = 30;//hit without the arrow
    [SerializeField] int scorePerMiss = -80;//Reduce points if missed the object or hit with wrong saber
    [SerializeField] int scorePerDistraction = -100;//Reduce points if hit the distraction or distraction damage player
    [SerializeField] int scorePerComboFive = 300;//Bonus for 5 good hits in a raw
    [SerializeField] int scorePerHitAfekaCircle = 40;//Score for hitting white circle


    // Start is called before the first frame update
    void Awake()
    {
        gameOver = false;
        song = PlayerPrefs.GetString("song");
        music.clip = Resources.Load<AudioClip>("Music/GameSongs/" + song);
        music.Play();
        difficulty = PlayerPrefs.GetFloat("level", 1);
        musicLength = PlayerPrefs.GetInt("songLength", 1);
        highScoreTable = FindObjectOfType<HighscoreTable>();
        lowestScore = PlayerPrefs.GetInt("lowestScore", 0);
        topScore = PlayerPrefs.GetInt("topScore", 0);
        liveBoard = FindObjectOfType<LiveBoard>();
        laserPointer.SetActive(false);
        sideLights = GameObject.Find("Sides").GetComponent<Component>();
        comboUpdateSign = GameObject.Find("ComboUpdateSign").GetComponent<Component>();
        goodLuckLabel = GameObject.Find("GoodLuckLabel").GetComponent<Component>();
        RaiseLevelLabel = GameObject.Find("RaiseLevel").GetComponent<Component>();
        DecreaseLevelLabel = GameObject.Find("DecreaseLevel").GetComponent<Component>();
        TopScoreLabel = GameObject.Find("TopScore").GetComponent<Component>();
        NewHighScoreLabel = GameObject.Find("NewHighScore").GetComponent<Component>();
        boostTimer = GameObject.Find("boostTimer").GetComponent<Component>();
        StartCoroutine(CheckNewHighScore());
        SwitchSlicer(true);
    }
    private void Start()
    {
        liveBoard.InitLabels(GetDifficultyName(), GetMusicLength());
        //Close all Switches
        SwitchTimerLabel(false);
        SwitchComboUpdateSign(false);
        SwitchChangeLevelLabel(false, false);
        SwitchChangeLevelLabel(false, true);
        SwitchAlertNewHighscoreLabel(false, false);
        SwitchAlertNewHighscoreLabel(false, true);
    }
    IEnumerator CheckNewHighScore()
    {
        float t = 0;
        bool newHighScore = false;
        bool isTopScore = false;
        while (t <= musicLength)
        {
            t += Time.deltaTime;
            if (!isTopScore)
            {
                if (score > lowestScore)
                {
                    //Check if its top or only new
                    if (score > topScore)
                    {
                        isTopScore = true;
                        StartCoroutine(PlayFireWorks(TopScoreFireWorks));
                        SwitchAlertNewHighscoreLabel(true, true);
                        break;
                    }
                    else if (!newHighScore)
                    {
                        newHighScore = true;
                        successSound.Play();
                        SwitchAlertNewHighscoreLabel(true, false);
                    }
                }
            }
            else
            {
                t = musicLength; //break the function - we found the top score
            }
            yield return null;
        }
        SwitchAlertNewHighscoreLabel(false, false);
    }
    IEnumerator ShowComboUpdateSign()
    {
        SwitchComboUpdateSign(true);
        float t = 0;
        comboSound.Play();
        while (t <= 1.5)
        {
            t += Time.deltaTime;
            yield return null;
        }
        SwitchComboUpdateSign(false);
    }
    IEnumerator PlayFireWorks(Component fireWorks)
    {
        fireWorks.gameObject.SetActive(true);
        float t = 0;
        while (t <= 3)
        {
            t += Time.deltaTime;
            yield return null;
        }
        fireWorks.gameObject.SetActive(false);
    }
    IEnumerator TimerLabel(int timer, string boostName)
    {
        //Turn on timer of double
        SwitchTimerLabel(true);
        float t = 0;
        bool ring = false;
        //Play timeup 3 seconds before ends
        TimeUp.PlayDelayed(timer - 3);
        while (t <= timer)
        {
            //update timer of double
            t += Time.deltaTime;
            liveBoard.UpdateTimerLiveLabel(t, timer);
            SetTimerLeft(timer - t);
            yield return null;
        }
        specialBoostLocked = false;
        //Return the facor to 1
        SwitchTimerLabel(false);
        if (boostName == "X2")
        {
            SetScoreFactor(1);
        }
        else if (boostName == "afekaBoost")//AfekaBoost
        {
            SwitchSidesLight(true);
            isAfekaBoost = false;
        }
    }

    IEnumerator DisableSwitchChangeLevelLabel(bool isRaise)
    {
        float t = 0;
        while (t <= 1.5)
        {
            t += Time.deltaTime;
            yield return null;
        }
        SwitchChangeLevelLabel(false, isRaise);
    }

    IEnumerator DisableSwitchHighScoreAlertLabel(bool isRaise)
    {
        float t = 0;
        while (t <= 1.5)
        {
            t += Time.deltaTime;
            yield return null;
        }
        SwitchAlertNewHighscoreLabel(false, isRaise);
    }
    //Hit & Miss
    public void HitBoost(string tag, GameObject boostObject)
    {
        String sign = "";
        switch (tag)
        {
            //Raise 1000 points
            case "BonusBoost":
                sign = boostObject.GetComponent<BonusBoost>().getBoostSign();
                String stringAmount = boostObject.GetComponent<BonusBoost>().getBoostAmount();
                int amount = int.Parse(stringAmount);
                if (sign == "-")
                {
                    amount *= -1;
                    badBoostSound.Play();
                }
                SetScore(amount);
                liveBoard.UpdateStatistics(2, score, accuracyRate);
                break;
            //Double the points for period time
            case "X2":
                if (!specialBoostLocked) //if there is no active X2
                {
                    specialBoostLocked = true;
                    SetScoreFactor(2);
                    //Disable the double factor after period time
                    StartCoroutine(TimerLabel(20, tag));
                }
                break;
            //Hit any cube ,the color doesn't mean
            case "afekaBoost":
                if (!specialBoostLocked) //if there is no active X2
                {
                    //goodBoostSound.Play();
                    isAfekaBoost = true;
                    SwitchSidesLight(false);
                    StartCoroutine(TimerLabel(15, tag));
                    if (difficulty == 1)
                    {
                        SetObjectTimeToArrive(5);
                    }
                    else
                    {
                        SetObjectTimeToArrive(3);
                    }
                }
                break;
        }
        if (sign != "-")
        {
            goodBoostSound.Play();
        }
    }

    public void HitAfekaCircle()
    {
        SetScore(scorePerHitAfekaCircle);
        goodBoostSound.Play();
        liveBoard.UpdateStatistics(0, score, accuracyRate);
    }
    //true=in direct , false- hit but not in arrow direction
    public void Hit(bool isDirect)
    {
        hits++;
        missInRow = 0;
        if (isDirect)
        {
            directHit++;
            RaiseCombo();
            SetScore(scorePerArrowHit);

            if (combo == 5)
            {
                StartCoroutine(ShowComboUpdateSign());
                SetScore(scorePerComboFive);
                ZeroCombo();
                RaiseTotalCombo();
            }
        }
        else
        {
            ZeroCombo();
            SetScore(scorePerUnArrowHit);
        }
        UpdateAccuracyRate();
        liveBoard.UpdateStatistics(0, score, accuracyRate);

    }

    public void Missed(bool isDistraction)
    {
        int points;
        if (isDistraction)
        {
            points = scorePerDistraction;
        }
        else
        {
            points = scorePerMiss;
        }
        SetScore(points);
        misses++;
        UpdateAccuracyRate();
        liveBoard.UpdateStatistics(1, score, accuracyRate);
        missInRow++;
        if (misses >= 7 - difficulty)
        {
            CheckGameOver();
        }
        ZeroCombo();
    }


    //Updates:
    private void UpdateAccuracyRate()
    {
        accuracyRate = (100f * directHit) / GetNumOfObjects();
        //print("THE ACCURACY REATE IS: "+ accuracyRate);
    }
    public void updateTime(int time)
    {
        liveBoard.UpdateTimeLive(time);
    }

    //Sets
    private void SetObjectTimeToArrive(int time)
    {
        objectTimeToArrive = time;
    }
    private void SetTimerLeft(float time)
    {
        timerLeft = (int)time;
    }
    private void SetScore(int additionToScore)
    {
        score += (additionToScore * scoreFactor);
    }
    private void SetDifficulty(int difficulty)
    {
        this.difficulty = difficulty;
    }
    private void SetScoreFactor(int facor)
    {
        scoreFactor = facor;
    }
    //Gets:
    public int GetObjectTimeToArrive()
    {
        return objectTimeToArrive;
    }
    public int GetTimerLeft()
    {
        return timerLeft;
    }
    public bool GetIsAfekaBoost()
    {
        return isAfekaBoost;
    }
    public string GetSongName()
    {
        return song;
    }
    public float GetMusicLength()
    {
        return musicLength;
    }
    public bool GetSpecialBoostLocked()
    {
        return specialBoostLocked;
    }
    public int GetScore()
    {
        return score;
    }
    public int GetNumOfObjects()
    {
        return (hits + misses);
    }
    public bool GetGameOver()
    {
        return gameOver;
    }
    public double GetAccuracyRate()
    {
        return accuracyRate;
    }
    public int GetHits()
    {
        return hits;
    }
    public int GetTotalCombos()
    {
        return totalCombos;
    }
    public int GetLowestScore()
    {
        return lowestScore;
    }
    public float GetDifficulty()
    {
        return difficulty;
    }
    public bool GetFinishScreenActivation()
    {
        return isFinishScreenActive;
    }
    public string GetDifficultyName()
    {
        string difficultyString = "";
        switch (difficulty)
        {
            case 1:
                difficultyString = "Easy";
                break;
            case 2:
                difficultyString = "Medium";
                break;
            case 3:
                difficultyString = "Hard";
                break;
        }
        return difficultyString;
    }

    //Switches:
    public void TurnOffLiveBoard()
    {
        liveBoard.TurnOffLiveBoard();
    }
    public void SwitchSidesLight(bool isActive)
    {
        sideLights.gameObject.SetActive(isActive);
    }

    private void SwitchComboUpdateSign(bool isActive)
    {
        comboUpdateSign.gameObject.SetActive(isActive);
    }
    private void SwitchSlicer(bool isActive)
    {
        Slicer.gameObject.SetActive(isActive);
    }

    public void SwitchGoodluckLabel(bool isActive)
    {
        goodLuckLabel.gameObject.SetActive(isActive);
    }
    public void SwitchTimerLabel(bool isActive)
    {
        boostTimer.gameObject.SetActive(isActive);
    }
    public void SwitchMusic(bool toPlay)
    {
        if (toPlay) //play music
        {
            music.Play();
        }
        else //stop music
        {
            music.Stop();
        }
    }
    public void SwitchChangeLevelLabel(bool isActive, bool isRaise)
    {
        if (isRaise)
        {
            //Switch to raise label
            RaiseLevelLabel.gameObject.SetActive(isActive);
        }
        else
        {
            //Switch the decrease label
            DecreaseLevelLabel.gameObject.SetActive(isActive);
        }
        if (isActive)
        {
            StartCoroutine(DisableSwitchChangeLevelLabel(isRaise));
        }
    }
    public void SwitchAlertNewHighscoreLabel(bool isActive, bool isTop)
    {
        if (isTop)
        {
            //Switch to topHighScore label
            TopScoreLabel.gameObject.SetActive(isActive);
        }
        else
        {
            //Switch the newHighScore label
            NewHighScoreLabel.gameObject.SetActive(isActive);
        }
        if (isActive)
        {
            StartCoroutine(DisableSwitchHighScoreAlertLabel(isTop));
        }
    }
    //OTHER
    private void CheckGameOver()
    {
        //Easy = 6 misses, Medium = 5 misses, Hard = 4 misses
        if (missInRow >= 7 - difficulty)
        {
            gameOver = true;  //FINISH GAME
            music.Stop();
            gameOverSound.Play();
            ActivateFinishScreen();
            SwitchSlicer(false);
        }
    }

    private void RaiseCombo()
    {
        combo++;
    }

    private void RaiseTotalCombo()
    {
        totalCombos++;
        liveBoard.UpdateCombosLive(totalCombos);
    }

    private void ZeroCombo()
    {
        combo = 0;
    }

    public void AddNewHighScore(int score, string name, string accuracy)
    {
        highScoreTable.AddHighscoreEntry(score, name, accuracy);
    }

    public void ActivateFinishScreen()
    {
        isFinishScreenActive = true;
        finishBoard.ActiveFinishBoard();
        TurnOffLiveBoard();
        bool isNewHighScore = IsNewHighScore(score, lowestScore);
        if (isNewHighScore)
        {
            PlayFireWorks(finishFireworks);
        }
        finishBoard.ShowFinishScreen(isNewHighScore, gameOver, score, hits, totalCombos, GetDifficultyName(), accuracyRate, GetNumOfObjects());
        laserPointer.SetActive(true);
    }

    private bool IsNewHighScore(int newScore, int lowestScore)
    {
        return newScore >= lowestScore;
    }

    public bool CheckLevel()
    {
        bool changeLevel = false;
        switch (difficulty)
        {
            case 1:
                if (accuracyRate >= RATE_TO_RAISE)
                {
                    //Upgrade difficulty to medium
                    difficulty = 2;
                    changeLevel = true;
                    successSound.Play();
                }
                break;
            case 2:
                if (accuracyRate >= RATE_TO_RAISE)
                {
                    //Upgrade difficulty to Hard
                    difficulty = 3;
                    changeLevel = true;
                    successSound.Play();
                }
                else if (accuracyRate <= RATE_TO_DECREASE)
                {
                    //Decrease difficuly to Easy
                    difficulty = 1;
                    changeLevel = true;
                    losingSound.Play();
                }
                break;
            case 3:
                if (accuracyRate <= RATE_TO_DECREASE)
                {
                    //Decrease difficuly to Medium
                    difficulty = 2;
                    changeLevel = true;
                    losingSound.Play();
                }
                break;
        }
        if (changeLevel)
        {
            liveBoard.UpdateDifficultyLive(GetDifficultyName());
            return true; //Level has been changed
        }
        return false; //Level has not been change
    }
}
