using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class OutfitControllerOld : MonoBehaviour
{
    [SerializeField] PhotonView photonView;

    [SerializeField] Gender _currentGender;

    public Transform mainCharacter;
    public Transform[] swappableCharacters;

    public Dictionary<string, List<SwappableBodyPart>> bodyPartsWrapper = new Dictionary<string, List<SwappableBodyPart>>();

    public string[] swappableBodyPartNames;
    public string commonPartName;
    public string[] subPartNames;

    public string[] choosenOutfit;

    [SerializeField]
    List<SwappableBodyPart> currentSwappableBodyParts;
    SwappableBodyPart currentSelectedBodyPart;

    public static Action<bool> OnEnableObject;

    private void Start()
    {
        photonView = GetComponent<PhotonView>();
        //StartOutfitChange();
    }







    public void StartOutfitChange()
    {
        //photonView = GetComponent<PhotonView>();
        //if (!photonView.IsMine)
        //{
        //    return;
        //}

        print("Starting to change outfit!!");
        photonView = GetComponent<PhotonView>();
        choosenOutfit = new string[swappableBodyPartNames.Length + 1];
        for (int i = 1; i < choosenOutfit.Length; i++)
        {
            choosenOutfit[i] = "0";
        }

        choosenOutfit[0] = swappableBodyPartNames[0];

        //bodyPartsWrapper = new Dictionary<string, List<SwappableBodyPart>>();

        for (int i = 0; i < swappableCharacters.Length; i++)
        {
            for (int j = 0; j < swappableBodyPartNames.Length; j++)
            {
                var swappablePart = swappableCharacters[i].Find(swappableBodyPartNames[j]);

                if (swappablePart != null)
                {
                    var currentOutfit = swappablePart.gameObject.AddComponent<Outfit>();
                    currentOutfit.target = mainCharacter.Find(swappableBodyPartNames[j]).gameObject;
                    currentOutfit.AssignToActor();

                    List<Transform> tempSubparts = null;
                    if (swappableBodyPartNames[j] == commonPartName)
                    {
                        tempSubparts = new List<Transform>();
                        for (int k = 0; k < subPartNames.Length; k++)
                        {
                            var currentSubpart = swappableCharacters[i].Find(subPartNames[k]);
                            if (currentSubpart != null)
                            {
                                var currentSubpartOutfit = currentSubpart.gameObject.AddComponent<Outfit>();
                                currentSubpartOutfit.target = mainCharacter.Find(subPartNames[k]).gameObject;
                                currentSubpartOutfit.AssignToActor();
                                tempSubparts.Add(currentSubpart);
                            }
                        }
                    }

                    SwappableBodyPart tempSwappableBodyParts = new SwappableBodyPart
                    {
                        parent = swappablePart,
                        subParts = tempSubparts
                    };


                    if (bodyPartsWrapper.ContainsKey(swappableBodyPartNames[j]))
                    {
                        var currentSwappableBodyparts = bodyPartsWrapper[swappableBodyPartNames[j]];
                        currentSwappableBodyparts.Add(tempSwappableBodyParts);
                    }
                    else
                    {
                        var newSwappableBodyParts = new List<SwappableBodyPart>
                        {
                            tempSwappableBodyParts
                        };
                        bodyPartsWrapper.Add(swappableBodyPartNames[j], newSwappableBodyParts);
                    }
                }

            }
        }


        SelectBodyPartWrapper(swappableBodyPartNames[0]);

       
    }


    void SelectBodyPartWrapper(string partName)
    {
        if (bodyPartsWrapper.ContainsKey(partName))
        {
            OnEnableObject?.Invoke(false); //jugaad :}

            currentSwappableBodyParts = bodyPartsWrapper[partName];

            //var currentSubCategoryButtons = subCategoryButtons[partName];

            //for (int i = 0; i < currentSubCategoryButtons.Count; i++)
            //{
            //    currentSubCategoryButtons[i].gameObject.SetActive(true);
            //}
        }

        choosenOutfit[0] = partName;

    }



    void SelectBodyPart(int index)
    {
        currentSelectedBodyPart = currentSwappableBodyParts[index];

        for (int i = 0; i < currentSwappableBodyParts.Count; i++)
        {
            if (i != index)
            {
                currentSwappableBodyParts[i].parent.gameObject.SetActive(false);
                if (currentSwappableBodyParts[i].subParts != null)
                    for (int j = 0; j < currentSwappableBodyParts[i].subParts.Count; j++)
                    {
                        currentSwappableBodyParts[i].subParts[j].gameObject.SetActive(false);
                    }
            }
        }

        currentSelectedBodyPart.parent.gameObject.SetActive(true);

        if (currentSelectedBodyPart.subParts != null)
            for (int i = 0; i < currentSelectedBodyPart.subParts.Count; i++)
            {
                currentSelectedBodyPart.subParts[i].gameObject.SetActive(true);
            }

        int indexOfSwappableParts = 0;

        for (int i = 0; i < swappableBodyPartNames.Length; i++)
        {
            if (swappableBodyPartNames[i] == choosenOutfit[0])
                indexOfSwappableParts = i + 1;
        }

        choosenOutfit[indexOfSwappableParts] = index.ToString();
    }


    public void SetPlayerBodyFromDatabase(string head, string bottom, string footwear, string top)
    {
        string[] bodyPartsToActivate = { head, bottom, footwear, top };

        for (int k = 0; k < 4; k++)
        {
            SelectBodyPartWrapper(swappableBodyPartNames[k]);
            currentSelectedBodyPart = currentSwappableBodyParts[int.Parse(bodyPartsToActivate[k])];
            int index = int.Parse(bodyPartsToActivate[k]);
            for (int i = 0; i < currentSwappableBodyParts.Count; i++)
            {
                if (i != index)
                {
                    currentSwappableBodyParts[i].parent.gameObject.SetActive(false);
                    if (currentSwappableBodyParts[i].subParts != null)
                        for (int j = 0; j < currentSwappableBodyParts[i].subParts.Count; j++)
                        {
                            currentSwappableBodyParts[i].subParts[j].gameObject.SetActive(false);
                        }
                }
            }

            currentSelectedBodyPart.parent.gameObject.SetActive(true);

            if (currentSelectedBodyPart.subParts != null)
                for (int i = 0; i < currentSelectedBodyPart.subParts.Count; i++)
                {
                    currentSelectedBodyPart.subParts[i].gameObject.SetActive(true);
                }

            int indexOfSwappableParts = 0;

            for (int i = 0; i < swappableBodyPartNames.Length; i++)
            {
                if (swappableBodyPartNames[i] == choosenOutfit[0])
                    indexOfSwappableParts = i + 1;
            }

            choosenOutfit[indexOfSwappableParts] = index.ToString();
        }
    }

}

