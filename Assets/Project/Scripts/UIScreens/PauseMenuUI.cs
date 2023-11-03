using DG.Tweening;
using Game.BaseFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Device;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuUI : UIScreenView
{
    public Button resumeButton;
    public Button settingsButton;
    public Button restartButton;
    public Button exitButton;

    [SerializeField] private GameObject firstObjectSelection;

    public Coroutine _cr;

    public override void OnScreenShowCalled()
    {
        base.OnScreenShowCalled();
        if (Gamepad.current != null)
        {
            EventSystem.current.SetSelectedGameObject(firstObjectSelection);
        }
        resumeButton.onClick.AddListener(OnClickResumeButton);
        settingsButton.onClick.AddListener(OnClickSettingsButton);
        restartButton.onClick.AddListener(OnClickRestartButton);
        exitButton.onClick.AddListener(OnClickExitButton);
        resumeButton.Select();
    }

    public override void OnScreenHideCalled()
    {
        base.OnScreenHideCalled();
        EventSystem.current.SetSelectedGameObject(null);
        resumeButton.onClick.RemoveListener(OnClickResumeButton);
        settingsButton.onClick.RemoveListener(OnClickSettingsButton);
        restartButton.onClick.RemoveListener(OnClickRestartButton);
        exitButton.onClick.RemoveListener(OnClickExitButton);
    }
    
    private void ButtonInteractionState(bool value)
    {
        resumeButton.interactable = value;
        settingsButton.interactable = value;
        restartButton.interactable = value;
        exitButton.interactable = value;
    }

    public override void OnBack()
    {
        base.OnBack();
      //  OnClickResumeButton();
    }

    private void OnClickResumeButton()
    {
        if(GS.Instance.IsGamePaused() & _cr == null)
        {
            SoundManager._soundManager._sounds.SoundToBeUsed(3, SoundManager.Soundtype.other, false, true);
            GameplayUI gameplayUI = UIController.Instance.GetScreen<GameplayUI>(ScreenType.Gameplay);
            GameStates gameState = gameplayUI.GetGameState();
            UIController.Instance.ShowNextScreen(ScreenType.Gameplay);
            GameManager.instance.currentGameState = gameState;
            _cr = StartCoroutine(Toggle());
        }
    }

    IEnumerator Toggle()
    {
        yield return new WaitForSecondsRealtime(1);

        Time.timeScale = 1f;
        GameManager.instance.input.EnableInput();
        GS.Instance.OnGameUnPause();
        SoundManager._soundManager.EnemiesToggleSound(false);
        _cr = null;
    }

    private void OnClickSettingsButton()
    {
        SoundManager._soundManager._sounds.SoundToBeUsed(3, SoundManager.Soundtype.other, false, true);
        UIController.Instance.ShowNextScreen(ScreenType.Settings);
    }

    private async void OnClickRestartButton()
    {
        var game = UIController.Instance.GetScreen<GameplayUI>(ScreenType.Gameplay);
        game.healthBar.ResetBar();
        SoundManager._soundManager.MuteAll(true);
        SoundManager._soundManager._sounds.SoundToBeUsed(3, SoundManager.Soundtype.other, false, true);
        UIController.Instance.HideScreen(ScreenType.Pause);
        GameManager.instance.currentGameState = GameStates.GameOver;
        LevelMapUI levelMapUI = UIController.Instance.GetScreen<LevelMapUI>(ScreenType.LevelMap);
        levelMapUI.ReloadLevelAfterGameOver();
        GameplayUI gameplayUI = UIController.Instance.GetScreen<GameplayUI>(ScreenType.Gameplay);
        gameplayUI.ReserPowerUpData();
        await Task.Delay(0);
        Time.timeScale = 1f;
    }

    private void OnClickExitButton()
    {
        if(GameController.Instance.CurrentPlayingLevel > 1)
        {
            MainMenuUI menu = UIController.Instance.GetScreen<MainMenuUI>(ScreenType.MainMenu);
            menu.ContinueButtonState();
        }
        var game = UIController.Instance.GetScreen<GameplayUI>(ScreenType.Gameplay);
        game.healthBar.ResetBar();
        SoundManager._soundManager.MuteAll(true);
        SoundManager._soundManager._sounds.SoundToBeUsed(3, SoundManager.Soundtype.other, false, true);
        Time.timeScale = 1f;
        GS.Instance.OnGameUnPause();
        ButtonInteractionState(false);
        GameplayUI gamplayScreen = UIController.Instance.GetScreen<GameplayUI>(ScreenType.Gameplay);
        if (gamplayScreen != null)
        {
            gamplayScreen.StopTheTimer();    
        }
        GameController.Instance.SceneIndex = 2;
        DOTween.KillAll();
        SoundManager._soundManager.day.Stop();
        SoundManager._soundManager.night.Stop();

        GameManager.instance.ChangeSceneAsync(1, () =>
        {
            ButtonInteractionState(true);
            SoundManager._soundManager._otherSounds.SoundToBeUsed(10, SoundManager.Soundtype.other, true, true);
            SoundManager._soundManager.day.DOFade(0, 2f);
            SoundManager._soundManager.night.DOFade(0, 2f);
            Debug.Log("OnBackgroundSceneLoaded Callback");
            UIController.Instance.ShowNextScreen(ScreenType.MainMenu);
        });
    }


    public void HoverOnBackButton(GameObject obj)
    {
        obj.SetActive(true);
    }

    public void HoverOffBackButton(GameObject obj)
    {
        obj.SetActive(false);
    }

    

    /*void OnExitGameToMainmenu()
    {
        Debug.Log("Inside - ExitGame");
        Time.timeScale = 1;

        //transform.parent.transform.GetChild(3).GetComponent<GameplayUI>().StopTheTimer();
        GameplayUI gamplayScreen = UIController.Instance.GetScreen<GameplayUI>(ScreenType.Gameplay);
        if (gamplayScreen != null)
        {
            gamplayScreen.StopTheTimer();    
        }
        GameController.Instance.SceneIndex = 2;

        StartCoroutine(SwicthToMainMenu());

        SoundManager._soundManager.day.DOFade(0, 2f);
        SoundManager._soundManager.night.DOFade(0, 2f);
    }*/

    /*async void OnRestartGame()
    {
        UIController.Instance.HideScreen(ScreenType.Pause);

        await Task.Delay(2000);

        Time.timeScale = 1;

        GameManager.instance.currentGameState = GameStates.GameOver;

        transform.parent.GetChild(2).GetComponent<LevelMapUI>().ReloadLevelAfterGameOver();
        transform.parent.GetChild(3).GetComponent<GameplayUI>().ReserPowerUpData();
    }*/

    /*IEnumerator SwicthToMainMenu()
    {
        ButtonInteractionState(false);

        DOTween.KillAll();

        UIController.Instance.HideScreen(ScreenType.Pause);

        yield return new WaitForSeconds(1f);

        UIController.Instance.HideScreen(ScreenType.Gameplay);

        SceneLoaderUI.Instance.Show();

        yield return new WaitForSeconds(1f);

        SoundManager._soundManager.day.Stop();
        SoundManager._soundManager.night.Stop();

       // transform.parent.GetChild(1).GetComponent<AnimationAnimatable>().ResetAnimator();

        sceneLoadOperation = SceneManager.LoadSceneAsync(1);
        sceneLoadOperation.allowSceneActivation = false;

        while (!sceneLoadOperation.isDone && sceneLoadOperation.progress < 0.9f)
        {
            yield return null;
        }

        sceneLoadOperation.allowSceneActivation = true;
        yield return new WaitForSeconds(0.5f);

        SceneLoaderUI.Instance.Hide();

        ButtonInteractionState(true);

        UIController.Instance.ShowScreen(ScreenType.MainMenu);

        SoundManager._soundManager._otherSounds.SoundToBeUsed(10, SoundManager.Soundtype.other, true, true);
    }*/


    /*public void SwitchToPause()
    {
        if (!InputController.GetPauseMenu())
        {
            if (GameManager.instance.currentGameState == GameStates.alive)
            {
                InputController.SetPauseMenu(true);

                Time.timeScale = 0;

                state = GameManager.instance.currentGameState;

                //UIController.Instance.ShowScreen(ScreenType.Pause);
                UIController.Instance.ShowNextScreen(ScreenType.Pause);

                GameManager.instance.currentGameState = GameStates.paused;
            }
            else if (GameManager.instance.currentGameState == GameStates.paused)
            {
                InputController.SetPauseMenu(true);

                Time.timeScale = 1;

                UIController.Instance.ShowNextScreen(ScreenType.Gameplay);

                GameManager.instance.currentGameState = state;
            }
        }
    }


    IEnumerator SwitchScreen()
    {
        yield return new WaitForSecondsRealtime(0.2f);

        InputController.SetPauseMenu(false);
    }

    public override void OnScreenShowAnimationCompleted()
    {
        base.OnScreenShowAnimationCompleted();
        StartCoroutine(SwitchScreen());
    }

    public override void OnScreenHideAnimationCompleted()
    {
        base.OnScreenHideAnimationCompleted();

        InputController.SetPauseMenu(false);
    }

    public void PauseSettings()
    {
        //StartCoroutine(SwitchScreen(ScreenType.Settings, ScreenType.Pause));
        UIController.Instance.ShowNextScreen(ScreenType.Settings);
    }*/

    /*IEnumerator SwitchScreen(ScreenType show, ScreenType hide)
    {
        ButtonInteractionState(false);

        yield return new WaitForSecondsRealtime(1f);

        UIController.Instance.HideScreen(hide);

        SceneLoaderUI.Instance.Show();

        yield return new WaitForSecondsRealtime(1f);

        SceneLoaderUI.Instance.Hide();

        UIController.Instance.ShowScreen(show);

        ButtonInteractionState(true);

    }*/
}
