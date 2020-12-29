using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class HighscoreTable : MonoBehaviour
{
    private Transform entryContainer;
    private Transform entryTemplate;
    private List<HighscoreEntry> highscoreEntryList;
    private List<Transform> highscoreEntryTransformList;
    private int lowestScore = 0;
    private void Awake()
    {
        entryContainer = transform.Find("highscoreEntryContainer");  //Get the container
        entryTemplate = entryContainer.Find("highscoreEntryTemplate"); //Get the template from the container
        entryTemplate.gameObject.SetActive(false); //Set the template(highscores) hidden

        //CleanHighscores();
        //AddHighscoreEntry(900, "Gavriel");

        Highscores highscores = GetHighScoreFromJson(); //Load highscores
        highscoreEntryTransformList = new List<Transform>();
        try
        {
            foreach (HighscoreEntry highscoreEntry in highscores.highscoreEntryList)
            {
                CreateHighscoreEntryTransform(highscoreEntry, entryContainer, highscoreEntryTransformList); //Enter scores into the table
            }
        }
        catch { }
        SaveLowestScore(highscores.highscoreEntryList[highscores.highscoreEntryList.Count - 1].score);
        saveTopScore(highscores.highscoreEntryList[0].score);
    }

    private void CreateHighscoreEntryTransform(HighscoreEntry highscoreEntry, Transform container, List<Transform> transformList)
    {
        float xPos = -350f;
        float yStartPos = 725f;
        float templateHeight = 250f; //Distance in 'y' between scores
        Transform entryTransform = Instantiate(entryTemplate, container); //Clone entryTransform inside entryTemplate and container
        entryTransform.transform.localPosition = new Vector2(xPos, yStartPos + (-templateHeight * transformList.Count));
        entryTransform.gameObject.SetActive(true);

        int rank = transformList.Count + 1;

        Text pos = entryTransform.Find("PositionText").GetComponent<Text>();
        Text score = entryTransform.Find("ScoreText").GetComponent<Text>();
        Text name = entryTransform.Find("NameText").GetComponent<Text>();
        Text accuracy = entryTransform.Find("AccuracyText").GetComponent<Text>();

        TransformHichscoreContainer(highscoreEntry, pos, score, name,accuracy, rank);

        //Highlight first place
        if (rank == 1)
        {
            PaintScore(pos, score, name, accuracy, Color.green);
        }

        transformList.Add(entryTransform);
    }

    private static void TransformHichscoreContainer(HighscoreEntry highscoreEntry, Text pos, Text score, Text name,Text accuracy, int rank)
    {
        string rankString = GetRankString(rank);
        pos.text = rankString;
        score.text = (highscoreEntry.score).ToString();
        name.text = highscoreEntry.name;
        accuracy.text = highscoreEntry.accuracy;
    }

    private static void PaintScore(Text pos, Text score, Text name, Text accuracy, Color color)
    {
        pos.color = color;
        score.color = color;
        name.color = color;
        accuracy.color = color;
    }

    private static string GetRankString(int rank)
    {
        string rankString;
        switch (rank)
        {
            default: rankString = rank + "TH"; break;
            case 1: rankString = "1ST"; break;
            case 2: rankString = "2ND"; break;
            case 3: rankString = "3RD"; break;
        }

        return rankString;
    }

    public void AddHighscoreEntry(int newScore, string name,string accuracy)
    {
        HighscoreEntry highscoreEntry = new HighscoreEntry { score = newScore, name = name, accuracy = accuracy };//Create HighscoreEntry
        Highscores highscores = GetHighScoreFromJson();//Load highscores
        bool added = true;
        int numOfScores = highscores.highscoreEntryList.Count;
        if (numOfScores > 9) //If more than 9 scores insert only if its improvement
        {
            added = false;
            HighscoreEntry lastScore = highscores.highscoreEntryList[9];
            if (newScore > lastScore.score)
            {
                highscores.highscoreEntryList.RemoveAt(numOfScores - 1); //Remove last score from highscores
                numOfScores--;
                highscores.highscoreEntryList.Add(highscoreEntry);
                added = true;
            }
        }
        else //less than 10 scores insert anyway
        {
            highscores.highscoreEntryList.Add(highscoreEntry);
        }

        if (numOfScores > 0 && added == true)
        {
            highscores = SortHighscores(highscores, numOfScores);//Sort
        }

        SaveHighscoreToJson(highscores);//Save updated highscores
    }

    private static void SaveHighscoreToJson(Highscores highscores)
    {
        string json = JsonUtility.ToJson(highscores);
        PlayerPrefs.SetString("highscoreTable", json);
        PlayerPrefs.Save();
    }

    private static void SaveLowestScore(int lowestScore)
    {
        PlayerPrefs.SetInt("lowestScore", lowestScore);
    }
    private static void saveTopScore(int topScore)
    {
        PlayerPrefs.SetInt("topScore", topScore);
    }

    private static Highscores GetHighScoreFromJson()
    {
        string jsonString = PlayerPrefs.GetString("highscoreTable");
        Highscores highscores = JsonUtility.FromJson<Highscores>(jsonString);
        return highscores;
    }
    public int getLowestScore()
    {
        return this.lowestScore;
    }
    //insertation sort
    private Highscores SortHighscores(Highscores highscores, int numOfScores)
    {
        HighscoreEntry newHighscoreUnsorted = highscores.highscoreEntryList[numOfScores];
        if (numOfScores == 10)
        {
            lowestScore = highscores.highscoreEntryList[numOfScores - 1].score;
        }
        for (int i = 0; i < numOfScores; i++)
        {
            HighscoreEntry currectHighScoreEntry = highscores.highscoreEntryList[i];
            if (currectHighScoreEntry.score < newHighscoreUnsorted.score)
            {
                //The new place of newScore should be i and all the rest should get i+1 place
                return Swap(highscores, numOfScores, newHighscoreUnsorted, i);
            }
        }
        if (numOfScores == 10)
        {
            lowestScore = highscores.highscoreEntryList[numOfScores - 1].score;
        }
        return highscores;
    }

    private static Highscores Swap(Highscores highscores, int numOfScores, HighscoreEntry newHighscoreUnsorted, int i)
    {
        for (int j = numOfScores; j > i; j--)
        {
            highscores.highscoreEntryList[j] = highscores.highscoreEntryList[j - 1];
        }
        highscores.highscoreEntryList[i] = newHighscoreUnsorted; //Insert the new score in his palce
        return highscores;
    }

    private void CleanHighscores()
    {
        Highscores highscores = GetHighScoreFromJson(); //Load highscores
        highscores.highscoreEntryList.Clear(); //CLEAR THE LIST
        SaveHighscoreToJson(highscores); //save highscore to json
    }

    private class Highscores
    {
        public List<HighscoreEntry> highscoreEntryList;
    }

    //Single high score entry
    [System.Serializable]
    private class HighscoreEntry
    {
        public int score;
        public string name;
        public string accuracy;
    }

}
//AddHighscoreEntry(900, "Gavriel");
//AddHighscoreEntry(1200, "New");
//AddHighscoreEntry(3100, "Dina");
//AddHighscoreEntry(40000, "AwsomePlayer");
//AddHighscoreEntry(55555, "Maya");
//AddHighscoreEntry(6900, "Gary");
//AddHighscoreEntry(7200, "Nir");
//AddHighscoreEntry(6100, "Dina");
//AddHighscoreEntry(9900, "Amit");
//AddHighscoreEntry(10, "LastOne");
//AddHighscoreEntry(33900, "ELEVEN");
//AddHighscoreEntry(1200, "NewLastOne");