using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance { get; private set; }

    [SerializeField] private Slider healthBar;
    [SerializeField] private Slider staminaBar;
    [SerializeField] private Slider ammoBar;

    private void Awake()
    {
        // singleton
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    public void SetMaxValues(float h, float s, float a)
    {
        healthBar.maxValue = h;
        staminaBar.maxValue = s;
        ammoBar.maxValue = a;
    }

    public void SetHealth(float x) => healthBar.value = x;
    public void SetStamina(float x) => staminaBar.value = x;
    public void SetAmmo(float x) => ammoBar.value = x;
}
