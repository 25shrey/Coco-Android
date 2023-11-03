using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class MagnetPower : PowerBase
{
    void Start()
    {
        transform.DORotate(new Vector3(0, 360, 0), 5f, RotateMode.WorldAxisAdd).SetEase(Ease.Linear).SetLoops(-1);
        totalTime = seconds;
    }
    /*public override void Ability(GameObject player)
    {
        player.GetComponent<SphereCollider>().enabled = true;
        player.GetComponent<PlayerPowerUps>().usingPower = PlayerPowerUps.CurrentlyUsingPower.Magnet;
    }*/
    public override void ActivatePowerUp()
    {
        SoundManager._soundManager._otherSounds.SoundToBeUsed(2, SoundManager.Soundtype.coin, false, true);
       // GameManager.instance.Player._sounds.SoundToBeUsed(2, SoundManager.Soundtype.coin, 0.3f);
        GameManager.instance.Player.playerPowerUps.usingPower.Add(this);
        GameManager.instance.Player.magnetSphereCollider.enabled = true;
        GameManager.instance.Player.PlayMagnetPowerVFX();
    }
    
    public async override void DeActivatePowerUp()
    {
        Debug.Log("++Deactivate Magnet");
        StopCountDown();
        GameManager.instance.Player.playerPowerUps.usingPower.Remove(this);
        GameManager.instance.Player.magnetSphereCollider.enabled = false;
        GameManager.instance.Player.StopMagnetPowerVFX();
        await Task.Delay(2000);
        PowerHandler.Instance.magnetPowerUpInUse = false;
    }
}
