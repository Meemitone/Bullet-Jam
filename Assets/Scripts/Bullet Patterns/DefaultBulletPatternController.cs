using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultBulletPatternController : MonoBehaviour
{
    [Header("This Bullet Pattern Script Does Nothing")]
    public int dummy;

    [Header("This entity spawned these bullets (assigned by this script)")]
    public Gun_Controller Spawner;
    // Start is called before the first frame update
    void Start()
    {
        Spawner = transform.parent.parent.GetComponentInChildren<Gun_Controller>();
        Spawner.CompleteFire();//this bullet pattern is so simple that it's finished firing immediately, there's no circular pattern spawned one at a time or something
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.childCount == 0)
        {
            Destroy(gameObject);
        }
    }
}
