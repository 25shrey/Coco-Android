using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using DG.Tweening;
using Game.BaseFramework;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.VFX;

public class BossArenaManage : Singleton<BossArenaManage>
{
    #region PUBLIC_VARS

    [FormerlySerializedAs("PlayerBlockCollider")] public List<BoxCollider> PlayerBlockColliders;
    public GameObject BossPath;

    [SerializeField] private AudioSource bossBGAudio;
    //[SerializeField] private AudioListener cocoListener;
    //[SerializeField] private AudioListener camListener;

    public enum Listener
    {
        Camera,
        Coco,
    }

    public Listener listener = Listener.Coco;


    #endregion

    #region PRIVATE_VARS
    
    int HeadBlastanime;
    
    #endregion

    #region UNITY_CALLBACKS

    // Start is called before the first frame update
    void Start()
    {
        //cocoListener = GameManager.instance.Player.GetComponent<AudioListener>();
        //camListener = Camera.main.GetComponent<AudioListener>();

        for (int i = 0; i < PlayerBlockColliders.Count; i++)
        {
            PlayerBlockColliders[i].enabled = false;
        }
    }

    public void PlayerBlockSetup()
    {
        for (int i = 0; i < PlayerBlockColliders.Count; i++)
        {
            PlayerBlockColliders[i].enabled = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BGMusicSwitcher(bool playORstop)
    {
        if (bossBGAudio.isPlaying)
        {
            bossBGAudio.Stop();
        }

        StartCoroutine(BGMusicSwitcherCoroutine(playORstop));
    }

    IEnumerator BGMusicSwitcherCoroutine(bool playORstop)
    {
        if(playORstop)
        {
            if(GameController.Instance.CurrentPlayingLevel < 11)
            {
                SoundManager._soundManager.day.DOFade(0, 0.2f);
            }
            else
            {
                SoundManager._soundManager.night.DOFade(0, 0.2f);
            }

            yield return new WaitForSeconds(0.2f);

            bossBGAudio.loop = true;
            bossBGAudio.Play();
            bossBGAudio.DOFade(GS.Instance.backgroundSoundVolume, 0.4f);
        }
        else
        {
            yield return new WaitForSeconds(0.05f);

            if (GameController.Instance.CurrentPlayingLevel < 11)
            {
                SoundManager._soundManager.day.DOFade(GS.Instance.backgroundSoundVolume, 0.4f);
            }
            else
            {
                SoundManager._soundManager.night.DOFade(GS.Instance.backgroundSoundVolume, 0.4f);
            }
        }
    }


    //public void SwitchListener(Listener listener)
    //{
    //    switch (listener)
    //    {
    //        case Listener.Camera:
    //            {
    //                camListener.enabled = true;
    //                cocoListener.enabled = false;
    //                break;
    //            }
    //        case Listener.Coco:
    //            {
    //                cocoListener.enabled = true;
    //                camListener.enabled = false;
    //                break;
    //            }
    //        default:
    //            {
    //                print("nothing");
    //                break;
    //            }
    //    }
    //}

    public void SetPathInBullArena()
    {
        for (int i = 0; i < PlayerBlockColliders.Count; i++)
        {
            PlayerBlockColliders[i].enabled = false;
        }
    }

    #endregion

    #region PUBLIC_FUNCTIONS

    #endregion

    #region PRIVATE_FUNCTIONS

    #endregion

    #region CO-ROUTINES

    #endregion

    #region EVENT_HANDLERS

    #endregion

    #region UI_CALLBACKS

    #endregion
}
