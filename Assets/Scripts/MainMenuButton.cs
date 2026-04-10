using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHover : MonoBehaviour, IPointerEnterHandler, ISelectHandler
{
    [SerializeField] private MainMenu menu;

    public void OnPointerEnter(PointerEventData eventData)
    {
        menu.OnButtonSelected(gameObject);
        EventSystem.current.SetSelectedGameObject(gameObject);
    }

    public void OnSelect(BaseEventData eventData)
    {
        menu.OnButtonSelected(gameObject);
    }
}
