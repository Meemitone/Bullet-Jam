using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Info")]
    public float speed;


    [Header("References")]
    private CharacterController playerControl;
    public BulletJam inputSystem;

    // Start is called before the first frame update
    void Awake()
    {
        playerControl = GetComponent<CharacterController>();
        inputSystem = new BulletJam();

    }

    private void OnEnable()
    {
        inputSystem.Player.Enable();
    }
    private void OnDisable()
    {
        inputSystem.Player.Disable();
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        Vector2 move = inputSystem.Player.Move.ReadValue<Vector2>();

        OnMove(move);

        Vector2 look = inputSystem.Player.Look.ReadValue<Vector2>();

        if(look.x > 0.05f || look.y > 0.05f || look.x < -0.05f || look.y < -0.05f)
        {
             look = Vector2.ClampMagnitude(look, 1);

            OnLook(look);
        }
    }

    public void OnMove(Vector2 moveInput)
    {

        Vector3 motion = new Vector3(moveInput.x * speed * Time.deltaTime, 0, moveInput.y * speed * Time.deltaTime);

        playerControl.Move(motion);

    }

    public void OnLook(Vector2 lookInput)
    {

        Quaternion lookRoatation = Quaternion.LookRotation(new Vector3(lookInput.x, 0, lookInput.y), Vector3.up);

        transform.rotation = lookRoatation;

    }

}
