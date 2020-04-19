using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BootMenu : MonoBehaviour
{
    public UnityEngine.UI.Image Logo;
    public TMPro.TMP_Text Text;

    public float LerpScale = 1.0f;
    public float ShowTime = 2.0f;

    public void Start()
    {
        Logo.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        Text.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);

        StartCoroutine(PlayBootSequence());
    }

    private IEnumerator PlayBootSequence()
    {
        var clearColor = new Color(1.0f, 1.0f, 1.0f, 0.0f);

        var time = 0.0f;
        while (time <= 1.0f)
        {
            var value = Color.Lerp(clearColor, Color.white, time);
            Logo.color = value;
            Text.color = value;

            time += (0.01f * LerpScale);
            yield return new WaitForFixedUpdate();
        }

        yield return new WaitForSecondsRealtime(ShowTime);

        time = 0.0f;
        while (time <= 1.0f)
        {
            var value = Color.Lerp(Color.white, clearColor, time);
            Logo.color = value;
            Text.color = value;

            time += (0.01f * LerpScale);
            yield return new WaitForFixedUpdate();
        }

        yield return new WaitForSecondsRealtime(1.0f);

        SceneManager.LoadScene("Main Menu");
    }
}
