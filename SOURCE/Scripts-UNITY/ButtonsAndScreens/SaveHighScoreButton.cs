using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveHighScoreButton : MonoBehaviour
{
    public AudioSource SaveSound;
    public AudioSource WrongAction;
    private Text name1;
    private Text score;
    private Text accuracy;
    private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        name1 = GameObject.Find("NameLabel").GetComponent<Text>();
        score = GameObject.Find("Score").GetComponent<Text>();
        accuracy = GameObject.Find("AccuracyRate").GetComponent<Text>();
    }

    public void Click()
    {
        if (!string.IsNullOrEmpty(name1.text.ToString())) //If name not null
        {
            gameManager.AddNewHighScore(int.Parse(score.text), name1.text, accuracy.text); //Add to HighScore table
            SaveSound.Play();
            gameObject.GetComponent<Button>().interactable = false; //Disable add more than one score
        }
        else // If name is null
        {
            WrongAction.Play();
        }
    }
}


