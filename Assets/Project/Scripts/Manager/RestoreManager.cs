using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Game.BaseFramework;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class RestoreManager : Singleton<RestoreManager>
{
    #region PUBLIC_VARS

    public List<BaseEnemy> deadEnemy;

    #endregion

    #region PRIVATE_VARS

    #endregion

    #region UNITY_CALLBACKS

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(transform.parent);
        deadEnemy = new List<BaseEnemy>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #endregion

    #region PUBLIC_FUNCTIONS

    public void AddDeadEnemy(BaseEnemy enemy)
    {
        deadEnemy.Add(enemy);
    }

    public void ClearData()
    {
        deadEnemy.Clear();
    }

    public async void RestoreData()
    {
        await Task.Delay(1600);
        for (int i = 0; i <deadEnemy.Count; i++)
        {
            if (deadEnemy[i].isDead && !deadEnemy[i].isRespawnBlock)
            {
                deadEnemy[i].Restore();
            }
        }
        deadEnemy.Clear();
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
