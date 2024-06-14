using System.Collections;
using System.Collections.Generic;

using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    private float currentHealth;
    float[] totalCooldowns = new float[6];
    
    
    void Awake()
    {
        instance = this;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
        //  foreach (Image item in cooldowntemplates)
        //  {
        //      item.gameObject.SetActive(false);           
        //  }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (RoundManager.instance.roundIsOngoing.Value is true)
        {
            CooldownInitiator();     
        }
       
    }

    public void ConnectPlayer(UnitController local)
    {
        unitController = local;
        LoadImages();
        maxHealth = unitController.unitClass.maxHealth;
        healthText.SetText(maxHealth+"/"+maxHealth);
        healthbar.GetComponent<Image>().fillAmount = 1;
        
        GetTotalCooldowns();
    }

    
    public void ModyfyHealthbar(int currentHealth){
       
            
        
       Debug.Log(unitController);
        healthText.SetText(currentHealth+"/"+maxHealth);
        float percentageHealthMissing = currentHealth/maxHealth;
        
        healthbar.GetComponent<Image>().fillAmount = percentageHealthMissing;
    }

    public void GetTotalCooldowns(){
        int i = 0; 
        Spell[] spells = unitController.inventory.spells;
        foreach (Spell item in spells )
        {
           totalCooldowns[i] = spells[i] is not null ? spells[i].cooldown : 0; 
            i++;    
        }
        
        Debug.Log(totalCooldowns[0]);
    }

    public void CooldownInitiator(){
        float[] cooldowns = unitController.unitCaster.getCooldowns();

        for (int i = 0; i < cooldowns.Length; i++)
        {
            if (true)
            {
                
            }
            cooldowntemplates[i].fillAmount=(cooldowns[i]<=0 ? 0 : cooldowns[i])/totalCooldowns[i];
            
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
