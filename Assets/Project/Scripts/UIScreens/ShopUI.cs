using Game.BaseFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
    
public class ShopUI : UIScreenView
{
    public Button backButton;

    AsyncOperation sceneLoadOperation;

    public override void OnScreenShowAnimationCompleted()
    {
        base.OnScreenShowAnimationCompleted();
    }

    public override void OnScreenHideAnimationCompleted()
    {
        base.OnScreenHideAnimationCompleted();
    }

    public void HoverOnBackButton(GameObject obj)
    {
        obj.SetActive(true);
    }

    public void HoverOffBackButton(GameObject obj)
    {
        obj.SetActive(false);
    }

    private void OnEnable()
    {
        backButton.onClick.AddListener(backButtonHit);
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

        UIController.Instance.HideScreen(ScreenType.Shop);

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
}
