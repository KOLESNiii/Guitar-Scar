using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

//Class for handling the health and armour bars in battle scene
public class HealthBar : MonoBehaviour
{
    public RectTransform bar;
    public bool isPlayer; //Whether the bar is for the player or the enemy
    //Initialises reference to own size
    void Start()
    {
        bar = GetComponent<RectTransform>();
    }

    public void SetValue(float value, float maxValue)
    {
        if (isPlayer) //different position for player and enemy bars
        {
            bar.offsetMin = new Vector2(420*(maxValue - value)/maxValue , bar.offsetMin.y);
        }
        else
        {
            bar.offsetMax = new Vector2(-420*(maxValue - value)/maxValue, bar.offsetMax.y);
        }
    }
}
