using Microsoft.Xna.Framework;

namespace Gridly
{
    public static class Times
    {
        public static double TotalElapsedSeconds { get; private set; }

        public static void Update(GameTime gameTime)
        {
            TotalElapsedSeconds = gameTime.ElapsedGameTime.TotalSeconds;
        }
    }
}
