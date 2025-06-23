using Raylib_cs;
using System.Numerics;

namespace Pool
{
    public enum BallType
    {
        RED,
        YELLOW,
        BLACK,
        WHITE
    }

    public class Ball
    {
        private Vector2 position, velocity, acceleration;
        private Color colour;
        private float radius, mass;
        private double ek;
        private BallType type;
        private bool hidden;

        public Vector2 Position { get => position; set => position = value; }
        public Vector2 Velocity { get => velocity; set => velocity = value; }
        public Vector2 Acceleration { get => acceleration; set => acceleration = value; }
        public bool Hidden { get => hidden; set => hidden = value; }
        public Color Colour { get => colour; set => colour = value; }
        public BallType Type { get => type; set => type = value; }
        public float Radius { get => radius; set => radius = value; }
        public float Mass { get => mass; set => mass = value; }
        public double Ek { get => ek; set => ek = value; }

        public Ball(Vector2 pos, Vector2 vel, float rad, float mass, Color col, BallType type)
        {
            hidden = false;
            position = pos;
            velocity = vel;
            radius = rad;
            colour = col;
            this.mass = mass;
            ek = 0;
            acceleration = new Vector2(0, 0);
            this.type = type;
        }

        public void render()
        {
            if (hidden) { return; }

            Raylib.DrawCircle((int)position.X, (int)position.Y, radius, colour);
            Raylib.DrawCircleLines((int)position.X, (int)position.Y, radius, Color.Black);
        }

        public void addPositionVector(Vector2 vec)
        {
            position.X += vec.X;
            position.Y += vec.Y;
        }

        public void updatePosition()
        {
            if (hidden) { return; }

            if (Math.Abs(velocity.X) <= 0.05f)
            {
                velocity.X = 0;
                acceleration.X = 0;
            }

            if (Math.Abs(velocity.Y) <= 0.05f)
            {
                velocity.Y = 0;
                acceleration.Y = 0;
            }

            velocity.X += acceleration.X * Constants.timestep;
            velocity.Y += acceleration.Y * Constants.timestep;

            position.X += velocity.X * Constants.timestep;
            position.Y += velocity.Y * Constants.timestep;

            applyFrictionalForce((float)Constants.frictionalForceTable);
        }

        public void applyFrictionalForce(float frictionalForce)
        {
            acceleration.X = (frictionalForce * velocity.X * -1) / mass;
            acceleration.Y = (frictionalForce * velocity.Y * -1) / mass;
        }

        public void calculateEnergy()
        {
            ek = 0.5 * mass * ((velocity.X * velocity.X) + (velocity.Y * velocity.Y));
        }

        public bool isInMotion()
        {
            return !(Math.Abs(velocity.X) == 0 || Math.Abs(velocity.Y) == 0);
        }
    }
}