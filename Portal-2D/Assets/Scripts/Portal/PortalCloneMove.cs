using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PortalCloneMove : MonoBehaviour
{
    [SerializeField] GameObject sourceObject;
    GameObject sourcePortal;
    GameObject destinationPortalOutput;

    void Update()
    {
        if (sourceObject == null || sourcePortal == null || destinationPortalOutput == null)
            return;

        transform.localPosition = PortalCloner.PointWorldToLocal(sourcePortal.transform, sourceObject.transform.position);

        var loaclScaleA = PortalCloner.VectorWorldToLocal(sourcePortal.transform, sourceObject.transform.localScale);

        transform.localScale = PortalCloner.VectorLocalToWorld(destinationPortalOutput.transform, loaclScaleA);
    }

    public void ResetSource(GameObject source, GameObject srcPortal, GameObject dstPortalOutput)
    {
        sourceObject = source;
        sourcePortal = srcPortal;
        destinationPortalOutput = dstPortalOutput;
    }
}