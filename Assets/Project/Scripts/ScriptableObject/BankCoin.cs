using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Amount", menuName = "Bank")]
public class BankCoin : ScriptableObject
{
    public int totalCoinInBank;

    public enum bankType
    {
        hanging
    }

    public bankType type;
}
