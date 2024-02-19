using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpellShop : MonoBehaviour
{
    public static SpellShop instance;

    public Spell[] spells;
    public SpellIcon[] ownedSpells =new SpellIcon[6];
    public SpellIcon spellIconTemplate;
    public SpellIcon selectedSpellicon;
    public Transform buyables;
    public Buyable selected;
    public TMP_Text descriptionText;
    public TMP_Text timerText;
    private float time;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        gameObject.SetActive(false);
    }
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
    public void SelectBuyable(SpellIcon spellIcon){
        selected=spellIcon.buyable;
        selectedSpellicon.Initialize(spellIcon.buyable);
        descriptionText.text=selected.description;
    }
    public void SetTimer(float time)
    {
        this.time = time;
    }

}
