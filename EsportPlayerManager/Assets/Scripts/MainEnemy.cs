using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainEnemy : MonoBehaviour
{
    public CharacterClass charClass;
    private MainCharacter playerRef;
    public List<CharacterClass> charactersOnTeam;
    bool enemyStartCombat = false;
    int whichCharacter = 0;
    [HideInInspector] public bool resetHealth;
    BanPickUI banPickRef;
    bool canEnemySwap = true;

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
        banPickRef = GameObject.FindGameObjectWithTag("CombatUI").GetComponent<BanPickUI>();
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
            resetHealth = true;
            //print("Starting combat, player health: " + playerRef.getCurrentCharacter().tempHealth);
            print("enemy started combat");
            //startCombat = false;
            StartCoroutine(AttackTrigger());
        }

        if (charClass.tempHealth <= 0 && resetHealth)
        {
            print("character death");
            swapCharacters();
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
            ActivateAbility();
            charClass.currentMana += charClass.manaIncreaseAmount;
            if(charClass.tempHealth <= 10f && canEnemySwap)
            {
                StartCoroutine(EnemySwapCoolDown());
                swapCharacters();
            }
            
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
    IEnumerator EnemySwapCoolDown()
    {
        canEnemySwap = false;
        yield return new WaitForSeconds(3f);
        canEnemySwap = true;
    }
    public void TakeDamage(int damageToTake)
    {
        charClass.tempHealth -= Mathf.Max(Mathf.CeilToInt((float)damageToTake / charClass.tempDefense), 1);
    }
    public void changeStats(CharacterClass charClassRef)
    {
        charClass.tempHealth -= Mathf.RoundToInt(charClassRef.abilities.decreaseStats[0]);
        charClass.tempAttack -= Mathf.RoundToInt(charClassRef.abilities.decreaseStats[1]);
        charClass.tempAttackSpeed *= charClassRef.abilities.decreaseStats[2];
        charClass.tempDefense -= Mathf.RoundToInt(charClassRef.abilities.decreaseStats[3]);
        //print("Enemy health: " + charClass.tempHealth);
    }
    public void ActivateEnemyAbility()
    {
        if (charClass.currentMana >= charClass.abilities.manaCost)
        {
            charClass.currentMana -= charClass.abilities.manaCost;
            // TO DO: activate ability
            ActivateAbility();
        }
    }
    public CharacterClass getCurrentCharacter()
    {
        return charClass;
    }
    public void ActivateAbility()
    {
        if (charClass.currentMana >= charClass.abilities.manaCost)
        {
            print("Activated ENEMY ability!");
            charClass.currentMana -= charClass.abilities.manaCost;
            // TO DO: activate ability
            // reduce enemy health by ability damage
            playerRef.getCurrentCharacter().tempHealth -= Mathf.CeilToInt(charClass.abilities.damage / playerRef.getCurrentCharacter().tempDefense);
            // change enemy stats by ability
            playerRef.changeStats(charClass);
            // buff yourself
            changeStats(charClass);
        }
    }
    public void swapCharacters() //CharacterClass whichToSwapTo
    {
        CharacterClass whichToSwapTo;
        if (charClass.tempHealth <= 0)
        {
            if (charactersOnTeam.Count <= 1 && resetHealth)
            {
                // game over
                print("PLAYER Win");
                enemyStartCombat = false;
                resetHealth = false;
                charactersOnTeam.RemoveAt(whichCharacter % charactersOnTeam.Count);
                playerRef.ChangeRound(1);
                foreach (CharacterClass tempChar in playerRef.listOfClasses)
                {
                    tempChar.resetVariables();
                }
                
                StopCoroutine(AttackTrigger());
                playerRef.StopAttacking();
                banPickRef.WinLoseCondition();
                
            }
            else
            {
                charactersOnTeam.RemoveAt(whichCharacter % charactersOnTeam.Count);
                whichToSwapTo = charactersOnTeam[whichCharacter % charactersOnTeam.Count];
                charClass = whichToSwapTo;
            }
        }
        else
        {
            whichCharacter++;
            print("Which char: " + whichCharacter % charactersOnTeam.Count);
            whichToSwapTo = charactersOnTeam[whichCharacter % charactersOnTeam.Count];
            charClass = whichToSwapTo;
        }

        banPickRef = GameObject.FindGameObjectWithTag("CombatUI").GetComponent<BanPickUI>();
        banPickRef.UpdateUI();
        print("current ENEMY character: " + charClass.name + " attack speed: " + charClass.tempAttackSpeed);
    }
}
