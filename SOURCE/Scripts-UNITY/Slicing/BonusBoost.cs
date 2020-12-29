using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class BonusBoost : MonoBehaviour
{
    public TextMeshPro BoostAmount;
    public TextMeshPro BoostSign;

    public string getBoostSign()
    {
        return BoostSign.text.ToString();
    }
    public string getBoostAmount()
    {
        return BoostAmount.text.ToString();
    }

    // Start is called before the first frame update
    void Awake()
    {
        defineTheBoost();
    }

    private void defineTheBoost()
    {
        int points = Random.Range(200, 900);
        BoostAmount.SetText(points.ToString());
        // Detdermine Sign
        if (Random.Range(1, 4) > 1) //33% bad boost,66% good
        {
            BoostSign.SetText("+");
        }
        else
        {
            BoostSign.SetText("-");
        }
    }
}
