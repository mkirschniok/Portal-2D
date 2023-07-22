using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalCloneMove : MonoBehaviour
{
    Vector3 offsetToSourceObject;
    [SerializeField] GameObject sourceObject;

    void Update()
    {
        if (sourceObject != null)
            transform.position = sourceObject.transform.position + offsetToSourceObject;
    }

    public void ResetSource(GameObject source)
    {
        sourceObject = source;
        if (sourceObject != null)
            offsetToSourceObject = transform.position - sourceObject.transform.position;
    }
}
