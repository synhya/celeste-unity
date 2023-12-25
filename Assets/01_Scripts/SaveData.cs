using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveData
{
    public static SaveData Instance => instance ??= new SaveData();
    private static SaveData instance = null;

    public int TotalJumps = 0;
    public int TotalWallJumps = 0;
} 
