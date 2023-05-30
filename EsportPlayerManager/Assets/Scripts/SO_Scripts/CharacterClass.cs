using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterData", menuName = "Character/Character")]
public class CharacterClass : ScriptableObject
{
    public string characterName;
    public int baseHealth;
    [HideInInspector] public int tempHealth;

    public int baseAttack;
    [HideInInspector] public int tempAttack;

    public float baseAttackSpeed;
    [HideInInspector] public float tempAttackSpeed;

    public int baseDefense;
    [HideInInspector] public int tempDefense;

    [HideInInspector] public float currentMana;

    public int baseDodge;
    [HideInInspector] public int tempDodge;

    public int manaIncreaseAmount;

    public Ability abilities;   // if we want to come back and add multiple actives, make this a list
    public PassiveAbility passiveAbility;
    public Class characterClass;
    public Sprite characterSprite;

    public void resetVariables()
    {
        tempHealth = baseHealth;
        tempAttack = baseAttack;
        tempAttackSpeed = baseAttackSpeed;
        tempDefense = baseDefense;
        currentMana = 0;
        //Debug.Log("Reset, health: " + tempHealth);
    }
}
