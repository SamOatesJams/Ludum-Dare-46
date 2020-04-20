using SamOatesGames.Systems;
using System.Collections;
using UnityEngine;

public class BeginWaveButton : SubscribableMonoBehaviour
{
    public float OffscreenX;
    public float OnscreenX;

    private EventAggregator m_eventAggregator;

    public void Start()
    {
        // Position offscreen to stat with
        var rectTransform = ((RectTransform) transform);
        var position = rectTransform.anchoredPosition;
        rectTransform.anchoredPosition = new Vector2(OffscreenX, position.y);

        m_eventAggregator = EventAggregator.GetInstance();
        m_eventAggregator.Subscribe<RequestDaytimeEvent>(this, OnRequestDaytimeEvent);
        m_eventAggregator.Subscribe<RequestNighttimeEvent>(this, OnRequestNighttimeEvent);
        m_eventAggregator.Subscribe<TutorialCompleteEvent>(this, OnTutorialCompleteEvent);

        var gameSession = GameSession.GetInstance();
        if (gameSession.Stage != GameSession.GameStage.Cutscene)
        {
            StartCoroutine(MoveButton((RectTransform) transform, 1.0f, OnscreenX));
        }
    }

    public void OnClick()
    {
        m_eventAggregator.Publish(new RequestNighttimeEvent());
    }

    private void OnTutorialCompleteEvent(TutorialCompleteEvent obj)
    {
        StartCoroutine(MoveButton((RectTransform)transform, 1.0f, OnscreenX));
    }

    private void OnRequestDaytimeEvent(RequestDaytimeEvent args)
    {
        StartCoroutine(MoveButton((RectTransform) transform, 1.0f, OnscreenX));
    }

    private void OnRequestNighttimeEvent(RequestNighttimeEvent args)
    {
        StartCoroutine(MoveButton((RectTransform)transform, 1.0f, OffscreenX));
    }

    private IEnumerator MoveButton(RectTransform rectTransform, float delay, float targetX)
    {
        yield return new WaitForSecondsRealtime(delay);

        var position = rectTransform.anchoredPosition;
        var startX = position.x;
        
        var time = 0.0f;
        while (time <= 1.0f)
        {
            var value = Mathf.Lerp(startX, targetX, time);
            rectTransform.anchoredPosition = new Vector2(value, position.y);

            time += 0.04f;
            yield return new WaitForFixedUpdate();
        }

        rectTransform.anchoredPosition = new Vector2(targetX, position.y);
        yield return null;
    }
}
