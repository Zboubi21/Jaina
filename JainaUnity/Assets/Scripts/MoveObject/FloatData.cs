public class FloatData : MoveObjectData
{

    float floatValue = 0;
    // public float FloatValue { get => FloatValue1; set => FloatValue1 = value; }

    public float FloatValue
    {
        get
        {
            return floatValue;
        }

        set
        {
            floatValue = value;
        }
    }
}
