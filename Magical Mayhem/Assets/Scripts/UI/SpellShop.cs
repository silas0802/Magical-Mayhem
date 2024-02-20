using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UI;

public class SpellShop : MonoBehaviour
{
    public static SpellShop instance;
    public bool testing = false;
    public Spell[] spells;
    public BuyableIcon[] ownedSpells =new BuyableIcon[6];
    public BuyableIcon[] ownedItems = new BuyableIcon[6];
    public BuyableIcon spellIconTemplate;
    public BuyableIcon selectedSpellicon;
    public Transform buyables;
    public Buyable selected;
    public Transform spellHolder;
    public Transform itemHolder;
    public TMP_Text descriptionText;
    public TMP_Text timerText;
    private float time;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        if (!testing)
        {
          gameObject.SetActive(false);  
        }
        
    }
    // Start is called before the first frame update
    void Start()
    {
         itemHolder.gameObject.SetActive(false);
        for (int i = 0; i < spells.Length; i++)
        {
            BuyableIcon buyableSpell = Instantiate(spellIconTemplate,buyables);
            buyableSpell.Initialize(spells[i]);
            buyableSpell.GetComponent<Button>().onClick.AddListener(()=>SelectBuyable(buyableSpell));   
        }  
        LoadSlots();
        
        
        
    }

    // Update is called once per frame
    void Update()
    {
        
        if (!testing)
        {
            time -= Time.deltaTime;
            if (time < 0)
        {
            gameObject.SetActive(false);
        }
        else
        {
            timerText.text = ((int)time).ToString();
        }    
        }
        
    }
    public void SelectBuyable(BuyableIcon buyableIcon){
        selected=buyableIcon.buyable;
        selectedSpellicon.Initialize(buyableIcon.buyable);
        descriptionText.text=selected.description;
    }
    public void SetTimer(float time)
    {
        this.time = time;
    }
    public void ToggleSpellHolder(bool active){
        spellHolder.gameObject.SetActive(active);
        itemHolder.gameObject.SetActive(!active);
        

    }

    public void LoadSlots(){
        
        foreach (BuyableIcon item in ownedSpells)
        {
            item.Initialize(item.buyable);
        }
        foreach (BuyableIcon item in ownedItems)
        {
            item.Initialize(item.buyable);
        }
        
    }
}
