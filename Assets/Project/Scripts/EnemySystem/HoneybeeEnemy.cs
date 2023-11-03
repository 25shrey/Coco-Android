using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.VFX;

public class HoneybeeEnemy : ChaseEnemy
{
    #region PUBLIC_VARS

    private Coroutine fixMovementco;
    private int flyAnimation;
    private int attackAnimation;
    private int afterDeathAnimation;
    private int deathAnimation;
    private bool isAttacking;

    public BoxCollider _boxCollider;
    public CapsuleCollider _capsuleCollider;

    [SerializeField] VisualEffect dieVFX;
    [SerializeField] VisualEffect attackVFX;
    
    #endregion

    #region PRIVATE_VARS

    #endregion

    #region UNITY_CALLBACKS

    public override void Start()
    {
        rb.constraints = RigidbodyConstraints.FreezeAll;
        base.Start();
        SetNextMoveTarget(true);
        flyAnimation = Animator.StringToHash("Fly");
        attackAnimation = Animator.StringToHash("Attack");
        afterDeathAnimation = Animator.StringToHash("AfterDeath");
        deathAnimation = Animator.StringToHash("Death");
        animator.SetTrigger(flyAnimation);
        base.ToggleSound(3, true, SoundManager.Soundtype.bee);
        fixMovementco = StartCoroutine(FixMovement());
    }
    
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Player>()&&!player.isDamageing && !player.isDead)
        {
            if (!isDead)
            {
                rb.velocity = Vector3.zero;
                StartCoroutine(player.JumpAttackResponce());
                Damage(0,true);
            }
        }
        else if (other.gameObject.GetComponent<DeathZone>())
        {
            Kill();
        }
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if (!isDead && PlayerCanDamage(collision.gameObject.GetComponent<Player>()) && !isAttacking)
        {
            AttackAnimation();
        }
        rb.velocity = Vector3.zero;
    }

    private void OnCollisionExit(Collision collision)
    {
        if(!isDead) 
        {
            animator.SetTrigger(flyAnimation);
        }
        rb.velocity = Vector3.zero;
    }


    #endregion

    #region PUBLIC_FUNCTIONS

    public async override void Kill()
    {
        if (!isDead)
        {
            dieVFX.Play();
            isDead = true;
            if (fixMovementco != null)
            {
                StopCoroutine(fixMovementco);
            }
            if (moveCo != null)
            {
                StopCoroutine(moveCo);
            }
            if (rotateCo != null)
            {
                StopCoroutine(rotateCo);
            }
            if (chaseCO != null)
            {
                StopCoroutine(chaseCO);
            }
            base.Kill(); 
            rb.constraints = RigidbodyConstraints.FreezeRotation;
            Debug.Log(" Bee Kill");
            _boxCollider.enabled = false;
            _capsuleCollider.enabled = false;
            animator.SetTrigger(deathAnimation);
            base.ToggleSound(2, false, SoundManager.Soundtype.bee);
            await Task.Delay(500);

            if (isDead)
            {
                base.StopAllSound(1f);
            }

            gameObject.SetActive(false);
        }
    }

    private bool PlayerCanDamage(Player plr)
    {
        return plr && !isDead && !player.isDamageing && !player.isDead;
    }


    public override async void AttackAnimation()
    {
        isAttacking = true;
        if (chaseCO != null)
        {
            StopCoroutine(chaseCO);
        }
        base.ToggleSound(0, false, SoundManager.Soundtype.bee);
       // SoundManager._soundManager._otherSounds.SoundToBeUsed(2, SoundManager.Soundtype.player, false, true);
        animator.SetTrigger(attackAnimation);
        await Task.Delay(200);
        attackVFX.Play();
        Vector3 dir = (player.transform.position - transform.position).normalized;
        player.Damage(dir,DamageAnimType.BeeDamage, damagePower);
        await Task.Delay(500);
        base.ToggleSound(1, true, SoundManager.Soundtype.bee);
        // GameManager.instance.Player._sounds.SoundToBeUsed(2, SoundManager.Soundtype.player, 0.5f);

        if (playerInRange)
        {
            chaseCO = StartCoroutine(Chase(GetTargetPos()));
        }        
        isAttacking = false;
    }


    public override void PlayerEnterOnRange()
    {
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        if (fixMovementco != null)
        {
            StopCoroutine(fixMovementco);
        }
        if (moveCo != null)
        {
            StopCoroutine(moveCo);
        }
        if (rotateCo != null)
        {
            StopCoroutine(rotateCo);
        }
        base.ToggleSound(1, true, SoundManager.Soundtype.bee);
        base.PlayerEnterOnRange();
    }

    public override void PlayerExitOnRange()
    {
        rb.constraints = RigidbodyConstraints.FreezeAll;
        base.PlayerExitOnRange();
        base.ToggleSound(3, true, SoundManager.Soundtype.bee);
        SetNextMoveTarget(false);
        animator.SetTrigger(flyAnimation);
        base.ToggleSound(3, true, SoundManager.Soundtype.bee);
        fixMovementco = StartCoroutine(FixMovement());
    }

    public override Vector3 GetTargetPos()
    { 
        return new Vector3(player.transform.position.x,
            player.transform.position.y, player.transform.position.z) + new Vector3(0,2.5f,0);
    }
    
    
    public override IEnumerator CastumGravity(float Grevity)
    { 
        yield return null;
        if (isDead)
        {
            castumGravity = StartCoroutine(base.CastumGravity(Grevity));
        }
        else
        {
            castumGravity = null;
        }
    }

    
    public override void Restore()
    {
        base.Restore();
        gameObject.SetActive(true);
        transform.position = startPos;
        animator.SetTrigger(flyAnimation);
        base.ToggleSound(3, true, SoundManager.Soundtype.bee);
        Respawn();
    }

    public async void Respawn()
    {
        if (PlayerInEndRange(player.transform.position))
        {
            playerInRange = false;
        }
        else
        {
            playerInRange = true;
        }
        await Task.Delay(50);
        if (!PlayerInStartRange(player.transform.position))
        {
            if (!playerInRange)
            {
                PlayerEnterOnRange();
                playerInRange = true;
            }
        }
        health = maxHealth;
        isDead = false;
        _capsuleCollider.enabled = true;
        await Task.Delay(1000);
        _boxCollider.enabled = true;
    }

    
    #endregion

    #region PRIVATE_FUNCTIONS

    #endregion

    #region CO-ROUTINES
    
    public IEnumerator FixMovement()
    {
        yield return rotateCo=StartCoroutine(Rotate(GetMoveingAngle(transform.position,targetPoint)));
        yield return moveCo = StartCoroutine(Move(targetPoint));
        SetNextMoveTarget(false);
        fixMovementco = StartCoroutine(FixMovement());
    }
    
    #endregion

    #region EVENT_HANDLERS

    #endregion

    #region UI_CALLBACKS

    #endregion
}
