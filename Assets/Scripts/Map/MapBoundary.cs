using UnityEngine;

public class MapBoundary : MonoBehaviour
{
    public static MapBoundary Instance;

    BoxCollider2D boundaryCollider;
    Bounds bounds;

    void Awake()
    {
        Instance = this;
        boundaryCollider = GetComponent<BoxCollider2D>();
        bounds = boundaryCollider.bounds;
    }

    public Vector2 ClampPosition(Vector2 position, float padding = 0.5f)
    {
        float x = Mathf.Clamp(position.x, bounds.min.x + padding, bounds.max.x - padding);
        float y = Mathf.Clamp(position.y, bounds.min.y + padding, bounds.max.y - padding);

        return new Vector2(x, y);
    }

    public bool IsInside(Vector2 position, float padding = 0.5f)
    {
        return position.x >= bounds.min.x + padding &&
               position.x <= bounds.max.x - padding &&
               position.y >= bounds.min.y + padding &&
               position.y <= bounds.max.y - padding;
    }
}
