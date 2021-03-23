using System.Collections.Generic;

public class SaveGame
{
    public string SaveGameName { get; set; }
    public List<BuildingSave> AllBuildings { get; set; }
    public List<CitizenSave> AllCitizens { get; set; }
    public List<string> AllJobs { get; set; }
    public int AmountWood { get; set; }
    public int AmountStone { get; set; }
    public int AmountGold { get; set; }
    public int AmountFood { get; set; }
    public float AmountHappiness { get; set; }
    public int Day { get; set; }
    public int Year { get; set; }
}
