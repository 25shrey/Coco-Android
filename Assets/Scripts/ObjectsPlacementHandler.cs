using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectsPlacementHandler : MonoBehaviour
{
    public List<GameObject> prefabs;
    public GameObject prefabGO;
    public float radius = 5f;
    public int howManyObjects = 5;

    [HideInInspector]
    public int selectedPrefabIndex=0;

    public enum Actions { AddObjects, RemoveObjects }
    public Actions action;

    public void Start()
    {
        
    }

    public void AddPrefab(GameObject newPrefabObj, Vector3 center)
    {
        Vector2 randomPos2D = Random.insideUnitCircle * radius;

        Vector3 randomPos = new Vector3(randomPos2D.x, 0f, randomPos2D.y) + center;

        newPrefabObj.transform.position = randomPos;

        newPrefabObj.transform.parent = transform;
    }

    public void RemoveObjects(Vector3 center)
    {
        GameObject[] allChildren = GetAllChildren();

        foreach (GameObject child in allChildren)
        {
            if (Vector3.SqrMagnitude(child.transform.position - center) < radius * radius)
            {
                DestroyImmediate(child);
            }
        }
    }

    public void RemoveAllObjects()
    {
        GameObject[] allChildren = GetAllChildren();

        foreach (GameObject child in allChildren)
        {
            DestroyImmediate(child);
        }
    }

    private GameObject[] GetAllChildren()
    {
        GameObject[] allChildren = new GameObject[transform.childCount];

        int childCount = 0;
        foreach (Transform child in transform)
        {
            allChildren[childCount] = child.gameObject;
            childCount += 1;
        }

        return allChildren;
    }

}