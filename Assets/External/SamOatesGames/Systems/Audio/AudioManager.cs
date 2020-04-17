using SamOatesGames.Systems;
using SamOatesGames.Systems.Core;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : UnitySingleton<AudioManager>, ISubscribable
{
    public static class AudioIds
    {
        public const int MainMenu = 1;
        public const int ButtonHighlight = 2;
        public const int ButtonClick = 3;
    }

    private EventAggregator m_eventAggregator;
    private readonly Dictionary<int, AudioSource> m_audioSourceMap = new Dictionary<int, AudioSource>();

    public AudioClip MenuThemeClip;

    public override void ResolveSystems()
    {
        base.ResolveSystems();
        m_eventAggregator = EventAggregator.GetInstance();
        m_eventAggregator.Subscribe<PlayAudioEvent>(this, HandlePlayAudioEvent);
    }

    public void OnDestroy()
    {
        if (m_eventAggregator != null)
        {
            m_eventAggregator.UnSubscribeAll(this);
        }
    }

    private void HandlePlayAudioEvent(PlayAudioEvent audioEvent)
    {
        if (!m_audioSourceMap.TryGetValue(audioEvent.AudioId, out var audioSource))
        {
            audioSource = CreateNewAudioSource(audioEvent.AudioId);
            m_audioSourceMap[audioEvent.AudioId] = audioSource;
        }

        audioSource.clip = audioEvent.AudioClip;
        audioSource.volume = audioEvent.VolumeScale; // TODO: Multiply by options volume
        audioSource.loop = audioEvent.Loop;
        audioSource.Play();
    }

    private AudioSource CreateNewAudioSource(int id)
    {
        var audioSourceGameObject = new GameObject
        {
            name = $"[{nameof(AudioManager)}] Audio Source ({id})"
        };
        audioSourceGameObject.transform.SetParent(transform);
        return audioSourceGameObject.AddComponent<AudioSource>();
    }
}
