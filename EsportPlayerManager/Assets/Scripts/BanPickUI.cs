using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class BanPickUI : MonoBehaviour
{
    private MainCharacter playerRef;
    private CharacterClass currPlayerChar;
    private MainEnemy enemyRef;
    private CharacterClass currEnemyChar;

    [SerializeField] GameObject banPickPanel;
    [SerializeField] GameObject character1UI, character2UI;
    [SerializeField] TMP_Text playerHealthText, playerManaText;
    [SerializeField] TMP_Text enemyHealthText, enemyManaText;
    [SerializeField] TMP_Text playerNameText;
    [SerializeField] TMP_Text enemyNameText;


    private int bpTimes = 0;
    private string bpPeriod = "Ban";

    private int[] isBPable = new int [10];
    private Button abilityButton;
    private Button swapButton;



    private void Start()
    {
        playerRef = GameObject.FindGameObjectWithTag("MainCharacter").GetComponent<MainCharacter>();
        currPlayerChar = playerRef.GetComponent<MainCharacter>().getCurrentCharacter();
        enemyRef = GameObject.FindGameObjectWithTag("MainEnemy").GetComponent<MainEnemy>();
        currEnemyChar = enemyRef.GetComponent<MainEnemy>().getCurrentCharacter();

    }



    public void BanPickChar(int number)
    {
        Debug.Log("BP times" + bpTimes);


        if (isBPable[number] == 0)
        {
            if (bpPeriod == "Ban")
            {
                Debug.Log("Ban: Char" + number);
                isBPable[number] = 1;
                bpTimes += 1;
            }
            else if (bpPeriod == "Pick")
            {
                Debug.Log("Pick: Char" + number);
                isBPable[number] = 1;
                bpTimes += 1;
                playerRef.charactersOnTeam.Add(playerRef.listOfClasses[number]);
            }
        }
        else
        {
            Debug.Log("char:" + number + "is already b/p");
            
        }



        if (bpTimes >= 2)
        {
            bpPeriod = "Pick";
            Debug.Log("Char BP status" + isBPable);
            /*
            for (int i = 0; i < 9; i++)
            {
                Debug.Log("Char " + i + ": " + isBPable[i]);
            }
            */
        }

        if (bpTimes >= 8)
        {
            Debug.Log("BP END! Char BP status" + isBPable);
            banPickPanel.SetActive(false);
            character1UI.SetActive(true);
            character2UI.SetActive(true);
            playerNameText.text = currPlayerChar.name;
            enemyNameText.text = currEnemyChar.name;
            GameObject manaButton = GameObject.Find("ActivatedAbilityButton");
            if(manaButton != null)
            {
                abilityButton = manaButton.GetComponent<Button>();
                abilityButton.onClick.AddListener(ActivateAbilityHelper);
            }
            else
            {
                print("MANA button isn't in scene or named correctly");
            }
            GameObject swapButtonObj = GameObject.Find("SwapCharactersButton");
            if(swapButtonObj != null)
            {
                swapButton = swapButtonObj.GetComponent<Button>();
                swapButton.onClick.AddListener(SwapButtonHelper);
            }
            else
            {
                print("SWAP button isn't in scene or named correctly");
            }
            playerRef.startCombat = true;
            for (int i = 0; i < 9; i++)
            {
                Debug.Log("Char " + i + ": " + isBPable[i]);
            }
        }
    }

    private void Update()
    {
        if(bpTimes >= 8)
        {
            playerHealthText.text = "Health: " + currPlayerChar.tempHealth;
            playerManaText.text = "Mana: " + currPlayerChar.currentMana;
            enemyHealthText.text = "Health: " + currEnemyChar.tempHealth;
            enemyManaText.text = "Mana: " + currEnemyChar.currentMana;
        }
    }
    private void ActivateAbilityHelper()
    {
        playerRef.ActivateAbility();
    }
    private void SwapButtonHelper()
    {
        playerNameText.text = currPlayerChar.name;
        playerRef.swapCharacters();
    }
}
