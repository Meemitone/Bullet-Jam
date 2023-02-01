using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserMail : MonoBehaviour
{
    public int damageSince = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
    }

    public bool laserTicked = false;
    
    public void LaserDamage(float nextTick)
    {
        if(!laserTicked)
        {
            damageSince++;
            laserTicked = true;
            Invoke(nameof(UntickedLaserFunction),nextTick);
        }
    }

    public void UntickedLaserFunction()
    {
        laserTicked = false;
    }
}
