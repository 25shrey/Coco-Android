using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScaleEffect", menuName = "Scroll/Scale Effect")]
public class ScaleEffect : BaseScrollSnapEffect
{
    public Vector2 selectedItemScale = Vector2.one * 1.25f;
    public Vector2 unselectedItemScale = Vector2.one;

    public override void OnItemUpdated(RectTransform transform, float displacement)
    {
        Scale(transform, displacement);
    }

    private void Scale(RectTransform transform, float displacement)
    {
        var ratio = GetEffectRatioAbs(displacement);
        var diff = selectedItemScale - unselectedItemScale;
        transform.localScale = unselectedItemScale + diff * ratio;
    }
}
