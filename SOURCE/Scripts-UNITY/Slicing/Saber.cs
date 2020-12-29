using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static OVRInput;

public class Saber : MonoBehaviour
{
    public LayerMask goodLayer;
    public LayerMask wrongLayer;
    public LayerMask distractLayer;
    public LayerMask boostLayer;
    public LayerMask afekaLayer;
    private Vector3 previousPos;
    private GameManager gameManager;
    //// Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 1, goodLayer))
        {
            if (Vector3.Angle(transform.position - previousPos, hit.transform.up) > 130)
            {
                //HIT in the right direction
                hit.transform.gameObject.layer = 0;
                gameManager.Hit(true);
            }
            // hit with the wrong direction
            else
            {
                hit.transform.gameObject.layer = 0;
                gameManager.Hit(false);
            }
        }
        else if (Physics.Raycast(transform.position, transform.forward, out hit, 1, wrongLayer))
        {
            hit.transform.gameObject.layer = 0;
            //Wrong cube hit
            gameManager.Missed(false);
        }
        else if (Physics.Raycast(transform.position, transform.forward, out hit, 1, distractLayer))
        {
            //Distract hit
            Destroy(hit.transform.gameObject); //Destroy the cube when hiting
            gameManager.Missed(true);
        }
        else if (Physics.Raycast(transform.position, transform.forward, out hit, 1, boostLayer))
        {
            //Hit boost
            GameObject sd = hit.transform.gameObject;         
            string tag = sd.tag.ToString();
            gameManager.HitBoost(tag, sd);
            Destroy(sd); //Destroy the cube when hiting
        }
        else if (Physics.Raycast(transform.position, transform.forward, out hit, 1, afekaLayer))
        {
            Destroy(hit.transform.gameObject); //Destroy the cube when hiting
            gameManager.HitAfekaCircle();
        }
        previousPos = transform.position;
    }

}