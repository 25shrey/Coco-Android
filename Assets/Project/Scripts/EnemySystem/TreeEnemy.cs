using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using UnityEngine.VFX;

public class TreeEnemy : ChaseEnemy
{
    #region PUBLIC_VARS

    public GameObject treeParts;
    public GameObject wholeTree;
    public CapsuleCollider _capsuleCollider;
    public BoxCollider killArea;
    public BoxCollider attackArea;

    #endregion

    #region PRIVATE_VARS

    [SerializeField] private VisualEffect treeVFX;
    private int runAnimation;
    private int idleAnimation;
    private int attackAnimation;
    private int spawnAnimation;
    private int deathAnimation;
    private bool playerInAttackRange;
    private Coroutine spawnAnimCo,resetAnimCo,attackCo,enemyCollisionCo;
    private Vector3 offSet = new Vector3(0, 3f, 0);

    #endregion

    #region UNITY_CALLBACKS

    public override void Start()
    {
        base.Start();
        transform.position -= offSet;
        runAnimation = Animator.StringToHash("Run");
        idleAnimation = Animator.StringToHash("Idle");
        attackAnimation = Animator.StringToHash("Attack");
        spawnAnimation = Animator.StringToHash("Spawn");
        deathAnimation = Animator.StringToHash("Death");
        wholeTree.SetActive(false);
        treeParts.SetActive(false);
    }
    
    
    private void OnCollisionEnter(Collision collision)
    {
        if (!isDead &&collision.gameObject.GetComponent<Player>())
        {
            rb.velocity = Vector3.zero;
        }
        TurtleEnemy collisionEnemy = collision.gameObject.GetComponent<TurtleEnemy>();
        if (DeadEnemyCanDamage(collisionEnemy) && enemyCollisionCo==null)
        {
            enemyCollisionCo = StartCoroutine(EnemyCheck(collisionEnemy));
        }
        if (collision.GetContact(0).thisCollider.gameObject.name == "KillArea"
            && collision.gameObject.GetComponent<Player>() && !isDead && !player.isDead && (player.transform.position - transform.position).y > 0)
        {
            base.ToggleSound(0, false, SoundManager.Soundtype.tree);

            collision.GetContact(0).thisCollider.enabled = false;
            StartCoroutine(player.JumpAttackResponce());
            Damage(0,true);

          //  SoundManager._soundManager._otherSounds.SoundToBeUsed(2, SoundManager.Soundtype.player, false, true);
           // GameManager.instance.Player._sounds.SoundToBeUsed(2, SoundManager.Soundtype.player, 0.5f);
        }
    }

    
    private bool DeadEnemyCanDamage(TurtleEnemy collisionEnemy)
    {
        return collisionEnemy && collisionEnemy.isDead;
    }
    
    private void OnCollisionExit(Collision other)
    {
        if (!isDead && other.gameObject.GetComponent<Player>())
        {
            rb.velocity = Vector3.zero;
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (PlayerCanDamage(other.gameObject.GetComponent<Player>()) && !isDead)
        {
            if (attackCo==null)
            {
                AttackAnimation();
            }
        }
        if (other.gameObject.GetComponent<DeathZone>())
        {
            Kill();
            gameObject.SetActive(false);
        }
    }

    public void OnTriggerStay(Collider other)
    {
        if (PlayerCanDamage(other.gameObject.GetComponent<Player>()) && !isDead)
        {
            playerInAttackRange = true;
            if (attackCo==null)
            {
                AttackAnimation();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (PlayerCanDamage(other.gameObject.GetComponent<Player>()))
        {
            playerInAttackRange = false;
        }
    }

    #endregion

    #region PUBLIC_FUNCTIONS

    public override void Kill()
    {
        if (!isDead)
        {
            base.ToggleSound(2, false, SoundManager.Soundtype.tree);
            killArea.enabled = false;
            attackArea.enabled = false;
            _capsuleCollider.enabled = false;
            Debug.Log("DeathAnimation");
            rb.constraints = RigidbodyConstraints.FreezeAll;
            isDead = true;
            if (spawnAnimCo != null)
            {
                StopCoroutine(spawnAnimCo);
            }
            if (rotateCo != null)
            {
                StopCoroutine(rotateCo);
            }
            if (chaseCO != null)
            {
                StopCoroutine(chaseCO);
            }
            if (resetAnimCo != null)
            {
                StopCoroutine(resetAnimCo);
            }
            animator.SetTrigger(deathAnimation);
            base.Kill();

            if (isDead)
            {
                base.StopAllSound(0.3f);
            }
        }
    }
    
    public override void PlayerEnterOnRange()
    {
        base.ToggleSound(1, true, SoundManager.Soundtype.tree);
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        _capsuleCollider.enabled = true;
        if (spawnAnimCo == null)
        {
            spawnAnimCo = StartCoroutine(ShowSpawnAnimation());
        }
    }

    
    public override void PlayerExitOnRange()
    {
        rb.constraints = RigidbodyConstraints.FreezeAll;
        killArea.enabled = false;
        attackArea.enabled = false;
        _capsuleCollider.enabled = false;
        if (resetAnimCo==null)
        {
            resetAnimCo = StartCoroutine(ShowResetAnimation());
        }
    }

    public override void AttackAnimation()
    {
        attackCo = StartCoroutine(AttackCo());
    }
    
    
    public override void Restore()
    {
        base.Restore();
        gameObject.SetActive(true);
        transform.position = startPos;
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
    }    

    #endregion

    #region PRIVATE_FUNCTIONS
    
    #endregion

    #region CO-ROUTINES
    
    
    public IEnumerator EnemyCheck(TurtleEnemy enemy)
    {
        float time = 2.5f;
        while (time > 0)
        {
            yield return null;
            time -= Time.deltaTime;
            if (enemy.rb.velocity.magnitude > 4.5)
            {
                Damage(health);
                break;
            }

            if (enemy.rb.velocity.magnitude <= 0.3)
            {
                break;
            }
        }

        enemyCollisionCo = null;
    }

    public IEnumerator AttackCo()
    {
        wholeTree.SetActive(true);
        animator.SetTrigger(attackAnimation);
        yield return new WaitForSeconds(0.25f);
        if(chaseCO!=null)
        {
            StopCoroutine(chaseCO);
            chaseCO = null;
        }
        float angle = GetMoveingAngle(transform.position, GetTargetPos());
        rotateCo = StartCoroutine(Rotate(angle));
        yield return new WaitForSeconds(0.4f);
        if (playerInAttackRange)
        {
            Vector3 dir = player.transform.position-transform.position;
            dir = new Vector3(dir.x, 0, dir.z).normalized;
            player.Damage(dir*1.25f,DamageAnimType.Damage, damagePower);
            base.ToggleSound(0, false, SoundManager.Soundtype.tree);
            yield return new WaitForSeconds(0.5f);
        }
        else
        {
            yield return new WaitForSeconds(0.15f);
        }
        if (playerInRange && !isDead)
        {
            animator.SetTrigger(runAnimation);
            base.ToggleSound(1, true, SoundManager.Soundtype.tree);
            treeParts.SetActive(false);
            if (chaseCO != null)
            {
                StopCoroutine(chaseCO);
                yield return null;
            }
            chaseCO = StartCoroutine(Chase(GetTargetPos()));

            base.ToggleSound(1, true, SoundManager.Soundtype.tree);
        }

        attackCo = null;
    }

    public IEnumerator ShowSpawnAnimation()
    {
        Debug.Log("ShowSpawnAnimation");
        while (resetAnimCo!=null || attackCo!=null)
        {
            yield return null;
        }
        transform.position = startPos - offSet;
        treeParts.SetActive(true);
        yield return moveCo = StartCoroutine(Move(transform.position + offSet));
        yield return new WaitForSeconds(0.25f);
        animator.SetTrigger(spawnAnimation);
        yield return new WaitForSeconds(0.75f);
        treeVFX.Play();
        if (playerInRange)
        {
            killArea.enabled = true;
            attackArea.enabled = true;
            base.PlayerEnterOnRange();
        }
        spawnAnimCo = null;
    }

    public IEnumerator ShowResetAnimation()
    {
        while (spawnAnimCo!=null || attackCo!=null)
        {
            yield return null;
        }
        base.PlayerExitOnRange();
        animator.SetTrigger(deathAnimation);
        yield return new WaitForSeconds(1.25f);
        yield return moveCo = StartCoroutine(Move(transform.position - offSet));
        yield return new WaitForSeconds(0.25f);
        wholeTree.SetActive(false);
        treeParts.SetActive(false);
        transform.position = startPos - offSet;
        resetAnimCo = null;
    }
    
    #endregion

    #region EVENT_HANDLERS

    #endregion

    #region UI_CALLBACKS

    #endregion
}