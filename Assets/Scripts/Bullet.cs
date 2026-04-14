using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int level;

    private SpriteRenderer rend;

    private void Start()
    {
        rend = GetComponent<SpriteRenderer>();

        //ApplyShadow();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // get level of tilemap
        Level tilemapLevel = other.GetComponentInParent<Level>();
        if (tilemapLevel == null) return;

        // destroy bullet on collision
        if (tilemapLevel.level >= level)
        {
            Destroy(gameObject);
            enabled = false;
            return;
        }

/*        // no need to consider triggers too far down
        if (tilemapLevel.level < level - 1) return;

        // check if the shadow is needed and apply it
        ApplyShadow();*/
    }

    void ApplyShadow()
    {
        bool isFree = LevelManager.instance.GetTileType(transform.position, level - 1) == TileType.None;
        rend.color = isFree ? Color.white : Color.black;
    }
}
