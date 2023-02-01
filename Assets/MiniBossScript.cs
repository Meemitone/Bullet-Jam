using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniBossScript : MonoBehaviour
{
    [SerializeField] private Animator anim;
    [SerializeField] private Vulnerability V;
    [SerializeField] private int hp;
    [SerializeField] private float desync;
    [SerializeField] private float openTime;
    [SerializeField] private float closeTime;
    [SerializeField] private float pastTime;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
    }



    private void OnCollisionEnter(Collision other)
    {
        if (V.vuln && (LayerMask.NameToLayer("BulletKiller") == other.gameObject.layer || LayerMask.NameToLayer("Player Bullets") == other.gameObject.layer))
        {
            hp--;
            if (hp <= 0)
            {
                //SHI NE 
            }
            Destroy(other.gameObject);
        }
    }
}
