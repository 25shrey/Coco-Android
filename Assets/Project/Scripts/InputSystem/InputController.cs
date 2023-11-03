using Game.BaseFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour
{
    public bool isAllowToSwitchCam;

    public static UnityAction OnCameraSwitch;
    public static UnityAction OnRun;
    public static UnityAction OnPrimaryAttack;
    //public static UnityAction pauseMenuAction;
    public static UnityAction onTurtleThrowAndPickup;
    public static UnityAction OnPlayerPunch;
    public static UnityAction magnetPowerUpAction;
    public static UnityAction shieldPowerUpAction;
    
    
    //Dev
    public static UnityAction screenshot;
    
    // Add Controller Option in Settings UI
    public static UnityAction<string> onControllerFound;
    public static UnityAction<InputType> onGameStartControlSwitch;


    static Vector2 MovementInputs;
    static Vector3 MovementDirection;
    static Vector2 ViewInputs;

    public static bool IsMoving;
    static bool IsJumping;
    static bool IsWalking;
    static bool IsSwitchingCamera;
    static bool IsPrimaryAttackActive;
    static bool pauseMenu;

    [SerializeField] PlayerInput playerInput;

    [Header("Basic Inputs")]
    [SerializeField] private InputActionReference jumpInputAction;
    [SerializeField] private InputActionReference switchCameraAction;
    [SerializeField] private InputActionReference movementActions;
    [SerializeField] private InputActionReference rotateViewActions;
    [SerializeField] private InputActionReference walkAction;
    [SerializeField] private InputActionReference primaryAttack;
    [SerializeField] private InputActionReference turtleThrowAndGrab;
    [SerializeField] private InputActionReference punchAction;
    //[SerializeField] private InputActionReference screenShotAction;

    [Header("PowerUp Inputs")]
    [SerializeField] private InputActionReference magnetPowerUpActions;
    [SerializeField] private InputActionReference shieldPowerUpActions;
    
    private InputType currentInput;
    private string gamepadString = "Gamepad";
    
    // Input action 
    private InputActionMap menuInputAction;
    private InputActionMap playerInputAction;

    private void OnEnable()
    {
        GS.onControllerSwitch += ChangeController;
        InputSystem.onDeviceChange += OnDeviceConnected;
    }




    private void OnDisable()
    {
        GS.onControllerSwitch -= ChangeController;
        InputSystem.onDeviceChange -= OnDeviceConnected;
    }

    private void OnInputEnable()
    {
        jumpInputAction.action.performed += Jump_performed;
      
        switchCameraAction.action.started += SwitchCamera_started;
        switchCameraAction.action.canceled += SwitchCamera_canceled;

        movementActions.action.started += OnMovementStarted;
        movementActions.action.performed += OnMovementStarted;
        movementActions.action.canceled += OnMovement_canceled;


        rotateViewActions.action.started += OnRotateView;
        rotateViewActions.action.performed += OnRotateView;
        rotateViewActions.action.canceled += OnRotateView;

        walkAction.action.started += OnWalk_started;
        walkAction.action.canceled += OnWalk_canceled;

        primaryAttack.action.started += OnPrimaryAttack_started;
        primaryAttack.action.started += OnPrimaryAttack_canceled;

        punchAction.action.started += OnPunch;
        punchAction.action.canceled += OnPunch_Cancelled;

        turtleThrowAndGrab.action.started += OnTurtleThrowAndGrab;

        magnetPowerUpActions.action.started += MagnetPowerUpActionPreformed;
        shieldPowerUpActions.action.started += ShieldPowerUpActionPerformed;
        

        //screenShotAction.action.started += OnScreenShotButtonPerformed;
    }

    private void OnInputDisable()
    {
        jumpInputAction.action.performed -= Jump_performed;

        switchCameraAction.action.started -= SwitchCamera_started;
        switchCameraAction.action.canceled -= SwitchCamera_canceled;

        movementActions.action.started -= OnMovementStarted;
        movementActions.action.performed -= OnMovementStarted;
        movementActions.action.canceled -= OnMovement_canceled;


        rotateViewActions.action.started -= OnRotateView;
        rotateViewActions.action.performed -= OnRotateView;
        rotateViewActions.action.canceled -= OnRotateView;

        walkAction.action.started -= OnWalk_started;
        walkAction.action.canceled -= OnWalk_canceled;

        primaryAttack.action.started -= OnPrimaryAttack_started;
        primaryAttack.action.started -= OnPrimaryAttack_canceled;

        punchAction.action.started -= OnPunch;
        punchAction.action.canceled -= OnPunch_Cancelled;

        turtleThrowAndGrab.action.started -= OnTurtleThrowAndGrab;

        magnetPowerUpActions.action.started -= MagnetPowerUpActionPreformed;
        shieldPowerUpActions.action.started -= ShieldPowerUpActionPerformed;
        
        //screenShotAction.action.started -= OnScreenShotButtonPerformed;
    }


    

    private void Start()
    {
        SavedDataHandler.Instance.LoadControllerRebingData();
        currentInput = SavedDataHandler.Instance.LoadInputType();
        HandleInputOnGameStart();
    }

    private void Jump_performed(InputAction.CallbackContext obj)
    {
        GameManager.instance.Player.HandleJump();
    }


    private void SwitchCamera_started(InputAction.CallbackContext obj)
    {
        if (isAllowToSwitchCam)
        {
            IsSwitchingCamera = true;
            OnCameraSwitch?.Invoke();
        }
    }

    private void SwitchCamera_canceled(InputAction.CallbackContext obj)
    {
        if (isAllowToSwitchCam)
        {
            IsSwitchingCamera = false;
        }
    }


    private void OnPrimaryAttack_started(InputAction.CallbackContext obj)
    {
        if (GameManager.instance.currentGameState != GameStates.alive)
        {
            return;
        }
        IsPrimaryAttackActive = true;

        OnPrimaryAttack?.Invoke();
    }
    private void OnPrimaryAttack_canceled(InputAction.CallbackContext obj)
    {
        IsPrimaryAttackActive = false;
    }

    private void OnWalk_started(InputAction.CallbackContext obj)
    {
        IsWalking = true;

        OnRun?.Invoke();
    }
    private void OnWalk_canceled(InputAction.CallbackContext obj)
    {
        IsWalking = false;
    }

    private void OnMovementStarted(InputAction.CallbackContext context)
    {
        if (GameManager.instance.currentGameState != GameStates.alive)
        {
            MovementDirection = Vector3.zero;
            IsMoving = false;
            return;
        }
        MovementInputs = context.ReadValue<Vector2>();

        MovementDirection.x = MovementInputs.x;
        MovementDirection.z = MovementInputs.y;

        IsMoving = true;
        GameManager.instance.Player.animationController.StartRunningVFX();
    }
    private void OnMovement_canceled(InputAction.CallbackContext context)
    {
        if (GameManager.instance.currentGameState != GameStates.alive)
        {
            MovementDirection = Vector3.zero;
            IsMoving = false;
            return;
        }
        //MovementInputs = context.ReadValue<Vector2>();
        MovementInputs = Vector3.zero;

        MovementDirection.x = MovementInputs.x;
        MovementDirection.y = 0;
        MovementDirection.z = MovementInputs.y;

        IsMoving = false;
        GameManager.instance.Player.animationController.StopRunningVFX();
    }

    private void OnRotateView(InputAction.CallbackContext context)
    {
        ViewInputs = context.ReadValue<Vector2>();
    }

    private void OnTurtleThrowAndGrab(InputAction.CallbackContext context)
    {
        /*Turtle Throw */
        onTurtleThrowAndPickup?.Invoke();
    }

    private void OnPunch(InputAction.CallbackContext context)
    {
        if (GameManager.instance.currentGameState != GameStates.alive)
        {
            return;
        }
        IsPrimaryAttackActive = true;
        OnPlayerPunch?.Invoke();
    }

    private void OnPunch_Cancelled(InputAction.CallbackContext context)
    {
        IsPrimaryAttackActive = false;
    }

    private void MagnetPowerUpActionPreformed(InputAction.CallbackContext context)
    {
        magnetPowerUpAction?.Invoke();
    }

    private void ShieldPowerUpActionPerformed(InputAction.CallbackContext context)
    {
        shieldPowerUpAction?.Invoke();
    }

    private void OnScreenShotButtonPerformed(InputAction.CallbackContext context)
    {
        Debug.Log("Inside _ OnScreenShotButtonPerformed");
        screenshot?.Invoke();
    }

    public static bool IsJumpPerformed()
    {
        return IsJumping;
    }

    public static bool IsCameraSwitchPerformed()
    {
        return IsSwitchingCamera;
    }

    public static bool IsWalkActive()
    {
        return IsWalking;
    }

    public static bool IsMovingInput()
    {
        return IsMoving;
    }


    public static float GetMovementXAxis()
    {
        return MovementInputs.x;
    }

    public static float GetMovementYAxis()
    {
        return MovementInputs.y;
    }

    public static Vector2 GetMovementInputAxis()
    {
        return MovementInputs;
    }

    public static Vector3 GetMovementVector()
    {
        return MovementDirection;
    }

    public static float GetRotationXAxis()
    {
        return ViewInputs.x;
    }

    public static float GetRotationYAxis()
    {
        return ViewInputs.y;
    }

    public static Vector2 GetRotationAxis()
    {
        return ViewInputs;
    }

    public static bool GetPauseMenu()
    {
        return pauseMenu;
    }

    public static void SetPauseMenu(bool val)
    {
        pauseMenu = val;
    }

    public void EnableInput()
    {
        playerInput.currentActionMap.Enable();
        OnInputEnable();
    }

    public bool ReturnState()
    {
        return playerInput.currentActionMap.enabled;
    }

    public void DisableInput()
    {
        playerInput.currentActionMap.Disable();
        OnInputDisable();
    }

    public void HandleInputOnGameStart()
    {
        // Check For save data if its controller than switch to controller otherwise keyboard
        var devices = InputSystem.devices.ToList();
        if (currentInput != InputType.KeyboardAndMouse)
        {
            // Gamepad
            InputDevice input = devices.Find(x => x is Gamepad);
            if (input != null)
            {
                Debug.Log(" Inside If Gampad Found");
                onControllerFound?.Invoke(gamepadString);
                SwitchInputToGamepad();
            }
            else
            {
                input = devices.Find(x => x is Keyboard);
                if (input != null)
                {
                    SwitchInputToKeyboard();
                }
                else
                {
                    Debug.LogError("please connect a controller and keyboard");
                }
            }
        }
        else
        {
            InputDevice input = devices.Find(x => x is Keyboard);
            if (input != null)
            {
                SwitchInputToKeyboard();
                input = devices.Find(x => x is Gamepad);
                if (input != null)
                {
                    onControllerFound?.Invoke(gamepadString);
                }
            }
            else
            {
                input = devices.Find(x => x is Gamepad);
                if (input != null)
                {
                    Debug.Log(" Inside else Gamepad Found");
                    onControllerFound?.Invoke(gamepadString);
                    SwitchInputToGamepad();
                }
                else
                {
                    Debug.LogError("please connect a controller and keyboard");
                }
            }
        }
        onGameStartControlSwitch?.Invoke(currentInput); 
    }


    public void OnDeviceConnected(InputDevice device, InputDeviceChange change)
    {
       // Debug.Log($"device name: {device} change: {change}");
        if (device is Gamepad)
        {
            switch (change)
            {
                case InputDeviceChange.Added:
                case InputDeviceChange.Reconnected:
                    // Add option for gamepad in UI
                    onControllerFound?.Invoke(gamepadString);
                    break;

                case InputDeviceChange.Removed:
                case InputDeviceChange.Disconnected:
                    if (currentInput == InputType.Controller)
                    {
                        // change to Keyboard
                        SwitchInputToKeyboard();
                    }
                    break;
            }
        }

    }

    [ContextMenu("Switch to Gamepad")]
    public void SwitchInputToGamepad()
    {
        playerInput.SwitchCurrentControlScheme("XboxController", Gamepad.current);
        currentInput = InputType.Controller;
    }
    
    [ContextMenu("Switch to Keyboard")]
    public void SwitchInputToKeyboard()
    {
        playerInput.SwitchCurrentControlScheme("Keyboard", Keyboard.current);
        currentInput = InputType.KeyboardAndMouse;
    }

    public void ChangeController(int index)
    {
        switch (index)
        {
            case 0: SwitchInputToKeyboard();
                currentInput = InputType.KeyboardAndMouse;
                break;
            
            case 1: SwitchInputToGamepad();
                currentInput = InputType.Controller;
                break;
        }
      //  SavedDataHandler.Instance.SaveInputType(currentInput);
    }

    /*void PauseMenuTrigger()
    {
        if (playerInput.currentActionMap.enabled)
        {
            if (UIController.Instance.LastScreenOnTheList == ScreenType.Gameplay ||
                UIController.Instance.LastScreenOnTheList == ScreenType.Cloud)
            {
                playerInput.currentActionMap.Disable();
                pauseMenuAction?.Invoke();
            }
        }
        else if (!playerInput.currentActionMap.enabled)
        {
            if (UIController.Instance.LastScreenOnTheList == ScreenType.Pause)
            {
                playerInput.currentActionMap.Enable();
                pauseMenuAction?.Invoke();
            }
        }
    }*/



    /*private void Update()
    {
        if (Keyboard.current[Key.Escape].wasPressedThisFrame && !pauseMenu)
        {
            PauseMenuTrigger();
        }
    }*/
}


public enum InputType
{
    KeyboardAndMouse,
    Controller,
}
