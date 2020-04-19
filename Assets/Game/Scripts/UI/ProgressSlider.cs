using SamOatesGames.Systems;
using UnityEngine;

public class ProgressSlider : SubscribableMonoBehaviour
{
    [Header("Target Resources")]
    public Sprite SunLogo;
    public Color SunBackgroundColor;
    public Sprite MoonLogo;
    public Color MoonBackgroundColor;

    [Header("Controls")]
    public UnityEngine.UI.Slider Slider;
    public UnityEngine.UI.Image SliderHandle;
    public UnityEngine.UI.Image SliderBackground;

    private EventAggregator m_eventAggregator;
    private GameSession m_gameSession;
    
    public void Start()
    {
        m_eventAggregator = EventAggregator.GetInstance();
        m_gameSession = GameSession.GetInstance();

        m_eventAggregator.Subscribe<RequestDaytimeEvent>(this, OnRequestDaytimeEvent);
        m_eventAggregator.Subscribe<RequestNighttimeEvent>(this, OnRequestNighttimeEvent);

        OnRequestDaytimeEvent(new RequestDaytimeEvent());
    }

    private void FixedUpdate()
    {
        Slider.value = m_gameSession.GetStageProgress();
    }

    private void OnRequestDaytimeEvent(RequestDaytimeEvent args)
    {
        SliderHandle.sprite = SunLogo;
        Slider.value = 0.0f;
        SliderBackground.color = SunBackgroundColor;
    }

    private void OnRequestNighttimeEvent(RequestNighttimeEvent args)
    {
        SliderHandle.sprite = MoonLogo;
        Slider.value = 0.0f;
        SliderBackground.color = MoonBackgroundColor;
    }
}
