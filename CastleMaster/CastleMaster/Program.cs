using CastleMaster.Errors;
using System;
namespace CastleMaster
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
#if !DEBUG
            try
            {
#endif
                using (Game game = new Game())
                {
                    game.Run();
                }
#if !DEBUG
            }
            catch (Exception e)
            {
                MrsMarple.Error("An error has occurred while running.\nA .log file was generated in the logs folder.", e, NamePreferences.DATETIME);
            }
#endif
        }
    }
#endif
}

