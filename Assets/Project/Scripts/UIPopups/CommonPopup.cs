using System;
using System.Collections;
using System.Collections.Generic;
using Game.BaseFramework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CommonPopup : UIPopupView
{
   [SerializeField] private TextMeshProUGUI titleText;
   [SerializeField] private TextMeshProUGUI descriptionText;
   [SerializeField] private Button bgButton;
   [SerializeField] private Button closeButton;
   [SerializeField] private Button yesButton;
   [SerializeField] private Button noButton;
   [SerializeField] private Button exitButton;

   private Action onYesClicked;
   private Action onNoClicked;

   public override void OnScreenShowCalled()
   {
      base.OnScreenShowCalled();
      Debug.Log("Common Popup - OnScreenShowCalled!");
      bgButton.onClick.AddListener(OnClickBgButton);
      closeButton.onClick.AddListener(OnClickCloseButton);
   }

   public override void OnScreenHideCalled()
   {
      base.OnScreenHideCalled();
      bgButton.onClick.RemoveListener(OnClickBgButton);
      closeButton.onClick.RemoveListener(OnClickCloseButton);
      Debug.Log("Common Popup - OnScreenHideCalled!");
      titleText.text = string.Empty;
      descriptionText.text = string.Empty;
      
      if (onYesClicked != null)
      {
         onYesClicked = null;
      }
      
      if (onNoClicked != null)
      {
         onNoClicked = null;
      }
      DisableListners();
   }

   public void SetData(CommonPopupType title, string description, bool isYesNoRequired = false, Action yesCallback = null, Action noCallback = null, float time = 5f)
   {
      titleText.text = title.ToString();
      descriptionText.text = description;
      
      yesButton.gameObject.SetActive(isYesNoRequired);  
      noButton.gameObject.SetActive(isYesNoRequired);
      if (isYesNoRequired)
      {
         EnableListners();
         onYesClicked = yesCallback;
         onNoClicked = noCallback;
      }
      else
      {
         Invoke("OnClickCloseButton", time);
      }
        yesButton.Select();
   }

   public void EnableListners()
   {
      yesButton.onClick.AddListener(OnClickYesButton);
      noButton.onClick.AddListener(OnClickNoButton);
   }

   public void DisableListners()
   {
      yesButton.onClick.RemoveListener(OnClickYesButton);
      noButton.onClick.RemoveListener(OnClickNoButton);
   }

   private void OnClickYesButton()
   {
      onYesClicked?.Invoke();
   }

   private void OnClickNoButton()
   {
      onNoClicked?.Invoke();
        exitButton.Select();
   }

   private void OnClickBgButton()
   {
      UIController.Instance.HidePopup(PopupType.CommonPopup);
   }

   private void OnClickCloseButton()
   {
      UIController.Instance.HidePopup(PopupType.CommonPopup);
   }

   
}

public enum CommonPopupType
{
   Error,
   Warning,
   Success,
   Message
}
