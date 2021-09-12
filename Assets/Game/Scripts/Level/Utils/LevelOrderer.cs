using UnityEngine;

[ExecuteAlways]
public class LevelOrderer : MonoBehaviour
{
    private void Update()
    {
        for (var i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            var childPos = child.transform.localPosition;
            childPos.z = i;
            child.transform.localPosition = childPos;
        }
    }
}
