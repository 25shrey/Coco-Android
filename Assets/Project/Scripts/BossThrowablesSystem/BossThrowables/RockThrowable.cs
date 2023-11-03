


using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

public class RockThrowable : BossThrowables
{
    [Header("Rock Parameters")]
    [SerializeField] private Rigidbody rockRigidbody;
    [SerializeField] private float launchAngle;
    [SerializeField] private float slerpTime;
    [SerializeField] private float offSet;
    [SerializeField] private float forwardForce;
    public SpriteRenderer UIRing;
    

    public override void Throw()
    {
        base.Throw();
        Launch(player.playerOrigin.position);
    }

    public void SetUIRing(Vector3 pos)
    {
        UIRing.transform.position =pos;
        UIRing.transform.SetParent(null);
        UIRing.transform.localScale = Vector3.one * damageRange;
    }

    protected override void Explode()
    {
        if (Vector3.Distance(UIRing.transform.position, GameManager.instance.Player.transform.position) < damageRange/2 +offSet)
        {
            Vector3 distance = player.transform.position - transform.position;
            Vector3 direction = distance.normalized * force;
            player.Damage(direction, DamageAnimType.BossDamage, damage);
        }
        DestroyObject();
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if (isThrown)
            OnObjectHit(collision);
    }

    protected override void DestroyObject()
    {
        base.audio.Stop();
        base.audio.loop = false;
        base.audio.clip = SoundManager._soundManager.Bull[12];
        base.audio.Play();
        StartCoroutine((Destroy()));
    }

    IEnumerator Destroy()
    {
        transform.GetChild(1).transform.gameObject.SetActive(false);
        transform.GetChild(0).transform.gameObject.SetActive(false);
        yield return new WaitForSeconds(1f);
        
        base.DestroyObject();
        Destroy(UIRing.gameObject);
    }

    private void Launch(Vector3 targetPosition)
    {
        ThrowBall(targetPosition);
    }

    private async void ThrowBall(Vector3 targetPosition)
    {
        gameObject.SetActive(false);
        await Task.Delay((int)(Random.Range(0.5f, 2.01f) * 1000));
        gameObject.SetActive(true);
        rockRigidbody.AddForce(new Vector3(0,-1,0)* forwardForce);
        rockRigidbody.AddTorque(new Vector3(Random.Range(-1.5f,1.5f),Random.Range(-1.5f,1.5f),Random.Range(-1.5f,1.5f)).normalized* forwardForce);
        
        float startDif = Mathf.Abs(transform.position.y - UIRing.transform.position.y),startAlpha =  UIRing.color.a;
        Debug.Log("StartValue : "+startDif);
        while (UIRing != null && isThrown)
        {
            Debug.Log("Value : "+Mathf.Abs(transform.position.y - UIRing.transform.position.y));
            float alpha =  startAlpha+(0.8f-startAlpha)*(1 - (Mathf.Abs(transform.position.y - UIRing.transform.position.y)/startDif));
            if (alpha > 0.8f)
            {
                alpha = 0.8f;
            }
            UIRing.color = new Color(UIRing.color.r, UIRing.color.g, UIRing.color.b, alpha);
            await Task.Delay(25);
        }
        
        //Slerp Throw....
        // float time = slerpTime* Vector3.Distance(transform.position,targetPosition)/25;
        // float currentTime = 0f;
        // Vector3 diretion = Vector3.zero;
        // Vector3 LastPos = Vector3.zero;
        // while(true)
        // {
        //     currentTime += Time.deltaTime;
        //     Vector3 center = (spawnPoint.position + targetPosition) * 0.5f;
        //     center -= Vector3.up * launchAngle;
        //     Vector3 spawnPointCenter = spawnPoint.position - center;
        //     Vector3 playerPosCenter = targetPosition - center;
        //     LastPos = transform.position;
        //     float fracTime = currentTime / time;
        //     transform.position = Vector3.Slerp(spawnPointCenter, playerPosCenter, fracTime);
        //     transform.position += center;
        //     if (currentTime > time)
        //     {
        //         break;
        //     }
        //     yield return null;  
        // }
        // diretion = transform.position - LastPos;
        // if (diretion.y > -0.005f)
        // {
        //     diretion.y = -0.01f;
        // }
        // rockRigidbody.AddForce(diretion.normalized * forwardForce);
    }
}
