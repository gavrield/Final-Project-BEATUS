using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScences : MonoBehaviour
{
    public int index;
    public AudioSource ClickSound;

    public void Click()
    {
        //ClickSound.Play();
        SceneManager.LoadScene(index);
    }
}
