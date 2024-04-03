using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Buyable : ScriptableObject
{
    [SerializeField,Tooltip("The items ID")]
    private int ID;
    [SerializeField,TextArea(5,10)] 
    private string Description;
    
    [SerializeField] 
    private Sprite Icon;
    
    [SerializeField,Range(1,20), Tooltip("The amount of gold required to purchase this spell")] 
    private int Price;

    public string description => this.Description;
    public Sprite icon => this.Icon;
    public int price => this.Price;
    public int id => this.ID;

    public void SetId(int id){
        ID = id;
    }
}
