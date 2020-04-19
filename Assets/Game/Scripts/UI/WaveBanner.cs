using System.Collections;
using SamOatesGames.Systems;
using TMPro;
using UnityEngine;

public class WaveBanner : SubscribableMonoBehaviour
{
    public UnityEngine.UI.Image BannerImage;
    public TMP_Text BannerText;

    public void Start()
    {
        var eventAggregator = EventAggregator.GetInstance();
        eventAggregator.Subscribe<StartWaveEvent>(this, OnStartWaveEvent);

        var bannerTransform = (RectTransform)BannerImage.transform;
        bannerTransform.anchoredPosition = new Vector2(bannerTransform.anchoredPosition.x, 0.0f);
    }

    private void OnStartWaveEvent(StartWaveEvent args)
    {
        var bannerTransform = (RectTransform)BannerImage.transform;
        StartCoroutine(MoveBanner(bannerTransform, bannerTransform.anchoredPosition.y, -bannerTransform.sizeDelta.y));
    }

    private IEnumerator MoveBanner(RectTransform bannerTransform, float startY, float endY)
    {
        var time = 0.0f;
        while (time <= 1.0f)
        {
            var value = Mathf.Lerp(startY, endY, time);
            bannerTransform.anchoredPosition = new Vector2(bannerTransform.anchoredPosition.x, value);

            time += 0.01f;
            yield return new WaitForFixedUpdate();
        }

        yield return new WaitForSecondsRealtime(3.0f);

        time = 0.0f;
        while (time <= 1.0f)
        {
            var value = Mathf.Lerp(endY, startY, time);
            bannerTransform.anchoredPosition = new Vector2(bannerTransform.anchoredPosition.x, value);

            time += 0.01f;
            yield return new WaitForFixedUpdate();
        }
    }
}
