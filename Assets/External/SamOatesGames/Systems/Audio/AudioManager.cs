using SamOatesGames.Systems;
using SamOatesGames.Systems.Core;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : UnitySingleton<AudioManager>, ISubscribable
{
    private EventAggregator m_eventAggregator;
    private readonly Dictionary<int, AudioSource> m_audioSourceMap = new Dictionary<int, AudioSource>();

    public bool AudioEnable { get; private set; } = true;

    public override void ResolveSystems()
    {
        base.ResolveSystems();
        m_eventAggregator = EventAggregator.GetInstance();
        m_eventAggregator.Subscribe<PlayAudioEvent>(this, HandlePlayAudioEvent);
        m_eventAggregator.Subscribe<PlayShiftedAudioEvent>(this, HandlePlayShiftedAudioEvent);
        m_eventAggregator.Subscribe<StopAudioEvent>(this, HandleStopAudioEvent);
    }

    public void OnDestroy()
    {
        if (m_eventAggregator != null)
        {
            m_eventAggregator.UnSubscribeAll(this);
        }
    }

    public void ToggleAudio()
    {
        AudioEnable = !AudioEnable;

        if (AudioEnable)
        {
            foreach (var audioSource in m_audioSourceMap.Values)
            {
                if (audioSource.loop)
                {
                    audioSource.Play();
                }
            }
        }
        else
        {
            foreach (var audioSource in m_audioSourceMap.Values)
            {
                audioSource.Stop();
            }
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

        if (AudioEnable)
        {
            audioSource.Play();
        }
    }

    private void HandlePlayShiftedAudioEvent(PlayShiftedAudioEvent audioEvent)
    {
        if (!m_audioSourceMap.TryGetValue(audioEvent.AudioId, out var audioSource))
        {
            audioSource = CreateNewAudioSource(audioEvent.AudioId);
            m_audioSourceMap[audioEvent.AudioId] = audioSource;
        }

        audioSource.clip = audioEvent.AudioClip;
        audioSource.volume = audioEvent.Volume; // TODO: Multiply by options volume
        audioSource.loop = false;
        audioSource.pitch = Random.Range(audioEvent.PitchRange.x, audioEvent.PitchRange.y);

        if (AudioEnable)
        {
            audioSource.Play();
        }
    }

    private void HandleStopAudioEvent(StopAudioEvent audioEvent)
    {
        if (!m_audioSourceMap.TryGetValue(audioEvent.AudioId, out var audioSource))
        {
            return;
        }

        audioSource.Stop();
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
