using DG.Tweening;
using Game.BaseFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class SplashUI : UIScreenView
{
    public Slider ProgressBar;

    private PlayerInput playerInput;
    AsyncOperation sceneLoadOperation;

    public override void OnAwake()
    {
        base.OnAwake();
        playerInput = GS.Instance.playerInput;
    }



    public override async void OnScreenShowCalled()
    {
        base.OnScreenShowCalled();
        playerInput.currentActionMap.Disable();
        //await Task.Delay(5000);
        StartCoroutine(SceneLoadRoutine());
    }

    public override void OnScreenHideCalled()
    {
        base.OnScreenHideCalled();
    }



    private IEnumerator SceneLoadRoutine()
    {
        yield return new WaitForSeconds(9);

        SceneLoaderUI.Instance.Hide();

        yield return new WaitForSeconds(1f);

        sceneLoadOperation = SceneManager.LoadSceneAsync(1);

        sceneLoadOperation.allowSceneActivation = false;

        while (!sceneLoadOperation.isDone && sceneLoadOperation.progress < 0.9f)
        {
            //float val = sceneLoadOperation.progress;
            //ProgressBar.value = val/2;
            Debug.Log("---"+sceneLoadOperation.progress);

            yield return new WaitForSeconds(1f);
        }

        //float time = Random.Range(1.5f, 3.5f);
        //ProgressBar.DOValue(Random.Range(0.1f,0.3f),time /1.75f);
        //yield return new WaitForSeconds(time);
        //time = Random.Range(1.5f, 3.5f);
        //ProgressBar.DOValue(Random.Range(0.4f,0.55f),time /2.5f);
        //yield return new WaitForSeconds(time);
        //time = Random.Range(1.5f, 3.5f);
        //ProgressBar.DOValue(Random.Range(0.825f,0.95f),time /2.5f);
        //yield return new WaitForSeconds(time);
        //time = Random.Range(3f, 4.5f);
        //ProgressBar.DOValue(1,time/1.5f);
        //yield return new WaitForSeconds(time);

        UIController.Instance.ShowNextScreen(ScreenType.MainMenu);
        sceneLoadOperation.allowSceneActivation = true;

        SoundManager._soundManager._otherSounds.SoundToBeUsed(10, SoundManager.Soundtype.other, true, true);
    }


}
