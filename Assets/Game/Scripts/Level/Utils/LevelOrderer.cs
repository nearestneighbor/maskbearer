using UnityEngine;

[ExecuteAlways]
public class LevelOrderer : MonoBehaviour
{
    private void Update()
    {
        var offset = 0;

        Arrange(transform, ref offset);
    }

    private void Arrange(Transform transform, ref int offset)
    {
        var pos = transform.transform.localPosition;
        pos.z = offset;
        transform.localPosition = pos;

        var inner = 0;
        for (var i = transform.childCount-1; i >=0; i--)
            Arrange(transform.GetChild(i), ref inner);

        offset += 1;
        offset += inner;
    }
}
