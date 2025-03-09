using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utility;

public class SoundManager : SingletonObject<SoundManager>
{
    [SerializeField] private List<SoundDictionaryEntry> soundList;

    private readonly Dictionary<string, AudioClip> m_SoundDictionary = new();

    protected override void Awake()
    {
        base.Awake();
        foreach (var entry in soundList.Where(entry => !m_SoundDictionary.ContainsKey(entry.key)))
        {
            m_SoundDictionary.Add(entry.key, entry.value);
        }
    }

    public void PlaySound(AudioSource audioSource, string soundKey)
    {
        if (m_SoundDictionary.TryGetValue(soundKey, out var clip))
        {
            audioSource.PlayOneShot(clip);
        }
    }
    
    [System.Serializable]
    public class SoundDictionaryEntry
    {
        public string key;
        public AudioClip value;
    }

}