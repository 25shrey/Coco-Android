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
using System.Linq;

public class StonemanEnemy : ChaseEnemy
{
    #region PUBLIC_VARS

    public BaseEnemy spawnEnemyPrefab;
    public List<Transform> spawnPoints;
    public CapsuleCollider bodyCollider;
    public Transform areaPoint;
    public bool isLookingPlayer;
    public StoneManState currentState;
    public Transform startRange;
    public Transform endRange;

    #endregion

    #region PRIVATE_VARS
    private int normalAttackCount;

    private int walkAnimation;
    private int superAttackAnimation;
    private int punchAttackAnimation;
    private int throwRopAnimation;
    private int ropCatchAnimation;
    private int pickPlayerAnimation;
    private int deathAnimation;
    private int damageAnimation;
    private int idelAnim1;
    private int idelAnim2;
    private int angryAnim;
    private int angryAttackAnim;

    [SerializeField] private PlayerDemageObject playerRingDamageObjectRight;
    [SerializeField] private PlayerDemageObject playerRingDamageObjectLeft;
    [SerializeField] private MeshCollider RightHand;
    [SerializeField] private MeshCollider LeftHand;
    [SerializeField] private Vein veinObj;
    [SerializeField] private BossHealthBar healthBar;
    private Vector3 centerPos;
    private bool isDestroy;
    [SerializeField] protected StonemanAnimationTrigger animationTrigger;
    private Coroutine enemyCollisionCo;
    private Coroutine damageAnimCo;
    private Coroutine superAttackCo;
    private List<BaseEnemy> spawnEnemys;

    [Header("Super Attack Paramter")]
    [SerializeField] private float superAttackDamageRange;


    [Header("Boss Attack")]
    [SerializeField] private GameObject handObj;
    [SerializeField] private GameObject playerPickPoint;
    [SerializeField] private float minAttackTime;
    [SerializeField] private float maxAttackTime;
    [SerializeField] private float punchAttackDistance; // 12 
    [SerializeField] private float minAttackDistance; // 30 
    [SerializeField] private bool inHand;
    [SerializeField] private bool subattackComplete;
    [SerializeField] private float  veinRotationAmount = 13f;

    private Vector3 plrInitRotation;
    private Vector3 plrInitPos;
    private Coroutine lookRoutine;
    
    [SerializeField] private AudioSource BGMusic;

    [Header("VFX")] 
    [SerializeField] private VisualEffect leftHandAngryVFX;
    [SerializeField] private VisualEffect rightHandAngryVFX;
    [SerializeField] private VisualEffect pickPlayerVFX;
    [SerializeField] private VisualEffect punchAttackVFX;
    [SerializeField] private GrowVineScript growVeins;

    #endregion

    #region UNITY_CALLBACKS
    
    protected void OnEnable()
    {
        animationTrigger.PickPlayer += SetPlayerInHand;
        animationTrigger.ThrowPlayer += RemovePlayerFromHand;
        animationTrigger.leftHandVFX += PlayLeftHandAngryVFX;
        animationTrigger.rightHandVFX += PlayRightHandAngryVFX;
        animationTrigger.playerPickVFXEvent += PlayPlayerPickVFX;
        animationTrigger.punchAttackVFXEvent += PlayPunchAttackVFX;
    }

    protected void OnDisable()
    {
        animationTrigger.PickPlayer -= SetPlayerInHand;
        animationTrigger.ThrowPlayer -= RemovePlayerFromHand;
        animationTrigger.leftHandVFX -= PlayLeftHandAngryVFX;
        animationTrigger.rightHandVFX -= PlayRightHandAngryVFX;
        animationTrigger.playerPickVFXEvent -= PlayPlayerPickVFX;
        animationTrigger.punchAttackVFXEvent -= PlayPunchAttackVFX;
    }

    // Start is called before the first frame update
    public override void Start()
    {
        healthBar = BossEnemyManager.Instance.healthBar;
        healthBar.SetBar();
        subattackComplete = true;
        centerPos = areaPoint.position;
        SetUp();
        spawnEnemys = new List<BaseEnemy>();
        rb.constraints = RigidbodyConstraints.FreezeAll;
        walkAnimation = Animator.StringToHash("Walk");
        superAttackAnimation = Animator.StringToHash("SuperAttack");
        punchAttackAnimation = Animator.StringToHash("PunchAttack");
        deathAnimation = Animator.StringToHash("Death");
        damageAnimation = Animator.StringToHash("Damage");
        idelAnim1 = Animator.StringToHash("Idle");
        idelAnim2 = Animator.StringToHash("Idle2");
        angryAnim = Animator.StringToHash("Angry");
        angryAttackAnim = Animator.StringToHash("AngryAttack");
        throwRopAnimation = Animator.StringToHash("PrimaryAttack");
        ropCatchAnimation = Animator.StringToHash("RopCatch");
        pickPlayerAnimation = Animator.StringToHash("PickPlayer");
        currentState = StoneManState.Entry;
        StartCoroutine(StartMoveTOAreaPoint());
        lookRoutine =  StartCoroutine(SetLookingAtPlayer());
        plrInitPos = player.transform.position;
        plrInitPos.y = 59.4f;
        GameManager.instance.input.EnableInput();
        MenuInput.EnableUiInput();
    }
    
    
    private void OnCollisionEnter(Collision collision)
    {
        if(PlayerCanDamage(collision.gameObject.GetComponent<Player>()) && currentState != StoneManState.SubAttack)
        {
            Vector3 dir = (player.transform.position-transform.position).normalized;
            if (currentState == StoneManState.AngryAttack)
            {
                player.Damage(dir * 4, DamageAnimType.Damage, damagePower);
            }
            else
            {
                player.Damage(dir * 3f, DamageAnimType.Damage, 20);
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

    private bool CanDamage()
    {
        return currentState == StoneManState.Idel;
    }

    public void GrowVeins()
    {
        growVeins.gameObject.SetActive(true);
        growVeins.GrowVines();
        Invoke("ReverseGrowVeins", 3.5f);
    }

    public async void ReverseGrowVeins()
    {
        growVeins.ReverseGrowVines();
        await Task.Delay(2000);
        growVeins.gameObject.SetActive(false);
    }

    public override async void Kill()
    {
        if (!isDead)
        {
            // base.ToggleSound(2, false, SoundManager.Soundtype.stonemanBoss);
            BossCinematicsManager.Instance.BossAudioSetter(SoundManager._soundManager.stoneman[2]);
            isLookingPlayer = false;
            currentState = StoneManState.Death;
          //  BossArenaManage.Instance.SwitchListener(BossArenaManage.Listener.Camera);
            BossArenaManage.Instance.BGMusicSwitcher(false);
            GameManager.instance.currentGameState = GameStates.PlayerPaused;
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
            base.Kill();
            await Task.Delay(100);
            SoundManager._soundManager.day.DOFade(GS.Instance.backgroundSoundVolume, 0.4f);
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
    
    #endregion

    #region PRIVATE_FUNCTIONS
    
    private void SetPlayerInHand()
    {
        if (!inHand)
        {
            inHand = true;
            plrInitRotation = player.transform.eulerAngles;
            player.transform.parent = handObj.transform;
            player.transform.localPosition = Vector3.zero;
            base.ToggleSound(5, false, SoundManager.Soundtype.stonemanBoss);
        }
    }

    private async void RemovePlayerFromHand()
    {
        if (inHand)
        {
            player.transform.SetParent(null);
            player.transform.DOMove(plrInitPos, 0.9f);
            await Task.Delay(100);
            CameraController.Instance.isFollowing = true;
            Vector3 dir = (player.transform.position - transform.position).normalized;
            player.Damage(dir * 0.2f, DamageAnimType.BossDamage, damagePower);
            await Task.Delay(800);
            player.transform.eulerAngles = plrInitRotation;
            player.characterController.enabled = true;
            player.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            player.isGrabbed = false;
            isLookingPlayer = true;
            StartCoroutine(RemoveCoco());
        }
    }

    IEnumerator RemoveCoco()
    {
        yield return new WaitForSeconds(4f);

        if (inHand)
        {
            inHand = false;
            subattackComplete = true;
        }
    }

    private void PlayLeftHandAngryVFX()
    {
      //  Debug.Log("Left hand Angry VFX");
        GameManager.instance.PlayVFX(leftHandAngryVFX);
        GameManager.instance.StopVFX(leftHandAngryVFX, 10000);
    }
    
    private void PlayRightHandAngryVFX()
    {
      //  Debug.Log("Right hand Angry VFX");
        GameManager.instance.PlayVFX(rightHandAngryVFX);
        GameManager.instance.StopVFX(rightHandAngryVFX, 10000);
    }

    private void PlayPlayerPickVFX()
    {
       // Debug.Log("PlayPickVFX");
        GameManager.instance.PlayVFX(pickPlayerVFX);
        GameManager.instance.StopVFX(pickPlayerVFX, 10000);
    }

    private void PlayPunchAttackVFX()
    {
      //  Debug.Log("PunchAttackVFX");
        GameManager.instance.PlayVFX(punchAttackVFX);
        GameManager.instance.StopVFX(punchAttackVFX, 500);
    }
    
    #endregion

    #region CO-ROUTINES

    public IEnumerator AttackTimerCo()
    {
        float reloadTime = Random.Range(minAttackTime, maxAttackTime) / 2;
        while (!isDead)
        {
            if (reloadTime > 0)
            {
                if (currentState != StoneManState.Idel)
                {
                    reloadTime -= Time.deltaTime/2;
                }
                else
                {
                    reloadTime -= Time.deltaTime;
                    Vector3 pos = new Vector3(transform.position.x, player.transform.position.y, transform.position.z);
                    if (Vector3.Distance(pos, player.transform.position) < punchAttackDistance)
                    {
                        reloadTime -= Time.deltaTime;
                    }
                }
                yield return null;
            }
            else if (currentState == StoneManState.Idel)
            {
                yield return StartCoroutine(SelectAttack());
                reloadTime = Random.Range(minAttackTime, maxAttackTime);
            }
            else
            {
                yield return new WaitForSeconds(1);
            }
        }
    }

    public IEnumerator SelectAttack()
    {
        Vector3 pos = new Vector3(transform.position.x, player.transform.position.y, transform.position.z);
        StoneManState attack = StoneManState.AngryAttack;
        if (Vector3.Distance(pos, player.transform.position) > minAttackDistance &&
            currentState == StoneManState.Idel && !inHand && subattackComplete)
        {
            attack = StoneManState.SubAttack;
        }
        else if (Random.Range(0, 3) < 2)
        {
            attack = StoneManState.AngryAttack;
        }
        if (Vector3.Distance(pos, player.transform.position) < punchAttackDistance)
        {
            attack = StoneManState.PunchAttack;
        }
        if ((normalAttackCount > 2 && Random.Range(0, 4) < 2) || normalAttackCount > 5)
        {
            attack = StoneManState.SuperAttack;
        }
        Debug.Log("Distance : "+Vector3.Distance(pos, player.transform.position));
        switch (attack)
        {
            case StoneManState.SuperAttack:
                normalAttackCount = 0;
                yield return StartCoroutine(SuperAttackAnimation());
                break;
            case StoneManState.SubAttack:
                yield return StartCoroutine(ShowAttackAnimation());
                normalAttackCount++;
                break;
            case StoneManState.AngryAttack:
                if (Random.Range(0, 2) == 1)
                {
                    ShowAngryAnimation();
                }
                else
                {
                    ShowAngryAttackAnimation();
                }
                normalAttackCount++;
                break;
            case StoneManState.PunchAttack:
                normalAttackCount++;
                ShowPunchAttack();
                break;
            default:
                Debug.Log("No Attack Found");
                break;
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

    public async void ShowAngryAttackAnimation()
    {
        base.ToggleSound(0, false, SoundManager.Soundtype.stonemanBoss);
        HandDamageColliders(true);
        currentState = StoneManState.AngryAttack;
        animator.SetTrigger(angryAttackAnim);
        Vector3 LeftRingStartScale = playerRingDamageObjectLeft.transform.localScale;
        Vector3 RightRingStartScale = playerRingDamageObjectRight.transform.localScale;
        await Task.Delay(1000);
        RingAttackRight(new Vector3(5, 5, 5), 1.5f);
        await Task.Delay(200);
        RingAttackLeft(new Vector3(5, 5, 5), 1.5f);
        await Task.Delay(200);
        CameraController.Instance.ShakeCamera(0.25f, 1f);
        StartCoroutine(GameManager.instance.CheckForAnimationCompletion(() =>
        {
            PlayIdleAnimation();
            RingAttackRight(RightRingStartScale, 1.5f);
            RingAttackLeft(LeftRingStartScale, 1.5f);
            HandDamageColliders(false);
        }, animator, 0));
    }
    
    public void ShowAngryAnimation()
    {
        base.ToggleSound(0, false, SoundManager.Soundtype.stonemanBoss);
        currentState = StoneManState.AngryAttack;
        animator.SetTrigger(angryAnim);
        CameraController.Instance.ShakeCamera(0.25f, 1f);
        StartCoroutine(GameManager.instance.CheckForAnimationCompletion(PlayIdleAnimation, animator, 0));
    }
    
    private void PlayIdleAnimation()
    {
        currentState = StoneManState.Idel;
        if (Random.Range(0, 2) == 1)
        {
            animator.SetTrigger(idelAnim1);
        }
        else
        {
            animator.SetTrigger(idelAnim2);
        }
    }

    public IEnumerator StartMoveTOAreaPoint()
    {
        base.ToggleSound(7, false, SoundManager.Soundtype.stonemanBoss);
        BossArenaManage.Instance.PlayerBlockSetup();
        animator.SetTrigger(walkAnimation);
        BossArenaManage.Instance.BossPath.SetActive(true);
        yield return moveCo = StartCoroutine(Move(new Vector3(centerPos.x, transform.position.y, centerPos.z)));
        PlayIdleAnimation();
        yield return new WaitForSeconds(7);
        StartCoroutine(AttackTimerCo());
        isLookingPlayer = true;
    }

    public void DamageAnimation()
    {
        base.ToggleSound(1, false, SoundManager.Soundtype.stonemanBoss);
        currentState = StoneManState.Damage;
        animator.SetTrigger(damageAnimation);
        StartCoroutine(GameManager.instance.CheckForAnimationCompletion(PlayIdleAnimation, animator, 0));
    }

    public void ShowPunchAttack()
    {
        base.ToggleSound(4, false, SoundManager.Soundtype.stonemanBoss);
        currentState = StoneManState.PunchAttack;
        animator.SetTrigger(punchAttackAnimation);
        StartCoroutine(GameManager.instance.CheckForAnimationCompletion(PlayIdleAnimation, animator, 0));
    }
    
    private IEnumerator ShowAttackAnimation()
    {
        base.ToggleSound(3, false, SoundManager.Soundtype.stonemanBoss);
        animator.SetTrigger(throwRopAnimation);
        currentState = StoneManState.SubAttack;

        yield return new WaitForSeconds(1f);
        isLookingPlayer = false;
        subattackComplete = false;
        veinObj.transform.parent.LookAt(player.transform.position);
        veinObj.transform.parent.transform.Rotate(veinRotationAmount, 0, 0);

        veinObj.Initiate();
    }

    public void PlayerGrabFail()
    {
        StartCoroutine(GameManager.instance.CheckForAnimationCompletion(PlayIdleAnimation, animator, 0));
        isLookingPlayer = true;
        subattackComplete = true;
    }

    public void PlayerGrabSuccessful()
    {
        player.isGrabbed = true;
        StartCoroutine(GameManager.instance.CheckForAnimationCompletion((() =>
        {
            StartCoroutine(ShowRopCatchAnimation());
        }), animator, 0));
    }

    public void RingAttackRight(Vector3 target, float time)
    {
        base.ToggleSound(8, false, SoundManager.Soundtype.stonemanBoss);
        playerRingDamageObjectRight.transform.DOKill();
        playerRingDamageObjectRight.gameObject.SetActive(true);
        playerRingDamageObjectRight.transform.DOScale(target, time);
    }

    public void RingAttackLeft(Vector3 target, float time)
    {
        playerRingDamageObjectLeft.transform.DOKill();
        playerRingDamageObjectLeft.gameObject.SetActive(true);
        playerRingDamageObjectLeft.transform.DOScale(target, time);
    }

    private IEnumerator ShowRopCatchAnimation()
    {
        yield return new WaitForSeconds(0.25f);
        animator.SetTrigger(ropCatchAnimation);
        yield return new WaitForSeconds(0.25f);
        Vector3 target = new Vector3(playerPickPoint.transform.position.x, plrInitPos.y,
            playerPickPoint.transform.position.z);
        player.characterController.enabled = false;
        yield return null;
        player.transform.DOMove(target,0.75f);
        yield return new WaitForSeconds(0.75f);
        //stopFollow
        CameraController.Instance.isFollowing = false;
        yield return null;
        animator.SetTrigger(pickPlayerAnimation);
        StartCoroutine(GameManager.instance.CheckForAnimationCompletion(PlayIdleAnimation, animator, 0));
    }
    
    private IEnumerator SuperAttackAnimation()
    {
        base.ToggleSound(6, false, SoundManager.Soundtype.stonemanBoss);
        animator.SetTrigger(superAttackAnimation);
        currentState = StoneManState.SuperAttack;
        yield return new WaitForSeconds(0.8f);
        CameraController.Instance.ShakeCamera(0.25f, 1f);
        GrowVeins();
        yield return new WaitForSeconds(2.5f);
        List<BaseEnemy> baseEnemies = new List<BaseEnemy>();
        for (int i = 0; i < spawnPoints.Count; i++)
        {
            Vector3 offset =Vector3.zero;
            if (Vector3.Distance(spawnPoints[i].position, playerPos) < 2)
            {
                offset = new Vector3(2.5f, 0, 0);
            }
            baseEnemies.Add(Instantiate(spawnEnemyPrefab, spawnPoints[i].position+offset,
                Quaternion.identity));
        }
        yield return null;
        for (int i = 0; i < baseEnemies.Count; i++)
        {
            baseEnemies[i].SetRange(startRange.position, endRange.position);
            baseEnemies[i].isRespawnBlock = true;
        }
        spawnEnemys.AddRange(baseEnemies);
        StartCoroutine(GameManager.instance.CheckForAnimationCompletion(()=>
        {
            superAttackCo = null;
            PlayIdleAnimation();
        }, animator, 0));
    }

    public IEnumerator EnemyCheck(TreeEnemy enemy)
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

    void HandDamageColliders(bool value)
    {
        LeftHand.enabled = value;
        RightHand.enabled = value;
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

public enum StoneManState
{
    Entry,
    Idel,
    SuperAttack,
    SubAttack,
    AngryAttack,
    PunchAttack,
    Death,
    Damage
}
