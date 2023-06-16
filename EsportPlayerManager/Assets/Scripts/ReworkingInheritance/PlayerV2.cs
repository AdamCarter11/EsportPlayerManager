using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerV2 : BaseCharacter
{
    // references
    [SerializeField] CharacterClass charClass;
    public List<CharacterClass> listOfClasses;
    private MainEnemy enemyRef;
    BanPickUI banPickRef;

    // checks
    bool startCombat = false;
    bool resetHealth = false;


    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "CombatScene")
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
            StartCoroutine(CoroutineAttack());
        }
        if (charClass.tempHealth <= 0 && resetHealth)
        {
            print("character death");
            //swapCharacters();
        }
    }
    IEnumerator CoroutineAttack()
    {
        while (true)
        {
            yield return new WaitForSeconds(charClass.baseAttackSpeed);
            AttackTrigger();
        }
    }

}
