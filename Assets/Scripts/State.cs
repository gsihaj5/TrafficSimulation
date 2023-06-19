using System;

public class State
{
    public float vin { get; }
    public float vout { get; }

    public State(float vin, float vout)
    {
        this.vin = vin;
        this.vout = vout;
    }

    public override bool Equals(object obj)
    {
        // Check if two states are equal based on their values
        if (obj is State other)
        {
            return Math.Abs(vin - other.vin) < .01f && Math.Abs(vout - other.vout) < .01f;
        }

        return false;
    }
    public float[] getArray()
    {
        return new[] { vin, vout };
    }
}
