using UnityEngine;

public class EnemyDamager : MonoBehaviour
{
    public int damageAmount;

    public Collider2D damagerCollider;

    private void Awake()
    {
        if (gameObject.layer != LayerMask.NameToLayer("Player Attack"))
            gameObject.layer = LayerMask.NameToLayer("Player Attack");

        damagerCollider ??= GetComponentInChildren<Collider2D>(true);
        if (!damagerCollider.isTrigger) damagerCollider.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

    }
}
