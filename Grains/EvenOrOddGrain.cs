using System;
using System.Threading.Tasks;
using GrainInterfaces;
using Microsoft.Extensions.Logging;

namespace Grains
{
    public class EvenOrOddGrain : Orleans.Grain, IEvenOrOdd
    {
        private readonly ILogger logger;

        public EvenOrOddGrain(ILogger<HelloGrain> logger)
        {
            this.logger = logger;
        }

        Task<string> IEvenOrOdd.Discovery(int number)
        {
            logger.LogInformation($"\n Even or Odd message received: number = '{number}'");
            return Task.FromResult($"\n '{number}' is {(number%2 == 0 ? "Even" : "Odd")}");
        }
    }
}
