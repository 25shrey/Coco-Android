using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BankBase : MonoBehaviour, IBankSystem
{
    public int MaxCoins;
    public int current;

    private GameObject baseObj;
    private Vector3 end;

    public void TotalCoins(GameObject obj)
    {
        current--;
        PlayerStats.Instance.obj.Coins++;
        if(current < 1)
        {
            current = 0;
            baseObj = obj;
            end = new Vector3(baseObj.transform.position.x + (Random.Range(-40, 40)), baseObj.transform.position.y + 60f, baseObj.transform.position.z + (Random.Range(-40, 40)));
            StartCoroutine(DisableObject(obj));
        }
    }

    public void Update()
    {
        if(!System.Object.ReferenceEquals(baseObj,null))
        {
            baseObj.transform.position = Vector3.Lerp(baseObj.transform.position, end, 0.6f * Time.deltaTime);
        }
    }

    IEnumerator DisableObject(GameObject obj)
    {
        yield return new WaitForSeconds(0.5f);

        var objchild = obj.transform.parent.GetChild(1);
        DOTween.Kill(objchild.GetChild(0).transform);

        baseObj = null;
        obj.transform.parent.GetComponent<FloatingObject>().KillChildObject(obj);
        objchild.gameObject.SetActive(false);    
    }
}
