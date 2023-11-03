using Game.BaseFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CustomiseUI : UIScreenView
{
    public Button backButton;
    public Button powerButton;
    public Button outfitButton;
    public Button accessoriesButton;
    public Button balloonSkinButton;
    public Button coinGemsButton;
    public Button vfxButton;
    private ScreenType lastScreen;
    public List<Canvas> panelList;
    [SerializeField] private GameObject firstSelectedObject;

    public List<Button> AllButtons = new List<Button>();    

    AsyncOperation sceneLoadOperation;

    private void OnEnable()
    {
        base.OnScreenShowAnimationCompleted();
        GameController._powerups += PowerUps;
        GameController._accessories += Accessories;
        GameController._outfits += Outfits;
        GameController._balloonskins += Balloonskins;
        GameController._coinGems += CoinGemsPacks;
        GameController._vfxEffects += VFXEffects;

        powerButton.onClick.AddListener(GameController.Instance.PowerUpHit);
        outfitButton.onClick.AddListener(GameController.Instance.OutfitsHit);
        accessoriesButton.onClick.AddListener(GameController.Instance.AccessoriesHit);
        balloonSkinButton.onClick.AddListener(GameController.Instance.BalloonSkinsHit);
        coinGemsButton.onClick.AddListener(GameController.Instance.CoinGemsHit);
        vfxButton.onClick.AddListener(GameController.Instance.VFXEffectsHit);
    }

    public override void OnScreenHideAnimationCompleted()
    {
        base.OnScreenHideAnimationCompleted();
    }

    public override void OnScreenShowCalled()
    {
        base.OnScreenShowCalled();
        
        EventSystem.current.SetSelectedGameObject(firstSelectedObject);
        
        backButton.onClick.AddListener(Back);
    }

    public override void OnScreenHideCalled()
    {
        base.OnScreenHideCalled();
        EventSystem.current.SetSelectedGameObject(null);
        backButton.onClick.RemoveListener(Back);
    }

    public void HoverOnBackButton(GameObject obj)
    {
        obj.SetActive(true);
    }

    public void HoverOffBackButton(GameObject obj)
    {
        obj.SetActive(false);
    }

    private void OnDisable()
    {
        GameController._powerups -= PowerUps;
        GameController._accessories -= Accessories;
        GameController._outfits -= Outfits;
        GameController._balloonskins -= Balloonskins;
        GameController._coinGems -= CoinGemsPacks;
        GameController._vfxEffects -= VFXEffects;

        backButton.onClick.RemoveListener(OnBack);
        powerButton.onClick.RemoveListener(GameController.Instance.PowerUpHit);
        outfitButton.onClick.RemoveListener(GameController.Instance.OutfitsHit);
        accessoriesButton.onClick.RemoveListener(GameController.Instance.AccessoriesHit);
        balloonSkinButton.onClick.RemoveListener(GameController.Instance.BalloonSkinsHit);
        coinGemsButton.onClick.RemoveListener(GameController.Instance.CoinGemsHit);
        vfxButton.onClick.RemoveListener(GameController.Instance.VFXEffectsHit);
    }

    private void ButtonInteractionState(bool value)
    {
        backButton.interactable = value;
        powerButton.interactable = value;
        outfitButton.interactable = value;
        accessoriesButton.interactable = value;
        balloonSkinButton.interactable = value;
        coinGemsButton.interactable = value;
        vfxButton.interactable = value;
    }

    void PowerUps()
    {
        StartCoroutine(ShowScreen(ScreenType.Customization, ScreenType.Powerups));
    }

    void Accessories()
    {
        StartCoroutine(ShowScreen(ScreenType.Customization, ScreenType.Accessories));
    }

    void Balloonskins()
    {
        StartCoroutine(ShowScreen(ScreenType.Customization, ScreenType.BalloonSkins));
    }

    void CoinGemsPacks()
    {
        StartCoroutine(ShowScreen(ScreenType.Customization, ScreenType.CoinGems));
    }

    void VFXEffects()
    {
        StartCoroutine(ShowScreen(ScreenType.Customization, ScreenType.VFXEffects));
    }

    void Outfits()
    {
        StartCoroutine(ShowScreen(ScreenType.Customization, ScreenType.Outfits));
    }

    public override void OnBack()
    {
        base.OnBack();
        //UIController.Instance.ShowNextScreen(ScreenType.MainMenu);
    }

    public void Back()
    {
        SoundManager._soundManager._sounds.SoundToBeUsed(3, SoundManager.Soundtype.other, false, true);
        UIController.Instance.ShowNextScreen(ScreenType.MainMenu);
    }

    void backButtonHit()
    {
        StartCoroutine(BackButtonCoroutine());
        // UIController.Instance.ShowNextScreen(Main);
    }

    IEnumerator BackButtonCoroutine()
    {
        ButtonInteractionState(false);

        yield return new WaitForSeconds(1f);

        lastScreen = UIController.Instance.GetLastOpenScreen();

        UIController.Instance.HideScreen(ScreenType.Customization);

        UIController.Instance.HideScreen(lastScreen);

        SceneLoaderUI.Instance.Show();

        yield return new WaitForSeconds(1f);

        sceneLoadOperation = SceneManager.LoadSceneAsync(1);

        sceneLoadOperation.allowSceneActivation = false;

        while (!sceneLoadOperation.isDone && sceneLoadOperation.progress < 0.9f)
        {
            yield return null;
        }

        sceneLoadOperation.allowSceneActivation = true;
        transform.parent.GetChild(1).GetComponent<AnimationAnimatable>().ResetAnimator();

        yield return new WaitForSeconds(0.5f);

        SceneLoaderUI.Instance.Hide();

        UIController.Instance.ShowScreen(ScreenType.MainMenu);

        ButtonInteractionState(true);
    }

    IEnumerator ShowScreen(ScreenType hide, ScreenType show)
    {
        ButtonInteractionState(false);

        yield return new WaitForSeconds(1f);

        lastScreen = UIController.Instance.GetLastOpenScreen();
        UIController.Instance.HideScreen(lastScreen);
        SceneLoaderUI.Instance.Show();

        yield return new WaitForSeconds(1f);

        SceneLoaderUI.Instance.Hide();

        UIController.Instance.ShowScreen(show);

        ButtonInteractionState(true);
    }

    public void MainMenuCocoState(bool playerIdle)
    {
        CocoMainMenuHandler.Instance.mainMenuCoco.SetPlayerState(playerIdle);
    }

    public void EnableDisablePanel(int index)
    {
        for (int i = 0; i < panelList.Count; i++)
        {
            if (i==index)
            {
                SoundManager._soundManager._sounds.SoundToBeUsed(3, SoundManager.Soundtype.other, false, true);
                panelList[i].enabled = true;
            }
            else
            {
                panelList[i].enabled = false;
            }
        }
    }

    public void BackButtonClick()
    {
        CocoMainMenuHandler.Instance.ResetCocoRotation();
    }

    public void SelectByController(GameObject obj)
    {
        HoverOnBackButton(obj);
    }

    public void DeselectedByController(GameObject obj)
    {
        HoverOffBackButton(obj);
    }
}