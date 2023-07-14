using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Cloner : MonoBehaviour
{
    [SerializeField] GameObject target;
    [SerializeField] GameObject objectToClone;
    GameObject clone;
    Transform spawnPosition;
    string cloneName = "ChellClone";
    int colliderEnterCount = 0;

    public void ExecuteTeleport(GameObject objectToTeleport)
    {
        if(clone)
        {
            var pos = objectToTeleport.transform.position;
            objectToTeleport.transform.position = clone.transform.position;
            clone.transform.position = pos;

            clone.GetComponent<PlayerCloneMove>().Reset();
        }
    }


    void OnTriggerEnter2D(Collider2D collision)
    {
       // PortalManager.Instance.ObjectToCloneProperty = collision.gameObject;
      //  PortalManager.Instance.CreateClone(gameObject, collision.gameObject);
      CreateClone();
    }

    void OnTriggerExit2D(Collider2D collision)
    {
       // PortalManager.Instance.ObjectToCloneProperty = collision.gameObject;
      //  PortalManager.Instance.CreateClone(gameObject, collision.gameObject);
      DestroyClone();
    }

    public void CreateClone()
    {
       colliderEnterCount++;
        if (clone)
            return;

        // stwórz instancjê klona w miejscu portalu, przez który przeszliœmy
       // if (portal.CompareTag("Orange Portal"))
         //   spawnPosition = bluePortal.transform;
       // else
         //   spawnPosition = orangePortal.transform;

        PortalManager.Instance.ObjectToCloneProperty = objectToClone;

        spawnPosition = target.transform;

        // stwórz klona w miejscu portala, przez który przechodzimy i wy³¹cz mu fizykê
        Vector3 yOffset = new Vector3(0, target.transform.position.y - objectToClone.transform.position.y, 0);
        clone = Instantiate(objectToClone, spawnPosition.position - yOffset, Quaternion.identity);
        // objectToClone.GetComponent<PlayerMovement>().Flip();
        if (clone.CompareTag("Player"))
            clone.GetComponent<PlayerMovement>().IsFacingRight = objectToClone.GetComponent<PlayerMovement>().IsFacingRight;
        clone.gameObject.name = cloneName;
        clone.AddComponent<PlayerCloneMove>();
        clone.GetComponent<Rigidbody2D>().simulated = false;
        clone.GetComponent<BoxCollider2D>().enabled = false;
    }

    public void DestroyClone()
    {
        return;
        colliderEnterCount--;
        if (colliderEnterCount == 0)
            Destroy(clone);
    }
}
