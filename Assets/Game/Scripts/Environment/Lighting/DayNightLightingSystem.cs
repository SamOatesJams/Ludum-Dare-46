using System;
using System.Collections;
using SamOatesGames.Systems;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class RequestDaytimeEvent : IEventAggregatorEvent { }
public class RequestNighttimeEvent : IEventAggregatorEvent { }

public class DayNightLightingSystem : SubscribableMonoBehaviour
{
    [Header("Light Sources")]
    public Light2D SunLight;
    public Light2D MoonLight;
    public TorchLight[] TorchLights;

    [Header("Target Intensities")]
    [Range(0.0f, 1.0f)] public float SunLightIntensity;
    [Range(0.0f, 1.0f)] public float MoonLightIntensity;
    [Range(0.0f, 1.0f)] public float TorchLightIntensity;

    public void Start()
    {
        var eventAggregator = EventAggregator.GetInstance();
        eventAggregator.Subscribe<RequestDaytimeEvent>(this, OnRequestDaytimeEvent);
        eventAggregator.Subscribe<RequestNighttimeEvent>(this, OnRequestNighttimeEvent);

        DefaultAllLighting();
    }

    private void DefaultAllLighting()
    {
        SunLight.enabled = true;
        SunLight.intensity = 0.0f;
        MoonLight.enabled = true;
        MoonLight.intensity = 0.0f;
        foreach (var torchLight in TorchLights)
        {
            torchLight.enabled = true;
            torchLight.Light.intensity = 0.0f;
        }

        InterpolateLightSource(value => SunLight.intensity = value, 0.0f, SunLightIntensity);
    }

    private void OnRequestDaytimeEvent(RequestDaytimeEvent args)
    {
        InterpolateLightSource(value => SunLight.intensity = value, 0.0f, SunLightIntensity);
        InterpolateLightSource(value => MoonLight.intensity = value, MoonLightIntensity, 0.0f);
        foreach (var torchLight in TorchLights)
        {
            InterpolateLightSource(value =>
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
        InterpolateLightSource(value => SunLight.intensity = value, SunLightIntensity, 0.0f);
        InterpolateLightSource(value => MoonLight.intensity = value, 0.0f, MoonLightIntensity);
        foreach (var torchLight in TorchLights)
        {
            InterpolateLightSource(value =>
            {
                torchLight.Light.intensity = value;
                if (value > 0.5f)
                {
                    torchLight.LightTorch();
                }
            }, 0.0f, TorchLightIntensity);
        }
    }

    private void InterpolateLightSource(Action<float> valueAction, float start, float end)
    {
        StartCoroutine(InterpolateLightSourceCoroutine(valueAction, start, end));
    }

    IEnumerator InterpolateLightSourceCoroutine(Action<float> valueAction, float start, float end)
    {
        var time = 0.0f;
        while (time < 1.0f)
        {
            var value = Mathf.Lerp(start, end, time);
            valueAction(value);

            time += 0.01f;
            yield return new WaitForFixedUpdate();
        }

        yield return null;
    }

#if UNITY_EDITOR
    //private void OnGUI()
    //{
    //    if (GUI.Button(new Rect(10, 30, 140, 30), "Request Daytime"))
    //    {
    //        OnRequestDaytimeEvent(new RequestDaytimeEvent());
    //    }

    //    if (GUI.Button(new Rect(160, 30, 140, 30), "Request Nighttime"))
    //    {
    //        OnRequestNighttimeEvent(new RequestNighttimeEvent());
    //    }
    //}
#endif
}
