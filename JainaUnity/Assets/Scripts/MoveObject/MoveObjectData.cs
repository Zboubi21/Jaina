public class MoveObjectData
{

    bool isRunning = false;
    // public bool IsRunning { get => IsRunning1; set => IsRunning1 = value; }
    public bool IsRunning
    {
        get
        {
            return isRunning;
        }

        set
        {
            isRunning = value;
        }
    }

    float fracJourney = 0; // Always between 0 and 1
    // public float FracJourney { get => fracJourney; set => fracJourney = value; }
    public float FracJourney
    {
        get
        {
            return fracJourney;
        }

        set
        {
            fracJourney = value;
        }
    }

}
