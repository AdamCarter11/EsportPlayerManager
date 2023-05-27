using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum passiveAbility
{
    increaseDamagePerAttack,
    StrongerLessAllies
}

[CreateAssetMenu(fileName = "PassiveAbility", menuName = "Abilities/PassiveAbility")]

public class PassiveAbility : ScriptableObject
{
    public string abilityName;
    [TextArea(4, 10)]
    public string description;
    // create a list of bools (check boxes) that allow for us to select what passives we want on the SO
    public passiveAbility passiveEffect;

}
