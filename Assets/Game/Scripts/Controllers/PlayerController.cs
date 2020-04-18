using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.InputSystem;

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
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        var movement = m_movementAction.ReadValue<Vector2>();
        transform.position += new Vector3(movement.x, movement.y, 0);

        var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var mouseTile = new Vector3Int(Mathf.FloorToInt(mousePos.x), Mathf.FloorToInt(mousePos.y), 0);

        var distance = Vector3.Distance(mouseTile + new Vector3(0.5f, 0.5f, 0), transform.position);
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
        var mouseTile = new Vector3Int(Mathf.FloorToInt(mousePos.x), Mathf.FloorToInt(mousePos.y), 0);
        var tileCentre = mouseTile + new Vector3(0.5f, 0.5f, 0);

        var distance = Vector3.Distance(tileCentre, transform.position);
        var blocked = distance > MaxPlacementDistance || CollisionMask.IsTileBlocked(mouseTile);

        if (!blocked)
        {
            Debug.Log("Placed Item");
            var obj = Instantiate(CurrentHeldItem, tileCentre, Quaternion.identity, null);
        }
        else
        {
            Debug.Log("Blocked item");
        }
    }
}
