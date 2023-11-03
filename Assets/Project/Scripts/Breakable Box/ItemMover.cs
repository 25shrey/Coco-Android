using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemMover : MonoBehaviour
{
    public float speed;
    public Vector3 target;
    public float arcHeight;

    Vector3 _startPosition;
    float _stepScale;
    float _progress;

    public bool reached;
    
    public BoxCollider _box;

    void Start()
    {
        _startPosition = transform.position;

        float distance = Vector3.Distance(_startPosition, target);
        _stepScale = speed / distance;
    }

    void Update()
    {
        if (!reached)
        {
            _progress = Mathf.Min(_progress + Time.deltaTime * _stepScale, 1.0f);
            float parabola = 1.0f - 4.0f * (_progress - 0.5f) * (_progress - 0.5f);
            Vector3 nextPos = Vector3.Lerp(_startPosition, target, _progress);
            nextPos.y += parabola * arcHeight;
            transform.position = nextPos;

            if (_progress == 1.0f)
                Arrived();
        }
    }

    void Arrived()
    {
        reached = true;
        _box.enabled = true;


        switch (transform.parent.parent.GetComponent<Breakableitem>().itemtype)
        {
            case Breakableitem.Itemtype.coin:
                {
                    transform.GetComponentInChildren<Coin>().enabled = true; break;
                }
            case Breakableitem.Itemtype.magnet:
                {
                    transform.GetComponent<MagnetPower>().enabled = true; break;
                }
            case Breakableitem.Itemtype.shield:
                {
                    transform.GetComponent<ShieldPower>().enabled = true; break;
                }
            case Breakableitem.Itemtype.life:
                {
                    transform.GetComponent<Life>().enabled = true; break;
                }
            case Breakableitem.Itemtype.fruit:
                {
                    transform.GetComponent<Fruit>().enabled = true; break;
                }
            default:
                {
                    print("empty"); break;
                }
        }
    }
}
