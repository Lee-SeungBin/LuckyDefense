using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    public static partial class Database
    {
        public static GlobalBalanceSetting GlobalBalanceSetting;

        public static void AssignGlobalBalanceSetting(GlobalBalanceSetting data)
        {
            GlobalBalanceSetting = data;
        }
    }
}

[CreateAssetMenu(fileName = "GlobalBalanceSetting", menuName = "LuckyDefense/Global Balance Setting")]
public class GlobalBalanceSetting : ScriptableObject
{
    [Header("Normal Wave Setting")]
    public int waveUnitMaxCount;
    public int waveUnitSpawnCount;
    public int waveTimer;
    public float waveUnitSpawnInterval;
    public float waveWaitingTime;
    public int waveUnitKillGold;
    public float waveUnitKillGoldMultiplier;
    public int initWaveUnitHealth;
    public float waveUnitHealthMultiplier;
    public float waveUnitSpeed;
    
    [Header("Boss Wave Setting")]
    public float bossWaveTimer;
    public int bossWaveStartWave;
    public int bossUnitKillDia;
    public float bossUnitKillDiaMultiplier;
    public float bossUnitKillDiaDivider;
    public int initBossUnitHealth;
    public float bossUnitHealthMultiplier;
    public float bossUnitHealthDivider;
    
    [Header("Player Setting")]
    public int playerUnitMaxCount;
    public int rareSpawnRequiredDia;
    public int heroSpawnRequiredDia;
    public int legendarySpawnRequiredDia;
    public int initStartGold;
    public int initSpawnGold;
    public int increaseSpawnGold;
}