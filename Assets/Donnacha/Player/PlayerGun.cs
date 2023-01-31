using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGun : MonoBehaviour
{

    public enum WhichGun { shotgun, smg, laser };

    [Header("Guns and Bullets")]
    public WhichGun currentGun;

    public List<GameObject> bulletPrefabs = new List<GameObject>();
    private GameObject bulletHolder;

    public float laserTime, shotgunTime, smgTime;
    private float diviation;

    private bool firing;
    private bool reloading;

    [Header ("Laser Info")]
    private Vector3 laserPos;
    private bool laserActive;
    public GameObject laserHit;
    public float laserSpeed;
    private LineRenderer laserLine;
    private List<GameObject> laserHits = new List<GameObject>();
    

    [Header("References")]
    public Transform firePoint;
    public LayerMask wall;

    private void Start()
    {
        if (bulletHolder == null)
        {
            bulletHolder = GameObject.Instantiate(new GameObject());
            bulletHolder.name = "Bullet Holder";
        }

        laserLine = firePoint.GetComponent<LineRenderer>();
        laserLine.enabled = false;

    }

    public void Firegun()
    {
        if (firing || reloading)
            return;

        firing = true;
        switch (currentGun)
        {
            default:
                Debug.Log("gunmissing");
                break;
            case (WhichGun.shotgun):
                GameObject firing = GameObject.Instantiate(bulletPrefabs[0], firePoint);
                firing.GetComponent<Shotgun>().Fire(bulletHolder, firePoint);
                Invoke(nameof(FiringEnd), shotgunTime);
                break;
            case (WhichGun.smg):
                GameObject bullet = GameObject.Instantiate(bulletPrefabs[1], firePoint.position, firePoint.rotation * Quaternion.Euler(90, 0, Random.Range(-diviation, diviation)));
                bullet.GetComponent<Rigidbody>().velocity = transform.forward * 20;
                Invoke(nameof(FiringEnd), smgTime);
                break;
            case (WhichGun.laser):
                LaserBegin();
                break;
        }
    }

    public void FiringEnd() => firing = false;

    private void LaserBegin()
    {

        laserPos = firePoint.position;
        laserLine.SetPosition(0, firePoint.position);
        laserLine.SetPosition(1, laserPos);

        laserLine.enabled = true;

        laserActive = true;
        Invoke(nameof(LaserEnd), laserTime);

    }

    private void LaserEnd()
    {
        foreach(GameObject las in laserHits)
        {
            Destroy(las);
        }
        laserHits.Clear();

        laserLine.enabled = false;
        laserActive = false;
        firing = false;

    }

    private void Update()
    {
        if (GetComponent<PlayerMovement>().inputSystem.Player.Fire.IsPressed())
            Firegun();
    }

    private void FixedUpdate()
    {

        if (laserActive)
        {
            RaycastHit hit;

            if(Physics.Raycast(firePoint.position, firePoint.forward, out hit/*, wall*/))
            {
                float laserDistance = Vector3.Distance(firePoint.position, laserPos);
                if (laserDistance < hit.distance)
                    laserPos = Vector3.Lerp(laserPos, hit.point, Time.deltaTime * laserSpeed);
                else if (laserDistance > hit.distance)
                    laserPos = hit.point;

                RaycastHit[] hits = Physics.SphereCastAll(firePoint.position, 0.175f, firePoint.forward, laserDistance);
                int i = 0;
                foreach(RaycastHit touch in hits)
                {

                    if (laserHits.Count <= i)
                        laserHits.Add(GameObject.Instantiate(laserHit));
                    laserHits[i].transform.position = touch.point;
                    laserHits[i].transform.rotation = Quaternion.LookRotation(touch.normal);

                    if(touch.transform.tag == "Enemy")
                    {
                        //damage enemy
                    }

                    i++;
                }
                int saveIndex = i;
                foreach(GameObject las in laserHits)
                    if(laserHits.IndexOf(las) == i)
                    {
                        Destroy(las);
                        i++;
                    }
                if (laserHits.Count >= saveIndex)
                    laserHits.RemoveRange(saveIndex, laserHits.Count - saveIndex);

            }

            laserLine.SetPosition(0, firePoint.position);
            laserLine.SetPosition(1, laserPos);

        }

    }

}
