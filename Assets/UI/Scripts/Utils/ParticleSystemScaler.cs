using UnityEngine;

[ExecuteAlways]
public class ParticleSystemScaler : MonoBehaviour
{
    void Update()
    {
        var system = GetComponent<ParticleSystem>();
        var systemShape = system.shape;

        systemShape.shapeType = ParticleSystemShapeType.Rectangle;
        systemShape.scale = new Vector3(
            Camera.main.orthographicSize * 2 * Camera.main.aspect,
            Camera.main.orthographicSize * 2,
            1
        );
    }
}
