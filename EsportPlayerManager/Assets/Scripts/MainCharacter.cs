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

    public List<CharacterClass> listOfClasses;    // List of all possible classes
    [SerializeField] CharacterClass charClass;              // Their current selected character
    public List<CharacterClass> charactersOnTeam;          // Holds all the characters the player can swap into (their team for that round)
    List<CharProfficencies> charProf = new List<CharProfficencies>();   // a list of our champions profficencies
    List<CharBalancing> charBalancingList = new List<CharBalancing>();   // a list of our champions profficencies
    private MainEnemy enemyRef;
    [HideInInspector] public int dayCount;
    [SerializeField] int seasonLength = 30;
    int startingSeasonLength;

    bool tempSwapBool = true;
    [HideInInspector] public bool startCombat;  // called from the banPick script to start combat
    int whichCharacter = 0;

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
        enemyRef = GameObject.FindGameObjectWithTag("MainEnemy").GetComponent<MainEnemy>();
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(scene.name == "CombatScene")
        {
            enemyRef = GameObject.FindGameObjectWithTag("MainEnemy").GetComponent<MainEnemy>();
        }
    }
    private void Update()
    {
        if (startCombat)
        {
            startCombat = false;
            StartCoroutine(AttackTrigger());
        }
        if(charClass.tempHealth <= 0)
        {
            swapCharacters();
        }
        SeasonBalancing();
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
            enemyRef.TakeDamage(charClass.tempAttack);

            charClass.currentMana += charClass.manaIncreaseAmount;
            switch (charClass.passiveAbility.passiveEffect)
            {
                case passiveAbility.increaseDamagePerAttack:
                    //charClass.tempAttack += 1;
                    break;
                case passiveAbility.StrongerLessAllies:
                    //print("stronger less allies passive passive");
                    break;
            }
            //print(charClass.baseAttack);
        }
    }

    public void swapCharacters() //CharacterClass whichToSwapTo
    {
        if(charClass.tempHealth <= 0)
        {
            if (charactersOnTeam.Count <= 1)
            {
                // game over
                print("PLAYER LOST");
            }
            else
            {
                charactersOnTeam.RemoveAt(whichCharacter);
            }
        }
        whichCharacter++;
        CharacterClass whichToSwapTo = charactersOnTeam[whichCharacter % 3];
        charClass = whichToSwapTo;
        print("current character: " + charClass.name + " attack speed: " + charClass.tempAttackSpeed);
    }
    public void TakeDamage(int damageToTake)
    {
        charClass.tempHealth -= Mathf.Max(Mathf.CeilToInt((float)damageToTake / charClass.tempDefense), 1);
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
        charClass.tempAttackSpeed -= charClassRef.abilities.decreaseStats[2];
        charClass.tempDefense -= Mathf.RoundToInt(charClassRef.abilities.decreaseStats[3]);
        //print("Enemy health: " + charClass.tempHealth);
    }
    public void ActivateAbility()
    {
        if (charClass.currentMana >= charClass.abilities.manaCost)
        {
            print("Activated ability!");
            charClass.currentMana -= charClass.abilities.manaCost;
            // TO DO: activate ability
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
