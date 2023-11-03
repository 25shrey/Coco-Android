using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;


public class UWPPostProcessBuild
{
    [PostProcessBuild]
    public static void OnPostProcessBuild(BuildTarget buildTarget, string path)
    {
        if (buildTarget != BuildTarget.WSAPlayer)
        {
            return;
        }
 
        string storeManifestPath = Path.Combine(path, Application.productName, "StoreManifest.xml");
        string manifest = File.ReadAllText(storeManifestPath);
        File.WriteAllText(storeManifestPath, manifest);
        Debug.Log("Repaired store manifest.");
    }
}
