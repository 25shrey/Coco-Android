using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour
{
    #region PUBLIC_VARS

    public Image healthBarImg;
    public float changeTime;
    
    #endregion

    #region PRIVATE_VARS

    #endregion

    #region UNITY_CALLBACKS

    #endregion

    #region PUBLIC_FUNCTIONS

    public void SetBar()
    {
        gameObject.SetActive(true);
        healthBarImg.fillAmount = 1;
        UpdateBar(1);
    }

    public void ResetBar()
    {
        gameObject.SetActive(false);
        healthBarImg.fillAmount = 0;
    }

    public void UpdateBar(float targetValue)
    {
        StartCoroutine(UpdateBarCo(targetValue, changeTime));
    }

    #endregion

    #region PRIVATE_FUNCTIONS

    #endregion

    #region CO-ROUTINES

    private IEnumerator UpdateBarCo(float targetValue,float time)
    {
        float t = 0, startValue = healthBarImg.fillAmount;
        targetValue = 1 - targetValue;
        yield return null;
        while (time>t)
        {
            yield return null;
            t += Time.deltaTime;
            healthBarImg.fillAmount = Mathf.Lerp(startValue,targetValue,t/time);
        }
        healthBarImg.fillAmount = targetValue;
    }
    
    #endregion

    #region EVENT_HANDLERS

    #endregion

    #region UI_CALLBACKS

    #endregion
}
