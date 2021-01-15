using UnityEngine;
using UnityEngine.EventSystems;

public class TriggerButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        print("OVER BUTTON ENTER");
        DomaManager.Instance.IsPointerOverUIButton = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        print("OVER BUTTON EXIT");
        DomaManager.Instance.IsPointerOverUIButton = false;
    }
}