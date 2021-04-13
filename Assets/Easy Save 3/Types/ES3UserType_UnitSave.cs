using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("UnitPosition", "UnitRotation", "CurrentHealth", "IsMelee", "UnitID")]
	public class ES3UserType_UnitSave : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_UnitSave() : base(typeof(UnitSave)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (UnitSave)obj;
			
			writer.WriteProperty("UnitPosition", instance.UnitPosition, ES3Type_Vector3.Instance);
			writer.WriteProperty("UnitRotation", instance.UnitRotation, ES3Type_Quaternion.Instance);
			writer.WriteProperty("CurrentHealth", instance.CurrentHealth, ES3Type_float.Instance);
			writer.WriteProperty("IsMelee", instance.IsMelee, ES3Type_bool.Instance);
			writer.WriteProperty("UnitID", instance.UnitID, ES3Type_int.Instance);
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (UnitSave)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "UnitPosition":
						instance.UnitPosition = reader.Read<UnityEngine.Vector3>(ES3Type_Vector3.Instance);
						break;
					case "UnitRotation":
						instance.UnitRotation = reader.Read<UnityEngine.Quaternion>(ES3Type_Quaternion.Instance);
						break;
					case "CurrentHealth":
						instance.CurrentHealth = reader.Read<System.Single>(ES3Type_float.Instance);
						break;
					case "IsMelee":
						instance.IsMelee = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "UnitID":
						instance.UnitID = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					default:
						reader.Skip();
						break;
				}
			}
		}

		protected override object ReadObject<T>(ES3Reader reader)
		{
			var instance = new UnitSave();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_UnitSaveArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_UnitSaveArray() : base(typeof(UnitSave[]), ES3UserType_UnitSave.Instance)
		{
			Instance = this;
		}
	}
}