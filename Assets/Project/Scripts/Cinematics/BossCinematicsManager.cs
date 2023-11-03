using Game.BaseFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCinematicsManager : Singleton<BossCinematicsManager>
{
    [SerializeField] private BossCinematics bossCinematics;
    private BossCinematics currentBossCinematics;

    private void Start()
    {
        currentBossCinematics = bossCinematics;
    }

    public void BossAudioSetter(AudioClip clip)
    {
        bossCinematics.SetAudioSourceClip(clip);
    }

    public void PlayBossCinematics(BossCinematicsShowCaseType showcaseType)
    {
        if (currentBossCinematics != null)
        {
            currentBossCinematics.gameObject.SetActive(true);
            currentBossCinematics.PlayCinematics(showcaseType);
        }
        else
        {
            GameManager.instance.currentGameState = GameStates.alive;
        }
    }
}


