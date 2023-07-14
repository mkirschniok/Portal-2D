using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public struct CloneAnimPair
{
    public CloneAnimPair(GameObject s, GameObject c)
    {
        source = s;
        clone = c;
    }

    public GameObject source;
    public GameObject clone;
}

public class CloneAnimSync : MonoBehaviour
{
    [SerializeField] GameObject animSourceObject;
    [SerializeField] GameObject animDestRoot;
    List<CloneAnimPair> cloneAnimList;


    // Start is called before the first frame update
    void Start()
    {
        cloneAnimList = new List<CloneAnimPair>();
        if( animSourceObject == null || animDestRoot == null )
            return;

        Transform[] srcComponents = animSourceObject.GetComponentsInChildren<Transform>();
        Transform[] dstComponents = animDestRoot.GetComponentsInChildren<Transform>();
        foreach( Transform dstComponent in dstComponents )
        {
            foreach( Transform sourceComponent in srcComponents )
            {
                if( sourceComponent.name == dstComponent.name )
                {
                    cloneAnimList.Add(new CloneAnimPair(sourceComponent.gameObject, dstComponent.gameObject));
                    break;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach( CloneAnimPair pair in cloneAnimList )
        {
            pair.source.transform.GetLocalPositionAndRotation(out var localPos, out var localRot);
            // Clone local pose to destination transform in one operation.
            pair.clone.transform.SetLocalPositionAndRotation(localPos, localRot);


            //pair.clone.transform.localPosition    = pair.source.transform.localPosition;
            //pair.clone.transform.localEulerAngles = pair.source.transform.localEulerAngles;
            //pair.clone.transform.localScale       = pair.source.transform.localScale;

            //UnityEngine.Debug.Log("Copy transform!");
        }
    }
}
