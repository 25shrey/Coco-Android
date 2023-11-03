using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class MushroomFloating : MonoBehaviour
{
    [SerializeField]
    private bool isJumping;
    public float height, distance;
    
    [Header("Visual Effect")]
    [SerializeField] private VisualEffect mushroomSpringVFX;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<Player>())
        {
            if (!isJumping)
            {
                SoundManager._soundManager._otherSounds.SoundToBeUsed(7, SoundManager.Soundtype.other, false, true);
                mushroomSpringVFX.Play();
                GameManager.instance.Player.CustumJump(height, distance);
                isJumping = true;
            }
        }
    }
    
    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.GetComponent<Player>())
        {

                if (isJumping)
                {

                    isJumping = false;
                    //GameManager.instance.input.enabled = true;
                    GameManager.instance.input.EnableInput();
                }
            
        }
    }
}
