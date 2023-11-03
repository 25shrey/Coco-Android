using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;
using UnityEngine.VFX;

namespace Game.CheckPoints
{
    [RequireComponent(typeof(BoxCollider))]
    public class CheckPointTrigger : MonoBehaviour
    {
        public Texture _reached;

        //event to get the player position when on trigger
        public delegate void Check(Vector3 position, int health, int coin, Transform player);
        public static event Check currentCheckPoint;

        //sets the index of the prefab
        [SerializeField]
        internal int index;
        [SerializeField]
        protected LayerMask _layer;

        [SerializeField]
        GameObject flag;
        [SerializeField]
        Renderer basemap;
        [SerializeField]
        Animator anim;
        [SerializeField]
        AudioSource audio;

        public VisualEffect checkpointVFX;
        string checkpointEventName = "OnCheckPointTrigger";

        private void Start()
        {
            audio = transform.GetChild(2).GetComponent<AudioSource>();
            SoundManager._soundManager.AddToAudioSourceList(audio);
            checkpointVFX.transform.eulerAngles = -1*anim.transform.eulerAngles;
        }

        public virtual void OnTriggerEnter(Collider other)
        {
            if (((1 << other.gameObject.layer) & _layer) != 0)
            {
                if (!System.Object.ReferenceEquals(currentCheckPoint,null))
                {
                   // SoundManager._soundManager._otherSounds.SoundToBeUsed(12, SoundManager.Soundtype.other, false, true, 0.7f);
                    audio.Play();
                    flag.SetActive(true);
                    FlagAnimation();
                    flag.transform.DOLocalMoveY(0, 1f);
                    //Invoke("FlagAnimation",0.5f);
                    currentCheckPoint(other.transform.position,
                        PlayerStats.Instance.obj.Health,PlayerStats.Instance.obj.Coins, other.transform);
                    transform.GetComponentInParent<CheckPoint>().currentCheckPointIndex = index;
                    transform.GetComponentInParent<CheckPoint>().currentRotation = other.transform.rotation;
                    //transform.gameObject.SetActive(false);
                    gameObject.GetComponent<BoxCollider>().enabled = false;
                    checkpointVFX.Play();
                    basemap.GetComponent<Renderer>().material.SetTexture("_BaseMap", _reached);
                    RestoreManager.Instance.ClearData();
                }
            }
        }


        void FlagAnimation()
        {
            anim.enabled = true;
        }

    }
}
