using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.VFX;

public class Life : MonoBehaviour, ICollectable
{
    [SerializeField] private int rewardScore;
    [SerializeField] bool isCollected;
    [SerializeField] private Transform heart;
    private GameObject player;

    [SerializeField]
    LayerMask _layer;

    [Header("VFX")]
    [SerializeField] private VisualEffect heartLevitatingVFX;
    [SerializeField] private VisualEffect heartCollectionVFX;


    void Start()
    {
        heart.transform.DORotate(new Vector3(0, 360, 0), 5f, RotateMode.WorldAxisAdd).SetEase(Ease.Linear).SetLoops(-1);

        player = GameManager.instance.Player.gameObject;
        heartLevitatingVFX.Play();
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

    public void Show()
    {
        isCollected = false;
        transform.GetComponent<BoxCollider>().enabled = true;
        heart.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutExpo);
        heart.transform.DORotate(new Vector3(0, 360, 0), 5f, RotateMode.WorldAxisAdd).SetEase(Ease.Linear).SetLoops(-1);
    }

    public void Hide()
    {
        transform.GetComponent<BoxCollider>().enabled = false;
        heart.transform.DORotate(new Vector3(0, 360, 0), 0.5f, RotateMode.WorldAxisAdd).SetEase(Ease.Linear).SetLoops(-1);
        heart.transform.DOScale(Vector3.zero, 0.8f).SetEase(Ease.OutExpo);

        StartCoroutine(Kill());
    }

    public void Collect()
    {
        if (!isCollected)
        {
            heartLevitatingVFX.Stop();
            heartLevitatingVFX.gameObject.SetActive(false);
            heartCollectionVFX.gameObject.SetActive(true);
            heartCollectionVFX.Play();
            GameManager.instance.AddScore(rewardScore);
            isCollected = true;
            GameManager.instance.Player.AddLife();
            Hide();
        }
    }


    IEnumerator Kill()
    {
        yield return new WaitForSeconds(2f);
        heartCollectionVFX.Stop();
        DOTween.Kill(transform);
        gameObject.SetActive(false);
    }

}
