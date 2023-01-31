using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Info")]
    public float speed;
    public float lasTurnSpeed = 2;
    private Vector3 motion;

    [Header("Dash")]
    public float distance = 2;
    public bool dodging = false;
    public float dashTime;
    private Vector3 dashLocation;


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

        transform.position = new Vector3(transform.position.x, 0, transform.position.z);
    }

    public void OnMove(Vector2 moveInput)
    {
        if (dodging)
        {
            if (Vector3.Distance(transform.position, dashLocation) > 0.5f)
                transform.position = Vector3.Lerp(transform.position, dashLocation, dashTime * Time.deltaTime);
            else
                dodging = false;

        }
        else
        {
            motion = new Vector3(moveInput.x * speed * Time.deltaTime, 0, moveInput.y * speed * Time.deltaTime);

            playerControl.Move(motion);
        }

    }

    public void OnLook(Vector2 lookInput)
    {

        Quaternion lookRoatation = Quaternion.LookRotation(new Vector3(lookInput.x, 0, lookInput.y), Vector3.up);

        if(GetComponent<PlayerGun>().laserActive)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, lookRoatation, lasTurnSpeed * Time.deltaTime);
            return;
        }
        if(!dodging)
            transform.rotation = lookRoatation;

    }

    public void Dodge(InputAction.CallbackContext context)
    {

        RaycastHit hit;
        if (context.phase.ToString() == "Started" && !dodging)
        {
            float dashDistance = distance;
            playerControl.Move(new Vector3());

            if (motion != new Vector3(0, 0, 0))
            {
                Vector3 directionFinder = Vector3.ClampMagnitude(motion, 1);
                transform.rotation = Quaternion.LookRotation(directionFinder);
            }
            if (Physics.Raycast(transform.position + new Vector3(0, 1, 0), transform.forward, out hit, distance + 0.2f))
            {
                Debug.Log(hit.transform.name);
                dashDistance = hit.distance - 0.7f;

                if (dashDistance < 0.5f)
                    return;
            }
            dashLocation = transform.position + transform.forward * dashDistance;
            dodging = true;

            Debug.Log(Vector3.Distance(transform.position, dashLocation) + " : " + dashDistance);

        }

    }

    private void OnCollisionEnter(Collision collision)
    {

        dodging = false;

    }

}
