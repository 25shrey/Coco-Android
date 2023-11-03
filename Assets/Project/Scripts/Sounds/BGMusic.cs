using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMusic : MonoBehaviour
{
    [HideInInspector]
    public AudioSource bgSource;

    // Start is called before the first frame update
    void Start()
    {
        bgSource = GetComponent<AudioSource>();
        SoundManager._soundManager.BGAudioSources(bgSource);
    }

    private void OnDestroy()
    {
        SoundManager._soundManager.RemoveBGAudioSource(bgSource);
    }
}
