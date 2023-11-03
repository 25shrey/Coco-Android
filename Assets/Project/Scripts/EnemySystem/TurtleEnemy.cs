using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.VFX;
using Random = UnityEngine.Random;

public class TurtleEnemy : ChaseEnemy, IEnemyOutOfBoundary
{
    #region PUBLIC_VARS

    public bool isRemoved;
    public int wakeUpTime;
    public LayerMask _layer;
    public bool outOfRange = false;

    public Coroutine playerPosCo;

    //Turtle Throw//
    // public TurtleThrow _throw;
    // public bool pickState
    // {
    //     set { _currentlyPicked = value; }
    //     get { return _currentlyPicked; }
    // }

    public VisualEffect KickVFX
    {
        get { return turtleKickTrailVFX; }
    }

    public VisualEffect DieVFX
    {
        get { return turtleDieVFX; }
    }

    #endregion

    #region PRIVATE_VARS

    [SerializeField] private VisualEffect turtleDieVFX;
    [SerializeField] private VisualEffect turtleKickTrailVFX;
    private int runAnimation;
    private int walkAnimation;
    private int attackAnimation;
    private int deathAnimation;
    private int PushAnimation;
    private int RespawnAnimation;
    public BoxCollider _boxCollider;
    public SphereCollider _sphereCollider;
    public CapsuleCollider _capsuleCollider;
    private Coroutine enemyCollisionCo;
    private Coroutine fixMovementco;
    private Coroutine wakeUpCo;
    private bool isForceBackContinue; 
    private float rotetionSpeedAfterDeath;
    public Quaternion _initialRotation;
    private float defoltAnimationSpeed;

    [SerializeField]
    private bool _currentlyPicked;

    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private float _distanceToGround;

    private Coroutine enemyoutOfBoundCoroutine;

    public float offset;

    #endregion

    #region UNITY_CALLBACKS

    public override void Start()
    {
        defoltAnimationSpeed = animator.speed;
        //Turtle Throw//
       // _throw = GetComponent<TurtleThrow>();
        _initialRotation = transform.rotation;
        base.ToggleSound(1, true, SoundManager.Soundtype.turtle);
        rotetionSpeedAfterDeath = 75;
        rb.constraints = RigidbodyConstraints.FreezeAll;
        base.Start();
        SetNextMoveTarget(true);
        fixMovementco = StartCoroutine(FixMovement());
        runAnimation = Animator.StringToHash("Run");
        walkAnimation = Animator.StringToHash("Walk");
        attackAnimation = Animator.StringToHash("Attack");
        deathAnimation = Animator.StringToHash("Dead");
        RespawnAnimation = Animator.StringToHash("Respawn");
        PushAnimation = Animator.StringToHash("HandPush");

        enemyoutOfBoundCoroutine = StartCoroutine(GroundCheckerCoroutine());

        //  playerPosCo = StartCoroutine(EnemyPositionCheckerCo());
    }

    public override void Update()
    {
        base.Update();
        if (isDead)
        {
            Rotate();
        }
        if (playerInRange)
        {
            animator.speed = defoltAnimationSpeed*(1+(chaseSpeed-minChessSpeed)/(maxChessSpeed-minChessSpeed));
        }
        else
        {
            animator.speed = defoltAnimationSpeed;
        }
    }

    IEnumerator EnemyPositionCheckerCo()
    {
        WaitForSeconds wait = new WaitForSeconds(0.2f);

        while (true)
        {
            yield return wait;

            EnemyPositionChecker();
        }
    }

    public void EnemyPositionChecker()
    {
        outOfRange = false;
        if (!PlayerInEndRange(this.transform.position))
        {
            if (!this.isDead)
            {
                outOfRange = true;
            }
        }

        if(!Physics.Raycast(transform.position, Vector3.down, 5f, _layer) && outOfRange)
        {
            this.Kill();
            outOfRange = false;
        }
    }

    private void Rotate()
    {
        if (rb.velocity.magnitude > 0.5f)
        {
            float velocityDri = rb.velocity.magnitude * Time.deltaTime *
                                rotetionSpeedAfterDeath;
            transform.Rotate(0, velocityDri, 0);
        }
    }

    #endregion
    
    #region PUBLIC_FUNCTIONS

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Player>()&&!player.isDamageing && !player.isDead)
        {
            if ((player.transform.position - transform.position).y > 0)
            {
                if (!isDead)
                {
                    StartCoroutine(player.JumpAttackResponce());
                    Damage(0,true);
                }
            }
        }
        else if (other.gameObject.GetComponent<DeathZone>() && !isRemoved)
        {
            //Turtle Throw//
            //_throw.DestroyThrowAffect();
            DeathZoneHit();
        }
    }

    public async void DeathZoneHit()
    {
            //Turtle Throw//
            // if (GameManager.instance.Player.grabPoint.transform.childCount > 0)
            // {
            //     GameManager.instance.Player._grabber._picked = false;
            //
            //     if(GameManager.instance.Player.viewer._turtle == this)
            //     {
            //         GameManager.instance.Player.viewer.CanPick = false;
            //         GameManager.instance.Player.viewer._turtle = null;
            //
            //         GameManager.instance.Player.grabPoint.transform.GetChild(0).SetParent(null);
            //     }
            // }

        isRemoved = true;
        Kill();
        _sphereCollider.enabled = false;
        await Task.Delay(2000);
        rb.constraints = RigidbodyConstraints.FreezeAll;
        if (castumGravity != null && isDead)
        {
            StopAllCoroutines();
        }
    }

    
    private async void OnCollisionEnter(Collision collision)
    {
        if (rb.velocity.y > 0 && !isDead)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        }
        TurtleEnemy collisionEnemy = collision.gameObject.GetComponent<TurtleEnemy>();
        if (DeadEnemyCanDamage(collisionEnemy) && enemyCollisionCo==null)
        {
            enemyCollisionCo = StartCoroutine(EnemyCheck(collisionEnemy));
        }
        else if (PlayerCanDamage(collision.gameObject.GetComponent<Player>()))
        {
            if (!_isAttacking)
            {
                _isAttacking = true;
                
                base.ToggleSound(0, false, SoundManager.Soundtype.turtle);

                Vector3 dir = (player.transform.position-transform.position).normalized;
                animator.SetTrigger(PushAnimation);
                await Task.Delay(250);
                player.Damage(dir,DamageAnimType.BeeDamage,damagePower);
              //  SoundManager._soundManager._otherSounds.SoundToBeUsed(2, SoundManager.Soundtype.player, false, true);
                player.PlayPushVFX();
                await Task.Delay(1000);
                player.StopPushVFX();

                base.ToggleSound(1, true, SoundManager.Soundtype.turtle);

                _isAttacking = false;
            }

            // GameManager.instance.Player._sounds.SoundToBeUsed(2, SoundManager.Soundtype.player, 0.5f);

            //StartCoroutine(PlayChaseSound());
        }
        else if (collision.gameObject.GetComponent<FloatingObject>())
        {
            transform.SetParent(collision.transform,true);
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
            //RaycastHit hit;
            //if (Physics.SphereCast(transform.position + (Vector3.up * 0.2f),
            //    radius,
            //    Vector3.down, out hit,
            //    _distanceToGround, _groundLayer))
            //{
            //    print("noitttttt");

            //}
            //else
            //{
            //    print("kill");
            //    // Kill();
            //    //if (enemyoutOfBoundCoroutine != null)
            //    //{
            //    //    StopCoroutine(enemyoutOfBoundCoroutine);
            //    //}
            //}

            if(Physics.Raycast(transform.position + (Vector3.up * offset), Vector3.down, _distanceToGround, _groundLayer))
            {
              //  print("not");
            }
            else
            {
                Kill();
                if (enemyoutOfBoundCoroutine != null)
                {
                    StopCoroutine(enemyoutOfBoundCoroutine);
                }
            }
        }
    }

    private async void OnCollisionExit(Collision other)
    {
        if (rb.velocity.y > 0 && !isDead)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        }
        if (!isDead && other.gameObject.GetComponent<Player>())
        {
            if (!isForceBackContinue)
            {
                StartCoroutine(ForceBack());
            }
        }
        if (other.gameObject.GetComponent<FloatingObject>())
        {
            transform.SetParent(null);
        }
    }

    private bool DeadEnemyCanDamage(TurtleEnemy collisionEnemy)
    {
        return collisionEnemy && collisionEnemy.isDead ;
    }

    public override async void Kill()
    {
        if (!isDead)
        {
            base.ToggleSound(2, false, SoundManager.Soundtype.turtle);
            turtleDieVFX.Play();
            isDead = true;
            rb.constraints = RigidbodyConstraints.FreezeRotation;
            _boxCollider.enabled = false;
            _sphereCollider.isTrigger = false;
            _sphereCollider.enabled = true;
            _capsuleCollider.enabled = false;
            GameManager.instance.PlayVFX(turtleDieVFX);
            animator.SetTrigger(deathAnimation);
            GameManager.instance.StopVFX(turtleDieVFX);
            GameManager.instance.PlayVFX(turtleKickTrailVFX);
          //  base.StopAllSound();

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
            if (castumGravity != null)
            {
                StopCoroutine(castumGravity);
                castumGravity = null;
            }
            await Task.Delay(800);

           //  base.StopAllSound();

            if (isDead)
            {
                base.StopAllSound(0.6f);
            }

            castumGravity = StartCoroutine(CastumGravity(-3200));
            if (wakeUpCo != null)
            {
                StopCoroutine(wakeUpCo);
            }

            if (GameManager.instance.Player.isBossFightStart)
            {
                this.gameObject.SetActive(false);
                return;
            }
            wakeUpCo = StartCoroutine(WakeUp());
        }
    }

    public override async void AttackAnimation()
    {
        if(chaseCO!=null)
        {
            StopCoroutine(chaseCO);
        }
        animator.SetTrigger(attackAnimation);
        await Task.Delay(2000);
        if (playerInRange)
        {
            chaseCO = StartCoroutine(Chase(GetTargetPos()));
        }
    }

    public override void PlayerEnterOnRange()
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
        animator.SetTrigger(runAnimation);
        if (!isDead)
        {
            base.ToggleSound(1, true, SoundManager.Soundtype.turtle);
        }
        base.PlayerEnterOnRange();
    }

    public override void PlayerExitOnRange()
    {
        rb.constraints = RigidbodyConstraints.FreezeAll;
        base.PlayerExitOnRange();
        SetNextMoveTarget(false);
        animator.SetTrigger(walkAnimation);
        if (!isDead)
        {
            base.ToggleSound(1, true, SoundManager.Soundtype.turtle);
        }
        fixMovementco = StartCoroutine(FixMovement());
    }

    public override async void Restore()
    {
        print("respawn after death");
        base.Restore();
        transform.position =
            walkArea.points[0].position; // new Vector3(startPos.x,walkArea.points[0].position.y,startPos.z) ;
        transform.rotation = _initialRotation;
        Respawn();
    }

    public async void Respawn()
    {
        if (castumGravity != null)
        {
            StopCoroutine(castumGravity);
            castumGravity = null;
        }
        if (isRespawnBlock)
        {
            return;
        }
        if (PlayerInEndRange(player.transform.position))
        {
            playerInRange = false;
        }
        else
        {
            playerInRange = true;
        }
        transform.GetChild(0).gameObject.SetActive(true);
        gameObject.SetActive(true);
        await Task.Delay(100);
        if (!PlayerInStartRange(player.transform.position))
        {
            if (!playerInRange)
            {
                PlayerEnterOnRange();
                playerInRange = true;
            }
        }
        Debug.Log("Respawn Turtle...");
        base.ToggleSound(1, true, SoundManager.Soundtype.turtle);
        health = maxHealth;
        isDead = false;
        isRemoved = false;
        _sphereCollider.enabled = false;
        _capsuleCollider.enabled = true;
        _sphereCollider.isTrigger = true;
        await Task.Delay(1000);
        _boxCollider.enabled = true;
        GameManager.instance.StopVFXImediatly(turtleKickTrailVFX, 1000);

        Invoke("ResumeChecker", 6f);
    }

    public void ResumeChecker()
    {
        enemyoutOfBoundCoroutine = StartCoroutine(GroundCheckerCoroutine());
    }
    
    public void ToggleTurtleSleepState(bool sleep)
    {
        if (sleep)
        {
            isRespawnBlock = true;
            isDead = true;
        }
        else
        {
            isRespawnBlock = false;
        }
    }

    public bool DeadState()
    {
        return isDead;
    }

    #endregion

    #region PRIVATE_FUNCTIONS

    #endregion

    #region CO-ROUTINES

    public IEnumerator WakeUp()
    {
        yield return new WaitForSeconds(wakeUpTime);
        if (!isRemoved)
        {
            if (!PlayerInEndRange(transform.position))
            {
                StartCoroutine(WakeUp());
            }
            else
            {
                if (isDead)
                {
                    Respawn();
                }
            }
        }
        wakeUpCo = null;
    }

    public IEnumerator ForceBack()
    {
        isForceBackContinue = true;
        float time = 0,mul = -2;
        while (rb.velocity.magnitude >3)
        {
            yield return null;
            rb.AddForce(rb.velocity.normalized*mul*rb.velocity.magnitude);
            mul *= 1.1f;
        }        
        isForceBackContinue = false;
    }

    public IEnumerator EnemyCheck(TurtleEnemy enemy)
    {
        float time = 2.5f;
        while (time>0)
        {
            yield return null;
            time -= Time.deltaTime;
            if(enemy.rb.velocity.magnitude>4.5)
            {
                Damage(health);
                break;
            }
            if(enemy.rb.velocity.magnitude<=0.3)
            {
                break;
            }
        }
        enemyCollisionCo = null;
    }
    
    public IEnumerator FixMovement()
    {
        yield return rotateCo=StartCoroutine(Rotate(GetMoveingAngle(transform.position,targetPoint),true));
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