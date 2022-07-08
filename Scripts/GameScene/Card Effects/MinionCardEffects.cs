using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Threading.Tasks;

public class MinionCardEffects : MonoBehaviour
{
    public GameObject minionPrefab;
    public GameObject cardPrefab;
    public GameObject cardOnBoardPrefab;

    private GameplayManager gameplayManager => GameObject.Find("GameplayManager").GetComponent<GameplayManager>();

    public CardSO defaultCard;

    public int ChildCount(int _childCount = 0)
    {
        if (GameplayManager.turn == Turn.PLAYER)
        {
            foreach (GameObject taunt in GameObject.FindGameObjectsWithTag("Player Minion"))
            {
                _childCount++;
            }
        }
        else if (GameplayManager.turn == Turn.OPPONENT)
        {
            foreach (GameObject taunt in GameObject.FindGameObjectsWithTag("Opponent Minion"))
            {
                _childCount++;
            }
        }
        return _childCount;
    }

    public int OpponentChildCount(int _childCount = 0)
    {
        if (GameplayManager.turn == Turn.OPPONENT)
        {
            foreach (GameObject taunt in GameObject.FindGameObjectsWithTag("Player Minion"))
            {
                _childCount++;
            }
        }
        else if (GameplayManager.turn == Turn.PLAYER)
        {
            foreach (GameObject taunt in GameObject.FindGameObjectsWithTag("Opponent Minion"))
            {
                _childCount++;
            }
        }
        return _childCount;
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

    #region Classic Cards
    public void ElvenArcher(List<GameObject> target, MinionScript minion)
    {
        if (target.Count != 0)
        {
            foreach (GameObject _target in target)
            {
                if (_target.CompareTag("Hero"))
                {
                    float armordanSonraGidecekHealth = 2 > _target.GetComponent<HeroScript>().armor ? Mathf.Abs(2 - _target.GetComponent<HeroScript>().armor) : 0;
                    _target.GetComponent<HeroScript>().armor -= 2;
                    _target.GetComponent<HeroScript>().currentHP -= armordanSonraGidecekHealth;
                    if (_target.GetComponent<HeroScript>().currentHP <= 0)
                    {
                        _target.GetComponent<SpriteRenderer>().color = new Color32(118, 46, 46, 255);
                        print(_target.name + " oyunu kaybetti.");
                    }
                }
                else if (_target.CompareTag("Player Minion") || _target.CompareTag("Opponent Minion"))
                {
                    _target.GetComponent<MinionScript>().TakeDamage(2);
                }
            }
            minion.GetComponent<MinionScript>().hasDivineShield = false;
        }
        else
        {
            List<GameObject> pool = new List<GameObject>();
            foreach (GameObject pMinion in GameObject.FindGameObjectsWithTag("Player Minion")) pool.Add(pMinion);
            foreach (GameObject oMinion in GameObject.FindGameObjectsWithTag("Opponent Minion")) pool.Add(oMinion);
            pool.Add(GameObject.Find("Player").transform.Find("Hero").transform.Find("Canvas").transform.Find("Hero").gameObject);
            pool.Add(GameObject.Find("Opponent").transform.Find("Hero").transform.Find("Canvas").transform.Find("Hero").gameObject);
            GameObject _target = pool[Random.Range(0, pool.Count)];
            if (_target.CompareTag("Hero"))
            {
                float armordanSonraGidecekHealth = 2 > _target.GetComponent<HeroScript>().armor ? Mathf.Abs(2 - _target.GetComponent<HeroScript>().armor) : 0;
                _target.GetComponent<HeroScript>().armor -= 2;
                _target.GetComponent<HeroScript>().currentHP -= armordanSonraGidecekHealth;

                foreach (MinionAbilites _card in _target.GetComponent<MinionScript>().Card.minionAbilites)
                {
                    if (_card.minionAbility == MinionAbility.Deathrattle)
                    {
                        System.Type scriptType = typeof(MinionCardEffects);
                        MethodInfo info = scriptType.GetMethod(_card.minionEffect);
                        info?.Invoke(GameObject.Find("GameplayManager").GetComponent<MinionCardEffects>(), new object[] { new List<GameObject> { gameObject }, this });
                    }
                }
                if (_target.GetComponent<HeroScript>().currentHP <= 0)
                {
                    _target.GetComponent<SpriteRenderer>().color = new Color32(118, 46, 46, 255);
                    print(_target.name + " oyunu kaybetti.");
                }
            }
            else if (_target.CompareTag("Player Minion") || _target.CompareTag("Opponent Minion"))
            {
                _target.GetComponent<MinionScript>().currentHP -= 2;
                if (_target.GetComponent<MinionScript>().currentHP <= 0)
                {
                    _target.GetComponent<RawImage>().color = new Color32(118, 46, 46, 255);
                    Destroy(_target, .7f);
                }
            }
        }
    }

    public void HornyFuck(List<GameObject> target, MinionScript minion)
    {
        if (target.Count != 0)
        {
            // target hero olduðu için [if target.comparetag hero] yapmýyorum
            foreach (GameObject _target in target)
            {
                if (_target.CompareTag("Hero"))
                {
                    float armordanSonraGidecekHealth = 3 > _target.GetComponent<HeroScript>().armor ? Mathf.Abs(3 - _target.GetComponent<HeroScript>().armor) : 0;
                    _target.GetComponent<HeroScript>().armor -= 3;
                    _target.GetComponent<HeroScript>().currentHP -= armordanSonraGidecekHealth;
                    if (_target.GetComponent<HeroScript>().currentHP <= 0)
                    {
                        _target.GetComponent<SpriteRenderer>().color = new Color32(118, 46, 46, 255);
                        print(_target.name + " oyunu kaybetti.");
                    }
                }
                else if (_target.CompareTag("Player Minion") || _target.CompareTag("Opponent Minion"))
                {
                    _target.GetComponent<MinionScript>().currentHP -= 3;
                    if (_target.GetComponent<MinionScript>().currentHP <= 0)
                    {
                        minion.currentAttack += 3;
                        _target.GetComponent<RawImage>().color = new Color32(118, 46, 46, 255);

                        foreach (MinionAbilites _card in _target.GetComponent<MinionScript>().Card.minionAbilites)
                        {
                            if (_card.minionAbility == MinionAbility.Deathrattle)
                            {
                                System.Type scriptType = typeof(MinionCardEffects);
                                MethodInfo info = scriptType.GetMethod(_card.minionEffect);
                                info?.Invoke(GameObject.Find("GameplayManager").GetComponent<MinionCardEffects>(), new object[] { new List<GameObject> { gameObject }, this });
                            }
                        }
                        Destroy(_target, .7f);
                    }
                }
            }
        }
        else
        {
            List<GameObject> pool = new List<GameObject>();
            foreach (GameObject pMinion in GameObject.FindGameObjectsWithTag("Player Minion")) pool.Add(pMinion);
            foreach (GameObject oMinion in GameObject.FindGameObjectsWithTag("Opponent Minion")) pool.Add(oMinion);
            pool.Add(GameObject.Find("Player").transform.Find("Hero").transform.Find("Canvas").transform.Find("Hero").gameObject);
            pool.Add(GameObject.Find("Opponent").transform.Find("Hero").transform.Find("Canvas").transform.Find("Hero").gameObject);
            GameObject _target = pool[Random.Range(0, pool.Count)];
            if (_target.CompareTag("Hero"))
            {
                float armordanSonraGidecekHealth = 3 > _target.GetComponent<HeroScript>().armor ? Mathf.Abs(3 - _target.GetComponent<HeroScript>().armor) : 0;
                _target.GetComponent<HeroScript>().armor -= 3;
                _target.GetComponent<HeroScript>().currentHP -= armordanSonraGidecekHealth;
                if (_target.GetComponent<HeroScript>().currentHP <= 0)
                {
                    _target.GetComponent<SpriteRenderer>().color = new Color32(118, 46, 46, 255);

                    print(_target.name + " oyunu kaybetti.");
                }
            }
            else if (_target.CompareTag("Player Minion") || _target.CompareTag("Opponent Minion"))
            {
                _target.GetComponent<MinionScript>().currentHP -= 3;
                if (_target.GetComponent<MinionScript>().currentHP <= 0)
                {
                    minion.currentAttack += 3;
                    _target.GetComponent<RawImage>().color = new Color32(118, 46, 46, 255);

                    foreach (MinionAbilites _card in _target.GetComponent<MinionScript>().Card.minionAbilites)
                    {
                        if (_card.minionAbility == MinionAbility.Deathrattle)
                        {
                            System.Type scriptType = typeof(MinionCardEffects);
                            MethodInfo info = scriptType.GetMethod(_card.minionEffect);
                            info?.Invoke(GameObject.Find("GameplayManager").GetComponent<MinionCardEffects>(), new object[] { new List<GameObject> { gameObject }, this });
                        }
                    }
                    Destroy(_target, .7f);
                }
            }
        }
    }

    public void BrokenControlCard(List<GameObject> target, MinionScript minion)
    {
        // target hero olduðu için [if target.comparetag hero] yapmýyorum
        foreach (GameObject _target in GameObject.FindGameObjectsWithTag("Hero"))
        {
            _target.GetComponent<HeroScript>().armor += 4;
        }
    }

    public void AmanThul(List<GameObject> target, MinionScript minion)
    {
        if (minion.transform.root.name == "Player")
        {
            foreach (GameObject _target in GameObject.FindGameObjectsWithTag("Opponent Minion"))
            {
                _target.GetComponent<MinionScript>().currentHP -= 7;
                if (_target.GetComponent<MinionScript>().currentHP <= 0)
                {
                    _target.GetComponent<RawImage>().color = new Color32(118, 46, 46, 255);
                    Destroy(_target, .7f);
                }
            }
        }
        else if (minion.transform.root.name == "Opponent")
        {
            foreach (GameObject _target in GameObject.FindGameObjectsWithTag("Player Minion"))
            {
                _target.GetComponent<MinionScript>().currentHP -= 7;
                if (_target.GetComponent<MinionScript>().currentHP <= 0)
                {
                    _target.GetComponent<RawImage>().color = new Color32(118, 46, 46, 255);

                    foreach (MinionAbilites _card in _target.GetComponent<MinionScript>().Card.minionAbilites)
                    {
                        if (_card.minionAbility == MinionAbility.Deathrattle)
                        {
                            System.Type scriptType = typeof(MinionCardEffects);
                            MethodInfo info = scriptType.GetMethod(_card.minionEffect);
                            info?.Invoke(GameObject.Find("GameplayManager").GetComponent<MinionCardEffects>(), new object[] { new List<GameObject> { gameObject }, this });
                        }
                    }
                    Destroy(_target, .7f);
                }
            }
        }
    }

    public void HayirlisiNeyseO(List<GameObject> target, MinionScript minion)
    {
        // each OTHER card demediðim için kendisini de katacak
        minion.currentAttack += minion.transform.root.transform.Find("Hero").transform.Find("Canvas").transform.Find("Hero").GetComponent<HeroScript>().hand.Count;
    }

    public void DoppelBanger(List<GameObject> target, MinionScript minion)
    {
        if (ChildCount() < 7)
        {
            GameObject newMinion = Instantiate(minionPrefab, minion.transform.root.name == "Player" ? GameObject.Find("Player").transform.Find("Battleground").transform.Find("Canvas").transform.Find("BattlegroundUI").transform : GameObject.Find("Opponent").transform.Find("Battleground").transform.Find("Canvas").transform.Find("BattlegroundUI").transform);
            newMinion.GetComponent<MinionScript>().Card = minion.Card;
            newMinion.name = minion.name;
            newMinion.tag = newMinion.transform.root.name == "Player" ? "Player Minion" : "Opponent Minion";
            newMinion.GetComponent<MinionScript>().numberOnBoard = minion.numberOnBoard;
            newMinion.GetComponent<MinionScript>().UpdateStats();
            newMinion.GetComponent<MinionScript>().currentAttack = minion.currentAttack;
            newMinion.GetComponent<MinionScript>().currentHP = minion.currentHP;
            newMinion.GetComponent<MinionScript>().defaultAttack = minion.defaultAttack;
            newMinion.GetComponent<MinionScript>().defaultHP = minion.defaultHP;

            foreach (GameObject _minion in GameObject.FindGameObjectsWithTag($"{newMinion.transform.root.name} Minion"))
            {
                if (newMinion.GetComponent<MinionScript>().numberOnBoard <= _minion.GetComponent<MinionScript>().numberOnBoard && newMinion != _minion)
                {
                    _minion.GetComponent<MinionScript>().numberOnBoard++;
                }
                _minion.GetComponent<RectTransform>().DOAnchorPos(_minion.GetComponent<MinionScript>().BasePosition(), .5f);
            }
        }
    }

    public void AlextrazsaTheSalak(List<GameObject> target, MinionScript minion)
    {
        if (target.Count != 0)
        {
            foreach (GameObject _target in target)
            {
                if (_target.CompareTag("Hero"))
                {
                    _target.GetComponent<HeroScript>().currentHP = 15;
                }
            }
        }
        else
        {
            List<GameObject> pool = new List<GameObject>();
            pool.Add(GameObject.Find("Player").transform.Find("Hero").transform.Find("Canvas").transform.Find("Hero").gameObject);
            pool.Add(GameObject.Find("Opponent").transform.Find("Hero").transform.Find("Canvas").transform.Find("Hero").gameObject);
            GameObject _target = pool[Random.Range(0, pool.Count)];
            _target.GetComponent<HeroScript>().currentHP = 15;
        }
    }

    public void LeeroyBenkins(List<GameObject> target, MinionScript minion)
    {
        for (int i = 0; i < 2; i++)
        {
            if (OpponentChildCount() < 7)
            {
                GameObject newMinion = Instantiate(minionPrefab, minion.transform.root.name == "Player" ? GameObject.Find("Opponent").transform.Find("Battleground").transform.Find("Canvas").transform.Find("BattlegroundUI").transform : GameObject.Find("Player").transform.Find("Battleground").transform.Find("Canvas").transform.Find("BattlegroundUI").transform);
                newMinion.GetComponent<MinionScript>().Card = minion.Card.tokens[0];
                newMinion.GetComponent<MinionScript>().UpdateStats();
                newMinion.name = minion.Card.tokens[0].name;
                newMinion.tag = newMinion.transform.root.name == "Player" ? "Player Minion" : "Opponent Minion"; 
                newMinion.GetComponent<MinionScript>().numberOnBoard = (int)OpponentChildCount()+1;

                if (newMinion.transform.root.name == "Player")
                {
                    foreach (GameObject _minion in GameObject.FindGameObjectsWithTag("Player Minion"))
                    {
                        if (newMinion.GetComponent<MinionScript>().numberOnBoard <= _minion.GetComponent<MinionScript>().numberOnBoard)
                        {
                            if (newMinion != _minion) _minion.GetComponent<MinionScript>().numberOnBoard++;
                        }
                        _minion.GetComponent<RectTransform>().DOAnchorPos(_minion.GetComponent<MinionScript>().BasePosition(), .5f);
                    }
                }
                else if (newMinion.transform.root.name == "Opponent")
                {
                    foreach (GameObject _minion in GameObject.FindGameObjectsWithTag("Opponent Minion"))
                    {
                        if (newMinion.GetComponent<MinionScript>().numberOnBoard <= _minion.GetComponent<MinionScript>().numberOnBoard)
                        {
                            if (newMinion != _minion) _minion.GetComponent<MinionScript>().numberOnBoard++;
                        }
                        _minion.GetComponent<RectTransform>().DOAnchorPos(_minion.GetComponent<MinionScript>().BasePosition(), .5f);
                    }
                }
            }

        }
    }

    public void BloodthirstyAbomination(List<GameObject> target, MinionScript minion)
    {
        if (minion.transform.root.name == "Player")
        {
            if (GameObject.FindGameObjectsWithTag("Opponent Minion").Length != 0)
            {
                GameObject _target = GameObject.FindGameObjectsWithTag("Opponent Minion")[Random.Range(0, GameObject.FindGameObjectsWithTag("Opponent Minion").Length)];
                _target.GetComponent<MinionScript>().skullPanel.SetActive(true);
                _target.tag = "Dead Opponent Minion";
                Destroy(_target, .7f);
            }
        }
        if (minion.transform.root.name == "Opponent")
        {
            if (GameObject.FindGameObjectsWithTag("Player Minion").Length != 0)
            {
                GameObject _target = GameObject.FindGameObjectsWithTag("Player Minion")[Random.Range(0, GameObject.FindGameObjectsWithTag("Player Minion").Length)];
                _target.GetComponent<MinionScript>().skullPanel.SetActive(true);
                _target.tag = "Dead Player Minion";
                Destroy(_target, .7f);
            }
        }
    }

    public void CairneBloodhoof(List<GameObject> target, MinionScript minion)
    {
        GameObject newMinion = Instantiate(minionPrefab, minion.transform.root.name == "Player" ? GameObject.Find("Player").transform.Find("Battleground").transform.Find("Canvas").transform.Find("BattlegroundUI").transform : GameObject.Find("Opponent").transform.Find("Battleground").transform.Find("Canvas").transform.Find("BattlegroundUI").transform);
        newMinion.GetComponent<MinionScript>().Card = minion.Card.tokens[0];
        newMinion.GetComponent<MinionScript>().UpdateStats();
        newMinion.name = minion.Card.tokens[0].name;
        newMinion.tag = newMinion.transform.root.name == "Player" ? "Player Minion" : "Opponent Minion";
        newMinion.GetComponent<MinionScript>().numberOnBoard = newMinion.transform.root.name == "Player" ? ChildCount()+1 : OpponentChildCount()+1;
        if (newMinion.transform.root.name == "Player")
        {
            foreach (GameObject _minion in GameObject.FindGameObjectsWithTag("Player Minion"))
            {
                if (newMinion.GetComponent<MinionScript>().numberOnBoard <= _minion.GetComponent<MinionScript>().numberOnBoard)
                {
                    if (newMinion != _minion) _minion.GetComponent<MinionScript>().numberOnBoard++;
                }
                _minion.GetComponent<RectTransform>().DOAnchorPos(_minion.GetComponent<MinionScript>().BasePosition(), .5f);
            }
        }
        else if (newMinion.transform.root.name == "Opponent")
        {
            foreach (GameObject _minion in GameObject.FindGameObjectsWithTag("Opponent Minion"))
            {
                if (newMinion.GetComponent<MinionScript>().numberOnBoard <= _minion.GetComponent<MinionScript>().numberOnBoard)
                {
                    if (newMinion != _minion) _minion.GetComponent<MinionScript>().numberOnBoard++;
                }
                _minion.GetComponent<RectTransform>().DOAnchorPos(_minion.GetComponent<MinionScript>().BasePosition(), .5f);
            }
        }
    }

    // battle-crybaby: eðer invoke adý battlecrybaby deðilse invokela. eðer target yoksa random
    public void Battlecrybaby(List<GameObject> target, MinionScript minion)
    {
        if (GameplayManager.turn == Turn.PLAYER)
        {
            foreach (GameObject _target in GameObject.FindGameObjectsWithTag("Player Minion"))
            {
                foreach (MinionAbilites _card in _target.GetComponent<MinionScript>().Card.minionAbilites)
                {
                    if (_card.minionEffect != "Battlecrybaby" && _card.minionAbility == MinionAbility.Battlecry)
                    {
                        System.Type scriptType = typeof(MinionCardEffects);
                        MethodInfo info = scriptType.GetMethod(_card.minionEffect);
                        info?.Invoke(this, new object[] { new List<GameObject> { }, minion });
                    }
                }
            }
        }
        else if (GameplayManager.turn == Turn.OPPONENT)
        {
            foreach (GameObject _target in GameObject.FindGameObjectsWithTag("Opponent Minion"))
            {
                foreach (MinionAbilites _card in _target.GetComponent<MinionScript>().Card.minionAbilites)
                {
                    if (_card.minionEffect != "Battlecrybaby" && _card.minionAbility == MinionAbility.Battlecry)
                    {
                        System.Type scriptType = typeof(MinionCardEffects);
                        MethodInfo info = scriptType.GetMethod(_card.minionEffect);
                        info?.Invoke(this, new object[] { new List<GameObject> { }, minion });
                    }
                }
            }
        }
    }

    public void ShudderHoarder(List<GameObject> target, MinionScript minion)
    {
        gameplayManager.DrawCard(minion.gameObject, minion.transform.root.name == "Player" ? Turn.PLAYER : Turn.OPPONENT);
    }

    public void WitchsCultNeopythe(List<GameObject> target, MinionScript minion)
    {
        if (GameplayManager.turn == Turn.PLAYER)
        {
            GameObject.Find("Opponent").transform.Find("Hero").transform.Find("Canvas").transform.Find("Hero").GetComponent<HeroScript>().externalMinionManaDebuff += 1;
            GameObject.Find("Opponent").transform.Find("Hero").transform.Find("Canvas").transform.Find("Hero").GetComponent<HeroScript>().debuffTimer = 2;
        }
        else if (GameplayManager.turn == Turn.OPPONENT)
        {
            GameObject.Find("Player").transform.Find("Hero").transform.Find("Canvas").transform.Find("Hero").GetComponent<HeroScript>().externalMinionManaDebuff += 1;
            GameObject.Find("Player").transform.Find("Hero").transform.Find("Canvas").transform.Find("Hero").GetComponent<HeroScript>().debuffTimer = 2;
        }
    }

    public void ShapegiverMalicia(List<GameObject> target, MinionScript minion)
    {
        if (ChildCount() < 7)
        {
            minion.Card.tokens[0].mana = minion.transform.root.transform.Find("Hero").transform.Find("Canvas").transform.Find("Hero").GetComponent<HeroScript>().hand.Count;
            minion.Card.tokens[0].attack = minion.transform.root.transform.Find("Hero").transform.Find("Canvas").transform.Find("Hero").GetComponent<HeroScript>().hand.Count;
            minion.Card.tokens[0].hp = minion.transform.root.transform.Find("Hero").transform.Find("Canvas").transform.Find("Hero").GetComponent<HeroScript>().hand.Count;
            GameObject newMinion = Instantiate(minionPrefab, minion.transform.root.name == "Player" ? GameObject.Find("Player").transform.Find("Battleground").transform.Find("Canvas").transform.Find("BattlegroundUI").transform : GameObject.Find("Opponent").transform.Find("Battleground").transform.Find("Canvas").transform.Find("BattlegroundUI").transform);
            newMinion.GetComponent<MinionScript>().Card = minion.Card.tokens[0];
            newMinion.GetComponent<MinionScript>().UpdateStats();
            newMinion.tag = newMinion.transform.root.name == "Player" ? "Player Minion" : "Opponent Minion";
            newMinion.GetComponent<MinionScript>().numberOnBoard = minion.numberOnBoard;
            if (newMinion.transform.root.name == "Player")
            {
                foreach (GameObject _minion in GameObject.FindGameObjectsWithTag("Player Minion"))
                {
                    if (newMinion.GetComponent<MinionScript>().numberOnBoard <= _minion.GetComponent<MinionScript>().numberOnBoard)
                    {
                        if (newMinion != _minion) _minion.GetComponent<MinionScript>().numberOnBoard++;
                    }
                    _minion.GetComponent<RectTransform>().DOAnchorPos(_minion.GetComponent<MinionScript>().BasePosition(), .5f);
                }
            }
            else if (newMinion.transform.root.name == "Opponent")
            {
                foreach (GameObject _minion in GameObject.FindGameObjectsWithTag("Opponent Minion"))
                {
                    if (newMinion.GetComponent<MinionScript>().numberOnBoard <= _minion.GetComponent<MinionScript>().numberOnBoard)
                    {
                        if (newMinion != _minion) _minion.GetComponent<MinionScript>().numberOnBoard++;
                    }
                    _minion.GetComponent<RectTransform>().DOAnchorPos(_minion.GetComponent<MinionScript>().BasePosition(), .5f);
                }
            }
        }
    }


    public void FungalLord(List<GameObject> target, MinionScript minion)
    {
        if (ChildCount() < 7)
        {
            if (target.Count != 0)
            {
                print(target);
                defaultCard.cardType = CardType.MINION;
                defaultCard.cardSprite = target[0].GetComponent<MinionScript>().Card.cardSprite;
                defaultCard.minionAbilites = target[0].GetComponent<MinionScript>().Card.minionAbilites;
                defaultCard.mana = 3;
                defaultCard.attack = 3;
                defaultCard.hp = 3;
                GameObject newMinion = Instantiate(minionPrefab, minion.transform.root.name == "Player" ? GameObject.Find("Player").transform.Find("Battleground").transform.Find("Canvas").transform.Find("BattlegroundUI").transform : GameObject.Find("Opponent").transform.Find("Battleground").transform.Find("Canvas").transform.Find("BattlegroundUI").transform);
                newMinion.GetComponent<MinionScript>().Card = defaultCard;
                newMinion.GetComponent<MinionScript>().UpdateStats();
                newMinion.tag = newMinion.transform.root.name == "Player" ? "Player Minion" : "Opponent Minion";
                newMinion.GetComponent<MinionScript>().numberOnBoard = minion.numberOnBoard;
                if (newMinion.transform.root.name == "Player")
                {
                    foreach (GameObject _minion in GameObject.FindGameObjectsWithTag("Player Minion"))
                    {
                        if (newMinion.GetComponent<MinionScript>().numberOnBoard <= _minion.GetComponent<MinionScript>().numberOnBoard)
                        {
                            if (newMinion != _minion) _minion.GetComponent<MinionScript>().numberOnBoard++;
                        }
                        _minion.GetComponent<RectTransform>().DOAnchorPos(_minion.GetComponent<MinionScript>().BasePosition(), .5f);
                    }
                }
                else if (newMinion.transform.root.name == "Opponent")
                {
                    foreach (GameObject _minion in GameObject.FindGameObjectsWithTag("Opponent Minion"))
                    {
                        if (newMinion.GetComponent<MinionScript>().numberOnBoard <= _minion.GetComponent<MinionScript>().numberOnBoard)
                        {
                            if (newMinion != _minion) _minion.GetComponent<MinionScript>().numberOnBoard++;
                        }
                        _minion.GetComponent<RectTransform>().DOAnchorPos(_minion.GetComponent<MinionScript>().BasePosition(), .5f);
                    }
                }
            }
            else
            {
                List<GameObject> pool = new List<GameObject>();
                foreach (GameObject pMinion in GameObject.FindGameObjectsWithTag("Player Minion")) pool.Add(pMinion);
                foreach (GameObject oMinion in GameObject.FindGameObjectsWithTag("Opponent Minion")) pool.Add(oMinion);
                if (pool.Count > 0)
                {
                    GameObject _target = pool[Random.Range(0, pool.Count)];
                    defaultCard.cardType = CardType.MINION;
                    defaultCard.cardSprite = _target.GetComponent<MinionScript>().Card.cardSprite;
                    defaultCard.minionAbilites = _target.GetComponent<MinionScript>().Card.minionAbilites;
                    defaultCard.mana = _target.GetComponent<MinionScript>().Card.mana;
                    defaultCard.attack = _target.GetComponent<MinionScript>().Card.attack;
                    defaultCard.hp = _target.GetComponent<MinionScript>().Card.hp;
                    GameObject newMinion = Instantiate(minionPrefab, minion.transform.root.name == "Player" ? GameObject.Find("Player").transform.Find("Battleground").transform.Find("Canvas").transform.Find("BattlegroundUI").transform : GameObject.Find("Opponent").transform.Find("Battleground").transform.Find("Canvas").transform.Find("BattlegroundUI").transform);
                    newMinion.GetComponent<MinionScript>().Card = defaultCard;
                    newMinion.GetComponent<MinionScript>().UpdateStats();
                    newMinion.tag = newMinion.transform.root.name == "Player" ? "Player Minion" : "Opponent Minion";
                    newMinion.GetComponent<MinionScript>().numberOnBoard = minion.numberOnBoard;
                    if (newMinion.transform.root.name == "Player")
                    {
                        foreach (GameObject _minion in GameObject.FindGameObjectsWithTag("Player Minion"))
                        {
                            if (newMinion.GetComponent<MinionScript>().numberOnBoard <= _minion.GetComponent<MinionScript>().numberOnBoard)
                            {
                                if (newMinion != _minion) _minion.GetComponent<MinionScript>().numberOnBoard++;
                            }
                            _minion.GetComponent<RectTransform>().DOAnchorPos(_minion.GetComponent<MinionScript>().BasePosition(), .5f);
                        }
                    }
                    else if (newMinion.transform.root.name == "Opponent")
                    {
                        foreach (GameObject _minion in GameObject.FindGameObjectsWithTag("Opponent Minion"))
                        {
                            if (newMinion.GetComponent<MinionScript>().numberOnBoard <= _minion.GetComponent<MinionScript>().numberOnBoard)
                            {
                                if (newMinion != _minion) _minion.GetComponent<MinionScript>().numberOnBoard++;
                            }
                            _minion.GetComponent<RectTransform>().DOAnchorPos(_minion.GetComponent<MinionScript>().BasePosition(), .5f);
                        }
                    }
                }
            }
        }
    }

    public void Cenaryuz(List<GameObject> target, MinionScript minion)
    {
        if (minion.transform.root.name == "Player")
        {
            foreach (GameObject _target in GameObject.FindGameObjectsWithTag("Player Minion"))
            {
                _target.GetComponent<MinionScript>().currentAttack += 2;
                _target.GetComponent<MinionScript>().currentHP += 2;
            }
        }
        else if (minion.transform.root.name == "Opponent")
        {
            print("cenar");
            foreach (GameObject _target in GameObject.FindGameObjectsWithTag("Opponent Minion"))
            {
                print("aryuz");
                _target.GetComponent<MinionScript>().currentAttack += 2;
                _target.GetComponent<MinionScript>().currentHP += 2;
            }
        }
        minion.GetComponent<MinionScript>().currentAttack -= 2;
        minion.GetComponent<MinionScript>().currentHP -= 2;
        print("cenaryuz");
    }

    public void EscapedManasaber(List<GameObject> target, MinionScript minion)
    {
        if (minion.transform.root.name == "Player")
        {
            GameObject.Find("Player").transform.Find("Hero").transform.Find("Canvas").transform.Find("Hero").GetComponent<HeroScript>().currentMana += 2;
        }
        if (minion.transform.root.name == "Opponent")
        {
            GameObject.Find("Opponent").transform.Find("Hero").transform.Find("Canvas").transform.Find("Hero").GetComponent<HeroScript>().currentMana += 2;
        }
    }
    #endregion

    #region Warma cards
    public void WarmaThePissed_Battlecry(List<GameObject> target, MinionScript minion)
    {
        if (target != null)
        {
            GameObject _target = target[0];
            _target.GetComponent<MinionScript>().TakeDamage(minion.gameObject);
            minion.GetComponent<MinionScript>().TakeDamage(_target);
            if (minion.currentHP > 0)
            {
                minion.currentAttack += 5;
                minion.currentHP += 5;
            }
        }
    }

    public void WarmaThePissed_Deathrattle(List<GameObject> target, MinionScript minion)
    {
        HeroScript mainHero = minion.transform.root.Find("Hero").transform.Find("Canvas").transform.Find("Hero").GetComponent<HeroScript>();
        mainHero.warmaThePissed = true;
        if (mainHero.warmaThePissed && mainHero.warmaTheAnnoyed && mainHero.warmaTheExhausted && mainHero.warmaTheBeliever)
        {
            GameObject newCard = Instantiate(cardPrefab, mainHero.gameObject.transform.parent.parent.parent.Find("Hand").transform.Find("Canvas").transform.Find("HandUI").transform);
            newCard.GetComponent<CardScript>().card = minion.Card.tokens[0];
            newCard.name = minion.Card.tokens[0].cardName;
            mainHero.hand.Add(newCard);
        }
    }

    public void WarmaTheAnnoyed_Battlecry(List<GameObject> target, MinionScript minion)
    {
        GameObject _target = target[0];
        _target.GetComponent<MinionScript>().TakeDamage(minion.gameObject);
        minion.GetComponent<MinionScript>().TakeDamage(_target.gameObject);
        if (minion.currentHP > 0)
        {
            minion.currentAttack += 5;
            minion.currentHP += 5;
        }
    }

    public void WarmaTheAnnoyed_Deathrattle(List<GameObject> target, MinionScript minion)
    {
        HeroScript mainHero = minion.transform.root.Find("Hero").transform.Find("Canvas").transform.Find("Hero").GetComponent<HeroScript>();
        mainHero.warmaTheAnnoyed = true;
        if (mainHero.warmaThePissed && mainHero.warmaTheAnnoyed && mainHero.warmaTheExhausted && mainHero.warmaTheBeliever)
        {
            GameObject newCard = Instantiate(cardPrefab, mainHero.gameObject.transform.parent.parent.parent.Find("Hand").transform.Find("Canvas").transform.Find("HandUI").transform);
            newCard.GetComponent<CardScript>().card = minion.Card.tokens[0];
            newCard.name = minion.Card.tokens[0].cardName;
            mainHero.hand.Add(newCard);
        }
    }

    public void WarmaTheExhausted_Battlecry(List<GameObject> target, MinionScript minion)
    {
    }

    public void WarmaTheExhausted_Deathrattle(List<GameObject> target, MinionScript minion)
    {
        HeroScript mainHero = minion.transform.root.Find("Hero").transform.Find("Canvas").transform.Find("Hero").GetComponent<HeroScript>();
        mainHero.warmaTheExhausted = true;
        if (mainHero.warmaThePissed && mainHero.warmaTheAnnoyed && mainHero.warmaTheExhausted && mainHero.warmaTheBeliever)
        {
            GameObject newCard = Instantiate(cardPrefab, mainHero.gameObject.transform.parent.parent.parent.Find("Hand").transform.Find("Canvas").transform.Find("HandUI").transform);
            newCard.GetComponent<CardScript>().card = minion.Card.tokens[0];
            newCard.name = minion.Card.tokens[0].cardName;
            mainHero.hand.Add(newCard);
        }
    }

    public void WarmaTheBeliever_Battlecry(List<GameObject> target, MinionScript minion)
    {

    }

    public void WarmaTheBeliever_Deathrattle(List<GameObject> target, MinionScript minion)
    {
        HeroScript mainHero = minion.transform.root.Find("Hero").transform.Find("Canvas").transform.Find("Hero").GetComponent<HeroScript>();
        mainHero.warmaTheBeliever = true;
        if (mainHero.warmaThePissed && mainHero.warmaTheAnnoyed && mainHero.warmaTheExhausted && mainHero.warmaTheBeliever)
        {
            GameObject newCard = Instantiate(cardPrefab, mainHero.gameObject.transform.parent.parent.parent.Find("Hand").transform.Find("Canvas").transform.Find("HandUI").transform);
            newCard.GetComponent<CardScript>().card = minion.Card.tokens[0];
            newCard.name = minion.Card.tokens[0].cardName;
            mainHero.hand.Add(newCard);
        }
    }
    #endregion

    #region Paladin Cards

    public void Googlebot(List<GameObject> target, MinionScript minion)
    {
        DeckScript deck = minion.transform.root.name == "Player" ? GameObject.Find("Player").transform.Find("Deck").GetComponent<DeckScript>() : GameObject.Find("Opponent").transform.Find("Deck").GetComponent<DeckScript>();
        foreach (CardSO card in deck.cards)
        {
            if (card.cardType == CardType.MINION)
            {
                MinionAbilites firstMA = card.minionAbilites.Find(m => m.minionAbility == MinionAbility.DivineShield);
                if (firstMA.minionAbility == MinionAbility.DivineShield)
                {
                    gameplayManager.DrawCard(minion.gameObject, minion.transform.root.name == "Player" ? Turn.PLAYER : Turn.OPPONENT, card);
                    return;
                }
            }
        }
    }

    public void CowardlyGolem(List<GameObject> target, MinionScript minion)
    {
        minion.hasDivineShield = true;
    }

    public void LordKyrie(List<GameObject> target, MinionScript minion)
    {
        if (OpponentChildCount() == 0) return;

        float lostAttack = target[0].GetComponent<MinionScript>().currentAttack - 1;
        float lostHealth = target[0].GetComponent<MinionScript>().currentHP - 1;
        target[0].GetComponent<MinionScript>().currentAttack = 1;
        target[0].GetComponent<MinionScript>().currentHP = 1;
        minion.GetComponent<MinionScript>().currentAttack += lostAttack;
        minion.GetComponent<MinionScript>().currentHP += lostHealth;
    }
    
    public void EluraProtectorOfLight(List<GameObject> target, MinionScript minion)
    {
        HeroScript mainHero = minion.transform.root.Find("Hero").transform.Find("Canvas").transform.Find("Hero").GetComponent<HeroScript>();
        foreach(GameObject card in mainHero.hand)
        {
            card.GetComponent<CardScript>().attack = 3;
            card.GetComponent<CardScript>().hp = 3;
        }
    }

    public void Flametouched(List<GameObject> target, MinionScript minion)
    {
        foreach (Transform min in GameObject.Find("Player").transform.Find("Battleground").Find("Canvas").Find("BattlegroundUI").transform)
        {
            min.gameObject.GetComponent<MinionScript>().hasDivineShield = true;

        }
        foreach (Transform min in GameObject.Find("Player").transform.Find("Battleground").Find("Canvas").Find("BattlegroundUI").transform)
        {
            min.gameObject.GetComponent<MinionScript>().hasDivineShield = true;
        }
    }
    
    public void MotivatedTauren_Battlecry(List<GameObject> target, MinionScript minion)
    {
        HeroScript mainHero = minion.transform.root.Find("Hero").transform.Find("Canvas").transform.Find("Hero").GetComponent<HeroScript>();
        CurrentEffects eff;
        eff.effectName = "MotivatedTauren";
        if (ExistingEffect("MotivatedTauren", minion.gameObject).effectName == eff.effectName)
        {
            CurrentEffects val = ExistingEffect("MotivatedTauren", minion.gameObject);
            minion.transform.root.Find("Hero").Find("Canvas").Find("Hero").GetComponent<HeroScript>()._currentEffects.Remove(ExistingEffect("DivineShieldCosts1Less", minion.gameObject));
            eff.howManyRounds = 999;
            eff.howManyStacked = val.howManyStacked + 1;
            minion.transform.root.Find("Hero").Find("Canvas").Find("Hero").GetComponent<HeroScript>()._currentEffects.Add(eff);
        }
        else
        {
            eff.howManyRounds = 999;
            eff.howManyStacked = 1;
            mainHero._currentEffects.Add(eff);
        }
    }
    public void MotivatedTauren_Deathrattle(List<GameObject> target, MinionScript minion)
    {
        HeroScript mainHero = minion.transform.root.Find("Hero").transform.Find("Canvas").transform.Find("Hero").GetComponent<HeroScript>();
        mainHero._currentEffects.Remove(ExistingEffect("MotivatedTauren", minion.gameObject));
       
    }
    
    public void CarielRoame_Battlecry(List<GameObject> target, MinionScript minion)
    {
        HeroScript mainHero = minion.transform.root.Find("Hero").transform.Find("Canvas").transform.Find("Hero").GetComponent<HeroScript>();
        CurrentEffects eff;
        eff.effectName = "CarielRoame";
        if (ExistingEffect("CarielRoame", minion.gameObject).effectName == eff.effectName)
        {
            CurrentEffects val = ExistingEffect("CarielRoame", minion.gameObject);
            minion.transform.root.Find("Hero").Find("Canvas").Find("Hero").GetComponent<HeroScript>()._currentEffects.Remove(ExistingEffect("DivineShieldCosts1Less", minion.gameObject));
            eff.howManyRounds = 999;
            eff.howManyStacked = val.howManyStacked + 1;
            minion.transform.root.Find("Hero").Find("Canvas").Find("Hero").GetComponent<HeroScript>()._currentEffects.Add(eff);
        }
        else
        {
            eff.howManyRounds = 999;
            eff.howManyStacked = 1;
            mainHero._currentEffects.Add(eff);
        }
    }
    public void CarielRoame_Deathrattle(List<GameObject> target, MinionScript minion)
    {
        HeroScript mainHero = minion.transform.root.Find("Hero").transform.Find("Canvas").transform.Find("Hero").GetComponent<HeroScript>();
        mainHero._currentEffects.Remove(ExistingEffect("CarielRoame", minion.gameObject));

    }

    public void TuralyonTheChampion(List<GameObject> target, MinionScript minion)
    {
        HeroScript mainHero = minion.transform.root.Find("Hero").transform.Find("Canvas").transform.Find("Hero").GetComponent<HeroScript>();
        CurrentEffects eff;
        eff.effectName = "TuralyonTheChampion";
        if (ExistingEffect("TuralyonTheChampion", minion.gameObject).effectName == eff.effectName)
        {
            CurrentEffects val = ExistingEffect("TuralyonTheChampion", minion.gameObject);
            minion.transform.root.Find("Hero").Find("Canvas").Find("Hero").GetComponent<HeroScript>()._currentEffects.Remove(ExistingEffect("DivineShieldCosts1Less", minion.gameObject));
            eff.howManyRounds = 999;
            eff.howManyStacked = val.howManyStacked + 1;
            minion.transform.root.Find("Hero").Find("Canvas").Find("Hero").GetComponent<HeroScript>()._currentEffects.Add(eff);
        }
        else
        {
            eff.howManyRounds = 999;
            eff.howManyStacked = 1;
            mainHero._currentEffects.Add(eff);
        }
    }

    #endregion

    #region Druid Cards 

    public void ManaSpirit(List<GameObject> target, MinionScript minion)
    {
        int minionCount = 0;
        foreach (GameObject min in GameObject.FindGameObjectsWithTag("Player Minion"))
        {
            minionCount++;
        }
        foreach (GameObject min in GameObject.FindGameObjectsWithTag("Opponent Minion"))
        {
            minionCount++;
        }
        minion.gameObject.transform.root.Find("Hero").Find("Canvas").Find("Hero").GetComponent<HeroScript>().currentMana += minionCount - 1;
    }

    public async void Bananaman(List<GameObject> target, MinionScript minion)
    {
        HeroScript hero = minion.transform.root.Find("Hero").transform.Find("Canvas").transform.Find("Hero").GetComponent<HeroScript>();

        GlobalCardEffects.ShuffleToDeck(hero, hero.deck, minion.Card.tokens[0], -1, 3);

        await Task.Delay(100);
        for (int i = -1; i <= 1; i++)
        {
            GameObject bananaCrate = Instantiate(cardOnBoardPrefab, minion.transform.position + Vector3.right * (i * 2), Quaternion.identity, minion.transform);
            bananaCrate.transform.localScale = 0.5f * Vector3.one;
            bananaCrate.GetComponent<RawImage>().texture = minion.Card.tokens[0].cardSprite.texture;
            bananaCrate.GetComponent<CardOnBoardScript>().card = minion.Card.tokens[0];
            BananaTweening(hero, bananaCrate);
        }
    }
    async void BananaTweening(HeroScript hero, GameObject bananaCrate)
    {
        bananaCrate.transform.DOScale(0.8f * Vector3.one, .5f);
        await Task.Delay(500);
        bananaCrate.transform.DOMove(hero.deck.transform.Find("Canvas").Find("DeckDrop").position, .5f);
        await Task.Delay(500);
        Destroy(bananaCrate);
    }

    public void ExcitedScientist(List<GameObject> target, MinionScript minion)
    {
        HeroScript whoseTurn = minion.transform.root.Find("Hero").transform.Find("Canvas").transform.Find("Hero").GetComponent<HeroScript>();
        IEnumerable<CardSO> cards =
            from _card in whoseTurn.hand
            where _card.GetComponent<CardScript>().cardDrawnThisTurn && _card != minion.Card
            select _card.GetComponent<CardScript>().card;
        for (int i = 0; i < cards.ToList().Count; i++)
        {
            if (whoseTurn.hand.Count < 10)
            {
                GameObject newCard = Instantiate(cardPrefab);
                newCard.transform.SetParent(whoseTurn.gameObject.transform.parent.parent.parent.Find("Hand").transform.Find("Canvas").transform.Find("HandUI").transform);
                newCard.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
                newCard.GetComponent<CardScript>().card = cards.ToList()[i];
                newCard.name = cards.ToList()[i].cardName;
                whoseTurn.hand.Add(newCard);
            }
        }
    }

    #endregion

    #region Shaman Cards 

    public void UnseenEvolve(List<GameObject> target, MinionScript minion)
    {
        if (minion.transform.root.name == "Player")
        {
            foreach (GameObject min in GameObject.FindGameObjectsWithTag("Player Minion"))
            {
                if (min != minion.gameObject) min.GetComponent<MinionScript>().EvolveMinion(1);
            }
        }
        else if (minion.transform.root.name == "Opponent")
        {
            foreach (GameObject min in GameObject.FindGameObjectsWithTag("Opponent Minion"))
            {
                if (min != minion.gameObject) min.GetComponent<MinionScript>().EvolveMinion(1);
            }
        }
    }

    public void RNJesus(List<GameObject> target, MinionScript minion)
    {
        GameObject _hero = minion.transform.root.name == "Player" ? GameObject.Find("Player").transform.Find("Hero").transform.Find("Canvas").transform.Find("Hero").gameObject : GameObject.Find("Opponent").transform.Find("Hero").transform.Find("Canvas").transform.Find("Hero").gameObject;
        HeroScript hero = _hero.GetComponent<HeroScript>();
        GameplayManager gm = gameplayManager;
        List<CardSO> _cards = new List<CardSO>();
        int destinedMana = (int)target[0].GetComponent<MinionScript>().Card.mana + 3;
        target[0].tag = "WillEvolve";
        foreach (CardSO _card in gm.manaCards[destinedMana].cards)
        {
            if (_card.cardType == CardType.MINION) _cards.Add(_card);
        }
        CardSO card1 = _cards[Random.Range(0, _cards.Count)];
        _cards.Remove(card1);
        CardSO card2 = _cards[Random.Range(0, _cards.Count)];
        _cards.Remove(card2);
        CardSO card3 = _cards[Random.Range(0, _cards.Count)];
        _cards.Remove(card3);
        hero.discovering = true;
        hero.discoverCards[0].GetComponent<DiscoverAttribute>().card = card1;
        hero.discoverCards[1].GetComponent<DiscoverAttribute>().card = card2;
        hero.discoverCards[2].GetComponent<DiscoverAttribute>().card = card3;
        hero.discoverFunction = "RNJesus_AfterDiscovering";
        hero.DiscoverCards();
    }

    public void RNJesus_AfterDiscovering(List<GameObject> target, GameObject minion)
    {
        print("hello?");
        MinionScript targeted = GameObject.FindGameObjectWithTag("WillEvolve").GetComponent<MinionScript>();
        print(targeted.gameObject.name);
        targeted.Card = target[0].GetComponent<DiscoverAttribute>().card; 
        print(targeted.Card.name);
        if (targeted.transform.root.name == "Player")
        {
            targeted.gameObject.tag = "Player Minion";
        }
        else if (targeted.transform.root.name == "Opponent")
        {
            targeted.gameObject.tag = "Opponent Minion";
        }
        targeted.UpdateStats();
    }
    #endregion

    #region Priest Cards 

    public async void DaybreakDeceptor(List<GameObject> target, MinionScript minion)
    {
        gameplayManager.DrawCard(minion.gameObject, minion.transform.root.name == "Player" ? Turn.PLAYER : Turn.OPPONENT);
        await Task.Delay(1300);
        if ((minion.transform.root.name == "Player" && GameObject.FindGameObjectsWithTag("Player Minion").Length == 1) || (minion.transform.root.name == "Opponent" && GameObject.FindGameObjectsWithTag("Opponent Minion").Length == 1)) 
            gameplayManager.DrawCard(minion.gameObject, minion.transform.root.name == "Player" ? Turn.PLAYER : Turn.OPPONENT);
    }

    public void FaithfulWellbeing(List<GameObject> target, MinionScript minion)
    {
        if ((minion.transform.root.name == "Player"))
        {
            foreach (GameObject min in GameObject.FindGameObjectsWithTag("Player Minion"))
            {
                if (min != minion.gameObject)
                {
                    min.GetComponent<MinionScript>().currentAttack++;
                    min.GetComponent<MinionScript>().currentHP++;
                }
            }
        }
        if ((minion.transform.root.name == "Opponent"))
        {
            foreach (GameObject min in GameObject.FindGameObjectsWithTag("Opponent Minion"))
            {
                if (min != minion.gameObject)
                {
                    min.GetComponent<MinionScript>().currentAttack++;
                    min.GetComponent<MinionScript>().currentHP++;
                }
            }
        }

    }

    public void HighPriestessIshanas(List<GameObject> target, MinionScript minion)
    {
        List<CardSO> cardSet = new List<CardSO>();
        foreach (CardSO card in minion.transform.root.Find("Deck").GetComponent<DeckScript>().cards)
        {
            if (card.mana <= 2 && card.cardType == CardType.MINION)
            {
                cardSet.Add(card);
            }
        }
        foreach (CardSO card in cardSet)
        {
            if (ChildCount() < 7)
            {
                GameObject newMinion = Instantiate(minionPrefab, minion.transform.root.name == "Player" ? GameObject.Find("Player").transform.Find("Battleground").transform.Find("Canvas").transform.Find("BattlegroundUI").transform : GameObject.Find("Opponent").transform.Find("Battleground").transform.Find("Canvas").transform.Find("BattlegroundUI").transform);
                newMinion.GetComponent<MinionScript>().Card = card;
                newMinion.name = card.name;
                newMinion.GetComponent<MinionScript>().UpdateStats();
                newMinion.tag = newMinion.transform.root.name == "Player" ? "Player Minion" : "Opponent Minion";
                newMinion.GetComponent<MinionScript>().numberOnBoard = (int)ChildCount()+1;
                if (newMinion.transform.root.name == "Player")
                {
                    foreach (GameObject _minion in GameObject.FindGameObjectsWithTag("Player Minion"))
                    {
                        if (newMinion.GetComponent<MinionScript>().numberOnBoard <= _minion.GetComponent<MinionScript>().numberOnBoard)
                        {
                            if (newMinion != _minion) _minion.GetComponent<MinionScript>().numberOnBoard++;
                        }
                        _minion.GetComponent<RectTransform>().DOAnchorPos(_minion.GetComponent<MinionScript>().BasePosition(), .5f);
                    }
                }
                else if (newMinion.transform.root.name == "Opponent")
                {
                    foreach (GameObject _minion in GameObject.FindGameObjectsWithTag("Opponent Minion"))
                    {
                        if (newMinion.GetComponent<MinionScript>().numberOnBoard <= _minion.GetComponent<MinionScript>().numberOnBoard)
                        {
                            if (newMinion != _minion) _minion.GetComponent<MinionScript>().numberOnBoard++;
                        }
                        _minion.GetComponent<RectTransform>().DOAnchorPos(_minion.GetComponent<MinionScript>().BasePosition(), .5f);
                    }
                }
                minion.transform.root.Find("Deck").GetComponent<DeckScript>().cards.Remove(card);
            }
        }
    }

    public void ShadowBringer_Battlecry(List<GameObject> target, MinionScript minion)
    {
        if (target.Count != 0)
        {
            foreach (GameObject _target in target)
            {
                if (_target.CompareTag("Hero"))
                {
                    float armordanSonraGidecekHealth = 2 > _target.GetComponent<HeroScript>().armor ? Mathf.Abs(2 - _target.GetComponent<HeroScript>().armor) : 0;
                    _target.GetComponent<HeroScript>().armor -= 2;
                    _target.GetComponent<HeroScript>().currentHP -= armordanSonraGidecekHealth;
                    if (_target.GetComponent<HeroScript>().currentHP <= 0)
                    {
                        _target.GetComponent<SpriteRenderer>().color = new Color32(118, 46, 46, 255);
                        print(_target.name + " oyunu kaybetti.");
                    }
                }
                else if (_target.CompareTag("Player Minion") || _target.CompareTag("Opponent Minion"))
                {
                    _target.GetComponent<MinionScript>().TakeDamage(2);
                }
            }
            minion.GetComponent<MinionScript>().hasDivineShield = false;
        }
        else
        {
            List<GameObject> pool = new List<GameObject>();
            foreach (GameObject pMinion in GameObject.FindGameObjectsWithTag("Player Minion")) pool.Add(pMinion);
            foreach (GameObject oMinion in GameObject.FindGameObjectsWithTag("Opponent Minion")) pool.Add(oMinion);
            pool.Add(GameObject.Find("Player").transform.Find("Hero").transform.Find("Canvas").transform.Find("Hero").gameObject);
            pool.Add(GameObject.Find("Opponent").transform.Find("Hero").transform.Find("Canvas").transform.Find("Hero").gameObject);
            GameObject _target = pool[Random.Range(0, pool.Count)];
            if (_target.CompareTag("Hero"))
            {
                float armordanSonraGidecekHealth = 2 > _target.GetComponent<HeroScript>().armor ? Mathf.Abs(2 - _target.GetComponent<HeroScript>().armor) : 0;
                _target.GetComponent<HeroScript>().armor -= 2;
                _target.GetComponent<HeroScript>().currentHP -= armordanSonraGidecekHealth;
                if (_target.GetComponent<HeroScript>().currentHP <= 0)
                {
                    _target.GetComponent<SpriteRenderer>().color = new Color32(118, 46, 46, 255);
                    print(_target.name + " oyunu kaybetti.");
                }
            }
            else if (_target.CompareTag("Player Minion") || _target.CompareTag("Opponent Minion"))
            {
                _target.GetComponent<MinionScript>().currentHP -= 2;
                if (_target.GetComponent<MinionScript>().currentHP <= 0)
                {
                    _target.GetComponent<RawImage>().color = new Color32(118, 46, 46, 255);

                    foreach (MinionAbilites _card in _target.GetComponent<MinionScript>().Card.minionAbilites)
                    {
                        if (_card.minionAbility == MinionAbility.Deathrattle)
                        {
                            System.Type scriptType = typeof(MinionCardEffects);
                            MethodInfo info = scriptType.GetMethod(_card.minionEffect);
                            info?.Invoke(GameObject.Find("GameplayManager").GetComponent<MinionCardEffects>(), new object[] { new List<GameObject> { gameObject }, this });
                        }
                    }
                    Destroy(_target, .7f);
                }
            }
        }
    }

    public void ShadowBringer_Deathrattle(List<GameObject> target, MinionScript minion)
    {
        HeroScript whoseTurn = minion.transform.root.Find("Hero").transform.Find("Canvas").transform.Find("Hero").GetComponent<HeroScript>();
        if (whoseTurn.hand.Count < 10)
        {
            GameObject newCard = Instantiate(cardPrefab);
            newCard.transform.SetParent(whoseTurn.gameObject.transform.parent.parent.parent.Find("Hand").transform.Find("Canvas").transform.Find("HandUI").transform);
            newCard.GetComponent<RectTransform>().localScale = new Vector3(1.35f, 1.35f, 1.35f);
            newCard.GetComponent<CardScript>().card = minion.Card;
            newCard.name = minion.Card.cardName;
            whoseTurn.hand.Add(newCard);
        }
    }

    public void LittleDemon(List<GameObject> target, MinionScript minion)
    {
        GameObject newMinion = Instantiate(minionPrefab, minion.transform.root.Find("Battleground").transform.Find("Canvas").transform.Find("BattlegroundUI").transform);
        newMinion.GetComponent<MinionScript>().Card = minion.Card.tokens[0];
        newMinion.GetComponent<MinionScript>().UpdateStats();
        newMinion.name = minion.Card.tokens[0].name;
        newMinion.tag = newMinion.transform.root.name == "Player" ? "Player Minion" : "Opponent Minion";
        newMinion.GetComponent<MinionScript>().numberOnBoard = newMinion.transform.root.name == "Player" ? ChildCount() : OpponentChildCount();
        if (newMinion.transform.root.name == "Player")
        {
            foreach (GameObject _minion in GameObject.FindGameObjectsWithTag("Player Minion"))
            {
                if (newMinion.GetComponent<MinionScript>().numberOnBoard <= _minion.GetComponent<MinionScript>().numberOnBoard)
                {
                    if (newMinion != _minion) _minion.GetComponent<MinionScript>().numberOnBoard++;
                }
                _minion.GetComponent<RectTransform>().DOAnchorPos(_minion.GetComponent<MinionScript>().BasePosition(), .5f);
            }
        }
        else if (newMinion.transform.root.name == "Opponent")
        {
            foreach (GameObject _minion in GameObject.FindGameObjectsWithTag("Opponent Minion"))
            {
                if (newMinion.GetComponent<MinionScript>().numberOnBoard <= _minion.GetComponent<MinionScript>().numberOnBoard)
                {
                    if (newMinion != _minion) _minion.GetComponent<MinionScript>().numberOnBoard++;
                }
                _minion.GetComponent<RectTransform>().DOAnchorPos(_minion.GetComponent<MinionScript>().BasePosition(), .5f);
            }
        }
    }

    public void Faceless(List<GameObject> target, MinionScript minion)
    {
        HeroScript whoseTurn = minion.transform.root.Find("Hero").transform.Find("Canvas").transform.Find("Hero").GetComponent<HeroScript>();
        whoseTurn.armor += 8;
    }

    public void Kanser(List<GameObject> target, MinionScript minion)
    {
        HeroScript whoseTurn = minion.transform.root.Find("Hero").transform.Find("Canvas").transform.Find("Hero").GetComponent<HeroScript>();
        if (whoseTurn.graveyard.Count == 0) return;

        IEnumerable<CardSO> summon =
            from _minion in whoseTurn.graveyard
            select _minion;
        CardSO summonThis = summon.ToList()[Random.Range(0, summon.ToList().Count)];

        GameObject newMinion = Instantiate(minionPrefab, minion.transform.root.name == "Player" ? GameObject.Find("Player").transform.Find("Battleground").transform.Find("Canvas").transform.Find("BattlegroundUI").transform : GameObject.Find("Opponent").transform.Find("Battleground").transform.Find("Canvas").transform.Find("BattlegroundUI").transform);
        newMinion.GetComponent<MinionScript>().Card = summonThis;
        newMinion.GetComponent<MinionScript>().UpdateStats();
        newMinion.name = summonThis.name;
        newMinion.tag = newMinion.transform.root.name == "Player" ? "Player Minion" : "Opponent Minion";
        newMinion.GetComponent<MinionScript>().numberOnBoard = newMinion.transform.root.name == "Player" ? ChildCount() : OpponentChildCount();
        newMinion.GetComponent<MinionScript>().hasDivineShield = true;

        IEnumerable<GameObject> minions =
            from _minion in GameObject.FindGameObjectsWithTag($"{(newMinion.transform.root.name == "Player" ? "Player" : "Opponent")} Minion")
            select _minion;

        foreach (var v in minions)
        {
            if (newMinion.GetComponent<MinionScript>().numberOnBoard <= v.GetComponent<MinionScript>().numberOnBoard && newMinion != v)
                v.GetComponent<MinionScript>().numberOnBoard++;
            v.GetComponent<RectTransform>().DOAnchorPos(v.GetComponent<MinionScript>().BasePosition(), .5f);
        }
    }

    public void ShadowcuAbla_Battlecry(List<GameObject> target, MinionScript minion)
    {
        HeroScript whoseTurn = minion.transform.root.Find("Hero").transform.Find("Canvas").transform.Find("Hero").GetComponent<HeroScript>();
        CurrentEffects eff;
        eff.effectName = "ShadowcuAbla";
        if (ExistingEffect("ShadowcuAbla", minion.gameObject).effectName == eff.effectName)
        {
            CurrentEffects val = ExistingEffect("ShadowcuAbla", minion.gameObject);
            eff.howManyRounds = 999;
            eff.howManyStacked = val.howManyStacked + 1;
            minion.transform.root.Find("Hero").Find("Canvas").Find("Hero").GetComponent<HeroScript>()._currentEffects.Add(eff);
        }
        else
        {
            eff.howManyRounds = 999;
            eff.howManyStacked = 1;
            whoseTurn._currentEffects.Add(eff);
        }
    }

    public void ShadowcuAbla_Deathrattle(List<GameObject> target, MinionScript minion)
    {
        HeroScript whoseTurn = minion.transform.root.Find("Hero").transform.Find("Canvas").transform.Find("Hero").GetComponent<HeroScript>();
        whoseTurn._currentEffects.Remove(ExistingEffect("ShadowcuAbla", minion.gameObject));
    }
    #endregion

    #region Warrior Cards
    public void BumperTrumper(List<GameObject> target, MinionScript minion)
    {
        if (ChildCount() < 7)
        {
            GameObject newMinion = Instantiate(minionPrefab, minion.transform.root.name == "Player" ? GameObject.Find("Player").transform.Find("Battleground").transform.Find("Canvas").transform.Find("BattlegroundUI").transform : GameObject.Find("Opponent").transform.Find("Battleground").transform.Find("Canvas").transform.Find("BattlegroundUI").transform);
            newMinion.GetComponent<MinionScript>().Card = minion.Card.tokens[0];
            newMinion.GetComponent<MinionScript>().UpdateStats();
            newMinion.tag = newMinion.transform.root.name == "Player" ? "Player Minion" : "Opponent Minion";
            newMinion.GetComponent<MinionScript>().numberOnBoard = minion.numberOnBoard + 1;
            if (newMinion.transform.root.name == "Player")
            {
                foreach (GameObject _minion in GameObject.FindGameObjectsWithTag("Player Minion"))
                {
                    if (newMinion.GetComponent<MinionScript>().numberOnBoard <= _minion.GetComponent<MinionScript>().numberOnBoard)
                    {
                        if (newMinion != _minion) _minion.GetComponent<MinionScript>().numberOnBoard++;
                    }
                    _minion.GetComponent<RectTransform>().DOAnchorPos(_minion.GetComponent<MinionScript>().BasePosition(), .5f);
                }
            }
            else if (newMinion.transform.root.name == "Opponent")
            {
                foreach (GameObject _minion in GameObject.FindGameObjectsWithTag("Opponent Minion"))
                {
                    if (newMinion.GetComponent<MinionScript>().numberOnBoard <= _minion.GetComponent<MinionScript>().numberOnBoard)
                    {
                        if (newMinion != _minion) _minion.GetComponent<MinionScript>().numberOnBoard++;
                    }
                    _minion.GetComponent<RectTransform>().DOAnchorPos(_minion.GetComponent<MinionScript>().BasePosition(), .5f);
                }
            }
        }
        if (ChildCount() < 7)
        {
            GameObject newMinion = Instantiate(minionPrefab, minion.transform.root.name == "Player" ? GameObject.Find("Player").transform.Find("Battleground").transform.Find("Canvas").transform.Find("BattlegroundUI").transform : GameObject.Find("Opponent").transform.Find("Battleground").transform.Find("Canvas").transform.Find("BattlegroundUI").transform);
            newMinion.GetComponent<MinionScript>().Card = minion.Card.tokens[0];
            newMinion.GetComponent<MinionScript>().UpdateStats();
            newMinion.tag = newMinion.transform.root.name == "Player" ? "Player Minion" : "Opponent Minion";
            newMinion.GetComponent<MinionScript>().numberOnBoard = minion.numberOnBoard + 2;
            if (newMinion.transform.root.name == "Player")
            {
                foreach (GameObject _minion in GameObject.FindGameObjectsWithTag("Player Minion"))
                {
                    if (newMinion.GetComponent<MinionScript>().numberOnBoard <= _minion.GetComponent<MinionScript>().numberOnBoard)
                    {
                        if (newMinion != _minion) _minion.GetComponent<MinionScript>().numberOnBoard++;
                    }
                    _minion.GetComponent<RectTransform>().DOAnchorPos(_minion.GetComponent<MinionScript>().BasePosition(), .5f);
                }
            }
            else if (newMinion.transform.root.name == "Opponent")
            {
                foreach (GameObject _minion in GameObject.FindGameObjectsWithTag("Opponent Minion"))
                {
                    if (newMinion.GetComponent<MinionScript>().numberOnBoard <= _minion.GetComponent<MinionScript>().numberOnBoard)
                    {
                        if (newMinion != _minion) _minion.GetComponent<MinionScript>().numberOnBoard++;
                    }
                    _minion.GetComponent<RectTransform>().DOAnchorPos(_minion.GetComponent<MinionScript>().BasePosition(), .5f);
                }
            }
        }
    }

    public void CityCrier(List<GameObject> target, MinionScript minion)
    {
        DeckScript deck = GameObject.Find(minion.transform.root.name).transform.Find("Deck").GetComponent<DeckScript>();

        IEnumerable<CardSO> chosenCardList =
            from _card in deck.cards
            where _card.minionAbilites.Any(a => a.minionAbility == MinionAbility.Rush)
            select _card;

        if (chosenCardList.ToList().Count == 0) return;

        CardSO chosenCard = chosenCardList.ToList()[Random.Range(0, chosenCardList.ToList().Count)];
        gameplayManager.DrawCard(minion.gameObject, minion.transform.root.name == "Player" ? Turn.PLAYER : Turn.OPPONENT, chosenCard);
    }

    public void Hungrief(List<GameObject> target, MinionScript minion)
    {
        HeroScript whoseTurn = minion.transform.root.Find("Hero").transform.Find("Canvas").transform.Find("Hero").GetComponent<HeroScript>();
        whoseTurn.armor += 3; 
    }

    public void VermillionAlbillion(List<GameObject> target, MinionScript minion)
    {
        HeroScript whoseTurn = minion.transform.root.Find("Hero").transform.Find("Canvas").transform.Find("Hero").GetComponent<HeroScript>();
        if (whoseTurn.hand.Count < 10)
        {
            GameObject newCard = Instantiate(cardPrefab);
            newCard.transform.SetParent(whoseTurn.gameObject.transform.parent.parent.parent.Find("Hand").transform.Find("Canvas").transform.Find("HandUI").transform);
            newCard.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            newCard.GetComponent<CardScript>().card = minion.Card.tokens[0];
            newCard.name = minion.Card.cardName;
            whoseTurn.hand.Add(newCard);
        }
    }

    public void KronxMurlochoof(List<GameObject> target, MinionScript minion)
    {
        HeroScript hero = minion.transform.root.Find("Hero").transform.Find("Canvas").transform.Find("Hero").GetComponent<HeroScript>();

        foreach (GameObject min in (minion.transform.root.name == "Player" ? GameObject.FindGameObjectsWithTag("Opponent Minion") : GameObject.FindGameObjectsWithTag("Player Minion")))
        {
            min.GetComponent<MinionScript>().currentHP -= hero.armor;
            min.GetComponent<MinionScript>().damageTakenGameObject.SetActive(true);
            min.GetComponent<MinionScript>().damageTakenTime = 1f;
            min.GetComponent<MinionScript>().damageTakenValue = hero.armor;
            if (min.GetComponent<MinionScript>().currentHP <= 0)
            {
                min.GetComponent<RawImage>().color = new Color32(118, 46, 46, 255);
                Destroy(min, .7f);
            }
        }
    }

    public void Rattlebaran(List<GameObject> target, MinionScript minion)
    {
        string mName = minion.gameObject.name;
        var match = new System.Text.RegularExpressions.Regex("_(?<number>[0-9]+)").Match(mName);
        print(match);
        if (match.Success)
        {
            print("yes");
            var value = match.Groups["number"].Value;
            print(value);
            GameObject newMinion = Instantiate(minionPrefab, minion.transform.root.name == "Player" ? GameObject.Find("Player").transform.Find("Battleground").transform.Find("Canvas").transform.Find("BattlegroundUI").transform : GameObject.Find("Opponent").transform.Find("Battleground").transform.Find("Canvas").transform.Find("BattlegroundUI").transform);
            newMinion.GetComponent<MinionScript>().Card = minion.Card.tokens[0];
            newMinion.tag = newMinion.transform.root.name == "Player" ? "Player Minion" : "Opponent Minion";
            newMinion.GetComponent<MinionScript>().UpdateStats();
            newMinion.name = "Rattlebaran_" + (int.Parse(value) < 14 ? (int.Parse(value) + 1) : 14);
            newMinion.GetComponent<MinionScript>().currentAttack = (int.Parse(value) < 14 ? (int.Parse(value) + 1) : 14);
            newMinion.GetComponent<MinionScript>().currentHP = (int.Parse(value) < 14 ? (int.Parse(value) + 1) : 14);
            newMinion.GetComponent<MinionScript>().numberOnBoard = newMinion.transform.root.name == "Player" ? ChildCount()+1 : OpponentChildCount()+1;
            if (newMinion.transform.root.name == "Player")
            {
                foreach (GameObject _minion in GameObject.FindGameObjectsWithTag("Player Minion"))
                {
                    if (newMinion.GetComponent<MinionScript>().numberOnBoard <= _minion.GetComponent<MinionScript>().numberOnBoard)
                    {
                        if (newMinion != _minion) _minion.GetComponent<MinionScript>().numberOnBoard++;
                    }
                    _minion.GetComponent<RectTransform>().DOAnchorPos(_minion.GetComponent<MinionScript>().BasePosition(), .5f);
                }
            }
            else if (newMinion.transform.root.name == "Opponent")
            {
                foreach (GameObject _minion in GameObject.FindGameObjectsWithTag("Opponent Minion"))
                {
                    if (newMinion.GetComponent<MinionScript>().numberOnBoard <= _minion.GetComponent<MinionScript>().numberOnBoard)
                    {
                        if (newMinion != _minion) _minion.GetComponent<MinionScript>().numberOnBoard++;
                    }
                    _minion.GetComponent<RectTransform>().DOAnchorPos(_minion.GetComponent<MinionScript>().BasePosition(), .5f);
                }
            }
        }
        else
        {
            GameObject newMinion = Instantiate(minionPrefab, minion.transform.root.name == "Player" ? GameObject.Find("Player").transform.Find("Battleground").transform.Find("Canvas").transform.Find("BattlegroundUI").transform : GameObject.Find("Opponent").transform.Find("Battleground").transform.Find("Canvas").transform.Find("BattlegroundUI").transform);
            newMinion.GetComponent<MinionScript>().Card = minion.Card.tokens[0];
            newMinion.tag = newMinion.transform.root.name == "Player" ? "Player Minion" : "Opponent Minion";
            newMinion.GetComponent<MinionScript>().UpdateStats();
            newMinion.name = "Rattlebaran_3";
            newMinion.GetComponent<MinionScript>().currentAttack = 3;
            newMinion.GetComponent<MinionScript>().currentHP = 3;
            newMinion.GetComponent<MinionScript>().numberOnBoard = newMinion.transform.root.name == "Player" ? ChildCount()+1 : OpponentChildCount()+1;
            if (newMinion.transform.root.name == "Player")
            {
                foreach (GameObject _minion in GameObject.FindGameObjectsWithTag("Player Minion"))
                {
                    if (newMinion.GetComponent<MinionScript>().numberOnBoard <= _minion.GetComponent<MinionScript>().numberOnBoard)
                    {
                        if (newMinion != _minion) _minion.GetComponent<MinionScript>().numberOnBoard++;
                    }
                    _minion.GetComponent<RectTransform>().DOAnchorPos(_minion.GetComponent<MinionScript>().BasePosition(), .5f);
                }
            }
            else if (newMinion.transform.root.name == "Opponent")
            {
                foreach (GameObject _minion in GameObject.FindGameObjectsWithTag("Opponent Minion"))
                {
                    if (newMinion.GetComponent<MinionScript>().numberOnBoard <= _minion.GetComponent<MinionScript>().numberOnBoard)
                    {
                        if (newMinion != _minion) _minion.GetComponent<MinionScript>().numberOnBoard++;
                    }
                    _minion.GetComponent<RectTransform>().DOAnchorPos(_minion.GetComponent<MinionScript>().BasePosition(), .5f);
                }
            }
        }
    }

    public void ImpatientPunk(List<GameObject> target, MinionScript minion)
    {
        HeroScript hero = minion.transform.root.Find("Hero").transform.Find("Canvas").transform.Find("Hero").GetComponent<HeroScript>();

        hero.armor += minion.currentAttack;
    }

    public void ExcitedRookie_Battlecry(List<GameObject> target, MinionScript minion)
    {
        HeroScript whoseTurn = minion.transform.root.Find("Hero").transform.Find("Canvas").transform.Find("Hero").GetComponent<HeroScript>();
        CurrentEffects eff;
        eff.effectName = "ExcitedRookie";
        if (ExistingEffect("ExcitedRookie", minion.gameObject).effectName == eff.effectName)
        {
            CurrentEffects val = ExistingEffect("ExcitedRookie", minion.gameObject);
            eff.howManyRounds = 999;
            eff.howManyStacked = val.howManyStacked + 1;
            minion.transform.root.Find("Hero").Find("Canvas").Find("Hero").GetComponent<HeroScript>()._currentEffects.Add(eff);
        }
        else
        {
            eff.howManyRounds = 999;
            eff.howManyStacked = 1;
            whoseTurn._currentEffects.Add(eff);
        }
    }

    public void ExcitedRookie_Deathrattle(List<GameObject> target, MinionScript minion)
    {
        HeroScript whoseTurn = minion.transform.root.Find("Hero").transform.Find("Canvas").transform.Find("Hero").GetComponent<HeroScript>();
        whoseTurn._currentEffects.Remove(ExistingEffect("ExcitedRookie", minion.gameObject));
    }

    public void DeathwingSadAspect(List<GameObject> target, MinionScript minion)
    {
        HeroScript whoseTurn = minion.transform.root.Find("Hero").transform.Find("Canvas").transform.Find("Hero").GetComponent<HeroScript>();
        Transform opponentHero = GameObject.Find(minion.transform.root.name == "Player" ? "Opponent" : "Player").transform.root.Find("Battleground").transform.Find("Canvas").Find("BattlegroundUI");
        IEnumerable<MinionScript> oppMinions =
            from _minions in opponentHero.Cast<Transform>()
            orderby _minions.GetComponent<MinionScript>().numberOnBoard ascending
            select _minions.GetComponent<MinionScript>();
        
        _DeathwingSadAspect(minion, oppMinions.ToList());
    }

    public async void _DeathwingSadAspect(MinionScript mini, List<MinionScript> _opp)
    {
        foreach (var opp in _opp)
        {
            if (!opp.gameObject) return;

            await opp.MinionHitTweening(mini);
            await Task.Delay(400);
            if (mini.currentHP > 0)
            {
                mini.currentAttack += 2;
                mini.currentHP += 2;
                await Task.Delay(250);
            }
            else break;
            await Task.Delay(200);
        }
    }

    public async void NadeMerchant(List<GameObject> target, MinionScript minion)
    {
        HeroScript hero = minion.transform.root.Find("Hero").transform.Find("Canvas").transform.Find("Hero").GetComponent<HeroScript>();
        HeroScript oppHero = hero.transform.root.name == "Player"
                                                         ? GameObject.Find("Opponent").transform.Find("Hero").transform.Find("Canvas").transform.Find("Hero").GetComponent<HeroScript>()
                                                         : GameObject.Find("Player").transform.Find("Hero").transform.Find("Canvas").transform.Find("Hero").GetComponent<HeroScript>();
        if (hero.currentMana == 0) return;

        GlobalCardEffects.ShuffleToDeck(hero, oppHero.deck, minion.Card.tokens[0], -1, 1);

        GameObject bombCard = Instantiate(cardOnBoardPrefab, minion.transform.position, Quaternion.identity, minion.transform);
        bombCard.transform.localScale = 0.5f * Vector3.one;
        bombCard.GetComponent<RawImage>().texture = minion.Card.tokens[0].cardSprite.texture;
        bombCard.GetComponent<CardOnBoardScript>().card = minion.Card.tokens[0];
        bombCard.transform.DOScale(0.8f * Vector3.one, .5f);
        await Task.Delay(500);
        bombCard.transform.DOMove(oppHero.deck.transform.Find("Canvas").Find("DeckDrop").position, .5f);
        await Task.Delay(500);
        Destroy(bombCard);
    }

    public async void BombaciMulayim(List<GameObject> target, MinionScript minion)
    {
        HeroScript hero = minion.transform.root.Find("Hero").transform.Find("Canvas").transform.Find("Hero").GetComponent<HeroScript>();
        HeroScript oppHero = hero.transform.root.name == "Player"
                                                         ? GameObject.Find("Opponent").transform.Find("Hero").transform.Find("Canvas").transform.Find("Hero").GetComponent<HeroScript>()
                                                         : GameObject.Find("Player").transform.Find("Hero").transform.Find("Canvas").transform.Find("Hero").GetComponent<HeroScript>();

        var bombs = oppHero.deck.cards.Where(a => a.name == "Bomb").Take((int)Mathf.Round(oppHero.deck.cards.Count(b => b.name == "Bomb"))).ToList();

        for (int i = 0; i < bombs.Count; i++)
        {
            oppHero.deck.cards.Remove(bombs[i]);
            oppHero.TakeDamage(5);
            await Task.Delay(500);
        }
    }

    public void CityDweller(List<GameObject> target, MinionScript minion)
    {
        HeroScript hero = minion.transform.root.Find("Hero").transform.Find("Canvas").transform.Find("Hero").GetComponent<HeroScript>();
        CurrentEffects eff;
        eff.effectName = "CityDweller";
        if (ExistingEffect("CityDweller", minion.gameObject).effectName == eff.effectName)
        {
            CurrentEffects val = ExistingEffect("CityDweller", minion.gameObject);
            eff.howManyRounds = 1;
            eff.howManyStacked = val.howManyStacked + 1;
            hero._currentEffects.Add(eff);
        }
        else
        {
            eff.howManyRounds = 1;
            eff.howManyStacked = 1;
            hero._currentEffects.Add(eff);
        }
    }
    #endregion

    #region Demon Hunter Cards 
    public void Aranna(List<GameObject> target, MinionScript minion)
    {
        HeroScript hero = minion.transform.root.Find("Hero").transform.Find("Canvas").transform.Find("Hero").GetComponent<HeroScript>();
        foreach (Transform min in minion.transform.root.Find("Battleground").Find("Canvas").Find("BattlegroundUI").transform)
        {
            if (min.gameObject != minion.gameObject)
            {
                min.gameObject.GetComponent<MinionScript>().currentAttack += 1;
                min.gameObject.GetComponent<MinionScript>().currentHP += 1;
            }
        }
    }
    #endregion

    #region Neutral Cards
    public void IvusTheFrostLord(List<GameObject> target, MinionScript minion)
    {
        HeroScript hero = minion.transform.root.Find("Hero").transform.Find("Canvas").transform.Find("Hero").GetComponent<HeroScript>();

        List<string> Specializer = new List<string>() { "+2/+2", "Divine Shield", "Taunt", "Rush" };

        int manaLeft = (int)hero.currentMana; 
        print("spent " + manaLeft + " mana.");
        for (int i= 0; i<manaLeft; i++)
        {
            string nextSpecializer = Specializer[Random.Range(0, Specializer.Count)];
            if (nextSpecializer != "+2/+2")
            {
                switch(nextSpecializer)
                {
                    case "Divine Shield": minion.hasDivineShield = true; print("gained divine shield"); break;
                    case "Taunt": minion.hasTaunt = true; print("gained taunt"); break;
                    case "Rush": minion.canAttack = true; print("gained rush"); break;
                }
                Specializer.Remove(nextSpecializer);
            } else
            {
                minion.currentAttack += 2;
                minion.currentHP += 2; 
                print("gained +2/+2");
            }
        }
        hero.currentMana = 0;
    }

    public void KorrakReis(List<GameObject> target, MinionScript minion)
    {
        if (minion.currentHP == 0) return;

        GameObject newMinion = Instantiate(minionPrefab, minion.transform.root.name == "Player" ? GameObject.Find("Player").transform.Find("Battleground").transform.Find("Canvas").transform.Find("BattlegroundUI").transform : GameObject.Find("Opponent").transform.Find("Battleground").transform.Find("Canvas").transform.Find("BattlegroundUI").transform);
        newMinion.GetComponent<MinionScript>().Card = minion.Card;
        newMinion.GetComponent<MinionScript>().UpdateStats();
        newMinion.name = minion.Card.name;
        newMinion.tag = newMinion.transform.root.name == "Player" ? "Player Minion" : "Opponent Minion";
        newMinion.GetComponent<MinionScript>().numberOnBoard = newMinion.transform.root.name == "Player" ? minion.numberOnBoard : minion.numberOnBoard;

        IEnumerable<GameObject> minions =
            from _minion in GameObject.FindGameObjectsWithTag($"{(newMinion.transform.root.name == "Player" ? "Player" : "Opponent")} Minion")
            select _minion;
        foreach (var v in minions)
        {
            if (newMinion.GetComponent<MinionScript>().numberOnBoard <= v.GetComponent<MinionScript>().numberOnBoard && newMinion != v)
                v.GetComponent<MinionScript>().numberOnBoard++;
            v.GetComponent<RectTransform>().DOAnchorPos(v.GetComponent<MinionScript>().BasePosition(), .5f);
        }
    }

    public void LorepublisherPolkelt(List<GameObject> target, MinionScript minion)
    {
        HeroScript hero = minion.transform.root.Find("Hero").transform.Find("Canvas").transform.Find("Hero").GetComponent<HeroScript>();
        hero.deck.cards = hero.deck.cards.OrderBy(a => a.mana).ThenBy(a => a.cardName).ToList();
    }

    public async void HypedFarmer(List<GameObject> target, MinionScript minion)
    {
        gameplayManager.DrawCard(minion.gameObject, minion.transform.root.name == "Player" ? Turn.PLAYER : Turn.OPPONENT);
        await Task.Delay(1000);
        if (!minion) return;
        gameplayManager.DrawCard(minion.gameObject, minion.transform.root.name == "Player" ? Turn.PLAYER : Turn.OPPONENT);
    }

    public async void Yoggerz(List<GameObject> target, MinionScript minion)
    {
        HeroScript hero = minion.transform.root.Find("Hero").transform.Find("Canvas").transform.Find("Hero").GetComponent<HeroScript>();

        for (int i = 0; i < 3; i++)
        {
            if (ChildCount() >= 7) return;

            List<CardSO> _minions = gameplayManager.allPlayableCards.Where(a => a.cardType == CardType.MINION).ToList();
            CardSO randomCard = _minions[Random.Range(0, _minions.Count)];
            GameObject newMinion = Instantiate(minionPrefab, minion.transform.root.name == "Player" ? GameObject.Find("Player").transform.Find("Battleground").transform.Find("Canvas").transform.Find("BattlegroundUI").transform : GameObject.Find("Opponent").transform.Find("Battleground").transform.Find("Canvas").transform.Find("BattlegroundUI").transform);
            newMinion.GetComponent<MinionScript>().Card = randomCard;
            newMinion.name = randomCard.cardName;
            newMinion.tag = newMinion.transform.root.name == "Player" ? "Player Minion" : "Opponent Minion";
            newMinion.GetComponent<MinionScript>().numberOnBoard = minion.numberOnBoard;
            newMinion.GetComponent<MinionScript>().UpdateStats();
            newMinion.GetComponent<MinionScript>().currentAttack = randomCard.attack;
            newMinion.GetComponent<MinionScript>().currentHP = randomCard.hp;
            newMinion.GetComponent<MinionScript>().defaultAttack = randomCard.attack;
            newMinion.GetComponent<MinionScript>().defaultHP = randomCard.hp;

            foreach (GameObject _minion in GameObject.FindGameObjectsWithTag($"{newMinion.transform.root.name} Minion"))
            {
                if (newMinion.GetComponent<MinionScript>().numberOnBoard <= _minion.GetComponent<MinionScript>().numberOnBoard && newMinion != _minion)
                {
                    _minion.GetComponent<MinionScript>().numberOnBoard++;
                }
                _minion.GetComponent<RectTransform>().DOAnchorPos(_minion.GetComponent<MinionScript>().BasePosition(), .5f);
            }

            await Task.Delay(250);
        }
    }
    #endregion

}