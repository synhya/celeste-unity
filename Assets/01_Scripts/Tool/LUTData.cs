using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewLUTData", menuName = "LUTData")]
public class LUTData : ScriptableObject
{
    public List<PairColor> Table = new List<PairColor>();
} 
