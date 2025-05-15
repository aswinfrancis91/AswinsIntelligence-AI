using Microsoft.SemanticKernel.ChatCompletion;

namespace AswinsIntelligence.Services;

public interface IConversationService
{
    ChatHistory GetOrCreateConversation(string userId);
    void ResetConversation(string userId);
}
