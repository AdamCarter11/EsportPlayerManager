using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerV2 : BaseCharacter
{
    // references
    //[SerializeField] CharacterClass currClass;
    public List<CharacterClass> listOfClasses;
    List<CharProfficencies> charProf = new List<CharProfficencies>();   // a list of our champions profficencies
    List<CharBalancing> charBalancingList = new List<CharBalancing>();   // a list of the champions balances

    private EnemyV2 enemyRef;
    BanPickUI banPickRef;

    [HideInInspector] public int dayCount;
    [SerializeField] int seasonLength = 30;
    int startingSeasonLength;

    int wins = 0;
    int combatRounds = 1;
    float mageBuff = 0;
    bool revived = false;

    // checks
    bool startCombat = false;
    bool resetHealth = false;


    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
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
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "CombatScene")
        {
            enemyRef = GameObject.FindGameObjectWithTag("MainEnemy").GetComponent<EnemyV2>();
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
            enemyRef.SwitchEnemyHelper();
            StartCoroutine(CoroutineAttack());
        }
        if (gameObject.GetComponent<BaseCharacter>().charClass.tempHealth <= 0 && resetHealth)
        {
            print("character death");
            swapCharacters();
        }
        SeasonBalancing();
    }
    IEnumerator CoroutineAttack()
    {
        while (true)
        {
            yield return new WaitForSeconds(gameObject.GetComponent<BaseCharacter>().charClass.baseAttackSpeed);
            AttackTrigger(enemyRef.gameObject);
        }
    }
    public void StopPlayerAttack()
    {
        StopCoroutine(CoroutineAttack());
    }

    // used to change the round in combat
    public void ChangeRound(int changeWinVal)
    {
        enemyRef.StopEnemyAttack();
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
            if (wins >= 2)
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

    // season balance changes
    void SeasonBalancing()
    {
        if (dayCount >= seasonLength)
        {
            // new season
            seasonLength += startingSeasonLength;
            for (int i = 0; i < listOfClasses.Count; i++)
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
        if (buffOrNerf == 1)
        {
            // buff
            int rando = UnityEngine.Random.Range(0, 5);
            if (rando == 0)
            {
                whichCharacter.baseHealth = (int)(whichCharacter.baseHealth * UnityEngine.Random.Range(1.1f, 1.3f));
            }
            if (rando == 1)
            {
                whichCharacter.baseAttack = (int)(whichCharacter.baseAttack * UnityEngine.Random.Range(1.1f, 1.3f));
            }
            if (rando == 2)
            {
                whichCharacter.baseAttackSpeed = (whichCharacter.baseAttackSpeed * UnityEngine.Random.Range(.7f, .9f));
            }
            if (rando == 3)
            {
                whichCharacter.baseDefense = (int)(whichCharacter.baseDefense * UnityEngine.Random.Range(1.1f, 1.3f));
            }
            if (rando == 4)
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
