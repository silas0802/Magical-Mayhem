using System.Collections;
using System.Collections.Generic;

using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HUDScript : NetworkBehaviour
{
    public static HUDScript instance;
    public UnitController unitController;
    public Image[] spellIcons;
    public TMP_Text healthText;
    public Transform healthbar;
    
    public Image[] cooldowntemplates = new Image[6];
    private float maxHealthBarLenght;

    private float maxHealth;
    float[] totalCooldowns;
    
    
    void Awake()
    {
        instance = this;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
        // foreach (Image item in cooldowntemplates)
        // {
        //     item.gameObject.SetActive(false);           
        // }
        
    }

    // Update is called once per frame
    void Update()
    {
       // CooldownInitiator();
    }

    public void ConnectPlayer(UnitController local)
    {
        unitController = local;
        LoadImages();
        maxHealth = unitController.GetHealth();
        ModyfyHealthbar(1,2);
        Debug.Log(unitController);
    }

    
    public void ModyfyHealthbar(int before,int after){
       
            
        
       Debug.Log(unitController);
        healthText.SetText(unitController.GetHealth()+"/"+maxHealth);
        float percentageHealthMissing = unitController.GetHealth()/maxHealth;
        
        healthbar.GetComponent<Image>().fillAmount = percentageHealthMissing;
        
    }

    public void GetTotalCooldowns(){
        totalCooldowns = unitController.unitCaster.getCooldowns();
        Debug.Log(totalCooldowns[0]);
    }

    public void CooldownInitiator(){
        float[] cooldowns = unitController.unitCaster.getCooldowns();

        for (int i = 0; i < cooldowns.Length; i++)
        {
            cooldowntemplates[i].fillAmount=cooldowns[i]/totalCooldowns[i];
            
        }
    }

    public void LoadImages() 
    {
        int i = 0;
        foreach (Image spellIcon in spellIcons)
        {
            if (unitController.inventory.spells[i])
            {
                spellIcon.color=new Color(255,255,255,255);
                spellIcon.sprite = unitController.inventory.spells[i].icon;
            }else
            {
                spellIcon.color = new Color(0, 0,0,0);
            }
            
            i++;
        }
    }

}
