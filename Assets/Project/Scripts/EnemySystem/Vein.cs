using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vein : MonoBehaviour
{
    public float lerpDuration = 3.0f;
    private Vector3 initialScale;
    private Vector3 target;
    Coroutine routine;
    [SerializeField]
    protected LayerMask _layer;
    [SerializeField]
    private StonemanEnemy _enemy;
    [SerializeField]
    private BoxCollider _boxCollider;

    public bool wasSuccessful;

    void Start()
    {
        _boxCollider = GetComponent<BoxCollider>();
        transform.localScale = Vector3.zero;
    }

    public void Initiate()
    {
        wasSuccessful = false;  
        transform.localScale = new Vector3(3f, 3f, 0f);
        _boxCollider.enabled = true;
        initialScale = transform.localScale;
        target = GameManager.instance.Player.transform.position;
        float distance = Vector3.Distance(target, transform.position);
        distance /= distance;
        target = new Vector3(initialScale.x, initialScale.y+1, -(distance+0.5f));
        routine = StartCoroutine(ScaleExpandCoroutine());
    }

    private IEnumerator ScaleExpandCoroutine()
    {
        float elapsedTime = 0.0f;

        while (elapsedTime < lerpDuration)
        {
            float lerpProgress = elapsedTime / lerpDuration;

            transform.localScale = Vector3.Lerp(initialScale, target, lerpProgress);

            elapsedTime = elapsedTime + (Time.deltaTime * 2);

            yield return null;
        }

        transform.localScale = target;
        _boxCollider.enabled = false;

        yield return new WaitForSeconds(0.2f);

        StartCoroutine(ScaleShrinkCoroutine(transform.localScale, initialScale, 1f));
    }

    private IEnumerator ScaleShrinkCoroutine(Vector3 initialScale, Vector3 target, float time)
    {
        yield return new WaitForSeconds(0.5f);

        float elapsedTime = 0.0f;

        while (elapsedTime < time)
        {
            float lerpProgress = elapsedTime / lerpDuration;
            transform.localScale = Vector3.Lerp(initialScale, target, lerpProgress);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localScale = target;
        if (!wasSuccessful)
        {
            _enemy.PlayerGrabFail();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & _layer) != 0)
        {
            wasSuccessful = true;
            StopCoroutine(routine);
            _enemy.PlayerGrabSuccessful();
            Player player = other.gameObject.GetComponent<Player>();
            if (player != null)
            {
                player.EnableRope();
            }
            StartCoroutine(ScaleShrinkCoroutine(transform.localScale, initialScale, 1.5f));
            _boxCollider.enabled = false;
        }
    }

}
