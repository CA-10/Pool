using System.Numerics;

namespace Pool
{
    public class VectorOperations
    {
        public static Vector2 addVectors(Vector2 vec1, Vector2 vec2)
        {
            return new Vector2(vec1.X + vec2.X, vec1.Y + vec2.Y);
        }

        public static Vector2 subtractVectors(Vector2 vec1, Vector2 vec2)
        {
            return new Vector2(vec1.X - vec2.X, vec1.Y - vec2.Y);
        }

        public static Vector2 subtractScalar(Vector2 vec, double scalar)
        {
            return new Vector2((float)(vec.X - scalar), (float)(vec.Y - scalar));
        }

        public static double magnitudeVector(Vector2 vec)
        {
            float sumSquared = (vec.X * vec.X) + (vec.Y * vec.Y);

            return Math.Sqrt(sumSquared);
        }

        public static Vector2 unitVector(Vector2 vec)
        {
            return new Vector2((float)(vec.X / magnitudeVector(vec)), (float)(vec.Y / magnitudeVector(vec)));
        }

        public static Vector2 multiplyScalar(Vector2 vec, float scalar)
        {
            return new Vector2(vec.X * scalar, vec.Y * scalar);
        }

        public static float dotProduct(Vector2 vec1, Vector2 vec2)
        {
            return (vec1.X * vec2.X) + (vec1.Y * vec2.Y);
        }
    }
}
