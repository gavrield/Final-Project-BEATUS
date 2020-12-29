using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour
{
    public GameObject OptionsBtn;
    public GameObject MainButton;
    public GameObject screenToShow;
    public GameObject StartNewGameBtn;
    public GameObject HallOfFameBtn;
    public GameObject HowToPlayBtn;
    public GameObject CreditsBtn;
    public Slider DifficultySlider;
    public int songLength = 0;
    public AudioSource OpenSound;
    public AudioSource CloseSound;
    public Text SongSelected;
    private float difficulty = 1;
    private string song = "";

    bool show;
    void Start()
    {
        show = false;
        if (MainButton.name == "OptionsBtn")
        {
            try //Only if the screen if enable
            {
                DifficultySlider = GameObject.Find("DifficultySlider").GetComponent<Slider>();
                SongSelected = GameObject.Find("SongSelected").GetComponent<Text>();
                SetDifficulty();
                SetSong();
            }
            catch { }
        }
    }

    void Update()
    {
        screenToShow.SetActive(show);
        StartNewGameBtn.SetActive(!show);
        HallOfFameBtn.SetActive(!show);
        if (MainButton.name == "OptionsBtn")
        {
            if (difficulty != DifficultySlider.value)
            {
                SetDifficulty();
            }
            if (song != SongSelected.text)
            {
                SetSong();
            }

            CreditsBtn.SetActive(!show);
            HowToPlayBtn.SetActive(!show);
        }
        else if (MainButton.name == "CreditsBtn")
        {
            HowToPlayBtn.SetActive(!show);
            OptionsBtn.SetActive(!show);
        }
        else if (MainButton.name == "HowToPlayBtn")
        {
            CreditsBtn.SetActive(!show);
            OptionsBtn.SetActive(!show);
        }
    }

    private void SetDifficulty()
    {
        difficulty = DifficultySlider.value;
        PlayerPrefs.SetFloat("level", difficulty);
    }

    private void SetSong()
    {
        song = SongSelected.text;
        PlayerPrefs.SetString("song", song);
        if (song.ToString() == "TheFatRat - Unity")
        {
            songLength = 236;
        }
        else if (song.ToString() == "Mikey's AA")
        {
            songLength = 98;
        }
        else if (song.ToString() == "Guns-N-Roses-Sweet-Child-O-Mine")
        {
            songLength = 420;
        }

        PlayerPrefs.SetInt("songLength", songLength);
        PlayerPrefs.Save();
    }

    public void Click()
    {
        if (!show) //Open
        {
            OpenSound.Play();
        }
        else //Close
        {
            CloseSound.Play();
        }
        show = !show;
    }
}