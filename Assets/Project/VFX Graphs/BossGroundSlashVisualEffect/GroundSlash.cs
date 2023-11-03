using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.VFX;

public class GroundSlash : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float slowDownRate;
    [SerializeField] private float destroyTime = 5f;
    [SerializeField] private Rigidbody slashRb;
    [SerializeField] private VisualEffect slashVFX;
    private Vector3 position;

    private void OnEnable()
    {
        Debug.Log("Inside GroundSlash - OnEnable");
        position = slashVFX.transform.localPosition;

        StartCoroutine(SlowDown());
        StartCoroutine(ResetContent(3f));
    }

  
    public void Shoot()
    {
        slashVFX.Play();    
        slashRb.velocity = transform.right * speed;
    }

    private IEnumerator SlowDown()
    {
        float time = 0; 
        if (time > 0)
        {
            slashRb.velocity = Vector3.Lerp(Vector3.zero, slashRb.velocity, time);
            time -= slowDownRate;
            yield return new WaitForSeconds(0.1f);
        }
    }

    private IEnumerator ResetContent(float resetTime = 3f)
    {
        yield return new WaitForSeconds(resetTime);
        slashRb.velocity = Vector3.zero;
        transform.localPosition = position;
        gameObject.SetActive(false);
    }
}
