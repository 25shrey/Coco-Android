using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public abstract class PowerBase : MonoBehaviour
{
    public int seconds;
    public int totalTime;
    public float currentTime;
    public int rewardScore;
    public Coroutine co;
    
    public PowerUpType type;

    public delegate void timerDelegate(float cur, int total, PowerUpType type);
    public static event timerDelegate _timerDelegate;

    public delegate void powerType(PowerUpType type);
    public static event powerType _powerType;

    public delegate void deactivateType(PowerUpType type);
    public static event deactivateType _deactivateType;

    public abstract void ActivatePowerUp();
    public abstract void DeActivatePowerUp();

    public void DisablePowerUpObject()
    {
        transform.GetComponent<MeshRenderer>().enabled = false;
        transform.GetComponent<BoxCollider>().enabled = false;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Player>())
        {
            Collect();
        }
    }

    public void Collect()
    {
        DOTween.Kill(transform);
        GameManager.instance.AddScore(rewardScore);
        foreach (var item in GameManager.instance.Player.playerPowerUps.usingPower)
        {
            if(item.type == type)
            {
                GameManager.instance.Player.playerPowerUps.usingPower.Remove(item);
                item.StopCountDown();
                //_deactivateType(type);
                break;
            }
        }
        ActivatePowerUp();
        _powerType(type);
        if (co!=null)
        {
            StopCoroutine(co);
        }
        co = StartCoroutine(PowerUpTimer(seconds));
    }
    
    public IEnumerator PowerUpTimer(float count)
    {
        DisablePowerUpObject();
        while (count > 0)
        {
            _timerDelegate(count, totalTime, type);

            yield return null;

            count-=Time.deltaTime;

        }
        if (count <= 0)
        {
            DeActivatePowerUp();
            _deactivateType(type);
        }
    }

    public void StopCountDown()
    {
        StopCoroutine(co);
        _deactivateType(type);
    }

}
