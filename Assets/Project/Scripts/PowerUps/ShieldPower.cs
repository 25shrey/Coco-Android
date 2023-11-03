using DG.Tweening;
using Game.BaseFramework;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ShieldPower : PowerBase
{
    void Start()
    {
        transform.DORotate(new Vector3(0, 360, 0), 5f, RotateMode.WorldAxisAdd).SetEase(Ease.Linear).SetLoops(-1);
        totalTime = seconds;
    }
    /*public override void Ability(GameObject player)
    {
        player.GetComponent<PlayerPowerUps>().usingPower = PlayerPowerUps.CurrentlyUsingPower.Shield;
    }*/
    
    public override void ActivatePowerUp()
    {
        Debug.Log("/ActivatePowerUp");
        SoundManager._soundManager._otherSounds.SoundToBeUsed(3, SoundManager.Soundtype.coin, false, true);
        //GameManager.instance.Player._sounds.SoundToBeUsed(3, SoundManager.Soundtype.coin, 0.3f);
        GameManager.instance.Player.playerPowerUps.usingPower.Add(this);
        GameManager.instance.Player.shieldPowerUpEffect.SetActive(true);
        GameManager.instance.Player.spawnShieldRipples.EnableOrDisableShield(true);
    }
    
    public override void DeActivatePowerUp()
    {
        Debug.Log("/DeActivatePowerUp");
        StopCountDown();
        SoundManager._soundManager._otherSounds.SoundToBeUsed(8, SoundManager.Soundtype.other, false, true);
       // GameManager.instance.Player._sounds.SoundToBeUsed(8, SoundManager.Soundtype.other, 0.3f);
        GameManager.instance.Player.playerPowerUps.usingPower.Remove(this);
        GameManager.instance.Player.spawnShieldRipples.EnableOrDisableShield(false, () =>
        {
            Debug.Log("/DeActivatePowerUp CallBack");
            var game = UIController.Instance.GetScreen<GameplayUI>(ScreenType.Gameplay);
            if(game.shieldImageTimer.fillAmount < 0.02f || GameManager.instance.currentGameState == GameStates.PlayerRespawn
            || GameManager.instance.currentGameState == GameStates.alive)
            {
                Debug.Log("/DeActivatePowerUp CallBack If");
                //Debug.Log("*DeActivatePowerUp isShieldEnabled"+GameManager.instance.Player.spawnShieldRipples.isShieldEnabled);
                //if (!GameManager.instance.Player.spawnShieldRipples.isShieldEnabled)
                //{
                    GameManager.instance.Player.shieldPowerUpEffect.SetActive(false);
                //}
                Delay();
            }
        });
    }

    public async void Delay()
    {
        await Task.Delay(2000);
        PowerHandler.Instance.shieldPowerUpInUse = false;
    }
}
