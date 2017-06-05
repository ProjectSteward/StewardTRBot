using System;
using System.Collections.Generic;
using Steward.Ai.Watson.Conversation.Model;

namespace Steward.Ai.Watson.Conversation
{
    internal class ConversationData : IConversationData
    {
        private readonly MessageResponse data;

        internal ConversationData(MessageResponse data)
        {
            this.data = data;
        }

        object IConversationData.this[string name] => data.Context[name];

        dynamic IConversationData.Context => data.Context;

        IEnumerable<object> IConversationData.OutputText => data.Output.text;

        bool IConversationData.IsAskingForOwner()
        {
            return (data.Context.ask_for_owner != null) && Convert.ToBoolean(data.Context.ask_for_owner);
        }

        bool IConversationData.IsAskingForServiceCloudCase()
        {
            return (data.Context.ask_for_case != null) && Convert.ToBoolean(data.Context.ask_for_case);
        }

        bool IConversationData.IsEnded()
        {
            return !(data.Context.current_case != null && !string.IsNullOrWhiteSpace(Convert.ToString(data.Context.current_case)));
        }

        bool IConversationData.IsHandled()
        {
            var canBeHandled = data.Context.can_not_be_handled;
            return !(canBeHandled != null && Convert.ToBoolean(canBeHandled));
        }
    }
}