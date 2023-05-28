using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainEnemy : MonoBehaviour
{
    [SerializeField] CharacterClass charClass;
    private MainCharacter playerRef;

    private void Start()
    {
        playerRef = GameObject.FindGameObjectWithTag("MainCharacter").GetComponent<MainCharacter>();
        

        charClass.resetVariables();
        switch (charClass.passiveAbility.passiveEffect)
        {
            case passiveAbility.increaseDamagePerAttack:
                print("increase damage passive");
                break;
            case passiveAbility.StrongerLessAllies:
                print("stronger less allies passive passive");
                break;
        }
        print("Starting attack: " + charClass.tempAttack);
        StartCoroutine(AttackTrigger());
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            playerRef.changeStats(charClass);
        }
    }
    IEnumerator AttackTrigger()
    {
        while (true)
        {
            yield return new WaitForSeconds(charClass.baseAttackSpeed);
            charClass.currentMana += charClass.manaIncreaseAmount;
            if (charClass.currentMana >= charClass.abilities.manaCost)
            {
                charClass.currentMana -= charClass.abilities.manaCost;
                // TO DO: activate ability
            }
            switch (charClass.passiveAbility.passiveEffect)
            {
                case passiveAbility.increaseDamagePerAttack:
                    charClass.tempAttack += 1;
                    break;
                case passiveAbility.StrongerLessAllies:
                    print("stronger less allies passive passive");
                    break;
            }
            //print(charClass.tempAttackSpeed);
        }
    }
    public void changeStats(CharacterClass charClassRef)
    {
        charClass.tempHealth -= Mathf.RoundToInt(charClassRef.abilities.decreaseStats[0]);
        charClass.tempAttack -= Mathf.RoundToInt(charClassRef.abilities.decreaseStats[1]);
        charClass.tempAttackSpeed -= charClassRef.abilities.decreaseStats[2];
        charClass.tempDefense -= Mathf.RoundToInt(charClassRef.abilities.decreaseStats[3]);
        //print("Enemy health: " + charClass.tempHealth);
    }
}
