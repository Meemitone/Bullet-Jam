using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIScript : MonoBehaviour
{
    [Header("Needed stuff")] 
    public GameObject menu;
    public Text ammoText;

    [Header("Images")]
    public Image hearts;
    public Image[] heartGlows;
    public Image chainsawChargeImage;
    public Image chainsawGlow;
    public Image[] weaponLights;
    
    [Header("Numbers")]
    public int weaponIndex = 0;
    public int playerHealth = 8;
    public float chainsawCharge = 0;
    public int smgClip = 0;
    public int smgAmmo = 0;
    public int laserClip = 0;
    public int laserAmmo = 0;

    [Header("Info")]
    public bool paused = false;

    
    
    // Start is called before the first frame update
    void Start()
    {
       Cursor.visible = false;
       Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab)){pauseGame();} // the button above tab
        UiUpdate();
    }

    public void pauseGame()
    {
        paused = !paused; // switches the pause state
        menu.SetActive(paused);
        if (paused) // pauses the game
        {
            Time.timeScale = 0;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else // unpauses the game
        {
            Time.timeScale = 1;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    void UiUpdate()
    {
        //health
        hearts.fillAmount = playerHealth / 8f;
        for (int i = 0; i < 4; i++)
        {
            heartGlows[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < playerHealth/2; i++)
        {
            heartGlows[i].gameObject.SetActive(true);
        }
        
        //weapon
        for (int i = 0; i < 3; i++)
        {
            if (weaponIndex == i)
            {
                weaponLights[i].gameObject.SetActive(true);
            }
            else
            {
                weaponLights[i].gameObject.SetActive(false);
            }
        }
        
        //ammo ∞
        chainsawChargeImage.fillAmount = chainsawCharge;
        if(chainsawCharge >= 1f)
            chainsawGlow.gameObject.SetActive(true);
        else
            chainsawGlow.gameObject.SetActive(false);
        
        switch (weaponIndex)
        {
            case 0:

                ammoText.text = "∞";
                ammoText.gameObject.transform.localScale = new Vector3(1.5f, 1.5f,1);
                break;
            case 1:

                ammoText.text = smgClip + "/" + smgAmmo;
                ammoText.gameObject.transform.localScale = new Vector3(0.5f, 0.5f,1);
                break;
            case 2:

                ammoText.text = laserClip + "/" + laserAmmo;
                ammoText.gameObject.transform.localScale = new Vector3(0.5f, 0.5f,1);
                break;
        }

    }
}
