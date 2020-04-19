using System;
using System.Collections;
using SamOatesGames.Systems;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class RequestDaytimeEvent : IEventAggregatorEvent { }
public class RequestNighttimeEvent : IEventAggregatorEvent { }

public class DayNightLightingSystem : SubscribableMonoBehaviour
{
    private EventAggregator m_eventAggregator;
    private GameSession m_gameSession;

    [Header("Light Sources")]
    public Light2D SunLight;
    public Light2D MoonLight;
    public TorchLight[] TorchLights;

    [Header("Target Intensities")]
    [Range(0.0f, 1.0f)] public float SunLightIntensity;
    [Range(0.0f, 1.0f)] public float MoonLightIntensity;
    [Range(0.0f, 1.0f)] public float TorchLightIntensity;

    [Header("Audio Clips")]
    public AudioClip DaytimeAudio;
    public AudioClip NighttimeAudio;

    public void Start()
    {
        m_gameSession = GameSession.GetInstance();

        m_eventAggregator = EventAggregator.GetInstance();
        m_eventAggregator.Subscribe<RequestDaytimeEvent>(this, OnRequestDaytimeEvent);
        m_eventAggregator.Subscribe<RequestNighttimeEvent>(this, OnRequestNighttimeEvent);

        DefaultAllLighting();
    }

    private void DefaultAllLighting()
    {
        m_eventAggregator.Publish(new PlayAudioEvent(AudioIds.MenuTheme, DaytimeAudio, 0.75f, true));

        SunLight.enabled = true;
        SunLight.intensity = 0.0f;
        MoonLight.enabled = true;
        MoonLight.intensity = 0.0f;
        foreach (var torchLight in TorchLights)
        {
            torchLight.enabled = true;
            torchLight.Light.intensity = 0.0f;
        }

        InterpolateValue(value => SunLight.intensity = value, 0.0f, SunLightIntensity);
    }

    private void OnRequestDaytimeEvent(RequestDaytimeEvent args)
    {
        m_eventAggregator.Publish(new PlayAudioEvent(AudioIds.MenuTheme, DaytimeAudio, 0.75f, true));

        InterpolateValue(value => SunLight.intensity = value, 0.0f, SunLightIntensity);
        InterpolateValue(value => MoonLight.intensity = value, MoonLightIntensity, 0.0f);
        foreach (var torchLight in TorchLights)
        {
            InterpolateValue(value =>
            {
                torchLight.Light.intensity = value;
                if (value < 0.5f)
                {
                    torchLight.ExtinguishTorch();
                }
            }, TorchLightIntensity, 0.0f);
        }
    }

    private void OnRequestNighttimeEvent(RequestNighttimeEvent args)
    {
        m_eventAggregator.Publish(new PlayAudioEvent(AudioIds.MenuTheme, NighttimeAudio, 0.75f, true));

        InterpolateValue(value => SunLight.intensity = value, SunLightIntensity, 0.0f);
        InterpolateValue(value => MoonLight.intensity = value, 0.0f, MoonLightIntensity);
        foreach (var torchLight in TorchLights)
        {
            InterpolateValue(value =>
            {
                torchLight.Light.intensity = value;
                if (value > 0.5f)
                {
                    torchLight.LightTorch();
                }
            }, 0.0f, TorchLightIntensity);
        }

        InterpolateValue(value =>
        {
            if (value >= 0.9f)
            {
                m_eventAggregator.Publish(new StartWaveEvent(m_gameSession.Wave));
            }
        }, 0.0f, 1.0f);
    }

    private void InterpolateValue(Action<float> valueAction, float start, float end)
    {
        StartCoroutine(InterpolateLightSourceCoroutine(valueAction, start, end));
    }

    IEnumerator InterpolateLightSourceCoroutine(Action<float> valueAction, float start, float end)
    {
        var time = 0.0f;
        while (time <= 1.0f)
        {
            var value = Mathf.Lerp(start, end, time);
            valueAction(value);

            time += 0.01f;
            yield return new WaitForFixedUpdate();
        }

        yield return null;
    }
}
