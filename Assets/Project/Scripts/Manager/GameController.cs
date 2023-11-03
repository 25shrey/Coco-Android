using System;
using DG.Tweening;
using Game.BaseFramework;
using TMPro;
using UnityEngine;

public class GameController : Singleton<GameController>
{
    #region delegates

    public delegate void NewGameDelegate();
    public delegate void ContinueGameDelegate();
    public delegate void RestartGameAfterDeath();
    public delegate void Customise();
    public delegate void Settings();
    public delegate void Powerups();
    public delegate void Outfits();
    public delegate void Accessories();
    public delegate void BalloonSkins();
    public delegate void CoinGemsPacks();
    public delegate void VFXEffects();
    public delegate void Shop();
    public delegate void Exit();
    public delegate void Restart();
    public delegate void PauseSettings();


    public static event NewGameDelegate _newGameDelegate;
    public static event ContinueGameDelegate _continueGameDelegate;
    public static event RestartGameAfterDeath _restartGameAfterDeath;
    public static event Customise _customise;
    public static event Settings _settings;
    public static event Powerups _powerups;
    public static event Outfits _outfits;
    public static event Accessories _accessories;
    public static event BalloonSkins _balloonskins;
    public static event CoinGemsPacks _coinGems;
    public static event VFXEffects _vfxEffects;
    public static event Shop _shop;
    public static event Exit _exit;
    public static event Restart _restart;
    public static event PauseSettings _pauseSettings;

    #endregion

    #region private variables

    [SerializeField]
    private int sceneIndex = 2;
    [SerializeField]
    private int currentPlayingLevel;

    public TMP_InputField currentPlayingLevelText;
    
    [SerializeField]
    Sounds _sounds;

    public SceneTransitionCloudUI clouds;

    #endregion


    #region properties for private variables
    public int SceneIndex
    {
        get { return sceneIndex; }
        set { sceneIndex = value; }
    }

    public int CurrentPlayingLevel
    {
        get { return currentPlayingLevel; }
        set { currentPlayingLevel = value; }
    }

    #endregion

    void Start()
    {
        //to disable the continue button
        ToggleContinueButton();
    }

    void ToggleContinueButton()
    {
        if (SavedDataHandler.Instance._saveData.lastLevelCompleted < 1)
        {
            var obj = transform.GetChild(1).gameObject.transform.GetChild(1);
            obj.GetChild(1).gameObject.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.SetActive(false);
        }
        else
        {
            var obj = transform.GetChild(1).gameObject.transform.GetChild(1);
            obj.GetChild(1).gameObject.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.SetActive(true);
        }
    }

    #region Ui related methods

    //To start a new game
    public void NewGameHit()
    {
        _sounds.SoundToBeUsed(3, SoundManager.Soundtype.other, false, true);
        CurrentPlayingLevel = SceneIndex - 1;
        SceneIndex += 1;
        _newGameDelegate();

        SoundManager._soundManager._otherSounds.GetComponent<AudioSource>().DOFade(0, 3f);
    }


    //to start fron the continue
    public void ContinueGameHit()
    {
        _sounds.SoundToBeUsed(3, SoundManager.Soundtype.other, false, true);
        SceneIndex = SceneIndex + SavedDataHandler.Instance._saveData.lastLevelCompleted + 1;
        CurrentPlayingLevel = SavedDataHandler.Instance._saveData.lastLevelCompleted + 1;
        _continueGameDelegate();

        SoundManager._soundManager._otherSounds.GetComponent<AudioSource>().DOFade(0, 3f);
    }

    public void PlayCustomLevel()
    {
        Debug.Log("---"+currentPlayingLevelText.text);
        //currentPlayingLevel=Int32.Parse(currentPlayingLevelText.text);
        currentPlayingLevel=Convert.ToInt32(currentPlayingLevelText.text);
        if (currentPlayingLevel < 1 || currentPlayingLevel > 20)
        {
            currentPlayingLevel = 1;
        }
        SceneIndex = CurrentPlayingLevel + 2;
        _continueGameDelegate();

        SoundManager._soundManager._otherSounds.GetComponent<AudioSource>().DOFade(0, 3f);
    }

    public void RestartAfterGameOver()
    {
        _restartGameAfterDeath();
    }

    public void CustomiseHit()
    {
        _sounds.SoundToBeUsed(3, SoundManager.Soundtype.other, false, true);
        _customise();
    }

    public void SettingsHit()
    {
        _sounds.SoundToBeUsed(3, SoundManager.Soundtype.other, false, true);
        _settings();
    }

    public void PowerUpHit()
    {
        _powerups();
    }

    public void AccessoriesHit()
    {
        _accessories();
    }

    public void OutfitsHit()
    {
        _outfits();
    }

    public void BalloonSkinsHit()
    {
        _balloonskins();
    }
    
    public void CoinGemsHit()
    {
        _coinGems();
    }
    
    public void VFXEffectsHit()
    {
        _vfxEffects();
    }

    public void ShopHit()
    {
        _sounds.SoundToBeUsed(3, SoundManager.Soundtype.other, false, true);
        _shop();
    }

    public void ExitHit()
    {
        ToggleContinueButton();
        _exit();
    }

    public void PausesettingsHit()
    {
        _pauseSettings();
    }

    #endregion
}

