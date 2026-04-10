using UnityEngine;

public class MinimapMarker : MonoBehaviour
{
    public MinimapMarkerDefinitionSO def;
    public Vector2 pos;
    public RectTransform rt;
    public CanvasGroup img;

    private void Awake()
    {
        rt= GetComponent<RectTransform>();
        img= GetComponent<CanvasGroup>();
    }
}
