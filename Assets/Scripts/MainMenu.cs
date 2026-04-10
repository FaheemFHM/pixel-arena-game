using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour, IMenu
{
    [SerializeField] private RectTransform arrow;
    [SerializeField] private float buttonSeparation;
    private Vector3 arrowStartPos;

    [SerializeField] private Button[] buttons;
    private int selectedIndex;

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

        arrow.anchoredPosition = arrowStartPos - (Vector3.up * buttonSeparation * newIndex);

        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].transform.GetChild(1).gameObject.SetActive(i == selectedIndex);
        }
    }
}
