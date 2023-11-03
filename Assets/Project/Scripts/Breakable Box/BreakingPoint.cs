using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakingPoint : MonoBehaviour
{
    public Breakableitem item;
    public LayerMask _layer;
    public GameObject Up;
    public GameObject Down;

    public void OnTriggerEnter(Collider collision)
    {
        if (((1 << collision.gameObject.layer) & _layer) != 0)
        {
            if (GameManager.instance.Player.playerPowerUps.IsPowerUpActive(PowerUpType.Magnet))
            {
                if(Vector3.Distance(GameManager.instance.Player.transform.position, transform.position) < 3f)
                {
                    ExplodeInitiate(collision);
                }
            }
            else
            {
                ExplodeInitiate(collision);
            }
        }
    }

    void ExplodeInitiate(Collider collision)
    {
        if (transform.gameObject == Up)
        {
            item.type = Breakableitem.BrekableType.Ground;
        }
        else
        {
            item.type = Breakableitem.BrekableType.Air;
        }

        item.Explode(collision.transform.position + (Vector3.down * 2));

        Destroy(Up);
        Destroy(Down);
    }
}
