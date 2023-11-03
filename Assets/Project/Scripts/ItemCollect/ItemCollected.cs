using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using PlanetariumApp;
using UnityEngine;
using UnityEngine.Serialization;

public class ItemCollected : Singleton<ItemCollected>
{
    private int health;
    private int coins;

    public int Health
    {
        get { return health; }
        set { health = value; }
    }

    public int Coins
    {
        get { return coins; }
        set { coins = value; }
    }
}
