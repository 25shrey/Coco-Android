using UnityEngine;
using System.Collections;

public class FloatingObject : MonoBehaviour
{
    public float amplitude = 0.5f;
    public float speed = 1f;
    public float delayMin = 0.1f;
    public float delayMax = 0.5f;

    internal Vector3 startPos,endPos;
    internal float delay;

    public enum ObjectType
    {
        FloatingPlatform,
        HamgingBank,
        Health
    }

    public ObjectType type;

    public virtual void Start()
    {
        startPos = transform.position;
        //startPos = transform.position - new Vector3(0,amplitude,0);
        //endPos = transform.position + new Vector3(0,amplitude,0);
        //StartCoroutine(Move());
        delay = Random.Range(delayMin, delayMax);
    }

    public IEnumerator Move()
    {
        float time=amplitude/speed, totalTime=amplitude/speed;
        while (time>0)
        {
            time -= Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, endPos, 1 - time / totalTime); 
            yield return null;
        }
        transform.position = endPos;
        time = totalTime;
        while (time>0)
        {
            time -= Time.deltaTime;
            transform.position = Vector3.Lerp(endPos, startPos, 1 - time / totalTime); 
            yield return null;
        }
        transform.position = startPos;
        StartCoroutine(Move());
    }

    public virtual void FixedUpdate()
    {
        float time = Time.time - delay;
        float newY = startPos.y + amplitude * Mathf.Sin(speed * time);
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (LayerMask.LayerToName(collision.gameObject.layer).Equals("Coco"))
        {
            collision.transform.SetParent(transform, true);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (LayerMask.LayerToName(collision.gameObject.layer).Equals("Coco"))
        {
            collision.transform.SetParent(null);
            collision.transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);
        }
    }

    public void KillChildObject(GameObject obj)
    {
        StartCoroutine(KillChildObjectCoroutine(obj)); 
    }

    IEnumerator KillChildObjectCoroutine(GameObject obj)
    {
        yield return new WaitForSeconds(1);
        obj.SetActive(false);   
    }
}