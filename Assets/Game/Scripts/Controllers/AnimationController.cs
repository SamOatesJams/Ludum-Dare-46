using SamOatesGames.Systems;
using UnityEngine;

[RequireComponent(typeof(Animator), typeof(SpriteRenderer))]
public class AnimationController : SubscribableMonoBehaviour
{
    private Vector3 m_lastPosition;
    private Transform m_punchTarget;

    private Animator m_animator;
    private SpriteRenderer m_renderer;

    public static readonly int WalkUpAnimationBool = Animator.StringToHash("WalkUp");
    public static readonly int WalkDownAnimationBool = Animator.StringToHash("WalkDown");
    public static readonly int WalkSideAnimationBool = Animator.StringToHash("WalkSide");
    public static readonly int IsDeadAnimationBool = Animator.StringToHash("IsDead");
    public static readonly int PunchingAnimationBool = Animator.StringToHash("Punching");

    void Start()
    {
        var eventAggregator = EventAggregator.GetInstance();
        eventAggregator.Subscribe<EnemyDeathEvent>(this, OnEnemyDeathEvent);
        eventAggregator.Subscribe<PlayerDiedEvent>(this, OnPlayerDiedEvent);
        
        m_animator = GetComponent<Animator>();
        m_renderer = GetComponent<SpriteRenderer>();

        m_lastPosition = transform.position;
    }

    private void OnPlayerDiedEvent(PlayerDiedEvent args)
    {
        if (args.Player.gameObject != gameObject)
        {
            return;
        }

        m_animator.SetBool(IsDeadAnimationBool, true);
    }

    private void OnEnemyDeathEvent(EnemyDeathEvent args)
    {
        if (args.Enemy.gameObject != gameObject)
        {
            return;
        }

        m_animator.SetBool(IsDeadAnimationBool, true);
    }

    public void SetPunchTarget(Transform punchTarget)
    {
        m_punchTarget = punchTarget;
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

        if (m_punchTarget != null)
        {
            m_renderer.flipX = transform.position.x < m_punchTarget.position.x;
            m_animator.SetBool(PunchingAnimationBool, true);
        }
        else
        {
            m_animator.SetBool(PunchingAnimationBool, false);
        }

        m_lastPosition = transform.position;
    }
}
