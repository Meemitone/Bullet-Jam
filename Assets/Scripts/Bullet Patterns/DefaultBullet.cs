using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultBullet : MonoBehaviour
{
    [Tooltip("Speed of the projectile in units/second")]
    [SerializeField] public float Speed;
    [Tooltip("Direction of the projectile (calculated)")]
    [SerializeField] public Vector3 Direction;
    public float MaxTravel = 100;
    private Vector3 start;
    // Start is called before the first frame update
    void Start()
    {
        start = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.localPosition += Direction * Time.deltaTime * Speed;
        if(Vector3.Distance(transform.position, start) > MaxTravel)
        {
            Destroy(gameObject);
        }
    }

    /*
     * Create a function that despawns the bullet when offscreen, see STAGING for details
     * */
}
