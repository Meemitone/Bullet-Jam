using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultBullet : MonoBehaviour
{
    [Tooltip("Speed of the projectile in units/second")]
    [SerializeField] private float Speed;
    [Tooltip("Direction of the projectile (calculated)")]
    [SerializeField] private Vector3 Direction;
    [Tooltip("Player GameObject (found by script)")]
    [SerializeField] private GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        GameObject[] playerGroup = FindObjectsOfType<GameObject>();
        foreach(GameObject g in playerGroup)
        {
            if (g.CompareTag("Player"))
                player = g;
        }
        if(player == null)
        {
            Debug.Log("Bullet can't find player");
            Destroy(this.gameObject);
        }
        Direction = player.transform.position - transform.position;
        Direction.y = 0;
        Direction.Normalize();
        Direction *= Speed;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += Direction * Time.deltaTime;
    }

    /*
     * Create a function that despawns the bullet when offscreen, see STAGING for details
     * */
}
