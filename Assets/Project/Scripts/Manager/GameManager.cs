using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Game.BaseFramework;
using Game.CheckPoints;
using GameCoreFramework;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.VFX;

public class GameManager : PlanetariumApp.Singleton<GameManager>
{
    #region PUBLIC_VARS

    public CameraController CameraController;
    public Player Player;
    public PlayerAnimationController endingSequencePlayer;
    public GameStates currentGameState = GameStates.alive;
    public int score;
    public bool initateCloudScreen;
    public bool endCloudScreen;
    public bool isCinematicCloudScreen = false;
    public bool isDeadZoneTrigger;

    [HideInInspector]
    public InputController input;


    #endregion

    #region PRIVATE_VARS
    
    private bool gameOver;

    #endregion

    #region UNITY_CALLBACKS

    public override void OnAwake()
    {
        input = GS.Instance.input;
    }

    public void Start()
    {
        StartGame();
        initateCloudScreen = true;
    }

    #endregion

    public delegate void ShowScoreDelegate(int value);
    public static event ShowScoreDelegate _showScoreDelegate;

    public delegate void Clouds();
    public static event Clouds _clouds;

    public delegate void CloudsEnd(float sec);
    public static event CloudsEnd _cloudsEnd;
    

    #region PUBLIC_FUNCTIONS

    public void StartGame()
    {
        score = 0;
        _showScoreDelegate(00);
        SetInventoryData();
    }

    public void SetInventoryData()
    {
        InventoryManager.Instance.newArmature = Player.clothDataReference.boneStructure;
        InventoryManager.Instance.newSkinMeshParent = Player.clothDataReference.newSkinMeshOutfitParent;
        InventoryManager.Instance.newMeshParent = Player.clothDataReference.newMeshOutfitParent;
        InventoryManager.Instance.cocoClothReference = Player.clothDataReference;
        //InventoryManager.Instance.cocoReference = Player;
        //Debug.Log("--- it is done");
        InventoryManager.Instance.LoadInventoryData();
        InventoryManager.Instance.newArmature = endingSequencePlayer.clothDataReference.boneStructure;
        InventoryManager.Instance.newSkinMeshParent = endingSequencePlayer.clothDataReference.newSkinMeshOutfitParent;
        InventoryManager.Instance.newMeshParent = endingSequencePlayer.clothDataReference.newMeshOutfitParent;
        InventoryManager.Instance.cocoClothReference = endingSequencePlayer.clothDataReference;
        //InventoryManager.Instance.cocoReference = endingSequencePlayer;
        //Debug.Log("--- it is done");
        InventoryManager.Instance.LoadInventoryData();
    }
    
    public async void RestartGame()
    {
        if (!gameOver)
        {
            SavedDataHandler.Instance.Coin += PlayerStats.Instance.obj.Coins;
            gameOver = true;
            SoundManager._soundManager.MuteAll(true);
            Player.isDead = false;
            score = 0;
            print("Game end.....");
            GameController.Instance.RestartAfterGameOver();
            await Task.Delay(1000);
            _showScoreDelegate(00);
            SoundManager._soundManager._playerSounds.SoundToBeUsed(1, SoundManager.Soundtype.player, false, false);
            await Task.Delay(4000);
            gameOver = false;
        }
    }

    public void AddScore(int value)
    {
        score += value;
        PlayerStats.Instance.obj.Score = score;
        _showScoreDelegate(score);
    }

    public void PlayVFX(VisualEffect vfx)
    {
        vfx.gameObject.SetActive(true);
        vfx.Play();
    }

    public async void StopVFX(VisualEffect vfx, int delay = 2000)
    {
        await Task.Delay(delay);
        vfx.Stop();
        vfx.gameObject.SetActive(false);
    }

    public async void StopVFXImediatly(VisualEffect vfx, int delay = 2000)
    {
        vfx.Stop();
        await Task.Delay(delay);
        vfx.gameObject.SetActive(false);
    }

    public IEnumerator CheckForAnimationCompletion(Action callback, Animator animator, int animationLayer)
    {
        yield return null;
        while (true)
        {
            if (animator.GetCurrentAnimatorStateInfo(animationLayer).normalizedTime > 1 && !animator.IsInTransition(animationLayer))
            {
                callback();
                break;
            }
            else
                yield return null;
        }
    }

    public IEnumerator CheckForTimelineCompletion(PlayableDirector director, Action Callback)
    {

        yield return null;
        while (true)
        {
            if (director.state != PlayState.Playing)
            {
                Callback();
                break;
            }
            else
                yield return null;
        }
    }
    
    public IEnumerator MovePlayerToParticularPoint(Transform player, Vector3 endPoint, float movementSpeed, float rotationSpeed, Action Callback, float minDistance = 1.7f)
    {
        Vector3 direction = (endPoint - player.position).normalized;
        Vector3 lookDir = endPoint - player.position;
        float actualDistance = Vector3.Distance(player.position, endPoint);
        float time = actualDistance / movementSpeed;
        Debug.Log($"<color=red>Time : {time}</color>");
        while(true)
        {
            Player.characterController.Move(direction * Time.deltaTime * movementSpeed);
            lookDir.y = 0;
            Quaternion rotation = Quaternion.LookRotation(lookDir);
            player.rotation = Quaternion.Slerp(player.rotation, rotation, Time.deltaTime * rotationSpeed);
            Player.StartCustomRunAnimation();
            float distance = Vector3.Distance(player.position, endPoint);
            Debug.Log($"Distance : {distance}");
            if (distance < minDistance)
            {
                Player.EndCustomRunAnimation();
                Callback?.Invoke();
                break;
            }
            yield return null;
        }
    }

    public IEnumerator MovePlayer(Transform player, Vector3 endPoint, float movementSpeed, float rotationSpeed, Action Callback, float minDistance = 1.7f)
    {
        var dis = Vector3.Distance(player.position, endPoint);
        var velocity = 1f;
        var time = dis / velocity;

        while(time > 0)
        {
            time = time - (Time.deltaTime * velocity * movementSpeed);

            var dir = (Player.transform.position - endPoint).normalized;

            Player.characterController.Move((-dir) * Time.deltaTime * (movementSpeed));

            Player.StartCustomRunAnimation();

            var lookPos = endPoint - Player.transform.position;

            lookPos.y = 0;

            var rotation = Quaternion.LookRotation(lookPos);

            Player.transform.rotation = Quaternion.Slerp(Player.transform.rotation, rotation, Time.deltaTime * 16f);

            if (Player.transform.position == endPoint || Vector3.Distance(Player.transform.position, endPoint) < 2.3f)
            {
                time = 0;
                break;
            }

            yield return null;
        }

        Player.EndCustomRunAnimation();

        Player.transform.position = endPoint;

        Callback?.Invoke();
    }

    public async Task ChangeSceneAsync(int sceneIndex, Action Callback)
    {
        var task = SceneManager.LoadSceneAsync(sceneIndex);
        while (true)
        {
            if (!task.isDone)
            {
                await Task.Yield();
            }
            else
            {
                Callback?.Invoke();
                break;
            }
        }
    }

    private void Update()
    {
        if (currentGameState == GameStates.PlayerRespawn && initateCloudScreen)
        {
            ShowClouds();
            initateCloudScreen = false;
            CameraController.SetCameraPlaneAfterDeath();
            Player.playerPowerUps.DeactivateAllPower();
            GS.Instance.input.DisableInput();
        }
        else if (currentGameState == GameStates.alive && endCloudScreen && !isCinematicCloudScreen)
        {
            ShowCloudsEnd();
            endCloudScreen = false;
        }
    }

    public void ShowClouds()
    {
        _clouds();
    }

    public void ShowCloudsEnd()
    {
        _cloudsEnd(0.2f);
    }

    public void OnCinematicsShowCloud()
    {
        isCinematicCloudScreen = true;
        _clouds();
    }

    public void OnCinematicsShowCloudEnd()
    {
        isCinematicCloudScreen = false;
        _cloudsEnd(0.2f);
    }

    


    #endregion

    #region PRIVATE_FUNCTIONS

    #endregion

    #region CO-ROUTINES

    #endregion

    #region EVENT_HANDLERS

    #endregion

    #region UI_CALLBACKS

    #endregion
}


public enum GameStates
{
    PlayerRespawn,
    alive,
    GameOver,
    paused,
    level_complete,
    level_intro,
    level_end_animation,
    PlayerPaused
}

