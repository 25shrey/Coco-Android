using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Game.BaseFramework
{
    public enum CanvasState
    {
        Active,
        Inactive
    }

    public enum Direction
    {
        Forward,
        Reverse
    }

    public abstract class UIBase : MonoBehaviour
    {
        public static string BACKGROUND = "Background";
        public static string PARENT = "Parent";
        [HideInInspector]
        public bool BackKeyActive = false;
        public delegate void ScreenStateChanged(CanvasState _state);
        public event ScreenStateChanged OnScreenStateChanged;

        public Canvas BaseCanvas { get; }
        public GraphicRaycaster BaseRaycaster { get; }
        [HideInInspector]public bool isLoading = false;
        public CanvasState State
        {
            get
            {
                return _state;
            }
            set
            {
                _state = value;

                NotifyStateChanged(_state);
            }
        }
        CanvasState _state;
        Canvas _canvas;
        GraphicRaycaster _raycaster;
        private void OnEnable() { Enable(); }
        private void OnDisable() { Disable(); }
        private void Awake()
        {
            _canvas = GetComponent<Canvas>();
            _raycaster = GetComponent<GraphicRaycaster>();

            ResetCanvas();
            OnAwake();
        }

        public virtual void OnAwake() { }
        public virtual void Enable() { }
        public virtual void Disable() { }

        void ToggleCanvasState(bool hasToEnable)
        {

            if (hasToEnable)
            {
                State = CanvasState.Active;
            }
            else
            {
                State = CanvasState.Inactive;
            }
        }
        public void ToggleCanvas(bool value)
        {
            _canvas.enabled = value;
        }
        public void ToggleRaycast(bool value)
        {
            _raycaster.enabled = value;
        }
        void ResetCanvas()
        {
            _raycaster.enabled = false;
            _canvas.enabled = false;
            _state = CanvasState.Inactive;
        }

        void NotifyStateChanged(CanvasState state)
        {
            OnScreenStateChanged?.Invoke(state);
        }

        public virtual void OnBack() 
        {
            Debug.Log("OnBack Called!");
            if (GS.Instance.input.ReturnState())
            {
                SoundManager._soundManager._sounds.SoundToBeUsed(3, SoundManager.Soundtype.other, false, true);
            }
        }
        
        public virtual void Show()
        {
            if (isLoading) return;
            isLoading = true;
            OnScreenShowCalled();
            ToggleCanvasState(true);
        }
        public virtual void Hide()
        {
            if (isLoading) return;
            isLoading = true;
            OnScreenHideCalled();
            ToggleCanvasState(false);
        }

        public virtual void OnScreenShowCalled() { }

        public virtual void OnScreenHideCalled() { }

        public virtual void OnScreenShowAnimationCompleted() { }

        public virtual void OnScreenHideAnimationCompleted() { }

    }

    public class UIView : UIBase
    {
        [HideInInspector]
        public Image Background;
        [HideInInspector]
        public RectTransform Parent;

        UIAnimator _uiAnimator;
        public override void Enable()
        {
            base.Enable();
            Events.OnScreenChanged += ToggleRaycaster;
        }
        public override void Disable()
        {
            base.Disable();
            Events.OnScreenChanged -= ToggleRaycaster;
        }
        public override void OnAwake()
        {
            base.OnAwake();
            Background = transform.Find(BACKGROUND).GetComponent<Image>();
            Parent = transform.Find(PARENT).GetComponent<RectTransform>();
            _uiAnimator = GetComponent<UIAnimator>();
        }
        
        public override void OnScreenShowCalled()
        {
            base.OnScreenShowCalled();
            ToggleCanvas(true);
        }

        public override void OnScreenShowAnimationCompleted()
        {
            base.OnScreenShowAnimationCompleted();
            isLoading = false;
            ToggleRaycaster(true);
        }
        
        public override void OnScreenHideCalled()
        {
            base.OnScreenHideCalled();
            ToggleRaycaster(false);

            if (_uiAnimator == null)
            {
                OnScreenHideAnimationCompleted();
            }
        }

        public override void OnScreenHideAnimationCompleted()
        {
            base.OnScreenHideAnimationCompleted();
            isLoading = false;
            ToggleCanvas(false);
        }

        /*IEnumerator BackKeyUpdateRoutine()
        {
            //yield return new WaitForSeconds(.5f);
          //  Debug.Log("==>> Back active : " + transform.name);
            while (BackKeyActive)
            {
                if (Keyboard.current.escapeKey.wasPressedThisFrame || (Gamepad.current != null && Gamepad.current.startButton.wasPressedThisFrame))
                {
                    BackKeyActive = false;
                    OnBack();
                    yield return new WaitForSeconds(1f);
                }
                yield return null;
            }
          //  Debug.Log("==>> Back Deactive : " + transform.name);
        }*/

        public void ToggleRaycaster(bool isActive)
        {
            ToggleRaycast(isActive);
            BackKeyActive = isActive;
            if (isActive)
            {
                MenuInput.Instance.onBack += OnBack;
            }
            else
            {
                MenuInput.Instance.onBack -= OnBack;
            }
        }
    }
}