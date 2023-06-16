using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseCharacter : MonoBehaviour
{
    [SerializeField] CharacterClass charClass;
    public List<CharacterClass> charactersOnTeam;
    private MainEnemy enemyRef; // TO DO: this probably should get removed, and then the functions should use parameters
    BanPickUI banPickRef;
    [HideInInspector] public bool startCombat;  // TO DO: there may be a better solution to this


    bool revived = false;
    float mageBuff = 0;
    [HideInInspector] public bool resetHealth;
    int whichCharacter = 0;

    public void ApplyClassBonuses()
    {
        // count how many times a specific class shows up
        int rogue = 0;
        foreach (CharacterClass tempChar in charactersOnTeam)
        {
            if (tempChar.characterClass.occupationClassTypeEnumVal == OccupationClassType.Rogue)
            {
                rogue++;
            }
        }
        // apply bonuses based on class amount
        foreach (CharacterClass tempChar in charactersOnTeam)
        {
            if (tempChar.characterClass.occupationClassTypeEnumVal == OccupationClassType.Rogue)
            {
                if (rogue == 1)
                    tempChar.tempDodge = 25;
                if (rogue == 2)
                    tempChar.tempDodge = 50;
                if (rogue == 3)
                    tempChar.tempDodge = 60;
            }
        }
    }
    private int CountOccupationalClass(OccupationClassType elementCheck)
    {
        int i = 0;
        foreach (CharacterClass tempChar in charactersOnTeam)
        {
            if (tempChar.characterClass.occupationClassTypeEnumVal == elementCheck)
            {
                i++;
            }
        }
        return i;
    }
    private int CountElementClass(ElementalClassType elementCheck)
    {
        int i = 0;
        foreach (CharacterClass tempChar in charactersOnTeam)
        {
            if (tempChar.characterClass.classTypeEnumVal == elementCheck)
            {
                i++;
            }
        }
        return i;
    }
    public void ActivateAbility()
    {
        if (charClass.currentMana >= charClass.abilities.manaCost)
        {
            if (CountOccupationalClass(OccupationClassType.Mage) > 2)
            {
                print("Activated PLAYER ability!");
                charClass.currentMana -= charClass.abilities.manaCost;
                // reduce enemy health by ability damage
                enemyRef.getCurrentCharacter().tempHealth -= Mathf.CeilToInt(charClass.abilities.damage / enemyRef.charClass.tempDefense * (1 + mageBuff / 100));
                // change enemy stats by ability
                enemyRef.changeStats(charClass);
                // buff yourself
                changeStats(charClass);
                if (CountElementClass(ElementalClassType.Water) > 2)
                {
                    // reduce enemy health by ability damage
                    enemyRef.getCurrentCharacter().tempHealth -= Mathf.CeilToInt(charClass.abilities.damage / enemyRef.charClass.tempDefense * .50f * (1 + mageBuff / 100));
                    // change enemy stats by ability
                    enemyRef.changeStats(charClass);
                    // buff yourself
                    changeStats(charClass);
                }
                mageBuff += 20;
            }
            else
            {
                print("Activated PLAYER ability!");
                charClass.currentMana -= charClass.abilities.manaCost;
                // reduce enemy health by ability damage
                enemyRef.getCurrentCharacter().tempHealth -= Mathf.CeilToInt(charClass.abilities.damage / enemyRef.charClass.tempDefense);
                // change enemy stats by ability
                enemyRef.changeStats(charClass);
                // buff yourself
                changeStats(charClass);
                if (CountElementClass(ElementalClassType.Water) > 2)
                {
                    // reduce enemy health by ability damage
                    enemyRef.getCurrentCharacter().tempHealth -= Mathf.CeilToInt(charClass.abilities.damage / enemyRef.charClass.tempDefense * .50f);
                    // change enemy stats by ability
                    enemyRef.changeStats(charClass);
                    // buff yourself
                    changeStats(charClass);
                }
            }
        }
    }

    public void changeStats(CharacterClass charClassRef)
    {
        charClass.tempHealth -= Mathf.RoundToInt(charClassRef.abilities.decreaseStats[0]);
        charClass.tempAttack -= Mathf.RoundToInt(charClassRef.abilities.decreaseStats[1]);
        charClass.tempAttackSpeed *= charClassRef.abilities.decreaseStats[2];
        charClass.tempDefense -= Mathf.RoundToInt(charClassRef.abilities.decreaseStats[3]);
        //print("Enemy health: " + charClass.tempHealth);
    }

    public void TakeDamage(int damageToTake)
    {
        int rando = UnityEngine.Random.Range(0, 100);
        if (rando >= charClass.tempDodge)
        {
            charClass.tempHealth -= Mathf.Max(Mathf.CeilToInt((float)damageToTake / charClass.tempDefense), 1);
            if (charClass.tempHealth < charClass.baseHealth / 10 && CountOccupationalClass(OccupationClassType.Berserker) > 2)
            {
                //activate berserk mana regen
                if (charClass.currentMana < 100)
                {
                    charClass.currentMana = 100;
                }
            }
        }
        else
        {
            print("miss: " + charClass.tempDodge);
            if (charClass.characterClass.occupationClassTypeEnumVal == OccupationClassType.Rogue)
            {
                enemyRef.TakeDamage(charClass.tempAttack);
            }
        }
    }
    
    public void swapCharacters(List<CharacterClass> listOfClasses = null)    // this function also calls the ienumerator for attack, so it probably needs an extra parameter
    {
        CharacterClass whichToSwapTo;

        banPickRef = GameObject.FindGameObjectWithTag("CombatUI").GetComponent<BanPickUI>();    // TO DO: this can definitly be implimented better (maybe only do it on the player?)

        if (charClass.tempHealth <= 0)
        {
            if (charactersOnTeam.Count <= 1 && resetHealth)
            {
                // game over
                print("PLAYER LOST");
                resetHealth = false;

                startCombat = false;
                charactersOnTeam.RemoveAt(whichCharacter % charactersOnTeam.Count);
                if(listOfClasses == null)
                {
                    PlayerV2 playerRef = GameObject.FindGameObjectWithTag("MainCharacter").GetComponent<PlayerV2>();
                    listOfClasses = playerRef.listOfClasses;
}
                foreach (CharacterClass tempChar in listOfClasses)
                {
                    tempChar.resetVariables();
                }
                banPickRef.WinLoseCondition();

                //StopCoroutine(AttackTrigger()); // TO DO: have a reference to the coroutine so we can stop them from this script

                enemyRef.StopAttacking();

                //ChangeRound(0); // TO DO: call this in the player script
            }
            else
            {
                if (CountElementClass(ElementalClassType.Grass) >= 3 && !revived)
                {
                    revived = true;
                    charClass.tempHealth += Mathf.CeilToInt(charClass.baseHealth * .10f);
                }
                else
                {
                    charactersOnTeam.RemoveAt(whichCharacter % charactersOnTeam.Count);
                    whichToSwapTo = charactersOnTeam[whichCharacter % charactersOnTeam.Count];
                    charClass = whichToSwapTo;
                }
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
        print("current character: " + charClass.name + " attack speed: " + charClass.tempAttackSpeed);
    }
    
    public void AttackTrigger()
    {
        
    }
    //maybe also need a stop attacking func
}
