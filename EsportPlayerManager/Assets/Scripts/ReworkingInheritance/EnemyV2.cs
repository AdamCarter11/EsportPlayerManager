using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyV2 : BaseCharacter
{
    // reference
    //public CharacterClass currClass;
    private GameObject playerRef;

    // checks
    bool enemyStartCombat = false;
    bool resetHealth = false;

    private void Start()
    {
        playerRef = GameObject.FindGameObjectWithTag("MainCharacter");
    }

    private void Update()
    {
        if (enemyStartCombat)
        {
            enemyStartCombat = false;
            resetHealth = true;
            //print("Starting combat, player health: " + playerRef.getCurrentCharacter().tempHealth);
            print("enemy started combat");
            //startCombat = false;
            StartCoroutine(CoroutineAttack());
        }

        if (gameObject.GetComponent<BaseCharacter>().charClass.tempHealth <= 0 && resetHealth)
        {
            print("character death");
            //swapCharacters();
        }
    }
    IEnumerator CoroutineAttack()
    {
        while (true)
        {
            yield return new WaitForSeconds(gameObject.GetComponent<BaseCharacter>().charClass.baseAttackSpeed);
            AttackTrigger(playerRef);
        }
    }
    public void StopEnemyAttack()
    {
        StopCoroutine(CoroutineAttack());
    }
    public void SwitchEnemyHelper()
    {
        enemyStartCombat = true;
    }
    public void TakeDamageHelper(int tempDamage)
    {
        TakeDamage(tempDamage);
    }
    /*
    public CharacterClass getCurrentCharacter()
    {
        return currClass;
    }
    */
}
