using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;
using Random = UnityEngine.Random;
using Task = System.Threading.Tasks.Task;

public class DragonEnemy : BossBase
{
    #region VARS

    [Header("Boss Status")]
    private DragonState currentBossState;

    [Header("Boss Body Parameters")]
    public CapsuleCollider bodyCollider;
    public List<CapsuleCollider> bodyColliders;
    
    [Header("Boss Entry Parameters")]
    [SerializeField] private List<Transform> entryPoints;
    private Vector3 bossEntryTargetPos;

    [Header("Boss SuperAttack Parameters")]
    [SerializeField] protected DragonAnimationTrigger animationTrigger;
    [SerializeField] private BaseEnemy spawnEnemyPrefab;
    [SerializeField] private float spawnDistance;
    [SerializeField] private Transform startRange;
    [SerializeField] private Transform endRange;
    [SerializeField] private List<Transform> spawnPoints;
    [SerializeField] private BossHealthBar healthBar;
    private List<BaseEnemy> spawnedEnemies = new List<BaseEnemy>();
    
    [Header("Boss Animation parameters")]
    private int idleAnimation1;
    private int idleAnimation2;
    private int idleAnimation3;
    private int flyIdleAnimation;
    private int walkAnimation;
    private int groundAttackAnimation;
    private int flyAttackAnimation;
    private int lendAnimation;
    private int teckoffAnimation;
    private int damageAnimation;
    private int flyDamageAnimation;
    private int deathAnimation;
    private int spawnAttackAnimation;
    private int tailAttackAnimation;

    [Header("Boss Attack parameters")]
    [SerializeField] private float minAttackTime;
    [SerializeField] private float maxAttackTime;
    [SerializeField] private float maxTailAttackDistance;
    private int contiuneAttackCount;
    private int contiuneNormalAttackCount;

    [Header("Dragon Flying Parameters")] 
    [SerializeField] private float flyingSpeed;

    [Header("Dragon VFX Assets")] 
    [SerializeField] private GameObject trailVFXParent;
    [SerializeField] private GameObject earthShatterObject;
    [SerializeField] private VisualEffect earthShatterVFX;
    [SerializeField] private Transform earthShatterReference;
    [SerializeField] private GameObject tornadoVFXObject;
    [SerializeField] private GameObject tornadoObjectParent;
    [SerializeField] private Transform flyAttackReference;
    [SerializeField] private List<VisualEffect> beeSpawnVFX;

    [Header("Fire Breath parameters")] 
    [SerializeField] private float sphereCastRadius;
    [SerializeField] private float maxRaycastDistance;
    [SerializeField] private float fireBreathDamage;
    [SerializeField] private LayerMask cocoLayerMask;
    [SerializeField] private Transform fireAnchor;
    [SerializeField] private VisualEffect dragonBreathVFX;
    [SerializeField] private Collider trailVFXCollider;
    private bool isThrowingFire = false;
    #endregion

    #region UNITY_CALLBACKS
    
    
    protected override void OnEnable()
    {
        animationTrigger.OnAnimationEvenTrigger1 += SpawnEnemies;
        animationTrigger.earthShatterTrigger += EarthShatterVFXEffect;
        animationTrigger.trailAttackTrigger += DisableTrailEffect;
        animationTrigger.groundAttackTrigger += PlayGroundAttackVFX;
        animationTrigger.groundAttackTriggerStop += StopGroundAttackVFX;
    }

    protected override void OnDisable()
    {
        animationTrigger.OnAnimationEvenTrigger1 -= SpawnEnemies;
        animationTrigger.earthShatterTrigger -= EarthShatterVFXEffect;
        animationTrigger.trailAttackTrigger -= DisableTrailEffect;
        animationTrigger.groundAttackTrigger -= PlayGroundAttackVFX;
        animationTrigger.groundAttackTriggerStop -= StopGroundAttackVFX;
    }

    void Start()
    {
        healthBar = BossEnemyManager.Instance.healthBar;
        healthBar.SetBar();
        //transform.position = entryPoints[0].position;
        SetUp();
        rb.constraints = RigidbodyConstraints.FreezeAll;
        idleAnimation1 = Animator.StringToHash("Idle1");
        idleAnimation2 = Animator.StringToHash("Idle2");
        idleAnimation3 = Animator.StringToHash("Idle3");
        flyIdleAnimation = Animator.StringToHash("FlyIdle");
        walkAnimation = Animator.StringToHash("Walk");
        groundAttackAnimation = Animator.StringToHash("GroundAttack");
        flyAttackAnimation = Animator.StringToHash("FlyAttack");
        lendAnimation = Animator.StringToHash("Lend");
        teckoffAnimation = Animator.StringToHash("Teckoff");
        damageAnimation = Animator.StringToHash("Damage");
        flyDamageAnimation = Animator.StringToHash("FlyDamage");
        deathAnimation = Animator.StringToHash("Death");
        spawnAttackAnimation = Animator.StringToHash("SpawnBee");
        tailAttackAnimation = Animator.StringToHash("TailAttack");
        BossEntry();
        StartCoroutine(SetLookingAtPlayer());
        GameManager.instance.input.EnableInput();
        MenuInput.EnableUiInput();
    }

    private void Update()
    {
        if (isThrowingFire)
        {
            RaycastHit[] hits;
            hits = Physics.SphereCastAll(fireAnchor.position, sphereCastRadius, fireAnchor.right, maxRaycastDistance, cocoLayerMask);
            for (int index = 0; index < hits.Length; index++)
            {
                Player player = hits[index].transform.GetComponent<Player>();
                if (player != null)
                {
                    player.Damage(fireAnchor.right, DamageAnimType.BossDamage, fireBreathDamage);
                }
            }
        }
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if(PlayerCanDamage(collision.gameObject.GetComponent<Player>()))
        {
            Debug.Log("Player Hit OnCollisionEnter....");
            Vector3 dir = (player.transform.position-transform.position).normalized;
            if (currentBossState == DragonState.TailAttack)
            {
                player.Damage(dir * 3, DamageAnimType.Damage, damagePower);
            }
            else
            {
                player.Damage(dir * 2.5f, DamageAnimType.Damage, 20);
            }
        }
    }

    

    #endregion

    #region PUBLIC_FUNCTIONS
    
    public override void Damage(float damageValue, bool attackByJump = false)
    {
        if (!CanDamage())
        {
            return;
        }
        base.Damage(damageValue);
        healthBar.UpdateBar(health/maxHealth);
        if (!isDead)
        {
            DamageAnimation();
        }
    }

    public override async void Kill()
    {
        if (!isDead)
        {
            BossCinematicsManager.Instance.BossAudioSetter(SoundManager._soundManager.dragon[2]);
            isLookingPlayer = false;
            currentBossState = DragonState.Death;
           // BossArenaManage.Instance.SwitchListener(BossArenaManage.Listener.Camera);
            BossArenaManage.Instance.BGMusicSwitcher(false);
            // GameManager.instance.currentGameState = GameStates.PlayerPaused;
            BossCinematicsManager.Instance.PlayBossCinematics(BossCinematicsShowCaseType.EndCinematics);
            player.bossFightEnd();
            isDead = true;
            rb.constraints = RigidbodyConstraints.FreezeAll;
            StartCoroutine(GameManager.instance.CheckForAnimationCompletion(()=>
            {
                animator.speed = 0.6f;
                animator.SetTrigger(deathAnimation);
            }, animator, 0));
           // base.ToggleSound(2, false, SoundManager.Soundtype.DragonBoss);
            if (rotateCo != null)
            {
                StopCoroutine(rotateCo);
            }
            if (castumGravity != null)
            {
                StopCoroutine(castumGravity);
            }
            base.Kill();
            await Task.Delay(100);
            //bodyCollider.enabled = false;
            foreach (CapsuleCollider collider in bodyColliders)
            {
                collider.enabled = false;
            }
            for (int i = 0; i < spawnedEnemies.Count; i++)
            {
                if (spawnedEnemies[i] != null && !spawnedEnemies[i].isDead)
                {
                    await Task.Delay(200);
                    spawnedEnemies[i].Kill();
                }
            }
            await Task.Delay(3000);
            BossArenaManage.Instance.SetPathInBullArena();
           // BossArenaManage.Instance.SwitchListener(BossArenaManage.Listener.Coco);
            healthBar.ResetBar();
        }
    }
    
    private void DamageAnimation()
    {
        base.ToggleSound(1, false, SoundManager.Soundtype.DragonBoss);
        if (currentBossState == DragonState.FlyIdle)
        {
            animator.SetTrigger(flyDamageAnimation);
            StartCoroutine(GameManager.instance.CheckForAnimationCompletion(PlayFlyIdleAnimation, animator, 0));
        }
        else
        {
            animator.SetTrigger(damageAnimation);
            StartCoroutine(GameManager.instance.CheckForAnimationCompletion(()=>
            {
                PlayGroundIdleAnimation();
            }, animator, 0));
        }
        currentBossState = DragonState.Damage;
    }

    protected override bool CanDamage()
    {
        return currentBossState == DragonState.FlyIdle || currentBossState == DragonState.GroundIdle;
    }

    #endregion

    #region PRIVATE_FUNCTIONS

    private DragonState privise;
    
    private async void SuperAttack()
    {
        privise = currentBossState;
        currentBossState = DragonState.SuperAttack;
        animator.SetTrigger(spawnAttackAnimation);
        base.ToggleSound(4, false, SoundManager.Soundtype.DragonBoss);
        StartCoroutine(GameManager.instance.CheckForAnimationCompletion(SuperAttackAnimationEnd, animator, 0));
        await Task.Delay(700);
        SpawnEnemies();
    }

    private void SuperAttackAnimationEnd()
    {
        if (privise == DragonState.FlyIdle)
        {
            PlayFlyIdleAnimation();
        }
        else
        {
            PlayGroundIdleAnimation();
        }
    }
    
    private async void FlyAttack()
    {
        currentBossState = DragonState.FlyAttack;
        animator.SetTrigger(flyAttackAnimation);
        StartCoroutine(GameManager.instance.CheckForAnimationCompletion(PlayFlyIdleAnimation, animator, 0));
        tornadoObjectParent.transform.position = new Vector3(flyAttackReference.position.x,55.65f,flyAttackReference.position.z); //47.651 Y
        tornadoObjectParent.transform.eulerAngles = new Vector3(0f,flyAttackReference.eulerAngles.y,0f);
        Debug.Log("----FlyAttackVFXEffect");
        tornadoVFXObject.SetActive(true);
        await Task.Delay(500);
        tornadoVFXObject.transform.DOLocalMoveX(40, 2.5f).OnComplete(() =>
        {
            tornadoVFXObject.SetActive(false);
            tornadoVFXObject.transform.localPosition=Vector3.zero;
        });
    }

    private void GroundAttack()
    {
        currentBossState = DragonState.GroundAttack;
        animator.SetTrigger(groundAttackAnimation);
        base.ToggleSound(3, true, SoundManager.Soundtype.DragonBoss);
        StartCoroutine(GameManager.instance.CheckForAnimationCompletion(PlayGroundIdleAnimation, animator, 0));
    }

    private void TailAttack()
    {
        currentBossState = DragonState.TailAttack;
        trailVFXParent.SetActive(true);
        animator.SetTrigger(tailAttackAnimation);
        base.ToggleSound(7, true, SoundManager.Soundtype.DragonBoss);
        StartCoroutine(GameManager.instance.CheckForAnimationCompletion(PlayGroundIdleAnimation, animator, 0));
        //await Task.Delay(1080);
        /*earthShatterObject.transform.position = earthShatterReference.position;
        earthShatterObject.transform.eulerAngles = earthShatterReference.eulerAngles;
        Debug.Log("----TrailAttackVFXEffect");
        earthShatterVFX.Play();*/
        //await Task.Delay(250);
        //trailVFXParent.SetActive(false);
    }

    private async void EarthShatterVFXEffect()
    {
        earthShatterObject.transform.position = new Vector3(earthShatterReference.position.x,47.651f,earthShatterReference.position.z); //47.651 Y
        earthShatterObject.transform.eulerAngles = new Vector3(0f,earthShatterReference.eulerAngles.y,0f);
        Debug.Log("----TrailAttackVFXEffect");
        earthShatterVFX.Play();
        trailVFXCollider.enabled = true;
        await Task.Delay(1500);
        trailVFXCollider.enabled = false;
    }
    
    private void DisableTrailEffect()
    {
        trailVFXParent.SetActive(false);
    }
    
    private async void Teckoff()
    {
        contiuneAttackCount = 0;
        currentBossState = DragonState.Teckoff;
        animator.SetTrigger(teckoffAnimation);
        base.ToggleSound(4, true, SoundManager.Soundtype.DragonBoss);
        await Task.Delay(200);
        transform.DOMove(transform.position + new Vector3(0, 6.25f, 0), 1.45f).SetEase(Ease.OutSine);
        StartCoroutine(GameManager.instance.CheckForAnimationCompletion(PlayFlyIdleAnimation, animator, 0));
    }

    private void Landing()
    {        
        contiuneAttackCount = 0;
        currentBossState = DragonState.Landing;
        animator.SetTrigger(lendAnimation);
        base.ToggleSound(6, false, SoundManager.Soundtype.DragonBoss);
        transform.DOMove(transform.position + new Vector3(0, -6.25f, 0), 1.4f).SetEase(Ease.InCubic);
        StartCoroutine(GameManager.instance.CheckForAnimationCompletion(()=>
        {
            PlayGroundIdleAnimation();
        }, animator, 0));
    }

    private void PlayGroundIdleAnimation()
    {
        currentBossState = DragonState.GroundIdle; 
        animator.SetTrigger(idleAnimation1);
        IdleSound();
    }

    async void IdleSound()
    {
        await Task.Delay(500);

        base.ToggleSound(5, false, SoundManager.Soundtype.DragonBoss);
    }
    
    private void PlayFlyIdleAnimation()
    {
        currentBossState = DragonState.FlyIdle; 
        animator.SetTrigger(flyIdleAnimation);
        IdleSound();
    }

    private void PlayGroundAttackVFX()
    {
        GameManager.instance.PlayVFX(dragonBreathVFX);
        base.ToggleSound(3, true, SoundManager.Soundtype.DragonBoss);
        isThrowingFire = true;
    }

    private void StopGroundAttackVFX()
    {
        StartCoroutine(ReduceSpawnRateOfFire(dragonBreathVFX, 5f, () =>
        {
            Debug.Log("Completed - Fire Breath!");
            isThrowingFire = false;
            dragonBreathVFX.SetFloat("SpawnRate", 50f);
            GameManager.instance.StopVFXImediatly(dragonBreathVFX);
        }));
        base.ToggleSound(5, true, SoundManager.Soundtype.DragonBoss);
    }

    private IEnumerator ReduceSpawnRateOfFire(VisualEffect vfx, float duration, Action Callback)
    {
        float startValue = vfx.GetFloat("SpawnRate");
        Debug.Log("SpawnRate : " + startValue);
        float elapsedTime = 0f;
        while (true)
        {
            elapsedTime += Time.deltaTime;
            Debug.Log("Spawn Rate : " + elapsedTime);
            vfx.SetFloat("SpawnRate", Mathf.Lerp(startValue, 0, 1 - (elapsedTime / duration)));
            if (elapsedTime <= duration)
            {
                Callback();
                break;
            }
            else
            {
                yield return null;
            }
        }
    }

    protected override void BossEntry()
    {
        currentBossState = DragonState.Entry;
        BossArenaManage.Instance.PlayerBlockSetup();
        PlayGroundIdleAnimation();
        base.ToggleSound(5, false, SoundManager.Soundtype.DragonBoss);
        isLookingPlayer = true;
        StartCoroutine(AttackTimer());
        //animator.SetTrigger(flyIdleAnimation);
        //StartCoroutine(MoveBossTowardsTarget());
    }
    
    #endregion

    #region CO-ROUTINES

    private IEnumerator MoveBossTowardsTarget()
    {
        
        for (int i = 1; i < entryPoints.Count; i++)
        {
            if (i == entryPoints.Count - 1)
            {
                speed = 18;//Landing speed...
                animator.SetTrigger(lendAnimation);
                base.ToggleSound(6, false, SoundManager.Soundtype.DragonBoss);
                animator.speed = 0.8f;
            }
            yield return moveCo = StartCoroutine(Move(new Vector3(entryPoints[i].position.x, entryPoints[i].position.y,
                entryPoints[i].position.z)));
        }
        animator.speed = 1;
        BossArenaManage.Instance.PlayerBlockSetup();
        PlayGroundIdleAnimation();
        isLookingPlayer = true;
        StartCoroutine(AttackTimer());
    }
    
    public IEnumerator AttackTimer()
    {
        float reloadTime = Random.Range(minAttackTime,maxAttackTime)/2;
        while (!isDead)
        {
            if (reloadTime > 0)
            {
                reloadTime-=Time.deltaTime;
                yield return null;
            }
            else
            {
                if (CanAttack())
                {
                    PlayAttackAnimation();
                    reloadTime = Random.Range(minAttackTime, maxAttackTime) + 2.2f; // animation Time...
                }
                yield return null;
            }
        }
    }

    private bool CanAttack()
    {
        return currentBossState == DragonState.GroundIdle || currentBossState == DragonState.FlyIdle;
    }

    public void PlayAttackAnimation()
    {
        if (currentBossState == DragonState.GroundIdle)
        {
            if (contiuneAttackCount > 3)
            {
                if(Random.Range(0,2)==0 && health>350)
                {
                    Teckoff();
                    return;
                }
            }
            contiuneAttackCount++;
            if (contiuneNormalAttackCount > 3 && Random.Range(0, 2) == 0)
            {
                contiuneNormalAttackCount = 0;
                SuperAttack();//Main Attack
            }
            else if(Vector3.Distance(transform.position, player.transform.position) < maxTailAttackDistance && Random.Range(0, 3) < 2)
            {
                contiuneNormalAttackCount++;
                TailAttack();//Main Attack
                //SuperAttack();
            }
            else
            {
                contiuneNormalAttackCount++;
                GroundAttack();//Main Attack
                //SuperAttack();
            }
        }
        else if(currentBossState == DragonState.FlyIdle)
        {
            if (health < 250)
            {
                Landing();
                return;
            }
            if (contiuneAttackCount > 2)
            {
                if(Random.Range(0,2)==0)
                {
                    Landing();
                    return;
                }
            }
            contiuneAttackCount++;
            contiuneNormalAttackCount++;
            FlyAttack();
        }
    }
    
    private async void SpawnEnemies()
    {
        List<BaseEnemy> baseEnemies = new List<BaseEnemy>();
        for (int i = 0; i < spawnPoints.Count; i++)
        {
            beeSpawnVFX[i].Play();
        }
        await Task.Delay(500);
        for (int i = 0; i < spawnPoints.Count; i++)
        {
            baseEnemies.Add(Instantiate(spawnEnemyPrefab, spawnPoints[i].position,
                Quaternion.identity));
            (baseEnemies[i] as HoneybeeEnemy).walkArea.points[0].point.position = spawnPoints[i].position;
        }
        await Task.Delay(50);
        for (int i = 0; i < baseEnemies.Count; i++)
        {
            baseEnemies[i].SetRange(startRange.position, endRange.position);
            baseEnemies[i].isRespawnBlock = true;
        }
        spawnedEnemies.AddRange(baseEnemies);
    }
    
    #endregion

    #region EVENT_HANDLERS

    #endregion

    #region UI_CALLBACKS

    #endregion
}

public enum DragonState
{
    Entry,
    GroundIdle,
    Walk,
    FlyIdle,
    SuperAttack,
    GroundAttack,
    FlyAttack,
    TailAttack,
    Damage,
    Death,
    Teckoff,
    Landing,
    Angry,
    Jump
}