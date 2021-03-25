using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("BuildingName", "BuildingID", "BuildingPosition", "BuildingRotation", "BuildFinished", "Progress")]
	public class ES3UserType_BuildingSave : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_BuildingSave() : base(typeof(BuildingSave)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (BuildingSave)obj;
			
			writer.WriteProperty("BuildingName", instance.BuildingName, ES3Type_string.Instance);
			writer.WriteProperty("BuildingID", instance.BuildingID, ES3Type_int.Instance);
			writer.WriteProperty("BuildingPosition", instance.BuildingPosition, ES3Type_Vector3.Instance);
			writer.WriteProperty("BuildingRotation", instance.BuildingRotation, ES3Type_Quaternion.Instance);
			writer.WriteProperty("BuildFinished", instance.BuildFinished, ES3Type_bool.Instance);
			writer.WriteProperty("Progress", instance.Progress, ES3Type_float.Instance);
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (BuildingSave)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "BuildingName":
						instance.BuildingName = reader.Read<System.String>(ES3Type_string.Instance);
						break;
					case "BuildingID":
						instance.BuildingID = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "BuildingPosition":
						instance.BuildingPosition = reader.Read<UnityEngine.Vector3>(ES3Type_Vector3.Instance);
						break;
					case "BuildingRotation":
						instance.BuildingRotation = reader.Read<UnityEngine.Quaternion>(ES3Type_Quaternion.Instance);
						break;
					case "BuildFinished":
						instance.BuildFinished = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "Progress":
						instance.Progress = reader.Read<System.Single>(ES3Type_float.Instance);
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