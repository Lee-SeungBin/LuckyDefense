using Units;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    #region Projectile References

    [SerializeField] private SpriteRenderer projectileSprite;
    [SerializeField] private float speed;
    [SerializeField] private float explosionRadius;
    
    private int m_Damage;
    private Transform m_Target;
    
    private readonly Collider2D[] m_HitColliderCache = new Collider2D[8];
    
    #endregion

    #region MonoBehaviour Events

    private void Update()
    {
        if (m_Target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector2 direction = (m_Target.position - transform.position).normalized;
        transform.Translate(direction * speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, m_Target.position) < 0.1f)
        {
            OnHitTarget();
        }
    }

    #endregion

    #region Projectile Functions

    /// <summary>
    /// Projectile initialization
    /// </summary>
    /// <param name="target"> target Transform </param>
    /// <param name="towerType"> parent tower Type </param>
    /// <param name="damage"> parent tower attack damage </param>
    /// <param name="sprite"> projectile sprite </param>
    public void SetProjectile(Transform target, TowerType towerType, int damage, Sprite sprite)
    {
        if (towerType is TowerType.MeleeMulti or TowerType.RangedMulti)
        {
            explosionRadius = 1f;
        }

        projectileSprite.sprite = sprite;
        if (projectileSprite.sprite == null)
        {
            projectileSprite.color = Color.clear;
        }
        m_Target = target;
        m_Damage = damage;
    }

    /// <summary>
    /// Projectile arrives target point
    /// </summary>
    private void OnHitTarget()
    {
        if (explosionRadius > 0f)
        {
            Explode();
        }
        else
        {
            if (m_Target.TryGetComponent(out EnemyUnit enemyUnit))
            {
                enemyUnit.TakeDamage(m_Damage);
            }
        }

        Destroy(gameObject);
    }

    /// <summary>
    /// Projectile multi attack
    /// </summary>
    private void Explode()
    {
        var size = Physics2D.OverlapCircleNonAlloc(transform.position, explosionRadius, m_HitColliderCache);
        for (var i = 0; i < size; i++)
        {
            if (m_HitColliderCache[i] != null && m_HitColliderCache[i].TryGetComponent(out Units.EnemyUnit enemyUnit))
            {
                enemyUnit.TakeDamage(m_Damage);
            }
        }
    }

    #endregion
}