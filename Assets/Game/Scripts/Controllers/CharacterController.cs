using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterController : MonoBehaviour
{
    private PlayerInput m_playerInput;
    private InputAction m_movementAction;

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
    }
}
