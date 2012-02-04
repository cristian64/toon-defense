using System;

namespace ToonDefense
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (ToonDefense game = new ToonDefense())
            {
                game.Run();
            }
        }
    }
#endif
}

