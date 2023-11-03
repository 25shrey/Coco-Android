using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour, ICollectable
{
    [SerializeField] bool isCollected;

    [SerializeField] LayerMask _layer;

    [SerializeField] int _time;

    private GameObject player;

    public delegate void IncreaseTimerValue(int value);
    public static event IncreaseTimerValue increaseTimerValue;

    public void Collect()
    {
        if (!isCollected)
        {
            isCollected = true;
            increaseTimerValue(_time);
            Hide();
        }
    }

    public void Hide()
    {
        transform.DORotate(new Vector3(0, 360, 0), 0.5f, RotateMode.WorldAxisAdd).SetEase(Ease.Linear).SetLoops(-1);
        transform.GetComponent<SphereCollider>().enabled = false;
        transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.OutExpo);

        StartCoroutine(Kill());
    }

    public void Show()
    {
        isCollected = false;
        transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutExpo);
        transform.DORotate(new Vector3(0, 360, 0), 5f, RotateMode.WorldAxisAdd).SetEase(Ease.Linear).SetLoops(-1);
        transform.GetComponent<SphereCollider>().enabled = true;
    }

    void Start()
    {
        transform.DORotate(new Vector3(0, 360, 0), 5f, RotateMode.WorldAxisAdd).SetEase(Ease.Linear).SetLoops(-1);

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
                    if(Vector3.Distance(player.transform.position,transform.position) < 5f)
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

    IEnumerator Kill()
    {
        yield return new WaitForSeconds(0.5f);
        DOTween.Kill(transform);
    }
}
