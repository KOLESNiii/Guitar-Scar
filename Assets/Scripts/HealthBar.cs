using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.XR;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    public RectTransform bar;
    public bool isPlayer;
    void Start()
    {
        bar = GetComponent<RectTransform>();
    }

    public void SetValue(float value, float maxValue)
    {
        if (isPlayer)
        {
            bar.offsetMin = new Vector2(420*(maxValue - value)/maxValue , bar.offsetMin.y);
        }
        else
        {
            bar.offsetMax = new Vector2(-420*(maxValue - value)/maxValue, bar.offsetMax.y);
        }
    }
}
