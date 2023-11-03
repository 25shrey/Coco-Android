using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using PlanetariumApp;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.SocialPlatforms.Impl;

public class PlayerStatsGeneric<T>
{
    private int health;
    private int coins;
    private int currentLevel;
    private int life;
    private int score;


    public int Health
    {
        get { return health; }
        set { health = value; }
    }

    public int Life
    {
        get { return life; }
        set { life = value; }
    }

    public int Coins
    {
        get { return coins; }
        set { coins = value; }
    }

    public int CurrentLevel
    {
        get { return currentLevel; }
        set { currentLevel = value; }
    }
    public int Score
    {
        get { return score; }
        set { score = value; }
    }
}
