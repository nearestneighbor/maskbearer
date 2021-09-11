using UnityEngine;

public static class GameObjectExtensions
{
    public static void FaceGameObject(this GameObject gameObject, GameObject otherGameObject)
    {
        var localScale = gameObject.transform.localScale;
        gameObject.transform.localScale = new Vector3(
            Mathf.Sign(otherGameObject.transform.position.x - gameObject.transform.position.x),
            localScale.y,
            localScale.z);
    }
}
