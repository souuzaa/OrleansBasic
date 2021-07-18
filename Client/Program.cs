using GrainInterfaces;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using System;
using System.Threading.Tasks;

namespace Client
{
    public class Program
    {
        static int Main(string[] args)
        {
            return RunMainAsync().Result;
        }

        private static async Task<int> RunMainAsync()
        {
            try
            {
                using (var client = await ConnectClient())
                {
                    await DoClientWork(client);
                    Console.ReadKey();
                }

                return 0;
            }
            catch (Exception e)
            {
                Console.WriteLine($"\nException while trying to run client: {e.Message}");
                Console.WriteLine("Make sure the silo the client is trying to connect to is running.");
                Console.WriteLine("\nPress any key to exit.");
                Console.ReadKey();
                return 1;
            }
        }

        private static async Task<IClusterClient> ConnectClient()
        {
            IClusterClient client;
            client = new ClientBuilder()
                .UseLocalhostClustering()
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "dev";
                    options.ServiceId = "OrleansBasics";
                })
                .ConfigureLogging(logging => logging.AddConsole())
                .Build();

            await client.Connect();
            Console.WriteLine("Client successfully connected to silo host \n");
            return client;
        }

        private static async Task DoClientWork(IClusterClient client)
        {
            // example of calling grains from the initialized client
            var friend = client.GetGrain<IEvenOrOdd>(1);
            var notFriend = client.GetGrain<IEvenOrOdd>(2);

            Random random = new Random();

            //friend
            Console.WriteLine($"\n\n{await friend.Discovery(random.Next(100))}\n\n");
            await friend.Visit();
            Console.WriteLine($"\n\n{await friend.Discovery(random.Next(100))}\n\n");
            await friend.Visit();
            Console.WriteLine($"\n\n{await friend.Discovery(random.Next(100))}\n\n");
            await friend.Visit();

            //not friend
            Console.WriteLine($"\n\n{await notFriend.Discovery(random.Next(100))}\n\n");
            await notFriend.Visit();
            Console.WriteLine($"\n\n{await notFriend.Discovery(random.Next(100))}\n\n");
            await notFriend.Visit();

            //stats
            Console.WriteLine($"{Environment.NewLine}-----{Environment.NewLine}");
            await PrettyPrintGrainVisits(friend);
            await PrettyPrintGrainVisits(notFriend);
        }

        private static async Task PrettyPrintGrainVisits(IEvenOrOdd grain)
        {
            Console.WriteLine($"{grain.GetPrimaryKeyString()} has visited {await grain.GetNumberOfVisits()} times");
        }
    }
}
