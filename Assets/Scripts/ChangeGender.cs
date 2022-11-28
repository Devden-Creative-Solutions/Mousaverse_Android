using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Gui;

public class ChangeGender : MonoBehaviour
{
    [SerializeField] LeanToggle _maleToggle;
    [SerializeField] LeanToggle _femaleToggle;
    [SerializeField] OutfitController _maleOutfilController;
    [SerializeField] OutfitController _femaleOutfilController;



    public void ToggleMale()
    {
        _maleToggle.TurnOn();
    }

    public void UpdateMaleDatabase()
    {
        FirebaseManager.Instance.UpdateTheGender(Gender.Male);
        _maleOutfilController.UpdatePlayerBodyDatabase();
        _maleOutfilController.SelectTheBodyPartToHead();
    }

    public void ToggleFemale()
    {
     
        _femaleToggle.TurnOn();
    }

    public void UpdateFemaleDatabase()
    {
        FirebaseManager.Instance.UpdateTheGender(Gender.Female);
        _femaleOutfilController.UpdatePlayerBodyDatabase();
        _femaleOutfilController.SelectTheBodyPartToHead();
    }
}
