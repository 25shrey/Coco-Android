using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;

public class BossThrowables : MonoBehaviour
{
    [Header("Base Parameters")]
    [SerializeField] protected float damage;
    [SerializeField] protected float force;
    [SerializeField] protected float destoryTime = 5f;
    [SerializeField] public float damageRange;
    [SerializeField] private LayerMask layerMask;
    protected bool isThrown = false;
    protected Transform spawnPoint;
    protected Player player;

    [Header("VFX")]
    [SerializeField] protected VFXPlayer explodeVFXPrefab;
    [SerializeField] protected VisualEffect smokeTrail;

    [SerializeField] internal AudioSource audio;

    protected virtual void OnEnable() => Init();


    protected virtual void Init()
    {
        player = GameManager.instance.Player;
        smokeTrail.Play();
        audio.Play();
        //Invoke(destroyObjectString, destoryTime);
    }

    public virtual void SetData(Transform spawnPosition)
    {
        spawnPoint = spawnPosition;
    }

    public virtual void Throw()
    {
        isThrown = true; 
    }

    protected virtual void OnObjectHit(Collision collision)
    {
        if (((1 << collision.gameObject.layer) & layerMask) != 0)
        {
            Explode();
            if (explodeVFXPrefab != null)
            {
                VFXPlayer vfx = Instantiate(explodeVFXPrefab, transform.position - 
                                                              
                                                              new Vector3(0,1.2f,0), Quaternion.identity);
                vfx.PlayVFX();
            }
            else
            {
                Debug.LogWarning("VFX Missing!");
            }
        }

        if (collision.gameObject.TryGetComponent<Player>(out Player player))
        {
            Vector3 distance = player.transform.position - transform.position;
            Vector3 direction = distance.normalized * force;
            player.Damage(direction*1.5f, DamageAnimType.Damage, damage);
            DestroyObject();
        }
    }


    protected virtual void Explode()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, damageRange);
        foreach(Collider collider in colliders)
        {
            if (collider.TryGetComponent<Player>(out Player player) && collider.gameObject != gameObject)
            {
                Vector3 distance = player.transform.position - transform.position;
                Vector3 direction = distance.normalized * force;
                player.Damage(direction*1.5f, DamageAnimType.Damage, damage);
            }
        }
        DestroyObject();
    }

    protected virtual void DestroyObject()
    {
        isThrown = false;
        audio.Stop();
        Destroy(gameObject);
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, damageRange);
    }
}

public enum BossThrowableType
{
    RockUpwardsThrow,
    RockDownwardsThrow,
}

[System.Serializable]
public class BossThrowContainer
{
    public BossThrowableType bossThrowableType;
    public BossThrowData bossThrowData;
}

[System.Serializable]
public class BossThrowData
{
    public BossThrowables bossThrowablesPrefab;
    public Transform throwPoint;
}