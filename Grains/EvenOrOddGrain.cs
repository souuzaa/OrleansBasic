using System;
using System.Threading.Tasks;
using GrainInterfaces;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Providers;

namespace Grains
{
    [StorageProvider(ProviderName = Constants.OrleansMemoryProvider)]
    public class EvenOrOddGrain : Orleans.Grain<VisitTrackerState>, IEvenOrOdd
    {
        private readonly ILogger logger;

        public EvenOrOddGrain(ILogger<EvenOrOddGrain> logger)
        {
            this.logger = logger;
        }

        public Task<int> GetNumberOfVisits()
        {
            return Task.FromResult(State.NumberOfVisits);
        }

        public async Task Visit()
        {
            var now = DateTime.Now;
            if (!State.FirstVisit.HasValue)
            {
                State.FirstVisit = now;
            }
            State.NumberOfVisits++;
            State.LastVisit = now;
            await WriteStateAsync();
        }

        Task<string> IEvenOrOdd.Discovery(int number)
        {
            logger.LogInformation($"\n Even or Odd message received: number = '{number}'");
            return Task.FromResult($"\n '{number}' is {(number%2 == 0 ? "Even" : "Odd")} - {this.GetGrainIdentity()}");
        }
    }

    public class VisitTrackerState
    {
        public DateTime? FirstVisit { get; set; }
        public DateTime? LastVisit { get; set; }
        public int NumberOfVisits { get; set; }
    }
}
