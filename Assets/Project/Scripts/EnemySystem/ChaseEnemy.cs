using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public abstract class ChaseEnemy : EnemyMovement
{
    #region PUBLIC_VARS

    public ChessArea chessArea;
    [HideInInspector] public float chaseSpeed;
    public float minChessSpeed;
    public float maxChessSpeed;
    public float acceleration;
    public float accuracyInTime;
    public Coroutine chaseCO;
    [HideInInspector] public bool playerInRange;
    [HideInInspector] public Vector3 playerPos;
    
    #endregion

    #region PRIVATE_VARS

    #endregion

    #region UNITY_CALLBACKS

    public override void Start()
    {
        base.Start();
        chessArea.SetArea();
        chaseSpeed = minChessSpeed;
    }

    public override void Update()
    {
        if (!isDead && !GameController.Instance.clouds.isShowingClouds) //&& GameManager.instance.currentGameState == GameStates.alive)
        {
            playerPos = player.transform.position;
            //if (PlayerInRange(playerPos) && !player.isDead)
            //{
            //    if (!playerInRange)
            //    {
            //        PlayerEnterOnRange();
            //        playerInRange = true;
            //    }
            //}
            //else
            //{
            //    if (playerInRange)
            //    {
            //        PlayerExitOnRange();
            //        playerInRange = false;
            //    }
            //}

            if (PlayerInStartRange(playerPos))
            {
                if (!playerInRange)
                {
                    PlayerEnterOnRange();
                    playerInRange = true;
                }
            }
            else if (!PlayerInEndRange(playerPos))
            {
                if (playerInRange)
                {
                    PlayerExitOnRange();
                    playerInRange = false;
                }
            }
        }
    }


    #endregion

    #region PUBLIC_FUNCTIONS
    
    public override void SetRange(Vector3 start, Vector3 end)
    {
        chessArea.startPos = start;
        chessArea.endPos = end;
        chessArea.startingRange.startPos = start;
        chessArea.endingRange.startPos = start;
        chessArea.startingRange.endPos = end;
        chessArea.endingRange.endPos = end;
    }

    public virtual void PlayerEnterOnRange()
    {
        if (castumGravity == null)
        {
            castumGravity = StartCoroutine(CastumGravity(-2000));
        }
        chaseSpeed = minChessSpeed;
        if (chaseCO != null)
        {
            StopCoroutine(chaseCO);
        }
        chaseCO = StartCoroutine(Chase(GetTargetPos()));
    }
    
    public virtual void PlayerExitOnRange()
    {
        if (castumGravity != null)
        {
            StopCoroutine(castumGravity);
            castumGravity = null;
        }
        if (chaseCO != null)
        {
            StopCoroutine(chaseCO);
            chaseCO = null;
        }
        if (rotateCo != null)
        {
            StopCoroutine(rotateCo);
            rotateCo = null;
        }
    }

    public bool PlayerInRange(Vector3 playerPos)
    {
        if (playerPos.x < chessArea.startPos.x && playerPos.y > chessArea.startPos.y &&
            playerPos.z > chessArea.startPos.z)
        {
            if (playerPos.x > chessArea.endPos.x && playerPos.y < chessArea.endPos.y &&
                playerPos.z < chessArea.endPos.z)
            {
                return true;
            }
        }
        return false;
    }
    
    public bool PlayerInStartRange(Vector3 playerPos)
    {
        if (playerPos.x < chessArea.startingRange.startPos.x && playerPos.y > chessArea.startingRange.startPos.y &&
            playerPos.z > chessArea.startingRange.startPos.z)
        {
            if (playerPos.x > chessArea.startingRange.endPos.x && playerPos.y < chessArea.startingRange.endPos.y &&
                playerPos.z < chessArea.startingRange.endPos.z)
            {
                return true;
            }
        }
        return false;
    }
    
    public bool PlayerInEndRange(Vector3 playerPos)
    {
        if (playerPos.x < chessArea.endingRange.startPos.x && playerPos.y > chessArea.endingRange.startPos.y &&
            playerPos.z > chessArea.endingRange.startPos.z)
        {
            if (playerPos.x > chessArea.endingRange.endPos.x && playerPos.y < chessArea.endingRange.endPos.y &&
                playerPos.z < chessArea.endingRange.endPos.z)
            {
                return true;
            }
        }
        return false;
    }

    public virtual Vector3 GetTargetPos()
    {
        return new Vector3(player.transform.position.x,
            transform.position.y, player.transform.position.z);
    }
    
    #endregion

    #region PRIVATE_FUNCTIONS

    #endregion

    #region CO-ROUTINES

    public IEnumerator Chase(Vector3 endPos, bool isWaitForRotate = true)
    {
        float angle = GetMoveingAngle(transform.position, endPos);
        if (isWaitForRotate)
        {
            yield return rotateCo = StartCoroutine(Rotate(angle));
        }
        else
        {
            rotateCo = StartCoroutine(Rotate(angle));
        }
        chaseSpeed -= (Mathf.Abs(transform.eulerAngles.y - angle) / 60) * acceleration * accuracyInTime;
        if (chaseSpeed < minChessSpeed)
        {
            chaseSpeed = minChessSpeed;
        }
        Vector3 start = transform.position;
        float distance = Vector3.Distance(start, endPos);
        Vector3 direction = (endPos - start).normalized;
        float time = distance / chaseSpeed;
        float totalTime = time;
        float resetTime = 0;
        while (time > 0)
        {
            time -= Time.deltaTime;
            resetTime += Time.deltaTime;
            transform.position += (direction * distance / totalTime) * Time.deltaTime;
            if(enemy != Enemy.bee && GameManager.instance.Player.playerPowerUps.IsPowerUpActive(PowerUpType.Shield))
            {
                transform.position = new Vector3(transform.position.x, start.y, transform.position.z);
            }
            if (chaseSpeed < maxChessSpeed)
            {
                chaseSpeed += acceleration * Time.deltaTime;
            }
            else
            {
                chaseSpeed = maxChessSpeed;
            }
            totalTime = distance / chaseSpeed;
            yield return null;
            if (resetTime > accuracyInTime)
            {
                break;
            }
        }
        if (!isDead)
        {
            chaseCO = StartCoroutine(Chase(GetTargetPos(), false));
        }
        //if (GameManager.instance.currentGameState == GameStates.alive)
        //{
  
        //}
        //else
        //{
        //    rotateCo = StartCoroutine(Rotate(GetMoveingAngle(transform.position, targetPoint), true));
        //    moveCo = StartCoroutine(Move(targetPoint));
        //}
    }


    #endregion

    #region EVENT_HANDLERS

    #endregion

    #region UI_CALLBACKS

    #endregion
}

[Serializable]
public class ChessArea
{
    public Transform startPoint;
    public Transform endPoint;
    [HideInInspector] public Vector3 startPos;
    [HideInInspector] public Vector3 endPos;

    public AreaData startingRange;
    public AreaData endingRange;
    
    
    public void SetArea()
    {
        startingRange.SetArea();
        endingRange.SetArea();
        startPos = startPoint.position;
        endPos = endPoint.position;
    }
}


[Serializable]
public class AreaData
{
    public Transform startPoint;
    public Transform endPoint;
    [HideInInspector] public Vector3 startPos;
    [HideInInspector] public Vector3 endPos;
    
    public void SetArea()
    {
        startPos = startPoint.position;
        endPos = endPoint.position;
    }
}
