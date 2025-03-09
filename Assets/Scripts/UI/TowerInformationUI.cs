using System.Globalization;
using System.Linq;
using TMPro;
using Units;
using UnityEngine;

namespace UI
{
    public class TowerInformationUI : MonoBehaviour
    {
        #region Tower Information UI References
        
        [SerializeField] private TextMeshProUGUI towerNameText;
        [SerializeField] private TextMeshProUGUI towerAttackDamageText;
        [SerializeField] private TextMeshProUGUI towerAttackSpeedText;

        #endregion

        public void SetTowerInformation(Tower targetTower)
        {
            if (targetTower == null)
            {
                return;
            }
            var targetTowerStatData =
                Data.Database.TowerSetting.towerSettingDatas.FirstOrDefault(towerSettingData =>
                    towerSettingData.identifier == targetTower.Identifier)?.towerData;
            if (targetTowerStatData != null)
            {
                towerNameText.text = targetTowerStatData.name;
            }
            
            towerAttackDamageText.text = targetTower.TowerStat.attackDamage.ToString();
            towerAttackSpeedText.text = targetTower.TowerStat.attackSpeed.ToString(CultureInfo.InvariantCulture);
        }
    }
}