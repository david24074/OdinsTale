using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveGame
{
    private string SaveGameName { get; set; }
    private List<BuildingSave> AllBuildings = new List<BuildingSave>();
    private List<CitizenSave> AllCitizens = new List<CitizenSave>();
    private int AmountWood { get; set; }
    private int AmountStone { get; set; }
    private int AmountGold { get; set; }
    private int AmountHappiness { get; set; }
}
