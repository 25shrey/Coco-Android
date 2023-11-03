using System;
using System.Collections;
using System.Collections.Generic;
using GameCoreFramework;
using UnityEngine;
using UnityEngine.VFX;


public class PlayerAnimationController : MonoBehaviour
{
    Animator animator;
    public VisualEffect jumpLandingEffect;
    public VisualEffect highJumpEffect;
   
    /*public Transform boneStructure;
    public Transform newSkinMeshOutfitParent;
    public Transform newMeshOutfitParent;*/
    public CocoClothData clothDataReference;


    float current, Input;

    const float WALKMULTIPLIER = 0.5f, RUNMULTIPLIER = 1f;

    public bool isJumpAnimating;
    public bool isAutoRunning;

    int MovementHash;
    int JumpHash;
    int JumpCountHash;
    private int ThrowHash;
    private int BossDamage;
    private int BeeDamage;
    private int Damage;
    private int LeftPunch;
    private int RightPunch;
    private int Death;
    private int FallDeath;
    private int DeathExit;
    
    //Trutle Throw//
    // private int ThrowTurtle;
    // private int PickTurtle;
    // private int PickTurtleFail;


    private void Awake()
    {
        animator = GetComponent<Animator>();
        MovementHash = Animator.StringToHash("Movement");
        JumpHash = Animator.StringToHash("Jump");
        JumpCountHash = Animator.StringToHash("JumpCount");
        ThrowHash = Animator.StringToHash("PrimaryAttack");
        Damage = Animator.StringToHash("Damage");
        BeeDamage = Animator.StringToHash("BeeDamage");
        BossDamage = Animator.StringToHash("BossDamage");
        RightPunch = Animator.StringToHash("RightPunch");
        LeftPunch = Animator.StringToHash("LeftPunch");
        Death = Animator.StringToHash("Death");
        FallDeath = Animator.StringToHash("FallDeath");
        DeathExit = Animator.StringToHash("DeathExit");
        
        //Trutle Throw//
        // ThrowTurtle = Animator.StringToHash("TurtleThow");
        // PickTurtle = Animator.StringToHash("TurtlePick");
        // PickTurtleFail = Animator.StringToHash("NotPicked");
    }

    private void Start()
    {
        if (jumpLandingEffect != null)
        {
            jumpLandingEffect.transform.SetParent(null);
        }
        if (highJumpEffect != null)
        {
            highJumpEffect.transform.SetParent(null);
        }
        //Debug.Log("runningSmokeTrail.culled"+ runningSmokeTrail.culled);
    }

    private void Update()
    {
        if (!isAutoRunning)
        {
            HandleMoveAnimation();
        }
        else
        {
            AutoRunAnimation();
        }
    }

    public void OnJump(int JumpCount)
    {
        highJumpEffect.transform.position = transform.position;
        if (JumpCount==1)
        {
            highJumpEffect.Play();
        }
        animator.SetInteger(JumpCountHash, JumpCount);
        animator.SetBool(JumpHash, true);
    }

    public void OnJumpLanded()
    {
        if (animator.GetBool(JumpHash))
        {
            if (InputController.IsMovingInput())
            {
                StartRunningVFX();
            }
            JumpLandingVFX(); 
        }
        animator.SetBool(JumpHash, false);

    }
    public void OnJumpCountReset(int JumpCount)
    {
        animator.SetInteger(JumpCountHash, JumpCount);
    }

    public void AttackAnimation(PlrAttackType attackType)
    {
        switch (attackType)
        {
            case  PlrAttackType.ThrowFireBall:
                animator.SetTrigger(ThrowHash);
                break;
            case  PlrAttackType.LeftPunch:
                animator.SetTrigger(LeftPunch);
                break;
            case  PlrAttackType.RightPunch:
                animator.SetTrigger(RightPunch);
                break;
        }
        
    }

    //Turtle Throw//
    // public void PickUpAndThrow(string state)
    // {
    //     switch (state)
    //     {
    //         case "Pick":
    //             {
    //                 animator.SetTrigger(PickTurtle);
    //                 break;
    //             }
    //         case "Throw":
    //             {
    //                 animator.SetTrigger(ThrowTurtle);
    //                 break;
    //             }
    //         case "Fail":
    //             {
    //                 animator.SetTrigger(PickTurtleFail);
    //                 break;
    //             }
    //     }
    // }

    public void DeathAnimationShow(bool isFallDeath)
    {
        if (isFallDeath)
        {
            // GameManager.instance.Player._sounds.SoundToBeUsed(5, SoundManager.Soundtype.player, 0.5f);
            SoundManager._soundManager._playerSounds.SoundToBeUsed(5, SoundManager.Soundtype.player, false, true);
            animator.SetTrigger(FallDeath);
        }
        else
        {
            // GameManager.instance.Player._sounds.SoundToBeUsed(4, SoundManager.Soundtype.player, 0.5f);
            animator.SetTrigger(Death);
            SoundManager._soundManager._playerSounds.SoundToBeUsed(4, SoundManager.Soundtype.player, false, true);
        }
    }
    
    public async void StopDeathAnimation()
    {
        animator.SetTrigger(DeathExit);
    }
    
    
    public void DamageAnimationShow(DamageAnimType damageAnimType)
    {
        switch (damageAnimType)
        {
            case DamageAnimType.Damage:
                animator.SetTrigger(Damage);
                break;
            case DamageAnimType.BeeDamage:
                animator.SetTrigger(BeeDamage);
                break;
            case DamageAnimType.BossDamage:
                animator.SetTrigger(BossDamage);
                break;
        }
    }
    
    public void HandleMoveAnimation()
    {
        Input = 0;
        //if (InputController.IsMovingInput() && GameManager.instance.currentGameState == GameStates.alive && GameManager.instance.Player.characterController.enabled)
        if (ControllerMobile.IsMoving()
            && GameManager.instance.currentGameState == GameStates.alive 
            && GameManager.instance.Player.characterController.enabled)
        {
            Input = RUNMULTIPLIER;

            if (InputController.IsWalkActive())
            {
                Input = WALKMULTIPLIER;
            }
        }
        current = Mathf.Lerp(current, Input, 10 * Time.deltaTime);
        animator.SetFloat(MovementHash, current);
    }

    public void AutoRunAnimation()
    {
        Input = RUNMULTIPLIER;
        
        current = Mathf.Lerp(current, Input, 10 * Time.deltaTime);

        animator.SetFloat(MovementHash, current);
    }

    public void StartRunningVFX()
    {
        if (GameManager.instance.Player.JumpCount==0 && !GameManager.instance.Player.isDead)
        {
            if(InputController.IsMovingInput())
            {
                int index = 0;
                if (!GameManager.instance.Player.OnBridge)
                {
                    index = 10;
                }
                else
                {
                    index = 11;
                }
                SoundManager._soundManager._playerSounds.SoundToBeUsed(index, SoundManager.Soundtype.player, true, true);
            }
        }
    }
    public void StopRunningVFX()
    {
        if(!GameManager.instance.Player.isDead)
        {
            SoundManager._soundManager._playerSounds.SoundToBeUsed(10, SoundManager.Soundtype.player, false, false);
        }
    }

    public void JumpLandingVFX()
    {
        jumpLandingEffect.transform.position = transform.position;
        jumpLandingEffect.Play();
    }
}

public enum PlrAttackType
{
    ThrowFireBall,
    LeftPunch,
    RightPunch
}

public enum DamageAnimType
{
    Damage,
    BeeDamage,
    BossDamage
}
