using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Game/Item")]
public class Item : ScriptableObject
{
    [SerializeField] private string Description;
    [SerializeField] private Sprite Icon;
    [SerializeField] private int Price;

    public string description => this.Description;
    public Sprite icon => this.Icon;
    public int price => this.Price;

    
}
