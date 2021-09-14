using System;
using UnityEngine;

/*
 * A subclass of CircularRange.
 * This class overrides the method rotate()
 */
public class CircularRangeControlY : CircularRange
{
    public CircularRangeControlX cX;
    public CircularRangeControlZ cZ;

    // rotates with the currentValue of the second circular slider around the y-axis
    public override void rotate()
    {
        foreach (GameObject objects in listofObjects)
        {
            objects.transform.eulerAngles = new Vector3(cX.getCurrentValue(), CurrentValue, cZ.getCurrentValue());
        }
    }
}