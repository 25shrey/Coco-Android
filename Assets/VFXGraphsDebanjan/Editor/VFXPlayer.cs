using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.VFX;
using UnityEngine;
using UnityEngine.VFX;
using UnityEditor.Experimental.VFX;
using System.IO;

public class VFXPlayer : EditorWindow
{
    private string vfxEventName;
    private string mainFolderName;
    private string newVFXName;
    private string scriptName;
    private string instructionsText;
    private float delay;
    private bool isEventNameRequired;
    private bool isActionRequired;
    private bool showInstruction;
    private bool createNewVFX;
    private bool createScript;
    private GameObject selectedObject;
    private VisualEffect vfx;
    private VFXAction vfxAction;

    [MenuItem("Yudiz/VFX Player")]
    static void Init()
    {
        GetWindow(typeof(VFXPlayer));
    }


    private void OnFocus()
    {
        Selection.selectionChanged += FetchVisualEffectOnClick;    
    }
    private void OnLostFocus()
    {
        Selection.selectionChanged -= FetchVisualEffectOnClick;
    }


    private void FetchVisualEffectOnClick()
    {
        selectedObject = Selection.activeGameObject;
        if (selectedObject.TryGetComponent(out VisualEffect visualEffect))
        {
            vfx = visualEffect;
        }
        else
        {
            vfx = null;
            Debug.Log("<color=red>VFX Not Found!, Try Clicking on the Gameobject in Heirarcy which has a Visual Effect Component attached!</color>");
        }
    }

    private void OnGUI()
    {
        instructionsText =  "Here are the steps for using the VFX Player: \n" +
            "1. Click on the Gameobject where Visual Effect component is added! \n" +
            "2. Then Click on the Event name if required (Do this only if required) \n" +
            "3. Then Fill the event name if Event name required is true \n" +
            "4. Then Click on Action required if you need some sort of compeletion or any method that needs to be called. \n" +
            "5. If your are enabling Completion required then make sure to inherit VFXActon script to the scripts that you are using and then you override method OnVFXPlay and OnVFXComplete you want to call. \n";

        


        isEventNameRequired = EditorGUILayout.Toggle("Event Name Required", isEventNameRequired);
        GUILayout.Space(10f);

        if (isEventNameRequired)
        {
            vfxEventName = EditorGUILayout.TextField("Event Name", vfxEventName);
            GUILayout.Space(10f);
        }
        else
        {
            vfxEventName = string.Empty;
        }


        isActionRequired = EditorGUILayout.Toggle("Action Required", isActionRequired);
        GUILayout.Space(10f);

        if (isActionRequired)
        {
            delay = EditorGUILayout.Slider("VFX Completion Delay Time", delay, 0.1f, 10f);
            GUILayout.Space(10f);

            vfxAction = EditorGUILayout.ObjectField("VFX Action Script", vfxAction, typeof(VFXAction), true) as VFXAction;
            GUILayout.Space(10f);
        }
        else
        {
            delay = 0;
            vfxAction = null;
        }


        showInstruction = EditorGUILayout.Toggle("Show Instruction", showInstruction);
        GUILayout.Space(10f);
        
        createNewVFX = EditorGUILayout.Toggle("Create New VFX", createNewVFX);
        GUILayout.Space(10f);

        if (GUILayout.Button("Play VFX"))
        {
            PlayVFX(vfxEventName, vfxAction, vfx);
        }


        GUILayout.Space(10f);

        if (GUILayout.Button("Edit VFX"))
        {
            if (vfx != null)
            {
                VisualEffectAsset asset = vfx.visualEffectAsset;
                EditorGUIUtility.PingObject(asset);
            }
            else
            {
                Debug.Log("<color=red>VFX Not Found!, Try Clicking on the Gameobject in Heirarcy which has a Visual Effect Component attached!</color>");
            }
        }

        if (createNewVFX)
        {
            GUILayout.Space(10f);
            mainFolderName = EditorGUILayout.TextField("Main Folder Name", mainFolderName);
            GUILayout.Space(10f);
            newVFXName = EditorGUILayout.TextField("VFX Name", newVFXName);
            GUILayout.Space(10f);
            createScript = EditorGUILayout.Toggle("Create VFX Action Script", createScript);
            GUILayout.Space(10f);


            if (createScript)
            {
                scriptName = EditorGUILayout.TextField("Script Name", scriptName);
                GUILayout.Space(10f);
            }
            else
            {
                scriptName = string.Empty;
            }


            if (GUILayout.Button("Create New VFX"))
            {
                // Create Main Folder
                var assetPath = Utility.GetAssetPathDirectory();
                Debug.Log(assetPath);
                CreateFolder(assetPath, mainFolderName);

                // Create Folders inside the Main Folder Path
                var createOtherFolderPath = assetPath + "/" + mainFolderName;
                Debug.Log(createOtherFolderPath);
                CreateFolder(createOtherFolderPath, "Model");
                CreateFolder(createOtherFolderPath, "Prefab");
                CreateFolder(createOtherFolderPath, "Scripts");
                CreateFolder(createOtherFolderPath, "Effect");

                //Create Folder inside the Model Folder
                var modelPath = assetPath + "/" + mainFolderName + "/" + "Model";
                CreateFolder(modelPath, "Materials");

                //Create Visual Effect Asset
                string templatePath = @"D:\Debanjan Ghosh\VFXDemo\Library\PackageCache\com.unity.visualeffectgraph@14.0.4\Editor\Templates\SimpleParticleSystem.vfx";

                // Copy VFX Graph 
                var VFXPath = assetPath + "/" + mainFolderName + "/" + "Effect";
                Debug.Log(VFXPath);
                Debug.Log("Graph Path Validation : " + AssetDatabase.IsValidFolder(templatePath));         
                if (AssetDatabase.IsValidFolder(createOtherFolderPath))
                {
                    Debug.Log($"file exist = {File.Exists(templatePath)} and File path: {templatePath}");
                    //var asset = AssetDatabase.CopyAsset(templatePath, VFXPath);
                    var finalPath = VFXPath + "/" + newVFXName + ".vfx";
                    if (File.Exists(templatePath))
                    {
                        File.Copy(templatePath, finalPath);
                    }
                    else
                    {
                        Debug.Log("template file does not exist");
                    }
                    //Debug.Log(asset);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
                else
                {
                    Debug.Log("<color=red>Folder does not exist or path given is wrong please check and change the path.</color>");
                }

                if (createScript)
                {
                    var scriptPath = assetPath + "/" + mainFolderName + "/" + "Scripts" + "/" + $"{scriptName}.cs";

                    string template = @"
using UnityEngine;

public class #SCRIPTNAME# : VFXAction
{
    // Add your code here
}";

                    template = template.Replace("#SCRIPTNAME#", scriptName);

                    System.IO.File.WriteAllText(scriptPath, template);
                    AssetDatabase.Refresh();
                }
            }
        }
        else
        {
            mainFolderName = string.Empty;
            newVFXName = string.Empty;
        }


        if (showInstruction)
        {
            GUILayout.Space(25f);

            GUIStyle labelFieldStyles = new GUIStyle(GUI.skin.label);
            labelFieldStyles.alignment = TextAnchor.MiddleCenter;
            labelFieldStyles.normal.textColor = Color.green;
            labelFieldStyles.fontStyle= FontStyle.Bold;
            labelFieldStyles.fontSize = 22;
            EditorGUILayout.LabelField("Instructions", labelFieldStyles);
            GUILayout.Space(10f);

            GUIStyle selectableLabelStyle = new GUIStyle(EditorStyles.textArea);
            selectableLabelStyle.fontSize = 16;
            selectableLabelStyle.fontStyle = FontStyle.Normal;
            EditorGUILayout.SelectableLabel(instructionsText, selectableLabelStyle, GUILayout.Height(200f));
        }


        
    }
    private void PlayVFX(string eventName, VFXAction action, VisualEffect vfx)
    {
        if (vfx != null && isEventNameRequired && isActionRequired)
        {
            Debug.Log($"Event name: {eventName}");
            vfx.SendEvent(eventName);
            if (action != null)
            {
                action.OnVFXPlay(delay);
            }
            else
            {
                Debug.Log("<color=red>Script reference is Empty in the window please check and add the reference</color>");
            }
        }
        else if (vfx != null && !isEventNameRequired && isActionRequired)
        {
            vfx.Play();
            if (action != null)
            {
                action.OnVFXPlay(delay);
            }
            else
            {
                Debug.Log("<color=red>Script reference is Empty in the window please check and add the reference</color>");
            }
        }
        else if (vfx != null && isEventNameRequired && !isActionRequired)
        {
            vfx.SendEvent(eventName);
        }
        else if (vfx != null && !isEventNameRequired && !isActionRequired)
        {
            vfx.Play();
        }
        else if (vfx == null)
        {
            Debug.Log("<color=red>VFX Not Found!, Try Clicking on the Gameobject in Heirarcy which has a Visual Effect Component attached!</color>");
        }
    }

    private void CreateFolder(string parentPath, string folderName)
    {
        string finalPath = parentPath + "/" + folderName;    
        if (!AssetDatabase.IsValidFolder(finalPath))
        {
            string guid = AssetDatabase.CreateFolder(parentPath, folderName);
            string newFolderPath = AssetDatabase.GUIDToAssetPath(guid);
            Debug.Log($"new Folder path : {newFolderPath} guid : {guid}");
            Debug.Log("<color=green>Folder Create Please check Project Tab.</color>");
        }
        else
        {
            Debug.Log("<color=red>Unable to Create Folder Sorry please try another folder or enable Read/write in window settings.</color>");
        }
    }
}         