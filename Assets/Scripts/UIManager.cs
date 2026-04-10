using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    // singleton
    public static UIManager instance { get; private set; }

    // health, stamina, ammo
    [System.Serializable]
    public struct StatBarEntry
    {
        public StatType type;
        public Slider slider;
    }

    [SerializeField] private List<StatBarEntry> statBars;
    private Dictionary<StatType, Slider> bars;

    // minimap player
    [SerializeField] private RectTransform minimapPlayer;

    // minimap markers
    public List<MinimapMarker> mapMarkers;
    [SerializeField] private float minimapRange = 40f;

    [SerializeField] private float mapRangeMin_opacity = 16f; // max opacity within this dist
    [SerializeField] private float mapRangeMax_opacity = 32f; // min opacity beyond this dist

    [SerializeField] private float mapRangeMax_distance = 16f; // within minimap circumference if within this dist

    private void Awake()
    {
        // singleton
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        // others
        CreateStatsDict();
    }

    public void SetMaxValue(StatType type, float value) => bars[type].maxValue = value;
    public void SetValue(StatType type, float value) => bars[type].value = value;

    void CreateStatsDict()
    {
        bars = new Dictionary<StatType, Slider>();
        foreach (var entry in statBars) bars[entry.type] = entry.slider;
    }

    public void SetPlayerRot(float rot) => minimapPlayer.rotation = Quaternion.Euler(0, 0, rot);

    public void UpdateMapMarkers(Vector3 playerPos)
    {
        foreach (MinimapMarker m in mapMarkers)
        {
            // position
            Vector2 offset = m.pos - (Vector2)playerPos;
            float dist = offset.magnitude;
            Vector2 dir = offset.normalized;

            float t = Mathf.Clamp01(dist / mapRangeMax_distance);
            float radius = t * minimapRange;

            m.rt.anchoredPosition = dir * radius;

            // opacity
            float tOpacity = Mathf.InverseLerp(mapRangeMin_opacity, mapRangeMax_opacity, dist);
            m.img.alpha = Mathf.Lerp(1f, 0f, tOpacity);
        }
    }
}
