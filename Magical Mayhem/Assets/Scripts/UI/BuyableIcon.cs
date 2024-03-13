using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BuyableIcon : MonoBehaviour
{
    private Image image;
    private Sprite defaultImage;
    
    public Buyable buyable {get; private set;}
    public void Initialize(Buyable buyable){
        image = GetComponent<Image>();
        this.buyable=buyable;
        if (buyable==null)
        {
            image.sprite=defaultImage;
            
        }else{
        image.sprite = buyable.icon;
        
        }
    

        
    }
    
    public void SetColor(Color color){
        image.color=color;
    }
    public void setNull(){
        buyable=null;
    }
    

}
