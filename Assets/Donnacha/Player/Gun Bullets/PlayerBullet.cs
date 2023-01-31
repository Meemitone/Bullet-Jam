using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{

    public int damage = 1;

    // Start is called before the first frame update
    void Start()
    {

        Invoke(nameof(DeleteMe), 1);

    }

    private void DeleteMe()
    {
        Destroy(gameObject);
    }
}
