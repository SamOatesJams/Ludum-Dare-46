using SamOatesGames.Systems;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{
    public float MaxPlacementDistance = 1.5f;
    public GameObject CurrentHeldItem;

    public Tile HighlightTileGreen;
    public Tile HighlightTileBlocked;

    public CollisionMask CollisionMask;
    public Tilemap HighlightLayer;

    private PlayerInput m_playerInput;
    private InputAction m_movementAction;
    private Vector3Int m_lastHighlightTile;
    private bool m_blocked;

    // Start is called before the first frame update
    void Start()
    {
        m_playerInput = GetComponent<PlayerInput>();
        m_movementAction = m_playerInput.currentActionMap.FindAction("Move");

        var eventAggregator = EventAggregator.GetInstance();
        eventAggregator.Publish(new SetCameraFollowTransformEvent(transform));
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        var movement = m_movementAction.ReadValue<Vector2>();
        transform.position += new Vector3(movement.x, movement.y, 0);

        var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos = new Vector3(mousePos.x, mousePos.y, 0);
        var mouseTile = CollisionMask.ToTilemapLoc(mousePos);

        var distance = Vector3.Distance(mousePos, transform.position);
        var blocked = distance > MaxPlacementDistance || CollisionMask.IsTileBlocked(mouseTile);

        if (m_lastHighlightTile != mouseTile || m_blocked != blocked)
        {
            HighlightLayer.SetTile(m_lastHighlightTile, null);
            HighlightLayer.SetTile(mouseTile, blocked ? HighlightTileBlocked : HighlightTileGreen);
            m_lastHighlightTile = mouseTile;
            m_blocked = blocked;
        }
    }

    public void OnPlaceItem(InputValue value)
    {
        var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos = new Vector3(mousePos.x, mousePos.y, 0);
        var mouseTile = CollisionMask.ToTilemapLoc(mousePos);

        var tileCentre = mouseTile + new Vector3(1f, 1f, 0);

        var distance = Vector3.Distance(tileCentre, transform.position);
        var blocked = distance > MaxPlacementDistance || CollisionMask.IsTileBlocked(mouseTile);

        if (!blocked)
        {
            Debug.Log("Placed Item");
            var obj = Instantiate(CurrentHeldItem, tileCentre, Quaternion.identity, null);
            CollisionMask.SetTile(mouseTile, ItemType.WoodSpikes);
        }
        else
        {
            Debug.Log("Blocked item");
        }
    }
}
