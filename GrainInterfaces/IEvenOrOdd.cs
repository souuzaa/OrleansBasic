using System;
using System.Threading.Tasks;

namespace GrainInterfaces
{
    public interface IEvenOrOdd : Orleans.IGrainWithIntegerKey
    {
        Task<string> Discovery(int number);
        Task<int> GetNumberOfVisits();
        Task Visit();
    }
}