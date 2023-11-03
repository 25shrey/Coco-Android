using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class LevelEndScene : MonoBehaviour
{
    private PlayableDirector dir;
    private Camera cam;


    public static LevelEndScene ins;

    // Start is called before the first frame update
    private void Awake()
    {
        if(ins == null)
        {
            ins = this;
        }
    }
    void Start()
    {
        dir = GetComponent<PlayableDirector>();
        //cam = transform.GetChild(7).GetComponent<Camera>();

        cam = transform.parent.GetChild(13).GetComponent<Camera>();
    }

    public void StartEndSequence()
    {
        cam.enabled = true;
        dir.Play();
    }

    public void StopEndSequence()
    {
        dir.Stop();
    }
}
