using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class Groot_Behaviour : MonoBehaviour
{
    private enum States
    {
        FindCover, //DONE (I think?) (gotta do the meaty get position for new cover code)
        Shoot, //DONE
        Firing, //DONE
        Strafe, //
        Die, //Initialise death subroutine and then enter Empty
        Empty //Do nothing as death resolves DONE
    }

    [SerializeField] private States state;
    [SerializeField] private NavMeshAgent nav;
    [SerializeField] private GameObject player;
    [SerializeField] private Gun_Controller gun;
    [SerializeField] private float aggression = 0f; //%chance that groot tries to shoot you
    [SerializeField] private float aggroRate = 0.01f;
    [SerializeField] private float cowardMultiplier = 2.5f;
    [SerializeField] private float strafeChanceMaximum = 1 / 3;
    [SerializeField] private float minStrafeRange, maxStrafeRange;
    [SerializeField] private int hp = 7;
    private float initacc;
    // Start is called before the first frame update
    void Start()
    {
        nav = GetComponent<NavMeshAgent>();
        initacc = nav.acceleration;
        gun = GetComponentInChildren<Gun_Controller>();
        GameObject[] Group = FindObjectsOfType<GameObject>();
        player = null;
        foreach (GameObject g in Group)
        {
            if (g.CompareTag("Player"))
                player = g;
        }
        if (player == null)
        {
            Debug.Log("Enemy can't find player");
            Destroy(transform.parent.gameObject);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        switch (state)//ment
        {
            case States.FindCover:
                if (Vector3.Distance(nav.destination, transform.position) < 0.03)
                {
                    //I am at the cover I intended to get to
                    aggression += aggroRate * cowardMultiplier;
                    RaycastHit coverChecker = new RaycastHit();
                    Physics.Raycast(transform.position, player.transform.position - transform.position, out coverChecker, (player.transform.position - transform.position).magnitude);
                    if (coverChecker.collider.gameObject == player) //did we hit the player when targeted?
                    {
                        state = getNewState();
                    }
                    if (aggression > 0.5)
                    {
                        state = getNewState();
                    }
                }
                else
                {
                    aggression += aggroRate;
                    if (aggression > 0.8)
                    {
                        state = getNewState();
                    }
                }
                break;

            case States.Shoot:
                //check line of sight
                RaycastHit shootChecker = new RaycastHit();
                Physics.Raycast(transform.position, player.transform.position - transform.position, out shootChecker, (player.transform.position - transform.position).magnitude);
                if (shootChecker.collider.gameObject == player) //did we hit the player when targeted?
                {
                    //fire
                    if (callOfDutyShootAMan())
                    {
                        aggression -= 0.2f;
                        state = States.Firing;
                        //animation bool on goes here
                    }
                    else
                    {
                        nav.SetDestination(transform.position);
                    }
                }
                else
                {
                    //move towards player
                    nav.SetDestination(player.transform.position);
                }
                break;

            case States.Firing:
                nav.speed = 0;
                nav.acceleration = 0;
                if (gun.State < 2)
                {
                    //animation bool turn off here
                    nav.acceleration = initacc;
                    state = getNewState();
                }
                break;

            case States.Strafe:
                aggression += aggroRate;//increase aggression

                Physics.Raycast(transform.position, player.transform.position - transform.position, out shootChecker, (player.transform.position - transform.position).magnitude);
                if (shootChecker.collider.gameObject == player && aggression > 0.7) //Can we shoot the player?
                {
                    state = getNewState();
                }
                else
                {
                    if (Vector3.Distance(nav.destination, transform.position) < 0.03)
                        state = getNewState();
                }


                if (Vector3.Distance(nav.destination, transform.position) < 0.03)//if close enough to where was going
                {
                    getNewState();//reroll priorities
                }
                break;

            case States.Die:
                GetComponentInChildren<Animator>().enabled = false;//deactivate the animator
                Collider[] finder = GetComponentsInChildren<Collider>();
                foreach (Collider c in finder)
                    c.gameObject.SetActive(true); //find each collider in this things children, ie groot upp and groot lower, and set them active
                state = States.Empty; //do no more after the next line
                Destroy(gameObject, Random.Range(3, 5)); //remove this, the prefab model, but leave the bullets
                break;

            default:
                break;
        }
    }

    private bool callOfDutyShootAMan()
    {
        if (gun.State == 0)
        {
            if (Random.value <= 0.75)
            {
                gun.Fire(1, true);
                return true;
            }
            else
            {
                gun.Fire(2, true);
                return true;
            }
        }
        return false;
    }

    private States getNewState()
    {
        float randres = Random.Range(0f, 1f);
        float gap = Mathf.Sin(aggression * Mathf.PI) * strafeChanceMaximum;
        float attack = aggression - gap / 2;
        if (randres <= attack)
        {
            return States.Shoot;
        }
        else if (randres <= attack + gap)
        {
            nav.SetDestination(transform.position);
            return States.Strafe;
        }
        else
        {
            CoverTarget();
            return States.FindCover;
        }
    }

    private void StrafeTarget(int attempt)
    {
        if (attempt > 10)
        {
            getNewState();
            return;
        }

        Vector3[] targets = new Vector3[4];
        float[] dists = new float[4];

        float offset = Random.Range(0, maxStrafeRange - minStrafeRange);
        Vector3 randomPoint = Random.insideUnitSphere;
        randomPoint.y = 0;
        randomPoint *= offset;

        targets[0] = randomPoint;
        targets[1] = -randomPoint;
        targets[2] = new Vector3(randomPoint.z, 0, -randomPoint.x);
        targets[3] = -targets[2];

        for (int i = 0; i < 4; i++)
        {
            dists[i] = Vector3.Distance(transform.position, targets[i]);
        }

        float mindist = Mathf.Min(dists);
        for (int i = 0; i < 4; i++)
        {
            if (dists[i] == mindist)
            {
                NavMeshPath path = new NavMeshPath();
                nav.SetDestination(player.transform.position + targets[i]);
                nav.CalculatePath(targets[i], path);
                if (path.status == NavMeshPathStatus.PathComplete)
                {
                    return;
                }
            }
        }
        StrafeTarget(attempt + 1);
    }

    private void CoverTarget()
    {
        RaycastHit shootChecker = new RaycastHit();
        Physics.Raycast(transform.position, player.transform.position - transform.position, out shootChecker, (player.transform.position - transform.position).magnitude);
        if (shootChecker.collider.gameObject == player) //did we hit the player when targeted?
        {
            //we are not currently in cover
            Vector3[] testpos = new Vector3[20];
            Vector3 playerToMe = transform.position - player.transform.position;
            playerToMe.y = 0;
            playerToMe.Normalize();
            Vector3 testDir = playerToMe;
            testDir.x = -playerToMe.z;
            testDir.z = playerToMe.x;

            for (int i = 0; i < 10; i++)
            {
                testpos[2 * i] = transform.position + (i + 1) * testDir;
                testpos[2 * i + 1] = transform.position - (i + 1) * testDir;
            }

            //evens are one direction, odds are the other

            //for each testpos, check the raycast for cover
            for (int i = 0; i < 20; i++)
            {
                Physics.Raycast(transform.position, player.transform.position - transform.position, out shootChecker, (player.transform.position - transform.position).magnitude);
                if(shootChecker.collider.gameObject!=player)
                {
                    //cover found
                    Vector3 coverTarg = shootChecker.point;
                    playerToMe = coverTarg - player.transform.position;
                    playerToMe.Normalize();
                    coverTarg += playerToMe;
                    nav.SetDestination(coverTarg);
                }
            }
            aggression += 0.1f;
            getNewState();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player Bullet") && state!= States.Empty && state!= States.Die)
        {
            hp--;
            if(hp <=0)
            {
                state = States.Die;
            }
        }
    }
}
