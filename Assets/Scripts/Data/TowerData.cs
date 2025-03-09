using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Data
{
    public partial class Database
    {
        public static TowerData TowerData;

        public static void InitializeTowerData(string csvContent)
        {
            TowerData = new TowerData();
            TowerData.ParseFromCsv(csvContent);
        }
    }

    public class TowerData : Dictionary<string, TowerDataTuple>
    {
        public void ParseFromCsv(string csvContent)
        {
            var csvLines = Database.SplitCsvLine(csvContent);

#if UNITY_EDITOR
            var conflictedLineInformation = new StringBuilder();
#endif

            for (var i = 2; i < csvLines.Length; ++i)
            {
                var csvElement = Database.SplitCsvElement(csvLines[i]);
                var tuple = new TowerDataTuple(csvElement);

                if (!TryAdd(tuple.identifier, tuple))
                {
#if UNITY_EDITOR
                    conflictedLineInformation.AppendLine($"identifier {tuple.identifier} at line {(i + 1)} is conflicted");
#endif
                }
            }

#if UNITY_EDITOR
            if (conflictedLineInformation.Length > 0)
            {
                Debug.LogError($"Conflicted line detected while parsing Localization table\n{conflictedLineInformation}");
            }
#endif
        }
    }

    public class TowerDataTuple
    {
        public readonly string identifier;
        public readonly string name;
        public readonly int attackDamage;
        public readonly float attackSpeed;
        public readonly float attackRange;
        public readonly int resellGold;
        public readonly int resellDia;

        public TowerDataTuple(params string[] csvElements)
        {
#if UNITY_EDITOR
            if (csvElements.Length != 7)
            {
                throw new ArgumentException("Provided CSV Content seems to be contaminated");
            }
#endif

            identifier = csvElements[0];
            name = csvElements[1];
            attackDamage = int.Parse(csvElements[2]);
            attackSpeed = float.Parse(csvElements[3]);
            attackRange = float.Parse(csvElements[4]);
            resellGold = int.Parse(csvElements[5]);
            resellDia = int.Parse(csvElements[6]);
        }
    }
}