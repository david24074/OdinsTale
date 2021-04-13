using UnityEngine;

public class UnitSave
{
    public Vector3 UnitPosition { get; set; }
    public Quaternion UnitRotation { get; set; }
    public float CurrentHealth { get; set; }
    public bool IsMelee { get; set; }
    public int UnitID { get; set; }
}
