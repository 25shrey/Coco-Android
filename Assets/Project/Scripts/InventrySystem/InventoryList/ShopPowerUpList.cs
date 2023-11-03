using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopPowerUpList : MonoBehaviour
{
    public RectTransform rootTransform;
    public InventoryPowerUpUiItem itemPrefab;

    public List<InventoryPowerUpUiItem> powerUpUiItemsButtonData;

    [SerializeField] private CustomiseUI ui;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void PopulateList(PowerUps_DataSO data)
    {
        foreach (Transform t in rootTransform)
        {
            Destroy(t.gameObject);
        }

        powerUpUiItemsButtonData.Clear();   
        for (int i = 0; i < data.inventoryData.Capacity; i++)
        {
            InventoryPowerUpUiItem powerUpItem = Instantiate(itemPrefab, rootTransform);
            powerUpItem.SetUiItemData(data.inventoryData[i]);
            powerUpUiItemsButtonData.Add(powerUpItem);
            powerUpItem.pageIndex = 1;
        }

        SetNavigation();
    }

    public void SetNavigation()
    {
        for (int index = 0; index < powerUpUiItemsButtonData.Count; index++)
        {
            Navigation nav = new Navigation();
            nav.mode = Navigation.Mode.Explicit;
            if (index == 0)
            {
                nav.selectOnRight = powerUpUiItemsButtonData[index + 1].selectButton;
                nav.selectOnLeft = ui.AllButtons[3];
            }
            else if (index == powerUpUiItemsButtonData.Count - 1)
            {
                nav.selectOnLeft = powerUpUiItemsButtonData[index - 1].selectButton;
            }
            else
            {
                nav.selectOnRight = powerUpUiItemsButtonData[index + 1].selectButton;
                nav.selectOnLeft = powerUpUiItemsButtonData[index - 1].selectButton;
            }
            powerUpUiItemsButtonData[index].selectButton.navigation = nav;
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
                panelNav.selectOnRight = powerUpUiItemsButtonData[0].selectButton;
                panelNav.selectOnDown = ui.AllButtons[i + 1];
                panelNav.selectOnUp = ui.AllButtons[i - 1];
                ui.AllButtons[i].navigation = panelNav;
            }
        }
    }
}