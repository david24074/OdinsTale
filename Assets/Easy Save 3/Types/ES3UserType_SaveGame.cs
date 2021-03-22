using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("SaveGameName", "AmountWood", "AmountStone", "AmountGold", "AmountHappiness")]
	public class ES3UserType_SaveGame : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_SaveGame() : base(typeof(SaveGame)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (SaveGame)obj;
			
			writer.WritePrivateProperty("SaveGameName", instance);
			writer.WritePrivateProperty("AmountWood", instance);
			writer.WritePrivateProperty("AmountStone", instance);
			writer.WritePrivateProperty("AmountGold", instance);
			writer.WritePrivateProperty("AmountHappiness", instance);
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (SaveGame)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "SaveGameName":
					reader.SetPrivateProperty("SaveGameName", reader.Read<System.String>(), instance);
					break;
					case "AmountWood":
					reader.SetPrivateProperty("AmountWood", reader.Read<System.Int32>(), instance);
					break;
					case "AmountStone":
					reader.SetPrivateProperty("AmountStone", reader.Read<System.Int32>(), instance);
					break;
					case "AmountGold":
					reader.SetPrivateProperty("AmountGold", reader.Read<System.Int32>(), instance);
					break;
					case "AmountHappiness":
					reader.SetPrivateProperty("AmountHappiness", reader.Read<System.Int32>(), instance);
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