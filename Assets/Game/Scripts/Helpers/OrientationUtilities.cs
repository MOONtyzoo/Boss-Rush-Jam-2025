using UnityEngine;

public enum Orientation {
    RightSideUp,
    SidewaysLeft,
    SidewaysRight,
    UpsideDown
}

/// <summary>
/// <para> The purpose of this class is to provide utility functions to help reason about the orientation of objects in the code. </para>
/// <para> The most important use-case is to get an Orientation enum and use that for logic instead of doing the math yourself. </para>
/// </summary>
public static class OrientationUtilities
{    
    /// <summary>
    /// Returns the orientation of a transform in a human-readable enum
    /// </summary>
    public static Orientation GetOrientationOfTransform(Transform transform) {
        float angle = Vector2.SignedAngle(Vector2.up, transform.up);
        return GetOrientationOfAngle(angle);
    }

    /// <summary>
    /// Converts a 2D direction vector to a human-readable enum which describes the orientation
    /// </summary>
    public static Orientation GetOrientationOfVector(Vector2 directionVector) {
        float angle = Vector2.SignedAngle(Vector2.up, directionVector);
        return GetOrientationOfAngle(angle);
    }

    /// <summary>
    /// Converts an angle to a human-readable enum which describes the orientation
    /// </summary>
    public static Orientation GetOrientationOfAngle(float angle) {
        float modulatedAngle = ModulateAngle(angle);
        
        // This function takes an angle (between 0 and 360) and converts it to an int (from 0 to 4)
        // representing a "snapped" orientation to 4 spots.
        int angleGroupNum = Mathf.FloorToInt((modulatedAngle + 45)/90f);

        switch(angleGroupNum) {
            case 0: return Orientation.RightSideUp;
            case 1: return Orientation.SidewaysLeft;
            case 2: return Orientation.UpsideDown;
            case 3: return Orientation.SidewaysRight;
            case 4: return Orientation.RightSideUp;
        }
        return Orientation.RightSideUp;
    }

    /// <summary>
    /// Converts an angle it to it's equivalent representation within 0-360 degrees
    /// </summary>
    public static float ModulateAngle(float angle) {
        if (angle > 0) {
            angle = angle % 360;
        } else {
            angle = 360 + -(-angle % 360);
        }
        return angle;
    }
}
