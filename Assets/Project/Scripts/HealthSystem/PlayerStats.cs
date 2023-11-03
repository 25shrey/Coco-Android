using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Game.BaseFramework;

public class PlayerStats : Singleton<PlayerStats>
{
    public PlayerStatsGeneric<int> obj = new PlayerStatsGeneric<int>();


    private void OnEnable()
    {
        Coin.coinDelegate += UpdateCoinData;
        HangingBank.bankCoinDelegate += UpdateCoinData;
    }

    private void OnDisable()
    {
        Coin.coinDelegate -= UpdateCoinData;
        HangingBank.bankCoinDelegate -= UpdateCoinData;
    }

    void UpdateCoinData(int coin)
    {

    }

}