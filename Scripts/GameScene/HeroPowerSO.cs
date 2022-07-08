using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HeroPowerTarget { SELF, OPPONENT, FRIENDLYMINION, FRIENDLYALL, OPPONENTMINION, OPPONENTALL, ALL}

[CreateAssetMenu(fileName = "New Hero Power", menuName = "New Hero Power", order = 52)]
public class HeroPowerSO : ScriptableObject
{
    public string heroPowerName;
    public string description;
    public float mana;
    public string heroPowerEffect;
    public HeroPowerTarget heroPowerTarget;
}
