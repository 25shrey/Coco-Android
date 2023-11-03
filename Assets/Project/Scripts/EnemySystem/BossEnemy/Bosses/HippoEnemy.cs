using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using GameCoreFramework;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.VFX;
using Random = UnityEngine.Random;
using Sequence = DG.Tweening.Sequence;

public class HippoEnemy : BossBase
{

    public Vector3 rotationDiretion;
    public float ballRotationSpeed;
    
    [Header("Boss Status")]
    private HippoState currentBossState;

    [Header("Boss Body Parameters")]
    public CapsuleCollider bodyCollider;
    public SphereCollider ballCollider;
    
    [Header("Boss Entry Parameters")]
    [SerializeField] private Transform bossEntryTargetTransform;
    private Vector3 bossEntryTargetPos;

    [Header("Boss SuperAttack Parameters")]
    [SerializeField] private BaseEnemy spawnEnemyPrefab;
    [SerializeField] private Transform startRange;
    [SerializeField] private Transform endRange;
    [SerializeField] private List<Transform> spawnPoints;
    private List<BaseEnemy> spawnedEnemies;
    
    [Header("Boss Animation parameters")]
    private int idleAnimation1;
    private int idleAnimation2;
    private int idleAnimation3;
    private int walkAnimation;
    private int runAnimation;
    private int superAttackAnimation;
    private int damageAnimation;
    private int deathAnimation;
    private int rebounceBallAnimation;
    private int hippoBallAnimation;
    private int BallToHippoAnimation;
    private int AngryAnimation;
    private int JumpAnimation;

    [Header("Boss Attack parameters")]
    [SerializeField] private int attackLevel;
    [SerializeField] private float AttackSpeed;
    [SerializeField] private float minimumBallAttackDistance;
    [SerializeField] private float maximumRebounceAttackDistance;
    [SerializeField] private float minimumSuperAttackDistance;
    [SerializeField] private float maximumJumpAttackDistance;
    [SerializeField] private float minAttackTime;
    [SerializeField] private float maxAttackTime;
    [SerializeField] private float spawnDistance;
    [SerializeField] private HippoState lastAttack;
    [SerializeField] private PlayerDemageObject playerRingDamageObject;
    [SerializeField] private PlayerDemageObject playerCubeDamageObject;
    [SerializeField] private BossHealthBar healthBar;
    private int normalAttackCount;

    [Header("Boss Visual Effect")]
    [SerializeField] private VisualEffect angryVFX;
    [SerializeField] private VisualEffect jumpAttackVFX;
    [SerializeField] private VisualEffect reBounceVFX;
    [SerializeField] private VisualEffect shockWaveVFX;
    [SerializeField] private VisualEffect ringAttackVFX;
    [SerializeField] private Animation hippoFresnelEffect;
    [SerializeField] private Transform ringAttackVFXTransform;

    [Header("Armadillo Ball Parameters")]
    [SerializeField] private float armadilloMovementTime;
    [SerializeField] private GameObject armadilloBall;
    [SerializeField] private Transform armadilloBallAnchor;
    [SerializeField] private Transform spawnPoint;

    protected override void OnEnable()
    {
        enemyAnimationTrigger.OnAnimationEvenTrigger1 += SpawnEnemies;
        enemyAnimationTrigger.hippoReBounceVFXEventTrigger += ReBounceVFXEffect;
        enemyAnimationTrigger.hippoShockWaveVFXEventTrigger += ShockWaveVFXEffect;
        enemyAnimationTrigger.hippoRingAttackVFXEventTrigger += RingAttackVFXEffect;
        enemyAnimationTrigger.OnHippoArmadilloThrow += ThrowArmadilloBall;
    }

    protected override void OnDisable()
    {
        enemyAnimationTrigger.OnAnimationEvenTrigger1 -= SpawnEnemies;
        enemyAnimationTrigger.hippoReBounceVFXEventTrigger -= ReBounceVFXEffect;
        enemyAnimationTrigger.hippoShockWaveVFXEventTrigger -= ShockWaveVFXEffect;
        enemyAnimationTrigger.hippoRingAttackVFXEventTrigger -= RingAttackVFXEffect;
        enemyAnimationTrigger.OnHippoArmadilloThrow -= ThrowArmadilloBall;
    }

    public override void Start()
    {
        healthBar = BossEnemyManager.Instance.healthBar;
        healthBar.SetBar();
        spawnedEnemies = new List<BaseEnemy>();
        bossEntryTargetPos = bossEntryTargetTransform.position;
        SetUp();
        bodyCollider.enabled = true;
        ballCollider.enabled = false;
        rb.constraints = RigidbodyConstraints.FreezeAll;
        SetupAnimation();
        GameManager.instance.input.EnableInput();
        MenuInput.EnableUiInput();
        StartCoroutine(SetLookingAtPlayer());
        BossEntry();
        attackLevel = 1;
    }

    #region Collision

    
    private void OnCollisionEnter(Collision collision)
    {
        TurtleEnemy collisionEnemy = collision.gameObject.GetComponent<TurtleEnemy>();
        if (DeadEnemyCanDamage(collisionEnemy))
        {
             StartCoroutine(EnemyCheck(collisionEnemy));
        }
        else if(PlayerCanDamage(collision.gameObject.GetComponent<Player>()))
        {
            float power,damageValue;
            DamageAnimType type;
            if (currentBossState == HippoState.BallAttack)
            {
                Debug.Log("Player Hit....");
                damageValue = damagePower;
                power = 3.5f;
                type = DamageAnimType.BossDamage;
            }
            else
            {
                type = DamageAnimType.Damage;
                damageValue = damagePower/2;
                power = 1.25f;
            }
            Vector3 dir = (player.transform.position - transform.position).normalized;
            player.Damage(dir * power, type, damageValue);
        }
    }
    
    public void OnTriggerStay(Collider other)
    {
        // if (PlayerCanDamage(other.gameObject.GetComponent<Player>()) && currentState==BullState.IdelAnimation && isPunchAttackUnlock && damageAnimCo== null)
        // {
        //     if (Random.Range(0, 4) <= 1)
        //     {
        //         AttackAnimation();
        //     }
        //     else
        //     {
        //         isPunchAttackUnlock = false;
        //         StartCoroutine(ReloadingPunchAttack(false));
        //     }
        // }
        // else if(currentBossState == HippoState.BallAttack && PlayerCanDamage(other.gameObject.GetComponent<Player>()))
        // {
        //     Debug.Log("Player Hit OnTriggerStay....");
        //     Vector3 dir = (player.transform.position-transform.position).normalized;
        //     player.Damage(dir*3.5f,DamageAnimType.BossDamage,damagePower);
        // }
    }

    #endregion


    protected override void SetupAnimation()
    {
        idleAnimation1 = Animator.StringToHash("Idle1");
        idleAnimation2 = Animator.StringToHash("Idle2");
        idleAnimation3 = Animator.StringToHash("Idle3");
        walkAnimation = Animator.StringToHash("Walk");
        runAnimation = Animator.StringToHash("Run");
        superAttackAnimation = Animator.StringToHash("SuperAttack");
        damageAnimation = Animator.StringToHash("Damage");
        deathAnimation = Animator.StringToHash("Death");
        rebounceBallAnimation = Animator.StringToHash("RebounceBall");
        hippoBallAnimation = Animator.StringToHash("HippoBall");
        BallToHippoAnimation = Animator.StringToHash("BallToHippo");
        AngryAnimation = Animator.StringToHash("Angry");
        JumpAnimation = Animator.StringToHash("Jump");
    }

    protected override void BossEntry()
    {
        currentBossState = HippoState.Entry;
        PlayIdleAnimation();
        StartCoroutine(AttackTimer());
        isLookingPlayer = true;
        //BossArenaManage.Instance.ShowStartBossCinematics(BossCinematicsShowCaseType.EntryCinematics);
        //animator.SetTrigger(walkAnimation);
        //StartCoroutine(MoveBossTowardsTarget(bossEntryTargetPos));
    }


    public override void Damage(float damageValue, bool attackByJump = false)
    {
        if (!CanDamage())
        {
            return;
        }
        base.Damage(damageValue);
        healthBar.UpdateBar(health/maxHealth);
        if (health < 250 && attackLevel ==3)
        {
            attackLevel = 4;
        }
        else if(health<500 && attackLevel == 2)
        {
            attackLevel = 3;
        }
        else if(health<750 && attackLevel ==1)
        {
            attackLevel = 2;
        }
        if (!isDead)
        {
            if (damageAnimCo != null)
            {
                StopCoroutine(damageAnimCo);
            }
            damageAnimCo = StartCoroutine(DamageAnimation());
        }
    }

    public IEnumerator DamageAnimation()
    {
        base.ToggleSound(2, false, SoundManager.Soundtype.hippoBoss);
        currentBossState = HippoState.Damage;
        animator.SetTrigger(damageAnimation);
        yield return StartCoroutine(GameManager.instance.CheckForAnimationCompletion(()=>
        {
            if (!isDead)
            {
                PlayIdleAnimation();
            }
        }, animator, 0));
        damageAnimCo = null;
    }

    
    private bool CanDamage()
    {
        return currentBossState == HippoState.Idle;
    }

    private bool DeadEnemyCanDamage(TurtleEnemy collisionEnemy)
    {
        return collisionEnemy && collisionEnemy.isDead;
    }

    public override async void Kill()
    {
        if (!isDead)
        {
            BossCinematicsManager.Instance.BossAudioSetter(SoundManager._soundManager.hippo[3]);
            //base.ToggleSound(3, false, SoundManager.Soundtype.hippoBoss);
           // BossArenaManage.Instance.SwitchListener(BossArenaManage.Listener.Camera);
            BossArenaManage.Instance.BGMusicSwitcher(false);
            isLookingPlayer = false;
            currentBossState = HippoState.Death;
           // GameManager.instance.currentGameState = GameStates.PlayerPaused;
            BossCinematicsManager.Instance.PlayBossCinematics(BossCinematicsShowCaseType.EndCinematics);
            player.bossFightEnd();
            isDead = true;
            rb.constraints = RigidbodyConstraints.FreezeAll;
            animator.SetTrigger(deathAnimation);
            
            if (rotateCo != null)
            {
                StopCoroutine(rotateCo);
            }
            if (castumGravity != null)
            {
                StopCoroutine(castumGravity);
            }
            await Task.Delay(1000);
            transform.DOMove(transform.position - transform.right*6.5f, 1.6f);
            base.Kill();
            await Task.Delay(100);
            bodyCollider.enabled = false;
            for (int i = 0; i < spawnedEnemies.Count; i++)
            {
                if (spawnedEnemies[i] != null && !spawnedEnemies[i].isDead)
                {
                    await Task.Delay(200);
                    spawnedEnemies[i].Kill();
                }
            }
            await Task.Delay(3000);
          //  BossArenaManage.Instance.SwitchListener(BossArenaManage.Listener.Coco);
            BossArenaManage.Instance.SetPathInBullArena();
        }
    }

    public async void ShowAngryAnimation(Action OnComplite)
    {
        base.ToggleSound(0, false, SoundManager.Soundtype.hippoBoss);
        currentBossState = HippoState.Angry;
        animator.SetTrigger(AngryAnimation);
        await Task.Delay(1200);
        GameManager.instance.PlayVFX(angryVFX);
        CameraController.Instance.ShakeCamera(0.05f, 0.5f);
        StartCoroutine(GameManager.instance.CheckForAnimationCompletion(OnComplite, animator, 0));
        GameManager.instance.StopVFX(angryVFX, 5000);
    }

    public async void ShowJumpAttackAnimation()
    {
        base.ToggleSound(4, false, SoundManager.Soundtype.hippoBoss);
        currentBossState = HippoState.Jump;
        animator.SetTrigger(JumpAnimation);
        await Task.Delay(1000);
        GameManager.instance.PlayVFX(jumpAttackVFX);
        RingAttack(new Vector3(25, 25, 25), 0.6f);
        await Task.Delay(810);
        playerRingDamageObject.gameObject.SetActive(false);
        StartCoroutine(GameManager.instance.CheckForAnimationCompletion(() =>
        {
            PlayIdleAnimation();
        }, animator, 0));
    }
    
    public async void RebounceAnimation()
    {
        Debug.Log("----RebounceAnimation");
        bodyCollider.enabled = false;
        currentBossState = HippoState.ReBounce;
        hippoFresnelEffect.Play();
        animator.SetTrigger(rebounceBallAnimation);
        await Task.Delay(1500);
        base.ToggleSound(5, false, SoundManager.Soundtype.hippoBoss);
        CubeAttack(100,4f);
        await Task.Delay(1510);
        playerCubeDamageObject.gameObject.SetActive(false);
        ballCollider.enabled = true;
        StartCoroutine(GameManager.instance.CheckForAnimationCompletion(()=>
        {
            bodyCollider.enabled = true;
            ballCollider.enabled = false;
            PlayIdleAnimation();
        }, animator, 0));
    }

    public void ReBounceVFXEffect()
    {
        CameraController.Instance.ShakeCamera(0.05f, 0.5f);
        Debug.Log("----ReBounceVFXEffect");
        reBounceVFX.Play();
    }
    public void ShockWaveVFXEffect()
    {
        Debug.Log("----ShockWaveVFXEffect");
        shockWaveVFX.Play();
    }
    public void RingAttackVFXEffect()
    {
        ringAttackVFX.transform.position = ringAttackVFXTransform.position;
        ringAttackVFX.transform.eulerAngles = ringAttackVFXTransform.eulerAngles;
        Debug.Log("----RingAttackVFXEffect");
        ringAttackVFX.Play();
    }
    
    private async void SuperAttackAnimation()
    {
        base.ToggleSound(7, false, SoundManager.Soundtype.hippoBoss);
        currentBossState = HippoState.SuperAttack;
        animator.SetTrigger(superAttackAnimation);
        await Task.Delay(500);
        armadilloBall.SetActive(true);
        StartCoroutine(GameManager.instance.CheckForAnimationCompletion(()=>
        {
            PlayIdleAnimation();
            GameManager.instance.StopVFX(angryVFX);
        }, animator, 0));
    }

    private async void ThrowArmadilloBall()
    {
        armadilloBall.transform.SetParent(null);
        await ThrowArmadilloBall(armadilloBall.transform, spawnPoint, armadilloMovementTime ,() =>
        {
            armadilloBall.transform.SetParent(armadilloBallAnchor);
            armadilloBall.transform.localPosition = Vector3.zero;
            armadilloBall.transform.localRotation = Quaternion.identity;
        });
    }

    private async Task ThrowArmadilloBall(Transform currentObject,Transform endPos, float duration ,Action Callback)
    {
        Tween rotateTween = currentObject.DORotate(new Vector3(0, 0, 360f), 0.5f);
        rotateTween.SetLoops(-1);
        Sequence tweenSequence = DOTween.Sequence();
        Tween tween = currentObject.DOMove(endPos.position, duration);
        Task task = tweenSequence.Append(tween).InsertCallback(0.75f, SpawnEnemies).AsyncWaitForCompletion();
        while(true)
        {
            if (!task.IsCompleted)
            {
                await Task.Yield();
            }
            else
            {
                rotateTween.Kill();
                Callback();
                break;
            }
        }
    }
    
    private async void HippoBallAttackAnimation()
    {
        base.ToggleSound(6, false, SoundManager.Soundtype.hippoBoss);
        currentBossState = HippoState.BallAttack;
        animator.SetTrigger(hippoBallAnimation);
        await Task.Delay(1400);
        transform.DOMove(transform.position + new Vector3(0, 10, 0), 0.55f);
        StartCoroutine(GameManager.instance.CheckForAnimationCompletion(()=>
        {
            StartCoroutine(BallAttack());
        }, animator, 0));
    }

    private async void SpawnEnemies()
    {
        List<BaseEnemy> baseEnemies = new List<BaseEnemy>();
        for (int index = 0; index <1; index++)
        {
            //transform.position + transform.right * spawnDistance
            //baseEnemies.Add(Instantiate(spawnEnemyPrefab, spawnPoint.position, spawnPoint.rotation));
            ArmadilloEnemy armadilloEnemy = Instantiate(spawnEnemyPrefab, spawnPoint.position, spawnPoint.rotation) as ArmadilloEnemy;
            armadilloEnemy.walkArea.points[0].point.position = spawnPoint.position;
            armadilloEnemy.OnSpawnedByBoss(() => armadilloBall.SetActive(false));
            baseEnemies.Add(armadilloEnemy);
        }
        await Task.Delay(50);
        for (int i = 0; i < baseEnemies.Count; i++)
        {
            baseEnemies[i].SetRange(startRange.position, endRange.position);
            baseEnemies[i].isRespawnBlock = true;
        }
        spawnedEnemies.AddRange(baseEnemies);
    }

    

    private void PlayIdleAnimation()
    {
        currentBossState = HippoState.Idle;
        switch (Random.Range(1, 4))
        {
            case 1:
                animator.SetTrigger(idleAnimation1);
                break;
            case 2:
                animator.SetTrigger(idleAnimation2);
                break;
            case 3:
                animator.SetTrigger(idleAnimation3);
                break;
        }
    }

    private IEnumerator AttackTimer()
    {
        float reloadTime = Random.Range(minAttackTime,maxAttackTime)/2;
        normalAttackCount = 4;
        while (!isDead)
        {
            if (reloadTime > 0)
            {
                reloadTime-=Time.deltaTime;
                if (Vector3.Distance(transform.position, player.transform.position) < maximumJumpAttackDistance)
                {
                    reloadTime-=Time.deltaTime;
                }
                yield return null;
            }
            else if (currentBossState == HippoState.Idle && attackLevel > 0)
            {
                yield return StartCoroutine(SelectAttack());
                reloadTime =  Random.Range(minAttackTime,maxAttackTime) + 2.2f;// animation Time...
            }
            else
            {
                yield return null;
            }
        }
    }

    public IEnumerator SelectAttack()
    {
        List<HippoState> attacks = new List<HippoState>();
        if (Vector3.Distance(transform.position, player.transform.position) > minimumSuperAttackDistance)
        {
            attacks.Add(HippoState.SuperAttack);
        }

        if (Vector3.Distance(transform.position, player.transform.position) > minimumBallAttackDistance)
        {
            attacks.Add(HippoState.BallAttack);
        }

        if (Vector3.Distance(transform.position, player.transform.position) < maximumRebounceAttackDistance)
        {
            attacks.Add(HippoState.ReBounce);
        }

        if (Vector3.Distance(transform.position, player.transform.position) < maximumJumpAttackDistance)
        {
            attacks.Add(HippoState.Jump);
        }

        if (attacks.Count > 1)
        {
            attacks.Remove(lastAttack);
        }

        if (attacks.Count > 1 && normalAttackCount < 5)
        {
            attacks.Remove(HippoState.SuperAttack);
        }

        HippoState attack = attacks[Random.Range(0, attacks.Count)];
        if (normalAttackCount > 7 && attacks.Contains(HippoState.SuperAttack))
        {
            attack = HippoState.SuperAttack;
        }

        lastAttack = attack;
        switch (attack)
        {
            case HippoState.SuperAttack:
                ShowAngryAnimation(SuperAttackAnimation);
                normalAttackCount = 0;
                yield return new WaitForSeconds(2);
                break;
            case HippoState.BallAttack:
                HippoBallAttackAnimation();
                normalAttackCount++;
                break;
            case HippoState.ReBounce:
                RebounceAnimation();
                normalAttackCount++;
                break;
            case HippoState.Jump:
                ShowJumpAttackAnimation();
                normalAttackCount++;
                break;
            default:
                Debug.Log("No Attack Found");
                break;
        }
    }


    private IEnumerator MoveBossTowardsTarget(Vector3 target)
    {
        animator.SetTrigger(walkAnimation);
        yield return moveCo = StartCoroutine(Move(new Vector3(target.x, transform.position.y,target.z)));
        ShowAngryAnimation(() =>
        {
            OnEntryCinematicsCompleted();
        });
    }

    private IEnumerator BallAttack()
    {
        bodyCollider.enabled = false;            
        ballCollider.enabled = true;
        isLookingPlayer = false;
        Vector3 start = transform.position, end = player.transform.position;
        float totalTime = Vector3.Distance(start,end)/AttackSpeed;
        Debug.Log(totalTime);
        float time = totalTime;
        Vector3 startAngle = animator.transform.parent.localEulerAngles;
        while (time>0)
        {
            yield return null;
            time -= Time.deltaTime;
            if (time > totalTime / 2f)
            {
                end = player.transform.position;
            }
            transform.position = Vector3.Lerp(start,end,1-time/totalTime);
            animator.transform.parent.localEulerAngles += rotationDiretion.normalized*ballRotationSpeed;
        }
        time = totalTime;
        while (time>0)
        {
            yield return null;
            time -= Time.deltaTime;
            transform.position = Vector3.Lerp(end,start,1-time/totalTime);
            animator.transform.parent.localEulerAngles += rotationDiretion.normalized*ballRotationSpeed*-1;
        }
        animator.transform.parent.localEulerAngles = startAngle;
        animator.SetTrigger(BallToHippoAnimation);
        yield return new WaitForSeconds(0.2f);
        transform.DOMove(transform.position - new Vector3(0, 10, 0), 0.55f);
        StartCoroutine(GameManager.instance.CheckForAnimationCompletion(()=>
        {
            bodyCollider.enabled = true;
            ballCollider.enabled = false;
            isLookingPlayer = true;
            PlayIdleAnimation();
        }, animator, 0));
    }

    public void RingAttack(Vector3 target,float time)
    {
        playerRingDamageObject.transform.localScale = new Vector3(2, 2, 2);
        playerRingDamageObject.gameObject.SetActive(true);
        playerRingDamageObject.transform.DOScale(target, time);
    }
    
    public void CubeAttack(float target,float time)
    {
        playerCubeDamageObject.transform.localScale = new Vector3(1, 15, 6);
        playerCubeDamageObject.gameObject.SetActive(true);
        playerCubeDamageObject.transform.DOScaleX(target, time);
    }

    public void OnEntryCinematicsStart()
    {
        BossEntry();
    }

    public void OnEntryCinematicsCompleted()
    {
        
    }
    
}

public enum HippoState
{
    Entry,
    Idle,
    SuperAttack,
    ReBounce,
    BallAttack,
    Damage,
    Death,
    Angry,
    Jump,
}
