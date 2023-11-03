using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HighlightedButton : MonoBehaviour
{
    #region PUBLIC_VARS

    public Button Button;
    public BaseInventoryUiItem item;
    public BaseScroll scrollReference;

    #endregion

    #region PRIVATE_VARS

    #endregion

    #region UNITY_CALLBACKS



    public void SelectedButton()
    {
        print(item.pageIndex);
    }

    #endregion

    #region PUBLIC_FUNCTIONS

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
