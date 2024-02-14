using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellShop : MonoBehaviour
{
    public SpellIcon spellIcon;
    public Transform trans;
    // Start is called before the first frame update
    void Start()
    {
        Spell[] spells = Resources.LoadAll<Spell>("/Spells");  
        for (int i = 0; i < spells.Length; i++)
        {
            SpellIcon temp = Instantiate(spellIcon,transform);
            temp.Initialize(spells[i]);    
        }  
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
