using System.Collections.Generic;
using UnityEngine;
using Units;

namespace Data
{
    public static partial class Database
    {
        public static TowerSetting TowerSetting;

        public static void AssignTowerSetting(TowerSetting data)
        {
            TowerSetting = data;
        }
    }
}

[CreateAssetMenu(fileName = "TowerSetting", menuName = "LuckyDefense/Tower Setting")]
public class TowerSetting : ScriptableObject
{
    public List<TowerSettingData> towerSettingDatas;
    
    [System.Serializable]
    public class TowerSettingData
    {
        public Tower tower;
        public string identifier;
        public Sprite projectileSprite;
        public TowerType towerType;
        public TowerGrade towerGrade;
        public TowerData towerData;
    }
    
    [System.Serializable]
    public class TowerData
    {
        public string name;
        public int attackDamage;
        public float attackSpeed;
        public float attackRange;
        public int resellGold;
        public int resellDia;
    }
}