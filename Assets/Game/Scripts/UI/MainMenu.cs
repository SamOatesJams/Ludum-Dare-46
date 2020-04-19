using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void OnBeginButtonClicked()
    {
        SceneManager.LoadScene("Game Scene");
    }
}
