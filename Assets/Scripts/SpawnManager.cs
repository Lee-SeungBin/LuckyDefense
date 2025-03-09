using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Data;
using Units;
using Utility;

public class SpawnManager : SingletonObject<SpawnManager>
{
    #region Spawn Manager References

    [SerializeField] private EnemyUnit enemyUnitPrefab;
    [SerializeField] private EnemyUnit bossUnitPrefab;
    [SerializeField] private List<SpawnSetting> spawnSettings;

    private int m_BossSpawnCountCache;
    private int m_BossKillDiaCache;
    private EnemyUnitStat m_CacheEnemyUnitStat;
    
    public event OnEnemyUnitCreateDelegate OnEnemyUnitCreateEvent;
    public event OnEnemySpawnEndedDelegate OnEnemySpawnEndedEvent;

    #endregion

    #region MonoBehaviour Events

    private void Start()
    {
        OnEnemyUnitCreateEvent += UI.HUD.Get.IncreaseUnitCount;
        OnEnemySpawnEndedEvent += GameManager.Get.StartNextWave;
    }

    #endregion

    #region Spawn Manager Functions

        public void StartSpawnEnemy(int currentWave)
    {
        m_CacheEnemyUnitStat.health = Mathf.FloorToInt(Database.GlobalBalanceSetting.initWaveUnitHealth *
                                            Mathf.Pow(Database.GlobalBalanceSetting.waveUnitHealthMultiplier,
                                                currentWave));
        m_CacheEnemyUnitStat.speed = Database.GlobalBalanceSetting.waveUnitSpeed;
        StartCoroutine(SpawnEnemy());
    }

    public void StartSpawnBossEnemy(int currentWave)
    {
        m_BossKillDiaCache = 0;
        var bossWaveStep = currentWave / Database.GlobalBalanceSetting.bossUnitHealthDivider;
        m_CacheEnemyUnitStat.health = Mathf.FloorToInt(Database.GlobalBalanceSetting.initBossUnitHealth *
                                                       Mathf.Pow(Database.GlobalBalanceSetting.bossUnitHealthMultiplier,
                                                           bossWaveStep));
        m_CacheEnemyUnitStat.speed = Database.GlobalBalanceSetting.waveUnitSpeed;
        foreach (var spawnSetting in spawnSettings)
        {
            var enemyUnit = Instantiate(bossUnitPrefab, spawnSetting.spawnPoint, Quaternion.identity);
            enemyUnit.transform.SetParent(transform);
            var bossKillDiaStep = currentWave / Database.GlobalBalanceSetting.bossUnitKillDiaDivider;
            var killDia = Mathf.FloorToInt(Database.GlobalBalanceSetting.bossUnitKillDia *
                                           Mathf.Pow(Database.GlobalBalanceSetting.bossUnitKillDiaMultiplier,
                                               bossKillDiaStep));
            enemyUnit.SetEnemy(spawnSetting.enemyTargetPoints, 0, killDia, m_CacheEnemyUnitStat);
            enemyUnit.OnDestroyEvent += DecreaseBossSpawnCountCache;
        }
        m_BossSpawnCountCache = spawnSettings.Count;
        UI.HUD.Get.StartBossWaveTimer();
    }

    private void DecreaseBossSpawnCountCache(EnemyUnit enemyUnit)
    {
        m_BossSpawnCountCache--;
        m_BossKillDiaCache += enemyUnit.KillDia;
        if (m_BossSpawnCountCache <= 0)
        {
            UI.HUD.Get.StopBossWaveTimer();
            UI.HUD.Get.WaveDirectedUI.ShowBossKillText(m_BossKillDiaCache);
            GameManager.Get.GetDia(m_BossKillDiaCache);
            OnEnemySpawnEndedEvent?.Invoke();
        }
    }
    
    private IEnumerator SpawnEnemy()
    {
        for (var i = 0; i < Database.GlobalBalanceSetting.waveUnitSpawnCount; i++)
        {
            foreach (var spawnSetting in spawnSettings)
            {
                var enemyUnit = Instantiate(enemyUnitPrefab, spawnSetting.spawnPoint, Quaternion.identity);
                enemyUnit.transform.SetParent(transform);
                enemyUnit.name = i.ToString();
                var killGold = Mathf.FloorToInt(Database.GlobalBalanceSetting.waveUnitKillGold *
                                                Mathf.Pow(Database.GlobalBalanceSetting.waveUnitKillGoldMultiplier,
                                                    GameManager.Get.CurrentWave));
                enemyUnit.SetEnemy(spawnSetting.enemyTargetPoints, killGold, 0, m_CacheEnemyUnitStat);
                OnEnemyUnitCreateEvent?.Invoke();
                enemyUnit.OnDestroyEvent += UI.HUD.Get.DecreaseUnitCount;
                enemyUnit.OnDestroyEvent += GameManager.Get.GetGold;
            }

            yield return new WaitForSeconds(Database.GlobalBalanceSetting.waveUnitSpawnInterval);
        }
        
        OnEnemySpawnEndedEvent?.Invoke();
    }

    #endregion
    
    [System.Serializable]
    public struct SpawnSetting
    {
        public Vector2 spawnPoint;
        public List<Vector2> enemyTargetPoints;
    }
    
    public delegate void OnEnemyUnitCreateDelegate();

    public delegate void OnEnemySpawnEndedDelegate();
}