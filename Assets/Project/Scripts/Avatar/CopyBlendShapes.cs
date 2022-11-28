using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyBlendShapes : MonoBehaviour
{
    SkinnedMeshRenderer playerHead;

    SkinnedMeshRenderer slothHead;

    string[] faceBlendNames = new string[] { "jawOpen", "mouthSmile", "eyesClosed", "mouthFunnel", "mouthPucker", "eyeBlinkLeft", "eyeBlinkRight" };


    private void Start()
    {
        playerHead = GetComponent<SkinnedMeshRenderer>();


    }

    private void FixedUpdate()
    {
        
    }


}
