
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelect : MonoBehaviour, IMenu
{
    [SerializeField] private RectTransform arrow;
    [SerializeField] private float buttonSeparation;
    private Vector3 arrowStartPos;

    [SerializeField] private Button[] buttons;
    [SerializeField] private int selectedIndex;

    private void Awake()
    {
        arrowStartPos = arrow.anchoredPosition;
        SelectButton(0);
    }

    public void OnButtonSelected(GameObject buttonObj, bool isPointer)
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i].gameObject == buttonObj)
            {
                SelectButton(i);
                return;
            }
        }
    }

    public void SelectButton(int newIndex)
    {
        selectedIndex = newIndex;

        arrow.anchoredPosition = arrowStartPos + (Vector3.right * buttonSeparation * newIndex);
    }
}
