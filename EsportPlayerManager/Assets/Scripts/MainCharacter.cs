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
public class MainCharacter : MonoBehaviour
{
    // making it persist between scenes for training purposes
    private static MainCharacter instance;

    public List<CharacterClass> listOfClasses;    // List of all possible classes
    [SerializeField] CharacterClass charClass;              // Their current selected character
    public List<CharacterClass> charactersOnTeam;          // Holds all the characters the player can swap into (their team for that round)
    List<CharProfficencies> charProf = new List<CharProfficencies>();   // a list of our champions profficencies
    private MainEnemy enemyRef;
    [HideInInspector] public int dayCount;

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
        // Settiing up base characters (will need to add a check later for saved data)
        CharProfficencies fireDudeObj = new CharProfficencies();
        fireDudeObj.nameOfClass = listOfClasses[0].name;   // TO DO: make these match with the made scriptable objects names
        charProf.Add(fireDudeObj);

        CharProfficencies rockGuyObj = new CharProfficencies();
        rockGuyObj.nameOfClass = listOfClasses[1].name;
        charProf.Add(rockGuyObj);

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
        charClass.tempHealth -= Mathf.Max(damageToTake - charClass.tempDefense , 0);
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

}
