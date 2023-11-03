using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseEnemy : MonoBehaviour
{
    #region PUBLIC_VARS

    public float health;
    public float maxHealth;
    public float speed;
    public float damagePower;
    public List<BaseReward> rewards;
    [HideInInspector] public Player player;
    public float rotateSpeed;
    public Animator animator;
    public Rigidbody rb;
    public bool isDead;
    public int rewardScore;
    public Vector3 startPos;
    public bool isRespawnBlock ;
    public bool _isAttacking;
    public AudioSource audio;

    public enum Enemy
    {
        turtle,
        bee,
        tree,
        armdilo
    }

    public Enemy enemy;
    
    #endregion

    #region PRIVATE_VARS

    #endregion

    #region UNITY_CALLBACKS

    public virtual void Start()
    {
        SetUp();

        SoundManager._soundManager.AddInList(audio);
    }

    public virtual void Update()
    {

    }

    #endregion

    #region PUBLIC_FUNCTIONS

    public void SetUp()
    {
        startPos = transform.position;
        player = GameManager.instance.Player;
        maxHealth = health;
    }
    
    public virtual void Damage(float damageValue,bool attackByJump = false)
    {
        if (attackByJump)
        {
            health = 0;
        }
        else
        {
            health -= damageValue;
        }
        if (health <= 0)
        {
            health = 0;
            Kill();
        }
    }

    public virtual void Kill()
    {
        health = 0;
        RestoreManager.Instance.AddDeadEnemy(this);
        GameManager.instance.AddScore(rewardScore);   
    }
    
    public virtual void AttackAnimation()
    {
        
    }
    
    public bool PlayerCanDamage(Player plr)
    {
        return plr && !isDead && !player.isDamageing && !player.isDead;
    }

    public virtual void SetRange(Vector3 start, Vector3 end)
    {
        
    }

    public virtual void Restore()
    {
        
    }

    public void ToggleSound(int i, bool loop, SoundManager.Soundtype type)
    {
        if (audio.isPlaying)
        {
            audio.Stop();
        }

        switch ((int)type)
        {
            case 0:
                {
                    audio.clip = SoundManager._soundManager.turtle[i];
                    break;
                }
            case 1:
                {
                    audio.clip = SoundManager._soundManager.bee[i];
                    break;
                }
            case 2:
                {
                    audio.clip = SoundManager._soundManager.tree[i];
                    break;
                }
            case 3:
                {
                    audio.clip = SoundManager._soundManager.armedillo[i];
                    break;
                }
            case 10:
                {
                    audio.clip = SoundManager._soundManager.Bull[i];
                    break;
                }
            case 11:
                {
                    audio.clip = SoundManager._soundManager.dragon[i];
                    break;
                }
            case 12:
                {
                    audio.clip = SoundManager._soundManager.hippo[i];
                    break;
                }
            case 13:
                {
                    audio.clip = SoundManager._soundManager.stoneman[i];
                    break;
                }
            default:
                {
                    print("none");
                    break;
                }
        }

        audio.loop = loop;
        audio.volume = GS.Instance.sfxSoundVolume;
        audio.Play();
    }

    public void StopAllSound(float time)
    {
        StartCoroutine(StopSoundsCoroutine(time));
    }


    #endregion

    #region PRIVATE_FUNCTIONS

    #endregion

    #region CO-ROUTINES

    IEnumerator StopSoundsCoroutine(float time)
    {
        yield return new WaitForSeconds(time);

        if (isDead)
        {
            audio.Stop();   
        }
    }

    #endregion

    #region EVENT_HANDLERS

    #endregion

    #region UI_CALLBACKS

    #endregion
}