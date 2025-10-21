import React, { useState } from 'react';
import './GameSetup.css';

const GameSetup = ({ onGameCreated, onError, isLoading, setIsLoading }) => {
  const [boardSize, setBoardSize] = useState(5);
  const [gameMode, setGameMode] = useState('Simple');

  const createGame = async () => {
    setIsLoading(true);
    
    try {
      const response = await fetch('/api/SOS/create', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({
          boardSize: boardSize,
          gameMode: gameMode
        })
      });

      if (!response.ok) {
        const errorData = await response.text();
        throw new Error(errorData || `Failed to create game: ${response.status}`);
      }

      const gameData = await response.json();
      onGameCreated(gameData);
    } catch (error) {
      console.error('Error creating game:', error);
      onError(`Failed to create game: ${error.message}`);
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="game-setup">
      <h2>Start New Game</h2>
      
      <div className="setup-form">
        <div className="form-group">
          <label htmlFor="boardSize">Board Size:</label>
          <select 
            id="boardSize"
            value={boardSize} 
            onChange={(e) => setBoardSize(parseInt(e.target.value))}
            disabled={isLoading}
          >
            {[3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20].map(size => (
              <option key={size} value={size}>{size} x {size}</option>
            ))}
          </select>
        </div>

        <div className="form-group">
          <label htmlFor="gameMode">Game Mode:</label>
          <select 
            id="gameMode"
            value={gameMode} 
            onChange={(e) => setGameMode(e.target.value)}
            disabled={isLoading}
          >
            <option value="Simple">Simple Game</option>
            <option value="General">General Game</option>
          </select>
        </div>

        <div className="game-mode-description">
          {gameMode === 'Simple' ? (
            <p><strong>Simple Game:</strong> First player to form an SOS wins immediately.</p>
          ) : (
            <p><strong>General Game:</strong> Play until the board is full. Most SOS sequences wins.</p>
          )}
        </div>

        <button 
          onClick={createGame} 
          disabled={isLoading}
          className="create-button"
        >
          {isLoading ? 'Creating...' : 'Create Game'}
        </button>
      </div>
    </div>
  );
};

export default GameSetup;