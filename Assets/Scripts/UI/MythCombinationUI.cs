using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Data;
using Utility;

namespace UI
{
    public class MythCombinationUI : MonoBehaviour
    {
        #region Myth Combination UI References

        [SerializeField] private TMPro.TextMeshProUGUI targetMythName;
        [SerializeField] private Transform mythListTransform;
        [SerializeField] private Transform requiredMaterialsTransform;
        [SerializeField] private MythListButton mythListButtonPrefab;
        [SerializeField] private MythRequiredMaterial mythRequiredMaterialPrefab;
        
        private readonly List<MythListButton> m_ActiveMythListButtons = new();
        private ObjectPool<MythListButton> m_MythListButtonPool;
        
        private readonly List<MythRequiredMaterial> m_ActiveMythRequiredMaterials = new();
        private ObjectPool<MythRequiredMaterial> m_MythRequiredMaterialPool;
        
        private int m_CurrentSelectedIndex;
        private MythCombinationSetting.CombinationInfo m_CurrentCombinationInfo;
        private StageManager m_StageManager;

        #endregion

        #region Monobehaviour Events

        private void Awake()
        {
            m_MythListButtonPool = ObjectPool<MythListButton>.CreateObjectPool(mythListButtonPrefab, mythListTransform);
            m_MythRequiredMaterialPool =
                ObjectPool<MythRequiredMaterial>.CreateObjectPool(mythRequiredMaterialPrefab,
                    requiredMaterialsTransform);
        }

        private void OnEnable()
        {
            if (Database.MythCombinationSetting.mythCombinations.Count <= 0)
            {
                return;
            }

            m_StageManager ??= GameManager.Get.GetStageManager(0);
            m_MythListButtonPool.ReturnObjects(m_ActiveMythListButtons);
            m_ActiveMythListButtons.Clear();

            foreach (var combinationInfo in Database.MythCombinationSetting.mythCombinations)
            {
                var mythListButton = m_MythListButtonPool.GetOrCreate();
                mythListButton.gameObject.SetActive(true);
                mythListButton.transform.SetParent(mythListTransform, false);

                var towerSettingData = Database.TowerSetting.towerSettingDatas.FirstOrDefault(tower =>
                    tower.towerGrade == Units.TowerGrade.Myth && tower.towerType == combinationInfo.mythType);

                if (towerSettingData == null)
                {
                    continue;
                }

                targetMythName.text = towerSettingData.towerData.name;
                mythListButton.SetMythListButton(towerSettingData.towerData.name);
                mythListButton.onClick.AddListener(() =>
                    SelectMythList(combinationInfo, towerSettingData.towerData.name));


                m_ActiveMythListButtons.Add(mythListButton);
            }

            m_ActiveMythListButtons[0].onClick.Invoke();
        }

        #endregion

        #region Myth Combination UI Functions

        private void SelectMythList(MythCombinationSetting.CombinationInfo combinationInfo, string mythName)
        {
            m_MythRequiredMaterialPool.ReturnObjects(m_ActiveMythRequiredMaterials);
            m_ActiveMythRequiredMaterials.Clear();
            targetMythName.text = mythName;
            m_CurrentCombinationInfo = combinationInfo;
            
            foreach (var materialTowerInfo in combinationInfo.materialTowerInfos)
            {
                var requiredMaterialTower = m_MythRequiredMaterialPool.GetOrCreate();
                requiredMaterialTower.gameObject.SetActive(true);
                requiredMaterialTower.transform.SetParent(requiredMaterialsTransform, false);
                var targetTower =
                    m_StageManager.FindSameTower(materialTowerInfo.towerType, materialTowerInfo.towerGrade);
                requiredMaterialTower.SetMythRequiredMaterial(materialTowerInfo, targetTower != null);
                m_ActiveMythRequiredMaterials.Add(requiredMaterialTower);
            }
        }
        
        public void OnClickCombinationButton()
        {
            m_StageManager.CombinationTower(m_CurrentCombinationInfo);
        }

        #endregion
    }
}