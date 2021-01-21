using System;
using System.Linq;
using TMPro;
using UnityEngine;

public class FloorsDropdown : MonoBehaviour
{
    [Header("Current Status Doma")]
    public CurrentStatusDoma currentStatusDoma;

    TMP_Dropdown dropDown;
    DomaManager domaManager;

    void Start()
    {
        domaManager = DomaManager.Instance;
        dropDown = GetComponent<TMP_Dropdown>();

        dropDown.ClearOptions();
        dropDown.AddOptions(currentStatusDoma.appSystem.Floors.Select(x => x.Name).ToList());

        dropDown.onValueChanged.AddListener(delegate
        {
            dropDownValueChangedHandler(dropDown);
        });
    }

    void Update()
    {  
    }

    void Destroy()
    {
        dropDown.onValueChanged.RemoveAllListeners();
    }

    private void dropDownValueChangedHandler(TMP_Dropdown target)
    {
        domaManager.SetActiveFloorById(target.value);
        currentStatusDoma.ShowWalls2dOnFloors(new Guid[] { currentStatusDoma.activeFloor.Id });
    }

    public void SetDropdownIndex(int index)
    {
        dropDown.value = index;
    }
}
