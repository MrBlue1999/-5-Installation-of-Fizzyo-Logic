using Microsoft.Xna.Framework;
using System;
using System.IO;
using System.Xml;
using GameFile;
namespace GameFile
{
#if WINDOWS || LINUX
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        public static void Main()
        {
            using (var game = new Game1())
                game.Run();
        }
    }
#endif
}