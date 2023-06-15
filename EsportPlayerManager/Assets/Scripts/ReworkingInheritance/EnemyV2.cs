using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyV2 : BaseCharacter
{
    // reference
    public CharacterClass charClass;

    // checks
    bool enemyStartCombat = false;
    bool resetHealth = false;

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

        if (charClass.tempHealth <= 0 && resetHealth)
        {
            print("character death");
            swapCharacters();
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
