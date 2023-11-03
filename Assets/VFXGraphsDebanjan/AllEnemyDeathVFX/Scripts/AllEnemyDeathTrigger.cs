using UnityEngine;

public class AllEnemyDeathTrigger : VFXAction
{
    [SerializeField] private GameObject content;
    public override void OnVFXPlay(float delay)
    {
        content.SetActive(false);
        base.OnVFXPlay(delay);
    }

    public override void OnVFXCompleted()
    {
        base.OnVFXCompleted();
        content.SetActive(true);
    }

}