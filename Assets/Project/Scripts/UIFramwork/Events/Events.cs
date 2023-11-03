using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Events : MonoBehaviour
{

    public static event Action<string,float> OnDownlodProgress;
    public static event Action OnDownlodFailed;

    public static event Action<string,bool> OnNearestSculpture;

    public static event Action OnLanguageChanged;
    public static event Action<bool> OnScreenChanged;

    

    public static void DownloadProgress(string name,float progress)
    {
        if (OnDownlodProgress != null)
            OnDownlodProgress(name, progress);
        //OnDownlodProgress?.Invoke(name, progress);
    }

    public static void DownloadFailed()
    {
        if (OnDownlodFailed != null)
            OnDownlodFailed();
    }

    public static void NearestSculpture(string sculpId,bool isNearest)
    {
        if (OnNearestSculpture != null)
            OnNearestSculpture(sculpId,isNearest);
    }

    public static void LanguageChanged()
    {
        if (OnLanguageChanged != null)
            OnLanguageChanged();
    }

    public static void ScreenChanged(bool val)
    {
        OnScreenChanged?.Invoke(val);
    }
}
