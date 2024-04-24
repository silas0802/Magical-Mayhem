using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDScript : MonoBehaviour
{
    public static HUDScript instance;
    public UnitController unitController;
    public Image[] spellIcons;

    
    void Awake()
    {
        instance = this;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ConnectPlayer(UnitController local)
    {
        unitController = local;
        LoadImages();
    }

    public void LoadImages() 
    {
        int i = 0;
        foreach (Image spellIcon in spellIcons)
        {
            if (unitController.inventory.spells[i])
            {
                spellIcon.sprite = unitController.inventory.spells[i].icon;
            }
            
            i++;
        }
    }

}
