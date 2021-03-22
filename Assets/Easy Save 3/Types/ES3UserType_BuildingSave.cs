using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("BuildingName", "BuildingPosition", "BuildingRotation", "BuildFinished", "Progress")]
	public class ES3UserType_BuildingSave : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_BuildingSave() : base(typeof(BuildingSave)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (BuildingSave)obj;
			
			writer.WritePrivateProperty("BuildingName", instance);
			writer.WritePrivateProperty("BuildingPosition", instance);
			writer.WritePrivateProperty("BuildingRotation", instance);
			writer.WritePrivateProperty("BuildFinished", instance);
			writer.WritePrivateProperty("Progress", instance);
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (BuildingSave)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "BuildingName":
					reader.SetPrivateProperty("BuildingName", reader.Read<System.String>(), instance);
					break;
					case "BuildingPosition":
					reader.SetPrivateProperty("BuildingPosition", reader.Read<UnityEngine.Vector3>(), instance);
					break;
					case "BuildingRotation":
					reader.SetPrivateProperty("BuildingRotation", reader.Read<UnityEngine.Quaternion>(), instance);
					break;
					case "BuildFinished":
					reader.SetPrivateProperty("BuildFinished", reader.Read<System.Boolean>(), instance);
					break;
					case "Progress":
					reader.SetPrivateProperty("Progress", reader.Read<System.Single>(), instance);
					break;
					default:
						reader.Skip();
						break;
				}
			}
		}

		protected override object ReadObject<T>(ES3Reader reader)
		{
			var instance = new BuildingSave();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_BuildingSaveArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_BuildingSaveArray() : base(typeof(BuildingSave[]), ES3UserType_BuildingSave.Instance)
		{
			Instance = this;
		}
	}
}