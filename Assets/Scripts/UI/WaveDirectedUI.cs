using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace UI
{
    public class WaveDirectedUI : MonoBehaviour
    {
        #region Wave Directed UI References

        [SerializeField] private TextMeshProUGUI waveDirectedText;
        [SerializeField] private TextMeshProUGUI bossKillText;
        [SerializeField] private GameObject waveBackgroundObject;
        [SerializeField] private GameObject bossSpawnObject;
        [SerializeField] private float textSpeed;
        [SerializeField] private float activeTime;
        
        private Coroutine m_TypeWaveCoroutine;

        #endregion

        #region Wave Directed Ui Functions

                public void ShowWaveUI()
        {
            gameObject.SetActive(true);
            waveBackgroundObject.gameObject.SetActive(true);
            waveDirectedText.text = string.Empty;

            if (m_TypeWaveCoroutine != null)
            {
                StopCoroutine(m_TypeWaveCoroutine);
                m_TypeWaveCoroutine = null;
            }

            m_TypeWaveCoroutine = StartCoroutine(TypeWaveText($"WAVE {GameManager.Get.CurrentWave}"));

            if (GameManager.Get.CurrentWave % Data.Database.GlobalBalanceSetting.bossWaveStartWave != 0)
            {
                return;
            }
            bossSpawnObject.transform.localScale = Vector3.zero;
            bossSpawnObject.SetActive(true);
                
            bossSpawnObject.transform.DOScale(1.5f, 0.8f).SetEase(Ease.OutBack)
                .OnComplete(() =>
                {
                    bossSpawnObject.transform.DOScale(1f, 0.8f).SetEase(Ease.InOutQuad);
                });
        }

        private IEnumerator TypeWaveText(string message)
        {
            foreach (var letter in message)
            {
                waveDirectedText.text += letter;
                yield return new WaitForSeconds(textSpeed);
            }
            
            yield return new WaitForSeconds(activeTime);
            gameObject.SetActive(false);
            bossSpawnObject.SetActive(false);
        }

        public void ShowBossKillText(int amount)
        {
            bossKillText.text = string.Empty;
            bossKillText.text = $"보스처치! 다이아 {amount} 획득!";
            waveBackgroundObject.SetActive(false);
            gameObject.SetActive(true);
            bossKillText.transform.localScale = Vector3.zero;
            bossKillText.gameObject.SetActive(true);
                
            bossKillText.transform.DOScale(1.5f, 0.8f).SetEase(Ease.OutBack)
                .OnComplete(() =>
                {
                    bossKillText.transform.DOScale(1f, 0.8f).SetEase(Ease.InOutQuad).OnComplete(() =>
                    {
                        bossKillText.gameObject.SetActive(false);
                    });
                });
        }
        
        #endregion
    }
}