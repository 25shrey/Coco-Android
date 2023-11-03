using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Serialization;

public class CustomSelectable : Selectable
{
    //Events
    public delegate void OnOptionsValueChangedDelegate(string optionSelected, int optionIndex);
    public event OnOptionsValueChangedDelegate onOptionsValueChanged; 
    
    //Variables
    [SerializeField] private List<string> options;
    [SerializeField] private TextMeshProUGUI text;
    private string currentSelected;
    
    [HideInInspector]
    public int currentIndex = 0;
    public int currentValue
    {
        get { return currentIndex; }
    }

    protected override void Start()
    {
        base.Start();
        Init();
       // resetButton.onClick.AddListener(ResetData);
    }

    private void Init()
    {
        if (options.Count > 0)
        {
            text.text = options[currentIndex];
        }
    }

    public void OnLeftButtonClickChangeText()
    {
        if (currentIndex == 0)
        {
            currentIndex = options.Count - 1; 
            text.text = options[currentIndex];
        }
        else
        {
            currentIndex--;
            text.text = options[currentIndex];
        }
        onOptionsValueChanged?.Invoke(currentSelected, currentIndex);
        currentSelected = text.text;
    }

    public void OnRightButtonClickChangeText()
    {
        if (currentIndex == options.Count - 1)
        {
            currentIndex = 0;
            text.text = options[currentIndex];
        }
        else
        {
            currentIndex++;
            text.text = options[currentIndex];
        }
        onOptionsValueChanged?.Invoke(currentSelected, currentIndex);
        currentSelected = text.text;
    }

    public void AddOptions(List<string> data)
    {
        options = data;
    }

    public string GetCurrentSelected()
    {
        return currentSelected;
    }
    
    public void RefreshOption(int index)
    {
        currentIndex = index;
        text.text = options[currentIndex];
    }

    public void AddNewDataInOptions(string data)
    {
        options.Add(data);
    }
    
    
    public void ClearAllOptions()
    {
        options.Clear();
    }

    public void ResetData()
    {
        currentIndex = 0;
        text.text = options[currentIndex];
        currentSelected = text.text;
    }
}
