using System;
using UnityEngine;

/*
 * A subclass of CircularRange.
 * This class overrides the method rotate()
 */
public class CircularRangeControlZ : CircularRange
{
    public CircularRangeControlX cX;
    public CircularRangeControlY cY;

    // rotates with the currentValue of the  third circular slider around the z-axis 
    public override void rotate()
    {
        foreach (GameObject objects in listofObjects)
        {
            objects.transform.eulerAngles = new Vector3(cX.getCurrentValue(), cY.getCurrentValue(), CurrentValue);
        }
    }
}