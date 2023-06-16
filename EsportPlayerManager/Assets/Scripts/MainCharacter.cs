using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public struct CharProfficencies
{
    public string nameOfClass;
    public float lvlUp;
    public int level;
}
public struct CharBalancing
{
    public string name;
    public int bans;
    public int picks;
    public int wins;
}
public class MainCharacter : MonoBehaviour
{
    // making it persist between scenes for training purposes
    private static MainCharacter instance;
    
    //Base vars
    [SerializeField] CharacterClass charClass;              // Their current selected character
    public List<CharacterClass> charactersOnTeam;          // Holds all the characters the player can swap into (their team for that round)

    //Player vars
    public List<CharacterClass> listOfClasses;    // List of all possible classes
    List<CharProfficencies> charProf = new List<CharProfficencies>();   // a list of our champions profficencies
    List<CharBalancing> charBalancingList = new List<CharBalancing>();   // a list of our champions profficencies
    private MainEnemy enemyRef;

    [HideInInspector] public int dayCount;
    [SerializeField] int seasonLength = 30;
    int startingSeasonLength;

    //bool tempSwapBool = true;
    [HideInInspector] public bool startCombat;  // called from the banPick script to start combat
    [HideInInspector] public bool resetHealth;
    int whichCharacter = 0;
    BanPickUI banPickRef;
    int wins = 0;
    int combatRounds = 1;
    bool revived = false;
    float mageBuff = 0;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        //enemyRef = GameObject.FindGameObjectWithTag("MainEnemy").GetComponent<MainEnemy>();
        //enemyRef.changeStats(charClass);
        //charClass.resetVariables();
        foreach (CharacterClass tempChar in listOfClasses)
        {
            tempChar.resetVariables();
        }

        SetUpClasses();

        for (int i = 0; i < charProf.Count; i++)
        {
            CharBalancing tempDude = new CharBalancing();
            tempDude.name = charProf[i].nameOfClass;
            charBalancingList.Add(tempDude);
            print(charBalancingList[i].name);
        }
        startingSeasonLength = seasonLength;
        // if trained fireDude
        //IncreaseProf(fireDudeObj, .1f);
        //enemyRef = GameObject.FindGameObjectWithTag("MainEnemy").GetComponent<MainEnemy>();
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(scene.name == "CombatScene")
        {
            enemyRef = GameObject.FindGameObjectWithTag("MainEnemy").GetComponent<MainEnemy>();
            banPickRef = GameObject.FindGameObjectWithTag("CombatUI").GetComponent<BanPickUI>();
        }
        
    }
    private void Update()
    {
        if (startCombat)
        {
            print("Starting combat");
            startCombat = false;
            resetHealth = true;
            enemyRef.SwitchEnemyToAttack();
            StartCoroutine(AttackTrigger());
        }
        if(charClass.tempHealth <= 0 && resetHealth)
        {
            print("character death");
            swapCharacters();
        }
        SeasonBalancing();
    }
    public void StopAttacking()
    {
        //StopCoroutine(AttackTrigger());
        StopAllCoroutines();
    }
    public void ChangeRound(int changeWinVal)
    {
        enemyRef.StopAttacking();
        StopAllCoroutines();
        combatRounds++;
        wins += changeWinVal;

        banPickRef.DisableBenchUI();
        mageBuff = 0;
        revived = false;
        foreach (CharacterClass tempChar in listOfClasses)
        {
            tempChar.resetVariables();
            print("reset: " + tempChar.characterName);
        }
        charactersOnTeam.Clear();

        enemyRef.charactersOnTeam.Clear();

        if (combatRounds >= 4)
        {
            // end combat
            if(wins >= 2)
            {
                // player won
            }
            else
            {
                // enemy won
            }
            SceneManager.LoadScene("Room");
            banPickRef.bpTimes = 0;
            combatRounds = 0;
            dayCount++;
            wins = 0;
        }
    }
    public int ReturnRounds()
    {
        return combatRounds;
    }
    void SeasonBalancing()
    {
        if(dayCount >= seasonLength)
        {
            // new season
            seasonLength += startingSeasonLength;
            for(int i = 0; i < listOfClasses.Count; i++)
            {
                bool canUpdate = true;
                // if the wins / picks > .65 (65%) then nerf
                if ((float)charBalancingList[i].wins / charBalancingList[i].picks > .65f)
                {
                    canUpdate = false;
                    // nerf listOfClasses[i]
                }
                // if the wins / picks < .45 (45%) then buff
                if ((float)charBalancingList[i].wins / charBalancingList[i].picks < .45f)
                {
                    canUpdate = false;
                    // buff listOfClasses[i]
                }
                // if the picks < 5 (picked less than 5 times, 5 should be a var) then buff
                if (charBalancingList[i].picks > 5 && canUpdate)
                {
                    canUpdate = false;
                    // buff listOfClasses[i]
                }
                // if the bans > 10 (banned more than 10 times, 10 should be a var) then nerf
                if (charBalancingList[i].bans > 10 && canUpdate)
                {
                    // nerf listOfClasses[i]
                }
            }
        }
    }
    void BuffOrNerf(CharacterClass whichCharacter, int buffOrNerf)
    {
        if(buffOrNerf == 1)
        {
            // buff
            int rando = UnityEngine.Random.Range(0,5);
            if(rando == 0)
            {
                whichCharacter.baseHealth = (int)(whichCharacter.baseHealth * UnityEngine.Random.Range(1.1f, 1.3f));
            }
            if(rando == 1)
            {
                whichCharacter.baseAttack = (int)(whichCharacter.baseAttack * UnityEngine.Random.Range(1.1f, 1.3f));
            }
            if(rando == 2)
            {
                whichCharacter.baseAttackSpeed = (whichCharacter.baseAttackSpeed * UnityEngine.Random.Range(.7f, .9f));
            }
            if (rando == 3)
            {
                whichCharacter.baseDefense = (int)(whichCharacter.baseDefense * UnityEngine.Random.Range(1.1f, 1.3f));
            }
            if(rando == 4)
            {
                whichCharacter.manaIncreaseAmount = (int)(whichCharacter.manaIncreaseAmount * UnityEngine.Random.Range(1.1f, 1.3f));
            }
        }
        else if (buffOrNerf == 2)
        {
            // nerf
            int rando = UnityEngine.Random.Range(0, 5);
            if (rando == 0)
            {
                whichCharacter.baseHealth = (int)(whichCharacter.baseHealth * UnityEngine.Random.Range(.7f, .9f));
            }
            if (rando == 1)
            {
                whichCharacter.baseAttack = (int)(whichCharacter.baseAttack * UnityEngine.Random.Range(.7f, .9f));
            }
            if (rando == 2)
            {
                whichCharacter.baseAttackSpeed = (whichCharacter.baseAttackSpeed * UnityEngine.Random.Range(1.1f, 1.3f));
            }
            if (rando == 3)
            {
                whichCharacter.baseDefense = (int)(whichCharacter.baseDefense * UnityEngine.Random.Range(.7f, .9f));
            }
            if (rando == 4)
            {
                whichCharacter.manaIncreaseAmount = (int)(whichCharacter.manaIncreaseAmount * UnityEngine.Random.Range(.7f, .9f));
            }
        }
        else
        {
            print("Invalid buff or nerf value");
        }
    }
    IEnumerator AttackTrigger()
    {
        while (true)
        {
            yield return new WaitForSeconds(charClass.baseAttackSpeed);
            // damage enemy
            float bonusDamage = 0;
            if(charClass.characterClass.occupationClassTypeEnumVal == OccupationClassType.Berserker)
            {
                if(CountOccupationalClass(OccupationClassType.Berserker) >= 2)
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
            enemyRef.TakeDamage(charClass.tempAttack + Mathf.CeilToInt(bonusDamage));
            if(CountElementClass(ElementalClassType.Water) >= 3)
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
                        enemyRef.charClass.currentMana -= 2;
                        charClass.currentMana += 2;
                    }
                    else if (CountOccupationalClass(OccupationClassType.Mage) >= 1)
                    {
                        enemyRef.charClass.currentMana -= 1;
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
    }

    public void swapCharacters() //CharacterClass whichToSwapTo
    {
        CharacterClass whichToSwapTo;
        if (charClass.tempHealth <= 0)
        {
            if (charactersOnTeam.Count <= 1 && resetHealth)
            {
                // game over
                print("PLAYER LOST");
                resetHealth = false;
                startCombat = false;
                charactersOnTeam.RemoveAt(whichCharacter % charactersOnTeam.Count);
                foreach (CharacterClass tempChar in listOfClasses)
                {
                    tempChar.resetVariables();
                }
                banPickRef.WinLoseCondition();
                StopCoroutine(AttackTrigger());
                enemyRef.StopAttacking();
                ChangeRound(0);
            }
            else
            {
                if(CountElementClass(ElementalClassType.Grass) >= 3 && !revived)
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
    public void TakeDamage(int damageToTake)
    {
        int rando = UnityEngine.Random.Range(0,100);
        if (rando >= charClass.tempDodge)
        {
            charClass.tempHealth -= Mathf.Max(Mathf.CeilToInt((float)damageToTake / charClass.tempDefense), 1);
            if(charClass.tempHealth < charClass.baseHealth / 10 && CountOccupationalClass(OccupationClassType.Berserker) > 2)
            {
                //activate berserk mana regen
                if(charClass.currentMana < 100)
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
    public void IncreaseProf(int whichChar , float xpInrease)
    {
        CharProfficencies tempCharObj = new CharProfficencies();
        tempCharObj.lvlUp = charProf[whichChar].lvlUp;
        //print(tempCharObj.lvlUp);
        tempCharObj.lvlUp += xpInrease;
        if(tempCharObj.lvlUp >= 1)
        {
            tempCharObj.lvlUp = tempCharObj.lvlUp - 1f;
            tempCharObj.level++;
        }
        charProf[whichChar] = tempCharObj;
    }
    public float returnCharProf(int whichChar)
    {
        //print("return val: " + charProf[whichChar].lvlUp);
        return charProf[whichChar].lvlUp;
    }
    public CharacterClass getCurrentCharacter()
    {
        return charClass;
    }
    public void changeStats(CharacterClass charClassRef)
    {
        charClass.tempHealth -= Mathf.RoundToInt(charClassRef.abilities.decreaseStats[0]);
        charClass.tempAttack -= Mathf.RoundToInt(charClassRef.abilities.decreaseStats[1]);
        charClass.tempAttackSpeed *= charClassRef.abilities.decreaseStats[2];
        charClass.tempDefense -= Mathf.RoundToInt(charClassRef.abilities.decreaseStats[3]);
        //print("Enemy health: " + charClass.tempHealth);
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
    public void UpdateBalanceInfo(string charName, int banOrPick)
    {
        for(int i = 0; i < charBalancingList.Count; i++)
        {
            if (charBalancingList[i].name == charName)
            {
                if(banOrPick == 1)
                {
                    var tempBanCount = charBalancingList[i];
                    tempBanCount.bans++;
                    charBalancingList[i] = tempBanCount;
                }
                if(banOrPick == 2)
                {
                    var tempBanCount = charBalancingList[i];
                    tempBanCount.picks++;
                    charBalancingList[i] = tempBanCount;
                }
            }
        }
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
    public void ApplyClassBonuses()
    {
        // count how many times a specific class shows up
        int rogue = 0;
        foreach(CharacterClass tempChar in charactersOnTeam)
        {
            if(tempChar.characterClass.occupationClassTypeEnumVal == OccupationClassType.Rogue)
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
                if(rogue == 2)
                    tempChar.tempDodge = 50;
                if(rogue == 3)
                    tempChar.tempDodge = 60;
            }
        }
    }

    #region SettingUpClasses
    void SetUpClasses()
    {
        // Settiing up base characters (will need to add a check later for saved data)
        CharProfficencies fireDudeObj = new CharProfficencies();
        fireDudeObj.nameOfClass = listOfClasses[0].name;   // TO DO: make these match with the made scriptable objects names
        charProf.Add(fireDudeObj);

        CharProfficencies rockGuyObj = new CharProfficencies();
        rockGuyObj.nameOfClass = listOfClasses[1].name;
        charProf.Add(rockGuyObj);

        CharProfficencies waterGirl = new CharProfficencies();
        waterGirl.nameOfClass = listOfClasses[2].name;
        charProf.Add(waterGirl);

        CharProfficencies grassGuy = new CharProfficencies();
        grassGuy.nameOfClass = listOfClasses[3].name;
        charProf.Add(grassGuy);

        CharProfficencies ElectricGuy = new CharProfficencies();
        ElectricGuy.nameOfClass = listOfClasses[4].name;
        charProf.Add(ElectricGuy);

        CharProfficencies iceDude = new CharProfficencies();
        iceDude.nameOfClass = listOfClasses[5].name;
        charProf.Add(iceDude);

        CharProfficencies assasinDude = new CharProfficencies();
        assasinDude.nameOfClass = listOfClasses[6].name;
        charProf.Add(assasinDude);

        CharProfficencies mageGuy = new CharProfficencies();
        mageGuy.nameOfClass = listOfClasses[7].name;
        charProf.Add(mageGuy);

        CharProfficencies fighterGuy = new CharProfficencies();
        fighterGuy.nameOfClass = listOfClasses[8].name;
        charProf.Add(fighterGuy);
    }
    #endregion
}
