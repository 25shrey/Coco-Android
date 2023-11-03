using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.BaseFramework
{
    [Serializable]
    public class UIScreen
    {
        public ScreenType screenType;
        public UIScreenView screenView;
    }
    [Serializable]
    public class UIPopup
    {
        public PopupType popupType;
        public UIPopupView popupView;
    }
    public enum ScreenType
    {
        None,
        Splash,
        MainMenu,
        Gameplay,
        LevelMap,
        Settings,
        Customization,
        Pause,
        Powerups,
        BalloonSkins,
        Outfits,
        Accessories,
        Shop, 
        Cloud,
        CoinGems,
        VFXEffects
    }
    public enum PopupType
    {
        None,
        CommonPopup 
    }

    public class UIController : Singleton<UIController>
    {
        public ScreenType StartScreen;
        public List<UIScreen> Screens;
        public List<UIPopup> Popups;

        [SerializeField]
        List<ScreenType> currentScreens;
        [SerializeField] 
        List<UIPopup> currentPopup;
        [HideInInspector]
        public ScreenType previousScreen;
        public static float AspectRatio;

        public List<TMP_Text> coinTexts;
        public List<TMP_Text> gemsTexts;

        public TMP_Text powerUpFireBallText;
        public TMP_Text powerUpShieldText;
        public TMP_Text powerUpMagnetText;
        public TMP_Text shopPowerUpFireBallText;
        public TMP_Text shopPowerUpShieldText;
        public TMP_Text shopPowerUpMagnetText;

        public ScreenType LastScreenOnTheList
        {
            get { return currentScreens[currentScreens.Count - 1]; }
        }

        public ScreenType FirstScreenOnTheList
        {
            get { return currentScreens[0];}
        }

        public int CurrentScreensLength
        {
            get { return currentScreens.Count; }
        }

        public override void Awake()
        {
            base.Awake();
            AspectRatio = Screen.width / (Screen.height * 1f);
        }

        private IEnumerator Start()
        {
           // Application.targetFrameRate = (int)Screen.currentResolution.refreshRateRatio.value;
            currentScreens = new List<ScreenType>();
            SceneLoaderUI.Instance.Show();

            yield return null;
            ShowScreen(StartScreen);

            yield return new WaitForSeconds(1f);

            //SavedDataHandler.Instance.SetFirstLaunch();
        }
        
        public void RemoveUnwantedScreen()
        {
            int count = 0;
            for (int i = 0; i < currentScreens.Count; i++)
            {
                if (ScreenType.Cloud == currentScreens[i])
                {
                    count++;
                }
            }

            while (count > 0)
            {
                currentScreens.Remove(ScreenType.Cloud);
                count--;
            }
        }

        public void ShowNextScreen(ScreenType screenType, float Delay = 0.2f)
        {
            Debug.Log("Inside - ShowNextScreen ScreenType : " + screenType);
            if (currentScreens.Count > 0)
            {
                HideScreen(currentScreens.Last());
            }
            else
            {
                Delay = 0;
            }

            StartCoroutine(ExecuteAfterDelay(Delay, () =>
            {
                ShowScreen(screenType);
            }));
        }

        public void ShowScreen(ScreenType screenType)
        {
            getScreen(screenType).Show();

            currentScreens.Add(screenType);
        }

        public void HideScreen(ScreenType screenType)
        {
            getScreen(screenType).Hide();

            currentScreens.Remove(screenType);
        }

        public UIScreenView getScreen(ScreenType screenType)
        {
            return Screens.Find(screen => screen.screenType == screenType).screenView;
        }

        public UIPopupView GetPopup(PopupType popupType)
        {
            return Popups.Find(pop => pop.popupType == popupType).popupView;
        }
        UIPopup _popup = null;
        
        
        public void ShowPopup(PopupType popup)
        {
            _popup = Popups.Find(x => x.popupType == popup);
            if (_popup == null) return;
            Debug.Log("Popup null");
            if (currentPopup.Contains(_popup)) return;
            Debug.Log("Popup contains");
            Events.ScreenChanged(false);
            _popup.popupView.previousPopup = (currentPopup.Count == 0) ? getScreen(currentScreens.Last()) : currentPopup.Last().popupView;
            currentPopup.Add(_popup);
            _popup.popupView.Show();
        }
        //private void Update()
        //{
        //    OpenScreens();
        //    Debug.Log("POpup : "+currentPopup.Count);
        //}
        public void HidePopup(PopupType popupType)
        {
            Debug.Log("Start Hide Popup :" + popupType + " || " + currentPopup.Count);
            Events.ScreenChanged(false);
            GetPopup(popupType).Hide();
            currentPopup.Remove(currentPopup.Find(pop => pop.popupType == popupType));
            Debug.Log("End Hide Popup :" + popupType + " || " + currentPopup.Count);
            //if (currentPopup.Count == 0)
            //    Helper.Execute(this, () => getScreen(getCurrentScreen()).ToggleRaycaster(true), 0.8f);
        }

        public ScreenType getCurrentScreen()
        {
            return currentScreens.Last();
        }

        public ScreenType GetLastOpenScreen()
        {
            return currentScreens[currentScreens.Count - 1];
        }
        IEnumerator ExecuteAfterDelay(float Delay, Action CallbackAction)
        {
            yield return new WaitForSecondsRealtime(Delay);

            CallbackAction();
        }
        
        public T GetPopup<T>(PopupType sName) => (T)Popups.Find(pop => pop.popupType == sName).popupView.GetComponent<T>();
        public T GetScreen<T>(ScreenType sName) => (T)Screens.Find(screen => screen.screenType == sName).screenView.GetComponent<T>();
    }

}