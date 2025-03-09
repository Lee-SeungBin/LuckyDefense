using DG.Tweening;
using UnityEngine;

namespace UI
{
    public class HitDamageUI : MonoBehaviour
    {
        #region Hit Dmamage UI References

        [SerializeField] private Transform enemyWorldSpaceTransform;
        [SerializeField] private TMPro.TextMeshProUGUI hitDamageText;
        [SerializeField] private float moveDistance;
        [SerializeField] private float duration;
        [SerializeField] private float scaleUpFactor;

        #endregion

        #region Hit Damage UI Function
        
        public void ShowDamage(int damage)
        {
            transform.SetParent(enemyWorldSpaceTransform);
            gameObject.SetActive(true);
            hitDamageText.text = damage.ToString();
            hitDamageText.alpha = 1f;

            var startPosition = transform.position;
            var endPosition = startPosition + new Vector3(0, moveDistance, 0);
            var localScale = transform.localScale;
            transform.DOScale(scaleUpFactor, duration).SetEase(Ease.OutBack)
                .OnComplete(() => transform.DOScale(localScale, duration).SetEase(Ease.InOutQuad));

            transform.DOMove(endPosition, duration).SetEase(Ease.OutQuad);
            hitDamageText.DOFade(0, duration).SetEase(Ease.Linear)
                .OnComplete(() => Destroy(gameObject));
        }

        #endregion
        
        public void OnDestroy()
        {
            DOTween.Kill(transform);
            DOTween.Kill(hitDamageText);
        }
    }
}