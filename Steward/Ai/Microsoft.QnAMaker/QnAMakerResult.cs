using Newtonsoft.Json;

namespace Steward.Ai.Microsoft.QnAMaker
{
    public class QnaMakerQuestion
    {
        /// <summary>
        /// The top answer found in the QnA Service.
        /// </summary>
        [JsonProperty(PropertyName = "question")]
        public string Question { get; set; }
    }

    public class QnaMakerResult
    {
        /// <summary>
        /// The top answer found in the QnA Service.
        /// </summary>
        [JsonProperty(PropertyName = "answer")]
        public string Answer { get; set; }

        /// <summary>
        /// The score in range [0, 100] corresponding to the top answer found in the QnA    Service.
        /// </summary>
        [JsonProperty(PropertyName = "score")]
        public double Score { get; set; }
    }
}