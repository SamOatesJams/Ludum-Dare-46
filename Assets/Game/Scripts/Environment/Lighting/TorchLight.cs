using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

[RequireComponent(typeof(Animator))]
public class TorchLight : MonoBehaviour
{
    private Animator m_animator;
    
    public Light2D Light { get; private set; }

    public static readonly int LitAnimationBool = Animator.StringToHash("Lit");

    
    public void Start()
    {
        m_animator = GetComponent<Animator>();
        Light = GetComponentInChildren<Light2D>();
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
