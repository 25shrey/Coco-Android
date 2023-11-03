using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Data", menuName = "Level")]
public class LevelData : ScriptableObject
{
    [Header("Time")]
    public int seconds;

    [Header("Level Completed")]
    public bool isCompleted = false;

    [Header("Coins")]
    public int coins;

    [Header("Health")]
    public int health;

    [Header("Score")]
    public int score;

    [Header("Next Level Unlocked")]
    public bool isNextUnlocked;
}
