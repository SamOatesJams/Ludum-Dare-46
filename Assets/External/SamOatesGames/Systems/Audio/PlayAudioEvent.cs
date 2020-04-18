using UnityEngine;
using SamOatesGames.Systems;

public class PlayAudioEvent : IEventAggregatorEvent
{
    private const float c_defaultVolumeScale = 1.0f;
    private const bool c_defaultLoopState = false;

    public int AudioId { get; }
    public AudioClip AudioClip { get; }
    public float VolumeScale { get; }
    public bool Loop { get; }

    public PlayAudioEvent(int audioId, AudioClip audioClip, float volumeScale, bool loop)
    {
        AudioId = audioId;
        AudioClip = audioClip;
        VolumeScale = volumeScale;
        Loop = loop;
    }

    public PlayAudioEvent(int audioId, AudioClip audioClip)
        : this(audioId, audioClip, c_defaultVolumeScale, c_defaultLoopState)
    {
    }

    public PlayAudioEvent(int audioId, AudioClip audioClip, float volumeScale)
        : this(audioId, audioClip, volumeScale, c_defaultLoopState)
    {
    }

    public PlayAudioEvent(int audioId, AudioClip audioClip, bool loop)
        : this(audioId, audioClip, c_defaultVolumeScale, loop)
    {
    }
}
