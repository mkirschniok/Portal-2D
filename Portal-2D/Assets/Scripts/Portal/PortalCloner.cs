using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class PortalCloner : MonoBehaviour
{
    [SerializeField] GameObject clonePrefab;
    [SerializeField] GameObject destinationPortal;

    //GameObject clone;

    static public Vector3 TransformPosBetweenPortals(Vector3 absPos, GameObject srcPortal, GameObject dstPortal)
    {
        if (srcPortal==null || dstPortal==null)
            return absPos;

        Vector3 objectSrcPortalLocPos = srcPortal.transform.worldToLocalMatrix.MultiplyPoint(absPos);
        Vector3 objectDstPortalLocPos = objectSrcPortalLocPos;
        objectDstPortalLocPos.x *= -1;

        return dstPortal.transform.localToWorldMatrix.MultiplyPoint( objectDstPortalLocPos );
    }

    static public Vector3 PointLocalToWorld(Transform parent, Vector3 pos)
    {
        pos.x *= Mathf.Sign(parent.localScale.x);
        pos.y *= Mathf.Sign(parent.localScale.y);
        pos.z *= Mathf.Sign(parent.localScale.z);
        return parent.localToWorldMatrix.MultiplyPoint(pos);
    }
    static public Vector3 PointWorldToLocal(Transform parent, Vector3 pos)
    {
        var result = parent.worldToLocalMatrix.MultiplyPoint(pos);
        result.x *= Mathf.Sign(parent.localScale.x);
        result.y *= Mathf.Sign(parent.localScale.y);
        result.z *= Mathf.Sign(parent.localScale.z);
        return result;
    }

    static public Vector3 VectorLocalToWorld(Transform parent, Vector3 vec)
    {
        var A = PointLocalToWorld(parent,vec);
        var B = PointLocalToWorld(parent,Vector3.zero);
        return A-B;
    }
    static public Vector3 VectorWorldToLocal(Transform parent, Vector3 vec)
    {
        var A = PointWorldToLocal(parent,vec);
        var B = PointWorldToLocal(parent,Vector3.zero);
        return A-B;
    }

    void OnTriggerEnter2D( Collider2D collision )
    {
        GameObject clone;

        Vector3 worldClonePos = TransformPosBetweenPortals(collision.gameObject.transform.position, gameObject, destinationPortal);
        //clone = Instantiate( clonePrefab , worldClonePos , Quaternion.identity );

        var prefabToClone = clonePrefab;

        if (collision.gameObject.GetComponent<PlayerMovement>() == null)
            prefabToClone = collision.gameObject;

        clone = Instantiate(prefabToClone, worldClonePos, Quaternion.identity, destinationPortal.transform);
        clone.transform.localRotation = collision.gameObject.transform.localRotation;

        //Object.Destroy( clone.GetComponent<Rigidbody2D>() );
        //Object.Destroy( clone.GetComponent<BoxCollider2D>() );

        var clonerController = collision.gameObject.GetComponent<PortalCloneController>();
        if (clonerController == null)
            clonerController = collision.gameObject.AddComponent<PortalCloneController>();

        clonerController.ResetClone(clone, gameObject, destinationPortal);

        var animSync = clone.GetComponent<CloneAnimSync>();
        if( animSync )
            animSync.StartAnim( collision.gameObject );
    }
    void OnTriggerStay2D(Collider2D collision)
    {
        var clone = PortalCloneController.GetCloneFromObject(collision.gameObject);
        if (clone == null)
            return;

        var srcTrueRight = VectorLocalToWorld(transform, Vector3.right);
        var dstTrueRight = VectorLocalToWorld(destinationPortal.transform, Vector3.right);
        var testpoint = transform.position + srcTrueRight*10;
        if( Vector3.Distance( testpoint , collision.gameObject.transform.position ) < 10 )
        {
            collision.gameObject.transform.position = clone.transform.position + dstTrueRight*0.2f;

            if (Mathf.Abs(Vector3.Angle(srcTrueRight, dstTrueRight)) < 90)
            {
                var playerMovement = collision.gameObject.GetComponent<PlayerMovement>();
                if (playerMovement!=null)
                    playerMovement.InverseXMovementAxis();
            }
            DestroyClone( collision.gameObject );
        }
    }

    void OnTriggerExit2D( Collider2D collision )
    {
        DestroyClone( collision.gameObject );
    }

    void DestroyClone( GameObject src )
    {
        var clone = PortalCloneController.GetCloneFromObject(src);
        if( clone )
            Destroy( clone );
    }
}
