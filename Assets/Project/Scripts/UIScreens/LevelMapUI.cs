using Game.BaseFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

public class LevelMapUI : UIScreenView
{

    public Levels[] levelsArray;
    [SerializeField]
    private TMP_Text levelText;
    [SerializeField]
    private bool screenLoadingStarted;
    private Coroutine MuteCo;

    private SceneTransitionCloudUI clouds;
    private GameplayUI game;

    private void Start()
    {
        game = UIController.Instance.GetScreen<GameplayUI>(ScreenType.Gameplay);
        clouds = GameController.Instance.clouds;
    }

    public override void OnScreenShowAnimationCompleted()
    {
        if (!screenLoadingStarted)
        {
            base.OnScreenShowAnimationCompleted();
            StartCoroutine(SwitchScreen(ScreenType.LevelMap));
            screenLoadingStarted = true;
        }
        Debug.Log("--------");
    }

    public override void OnScreenHideAnimationCompleted()
    {
        base.OnScreenHideAnimationCompleted();
        screenLoadingStarted = false;
    }

    public override void OnScreenHideCalled()
    {
        base.OnScreenHideCalled();
    }

    public override void OnScreenShowCalled()
    {
        base.OnScreenShowCalled();
    }

    public override void Show()
    {
        base.Show();
    }

    AsyncOperation sceneLoadOperation;

    //IEnumerator SwitchScreen(ScreenType screen)
    //{
    //    UpdateLevelUI();

    //    yield return new WaitForSeconds(1f);

    //    UIController.Instance.HideScreen(screen);

    //    SceneLoaderUI.Instance.Show();

    //    yield return new WaitForSeconds(1f);

    //    if (screenLoadingStarted)
    //    {
    //        OnScreenHideAnimationCompleted();
    //    }
    //    DOTween.KillAll();

    //    sceneLoadOperation = SceneManager.LoadSceneAsync(GameController.Instance.SceneIndex);

    //    sceneLoadOperation.allowSceneActivation = false;

    //    UIController.Instance.ShowScreen(ScreenType.Cloud);

    //    while (!sceneLoadOperation.isDone && sceneLoadOperation.progress < 0.9f)
    //    {
    //        // UIController.Instance.HideScreen(ScreenType.Cloud);
    //        if (!clouds.isShowingClouds)
    //        {
    //            clouds.ShowClouds();
    //        }
    //        yield return null;
    //    }

    //    // UIController.Instance.HideScreen(ScreenType.Cloud);
    //    SoundManager._soundManager._otherSounds.SoundToBeUsed(2, SoundManager.Soundtype.other, false, false);

    //    game._fadeImg.DOFade(0f, 0f);

    //    sceneLoadOperation.allowSceneActivation = true;

    //    yield return new WaitForSeconds(0.5f);

    //    SceneLoaderUI.Instance.Hide();

    //    yield return new WaitForSeconds(0.5f);

    //    SoundManager._soundManager._otherSounds.SoundToBeUsed(5, SoundManager.Soundtype.other, false, true);

    //    UIController.Instance.ShowScreen(ScreenType.Gameplay);

    //    SethTheLevelTimer(GameController.Instance.CurrentPlayingLevel);

    //    //transform.parent.GetChild(4).GetComponent<PauseMenuUI>().EnableThePauseMenuEvent();

    //    game.StartTheTimer();

    //    yield return new WaitForSeconds(0.5f);

    //    clouds.RemoveClouds(1f);

    //    UIController.Instance.RemoveUnwantedScreen();
    //}

    IEnumerator SwitchScreen(ScreenType screen)
    {
        UpdateLevelUI();

        yield return new WaitForSeconds(1f);

        UIController.Instance.HideScreen(screen);

        SceneLoaderUI.Instance.Show();

        yield return new WaitForSeconds(1f);

        if (screenLoadingStarted)
        {
            OnScreenHideAnimationCompleted();
        }
        DOTween.KillAll();

        sceneLoadOperation = SceneManager.LoadSceneAsync(GameController.Instance.SceneIndex);

        sceneLoadOperation.allowSceneActivation = false;

        while (!sceneLoadOperation.isDone && sceneLoadOperation.progress < 0.9f)
        {
            // UIController.Instance.HideScreen(ScreenType.Cloud);
            yield return null;
        }

        Debug.Log("Scene Loaded");
        // UIController.Instance.HideScreen(ScreenType.Cloud);
        SoundManager._soundManager._otherSounds.SoundToBeUsed(2, SoundManager.Soundtype.other, false, false);

        game._fadeImg.DOFade(0f, 0f);

        sceneLoadOperation.allowSceneActivation = true;

        yield return new WaitForSeconds(0.5f);

        SceneLoaderUI.Instance.Hide();

        yield return new WaitForSeconds(0.5f);

        SoundManager._soundManager._otherSounds.SoundToBeUsed(5, SoundManager.Soundtype.other, false, true);

        UIController.Instance.ShowNextScreen(ScreenType.Gameplay);

        SethTheLevelTimer(GameController.Instance.CurrentPlayingLevel);

        //transform.parent.GetChild(4).GetComponent<PauseMenuUI>().EnableThePauseMenuEvent();

        game.StartTheTimer();
    }

    public void ReloadLevelAfterGameOver()
    {
        GameController.Instance.clouds.ImageStateSetter(false);

        GS.Instance.input.EnableInput();

        if (GameManager.instance.currentGameState == GameStates.GameOver)
        {
            clouds.RemoveClouds();
        }

        UIController.Instance.RemoveUnwantedScreen();

        game._fadeImg.DOFade(1f,1f);

        PlayerStats.Instance.obj.Coins = 0;
        PlayerStats.Instance.obj.Health = 0;
        PlayerStats.Instance.obj.Score = 0;

        game.ReserPowerUpData();

        StartCoroutine(ReloadLevelAfterGameOverCo());
    }

    IEnumerator ReloadLevelAfterGameOverCo()
    {
        yield return new WaitForSeconds(1f);

        StartCoroutine(SwitchScreen(ScreenType.Gameplay));
    }


    //sets the seconds from the level array of scriptable object to the UI
    void SethTheLevelTimer(int levelNumber)
    {
        for (int i = 0; i < levelsArray.Length; i++)
        {
            if (levelNumber == levelsArray[i].levelNumber)
            {
                var game = UIController.Instance.GetScreen<GameplayUI>(ScreenType.Gameplay);
                game.SetTimer(levelsArray[i].data.seconds);
               // transform.parent.transform.GetChild(3).GetComponent<GameplayUI>().SetTimer(levelsArray[i].data.seconds);
            }
        }
    }

    //Method responsible for setting the scriptable objects after the level is completed
    //saves the data in SaveDataHandler class and sets the variable for the next level to load
    private void OnLevelIsCompleted(bool isCompleted, int health, int coins, int score)
    {
        for (int i = 0; i < levelsArray.Length; i++)
        {
            levelsArray[i].data.isNextUnlocked = true;

            if (levelsArray[i].levelNumber == GameController.Instance.CurrentPlayingLevel)
            {
                levelsArray[i].data.isCompleted = isCompleted;
                levelsArray[i].data.isNextUnlocked = isCompleted;
                levelsArray[i].data.health = health;
                levelsArray[i].data.coins = coins;
                levelsArray[i].data.score = score;

                SavedDataHandler.Instance._saveData.isFirstLaunch = false;

                break;
            }
        }

        SavedDataHandler.Instance.Coin += coins;
        
        SavedDataHandler.Instance._saveData.additionalHealth = 0;
        SavedDataHandler.Instance._saveData.totalCoinsCollected = 0;

        foreach (var item in levelsArray)
        {
            SavedDataHandler.Instance._saveData.additionalHealth += item.data.health;
            SavedDataHandler.Instance._saveData.totalCoinsCollected += item.data.coins;
        }


        SavedDataHandler.Instance._saveData.lastLevelCompleted = GameController.Instance.CurrentPlayingLevel;
        SaveGameData.Save(SavedDataHandler.Instance._saveData, SavedDataHandler.Instance.password);

        GameController.Instance.CurrentPlayingLevel += 1;
        GameController.Instance.SceneIndex += 1;

        DOTween.KillAll();

        PlayerStats.Instance.obj.Coins = 0;
        PlayerStats.Instance.obj.Health = 0;
        PlayerStats.Instance.obj.Score = 0;

        if (SoundManager._soundManager.day.isPlaying)
        {
            SoundManager._soundManager.day.Stop();
        }
        else
        {
            SoundManager._soundManager.night.Stop();
        }

        if (GameController.Instance.CurrentPlayingLevel > 20)
        {
            StartCoroutine(SwitchScreenAfterLevel(ScreenType.MainMenu, 1));
        }
        else
        {
            StartCoroutine(SwitchScreenAfterLevel(ScreenType.LevelMap, 2));
        }
        return;
    }


    //always sets the level array to zero if the file is not present
    public void InitialeLevelHider()
    {
        foreach (var item in levelsArray)
        {
            item.data.isNextUnlocked = false;
            item.data.isCompleted = false;
            item.data.health = 0;
            item.data.coins = 0;
            item.data.score = 0;
        }
    }

    private void OnEnable()
    {
        LevelComplete.levelCompleteDelegate += OnLevelIsCompleted;
        GameController._restartGameAfterDeath += ReloadLevelAfterGameOver;
    }

    private void OnDisable()
    {
        LevelComplete.levelCompleteDelegate -= OnLevelIsCompleted;
        GameController._restartGameAfterDeath -= ReloadLevelAfterGameOver;
    }

    //temprary switch screen after level 10
    IEnumerator SwitchScreenAfterLevel(ScreenType screen, int index)
    {
        clouds.ShowClouds();
        clouds.levelLoadClouds = false;

        SoundManager._soundManager.MuteAll(true);

        yield return new WaitForSeconds(1f);

        UIController.Instance.HideScreen(ScreenType.Gameplay);
        
        SceneLoaderUI.Instance.Show();
       
        yield return new WaitForSeconds(1f);

        sceneLoadOperation = SceneManager.LoadSceneAsync(index);
        clouds.RemoveClouds(1f);
        sceneLoadOperation.allowSceneActivation = false;

        while (!sceneLoadOperation.isDone && sceneLoadOperation.progress < 0.9f)
        {
            yield return null;
        }

        if (MuteCo == null)
        {
            MuteCo = StartCoroutine(Muteco());
        }

        sceneLoadOperation.allowSceneActivation = true;
        yield return new WaitForSeconds(0.5f);

        SceneLoaderUI.Instance.Hide();

        UIController.Instance.ShowScreen(screen);

        if (GameController.Instance.CurrentPlayingLevel > 20)
        {
            var menu = UIController.Instance.GetScreen<MainMenuUI>(ScreenType.MainMenu);
            menu.continueGameButton.enabled = true;

            transform.parent.GetChild(1).GetComponent<AnimationAnimatable>().ResetAnimator();

            GameController.Instance.CurrentPlayingLevel = 0;
            GameController.Instance.SceneIndex = 2;

            UpdateLevelUI();
        }
        else
        {
            UpdateLevelUI();
            yield return new WaitForSeconds(2f);
        }
    }

    IEnumerator Muteco()
    {
        while (GameManager.instance == null)
        {
            yield return null;
        }

        SoundManager._soundManager.MuteAll(false);

        MuteCo = null;
    }

    //for skipping a level
    public void SkipCompleteLevel()
    {
       // transform.parent.GetChild(3).GetComponent<GameplayUI>().StopTheTimer();
       //  OnLevelIsCompleted(true, 0, 0, 0);
    }

    public void UpdateLevelUI()
    {
        levelText.text = "LEVEL : " + GameController.Instance.CurrentPlayingLevel.ToString();
    }
}

[Serializable]
public class Levels
{
    public LevelData data;
    public int levelNumber;
}
