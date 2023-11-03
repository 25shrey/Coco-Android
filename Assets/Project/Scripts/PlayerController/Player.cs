using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using Game.BaseFramework;
using Game.CheckPoints;
using GameCoreFramework;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.VFX;
using UnityEngine.Windows;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
    public float bossDamageAnimationTime = 1;
    
    /*public Transform boneStructure;
    public Transform newSkinMeshOutfitParent;
    public Transform newMeshOutfitParent;*/
    public CocoClothData clothDataReference;
    
    public int life;
    public float WalkSpeed = 2f;
    [Range(0f, 1f)]
    public float turnSpeed = 0.02f;
    public float RunSpeed = 6f;
    public int JumpCount = 0;
    public PlayerAnimationController animationController;
    public PlayerPowerUps playerPowerUps;
    public float maxFireTime;
    private float health;
    private float maxHealth;
    public bool isDamageing;
    public bool isJumpDisenable;
    public GameObject shieldPowerUpEffect;
    public SpawnShieldRipples spawnShieldRipples;
    public Light flashLight;
    public SphereCollider magnetSphereCollider;

    public Transform playerOrigin;
    public Rigidbody rb;
    public List<JumpData> Jumps;
    public Weapon weapon;
    [SerializeField] private Transform forwadPoint;
    [SerializeField] private GameObject rope;
    public bool isGrabbed;

    public Transform Forwardpoint
    {
        get { return forwadPoint; }
    }

    [HideInInspector]
    public CharacterController characterController;
    
    [SerializeField] CheckPoint checker;

    [SerializeField]
    Vector3 currentMovement;


    float groundedGravity = -0.05f;
    private Coroutine playerDamageCo;
    private Coroutine playerFireDamageCo;
    private Vector3 lastMoveingDirection;
    public float deadZoneDistanceChecker;
    public float GroundZoneOffset;

    [SerializeField] LayerMask _deadZoneLayer;
    [SerializeField] LayerMask _groundLayer;

    Vector3 _initialPos;
    Quaternion _initialRot;

    public bool isDead;
    public bool isRespawned = true;
    public bool isBossFightStart;

    private Vector3 startRange;
    private Vector3 EndRange;
    private bool _onBridge;

    public BoxCollider leftHeandColliders;
    public BoxCollider rightHeandColliders;
    public bool isLastRightHandPunch;

    [SerializeField]
    bool reachedToInitialPoint = false;

    [Header("VFX")]
    [SerializeField] private VisualEffect bossHitVFX;
    [SerializeField] private VisualEffect enemyPushVFX;
    [SerializeField] private VisualEffect magnetPowerUpVFX;
    [SerializeField] private VisualEffect impactVFX;
    [SerializeField] private VisualEffect fireImpactVFX;
    
    [Header("Player Jump Variables")]
    [SerializeField] private float groundDistanceForJump;
    [SerializeField] private CapsuleCollider m_Capsule;
    
    [SerializeField] private bool _respawningInitiated;

    [HideInInspector] public AudioListener audioListener;

    public bool _reachedToInitialPoint
    {
        get { return reachedToInitialPoint; }
    }

    public bool OnBridge
    {
        get { return _onBridge; }
        set { _onBridge = value; }
    }

    public bool _powerCanBeUsed;

    /// Turtle Throw//
    // [Header("Grab Variables")]
    // public Transform grabPoint;
    // public PlayerGrabber _grabber;
    // [SerializeField] private PlayerViewPoint _viewPoint;

    /// Turtle Throw//
    // public PlayerViewPoint viewer
    // {
    //     get { return _viewPoint; }
    // }


    public delegate void PlayerHealthUI(int health);
    public static event PlayerHealthUI _playerHealthUI;
    
    public delegate void PlayerLifeUI(int life);
    public static event PlayerLifeUI _playerLifeUI;

    public delegate void PlayerCoinUI(int coin);
    public static event PlayerCoinUI _playerCoinUI;


    [Serializable]
    public class JumpData
    {
        public float maxJumpHeight = 6f, maxJumpTime = 0.75f;

        
        [HideInInspector]
        public float initialJumpVelocity, gravity = -9.8f;
    }

    private void Awake()
    {
        leftHeandColliders.enabled = false;
        rightHeandColliders.enabled = false;
        isLastRightHandPunch = false;
        maxHealth = GS.Instance.StartingHelth;
        Initialized(GS.Instance.StartingLife);
        audioListener = GetComponent<AudioListener>(); //chamge it when using scene without the hot Balloon
        _powerCanBeUsed = true;
    }
    
    private void OnEnable()
    {
        InputController.OnPrimaryAttack += AttackFireBall;
        InputController.OnPlayerPunch += PunchAttack;
        InputController.magnetPowerUpAction += MagnetPowerUpEffect;
        InputController.shieldPowerUpAction += ShieldPowerUpEffect;
    }

    private void OnDisable()
    {
        InputController.OnPrimaryAttack -= AttackFireBall;
        InputController.OnPlayerPunch -= PunchAttack;
        InputController.magnetPowerUpAction -= MagnetPowerUpEffect;
        InputController.shieldPowerUpAction -= ShieldPowerUpEffect;
    }

    
    private void OnTriggerEnter(Collider other)
    {
        BaseEnemy enemy = other.gameObject.GetComponent<BaseEnemy>();
        if (IsPlayerPunchAttackOnEnemy(enemy))
        {
            enemy.Damage(50,true);
        }
    }

    public void DeathZoneHit()
    {
        if(GameManager.instance.currentGameState == GameStates.alive && !isDead)
        {
            print("respaning started...");
            Damage(Vector3.zero,DamageAnimType.Damage,0,true,false);

            
            /// Turtle Throw//
            // if (grabPoint.transform.childCount > 0 || _grabber._picked)
            // {
            //     grabPoint.transform.GetComponentInChildren<TurtleThrow>().DestroyThrowAffect();
            //     animationController.PickUpAndThrow("Fail");
            //     _grabber._picked = false;
            //     viewer.CanPick = false;
            //     viewer._turtle = null;
            //     grabPoint.transform.GetChild(0).SetParent(null);
            // }
        }
    }

    private async void OnCollisionEnter(Collision collision)
    {
        // if (collision.gameObject.GetComponent<TerrainCollider>() && isDead)
        // {
        //     if (life > 0)
        //     {
        //         GameManager.instance.currentGameState = GameStates.PlayerRespawn;
        //         animationController.StopDeathAnimation();
        //         characterController.enabled = false;
        //
        //         await Task.Delay(4000);
        //         Respawn();
        //         if (!isBossFightStart)
        //         {
        //             RestoreManager.Instance.RestoreData();
        //         }
        //         else
        //         {
        //             RestoreManager.Instance.ClearData();
        //         }
        //         CameraController.Instance.CameraBackToSpawnPoint();
        //     }
        //     else
        //     {
        //         GameManager.instance.currentGameState = GameStates.GameOver;
        //         await Task.Delay(1000);
        //         GameManager.instance.ShowClouds();
        //         await Task.Delay(3000);
        //         GameManager.instance.RestartGame();
        //     }
        // }
    }


    public bool IsPlayerPunchAttackOnEnemy(BaseEnemy enemy)
    {
        return enemy && !enemy.gameObject.CompareTag("EnemyAttackArea") &&!enemy.isDead&&
               (leftHeandColliders.enabled || rightHeandColliders.enabled) && !isDead;
    }
    
    public void Initialized(int life = 3, bool isRestart = false)
    {
        rb = GetComponent<Rigidbody>();
       
        this.life = life;
        health = maxHealth;
        characterController = GetComponent<CharacterController>();
        SetPosition(isRestart);
        characterController.enabled = true;
        PlayerStats.Instance.obj.Health = (int)health;
        PlayerStats.Instance.obj.Life = life;
        if (GameObject.Find("UI") != null)
        {
            _playerHealthUI((int)health);
            _playerLifeUI(life);
        }
        _playerCoinUI(0);
    }

    void SetPosition(bool isRestart)
    {
        if (isRestart)
        {
            transform.position = _initialPos;
            transform.rotation = _initialRot;
        }
        else
        {
            _initialPos = transform.position;
            _initialRot = transform.rotation;
            SetupJumpVariables();
        }
    }

    private void Update()
    {
        if (GameManager.instance.currentGameState == GameStates.alive)
        {
            if (!isDamageing && !isDead)
            {
                //currentMovement.x = InputController.GetMovementVector().z;
                // currentMovement.z = InputController.GetMovementVector().x;
                currentMovement.x = ControllerMobile.XAxis();
                currentMovement.z = ControllerMobile.ZAxis();
            }
            else
            {
                currentMovement.x = 0;
                currentMovement.z = 0;
            }

            if (currentMovement.x != 0||currentMovement.z!=0)
            {
                lastMoveingDirection = currentMovement;
            }
            if (characterController.enabled)
            {
                HandleMovement();
            }

            HandleRotation();
            
            HandleGravity();

            GroundCheck(deadZoneDistanceChecker, _deadZoneLayer);
        }
        else if(GameManager.instance.currentGameState == GameStates.PlayerPaused)
        {
            currentMovement.x = 0;
            currentMovement.z = 0; 
            if (characterController.enabled)
            {
                HandleMovement();
            }
            HandleGravity();
        }

        if (GameManager.instance.currentGameState == GameStates.level_intro)
        {
            GroundCheck(deadZoneDistanceChecker*GroundZoneOffset, _groundLayer);
        }
    }

    // public bool OnGroundChecker()
    // {
    //     if (Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, groundDistanceForJump, _groundLayer))
    //     {
    //         return true;
    //     }
    //     else
    //     {
    //         if (_grounded.isGrounded)
    //         {
    //             return true;
    //         }
    //         else
    //         {
    //             return false;
    //         }
    //     }
    // }

    public bool OnGroundChecker()
    {
        RaycastHit hit; //get a hit variable to store the hit information
        if (Physics.SphereCast(
                m_Capsule.transform.position + m_Capsule.center + (Vector3.up * 0.1f),
                m_Capsule.height / 2,
                Vector3.down, 
                out hit,
                groundDistanceForJump, _groundLayer))
        {
            return  true;
        } 
        else
        {
            return false;  
        }
    }

    public bool isMoving()
    {
        return currentMovement.x != 0 || currentMovement.z != 0;
    }

    private void SetupJumpVariables()
    {
        if (Jumps != null && Jumps.Count > 0)
        {
            foreach (var jump in Jumps)
            {
                // s = d/t

                float timeToApex = jump.maxJumpTime / 2; // s

                jump.gravity = (-2 * jump.maxJumpHeight) / Mathf.Pow(timeToApex, 2); // m/s^2

                jump.initialJumpVelocity = (2 * jump.maxJumpHeight) / timeToApex; // m/s

            }
            Jumps.Insert(0, Jumps[0]);
        }
        else
        {
            Jumps = new List<JumpData> { new JumpData() };
        }

    }
    
    public void HandleJump()
    {
        if (PlayerCanJump())
        {
            SoundManager._soundManager._playerSounds.SoundToBeUsed(8, SoundManager.Soundtype.player, false, true);
            animationController.OnJump(JumpCount);
            JumpCount++;
            if (InputController.IsWalkActive())
            {
                currentMovement.y = (Jumps[JumpCount].initialJumpVelocity * 0.5f * 0.7f);
            }
            else
            {
                currentMovement.y = Jumps[JumpCount].initialJumpVelocity * 0.5f;
            }
        }
    }

    public bool PlayerCanJump()
    {
        if (isDead || GameManager.instance.currentGameState != GameStates.alive)
        {
            return false;
        }
        if (characterController.enabled)
        {
            HandleMovement();
        }
        else
        {
            return false;
        }
        if (isJumpDisenable || isDamageing)
        {
            return false;
        }
        if (JumpCount + 1 == Jumps.Count || (JumpCount==0 && !OnGroundChecker() && !characterController.isGrounded && !transform.parent))
        {
            return false;
        }
        
        /// Turtle Throw//
        // if(JumpCount == 1 && _grabber._picked)
        // {
        //     return false;
        // }
        return true;
    }

    public void HandleGravity()
    {
        bool isFalling = currentMovement.y <= 0 || !InputController.IsJumpPerformed();
        float fallMultiplier = 2f;

        if (characterController.isGrounded || !isRespawned)
        {
            if (transform.parent == null)
            {
                currentMovement.y = groundedGravity;
            }
            else
            {
                currentMovement.y = 0;
            }

            JumpCount = 0;
            animationController.OnJumpLanded();
            animationController.OnJumpCountReset(JumpCount);

        }
        else if (isFalling)
        {
            float prevVelocity = currentMovement.y;
            float newVelocity = currentMovement.y + (Jumps[JumpCount].gravity * fallMultiplier * Time.deltaTime);

            float nextVelocity = prevVelocity + newVelocity;
            nextVelocity *= 0.5f;

            currentMovement.y = nextVelocity;
        }
        else
        {
            float prevVelocity = currentMovement.y;
            float newVelocity = currentMovement.y + (Jumps[JumpCount].gravity * Time.deltaTime);

            float nextVelocity = prevVelocity + newVelocity;
            nextVelocity *= 0.5f;

            currentMovement.y = nextVelocity;

        }
    }

    public void HandleMovement()
    {
        currentMovement.x *= -1;
        if (IsBlock(currentMovement * RunSpeed * Time.deltaTime))
        {
            return;
        }
        if (InputController.IsWalkActive())
        {
            characterController.Move(currentMovement * WalkSpeed * Time.deltaTime);
        }
        else
        {
            characterController.Move(currentMovement * RunSpeed * Time.deltaTime);
        }
    }

    public bool IsBlock(Vector3 Input)
    {
        return isBossFightStart && !PlayerInRange(transform.position + Input);
    }

    public void SetBossFight(Vector3 startPoint, Vector3 endPoint)
    {
        isBossFightStart = true;
        startRange = startPoint;
        EndRange = endPoint;
        if (SavedDataHandler.Instance.PowerUpFireBallCount < 10)
        {
            SavedDataHandler.Instance.PowerUpFireBallCount = 10;
        }
        //GameManager.instance.currentGameState = GameStates.PlayerPaused;
        if (flashLight != null)
        {
            flashLight.enabled = false;
        }
        SoundManager._soundManager._playerSounds.SoundToBeUsed(8, SoundManager.Soundtype.player, false, false);
        BossCinematicsManager.Instance.PlayBossCinematics(BossCinematicsShowCaseType.EntryCinematics);
    }

    public void bossFightEnd()
    {
        if (flashLight != null)
        {
            flashLight.enabled = true;
        }
        
        isBossFightStart = false;
    }
    
    public bool PlayerInRange(Vector3 playerPos)
    {
        if (playerPos.x < startRange.x && playerPos.y > startRange.y &&
            playerPos.z > startRange.z)
        {
            if (playerPos.x > EndRange.x && playerPos.y < EndRange.y &&
                playerPos.z < EndRange.z)
            {
                return true;
            }
        }
        return false;
    }


    public void HandleRotation()
    {
        float _currentVelocity = 0.0f;

        if (InputController.IsMovingInput() || ControllerMobile.IsMoving())
        {
            var targetAngle = Mathf.Atan2(currentMovement.x, currentMovement.z) * Mathf.Rad2Deg;
            var angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _currentVelocity, turnSpeed);
            transform.rotation = Quaternion.Euler(0.0f, angle, 0.0f);
        }
    }

    public void GroundCheck(float distance, LayerMask _layer)
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position + (Vector3.up * 0.1f) , Vector3.down, out hit, distance, _layer))
        {
           // Debug.DrawRay(transform.position, Vector3.down, Color.black);
           
            if (GameManager.instance.currentGameState == GameStates.level_intro)
            {
                reachedToInitialPoint = true;

                Jumps.RemoveRange(2, Jumps.Count - 2);

                SetPosition(false);
            }
        }
    }
    
    public async void AttackFireBall()
    {
        if (!PowerHandler.Instance.isFired && SavedDataHandler.Instance.PowerUpFireBallCount>0)
        {
            SoundManager._soundManager._playerSounds.SoundToBeUsed(1, SoundManager.Soundtype.player, false, true);
            animationController.AttackAnimation(PlrAttackType.ThrowFireBall);
            PowerHandler.Instance.Fire(forwadPoint);
        }
    }
    
    public async void MagnetPowerUpEffect()
    {
        if (!PowerHandler.Instance.magnetPowerUpInUse && SavedDataHandler.Instance.PowerUpMagnetCount>0 && _powerCanBeUsed && GameManager.instance.currentGameState ==
            GameStates.alive)
        {
            SoundManager._soundManager._playerSounds.SoundToBeUsed(1, SoundManager.Soundtype.player, false, true);
            //animationController.AttackAnimation(PlrAttackType.ThrowFireBall);
            //PowerHendler.Instance.Fire(forwadPoint);
            PowerHandler.Instance.UseMagnetPower();
        }
    }
    
    public async void ShieldPowerUpEffect()
    {
        if (!PowerHandler.Instance.shieldPowerUpInUse && SavedDataHandler.Instance.PowerUpShieldCount>0 && _powerCanBeUsed && GameManager.instance.currentGameState ==
            GameStates.alive)
        {
            SoundManager._soundManager._playerSounds.SoundToBeUsed(1, SoundManager.Soundtype.player, false, true);
            //animationController.AttackAnimation(PlrAttackType.ThrowFireBall);
            //PowerHendler.Instance.Fire(forwadPoint);
            PowerHandler.Instance.UseShieldPower();
        }
    }


    /// Turtle Throw//
    // public void PickAndThrowAttack(TurtleEnemy turtle = null)
    // {
    //     if (!playerPowerUps.IsPowerUpActive(PowerUpType.Shield))
    //     {
    //         if (turtle != null)
    //         {
    //             GameManager.instance.Player._grabber.InitiateGrab(turtle);
    //         }
    //         else
    //         {
    //             GameManager.instance.Player._grabber.InitateThrow();
    //         }
    //     }
    // }

    public async void PunchAttack()
    {
        return;// Remove Punch Attack...
        if (isLastRightHandPunch)
        {
            //  _sounds.SoundToBeUsed(0, SoundManager.Soundtype.player, 0.8f);
            SoundManager._soundManager._playerSounds.SoundToBeUsed(0, SoundManager.Soundtype.player, false, true);
            animationController.AttackAnimation(PlrAttackType.LeftPunch);
            isLastRightHandPunch = false;
            leftHeandColliders.enabled = true;
        }
        else
        {
            SoundManager._soundManager._playerSounds.SoundToBeUsed(0, SoundManager.Soundtype.player, false, true);
            animationController.AttackAnimation(PlrAttackType.RightPunch);
            isLastRightHandPunch = true;
            rightHeandColliders.enabled = true;
        }

        await Task.Delay(700);
        leftHeandColliders.enabled = false;
        rightHeandColliders.enabled = false;
    }

    public void Damage(Vector3 dir,DamageAnimType damageAnimType,float damageValue = 1, bool isDirectKill = false,bool isShowAnim = true)
    {
        if (GameManager.instance.currentGameState != GameStates.alive || isDead)
        {
            return;
        }
        dir = new Vector3(dir.x, 0, dir.z);
        if (!playerPowerUps.IsPowerUpActive(PowerUpType.Shield) || isDirectKill )
        {
            health -= damageValue;

            if (health < 0)
            {
                health = 0;
            }
            if (playerDamageCo == null && isShowAnim)
            {
                playerDamageCo = StartCoroutine(DamageAnimation(dir,damageAnimType,isDirectKill));
                SoundManager._soundManager._otherSounds.SoundToBeUsed(2, SoundManager.Soundtype.player, false, true);
            }
            else if (isDirectKill || health <= 0)
            {
                if (!_respawningInitiated)
                {
                    _respawningInitiated = true;
                    Death(isDirectKill);
                }
            }
            PlayerStats.Instance.obj.Health = (int)health;
            PlayerStats.Instance.obj.Life = life;
            if (GameObject.Find("UI") != null)
            {
                _playerHealthUI((int)health);
                _playerLifeUI(life);
            }
        }
    }

    private async Task Death(bool isDirectKill)
    {
        isDead = true;
        health = 0;
        playerPowerUps.DeactivateAllPower();
        if (life > 1)
        {
            life--;
            animationController.DeathAnimationShow(isDirectKill);
            if (!isDirectKill)
            {
                await Task.Delay(1800);
            }
            else
            {
                await Task.Delay(500);
            }

            GameManager.instance.currentGameState = GameStates.PlayerRespawn;
            await Task.Delay(1000);
            characterController.enabled = false;
            animationController.StopDeathAnimation();
            Respawn();
            if (!isBossFightStart)
            {
                RestoreManager.Instance.RestoreData();
            }
            else
            {
                RestoreManager.Instance.ClearData();
            }

            CameraController.Instance.CameraBackToSpawnPoint();
            await Task.Delay(2000);
            GameManager.instance.isDeadZoneTrigger = false;
        }
        else
        {
            life = 0;
            RestoreManager.Instance.ClearData();
            animationController.DeathAnimationShow(isDirectKill);
            if (!isDirectKill)
            {
                await Task.Delay(1800);

            }
            else
            {
                await Task.Delay(500);
            }

            GameManager.instance.currentGameState = GameStates.GameOver;
            GameManager.instance.ShowClouds();
            await Task.Delay(1000);
            characterController.enabled = false;
            GameManager.instance.RestartGame();
        }
    }

    private float ramainingFireTime;
    private float currentFireDamagePerSec;
    public void DamageByFire(float value)
    {
        ramainingFireTime = maxFireTime;
        currentFireDamagePerSec = value;
        if (playerFireDamageCo == null)
        {
            Debug.Log("playerFireDamageCo");
            playerFireDamageCo = StartCoroutine(FireDamageAnimation());
        }
    }

    private IEnumerator FireDamageAnimation()
    {
        while (ramainingFireTime>0)
        {
            ramainingFireTime--;
            Damage(Vector3.zero,DamageAnimType.Damage, currentFireDamagePerSec, false, false);
            yield return new WaitForSeconds(1);
        }

        playerFireDamageCo = null;
    }

    public IEnumerator DamageAnimation(Vector3 direction , DamageAnimType damageAnimType, bool isDirectKill)
    {
        float time = 0.4f;
        isDamageing = true;
        Vector3 startScale = transform.localScale,endScale=transform.localScale/1.75f;
        Transform parent = null;
        if (transform.parent != null)
        {
            parent = transform.parent;
        }
        animationController.DamageAnimationShow(damageAnimType);
        while (time>0)
        {
            time -= Time.deltaTime;
            currentMovement.x = direction.x * -1;
            currentMovement.z = direction.z;
          //  print("pushed....");
            HandleMovement();
            yield return null;
        }
        if (parent != null && transform.parent == null)
        {
            transform.SetParent(parent);
            transform.localScale = startScale;
            transform.SetParent(null);
        }
        else
        {
            transform.localScale = startScale;
        }
        if(DamageAnimType.BossDamage == damageAnimType)
        {
            yield return new WaitForSeconds(1f);
        }
        else if(DamageAnimType.Damage == damageAnimType)
        {
            yield return new WaitForSeconds(0.45f);
        }
        playerDamageCo = null;
        isDamageing = false;
        if (isDirectKill || health <= 0)
        {
            if (!_respawningInitiated)
            {
                _respawningInitiated = true;
                Death(isDirectKill);
            }
        }
    }
    
    public IEnumerator JumpAttackResponce()
    {
        if (!isJumpDisenable)
        {
            isJumpDisenable = true;
            CustumJump(1.8f, 3);
            float time = 0.4f;
            while (time > 0)
            {
                time -= Time.deltaTime;
                currentMovement.x = lastMoveingDirection.x;
                currentMovement.z = lastMoveingDirection.z;
                HandleMovement();
                yield return null;
            }
            isJumpDisenable = false;
        }
    }
    
    public void Respawn()
    {
        InputController.IsMoving = false;
        isDead = false;
        isDamageing = false;
        GameManager.instance.Player.animationController.StopRunningVFX();
        Debug.Log("Respawn");
        health = maxHealth;
        isRespawned = false;
        characterController.enabled = false;
        animationController.OnJumpLanded();
        if (checker.checkPointIndex > 0)
        {
            transform.position = checker.playerPosition + (Vector3.up * 5f);
            transform.rotation = checker.currentRotation;
        }
        else if (checker.checkPointIndex <= 0)
        {
            transform.rotation = _initialRot;
            transform.position = _initialPos + (Vector3.up * 5f);
        }
        
        PlayerStats.Instance.obj.Health = (int)health;
        PlayerStats.Instance.obj.Life = life;
        if (GameObject.Find("UI") != null)
        {
            _playerHealthUI((int)health);
            _playerLifeUI(life);
        }
        
        _respawningInitiated = false;
    }

    public void CustumJump(float height, float distance)
    {
        //GameManager.instance.Player.animationController.StopRunningVFX();
        JumpCount =1;
        currentMovement.y = height;

        characterController.Move(currentMovement * distance * Time.deltaTime);

        CustomJumpAnimation();
    }

    public void CustomJumpAnimation()
    {
        animationController.OnJump(0);
    }

    public void StartCustomRunAnimation()
    {
        animationController.isAutoRunning = true;
    }

    public void EndCustomRunAnimation()
    {
        animationController.isAutoRunning = false;
    }

    public void AddLife()
    {
        life++;
        PlayerStats.Instance.obj.Life++;
        _playerLifeUI(life);
        // _sounds.SoundToBeUsed(1, SoundManager.Soundtype.coin, 0.2f);
        SoundManager._soundManager._otherSounds.SoundToBeUsed(1, SoundManager.Soundtype.coin, false, true);
    }

    public void AddHealth()
    {
        PlayerStats.Instance.obj.Health = (int)maxHealth;
        health = maxHealth;
        _playerHealthUI((int)health);
        // _sounds.SoundToBeUsed(1, SoundManager.Soundtype.coin, 0.2f);

        SoundManager._soundManager._otherSounds.SoundToBeUsed(1, SoundManager.Soundtype.coin, false, true);
    }
    

    public async void PlayBossHitVFX()
    {
        GameManager.instance.PlayVFX(bossHitVFX);
        await Task.Delay(1500);
        StopBossHitVFX();
    }

    public void StopBossHitVFX()
    {
        GameManager.instance.StopVFXImediatly(bossHitVFX, 1500);
    }

    public void PlayPushVFX()
    {
        GameManager.instance.PlayVFX(enemyPushVFX);
    }

    public void StopPushVFX()
    {
        GameManager.instance.StopVFXImediatly(enemyPushVFX);
    }

    public void PlayMagnetPowerVFX()
    {
        GameManager.instance.PlayVFX(magnetPowerUpVFX);
    }

    public void StopMagnetPowerVFX()
    {
        GameManager.instance.StopVFXImediatly(magnetPowerUpVFX,0);
    }
    
    public void PlayImpactVFX(int timeInMiliseconds)
    {
        GameManager.instance.PlayVFX(impactVFX);
        GameManager.instance.StopVFX(impactVFX, timeInMiliseconds);
    }

    public void PlayFireImpactVFX(int timeInMilliseconds)
    {
        GameManager.instance.PlayVFX(fireImpactVFX);
        GameManager.instance.StopVFX(fireImpactVFX, timeInMilliseconds);
    }

    public void EnableRope()
    {
        rope.SetActive(true);
        Invoke("DisableRope", 1.5f);
    }

    public void DisableRope()
    {
        rope.SetActive(false);
    }
}
