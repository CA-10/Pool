using Raylib_cs;
using System.Diagnostics;
using System.Numerics;

namespace Pool
{
    public class BallListHandler
    {
        private List<Ball> balls;
        private static BallListHandler instance;
        private static readonly object MUTEX = new object();
        private int cueBallIndex = 0;

        public static BallListHandler Instance
        {
            get
            {
                lock (MUTEX)
                {
                    if (instance == null)
                    {
                        instance = new BallListHandler();
                    }

                    return instance;
                }
            }
        }

        public List<Ball> Balls { get => balls; set => balls = value; }
        public int CueBallIndex { get => cueBallIndex; set => cueBallIndex = value; }

        private BallListHandler()
        {
            balls = new List<Ball>();
        }

        public void addBall(Ball ball)
        {
            if (ball.Type == BallType.WHITE)
            {
                cueBallIndex = balls.Count;
            }

            balls.Add(ball);
        }

        public void update()
        {
            if (balls == null) { return; }

            for (int i = 0; i < balls.Count; i++)
            {
                if (balls[i].Hidden) { continue; }

                balls[i].updatePosition();

                for (int j = i + 1; j < balls.Count; j++)
                {
                    if (balls[j].Hidden) { continue; }
                    if (i == j) { continue; }

                    if (Raylib.CheckCollisionCircles(balls[i].Position, balls[i].Radius, balls[j].Position, balls[j].Radius))
                    {
                        collisionResolution(balls[i], balls[j]);
                        collision(balls[i], balls[j]);
                    }
                }
                
                foreach (Vector2 pocket in Pockets.Instance.pocketPositions)
                {
                    if (Raylib.CheckCollisionCircles(balls[i].Position, Constants.ballRadius, pocket, Pockets.Instance.pocketRadius))
                    {
                        balls[i].Hidden = true;

                        if (i == cueBallIndex)
                        {
                            balls[i].Position = new Vector2(300, 400);
                            balls[i].Velocity = new Vector2(0, 0);
                            balls[i].Acceleration = new Vector2(0, 0);
                        }
                    }
                }

                if (balls[i].Position.X + balls[i].Radius >= Constants.screenWidth - Constants.tableWidth || balls[i].Position.X - balls[i].Radius <= Constants.tableWidth)
                {
                    balls[i].Velocity = new Vector2(balls[i].Velocity.X * -1, balls[i].Velocity.Y);
                    balls[i].applyFrictionalForce((float)Constants.frictionalForceWall);
                }

                if (balls[i].Position.Y + balls[i].Radius >= Constants.screenHeight - Constants.tableWidth || balls[i].Position.Y - balls[i].Radius <= Constants.tableWidth)
                {
                    balls[i].Velocity = new Vector2(balls[i].Velocity.X, balls[i].Velocity.Y * -1);
                    balls[i].applyFrictionalForce((float)Constants.frictionalForceWall);
                }
            }
        }

        private void collisionResolution(Ball ball1, Ball ball2)
        {
            Vector2 distance = VectorOperations.subtractVectors(ball1.Position, ball2.Position);
            double collisionDepth = ball1.Radius + ball2.Radius - VectorOperations.magnitudeVector(distance);
            Vector2 resolution = VectorOperations.multiplyScalar(VectorOperations.unitVector(distance), (float)collisionDepth / 2);

            ball1.addPositionVector(resolution);
            ball2.addPositionVector(VectorOperations.multiplyScalar(resolution, -1));
        }

        private void collision(Ball ball1, Ball ball2)
        {
            Vector2 normal = ball2.Position - ball1.Position;
            normal = VectorOperations.unitVector(normal);

            Vector2 relativeVelocity = ball2.Velocity - ball1.Velocity;

            float velocityAlongNormal = VectorOperations.dotProduct(relativeVelocity, normal);

            if (velocityAlongNormal > 0)
                return;

            float impulseScalar = -(1 + Constants.coefficientOfRestitution) * velocityAlongNormal;
            impulseScalar /= (1 / ball1.Mass) + (1 / ball2.Mass);

            Vector2 impulse = normal * impulseScalar;
            ball1.Velocity = ball1.Velocity - impulse / ball1.Mass;
            ball2.Velocity = ball2.Velocity + impulse / ball2.Mass;
        }

        public void render()
        {
            if (balls == null) { return; }

            foreach (Ball ball in balls)
            {
                ball.render();
            }
        }

        public bool areBallsMoving()
        {
            foreach (Ball ball in balls)
            {
                if (ball.Hidden) { continue; }

                if (ball.isInMotion()) { return true; }
            }

            return false;
        }
    }
}