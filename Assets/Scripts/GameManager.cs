using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Data;
using UnityEngine;
using Utility;

public class GameManager : SingletonObject<GameManager>
{
    #region GameManager References
    
    [SerializeField] private List<StageManager> stageManagers;
    [SerializeField] private List<TMPro.TextMeshProUGUI> waveStartTimerTexts;
    [SerializeField] private CanvasGroup waveStartTimerCanvasGroup;
    
    public int CurrentWave
    {
        get => m_CurrentWave;
        private set
        {
            m_CurrentWave = value;
            OnChangeWaveEvent?.Invoke();
        }
    }
    
    private int m_CurrentWave;
    
    public event OnChangeWaveDelegate OnChangeWaveEvent;

    #endregion

    #region MonoBehaviour References

    private void Start()
    {
        StartNextWave();
    }

    #endregion

    #region GameManger Functions

    public void GetGold(Units.EnemyUnit enemyUnit)
    {
        foreach (var stageManager in stageManagers)
        {
            stageManager.UpdateCurrentGold(enemyUnit.KillGold);
        }
    }

    public void GetDia(int amount)
    {
        foreach (var stageManager in stageManagers)
        {
            stageManager.UpdateCurrentDia(amount);
        }
    }

    public StageManager GetStageManager(int stageIndex)
    {
        return stageManagers[stageIndex];
    }
    
    public void StartNextWave()
    {
        StartCoroutine(NextWave());
    }

    private IEnumerator NextWave()
    {
        waveStartTimerCanvasGroup.alpha = 1;
        
        for (var i = Database.GlobalBalanceSetting.waveWaitingTime; i > 0; i--)
        {
            foreach (var waveStartTimerText in waveStartTimerTexts)
            {
                waveStartTimerText.text = i.ToString(CultureInfo.InvariantCulture);
            }
            yield return new WaitForSeconds(1);
        }

        waveStartTimerCanvasGroup.alpha = 0;
        CurrentWave++;
        
        if (m_CurrentWave % Database.GlobalBalanceSetting.bossWaveStartWave == 0)
        {
            SpawnManager.Get.StartSpawnBossEnemy(m_CurrentWave);
        }
        else
        {
            SpawnManager.Get.StartSpawnEnemy(m_CurrentWave);    
        }
    }

    public void GameOver()
    {
        StopAllCoroutines();
        foreach (var stageManager in stageManagers)
        {
            stageManager.gameObject.SetActive(false);
        }
    }

    public void RestartGame()
    {
        var currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        UnityEngine.SceneManagement.SceneManager.LoadScene(currentSceneName);
    }

    #endregion
    
    public delegate void OnChangeWaveDelegate();
    
    public enum ResourceType
    {
        Gold,
        Dia
    }
}