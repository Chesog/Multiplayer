namespace Net_ChMatchMaker
{
    using System;
    internal class Program
    {
        private static MatchMaker _matchMaker = new MatchMaker();
    
        public static void Main(string[] args)
        {
      
            _matchMaker.InitMatchMaker();
            //string serverPath = Directory.GetParent(Directory.GetCurrentDirectory());
      
            Console.WriteLine("Hola MatchMaker Mundo");
            Console.ReadKey();
            Console.WriteLine($"(SolutionDir)..\\..\\ServerProject\\Net_ChServer\\Net_ChServer\\bin\\Debug\\Net_ChServer.exe");
            Console.ReadKey();
        }
    }
}