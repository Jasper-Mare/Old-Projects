namespace FighterPilotSim {
    class Program {
        static void Main(string[] args) {
            

            Game game = new Game();
            Options.playerName = "Jasper";
            while(game.Run()) { }
        }
    }
}
