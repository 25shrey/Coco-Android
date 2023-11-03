using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyMovement : BaseEnemy
{
    #region PUBLIC_VARS

    public WalkArea walkArea;
    [HideInInspector] public Vector3 currentPos;
    [HideInInspector] public Vector3 targetPoint;
    public Coroutine moveCo;
    public Coroutine rotateCo;
    public Coroutine castumGravity;
    
    #endregion

    #region PRIVATE_VARS

    #endregion

    #region UNITY_CALLBACKS

    public override void Start()
    {
        base.Start();
        walkArea.SetPosition();
        for (int i = 0; i < walkArea.points.Count; i++)
        {
            Destroy(walkArea.points[i].point.gameObject);
        }
    }

    #endregion

    #region PUBLIC_FUNCTIONS
    
    
    public virtual IEnumerator CastumGravity(float Grevity)
    {
        while (true)
        {
            yield return null;
            rb.AddForce(new Vector3(0,Grevity*Time.deltaTime,0));
        }
    }
    
    public void SetNextMoveTarget(bool isFirstTarget)
    {
        WalkPoint point;
        if (isFirstTarget)
        {
            point = walkArea.points[0];
            currentPos = point.position;
            transform.position = currentPos;
            targetPoint = point.nextPositions;
        }
        else
        {
            point = walkArea.points.Find(x => x.position == targetPoint);
            targetPoint =  point.nextPositions;
            currentPos = point.position;
        }
    }

    public IEnumerator Move(Vector3 targetPoint)
    {
        Vector3 currentPos = transform.position;
        float distance = Vector3.Distance(currentPos, targetPoint);
        float time = distance / speed;
        float totalTime = time;
        while (time > 0)
        {
            time -= Time.deltaTime;
            transform.position =
                Vector3.Lerp(currentPos, targetPoint, (totalTime - time) / totalTime);
            yield return null;
        }
        transform.position = targetPoint;
    }
    
    public IEnumerator Rotate(float angle,bool directRotate=false)
    {
        Vector3 startEulerAngles = transform.eulerAngles;
        Vector3 endEulerAngles = new Vector3(startEulerAngles.x,
            angle, startEulerAngles.z);
        if (directRotate)
        {
            transform.eulerAngles = endEulerAngles;
        }
        else
        {
            float totalTime = Mathf.Abs(startEulerAngles.y - endEulerAngles.y) / rotateSpeed, time = totalTime;
            while (time > 0)
            {
                time -= Time.deltaTime;
                transform.eulerAngles =
                    Vector3.Lerp(startEulerAngles, endEulerAngles, (totalTime - time) / totalTime);
                yield return null;
            }
        }
        rotateCo = null;
    }
    
    public float GetMoveingAngle(Vector3 currentPos,Vector3 targetPos)
    {
        Vector2 start = new Vector2(currentPos.x, currentPos.z);
        Vector2 end = new Vector2(targetPos.x, targetPos.z);
        Vector2 movingDirection = (end - start).normalized;
        float angle;
        if (movingDirection == Vector2.left)
        {
            angle= 180;
        }
        else
        {
            if (movingDirection.y < 0)
            {
                angle = Vector3.Angle(new Vector2(1, 0), movingDirection);
            }
            else
            {
                angle = 360 - Vector3.Angle(new Vector2(1, 0), movingDirection);
            }
        }
        if (transform.eulerAngles.y > 360)
        {
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y - 360, transform.eulerAngles.z);
        }
        else if(transform.eulerAngles.y <0)
        {
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y + 360, transform.eulerAngles.z);
        }
        float angleDiff = Mathf.Abs( transform.eulerAngles.y - angle);
        if (angleDiff > 180)
        {
            if (angle < 180)
            {
                angle += 360;
            }
            else
            {
                angle -= 360;
            }
        }
        return angle;
    }

    
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

[Serializable]
public class WalkArea
{
    public List<WalkPoint> points;

    public void SetPosition()
    {
        for (int i = 0; i < points.Count; i++)
        {
            points[i].SetPoint();
        }
    }
}

[Serializable]
public class WalkPoint
{
    public Transform point;
    public Transform targetPos;
    [HideInInspector] public Vector3 position;
    [HideInInspector] public Vector3 nextPositions;

    public void SetPoint()
    {
        position = point.position;
        nextPositions=targetPos.position;
    }
}