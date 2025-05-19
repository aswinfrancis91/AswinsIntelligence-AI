import React, { useState } from 'react';
import './ChatMessage.css';

const ChatMessage = ({ message }) => {
    const [showDetails, setShowDetails] = useState(false);

    // Helper to determine if the content is JSON data for a table
    const isTableData = (text) => {
        try {
            if (typeof text !== 'string') return false;
            const data = JSON.parse(text);
            return Array.isArray(data) && data.length > 0 && typeof data[0] === 'object';
        } catch (e) {
            return false;
        }
    };

    // Render a table from JSON data
    const renderTable = (jsonText) => {
        try {
            const data = JSON.parse(jsonText);

            if (!Array.isArray(data) || data.length === 0) {
                return <div className="table-empty">No data available</div>;
            }

            // Get column headers from the first item
            const columns = Object.keys(data[0]);

            return (
                <div className="table-container">
                    <table className="data-table">
                        <thead>
                        <tr>
                            {columns.map((column, index) => (
                                <th key={index}>{column}</th>
                            ))}
                        </tr>
                        </thead>
                        <tbody>
                        {data.map((row, rowIndex) => (
                            <tr key={rowIndex}>
                                {columns.map((column, colIndex) => (
                                    <td key={colIndex}>{row[column]}</td>
                                ))}
                            </tr>
                        ))}
                        </tbody>
                    </table>
                </div>
            );
        } catch (e) {
            return <div className="table-empty">Error displaying data: {e.message}</div>;
        }
    };

    // Determine if we should render a table or regular text
    const renderMessageContent = () => {
        if (message.sender === 'ai' && isTableData(message.text)) {
            return (
                <>
                    <p>Here are the results:</p>
                    {renderTable(message.text)}
                </>
            );
        }

        return <p>{message.text}</p>;
    };

    return (
        <div className={`message ${message.sender}`}>
            <div className="message-content">
                {renderMessageContent()}

                {/* Show SQL query and thoughts if available */}
                {message.sender === 'ai' && (message.sqlQuery || message.thoughts) && (
                    <div className="message-details">
                        <button
                            className="details-button"
                            onClick={() => setShowDetails(!showDetails)}
                        >
                            {showDetails ? "Hide SQL" : "Show SQL"}
                        </button>

                        {showDetails && message.sqlQuery && (
                            <div className="sql-container">
                                <pre>{message.sqlQuery}</pre>
                            </div>
                        )}

                        {showDetails && message.thoughts && (
                            <div className="thoughts">
                                <h4>AI's Thought Process:</h4>
                                <p>{message.thoughts}</p>
                            </div>
                        )}
                    </div>
                )}
            </div>
        </div>
    );
};

export default ChatMessage;