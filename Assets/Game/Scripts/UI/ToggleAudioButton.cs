using UnityEngine;

public class ToggleAudioButton : MonoBehaviour
{
    private AudioManager m_audioManager;

    public void Start()
    {
        m_audioManager = AudioManager.GetInstance();
    }

    public void OnClick()
    {
        m_audioManager.ToggleAudio();
    }
}
