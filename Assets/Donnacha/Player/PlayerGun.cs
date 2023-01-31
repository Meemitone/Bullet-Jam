using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerGun : MonoBehaviour
{

    public enum WhichGun { shotgun, smg, laser };

    [Header("Guns and Bullets")]
    public WhichGun currentGun; [Tooltip("Don't touch use bumpers/ E & Q")]
    public List<GameObject> bulletPrefabs = new List<GameObject>(); [Tooltip("Index 0 = Shotgun | Index 1 = SMG")]
    private GameObject bulletHolder; 


    private int gunIndex = 0;
    private int gunCount = 1;

    private bool firing;
    private bool reloading;

    [Header("Laser Info")]
    public bool laserActive;
    public GameObject laserHit;
    public float laserSpeed;
    public float laserUpTime; [Tooltip("How long laser lasts in total")]
    private LineRenderer laserLine;
    private List<GameObject> laserHits = new List<GameObject>();
    private Transform laserPos;
    public int laserInventoryAmmo = 10;


    [Header("SMG Info")]
    public float maxDiviation; [Tooltip("Diviation on both sides")]
    public float diviationMulti;
    public float smgBulletSpeed;
    public float smgFireRate;
    private float diviation;
    public int smgInventoryAmmo = 100;
    public int smgClipMax = 20;
    public int smgAmmoCurrent;

    [Header("Shotgun Info")]
    public float shotgunFireRate;


    [Header("References")]
    public Transform firePoint; [Tooltip("Location for bullets to begin")]
    public LayerMask walls;
    public LayerMask sparksMask;

    private void Start()
    {
        if (bulletHolder == null)
        {
            bulletHolder = GameObject.Instantiate(new GameObject());
            bulletHolder.name = "Bullet Holder";
        }

        laserLine = firePoint.GetComponent<LineRenderer>();
        laserLine.enabled = false;

        laserPos = GameObject.Instantiate(new GameObject()).transform;

        currentGun = WhichGun.shotgun;

        smgAmmoCurrent = smgClipMax;

    }

    public void Firegun()
    {
        if (firing || reloading || GetComponent<PlayerMovement>().dodging)
            return;

        firing = true;
        switch (currentGun)
        {
            default:
                Debug.Log("gunmissing");
                break;
            case (WhichGun.shotgun):
                GameObject fired = GameObject.Instantiate(bulletPrefabs[0], firePoint);
                fired.GetComponent<Shotgun>().Fire(bulletHolder, firePoint);
                Invoke(nameof(FiringEnd), shotgunFireRate);
                break;
            case (WhichGun.smg):
                if (smgAmmoCurrent == 0)
                {

                    reloading = true;
                    firing = false;
                    ReloadSMG();
                }
                smgAmmoCurrent--;
                GameObject bullet = GameObject.Instantiate(bulletPrefabs[1], firePoint.position, firePoint.rotation * Quaternion.Euler(90, 0, Random.Range(-diviation, diviation)), bulletHolder.transform);
                bullet.GetComponent<Rigidbody>().velocity = bullet.transform.up * smgBulletSpeed;
                Invoke(nameof(FiringEnd), smgFireRate);
                break;
            case (WhichGun.laser):
                if (laserInventoryAmmo == 0)
                {
                    firing = false;
                    return;
                }
                laserInventoryAmmo--;
                LaserBegin();
                break;
        }
    }

    public void ReloadSMG()
    {
        if (smgInventoryAmmo >= smgClipMax)
        {
            smgAmmoCurrent = smgClipMax;
            smgInventoryAmmo -= smgClipMax;
        }
        else
        {
            smgAmmoCurrent = smgInventoryAmmo;
            smgInventoryAmmo = 0;
        }

        if (smgAmmoCurrent != 0)
            Invoke(nameof(EndReload), 1.5f);
        else
            reloading = false;

    }

    public void EndReload()
    {
        reloading = false;
    }

    public void FiringEnd() => firing = false;

    private void LaserBegin()
    {

        laserPos.position = firePoint.position;
        laserLine.SetPosition(0, firePoint.position);
        laserLine.SetPosition(1, laserPos.position);

        laserLine.enabled = true;

        laserActive = true;
        Invoke(nameof(LaserEnd), laserUpTime);

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
        {
            Firegun();

            if (currentGun == WhichGun.smg && diviation < maxDiviation)
            {
                diviation += diviationMulti * Time.deltaTime;
                if (diviation > maxDiviation)
                    diviation = maxDiviation;
            }
        }
        else
            diviation = 0;
    }

    private void FixedUpdate()
    {
        if (laserActive)
        {
            RaycastHit hit;

            if(Physics.Raycast(firePoint.position, firePoint.forward, out hit, 1000, layerMask: walls))
            {
                float laserDistance = Vector3.Distance(firePoint.position, laserPos.position);
                if (laserDistance < hit.distance)
                    laserPos.position = Vector3.Lerp(laserPos.position, hit.point, Time.deltaTime * laserSpeed);
                else if (laserDistance > hit.distance)
                    laserPos.position = hit.point;

                laserLine.SetPosition(0, firePoint.position);
                laserLine.SetPosition(1, laserPos.position);

                RaycastHit[] hits = Physics.SphereCastAll(firePoint.position, laserLine.startWidth / 2, firePoint.forward, laserDistance, layerMask: sparksMask);
                RaycastHit[] hits1 = Physics.SphereCastAll(laserPos.position + -firePoint.forward * laserLine.startWidth, laserLine.startWidth / 2, -firePoint.forward, laserDistance, layerMask: sparksMask);

                if (hits.Length == 0)
                    return;
                int i = 0;
                foreach(RaycastHit touch in hits)
                {

                    if (laserHits.Count <= i)
                        laserHits.Add(GameObject.Instantiate(laserHit, new Vector3(-200, -200, -200), transform.rotation));
                    laserHits[i].transform.position = touch.point;
                    laserHits[i].transform.rotation = Quaternion.LookRotation(touch.normal);
                    laserHits[i].name = touch.transform.name + " Sparks";

                    if(touch.transform.tag == "Enemy")
                    {
                        //damage enemy
                    }

                    i++;
                }
                foreach (RaycastHit touch in hits1)
                {

                    if (laserHits.Count <= i)
                        laserHits.Add(GameObject.Instantiate(laserHit, new Vector3(-200, -200, -200), transform.rotation));
                    laserHits[i].transform.position = touch.point;
                    laserHits[i].transform.rotation = Quaternion.LookRotation(touch.normal);
                    laserHits[i].name = touch.transform.name + " Sparks";
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
                    laserHits.RemoveRange(saveIndex, laserHits.Count - saveIndex );

            }
        }

    }

    public void OnSwapWeaponRight(InputAction.CallbackContext action)
    {
        if(action.phase.ToString() == "Started" || action.phase.ToString() == "Started" && currentGun == WhichGun.laser  && !firing)
        {
            if (gunIndex < 3 - 1)
                gunIndex++;
            else
                gunIndex = 0;

            switch (gunIndex)
            {
                default:
                    currentGun = WhichGun.shotgun;
                    break;
                case (1):
                    currentGun = WhichGun.smg;
                    break;
                case (2):
                    currentGun = WhichGun.laser;
                    break;
            }
        }
    }

    public void OnSwapWeaponLeft(InputAction.CallbackContext action)
    {
        if (action.phase.ToString() == "Started" || action.phase.ToString() == "Started" && currentGun == WhichGun.laser && !firing)
        {
            if (gunIndex > 0)
                gunIndex--;
            else
                gunIndex = 2;

            switch (gunIndex)
            {
                default:
                    currentGun = WhichGun.shotgun;
                    break;
                case (1):
                    currentGun = WhichGun.smg;
                    break;
                case (2):
                    currentGun = WhichGun.laser;
                    break;
            }
        }
    }

    public void AddGun()
    {
        gunCount++;
    }

}
