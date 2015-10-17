namespace Ethos.Data.Entities
{
    public class Player
    {
        public int Id { get; }
        public string DisplayName { get; }

        public Player(int id, string displayName)
        {
            Id = id;
            DisplayName = displayName;
        }

        protected Player()
        {
        }
    }
}