using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Game.BaseFramework
{
    public class UIScreenView : UIView
    {
        
    }

    public class UIPopupView : UIView
    {
        [HideInInspector] public UIView previousPopup = null;

        public override void OnScreenHideAnimationCompleted()
        {
            base.OnScreenHideAnimationCompleted();
            Debug.Log("isLoading : " + isLoading);
            if (previousPopup != null)
            {
                previousPopup.ToggleRaycaster(true);
                previousPopup = null;
            }
        }
    }
}