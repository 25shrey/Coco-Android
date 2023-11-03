using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PowerUpType
{
    None,
    Magnet,
    Shield,
    FireBall
}
public class PlayerPowerUps : MonoBehaviour
{   
    public List<PowerBase> usingPower; /*= CurrentlyUsingPower.None;*/

    public bool IsPowerUpActive(PowerUpType powerUpType)
    {
        if (usingPower.Find(x=>x.type == powerUpType))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    
    public void DeactivateAllPower()
    {
        while (usingPower.Count>0)
        {
            usingPower[0].DeActivatePowerUp();
        }
    }
}
