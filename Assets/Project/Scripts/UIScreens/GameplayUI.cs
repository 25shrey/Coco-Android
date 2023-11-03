using Game.BaseFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class GameplayUI : UIScreenView
{
    public BossHealthBar healthBar;

    public TMP_Text _coins;
    public TMP_Text _health;
    public TMP_Text _timer;
    public TMP_Text _life;
    public TMP_Text _score;
    public TMP_Text _level;
    public Image shieldImageTimer;
    public Image magnetImageTimer;

    public RectTransform shieldInferior;
    public RectTransform magnetInferior;

    public Image _fadeImg;

    public ControllerMobile _controllerMobile;

    public Coroutine _cr;

    int seconds;
    Coroutine countDownTimerCoroutine;

    private GameStates savedState;

    public override void OnScreenShowAnimationCompleted()
    {
        base.OnScreenShowAnimationCompleted();
    }

    public void StartTheTimer()
    {
        if (System.Object.ReferenceEquals(countDownTimerCoroutine, null))
        {
            countDownTimerCoroutine = StartCoroutine(CountDownTimer());
        }
    }

    public override void OnScreenShowCalled()
    {
        base.OnScreenShowCalled();
       // Cursor.visible = false;
    }

    public override void OnScreenHideCalled()
    {
        base.OnScreenHideCalled();
       // Cursor.visible = true;
    }


    public override void Show()
    {
        base.Show();
        _level.text = "LEVEL   " + GameController.Instance.CurrentPlayingLevel;
    }

    
    private void OnEnable()
    {
        Coin.coinDelegate += UpdateCoinData;
        HangingBank.bankCoinDelegate += UpdateCoinData;
        Player._playerHealthUI += UpdateHealthData;
        Player._playerLifeUI += UpdateLifeData;
        Player._playerCoinUI += UpdateCoinData;
        Timer.increaseTimerValue += UpdateTimer;
        LevelComplete.reachedEndPointDelegate += StopTheTimer;
        GameManager._showScoreDelegate += ShowPlayerScore;
        PowerBase._powerType += Activate;
        PowerBase._deactivateType += Deactivate;
        PowerBase._timerDelegate += ShowTimer;
    }

    private void OnDisable()
    {
        Coin.coinDelegate -= UpdateCoinData;
        HangingBank.bankCoinDelegate -= UpdateCoinData;
        Player._playerHealthUI -= UpdateHealthData;
        Player._playerCoinUI -= UpdateCoinData;
        Player._playerLifeUI -= UpdateLifeData;
        Timer.increaseTimerValue -= UpdateTimer;
        LevelComplete.reachedEndPointDelegate -= StopTheTimer;
        GameManager._showScoreDelegate -= ShowPlayerScore;
        PowerBase._powerType -= Activate;
        PowerBase._deactivateType -= Deactivate;
        PowerBase._timerDelegate -= ShowTimer;
        GameManager._clouds -= CloudUi;
    }

   

    public override void OnBack()
    {
        //base.OnBack();
        //Debug.Log("Inside - OnBack");
        //savedState = GameManager.instance.currentGameState;
        //UIController.Instance.ShowNextScreen(ScreenType.Pause);
        //GameManager.instance.currentGameState = GameStates.paused;
        //Time.timeScale = 0f;
        //GameManager.instance.input.DisableInput();
        //GS.Instance.OnGamePause();
        /*if (GameManager.instance.currentGameState == GameStates.alive)
        {*/

        /*}*/
        if (GameManager.instance.currentGameState == GameStates.alive && !GameManager.instance.isDeadZoneTrigger)
        {
            if (!GS.Instance.IsGamePaused() && _cr == null)
            {
                base.OnBack();
                savedState = GameManager.instance.currentGameState;
                UIController.Instance.ShowNextScreen(ScreenType.Pause);
                GameManager.instance.currentGameState = GameStates.paused;
                _cr = StartCoroutine(Toggle());
            }
        }
    }

    IEnumerator Toggle()
    {
        yield return new WaitForSeconds(1);

        SoundManager._soundManager.EnemiesToggleSound(true);
        GameManager.instance.input.DisableInput();
        GS.Instance.OnGamePause();
        _cr = null;
        Time.timeScale = 0f;
    }

    public GameStates GetGameState()
    {
        return savedState;
    }

    void UpdateCoinData(int coin)
    {
        _coins.text = PlayerStats.Instance.obj.Coins.ToString();
    }

    void Activate(PowerUpType type)
    {
        switch (type)
        {
            case PowerUpType.Magnet:
                {
                    magnetImageTimer.transform.parent.gameObject.SetActive(true);
                    magnetImageTimer.enabled = true;
                    break;
                }
            case PowerUpType.Shield:
                {
                    shieldImageTimer.transform.parent.gameObject.SetActive(true);
                    shieldImageTimer.enabled = true;
                    break;
                }
        }
    }

    void Deactivate(PowerUpType type)
    {
        switch (type)
        {
            case PowerUpType.Magnet:
                {
                    magnetImageTimer.transform.parent.gameObject.SetActive(false);
                    magnetImageTimer.enabled = false;
                    magnetInferior.eulerAngles = Vector3.zero;
                    break;
                }
            case PowerUpType.Shield:
                {
                    shieldImageTimer.transform.parent.gameObject.SetActive(false);
                    shieldImageTimer.enabled = false;
                    shieldInferior.eulerAngles = Vector3.zero;
                    break;
                }
        }
    }

    public void ReserPowerUpData()
    {
        magnetImageTimer.transform.parent.gameObject.SetActive(false);
        shieldImageTimer.transform.parent.gameObject.SetActive(false);
        magnetImageTimer.enabled = false;
        shieldImageTimer.enabled = false;
        magnetInferior.eulerAngles = Vector3.zero;
        shieldInferior.eulerAngles = Vector3.zero;
    }

    public void ResetData()
    {
        PlayerStats.Instance.obj.Coins = 0;
        PlayerStats.Instance.obj.Health = 0;
        _coins.text = PlayerStats.Instance.obj.Coins.ToString();
        _health.text = PlayerStats.Instance.obj.Health.ToString();
    }

    void UpdateHealthData(int health)
    {
        if (System.Object.ReferenceEquals(PlayerStats.Instance, null))
        {
            _health.text = health.ToString();
        }
        else
        {
            _health.text = PlayerStats.Instance.obj.Health.ToString();
        }
    }
    
    void UpdateLifeData(int life)
    {
        if (PlayerStats.Instance != null)
        {
            _life.text = life.ToString();
        }
        else
        {
            _life.text = PlayerStats.Instance.obj.Life.ToString();
        }
    }

    void UpdateTimer(int value)
    {
        seconds = seconds + value;
    }

    void ShowPlayerScore(int score)
    {
        _score.text = score.ToString(); 
    }
    

    public void SetTimer(int _seconds)
    {  
        float min = _seconds / 60;
        float sec = _seconds % 60;

        if (sec < 10)
        {
            _timer.text = min.ToString() + ":0" + sec.ToString();
        }
        else
        {
            _timer.text = min.ToString() + ":" + sec.ToString();
        }

        seconds = _seconds;

        ShowPlayerScore(00);
    }

    void SetBGSound()
    {
        if (GameController.Instance.CurrentPlayingLevel < 11)
        {
            SoundManager._soundManager.night.Stop();
            SoundManager._soundManager.day.clip = SoundManager._soundManager.others[1];
            SoundManager._soundManager.day.volume = GS.Instance.backgroundSoundVolume;
            SoundManager._soundManager.day.Play();
            //StartCoroutine(FadeBGMusic(SoundManager._soundManager.day));
        }
        else
        {
            SoundManager._soundManager.day.Stop();
            SoundManager._soundManager.night.clip = SoundManager._soundManager.others[1];
            SoundManager._soundManager.night.volume = GS.Instance.backgroundSoundVolume;
            SoundManager._soundManager.night.Play();
            //StartCoroutine(FadeBGMusic(SoundManager._soundManager.night));
        }

        GameManager._clouds += CloudUi;
        GameController.Instance.clouds.SuscribeClouds();
    }

    IEnumerator CountDownTimer()
    {
        yield return new WaitForSeconds(0.5f);

        SetBGSound();

        yield return new WaitForSeconds(3.6f);

        if (GameController.Instance.CurrentPlayingLevel < 21)
        {
            yield return new WaitUntil(() => (GameManager.instance.Player._reachedToInitialPoint));

            while (seconds != 0 && GameManager.instance.Player._reachedToInitialPoint)
            {
                yield return new WaitForSeconds(1f);

                seconds--;

                float min = seconds / 60;
                float sec = seconds % 60;

                if (sec < 10)
                {
                    _timer.text = min.ToString() + ":0" + sec.ToString();
                }
                else
                {
                    _timer.text = min.ToString() + ":" + sec.ToString();
                }

                while (!GameManager.instance.Player.isRespawned)
                {
                    yield return null;
                }

                if (GameManager.instance.currentGameState == GameStates.level_complete || GameManager.instance.currentGameState == GameStates.GameOver
                    || GameManager.instance.currentGameState == GameStates.level_end_animation)
                {
                    StopTheTimer();

                    break;
                }
            }

            if (seconds <= 0)
            {
                SoundManager._soundManager._otherSounds.SoundToBeUsed(9, SoundManager.Soundtype.other, false, true);

                GameManager.instance.currentGameState = GameStates.PlayerPaused;

                RemoveBossHealthBar();

                yield return new WaitForSeconds(0.2f);

                SoundManager._soundManager._otherSounds.SoundToBeUsed(4, SoundManager.Soundtype.other, false, true);

                yield return new WaitForSeconds(1f);

                GameManager.instance.currentGameState = GameStates.GameOver;

                StopTheTimer();

                GameManager.instance.RestartGame();

                SoundManager._soundManager.BGMusicSetter();
                
                SoundManager._soundManager.SetAudioSounds();

                //transform.parent.GetChild(2).GetComponent<LevelMapUI>().ReloadLevelAfterGameOver();
            }
        }
    }

    public void RemoveBossHealthBar()
    {
        healthBar.ResetBar();
    }

    public void StopTheTimer()
    {
        if (!System.Object.ReferenceEquals(countDownTimerCoroutine, null))
        {
            StopCoroutine(countDownTimerCoroutine);
            countDownTimerCoroutine = null;
        }
    }

    void ShowTimer(float time, int total, PowerUpType type)
    {
        switch (type)
        {
            case PowerUpType.Magnet:
                {
                    if (time > 0)
                    {
                        magnetImageTimer.fillAmount = time / total; 
                        magnetInferior.eulerAngles = new Vector3(0, 0, Mathf.Lerp(360, 0, time / total));
                    }

                    break;
                }
            case PowerUpType.Shield:
                {
                    if (time > 0)
                    {
                        shieldImageTimer.fillAmount = time / total;
                        shieldInferior.eulerAngles = new Vector3(0, 0, Mathf.Lerp(360, 0, time / total));
                    }

                    break;
                }
        }
    }

    public IEnumerator Cloud()
    {
        GameController.Instance.clouds.isShowingClouds = true;

        GameController.Instance.clouds.cloudAnim.SetBool("MoveClouds", true);

        UIController.Instance.ShowScreen(ScreenType.Cloud);

        yield return new WaitForSeconds(0.1f);

        GameManager.instance.endCloudScreen = true;
    }

    public void CloudUi()
    {
        StartCoroutine(Cloud());
    }
}
