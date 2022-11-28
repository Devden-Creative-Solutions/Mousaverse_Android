using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class BlendshapesSync : MonoBehaviourPun, IPunObservable
{
    [SerializeField]
    PhotonView photonView;

    [SerializeField]
    ARFaceManager arFaceManager;

    SkinnedMeshRenderer slothHead;
    string slothHeadBlendPrefix = "blendShape2.";
    int[] slothHeadBlendIndexes;


    SkinnedMeshRenderer skinnedMesh;
    [SerializeField]
    SkinnedMeshRenderer teethSkinnedMesh;
    string[] faceBlendNames = new string[] { "jawOpen", "mouthSmileLeft", "mouthSmileRight", "mouthFunnel", "mouthPucker", "eyeBlinkLeft", "eyeBlinkRight" };

    float[] faceBlendWeights;
    int[] faceBlendIndexes;

    bool isSlothHeadIndexesFilled;

    [SerializeField]
    private PlayerType currentPlayerType;

    bool ArFaceManagerPresent;

    private void Start()
    {
        if (photonView.IsMine)
        {
            currentPlayerType = PlayerInfoDontDestroy.Instance.currentPlayerType;

            if (currentPlayerType == PlayerType.Singer)
            {
                arFaceManager = FindObjectOfType<ARFaceManager>();
                if (arFaceManager != null)
                {
                    arFaceManager.facesChanged += ArFaceManager_facesChanged;
                    slothHeadBlendIndexes = new int[faceBlendNames.Length];
                }
                else
                    ArFaceManagerPresent = false;
            }
        }

        faceBlendWeights = new float[faceBlendNames.Length];
        faceBlendIndexes = new int[faceBlendNames.Length];


        teethSkinnedMesh = transform.parent.Find("Wolf3D_Teeth").GetComponent<SkinnedMeshRenderer>();
        skinnedMesh = GetComponent<SkinnedMeshRenderer>();


        for (int i = 0; i < faceBlendIndexes.Length; i++)
        {
            faceBlendIndexes[i] = skinnedMesh.sharedMesh.GetBlendShapeIndex(faceBlendNames[i]);
        }

    }

    void OnDestroy()
    {
        if (!ArFaceManagerPresent)
            return;

        if (photonView.IsMine && currentPlayerType == PlayerType.Singer)
            arFaceManager.facesChanged -= ArFaceManager_facesChanged;
    }

    private void ArFaceManager_facesChanged(ARFacesChangedEventArgs obj)
    {
        if (!photonView.IsMine || currentPlayerType != PlayerType.Singer)
            return;

        Debug.Log("Added : " + obj.added.Count + "  Updated : " + obj.updated.Count + "  Removed : " + obj.removed.Count);
        Debug.Log("obj name : " + obj.updated[obj.updated.Count - 1].transform.name);

        slothHead = obj.updated[obj.updated.Count - 1].transform.GetChild(1).GetComponent<SkinnedMeshRenderer>();

        if (!isSlothHeadIndexesFilled && slothHead)
        {
            for (int i = 0; i < faceBlendIndexes.Length; i++)
            {
                var slothHeadFormat = slothHeadBlendPrefix + faceBlendNames[i];

                if (slothHeadFormat.Contains("Left"))
                    slothHeadFormat = slothHeadFormat.Replace("Left", "_L");
                else if (slothHeadFormat.Contains("Right"))
                    slothHeadFormat = slothHeadFormat.Replace("Right", "_R");

                Debug.Log("sloth Head Blend Names " + i + " : " + slothHeadFormat);

                slothHeadBlendIndexes[i] = slothHead.sharedMesh.GetBlendShapeIndex(slothHeadFormat);
            }

            isSlothHeadIndexesFilled = true;
        }
    }

    private void Update()
    {
        if (!photonView.IsMine)
        {
            for (int i = 0; i < faceBlendIndexes.Length; i++)
            {
                var smoothenedVal = Mathf.Lerp(skinnedMesh.GetBlendShapeWeight(faceBlendIndexes[i]), faceBlendWeights[i], 15.0f * Time.deltaTime);
                skinnedMesh.SetBlendShapeWeight(faceBlendIndexes[i], smoothenedVal);
                teethSkinnedMesh.SetBlendShapeWeight(faceBlendIndexes[i], smoothenedVal);
            }
        }

    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (!gameObject.activeInHierarchy)
            return;

        if (stream.IsWriting)
        {
            if (ArFaceManagerPresent && skinnedMesh)
            {
                if (slothHead && currentPlayerType == PlayerType.Singer)
                {
                    for (int i = 0; i < faceBlendIndexes.Length; i++)
                    {
                        //var weight = skinnedMesh.GetBlendShapeWeight(faceBlendIndexes[i]);
                        var weight = slothHead.GetBlendShapeWeight(slothHeadBlendIndexes[i]);

                        skinnedMesh.SetBlendShapeWeight(faceBlendIndexes[i], weight);

                        stream.SendNext(weight);
                        teethSkinnedMesh.SetBlendShapeWeight(faceBlendIndexes[i], weight);
                    }
                }
            }
        }
        else if (stream.IsReading)
        {
            if (skinnedMesh)
            {
                for (int i = 0; i < faceBlendIndexes.Length; i++)
                {
                    var weight = (float)stream.ReceiveNext();
                    faceBlendWeights[i] = weight;
                    //skinnedMesh.SetBlendShapeWeight(faceBlendIndexes[i], weight);
                    //teethSkinnedMesh.SetBlendShapeWeight(faceBlendIndexes[i], weight);
                }
            }
        }
    }


}
