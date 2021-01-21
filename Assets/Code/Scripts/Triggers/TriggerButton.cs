using UnityEngine;
using UnityEngine.EventSystems;

public class TriggerButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        DomaManager.Instance.IsPointerOverUIButton = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        DomaManager.Instance.IsPointerOverUIButton = false;
    }
}