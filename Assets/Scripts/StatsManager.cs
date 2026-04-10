using UnityEngine;
using System.Collections.Generic;

public class StatsManager : MonoBehaviour
{
    [System.Serializable]
    public class Stat
    {
        public float maxVal;
        public float rechargeDelay;
        public float rechargeDur;
        [HideInInspector] public float rechargeRate;
        [HideInInspector] public float timer;
        [HideInInspector] public float val;
    }

    [System.Serializable]
    public struct StatEntry
    {
        public StatType type;
        public Stat stat;
    }

    private Dictionary<StatType, Stat> stats;
    [SerializeField] private List<StatEntry> statEntries;

    private void Awake()
    {
        CreateStatsDict();
    }

    private void Start()
    {
        // fill all ui sliders
        foreach (var kvp in stats)
        {
            StatType sType = kvp.Key;
            Stat stat = kvp.Value;

            UIManager.instance.SetMaxValue(sType, stat.maxVal);
            UIManager.instance.SetValue(sType, stat.val);
        }
    }

    private void Update()
    {
        foreach (var kvp in stats)
        {
            HandleRecharge(kvp.Key, kvp.Value);
        }
    }

    void HandleRecharge(StatType type, Stat s)
    {
        // increment timer
        s.timer += Time.deltaTime;

        // exit early
        if (s.timer < s.rechargeDelay) return;
        if (s.val >= s.maxVal) return;

        // calculate recharge amount
        float amount = s.rechargeRate * Time.deltaTime;

        // apply changes
        EditStat(type, amount, false);
    }

    public void EditStat(StatType sType, float amount, bool resetTimer = true)
    {
        // get stat
        Stat s = stats[sType];

        // update timer
        if (resetTimer) s.timer = 0f;

        // update value
        s.val += amount;

        // clamp value
        if (s.val < 0)
        {
            s.val = s.maxVal;
        }
        else if (s.val > s.maxVal)
        {
            s.val = s.maxVal;
        }

        // ui update
        UIManager.instance.SetValue(sType, s.val);
    }

    public bool HasStat(StatType sType, float cost)
    {
        return stats[sType].val >= cost;
    }


    void CreateStatsDict()
    {
        stats = new Dictionary<StatType, Stat>();

        foreach (var entry in statEntries)
        {
            // get the stat type
            StatType sType = entry.type;

            // store the stat object in the dict with key = type
            stats[sType] = entry.stat;

            // calculate some values
            stats[sType].val = stats[sType].maxVal;
            stats[sType].rechargeRate = stats[sType].maxVal / stats[sType].rechargeDur;
        }
    }
}
