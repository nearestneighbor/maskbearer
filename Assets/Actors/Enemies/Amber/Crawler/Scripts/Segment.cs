using PathCreation;
using UnityEngine;

// Moves along a path at constant speed.
// Depending on the end of path instruction, will either loop, reverse, or stop at the end of the path.
public class Segment : MonoBehaviour
{
    public PathCreator pathCreator;
    public EndOfPathInstruction endOfPathInstruction;
    public float speed = 5;
    public float offset;
    private float _distanceTravelled;

    private void Start()
    {
        _distanceTravelled += offset;
        if (pathCreator != null)
        {
            // Subscribed to the pathUpdated event so that we're notified if the path changes during the game
            pathCreator.pathUpdated += OnPathChanged;
        }
    }

    private void Update()
    {
        if (pathCreator != null)
        {
            _distanceTravelled += speed * Time.deltaTime;
            transform.position = pathCreator.path.GetPointAtDistance(_distanceTravelled, endOfPathInstruction);
            var nextPoint = pathCreator.path.GetPointAtDistance(_distanceTravelled + 0.1f, endOfPathInstruction);
            Vector3 dir = nextPoint - transform.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    // If the path changes during the game, update the distance travelled so that the follower's position on the new path
    // is as close as possible to its position on the old path
    private void OnPathChanged()
    {
        _distanceTravelled = pathCreator.path.GetClosestDistanceAlongPath(transform.position);
    }
}