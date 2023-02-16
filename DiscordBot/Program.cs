using System;
using System.Linq;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;

//bot token:
//OTczOTc0MDk5NTQ4MzA3NTAw.GNcc8z.a1z74l12yhL-9uea-Jv81XJx35nfJvFfb699zM

namespace DiscordBot { 
    internal class Program {
        static Random random = new Random();
        static int questionsTillRespond;
        static void Main(string[] args) {
            questionsTillRespond = random.Next(1,10);
            MainAsync().GetAwaiter().GetResult();
        }

        static bool quit = false;

        static async Task MainAsync() {
            DiscordClient discord = new DiscordClient(new DiscordConfiguration() { 
                Token = @"", //fill in token
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.AllUnprivileged
            });

            discord.MessageCreated += Discord_MessageCreated;

            await discord.ConnectAsync();

            while (!quit) { 
                await Task.Delay(100);
            }
            Console.WriteLine("Stopping...");
            await discord.DisconnectAsync();
            Console.WriteLine("Stopped");

        }

        private static async Task Discord_MessageCreated(DiscordClient sender, DSharpPlus.EventArgs.MessageCreateEventArgs e) {
            
            if (e.Message.Content.ToLower() == @"!id") {
                DiscordMessage response = e.Message.RespondAsync(e.Author.Id.ToString()).Result;
                await Task.Delay(5000);
                e.Message.DeleteAsync();
                response.DeleteAsync();
                return;
            }


            if (e.Message.Content.ToLower() == @"!stop") {
                await e.Message.RespondAsync("Stopping");
                quit = true;
                return;
            }

            if (e.Message.Content.ToLower() == @"!clone") {
                await e.Channel.CloneAsync("bot reasons");
                return;
            }

            if (e.Message.Content.ToLower().Contains("?")) {
                questionsTillRespond--;
                if (questionsTillRespond == 0) {
                    questionsTillRespond = random.Next(2, 10);
                } else return;

                string response = "";
                int choice = random.Next(5);
                switch (choice) { 
                    case 0: response = "Yes"; break;
                    case 1: response = "No"; break;
                    case 2: response = "Maybe..."; break;
                    case 3: response = @":grimacing:"; break;
                    case 4: response = @":face_with_raised_eyebrow:"; break;
                }
                await e.Message.RespondAsync(response);
            }

            if (e.Author.Id == 0) { //me 
                if (random.Next(0,10) != 0) {
                    return;
                }

                string response = "";
                int choice = random.Next(4);
                switch (choice) { 
                    case 0: response = @"Hi"; break;
                    case 1: response = @"Hey"; break;
                    case 2: response = @"Hello"; break;
                    case 3: response = @"Good to see you"; break;
                }
                await e.Message.RespondAsync(response);
            }



        }

    }
}
