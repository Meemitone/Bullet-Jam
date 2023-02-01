using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RoomScript : MonoBehaviour
{
    public PlayerMovement player;
    public bool roomActive = false;
    public GameObject[] plannedEnemies;
    public List<GameObject> spawnedEnemies;
    public int enemyIndex;
    public int enemyCount;
    

    public GameObject[] allSpawners;
    public List<GameObject> spawners;
    public GameObject[] allDoors;
    public List<GameObject> doors;
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<MeshRenderer>().enabled = false;
        player = FindObjectOfType<PlayerMovement>();
            
        float xDist = gameObject.transform.localScale.x/2;
        float zDist = gameObject.transform.localScale.z/2;
        
        allSpawners = GameObject.FindGameObjectsWithTag("Spawner");
        for (int i = 0; i < allSpawners.Length; i++)
        {
            if (allSpawners[i].transform.position.x - transform.position.x < xDist && allSpawners[i].transform.position.z - transform.position.z < zDist)
            {
                spawners.Add(allSpawners[i]);
            }
        }
        
        allDoors = GameObject.FindGameObjectsWithTag("Door");
        for (int i = 0; i < allDoors.Length; i++)
        {
            if (allDoors[i].transform.position.x - transform.position.x < xDist && allDoors[i].transform.position.z - transform.position.z < zDist)
            {
                doors.Add(allDoors[i]);
                doors[i].SetActive(false);
            }
        }
        
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void spawnEnemy(GameObject Enemy)
    {
        float[] distancesToPlayer = new float[spawners.Count];
        float[] spawnChances = new float[spawners.Count];
        float totalDist = 0;
        Vector3 spawnPos = Vector3.zero;

        for (int i = 0; i < spawners.Count; i++)
        {
            distancesToPlayer[i] = (spawners[i].transform.position - player.transform.position).magnitude; // finds the distances to player and adds them up
            totalDist += distancesToPlayer[i];
        }

        float rand = Random.Range(0, 1f);
        for (int i = 0; i < spawners.Count; i++)
        {
            spawnChances[i] = distancesToPlayer[i] / totalDist; // finds out the chance of using a particular spawner, greater distances have higher chances
            float thisChance = spawnChances[i];
            
            for (int j = 0; j < i; j++)
            {
                thisChance += spawnChances[j]; // if rand is less than this chance + all chances below this, so for a 60% chance is after a 10% chance, use that one instead
            }

            if (rand < thisChance)
            {
                spawnPos = spawners[i].transform.position;
            }
        }
        
        Instantiate(Enemy, spawnPos, Quaternion.Euler(Vector3.back));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other == player)
        {
            for (int i = 0; i < doors.Count; i++)
            {
                doors[i].SetActive(true);
            }
        }
    }
}
