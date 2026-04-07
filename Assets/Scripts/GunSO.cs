using UnityEngine;

[CreateAssetMenu(fileName = "NewGun", menuName = "Gun")]
public class GunSO : ScriptableObject
{
    public Sprite sprite;
    public Sprite crosshair;

    public Vector2 firePoint;
    [Range(0.1f, 2f)] public float fireRate = 0.333f;

    public bool isAuto = true;

    [Range(1, 100)] public int ammoCost = 1;

    [Range(1, 5)] public int burstCount = 1;
    [Range(0f, 1f)] public float burstRate = 0.1f;

    [Range(1, 5)] public int spreadCount = 1;
    [Range(0f, 45f)] public float spreadAngle = 0f;

    public GameObject bulletPrefab;
    [Range(1f, 20f)] public float bulletSpeed = 10f;
    [Range(0f, 100f)] public float damage = 1f;
}
