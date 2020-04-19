using System.Collections;
using System.Runtime.CompilerServices;
using SamOatesGames.Systems;
using UnityEngine;

public class BeginWaveButton : SubscribableMonoBehaviour
{
    private EventAggregator m_eventAggregator;
    private float m_visiblePositionX;
    private float m_offscreenPositionX;

    public void Start()
    {
        // Position offscreen to stat with
        var rectTransform = ((RectTransform) transform);
        var position = rectTransform.anchoredPosition;
        m_visiblePositionX = position.x;
        m_offscreenPositionX = rectTransform.sizeDelta.x;
        rectTransform.anchoredPosition = new Vector2(m_offscreenPositionX, position.y);

        m_eventAggregator = EventAggregator.GetInstance();
        m_eventAggregator.Subscribe<RequestDaytimeEvent>(this, OnRequestDaytimeEvent);

        StartCoroutine(MoveButton((RectTransform)transform, 1.0f, m_visiblePositionX));
    }

    public void OnClick()
    {
        m_eventAggregator.Publish(new RequestNighttimeEvent());
        StartCoroutine(MoveButton((RectTransform)transform, 1.0f, m_offscreenPositionX));
    }

    private void OnRequestDaytimeEvent(RequestDaytimeEvent args)
    {
        StartCoroutine(MoveButton((RectTransform) transform, 1.0f, m_visiblePositionX));
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

            time += 0.01f;
            yield return new WaitForFixedUpdate();
        }

        yield return null;
    }
}
