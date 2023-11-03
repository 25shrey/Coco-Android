using System;
using System.Collections;
using UnityEngine.VFX;
using UnityEngine;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

public static class Utility 
{
    public static IEnumerator CheckForVFXCompletion(float delay, Action callback)
    {
        yield return new WaitForSeconds(delay);
        callback();
    }

   public static string GetAssetPathDirectory()
    {
        return "Assets/Project/VFXGraphs";
    }
}
