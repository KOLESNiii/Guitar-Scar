using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Base class for the extra portion of the health or armour bar
public class ExtraBar : MonoBehaviour
{
    public RectTransform bar;

    void Start()
    {
        bar = GetComponent<RectTransform>();
    }

    //Sets the value of the extra bar
    public void SetValue(float value, float maxValue, float tempMaxValue)
    {
        if (maxValue == tempMaxValue) //If the max value is the same as the temp max value, that means the extra bar is not needed
        {
            bar.offsetMin = new Vector2(0, bar.offsetMin.y);
        }
        else
        {
            if (value > maxValue) //If the value is greater than the max value, the extra bar is needed
            {
                bar.offsetMin = new Vector2(-100 * (value - maxValue) / (tempMaxValue - maxValue), bar.offsetMin.y);
            }
            else //If there is extra but the value is not greater than the max value, the extra bar is not needed
            {
                bar.offsetMin = new Vector2(0, bar.offsetMin.y);
            }
        }
    }
}
