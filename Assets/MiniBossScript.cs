using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniBossScript : MonoBehaviour
{
    [SerializeField] private Animator anim;
    [SerializeField] private int hp;
    [SerializeField] private float openTime;
    [SerializeField] private float closeTime;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    private void OnCollisionEnter(Collision other)
    {
        if (LayerMask.NameToLayer("Player Bullets") == other.gameObject.layer)
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
