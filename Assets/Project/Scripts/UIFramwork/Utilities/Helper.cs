using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Helper
{
    public static Coroutine Execute(this MonoBehaviour monoBehaviour, System.Action action, float time)
    {
        return monoBehaviour.StartCoroutine(DelayedAction(action, time));
    }
    static IEnumerator DelayedAction(System.Action action, float time)
    {
        yield return new WaitForSeconds(time);

        action();
    }

    /// <summary>
    /// Returns the color with the desired alpha level
    /// </summary>
    /// <returns>Alpha value</returns>
    /// <param name="color">Color whose alpha is to be changed.</param>
    /// <param name="alpha">Alpha value between 0 and 1.</param>
    public static Color WithAlpha(this Color color, float alpha)
    {
        return new Color(color.r, color.g, color.b, alpha);
    }
    public static float Map(float current1, float current2, float target1, float target2, float val)
    {
        //third parameter is the interpolant between the current range which, in turn, is used in the linear interpolation of the target range. 
        return Mathf.Lerp(target1, target2, Mathf.InverseLerp(current1, current2, val));
    }

    public static DateTime DateConvert(string _date)
    {
        string[] dateFormats = new[] { "yyyy/MM/dd", "MM/dd/yyyy", "dd/MM/yyyy", "MM/dd/yyyyHH:mm:ss", "dd-MM-yyyy", "MM-dd-yyyy", "yyyy-dd-MM", "yyyy-MM-dd" , "dd MMM yyyy" };
        System.Globalization.CultureInfo provider = new System.Globalization.CultureInfo("en-US");
        DateTime date = DateTime.ParseExact(_date, dateFormats, provider, System.Globalization.DateTimeStyles.AdjustToUniversal);
        Debug.LogError("Converted Date : " + date.ToString());
        return date;
    }
}
