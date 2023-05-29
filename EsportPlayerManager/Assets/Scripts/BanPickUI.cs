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
    [SerializeField] TMP_Text roundsText;
    [SerializeField] List<Image> benchCharImages;
    [SerializeField] List<Image> enemyBenchCharImages;
    [SerializeField] List<Text> benchHpText;
    [SerializeField] List<Text> benchManaText;
    [SerializeField] List<Text> enemyBenchHpText;
    [SerializeField] List<Text> enemyBenchManaText;


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
        //playerRef.dayCount++;
        UpdateUIImages(0, -1);
    }

    void UpdateUIImages(int whichMode, int whichSprite)
    {
        int i = 0;
        foreach (Transform childTransform in banPickPanel.transform)
        {
            // Access the child object
            GameObject childObject = childTransform.gameObject;
            print("Name: " + childObject.name);

            // Check if the child object has the specific component
            Image component = childObject.GetComponent<Image>();
            if (component != null && i < playerRef.listOfClasses.Count)
            {
                if(whichMode == 0)
                {
                    component.sprite = playerRef.listOfClasses[i].characterSprite;
                }
                if(whichMode == 1 && i == whichSprite)
                {
                    component.color = Color.gray;
                }
                if (whichMode == 2 && i == whichSprite)
                {
                    component.color = Color.red;
                }
                if(whichMode == 3)
                {
                    component.color = Color.white;
                }
                i++;
            }
        }
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
                playerRef.UpdateBalanceInfo(playerRef.listOfClasses[number].name, 1); // used to keep track of how many times a char has been banned
                UpdateUIImages(2, number);

                // enemy ban
                number = Random.Range(0, 9);
                while (isBPable[number] != 0)
                {
                    number = Random.Range(0, 9);
                }
                Debug.Log("Char BP status" + isBPable);
                isBPable[number] = 1;
                bpTimes += 1;
                UpdateUIImages(2, number);
            }
            else if (bpPeriod == "Pick")
            {
                Debug.Log("Pick: Char" + number);
                isBPable[number] = 2;
                playerRef.charactersOnTeam.Add(playerRef.listOfClasses[number]);
                playerRef.UpdateBalanceInfo(playerRef.listOfClasses[number].name, 2); // 2 keeps track of pick rate
                UpdateUIImages(1, number);
                bpTimes += 1;

            }
            bpPeriod = "Pick";


            if (bpTimes >= 3)
            {
                // enemy pick
                number = Random.Range(0, 9);
                while (isBPable[number] != 0)
                {
                    number = Random.Range(0, 9);
                }

                Debug.Log("Char BP status" + isBPable);
                enemyRef.charactersOnTeam.Add(playerRef.listOfClasses[number]);
                isBPable[number] = 2;
                bpTimes += 1;
                UpdateUIImages(1, number);
                /*
                for (int i = 0; i < 9; i++)
                {
                    Debug.Log("Char " + i + ": " + isBPable[i]);
                }
                */
            }   
        }
        else
        {
            Debug.Log("char:" + number + "is already b/p");
            
        }
        UpdateBench();

        if (bpTimes >= 8)
        {
            Debug.Log("BP END! Char BP status" + isBPable);
            banPickPanel.SetActive(false);
            character1UI.SetActive(true);
            character2UI.SetActive(true);
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
            playerRef.swapCharacters();
            enemyRef.swapCharacters();
            currPlayerChar = playerRef.GetComponent<MainCharacter>().getCurrentCharacter();
            playerNameText.text = currPlayerChar.name;
            character1UI.GetComponent<Image>().sprite = currPlayerChar.characterSprite;
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


        for (int i=0; i<playerRef.charactersOnTeam.Count; i ++)
        {
            benchHpText[i].text = "Health: " + playerRef.charactersOnTeam[i].tempHealth;
            benchManaText[i].text = "Mana: " + playerRef.charactersOnTeam[i].currentMana;

            /*
            healthSlider = benchCharImages[i].GetComponent<Slider>();
            healthSlider.maxValue = playerRef.charactersOnTeam[i].baseHealth;
            healthSlider.value = playerRef.charactersOnTeam[i].tempHealth;
            */
        }

        for (int i = 0; i < enemyRef.charactersOnTeam.Count; i++)
        {
            enemyBenchHpText[i].text = "Health: " + enemyRef.charactersOnTeam[i].tempHealth;
            enemyBenchManaText[i].text = "Mana: " + enemyRef.charactersOnTeam[i].currentMana;

        }

    }



    private void ActivateAbilityHelper()
    {
        playerRef.ActivateAbility();
    }
    private void SwapButtonHelper()
    {
        playerRef.swapCharacters();
        currPlayerChar = playerRef.GetComponent<MainCharacter>().getCurrentCharacter();
        playerNameText.text = currPlayerChar.name;
        character1UI.GetComponent<Image>().sprite = currPlayerChar.characterSprite;
    }
    public void UpdateUI()
    {
        currPlayerChar = playerRef.GetComponent<MainCharacter>().getCurrentCharacter();
        currEnemyChar = enemyRef.GetComponent<MainEnemy>().getCurrentCharacter();
        playerNameText.text = currPlayerChar.name;
        enemyNameText.text = currEnemyChar.name;
        character1UI.GetComponent<Image>().sprite = currPlayerChar.characterSprite;
        playerHealthText.text = "Health: " + currPlayerChar.tempHealth;
        playerManaText.text = "Mana: " + currPlayerChar.currentMana;
        enemyHealthText.text = "Health: " + currEnemyChar.tempHealth;
        enemyManaText.text = "Mana: " + currEnemyChar.currentMana;
        roundsText.text = "Round: " + playerRef.ReturnRounds();
    }
    public void WinLoseCondition()
    {
        bpPeriod = "Ban";
        bpTimes = 0;
        for (int i = 0; i < 9; i++)
        {
            isBPable[i] = 0;
        }
        banPickPanel.SetActive(true);
        character1UI.SetActive(false);
        character2UI.SetActive(false);
        UpdateUIImages(3,-1);
    }

    void UpdateBench()
    {
        for (int i=0; i < playerRef.charactersOnTeam.Count; i ++)
        {
            benchCharImages[i].gameObject.SetActive(true);
            benchCharImages[i].GetComponent<Image>().sprite = playerRef.charactersOnTeam[i].characterSprite;
            
        }

        for (int i = 0; i < enemyRef.charactersOnTeam.Count; i++)
        {
            enemyBenchCharImages[i].gameObject.SetActive(true);
            enemyBenchCharImages[i].GetComponent<Image>().sprite = enemyRef.charactersOnTeam[i].characterSprite;
        }
    }


}
