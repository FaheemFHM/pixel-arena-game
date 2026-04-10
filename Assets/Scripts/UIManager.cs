using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    [System.Serializable]
    public struct StatBarEntry
    {
        public StatType type;
        public Slider slider;
    }

    public static UIManager instance { get; private set; }

    [SerializeField] private List<StatBarEntry> statBars;

    private Dictionary<StatType, Slider> bars;

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

        foreach (var entry in statBars)
        {
            bars[entry.type] = entry.slider;
        }
    }
}
