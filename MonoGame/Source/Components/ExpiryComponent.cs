namespace MonoGame
{
    public class ExpiryComponent
    {
        public float TimeRemaining { get; set; }

        public ExpiryComponent(float timeRemaining)
        {
            TimeRemaining = timeRemaining;
        }
    }
}