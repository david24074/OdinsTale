using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("CitizenPosition", "CitizenRotation", "CurrentJobName")]
	public class ES3UserType_CitizenSave : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_CitizenSave() : base(typeof(CitizenSave)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (CitizenSave)obj;
			
			writer.WritePrivateProperty("CitizenPosition", instance);
			writer.WritePrivateProperty("CitizenRotation", instance);
			writer.WritePrivateProperty("CurrentJobName", instance);
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (CitizenSave)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "CitizenPosition":
					reader.SetPrivateProperty("CitizenPosition", reader.Read<UnityEngine.Vector3>(), instance);
					break;
					case "CitizenRotation":
					reader.SetPrivateProperty("CitizenRotation", reader.Read<UnityEngine.Quaternion>(), instance);
					break;
					case "CurrentJobName":
					reader.SetPrivateProperty("CurrentJobName", reader.Read<System.String>(), instance);
					break;
					default:
						reader.Skip();
						break;
				}
			}
		}

		protected override object ReadObject<T>(ES3Reader reader)
		{
			var instance = new CitizenSave();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_CitizenSaveArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_CitizenSaveArray() : base(typeof(CitizenSave[]), ES3UserType_CitizenSave.Instance)
		{
			Instance = this;
		}
	}
}