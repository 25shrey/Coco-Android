using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Game.BaseFramework;
using UnityEngine.EventSystems;

public class MenuInput : Singleton<MenuInput>
{
    public bool isAllowedToRotate;
    public float rotateSpeed;

    public delegate void OnBackButtonDelegate();
    public event OnBackButtonDelegate onBack;

    public delegate void ScrollRight();
    public delegate void ScrollLeft();

    public static event ScrollRight _scrollRight;
    public static event ScrollLeft _scrollLeft;
    
    private Vector2 startPos;
    public Vector2 curPos;
    private float Amount;
    private MenuAction controls;

    private static bool enableUi;

    private void OnEnable()
    {
        controls = new MenuAction();
        controls.Mouse.Enable();
        controls.Mouse.MouseClick.started += MouseSelectStarted;
        controls.Mouse.MouseClick.canceled += MouseUnselectCancelled;

        controls.Mouse.MousePosition.started += MouseDragStarted;
        controls.Mouse.MousePosition.canceled += MouseDragCancel;
        controls.Mouse.MousePosition.performed += MouseDragPerform;
        
        controls.UIInputs.Enable();

        controls.ControllerScroll.Enable();

        controls.UIInputs.Back.started += OnBackButtonPressed;

        controls.ControllerScroll.ScrollerRight.performed += OnRightScroll;
        controls.ControllerScroll.ScrollerLeft.performed += OnLeftScroll;
    }

    private void OnDisable()
    {
        controls.Mouse.MouseClick.started -= MouseSelectStarted;
        controls.Mouse.MouseClick.canceled -= MouseUnselectCancelled;

        controls.Mouse.MousePosition.started -= MouseDragStarted;
        controls.Mouse.MousePosition.canceled -= MouseDragCancel;
        controls.Mouse.MousePosition.performed -= MouseDragPerform;
        controls.Mouse.Disable();
        
        controls.UIInputs.Back.started -= OnBackButtonPressed;

        controls.ControllerScroll.ScrollerRight.performed -= OnRightScroll;
        controls.ControllerScroll.ScrollerLeft.performed -= OnLeftScroll;

        controls.UIInputs.Disable();
        controls.ControllerScroll.Disable();
    }

    private void MouseSelectStarted(InputAction.CallbackContext obj)
    {
        if(UIController.Instance.CurrentScreensLength > 0)
        {
            var screen = UIController.Instance.getCurrentScreen();

            if (screen == ScreenType.Customization)
            {
                isAllowedToRotate = true;
            }
        }
    }

    private void MouseDragStarted(InputAction.CallbackContext obj)
    {
        if(isAllowedToRotate)
        {
            startPos = obj.ReadValue<Vector2>();

            Amount = 0;
        }
    }

    private void MouseDragPerform(InputAction.CallbackContext context)
    {
        if (isAllowedToRotate)
        {
            curPos = context.ReadValue<Vector2>();
            if(curPos.x == 0)
            {
                Amount = 0;
            }
            else if (curPos.x < startPos.x)
            {
                Amount = 1f;
            }
            else if(curPos.x > startPos.x)
            {
                Amount = -1f;
            }
        }
    }

    private void MouseDragCancel(InputAction.CallbackContext context)
    {
        if (isAllowedToRotate)
        {
            isAllowedToRotate = false;
        }
    }

    public float RotationAmount()
    {
        return Amount;
    }

    private void MouseUnselectCancelled(InputAction.CallbackContext obj)
    {
        if (isAllowedToRotate)
        {
            isAllowedToRotate = false;
        }
    }

    private void OnBackButtonPressed(InputAction.CallbackContext obj)
    {
        if(GameController.Instance.SceneIndex > 2 && !enableUi)
        {
            onBack?.Invoke();
        }
    }

    public static bool ReturnState()
    {
        return enableUi;
    }

    public static void EnableUiInput()
    {
        enableUi = false;
    }

    public static void DisableInput()
    {
        enableUi = true;
    }

    private void OnRightScroll(InputAction.CallbackContext obj)
    {
       // _scrollRight();
    }

    private void OnLeftScroll(InputAction.CallbackContext obj)
    {
       // _scrollLeft();
    }
}
