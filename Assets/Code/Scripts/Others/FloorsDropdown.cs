using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FloorsDropdown : MonoBehaviour
{
    TMP_Dropdown dropDown;
    DomaManager domaManager;

    void Start()
    {
        domaManager = DomaManager.Instance;
        dropDown = GetComponent<TMP_Dropdown>();

        dropDown.ClearOptions();
        dropDown.AddOptions(domaManager.DomaFloors.Select(x => x.Name).ToList());

        dropDown.onValueChanged.AddListener(delegate {
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
        domaManager.Show2DObjectsOnTags(new string[] { "Floor_" + target.value });

        Debug.Log("Selected: " + target.value);
    }

    public void SetDropdownIndex(int index)
    {
        dropDown.value = index;
    }
}
