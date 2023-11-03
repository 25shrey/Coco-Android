using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionEffect : MonoBehaviour
{
    private void OnEnable()
    {
        StartCoroutine(StartCoutDown());        
    }

    IEnumerator StartCoutDown()
    {
        yield return new WaitForSeconds(1);
        transform.gameObject.SetActive(false);  
    }
}
