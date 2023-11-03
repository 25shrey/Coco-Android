using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class HangingBank : BankBase
{
    public LayerMask _layerToDetect;
    public BankCoin totalCoins;

    GameObject boxObj;
    GameObject coinObj;

    public float coinjumpAmount;

    public delegate void BankCoinDelegate(int numberOfCoins);
    public static event BankCoinDelegate bankCoinDelegate;

    private void OnEnable()
    {
        boxObj = transform.parent.GetChild(0).gameObject;
        coinObj = transform.GetChild(0).gameObject;

        base.MaxCoins = totalCoins.totalCoinInBank;
        base.current = base.MaxCoins;
    }

    private void OnTriggerExit(Collider other)
    {
        if (((1 << other.gameObject.layer) & _layerToDetect) != 0)
        {
            if(base.current > 0 && (Vector3.Distance(other.transform.position,transform.position) < 5f))
            {

                if(base.current > 1)
                {
                    boxObj.transform.DOPunchPosition(new Vector3(0, 0.5f, 0), 0.5f);
                }

                base.TotalCoins(boxObj);
                bankCoinDelegate(PlayerStats.Instance.obj.Coins);

                coinObj.transform.localScale = Vector3.one;
                coinObj.transform.DORotate(new Vector3(0, 360, 0), 5f, RotateMode.WorldAxisAdd).SetEase(Ease.Linear).SetLoops(-1);
                StartCoroutine(CoinrTrigger());
            }
        }
    }

    IEnumerator CoinrTrigger()
    {
        var initial = coinObj.transform.localPosition.y;

        coinObj.transform.DOLocalMoveY(coinjumpAmount + coinObj.transform.localPosition.y, 0.15f).SetEase(Ease.Linear);

        yield return new WaitForSeconds(0.15f);

        coinObj.transform.DOLocalMoveY(initial, 0f).SetEase(Ease.Linear);
        coinObj.transform.localScale = Vector3.zero;

        DOTween.Kill(transform);
    }
}
