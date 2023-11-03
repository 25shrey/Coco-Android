using Game.BaseFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneLoaderUI : UIScreenView
{
    public static SceneLoaderUI Instance;

    public Action OnLoaderShow;
    public Action OnLoaderHide;

    public override void OnAwake()
    {
        base.OnAwake();
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public override void OnScreenShowAnimationCompleted()
    {
        base.OnScreenShowAnimationCompleted();
        OnLoaderShow?.Invoke();
    }

    public override void OnScreenHideAnimationCompleted()
    {
        base.OnScreenHideAnimationCompleted();
        OnLoaderHide?.Invoke();
    }
}
