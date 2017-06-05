using System.Threading.Tasks;

namespace Steward.Ai.Microsoft.QnAMaker
{
    internal interface IQnAMakerService
    {
        Task<QnaMakerResult> SearchKbAsync(string message);
    }
}
