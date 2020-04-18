using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator), typeof(SpriteRenderer))]
public class AnimationController : MonoBehaviour
{
    private Vector3 m_lastPosition;

    private Animator m_animator;
    private SpriteRenderer m_renderer;

    public static readonly int WalkUpAnimationBool = Animator.StringToHash("WalkUp");
    public static readonly int WalkDownAnimationBool = Animator.StringToHash("WalkDown");
    public static readonly int WalkSideAnimationBool = Animator.StringToHash("WalkSide");

    void Start()
    {
        m_animator = GetComponent<Animator>();
        m_renderer = GetComponent<SpriteRenderer>();

        m_lastPosition = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        var direction = (transform.position - m_lastPosition);
        var angle = Vector3.SignedAngle(Vector3.up, direction, new Vector3(0, 0, 1));

        bool walkUp = false;
        bool walkDown = false;
        bool walkSide = false;

        m_renderer.flipX = false;

        if (direction.sqrMagnitude > 0)
        {
            // Up
            if (angle > -45.0f && angle <= 45.0f)
            {
                walkUp = true;
            }
            // Right
            else if (angle > 45.0f && angle <= 135.0f)
            {
                walkSide = true;
                m_renderer.flipX = true;
            }
            // Left
            else if (angle > -135.0f && angle <= -45.0f)
            {
                walkSide = true;
            }
            // Down
            else
            {
                walkDown = true;
            }
        }

        m_animator.SetBool(WalkUpAnimationBool, walkUp);
        m_animator.SetBool(WalkDownAnimationBool, walkDown);
        m_animator.SetBool(WalkSideAnimationBool, walkSide);

        m_lastPosition = transform.position;
    }
}
