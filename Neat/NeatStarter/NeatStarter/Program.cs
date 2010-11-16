using System;

namespace NeatStarter
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (NeatStarterGame game = new NeatStarterGame())
            {
                game.Run();
            }
        }
    }
#endif
}

