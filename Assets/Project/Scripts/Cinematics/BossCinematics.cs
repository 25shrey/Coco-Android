using Cinemachine;
using GameCoreFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

public class BossCinematics : MonoBehaviour
{
    [Header("Cinematics Data")]
    [SerializeField] protected List<BossInternalCinematics> bossInternalCinematicsList;
    protected BossInternalCinematics currentBossInternalCinematic;
    private CinemachineVirtualCamera currentVirtualCamera;
    private PlayableDirector currentDirector;
    private InputController inputController;
    [SerializeField] private AudioListener camListener;
    [SerializeField] private AudioSource Boss;

    private void Awake()
    {
        inputController = GS.Instance.input;
    }


    public virtual void PlayCinematics(BossCinematicsShowCaseType bossCinematicsShowCase)
    {
        currentBossInternalCinematic = bossInternalCinematicsList.Find(x => x.bossCinematicsShowCaseType == bossCinematicsShowCase);
        if (currentBossInternalCinematic != null)
        {
            Init(currentBossInternalCinematic.cinemachineVirtualCamera, currentBossInternalCinematic.playableDirector);
            currentBossInternalCinematic.playableDirector.Play();
            StartCoroutine(GameManager.instance.CheckForTimelineCompletion(currentBossInternalCinematic.playableDirector, OnCinematicsCompleted));
        }
    }

    protected virtual void Init(CinemachineVirtualCamera camera, PlayableDirector director)
    {
        currentBossInternalCinematic.BeforeAnimationCompletionEvent?.Invoke();
        inputController.DisableInput();
        if (currentVirtualCamera != null) currentVirtualCamera = null;
        if (currentDirector != null) currentDirector = null;
        currentVirtualCamera = camera;
        currentDirector = director;
        currentVirtualCamera.gameObject.SetActive(true);
        currentDirector.gameObject.SetActive(true);
        if (!GameManager.instance.Player.gameObject.activeInHierarchy)
        {
            SoundManager._soundManager._playerSounds.SoundToBeUsed(10, SoundManager.Soundtype.player, false, false);
            camListener.enabled = true;
            Boss.enabled = true;
            Boss.loop = false;
            Boss.volume = GS.Instance.sfxSoundVolume;
            WaitToPlay(1000);
        }
    }

    public async void WaitToPlay(int delay)
    {
        await Task.Delay(delay);
        Boss.Play();
    }

    public void SetAudioSourceClip(AudioClip clip)
    {
        Boss.clip = clip;
    }


    protected virtual void OnCinematicsCompleted()
    {
        inputController.EnableInput();
        currentVirtualCamera.gameObject.SetActive(false);
        currentDirector.gameObject.SetActive(false);
        GameManager.instance.currentGameState = GameStates.alive;
        currentVirtualCamera = null;
        currentDirector = null;
        currentBossInternalCinematic.OnAnimationCompeleteEvent?.Invoke();
        if (camListener.enabled)
        {
            camListener.enabled = false;
            if (Boss.isPlaying)
            {
                Boss.Stop();
                Boss.enabled = false;
                BossArenaManage.Instance.BGMusicSwitcher(false);
            }
        }
    }
}
    
public enum BossCinematicsShowCaseType
{
    EntryCinematics,
    PowerShowcaseCinematics,
    EndCinematics,
    BullHeadCinematics,
}

[System.Serializable]
public class BossInternalCinematics
{
    public BossCinematicsShowCaseType bossCinematicsShowCaseType;
    public PlayableDirector playableDirector;
    public CinemachineVirtualCamera cinemachineVirtualCamera;
    public UnityEvent BeforeAnimationCompletionEvent;
    public UnityEvent OnAnimationCompeleteEvent;
}
