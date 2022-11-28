using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ShowARCamView : MonoBehaviour
{
    WebCamTexture cameraTexture;

    [SerializeField]
    RawImage camImage;

    //bool permissionGranted;
    //bool playCamTexture;

    //public void VerifyPermission()
    //{
    //    iOSCameraPermission.VerifyPermission(gameObject.name, "SampleCallback");
    //}

    //private void SampleCallback(string permissionWasGranted)
    //{
    //    Debug.Log("Callback.permissionWasGranted = " + permissionWasGranted);

    //    if (permissionWasGranted == "true")
    //    {
    //        permissionGranted = true;
           
    //    }
    //    else
    //    {
    //        permissionGranted = false;
    //    }
    //}

    //private void Update()
    //{
    //    if (permissionGranted)
    //    {
    //        if (!playCamTexture)
    //        {
               
    //            playCamTexture = true;
    //        }
    //    }
    //}


    void Start()
    {
        if (DetermineIfFaceTrackingSupported())
            return;

        camImage.gameObject.SetActive(false);

        //string selectedDeviceName = "";
        //WebCamDevice[] allDevices = WebCamTexture.devices;
        //for (int i = 0; i < allDevices.Length; i++)
        //{
        //    if (allDevices[i].isFrontFacing)
        //    {
        //        selectedDeviceName = allDevices[i].name;
        //        break;
        //    }
        //}

        //cameraTexture = new WebCamTexture(selectedDeviceName);
        //camImage.texture = cameraTexture;
        //cameraTexture.Play();

    }

    public static bool DetermineIfFaceTrackingSupported()
    {
        var descriptors = new List<XRFaceSubsystemDescriptor>();
        SubsystemManager.GetSubsystemDescriptors(descriptors);
        if (descriptors.Any())
        {
            var descriptor = descriptors.First();
            Debug.Log("Face Tracking is supported, supportsEyeTracking: " + descriptor.supportsEyeTracking + ", supportsFacePose: " + descriptor.supportsFacePose + ", supportsFaceMeshNormals: " + descriptor.supportsFaceMeshNormals + ", supportsFaceMeshUVs: " + descriptor.supportsFaceMeshUVs + ", supportsFaceMeshVerticesAndIndices: " + descriptor.supportsFaceMeshVerticesAndIndices);

            return true;
        }
        else
        {
            Debug.Log("Face Tracking is not supported.");
            return false;
        }
    }

}
