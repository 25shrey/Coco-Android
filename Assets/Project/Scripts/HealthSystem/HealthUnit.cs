using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthUnit
{
    #region  private property of current object healty
    int currentHealth;
    int currentMaxHealth;
    #endregion


    #region property to set and get the current object health
    public int CurrentHealth
    {
        get { return currentHealth; }
        set { currentHealth = value; }
    }

    public int CurrentMaxHealth
    {
        get { return currentMaxHealth; }
        set { currentMaxHealth = value; }
    }
    #endregion


    //contructor
    public HealthUnit(int health, int maxhealth)
    {
        currentHealth = health;
        currentMaxHealth = maxhealth;
    }


    #region methods for taking damage and healing
    public void DoDamage(int damage)
    {
        if (currentHealth > 0)
        {
            currentHealth -= damage;
        }
    }

    public void DoHealing(int heal)
    {
        if (currentHealth < currentMaxHealth)
        {
            currentHealth += heal;
        }

        if (currentHealth >= currentMaxHealth)
        {
            currentHealth = currentMaxHealth;
        }
    }
    #endregion
}
