using System.Linq;
using Units;
using UnityEngine;

namespace UI
{
    public class MythRequiredMaterial : MonoBehaviour
    {
        private const string HaveTower = "보유";
        private const string NotHaveTower = "미보유";

        #region Myth Required Material References

        [SerializeField] private TMPro.TextMeshProUGUI nameText;
        [SerializeField] private TMPro.TextMeshProUGUI haveText;
        [SerializeField] private UnityEngine.UI.Image backgroundImage;
        [SerializeField] private Color rareColor;
        [SerializeField] private Color heroColor;

        #endregion
        
        public void SetMythRequiredMaterial(MythCombinationSetting.MaterialTowerInfo targetTowerInfo, bool haveTower)
        {
            backgroundImage.color = targetTowerInfo.towerGrade switch
            {
                TowerGrade.Rare => rareColor,
                TowerGrade.Hero => heroColor,
                _ => backgroundImage.color
            };
            var towerSettingData = Data.Database.TowerSetting.towerSettingDatas.FirstOrDefault(tower =>
                tower.towerGrade == targetTowerInfo.towerGrade && tower.towerType == targetTowerInfo.towerType);
            
            if (towerSettingData == null)
            {
                return;
            }
            
            if (Data.Database.TowerData.TryGetValue(towerSettingData.identifier, out var towerData))
            {
                nameText.text = towerData.name;
            }

            haveText.text = haveTower ? HaveTower : NotHaveTower;
        }
        
    }
}