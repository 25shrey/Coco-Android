using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.CheckPoints
{
    public class CheckPoint : MonoBehaviour
    {
        //fields
        [SerializeField]
        List<GameObject> points = new List<GameObject>();
        [SerializeField]
        private Vector3 PlayerLastSpawnPosition;
        [SerializeField]
        private Transform playerData;
        internal int currentCheckPointIndex;
        private int _health;
        private int _coins;
        private Quaternion _currentRotation;


        #region properties to get the data
        //USE THE PROPERTY TO GET THE PLAYER POSITION AFTER THE PLAYER DEATH
        // FOR RESPAWNING THE PLAYER
        public Vector3 playerPosition
        {
            get { return PlayerLastSpawnPosition; }
        }

        public int checkPointIndex
        {
            get { return currentCheckPointIndex; }
        }

        public int playerHealth
        {
            get { return _health; }
        }

        public int playerCoins
        {
            get { return _coins; }
        }

        public Transform playerDataDetails
        {
            get { return playerData; }
        }

        public Quaternion currentRotation
        {
            get { return _currentRotation; }
            internal set { _currentRotation = value; }
        }

        #endregion



        #region unity methods

        private void OnEnable()
        {
            CheckPointTrigger.currentCheckPoint += PlayerPosition;
        }

        void Start()
        {
            GetAllChild(transform.childCount);
        }

        private void OnDisable()
        {
            CheckPointTrigger.currentCheckPoint -= PlayerPosition;
        }

        private void Update()
        {
            if(GameManager.instance.currentGameState == GameStates.GameOver)
            {
                foreach (var item in points)
                {
                    item.SetActive(true);
                }
                currentCheckPointIndex = 0;  
            }
        }

        #endregion




        #region check points data logic

        void GetAllChild(int count)
        {
            for (int i = 0; i < count; i++)
            {
                var obj = transform.GetChild(i);
                obj.GetComponent<CheckPointTrigger>().index = i+1;
                points.Add(obj.gameObject);
            }
        }

        private void PlayerPosition(Vector3 position, int health, int coins, Transform data)
        {
            PlayerLastSpawnPosition = position;
            _health = health;
            _coins = coins;
            playerData = data;
        }

        #endregion
    }
}
