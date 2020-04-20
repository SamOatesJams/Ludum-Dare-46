using SamOatesGames.Systems;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverBanner : SubscribableMonoBehaviour
{
    public UnityEngine.UI.Image BannerImage;
    public TMPro.TMP_Text WaveText;
    public AudioClip GameOverAudio;

    private EventAggregator m_eventAggregator;
    private GameSession m_gameSession;
    private bool m_shown;

    public void Start()
    {
        m_gameSession = GameSession.GetInstance();

        m_eventAggregator = EventAggregator.GetInstance();
        m_eventAggregator.Subscribe<GameOverEvent>(this, OnGameOverEvent);

        m_shown = false;
        var bannerTransform = (RectTransform)BannerImage.transform;
        bannerTransform.anchoredPosition = new Vector2(bannerTransform.anchoredPosition.x, 0.0f);
    }

    private void OnGameOverEvent(GameOverEvent args)
    {
        if (m_shown)
        {
            return;
        }

        WaveText.text = $"Survived {m_gameSession.Wave} {(m_gameSession.Wave == 1 ? "Wave" : "Waves")}";

        m_shown = true;
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

        bannerTransform.anchoredPosition = new Vector2(bannerTransform.anchoredPosition.x, endY);
        yield return new WaitForSecondsRealtime(3.0f);

        SceneManager.LoadScene("Main Menu");
    }
}
