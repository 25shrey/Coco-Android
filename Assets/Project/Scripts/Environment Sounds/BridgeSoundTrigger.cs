using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeSoundTrigger : MonoBehaviour
{
    #region private variables

    [SerializeField] private LayerMask _layer;

    #endregion

    #region Unity callbacks

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & _layer) != 0)
        {
            GameManager.instance.Player.OnBridge = true;

            GameManager.instance.Player.animationController.StartRunningVFX();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (((1 << other.gameObject.layer) & _layer) != 0)
        {
            var audio = SoundManager._soundManager._playerSounds._audio;

            if (audio.clip.name == SoundManager._soundManager.player[11].name)
            {
                GameManager.instance.Player.OnBridge = false;

                GameManager.instance.Player.animationController.StopRunningVFX();

                GameManager.instance.Player.animationController.StartRunningVFX();
            }
        }
    }

    #endregion
}
