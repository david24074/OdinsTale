using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("CitizenPosition", "CitizenRotation", "CurrentJobIndex", "CurrentJobID", "CitizenID")]
	public class ES3UserType_CitizenSave : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_CitizenSave() : base(typeof(CitizenSave)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (CitizenSave)obj;
			
			writer.WriteProperty("CitizenPosition", instance.CitizenPosition, ES3Type_Vector3.Instance);
			writer.WriteProperty("CitizenRotation", instance.CitizenRotation, ES3Type_Quaternion.Instance);
			writer.WriteProperty("CurrentJobIndex", instance.CurrentJobIndex, ES3Type_int.Instance);
			writer.WriteProperty("CurrentJobID", instance.CurrentJobID, ES3Type_string.Instance);
			writer.WriteProperty("CitizenID", instance.CitizenID, ES3Type_string.Instance);
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (CitizenSave)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "CitizenPosition":
						instance.CitizenPosition = reader.Read<UnityEngine.Vector3>(ES3Type_Vector3.Instance);
						break;
					case "CitizenRotation":
						instance.CitizenRotation = reader.Read<UnityEngine.Quaternion>(ES3Type_Quaternion.Instance);
						break;
					case "CurrentJobIndex":
						instance.CurrentJobIndex = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "CurrentJobID":
						instance.CurrentJobID = reader.Read<System.String>(ES3Type_string.Instance);
						break;
					case "CitizenID":
						instance.CitizenID = reader.Read<System.String>(ES3Type_string.Instance);
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