using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Breakableitem : MonoBehaviour
{
    public Rigidbody Rigidbody;

    [SerializeField]
    private GameObject BrokenObjects;

    [SerializeField]
    private float ExplosiveForce = 1000;
    [SerializeField]
    private float ExplosiveRadius = 2;

    public enum BrekableType
    {
        Air,
        Ground
    }

    public BrekableType type;

    public GameObject item;
    public Transform Collectable;
    public int numberofItems;
    public int radius;

    public VisualEffect breakVfx;

    [SerializeField] bool hit;

    public enum Itemtype
    {
        coin,
        magnet,
        shield,
        life,
        fruit,
        none
    }

    public Itemtype itemtype;

    private void Update()
    {
        if (hit)
        {
            Explode(Vector3.one);
            hit = false;
        }
    }

    public void Explode(Vector3 pos)
    {
        //GameManager.instance.Player._sounds.SoundToBeUsed(2, SoundManager.Soundtype.other, 0.3f);

        SoundManager._soundManager._otherSounds.SoundToBeUsed(2, SoundManager.Soundtype.other, false, true);

        Destroy(Rigidbody);
        Rigidbody.gameObject.SetActive(false);

        BrokenObjects.SetActive(true);

        Rigidbody[] rigidbodies = BrokenObjects.GetComponentsInChildren<Rigidbody>();
        if (type == BrekableType.Air)
        {
            foreach (Rigidbody body in rigidbodies)
            {
                body.AddExplosionForce(ExplosiveForce, pos, ExplosiveRadius);
            }
        }
        else
        {
            ExplosiveForce = 600;
            ExplosiveRadius = 1.6f;
            foreach (Rigidbody body in rigidbodies)
            {
                body.AddExplosionForce(ExplosiveForce, transform.position, ExplosiveRadius);
            }
        }
        breakVfx.Play();
        if (itemtype != Itemtype.none)
        {
            GenerateItems();
        }

        StartCoroutine(FadeOutRigidBodies(rigidbodies));
    }

    IEnumerator FadeOutRigidBodies(Rigidbody[] rb)
    {
        yield return new WaitForSeconds(0.4f);

        breakVfx.Stop();

        yield return new WaitForSeconds(0.4f);

        foreach(var r in rb)
        {
            Destroy(r.gameObject);
        }
    }

    void GenerateItems()
    {
        List<GameObject> objList = new List<GameObject>();

        for (int i = 0; i < numberofItems; i++)
        {
            var obj = Instantiate(item, transform.position, item.transform.rotation);
            obj.transform.SetParent(Collectable);

            objList.Add(obj);
        }

        if (numberofItems == 1)
        {
            var angle = 3 * Mathf.PI * 2 / 6;

            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;

            objList[0].GetComponent<ItemMover>().target = transform.position + new Vector3(x, 0, z);
            objList[0].GetComponent<ItemMover>().speed = radius * 2;

            return;
        }

        for (int i = 0; i < numberofItems; i++)
        {
            var angle = i * Mathf.PI * 2 / numberofItems;

            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;

            //objList[i].transform.position = transform.position + new Vector3(x, 0, z);

            objList[i].GetComponent<ItemMover>().target = transform.position + new Vector3(x, 0, z);
            objList[i].GetComponent<ItemMover>().speed = radius * 2;
        }
    }

    //private IEnumerator FadeOutRigidBodies(Rigidbody[] Rigidbodies)
    //{
    //    WaitForSeconds Wait = new WaitForSeconds(PieceSleepCheckDelay);
    //    float activeRigidbodies = (Rigidbodies.Length / 5);

    //    while (activeRigidbodies > 0)
    //    {
    //        yield return Wait;

    //        foreach (Rigidbody rigidbody in Rigidbodies)
    //        {
    //            if (rigidbody.IsSleeping())
    //            {
    //                activeRigidbodies--;
    //            }
    //        }
    //    }


    //    yield return new WaitForSeconds(PieceDestroyDelay);

    //    float time = 0;
    //    Renderer[] renderers = Array.ConvertAll(Rigidbodies, GetRendererFromRigidbody);

    //    foreach (Rigidbody body in Rigidbodies)
    //    {
    //        Destroy(body.GetComponent<BoxCollider>());
    //        Destroy(body);
    //    }

    //    while (time < 1)
    //    {
    //        float step = Time.deltaTime * PieceFadeSpeed;
    //        foreach (Renderer renderer in renderers)
    //        {
    //            renderer.transform.Translate(Vector3.down * (step / renderer.bounds.size.y), Space.World);
    //        }

    //        time += step;
    //        yield return null;
    //    }

    //    foreach (Renderer renderer in renderers)
    //    {
    //        Destroy(renderer.gameObject);
    //    }
    //    Destroy(gameObject);
    //}

    //private Renderer GetRendererFromRigidbody(Rigidbody Rigidbody)
    //{
    //    return Rigidbody.GetComponent<Renderer>();
    //}
}
