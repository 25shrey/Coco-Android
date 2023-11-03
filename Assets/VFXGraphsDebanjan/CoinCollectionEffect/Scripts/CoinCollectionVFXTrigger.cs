using Unity.VisualScripting;
using UnityEngine;

public class CoinCollectionVFXTrigger : VFXAction
{
    [SerializeField] private GameObject content;

    public override void OnVFXPlay(float delay)
    {
        base.OnVFXPlay(delay);
        content.SetActive(false);
    }

    public override void OnVFXCompleted()
    {
        base.OnVFXCompleted();
        content.SetActive(true);
    }
}