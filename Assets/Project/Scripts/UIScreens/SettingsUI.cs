using Game.BaseFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class SettingsUI : UIScreenView
{
    public Button backButton;
    [SerializeField] private GameObject firstSelectedObject;

    [Header("Setting Screen")] 
    [SerializeField] private SettingsScreenView settingScreens;
    [SerializeField] private Button controlsButton;
    [SerializeField] private Button graphicsButton;
    [SerializeField] private Button soundsButton;

    [Header("Settings - Graphics")] 
    [SerializeField] private CustomSelectable resolutionSelectable;
    [SerializeField] private CustomSelectable qualitySelectable;
    [SerializeField] private CustomSelectable fpsSelectable;
    private Resolution[] resolutions;


    [Header("Setting - Controls")] 
    [SerializeField] private CustomSelectable controlSelectable;
    [SerializeField] private GameObject inputScrollView;
    [SerializeField] private Button resetButton;
    [SerializeField] private Image gamepadImage;
    [SerializeField] private InputActionAsset inputActions; 
    [SerializeField] private Scrollbar verticalScrollbar;
    [SerializeField] private Button resetButtonGraphics;

    [Header("Settings - Sounds")]
    [SerializeField] private Slider backgroundSoundSlider;
    [SerializeField] private Slider sfxSoundSlider;
    [SerializeField] private Button resetButtonSound;

    private SettingData settingData;

    public List<GameObject> AllScreensObj;

    public override void Enable()
    {
        base.Enable();
        InputController.onControllerFound += AddOptionForController;
        InputController.onGameStartControlSwitch += SwitchControlsOnGameStart;
    }

    private void Start()
    {
        SetupResolutionsData();
        LoadSettingsData();
        ToggelObjects(0);
    }

    public override void OnScreenShowCalled()
    {
        base.OnScreenShowCalled();
        
        EventSystem.current.SetSelectedGameObject(firstSelectedObject);

        // Settings Button
        backButton.onClick.AddListener(OnBack);
        controlsButton.onClick.AddListener(OnControlsButtonClicked);
        graphicsButton.onClick.AddListener(OnGraphicsButtonClicked);
        soundsButton.onClick.AddListener(OnSoundsButtonClicked);

        // Settings - Graphics Events
        resolutionSelectable.onOptionsValueChanged += OnOptionsResolutionDataChanged;
        qualitySelectable.onOptionsValueChanged += OnQualityDataChanged;
        fpsSelectable.onOptionsValueChanged += OnFPSOptionsDataChanged;
        
        // Settings - Controller settings events
        controlSelectable.onOptionsValueChanged += OnControllerDataChanged;
        resetButton.onClick.AddListener(ResetAllKeyboardBinding);
            
        // Settings - Sounds
        backgroundSoundSlider.onValueChanged.AddListener(OnBackgroundMusicChange);
        sfxSoundSlider.onValueChanged.AddListener(OnSfxMusicChange);
        resetButtonSound.onClick.AddListener(RestSound);
        
        // Show Control Screen in Screen settings screen Show
        settingScreens.EnableCanvas(SettingsScreenView.SettingScreen.Controls);
        verticalScrollbar.value = 1f;
        ToggelObjects(0);
    }

    public override void OnScreenHideCalled()
    {
        base.OnScreenHideCalled();
        EventSystem.current.SetSelectedGameObject(null);
        //Setting Buttons
        backButton.onClick.RemoveListener(OnBack);
        controlsButton.onClick.RemoveListener(OnControlsButtonClicked);
        graphicsButton.onClick.RemoveListener(OnGraphicsButtonClicked);
        soundsButton.onClick.RemoveListener(OnSoundsButtonClicked);
        
        // Settings - Graphics Events
        resolutionSelectable.onOptionsValueChanged -= OnOptionsResolutionDataChanged;
        qualitySelectable.onOptionsValueChanged -= OnQualityDataChanged;
        fpsSelectable.onOptionsValueChanged -= OnFPSOptionsDataChanged;
        

        // Settings - Controller settings events
        controlSelectable.onOptionsValueChanged -= OnControllerDataChanged;
        resetButton.onClick.RemoveListener(ResetAllKeyboardBinding);

        // Settings - Sounds
        backgroundSoundSlider.onValueChanged.RemoveListener(OnBackgroundMusicChange);
        sfxSoundSlider.onValueChanged.RemoveListener(OnSfxMusicChange);
        resetButtonSound.onClick.RemoveListener(RestSound);

        // Hide all Settings screens
        settingScreens.HideAllScreen();
        
        SaveControllerData();
        
        SaveSettingsData(settingData);
    }
    

    public override void Disable()
    {
        base.Disable();
        InputController.onControllerFound -= AddOptionForController;
        InputController.onGameStartControlSwitch -= SwitchControlsOnGameStart;
        RemoveResolutionData();
    }
    

    

    /*void backButtonHit()
    {
        if (UIController.Instance.FirstScreenOnTheList == ScreenType.Settings)
        {
            StartCoroutine(BackButtonCoroutine());
        }
        else if (UIController.Instance.LastScreenOnTheList == ScreenType.Settings &&
                 UIController.Instance.FirstScreenOnTheList == ScreenType.Gameplay)
        {
            StartCoroutine(BackToScreen(ScreenType.Settings, ScreenType.Pause));
        }
    }*/
    

    public override void OnBack()
    {
        base.OnBack();
        if (GS.Instance.IsGamePaused())
        {
            UIController.Instance.ShowNextScreen(ScreenType.Pause);
        }
        else
        {
            UIController.Instance.ShowNextScreen(ScreenType.MainMenu);

        }
    }

    public void SaveControllerData()
    {
        Debug.Log("SaveControllerData Called!");
        SavedDataHandler.Instance.SaveControllerRebingData();
    }

    public void OnHoverOnBackButton(GameObject obj)
    {
        obj.SetActive(true);
    }

    public void OnHoverOffBackButton(GameObject obj)
    {
        obj.SetActive(false);
    }

    public void Selected(GameObject obj)
    {
        OnHoverOnBackButton(obj);
    }

    public void Deselected(GameObject obj)
    {
        OnHoverOffBackButton(obj);
    }

    #region SETTING-GRAPHICS

    private void SetupResolutionsData()
    {
        resolutions = Screen.resolutions;
        resolutionSelectable.ClearAllOptions();
        int currentResolutionIndex = 0;
        List<string> options = new List<string>();
        for (int index = 0; index < resolutions.Length; index++)
        {
            string option = resolutions[index].width + "x" + resolutions[index].height;
            options.Add(option);
            if (resolutions[index].width == Screen.currentResolution.width &&
                resolutions[index].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = index;
            }
        }

        resolutionSelectable.AddOptions(options);
        resolutionSelectable.RefreshOption(currentResolutionIndex);
    }

    void ToggelObjects(int indice)
    {
        for (int index = 0; index < AllScreensObj.Count; index++)
        {
            if(indice == index)
            {
                AllScreensObj[index].SetActive(true);
            }
            else
            {
                AllScreensObj[index].SetActive(false);
            }
        }
    }

    private void OnControlsButtonClicked()
    {
        //SoundManager._soundManager._sounds.SoundToBeUsed(3, SoundManager.Soundtype.other, false, true);
        settingScreens.EnableCanvas(SettingsScreenView.SettingScreen.Controls);
        ToggelObjects(0);
    }
    
    private void OnGraphicsButtonClicked()
    {
        //SoundManager._soundManager._sounds.SoundToBeUsed(3, SoundManager.Soundtype.other, false, true);
        settingScreens.EnableCanvas(SettingsScreenView.SettingScreen.Graphics);
        ToggelObjects(1);
    }
    
    private void OnSoundsButtonClicked()
    {
        //SoundManager._soundManager._sounds.SoundToBeUsed(3, SoundManager.Soundtype.other, false, true);
        settingScreens.EnableCanvas(SettingsScreenView.SettingScreen.Sounds);
        ToggelObjects(2);
    }


    private void RemoveResolutionData()
    {
        resolutionSelectable.ClearAllOptions();
    }

    private void OnOptionsResolutionDataChanged(string value, int index)
    {
        SetResolution(index);
    }

    private void SetResolution(int resolutionIndex)
    {
        Resolution currentResolution = resolutions[resolutionIndex];
        Screen.SetResolution(currentResolution.width, currentResolution.height, FullScreenMode.FullScreenWindow);
        settingData.resolutionIndex = resolutionIndex;
    }

    private void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        settingData.qualityIndex = qualityIndex;
    }

    private void OnQualityDataChanged(string value, int index)
    {
        SetQuality(index);
    }

    private void SetFPS(int index)
    {
        switch (index)
        {
            case 0:
                Application.targetFrameRate = -1;
                break;

            case 1:
                Application.targetFrameRate = 30;
                break;

            case 2:
                Application.targetFrameRate = 60;
                break;

            case 3:
                Application.targetFrameRate = 120;
                break;

            default:
                Application.targetFrameRate = -1;
                break;
        }

        settingData.fpsDataIndex = index;
    }

    private void OnFPSOptionsDataChanged(string value, int index)
    {
        SetFPS(index);
    }

    #endregion

    #region SETTINGS-CONTROLS

    public void OnRebindUIClicked()
    {
        CommonPopup popup = UIController.Instance.GetPopup<CommonPopup>(PopupType.CommonPopup);
        if (popup != null)  
        {
            popup.SetData(CommonPopupType.Message, "Please Enter any Key......");
            UIController.Instance.ShowPopup(PopupType.CommonPopup);
        }
    }

    public void OnRebindUICompeleted()
    {
        UIController.Instance.HidePopup(PopupType.CommonPopup);
    }

    private void SetController(int index)
    {
        GS.Instance.OnGameControllerChanged(index);
        switch (index)
        {
            case 0:
                inputScrollView.SetActive(true);
                resetButton.gameObject.SetActive(true);
                gamepadImage.enabled = false;
                break;

            case 1:
                inputScrollView.SetActive(false);
                resetButton.gameObject.SetActive(false);
                gamepadImage.enabled = true;
                break;
        }
    }

    private void OnControllerDataChanged(string value, int index)
    {
        SetController(index);
    }

    private void AddOptionForController(string data)
    {
        Debug.Log("Inside - Options Controller");
        controlSelectable.AddNewDataInOptions(data);
    }

    private void SwitchControlsOnGameStart(InputType inputType)
    {
        controlSelectable.RefreshOption((int)inputType);
        SetController((int)inputType);
    }

    private void ResetAllKeyboardBinding()
    {
        for (int index = 0; index < inputActions.actionMaps.Count; index++)
        {
            inputActions.actionMaps[index].RemoveAllBindingOverrides();
        }
    }

    public void ResetAllGraphics()
    {
        OnFPSOptionsDataChanged("",0);
        fpsSelectable.ResetData();
        OnQualityDataChanged("", 0);
        qualitySelectable.ResetData();
        OnOptionsResolutionDataChanged("", 0);
        resolutionSelectable.ResetData();
    }
    
    #endregion

    #region SETTINGS-SOUNDS

    private void OnBackgroundMusicChange(float value)
    {
        GS.Instance.backgroundSoundVolume = value;
        SoundManager._soundManager.BGAudioVolumeSetter();
    }

    private void OnSfxMusicChange(float value)
    {
        GS.Instance.sfxSoundVolume = value;
    }

    void RestSound()
    {
        backgroundSoundSlider.value = 0.6f;
        sfxSoundSlider.value = 0.6f;
        GS.Instance.backgroundSoundVolume = 0.6f;
        GS.Instance.sfxSoundVolume = 0.6f;
        SoundManager._soundManager.VolumeSetter();
    }

    #endregion

    #region SAVE-SETTINGS

    public void SaveSettingsData(SettingData settingData)
    {
        SavedDataHandler.Instance.SaveSettingsData(settingData);   
    }

    public void LoadSettingsData()
    {
        settingData = SavedDataHandler.Instance.LoadSettingsData();
        if (settingData != null)
        {
            // Set Resolution Data
            if (!SavedDataHandler.Instance.IsFirstTimeLaunched())
                SetResolution(settingData.resolutionIndex);

            // Set Quality Data
            qualitySelectable.RefreshOption(settingData.qualityIndex);
            SetQuality(settingData.qualityIndex);
            
            // Set FPS Data
            fpsSelectable.RefreshOption(settingData.fpsDataIndex);
            SetFPS(settingData.fpsDataIndex);
        }
    }
    

    #endregion
    
    
    
    
}

[Serializable]
public class SettingsScreenView
{
    public List<SettingView> settingsViews;
    private SettingView currentSettingScreen;
    public enum SettingScreen
    {
        Graphics,
        Controls,
        Sounds,
    }

    public void EnableCanvas(SettingScreen screen, int delay = 500)
    {
        ExecuteAfterDelay(delay, () =>
        {
            SettingView screenView = settingsViews.Find(x => x.screenName == screen);
            if (screenView != null)
            {
                if (currentSettingScreen != null)
                {
                    if (currentSettingScreen == screenView)
                        return;

                    currentSettingScreen.canvas.enabled = false;
                    currentSettingScreen = null;
                }

                currentSettingScreen = screenView;
                currentSettingScreen.canvas.enabled = true;
                SoundManager._soundManager._sounds.SoundToBeUsed(3, SoundManager.Soundtype.other, false, true);
            }
        });
    }

    public void HideAllScreen()
    {
        for (int index = 0; index < settingsViews.Count; index++)
        {
            settingsViews[index].canvas.enabled = false;
        }
        currentSettingScreen = null;
    }
    
    private async void ExecuteAfterDelay(int delay, Action CallbackAction)
    {
        await Task.Delay(delay);
        CallbackAction?.Invoke();
    }
    
    [Serializable]
    public class SettingView
    {
        public Canvas canvas;
        public SettingScreen screenName;
    }
}

[Serializable]
public class SettingData
{
    public int resolutionIndex;
    public int qualityIndex;
    public int fpsDataIndex;
}


