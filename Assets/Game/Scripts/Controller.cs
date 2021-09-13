using UnityEngine;


public class Controller : MonoBehaviour
{
    const float skinWidth = .015f;

    public LayerMask collisionMask;
    public int horizontalRayCount = 4;
    public int verticalRayCount = 4;

    float horizontalRaySpacing;
    float verticalRaySpacing;

    RaycastOrigins raycastOrigins;

    [SerializeField]
    public CollisionInfo collisions;

    [SerializeField]
    new BoxCollider2D collider;

    RaycastHit2D[] verticalHits, horizontalHits;
    void Awake(){
        CalculateRaySpacing();
        verticalHits = new RaycastHit2D[verticalRayCount];
        horizontalHits = new RaycastHit2D[horizontalRayCount];
    }

    
    void VerticalCollisions(ref Vector2 delta){
        float directionY = Mathf.Sign(delta.y);
        float rayLength = Mathf.Abs(delta.y) + skinWidth;
        for (int i = 0; i < verticalRayCount; i++){
            Vector2 rayOrigin = (directionY == -1)? raycastOrigins.bottomLeft: raycastOrigins.topLeft;
            rayOrigin += Vector2.right * (verticalRaySpacing * i + delta.x);
            verticalHits[i] = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);
            Debug.DrawRay(rayOrigin, Vector2.up * directionY * 10 * rayLength, Color.red);
        }

        collisions.below = collisions.above = collisions.leftEdge = false;
        for (int i = 0; i < verticalRayCount; i++){
            if (verticalHits[i]){
                delta.y = (verticalHits[i].distance - skinWidth) * directionY;
                collisions.below |= directionY == -1;
                collisions.above |= directionY == 1;

                if (i > 0 && !verticalHits[i-1]) {
                    collisions.leftEdge = true;
                }

                if (i < verticalRayCount - 1 && !verticalHits[i+1]){
                    collisions.rightEdge = true;
                }
            }
        }
    }

    void HorizontalCollisions(ref Vector2 delta){
        float directionX = Mathf.Sign(delta.x);
        float rayLength = Mathf.Abs(delta.x) + skinWidth;
        for (int i = 0; i < horizontalRayCount; i++){
            Vector2 rayOrigin = (directionX == -1)? raycastOrigins.bottomLeft: raycastOrigins.bottomRight;
            rayOrigin += Vector2.up * (horizontalRaySpacing * i);
            horizontalHits[i] = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);
            Debug.DrawRay(rayOrigin, Vector2.right * directionX * 10 *  rayLength, Color.red);
        }

        collisions.left = collisions.right = false;
        for (int i = 0; i < horizontalRayCount; i++){
            if (horizontalHits[i]){
                delta.x = (horizontalHits[i].distance - skinWidth) * directionX;
                collisions.left |= directionX == -1;
                collisions.right |= directionX == 1;
            }
        }
    }

    public void Move(Vector2 delta){
        UpdateRaycastOrigins();
        collisions.Reset();
        
        if (delta.x != 0){
            HorizontalCollisions(ref delta);
        }
        if (delta.y != 0){
            VerticalCollisions(ref delta);
        }
        transform.Translate(delta);
        Physics2D.SyncTransforms();
    }

    void UpdateRaycastOrigins(){
        Bounds bounds = collider.bounds;
        bounds.Expand(skinWidth * -2);

        raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        raycastOrigins.topright = new Vector2(bounds.max.x, bounds.max.y);
    }


    void CalculateRaySpacing(){
        Bounds bounds = collider.bounds;
        bounds.Expand(skinWidth * -2);
        horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);
        verticalRayCount = Mathf.Clamp(verticalRayCount, 2, int.MaxValue);

        horizontalRaySpacing = (bounds.size.y) / (horizontalRayCount - 1);
        verticalRaySpacing = (bounds.size.x) / (verticalRayCount - 1);
    }

    
    public struct RaycastOrigins{
        public Vector2 topLeft, topright;
        public Vector2 bottomLeft, bottomRight;
    }

    [System.Serializable]
    public struct CollisionInfo{
        [SerializeField]
        public bool above, below;
        [SerializeField]
        public bool left, right;

        [SerializeField]
        public bool leftEdge, rightEdge;

        public void Reset(){
            above = below = false;
            left = right = false;
            leftEdge = rightEdge = false;
        }
    }

}
