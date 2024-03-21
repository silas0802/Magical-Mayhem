using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SpellShop : NetworkBehaviour
{
    public static SpellShop instance;
    public UnitController localUnitController;
    public bool testing = true;
    public Dictionary<int, Buyable> buyableIDs = new Dictionary<int, Buyable>();
    private bool toggleSpellHolder = false;
    public TMP_Text toggleSpellItemButtonText;
    public Button sellButton;
    public Button upgradeButton;
    public Button buyButton;

    public TMP_Text goldText;
    public TMP_Text healthText;
    public TMP_Text frostMultiplier;
    public TMP_Text ArcaneMultiplier;
    public TMP_Text fireMultiplier;
    public Spell[] spells;
    public Item[] items;
    public BuyableIcon[] initatedSpells;
    public BuyableIcon[] initiatedItems;
    public BuyableIcon[] ownedSpells = new BuyableIcon[6];
    public BuyableIcon[] ownedItems = new BuyableIcon[6];
    public BuyableIcon spellIconTemplate;
    public BuyableIcon selectedSpellicon;
    public Transform[] buyableSlots = new Transform[6];
    public Transform[] buyableBuyableHolders = new Transform[2];
    public Buyable selectedBuyable;
    public bool buyablePhase = false;
    public Transform selectedSpellSlot;
    public Transform spellHolder;
    public Transform itemHolder;
    public TMP_Text descriptionText;
    public TMP_Text timerText;
    [SerializeField] private float time;


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

        initatedSpells = new BuyableIcon[spells.Length];
        initiatedItems = new BuyableIcon[items.Length];

        InitializeBuyableBuyables();
        LoadSlots();
        ToggleSpellHolder();
        // upgradeButton.gameObject.SetActive(false);
        // sellButton.gameObject.SetActive(false);



    }

    private void InitializeBuyableBuyables(){
          for (int i = 0; i < spells.Length; i++)
        {   
            BuyableIcon buyableSpell;
            Debug.Log(spells[i].elementType);
            if (spells[i].elementType is SpellElementType.Fire)
            {
                buyableSpell = Instantiate(spellIconTemplate, buyableSlots[0]);    
            }else if(spells[i].elementType is SpellElementType.Arcane)
            {
                 buyableSpell = Instantiate(spellIconTemplate, buyableSlots[1]);
            }else
            {
                 buyableSpell = Instantiate(spellIconTemplate, buyableSlots[2]);
            }
            
            buyableSpell.Initialize(spells[i]);
            buyableSpell.GetComponent<Button>().onClick.AddListener(() => { SelectBuyable(buyableSpell); CancelBuyablePhase(); ActivateSellButton(); });

            initatedSpells[i] = buyableSpell;
        }

        for (int i = 0; i < items.Length; i++)
        {
            BuyableIcon buyableItem;
            if (items[i].itemType is ItemType.Offensive)
            {
                buyableItem = Instantiate(spellIconTemplate, buyableSlots[3]);    
            }else if (items[i].itemType is ItemType.Defensive)
            {
                buyableItem = Instantiate(spellIconTemplate, buyableSlots[4]);
            }else
            {
                buyableItem = Instantiate(spellIconTemplate, buyableSlots[5]);
            }
            
            buyableItem.Initialize(items[i]);
            buyableItem.GetComponent<Button>().onClick.AddListener(() => { SelectBuyable(buyableItem); CancelBuyablePhase(); ActivateSellButton(); });
            initiatedItems[i] = buyableItem;
        }
    }
    public void InitalizePlayerInformation()
    {
        goldText.SetText(localUnitController.inventory.gold.ToString());
        healthText.SetText(localUnitController.GetHealth().ToString());
        Debug.Log("initializing player information");

    }

    public void ConnectPlayer(UnitController local)
    {
        localUnitController = local;
        Debug.Log("unitcontroller initiated with: " + local);
    }
    // Update is called once per frame
    void Update()
    {

        // if (!testing)
        // {
        //     time -= Time.deltaTime;
        //     if (time < 0 && gameObject.activeSelf)
        //     {
        //         gameObject.SetActive(false);
        //     }
        //     else
        //     {
        //         timerText.text = ((int)time).ToString();
        //     }
        // }

    }
    public void SelectBuyable(BuyableIcon buyableIcon)
    {
        if (!buyablePhase)
        {
            selectedSpellicon.GetComponent<Image>().color= new Color(255,255,255,255);
            if (buyableIcon == null)
            {
                descriptionText.text = null;
                selectedBuyable = null;
                selectedSpellicon.Initialize(null);
                buyButton.GetComponent<Image>().color = new Color(255, 255, 255, 255);
                selectedSpellicon.GetComponent<Image>().color= new Color(0,0,0,255);
            }
            else
            {
                selectedBuyable = buyableIcon.buyable;
                selectedSpellicon.Initialize(buyableIcon.buyable);
                descriptionText.text = selectedBuyable.description;
                if (localUnitController.inventory.gold < selectedBuyable.price)
                {
                    buyButton.GetComponent<Image>().color = new Color32(255, 255, 255, 100);
                }
                else
                {
                    buyButton.GetComponent<Image>().color = new Color(255, 255, 255, 255);
                }
            }
        }
    }
    public void SetTimer(float time)
    {
        this.time = time;
    }

    public void ToggleSpellHolder()
    {
        toggleSpellHolder = !toggleSpellHolder;
        buyableBuyableHolders[0].gameObject.SetActive(toggleSpellHolder);
        buyableBuyableHolders[1].gameObject.SetActive(!toggleSpellHolder);
        spellHolder.gameObject.SetActive(toggleSpellHolder);
        itemHolder.gameObject.SetActive(!toggleSpellHolder);

        // foreach (BuyableIcon item in initatedSpells)
        // {
        //     item.gameObject.SetActive(toggleSpellHolder);

        // }

        // foreach (BuyableIcon item in initiatedItems)
        // {
        //     item.gameObject.SetActive(!toggleSpellHolder);
        // }
        

        if (toggleSpellHolder)
        {
            toggleSpellItemButtonText.SetText("Items");
        }
        else
        {
            toggleSpellItemButtonText.SetText("Spells");
        }

        SelectBuyable(null);
    }

    public void LoadSlots()
    {

        foreach (BuyableIcon item in ownedSpells)
        {
            item.Initialize(item.buyable);
        }
        foreach (BuyableIcon item in ownedItems)
        {
            item.Initialize(item.buyable);
        }

    }
    public void OnValidate()
    {
        LoadSlots();
    }

    public void ServerTryBuyBuyable()
    {
        localUnitController.TryGetItem(NetworkManager.Singleton.LocalClientId, selectedBuyable.GetID());
    }
    public void BuyBuyable()
    {

        Spell spell = selectedBuyable as Spell;
        Item item = selectedBuyable as Item;
        buyablePhase = true;
        if (selectedBuyable is Spell)
        {
            foreach (BuyableIcon item1 in ownedSpells)
            {
                if (item1.buyable == null)
                {
                    item1.SetColor(new Color32(40, 255, 0, 255));
                }

            }
        }
        else
        {

            for (int i = 0; i < localUnitController.inventory.items.Length; i++)
            {

                if (localUnitController.inventory.items[i] is null)
                {


                    localUnitController.inventory.items[i] = item;
                    localUnitController.TryPlaceBuyable(item.GetID(), i);
                    UpdateVisuals(ownedItems[i]);
                    EndByablePhase();
                    break;
                }
            }


        }


    }

    public void PlaceBuyable(BuyableIcon icon)
    {
        if (buyablePhase && !icon.buyable)
        {
            int j = 0;
            for (int i = 0; i < ownedSpells.Length; i++)
            {

                if (ownedSpells[i] == icon)
                {
                    break;
                }
                j++;
            }
            Spell spell = selectedBuyable as Spell;
            localUnitController.inventory.spells[j] = spell;


            localUnitController.TryPlaceBuyable(spell.GetID(), j);
            UpdateVisuals(icon);


            EndByablePhase();
        }


    }
    void UpdateVisuals(BuyableIcon icon)
    {   
        if (icon is not null)
        {
             icon.Initialize(selectedBuyable);

        }
       
        goldText.SetText(localUnitController.inventory.gold.ToString());
        if (selectedBuyable is Item)
        {
            Item item = selectedBuyable as Item;


            healthText.SetText(localUnitController.GetHealth().ToString());
            frostMultiplier.SetText(localUnitController.GetFrostMult().ToString() + "%");
            ArcaneMultiplier.SetText(localUnitController.GetArcaneMult().ToString() + "%");
            fireMultiplier.SetText(localUnitController.GetFireMult().ToString() + "%");


        }

    }

    void UpdatePlayer()
    {

    }
    public void EndByablePhase()
    {
        buyablePhase = false;
        if (selectedBuyable is Spell)
        {
            foreach (BuyableIcon item in ownedSpells)
            {
                if (item.buyable is not null)
                {
                    item.SetColor(new Color32(255, 255, 255, 255));
                }
                else
                {
                    item.SetColor(new Color32(0, 0, 0, 0));
                }
            }
        }
        else
        {
            foreach (BuyableIcon item in ownedItems)
            {
                if (item.buyable is not null)
                {
                    item.SetColor(new Color32(255, 255, 255, 255));
                }
                else
                {
                    item.SetColor(new Color32(0, 0, 0, 0));
                }

            }
        }

        SelectBuyable(null);

    }

    public void CancelBuyablePhase()
    {
        if (buyablePhase)
        {
            EndByablePhase();
        }
    }


    public void ActivateSellButton()
    {

        if (localUnitController.inventory.items.Contains(selectedBuyable) || localUnitController.inventory.spells.Contains(selectedBuyable))
        {
            upgradeButton.gameObject.SetActive(true);
            sellButton.gameObject.SetActive(true);

        }
        else
        {
            upgradeButton.gameObject.SetActive(false);
            sellButton.gameObject.SetActive(false);
        }


    }

    private void MakeMap()
    {
        foreach (Spell item in spells)
        {
            buyableIDs.Add(item.GetID(), item);
        }
        foreach (Item item in items)
        {
            buyableIDs.Add(item.GetID(), item);
        }
    }

    public void TrySellOwnedBuyable(){
       localUnitController.TrySellItem(NetworkManager.Singleton.LocalClientId,selectedBuyable.GetID());
    }

    public void SellOwnedBuyable(){
        if (selectedBuyable is Item)
        {
             int counter=0;
            for (int i = 0; i < localUnitController.inventory.items.Length; i++)
            {   

                if (localUnitController.inventory.items[i]==selectedBuyable)
                {
                    break;
                }
                counter++;
                
            }
            localUnitController.inventory.items[counter]=null;
            
            foreach (BuyableIcon item in ownedItems)
            {
                if (item.buyable==selectedBuyable)
                {
                    item.Initialize(null);
                    item.SetColor(new Color32(0, 0, 0, 0));
                    break;
                }
            }
            
           
        }else
        {
            foreach (BuyableIcon item in ownedSpells)
            {
                if (item.buyable == selectedBuyable)
                {
                    item.Initialize(null);
                    item.SetColor(new Color32(0, 0, 0, 0));
                }
            }
             int counter=0;
            for (int i = 0; i < localUnitController.inventory.spells.Length; i++)
            {   

                if (localUnitController.inventory.spells[i] == selectedBuyable)
                {
                    break;
                }
                counter++;
                
            }
            localUnitController.inventory.spells[counter]=null;
            
        }
        UpdateVisuals(null);
        SelectBuyable(null);
    }




}
