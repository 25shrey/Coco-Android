using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.VFX;

public class Coin : MonoBehaviour, ICollectable
{
    [SerializeField] private int rewardScore;
    [SerializeField] bool isCollected;

    [SerializeField] LayerMask _layer;

    public bool TriggerByPower;

    public delegate void CoinDelegate(int numberOfCoins);

    public static event CoinDelegate coinDelegate;

    private GameObject player;

    [SerializeField] VisualEffect collectVFX;

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
                    StartCoroutine(CollectByMagnet());
                }
                else
                {
                    Collect();
                    //TriggerByPower = true;
                }
            }
        }
    }

    public void Show()
    {
        isCollected = false;
        transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutExpo);
        transform.DORotate(new Vector3(0, 360, 0), 5f, RotateMode.WorldAxisAdd).SetEase(Ease.Linear).SetLoops(-1);
        transform.GetComponent<BoxCollider>().enabled = false;
    }

    public void Hide()
    {
        transform.DORotate(new Vector3(0, 360, 0), 0.5f, RotateMode.WorldAxisAdd).SetEase(Ease.Linear).SetLoops(-1);
        transform.GetComponent<BoxCollider>().enabled = false;
        transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.OutExpo);

        StartCoroutine(Kill());
    }

    public void Collect()
    {
        if (!isCollected)
        {
            collectVFX.Play();
            isCollected = true;
            GameManager.instance.AddScore(rewardScore);
            PlayerStats.Instance.obj.Coins++;
            coinDelegate(PlayerStats.Instance.obj.Coins);
            Hide();
        }
    }

    IEnumerator Kill()
    {
        yield return new WaitForSeconds(0.5f);
        collectVFX.Stop();
        DOTween.Kill(transform);
    }

    public IEnumerator CollectByMagnet()
    {
        float time, totalTime = 0.21f;
        time = totalTime;
        Vector3 startPos = transform.position, startScale = transform.localScale;
        while (time > 0)
        {
            time -= Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, player.transform.position + new Vector3(0, 1.5f, 0),
                1 - time / totalTime);
            transform.localScale = Vector3.Lerp(startScale, startScale / 3, 1 - time / totalTime);
            yield return null;
        }

        transform.position = player.transform.position + new Vector3(0, 1.5f, 0);
        transform.localScale = startScale / 3;
        Collect();
    }

    private void FixedUpdate()
    {
        if (TriggerByPower)
        {
            transform.position = Vector3.MoveTowards(transform.position,
                Vector3.Lerp(transform.position, player.transform.position, 2), 2);

            if (transform.position == player.transform.position && !isCollected)
            {
                Collect();
            }
        }
    }
}