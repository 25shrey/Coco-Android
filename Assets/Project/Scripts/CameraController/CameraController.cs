using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.VFX;
using static UnityEngine.GraphicsBuffer;
using static Cinemachine.CinemachineTargetGroup;

namespace GameCoreFramework
{
    [RequireComponent(typeof(Camera))]
    public class CameraController : MonoBehaviour
    {
        public float RotetionSpeed;
        public GameObject rotetionTarget;
        public static CameraController Instance;

        public CameraMode Mode;

        public Transform Target;
        public Transform cocoEndAnimation;

        public float followSpeed;
        public Vector3 playerLockAtOffset;
        public Camera gameCamera;

        private Vector3 Finalposition;
        public Vector3 offset;
        public Vector3 reverseoffset;
        float desiredAngle, distance;
        Quaternion rotation;
        int count = Enum.GetNames(typeof(CameraMode)).Length;

        Coroutine camRoutine;
        InputController _input;

        [SerializeField] Vector3 initialPosition;
        [SerializeField] float _Speed;

        public bool introAnimationEnded;

        public Transform cameraEndPoint;

        public float returnSpeed;

        public bool isFollowing;

        private float shakeMagnitude;
        private float shakeTimer;
        private float cameraMoveOnGrabSpeed = 5f;

        private Coroutine routine;

        public float nearClippingPlane = 4f;

        public float cameraBlockOffset=8;

        public void Awake()
        {
            isFollowing = true;
            Instance = this;
            _input = GS.Instance.input;
            cameraBlockLayerMask=LayerMask.GetMask("Walkable");
        }

        void OnEnable()
        {
            SetupVariables();
        }

        private void SetupVariables()
        {
            RotetionSpeed = GS.Instance.CamaraRotetionSpeed;
            followSpeed = GS.Instance.CameraFollowSpeed;
            returnSpeed = 400;
            playerLockAtOffset = new Vector3(0, 2.5f, 0);
            gameCamera = gameObject.GetComponent<Camera>();
            gameCamera.nearClipPlane = nearClippingPlane;
            Target = GameManager.instance.Player.transform;
            offset = Target.transform.position - transform.position;
            reverseoffset = transform.position - Target.transform.position;
            SwitchMode(2);
            InputController.OnCameraSwitch += OnCameraSwitchPerformed;
        }

        void SetOffSet()
        {
            offset = new Vector3(-13, -8.5f, 0.8f);
            reverseoffset = offset * -1;
            distance = GetDistance();
        }

        public float GetDistance()
        {
            Vector3 v1 = transform.position, v2 = Target.transform.position;
            return Vector2.Distance(new Vector2(v1.x, v1.z), new Vector2(v2.x, v2.z));
        }

        void LateUpdate()
        {
            //Camera Shake 
            //Check for Camera Shake timer
            if (shakeTimer > 0)
            {
                // Generate random Shake offset
                float xOffset = UnityEngine.Random.Range(-1f, 1f) * shakeMagnitude;
                float yOffset = UnityEngine.Random.Range(-1f, 1f) * shakeMagnitude;

                // Apply Shake offset
                transform.localPosition = new Vector3(transform.localPosition.x + xOffset,
                    transform.localPosition.y + yOffset, transform.localPosition.z);

                // Decrease Shake Timer
                shakeTimer -= Time.deltaTime;
            }

            // Camera Follow
            if (GameManager.instance.currentGameState == GameStates.alive && isFollowing)
            {
                Finalposition = transform.position;
                switch (Mode)
                {
                    case CameraMode.LOOKATCAMERA:
                        Finalposition = transform.position;
                        break;

                    case CameraMode.DUNGEONFOLLOW:
                        Vector3 desiredPosition = Target.transform.position + reverseoffset;
                        Finalposition = desiredPosition;
                        break;

                    case CameraMode.MOUSEAIM:

                        float horizontal = InputController.GetRotationXAxis() * 2;
                        Target.transform.Rotate(0, horizontal, 0);
                        desiredAngle = Target.transform.eulerAngles.y;
                        rotation = Quaternion.Euler(0, desiredAngle, 0);
                        Finalposition = Target.transform.position - (rotation * offset);
                        break;

                    case CameraMode.FOLLOWTARGET:
                        desiredAngle = Target.transform.eulerAngles.y;
                        rotation = Quaternion.Euler(0, desiredAngle, 0);
                        Finalposition = Target.transform.position - (rotation * offset);
                        break;

                    case CameraMode.Directionly:
                        float dis = GetDistance() - distance;
                        Vector2 v;
                        if (Mathf.Abs(dis) > 0.2f)
                        {
                            v = (new Vector2(Target.transform.position.x, Target.transform.position.z) -
                                 new Vector2(transform.position.x, transform.position.z)).normalized;
                            v *= dis;
                            Vector3 DirectionlyPosition = new Vector3(transform.position.x + v.x,
                                Target.transform.position.y + reverseoffset.y, transform.position.z + v.y);
                            Finalposition = DirectionlyPosition;
                            Debug.Log("dis : " + dis);
                        }

                        break;
                }

                float x, y, z;
                if (Target.parent)
                {
                    followSpeed += followSpeed * 0.1f;
                    Vector3 movingDirection = (Finalposition - transform.position).normalized;
                    if (!Physics.Raycast(transform.position, movingDirection, cameraBlockOffset, cameraBlockLayerMask))
                    {
                        if (followSpeed > 150)
                        {
                            transform.position = Vector3.Lerp(transform.position, Finalposition, 1.2f);
                        }
                        else
                        {
                            transform.position = Vector3.Lerp(transform.position, Finalposition,
                                followSpeed * Time.deltaTime);
                        }
                    }
                    Debug.DrawRay(transform.position, movingDirection * cameraBlockOffset, Color.red);
                }
                else
                {
                    followSpeed = GS.Instance.CameraFollowSpeed;
                    x = Mathf.Lerp(transform.position.x, Finalposition.x, GS.Instance.xFollowSpeed * Time.deltaTime);
                    y = transform.position.y;
                    if (Mathf.Abs(transform.position.y - Finalposition.y) > GS.Instance.YOfset)
                    {
                        y = Mathf.Lerp(transform.position.y, Finalposition.y,
                            GS.Instance.yFollowSpeed * Time.deltaTime);
                    }
                    else if (GameManager.instance.Player.isMoving())
                    {
                        y = Mathf.Lerp(transform.position.y, Finalposition.y,
                            GS.Instance.yFollowSpeed * Time.deltaTime * GS.Instance.RunMul);
                    }
                    else
                    {
                        y = Mathf.Lerp(transform.position.y, Finalposition.y,
                            GS.Instance.yFollowSpeed * Time.deltaTime * GS.Instance.StayMul);
                    }

                    z = transform.position.z;
                    if (Mathf.Abs(transform.position.z - Finalposition.z) > GS.Instance.ZOfset)
                    {
                        z = Mathf.Lerp(transform.position.z, Finalposition.z,
                            GS.Instance.zFollowSpeed * Time.deltaTime);
                    }
                    else if (GameManager.instance.Player.isMoving())
                    {
                        z = Mathf.Lerp(transform.position.z, Finalposition.z,
                            GS.Instance.zFollowSpeed * Time.deltaTime * GS.Instance.RunMul);
                    }
                    else
                    {
                        z = Mathf.Lerp(transform.position.z, Finalposition.z,
                            GS.Instance.zFollowSpeed * Time.deltaTime * GS.Instance.StayMul);
                    }

                    Vector3 movingDirection = (new Vector3(x, y, z) - transform.position).normalized;

                    //if (!Physics.Raycast(transform.position, movingDirection, cameraBlockOffset, cameraBlockLayerMask))
                    //{
                    //    transform.position = new Vector3(x, y, z);
                    //}
                    if (Physics.Raycast(transform.position, movingDirection, cameraBlockOffset, cameraBlockLayerMask))
                    {
                        Debug.DrawRay(transform.position, movingDirection * cameraBlockOffset, Color.red);
                        if (movingDirection.y < 0)
                        {
                            return;
                        }
                    }
                    transform.position = new Vector3(x, y, z);
                    Debug.DrawRay(transform.position, movingDirection * cameraBlockOffset, Color.red);
                }

                Vector3 targetRotetion = GetTargetRotation();
                x = transform.eulerAngles.x;
                if (Mathf.Abs(transform.eulerAngles.x - targetRotetion.x) > GS.Instance.XRotetionOfset)
                {
                    x = Mathf.Lerp(transform.eulerAngles.x, targetRotetion.x,
                        GS.Instance.xRotetionSpeed * Mathf.Abs(transform.eulerAngles.x - targetRotetion.x) *
                        Time.deltaTime / 10);
                }
                else if (GameManager.instance.Player.isMoving())
                {
                    x = Mathf.Lerp(transform.eulerAngles.x, targetRotetion.x,
                        GS.Instance.xRotetionSpeed * GS.Instance.RunMul *
                        Mathf.Abs(transform.eulerAngles.x - targetRotetion.x) * Time.deltaTime / 10);
                }
                else
                {
                    x = Mathf.Lerp(transform.eulerAngles.x, targetRotetion.x,
                        GS.Instance.xRotetionSpeed * GS.Instance.StayMul *
                        Mathf.Abs(transform.eulerAngles.x - targetRotetion.x) * Time.deltaTime / 10);
                }

                y = transform.eulerAngles.y;
                if (Mathf.Abs(transform.eulerAngles.y - targetRotetion.y) > GS.Instance.YRotetionOfset)
                {
                    y = Mathf.Lerp(transform.eulerAngles.y, targetRotetion.y,
                        GS.Instance.yRotetionSpeed * Time.deltaTime);
                }
                else if (GameManager.instance.Player.isMoving())
                {
                    y = Mathf.Lerp(transform.eulerAngles.y, targetRotetion.y,
                        GS.Instance.yRotetionSpeed * Time.deltaTime * GS.Instance.RunMul);
                }
                else
                {
                    y = Mathf.Lerp(transform.eulerAngles.y, targetRotetion.y,
                        GS.Instance.yRotetionSpeed * Time.deltaTime * GS.Instance.StayMul);
                }

                if (!GameManager.instance.Player.isGrabbed)
                {
                    z = Mathf.Lerp(transform.eulerAngles.z, targetRotetion.z,
                        GS.Instance.zRotetionSpeed * Time.deltaTime);
                    transform.eulerAngles = new Vector3(x, y, z);
                }
                else if (GameManager.instance.Player.isGrabbed && GameController.Instance.CurrentPlayingLevel == 15)
                {
                    PlayerGrabCameraSetter();
                }
            }
            else if (GameManager.instance.currentGameState == GameStates.level_intro)
            {
                if (introAnimationEnded)
                {
                    SetCameraPosition(Target, initialPosition, Target, _Speed);

                    Target.transform
                        .DOScale(new Vector3(GS.Instance.cocoHeight, GS.Instance.cocoHeight, GS.Instance.cocoHeight),
                            0.5f)
                        .SetEase(Ease.OutExpo);

                    if (Target.GetComponent<Player>()._reachedToInitialPoint)
                    {
                        SetOffSet();

                        GameManager.instance.currentGameState = GameStates.alive;
                    }
                }
                else
                {
                    Vector3 Finalposition = transform.position;

                    Vector3 desiredPosition = Target.transform.position + reverseoffset;

                    Finalposition = desiredPosition;
                    Vector3 movingDirection = (Finalposition - transform.position).normalized;
                    if (!Physics.Raycast(transform.position, movingDirection, cameraBlockOffset, cameraBlockLayerMask))
                    {
                        transform.position = Vector3.Lerp(transform.position, Finalposition,
                            (followSpeed + 2) * Time.deltaTime);
                    }
                    Debug.DrawRay(transform.position, movingDirection * cameraBlockOffset, Color.red);
                }
            }
            else if (GameManager.instance.currentGameState == GameStates.level_end_animation)
            {
                SetCameraPosition(cameraEndPoint, cameraEndPoint.position, cocoEndAnimation, 0.05f);
            }
        }

        public LayerMask cameraBlockLayerMask;

        public void SetCameraPosition(Transform Target, Vector3 position, Transform track, float move)
        {
            Vector3 lTargetDir = Target.position - transform.position;

            if (lTargetDir != Vector3.zero)
            {
                if (GameManager.instance.currentGameState == GameStates.level_end_animation)
                {
                    transform.rotation = Quaternion.RotateTowards(transform.rotation,
                        Quaternion.LookRotation(lTargetDir), Time.time * _Speed);

                    transform.LookAt(track.transform.position + playerLockAtOffset, track.transform.up);
                }
                else
                {
                    transform.rotation = Quaternion.RotateTowards(transform.rotation,
                        Quaternion.LookRotation(lTargetDir + playerLockAtOffset), Time.time * (_Speed / 2));
                }
            }

            Vector3 movingDirection = (position - transform.position).normalized;
            if (!Physics.Raycast(transform.position, movingDirection, cameraBlockOffset, cameraBlockLayerMask))
            {
                transform.position = Vector3.MoveTowards(transform.position,
                    Vector3.Lerp(transform.position, position, _Speed), move);
            }
            Debug.DrawRay(transform.position, movingDirection * cameraBlockOffset, Color.red);
        }

        public void IntroEnded()
        {
            introAnimationEnded = true;
            StartCoroutine(FOV(3));
            Target.GetComponent<Player>().CustomJumpAnimation();
        }

        IEnumerator FOV(float time)
        {
            while (time > 0)
            {
                gameCamera.fieldOfView = Mathf.Lerp(gameCamera.fieldOfView, 60, 3f * Time.deltaTime);

                time -= Time.deltaTime;

                yield return null;
            }

            gameCamera.fieldOfView = 60;
        }

        public void SwitchMode()
        {
            // Increament the mode and mod it so that it does not go out of enum
            Mode = (CameraMode)(((int)++Mode) % count);
        }

        public void SwitchMode(int ModeIndex)
        {
            Mode = (CameraMode)ModeIndex;
        }

        private void OnCameraSwitchPerformed()
        {
            SwitchMode();
        }

        public void CameraBackToSpawnPoint()
        {
            _input.DisableInput();
            Target.transform.localScale = new Vector3(0, 0, 0);
            if (GameManager.instance.Player.flashLight != null)
            {
                GameManager.instance.Player.flashLight.enabled = false;
            }

            if (camRoutine == null)
            {
                camRoutine = StartCoroutine(SwitchAfterThePlayerPositionSet());
            }
        }

        IEnumerator SetPos()
        {
            yield return new WaitForSeconds(1f);

            SetOffSet();

            GameManager.instance.currentGameState = GameStates.alive;
        }

        public IEnumerator SwitchAfterThePlayerPositionSet()
        {
            yield return new WaitForSeconds(0.5f);

            float dis = Vector3.Distance(Target.transform.position + reverseoffset, transform.position);
            Vector3 desiredPosition = Target.transform.position + reverseoffset, startPos = transform.position;
            Finalposition = desiredPosition;
            float time = dis / returnSpeed, totalTime = dis / returnSpeed;
            while (time > 0)
            {
                yield return null;
                time -= Time.deltaTime;
                transform.position = Vector3.Lerp(startPos, Finalposition, 1 - time / totalTime);
                transform.LookAt(Target.transform.position + playerLockAtOffset, Target.transform.up);
            }

            GameManager.instance.currentGameState = GameStates.alive;

            Target.transform
                .DOScale(new Vector3(GS.Instance.cocoHeight, GS.Instance.cocoHeight, GS.Instance.cocoHeight), 1.5f)
                .SetEase(Ease.OutExpo);

            if (GameManager.instance.Player.flashLight != null)
            {
                GameManager.instance.Player.flashLight.enabled = true;
            }

            yield return new WaitForSeconds(0.3f);

            Target.GetComponent<Player>().isRespawned = true;
            Target.GetComponent<CharacterController>().enabled = true;

            camRoutine = null;

            yield return new WaitForSeconds(0.8f);

            _input.EnableInput();
        }

        public IEnumerator MoveCameraWithBalloon(Vector3 target, float speed)
        {
            Vector3 currentPos = transform.position;
            float distance = Vector3.Distance(currentPos, target);
            float time = distance / speed;
            float totalTime = time;
            while (time > 0)
            {
                time -= Time.deltaTime;
                transform.position = Vector3.Lerp(currentPos, target, (totalTime - time) / totalTime);

                yield return null;
            }

            transform.position = target;
        }

        public Vector3 GetTargetRotation()
        {
            rotetionTarget.transform.LookAt(Target.transform.position + playerLockAtOffset, Target.transform.up);
            float x = ParseAngel(rotetionTarget.transform.eulerAngles.x),
                y = ParseAngel(rotetionTarget.transform.eulerAngles.y),
                z = ParseAngel(rotetionTarget.transform.eulerAngles.z);

            Vector3 output = new Vector3(GetOutPutAngle(transform.eulerAngles.x, x),
                GetOutPutAngle(transform.eulerAngles.y, y), GetOutPutAngle(transform.eulerAngles.z, z));
            return output;
        }

        void PlayerGrabCameraSetter()
        {
            Vector3 newDirection = Target.position - transform.position;

            Quaternion newRotation = Quaternion.LookRotation(newDirection);

            transform.rotation =
                Quaternion.Lerp(transform.rotation, newRotation, cameraMoveOnGrabSpeed * Time.deltaTime);
        }

        public float GetOutPutAngle(float current, float target)
        {
            while (Mathf.Abs(current - target) > 180)
            {
                if (current < target)
                {
                    target -= 360;
                }
                else
                {
                    target += 360;
                }
            }

            return target;
        }

        public float ParseAngel(float angle)
        {
            if (angle < 360)
            {
                while (angle < 0)
                {
                    angle += 360;
                }
            }
            else if (angle > 360)
            {
                while (angle > 360)
                {
                    angle -= 360;
                }
            }

            return angle;
        }

        public void ShakeCamera(float cameraShakeMagnitude = 0.05f, float cameraShakeTime = 0.2f)
        {
            shakeMagnitude = cameraShakeMagnitude;
            shakeTimer = cameraShakeTime;
        }

        IEnumerator StartBGSounds()
        {
            yield return new WaitForSeconds(1.8f);

            if (GameController.Instance.CurrentPlayingLevel < 11)
            {
                SoundManager._soundManager.day.Stop();
                SoundManager._soundManager.night.Stop();
                SoundManager._soundManager.day.clip = SoundManager._soundManager.others[11];
                SoundManager._soundManager.day.loop = true;
                SoundManager._soundManager.day.volume = GS.Instance.backgroundSoundVolume;
                SoundManager._soundManager.day.Play();
            }
            else
            {
                SoundManager._soundManager.night.Stop();
                SoundManager._soundManager.day.Stop();
                SoundManager._soundManager.night.clip = SoundManager._soundManager.others[11];
                SoundManager._soundManager.night.loop = true;
                SoundManager._soundManager.night.volume = GS.Instance.backgroundSoundVolume;
                SoundManager._soundManager.night.Play();
            }
        }

        public void FadeBGMusic()
        {
            if (SoundManager._soundManager.night.isPlaying)
            {
                SoundManager._soundManager.night.DOFade(0, 1.7f);
            }
            else
            {
                SoundManager._soundManager.day.DOFade(0, 1.7f);
            }

            StartLevelSound();
        }

        void StartLevelSound()
        {
            if (routine == null)
            {
                routine = StartCoroutine(StartBGSounds());
            }
            else
            {
                StopCoroutine(routine);
                routine = StartCoroutine(StartBGSounds());
            }
        }

        public void SetCameraPlaneAfterDeath()
        {
            if (gameCamera.nearClipPlane == 0.1f)
            {
                gameCamera.nearClipPlane = nearClippingPlane;
            }
        }
    }

    public enum CameraMode
    {
        FOLLOWTARGET,
        MOUSEAIM,
        DUNGEONFOLLOW,
        LOOKATCAMERA,
        Directionly
    }
}