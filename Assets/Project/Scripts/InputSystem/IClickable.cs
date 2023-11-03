using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IClickable
{
    public bool clickStatus
    {
        get;
        set;
    }

    public void OnClick();
}
