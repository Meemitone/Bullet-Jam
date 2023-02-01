using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainsawControl : MonoBehaviour
{

    public GameObject player;
    public BoxCollider chainsaw;

    public void RevSaw()
    {
        chainsaw.enabled = true;


    }

    public void StopSaw()
    {

        chainsaw.enabled = false;
        player.GetComponent<PlayerGun>().chainsawing = false;

    }

}
