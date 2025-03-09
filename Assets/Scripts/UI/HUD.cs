using System.Collections;
using Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utility;

namespace UI
{
    public class HUD : SingletonObject<HUD>
    {
        #region HUD References

        [SerializeField] private GameObject gameOverUI;
        
        [Header("Top Banner UI")]
        [SerializeField] private TextMeshProUGUI waveCountText;
        [SerializeField] private TextMeshProUGUI elapsedTimeText;
        [SerializeField] private TextMeshProUGUI waveUnitCountText;
        [SerializeField] private TextMeshProUGUI bossWaveTimerText;
        [SerializeField] private Slider waveUnitSlider;
        [SerializeField] private TowerInformationUI towerInformationUI;

        [Space(10), Header("Bottom Banner UI")]
        [SerializeField] private Transform resourcePanel;
        [SerializeField] private TextMeshProUGUI currentGoldText;
        [SerializeField] private TextMeshProUGUI currentDiaText;
        [SerializeField] private TextMeshProUGUI towerUnitCountText;
        [SerializeField] private TextMeshProUGUI spawnRequiredGoldText;
        [SerializeField] private LuckySpawnUI luckySpawnUI;
        
        [Space(10), Header("Directed UI")]
        [SerializeField] private WaveDirectedUI waveDirectedUI;
        [SerializeField] private IncreaseResourceUI increaseResourceUI;
        [SerializeField] private LuckySpawnEffectUI luckySpawnEffectUI;
        
        public WaveDirectedUI WaveDirectedUI => waveDirectedUI;
        
        private float m_RemainTime;
        private float m_CurrentBossWaveTimer;
        private int m_CurrentWaveUnitCount;
        private Coroutine m_BossWaveTimerCoroutine;
        private Coroutine m_WaveUnitTimerCoroutine;

        #endregion

        #region MonoBehaviour Events

        private void Start()
        {
            GameManager.Get.OnChangeWaveEvent += UpdateWaveEventText;
            GameManager.Get.OnChangeWaveEvent += waveDirectedUI.ShowWaveUI;
            GameManager.Get.GetStageManager(0).OnChangeCurrentTowerCountEvent += UpdateTowerUnitText;
            GameManager.Get.GetStageManager(0).OnChangeCurrentRequiredGoldEvent += UpdateSpawnRequiredGoldText;
            GameManager.Get.GetStageManager(0).OnChangeCurrentGoldEvent += UpdateGoldText;
            GameManager.Get.GetStageManager(0).OnChangeCurrentDiaEvent += UpdateDiaText;
            waveUnitSlider.maxValue = Database.GlobalBalanceSetting.waveUnitMaxCount;
            luckySpawnUI.Initialize();
        }

        #endregion

        #region HUD Functions

        private IEnumerator UpdateWaveTimer()
        {
            m_RemainTime = Database.GlobalBalanceSetting.waveTimer;
            while (m_RemainTime > 0)
            {
                m_RemainTime -= 1f;
                var hours = Mathf.FloorToInt(m_RemainTime / 60) % 24;
                var minutes = Mathf.FloorToInt(m_RemainTime) % 60;

                elapsedTimeText.text = $"{hours:00}:{minutes:00}";
                yield return new WaitForSeconds(1f);
            }
        }

        public void StartBossWaveTimer()
        {
            m_CurrentBossWaveTimer = Database.GlobalBalanceSetting.bossWaveTimer;
            bossWaveTimerText.gameObject.SetActive(true);
            m_BossWaveTimerCoroutine = StartCoroutine(UpdateBossWaveTimer());
        }

        public void StopBossWaveTimer()
        {
            if (m_BossWaveTimerCoroutine != null)
            {
                StopCoroutine(m_BossWaveTimerCoroutine);
                m_BossWaveTimerCoroutine = null;
            }
            bossWaveTimerText.gameObject.SetActive(false);
        }
        
        private IEnumerator UpdateBossWaveTimer()
        {
            while (m_CurrentBossWaveTimer > 0)
            {
                m_CurrentBossWaveTimer -= 1f;

                var minutes = Mathf.FloorToInt(m_CurrentBossWaveTimer / 60);
                var seconds = Mathf.FloorToInt(m_CurrentBossWaveTimer % 60);

                bossWaveTimerText.text = $"{minutes:00}:{seconds:00}";
                yield return new WaitForSeconds(1f);
            }
            GameManager.Get.GameOver();
            ShowGameOverScreen();
            bossWaveTimerText.gameObject.SetActive(false);
        }
        
        public void IncreaseUnitCount()
        {
            m_CurrentWaveUnitCount++;
            UpdateWaveUnitText();
        }

        public void DecreaseUnitCount(Units.EnemyUnit enemyUnit)
        {
            if (m_CurrentWaveUnitCount > 0)
            {
                m_CurrentWaveUnitCount--;
                UpdateWaveUnitText();
            }
        }

        private void UpdateWaveEventText()
        {
            if (m_WaveUnitTimerCoroutine != null)
            {
                StopCoroutine(m_WaveUnitTimerCoroutine);
                m_WaveUnitTimerCoroutine = null;
            }
            m_WaveUnitTimerCoroutine = StartCoroutine(UpdateWaveTimer());
            waveCountText.text = $"WAVE {GameManager.Get.CurrentWave}";
        }
        
        private void UpdateWaveUnitText()
        {
            waveUnitCountText.text = $"{m_CurrentWaveUnitCount} / {Database.GlobalBalanceSetting.waveUnitMaxCount}";
            waveUnitSlider.value = m_CurrentWaveUnitCount;
            if (Database.GlobalBalanceSetting.waveUnitMaxCount <= m_CurrentWaveUnitCount)
            {
                GameManager.Get.GameOver();
                ShowGameOverScreen();
            }
        }

        private void UpdateTowerUnitText(int currentTowerUnitCount)
        {
            var towerUnitCountString =
                $"{currentTowerUnitCount} / {Database.GlobalBalanceSetting.playerUnitMaxCount}";
            towerUnitCountText.text = towerUnitCountString;
            luckySpawnUI.UpdateCurrentTowerUnit(towerUnitCountString);
        }

        private void UpdateSpawnRequiredGoldText(int currentSpawnRequiredGold)
        {
            spawnRequiredGoldText.text = $"{currentSpawnRequiredGold}";
        }
        
        private void UpdateGoldText(int currentGold)
        {
            currentGoldText.text = currentGold.ToString();
        }

        private void UpdateDiaText(int currentDia)
        {
            var diaString = currentDia.ToString();
            currentDiaText.text = diaString;
            luckySpawnUI.UpdateCurrentDia(diaString);
        }
        
        public void SetActiveTowerInformationUI(bool active, Units.Tower tower)
        {
            towerInformationUI.gameObject.SetActive(active);
            if (active)
            {
                towerInformationUI.SetTowerInformation(tower);
            }
        }

        public void ShowResourceEffectUI(GameManager.ResourceType resourceType, int amount)
        {
            var effectUI = Instantiate(increaseResourceUI, resourcePanel.position, Quaternion.identity);
            effectUI.transform.SetParent(resourcePanel);
            effectUI.transform.localScale = Vector3.one;
            effectUI.IncreaseResource(resourceType, amount);
        }

        public void ShowLuckySpawnEffectUI(Units.TowerGrade grade, bool result)
        {
            var effectUI = Instantiate(luckySpawnEffectUI, luckySpawnUI.transform.position, Quaternion.identity);
            effectUI.transform.SetParent(luckySpawnUI.transform);
            effectUI.transform.localScale = Vector3.one;
            effectUI.ShowEffect(grade ,result);
        }
        
        private void ShowGameOverScreen()
        {
            gameOverUI.SetActive(true);
        }

        #endregion
    }
}