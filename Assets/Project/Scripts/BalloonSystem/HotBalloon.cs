using GameCoreFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;

public class HotBalloon : MonoBehaviour
{
    public Animator anim;
    public InputController Input;
    public CameraController cam;
    public List<MeshCollider> colliders = new List<MeshCollider>();
    public PlayableDirector dir;

    private PlayerInput playerInput;

    private InputActionReference fastForwardAnimation;
    private InputActionReference levelSkipAnimation;
    private void OnEnable()
    {
        levelSkipAnimation.action.started += FastForwardBallonAnimation;
        levelSkipAnimation.action.canceled += SkipCurrentLevel;
    }

    private void Awake()
    {
        Input = GS.Instance.input;
        playerInput = GS.Instance.playerInput;
        fastForwardAnimation = GS.Instance.fastForwardAnimationInput;
        levelSkipAnimation = GS.Instance.levelSkipInput;
        playerInput.currentActionMap.Enable();
        dir = GetComponent<PlayableDirector>();

        for (int i = 0; i < transform.childCount; i++)
        {
            if (i > 5 && i < 12)
            {
                colliders.Add(transform.GetChild(i).GetComponent<MeshCollider>());
            }
        }
    }

    private void OnDisable()
    {
        fastForwardAnimation.action.started -= FastForwardBallonAnimation;
        levelSkipAnimation.action.canceled -= SkipCurrentLevel;
    }

    void Start()
    {
        if (GameManager.instance.currentGameState == GameStates.level_intro)
        {
            //Input.enabled = false;
            playerInput.currentActionMap.Disable();
            GameManager.instance.Player.GetComponent<Animator>().enabled = false;
        }

        dir.Play();

    }

    public void InitiateDoorOpen()
    {
        foreach (var item in colliders)
        {
            item.enabled = false;
        }
        GameManager.instance.Player.GetComponent<Animator>().enabled = true;

        //Input.enabled = true;
        playerInput.currentActionMap.Enable();

        Debug.Log("Player out..");
        GameManager.instance.Player.transform.SetParent(null);

        cam.IntroEnded();
    }

    public void PlayerDrop()
    {
        Debug.Log("Player Drop");
        cam.FadeBGMusic();
        GameManager.instance.Player.GroundZoneOffset = 8;
    }
    
    public void DisableHotBalloon()
    {
        int count = transform.childCount;

        for (int i = 0; i < count; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    void FastForwardBallonAnimation(InputAction.CallbackContext obj)
    {
        if (!transform.GetComponent<Animator>().enabled && !playerInput.currentActionMap.enabled &&
            (GameManager.instance.currentGameState == GameStates.alive || GameManager.instance.currentGameState == GameStates.level_end_animation))
        {
            LevelComplete.SkipEndAnimation();
        }
    }

    //for skipping a level
    void SkipCurrentLevel(InputAction.CallbackContext obj)
    {
        GameManager.instance.currentGameState = GameStates.level_complete;
        GameObject.FindObjectOfType<LevelMapUI>().SkipCompleteLevel();
    }

}