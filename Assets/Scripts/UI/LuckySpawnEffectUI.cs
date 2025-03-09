using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

namespace UI
{
    public class LuckySpawnEffectUI : MonoBehaviour
    {
        #region Lucky Spawn Effect UI References
        
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private UnityEngine.UI.Image resultIcon;
        [SerializeField] private Sprite successSprite;
        [SerializeField] private Sprite failedSprite;
        [SerializeField] private TMPro.TextMeshProUGUI resultText;
        [SerializeField] private float moveDistance;
        [SerializeField] private float duration;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private List<PositionData> positionDatas;

        #endregion
        
        public void ShowEffect(Units.TowerGrade grade, bool result)
        {           
            var positionData = positionDatas.FirstOrDefault(data => data.towerGrade == grade);
            
            if (positionData == null)
            {
                return;
            }
            
            transform.position = positionData.spawnTransform.position;
            gameObject.SetActive(true);
            resultIcon.sprite = result ? successSprite : failedSprite;
            resultText.text = result ? "성공!" : "실패..";
            canvasGroup.alpha = 1;
            SoundManager.Get.PlaySound(audioSource, result ? "LuckySpawnSuccess" :"LuckySpawnFailed");
            var startPosition = positionData.spawnTransform.position;
            var endPosition = startPosition + new Vector3(0, moveDistance, 0);
            
            transform.DOMove(endPosition, duration).SetEase(Ease.OutQuad);
            canvasGroup.DOFade(0, duration).SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    if (this == null)
                    {
                        return;
                    }
                    Destroy(gameObject);
                });
        }
    }

    [System.Serializable]
    public class PositionData
    {
        public Units.TowerGrade towerGrade;
        public Transform spawnTransform;
    }
}