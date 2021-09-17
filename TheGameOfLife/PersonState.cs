namespace TheGameOfLife
{
    public class PersonState
    {
        public bool IsAlive { get; set; }

        public PersonState(bool alive)
        {
            IsAlive = alive;
        }

        public override string ToString()
        {
            return IsAlive ? "*" : ".";
        }
    }
}