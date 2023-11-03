

using GameCoreFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.VFX;
using Random = UnityEngine.Random;

public class BullEnemy : ChaseEnemy
{
    #region PUBLIC_VARS

    public BaseEnemy spawnEnemyPrefab;
    public List<Transform> spawnPoints;
    public BoxCollider attackRightHand;
    public BoxCollider attackLeftHand;
    public BoxCollider attackRang;
    public BoxCollider bodyCollider;
    public List<VisualEffect> turtleSpawnTrail;
    public List<VisualEffect> turtleSpawnEffect;
    public Transform areaPoint;
    public bool isLookingPlayer;
    public BullState currentState;
    public float PunchAttackReloadTime;
    public float superAttackReloadTime;
    public float throwAttackReloadTime;
    public Transform startRange;
    public Transform endRange;
    public int attackLevel;
    public List<Transform> attackAreas;

    #endregion

    #region PRIVATE_VARS
    [SerializeField] private VisualEffect bossSlamVFX;
    [SerializeField] private List<BossThrowContainer> bossThrowContainer;
    [SerializeField] private EnemyAnimationTrigger animationTrigger;
    [SerializeField] private BossHealthBar healthBar;

    private int entryAnimation;
    private int walkAnimation;
    private int primaryAttackAnimation;
    private int superAttackAnimation;
    private int deathAnimation;
    private int damageAnimation;
    private int chaseAnimation;
    private int idelAnimation;
    private int Idel2Anim;
    private int DownThrowAnim;
    private int UpwardThrowAnim;
    private int AirStickAnim;
    private int AngryAnim;
    
    
    private Vector3 centerPos;
    private bool isDestroy;
    [SerializeField] private float PunchAttackRunningTime =0;
    [SerializeField] private bool isPunchAttackUnlock;
    private bool isUpwardThrowUnlock;

    private Coroutine enemyCollisionCo;
    private Coroutine damageAnimCo;
    private Coroutine superAttackCo;

    private List<BaseEnemy> spawnEnemys;

    [Header("Super Attack Paramter")]
    [SerializeField] private float superAttackDamageRange;
    [SerializeField] private float superAttackMaxDamage;
    [SerializeField] private float superAttackMaxDamageForce;

    #endregion

    #region UNITY_CALLBACKS

    private void OnEnable()
    {
        animationTrigger.OnAnimationEvenTrigger2 += DownThrowAttack;
    }

    // Start is called before the first frame update
    public override void Start()
    {
        // StartCoroutine(BGMusicSwitcher());
        healthBar = BossEnemyManager.Instance.healthBar;
        healthBar.SetBar();
        DiactiveZone(0);
        isPunchAttackUnlock = false;
        PunchAttackRunningTime = 0;
        centerPos = areaPoint.position;
        SetUp();
        spawnEnemys = new List<BaseEnemy>();
        attackRightHand.enabled = false;
        attackLeftHand.enabled = false;
        rb.constraints = RigidbodyConstraints.FreezeAll;
        entryAnimation = Animator.StringToHash("Entry");
        walkAnimation = Animator.StringToHash("Walk");
        primaryAttackAnimation = Animator.StringToHash("PrimaryAttack");
        superAttackAnimation = Animator.StringToHash("SuperAttack");
        deathAnimation = Animator.StringToHash("Death");
        damageAnimation = Animator.StringToHash("Damage");
        chaseAnimation = Animator.StringToHash("Chase");
        idelAnimation = Animator.StringToHash("Idel");
        Idel2Anim = Animator.StringToHash("Idel2");
        DownThrowAnim = Animator.StringToHash("DownThrow");
        UpwardThrowAnim = Animator.StringToHash("UpwardThrow");
        AirStickAnim = Animator.StringToHash("AirStick");
        AngryAnim = Animator.StringToHash("Angry");
        currentState = BullState.Entry;
        attackLevel = 1;
        StartCoroutine(StartMoveTOAreaPoint());
        StartCoroutine(SetLookingAtPlayer());
    }

    //IEnumerator BGMusicSwitcher()
    //{
    //    SoundManager._soundManager.day.DOFade(0, 0.2f);

    //    yield return new WaitForSeconds(0.2f);

    //    BGMusic.loop = true;
    //    BGMusic.Play();
    //    BGMusic.DOFade(0.15f, 0.2f);
    //}

    
    private void OnCollisionEnter(Collision collision)
    {
        if (rb.velocity.magnitude > 0.5 && !isDead)
        {
            rb.velocity = new Vector3(0, 0, 0);
        }
        TurtleEnemy collisionEnemy = collision.gameObject.GetComponent<TurtleEnemy>();
        if (DeadEnemyCanDamage(collisionEnemy) && enemyCollisionCo==null)
        {
            enemyCollisionCo = StartCoroutine(EnemyCheck(collisionEnemy));
        }
        else if( PlayerCanDamage(collision.gameObject.GetComponent<Player>()))
        {
            float power,damageValue;
            DamageAnimType type;
            if (currentState == BullState.PunchAttack)
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

    private void OnCollisionExit(Collision other)
    {
        if (rb.velocity.magnitude > 0.5 && !isDead)
        {
            rb.velocity = new Vector3(0, 0, 0);
        }
    }
    
    public void OnTriggerStay(Collider other)
    {
        if (PlayerCanDamage(other.gameObject.GetComponent<Player>()) && currentState==BullState.IdelAnimation && isPunchAttackUnlock && damageAnimCo== null)
        {
            if (Random.Range(0, 4) <= 1) //(Random.Range(0, 4) <= 1)
            {
                AttackAnimation();
            }
            else
            {
                isPunchAttackUnlock = false;
                StartCoroutine(ReloadingPunchAttack(false));
            }
        }
        else if(currentState == BullState.PunchAttack && PlayerCanDamage(other.gameObject.GetComponent<Player>()))
        {
            Debug.Log("Player Hit OnTriggerStay....");
            Vector3 dir = (player.transform.position-transform.position).normalized;
            player.Damage(dir*3.5f,DamageAnimType.BossDamage,damagePower);
        }
    }

    public async void OnTriggerEnter(Collider other)
    {
        if(currentState == BullState.PunchAttack && PlayerCanDamage(other.gameObject.GetComponent<Player>()))
        {
            Debug.Log("Player Hit OnTriggerEnter....");
            Vector3 dir = (player.transform.position-transform.position).normalized;
            player.Damage(dir*3f,DamageAnimType.BossDamage,damagePower);
            player.PlayBossHitVFX();
        }
        else if (other.gameObject.GetComponent<DeathZone>() && !isDestroy)
        {
            isDestroy = true;
            Kill();
            await Task.Delay(3000);
            if (castumGravity != null)
            {
                StopAllCoroutines();
            }
        }
    }

    private void OnDisable()
    {
        animationTrigger.OnAnimationEvenTrigger2 -= DownThrowAttack;
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
        if (health < 250 && attackLevel ==3)
        {
            attackLevel = 4;
            PunchAttackReloadTime *= 0.75f;
        }
        else if(health<500 && !isUpwardThrowUnlock)
        {
            attackLevel = 3;
            isUpwardThrowUnlock = true;
            throwAttackReloadTime *= 0.75f;
            superAttackReloadTime *= 0.75f;
            PunchAttackReloadTime *= 0.75f;
        }
        else if(health<800 && attackLevel ==1)
        {
            attackLevel = 2;
            StartCoroutine(ReloadingPunchAttack());
        }
        if (!isDead)
        {
            attackRightHand.enabled = false;
            attackRang.enabled = true;
            if (damageAnimCo != null)
            {
                StopCoroutine(damageAnimCo);
            }
            damageAnimCo = StartCoroutine(DamageAnimation());
        }
    }

    private bool CanDamage()
    {
        return currentState == BullState.IdelAnimation || currentState == BullState.PunchAttack;
    }

    private bool DeadEnemyCanDamage(TurtleEnemy collisionEnemy)
    {
        return collisionEnemy && collisionEnemy.isDead;
    }

    public override async void Kill()
    {
        if (!isDead)
        {
            isLookingPlayer = false;
            currentState = BullState.Death;
            BossCinematicsManager.Instance.BossAudioSetter(SoundManager._soundManager.Bull[2]);
            // BossArenaManage.Instance.SwitchListener(BossArenaManage.Listener.Camera);
            BossArenaManage.Instance.BGMusicSwitcher(false);
            GameManager.instance.currentGameState = GameStates.PlayerPaused;
            BossCinematicsManager.Instance.PlayBossCinematics(BossCinematicsShowCaseType.EndCinematics);
            player.bossFightEnd();
            isDead = true;
            rb.constraints = RigidbodyConstraints.FreezeAll;
            animator.SetTrigger(deathAnimation);
            //base.ToggleSound(2, false, SoundManager.Soundtype.BullBoss);
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
           // SoundManager._soundManager.day.DOFade(0.06f, 0.4f);
            attackRang.enabled = false;
            attackRightHand.enabled = false;
            bodyCollider.enabled = false;
            for (int i = 0; i < spawnEnemys.Count; i++)
            {
                if (spawnEnemys[i] != null && !spawnEnemys[i].isDead)
                {
                    await Task.Delay(200);
                    spawnEnemys[i].Kill();
                }
            }
            await Task.Delay(4000);
           // BossArenaManage.Instance.SwitchListener(BossArenaManage.Listener.Coco);
            BossArenaManage.Instance.SetPathInBullArena();
            healthBar.ResetBar();
        }
    }

    public override void AttackAnimation()
    {
        StartCoroutine(PunchAttackAnimation());
    }
    
    
    public List<Transform> GetSpawnPoint()
    {
        List<Transform> currentSpawnPoint = new List<Transform>();
        if (attackLevel < 3)
        {
            currentSpawnPoint.Add(spawnPoints[2]);
            currentSpawnPoint.Add(spawnPoints[3]);
        }
        else if (attackLevel == 3)
        {
            currentSpawnPoint.Add(spawnPoints[2]);
            currentSpawnPoint.Add(spawnPoints[3]);
            currentSpawnPoint.Add(spawnPoints[4]);
        }
        else if(attackLevel == 4)
        {
            for (int i = 0; i < 4; i++)
            {
                currentSpawnPoint.Add(spawnPoints[i]);
            }
        }
        else if (attackLevel > 5)
        {
            currentSpawnPoint.AddRange(spawnPoints);
        }
        return currentSpawnPoint;
    }

    public void DownThrowAttack()
    {
        BossThrowData bossThrowData = bossThrowContainer.Find(x => x.bossThrowableType == BossThrowableType.RockDownwardsThrow).bossThrowData;
        BossThrowables prefab = Instantiate(bossThrowData.bossThrowablesPrefab, bossThrowData.throwPoint.position, Quaternion.identity);
        prefab.SetData(bossThrowData.throwPoint);
        prefab.Throw();
    }

    public void UpwardThrowAttack()
    {
        BossThrowData bossThrowData = bossThrowContainer.Find(x => x.bossThrowableType == BossThrowableType.RockUpwardsThrow).bossThrowData;
        List<Transform> areas = new List<Transform>();
        areas.AddRange(attackAreas);
        Vector3 targetPos = player.transform.position;
        for (int i = 0; i < 8; i++)
        {
            Transform trans = areas[Random.Range(0,areas.Count)];
            targetPos = trans.position;
            RockThrowable prefab = Instantiate(bossThrowData.bossThrowablesPrefab) as RockThrowable;
            prefab.transform.position = targetPos + new Vector3(Random.Range(-5.3f,5.3f), Random.Range(30.1f,36.5f), Random.Range(-5.3f,5.3f));
            prefab.SetUIRing( new Vector3(prefab.transform.position.x,targetPos.y,prefab.transform.position.z));
            prefab.SetData(trans);
            prefab.Throw();
            areas.Remove(trans);
            DiactiveZone(5);
        }
        targetPos = new Vector3(player.transform.position.x, targetPos.y+0.15f, player.transform.position.z); 
        RockThrowable rock = Instantiate(bossThrowData.bossThrowablesPrefab) as RockThrowable;
        rock.transform.position = targetPos + new Vector3(Random.Range(-1.1f,1.1f), Random.Range(30.1f,36.5f), Random.Range(-1.1f,1.1f));
        rock.SetUIRing( new Vector3(rock.transform.position.x,targetPos.y,rock.transform.position.z));
        rock.Throw();
    }

    public async void DiactiveZone(float time)
    {
        await Task.Delay((int)(time*1000));
        for (int i = 0; i < attackAreas.Count; i++)
        {
            attackAreas[i].gameObject.SetActive(false);
        }
    }

    #endregion

    #region PRIVATE_FUNCTIONS

    #endregion

    #region CO-ROUTINES


    public IEnumerator PunchAttackAnimation()
    {
        currentState = BullState.PunchAttack;
        isPunchAttackUnlock = false;
        attackRang.enabled = false;
        attackRightHand.enabled = true;
        animator.SetTrigger(primaryAttackAnimation);
        base.ToggleSound(9, false, SoundManager.Soundtype.BullBoss);
        yield return new WaitForSeconds(0.1f);
        if (chaseCO != null)
        {
            StopCoroutine(chaseCO);
        }
        yield return new WaitForSeconds(0.6f);
        attackRightHand.enabled = false;
        attackRang.enabled = true;
        animator.SetTrigger(idelAnimation);
        currentState = BullState.IdelAnimation;
        StartCoroutine(ReloadingPunchAttack());
    }
    
    public IEnumerator SetAirStickAnimation()
    {
        isPunchAttackUnlock = false;
        currentState = BullState.UpwardThrow;
        animator.SetTrigger(AirStickAnim);
        base.ToggleSound(0, false, SoundManager.Soundtype.BullBoss);
        yield return new WaitForSeconds(2f);
        UpwardThrowAttack();        
        yield return new WaitForSeconds(2f);
        animator.SetTrigger(Idel2Anim);
        base.ToggleSound(4, false, SoundManager.Soundtype.BullBoss);
        currentState = BullState.IdelAnimation;
    }

    public IEnumerator SetDownwardThrowAnimation()
    {
        currentState = BullState.DownwardThrow;
        animator.SetTrigger(DownThrowAnim);
        yield return new WaitForSeconds(2);
        animator.SetTrigger(Idel2Anim);
        base.ToggleSound(4, false, SoundManager.Soundtype.BullBoss);
        currentState = BullState.IdelAnimation;
    }

    
    public IEnumerator SuperAttackCo()
    {
        float RelodTime =  Random.Range(superAttackReloadTime*0.75f,superAttackReloadTime*1.2f);
        while (!isDead)
        {
            if (RelodTime > 0)
            {
                RelodTime--;
                yield return new WaitForSeconds(1);
            }
            else if(currentState == BullState.IdelAnimation && attackLevel>0)
            {
                yield return StartCoroutine(SuperAttackAnimation());
                RelodTime =  Random.Range(superAttackReloadTime*0.75f,superAttackReloadTime*1.2f);
            }
            else
            {
                yield return new WaitForSeconds(3);
            }   
        }
    }
    
    public IEnumerator ThrowCo()
    {
        float RelodTime =  Random.Range(throwAttackReloadTime*0.75f,throwAttackReloadTime*1.2f);
        while (!isDead)
        {
            if (RelodTime > 0)
            {
                RelodTime--;
                yield return new WaitForSeconds(1);
            }
            else if(currentState == BullState.IdelAnimation)
            {
                bool upperThrow=false;
                if (isUpwardThrowUnlock)
                {
                    upperThrow = Random.Range(0, 3) < 2;
                }
                if (upperThrow) //(upperThrow)
                {
                    yield return StartCoroutine(SetAirStickAnimation());
                }
                else
                {
                    yield return StartCoroutine(SetDownwardThrowAnimation());
                }
                RelodTime = Random.Range(throwAttackReloadTime*0.75f,throwAttackReloadTime*1.2f);;
            }
            else
            {
                yield return new WaitForSeconds(1);
            }   
        }
    }

    public IEnumerator SetLookingAtPlayer()
    {
        while (!isDead)
        {
            if (isLookingPlayer)
            {
                float angle = GetMoveingAngle(transform.position, player.transform.position);
                yield return rotateCo = StartCoroutine(Rotate(angle));
            }
            else
            {
                yield return new WaitForSeconds(1);
            }
        }
    }
    
    public IEnumerator StartMoveTOAreaPoint()
    {
        BossArenaManage.Instance.PlayerBlockSetup();
        animator.SetTrigger(walkAnimation);
        base.ToggleSound(6, true, SoundManager.Soundtype.BullBoss);
        BossArenaManage.Instance.BossPath.SetActive(true);
        yield return moveCo = StartCoroutine(Move(new Vector3(centerPos.x, transform.position.y,centerPos.z)));
        yield return StartCoroutine(AngryAnimation(Idel2Anim));
        StartCoroutine(SuperAttackCo());
        StartCoroutine(ThrowCo());
        isLookingPlayer = true;
        GameManager.instance.input.EnableInput();
        MenuInput.EnableUiInput();
    }

    public IEnumerator AngryAnimation(int nextAnimation)
    {
        currentState = BullState.AngryAnimation;
        animator.SetTrigger(AngryAnim);
        base.ToggleSound(1, false, SoundManager.Soundtype.BullBoss);
        yield return new WaitForSeconds(1.3f);
        animator.SetTrigger(nextAnimation);
        if (Idel2Anim == nextAnimation || idelAnimation == nextAnimation)
        {
            currentState = BullState.IdelAnimation;
        }
    }
    
    public IEnumerator DamageAnimation()
    {
        currentState = BullState.Damage;
        animator.SetTrigger(damageAnimation);
        base.ToggleSound(7, false, SoundManager.Soundtype.BullBoss);
        yield return new WaitForSeconds(2.2f);
        if (!isDead)
        {
            animator.SetTrigger(idelAnimation);
            base.ToggleSound(4, false, SoundManager.Soundtype.BullBoss);
            currentState = BullState.IdelAnimation;
        }
        damageAnimCo = null;
    }

    private IEnumerator SuperAttackAnimation()
    {
        isPunchAttackUnlock = false;
        yield return StartCoroutine(AngryAnimation(superAttackAnimation));
        currentState = BullState.SuperAttack;
        base.ToggleSound(11, false, SoundManager.Soundtype.BullBoss);
        yield return new WaitForSeconds(0.8f);
        CameraController.Instance.ShakeCamera(0.25f, 1f);
        Collider[] colliders = Physics.OverlapSphere(transform.position, superAttackDamageRange);
        foreach (Collider collider in colliders)
        {
            if (collider.TryGetComponent<Player>(out Player player) && collider.gameObject != gameObject)
            {
                float distance = Vector3.Distance(transform.position, player.transform.position);
                if (distance < superAttackDamageRange && player.OnGroundChecker())
                {
                    Vector3 direction = (player.transform.position - transform.position).normalized;
                    float damageAmount = superAttackMaxDamage * (1 - (distance / superAttackDamageRange));
                    Vector3 directionAmount =
                        direction * (superAttackMaxDamageForce * damageAmount / superAttackMaxDamage);
                    player.Damage(directionAmount, DamageAnimType.BossDamage, damageAmount);
                    break;
                }
            }
        }
        bossSlamVFX.gameObject.SetActive(true);
        bossSlamVFX.Play();
        List<Transform> currentSpawnPoint = GetSpawnPoint();
        for (int i = 0; i < currentSpawnPoint.Count; i++)
        {
            turtleSpawnTrail[i].Play();
            turtleSpawnTrail[i].transform.DOMove(currentSpawnPoint[i].position, 1.2f);
        }
        yield return new WaitForSeconds(0.5f);
        attackRightHand.enabled = true;
        attackLeftHand.enabled = true;
        yield return new WaitForSeconds(0.8f);
        bossSlamVFX.gameObject.SetActive(false);
        List<BaseEnemy> baseEnemies = new List<BaseEnemy>();
        for (int i = 0; i < currentSpawnPoint.Count; i++)
        {
            turtleSpawnEffect[i].transform.position = currentSpawnPoint[i].position;
            turtleSpawnEffect[i].Play();
            turtleSpawnTrail[i].Stop();
            baseEnemies.Add(Instantiate(spawnEnemyPrefab, currentSpawnPoint[i].position,
                Quaternion.identity));
            (baseEnemies[i] as TurtleEnemy).walkArea.points[0].point.position = currentSpawnPoint[i].position;
            turtleSpawnTrail[i].transform.localPosition = Vector3.zero;
        }
        yield return null;
        for (int i = 0; i < baseEnemies.Count; i++)
        {
            baseEnemies[i].SetRange(startRange.position, endRange.position);
            baseEnemies[i].isRespawnBlock = true;
        }
        spawnEnemys.AddRange(baseEnemies);
        yield return new WaitForSeconds(0.8f);
        attackRightHand.enabled = false;
        attackLeftHand.enabled = false;
        superAttackCo = null;
        animator.SetTrigger(Idel2Anim);
        base.ToggleSound(4, false, SoundManager.Soundtype.BullBoss);
        currentState = BullState.IdelAnimation;
    }

    public IEnumerator ReloadingPunchAttack(bool isFullTimeReload=true)
    {
        if (!isPunchAttackUnlock)
        {
            if (PunchAttackRunningTime <= 0)
            {
                if (isFullTimeReload)
                {
                    PunchAttackRunningTime = Random.Range(PunchAttackReloadTime*0.75f,PunchAttackReloadTime*1.2f);
                }
                else
                {
                    PunchAttackRunningTime = PunchAttackReloadTime/2;
                }
                while (PunchAttackRunningTime > 0)
                {
                    PunchAttackRunningTime -= Time.deltaTime;
                    yield return null;
                }

                isPunchAttackUnlock = true;
            }
            else
            {
                PunchAttackRunningTime = PunchAttackReloadTime;
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
            if (enemy.rb.velocity.magnitude > 1.4f)
            {
                Damage(50);
                break;
            }

            if (enemy.rb.velocity.magnitude <= 0.35f)
            {
                break;
            }
        }
        enemyCollisionCo = null;
    }

    #endregion

    #region EVENT_HANDLERS

    #endregion

    #region UI_CALLBACKS

    #endregion


    #region GIZMOS
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, superAttackDamageRange);
    }
    #endregion
}

public enum BullState
{
    Entry,
    IdelAnimation,
    PunchAttack,
    SuperAttack,
    UpwardThrow,
    DownwardThrow,
    AngryAnimation,
    Death,
    Damage
}