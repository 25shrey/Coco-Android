using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ObjectsPlacementHandler))]
public class ObjectPlacementEditor : Editor
{
    private ObjectsPlacementHandler objectsHandler;

    private Vector3 center;

    private void OnEnable()
    {
        objectsHandler = target as ObjectsPlacementHandler;

        Tools.hidden = true;
    }

    private void OnDisable()
    {
        Tools.hidden = false;
    }

    private void OnSceneGUI()
    {
        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            center = hit.point;

            SceneView.RepaintAll();
        }


        Handles.color = Color.white;

        Handles.DrawWireDisc(center, Vector3.up, objectsHandler.radius);

        HandleUtility.AddDefaultControl(0);

        if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
        {
            if (objectsHandler.action == ObjectsPlacementHandler.Actions.AddObjects)
            {
                AddNewPrefabs();

                MarkSceneAsDirty();
            }
            else if (objectsHandler.action == ObjectsPlacementHandler.Actions.RemoveObjects)
            {
                objectsHandler.RemoveObjects(center);

                MarkSceneAsDirty();
            }
        }
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        string[] options = new string[objectsHandler.prefabs.Count];
        for (int i = 0; i < options.Length; i++)
        {
            options[i] = objectsHandler.prefabs[i].name;
        }
        objectsHandler.selectedPrefabIndex = EditorGUILayout.Popup("Label", objectsHandler.selectedPrefabIndex, options);
        
        if (GUILayout.Button("Remove all objects"))
        {
            if (EditorUtility.DisplayDialog("Safety check!", "Do you want to remove all objects?", "Yes", "No"))
            {
                objectsHandler.RemoveAllObjects();

                MarkSceneAsDirty();
            }
        }
    }

    private void MarkSceneAsDirty()
    {
        UnityEngine.SceneManagement.Scene activeScene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();

        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(activeScene);
    }

    private void AddNewPrefabs()
    {
        int howManyObjects = objectsHandler.howManyObjects;

        GameObject prefabGO = objectsHandler.prefabs[objectsHandler.selectedPrefabIndex];

        for (int i = 0; i < howManyObjects; i++)
        {
            GameObject newGO = PrefabUtility.InstantiatePrefab(prefabGO) as GameObject;

            objectsHandler.AddPrefab(newGO, center);
        }
    }


    /// <summary>
    /// Displays a vertical list of toggles and returns the index of the selected item.
    /// </summary>
    public static int ToggleList(int selected, GUIContent[] items)
    {
        // Keep the selected index within the bounds of the items array
        selected = selected < 0 ? 0 : selected >= items.Length ? items.Length - 1 : selected;

        GUILayout.BeginVertical();
        for (int i = 0; i < items.Length; i++)
        {
            // Display toggle. Get if toggle changed.
            bool change = GUILayout.Toggle(selected == i, items[i]);
            // If changed, set selected to current index.
            if (change)
                selected = i;
        }
        GUILayout.EndVertical();

        // Return the currently selected item's index
        return selected;

    }
}