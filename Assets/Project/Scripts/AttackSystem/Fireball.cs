using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Fireball : MonoBehaviour
{
     #region PUBLIC_VARS

    private Rigidbody rb;
    public Vector3 direction;
    public float height;
    public float width;
    public Vector3 lastDir;
    public float time;
    public GameObject muzzlePrefab;
    public GameObject hitPrefab;
    public float damagePower = 30;

    public float gravity;
    [SerializeField]
    private LayerMask _layer;
    #endregion

    #region PRIVATE_VARS
    
    private Vector3 offset;
    private bool collided;

    [SerializeField]
    private AudioSource _aud;
        
    #endregion

    #region UNITY_CALLBACKS
    void Start()
    {
        if (muzzlePrefab != null) {
            var muzzleVFX = Instantiate(muzzlePrefab, transform.position, Quaternion.identity);
            muzzleVFX.transform.forward = gameObject.transform.forward + offset;
            var ps = muzzleVFX.GetComponent<ParticleSystem>();
            if (ps != null)
                Destroy (muzzleVFX, ps.main.duration);
            else {
                var psChild = muzzleVFX.transform.GetChild(0).GetComponent<ParticleSystem>();
                Destroy (muzzleVFX, psChild.main.duration);
            }
        }
    }

    public void Initialized()
    {
        rb = GetComponent<Rigidbody>();
        StartCoroutine(AutoDestroy());
        direction = direction.normalized*width;
        direction.y = 0;
        rb.velocity = direction;
    }

    void Update()
    {
        lastDir = rb.velocity;
        rb.AddForce(Vector3.up * gravity * Time.deltaTime, ForceMode.Acceleration);

    }

    private void OnCollisionEnter(Collision collision)
    {
        BaseEnemy enemy = collision.gameObject.GetComponent<BaseEnemy>();

        if (collision.gameObject.GetComponent<FloatingObject>())
        {
            ToggleSound(SoundManager._soundManager.player[6]);
            DestroyEffect(collision);
        }
        else if (enemy && !collided)
        {
            ToggleSound(SoundManager._soundManager.player[6]);
            enemy.Damage(damagePower);
            DestroyEffect(collision);
        }
        else if(((1 << collision.gameObject.layer) & _layer) != 0)
        {
            Vector3 reflectionDir = Vector3.Reflect(lastDir.normalized, collision.contacts[0].normal);
            reflectionDir.y = 1;
            if (collision.gameObject.layer == 8)
            {
                reflectionDir = reflectionDir.normalized*width;
                reflectionDir.y = height;
                rb.velocity = reflectionDir;
                //rb.AddForce(reflectionDir.normalized * height, ForceMode.VelocityChange);
            }
            else
            {
                reflectionDir = reflectionDir.normalized*width;
                reflectionDir.y = 1;
                rb.velocity = reflectionDir;
            }
        }
        else
        {
            ToggleSound(SoundManager._soundManager.player[6]);
            DestroyEffect(collision);
        }
    }

    private void DestroyEffect(Collision collision)
    {
        collided = true;

        GetComponent<Rigidbody>().isKinematic = true;

        ContactPoint contact = collision.contacts[0];
        Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
        Vector3 pos = contact.point;

        if (hitPrefab != null)
        {
            var hitVFX = Instantiate(hitPrefab, pos, rot);
            var ps = hitVFX.GetComponent<ParticleSystem>();
            if (ps == null)
            {
                var psChild = hitVFX.transform.GetChild(0).GetComponent<ParticleSystem>();
                Destroy(hitVFX, psChild.main.duration);
            }
            else
                Destroy(hitVFX, ps.main.duration);
        }

        StartCoroutine(DestroyParticle(0.15f));
    }

    void ToggleSound(AudioClip clip)
    {
        _aud.clip = clip;

        if (_aud.isPlaying)
        {
            _aud.Stop();
        }

        _aud.loop = false;
        _aud.Play();
    }

    #endregion

    #region PUBLIC_FUNCTIONS

    #endregion

    #region PRIVATE_FUNCTIONS

    #endregion

    #region CO-ROUTINES

    public IEnumerator AutoDestroy()
    {
        float t = time;
        while (t>0)
        {
            t -= Time.deltaTime;
            yield return null;
        }
        Destroy(gameObject);
    }
    
    public IEnumerator DestroyParticle (float waitTime) {

        if (transform.childCount > 0 && waitTime != 0) {
            List<Transform> tList = new List<Transform> ();

            foreach (Transform t in transform.GetChild(0).transform) {
                tList.Add (t);
            }		

            while (transform.GetChild(0).localScale.x > 0) {
                yield return new WaitForSeconds (0.01f);
                transform.GetChild(0).localScale -= new Vector3 (0.1f, 0.1f, 0.1f);
                for (int i = 0; i < tList.Count; i++) {
                    tList[i].localScale -= new Vector3 (0.1f, 0.1f, 0.1f);
                }
            }
        }
		
        yield return new WaitForSeconds (waitTime);
        Destroy (gameObject);
    }
    
    #endregion

    #region EVENT_HANDLERS

    #endregion

    #region UI_CALLBACKS

    #endregion
}
