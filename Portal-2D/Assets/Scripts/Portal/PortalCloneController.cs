using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalCloneController : MonoBehaviour
{
    [SerializeField] GameObject clone;
    GameObject ourPortal;
    GameObject clonePortalOutput;

    public GameObject GetClone()
    {
        return clone;
    }

    public static GameObject GetCloneFromObject(GameObject obj)
    {
        var cloneController = obj.GetComponent<PortalCloneController>();
        if (cloneController == null)
            return null;
        return cloneController.GetClone();
    }

    void Update()
    {
        if (clone == null || ourPortal == null || clonePortalOutput == null)
            return;

        clone.transform.localPosition = PortalCloner.PointWorldToLocal(ourPortal.transform, transform.position);

        var localScaleA = PortalCloner.VectorWorldToLocal(ourPortal.transform, transform.localScale);

        clone.transform.localScale = PortalCloner.VectorLocalToWorld(clonePortalOutput.transform, localScaleA);
    }

    public void ResetClone(GameObject newClone, GameObject srcPortal, GameObject dstPortalOutput)
    {
        if (clone!=null && clone!=newClone)
        {
            Destroy(clone);
            clone = null;
        }

        clone = newClone;
        ourPortal = srcPortal;
        clonePortalOutput = dstPortalOutput;
    }
}
