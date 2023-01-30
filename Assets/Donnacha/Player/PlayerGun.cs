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

    private bool firing;

    [Header ("Laser Stuff")]
    public float laserTime;
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
        if (firing)
            return;

        firing = true;
        switch (currentGun)
        {
            default:
                Debug.Log("gunmissing");
                break;
            case (WhichGun.shotgun):
                SpawnBullet(0);
                break;
            case (WhichGun.smg):
                SpawnBullet(1);
                break;
            case (WhichGun.laser):
                LaserBegin();
                break;
        }
    }

    private void SpawnBullet(int index)
    {
        

        GameObject newBullet = GameObject.Instantiate(bulletPrefabs[index], transform.position, transform.rotation, bulletHolder.transform);

    }

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

    private void FixedUpdate()
    {

        if (laserActive)
        {
            RaycastHit hit;

            if(Physics.Raycast(firePoint.position, firePoint.forward, out hit/*, wall*/))
            {
                Debug.Log(hit.transform.name);
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
