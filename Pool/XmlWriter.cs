using System.Drawing;
using System.Numerics;
using System.Xml;
using Raylib_cs;

namespace Pool
{
    public class XmlWriter
    {
        private static XmlWriter instance;
        private static readonly object MUTEX = new object();

        public static XmlWriter Instance
        {
            get
            {
                lock (MUTEX)
                {
                    if (instance == null)
                    {
                        instance = new XmlWriter();
                    }

                    return instance;
                }
            }
        }

        private XmlWriter()
        {

        }

        private void createBallPositionsFile()
        {
            try
            {
                XmlDocument document = new XmlDocument();

                XmlElement root = document.CreateElement("balls");
                document.AppendChild(root);

                document.Save(Constants.ballPositionFilePath);
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not create the balls position file.");
                Console.WriteLine(e.Message);
            }
        }

        public void addBalls(List<Ball> balls)
        {
            if (!File.Exists(Constants.ballPositionFilePath))
            {
                createBallPositionsFile();
            }

            XmlDocument document = new XmlDocument();

            try
            {
                document.Load(Constants.ballPositionFilePath);
                XmlNodeList root = document.GetElementsByTagName("balls");

                foreach (Ball ball in balls)
                {
                    XmlElement entry = document.CreateElement("ball");

                    entry.SetAttribute("position_x", ball.Position.X.ToString());
                    entry.SetAttribute("position_y", ball.Position.Y.ToString());
                    entry.SetAttribute("type", ball.Type.ToString());

                    root[0].AppendChild(entry);
                }

                document.Save(Constants.ballPositionFilePath);
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not add balls to the file.");
                Console.WriteLine(e.Message);
            }
        }

        public List<Ball>? loadBalls()
        {
            if (!File.Exists(Constants.ballPositionFilePath))
            {
                Console.WriteLine("File does not exist.");
                return null;
            }

            XmlDocument document = new XmlDocument();
            List<Ball> returnList = new List<Ball>();

            try
            {
                document.Load(Constants.ballPositionFilePath);
                XmlNodeList root = document.GetElementsByTagName("ball");

                foreach (XmlNode ball in root)
                {
                    Raylib_cs.Color col = new Raylib_cs.Color();

                    switch (ball.Attributes["type"].Value)
                    {
                        case "RED":
                        {
                            col = Raylib_cs.Color.Maroon;
                        }
                        break;

                        case "YELLOW":
                        {
                            col = Raylib_cs.Color.Yellow;
                        }
                        break;

                        case "BLACK":
                        {
                            col = Raylib_cs.Color.Black;
                        }
                        break;

                        case "WHITE":
                        {
                            col = Raylib_cs.Color.White;
                        }
                        break;
                    }

                    BallType type = new BallType();

                    if (!Enum.TryParse(ball.Attributes["type"].Value, out type))
                    {
                        Console.WriteLine("Ball Type Mismatch");
                        return null;
                    }

                    Ball b = new Ball
                    (
                        new Vector2(float.Parse(ball.Attributes["position_x"].Value), float.Parse(ball.Attributes["position_y"].Value)),
                        new Vector2(0, 0),
                        Constants.ballRadius, 
                        Constants.ballMass,
                        col,
                        type
                    );

                    returnList.Add(b);
                }

                document.Save(Constants.ballPositionFilePath);

                return returnList;
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not add balls to the file.");
                Console.WriteLine(e.Message);
                return null;
            }
        }
    }
}