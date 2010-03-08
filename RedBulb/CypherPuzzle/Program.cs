using System;

namespace CipherPuzzle
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (CipherPuzzle.Game1 game = new CipherPuzzle.Game1())
            {
                game.Run();
            }
        }
    }
}

