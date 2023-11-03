using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopBalloonList : MonoBehaviour
{
    public RectTransform rootTransform;
    public InventoryBalloonUiItem itemPrefab;

    public List<InventoryBalloonUiItem> balloonUiItemsButtonData;

    [SerializeField] private CustomiseUI ui;

    public void PopulateList(Balloon_DataSO data)
    {
        foreach (Transform t in rootTransform)
        {
            Destroy(t.gameObject);
        }
        
        InventoryManager.Instance.currentBalloonUiItemList.Clear();
        balloonUiItemsButtonData.Clear();   

        for (int i = 0; i < data.inventoryData.Capacity; i++)
        {
            InventoryBalloonUiItem balloonItem = Instantiate(itemPrefab, rootTransform);
            balloonItem.SetUiItemData(data.inventoryData[i]);
            balloonUiItemsButtonData.Add(balloonItem);
            balloonItem.pageIndex = 1;
        }

        SetNavigation();
    }


    public void SetNavigation()
    {
        for (int index = 0; index < balloonUiItemsButtonData.Count; index++)
        {
            Navigation nav = new Navigation();
            nav.mode = Navigation.Mode.Explicit;
            if (index == 0)
            {
                nav.selectOnRight = balloonUiItemsButtonData[index + 1].selectButton;
                nav.selectOnLeft = ui.AllButtons[2];
            }
            else if (index == balloonUiItemsButtonData.Count - 1)
            {
                nav.selectOnLeft = balloonUiItemsButtonData[index - 1].selectButton;
            }
            else
            {
                nav.selectOnRight = balloonUiItemsButtonData[index + 1].selectButton;
                nav.selectOnLeft = balloonUiItemsButtonData[index - 1].selectButton;
            }
            balloonUiItemsButtonData[index].selectButton.navigation = nav;
        }


        for (int i = 0; i <= ui.AllButtons.Count - 1; i++)
        {
            if (i == 0)
            {
                Navigation panelNav = new Navigation();
                panelNav.mode = Navigation.Mode.Explicit;
                panelNav.selectOnDown = ui.AllButtons[i + 1];
                ui.AllButtons[i].navigation = panelNav;
            }
            else if (i == ui.AllButtons.Count - 2)
            {
                Navigation panelNav = new Navigation();
                panelNav.mode = Navigation.Mode.Explicit;
                panelNav.selectOnUp = ui.AllButtons[i - 1];
                panelNav.selectOnDown = ui.AllButtons[i + 1];
                ui.AllButtons[i].navigation = panelNav;
            }
            else if (i != ui.AllButtons.Count - 1)
            {
                Navigation panelNav = new Navigation();
                panelNav.mode = Navigation.Mode.Explicit;
                panelNav.selectOnRight = balloonUiItemsButtonData[0].selectButton;
                panelNav.selectOnDown = ui.AllButtons[i + 1];
                panelNav.selectOnUp = ui.AllButtons[i - 1];
                ui.AllButtons[i].navigation = panelNav;
            }
        }
    }
}