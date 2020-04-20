using SamOatesGames.Systems;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public enum PlayerType
{
    Scientist,
    Monster
};

[RequireComponent(typeof(NavMovementController), typeof(PlayerInput))]
public class PlayerController : EntityController
{
    public float HealthRegenAmount = 0.1f;

    [Header("Player Data")]
    public PlayerType PlayerType;

    [Header("Item placement")]
    public float MaxPlacementDistance = 1.5f;
    public ItemDescription CurrentHeldItem;
    
    [Header("Movement")]
    public float MovementSpeedModififer = 1.0f;

    [Header("Attack")]
    public double AttackDamage = 1.0;
    public double AttackRange = 1.0;
    public double AttackDelay = 1.0;
    public LayerMask AttackLayerMask;
    public AudioClip[] AttackAudioClips;
    public AudioClip[] HurtAudioClips;

    [Header("World References")]
    public Transform Target;
    public CollisionMask CollisionMask;

    [Header("Audio Clips")]
    public AudioClip PlaceItemAudioClip;
    public AudioClip FailedPlaceItemAudioClip;
    public AudioClip NotEnoughResourcesAudioClip;
    public AudioClip PickupItemAudioClip;
    public AudioClip[] WaveCompleteAudioClips;

    private PlayerInput m_playerInput;
    private InputAction m_movementAction;

    private bool m_active;

    private float m_lastAttackTime;

    private NavMovementController m_navigationController;
    private EventAggregator m_eventAggregator;
    private InventorySystem m_inventorySystem;

    private SpriteRenderer m_placementPreview;
    private readonly Color m_disabledPreviewColor = new Color(1.0f, 1.0f, 1.0f, 0.25f);

    private AnimationController m_animationController;

    void Start()
    {
        m_playerInput = GetComponent<PlayerInput>();
        m_navigationController = GetComponent<NavMovementController>();
        m_animationController = GetComponent<AnimationController>();

        if (PlayerType == PlayerType.Scientist)
        {
            var placementPreview = new GameObject("_Placement_preview", typeof(SpriteRenderer));
            m_placementPreview = placementPreview.GetComponent<SpriteRenderer>();
            m_placementPreview.transform.SetParent(transform);
            m_placementPreview.sortingOrder = 10000;
            m_placementPreview.sprite = CurrentHeldItem.PreviewImage;
        }

        m_playerInput.DeactivateInput();

        m_inventorySystem = InventorySystem.GetInstance();

        m_eventAggregator = EventAggregator.GetInstance();
        m_eventAggregator.Subscribe<RequestDaytimeEvent>(this, OnRequestDaytimeEvent);
        m_eventAggregator.Subscribe<RequestNighttimeEvent>(this, OnRequestNighttimeEvent);
        m_eventAggregator.Subscribe<SwapItemEvent>(this, OnSwapItem);
        m_eventAggregator.Subscribe<EndWaveEvent>(this, OnEndWaveEvent);

        OnRequestDaytimeEvent(new RequestDaytimeEvent());

        Health = MaxHealth;
    }

    private void OnEndWaveEvent(EndWaveEvent args)
    {
        if (PlayerType == PlayerType.Scientist)
        {
            var audioClip = WaveCompleteAudioClips[Random.Range(0, WaveCompleteAudioClips.Length)];
            m_eventAggregator.Publish(
                new PlayShiftedAudioEvent(
                    AudioIds.Doctor, 
                    audioClip, 
                    new Vector2(0.9f, 1.1f),
                    0.75f)
                );
        }
    }

    public void DamagePlayer(double amount)
    {
        Health -= amount;

        m_eventAggregator.Publish(new PlayShiftedAudioEvent(
            AudioIds.MonsterAttack,
            HurtAudioClips[Random.Range(0, HurtAudioClips.Length)],
            new Vector2(0.5f, 1.0f),
            0.75f));

        if (Health <= 0)
        {
            m_eventAggregator.Publish(new PlayerDiedEvent(this));
            m_eventAggregator.Publish(new GameOverEvent());
            m_active = false;
        }
    }

    private void OnRequestNighttimeEvent(RequestNighttimeEvent obj)
    {
        switch (PlayerType)
        {
            case PlayerType.Scientist:
                m_placementPreview.enabled = false;
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
                m_placementPreview.enabled = true;
                m_eventAggregator.Publish(new SetCameraFollowTransformEvent(transform));
                m_active = true;
                m_playerInput.ActivateInput();

                m_movementAction = m_playerInput.currentActionMap.FindAction("Move");
                break;
            case PlayerType.Monster:
                NavigateToTarget();
                m_active = false;
                m_playerInput.DeactivateInput();

                Health = Math.Max(Health + (HealthRegenAmount * MaxHealth), MaxHealth);
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

        if (Time.time - m_lastAttackTime > AttackDelay)
        {
            m_animationController.SetPunchTarget(null);
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

        if (blocked)
        {
            m_eventAggregator.Publish(new PlayShiftedAudioEvent(AudioIds.FailedPlaceItem, FailedPlaceItemAudioClip, new Vector2(0.5f, 1.0f)));
            return;
        }

        var resourceCount = m_inventorySystem.GetItemAmount(CurrentHeldItem.ResourceUsed);
        if (resourceCount < CurrentHeldItem.NumberOfResourcesUsed)
        {
            m_eventAggregator.Publish(new PlayShiftedAudioEvent(AudioIds.FailedPlaceItem, NotEnoughResourcesAudioClip, new Vector2(1.0f, 2.0f)));
            return;
        }

        var trapObj = Instantiate(CurrentHeldItem.Prefab, tileCentre, Quaternion.identity, null);
        var itemData = trapObj.GetComponent<PlaceableItem>();
        itemData.SwapCollisionMask(CollisionMask, mouseTile);
        m_eventAggregator.Publish(new PlayShiftedAudioEvent(AudioIds.PlaceItem, PlaceItemAudioClip, new Vector2(1.0f, 2.0f)));

        m_inventorySystem.UseItem(CurrentHeldItem.ResourceUsed, CurrentHeldItem.NumberOfResourcesUsed);
    }

    public void Attack()
    {
        // Delay between attacks
        if (Time.time - m_lastAttackTime <= AttackDelay)
        {
            return;
        }

        var mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        // Get attacked enemy
        var ray = Physics2D.Raycast(mousePos, Vector2.zero, float.PositiveInfinity, AttackLayerMask);
        if (ray.collider == null)
        {
            return;
        }

        // Only allow attacks on nearby enemies
        if (Vector3.Distance(ray.transform.position, transform.position) >= AttackRange)
        {
            return;
        }

        var enemy = ray.transform.GetComponent<EnemyController>();
        if (enemy == null)
        {
            return;
        }

        Debug.Log("Attacking enemy");
        enemy.PlayerAttack(this, AttackDamage);
        m_eventAggregator.Publish(new PlayShiftedAudioEvent(
            AudioIds.MonsterAttack, 
            AttackAudioClips[Random.Range(0, AttackAudioClips.Length)],
            new Vector2(0.5f, 1.0f), 
            0.75f));

        m_lastAttackTime = Time.time;

        m_animationController.SetPunchTarget(enemy.transform);
    }

    public void OnPlaceItem(InputValue value)
    {
        if (PlayerType != PlayerType.Scientist || !m_active)
        {
            return;
        }

        PlaceCurrentItem();
    }

    public void OnAttack(InputValue value)
    {
        if (PlayerType != PlayerType.Monster || !m_active)
        {
            return;
        }

        Attack();
    }

    private void OnSwapItem(SwapItemEvent itemSwap)
    {
        if (PlayerType != PlayerType.Scientist)
        {
            return;
        }

        CurrentHeldItem = itemSwap.ItemDescription;
        m_placementPreview.sprite = CurrentHeldItem.PreviewImage;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var pickupableResource = other.GetComponent<PickupableResource>();
        if (pickupableResource != null)
        {
            m_inventorySystem.AddItem(pickupableResource.Type, 1);
            m_eventAggregator.Publish(new PlayShiftedAudioEvent(AudioIds.PickupItem, PickupItemAudioClip, new Vector2(1.0f, 2.0f)));
            Destroy(pickupableResource.gameObject);
        }
    }
}
