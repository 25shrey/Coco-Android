using UnityEngine;
using UnityEngine.VFX;

public class VFXPlayer : MonoBehaviour
{
    [SerializeField] private VisualEffect vfx;
    [SerializeField] private float timeToDestroy;
    string destroyVFXString = "DestoryVFX";

    public void PlayVFX()
    {
        vfx.Play();
        Invoke(destroyVFXString, timeToDestroy);
    }

    public void DestoryVFX()
    {
        Destroy(gameObject);
    }
}
