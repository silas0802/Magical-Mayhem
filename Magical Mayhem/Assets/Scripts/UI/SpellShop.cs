using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellShop : MonoBehaviour
{
    public Spell[] spells;
    public SpellIcon[] ownedSpells =new SpellIcon[6];
    public SpellIcon spellIcon;
    public Transform buyables;
    // Start is called before the first frame update
    void Start()
    {
         
        for (int i = 0; i < spells.Length; i++)
        {
            SpellIcon temp = Instantiate(spellIcon,buyables);
            temp.Initialize(spells[i]);    
        }  
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
