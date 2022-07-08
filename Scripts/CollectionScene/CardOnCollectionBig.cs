using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardOnCollectionBig : MonoBehaviour
{
    public CardSO card;
    public float mana;
    // Minion ise
    public float attack;
    public float hp;
    public float maxHP;

    private CollectionManager _cm;

    [SerializeField] private Sprite _awakenKeyword, _formKeyword, _markKeyword, _transformKeyword;
    [SerializeField] private Image _keywordPanel;

    [SerializeField] private GameObject _designerNotes;
    [SerializeField] private TextMeshProUGUI _notes;

    private void Awake()
    {
        _cm = GameObject.Find("CollectionManager").GetComponent<CollectionManager>();
    }

    private void Update()
    {
        // Original Card
        transform.Find("OriginalCard").Find("CardImage").GetComponent<RawImage>().texture = card.cardSprite.texture;
        transform.Find("OriginalCard").Find("Mana").GetComponent<TextMeshProUGUI>().text = card.mana.ToString();
        transform.Find("OriginalCard").Find("Attack").gameObject.SetActive(card.cardType == CardType.MINION || card.cardType == CardType.WEAPON);
        transform.Find("OriginalCard").Find("Health").gameObject.SetActive(card.cardType == CardType.MINION || card.cardType == CardType.WEAPON);
        transform.Find("OriginalCard").Find("Mana").GetComponent<TextMeshProUGUI>().GetComponent<RectTransform>().anchoredPosition = card.legendary ? new Vector3(-48.5f, 86.7f, 0) : new Vector3(-47.9f, 94.5f, 0);

        transform.Find("OriginalCard").Find("Attack").GetComponent<TextMeshProUGUI>().text = card.cardType == CardType.MINION ? card.attack.ToString() : card.weaponAttack.ToString();
        transform.Find("OriginalCard").Find("Health").GetComponent<TextMeshProUGUI>().text = card.cardType == CardType.MINION ? card.hp.ToString() : card.weaponDurability.ToString();

        // Token Card
        transform.Find("TokenCard").gameObject.SetActive(card.tokens.Count != 0);
        if (card.tokens.Count != 0)
        {
            transform.Find("TokenCard").Find("CardImage").GetComponent<RawImage>().texture = card.corruptedVersion ? card.corruptedVersion.cardSprite.texture : card.tokens[0].cardSprite.texture;
            transform.Find("TokenCard").Find("TokenText").GetComponent<TextMeshProUGUI>().text = card.corruptedVersion ? "Corrupted" : "Token";
            transform.Find("TokenCard").Find("Mana").GetComponent<TextMeshProUGUI>().text = card.tokens[0].mana.ToString();
            transform.Find("TokenCard").Find("Attack").gameObject.SetActive(card.tokens[0].cardType == CardType.MINION);
            transform.Find("TokenCard").Find("Health").gameObject.SetActive(card.tokens[0].cardType == CardType.MINION);
            transform.Find("TokenCard").Find("Mana").GetComponent<TextMeshProUGUI>().GetComponent<RectTransform>().anchoredPosition = card.tokens[0].legendary ? new Vector3(-53.7f, 81, 0) : new Vector3(-53.7f, 90, 0);

            if (card.tokens[0].cardType != CardType.MINION) return;

            transform.Find("TokenCard").Find("Attack").GetComponent<TextMeshProUGUI>().text = card.tokens[0].attack.ToString();
            transform.Find("TokenCard").Find("Health").GetComponent<TextMeshProUGUI>().text = card.tokens[0].hp.ToString();
        }

    }

    public void CloseBigCard()
    {
        _cm.cardOnCollectionBigPrefab.SetActive(false);
    }

    public void OnEnable() => KeywordRelated();

    public void KeywordRelated()
    {
        _designerNotes.SetActive(card.DesignerNotes != string.Empty);
        _notes.text = card.DesignerNotes;

        _keywordPanel.gameObject.SetActive(card.minionAbilites
                                               .Any(c => c.minionAbility == MinionAbility.Awaken
                                                    || c.minionAbility == MinionAbility.Form
                                                    || c.minionAbility == MinionAbility.Mark
                                                    || c.minionAbility == MinionAbility.Transform));

        foreach(MinionAbilites ability in card.minionAbilites)
        {
            _keywordPanel.sprite = ability.minionAbility switch
            {
                MinionAbility.Awaken => _awakenKeyword,
                MinionAbility.Form => _formKeyword,
                MinionAbility.Mark => _markKeyword,
                MinionAbility.Transform => _transformKeyword,
                _ => null
            };
        }
    }
}
