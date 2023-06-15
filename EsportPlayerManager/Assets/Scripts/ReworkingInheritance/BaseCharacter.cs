using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseCharacter : MonoBehaviour
{
    public List<CharacterClass> charactersOnTeam;

    public void ApplyClassBonuses()
    {

    }
    private int CountOccupationalClass(OccupationClassType elementCheck)
    {
        return -1;
    }
    private int CountElementClass(ElementalClassType elementCheck)
    {
        return -1;
    }
    public void ActivateAbility()
    {

    }
    public void changeStats(CharacterClass charClassRef)
    {

    }
    public void TakeDamage(int damageToTake)
    {

    }
    public void swapCharacters()    // this function also calls the ienumerator for attack, so it probably needs an extra parameter
    {

    }
    public void AttackTrigger()
    {
        
    }
    //maybe also need a stop attacking func
}
