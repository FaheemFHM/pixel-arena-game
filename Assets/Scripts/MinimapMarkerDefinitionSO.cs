using UnityEngine;

[CreateAssetMenu(menuName = "Minimap Marker Definition")]
public class MinimapMarkerDefinitionSO : ScriptableObject
{
    public MinimapMarkerType type;
    public Sprite sprite;
}
