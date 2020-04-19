using SamOatesGames.Systems;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverBanner : SubscribableMonoBehaviour
{
    public UnityEngine.UI.Image BannerImage;
    public AudioClip GameOverAudio;

    private EventAggregator m_eventAggregator;

    public void Start()
    {
        m_eventAggregator = EventAggregator.GetInstance();
        m_eventAggregator.Subscribe<GameOverEvent>(this, OnGameOverEvent);

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
        m_eventAggregator.Publish(new StopAudioEvent(AudioIds.MenuTheme));
        m_eventAggregator.Publish(new PlayAudioEvent(AudioIds.GameOver, GameOverAudio, false));

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
