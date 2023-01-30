using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Default_Enemy_Behaviour : MonoBehaviour
{
    public Gun_Controller gun;
    public int State = 0;
    public Vector3 centerpos;
    private Vector3 target;
    public float outersize;
    private float time = 0;
    private float stoptime = 0;
    public float TimeToStop = 1;
    private float speed = 1;
    private float ResumeTime = 0;
    public float TimeToResume = 1;
    /*
     * Possible Enemy States
     *      Patroling
     *      Stopping
     *      Firing
     *      Resuming
     * */

    // Start is called before the first frame update
    void Start()
    {
        centerpos = transform.position;
        gun = GetComponentInChildren<Gun_Controller>();
    }

    // Update is called once per frame
    void Update()
    {
        if (State == 0)
        {
            time += Time.deltaTime;
            float movZ = outersize * Mathf.Sin(time);
            Vector3 difference = new Vector3(0, 0, movZ);
            target = centerpos + difference;
            if(gun.State == 0)
            {
                State = 1;
            }
        }
        else if(State == 1)
        {
            stoptime += Time.deltaTime;
            stoptime = Mathf.Clamp(stoptime, 0, TimeToStop);
            time += Time.deltaTime * (TimeToStop - stoptime) / (TimeToStop);
            float movZ = outersize * Mathf.Sin(time);
            Vector3 difference = new Vector3(0, 0, movZ);
            target = centerpos + difference;
            if(stoptime == TimeToStop)
            {
                State = 2;
                gun.Fire();
                stoptime = 0;
            }
        }
        else if (State == 2)
        {
            if (gun.State < 2)
            {
                State = 3;
            }
        }
        else if (State == 3)
        {
            ResumeTime += Time.deltaTime;
            ResumeTime = Mathf.Clamp(ResumeTime, 0, TimeToResume);
            time += Time.deltaTime * (ResumeTime) / (TimeToResume);
            float movZ = outersize * Mathf.Sin(time);
            Vector3 difference = new Vector3(0, 0, movZ);
            target = centerpos + difference;
            if (ResumeTime == TimeToStop)
            {
                State = 0;
                ResumeTime = 0;
            }
        }
        transform.position = Vector3.Lerp(transform.position, target, speed);
    }

}
