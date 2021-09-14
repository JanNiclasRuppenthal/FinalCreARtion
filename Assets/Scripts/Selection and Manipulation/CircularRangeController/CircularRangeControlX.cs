using System;
using UnityEngine;

/*
 * A subclass of CircularRange.
 * This class overrides the method rotate()
 */
public class CircularRangeControlX : CircularRange
{
    public CircularRangeControlY cY;
    public CircularRangeControlZ cZ;

    // rotates with the currentValue of the first circular slider around the x-axis
    public override void rotate()
    {
        foreach (GameObject objects in listofObjects)
        {
            objects.transform.localEulerAngles = new Vector3(CurrentValue, cY.getCurrentValue(), cZ.getCurrentValue());
        }
    }
}