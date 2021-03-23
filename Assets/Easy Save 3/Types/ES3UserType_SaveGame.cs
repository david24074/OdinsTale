using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("SaveGameName", "AllBuildings", "AllCitizens", "AllJobs", "AmountWood", "AmountStone", "AmountGold", "AmountFood", "AmountHappiness", "Day", "Year")]
	public class ES3UserType_SaveGame : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_SaveGame() : base(typeof(SaveGame)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (SaveGame)obj;
			
			writer.WriteProperty("SaveGameName", instance.SaveGameName, ES3Type_string.Instance);
			writer.WriteProperty("AllBuildings", instance.AllBuildings);
			writer.WriteProperty("AllCitizens", instance.AllCitizens);
			writer.WriteProperty("AllJobs", instance.AllJobs);
			writer.WriteProperty("AmountWood", instance.AmountWood, ES3Type_int.Instance);
			writer.WriteProperty("AmountStone", instance.AmountStone, ES3Type_int.Instance);
			writer.WriteProperty("AmountGold", instance.AmountGold, ES3Type_int.Instance);
			writer.WriteProperty("AmountFood", instance.AmountFood, ES3Type_int.Instance);
			writer.WriteProperty("AmountHappiness", instance.AmountHappiness, ES3Type_float.Instance);
			writer.WriteProperty("Day", instance.Day, ES3Type_int.Instance);
			writer.WriteProperty("Year", instance.Year, ES3Type_int.Instance);
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (SaveGame)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "SaveGameName":
						instance.SaveGameName = reader.Read<System.String>(ES3Type_string.Instance);
						break;
					case "AllBuildings":
						instance.AllBuildings = reader.Read<System.Collections.Generic.List<BuildingSave>>();
						break;
					case "AllCitizens":
						instance.AllCitizens = reader.Read<System.Collections.Generic.List<CitizenSave>>();
						break;
					case "AllJobs":
						instance.AllJobs = reader.Read<System.Collections.Generic.List<System.String>>();
						break;
					case "AmountWood":
						instance.AmountWood = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "AmountStone":
						instance.AmountStone = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "AmountGold":
						instance.AmountGold = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "AmountFood":
						instance.AmountFood = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "AmountHappiness":
						instance.AmountHappiness = reader.Read<System.Single>(ES3Type_float.Instance);
						break;
					case "Day":
						instance.Day = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "Year":
						instance.Year = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					default:
						reader.Skip();
						break;
				}
			}
		}

		protected override object ReadObject<T>(ES3Reader reader)
		{
			var instance = new SaveGame();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_SaveGameArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_SaveGameArray() : base(typeof(SaveGame[]), ES3UserType_SaveGame.Instance)
		{
			Instance = this;
		}
	}
}