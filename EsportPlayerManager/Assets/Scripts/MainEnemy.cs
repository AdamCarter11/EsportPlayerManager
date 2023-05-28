using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainEnemy : MonoBehaviour
{
    [SerializeField] CharacterClass charClass;
    private MainCharacter playerRef;
    public List<CharacterClass> charactersOnTeam;
    bool enemyStartCombat = false;

    private void Start()
    {
        playerRef = GameObject.FindGameObjectWithTag("MainCharacter").GetComponent<MainCharacter>();
        

        charClass.resetVariables();
        switch (charClass.passiveAbility.passiveEffect)
        {
            case passiveAbility.increaseDamagePerAttack:
                //print("increase damage passive");
                break;
            case passiveAbility.StrongerLessAllies:
                //print("stronger less allies passive passive");
                break;
        }
        //print("Starting attack: " + charClass.tempAttack);
        //StartCoroutine(AttackTrigger());
    }
    private void Update()
    {
        //print("Starting combat, player health: " + playerRef.getCurrentCharacter().tempHealth);
        if (Input.GetKeyDown(KeyCode.A))
        {
            playerRef.changeStats(charClass);
        }
        if (enemyStartCombat)
        {
            enemyStartCombat = false;
            //print("Starting combat, player health: " + playerRef.getCurrentCharacter().tempHealth);
            print("enemy started combat");
            //startCombat = false;
            StartCoroutine(AttackTrigger());
        }
    }
    public void SwitchEnemyToAttack()
    {
        enemyStartCombat = true;
    }
    public void StopAttacking()
    {
        //StopCoroutine(AttackTrigger());
        StopAllCoroutines();
    }
    IEnumerator AttackTrigger()
    {
        while (true)
        {
            yield return new WaitForSeconds(charClass.baseAttackSpeed);
            // deal damage to player
            playerRef.TakeDamage(charClass.tempAttack);

            charClass.currentMana += charClass.manaIncreaseAmount;
            
            switch (charClass.passiveAbility.passiveEffect)
            {
                case passiveAbility.increaseDamagePerAttack:
                    charClass.tempAttack += 1;
                    break;
                case passiveAbility.StrongerLessAllies:
                    //print("stronger less allies passive passive");
                    break;
            }
            //print(charClass.tempAttackSpeed);
        }
    }
    public void TakeDamage(int damageToTake)
    {
        charClass.tempHealth -= Mathf.Max(Mathf.CeilToInt((float)damageToTake / charClass.tempDefense), 1);
    }
    public void changeStats(CharacterClass charClassRef)
    {
        charClass.tempHealth -= Mathf.RoundToInt(charClassRef.abilities.decreaseStats[0]);
        charClass.tempAttack -= Mathf.RoundToInt(charClassRef.abilities.decreaseStats[1]);
        charClass.tempAttackSpeed -= charClassRef.abilities.decreaseStats[2];
        charClass.tempDefense -= Mathf.RoundToInt(charClassRef.abilities.decreaseStats[3]);
        //print("Enemy health: " + charClass.tempHealth);
    }
    public void ActivateEnemyAbility()
    {
        if (charClass.currentMana >= charClass.abilities.manaCost)
        {
            charClass.currentMana -= charClass.abilities.manaCost;
            // TO DO: activate ability
        }
    }
    public CharacterClass getCurrentCharacter()
    {
        return charClass;
    }
}
