using System.Collections.Generic;
using UnityEngine;

using Units;

namespace Data
{
    public static partial class Database
    {
        public static MythCombinationSetting MythCombinationSetting;

        public static void AssignMythCombinationSetting(MythCombinationSetting data)
        {
            MythCombinationSetting = data;
        }
    }
}

[CreateAssetMenu(fileName = "MythCombinationSetting", menuName = "LuckyDefense/MythCombination Setting")]
public class MythCombinationSetting : ScriptableObject
{
    public List<CombinationInfo> mythCombinations;
        
    [System.Serializable]
    public struct CombinationInfo
    {
        public TowerType mythType;
        public List<MaterialTowerInfo> materialTowerInfos;
    }
    
    [System.Serializable]
    public struct MaterialTowerInfo
    {
        public TowerGrade towerGrade;
        public TowerType towerType;
    }
}