using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ColourBlindManager : MonoBehaviour
{
    [SerializeField]
    private GameObject[] text;

    void Start()
    {
        if (Global.colourblindMode == 1)
        {
            foreach (GameObject t in text)
            {
                t.GetComponent<TextMeshProUGUI>().color = new Color(0, 250, 255);
            }
        }
        else if (Global.colourblindMode == 2)
        {
            foreach (GameObject t in text)
            {
                t.GetComponent<TextMeshProUGUI>().color = new Color(255, 0, 0);
            }
        }
    }
}
