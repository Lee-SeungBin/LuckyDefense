using UnityEngine;

namespace Data
{
    public static partial class Database
    {
        public static ProbabilitySetting ProbabilitySetting;

        public static void AssignProbabilitySetting(ProbabilitySetting data)
        {
            ProbabilitySetting = data;
        }
    }
}

[CreateAssetMenu(fileName = "ProbabilitySetting", menuName = "LuckyDefense/Probability Setting")]
public class ProbabilitySetting : ScriptableObject
{
    [Header("Lucky Spawn Probability")] public float rareLuckSpawn = 0.6f;
    public float heroLuckSpawn = 0.1f;
    public float legendaryLuckSpawn = 0.1f;

    [Space(20), Header("Normal Spawn Probability")]

    public float normalSpawn;
    public float rareSpawn;
    public float heroSpawn;
    public float legendarySpawn;
}