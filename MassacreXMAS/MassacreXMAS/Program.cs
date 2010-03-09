using System;

namespace Tree
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (Massacre game = new Massacre())
            {
                game.Window.Title = "Tree Massacre XMAS Edition";
                game.Run();
            }
        }
    }
}

