using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BuyableIcon : MonoBehaviour
{
    private Image image;
    private Sprite defaultImage;
    [SerializeField]private TMP_Text cost;
    
    public Buyable buyable {get; private set;}
    public void Initialize(Buyable buyable, bool owned){
        image = GetComponent<Image>();
        this.buyable=buyable;
        if (buyable==null)
        {
            image.sprite=defaultImage;
            cost.text="";
            
        }
        else
        {
            image.sprite = buyable.icon;
            if (owned)
            {
                cost.text ="";    
            }
            else{
                cost.text = buyable.price.ToString();
            }
        
        }
    

        
    }
    
    public void SetColor(Color color){
        image.color=color;
    }
    public void setNull(){
        buyable=null;
    }
    

}
