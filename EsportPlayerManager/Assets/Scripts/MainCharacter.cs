using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Unity.VisualScripting;
using UnityEngine;

public struct CharProfficencies
{
    public string nameOfClass;
    public float lvlUp;
    public int level;
}
public class MainCharacter : MonoBehaviour
{
    [SerializeField] List<CharacterClass> listOfClasses;    // List of all possible classes
    [SerializeField] CharacterClass charClass;              // Their current selected character
    private List<CharacterClass> charactersOnTeam;          // Holds all the characters the player can swap into (their team for that round)
    List<CharProfficencies> charProf = new List<CharProfficencies>();   // a list of our champions profficencies
    private MainEnemy enemyRef;

    bool tempSwapBool = true;

    private void Start()
    {
        enemyRef = GameObject.FindGameObjectWithTag("MainEnemy").GetComponent<MainEnemy>();
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

        StartCoroutine(AttackTrigger());
        

        // if trained fireDude
        //IncreaseProf(fireDudeObj, .1f);
    }
    private void Update()
    {
        // TO DO: actually make a swapping button and let them select who to swap to
        if (Input.GetKeyDown(KeyCode.Space) && tempSwapBool)
        {
            swapCharacters(listOfClasses[1]);
            tempSwapBool = false;
        }
        else if(Input.GetKeyDown(KeyCode.Space) && !tempSwapBool)
        {
            swapCharacters(listOfClasses[0]);
            tempSwapBool = true;
        }
    }

    IEnumerator AttackTrigger()
    {
        while (true)
        {
            yield return new WaitForSeconds(charClass.baseAttackSpeed);
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

    void swapCharacters(CharacterClass whichToSwapTo)
    {
        charClass = whichToSwapTo;
        print("current character: " + charClass.name + " attack speed: " + charClass.tempAttackSpeed);
    }

    void IncreaseProf(CharProfficencies whichChar , float xpInrease)
    {
        whichChar.lvlUp += xpInrease;
        if(whichChar.lvlUp >= 1)
        {
            whichChar.lvlUp = whichChar.lvlUp - 1f;
            whichChar.level++;
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
