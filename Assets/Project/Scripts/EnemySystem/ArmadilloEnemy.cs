
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;
using Random = UnityEngine.Random;

public class ArmadilloEnemy : ChaseEnemy, IEnemyOutOfBoundary
{
    #region PUBLIC_VARS

    public BoxCollider _boxCollider;
    public BoxCollider _bodyboxCollider;
    //public SphereCollider _sphereCollider;
    public CapsuleCollider _capsuleCollider;
    public Transform rotatingShell;
    public List<SkinnedMeshRenderer> skines;
    public GameObject skinnedMeshGroupGameObject;
    public GameObject meshRendererGroupGameobject;
    #endregion

    #region PRIVATE_VARS
    [SerializeField] private VisualEffect dieVFX;
    private int walkAnimation;
    private int ballFormingAnimation;
    private int deathAnimation;
    private int deathTdleAnimation;
    private int ballDeformingAnimation;
    private Coroutine fixMovementco;
    private Coroutine enemyCollisionCo;

    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private float _distanceToGround;

    public float offset;

    private Coroutine enemyoutOfBoundCoroutine;


    #endregion

    #region UNITY_CALLBACKS

    public override void Start()
    {
        Debug.Log("Inside - Start - Armadilo Enemy");
        rb.constraints = RigidbodyConstraints.FreezeAll;
        base.Start();
        SetNextMoveTarget(true);
        fixMovementco = StartCoroutine(FixMovement());
        walkAnimation = Animator.StringToHash("Walk");
        ballFormingAnimation = Animator.StringToHash("BallForming");
        ballDeformingAnimation = Animator.StringToHash("BallDeforming");
        deathAnimation = Animator.StringToHash("Death");
        deathTdleAnimation = Animator.StringToHash("DeathIdle");

        enemyoutOfBoundCoroutine = StartCoroutine(GroundCheckerCoroutine());
    }
    #endregion

    #region PUBLIC_FUNCTIONS

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Player>() && !player.isDamageing && !player.isDead)
        {
            if (!isDead)
            {
                StartCoroutine(player.JumpAttackResponce());
                Damage(0, true);
            }
        }
        else if (other.gameObject.GetComponent<DeathZone>())
        {
            Kill();
            gameObject.SetActive(false);
        }
    }

    private async void OnCollisionEnter(Collision collision)
    {
        TurtleEnemy collisionEnemy = collision.gameObject.GetComponent<TurtleEnemy>();
        if (DeadEnemyCanDamage(collisionEnemy) && enemyCollisionCo == null)
        {
            enemyCollisionCo = StartCoroutine(EnemyCheck(collisionEnemy));
        }
        else if (PlayerCanDamage(collision.gameObject.GetComponent<Player>()))
        {
            if (!_isAttacking)
            {
                _isAttacking = true;
                
                base.ToggleSound(0, false, SoundManager.Soundtype.armedillo);

                Vector3 dir = (player.transform.position - transform.position).normalized;
                player.Damage(dir,DamageAnimType.Damage, damagePower);
               // SoundManager._soundManager._otherSounds.SoundToBeUsed(2, SoundManager.Soundtype.player, false, true);
                player.PlayPushVFX();
                await Task.Delay(2000);
                player.StopPushVFX();
                base.ToggleSound(1, true, SoundManager.Soundtype.armedillo);
                // GameManager.instance.Player._sounds.SoundToBeUsed(2, SoundManager.Soundtype.player, 0.5f);   

                _isAttacking = false;
            }
        }
    }

    private bool DeadEnemyCanDamage(TurtleEnemy collisionEnemy)
    {
        return collisionEnemy && collisionEnemy.isDead;
    }

    public override void Kill()
    {
        if (!isDead)
        {
            base.ToggleSound(2, false, SoundManager.Soundtype.armedillo);
            isDead = true;
            rb.constraints = RigidbodyConstraints.FreezeAll;
            Debug.Log("Kill");
            _boxCollider.enabled = false;
            _bodyboxCollider.enabled = false;
            _capsuleCollider.enabled = false;
            GameManager.instance.PlayVFX(dieVFX);
            animator.SetTrigger(deathAnimation);
            GameManager.instance.StopVFX(dieVFX, 5000);
            if (moveCo != null)
            {
                StopCoroutine(moveCo);
            }
            if (fixMovementco != null)
            {
                StopCoroutine(fixMovementco);
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
            if (isDead)
            {
                base.StopAllSound(1.2f);
            }

            StartCoroutine(DeformBody());
        }
    }

    public IEnumerator GroundCheckerCoroutine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.5f);

        while (true)
        {
            yield return wait;
            GroundChecker();
        }
    }

    public void GroundChecker()
    {
        if (!isDead && !GameManager.instance.Player.isBossFightStart)
        {
            if (Physics.Raycast(transform.position + (Vector3.up * offset), Vector3.down, _distanceToGround, _groundLayer))
            {
                //  print("not");
            }
            else
            {
               //  print("kill");
                Kill();
                if (enemyoutOfBoundCoroutine != null)
                {
                    StopCoroutine(enemyoutOfBoundCoroutine);
                }
            }
        }
    }

    public override async void PlayerEnterOnRange()
    {
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        if (rotateCo != null)
        {
            StopCoroutine(rotateCo);
        }
        if (moveCo != null)
        {
            StopCoroutine(moveCo);
        }
        if (fixMovementco != null)
        {
            StopCoroutine(fixMovementco);
        }
        base.PlayerEnterOnRange();
        await Task.Delay(100);
        animator.SetTrigger(ballFormingAnimation);
        base.ToggleSound(1, true, SoundManager.Soundtype.armedillo);
        StartCoroutine(RotatingShell());
    }

    public override async void PlayerExitOnRange()
    {
        rb.constraints = RigidbodyConstraints.FreezeAll;
        base.PlayerExitOnRange();
        SetNextMoveTarget(false);
        fixMovementco = StartCoroutine(FixMovement());
        animator.SetTrigger(ballDeformingAnimation);
        await Task.Delay(400);
        if (!playerInRange && !isDead)
        {
            animator.SetTrigger(walkAnimation);
            base.ToggleSound(1, true, SoundManager.Soundtype.armedillo);
        }
    }
    
    
    public override void Restore()
    {
       // Invoke("StartGainTheGroundChecker", 2f);
        base.Restore();
        gameObject.SetActive(true);
        transform.position = startPos;
        Respawn();
    }


    public void ResumeChecker()
    {
        enemyoutOfBoundCoroutine = StartCoroutine(GroundCheckerCoroutine());
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
        for (int i = 0; i < skines.Count; i++)
        {
            skines[i].materials[0].SetFloat("_CutoffHeight",1.7f);
        }
        Debug.Log("Respawn Turtle...");
        health = maxHealth;
        isDead = false;
        _capsuleCollider.enabled = true;
        await Task.Delay(1000);
        _boxCollider.enabled = true;
        _bodyboxCollider.enabled = true;

        Invoke("ResumeChecker", 6f);
    }

    public async void OnSpawnedByBoss(Action callback)
    {
        Debug.Log("Inside - OnSpawnedByBoss - Armadilo Enemy");
        skinnedMeshGroupGameObject.SetActive(false);
        meshRendererGroupGameobject.SetActive(false);
        animator.SetTrigger(ballFormingAnimation);
        await Task.Delay(350);
        callback?.Invoke();
        skinnedMeshGroupGameObject.SetActive(true);
        meshRendererGroupGameobject.SetActive(true);
        StartCoroutine(RotatingShell());
    }
    #endregion

    #region PRIVATE_FUNCTIONS

    #endregion

    #region CO-ROUTINES

    public IEnumerator DeformBody()
    {
        yield return new WaitForSeconds(2);
        float time = 1.5f;
        float totalTime = time;
        while (time>0)
        {
            yield return null;
            time -= Time.deltaTime;
            for (int i = 0; i < skines.Count; i++)
            {
                float value = math.lerp(1.7f, -1.5f, 1-time/totalTime);
                skines[i].materials[0].SetFloat("_CutoffHeight",value);
            }
        }
    }

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

    public IEnumerator FixMovement()
    {
        yield return rotateCo = StartCoroutine(Rotate(GetMoveingAngle(transform.position, targetPoint), true));
        yield return moveCo = StartCoroutine(Move(targetPoint));
        SetNextMoveTarget(false);
        fixMovementco = StartCoroutine(FixMovement());
    }
    
    public IEnumerator RotatingShell()
    {
        float time=0.75f,totalTime=time;
        while (time >0)
        {
            yield return null;
            time -= Time.deltaTime;
            rotatingShell.localEulerAngles = Vector3.Lerp(Vector3.zero, new Vector3(360, 0, 0), 1 - time / totalTime);
        }
        StartCoroutine(RotatingShell());
    }

    #endregion

    #region

    #endregion

    #region UI_CALLBACKS

    #endregion
}