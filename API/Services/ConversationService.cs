using API.Interfaces;
using Microsoft.SemanticKernel.ChatCompletion;

namespace API.Services;

public class ConversationService : IConversationService
{
    /// <summary>
    /// A private dictionary storing the conversation history for users.
    /// The dictionary maps a user's unique identifier as a string to their
    /// corresponding <see cref="ChatHistory"/> instance. This enables
    /// tracking and managing individual user conversations.
    /// </summary>
    private readonly Dictionary<string, ChatHistory> _conversations = new();
    
    /// <inheritdoc/>
    public ChatHistory GetOrCreateConversation(string userId)
    {
        if (!_conversations.TryGetValue(userId, out var history))
        {
            history = new ChatHistory();
            _conversations[userId] = history;
        }
        return history;
    }
    
    /// <inheritdoc/>
    public void ResetConversation(string userId)
    {
        if (_conversations.ContainsKey(userId))
        {
            _conversations[userId] = new ChatHistory();
        }
    }
}

