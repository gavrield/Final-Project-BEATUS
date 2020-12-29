using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

using Random = UnityEngine.Random;

public class Spawner : MonoBehaviour
{
    public GameObject[] elements;
    public Transform[] points;
    private GameManager gameManager;
    public string[] bpm = null;
    private bool firstRun = true;
    private bool checkLevel = false;
    private float beat; //2*60 /bpm
    private float timer = 0;
    private int specialBoosts = 0;
    private float difficulty = 1;
    private int factor = 1;
    private bool spawn = false;
    private double timeElpased = 0;
    private float musicLength = 0;
    private int k = -1;
    private bool suspendSpawnBool = false;
    private bool spawnStarted = false;
    private float objectLifeTime = 10.2f; // how manny time to come from starting position to player
                                          //speed = deltaTime*2-> (deltaTime*2)*(1/deltaTime) -> 2m/s
                                          //x=x0 + (v0+v)*t/2 -> distanceFromPlayer = (2*t)/2 -> t= distanceFromPlayer sec
                                          //The time in facto in 7.6sec for 16.2m of distance
                                          // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>(); //its share between saber and elements         
        gameManager.SwitchMusic(true); //play music
    }

    // Update is called once per frame
    void Update()
    {
        if (spawn == true)
        {
            Spawning();
            if ((int)timeElpased % 25 == 0 && checkLevel == true && !gameManager.GetSpecialBoostLocked()) //Check the level every 25 sec if not in check and no in special mode
            {
                checkLevel = false;
                if (gameManager.CheckLevel())
                {
                    //Game manager changed the level
                    LevelHasBeenChanged();
                }
            }
        }
        else
        {
            NoSpawning();
        }
        timeElpased += Time.deltaTime;
        gameManager.updateTime((int)timeElpased);
    }

    private void LevelHasBeenChanged()
    {
        if (difficulty < GetDifficulty())
        {
            //Level Raised
            StartCoroutine(SuspendSpawn());
            gameManager.SwitchChangeLevelLabel(true, true);
        }
        else
        {
            //Level reduced
            gameManager.SwitchChangeLevelLabel(true, false);
        }
        DefineK();
        difficulty = GetDifficulty();
        StartCoroutine(DisableCheckLevel());
    }

    private void Spawning()
    {
        if (!IsGameOver())
        {
            SpawnObjects();
        }
        else //Game Over
        {
            StopSpawn();
        }
    }
    private void CalcBeat(int i)
    {
        //level 1 takes 6 secs from spawn to user, rest takes 3 secs
        float bpm1 = float.Parse(bpm[i + factor]);
        beat = 2 * 60 / bpm1;
    }
    private void NoSpawning()
    {
        if (firstRun)
        {
            firstRun = false;
            difficulty = GetDifficulty();
            musicLength = gameManager.GetMusicLength();
            bpm = File.ReadLines(Application.dataPath + "/Resources/Music/BpmVectors/" + gameManager.GetSongName() + "_bpm.txt").ToArray();
            StartCoroutine(ChangeBpm());
            DefineK();
            StartCoroutine(DelayStartSpawn());
            StartCoroutine(DisableGoodLuck());
        }
    }

    private void StopSpawn()
    {
        spawn = false;
        gameManager.SwitchMusic(false); //Stop music
        KillAllObjects();
    }

    private void KillAllObjects()
    {
        foreach (Transform element in points)
        {
            element.gameObject.SetActive(false);//destroy all spawns
        }
    }

    private void SpawnObjects()
    {
        if (!suspendSpawnBool)
        {
            timer += Time.deltaTime;
            if (timer >= beat)
            {
                if (gameManager.GetIsAfekaBoost()) // if special boost afeka is on
                {
                    if (gameManager.GetObjectTimeToArrive() <= gameManager.GetTimerLeft())
                    {
                        SpawnWhites();
                    }
                }
                else if (difficulty > 1)
                {
                    DoubleInstantiateCube();
                }
                else
                {
                    InstantiateCube();
                }
                timer -= beat;
            }
        }
    }

    private void SpawnWhites()
    {
        //points[6]-points[19] =Row
        //points[10]-points[13] =XRight
        //points[14]-points[17] =XLeft
        //Random between 6,10,14
        int[] OptionsForRandom = { 6, 10, 14 };
        int startPoint = OptionsForRandom[Random.Range(0, OptionsForRandom.Length)];
        InitCircles(startPoint);

    }
    private void InitCircles(int i)
    {
        GameObject cube1 = Instantiate(elements[7], points[i]);
        GameObject cube2 = Instantiate(elements[7], points[i + 1]);
        GameObject cube3 = Instantiate(elements[7], points[i + 2]);
        GameObject cube4 = Instantiate(elements[7], points[i + 3]);
        cube1.transform.localPosition = Vector3.zero;
        cube2.transform.localPosition = Vector3.zero;
        cube3.transform.localPosition = Vector3.zero;
        cube4.transform.localPosition = Vector3.zero;
    }
    private bool IsGameOver()
    {
        return gameManager.GetGameOver();
    }

    private void DefineK()
    {
        if (difficulty == 1)
        {
            k = 0;
            transform.position = new Vector3(0, 4.47f, 13.8f);
            factor = 2;
        }
        else
        {
            factor = 1;
            if (difficulty == 2)
            {
                transform.position = new Vector3(0, 4.47f, 13.8f);
            }
            else
            {
                transform.position = new Vector3(0, 4.47f, 20f);
            }
            k = (int)difficulty + 3;
        }
    }

    private void SetSpawn(bool isSpawn)
    {
        spawn = isSpawn;
    }

    private void InstantiateCube()
    {
        GameObject cube;
        int number = (Random.Range(1, 11));
        if (difficulty == 3 && number == 1) //10% distraction (if level == Hard)
        {
            InstantiateDistruction();
        }
        else if (number <= 9) //  90% or 80% single cube
        {
            cube = Instantiate(elements[Random.Range(0, 2)], points[Random.Range(0, 6)]);
            cube.transform.localPosition = Vector3.zero;
            cube.transform.Rotate(transform.forward, 90 * Random.Range(0, 4));
        }
        else // 10% boosts
        {
            // 12.5% special booost -> X2Boost or AfekaBoost
            if (Random.Range(1, 9) == 1 && specialBoosts < 2 && !gameManager.GetSpecialBoostLocked())
            {
                specialBoosts += 1;
                cube = Instantiate(elements[Random.Range(4, 6)], points[Random.Range(0, 6)]);
                cube.transform.localPosition = Vector3.zero;
            }
            // 87.5% Bonus boost
            else
            {
                cube = Instantiate(elements[6], points[Random.Range(0, 6)]);
                cube.transform.localPosition = Vector3.zero;
            }
        }
    }

    private void DoubleInstantiateCube()
    {
        if (Random.Range(1, 9) <= difficulty) //HARD = 33% double, MEDIUM = 22%
        {
            InstantiateRandomTwoCubes(Random.Range(1, 3));
        }
        else
        {
            InstantiateCube();
        }
    }

    private void InstantiateRandomTwoCubes(int doubleOption)
    {
        switch (doubleOption)
        {
            //Both Cubes in middle of screen
            case 1:
                //One in position 0 and other in 1 - randomly
                DoulbeInCenter();
                break;
            //Cubes together in one of the sides 2,3 or 4,5
            //Cubes in diffrent sides 2,4 or 3,5
            case 2:
                DoubleInSides();
                break;
        }
    }

    private void DoubleInSides()
    {
        int secPos = 0;
        int[] rotateOptions1 = { 1, 3 };
        int[] OptionsFor2N5 = { 2, 5 };
        int firstPos = Random.Range(2, 6);
        if (3 > firstPos || firstPos > 4)//if 2 or 5 ===> secPos = 3 or 4
        {
            secPos = Random.Range(3, 5);
        }
        else // if 3 or 4 ===> secPos = 2 or 5
        {
            secPos = OptionsFor2N5[Random.Range(0, OptionsFor2N5.Length)];
        }
        //Arrow only for sides
        int rotate1 = rotateOptions1[Random.Range(0, rotateOptions1.Length)];
        int rotate2 = rotateOptions1[Random.Range(0, rotateOptions1.Length)];
        CreateRandomTwoCubes(firstPos, secPos, rotate1, rotate2);
    }

    private void DoulbeInCenter()
    {
        int firstPos = Random.Range(0, 2);
        int secPos = (firstPos * -1) + 1;
        int[] rotateOptions = { 2, 4 };

        //Arrow cannot be inside
        int rotate1 = rotateOptions[Random.Range(0, rotateOptions.Length)];
        int rotate2 = rotateOptions[Random.Range(0, rotateOptions.Length)];
        CreateRandomTwoCubes(firstPos, secPos, rotate1, rotate2);
    }

    private void CreateRandomTwoCubes(int firstPos, int secPos, int rotate1, int rotate2)
    {
        GameObject cube1 = Instantiate(elements[0], points[firstPos]);
        GameObject cube2 = Instantiate(elements[1], points[secPos]);
        initCubeTransform(rotate1, cube1);
        initCubeTransform(rotate2, cube2);
    }

    private void InstantiateDistruction()
    {
        GameObject cube = Instantiate(elements[Random.Range(2, 4)], points[Random.Range(0, 2)]);
        initCubeTransform(Random.Range(0, 4), cube);
    }
    private void initCubeTransform(int rotate, GameObject cube)
    {
        cube.transform.Rotate(transform.forward, 90 * rotate);
        cube.transform.localPosition = Vector3.zero;
    }

    public float GetDifficulty()
    {
        return gameManager.GetDifficulty();
    }
    public void missed()
    {
        gameManager.Missed(false); //Missed object:
    }
    //Working in background
    IEnumerator ChangeBpm()
    {
        float t = 0;
        while (t < musicLength)
        {
            float t1 = 0;
            CalcBeat((int)timeElpased / 3);
            while (t1 <= 3)
            {
                t1 += Time.deltaTime;
                t += t1;
                yield return null;
            }
        }
    }
    IEnumerator DelayStartSpawn()
    {
        float t = 0;
        while (t <= k)
        {
            t += Time.deltaTime;
            yield return null;
        }
        spawn = true;
        spawnStarted = true;
        StartCoroutine(CheckFinish());
    }
    IEnumerator CheckFinish()
    {
        float t = 0;
        while (t <= musicLength)
        {

            if (((objectLifeTime + timeElpased) >= musicLength) && spawn == true)//stop to create new objects
            {
                spawn = false;
                timeElpased = 0; //zero timer for counting time until last cube pass and show finishScreen
            }
            else if (IsGameOver() || spawn == false && spawnStarted) //show finishedScreen after last object gone
                                                                     // && gameManager.getFinishScreenActivation() == false
            {
                gameManager.ActivateFinishScreen();
                gameObject.active = false;
                t = musicLength;

                //Have to notice how manny elements spawned for calculate the miss for score screen
            }
            t += Time.deltaTime;
            yield return null;
        }
    }
    IEnumerator DisableGoodLuck()
    {
        float t = 0;
        while (t <= 23)
        {
            t += Time.deltaTime;
            yield return null;
        }
        checkLevel = true;
        gameManager.SwitchGoodluckLabel(false);
    }

    IEnumerator DisableCheckLevel()
    {
        float t = 0;
        while (t <= 1)
        {
            t += Time.deltaTime;
            yield return null;
        }
        checkLevel = true;
    }
    IEnumerator SuspendSpawn()
    {
        suspendSpawnBool = true;
        float t = 0;
        float time = 5.5f - difficulty;
        while (t <= time)
        {
            t += Time.deltaTime;
            yield return null;
        }
        suspendSpawnBool = false;
    }
}