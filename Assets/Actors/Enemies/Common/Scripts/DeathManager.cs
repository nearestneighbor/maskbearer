using UnityEngine;

public class DeathManager : MonoBehaviour
{
    public delegate void DeathEvent();
    public event DeathEvent Death;
    protected virtual void OnDeath() => Death?.Invoke();

    public GameObject? corpsePrefab;
    private GameObject _corpseInst;

    public void Die()
    {
        OnDeath();

        if (corpsePrefab)
        {
            _corpseInst = Instantiate(corpsePrefab, transform.position, Quaternion.identity);
            var player = FindObjectOfType<Player>();
            _corpseInst?.FaceGameObject(player?.gameObject);
        }
        Destroy(gameObject);
    }
}
