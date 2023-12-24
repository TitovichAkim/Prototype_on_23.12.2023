using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "SetOfLevelEditor", menuName = "ScriptableObjects/SetOfLevelEditor")]
public class SetOfLevelEditor : ScriptableObject
{
    public LandscapeSO[] landscapeSOs;
    public CharacterSO[] characterSOs;
}
