using System.Threading.Tasks;

namespace Steward.Actor
{
    internal interface IActor
    {
        Task Execute();
    }
}
