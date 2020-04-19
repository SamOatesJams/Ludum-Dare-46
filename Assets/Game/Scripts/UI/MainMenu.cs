using SamOatesGames.Systems;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    private EventAggregator m_eventAggregator;

    public void Start()
    {
        m_eventAggregator = EventAggregator.GetInstance();
    }

    public void OnBeginButtonClicked()
    {
        m_eventAggregator.Publish(new StartNewGameEvent());
    }
}
