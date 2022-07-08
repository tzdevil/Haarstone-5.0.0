using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Reflection;
using UnityEngine.UI;

public class HeroPowerScript : MonoBehaviour
{
    public HeroPowerSO heroPower;
    public float manaCost;
    public bool usedThisTurn;
    public GameObject statsPanel;

    public Sprite normal, exhausted;

    private void Start()
    {
        manaCost = heroPower.mana;
    }

    private void Update()
    {
        transform.Find("Mana").GetComponent<TextMeshProUGUI>().text = !usedThisTurn ? heroPower.mana.ToString() : "";
        if (statsPanel.activeInHierarchy) statsPanel.transform.Find("Stats").GetComponent<TextMeshProUGUI>().text = "<color=#fade55>Mana:</color> " + heroPower.mana + "\n" + heroPower.description;
        transform.Find("HeroPower").GetComponent<RawImage>().texture = !usedThisTurn ? normal.texture : exhausted.texture;
    }

    public void UseHeroPower()
    {
        if (!usedThisTurn)
        {
            if ((GameplayManager.turn == Turn.PLAYER && transform.parent.parent.parent.name == "Player") || (GameplayManager.turn == Turn.OPPONENT && transform.parent.parent.parent.name == "Opponent"))
            {
                if (transform.parent.parent.parent.Find("Hero").transform.Find("Canvas").transform.Find("Hero").GetComponent<HeroScript>().currentMana >= manaCost)
                {
                    transform.parent.parent.parent.Find("Hero").transform.Find("Canvas").transform.Find("Hero").GetComponent<HeroScript>().currentMana -= manaCost;
                    Type scriptType = typeof(HeroPowerEffects);
                    MethodInfo info = scriptType.GetMethod(heroPower.heroPowerEffect);
                    info?.Invoke(GameObject.Find("GameplayManager").GetComponent<HeroPowerEffects>(), new object[] { new List<GameObject> { transform.parent.parent.parent.Find("Hero").transform.Find("Canvas").transform.Find("Hero").gameObject } });
                    usedThisTurn = true;
                }
            }
        }
    }

    public void HoldHeroPower()
    {
        statsPanel.SetActive(true);
    }
    
    public void StopHoldingHeroPower()
    {
        statsPanel.SetActive(false);
    }
}
