using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Lean.Gui;
using Lean.Transition;
using Lean.Transition.Method;

public class OutfitController : MonoBehaviour
{
    public GameObject categoryBtnPrefab;
    public GameObject subCategoryBtnPrefab;

    public Transform categoryBtnHolder;
    public Transform subCategoryBtnHolder;

    public Transform mainCharacter;
    public Transform[] swappableCharacters;

    public Dictionary<string, List<SwappableBodyPart>> bodyPartsWrapper;
    public Dictionary<string, List<SubCategoryBtn>> subCategoryButtons;

    public string[] swappableBodyPartNames;
    public string commonPartName;
    public string[] subPartNames;
    public string[] choosenOutfit;

    string[] itemNameArray = {"Face","Footwear","Top","Bottom"};
    [SerializeField] Button[] subCategoryButtonArray;

    [SerializeField] Gender currentGender;

    [SerializeField]
    List<SwappableBodyPart> currentSwappableBodyParts;

    [SerializeField]
    SwappableBodyPart currentSelectedBodyPart;

    public static Action<bool> OnEnableObject;

    private void Start()
    {
        Initialize();
        subCategoryButtonArray = new Button[subCategoryBtnHolder.transform.childCount];
    }

    private void OnDestroy()
    {
        foreach (var item in subCategoryButtons)
        {
            for (int i = 0; i < item.Value.Count; i++)
            {
                item.Value[i].Destroy();
            }
        }

        subCategoryButtons.Clear();

    }


    public void AddImagesToButton()
    {
        int i = 0;
        int j = 0;
        foreach(Transform x in subCategoryBtnHolder.transform)
        {
            if (i != 0)
            {
                subCategoryButtonArray[j] = x.GetComponent<Button>();
                j++;
            }
            i++;
        }

        int count = 0;
        int count2 = 0;
        foreach(var x in bodyPartsWrapper)
        {
            print("Key: " + x.Key);
            foreach(var y in x.Value)
            {
                
                switch (currentGender)
                {
                    case Gender.Female:
                        
                        string femalePath = "CharacterRenders\\Female\\" + y.parent.parent.gameObject.name + "\\" + itemNameArray[count];
                        print("Starting Female coroutine");
                        StartCoroutine(ChangeSprite(subCategoryButtonArray[count2], femalePath));
                        break;

                    case Gender.Male:
                        
                        string malePath = "CharacterRenders\\Male\\" + y.parent.parent.gameObject.name + "\\" + itemNameArray[count];
                        StartCoroutine(ChangeSprite(subCategoryButtonArray[count2], malePath));
                        
                        break;
                }
                count2++;
            }

            count++;
        }
    }

    IEnumerator ChangeSprite(Button button, string path)
    {
        var request = Resources.LoadAsync<Sprite>(path);
        yield return new WaitUntil(() => request.isDone);
        button.GetComponent<Image>().sprite = (Sprite)request.asset;
        button.GetComponent<Image>().preserveAspect = true;
        button.transform.GetChild(0).GetComponent<TMP_Text>().text = "";
        button.GetComponent<LeanGraphicColor>().BeginAllTransitions();
    }


    void Initialize()
    {
        choosenOutfit = new string[swappableBodyPartNames.Length + 1];
        for (int i = 1; i < choosenOutfit.Length; i++)
        {
            choosenOutfit[i] = "0";
        }

        choosenOutfit[0] = swappableBodyPartNames[0];


        bodyPartsWrapper = new Dictionary<string, List<SwappableBodyPart>>();
        subCategoryButtons = new Dictionary<string, List<SubCategoryBtn>>();
        subCategoryButtons.Clear();

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

        int count = 0;
        foreach (var item in bodyPartsWrapper)
        {
            var currentSwappableBodyParts = item.Value;
            
            var categoryBtn = Instantiate(categoryBtnPrefab, categoryBtnHolder).GetComponent<Button>();
            categoryBtn.gameObject.SetActive(true);
            categoryBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = GetActualBodyPartText(item.Key);
            categoryBtn.onClick.AddListener(() =>
            {
                SelectBodyPartWrapper(item.Key);
            });

            for (int i = 0; i < currentSwappableBodyParts.Count; i++)
            {
                int j = i;
                var subCategoryBtn = Instantiate(subCategoryBtnPrefab, subCategoryBtnHolder).GetComponent<Button>();
                //subCategoryBtn.gameObject.SetActive(true);
                var subCategoryButtonObj = subCategoryBtn.GetComponent<SubCategoryBtn>();
                subCategoryButtonObj.Init();
                if (subCategoryButtons.ContainsKey(item.Key))
                {
                    var tempSubcategoryButtons = subCategoryButtons[item.Key];
                    tempSubcategoryButtons.Add(subCategoryButtonObj);
                }
                else
                {
                    var newSubCategoryButtons = new List<SubCategoryBtn>
                        {
                            subCategoryButtonObj
                        };
                    subCategoryButtons.Add(item.Key, newSubCategoryButtons);
                }
                subCategoryBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Loading";
                

                subCategoryBtn.onClick.AddListener(() =>
                {
                    SelectBodyPart(j);
                });

                count++;
            }
        }
        Invoke(nameof(AddImagesToButton), 1f);
        SelectBodyPartWrapper(swappableBodyPartNames[0]);
    }

    public void SelectTheBodyPartToHead()
    {
        SelectBodyPartWrapper(swappableBodyPartNames[0]);
    }

    string GetActualBodyPartText(string bodypart)
    {
        string tempBodyPart = "";
        switch (bodypart)
        {
            case "Wolf3D_Head":
                tempBodyPart = "Head";
                break;

            case "Wolf3D_Outfit_Bottom":
                tempBodyPart = "Bottom";
                break;

            case "Wolf3D_Outfit_Footwear":
                tempBodyPart = "Footwear";
                break;

            case "Wolf3D_Outfit_Top":
                tempBodyPart = "Top";
                break;

        }

        return tempBodyPart;
    }

    void SelectBodyPartWrapper(string partName)
    {
        if (bodyPartsWrapper.ContainsKey(partName))
        {
            OnEnableObject?.Invoke(false); //jugaad :}

            currentSwappableBodyParts = bodyPartsWrapper[partName];

            var currentSubCategoryButtons = subCategoryButtons[partName];

            for (int i = 0; i < currentSubCategoryButtons.Count; i++)
            {
                currentSubCategoryButtons[i].gameObject.SetActive(true);
            }
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

        UpdatePlayerBodyDatabase();
    }


    public void UpdatePlayerBodyDatabase()
    {
        FirebaseManager.Instance.UpdatePlayerBodyParts(choosenOutfit[1], choosenOutfit[4], choosenOutfit[3], choosenOutfit[2],currentGender);
        SetTheBodyPartsInPlayerInfo();
    }

    public void SetPlayerBodyFromDatabase(string head,string bottom, string footwear,string top)
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

        SelectBodyPartWrapper(swappableBodyPartNames[0]);
        SetTheBodyPartsInPlayerInfo();


       
    }

   void SetTheBodyPartsInPlayerInfo()
    {
        TheBodyParts theBodyParts = new TheBodyParts();
        theBodyParts.Head = choosenOutfit[1];
        theBodyParts.Top = choosenOutfit[4];
        theBodyParts.FootWear = choosenOutfit[3];
        theBodyParts.Bottom = choosenOutfit[2];
        theBodyParts.Gender = currentGender.ToString();
        PlayerInfoDontDestroy.Instance.SetSelectedBodyParts(theBodyParts);
       
    }
}



public enum Gender
{
    Male,
    Female
}

[System.Serializable]
public class SwappableBodyPart
{
    public Transform parent;
    public List<Transform> subParts;
}

