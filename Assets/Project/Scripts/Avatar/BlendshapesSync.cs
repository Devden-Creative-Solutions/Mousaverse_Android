using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class BlendshapesSync : MonoBehaviourPun, IPunObservable
{
    [SerializeField]
    PhotonView photonView;


    SkinnedMeshRenderer slothHead;
    string slothHeadBlendPrefix = "blendShape1.";
    int[] slothHeadBlendIndexes;


    SkinnedMeshRenderer skinnedMesh;
    [SerializeField]
    SkinnedMeshRenderer teethSkinnedMesh;
    string[] faceBlendNames = new string[] { "MTH_SMILE1" };

    float[] faceBlendWeights;
    int[] faceBlendIndexes;

    bool isSlothHeadIndexesFilled;

    [SerializeField]
    private PlayerType currentPlayerType;


    private void Start()
    {
        

        faceBlendWeights = new float[faceBlendNames.Length];
        faceBlendIndexes = new int[faceBlendNames.Length];


        teethSkinnedMesh = transform.parent.Find("Wolf3D_Teeth").GetComponent<SkinnedMeshRenderer>();
        skinnedMesh = GetComponent<SkinnedMeshRenderer>();


        for (int i = 0; i < faceBlendIndexes.Length; i++)
        {
            faceBlendIndexes[i] = skinnedMesh.sharedMesh.GetBlendShapeIndex("jawOpen");
        }

        if (photonView.IsMine)
        {
            currentPlayerType = PlayerInfoDontDestroy.Instance.currentPlayerType;

            if (currentPlayerType == PlayerType.Singer)
            {

                slothHeadBlendIndexes = new int[faceBlendNames.Length];
                ArFaceManager_facesChanged();
            }
        }

    }


    private void ArFaceManager_facesChanged()
    {
        if (!photonView.IsMine || currentPlayerType != PlayerType.Singer)
            return;


        slothHead = GameObject.Find("MTH_DEF").GetComponent<SkinnedMeshRenderer>();

        if (!isSlothHeadIndexesFilled && slothHead)
        {
            for (int i = 0; i < faceBlendIndexes.Length; i++)
            {
                var slothHeadFormat = slothHeadBlendPrefix + faceBlendNames[i];

                //if (slothHeadFormat.Contains("Left"))
                //    slothHeadFormat = slothHeadFormat.Replace("Left", "_L");
                //else if (slothHeadFormat.Contains("Right"))
                //    slothHeadFormat = slothHeadFormat.Replace("Right", "_R");

                //Debug.Log("sloth Head Blend Names " + i + " : " + slothHeadFormat);

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
            if (skinnedMesh)
            {
                if (slothHead && currentPlayerType == PlayerType.Singer)
                {
                    for (int i = 0; i < faceBlendIndexes.Length; i++)
                    {
                        //var weight = skinnedMesh.GetBlendShapeWeight(faceBlendIndexes[i]);
                        var weight = slothHead.GetBlendShapeWeight(slothHeadBlendIndexes[i])-20;

                        if (weight < 0)
                            weight = 0;
                        if (weight > 100)
                            weight = 100;

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
