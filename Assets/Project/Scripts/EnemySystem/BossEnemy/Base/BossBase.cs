using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBase : ChaseEnemy
{
    [Header("Base Properties")]
    [SerializeField] protected EnemyAnimationTrigger enemyAnimationTrigger;
    protected bool isLookingPlayer;
    protected Coroutine damageAnimCo;

    protected virtual void OnEnable()
    { 

    }

    protected virtual void OnDisable()
    {

    }

    protected virtual void SetupAnimation()
    {

    }

    protected virtual bool CanDamage()
    {
        return true;
    }

    protected virtual void BossEntry()
    {

    }

    protected virtual bool DeadEnemyCanDamage(BaseEnemy enemy)
    {
        return true;
    }

    protected virtual IEnumerator SuperAttack()
    {
        yield return null;
    }

    protected virtual IEnumerator ThrowCo()
    {
        yield return null;
    }

    protected virtual IEnumerator SetLookingAtPlayer()
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

    protected virtual IEnumerator DamageAnimation()
    {
        yield return null;
    }

    protected virtual IEnumerator EnemyCheck(BaseEnemy enemy)
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
    }


}
