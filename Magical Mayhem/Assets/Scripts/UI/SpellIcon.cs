using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellIcon : MonoBehaviour
{
    public Image image;
    
    public void Initialize(Spell spell){
        image.sprite = spell.icon;
    }
}
