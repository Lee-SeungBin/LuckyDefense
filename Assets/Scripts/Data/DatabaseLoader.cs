using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Data
{
    public static partial class Database
    {
        public static string[] SplitCsvLine(in string content)
        {
            const string splitPattern = @"\r\n";
            return System.Text.RegularExpressions.Regex.Split(content, splitPattern);
        }

        public static string[] SplitCsvElement(in string content)
        {
            const string splitPattern = @",(?=(?:[^""]*""[^""]*"")*[^""]*$)";
            return System.Text.RegularExpressions.Regex.Split(content.Trim('\r'), splitPattern);
        }

        public static string[] SplitCsvElement(in string content, bool trimCarriageReturn)
        {
            const string splitPattern = @",(?=(?:[^""]*""[^""]*"")*[^""]*$)";
            return System.Text.RegularExpressions.Regex.Split(
                trimCarriageReturn ? content.Trim('\r') : content,
                splitPattern);
        }
    }
    
    public class DatabaseLoader : MonoBehaviour
    {
        [SerializeField]
        private List<DataAssetDefinition> dataAssetsToLoad = new(32);
        
        [SerializeField]
        private List<DataAssetDefinition> csvDataAssetsToLoad = new(32);

        private void Awake()
        {
            Debug.Log("Loading data assets...");
            Load();
        }

        private void Load()
        {
            foreach (var dataAsset in dataAssetsToLoad)
            {
                var asset = Resources.Load(dataAsset.assetPath);
                if (asset == null)
                {
                    Debug.LogError($"Failed to load data asset at path: {dataAsset.assetPath}");
                    continue;
                }
                
                var targetLoadMethod = typeof(Database).GetMethod(
                    $"Assign{dataAsset.containerName}",
                    BindingFlags.Static | BindingFlags.Public);
                if (targetLoadMethod == null)
                {
                    Debug.LogError($"Failed to assign data asset to Database: {dataAsset.containerName}");
                    continue;
                }
                
                targetLoadMethod.Invoke(null, new object[] {asset});
            }

            foreach (var csvDataAsset in csvDataAssetsToLoad)
            {
                var asset = Resources.Load<TextAsset>(csvDataAsset.assetPath);
                if (asset == null)
                {
                    Debug.LogError($"Failed to load data asset at path: {csvDataAsset.assetPath}");
                    continue;
                }
                
                var targetLoadMethod = typeof(Database).GetMethod(
                    $"Initialize{csvDataAsset.containerName}",
                    BindingFlags.Static | BindingFlags.Public);
                if (targetLoadMethod == null)
                {
                    Debug.LogError($"Failed to assign data asset to Database: {csvDataAsset.containerName}");
                    continue;
                }
                
                targetLoadMethod.Invoke(null, new object[] {asset.text});
            }
        }

        [Serializable]
        public struct DataAssetDefinition
        {
            [Tooltip("Relative path to the Resources folder of the data asset.\nFile extension is not required.")]
            public string assetPath;
            
            [Tooltip("Data container field name of Database class.")]
            public string containerName;
        }
    }
}