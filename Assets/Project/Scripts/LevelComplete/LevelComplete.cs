using Game.CheckPoints;
using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.VFX;

public class LevelComplete : MonoBehaviour
{
    float velocity = 1f;

    Vector3 start;
    public GameObject end;
    public GameObject flag;
    public Animator anim;
    public VisualEffect checkpointVFX;
    public List<VisualEffect> levelEndingVFX;

    public AudioListener listner;

    float time;

    Vector3 dir;

    public GameObject input;

    CharacterController ch;

    public GameObject coco;

    float speed = 4f;

    public PlayableDirector direc;

    [SerializeField] private LayerMask _layer;

    public delegate void LevelCompleteDelegate(bool _coompleted, int health, int coins, int score);

    public static event LevelCompleteDelegate levelCompleteDelegate;

    public delegate void ReachedEndPointDelegate();

    public static event ReachedEndPointDelegate reachedEndPointDelegate;

    private void Awake()
    {
        listner = Camera.main.GetComponent<AudioListener>();
        listner.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & _layer) != 0)
        {
            if (GameManager.instance.Player.playerPowerUps.IsPowerUpActive(
                    PowerUpType
                        .Magnet)) //GameManager.instance.Player.GetComponent<PlayerPowerUps>().usingPower == PlayerPowerUps.CurrentlyUsingPower.None
            {
                GameManager.instance.Player.GetComponent<PlayerPowerUps>().DeactivateAllPower();

                if (Vector3.Distance(GameManager.instance.Player.transform.position, transform.position) < 5f)
                {
                    SetConstraints(other);
                }
            }
            else
            {
                SetConstraints(other);
            }

            listner.enabled = true;
            GameManager.instance.Player.audioListener.enabled = false;
            flag.SetActive(true);
            anim.enabled = true;
            flag.transform.DOLocalMoveY(0, 1f);
            checkpointVFX.Play();
            for (int i = 0; i < levelEndingVFX.Count; i++)
            {
                levelEndingVFX[i].Play();
            }
            
        }
    }

    void SetConstraints(Collider other)
    {
        //GameObject.FindObjectOfType<InputController>().enabled = false;
        GameManager.instance.input.DisableInput();
        reachedEndPointDelegate();
        RestoreManager.Instance.ClearData();
        start = other.transform.position;
        ch = other.GetComponent<CharacterController>();
        StartCoroutine(Run(start, end, other));
        SoundManager._soundManager._otherSounds.SoundToBeUsed(6, SoundManager.Soundtype.other, false, true);
    }

    IEnumerator Run(Vector3 start, GameObject end, Collider other)
    {
        var dis = Vector3.Distance(start, end.transform.position);

        time = dis / velocity;

        while (time > 0)
        {
            time = time - (Time.deltaTime * velocity * speed);

            dir = (ch.transform.position - end.transform.position).normalized;

            ch.Move((-dir) * Time.deltaTime * (speed));

            GameManager.instance.Player.StartCustomRunAnimation();

            var lookPos = end.transform.position - other.transform.position;

            lookPos.y = 0;

            var rotation = Quaternion.LookRotation(lookPos);

            other.transform.rotation = Quaternion.Slerp(other.transform.rotation, rotation, Time.deltaTime * 16f);

            if (ch.transform.position == end.transform.position || Vector3.Distance(other.transform.position, end.transform.position) < 2.3f)
            {
                time = 0;
                break;
            }

            yield return null;
        }

        GameManager.instance.Player.EndCustomRunAnimation();

        other.transform.position = end.transform.position;
        other.transform.rotation = Quaternion.Euler(0, -90, 0);
        other.gameObject.SetActive(false);
        coco.SetActive(true);

        GameManager.instance.currentGameState = GameStates.level_end_animation;

        StartCoroutine(LevelCompleteCoroutine());
    }

    IEnumerator LevelCompleteCoroutine()
    {
        GameManager.instance.input.EnableInput();
        direc.Play();

        yield return new WaitForSeconds(5f);

        levelCompleteDelegate(true, PlayerStats.Instance.obj.Health, PlayerStats.Instance.obj.Coins,
            PlayerStats.Instance.obj.Score);

        transform.parent.gameObject.SetActive(false);

       // GameManager.instance.Player._sounds.SoundToBeUsed(6, SoundManager.Soundtype.other, 0.9f);

        Invoke("ChangetoNextlevel", 2.2f);
    }

    void ChangetoNextlevel()
    {
        GameManager.instance.currentGameState = GameStates.level_complete;
    }

    public static void SkipEndAnimation()
    {
        levelCompleteDelegate(true, PlayerStats.Instance.obj.Health, PlayerStats.Instance.obj.Coins,
            PlayerStats.Instance.obj.Score);

        GameManager.instance.currentGameState = GameStates.level_complete;
    }
}