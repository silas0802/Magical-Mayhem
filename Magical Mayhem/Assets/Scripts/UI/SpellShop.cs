using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpellShop : MonoBehaviour
{
    public Spell[] spells;
    public SpellIcon[] ownedSpells =new SpellIcon[6];
    public SpellIcon spellIconTemplate;
    public SpellIcon selectedSpellicon;
    public Transform buyables;
    public Buyable selected;
    public TMP_Text descriptionText;
    
    // Start is called before the first frame update
    void Start()
    {
         
        for (int i = 0; i < spells.Length; i++)
        {
            SpellIcon buyableSpell = Instantiate(spellIconTemplate,buyables);
            buyableSpell.Initialize(spells[i]);
            buyableSpell.GetComponent<Button>().onClick.AddListener(()=>SelectBuyable(buyableSpell));   
        }  
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SelectBuyable(SpellIcon spellIcon){
        selected=spellIcon.buyable;
        selectedSpellicon.Initialize(spellIcon.buyable);
        descriptionText.text=selected.description;
    }

}
