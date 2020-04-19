using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

[RequireComponent(typeof(Animator))]
public class TorchLight : MonoBehaviour
{
    public bool StartLit = false;

    private Animator m_animator;
    private Light2D m_light;

    public Light2D Light
    {
        get
        {
            if (m_light == null)
            {
                m_light = GetComponentInChildren<Light2D>();
            }
            return m_light;
        }
    }

    public static readonly int LitAnimationBool = Animator.StringToHash("Lit");

    
    public void Start()
    {
        m_animator = GetComponent<Animator>();
        m_light = Light;

        if (StartLit)
        {
            LightTorch();
        }
        else
        {
            ExtinguishTorch();
        }
    }

    public void LightTorch()
    {
        m_animator.SetBool(LitAnimationBool, true);
    }

    public void ExtinguishTorch()
    {
        m_animator.SetBool(LitAnimationBool, false);
    }
}
