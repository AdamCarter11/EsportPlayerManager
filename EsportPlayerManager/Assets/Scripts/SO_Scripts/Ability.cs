using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "Ability", menuName = "Abilities/Ability")]

public class Ability : ScriptableObject
{
    public string abilityName;
    [TextArea(4, 10)]
    public string abilityDesc;
    public int damage;
    public float manaCost;
    public float[] decreaseStats = new float[4]; // 4 possible stats
    public float[] increaseStats = new float[4]; // 4 possible stats
    public float damageOverTime;
}
