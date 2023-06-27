using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseCharacter : MonoBehaviour
{
    [SerializeField] public CharacterClass charClass;
    public List<CharacterClass> charactersOnTeam;
    //EnemyV2 enemyRef; // TO DO: this probably should get removed, and then the functions should use parameters
    BanPickUI banPickRef;
    [HideInInspector] public bool startCombat;  // TO DO: there may be a better solution to this


    bool revived = false;
    float mageBuff = 0;
    [HideInInspector] public bool resetHealth;
    int whichCharacter = 0;

    GameObject playerRef;
    GameObject enemyRef;

    private void Start()
    {
        playerRef = GameObject.FindGameObjectWithTag("MainCharacter");
        enemyRef = GameObject.FindGameObjectWithTag("MainEnemy");
    }
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
    public void ActivateAbility(GameObject whichRef)
    {
        BaseCharacter whichObj;
        if (gameObject.GetComponent<EnemyV2>() != null)
        {
            whichObj = playerRef.GetComponent<BaseCharacter>();
        }
        else
        {
            whichObj = enemyRef.GetComponent<BaseCharacter>();
        }
        if (charClass.currentMana >= charClass.abilities.manaCost)
        {
            if (CountOccupationalClass(OccupationClassType.Mage) > 2)
            {
                print("Activated PLAYER ability! w/ mage class");
                charClass.currentMana -= charClass.abilities.manaCost;
                // reduce enemy health by ability damage
                whichObj.charClass.tempHealth -= Mathf.CeilToInt(charClass.abilities.damage / whichObj.charClass.tempDefense * (1 + mageBuff / 100));
                // change enemy stats by ability
                whichObj.changeStats(charClass);
                // buff yourself
                changeStats(charClass);
                if (CountElementClass(ElementalClassType.Water) > 2)
                {
                    // reduce enemy health by ability damage
                    whichObj.charClass.tempHealth -= Mathf.CeilToInt(charClass.abilities.damage / whichObj.charClass.tempDefense * .50f * (1 + mageBuff / 100));
                    // change enemy stats by ability
                    whichObj.changeStats(charClass);
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
                whichObj.charClass.tempHealth -= Mathf.CeilToInt(charClass.abilities.damage / whichObj.charClass.tempDefense);
                // change enemy stats by ability
                whichObj.changeStats(charClass);
                // buff yourself
                changeStats(charClass);
                if (CountElementClass(ElementalClassType.Water) > 2)
                {
                    // reduce enemy health by ability damage
                    whichObj.charClass.tempHealth -= Mathf.CeilToInt(charClass.abilities.damage / whichObj.charClass.tempDefense * .50f);
                    // change enemy stats by ability
                    whichObj.changeStats(charClass);
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
                // have this check here to see who is the character that should be getting retaliated against
                if(gameObject.GetComponent<EnemyV2>() != null)
                {
                    playerRef.GetComponent<BaseCharacter>().TakeDamage(charClass.tempAttack);
                }
                else if(gameObject.GetComponent<PlayerV2>() != null)
                {
                    enemyRef.GetComponent<BaseCharacter>().TakeDamage(charClass.tempAttack);
                }
            }
        }
    }
    
    public void swapCharacters(List<CharacterClass> listOfClasses = null)
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

                playerRef.GetComponent<PlayerV2>().StopPlayerAttack();
                enemyRef.GetComponent<EnemyV2>().StopEnemyAttack();

                //This could be written better, but basically check which script it was called from and then change the rounds accordingly
                if(ReturnOtherCharacter() == enemyRef)
                    playerRef.GetComponent<PlayerV2>().ChangeRound(0);
                else if(ReturnOtherCharacter() == playerRef)
                    playerRef.GetComponent<PlayerV2>().ChangeRound(1);
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
    
    public void AttackTrigger(GameObject whichChar)
    {
        float bonusDamage = 0;
        if (charClass.characterClass.occupationClassTypeEnumVal == OccupationClassType.Berserker)
        {
            if (CountOccupationalClass(OccupationClassType.Berserker) >= 2)
            {
                bonusDamage = .5f * (charClass.baseHealth - charClass.tempHealth);
            }
            else if (CountOccupationalClass(OccupationClassType.Berserker) >= 1)
            {
                bonusDamage = .25f * (charClass.baseHealth - charClass.tempHealth);
            }
        }
        else
        {
            bonusDamage = 0;
        }
        whichChar.GetComponent<BaseCharacter>().TakeDamage(charClass.tempAttack + Mathf.CeilToInt(bonusDamage));
        if (CountElementClass(ElementalClassType.Water) >= 3)
            charClass.tempHealth += Mathf.CeilToInt(charClass.tempAttack * .15f);
        else if (CountElementClass(ElementalClassType.Water) >= 2)
            charClass.tempHealth += Mathf.CeilToInt(charClass.tempAttack * .12f);
        else if (CountElementClass(ElementalClassType.Water) >= 1)
            charClass.tempHealth += Mathf.CeilToInt(charClass.tempAttack * .08f);

        switch (charClass.characterClass.occupationClassTypeEnumVal)
        {
            case OccupationClassType.Mage:
                if (CountOccupationalClass(OccupationClassType.Mage) >= 2)
                {
                    whichChar.GetComponent<BaseCharacter>().charClass.currentMana -= 2;
                    charClass.currentMana += 2;
                }
                else if (CountOccupationalClass(OccupationClassType.Mage) >= 1)
                {
                    whichChar.GetComponent<BaseCharacter>().charClass.currentMana -= 1;
                    charClass.currentMana += 1;
                }
                break;
        }

        // TO DO: maybe make these variables right at the start so we don't keep checking?
        charClass.currentMana += Mathf.CeilToInt((float)charClass.manaIncreaseAmount * ((CountElementClass(ElementalClassType.Water) / 2) + 1));
        switch (charClass.passiveAbility.passiveEffect)
        {
            case passiveAbility.increaseDamagePerAttack:
                //charClass.tempAttack += 1;
                break;
            case passiveAbility.StrongerLessAllies:
                //print("stronger less allies passive passive");
                break;
        }
        // checks for fire elemental buffs
        if (CountElementClass(ElementalClassType.Fire) >= 2)
            charClass.tempAttack += 2;
        else if (CountElementClass(ElementalClassType.Fire) >= 1)
            charClass.tempAttack += 1;
    }
    
    private GameObject ReturnOtherCharacter()
    {
        GameObject whichCharToReturn;
        if (gameObject.GetComponent<EnemyV2>() != null)
        {
            whichCharToReturn = playerRef;
        }
        else
        {
            whichCharToReturn = enemyRef;
        }
        return whichCharToReturn;
    }
}
