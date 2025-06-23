using System.Numerics;

namespace Pool
{
    public class Pockets
    {
        private static Pockets instance;
        private static readonly object MUTEX = new object();

        public List<Vector2> pocketPositions;
        public int pocketRadius = 20;

        public static Pockets Instance
        {
            get
            {
                lock (MUTEX)
                {
                    if (instance == null)
                    {
                        instance = new Pockets();
                    }

                    return instance;
                }
            }
        }

        public List<Vector2> PocketPositions { get => pocketPositions; set => pocketPositions = value; }

        public Pockets()
        {
            pocketPositions = new List<Vector2>();
        }

        public void addPocket(Vector2 pocketPos)
        {
            pocketPositions.Add(pocketPos);
        }
    }
}