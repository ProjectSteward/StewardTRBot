using System.Collections.Generic;

namespace Steward.Ai.Watson.Conversation
{
    internal interface IConversationData
    {
        dynamic Context { get; }

        bool IsEnded();

        bool IsHandled();

        IEnumerable<object> OutputText { get; }

        bool IsAskingForOwner();

        bool IsAskingForServiceCloudCase();

        object this[string name] { get; }
    }
}
