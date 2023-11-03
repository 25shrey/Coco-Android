using Game.BaseFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OutfitsUI : UIScreenView
{
    public Button backButton;

    public override void OnScreenShowAnimationCompleted()
    {
        base.OnScreenShowAnimationCompleted();
    }

    public override void OnScreenHideAnimationCompleted()
    {
        base.OnScreenHideAnimationCompleted();
    }

    private void OnEnable()
    {
        backButton.onClick.AddListener(backButtonHit);
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

    }

    private void ButtonInteractionState(bool value)
    {
        backButton.interactable = value;
    }

    void backButtonHit()
    {
        StartCoroutine(BackButtonCoroutine());
    }

    IEnumerator BackButtonCoroutine()
    {
        ButtonInteractionState(false);

        yield return new WaitForSeconds(1f);

        UIController.Instance.HideScreen(ScreenType.Outfits);

        SceneLoaderUI.Instance.Show();

        yield return new WaitForSeconds(1f);

        SceneLoaderUI.Instance.Hide();

        UIController.Instance.ShowScreen(ScreenType.Customization);

        ButtonInteractionState(true);
    }
}
