import React, { useState, useRef, useEffect } from 'react';
import axios from 'axios';
import ChatMessage from './ChatMessage';
import './Chat.css';

const Chat = () => {
    const [messages, setMessages] = useState([]);
    const [input, setInput] = useState('');
    const [loading, setLoading] = useState(false);
    const messagesEndRef = useRef(null);
    const userId = "default"; // You can implement user authentication later

    const scrollToBottom = () => {
        messagesEndRef.current?.scrollIntoView({ behavior: 'smooth' });
    };

    useEffect(() => {
        scrollToBottom();
    }, [messages]);

    const handleSubmit = async (e) => {
        e.preventDefault();
        if (input.trim() === '') return;

        // Add user message to chat
        const userMessage = {
            text: input,
            sender: 'user',
            timestamp: new Date().toISOString()
        };

        setMessages(prevMessages => [...prevMessages, userMessage]);
        setInput('');
        setLoading(true);

        try {
            const response = await axios.post(`/AskAswin?question=${encodeURIComponent(input)}&userId=${userId}&model=OpenAI`);

            // Add AI response to chat
            const aiMessage = {
                text: typeof response.data.dbResult === 'object'
                    ? JSON.stringify(response.data.dbResult)
                    : (response.data.dbResult || response.data.sqlQuery || "I couldn't find an answer for that."),
                sender: 'ai',
                timestamp: new Date().toISOString(),
                sqlQuery: response.data.sqlQuery,
                thoughts: response.data.thoughts
            };

            setMessages(prevMessages => [...prevMessages, aiMessage]);

        } catch (error) {
            console.error('Error fetching response:', error);

            // Add error message
            const errorMessage = {
                text: 'Sorry, there was an error processing your request.',
                sender: 'system',
                timestamp: new Date().toISOString()
            };

            setMessages(prevMessages => [...prevMessages, errorMessage]);
        } finally {
            setLoading(false);
        }
    };

    const resetConversation = async () => {
        try {
            await axios.post(`/ResetConversation?userId=${userId}`);
            setMessages([]);
        } catch (error) {
            console.error('Error resetting conversation:', error);
        }
    };

    return (
        <div className="chat-container">
            <div className="chat-header">
                <h2>Ask Aswin</h2>
                <button onClick={resetConversation} className="reset-button">
                    New Conversation
                </button>
            </div>

            <div className="messages-container">
                {messages.map((message, index) => (
                    <ChatMessage key={index} message={message} />
                ))}
                {loading && (
                    <div className="message ai">
                        <div className="message-content">
                            <div className="typing-indicator">
                                <span></span>
                                <span></span>
                                <span></span>
                            </div>
                        </div>
                    </div>
                )}
                <div ref={messagesEndRef} />
            </div>

            <form onSubmit={handleSubmit} className="input-form">
                <input
                    type="text"
                    value={input}
                    onChange={(e) => setInput(e.target.value)}
                    placeholder="Ask a question..."
                    disabled={loading}
                />
                <button type="submit" disabled={loading || input.trim() === ''}>
                    Send
                </button>
            </form>
        </div>
    );
};

export default Chat;