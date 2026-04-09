using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;

public class PlayerMove : MonoBehaviour
{
    private InputManager inputs;
    private Rigidbody2D rb;

    [Header("Movement")]
    [SerializeField, Range(0f, 10f)] private float moveSpeed = 4f;
    [SerializeField, Range(0f, 10f)] private float sprintSpeed = 6f;
    [SerializeField, Range(0f, 1f)] private float verticalMoveDamping = 0.9f;
    [SerializeField] private Transform spriteTransform;
    private Vector2 moveInput;
    private Vector2 moveNorm;
    private Vector2 aimInput;
    private Vector2 aimNorm;

    private Vector2 myInput;
    private Vector2 myNorm = Vector2.right;

    private int dir = 1;

    [Header("Shooting")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private Transform gunPivot; // does the spin effect
    [SerializeField] private Transform gunHolder; // does the aiming rotation
    private Transform gunTransform; // does the recoil translation
    private Vector2 gunOffset;

    [SerializeField] private GunSO[] guns;
    private SpriteRenderer gunRend;
    private int gunsIndex = 0;
    private GunSO gun;

    private bool isBursting;
    private Vector3 recoilOffset;

    [SerializeField] private Transform bulletFolder;

    private float fireCooldown;
    private bool hasFired;

    [Header("Crosshair")]
    [SerializeField, Range(0f, 360f)] private float gunSpinSpeed = 135f;
    [SerializeField] private bool gunSpinClockwise = true;
    [SerializeField] private Transform crosshair;
    [SerializeField, Range(0f, 2f)] private float crosshairDistMin = 0.5f;
    [SerializeField, Range(1f, 10f)] private float crosshairDistMax = 3f;
    private SpriteRenderer crossRend;

    [SerializeField] private int level;

    private void Start()
    {
        // components
        inputs = GetComponent<InputManager>();
        rb = GetComponent<Rigidbody2D>();

        // subscriptions
        inputs.onSwitch += NextGun;
        inputs.onPrimary += PressFire;

        // gun
        gunTransform = gunHolder.GetChild(0);
        gunRend = gunTransform.GetComponentInChildren<SpriteRenderer>();
        gunOffset = Vector2.up * gunPivot.localPosition.y;

        crossRend = crosshair.GetComponent<SpriteRenderer>();

        gunsIndex = guns.Length - 1;
        NextGun();
    }

    private void Update()
    {
        ReadInputs();
        FindPlayerDirection();
        FlipPlayer();
        AimGun();
        UpdateCrosshair();
        HandleShooting();
        ReturnRecoil();

        Vector3Int gridPos = LevelManager.instance.GetGridPos(transform.position);
        TileData? t = LevelManager.instance.GetTile(gridPos, level + 1);
        Debug.Log(t.HasValue ? t.Value.tType.ToString() : "No tile");
    }

    private void FixedUpdate()
    {
        ApplyMove();
    }

    void ApplyMove()
    {
        float speed = inputs.IsSprinting ? sprintSpeed : moveSpeed;
        Vector2 vel = new Vector2(moveNorm.x, moveNorm.y * verticalMoveDamping);
        rb.linearVelocity = vel * speed;
    }

    void ReadInputs()
    {
        // get inputs
        moveInput = inputs.MoveInput;
        aimInput = inputs.AimInput;

        // get normalised inputs
        moveNorm = moveInput.normalized;
        aimNorm = aimInput.normalized;
    }

    void FindPlayerDirection()
    {
        myInput = aimInput.magnitude < 0.01f ? moveInput : aimInput;
        if (myInput.sqrMagnitude > 0.01f) myNorm = myInput.normalized;

        if (myNorm.x < -0.01f) dir = -1;
        else if (myNorm.x > 0.01f) dir = 1;
    }

    void FlipPlayer()
    {
        spriteTransform.localScale = new Vector3(dir, 1, 1);
    }

    void AimGun()
    {
        // spin gun
        int spinDir = (gunSpinClockwise ? -1 : 1);
        gunPivot.Rotate(0, 0, gunSpinSpeed * spinDir * Time.deltaTime);

        // aim gun
        float angle = Mathf.Atan2(myNorm.y, myNorm.x) * Mathf.Rad2Deg;
        gunHolder.rotation = Quaternion.Euler(0, 0, angle);

        // flip gun
        gunHolder.localScale = new Vector3(1, dir, 1);
    }

    void UpdateCrosshair()
    {
        // crosshair position
        float dist = myInput.magnitude * crosshairDistMax;
        crosshair.localPosition = (myNorm * dist) + gunOffset;

        // crosshair values
        float strength = Mathf.Clamp01(myInput.magnitude);
        float cutoff = crosshairDistMin / crosshairDistMax;
        float t = Mathf.InverseLerp(0f, cutoff, strength);
        
        // crosshair opacity
        Color c = crossRend.color;
        c.a = t;
        crossRend.color = c;

        // crosshair scale
        crosshair.localScale = Vector3.one * t;
    }

    void HandleShooting()
    {
        fireCooldown -= Time.deltaTime;

        if (!inputs.IsPrimary) return;

        if (fireCooldown < 0f && !isBursting)
        {
            if ((!gun.isAuto && !hasFired) || gun.isAuto)
            {
                StartCoroutine(FireBurst());
                hasFired = true;
            }
        }
    }

    void ReturnRecoil()
    {
        recoilOffset = Vector3.Lerp(recoilOffset, Vector3.zero, gun.recoilReturnSpeed * Time.deltaTime);
        if (recoilOffset.magnitude < 0.001f) recoilOffset = Vector3.zero;
        gunTransform.localPosition = recoilOffset;
    }

    IEnumerator FireBurst()
    {
        isBursting = true;

        for (int b = 0; b < gun.burstCount; b++)
        {
            float totalSpread = gun.spreadAngle * (gun.spreadCount - 1);

            for (int i = 0; i < gun.spreadCount; i++)
            {
                float shotAngle = -totalSpread / 2f + gun.spreadAngle * i;
                Shoot(shotAngle);
            }

            if (b < gun.burstCount - 1) yield return new WaitForSeconds(gun.burstRate);
        }

        fireCooldown = gun.fireRate;
        isBursting = false;
    }

    void Shoot(float angle)
    {
        // gun sprite faces right but the bullet one faces right

        Quaternion rot = gunHolder.rotation * Quaternion.Euler(0, 0, angle - 90f);
        GameObject b = Instantiate(gun.bulletPrefab, firePoint.position, rot, bulletFolder);

        b.GetComponent<Rigidbody2D>().linearVelocity = rot * Vector2.up * gun.bulletSpeed;

        Vector3 recoilDirLocal = gunTransform.InverseTransformDirection(-gunHolder.right);
        recoilOffset += recoilDirLocal * gun.recoilDistance;

        Destroy(b, 3f);
    }

    void NextGun(bool isPressing = true)
    {
        if (!isPressing) return;

        gunsIndex = (gunsIndex + 1 + guns.Length) % guns.Length;
        SetGun(guns[gunsIndex]);
    }

    void SetGun(GunSO newGun)
    {
        gun = newGun;
        gunRend.sprite = gun.sprite;
        firePoint.localPosition = gun.firePoint;
        crossRend.sprite = gun.crosshair;
    }

    void PressFire(bool isPressing)
    {
        if (!isPressing) hasFired = false;
    }
}
