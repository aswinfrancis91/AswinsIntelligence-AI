import React, { useState } from 'react';
import axios from 'axios';
import './ChatMessage.css';

const ChatMessage = ({ message }) => {
    const [showDetails, setShowDetails] = useState(false);
    const [chartImage, setChartImage] = useState(null);
    const [showChart, setShowChart] = useState(true);
    const [loadingChart, setLoadingChart] = useState(false);

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

    // Function to generate chart from data
    const generateChart = async () => {
        if (!isTableData(message.text)) return;

        // If we already have a chart, just show it
        if (chartImage) {
            setShowChart(true);
            return;
        }

        setLoadingChart(true);
        try {
            // The API expects a 'question' parameter
            const response = await axios.post(`/GenerateGraph?question=${encodeURIComponent(message.text)}`, null, {
                responseType: 'text'
            });

            // Check if the response is a valid base64 string
            if (response.data && typeof response.data === 'string') {
                // Prepend the data URL prefix for image rendering
                const imageData = `data:image/png;base64,${response.data}`;
                setChartImage(imageData);
                setShowChart(true);
            } else {
                console.error('Invalid image data received:', response.data);
            }
        } catch (error) {
            console.error('Error generating chart:', error);
        } finally {
            setLoadingChart(false);
        }
    };

    // Function to export JSON data as CSV
    const exportToCSV = () => {
        if (!isTableData(message.text)) return;

        try {
            const data = JSON.parse(message.text);
            if (!Array.isArray(data) || data.length === 0) {
                console.error('No data to export');
                return;
            }

            // Get headers from the first object
            const headers = Object.keys(data[0]);

            // Create CSV content with headers
            let csvContent = headers.join(',') + '\n';

            // Add data rows
            data.forEach(row => {
                const rowValues = headers.map(header => {
                    // Handle values that might contain commas or quotes
                    const cellValue = row[header];
                    if (cellValue === null || cellValue === undefined) {
                        return '';
                    }
                    const cell = String(cellValue);
                    // Escape quotes and wrap in quotes if needed
                    if (cell.includes(',') || cell.includes('"') || cell.includes('\n')) {
                        return `"${cell.replace(/"/g, '""')}"`;
                    }
                    return cell;
                });
                csvContent += rowValues.join(',') + '\n';
            });

            // Create and download the CSV file
            const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' });
            const url = URL.createObjectURL(blob);
            const link = document.createElement('a');
            link.setAttribute('href', url);
            link.setAttribute('download', 'data_export.csv');
            link.style.display = 'none';
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);

        } catch (error) {
            console.error('Error exporting to CSV:', error);
        }
    };

    // Toggle chart visibility
    const toggleChartVisibility = () => {
        setShowChart(!showChart);
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

                    <div className="chart-controls">
                        <div className="button-group">
                            {!chartImage && (
                                <button
                                    className="chart-button"
                                    onClick={generateChart}
                                    disabled={loadingChart}
                                >
                                    {loadingChart ? "Generating chart..." : "Generate Chart"}
                                </button>
                            )}

                            <button
                                className="export-csv-button"
                                onClick={exportToCSV}
                            >
                                Export to CSV
                            </button>
                        </div>

                        {chartImage && (
                            <div className="chart-container">
                                {showChart && (
                                    <img
                                        src={chartImage}
                                        alt="Data visualization"
                                        className="chart-image"
                                    />
                                )}
                                <button
                                    className="toggle-chart-button"
                                    onClick={toggleChartVisibility}
                                >
                                    {showChart ? "Hide Chart" : "Show Chart"}
                                </button>
                            </div>
                        )}
                    </div>
                </>
            );
        } else if (message.sender === 'ai' && typeof message.text === 'string' && message.text.startsWith('data:image')) {
            return (
                <div className="chart-container">
                    <img
                        src={message.text}
                        alt="Data visualization"
                        className="chart-image"
                    />
                </div>
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