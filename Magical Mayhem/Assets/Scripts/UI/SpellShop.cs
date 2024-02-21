using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;

public class SpellShop : MonoBehaviour
{
    public static SpellShop instance;
    public UnitController localUnitController;
    public bool testing = false;

    private int gold=50;
    private int health=1000;
    private int damage=500;    
    public TMP_Text goldText;
    public TMP_Text healthText;
    public TMP_Text damageText;
    public Spell[] spells;
    public Item[] items;
    public BuyableIcon[] initatedSpells;
    public BuyableIcon[] initiatedItems;
    public BuyableIcon[] ownedSpells =new BuyableIcon[6];
    public BuyableIcon[] ownedItems = new BuyableIcon[6];
    public BuyableIcon spellIconTemplate;
    public BuyableIcon selectedSpellicon;
    public Transform buyables;
    public Buyable selectedBuyable;
    public bool buyablePhase=false;
    public Transform selectedSpellSlot;
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
        goldText.SetText(gold.ToString());
        healthText.SetText(health.ToString());
        damageText.SetText(damage.ToString());
        initatedSpells = new BuyableIcon[spells.Length];
        initiatedItems = new BuyableIcon[items.Length];
         
        for (int i = 0; i < spells.Length; i++)
        {

            BuyableIcon buyableSpell = Instantiate(spellIconTemplate,buyables);
            buyableSpell.Initialize(spells[i]);
            buyableSpell.GetComponent<Button>().onClick.AddListener(()=>{SelectBuyable(buyableSpell); CancelBuyablePhase();});

            initatedSpells[i]=buyableSpell; 
        }

        for (int i = 0; i < items.Length; i++)
        {
            BuyableIcon buyableItem = Instantiate(spellIconTemplate,buyables);
            buyableItem.Initialize(items[i]);
            buyableItem.GetComponent<Button>().onClick.AddListener(()=>SelectBuyable(buyableItem));  
            initiatedItems[i]=buyableItem; 
        }  
        LoadSlots();
        ToggleSpellHolder(true);
        
        
        
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
        if (!buyablePhase)
        {
            if (buyableIcon==null)
            {
                descriptionText.text=null;
                selectedBuyable=null;
                selectedSpellicon.Initialize(null);
            }else{
                selectedBuyable=buyableIcon.buyable;
                selectedSpellicon.Initialize(buyableIcon.buyable);
                descriptionText.text=selectedBuyable.description;
            }
        }
    }
    public void SetTimer(float time)
    {
        this.time = time;
    }
    public void ToggleSpellHolder(bool active){
        spellHolder.gameObject.SetActive(active);
        
        
        foreach (BuyableIcon item in initatedSpells)
        {
            item.gameObject.SetActive(active);

        }

        foreach (BuyableIcon item in initiatedItems)
        {
            item.gameObject.SetActive(!active);
        }
        
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
    public void OnValidate(){
        LoadSlots();
    }

    public void BuyBuyable(){
        if (gold>selectedBuyable.price)
        {
            
        
            buyablePhase=true;
            if (selectedBuyable is Spell)
            {
                foreach (BuyableIcon item in ownedSpells)
                {
                if (item.buyable==null)
                {
                    item.SetColor(new Color32(40,255,0,255));
                }

                }    
            }else{
                foreach (BuyableIcon item in ownedItems)
                {
                if (item.buyable==null)
                {
                    item.SetColor(new Color32(40,255,0,255));
                }

                }

            }
        }
        
    }

    public void PlaceBuyable(BuyableIcon icon){
        if (buyablePhase&&!icon.buyable)
        {
            icon.Initialize(selectedBuyable);
            gold=gold-selectedBuyable.price;
            goldText.SetText(gold.ToString());
            if (selectedBuyable is Item)
            {
                Item buy = selectedBuyable as Item;
                health=health+buy.health;
                damage=damage+buy.damage;
                healthText.SetText(health.ToString());
                damageText.SetText(damage.ToString());    
                
                
            }
            
        }
        
        EndByablePhase();
    }

    public void EndByablePhase(){
        buyablePhase=false;
        if (selectedBuyable is Spell)
        {
            foreach (BuyableIcon item in ownedSpells)
            {
                item.SetColor(new Color32(255,255,255,255));
                        
            }    
        }else{
            foreach (BuyableIcon item in ownedItems)
            {
                item.SetColor(new Color32(255,255,255,255));
                        
            }
        }
        
        SelectBuyable(null);

    }

    public void CancelBuyablePhase(){
        if (buyablePhase)
        {
            EndByablePhase();
        }
    }


}
