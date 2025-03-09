using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

namespace UI
{
    public class IncreaseResourceUI : MonoBehaviour
    {
        #region Increase Resource UI References

        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private List<ResourceInfo> resourceSprites;
        [SerializeField] private UnityEngine.UI.Image resourceIcon;
        [SerializeField] private TMPro.TextMeshProUGUI resourceAmountText;
        [SerializeField] private float moveDistance;
        [SerializeField] private float duration;
        [SerializeField] private AudioSource audioSource;

        #endregion
        
        public void IncreaseResource(GameManager.ResourceType resourceType ,int amount)
        {
            var resourceInfo = resourceSprites.FirstOrDefault(resource => resource.resourceType == resourceType);
            
            if (resourceInfo == null)
            {
                return;
            }
            
            transform.position = resourceInfo.spawnTransform.position;
            gameObject.SetActive(true);
            resourceIcon.sprite = resourceInfo.resourceSprite;
            resourceAmountText.text = $"+{amount}";
            canvasGroup.alpha = 1;
            SoundManager.Get.PlaySound(audioSource, "GetResource");
            var startPosition = resourceInfo.spawnTransform.position;
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
    public class ResourceInfo
    {
        public GameManager.ResourceType resourceType;
        public Transform spawnTransform;
        public Sprite resourceSprite;
    }


}