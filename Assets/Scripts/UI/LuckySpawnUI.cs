using Data;
using TMPro;
using UnityEngine;

namespace UI
{
    public class LuckySpawnUI : MonoBehaviour
    {
        #region Lucky Spawn UI References

        [SerializeField] private TextMeshProUGUI currentDia;
        [SerializeField] private TextMeshProUGUI currentTowerUnit;
        
        [SerializeField] private TextMeshProUGUI rareProbabilityText;
        [SerializeField] private TextMeshProUGUI heroProbabilityText;
        [SerializeField] private TextMeshProUGUI legendaryProbabilityText;
        [SerializeField] private TextMeshProUGUI rareSpawnRequiredDiaText;
        [SerializeField] private TextMeshProUGUI heroSpawnRequiredDiaText;
        [SerializeField] private TextMeshProUGUI legendarySpawnRequiredDiaText;

        #endregion

        #region Lucky Spawn UI Functions

        public void Initialize()
        {
            rareProbabilityText.text = $"{Database.ProbabilitySetting.rareLuckSpawn * 100f:0}%";
            heroProbabilityText.text = $"{Database.ProbabilitySetting.heroLuckSpawn * 100f:0}%";
            legendaryProbabilityText.text = $"{Database.ProbabilitySetting.legendaryLuckSpawn * 100f:0}%";
            rareSpawnRequiredDiaText.text = Database.GlobalBalanceSetting.rareSpawnRequiredDia.ToString();
            heroSpawnRequiredDiaText.text = Database.GlobalBalanceSetting.heroSpawnRequiredDia.ToString();
            legendarySpawnRequiredDiaText.text = Database.GlobalBalanceSetting.legendarySpawnRequiredDia.ToString();
        }

        public void UpdateCurrentDia(string dia)
        {
            currentDia.text = dia;
        }

        public void UpdateCurrentTowerUnit(string towerUnit)
        {
            currentTowerUnit.text = towerUnit;
        }

        #endregion
    }
}