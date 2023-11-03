using Game.BaseFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneTransitionCloudUI : UIScreenView
{
    public Animator cloudAnim;
    public bool levelLoadClouds;
    public bool levelEndClouds;
    public Animator anim;
    public GameObject _img;
    public bool isShowingClouds;
    
    public override void OnScreenShowAnimationCompleted()
    {
        base.OnScreenShowAnimationCompleted();
    }

    public override void OnScreenHideAnimationCompleted()
    {
        base.OnScreenHideAnimationCompleted();
    }

    public void RemoveClouds(float sec = 0f)
    {
        StartCoroutine(RemoveCloudsCoroutine(sec));
    }

    private void OnDisable()
    {
        GameManager._cloudsEnd -= RemoveClouds;
    }

    IEnumerator RemoveCloudsCoroutine(float sec = 0f)
    {
        while (GameManager.instance==null)
        {
            yield return null;
        }

        if (GameManager.instance.endCloudScreen)
        {
            GameManager.instance.endCloudScreen = false;
        }

        cloudAnim.SetBool("MoveClouds", false);

        yield return new WaitForSeconds(sec);

        UIController.Instance.HideScreen(ScreenType.Cloud);

        if (!GameManager.instance.initateCloudScreen)
        {
            GameManager.instance.initateCloudScreen = true;
        }

        yield return new WaitForSeconds(1f);
        
        transform.GetComponent<Canvas>().enabled = false;

        GS.Instance.input.EnableInput();

        isShowingClouds = false;
    }

    IEnumerator ShowCloudsCoroutine()
    {
        GS.Instance.input.DisableInput();

        isShowingClouds = true;

        cloudAnim.SetBool("MoveClouds", true);

        UIController.Instance.ShowScreen(ScreenType.Cloud);

        yield return new WaitForSeconds(0.1f);


        if (levelLoadClouds && !levelEndClouds)
        {
            if (!GameManager.instance.endCloudScreen)
            {
                GameManager.instance.endCloudScreen = true;
            }
        }

        else
        {
            levelLoadClouds = true;
        }
        cloudAnim.SetFloat("Speed", 3f);
    }

    public void ShowClouds()
    {
        cloudAnim.SetFloat("Speed", 1f);
        StartCoroutine(ShowCloudsCoroutine());
    }


    public void SuscribeClouds()
    {
        GameManager._cloudsEnd += RemoveClouds;
    }

    //public void Grow()
    //{
    //    StartCoroutine(ShowGrow(2f));
    //}

    //public void Shrink()
    //{
    //    StartCoroutine(ShowShrink());
    //}

    public void ImageStateSetter(bool state)
    {
        _img.SetActive(state);
    }


    //IEnumerator ShowShrink()
    //{
    //    _img.SetActive(true);

    //    anim.SetTrigger("UI_Animation");

    //    UIController.Instance.ShowScreen(ScreenType.Cloud);

    //    yield return new WaitForSeconds(0.1f);


    //    if (levelLoadClouds && !levelEndClouds)
    //    {
    //        if (!GameManager.instance.endCloudScreen)
    //        {
    //            GameManager.instance.endCloudScreen = true;
    //        }
    //    }

    //    else
    //    {
    //        levelLoadClouds = true;
    //    }
    //}

    //IEnumerator ShowGrow(float sec)
    //{
    //    while (GameManager.instance == null)
    //    {
    //        yield return null;
    //    }

    //    anim.SetTrigger("UI_Animation_Reverse");

    //    if (GameManager.instance.endCloudScreen)
    //    {
    //        GameManager.instance.endCloudScreen = false;
    //    }

    //    yield return new WaitForSeconds(sec);

    //    UIController.Instance.HideScreen(ScreenType.Cloud);

    //    if (!GameManager.instance.initateCloudScreen)
    //    {
    //        GameManager.instance.initateCloudScreen = true;
    //    }

    //    _img.SetActive(false);

    //    yield return new WaitForSeconds(0.5f);

    //    GS.Instance.input.EnableInput();
    //}
}
