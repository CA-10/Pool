using Raylib_cs;
using Pool;
using System.Numerics;
using System.Net;

Raylib.SetConfigFlags(ConfigFlags.Msaa4xHint);
Raylib.InitWindow(Constants.screenWidth, Constants.screenHeight, "Pool");
Raylib.SetTargetFPS(80);

BallListHandler ballHandler;
EnvState environmentState;

Color devSelectedColour;
BallType devBallType;

float shotPower;
bool startedShot;

Vector2 ballDirection;
Vector2 endPoint;

bool areBallsMoving;
bool placedWhiteBall;

setup();

while (!Raylib.WindowShouldClose())
{
    Raylib.BeginDrawing();
    Raylib.ClearBackground(Color.DarkBrown);

    if (environmentState == EnvState.GAMEPLAY)
    {
        update();
        render();
    }
    else if (environmentState == EnvState.DEVELOPER)
    {
        devUpdate();
        devRender();
    }

    Raylib.EndDrawing();
}

void setup()
{
    Pockets.Instance.addPocket(new Vector2(Constants.tableWidth, Constants.tableWidth));
    Pockets.Instance.addPocket(new Vector2(Constants.tableWidth, Constants.screenHeight - Constants.tableWidth));
    Pockets.Instance.addPocket(new Vector2(Constants.screenWidth - Constants.tableWidth, Constants.tableWidth));
    Pockets.Instance.addPocket(new Vector2(Constants.screenWidth - Constants.tableWidth, Constants.screenHeight - Constants.tableWidth));
    Pockets.Instance.addPocket(new Vector2(Constants.screenWidth / 2, Constants.screenHeight - Constants.tableWidth + 15));
    Pockets.Instance.addPocket(new Vector2(Constants.screenWidth / 2, Constants.tableWidth - 15));

    shotPower = 0;
    startedShot = false;
    devSelectedColour = Color.Maroon;
    devBallType = BallType.RED;

    environmentState = EnvState.GAMEPLAY;
    ballHandler = BallListHandler.Instance;

    ballDirection = new Vector2();
    endPoint = new Vector2();

    areBallsMoving = false;
    placedWhiteBall = true;

    if (environmentState == EnvState.GAMEPLAY)
    {
        List<Ball>? loadedBalls = XmlWriter.Instance.loadBalls();

        if (loadedBalls != null)
        {
            ballHandler.Balls = loadedBalls;
        }      
    }
}

void update()
{
    handleInput();
    ballHandler.update();

    if (ballHandler.Balls[ballHandler.CueBallIndex].Hidden && !areBallsMoving)
    {
        placedWhiteBall = false;
        ballHandler.Balls[ballHandler.CueBallIndex].Hidden = false;
    }
}

void handleInput()
{
    areBallsMoving = ballHandler.areBallsMoving();
    ballDirection = VectorOperations.unitVector(ballHandler.Balls[ballHandler.CueBallIndex].Position - Raylib.GetMousePosition());

    if (!areBallsMoving && placedWhiteBall)
    {
        endPoint = ballHandler.Balls[ballHandler.CueBallIndex].Position + ballDirection * 1000;

        if (Raylib.IsMouseButtonDown(0))
        {
            if (shotPower <= Constants.maxShotPower)
            {
                startedShot = true;
                shotPower += 500 * Raylib.GetFrameTime();
            }
        }
    }

    if (Raylib.IsMouseButtonReleased(0) && startedShot && placedWhiteBall)
    {
        startedShot = false;
        ballHandler.Balls[ballHandler.CueBallIndex].Velocity = ballDirection * shotPower;
        shotPower = 0;
    }

    if (!placedWhiteBall)
    {
        if (Raylib.GetMouseY() >= Constants.tableWidth && Raylib.GetMouseY() <= Constants.screenHeight - Constants.tableWidth)
        {
            ballHandler.Balls[ballHandler.CueBallIndex].Position = new Vector2(ballHandler.Balls[ballHandler.CueBallIndex].Position.X, Raylib.GetMouseY());
        }
        
        if (Raylib.IsMouseButtonPressed(MouseButton.Right))
        {
            placedWhiteBall = true;
            ballHandler.Balls[ballHandler.CueBallIndex].Velocity = new Vector2(0, 0);
            return;
        }
    }
}

void render()
{
    renderTable();

    if (!areBallsMoving && placedWhiteBall)
    {
        Raylib.DrawLine(Raylib.GetMouseX(), Raylib.GetMouseY(), (int)ballHandler.Balls[ballHandler.CueBallIndex].Position.X, (int)ballHandler.Balls[ballHandler.CueBallIndex].Position.Y, Color.Black);
        Raylib.DrawLine((int)ballHandler.Balls[ballHandler.CueBallIndex].Position.X, (int)ballHandler.Balls[ballHandler.CueBallIndex].Position.Y, (int)endPoint.X, (int)endPoint.Y, Color.Black);
    }

    ballHandler.render();

    Raylib.DrawRectangleLines(Constants.tableWidth + 100, Constants.tableWidth - 60, 100, 50, Color.Black);
    Raylib.DrawRectangle(Constants.tableWidth + 100, Constants.tableWidth - 60, (int)(shotPower / (Constants.maxShotPower / 100)), 50, Color.Red);
}

void renderTable()
{
    Raylib.DrawRectangleLines(0, 0, Constants.screenWidth, Constants.screenHeight, Color.Black);

    Raylib.DrawRectangle(Constants.tableWidth, Constants.tableWidth, Constants.screenWidth - Constants.tableWidth * 2, Constants.screenHeight - Constants.tableWidth * 2, Color.Lime);
    Raylib.DrawRectangleLines(Constants.tableWidth, Constants.tableWidth, Constants.screenWidth - Constants.tableWidth * 2, Constants.screenHeight - Constants.tableWidth * 2, Color.Black);

    Raylib.DrawLine(300, Constants.tableWidth, 300, Constants.screenHeight - Constants.tableWidth, Color.White);
    
    foreach (Vector2 pocket in Pockets.Instance.pocketPositions)
    {
        Raylib.DrawCircle((int)pocket.X, (int)pocket.Y, Pockets.Instance.pocketRadius + 5, Color.Black);
    }
}

void devUpdate()
{
    if (Raylib.IsKeyPressed(KeyboardKey.R))
    {
        devSelectedColour = Color.Maroon;
        devBallType = BallType.RED;
    }
    else if (Raylib.IsKeyPressed(KeyboardKey.Y))
    {
        devSelectedColour = Color.Yellow;
        devBallType = BallType.YELLOW;
    }
    else if (Raylib.IsKeyPressed(KeyboardKey.W))
    {
        devSelectedColour = Color.White;
        devBallType = BallType.WHITE;
    }
    else if (Raylib.IsKeyPressed(KeyboardKey.B))
    {
        devSelectedColour = Color.Black;
        devBallType = BallType.BLACK;
    }
    else if (Raylib.IsKeyPressed(KeyboardKey.S))
    {
        XmlWriter.Instance.addBalls(ballHandler.Balls);
    }
    else if (Raylib.IsMouseButtonPressed(0))
    {
        ballHandler.addBall(new Ball(Raylib.GetMousePosition(), new Vector2(0, 0), Constants.ballRadius, Constants.ballMass, devSelectedColour, devBallType));
    }
}

void devRender()
{
    Raylib.DrawCircle(Raylib.GetMouseX(), Raylib.GetMouseY(), Constants.ballRadius, devSelectedColour);
    ballHandler.render();
}