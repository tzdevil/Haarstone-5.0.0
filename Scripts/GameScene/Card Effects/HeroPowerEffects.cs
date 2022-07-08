using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroPowerEffects : MonoBehaviour
{
    public void HealSelfForTwo(List<GameObject> target)
    {
        // target hero oldu�u i�in [if target.comparetag hero] yapm�yorum
        foreach (GameObject _target in target)
        {
            _target.GetComponent<HeroScript>().currentHP = _target.GetComponent<HeroScript>().currentHP + 2 > _target.GetComponent<HeroScript>().maxHP ? _target.GetComponent<HeroScript>().maxHP : _target.GetComponent<HeroScript>().currentHP + 2;
        }
    }

    public void GetTwoArmor(List<GameObject> target)
    {
        // target hero oldu�u i�in [if target.comparetag hero] yapm�yorum
        foreach (GameObject _target in target)
        {
            _target.GetComponent<HeroScript>().armor += 2;
        }
    }
    // minion targetlar�nda List<GameObject> yapabilirim, oradaki herkese i�ler. bufflar da damagelar da �al���r hehe
}
