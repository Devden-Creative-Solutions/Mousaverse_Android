using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AvatarGroupManager : MonoBehaviour
{

    #region singleton


    private static AvatarGroupManager _instance;

    public static AvatarGroupManager Instance { get { return _instance; } }


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    #endregion

    [SerializeField] GameObject AvatarParent;
    [SerializeField] GameObject StateMachine;
    [SerializeField] int currentAvatarGroupIndex;
    [SerializeField] int currentMainAvatarIndex;
    [SerializeField] MainAvatar SelectedAvatar = null;

    [SerializeField] List<AvatarGroup> avatarGroups;
    [SerializeField] List<MainAvatar> mainAvatars;
    AvatarGroup _currentAvatarGroup;
    MainAvatar _currentMainAvatar;

    public AvatarGroup CurrentAvatarGroup
    {
        get => CurrentAvatarGroup = _currentAvatarGroup;
        set
        {
            if (_currentAvatarGroup != null)
            {
                _currentAvatarGroup.Ended();
            }
            _currentAvatarGroup = value;
            _currentAvatarGroup.ResetEverything();
        }
    }

    public MainAvatar CurrentMainAvatar
    {
        get => CurrentMainAvatar = _currentMainAvatar;
        set
        {
            if (_currentMainAvatar != null)
            {
                _currentMainAvatar.Ended();
            }
            _currentMainAvatar = value;
            _currentMainAvatar.ResetEverything();
        }
    }

    private void Start()
    {
        populateStateMachine();
        //SetAvatarGroup(0);
    }

    void SetAvatarGroup(int index)
    {
        CurrentAvatarGroup = avatarGroups[index];
        currentAvatarGroupIndex = index;
        SetMainAvatar(CurrentAvatarGroup.FirstChildID);
    }

    void SetAvatarGroupWithoutChangingChild(int index)
    {
        CurrentAvatarGroup = avatarGroups[index];
        currentAvatarGroupIndex = index;
    }

    void SetMainAvatar(int index)
    {
        CurrentMainAvatar = mainAvatars[index];
        currentMainAvatarIndex = index;
    }

    public void SwitchGroup(int index)
    {
        SetAvatarGroup(index);

    }

    public void SwitchAvatar(int index)
    {
        SetMainAvatar(index);
    }

    public void GoNextAvatar()
    {
        int nextIndex = currentMainAvatarIndex + 1;
        if (nextIndex <= CurrentAvatarGroup.FirstChildID + CurrentAvatarGroup.childCount - 1)
        {
            SetMainAvatar(nextIndex);
        }
    }

    public void GoPreviousAvatar()
    {
        int prevIndex = currentMainAvatarIndex - 1;
        if (prevIndex >= CurrentAvatarGroup.FirstChildID)
        {
            SetMainAvatar(prevIndex);
        }
    }

    void populateStateMachine()
    {
        int i = 0;
        int j = 0;
        foreach (Transform x in AvatarParent.transform)
        {
            GameObject GO = new GameObject(x.name);
            GO.transform.SetParent(StateMachine.transform);
            GO.AddComponent<AvatarGroupBase>();
            GO.GetComponent<AvatarGroupBase>().GroupID = i;
            GO.GetComponent<AvatarGroupBase>().GroupGameobject = x.gameObject;
            avatarGroups.Add(GO.GetComponent<AvatarGroup>());
            foreach (Transform y in x)
            {
                GameObject GO2 = new GameObject(y.name);
                GO2.transform.SetParent(GO.transform);
                GO2.AddComponent<MainAvatarBase>();
                GO2.GetComponent<MainAvatarBase>().AvatarID = j;
                GO2.GetComponent<MainAvatarBase>().AvatarGameobject = y.gameObject;
                GO2.GetComponent<MainAvatarBase>().AvatarGroupID = GO.GetComponent<AvatarGroupBase>().GroupID;

                mainAvatars.Add(GO2.GetComponent<MainAvatar>());

                j++;
            }
            i++;
        }
        FillChildInfo();
    }

    void FillChildInfo()
    {
        foreach (var x in avatarGroups)
        {
            x.FirstChildID = x.gameObject.transform.GetChild(0).GetComponent<MainAvatar>().AvatarID;
            x.childCount = x.gameObject.transform.childCount;
        }
    }

    public void SelectAvatar()
    {
        SetCurrentAvatar();
        FirebaseManager.Instance.SaveAvatarID(CurrentMainAvatar.AvatarID);
    }


    public void SetCurrentAvatar()
    {
        SelectedAvatar = CurrentMainAvatar;
        PlayerInfoDontDestroy.Instance.SelectedAvatarName = SelectedAvatar.AvatarGameobject.name;
        UIManager.Instance.SetYourAvatarName(SelectedAvatar.AvatarGameobject.name);
        SwitchAvatar(SelectedAvatar.AvatarID);
        SetAvatarGroupWithoutChangingChild(SelectedAvatar.AvatarGroupID);
    }

    public void SetCurrentAvatar(int avatarID)
    {
        var selectedAvatar = mainAvatars.Where(res => res.AvatarID == avatarID).Single();
        SelectedAvatar = selectedAvatar;
        UIManager.Instance.SetYourAvatarName(SelectedAvatar.AvatarGameobject.name);
        PlayerInfoDontDestroy.Instance.SelectedAvatarName = SelectedAvatar.AvatarGameobject.name;
        PlayerInfoDontDestroy.Instance.SetSelectedAvatar(avatarID);
        print("The Selected Avatar is: " + selectedAvatar.AvatarGameobject.name);
        SwitchAvatar(avatarID);
        SetAvatarGroupWithoutChangingChild(selectedAvatar.AvatarGroupID);
    }
}
