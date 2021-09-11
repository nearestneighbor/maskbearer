using System.Collections;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(DeathManager))]
[RequireComponent(typeof(SpriteFlash))]
public class HealthManager : MonoBehaviour
{
    public delegate void DamageTaken();
    public event DamageTaken TookDamage;
    protected virtual void OnDamageTaken() => TookDamage?.Invoke();

    public int hp = 100;
    public bool invincible;
    public float damageCooldown = 0.15f;
    private bool _onDamageCooldown;

    public bool IsDead { get => hp <= 0; }

    public Collider2D damageCollider;
    private DeathManager _deathManager;
    public SpriteFlash spriteFlash;

    private void Awake()
    {
        damageCollider ??= GetComponentsInChildren<Collider2D>(true).First(collider => collider.isTrigger = true);
        damageCollider ??= GetComponentInChildren<Collider2D>(true);
        damageCollider.isTrigger = true;
        _deathManager ??= GetComponentInChildren<DeathManager>(true);
        spriteFlash ??= GetComponentInChildren<SpriteFlash>(true);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (_onDamageCooldown) return;

        if (collision.gameObject.layer == LayerMask.NameToLayer("Player Attack"))
        {
            _onDamageCooldown = true;
            StartCoroutine(DamageCooldown());
            TakeDamage(collision.GetComponentInChildren<EnemyDamager>().damageAmount);
        }
    }

    private IEnumerator DamageCooldown()
    {
        yield return new WaitForSeconds(damageCooldown);

        _onDamageCooldown = false;
    }

    public void TakeDamage(int damageAmount)
    {
        OnDamageTaken();
        spriteFlash.Flash(0.25f, 0.15f);
        hp -= damageAmount;
        if (hp <= 0)
            _deathManager.Die();
    }


}
