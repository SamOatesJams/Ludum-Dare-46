using SamOatesGames.Systems;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    private EventAggregator m_eventAggregator;

    public AudioClip MenuTheme;

    public void Start()
    {
        m_eventAggregator = EventAggregator.GetInstance();
        m_eventAggregator.Publish(new PlayAudioEvent(AudioIds.MenuTheme, MenuTheme, 0.75f, true));
    }

    public void OnBeginButtonClicked()
    {
        m_eventAggregator.Publish(new StopAudioEvent(AudioIds.MenuTheme));
        m_eventAggregator.Publish(new StartNewGameEvent());
    }
}
