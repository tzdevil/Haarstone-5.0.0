using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Threading.Tasks;
using System.Linq;

public enum Turn { PLAYER, OPPONENT}

[System.Serializable]
public struct ManaCards
{
    public List<CardSO> cards;
    public int cost;
}

public class GameplayManager : MonoBehaviour
{
    public static Turn turn;
    public GameObject player, opponent;

    public CardSO zeCoin;

    public GameObject cardPrefab;
    public GameObject minionPrefab;

    public TextMeshProUGUI showTurnText;

    public GameObject rope;
    public float roundRemainingTime;
    public float ropeFullValue;
    public float ropeValuePerMs;

    public string gameLog;
    public int round;

    public List<ManaCards> manaCards; // 0dan baþlar.
    public List<CardSO> allPlayableCards;

    public float endTurnTimer;

    public GameObject playerMulliganFade, opponentMulliganFade;

    public GameObject turnBanner;
    public TextMeshProUGUI turnText;

    public CardSO BombaciMulayim;

    public Sprite weaponIconOn, weaponIconOff;

    public GameObject cardOnBoardPrefab;

    public List<LogRound> Logs;

    public static void SetLeft(RectTransform rt, float left)
    {
        rt.offsetMin = new Vector2(left, rt.offsetMin.y);
    }

    public CurrentEffects ExistingEffect(string effectKey, GameObject _root, CurrentEffects def = default)
    {
        foreach (CurrentEffects eff in _root.transform.root.Find("Hero").Find("Canvas").Find("Hero").GetComponent<HeroScript>()._currentEffects)
        {
            if (eff.effectName == effectKey)
            {
                def = eff;
            }
        }
        return def;
    }

    private void Awake()
    {
        CardSO[] _cards = Resources.LoadAll("Database", typeof(CardSO)).Cast<CardSO>().ToArray();
        IEnumerable<CardSO> cardsInGame =
            from _card in _cards
            where !_card.isToken
            where !_card.HasBugs
            where !_card.NotReleased
            select _card;
        allPlayableCards.AddRange(cardsInGame);

        for (int i= 0; i<11; i++)
            manaCards[i].cards.AddRange(allPlayableCards.Where(a => i < 10 ? a.mana == i : a.mana >= 10).ToList());

        if (PlayerDatabase.currentDeck.Count != 0)
        {
            foreach (var deck in FindObjectsOfType<DeckScript>())
            {
                deck.cards.Clear();
                deck.cards.AddRange(PlayerDatabase.currentDeck);
            }
        }
        else
        {
            System.Random rand = new();

            IEnumerable<CardSO> playerCards =
                from card_ in allPlayableCards
                where !card_.quest
                orderby rand.Next()
                select card_;
            IEnumerable<CardSO> opponentCards =
                from card_ in allPlayableCards
                where !card_.quest
                orderby rand.Next()
                select card_;

            player.GetComponent<HeroScript>().deck.cards = playerCards.Take(30).ToList();
            opponent.GetComponent<HeroScript>().deck.cards = opponentCards.Take(30).ToList();
        }
    }

    private void Start()
    {
        round = 0;
        roundRemainingTime = 75;
        ropeFullValue = 350;
        ropeValuePerMs = (1470-ropeFullValue) / 15;
        player = GameObject.Find("Player").transform.Find("Hero").transform.Find("Canvas").transform.Find("Hero").gameObject;
        opponent = GameObject.Find("Opponent").transform.Find("Hero").transform.Find("Canvas").transform.Find("Hero").gameObject;
        gameLog = "Game Started";
        gameLog += "Player vs Opponent\n";
        MulliganPhase();
    }

    private void Update() { 
        showTurnText.text = "<color=#feda55>Turn:</color> " + (turn == Turn.PLAYER ? "Player" : "Opponent");
        roundRemainingTime -= Time.deltaTime;
        rope.SetActive(roundRemainingTime <= 15 && roundRemainingTime > 0);
        if (roundRemainingTime <= 15 && roundRemainingTime > 0) SetLeft(rope.GetComponent<RectTransform>(), ropeFullValue + ropeValuePerMs * (15 - roundRemainingTime)); 
        if (roundRemainingTime <= 0)
        {
            EndTurn();
        }
        if (endTurnTimer > 0) endTurnTimer -= Time.deltaTime;
    }

    public void TakeFatigueDamage(HeroScript fatigue)
    {
        if (fatigue.armor <= 0) 
            fatigue.currentHP -= fatigue.currentFatigueDamage;
        else
        {
            float armordanSonraGidecekHealth = fatigue.currentFatigueDamage > fatigue.armor ? Mathf.Abs(fatigue.currentFatigueDamage - fatigue.armor) : 0;
            fatigue.armor -= fatigue.currentFatigueDamage;
            fatigue.currentHP -= armordanSonraGidecekHealth;
        }
        fatigue.currentFatigueDamage++;
        fatigue.ShowTakeDamage(fatigue.currentFatigueDamage);
        gameLog += "<color=#5d79ae>" + fatigue.name + "</color> took " + fatigue.currentFatigueDamage + " from <color=#ba1615>FATIGUE</color> in round " + round + ".\n";
    }

    public CardSO FindTheQuest(HeroScript hero)
    {
        return hero.deck.cards.FirstOrDefault(a => a.quest);
    }

    public void MulliganPhase()
    {
        HeroScript whoseTurn = (turn == Turn.PLAYER ? GameObject.Find("Player").transform.Find("Hero").transform.Find("Canvas").transform.Find("Hero").gameObject : GameObject.Find("Opponent").transform.Find("Hero").transform.Find("Canvas").transform.Find("Hero").gameObject).GetComponent<HeroScript>();
        
        if ((round == 0 && turn == Turn.PLAYER) || (round == 1 && turn == Turn.OPPONENT))
        {
            if (turn == Turn.PLAYER) playerMulliganFade.SetActive(true);
            else opponentMulliganFade.SetActive(true);
            whoseTurn.mulliganPhase.SetActive(true);
            List<CardSO> _cards = new();
            _cards.AddRange(whoseTurn.deck.cards);
            for (int i=0; i<3; i++)
            {
                CardSO v = FindTheQuest(whoseTurn) != null ? FindTheQuest(whoseTurn) : _cards[UnityEngine.Random.Range(0, _cards.Count)];
                whoseTurn.mulliganPhase.transform.Find($"Mulligan{i+1}").GetComponent<MulliganAttribute>().card = v;
                _cards.Remove(v);
            }
        }
        else
        {
            DrawCard(null, turn);
        }
    }

    async void AddCoinToHand(GameObject hero)
    {
        await Task.Delay(600);
        GameObject coin = Instantiate(cardPrefab, Vector3.zero, Quaternion.identity, hero.transform.root.Find("Hand").Find("Canvas").Find("HandUI"));
        coin.GetComponent<CardScript>().card = zeCoin;
        coin.name = zeCoin.cardName;
        coin.GetComponent<CardScript>().cardDrawnThisTurn = true;
        hero.GetComponent<HeroScript>().hand.Add(coin);
        coin.GetComponent<RectTransform>().localScale = new Vector3(1.35f, 1.35f, 1.35f);
    }

    public void ConfirmMulliganPhase()
    {
        HeroScript whoseTurn = (turn == Turn.PLAYER ? GameObject.Find("Player").transform.Find("Hero").transform.Find("Canvas").transform.Find("Hero").gameObject : GameObject.Find("Opponent").transform.Find("Hero").transform.Find("Canvas").transform.Find("Hero").gameObject).GetComponent<HeroScript>(); 
        foreach(Transform mull in whoseTurn.mulliganPhase.transform)
        {
            if (mull.gameObject.CompareTag("Mulligan"))
            {
                if (!mull.gameObject.GetComponent<MulliganAttribute>().mulligan)
                {
                    DrawCard(whoseTurn.gameObject, turn, mull.gameObject.GetComponent<MulliganAttribute>().card);
                }
                else
                {
                    DrawCard(whoseTurn.gameObject, turn, whoseTurn.deck.cards.Where(a => a != mull.gameObject.GetComponent<MulliganAttribute>().card).ToList()[UnityEngine.Random.Range(0, whoseTurn.deck.cards.Where(a => a != mull.gameObject.GetComponent<MulliganAttribute>().card).ToList().Count)]);
                }
            }
            Destroy(whoseTurn.mulliganPhase);
        }
        //whoseTurn.transform.parent.Find("MulliganPanelAnimation").GetComponent<Animator>().Play("MulliganPanelFade");
        whoseTurn.transform.parent.Find("MulliganPanelAnimation").GetComponent<Animator>().enabled = true;
        Destroy(whoseTurn.transform.parent.Find("MulliganPanelAnimation").gameObject, 1.2f);
        if (whoseTurn.gameObject == player) AddCoinToHand(player);
    }

    public async void DrawCard(GameObject drawlamakIsteyenReferans, Turn turn)
    {
        HeroScript whoseTurn = (drawlamakIsteyenReferans == null) ? (turn == Turn.PLAYER ? GameObject.Find("Player").transform.Find("Hero").transform.Find("Canvas").transform.Find("Hero").gameObject : GameObject.Find("Opponent").transform.Find("Hero").transform.Find("Canvas").transform.Find("Hero").gameObject).GetComponent<HeroScript>() : (drawlamakIsteyenReferans.transform.root.Find("Hero").transform.Find("Canvas").transform.Find("Hero").GetComponent<HeroScript>());
        if (whoseTurn.deck.cards.Count > 0)
        {
            CardSO cardSelected = whoseTurn.deck.cards[0];
            if (whoseTurn.hand.Count < 10)
            {
                if (cardSelected.castsWhenDrawn)
                {
                    GameObject cardOnBoard = Instantiate(cardOnBoardPrefab, GameObject.Find("GlobalCanvas").transform);
                    cardOnBoard.GetComponent<CardOnBoardScript>().card = cardSelected;
                    cardOnBoard.transform.Find("Mana").GetComponent<TextMeshProUGUI>().text = cardSelected.mana.ToString();
                    cardOnBoard.GetComponent<RawImage>().texture = cardSelected.cardSprite.texture;
                    cardOnBoard.name = cardSelected.cardName;
                    cardOnBoard.GetComponent<CardScript>().Index = whoseTurn.hand.Count - 1;
                    whoseTurn.RotateAllCardsInHand();
                    Destroy(cardOnBoard, .9f);
                    Type scriptType = typeof(SpellCardEffects);
                    MethodInfo info = scriptType.GetMethod(cardSelected.spellEffect);
                    info?.Invoke(GameObject.Find("GameplayManager").GetComponent<SpellCardEffects>(), new object[] { new List<GameObject> { gameObject }, whoseTurn.gameObject });
                    gameLog += "<color=#5d79ae>" + whoseTurn.transform.root.name+ " drew <color=#287353>" + cardSelected.name+ "</color> and it was <color=#BA1615>Casts When Drawn</color> in round " + round + ".\n";
                }
                else
                {
                    GameObject newCard = Instantiate(cardPrefab, whoseTurn.gameObject.transform.parent.parent.parent.Find("Hand").transform.Find("Canvas").transform.Find("HandUI").transform.position + new Vector3(0, 10, 0), Quaternion.identity);
                    newCard.transform.SetParent(whoseTurn.gameObject.transform.parent.parent.parent.Find("Hand").transform.Find("Canvas").transform);
                    newCard.transform.position = turn == Turn.PLAYER ? new Vector2(12, -3.55f) : new Vector2(12, 3.55f);
                    newCard.GetComponent<RectTransform>().localScale = new Vector3(1.35f, 1.35f, 1.35f);
                    newCard.GetComponent<CardScript>().card = cardSelected;
                    newCard.name = cardSelected.cardName;
                    newCard.GetComponent<CardScript>().cardDrawnThisTurn = true;
                    whoseTurn.hand.Add(newCard); 
                    newCard.GetComponent<CardScript>().Index = whoseTurn.hand.Count - 1;
                    whoseTurn.RotateAllCardsInHand();
                    if (!(round == 0 && turn == Turn.PLAYER) || (round == 1 && turn == Turn.OPPONENT))
                    {
                        await DrawCardEffect(newCard, whoseTurn);
                    }
                    else
                        newCard.transform.SetParent(whoseTurn.gameObject.transform.parent.parent.parent.Find("Hand").transform.Find("Canvas").transform.Find("HandUI").transform);
                    gameLog += "<color=#5d79ae>" + whoseTurn.transform.root.name + " drew <color=#287353>" + cardSelected.name + "</color> in round " + round + ".\n";
                }
            }
            whoseTurn.deck.cards.RemoveAt(0);
            if (cardSelected.castsWhenDrawn) DrawCard(null, turn);
        }
        else
        {
            TakeFatigueDamage(whoseTurn);
        }
    }
    
    public async void DrawCard(GameObject drawlamakIsteyenReferans, Turn turn, CardSO drawlanacakKart)
    {
        HeroScript whoseTurn = (drawlamakIsteyenReferans == null) ? (turn == Turn.PLAYER ? GameObject.Find("Player").transform.Find("Hero").transform.Find("Canvas").transform.Find("Hero").gameObject : GameObject.Find("Opponent").transform.Find("Hero").transform.Find("Canvas").transform.Find("Hero").gameObject).GetComponent<HeroScript>() : (drawlamakIsteyenReferans.transform.root.Find("Hero").transform.Find("Canvas").transform.Find("Hero").GetComponent<HeroScript>());
        if (whoseTurn.deck.cards.Count > 0)
        {
            if (whoseTurn.hand.Count < 10)
            {
                GameObject newCard = Instantiate(cardPrefab);
                newCard.transform.SetParent(whoseTurn.gameObject.transform.parent.parent.parent.Find("Hand").transform.Find("Canvas").transform);
                newCard.transform.position = turn == Turn.PLAYER ? new Vector2( 12, -3.55f) : new Vector2(12, 3.55f);
                newCard.GetComponent<RectTransform>().localScale = new Vector3(1.35f, 1.35f, 1.35f);
                newCard.GetComponent<CardScript>().card = drawlanacakKart;
                newCard.GetComponent<CardScript>().cardDrawnThisTurn = true;
                newCard.name = drawlanacakKart.cardName;
                whoseTurn.hand.Add(newCard);
                await DrawCardEffect(newCard, whoseTurn);
                    //newCard.transform.SetParent(whoseTurn.gameObject.transform.parent.parent.parent.Find("Hand").transform.Find("Canvas").transform.Find("HandUI").transform);
                gameLog += "<color=#5d79ae>" + whoseTurn.transform.root.name + " drew <color=#287353>" + drawlanacakKart.name + "</color> in round " + round + ".\n";
                newCard.GetComponent<CardScript>().Index = whoseTurn.hand.Count - 1;
                whoseTurn.RotateAllCardsInHand();
            }
            whoseTurn.deck.cards.Remove(drawlanacakKart);
        }
        else
        {
            TakeFatigueDamage(whoseTurn);
        }
    }

    public async Task DrawCardEffect(GameObject card, HeroScript hero)
    {
        card.GetComponent<RawImage>().raycastTarget = false;
        card.transform.DOMove(new Vector3(7.35f, -0.25f, 0), .5f);
        card.transform.DOScale(new Vector3(2.5f, 2.5f, 2.5f), .5f);
        await Task.Delay(600);
        if (!card) return;
        card.transform.DOScale(new Vector3(1.35f, 1.35f, 1.35f), .4f);
        card.transform.DOMove(hero.gameObject.transform.root.Find("Hand").Find("Canvas").Find("HandUI").childCount > 0 ? (hero.gameObject.transform.root.Find("Hand").Find("Canvas").Find("HandUI").GetChild(hero.gameObject.transform.root.Find("Hand").Find("Canvas").Find("HandUI").childCount - 1).transform.position + new Vector3(2.25f, 0, 0)) : (hero.transform.root.name == "Player" ? new Vector3(0, -4.5f) : new Vector3(0, 4.5f)), .4f);
        await Task.Delay(410);
        if (!card) return;
        card.transform.SetParent(hero.gameObject.transform.parent.parent.parent.Find("Hand").transform.Find("Canvas").transform.Find("HandUI").transform);
        card.transform.localPosition = new Vector3(0, 0, 0);
        card.GetComponent<RawImage>().raycastTarget = true;
    }

    public async void NewTurn()
    {
        (turn == Turn.PLAYER ? player : opponent).GetComponent<HeroScript>().maxMana += ((turn == Turn.PLAYER) ? player : opponent).GetComponent<HeroScript>().maxMana < 10 ? 1 : 0; // + mana
        (turn == Turn.PLAYER ? player : opponent).GetComponent<HeroScript>().currentMana = (turn == Turn.PLAYER ? player : opponent).GetComponent<HeroScript>().maxMana;
        if (round >= 1)
        {
            turnBanner.SetActive(true);
            turnText.text = $"{(turn == Turn.PLAYER ? "YOUR" : "OPPONENT'S")} TURN";
            turnBanner.transform.DOScale(Vector3.one * 1.3f, .5f);
            await Task.Delay(1000);
            turnBanner.transform.DOScale(Vector3.zero, .5f);
            await Task.Delay(500);
            turnBanner.SetActive(false);
        }
        foreach(GameObject card in GameObject.FindGameObjectsWithTag("Card"))
        {
            card.GetComponent<CardScript>().cardDrawnThisTurn = false;
        }
        player.GetComponent<HeroScript>().remainingHeroAttacks++;
        opponent.GetComponent<HeroScript>().remainingHeroAttacks++;
        round++;
        gameLog += "New turn started.\n";
        roundRemainingTime = 75;
        MulliganPhase();
        player.GetComponent<HeroScript>().debuffTimer--;
        opponent.GetComponent<HeroScript>().debuffTimer--;
        if (player.GetComponent<HeroScript>().debuffTimer == 0)
        {
            player.GetComponent<HeroScript>().externalMinionManaDebuff = 0;
            player.GetComponent<HeroScript>().externalSpellManaDebuff = 0;
        }
        opponent.GetComponent<HeroScript>().debuffTimer--;
        if (opponent.GetComponent<HeroScript>().debuffTimer == 0)
        {
            opponent.GetComponent<HeroScript>().externalMinionManaDebuff = 0;
            opponent.GetComponent<HeroScript>().externalSpellManaDebuff = 0;
        }
        foreach (Transform card in GameObject.Find("Player").transform.Find("Hand").transform.Find("Canvas").transform.Find("HandUI").transform)
        {
            card.gameObject.GetComponent<CardScript>().debuffTimer--;
            if (card.gameObject.GetComponent<CardScript>().debuffTimer == 0) card.gameObject.GetComponent<CardScript>().externalManaChange = 0;
        }
        foreach (Transform card in GameObject.Find("Opponent").transform.Find("Hand").transform.Find("Canvas").transform.Find("HandUI").transform)
        {
            card.gameObject.GetComponent<CardScript>().debuffTimer--;
            if (card.gameObject.GetComponent<CardScript>().debuffTimer == 0) card.gameObject.GetComponent<CardScript>().externalManaChange = 0;
        }
        foreach(GameObject hp in GameObject.FindGameObjectsWithTag("Hero Power"))
            hp.GetComponent<HeroPowerScript>().usedThisTurn = false;
        if (turn == Turn.PLAYER)
        {
            foreach (GameObject minion in GameObject.FindGameObjectsWithTag("Player Minion"))
            {
                minion.GetComponent<MinionScript>().canAttack = true;
                minion.GetComponent<MinionScript>().attackedThisTurn = false;
                minion.GetComponent<MinionScript>().canAttackHeroes = true;
            }
            foreach (GameObject minion in GameObject.FindGameObjectsWithTag("Opponent Minion"))
            {
                minion.GetComponent<MinionScript>().canAttack = false;
                minion.GetComponent<MinionScript>().attackedThisTurn = false;
                minion.GetComponent<MinionScript>().canAttackHeroes = false;
            }
            if (GameObject.Find("Player").transform.Find("Hero").Find("Canvas").Find("Hero").GetComponent<HeroScript>()._currentEffects.Count > 0)
            {
                CurrentEffects _effect = default;
                foreach (CurrentEffects effect in GameObject.Find("Player").transform.Find("Hero").Find("Canvas").Find("Hero").GetComponent<HeroScript>()._currentEffects)
                {
                    _effect = effect;
                }
                _effect.howManyRounds--;
            }
        }
        else
        {
            foreach (GameObject minion in GameObject.FindGameObjectsWithTag("Player Minion"))
            {
                minion.GetComponent<MinionScript>().canAttack = false;
                minion.GetComponent<MinionScript>().attackedThisTurn = false;
                minion.GetComponent<MinionScript>().canAttackHeroes = false;
            }
            foreach (GameObject minion in GameObject.FindGameObjectsWithTag("Opponent Minion"))
            {
                minion.GetComponent<MinionScript>().canAttack = true;
                minion.GetComponent<MinionScript>().attackedThisTurn = false;
                minion.GetComponent<MinionScript>().canAttackHeroes = true;
            }
            if (GameObject.Find("Opponent").transform.Find("Hero").Find("Canvas").Find("Hero").GetComponent<HeroScript>()._currentEffects.Count > 0)
            {
                CurrentEffects _effect = default;
                foreach (CurrentEffects effect in GameObject.Find("Opponent").transform.Find("Hero").Find("Canvas").Find("Hero").GetComponent<HeroScript>()._currentEffects)
                {
                    _effect = effect;
                }
                _effect.howManyRounds--;
            }
        }
    }

    public void EndTurn()
    {
        if (endTurnTimer <= 0)
        {
            EndTurn(turn);
        }
    }

    public void EndTurn(Turn _turn)
    {
        EndOfTurnEffect();
        turn = _turn == Turn.PLAYER ? Turn.OPPONENT : Turn.PLAYER;
        GameObject.Find("Player").transform.Find("Battleground").Find("Canvas").Find("BattlegroundUI").GetComponent<Image>().raycastTarget = _turn == Turn.OPPONENT;
        GameObject.Find("Opponent").transform.Find("Battleground").Find("Canvas").Find("BattlegroundUI").GetComponent<Image>().raycastTarget = _turn == Turn.PLAYER;
        GameObject.Find("Player").transform.Find("Hand").Find("Canvas").Find("HandUI").Cast<Transform>().ToList().ForEach(delegate (Transform g) { g.GetComponent<RawImage>().raycastTarget = _turn == Turn.OPPONENT; });
        GameObject.Find("Opponent").transform.Find("Hand").Find("Canvas").Find("HandUI").Cast<Transform>().ToList().ForEach(delegate (Transform g) { g.GetComponent<RawImage>().raycastTarget = _turn == Turn.PLAYER; });
        NewTurn();
        endTurnTimer = 2f;
    }

    private void EndOfTurnEffect()
    {
        foreach(GameObject minion in GameObject.FindGameObjectsWithTag("Player Minion"))
        {
            bool value = false;
            foreach(MinionAbilites ab in minion.GetComponent<MinionScript>().Card.minionAbilites)
            {
                if (ab.minionAbility == MinionAbility.EndOfYourTurn) value = false;
                if (ab.minionAbility == MinionAbility.EndOfEveryTurn) value = true;
            }
            minion.GetComponent<MinionScript>().EndOfTurn(value);
        }
        foreach(GameObject minion in GameObject.FindGameObjectsWithTag("Opponent Minion"))
        {
            bool value = false;
            foreach (MinionAbilites ab in minion.GetComponent<MinionScript>().Card.minionAbilites)
            {
                if (ab.minionAbility == MinionAbility.EndOfYourTurn) value = false;
                if (ab.minionAbility == MinionAbility.EndOfEveryTurn) value = true;
            }
            minion.GetComponent<MinionScript>().EndOfTurn(value);
        }
    }
    
    public void AddNewCardToHand(HeroScript hero, CardSO card)
    {
        if (hero.hand.Count < 10)
        {
            GameObject newCard = Instantiate(cardPrefab);
            newCard.transform.SetParent(hero.gameObject.transform.parent.parent.parent.Find("Hand").transform.Find("Canvas").transform.Find("HandUI").transform);
            newCard.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            newCard.GetComponent<CardScript>().card = card;
            newCard.name = card.cardName;
            hero.hand.Add(newCard);
        }
    }

    public void AddNewLog(CardScript card, Turn turn)
    {
        LogRound log = new()
        {
            Card = card,
            Turn = turn,
            RoundNumber = round
        };
        Logs.Add(log);
        //print(log.Log());
    }

    public void AddNewLog(CardScript card, Turn turn, MinionScript minion)
    {
        LogRound log = new()
        {
            Card = card,
            Turn = turn,
            RoundNumber = round,
            TargetedCard = minion
        };
        Logs.Add(log);
        print(log.Log());
    }

    public void AddNewLog(CardScript card, Turn turn, HeroScript hero)
    {
        LogRound log = new()
        {
            Card = card,
            Turn = turn,
            RoundNumber = round,
            TargetedHero = hero
        };
        Logs.Add(log);
        print(log.Log());
    }
    
    IEnumerator SpawnNewLog()
    {
        // TODO
        //GameObject log = Instantiate(_logPrefab, _globalCanvas)

        float beginTime = Time.time;
        while (beginTime < Time.time + 2f)
            yield return null;


    }
}
