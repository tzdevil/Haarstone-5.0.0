using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardScript : MonoBehaviour
{
    // deckte bir pool olacak, oradan random çekeceðiz ve çektiðimiz kartý sileceðiz.
    public CardSO card;
    public float mana;
    // Minion ise
    public float attack;
    public float hp;
    public float maxHP;

    public bool chosen; // kart týklanmýþsa
    public bool holding; // kart holdlanýyorsa 
    public Vector3 startedHolding;
    public Vector3 defaultPosition;
    // eðer holding true ise ve son yeri ile ilk yeri arasýnda belirli bir sayýdan fazla bir deðer varsa chosen = false else true

    public bool canTarget; // eðer bir kart targetlayabiliyosa, kartý taþýmak yerine karttan bir þey çýkacak ve onu bir targeta götüreceðiz. ardýndan onpointerdrop ile fonksiyonu çalýþtýracaðýz.

    public float externalManaChange; // cult neopythe gibi kartlardan gelen deðer
    public float debuffTimer;

    private GameObject targetArrow;
    public GameObject targetPrefab;

    public GameObject canPlayGameObject;

    public GameObject bigCardPrefab;
    private GameObject _bigCardPrefab;

    public Sprite cardback;

    public bool cardDrawnThisTurn;

    public int Index;

    private void Start()
    {
        mana = card.mana;

        if (card.cardType == CardType.WEAPON)
        {
            attack = card.weaponAttack;
            hp = card.weaponDurability;
            maxHP = card.weaponDurability;
        }
        else
        {
            attack = card.attack;
            hp = card.hp;
            maxHP = card.hp;
        }

        canTarget = card.canTarget;
        transform.localScale = new Vector3(1.35f, 1.35f, 1.35f);
    }

    public void SetRotation(int currentCardCount)
    {
        Index = transform.GetSiblingIndex();
        if (currentCardCount <= 3)
        {
            GetComponent<RectTransform>().localRotation = Quaternion.Euler(0, 0, 0);
            return;
        }

        var bolunecekDeger = 24 / (currentCardCount - 1);
        float rotationValue = 12 - (bolunecekDeger * Index);

        GetComponent<RectTransform>().localRotation = Quaternion.Euler(0, 0, rotationValue);
    }

    private void Update()
    {
        if (Index != transform.GetSiblingIndex()) SetRotation(transform.root.Find("Hero").Find("Canvas").Find("Hero").GetComponent<HeroScript>().hand.Count);
 
        if (transform.parent.name == "HandUI")
        {
            if (GameplayManager.turn == Turn.PLAYER && transform.root.name == "Player" || GameplayManager.turn == Turn.OPPONENT && transform.root.name == "Opponent")
            {
                chosen = GameplayManager.turn == Turn.PLAYER ? (Input.mousePosition.y - startedHolding.y) > 175 : (Input.mousePosition.y - startedHolding.y) < -175;
            }
            if (holding)
            {
                if (chosen)
                {
                    GetComponent<RawImage>().enabled = true;
                    _bigCardPrefab.GetComponent<RawImage>().enabled = false;
                }
                else
                {
                    GetComponent<RawImage>().enabled = false;
                    _bigCardPrefab.GetComponent<RawImage>().enabled = true;
                }
            }
        }
        GetComponent<RawImage>().texture = card.cardSprite.texture;
        if (transform.root.Find("Hero").transform.Find("Canvas").transform.Find("Hero")) GetComponent<RawImage>().texture = ((GameplayManager.turn == Turn.PLAYER && transform.root.name == "Player" || GameplayManager.turn == Turn.OPPONENT && transform.root.name == "Opponent")) ? card.cardSprite.texture : cardback.texture;

        if (mana < 0) mana = 0;
        if (debuffTimer < 0) debuffTimer = 0;
        mana = card.mana + externalManaChange;
        mana += (card.cardType == CardType.MINION) ? transform.root.transform.Find("Hero").transform.Find("Canvas").transform.Find("Hero").GetComponent<HeroScript>().externalMinionManaDebuff : transform.root.transform.Find("Hero").transform.Find("Canvas").transform.Find("Hero").GetComponent<HeroScript>().externalSpellManaDebuff;

        foreach (MinionAbilites _card in card.minionAbilites)
        {
            if (_card.minionEffect == "ArmorGiant" && _card.minionAbility == MinionAbility.CostsLess)
            {
                mana -= Mathf.Clamp(transform.root.transform.Find("Hero").transform.Find("Canvas").transform.Find("Hero").GetComponent<HeroScript>().armor, 0, 15);
            }
            if (_card.minionEffect == "SnowyGiant" && _card.minionAbility == MinionAbility.CostsLess)
            {
                mana -= Mathf.Clamp((transform.root.name == "Player" ? GameObject.Find("Opponent") : GameObject.Find("Player")).transform.Find("Hero").transform.Find("Canvas").transform.Find("Hero").GetComponent<HeroScript>().maxHP - (transform.root.name == "Player" ? GameObject.Find("Opponent") : GameObject.Find("Player")).transform.Find("Hero").transform.Find("Canvas").transform.Find("Hero").GetComponent<HeroScript>().currentHP, 0, 20);
            }
            if (_card.minionAbility == MinionAbility.DivineShield)
            {
                foreach (CurrentEffects effect in transform.root.Find("Hero").Find("Canvas").Find("Hero").GetComponent<HeroScript>()._currentEffects)
                {
                    if (effect.effectName == "DivineShieldCosts1Less")
                    {
                        mana -= 1 * (float)effect.howManyStacked;
                    }
                }
                foreach (CurrentEffects effect in transform.root.Find("Hero").Find("Canvas").Find("Hero").GetComponent<HeroScript>()._currentEffects)
                {
                    if (effect.effectName == "DivineShieldMinionsCosts2LessThisTurn")
                    {
                        mana -= 2 * (float)effect.howManyStacked;
                    }
                }
            }
        }
        if (card.spellEffect == "CallOfKindness")
            mana -= Mathf.Clamp((GameObject.Find(transform.root.name)).transform.Find("Hero").transform.Find("Canvas").transform.Find("Hero").GetComponent<HeroScript>().hand.Count, 0, card.mana);
        if (card.cardType == CardType.WEAPON)
        {
            foreach (CurrentEffects effect in transform.root.Find("Hero").Find("Canvas").Find("Hero").GetComponent<HeroScript>()._currentEffects)
            {
                if (effect.effectName == "CityDweller")
                {
                    mana -= 3 * (float)effect.howManyStacked;
                }
            }
        }
        if ((GameplayManager.turn == Turn.PLAYER && transform.root.name == "Player") || (GameplayManager.turn == Turn.OPPONENT && transform.root.name == "Opponent"))
        {
            if (holding)
            {
                if (canTarget && ((card.cardTarget == CardTarget.FRIENDLYMINION && GameObjectCount("Player Minion") > 0) || (card.cardTarget == CardTarget.OPPONENTMINION && GameObjectCount("Opponent Minion") > 0) || (card.cardTarget == CardTarget.ALL) || (card.cardTarget == CardTarget.OPPONENTALL) || (card.cardTarget == CardTarget.FRIENDLYALL) || (card.cardTarget == CardTarget.PLAYER) || (card.cardTarget == CardTarget.OPPONENT) || (card.cardTarget == CardTarget.MINIONS && (GameObjectCount("Player Minion") > 0 || GameObjectCount("Opponent Minion") > 0))))
                {
                    targetArrow.transform.position = new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y, 0);
                }
                else
                {
                    transform.position = new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y, 0);
                }
            }
        }

        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(GetComponent<RawImage>().enabled);
        }
        if (GetComponent<RawImage>().enabled)
        {
            canPlayGameObject.SetActive(transform.root.Find("Hero").transform.Find("Canvas").transform.Find("Hero").GetComponent<HeroScript>().currentMana >= mana && ((GameplayManager.turn == Turn.PLAYER && transform.root.name == "Player") || (GameplayManager.turn == Turn.OPPONENT && transform.root.name == "Opponent")));
            transform.Find("Mana").GetComponent<TextMeshProUGUI>().text = mana.ToString();
            transform.Find("Attack").GetComponent<TextMeshProUGUI>().text = attack.ToString();
            transform.Find("Health").GetComponent<TextMeshProUGUI>().text = hp.ToString();
            transform.Find("Mana").GetComponent<TextMeshProUGUI>().GetComponent<RectTransform>().anchoredPosition = card.legendary ? new Vector3(-53.7f, 81, 0) : new Vector3(-53.7f, 90, 0);
            transform.Find("Mana").gameObject.SetActive(((GameplayManager.turn == Turn.PLAYER && transform.root.name == "Player" || GameplayManager.turn == Turn.OPPONENT && transform.root.name == "Opponent")));
            transform.Find("Attack").gameObject.SetActive((card.cardType == CardType.MINION || card.cardType == CardType.WEAPON) && ((GameplayManager.turn == Turn.PLAYER && transform.root.name == "Player" || GameplayManager.turn == Turn.OPPONENT && transform.root.name == "Opponent")));
            transform.Find("Health").gameObject.SetActive((card.cardType == CardType.MINION || card.cardType == CardType.WEAPON) && ((GameplayManager.turn == Turn.PLAYER && transform.root.name == "Player" || GameplayManager.turn == Turn.OPPONENT && transform.root.name == "Opponent")));
            canPlayGameObject.GetComponent<Image>().color = (card.hasCardDrawnThisTurnEffect && cardDrawnThisTurn) ? new Color32(210, 207, 50, 255) : new Color32(122, 255, 80, 255);
        }

        if ((card.cardType == CardType.MINION && attack < card.attack) || (card.cardType == CardType.WEAPON && attack < card.weaponAttack))
            transform.Find("Attack").GetComponent<TextMeshProUGUI>().color = new Color32(109, 36, 36, 255);
        if ((card.cardType == CardType.MINION && attack == card.attack) || (card.cardType == CardType.WEAPON && attack == card.weaponAttack))
            transform.Find("Attack").GetComponent<TextMeshProUGUI>().color = new Color32(255, 255, 255, 255);
        if ((card.cardType == CardType.MINION && attack > card.attack) || (card.cardType == CardType.WEAPON && attack > card.weaponAttack))
            transform.Find("Attack").GetComponent<TextMeshProUGUI>().color = new Color32(36, 108, 65, 255);

        if ((card.cardType == CardType.MINION && hp < card.hp) || (card.cardType == CardType.WEAPON && hp < card.weaponDurability))
            transform.Find("Health").GetComponent<TextMeshProUGUI>().color = new Color32(109, 36, 36, 255);
        if ((card.cardType == CardType.MINION && hp == card.hp) || (card.cardType == CardType.WEAPON && hp == card.weaponDurability))
            transform.Find("Health").GetComponent<TextMeshProUGUI>().color = new Color32(255, 255, 255, 255);
        if ((card.cardType == CardType.MINION && hp > card.hp) || (card.cardType == CardType.WEAPON && hp > card.weaponDurability))
            transform.Find("Health").GetComponent<TextMeshProUGUI>().color = new Color32(36, 108, 65, 255);

        if (mana > card.mana)
            transform.Find("Mana").GetComponent<TextMeshProUGUI>().color = new Color32(109, 36, 36, 255);
        else if (mana == card.mana)
            transform.Find("Mana").GetComponent<TextMeshProUGUI>().color = new Color32(255, 255, 255, 255);
        else if (mana < card.mana)
            transform.Find("Mana").GetComponent<TextMeshProUGUI>().color = new Color32(36, 108, 65, 255);

        //#region Card's Attack, Health and Mana Colors
        //transform.Find("Attack").GetComponent<TextMeshProUGUI>().color = AttackColor();
        //transform.Find("Health").GetComponent<TextMeshProUGUI>().color = HealthColor();
        //transform.Find("Mana").GetComponent<TextMeshProUGUI>().color = ManaColor();

        //Color32 AttackColor(object o = default) => o switch
        //{
        //    _ when (card.cardType == CardType.MINION && attack < card.attack || card.cardType == CardType.WEAPON && attack < card.weaponAttack) => new Color32(109, 36, 36, 255),
        //    _ when (card.cardType == CardType.MINION && attack == card.attack || card.cardType == CardType.WEAPON && attack == card.weaponAttack) => new Color32(255, 255, 255, 255),
        //    _ when (card.cardType == CardType.MINION && attack > card.attack || card.cardType == CardType.WEAPON && attack > card.weaponAttack) => new Color32(36, 108, 65, 255),
        //    _ => throw new System.NotImplementedException(),
        //};
        //Color32 HealthColor(object o = default) => o switch
        //{
        //    _ when (card.cardType == CardType.MINION && hp < card.hp) || (card.cardType == CardType.WEAPON && hp < card.weaponDurability) => new Color32(109, 36, 36, 255),
        //    _ when (card.cardType == CardType.MINION && hp == card.hp) || (card.cardType == CardType.WEAPON && hp == card.weaponDurability) => new Color32(255, 255, 255, 255),
        //    _ when (card.cardType == CardType.MINION && hp > card.hp) || (card.cardType == CardType.WEAPON && hp > card.weaponDurability) => new Color32(36, 108, 65, 255),
        //    _ => throw new System.NotImplementedException(),
        //};
        //Color32 ManaColor(object o = default) => o switch
        //{
        //    _ when mana > card.mana => new Color32(109, 36, 36, 255),
        //    _ when mana == card.mana => new Color32(255, 255, 255, 255),
        //    _ when mana < card.mana => new Color32(36, 108, 65, 255),
        //    _ => throw new System.NotImplementedException(),
        //};
        //#endregion
    }

    public int GameObjectCount(string gameObjectTag, int number = 0)
    {
        foreach (GameObject go in GameObject.FindGameObjectsWithTag(gameObjectTag)) number++;
        return number;
    }

    public void MouseDownCard()
    {
        if ((GameplayManager.turn == Turn.PLAYER && transform.root.name == "Player") || (GameplayManager.turn == Turn.OPPONENT && transform.root.name == "Opponent"))
        {
            foreach (GameObject card in GameObject.FindGameObjectsWithTag("Card"))
            {
                if (card) card.GetComponent<CardScript>().holding = false;
                if (card) card.GetComponent<CardScript>().chosen = false;
            }
            foreach (GameObject target in GameObject.FindGameObjectsWithTag("TargetArrow")) Destroy(target);
            holding = true;
            startedHolding = Input.mousePosition;
            defaultPosition = gameObject.transform.position;
            GetComponent<RawImage>().raycastTarget = false;
            if (canTarget && ((card.cardTarget == CardTarget.FRIENDLYMINION && GameObjectCount("Player Minion") > 0) || (card.cardTarget == CardTarget.OPPONENTMINION && GameObjectCount("Opponent Minion") > 0) || (card.cardTarget == CardTarget.ALL) || (card.cardTarget == CardTarget.OPPONENTALL) || (card.cardTarget == CardTarget.FRIENDLYALL) || (card.cardTarget == CardTarget.PLAYER) || (card.cardTarget == CardTarget.OPPONENT) || (card.cardTarget == CardTarget.MINIONS && (GameObjectCount("Player Minion") > 0 || GameObjectCount("Opponent Minion") > 0))))
            {
                targetArrow = Instantiate(targetPrefab, transform);
                targetArrow.GetComponent<Canvas>().overrideSorting = true;
                targetArrow.GetComponent<Canvas>().sortingLayerName = "TargetIcon";
            }
            else
            {
                foreach (GameObject minion in GameObject.FindGameObjectsWithTag("Player Minion"))
                {
                    minion.GetComponent<RawImage>().raycastTarget = false;
                }
                foreach (GameObject minion in GameObject.FindGameObjectsWithTag("Opponent Minion"))
                {
                    minion.GetComponent<RawImage>().raycastTarget = false;
                }
            }
            _bigCardPrefab = Instantiate(bigCardPrefab, transform.parent.parent);
            _bigCardPrefab.transform.position = new Vector2(transform.position.x, transform.position.y + (GameplayManager.turn == Turn.PLAYER ? 4.25f : -4.25f));
            _bigCardPrefab.GetComponent<CardPrefabBigScript>().OGcard = gameObject;
            _bigCardPrefab.GetComponent<CardPrefabBigScript>().card = card;
            _bigCardPrefab.GetComponent<CardPrefabBigScript>().mana = mana;
            _bigCardPrefab.GetComponent<CardPrefabBigScript>().attack = attack;
            _bigCardPrefab.GetComponent<CardPrefabBigScript>().hp = hp;
            _bigCardPrefab.GetComponent<CardPrefabBigScript>().maxHP = maxHP;
            _bigCardPrefab.GetComponent<CardPrefabBigScript>().canTarget = canTarget;
            _bigCardPrefab.GetComponent<RawImage>().texture = card.cardSprite.texture;
            gameObject.GetComponent<RawImage>().enabled = false;
        }
    }

    public void MouseUpCard()
    {
        if (_bigCardPrefab) Destroy(_bigCardPrefab);
        gameObject.GetComponent<RawImage>().enabled = true;
        if (holding)
        {
            if (GameplayManager.turn == Turn.PLAYER && transform.root.name == "Player" || GameplayManager.turn == Turn.OPPONENT && transform.root.name == "Opponent")
            {
                holding = false;
                chosen = (Input.mousePosition - startedHolding).magnitude <= 20;
                if (!holding) transform.position = defaultPosition;
                GetComponent<RawImage>().raycastTarget = true;
            }
            if (targetArrow) Destroy(targetArrow);

            foreach (GameObject minion in GameObject.FindGameObjectsWithTag("Player Minion"))
            {
                minion.GetComponent<RawImage>().raycastTarget = true;
            }
            foreach (GameObject minion in GameObject.FindGameObjectsWithTag("Opponent Minion"))
            {
                minion.GetComponent<RawImage>().raycastTarget = true;
            }
        }
    }
}
