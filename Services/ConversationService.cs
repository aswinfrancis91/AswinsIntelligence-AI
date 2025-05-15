using Microsoft.SemanticKernel.ChatCompletion;

namespace AswinsIntelligence.Services;

public class ConversationService : IConversationService
{
    private readonly Dictionary<string, ChatHistory> _conversations = new();
    
    public ChatHistory GetOrCreateConversation(string userId)
    {
        if (!_conversations.TryGetValue(userId, out var history))
        {
            history = new ChatHistory();
            _conversations[userId] = history;
        }
        return history;
    }
    
    public void ResetConversation(string userId)
    {
        if (_conversations.ContainsKey(userId))
        {
            _conversations[userId] = new ChatHistory();
        }
    }
}

