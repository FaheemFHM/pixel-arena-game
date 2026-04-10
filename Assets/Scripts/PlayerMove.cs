using UnityEngine;
using System.Collections;

public class PlayerMove : MonoBehaviour
{
    private InputManager inputs;
    private Rigidbody2D rb;
    private StatsManager stats;

    [Header("Movement")]
    [SerializeField, Range(0f, 10f)] private float moveSpeed = 4f;
    [SerializeField, Range(0f, 10f)] private float sprintSpeed = 6f;
    [SerializeField, Range(0f, 1f)] private float verticalMoveDamping = 0.9f;
    [SerializeField] private Transform spriteTransform;
    private SpriteRenderer rend;
    private Vector2 moveInput;
    private Vector2 moveNorm;
    private Vector2 aimInput;
    private Vector2 aimNorm;

    private Vector2 myInput;
    private Vector2 myNorm = Vector2.right;

    private float INPUT_THRESHOLD = 0.01f;

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

    [SerializeField] private int level = 0;

    private TileType prevTile;
    private TileType prevTileLower;
    private TileType curTile;
    private TileType curTileLower;

    private bool isRamping = false;
    private bool prevRamping = false;

    private void Start()
    {
        // components
        inputs = GetComponent<InputManager>();
        rb = GetComponent<Rigidbody2D>();
        stats = GetComponent<StatsManager>();

        // subscriptions
        inputs.onSwitch += NextGun;
        inputs.onPrimary += PressFire;

        // gun
        gunTransform = gunHolder.GetChild(0);
        gunOffset = Vector2.up * gunPivot.localPosition.y;

        rend = spriteTransform.GetComponent<SpriteRenderer>();
        gunRend = gunTransform.GetComponentInChildren<SpriteRenderer>();
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

        HandleRamps();
    }

    private void FixedUpdate()
    {
        ApplyMove();
    }

    void HandleRamps()
    {
        prevTile = curTile;
        prevTileLower = curTileLower;

        curTile = LevelManager.instance.GetTileType(rb.position, level);
        curTileLower = LevelManager.instance.GetTileType(rb.position, level - 1);

        prevRamping = isRamping;
        isRamping = (curTile == TileType.Ramp || curTileLower == TileType.Ramp);

        // entering ramp
        if (!prevRamping && isRamping)
        {
            rend.color = Color.red;
        }
        // exiting ramp
        else if (prevRamping && !isRamping)
        {
            rend.color = Color.white;

            // update level
            if (prevTile == TileType.Ramp && moveNorm.y > 0f) level++;
            else if (prevTileLower == TileType.Ramp && moveNorm.y < 0f) level--;
        }
    }

    void ApplyMove()
    {
        // dont bother making any tilemap calls if no input
        if (moveInput.magnitude < INPUT_THRESHOLD) return;

        // get values
        Vector2 delta = GetMoveDelta();
        Vector3 currentPos = rb.position;
        Vector3 newPos = currentPos;

        // attempt horizontal movement
        Vector3 xCheck = currentPos + new Vector3(delta.x, 0, 0);
        if (CanMoveTo(xCheck)) newPos.x += delta.x;

        // attempt vertical movement
        Vector3 yCheck = currentPos + new Vector3(0, delta.y, 0);
        if (CanMoveTo(yCheck)) newPos.y += delta.y;

        // apply movement vector
        rb.MovePosition(newPos);
    }

    Vector2 GetMoveDelta()
    {
        // gives the Vector2 representing unbounded movement
        Vector2 inp = new Vector2(moveNorm.x, moveNorm.y * verticalMoveDamping);
        float speed = inputs.IsSprinting ? sprintSpeed : moveSpeed;
        Vector2 delta = inp * speed * Time.fixedDeltaTime;
        return delta;
    }

    private bool CanMoveTo(Vector3 pos)
    {
        // get tile type on my level
        TileType tType = LevelManager.instance.GetTileType(pos, level);

        // dont walk into cliffs
        if (tType == TileType.Cliff) return false;

        // you can walk down along ramp
        if (curTile == TileType.Ramp) return true;
        if (curTileLower == TileType.Ramp) return true;

        // dont walk into the sides of floors
        if (tType == TileType.Floor) return false;

        // you cant fall off ground level
        if (level < 1) return true;

        // get tile type on lower level
        tType = LevelManager.instance.GetTileType(pos, level - 1);

        // dont walk off cliffs
        if (tType == TileType.None) return false;
        if (tType == TileType.Cliff) return false;

        // by default you are free to walk
        return true;
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
        myInput = aimInput.magnitude < INPUT_THRESHOLD ? moveInput : aimInput;
        if (myInput.sqrMagnitude > INPUT_THRESHOLD) myNorm = myInput.normalized;

        if (myNorm.x < -INPUT_THRESHOLD) dir = -1;
        else if (myNorm.x > INPUT_THRESHOLD) dir = 1;
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
        b.GetComponent<Bullet>().level = level;

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
