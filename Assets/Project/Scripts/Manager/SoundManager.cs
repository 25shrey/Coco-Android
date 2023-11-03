


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class SoundManager : MonoBehaviour
{
    public static SoundManager _soundManager;

    public AudioSource day;
    public AudioSource night;
    public AudioSource playerAudioSource;
    public AudioSource otherAudioSource;
    public Sounds _playerSounds;
    public Sounds _otherSounds;
    public Sounds _sounds;

    public enum Soundtype
    {
        turtle,
        bee,
        tree,
        armedillo,
        coin,
        life,
        magnet,
        shield,
        other,
        player,
        BullBoss,
        DragonBoss,
        hippoBoss,
        stonemanBoss,
        none
    };
    public Soundtype type = Soundtype.none;

    public List<AudioClip> turtle = new List<AudioClip>();

    public List<AudioClip> bee = new List<AudioClip>();

    public List<AudioClip> tree = new List<AudioClip>();

    public List<AudioClip> armedillo = new List<AudioClip>();

    public List<AudioClip> collectable = new List<AudioClip>();

    public List<AudioClip> others = new List<AudioClip>();

    public List<AudioClip> player = new List<AudioClip>();

    public List<AudioClip> Bull = new List<AudioClip>();

    public List<AudioClip> dragon = new List<AudioClip>();

    public List<AudioClip> hippo = new List<AudioClip>();

    public List<AudioClip> stoneman = new List<AudioClip>();


    public List<AudioSource> enemyAudioSources = new List<AudioSource>();

    public List<AudioSource> otherSources = new List<AudioSource>();

    private List<AudioSource> BGSources = new List<AudioSource>();

    void Awake()
    {
        if (_soundManager == null)
        {
            _soundManager = this;
        }
    }

    public void BGMusicSetter()
    {
        if (GameController.Instance.CurrentPlayingLevel < 11)
        {
            day.volume = GS.Instance.backgroundSoundVolume;
        }
        else
        {
            night.volume = GS.Instance.backgroundSoundVolume;
        }
    }

    public void SetAudioSounds()
    {
        playerAudioSource.volume = GS.Instance.sfxSoundVolume;
        otherAudioSource.volume = GS.Instance.sfxSoundVolume;
    }

    public void MuteAll(bool clear)
    {
        foreach (var item in enemyAudioSources)
        {
            item.volume = 0f;
        }

        foreach (var item in otherSources)
        {
            item.volume = 0f;
        }

        if (clear)
        {
            enemyAudioSources.Clear();
            otherSources.Clear();
        }
    }

    public void AddInList(AudioSource aud)
    {
        aud.volume = GS.Instance.sfxSoundVolume;
        enemyAudioSources.Add(aud);
    }

    public void AddToAudioSourceList(AudioSource audioSource)
    {
        audioSource.volume = GS.Instance.sfxSoundVolume;
        otherSources.Add(audioSource);
    }

    public void BGAudioSources(AudioSource source)
    {
        foreach (var item in BGSources)
        {
            if (source == item)
            {
                return;
            }
        }
        BGSources.Add(source);
    }

    public void RemoveBGAudioSource(AudioSource source)
    {
        BGSources.Remove(source);
    }

    public void BGAudioVolumeSetter()
    {
        for (byte i = 0; i < BGSources.Count; i++)
        {
            BGSources[i].volume = GS.Instance.backgroundSoundVolume;
        }
    }

    public void VolumeSetter()
    {
        for(byte i = 0; i < otherSources.Count; i++)
        {
            otherSources[i].volume = GS.Instance.sfxSoundVolume;
        }
        for (byte i = 0; i < enemyAudioSources.Count; i++)
        {
            enemyAudioSources[i].volume = GS.Instance.sfxSoundVolume;
        }
        BGAudioVolumeSetter();
    }

    public void EnemiesToggleSound(bool isPaused)
    {
        foreach (var item in enemyAudioSources)
        {
            if (isPaused)
            {
                item.Pause();
            }
            else
            {
                item.Play();
            }
        }
    }
}
