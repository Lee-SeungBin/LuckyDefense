using System.Globalization;
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

            if (Data.Database.TowerData.TryGetValue(targetTower.Identifier, out var towerData))
            {
                towerNameText.text = towerData.name;
            }
            
            towerAttackDamageText.text = targetTower.TowerStat.attackDamage.ToString();
            towerAttackSpeedText.text = targetTower.TowerStat.attackSpeed.ToString(CultureInfo.InvariantCulture);
        }
    }
}