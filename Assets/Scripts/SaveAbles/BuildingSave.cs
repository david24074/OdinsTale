using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingSave
{
    //Used for loading in the right building
    private string BuildingName { get; set; }
    private Vector3 BuildingPosition { get; set; }
    private Quaternion BuildingRotation { get; set; }
    //Has the building been fully built or not
    private bool BuildFinished { get; set; }
    //Can either be for the build progress or for resource gathering progress
    private float Progress { get; set; }
}
