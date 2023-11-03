using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RockDownwardsThrowable : BossThrowables
{
    [Header("Rock Parameters")]
    [SerializeField] private Rigidbody rockRigidbody;
    [SerializeField] private float rockForce;
    [SerializeField] private float rockTorque;

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    public override void Throw()
    {
        base.Throw();
        RoleRock(player.playerOrigin.position);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isThrown)
            OnObjectHit(collision);
    }


    public void RoleRock(Vector3 targetPosition)
    {
        Vector3 distance = targetPosition - spawnPoint.position;
        Vector3 direction = distance.normalized;
        direction.y = 0f;
        rockRigidbody.AddForce(direction * rockForce);
        Vector3 torqueDirection = new Vector3(0f, 0f, -rockTorque);
        rockRigidbody.AddTorque(torqueDirection);
    }

    
}
