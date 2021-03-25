using UnityEngine;

public class BuildingSave
{
    //Used for loading in the right building
    public string BuildingName { get; set; }
    public int BuildingID { get; set; }
    public Vector3 BuildingPosition { get; set; }
    public Quaternion BuildingRotation { get; set; }
    //Has the building been fully built or not
    public bool BuildFinished { get; set; }
    //Can either be for the build progress or for resource gathering progress
    public float Progress { get; set; }
}
