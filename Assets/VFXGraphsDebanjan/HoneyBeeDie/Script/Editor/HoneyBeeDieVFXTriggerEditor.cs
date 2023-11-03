/*using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(HoneybeeDieVFXTrigger))]
public class HoneyBeeDieVFXTriggerEditor : Editor
{
    HoneybeeDieVFXTrigger honeybeeScript;

    public override void OnInspectorGUI()
    {
        honeybeeScript = (HoneybeeDieVFXTrigger)target;
        base.OnInspectorGUI();
        GUILayout.Space(10f);
        var center = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold };
        GUILayout.Label("Honeybee Die Trigger Button", center, GUILayout.ExpandWidth(true));
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("TriggerHoneybeeDie"))
        {
            honeybeeScript.OnHoneybeeDie(); 
        }
        GUILayout.EndHorizontal();
    }
}*/
