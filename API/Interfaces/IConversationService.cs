using Microsoft.SemanticKernel.ChatCompletion;

namespace API.Interfaces;

public interface IConversationService
{
    /// <summary>
    /// Retrieves an existing conversation for the specified user or creates a new conversation if none exists.
    /// </summary>
    /// <param name="userId">The unique identifier of the user for whom the conversation is being retrieved or created.</param>
    /// <returns>A <see cref="ChatHistory"/> instance representing the user's conversation history.</returns>
    ChatHistory GetOrCreateConversation(string userId);

    /// <summary>
    /// Resets the conversation history for the specified user to a new, empty state.
    /// </summary>
    /// <param name="userId">The unique identifier of the user whose conversation should be reset.</param>
    void ResetConversation(string userId);
}
