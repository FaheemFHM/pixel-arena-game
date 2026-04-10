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
    [SerializeField] private float mapRangeMin = 10f; // max opacity within this dist
    [SerializeField] private float mapRangeMax = 24f; // min opacity beyond this dist

    private void Awake()
    {
        // singleton
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        // dictionary
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
            float dist = Vector3.Distance(playerPos, m.pos);

            float tOpacity = Mathf.InverseLerp(mapRangeMin, mapRangeMax, dist);
            m.img.alpha = Mathf.Lerp(1f, 0f, tOpacity);
        }
    }
}
