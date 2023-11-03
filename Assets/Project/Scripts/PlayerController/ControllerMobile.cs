using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerMobile : MonoBehaviour
{
    #region PUBLIC_VARS

    [SerializeField]
    private FixedJoystick joystick;

    static bool _movement;

    static float x;
    static float z;

    #endregion

    #region PRIVATE_VARS

    #endregion

    #region UNITY_CALLBACKS

    private void Update()
    {
        if(GameController.Instance.SceneIndex > 2)
        {
            Movement();
            MovementTowardsX();
            MovementTowardsZ();
        }
    }

    public static bool IsMoving()
    {
        return _movement;
    }

    void Movement()
    {
        if (joystick.Horizontal != 0 || joystick.Vertical != 0)
        {
            _movement =  true;
        }
        else
        {
            _movement = false;
        }
    }


    public static float XAxis()
    {
        return x;
    }

    public static float ZAxis()
    {
        return z;
    }

    void MovementTowardsX()
    {
        x = joystick.Vertical;
    }

    void MovementTowardsZ()
    {
        z = joystick.Horizontal;
    }

    public void HandleJump()
    {
        if (GameController.Instance.SceneIndex > 2)
        {
            GameManager.instance.Player.HandleJump();
        }
    }

    public void FireballAttack()
    {
        if (GameController.Instance.SceneIndex > 2)
        {
            GameManager.instance.Player.AttackFireBall();
        }
    }

    #endregion

    #region PUBLIC_FUNCTIONS

    #endregion

    #region PRIVATE_FUNCTIONS

    #endregion

    #region CO-ROUTINES

    #endregion

    #region EVENT_HANDLERS

    #endregion

    #region UI_CALLBACKS

    #endregion
}
