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
        if (buyable==null)
        {
            image.sprite=defaultImage;

        }else{
        image.sprite = buyable.icon;
        this.buyable=buyable;
        }
    

        
    }
    
    public void SetColor(Color color){
        image.color=color;
    }
    
    

}
