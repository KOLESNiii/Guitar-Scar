using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraBar : MonoBehaviour
{
    public RectTransform bar;

    void Start()
    {
        bar = GetComponent<RectTransform>();
    }

    public void SetValue(float value, float maxValue, float tempMaxValue)
    {
        if (maxValue == tempMaxValue)
        {
            bar.offsetMin = new Vector2(0, bar.offsetMin.y);
        }
        else
        {
            if (value > maxValue)
            {
                bar.offsetMin = new Vector2(-100 * (value - maxValue) / (tempMaxValue - maxValue), bar.offsetMin.y);
            }
            else
            {
                bar.offsetMin = new Vector2(0, bar.offsetMin.y);
            }
        }
    }
}
