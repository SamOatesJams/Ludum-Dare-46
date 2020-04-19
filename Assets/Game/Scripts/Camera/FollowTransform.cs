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

public class SetCameraPlayableBoundsEvent : IEventAggregatorEvent
{
    public Bounds Bounds { get; }

    public SetCameraPlayableBoundsEvent(Bounds bounds)
    {
        Bounds = bounds;
    }
}

[DefaultExecutionOrder(-50)]
[RequireComponent(typeof(Camera))]
public class FollowTransform : SubscribableMonoBehaviour
{
    public Vector3 Offset;
    public float Smoothness = 0.5f;

    private Camera m_camera;

    private Transform m_targetTransform;
    private Bounds? m_mapBounds;

    private Vector3 m_targetPosition;
    private Vector3 m_currentVelocity = Vector3.zero;

    public void Start()
    {
        m_camera = GetComponent<Camera>();

        var eventAggregator = EventAggregator.GetInstance();
        eventAggregator.Subscribe<SetCameraFollowTransformEvent>(this, OnSetCameraFollowTransformEvent);
        eventAggregator.Subscribe<SetCameraPlayableBoundsEvent>(this, OnSetCameraPlayableBoundsEvent);
    }

    private void OnSetCameraFollowTransformEvent(SetCameraFollowTransformEvent args)
    {
        var oldTransform = m_targetTransform;

        m_targetTransform = args.Transform;
        m_targetPosition = m_targetTransform.position + Offset;

        if (oldTransform == null)
        {
            transform.position = m_targetPosition;
        }
    }

    private void OnSetCameraPlayableBoundsEvent(SetCameraPlayableBoundsEvent args)
    {
        m_mapBounds = args.Bounds;
    }

    public void Update()
    {
        if (m_targetTransform == null)
        {
            return;
        }

        m_targetPosition = m_targetTransform.position + Offset;

        var oldPosition = transform.position;
        var newPosition = Vector3.SmoothDamp(oldPosition, m_targetPosition, ref m_currentVelocity, Smoothness);
        
        if (m_mapBounds.HasValue)
        {
            var mapBounds = m_mapBounds.Value;
            var cameraBounds = CalculateCameraBounds(newPosition);

            if (cameraBounds.min.x < mapBounds.min.x || cameraBounds.max.x > mapBounds.max.x)
            {
                newPosition = new Vector3(oldPosition.x, newPosition.y, newPosition.z);
            }

            if (cameraBounds.min.y < mapBounds.min.y || cameraBounds.max.y > mapBounds.max.y)
            {
                newPosition = new Vector3(newPosition.x, oldPosition.y, newPosition.z);
            }
        }

        transform.position = newPosition;
    }

    private Bounds CalculateCameraBounds(Vector3 targetPosition)
    {
        var screenAspect = Screen.width / Screen.height;
        var cameraHeight = m_camera.orthographicSize * 2.0f;

        return new Bounds(
            new Vector3(targetPosition.x, targetPosition.y, 0.0f), 
            new Vector3(cameraHeight * screenAspect, cameraHeight, 0.0f)
        );
    }
}
