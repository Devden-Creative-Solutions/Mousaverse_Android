using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Outfit : MonoBehaviour
{
    public GameObject target;

    private List<Transform> bones = new List<Transform>();
    private Transform[] boneArray;

    public bool callAtStart;

    private void Start()
    {
        if (callAtStart)
            AssignToActor();
    }


    public void AssignToActor()
    {
        //transform.parent = target.transform;
        //transform.localPosition = Vector3.zero;

        SkinnedMeshRenderer targetRenderer = target.GetComponent<SkinnedMeshRenderer>();
        Dictionary<string, Transform> boneMap = new Dictionary<string, Transform>();
        foreach (Transform bone in targetRenderer.bones)
        {
            boneMap[bone.name] = bone;
        }

        SkinnedMeshRenderer thisRenderer = GetComponent<SkinnedMeshRenderer>();

        boneArray = thisRenderer.bones;

        for (int i = 0; i < boneArray.Length; i++)
        {

            string boneName = boneArray[i].name;

            if (boneMap.TryGetValue(boneName, out boneArray[i]) == false)
            {
                Debug.LogError("failed to get bone: " + boneName);
                Debug.LogError(i);

                //Debug.Break();
            }
        }
        thisRenderer.bones = boneArray; //take effect
    }
}
