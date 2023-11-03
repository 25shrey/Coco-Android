using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopCoinGemsPackList : MonoBehaviour
{
    public RectTransform rootTransform;
    public InventoryCoinGemsPackUiItem itemPrefab;

    public List<InventoryCoinGemsPackUiItem> coin_gemUiItemsButtonData;

    [SerializeField] private CustomiseUI ui;

    public void PopulateList(CoinGemsPack_DataSO data)
    {
        foreach (Transform t in rootTransform)
        {
            Destroy(t.gameObject);
        }

        coin_gemUiItemsButtonData.Clear();
        for (int i = 0; i < data.inventoryData.Capacity; i++)
        {
            InventoryCoinGemsPackUiItem coinGemPackItem = Instantiate(itemPrefab, rootTransform);
            coinGemPackItem.SetUiItemData(data.inventoryData[i]);
            coin_gemUiItemsButtonData.Add(coinGemPackItem);
            coinGemPackItem.pageIndex = 1;
        }

        SetNavigation();
    }


    public void SetNavigation()
    {
        for (int index = 0; index < coin_gemUiItemsButtonData.Count; index++)
        {
            Navigation nav = new Navigation();
            nav.mode = Navigation.Mode.Explicit;
            if (index == 0)
            {
                nav.selectOnRight = coin_gemUiItemsButtonData[index + 1].selectButton;
                nav.selectOnLeft = ui.AllButtons[4];
            }
            else if (index == coin_gemUiItemsButtonData.Count - 1)
            {
                nav.selectOnLeft = coin_gemUiItemsButtonData[index - 1].selectButton;
            }
            else
            {
                nav.selectOnRight = coin_gemUiItemsButtonData[index + 1].selectButton;
                nav.selectOnLeft = coin_gemUiItemsButtonData[index - 1].selectButton;
            }
            coin_gemUiItemsButtonData[index].selectButton.navigation = nav;
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
                panelNav.selectOnRight = coin_gemUiItemsButtonData[0].selectButton;
                ui.AllButtons[i].navigation = panelNav;
            }
            else if (i != ui.AllButtons.Count - 1)
            {
                Navigation panelNav = new Navigation();
                panelNav.mode = Navigation.Mode.Explicit;
                panelNav.selectOnDown = ui.AllButtons[i + 1];
                panelNav.selectOnUp = ui.AllButtons[i - 1];
                ui.AllButtons[i].navigation = panelNav;
            }
        }
    }
}