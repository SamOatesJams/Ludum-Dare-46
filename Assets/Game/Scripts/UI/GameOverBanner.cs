using System.Collections;
using SamOatesGames.Systems;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverBanner : SubscribableMonoBehaviour
{
    public UnityEngine.UI.Image BannerImage;
    public TMP_Text BannerText;

    public void Start()
    {
        var eventAggregator = EventAggregator.GetInstance();
        eventAggregator.Subscribe<GameOverEvent>(this, OnGameOverEvent);

        var bannerTransform = (RectTransform)BannerImage.transform;
        bannerTransform.anchoredPosition = new Vector2(bannerTransform.anchoredPosition.x, 0.0f);
    }

    private void OnGameOverEvent(GameOverEvent args)
    {
        var bannerTransform = (RectTransform)BannerImage.transform;
        StartCoroutine(MoveBanner(bannerTransform, bannerTransform.anchoredPosition.y, bannerTransform.sizeDelta.y));
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

        SceneManager.LoadScene("Main Menu");
    }
}
