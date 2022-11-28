using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class RegionSelection : MonoBehaviour
{
    [SerializeField] TMP_Dropdown RegionDropdown;
    public AllServers allServers = new AllServers();

    private void Start()
    {
        InitializeRegionDropdown();
    }

    void InitializeRegionDropdown()
    {
        List<TMP_Dropdown.OptionData> optionData = new List<TMP_Dropdown.OptionData>();
        foreach (var x in allServers.serverRegions)
        {
            TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData();
            option.text = x.Server;
            optionData.Add(option);
        }
        RegionDropdown.AddOptions(options: optionData);
    }

    public void SetRegion()
    {
        string region = RegionDropdown.options[RegionDropdown.value].text;
        var regionCode = allServers.serverRegions.Where(e => e.Server == region).Select(e => e.ServerCode).Single();
        FirebaseManager.Instance.UpdateThePlayerRegion(region);
        PlayerInfoDontDestroy.Instance.SetSelectedRegionCode(regionCode);
    }

    public void SetDropdown(string region)
    {
        int counter = 0;
        foreach(var x in RegionDropdown.options)
        {
            if(x.text == region)
            {
                RegionDropdown.value = counter;
            }

            counter++;
        }
    }
}

[System.Serializable]
public class ServerRegion
{
    public string Server;
    public string ServerCode;
}

[System.Serializable]
public class AllServers
{
    public List<ServerRegion> serverRegions;
}
