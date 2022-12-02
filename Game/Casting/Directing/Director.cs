using System;
using System.Collections.Generic;
using Unit04.Game.Casting;
using Unit04.Game.Services;


namespace Unit04.Game.Directing
{
    /// <summary>
    /// <para>A person who directs the game.</para>
    /// <para>
    /// The responsibility of a Director is to control the sequence of play.
    /// </para>
    /// </summary>
    public class Director
    {
        public int score = 0;
        public int lives = 3;
        private KeyboardService keyboardService = null;
        private VideoService videoService = null;

        /// <summary>
        /// Constructs a new instance of Director using the given KeyboardService and VideoService.
        /// </summary>
        /// <param name="keyboardService">The given KeyboardService.</param>
        /// <param name="videoService">The given VideoService.</param>
        public Director(KeyboardService keyboardService, VideoService videoService)
        {
            this.keyboardService = keyboardService;
            this.videoService = videoService;
        }

        /// <summary>
        /// Starts the game by running the main game loop for the given cast.
        /// </summary>
        /// <param name="cast">The given cast.</param>
        public void StartGame(Cast cast)
        {
            videoService.OpenWindow();
            while (videoService.IsWindowOpen() && lives > 0)
            {
                GetInputs(cast);
                DoUpdates(cast);
                DoOutputs(cast);
            }


            videoService.CloseWindow();
        }

        /// <summary>
        /// Gets directional input from the keyboard and applies it to the robot.
        /// </summary>
        /// <param name="cast">The given cast.</param>
        private void GetInputs(Cast cast)
        {
            List<Actor> artifacts = cast.GetActors("artifacts");
            List<Actor> projectiles = cast.GetActors("projectiles");

            foreach (Actor actor in artifacts) 
            {
                Point artifactvelocity = keyboardService.MoveArtifactDown();
                actor.SetVelocity(artifactvelocity);
                int maxX = videoService.GetWidth();
                int maxY = videoService.GetHeight();
                actor.MoveNext(maxX, maxY);
            }

            foreach (Actor actor in projectiles) 
            {
                Point artifactvelocity = keyboardService.MoveArtifactUp();
                actor.SetVelocity(artifactvelocity);
                int maxX = videoService.GetWidth();
                int maxY = videoService.GetHeight();
                actor.MoveNext(maxX, maxY);
            }

            Actor robot = cast.GetFirstActor("robot");
            Point velocity = keyboardService.GetDirection();
            robot.SetVelocity(velocity);    
            keyboardService.FireProjectile(robot,cast);
        }

        /// <summary>
        /// Updates the robot's position and resolves any collisions with artifacts.
        /// </summary>
        /// <param name="cast">The given cast.</param>
        private void DoUpdates(Cast cast)
        {
            Actor banner = cast.GetFirstActor("banner");
            Actor robot = cast.GetFirstActor("robot");
            List<Actor> projectiles = cast.GetActors("projectiles");
            List<Actor> artifacts = cast.GetActors("artifacts");

            banner.SetText($"Score: {score.ToString()}     Lives: {lives.ToString()}");
            int maxX = videoService.GetWidth();
            int maxY = videoService.GetHeight();
            robot.MoveNext(maxX, maxY);

            Random random = new Random();

            foreach (Actor actor in artifacts)
            {
                if (robot.GetPosition().Equals(actor.GetPosition()))
                {
                    Artifact artifact = (Artifact) actor;
                    lives = lives -1;
                    banner.SetText($"Score: {score.ToString()}");
                    int x = random.Next(1, 60);
                    int y = 0;
                    Point position = new Point(x, y);
                    position = position.Scale(15);

                    artifact.SetPosition(position);
                }
            } 

            foreach (Actor projectile in projectiles)
            {
                foreach (Actor artifact in artifacts)
                {
                    int xLeft = artifact.GetPosition().GetX() - 1;
                    int xRight = artifact.GetPosition().GetX() + 1;
                    int yArtifact = artifact.GetPosition().GetY();
                    Point left = new Point(xLeft, yArtifact );
                    Point right = new Point(xRight, yArtifact );
                    
                    if (projectile.GetPosition().Equals(artifact.GetPosition()) || projectile.GetPosition().Equals(left) || projectile.GetPosition().Equals(right))
                    {
                        Artifact artifact1 = (Artifact) artifact;
                        score += artifact1.GetScore();
                        banner.SetText($"Score: {score.ToString()}");
                        int x = random.Next(1, 60);
                        int y = 0;
                        int x1 = robot.GetPosition().GetX();
                        int y1 = robot.GetPosition().GetY();
                        Point position = new Point(x, y);
                        Point position1 = new Point(x1, y1);
                        position = position.Scale(15);
                        position1 = position.Scale(15);
                    

                        artifact.SetPosition(position);
                
                    }

                    if (projectile.GetPosition().GetY().Equals(0))
                    {
                        int x1 = robot.GetPosition().GetX();
                        int y1 = robot.GetPosition().GetY();

                        Color color = new Color(0,0,0);

                        Point position1 = new Point(0, 0);
                        projectile.SetColor(color);

                        projectile.SetPosition(position1);

                        
                    }
                }
            } 
        }

        /// <summary>
        /// Draws the actors on the screen.
        /// </summary>
        /// <param name="cast">The given cast.</param>
        public void DoOutputs(Cast cast)
        {
            List<Actor> actors = cast.GetAllActors();
            videoService.ClearBuffer();
            videoService.DrawActors(actors);
            videoService.FlushBuffer();
        }

    }
}