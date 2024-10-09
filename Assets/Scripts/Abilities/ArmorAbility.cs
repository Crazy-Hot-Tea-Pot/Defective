﻿using UnityEngine;

[CreateAssetMenu(fileName = "New Armor Ability", menuName = "Abilities/Armor")]
public class ArmorAbility : Ability
{
    /// <summary>
    /// How much shielding
    /// </summary>
    public int shield;


    public override void Activate()
    {
        //Use the base of activate
        base.Activate();
        //Override from here down
        Debug.Log("Ability used" + abilityName);

        //If there is enough energy for the card
        if(GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().Energy - energyCost > 0)
        {
            //Apply shield to the player
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().ApplyShield(shield);
            //Cost energy
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().PlayedCardOrAbility(energyCost);
            Debug.Log(GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().Shield + "Current shield " + shield + " Restored and cost " + energyCost + " Making energy " + GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().Energy);
        }
        else
        {
            Debug.Log("Too high an energy cost");
        }

    }
}
