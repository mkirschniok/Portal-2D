using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    [SerializeField] Cloner cloner;

    void OnTriggerEnter2D(Collider2D collision)
    {
        cloner.ExecuteTeleport(collision.gameObject);
    }

    void OnTriggerExit2D(Collider2D collision)
    {
    }
}
