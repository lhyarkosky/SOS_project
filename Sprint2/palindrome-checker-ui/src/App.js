import React, { useState } from 'react';
import axios from 'axios';
import './App.css';

function App() {
  const [input, setInput] = useState('');
  const [result, setResult] = useState(null);
  const [loading, setLoading] = useState(false);

  const handleInputChange = (e) => {
    setInput(e.target.value);
  };

  const checkPalindrome = async () => {
    if (!input.trim()) {
      setResult({
        isValid: false,
        message: 'Please enter some text.'
      });
      return;
    }

    setLoading(true);
    try {
      const response = await axios.get(`https://localhost:7071/api/Palindrome/validate?text=${encodeURIComponent(input)}`);
      setResult(response.data);
    } catch (error) {
      console.error('Error checking palindrome:', error);
      setResult({
        isValid: false,
        message: 'An error occurred while checking the text.'
      });
    } finally {
      setLoading(false);
    }
  };

  const handleClear = () => {
    setInput('');
    setResult(null);
  };

  return (
    <div className="container">
      <h1>Palindrome Checker</h1>
      <p>Enter text to check for a palindrome:</p>
      
      <div className="input-group">
        <input
          type="text"
          value={input}
          onChange={handleInputChange}
          placeholder="Enter text..."
          className="text-input"
        />
      </div>
      
      <div className="button-group">
        <button onClick={checkPalindrome} disabled={loading} className="check-button">
          {loading ? 'Checking...' : 'Check'}
        </button>
        <button onClick={handleClear} className="clear-button">Clear</button>
      </div>
      
      {result && (
        <div className={`result ${result.isValid && result.message.includes('palindrome!') ? 'success' : 'error'}`}>
          {result.message}
        </div>
      )}
    </div>
  );
}

export default App;