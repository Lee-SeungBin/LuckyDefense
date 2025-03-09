using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Units
{
    public class EnemyUnit : MonoBehaviour
    {
        #region EnemyUnit References
                
        [SerializeField] private UnityEngine.UI.Slider healthBar;
        [SerializeField] private UI.HitDamageUI hitDamageUI;
        [SerializeField] private SpriteRenderer enemySprite;
        [SerializeField] private Color hitColor;
        [SerializeField] private float hitDuration;
        
        private int m_CurrentHealth;
        private int m_CurrentTargetIndex = -1;
        
        public int KillGold { get; private set; }
        public int KillDia { get; private set; }
        
        private EnemyUnitStat m_EnemyUnitStat;
        private Vector2 m_CurrentTargetPosition;
        private List<Vector2> m_TargetPoints = new();
        private Color m_CacheColor;
        
        public event OnDestroyDelegate OnDestroyEvent;

        #endregion
        
        #region MonoBehaviour Events
        
        private void Update()
        {
            MoveToTarget();
        }

        #endregion

        #region EnemyUnit Functions

        /// <summary>
        /// Enemy unit initialization
        /// </summary>
        /// <param name="targetPoints"> target point List </param>
        /// <param name="killGold"> enemy kill get gold amount </param>
        /// <param name="killDia"> enemy kill get dia amount </param>
        /// <param name="enemyUnitStat"> contains hp, speed </param>
        public void SetEnemy(List<Vector2> targetPoints, int killGold, int killDia, EnemyUnitStat enemyUnitStat)
        {
            m_TargetPoints = targetPoints;
            KillGold = killGold;
            KillDia = killDia;
            m_EnemyUnitStat = enemyUnitStat;
            healthBar.maxValue = m_EnemyUnitStat.health;
            m_CurrentHealth = m_EnemyUnitStat.health;
            m_CacheColor = enemySprite.color;
            SetNextTarget();
        }
        
        /// <summary>
        /// Enemy unit move to target point
        /// </summary>
        private void MoveToTarget()
        {
            transform.position = Vector3.MoveTowards(
                transform.position, 
                m_CurrentTargetPosition,
                m_EnemyUnitStat.speed * Time.deltaTime
            );

            if (Vector3.Distance(transform.position, m_CurrentTargetPosition) < 0.1f)
            {
                SetNextTarget();
            }
        }
        
        /// <summary>
        /// Enemy unit set next target point
        /// </summary>
        private void SetNextTarget()
        {
            m_CurrentTargetIndex = (m_CurrentTargetIndex + 1) % m_TargetPoints.Count;
            var nextTarget = m_TargetPoints[m_CurrentTargetIndex];
            m_CurrentTargetPosition = nextTarget;
        }

        /// <summary>
        /// Enemy unit take damage
        /// </summary>
        /// <param name="damage"> enemy unit hp damaged amount </param>
        public void TakeDamage(int damage)
        {
            m_CurrentHealth -= damage;
            healthBar.gameObject.SetActive(true);
            healthBar.value = m_CurrentHealth;
            
            var hitDamageInstance = Instantiate(hitDamageUI, transform.position, Quaternion.identity);
            hitDamageInstance.ShowDamage(damage);

            enemySprite.DOColor(hitColor, hitDuration).OnComplete(() => 
            {
                enemySprite.DOColor(m_CacheColor, hitDuration);
            });
            
            if (m_CurrentHealth <= 0)
            {
                DOTween.Kill(enemySprite);
                OnDestroyEvent?.Invoke(this);
                Destroy(gameObject);
            }
        }

        #endregion

        public delegate void OnDestroyDelegate(EnemyUnit enemyUnit);
    }

    [System.Serializable]
    public struct EnemyUnitStat
    {
        public int health;
        public float speed;
    }
}