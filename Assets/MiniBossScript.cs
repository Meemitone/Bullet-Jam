using System;
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
    [SerializeField] private bool state = true; //open
    // Start is called before the first frame update
    void Start()
    {
        pastTime = desync;
        if (state)
            desync = UnwindOpen(desync, out state);
        else
            desync = UnwindClosed(desync, out state);
    }

    private float UnwindOpen(float desync, out bool cond)
    {
        if (desync < openTime)
        {
            cond = true;
            return desync;
        }
        return UnwindClosed(desync - openTime, out cond);
    }

    private float UnwindClosed(float desync, out bool cond)
    {
        if (desync < closeTime)
        {
            cond = false;
            return desync;
        }
        return UnwindClosed(desync - openTime, out cond);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        pastTime += Time.deltaTime;
        if(state && pastTime > openTime)
        {
            state = !state;
            anim.SetTrigger("Change");
        }
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
