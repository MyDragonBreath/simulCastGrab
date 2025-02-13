using simulCastGrab;
using simulCastGrab.Events.Arguments;

namespace SimulCastSample
{
    public class Program
    { 
        public static void Main(string[] args)
        {
            var twitchauth = new TwitchAuthentication("YOUR USERNAME", "YOUR AUTH TOKEN");
            var youtubekey = "YOUR YOUTUBE API KEY";
            var youtubeSearch = new YoutubeSearchInformation(youtubekey, name:"CHANNEL YOU WANNA WATCH");
            var services = new SimulCastGrab("TWITCH CHANNEL YOU WANNA WATCH", twitchauth, youtubeSearch, youtubekey);
            services.onMessage += onMessage;

            bool exit = false;
            while (!exit)
            {
                var command = Console.ReadLine();
                switch (command)
                {
                    case "/exit":
                        exit = true;
                        break;
                    default:
                        if (!string.IsNullOrEmpty(command))
                        {
                            if (command.StartsWith("/") && command.Length > 1)
                            {
                                services.Twitch?.Client.SendRawMessage(command.Substring(1));
                            }
                            else
                            {
                                Console.WriteLine($"Sending >>> {command}");
                                services.Twitch?.SendMessage(command);
                            }
                        }
                        break;
                }
            }
            services.Dispose();
        }

        public static void onMessage(MessageArgs message)
        {
            Console.Out.WriteLine($"{message.Platform} [{message.Username}] {message.Message}");
        }
    }
}
