using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Fruit : MonoBehaviour, ICollectable
{
    #region PUBLIC_VARS
    
    [SerializeField] private int rewardScore;
    [SerializeField] bool isCollected;
    private GameObject player;
    [SerializeField]
    LayerMask _layer;

    [SerializeField] private Transform fruit;
    [SerializeField] private VisualEffect collectionVFX;

    #endregion

    #region PRIVATE_VARS

    #endregion

    #region UNITY_CALLBACKS

    void Start()
    {
        fruit.DORotate(new Vector3(0, 360, 0), 5f, RotateMode.WorldAxisAdd).SetEase(Ease.Linear).SetLoops(-1);

        player = GameManager.instance.Player.gameObject;
    }
    

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & _layer) != 0)
        {
            if (GameManager.instance.currentGameState == GameStates.alive)
            {
                if (GameManager.instance.Player.playerPowerUps.IsPowerUpActive(
                        PowerUpType.Magnet)) //GameManager.instance.Player.GetComponent<PlayerPowerUps>().usingPower == PlayerPowerUps.CurrentlyUsingPower.None
                {
                    if (Vector3.Distance(player.transform.position, transform.position) < 5f)
                    {
                        Collect();
                    }
                }
                else
                {
                    Collect();
                }
            }
        }
    }

    #endregion

    #region PUBLIC_FUNCTIONS

    public void Show()
    {
        isCollected = false;
        transform.GetComponent<BoxCollider>().enabled = true;
        fruit.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutExpo);
        fruit.DORotate(new Vector3(0, 360, 0), 5f, RotateMode.WorldAxisAdd).SetEase(Ease.Linear).SetLoops(-1);
    }

    public void Hide()
    {
        transform.GetComponent<BoxCollider>().enabled = false;
        fruit.DORotate(new Vector3(0, 360, 0), 0.5f, RotateMode.WorldAxisAdd).SetEase(Ease.Linear).SetLoops(-1);
        fruit.DOScale(Vector3.zero, 0.5f).SetEase(Ease.OutExpo);

        StartCoroutine(Kill());
    }

    public void Collect()
    {
        if (!isCollected)
        {
            GameManager.instance.AddScore(rewardScore);
            isCollected = true;
            GameManager.instance.Player.AddHealth();
            GameManager.instance.PlayVFX(collectionVFX);
            Hide();
        }
    }


    #endregion

    #region PRIVATE_FUNCTIONS

    #endregion

    #region CO-ROUTINES

    IEnumerator Kill()
    {
        yield return new WaitForSeconds(0.5f);
        GameManager.instance.StopVFX(collectionVFX, 4500);
        DOTween.Kill(transform);
    }
    
    #endregion

    #region EVENT_HANDLERS

    #endregion

    #region UI_CALLBACKS

    #endregion
}
