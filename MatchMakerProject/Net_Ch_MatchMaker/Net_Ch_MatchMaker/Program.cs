using System.Diagnostics;

namespace Net_Ch_MatchMaker
{
    using System;
    internal class Program
    {
        private static MatchMaker _matchMaker = new MatchMaker();
    
        public static void Main(string[] args)
        {
            _matchMaker.InitMatchMaker();
            while (_matchMaker.isMatchMakerRuning)
            {
                _matchMaker.Update();
            }
            //string serverPath = Path.GetFullPath("$(SolutionDir)..\\..\\ServerProject\\Net_ChServer\\Net_ChServer\\bin\\Debug\\Net_ChServer.exe");
            ////string serverPath = Directory.GetParent(Directory.GetCurrentDirectory());
            //
            //Console.WriteLine("Hola MatchMaker Mundo");
            //Console.ReadKey();
            //Console.WriteLine(" ");
            //Console.WriteLine(serverPath);
            //Console.ReadKey();

            //Process.Start(serverPath);
        }
    }
}