using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class PortalCloner : MonoBehaviour
{
    [SerializeField] GameObject clonePrefab;
    [SerializeField] GameObject destinationPortal;

    GameObject clone;

    void OnTriggerEnter2D( Collider2D collision )
    {
        if( clone )
            return;

        var positionOffset = collision.gameObject.transform.position - transform.position;

        clone = Instantiate( clonePrefab , destinationPortal.transform.position + positionOffset , Quaternion.identity );

        var cloneMove = clone.GetComponent<PortalCloneMove>();
        if( cloneMove != null )
            cloneMove.ResetSource( collision.gameObject );

        var animSync = clone.GetComponent<CloneAnimSync>();
        if( animSync )
            animSync.StartAnim( collision.gameObject );
    }
    void OnTriggerStay2D(Collider2D collision)
    {
        var testpoint = transform.position + transform.right*10;
        if( Vector3.Distance( testpoint , collision.gameObject.transform.position ) < 10 )
        {
            var positionOffset = collision.gameObject.transform.position - transform.position;
            collision.gameObject.transform.position = destinationPortal.transform.position + positionOffset;
            DestroyClone();
        }

    }

    void OnTriggerExit2D( Collider2D collision )
    {
        DestroyClone();
    }

    void DestroyClone()
    {
        if( clone )
        {
            Destroy( clone );
            clone = null;
        }
    }
}
