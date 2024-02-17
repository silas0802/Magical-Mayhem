using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SpellIcon : MonoBehaviour
{
    public Image image;
    public Buyable buyable;
    public void Initialize(Buyable buyable){
        image.sprite = buyable.icon;
        this.buyable=buyable;
        
        
    }
    void Start(){
        Initialize(buyable);
    }
    

}
