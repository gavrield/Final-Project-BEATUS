using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System.Threading;
using System;

public class SpwanedObject : MonoBehaviour
{
    private float timer = 0;
    private bool alive = true;
    private float difficulty = 1;
    private Spawner spawner; 
    // Start is called before the first frame update
    void Start()
    {
        spawner = FindObjectOfType<Spawner>(); //its share between saber and cubes 
        difficulty = spawner.GetDifficulty();
    }

    // Update is called once per frame
    void Update()
    {
        if ((transform.position.z) < 1.93 && alive) //if item passed user
        {
            if (tag == "Cube")
            {
                spawner.missed();
            }
            Destroy(gameObject);
            alive = false;
        }
        else
        {
            timer += Time.deltaTime;
            transform.position += Time.deltaTime * transform.forward * 2 * difficulty;
        }
    }
}
