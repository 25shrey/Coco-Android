using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sounds : MonoBehaviour
{
    public AudioSource _audio;
    Coroutine SoundCo;

    private void OnEnable()
    {
        Coin.coinDelegate += coinSound;
    }

    private void OnDisable()
    {
        Coin.coinDelegate -= coinSound;
    }

    void coinSound(int num)
    {
        SoundToBeUsed(0, SoundManager.Soundtype.coin, false, true);

        RunSoundAfterCollect();
    }

    public void SoundToBeUsed(int number, SoundManager.Soundtype type, bool loop, bool playORstop)
    {
        switch (type)
        {
            case SoundManager.Soundtype.coin:
            {
                if(GameController.Instance.SceneIndex > 2)
                    {
                        if (number == 0)
                        {
                            PlaySound(SoundManager._soundManager.collectable[number], loop, playORstop);
                        }
                        else
                        {
                            PlaySound(SoundManager._soundManager.collectable[number], loop, playORstop);
                        }
                    }
                    _audio.volume = GS.Instance.backgroundSoundVolume;
                    break;
            }
            case SoundManager.Soundtype.other:
            {
                    PlaySound(SoundManager._soundManager.others[number], loop, playORstop);
                    _audio.volume = GS.Instance.backgroundSoundVolume;
                    break;
            }
            case SoundManager.Soundtype.player:
                {
                    PlaySound(SoundManager._soundManager.player[number], loop, playORstop);
                    _audio.volume = GS.Instance.sfxSoundVolume;
                    break;
                }
        }
        
    }

    void PlaySound(AudioClip clip, bool loop, bool playORstop)
    {
        if (playORstop)
        {
            if (_audio.isPlaying)
            {
                _audio.Stop();
            }
            _audio.loop = loop;
            _audio.clip = clip;
            _audio.Play();
        }
        else
        {
            _audio.Stop();
        }
    }

    IEnumerator RunSoundAfterCollectCo()
    {
        yield return new WaitForSeconds(0.5f);

        if (GameManager.instance.Player.OnGroundChecker())
        {
            GameManager.instance.Player.animationController.StartRunningVFX();
        }
    }

    void RunSoundAfterCollect()
    {
        if (SoundCo != null)
        {
            StopCoroutine(SoundCo);
        }
        SoundCo = StartCoroutine(RunSoundAfterCollectCo());
    }
}
