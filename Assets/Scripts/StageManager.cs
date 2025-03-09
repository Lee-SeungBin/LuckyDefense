using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Units;
using Data;
using Random = UnityEngine.Random;

public class StageManager : MonoBehaviour
{
    #region Stage Manager References
    
    [SerializeField] private Vector2 stageSize;
    [SerializeField] private Vector2 startPos;
    [SerializeField] private bool auto;
    [SerializeField] private float autoInterval;
    private Tower[,] m_Towers;
    
    private int m_CurrentTowerCount;
    private int m_CurrentRequiredGold;
    private int m_CurrentGold;
    private int m_CurrentDia;
    
    private float m_ElapsedTime;
    
    public event OnChangeCurrentTowerCount OnChangeCurrentTowerCountEvent;
    public event OnChangeCurrentRequiredGold OnChangeCurrentRequiredGoldEvent;
    public event OnChangeCurrentGold OnChangeCurrentGoldEvent;
    public event OnChangeCurrentDia OnChangeCurrentDiaEvent;

    #endregion

    #region MonoBehaviour Events

    private void Awake()
    {
        InitializeGrid();
    }

    private void Start()
    {
        m_CurrentRequiredGold = Database.GlobalBalanceSetting.initSpawnGold;
        m_CurrentGold = Database.GlobalBalanceSetting.initStartGold;
    }

    private void Update()
    {
        if (!auto)
        {
            return;
        }
        
        m_ElapsedTime += Time.deltaTime;

        if (autoInterval <= m_ElapsedTime)
        {
            m_ElapsedTime = 0f;
            AutoPlay();
        }
    }

    #endregion

    #region Stage Manager Functions

        private void InitializeGrid()
    {
        m_Towers = new Tower[(int)stageSize.x, (int)stageSize.y];
    }

    private void UpdateCurrentTowerCount(int towerCount)
    {
        m_CurrentTowerCount += towerCount;
        OnChangeCurrentTowerCountEvent?.Invoke(m_CurrentTowerCount);
    }

    private void UpdateCurrentRequiredGold()
    {
        m_CurrentRequiredGold += Database.GlobalBalanceSetting.increaseSpawnGold;
        OnChangeCurrentRequiredGoldEvent?.Invoke(m_CurrentRequiredGold);
    }
    
    public void UpdateCurrentGold(int gold)
    {
        m_CurrentGold += gold;
        OnChangeCurrentGoldEvent?.Invoke(m_CurrentGold);
        if (auto || gold <= 0)
        {
            return;
        }
        UI.HUD.Get.ShowResourceEffectUI(GameManager.ResourceType.Gold, gold);
    }

    public void UpdateCurrentDia(int dia)
    {
        m_CurrentDia += dia;
        OnChangeCurrentDiaEvent?.Invoke(m_CurrentDia);
        if (auto || dia <= 0)
        {
            return;
        }
        UI.HUD.Get.ShowResourceEffectUI(GameManager.ResourceType.Dia, dia);
    }
    
    public void SpawnTower(int spawnType)
    {
        if (Database.GlobalBalanceSetting.playerUnitMaxCount <= m_CurrentTowerCount)
        {
            return;
        }
        
        var towerGrade = TowerGrade.Normal;
        var isLucky = false;
        switch (spawnType)
        {
            case 0:
                if (m_CurrentGold < m_CurrentRequiredGold)
                {
                    return;
                }
                var rand = Random.value;
                var normalSpawnProbability = Database.ProbabilitySetting.normalSpawn;
                var rareSpawnProbability = Database.ProbabilitySetting.rareSpawn;
                var heroSpawnProbability = Database.ProbabilitySetting.heroSpawn;

                if (rand < normalSpawnProbability)
                {
                    towerGrade = TowerGrade.Normal;
                }
                else if (rand < normalSpawnProbability + rareSpawnProbability)
                {
                    towerGrade = TowerGrade.Rare;
                }
                else if (rand < normalSpawnProbability + rareSpawnProbability + heroSpawnProbability)
                {
                    towerGrade = TowerGrade.Hero;
                }
                else
                {
                    towerGrade = TowerGrade.Legendary;
                }
                
                UpdateCurrentGold(-m_CurrentRequiredGold);
                UpdateCurrentRequiredGold();
                break;
            case 1:
                if (Database.GlobalBalanceSetting.rareSpawnRequiredDia > m_CurrentDia)
                {
                    return;
                }

                UpdateCurrentDia(-Database.GlobalBalanceSetting.rareSpawnRequiredDia);

                isLucky = Random.value < Database.ProbabilitySetting.rareLuckSpawn;
                UI.HUD.Get.ShowLuckySpawnEffectUI(TowerGrade.Rare, isLucky);

                if (isLucky) 
                {
                    towerGrade = TowerGrade.Rare;
                }
                else 
                {
                    return;
                }
                break;
            case 2:
                if (Database.GlobalBalanceSetting.heroSpawnRequiredDia > m_CurrentDia)
                {
                    return;
                }
                
                UpdateCurrentDia(-Database.GlobalBalanceSetting.heroSpawnRequiredDia);
                
                isLucky = Random.value < Database.ProbabilitySetting.heroLuckSpawn;
                UI.HUD.Get.ShowLuckySpawnEffectUI(TowerGrade.Hero, isLucky);

                if (isLucky) 
                {
                    towerGrade = TowerGrade.Hero;
                }
                else 
                {
                    return;
                }
                break;
            case 3:
                if (Database.GlobalBalanceSetting.legendarySpawnRequiredDia > m_CurrentDia)
                {
                    return;
                }
                
                UpdateCurrentDia(-Database.GlobalBalanceSetting.legendarySpawnRequiredDia);
                
                isLucky = Random.value < Database.ProbabilitySetting.legendaryLuckSpawn;
                UI.HUD.Get.ShowLuckySpawnEffectUI(TowerGrade.Legendary, isLucky);

                if (isLucky) 
                {
                    towerGrade = TowerGrade.Legendary;
                }
                else 
                {
                    return;
                }
                break;
            default:
                break;
        }

        var towerType = GetRandomTowerType();
        var existingTower = FindSameTowerUnderCount(towerType, towerGrade);
        
        if (existingTower == null)
        {
            PlaceTowerInEmptySpot(towerType, towerGrade);
        }
        else
        {
            existingTower.UpdateTowerUnitCount(1);
            UpdateCurrentTowerCount(1);
        }
    }

    public Tower FindSameTower(TowerType towerType, TowerGrade towerGrade)
    {
        for (var y = 0; y < stageSize.y; y++)
        {
            for (var x = 0; x < stageSize.x; x++)
            {
                if (m_Towers[x, y] != null &&
                    m_Towers[x, y].TowerType == towerType &&
                    m_Towers[x, y].TowerGrade == towerGrade)
                {
                    return m_Towers[x, y];
                }
            }
        }

        return null;
    }
    
    private Tower FindSameTowerUnderCount(TowerType towerType, TowerGrade towerGrade)
    {
        for (var y = 0; y < stageSize.y; y++)
        {
            for (var x = 0; x < stageSize.x; x++)
            {
                if (m_Towers[x, y] != null &&
                    m_Towers[x, y].TowerType == towerType &&
                    m_Towers[x, y].TowerGrade == towerGrade &&
                    m_Towers[x, y].TowerUnitCount < 3)
                {
                    return m_Towers[x, y];
                }
            }
        }

        return null;
    }
    
    private void PlaceTowerInEmptySpot(TowerType towerType, TowerGrade towerGrade)
    {
        for (var x = 0; x < stageSize.x; x++)
        {
            for (var y = 0; y < stageSize.y; y++)
            {
                if (m_Towers[x, y] == null)
                {
                    var spawnPos = startPos + new Vector2(x, -y);
                    var targetTower = Database.TowerSetting.towerSettingDatas.FirstOrDefault(tower =>
                        tower.towerType == towerType && tower.towerGrade == towerGrade)
                        ?.tower;
                    
                    var towerInstance = Instantiate(targetTower, spawnPos, Quaternion.identity, null);
                    towerInstance.transform.localScale = Vector3.one;
                    towerInstance.transform.SetParent(transform);
                    towerInstance.SetTower(towerType, towerGrade, this);
                    towerInstance.SetPosition(x, y);
            
                    m_Towers[x, y] = towerInstance;
                    UpdateCurrentTowerCount(1);
                    return;
                }
            }
        }

        Debug.Log("그리드가 가득 찼습니다.");
    }

    public void RemoveTower(Tower targetTower)
    {
        for (var y = 0; y < stageSize.y; y++)
        {
            for (var x = 0; x < stageSize.x; x++)
            {
                if (m_Towers[x, y] != targetTower)
                {
                    continue;
                }
                m_Towers[x, y] = null;
                Destroy(targetTower.gameObject);
                UpdateCurrentTowerCount(-targetTower.TowerUnitCount);
                return;
            }
        }
    }

    public void ResellTower(Tower targetTower)
    {
        targetTower.SetActiveUI(false);
        UpdateCurrentGold(targetTower.TowerStat.resellGold * targetTower.TowerUnitCount);
        UpdateCurrentDia(targetTower.TowerStat.resellDia);
        RemoveTower(targetTower);
    }
    
    private static TowerType GetRandomTowerType()
    {
        var types = (TowerType[])System.Enum.GetValues(typeof(TowerType));
        return types[Random.Range(0, types.Length)];
    }

    public void MoveTower(int fromX, int fromY, int toX, int toY)
    {
        if (toX < 0 || toX >= stageSize.x || toY < 0 || toY >= stageSize.y)
        {
            return;
        }

        if (m_Towers[toX, toY] == null)
        {
            m_Towers[toX, toY] = m_Towers[fromX, fromY];
            m_Towers[fromX, fromY] = null;
            m_Towers[toX, toY].SetPosition(toX, toY);
        }
        else
        {
            SwapTowers(fromX, fromY, toX, toY);
        }
    }

    private void SwapTowers(int x1, int y1, int x2, int y2)
    {
        (m_Towers[x1, y1], m_Towers[x2, y2]) = (m_Towers[x2, y2], m_Towers[x1, y1]);

        m_Towers[x1, y1].SetPosition(x1, y1);
        m_Towers[x2, y2].SetPosition(x2, y2);
    }

    public Vector2 GetNearestGridPosition(Vector2 worldPos)
    {
        var localPos = worldPos - startPos;
        var x = Mathf.RoundToInt(localPos.x);
        var y = Mathf.RoundToInt(-localPos.y);
        x = Mathf.Clamp(x, 0, (int)stageSize.x - 1);
        y = Mathf.Clamp(y, 0, (int)stageSize.y - 1);
        return new Vector2(x, y);
    }

    public Vector2 GetStartPosition()
    {
        return startPos;
    }

    public void CombinationTower()
    {
        foreach (var combination in Database.MythCombinationSetting.mythCombinations)
        {
            var foundTowers = new List<Tower>();

            foreach (var material in combination.materialTowerInfos)
            {
                var foundTower = FindSameTower(material.towerType, material.towerGrade);
                if (foundTower == null || foundTower.TowerUnitCount < 1)
                {
                    foundTowers.Clear();
                    break;
                }
                foundTowers.Add(foundTower);
            }
            
            if (foundTowers.Count == combination.materialTowerInfos.Count)
            {
                foreach (var tower in foundTowers)
                {
                    tower.UpdateTowerUnitCount(-1);
                    UpdateCurrentTowerCount(-1);
                }

                PlaceTowerInEmptySpot(combination.mythType, TowerGrade.Myth);
                return;
            }
        }
    }

    public void CombinationTower(MythCombinationSetting.CombinationInfo combinationInfo)
    {
        var foundTowers = new List<Tower>();

        foreach (var material in combinationInfo.materialTowerInfos)
        {
            var foundTower = FindSameTower(material.towerType, material.towerGrade);
            if (foundTower == null || foundTower.TowerUnitCount < 1)
            {
                return;
            }
            foundTowers.Add(foundTower);
        }

        foreach (var tower in foundTowers)
        {
            tower.UpdateTowerUnitCount(-1);
            UpdateCurrentTowerCount(-1);
        }

        PlaceTowerInEmptySpot(combinationInfo.mythType, TowerGrade.Myth);
    }
    
    public void TowerUpgrade(Tower targetTower)
    {
        if (targetTower.TowerUnitCount < 3)
        {
            return;
        }

        var upgradeTowerGrade = targetTower.TowerGrade + 1;

        var upgradeTower = Database.TowerSetting.towerSettingDatas
            .FirstOrDefault(tower => tower.towerType == targetTower.TowerType && tower.towerGrade == upgradeTowerGrade)
            ?.tower;
        
        if (upgradeTower == null)
        {
            return;
        }
        
        targetTower.SetActiveUI(false);
        var sameTower = FindSameTowerUnderCount(targetTower.TowerType, upgradeTowerGrade);

        if (sameTower != null)
        {
            sameTower.UpdateTowerUnitCount(1);
            RemoveTower(targetTower);
        }
        else
        {
            var spawnPos = new Vector2(targetTower.GridX, -targetTower.GridY);
            var upgradeTowerInstance = Instantiate(upgradeTower, spawnPos, Quaternion.identity, null);
            upgradeTowerInstance.transform.localScale = Vector3.one;
            upgradeTowerInstance.transform.SetParent(transform);
            upgradeTowerInstance.SetTower(targetTower.TowerType, upgradeTowerGrade, this);
            upgradeTowerInstance.SetPosition(targetTower.GridX, targetTower.GridY);

            RemoveTower(targetTower);
            m_Towers[targetTower.GridX, targetTower.GridY] = upgradeTowerInstance;
        }
        UpdateCurrentTowerCount(1);
    }

    private void AutoPlay()
    {
        if (Database.GlobalBalanceSetting.playerUnitMaxCount > m_CurrentTowerCount && m_CurrentGold >= m_CurrentRequiredGold)
        {
            SpawnTower(0);
        }
        
        for (var y = 0; y < stageSize.y; y++)
        {
            for (var x = 0; x < stageSize.x; x++)
            {
                var tower = m_Towers[x, y];
                if (tower != null && tower.TowerUnitCount >= 3)
                {
                    TowerUpgrade(tower);
                }
            }
        }

        CombinationTower();
    }

    #endregion
    
    public delegate void OnChangeCurrentTowerCount(int currentTowerCount);
    public delegate void OnChangeCurrentRequiredGold(int currentRequiredGold);
    public delegate void OnChangeCurrentGold(int currentGold);
    public delegate void OnChangeCurrentDia(int dia);
}