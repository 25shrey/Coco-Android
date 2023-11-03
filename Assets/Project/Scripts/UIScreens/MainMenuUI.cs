using Game.BaseFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : UIScreenView
{
    public Button newGameButton;
    public Button continueGameButton;
    public Button customiseButton;
    public Button settingButton;
    public Button shopButton;
    public Button loadButton;
    public Button exitButton;

    public TMP_InputField tempCoins;
    public TMP_InputField tempGems;

    [SerializeField] private GameObject firstSelectedGameobject;

    AsyncOperation sceneLoadOperation;

    public override void OnScreenShowAnimationCompleted()
    {
        base.OnScreenShowAnimationCompleted();
        //StartCoroutine(SwitchScreen());
    }

    public override void OnScreenShowCalled()
    {
        base.OnScreenShowCalled();
        if (Gamepad.current != null)
        {
            EventSystem.current.SetSelectedGameObject(firstSelectedGameobject);
        }
        exitButton.onClick.AddListener(OnExitButtonClicked);
    }

    public override void OnScreenHideCalled()
    {
        base.OnScreenHideCalled();
        EventSystem.current.SetSelectedGameObject(null);
        exitButton.onClick.RemoveListener(OnExitButtonClicked);
    }


    IEnumerator SwitchScreen()
    {
        ButtonInteractionState(false);

        yield return new WaitForSeconds(1f);

        UIController.Instance.HideScreen(ScreenType.MainMenu);
        SceneLoaderUI.Instance.Show();

        yield return new WaitForSeconds(1f);

        sceneLoadOperation = SceneManager.LoadSceneAsync(2);
        sceneLoadOperation.allowSceneActivation = false;

        while (!sceneLoadOperation.isDone && sceneLoadOperation.progress < 0.9f)
        {
            yield return null;
        }

        sceneLoadOperation.allowSceneActivation = true;
        yield return new WaitForSeconds(0.5f);


        SceneLoaderUI.Instance.Hide();


        UIController.Instance.ShowScreen(ScreenType.LevelMap);

        ButtonInteractionState(true);
    }


    public override void OnScreenHideAnimationCompleted()
    {
        base.OnScreenHideAnimationCompleted();
    }

    public void NewGame()
    {
        Debug.Log("New Game");
        SavedDataHandler.Instance.ResetData();
        //transform.parent.GetChild(2).GetComponent<LevelMapUI>().UpdateLevelUI();
        LevelMapUI levelmap = UIController.Instance.GetScreen<LevelMapUI>(ScreenType.LevelMap);
        levelmap.UpdateLevelUI();
        StartCoroutine(SwitchScreen()); 
    }

    public void ContinueGame()
    {
        transform.parent.GetChild(2).GetComponent<LevelMapUI>().UpdateLevelUI();
        StartCoroutine(SwitchScreen());
    }

    public void CustomiseGame()
    {
        //StartCoroutine(SwitchScreenInMain(ScreenType.Customization, ScreenType.MainMenu));
        UIController.Instance.ShowNextScreen(ScreenType.Customization);
        //StartCoroutine(SwitchScreen(ScreenType.Customization, ScreenType.MainMenu));
        //StartCoroutine(SwitchScreen(ScreenType.Powerups, ScreenType.MainMenu));
        //  transform.parent.GetChild(6).GetComponent<AnimationAnimatable>().ResetAnimator();
    }

    public void SettingsGame()
    {
        //StartCoroutine(SwitchScreenInMain(ScreenType.Settings, ScreenType.MainMenu));
        UIController.Instance.ShowNextScreen(ScreenType.Settings);
      //  transform.parent.GetChild(5).GetComponent<AnimationAnimatable>().ResetAnimator();
    }

    public void ShopGame()
    {
        StartCoroutine(SwitchScreenInMain(ScreenType.Shop, ScreenType.MainMenu));
       // transform.parent.GetChild(11).GetComponent<AnimationAnimatable>().ResetAnimator();
    }
    public void HoverOnBackButton(GameObject obj)
    {
        obj.SetActive(true);
    }

    public void HoverOffBackButton(GameObject obj)
    {
        obj.SetActive(false);
    }

    private void ButtonInteractionState(bool value)
    {
        newGameButton.interactable = value;
        continueGameButton.interactable = value;
        customiseButton.interactable = value;
        settingButton.interactable = value;
        shopButton.interactable = value;
        loadButton.interactable = value;
    }

    private void OnEnable()
    {
        GameController._newGameDelegate += NewGame;
        GameController._continueGameDelegate += ContinueGame;
        GameController._customise += CustomiseGame;
        GameController._settings += SettingsGame;
        GameController._shop += ShopGame;

        newGameButton.onClick.AddListener(GameController.Instance.NewGameHit);
        continueGameButton.onClick.AddListener(GameController.Instance.ContinueGameHit);
        customiseButton.onClick.AddListener(GameController.Instance.CustomiseHit);
        settingButton.onClick.AddListener(GameController.Instance.SettingsHit);
        shopButton.onClick.AddListener(GameController.Instance.ShopHit);
    }

    private void OnDisable()
    {
        GameController._newGameDelegate -= NewGame;
        GameController._continueGameDelegate -= ContinueGame;
        GameController._customise -= CustomiseGame;
        GameController._settings -= SettingsGame;
        GameController._shop -= ShopGame;
    }


    IEnumerator SwitchScreenInMain(ScreenType show, ScreenType hide)
    {
        ButtonInteractionState(false);

        yield return new WaitForSeconds(1f);

        UIController.Instance.HideScreen(hide);

        SceneLoaderUI.Instance.Show();

        yield return new WaitForSeconds(1f);

        sceneLoadOperation = SceneManager.LoadSceneAsync(2);

        sceneLoadOperation.allowSceneActivation = false;

        while (!sceneLoadOperation.isDone && sceneLoadOperation.progress < 0.9f)
        {
            yield return null;
        }

        sceneLoadOperation.allowSceneActivation = true;
        yield return new WaitForSeconds(0.5f);


        SceneLoaderUI.Instance.Hide();

        UIController.Instance.ShowScreen(show);

        ButtonInteractionState(true);
    }
    
    IEnumerator SwitchScreen(ScreenType show, ScreenType hide)
    {
        ButtonInteractionState(false);

        yield return new WaitForSeconds(1f);

        UIController.Instance.HideScreen(hide);

        SceneLoaderUI.Instance.Show();

        yield return new WaitForSeconds(1f);

        /*sceneLoadOperation = SceneManager.LoadSceneAsync(2);

        sceneLoadOperation.allowSceneActivation = false;

        while (!sceneLoadOperation.isDone && sceneLoadOperation.progress < 0.9f)
        {
            yield return null;
        }

        sceneLoadOperation.allowSceneActivation = true;*/
        yield return new WaitForSeconds(0.5f);


        SceneLoaderUI.Instance.Hide();

        UIController.Instance.ShowScreen(show);

        ButtonInteractionState(true);
    }

    private void OnExitButtonClicked()
    {
        UIController.Instance.ShowPopup(PopupType.CommonPopup);
        CommonPopup popup = UIController.Instance.GetPopup<CommonPopup>(PopupType.CommonPopup);
        if (popup != null)
        {
            string exitText = "Are you sure you want to Close the Game?"; 
            popup.SetData(CommonPopupType.Message, exitText, true, () =>
            {
                Debug.Log("Game Exit");
                Application.Quit();
            },
                () =>
                {
                    UIController.Instance.HidePopup(PopupType.CommonPopup);
                });
        }
    }

    public void ContinueButtonState()
    {
        continueGameButton.gameObject.SetActive(true);
    }

    public void TempAddCoinsGems()
    {
        if (tempCoins!=null)
        {
            Debug.Log(tempCoins.text);
            string temp = tempCoins.text;
            SavedDataHandler.Instance.Coin += Convert.ToInt32(temp);
        }
        if (tempGems!=null)
        {
            Debug.Log(tempGems.text);
            string temp = tempGems.text;
            SavedDataHandler.Instance.Gems += Convert.ToInt32(temp);
        }
    }
    
}
