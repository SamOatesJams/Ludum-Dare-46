using SamOatesGames.Systems;
using UnityEngine;

public class SetCameraFollowTransformEvent : IEventAggregatorEvent
{
    public Transform Transform { get; }

    public SetCameraFollowTransformEvent(Transform transform)
    {
        Transform = transform;
    }
}

[DefaultExecutionOrder(-50)]
public class FollowTransform : SubscribableMonoBehaviour
{
    public Vector3 Offset;
    public float Smoothness = 0.5f;

    private Transform m_targetTransform;
    private Vector3 m_targetPosition;
    private Vector3 m_currentVelocity = Vector3.zero;

    public void Start()
    {
        var eventAggregator = EventAggregator.GetInstance();
        eventAggregator.Subscribe<SetCameraFollowTransformEvent>(this, OnSetCameraFollowTransformEvent);
    }

    private void OnSetCameraFollowTransformEvent(SetCameraFollowTransformEvent args)
    {
        m_targetTransform = args.Transform;
        m_targetPosition = m_targetTransform.position + Offset;
        transform.position = m_targetPosition;
    }

    public void Update()
    {
        if (m_targetTransform == null)
        {
            return;
        }

        m_targetPosition = m_targetTransform.position + Offset;
        transform.position = Vector3.SmoothDamp(transform.position, m_targetPosition, ref m_currentVelocity, Smoothness);
    }
}
