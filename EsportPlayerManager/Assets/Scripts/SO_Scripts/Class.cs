using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ElementalClassType
{
    Fire,
    Water,
    Grass,
    Electric,
    Rock,
    Ice,
    None
}
public enum OccupationClassType
{
    Assassin,
    Mage,
    Fighter,
    Berserker,
    Rogue,
    None
}

[CreateAssetMenu(fileName = "CharacterData", menuName = "Character/Class")]
public class Class : ScriptableObject
{
    public string className;
    [TextArea(4, 10)]
    public string description;
    // create a list of bools (check boxes) that allow for us to select what passives we want on the SO
    public ElementalClassType classTypeEnumVal;
    public OccupationClassType occupationClassTypeEnumVal;

}
