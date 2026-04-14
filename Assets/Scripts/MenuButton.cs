using UnityEngine;
using UnityEngine.EventSystems;

public class MenuButton : MonoBehaviour, IPointerEnterHandler, ISelectHandler
{
    private IMenu menu;

    private void Awake()
    {
        menu = GetComponentInParent<IMenu>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        menu.OnButtonSelected(gameObject, true);
        EventSystem.current.SetSelectedGameObject(gameObject);
    }

    public void OnSelect(BaseEventData eventData)
    {
        menu.OnButtonSelected(gameObject, false);
    }
}
