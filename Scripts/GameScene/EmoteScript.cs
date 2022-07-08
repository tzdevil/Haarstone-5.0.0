using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EmoteScript : MonoBehaviour
{
    public void UseEmote()
    {
        switch (name)
        {
            case "Greetings":
                transform.parent.parent.Find("Hero").GetComponent<HeroScript>().usedEmote.GetComponent<RawImage>().texture = transform.parent.parent.Find("Hero").GetComponent<HeroScript>().emotes[0].texture;
                transform.parent.parent.Find("Hero").GetComponent<HeroScript>().emotePanel.SetActive(false);
                transform.parent.parent.Find("Hero").GetComponent<HeroScript>().emoteUsed = true;
                transform.parent.parent.Find("Hero").GetComponent<HeroScript>().emoteUsedTime = 2f;
                break;
            case "Well Played":
                transform.parent.parent.Find("Hero").GetComponent<HeroScript>().usedEmote.GetComponent<RawImage>().texture = transform.parent.parent.Find("Hero").GetComponent<HeroScript>().emotes[1].texture;
                transform.parent.parent.Find("Hero").GetComponent<HeroScript>().emotePanel.SetActive(false);
                transform.parent.parent.Find("Hero").GetComponent<HeroScript>().emoteUsed = true;
                transform.parent.parent.Find("Hero").GetComponent<HeroScript>().emoteUsedTime = 2f;
                break;
            case "Thanks":
                transform.parent.parent.Find("Hero").GetComponent<HeroScript>().usedEmote.GetComponent<RawImage>().texture = transform.parent.parent.Find("Hero").GetComponent<HeroScript>().emotes[2].texture;
                transform.parent.parent.Find("Hero").GetComponent<HeroScript>().emotePanel.SetActive(false);
                transform.parent.parent.Find("Hero").GetComponent<HeroScript>().emoteUsed = true;
                transform.parent.parent.Find("Hero").GetComponent<HeroScript>().emoteUsedTime = 2f;
                break;
            case "Wow":
                transform.parent.parent.Find("Hero").GetComponent<HeroScript>().usedEmote.GetComponent<RawImage>().texture = transform.parent.parent.Find("Hero").GetComponent<HeroScript>().emotes[3].texture;
                transform.parent.parent.Find("Hero").GetComponent<HeroScript>().emotePanel.SetActive(false);
                transform.parent.parent.Find("Hero").GetComponent<HeroScript>().emoteUsed = true;
                transform.parent.parent.Find("Hero").GetComponent<HeroScript>().emoteUsedTime = 2f;
                break;
            case "Oops":
                transform.parent.parent.Find("Hero").GetComponent<HeroScript>().usedEmote.GetComponent<RawImage>().texture = transform.parent.parent.Find("Hero").GetComponent<HeroScript>().emotes[4].texture;
                transform.parent.parent.Find("Hero").GetComponent<HeroScript>().emotePanel.SetActive(false);
                transform.parent.parent.Find("Hero").GetComponent<HeroScript>().emoteUsed = true;
                transform.parent.parent.Find("Hero").GetComponent<HeroScript>().emoteUsedTime = 2f;
                break;
            case "Threaten":
                transform.parent.parent.Find("Hero").GetComponent<HeroScript>().usedEmote.GetComponent<RawImage>().texture = transform.parent.parent.Find("Hero").GetComponent<HeroScript>().emotes[5].texture;
                transform.parent.parent.Find("Hero").GetComponent<HeroScript>().emotePanel.SetActive(false);
                transform.parent.parent.Find("Hero").GetComponent<HeroScript>().emoteUsed = true;
                transform.parent.parent.Find("Hero").GetComponent<HeroScript>().emoteUsedTime = 2f;
                break;
            default:
                transform.parent.parent.Find("Hero").GetComponent<HeroScript>().usedEmote.GetComponent<RawImage>().texture = transform.parent.parent.Find("Hero").GetComponent<HeroScript>().emotes[0].texture;
                transform.parent.parent.Find("Hero").GetComponent<HeroScript>().emotePanel.SetActive(false);
                transform.parent.parent.Find("Hero").GetComponent<HeroScript>().emoteUsed = true;
                transform.parent.parent.Find("Hero").GetComponent<HeroScript>().emoteUsedTime = 2f;
                break;
        }
    }
}
