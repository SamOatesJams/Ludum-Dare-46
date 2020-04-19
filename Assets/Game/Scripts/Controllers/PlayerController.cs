﻿using SamOatesGames.Systems;
using UnityEngine;
using UnityEngine.InputSystem;

public enum PlayerType
{
    Scientist,
    Monster
};

[RequireComponent(typeof(NavMovementController), typeof(PlayerInput))]
public class PlayerController : SubscribableMonoBehaviour
{
    public PlayerType PlayerType;

    [Header("Item placement")]
    public float MaxPlacementDistance = 1.5f;
    public GameObject CurrentHeldItem;
    
    [Header("Movement")]
    public float MovementSpeedModififer = 1.0f;

    [Header("World References")]
    public Transform Target;
    public CollisionMask CollisionMask;

    private PlayerInput m_playerInput;
    private InputAction m_movementAction;

    private bool m_active;

    private NavMovementController m_navigationController;
    private EventAggregator m_eventAggregator;

    private SpriteRenderer m_placementPreview;
    private Color m_disabledPreviewColor = new Color(1.0f, 1.0f, 1.0f, 0.25f);

    void Start()
    {
        m_playerInput = GetComponent<PlayerInput>();
        m_navigationController = GetComponent<NavMovementController>();

        var placementPreview = new GameObject("_Placement_preview", typeof(SpriteRenderer));
        m_placementPreview = placementPreview.GetComponent<SpriteRenderer>();
        m_placementPreview.transform.SetParent(transform);
        m_placementPreview.sortingOrder = 10000;
        m_placementPreview.sprite = CurrentHeldItem.GetComponent<SpriteRenderer>().sprite;

        m_playerInput.DeactivateInput();

        m_eventAggregator = EventAggregator.GetInstance();
        m_eventAggregator.Subscribe<RequestDaytimeEvent>(this, OnRequestDaytimeEvent);
        m_eventAggregator.Subscribe<RequestNighttimeEvent>(this, OnRequestNighttimeEvent);
        m_eventAggregator.Subscribe<SwapItemEvent>(this, OnSwapItem);

        OnRequestDaytimeEvent(new RequestDaytimeEvent());
    }

    private void OnRequestNighttimeEvent(RequestNighttimeEvent obj)
    {
        switch (PlayerType)
        {
            case PlayerType.Scientist:
                NavigateToTarget();
                m_active = false;
                m_playerInput.DeactivateInput();
                break;
            case PlayerType.Monster:
                m_eventAggregator.Publish(new SetCameraFollowTransformEvent(transform));
                m_active = true;
                m_playerInput.ActivateInput();

                m_movementAction = m_playerInput.currentActionMap.FindAction("Move");
                break;
        }
    }

    private void OnRequestDaytimeEvent(RequestDaytimeEvent obj)
    {
        switch (PlayerType)
        {
            case PlayerType.Scientist:
                m_eventAggregator.Publish(new SetCameraFollowTransformEvent(transform));
                m_active = true;
                m_playerInput.ActivateInput();

                m_movementAction = m_playerInput.currentActionMap.FindAction("Move");
                break;
            case PlayerType.Monster:
                NavigateToTarget();
                m_active = false;
                m_playerInput.DeactivateInput();
                break;
        }
    }

    private void NavigateToTarget()
    {
        m_navigationController.NavigateTo(Target.position);
    }

    private void HandleInput()
    {
        var movement = m_movementAction.ReadValue<Vector2>();
        transform.position += new Vector3(movement.x, movement.y, 0);

        if (PlayerType == PlayerType.Scientist)
        {
            var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos = new Vector3(mousePos.x, mousePos.y, 0);
            var mouseTile = CollisionMask.ToTilemapLoc(mousePos);

            var distance = Vector3.Distance(mousePos, transform.position);
            var blocked = distance > MaxPlacementDistance || CollisionMask.IsTileBlocked(mouseTile);

            m_placementPreview.transform.position = mouseTile + new Vector3(1.0f, 1.0f);
            m_placementPreview.color = blocked ? m_disabledPreviewColor : Color.white;
        }
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        if (m_active)
        {
            HandleInput();
        }
    }

    public void PlaceCurrentItem()
    {
        var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos = new Vector3(mousePos.x, mousePos.y, 0);
        var mouseTile = CollisionMask.ToTilemapLoc(mousePos);

        var tileCentre = mouseTile + new Vector3(1f, 1f, 0);

        var distance = Vector3.Distance(tileCentre, transform.position);
        var blocked = distance > MaxPlacementDistance || CollisionMask.IsTileBlocked(mouseTile);

        if (!blocked)
        {
            Instantiate(CurrentHeldItem, tileCentre, Quaternion.identity, null);
            CollisionMask.SetTile(mouseTile, ItemType.WoodSpikes);
        }
    }

    public void OnPlaceItem(InputValue value)
    {
        if (PlayerType != PlayerType.Scientist || !m_active)
        {
            return;
        }

        PlaceCurrentItem();
    }

    private void OnSwapItem(SwapItemEvent itemSwap)
    {
        CurrentHeldItem = itemSwap.item;
        m_placementPreview.sprite = CurrentHeldItem.GetComponent<SpriteRenderer>().sprite;
    }
}
