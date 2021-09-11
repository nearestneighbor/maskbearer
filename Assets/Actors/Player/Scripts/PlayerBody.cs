using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerBody : MonoBehaviour
{
    public float invincibleTime;
    private bool _invincible;
    public bool Invincible { get; set; }

    private Timer _invincibleTimer;

    private void Start()
    {
        _invincibleTimer = new Timer(invincibleTime);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        var go = collision.gameObject;
        if ((go.layer != LayerMask.NameToLayer("Enemy") &&
            go.layer != LayerMask.NameToLayer("Enemy Attack")) ||
            _invincible ||
            !go.GetComponent<PlayerDamager>())
            return;

        _invincibleTimer.Start();
        var damager = go.GetComponent<PlayerDamager>();
        PlayerData.Instance.StartDrainingHealth(damager.damageAmount);
    }

    private void Update()
    {
        _invincibleTimer.Update();
        _invincible = _invincibleTimer;
    }
}
