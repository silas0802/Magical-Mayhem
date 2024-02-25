using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class SpellShop : NetworkBehaviour
{
    public static SpellShop instance;
    public UnitController localUnitController;
    public bool testing = false;
    public Dictionary<int, Buyable> buyableIDs = new Dictionary<int, Buyable>();
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
        
        
        MakeMap();
        goldText.SetText(gold.ToString());
        healthText.SetText(health.ToString());
        damageText.SetText(damage.ToString());
        initatedSpells = new BuyableIcon[spells.Length];
        initiatedItems = new BuyableIcon[items.Length];
         
        for (int i = 0; i < spells.Length; i++)
        {
            Debug.Log("i am in spells initiate");
            BuyableIcon buyableSpell = Instantiate(spellIconTemplate,buyables);
            buyableSpell.Initialize(spells[i]);
            buyableSpell.GetComponent<Button>().onClick.AddListener(()=>{SelectBuyable(buyableSpell); CancelBuyablePhase();});

            initatedSpells[i]=buyableSpell; 
        }

        for (int i = 0; i < items.Length; i++)
        {
            BuyableIcon buyableItem = Instantiate(spellIconTemplate,buyables);
            buyableItem.Initialize(items[i]);
            buyableItem.GetComponent<Button>().onClick.AddListener(()=>{SelectBuyable(buyableItem); CancelBuyablePhase();});  
            initiatedItems[i]=buyableItem; 
        }  
        LoadSlots();
        ToggleSpellHolder(true);
        
        
        
    }

    public void ConnectPlayer(){
        List<UnitController> players = RoundManager.instance.GetUnits;
        foreach (UnitController item in players)
        {
            if (item.GetComponent<NetworkObject>().IsLocalPlayer)
            {
                
                localUnitController=item;       
                         
            }            
        }
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
        
        SelectBuyable(null);
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
        
        if (gold>selectedBuyable.price&&!localUnitController.inventory.items.Contains(selectedBuyable)&&!localUnitController.inventory.spells.Contains(selectedBuyable))
        {
            
            Spell spell =selectedBuyable as Spell;
            Item item = selectedBuyable as Item;
            buyablePhase=true;
            if (selectedBuyable is Spell)
            {
                foreach (BuyableIcon item1 in ownedSpells)
                {
                if (item1.buyable==null)
                {
                    item1.SetColor(new Color32(40,255,0,255));
                }

                }    
            }else{
                
                for (int i = 0; i < localUnitController.inventory.items.Length; i++)
                {   
                
                    if (localUnitController.inventory.items[i] is null)
                    {
                        ulong id = NetworkManager.Singleton.LocalClient.ClientId;
                        
                        localUnitController.inventory.items[i]=item;
                        localUnitController.TryPlaceBuyable(item.GetID(),i);
                        ownedItems[i].Initialize(selectedBuyable);
                        gold=gold-selectedBuyable.price;
                        goldText.SetText(gold.ToString());
                        health=health+item.health;
                        damage=damage+item.damage;
                        healthText.SetText(health.ToString());
                        damageText.SetText(damage.ToString());

                        EndByablePhase();
                        break;
                    }
                }
                

            }
        }
        
    }

    public void PlaceBuyable(BuyableIcon icon){
        if (buyablePhase&&!icon.buyable)
        {   
            int j=0;
            for (int i = 0; i < ownedSpells.Length; i++)
            {   

                if (ownedSpells[i]==icon)
                {
                    break;
                }
                j++;
            }
            Spell spell = selectedBuyable as Spell;
            localUnitController.inventory.spells[j]=spell;
            Debug.Log(IsServer);
            Debug.Log(IsHost);
            Debug.Log(IsClient);
            
            localUnitController.TryPlaceBuyable(spell.GetID(),j);
            
            icon.Initialize(selectedBuyable);
            gold=gold-selectedBuyable.price;
            goldText.SetText(gold.ToString());
            Debug.Log(localUnitController.inventory.spells[j]);
           
            
        }
        
        EndByablePhase();
    }

    public void EndByablePhase(){
        buyablePhase=false;
        if (selectedBuyable is Spell)
        {
            foreach (BuyableIcon item in ownedSpells)
            {   
                if (item.buyable is not null)
                {
                    item.SetColor(new Color32(255,255,255,255));    
                }else{
                    item.SetColor(new Color32(0,0,0,0));
                }     
            }    
        }else{
            foreach (BuyableIcon item in ownedItems)
            {
                if (item.buyable is not null)
                {
                    item.SetColor(new Color32(255,255,255,255));    
                }else{
                    item.SetColor(new Color32(0,0,0,0));
                }
                        
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


    private void MakeMap(){
        foreach (Spell item in spells)
        {
            buyableIDs.Add(item.GetID(),item);
        }
        foreach (Item item in items)
        {
            buyableIDs.Add(item.GetID(),item);
        }
    }
    

    
}
