using System.Collections.Generic;

public class SaveGame
{
    public string SaveGameName { get; set; }
    public List<BuildingSave> AllBuildings { get; set; }
    public List<CitizenSave> AllCitizens { get; set; }
    public int AmountWood { get; set; }
    public int AmountStone { get; set; }
    public int AmountGold { get; set; }
    public int AmountHappiness { get; set; }
}
