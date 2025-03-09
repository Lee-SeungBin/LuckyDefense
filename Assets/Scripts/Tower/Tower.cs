using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Units
{
    public class Tower : MonoBehaviour
    {
        private static readonly int AttackTrigger = Animator.StringToHash("Attack");
        private static readonly int NormalState = Animator.StringToHash("NormalState");
        private static readonly int AttackSpeed = Animator.StringToHash("AttackSpeed");
        private static readonly int Spawn = Animator.StringToHash("Spawn");

        #region Tower References

        [SerializeField] private Button syntheticButton;
        [SerializeField] private Button resellButton;
        [SerializeField] private Button closeButton;
        [SerializeField] private TMPro.TextMeshProUGUI towerUnitCountText;
        [SerializeField] private RectTransform attackRange;
        [SerializeField] private Projectile projectilePrefab;
        [SerializeField] private float dragThreshold = 0.1f;
        [SerializeField] private float sensingInterval = 0.5f;
        [SerializeField] private LayerMask sensingLayerMask;
        [SerializeField] private Animator towerAnimator;
        [SerializeField] private Animator towerSpawnAnimator;
        [SerializeField] private AudioSource attackAudio;
        [SerializeField] private AudioSource spawnAudio;
        
        public TowerGrade TowerGrade { get; private set; }
        public TowerType TowerType { get; private set; }
        public int TowerUnitCount { get; private set; }
        public int GridX { get; private set; }
        public int GridY { get; private set; }
        public string Identifier { get; private set; }

        public TowerStat TowerStat => m_TowerStat;
        
        private StageManager m_StageManager;
        private TowerStat m_TowerStat;
        private bool m_IsDragging;
        private float m_LastAttackTime;
        private float m_SensingIntervalTimer;
        private Transform m_CurrentTargetTransform;
        private Sprite m_ProjectileSprite;
        private Vector3 m_MouseDownPos;
        private readonly Collider2D[] m_SensingEnemyCache = new Collider2D[16];

        #endregion
        
        #region MonoBehaviour Mouse Events

        private void OnMouseDown()
        {
            if (m_StageManager != GameManager.Get.GetStageManager(0))
            {
                return;
            }
            m_MouseDownPos = Input.mousePosition;
            m_IsDragging = false;
            SetActiveUI(false);
        }

        private void OnMouseDrag()
        {
            if (Camera.main == null || m_StageManager != GameManager.Get.GetStageManager(0))
            {
                return;
            }

            if (Vector3.Distance(m_MouseDownPos, Input.mousePosition) > dragThreshold)
            {
                m_IsDragging = true;
                Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                transform.position = new Vector3(mousePos.x, mousePos.y, transform.position.z);
            }
        }

        private void OnMouseUp()
        {
            if (m_StageManager != GameManager.Get.GetStageManager(0))
            {
                return;
            }
            
            if (m_IsDragging)
            {
                m_IsDragging = false;
                SnapToGrid();
            }
            else
            {
                SetActiveUI(true);
            }
        }

        #endregion

        #region MonoBehaviour Events

        private void Update()
        {
            if (m_IsDragging)
            {
                m_LastAttackTime = 0f;
                m_SensingIntervalTimer = 0f;
                return;
            }
            
            m_SensingIntervalTimer += Time.deltaTime;

            if (m_SensingIntervalTimer >= sensingInterval)
            {
                SensingEnemy();
            }

            if (m_CurrentTargetTransform == null)
            {
                return;
            }
            
            m_LastAttackTime += Time.deltaTime;

            if (m_LastAttackTime >= 1 / m_TowerStat.attackSpeed)
            {
                m_LastAttackTime = 0f;
                Attack(m_CurrentTargetTransform);
            }
        }

        #endregion

        #region Tower Functions

        private void SensingEnemy()
        {
            var size = Physics2D.OverlapCircleNonAlloc(transform.position, m_TowerStat.attackRange, m_SensingEnemyCache, sensingLayerMask);

            var closestDistance = Mathf.Infinity;
            Transform closestTarget = null;

            for (var i = 0; i < size; i++)
            {
                if (m_SensingEnemyCache[i] == null)
                {
                    continue;
                }

                var distance = Vector2.Distance(transform.position, m_SensingEnemyCache[i].transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestTarget = m_SensingEnemyCache[i].transform;
                }
            }

            if (m_CurrentTargetTransform != null)
            {
                var distanceToCurrentTarget = Vector2.Distance(transform.position, m_CurrentTargetTransform.position);
                if (distanceToCurrentTarget > m_TowerStat.attackRange)
                {
                    m_CurrentTargetTransform = null;
                }
            }

            if (closestTarget != null)
            {
                m_CurrentTargetTransform = closestTarget;
            }
        }

        private void Attack(Transform target)
        {
            towerAnimator.SetTrigger(AttackTrigger);
            var projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            projectile.SetProjectile(target.transform, TowerType, m_TowerStat.attackDamage, m_ProjectileSprite);
            
            if (m_StageManager != GameManager.Get.GetStageManager(0))
            {
                return;
            }
            SoundManager.Get.PlaySound(attackAudio, TowerType.ToString());
        }
        
        private void SnapToGrid()
        {
            var gridPos = m_StageManager.GetNearestGridPosition(transform.position);
            var newX = (int)gridPos.x;
            var newY = (int)gridPos.y;
    
            var currentX = GridX;
            var currentY = GridY;
    
            if (newX != currentX || newY != currentY)
            {
                m_StageManager.MoveTower(currentX, currentY, newX, newY);
            }
            else
            {
                SetPosition(currentX, currentY);
            }
        }
    
        public void SetPosition(int x, int y)
        {
            GridX = x;
            GridY = y;
            var startPos = m_StageManager.GetStartPosition();
            transform.position = startPos + new Vector2(x, -y);
        }

        public void SetTower(TowerType type, TowerGrade grade, StageManager stageManager)
        {
            m_StageManager = stageManager;
            TowerType = type;
            TowerGrade = grade;
            TowerUnitCount = 1;
            if (m_StageManager == GameManager.Get.GetStageManager(0))
            {
                SoundManager.Get.PlaySound(spawnAudio, "Spawn");
            }
            towerSpawnAnimator.SetTrigger(Spawn);
            foreach (var towerSettingData in Data.Database.TowerSetting.towerSettingDatas.Where(data =>
                         data.towerType == TowerType && data.towerGrade == TowerGrade))
            {
                Identifier = towerSettingData.identifier;
            }

            var targetTowerSettingData =
                Data.Database.TowerSetting.towerSettingDatas.FirstOrDefault(towerSettingData =>
                    towerSettingData.identifier == Identifier);

            if (targetTowerSettingData != null &&
                Data.Database.TowerData.TryGetValue(targetTowerSettingData.identifier, out var towerData))
            {
                m_TowerStat.attackDamage = towerData.attackDamage;
                m_TowerStat.attackRange = towerData.attackRange;
                m_TowerStat.attackSpeed = towerData.attackSpeed;
                m_TowerStat.resellGold = towerData.resellGold;
                m_TowerStat.resellDia = towerData.resellDia;

                m_ProjectileSprite = targetTowerSettingData.projectileSprite;
                attackRange.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, towerData.attackRange * 2);
                attackRange.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, towerData.attackRange * 2);
            }

            if (grade.Equals(TowerGrade.Hero) || grade.Equals(TowerGrade.Legendary) || grade.Equals(TowerGrade.Myth))
            {
                syntheticButton.interactable = false;
            }
            syntheticButton.onClick.AddListener(()=>
            {
                stageManager.TowerUpgrade(this);
            });
            resellButton.onClick.AddListener(() =>
            {
                stageManager.ResellTower(this);
            });
            towerAnimator.SetFloat(AttackSpeed, m_TowerStat.attackSpeed * 2f);
            towerAnimator.SetFloat(NormalState,
                type.Equals(TowerType.RangedMulti) ? 1f : type.Equals(TowerType.RangedSingle) ? 0.5f : 0f);
        }

        public void SetActiveUI(bool isActive)
        {
            resellButton.gameObject.SetActive(isActive);
            syntheticButton.gameObject.SetActive(isActive);
            attackRange.gameObject.SetActive(isActive);
            closeButton.gameObject.SetActive(isActive);
            UI.HUD.Get.SetActiveTowerInformationUI(isActive, this);
        }

        public void UpdateTowerUnitCount(int count)
        {
            TowerUnitCount += count;
            if (Data.Database.TowerData.TryGetValue(Identifier, out var towerData))
            {
                m_TowerStat.attackDamage = towerData.attackDamage * TowerUnitCount;
            }
            
            UpdateTowerUnitCountText();
            if (TowerUnitCount <= 0)
            {
                m_StageManager.RemoveTower(this);
            }
        }

        private void UpdateTowerUnitCountText()
        {
            if (TowerUnitCount <= 1)
            {
                towerUnitCountText.gameObject.SetActive(false);
                return;
            }
            towerSpawnAnimator.SetTrigger(Spawn);
            if (m_StageManager == GameManager.Get.GetStageManager(0))
            {
                SoundManager.Get.PlaySound(spawnAudio, "Spawn");
            }
            towerUnitCountText.gameObject.SetActive(true);
            towerUnitCountText.text = $"x{TowerUnitCount}";
        }

        #endregion
    }
    
    public enum TowerGrade
    {
        Normal,
        Rare,
        Hero,
        Legendary,
        Myth
    }
    
    public enum TowerType
    {
        MeleeSingle,
        MeleeMulti,
        RangedSingle,
        RangedMulti
    }

    [System.Serializable]
    public struct TowerStat
    {
        public int attackDamage;
        public float attackSpeed;
        public float attackRange;
        public int resellGold;
        public int resellDia;
    }
}