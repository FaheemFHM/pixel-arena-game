using UnityEngine;
using System.Collections;

public class StatsManager : MonoBehaviour
{
    [SerializeField] private float maxHealth = 10f;
    [SerializeField] private float healthRechargeDelay = 3f;
    [SerializeField] private float healthRechargeDur = 5f;
    private float healthRechargeRate;
    private float health;

    [SerializeField] private float maxStamina = 5f;
    [SerializeField] private float staminaRechargeDelay = 2f;
    [SerializeField] private float staminaRechargeDur = 5f;
    private float staminaRechargeRate;
    private float stamina;

    [SerializeField] private int maxAmmo = 40;
    [SerializeField] private float ammoRechargeDelay = 1f;
    [SerializeField] private float ammoRechargeDur = 5f;
    private float ammoRechargeRate;
    private int ammo;

    private void Start()
    {
        health = maxHealth;
        stamina = maxStamina;
        ammo = maxAmmo;

        healthRechargeRate = maxHealth / healthRechargeDur;
        staminaRechargeRate = maxStamina / staminaRechargeDur;
        ammoRechargeRate = maxAmmo / ammoRechargeDur;

        UIManager.instance.SetMaxValues(maxHealth, maxStamina, maxAmmo);
    }

    public void EditHealth(float toAdd)
    {
        health += toAdd;

        if (health < 0)
        {
            health = maxHealth;
            Debug.Log("Out health");
        }
        else if (health > maxHealth)
        {
            health = maxHealth;
        }

        UIManager.instance.SetHealth(health);
    }

    public void EditStamina(float toAdd)
    {
        stamina += toAdd;

        if (stamina < 0)
        {
            stamina = maxStamina;
            Debug.Log("Out of stamina");
        }
        else if (stamina > maxStamina)
        {
            stamina = maxStamina;
        }

        UIManager.instance.SetStamina(stamina);
    }

    public void EditAmmo(int toAdd)
    {
        ammo += toAdd;

        if (ammo < 0)
        {
            ammo = maxAmmo;
            Debug.Log("Out of ammo");
        }
        else if (ammo > maxAmmo)
        {
            ammo = maxAmmo;
        }

        UIManager.instance.SetAmmo(ammo);
    }

    public bool HasAmmo(int ammoCost)
    {
        return ammo >= ammoCost;
    }
}
