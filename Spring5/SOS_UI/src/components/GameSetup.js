import React, { useState } from 'react';
import GameService from '../services/GameService';
import './GameSetup.css';

const GameSetup = ({ onGameCreated, onError, isLoading, setIsLoading }) => {
  const [boardSize, setBoardSize] = useState(5);
  const [gameMode, setGameMode] = useState('Simple');
  const [player1Name, setPlayer1Name] = useState('');
  const [player2Name, setPlayer2Name] = useState('');
  const [player1Type, setPlayer1Type] = useState('Human');
  const [player2Type, setPlayer2Type] = useState('Human');

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
          gameMode: gameMode,
          player1Name: player1Name || null,
          player2Name: player2Name || null,
          player1Type: player1Type,
          player2Type: player2Type
        })
      });

      if (!response.ok) {
        const errorMessage = await GameService.getErrorMessage(response, `Failed to create game: ${response.status}`);
        throw new Error(errorMessage);
      }

      const gameData = await response.json();
      onGameCreated(gameData, { player1Type, player2Type });
    } catch (error) {
      console.error('Error creating game:', error);
      onError(error.message);
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

        <div className="form-group">
          <label htmlFor="player1Name">Player 1 Name (optional):</label>
          <input
            type="text"
            id="player1Name"
            value={player1Name}
            onChange={(e) => setPlayer1Name(e.target.value)}
            placeholder="Player1"
            maxLength="50"
            disabled={isLoading}
          />
        </div>

        <div className="form-group">
          <label>Player 1:</label>
          <div className="player-type">
            <label>
              <input
                type="radio"
                name="player1Type"
                value="Human"
                checked={player1Type === 'Human'}
                onChange={(e) => setPlayer1Type(e.target.value)}
                disabled={isLoading}
              />
              Human
            </label>
            <label>
              <input
                type="radio"
                name="player1Type"
                value="Computer"
                checked={player1Type === 'Computer'}
                onChange={(e) => setPlayer1Type(e.target.value)}
                disabled={isLoading}
              />
              AI
            </label>
          </div>
        </div>

        <div className="form-group">
          <label htmlFor="player2Name">Player 2 Name (optional):</label>
          <input
            type="text"
            id="player2Name"
            value={player2Name}
            onChange={(e) => setPlayer2Name(e.target.value)}
            placeholder="Player2"
            maxLength="50"
            disabled={isLoading}
          />
        </div>

        <div className="form-group">
          <label>Player 2:</label>
          <div className="player-type">
            <label>
              <input
                type="radio"
                name="player2Type"
                value="Human"
                checked={player2Type === 'Human'}
                onChange={(e) => setPlayer2Type(e.target.value)}
                disabled={isLoading}
              />
              Human
            </label>
            <label>
              <input
                type="radio"
                name="player2Type"
                value="Computer"
                checked={player2Type === 'Computer'}
                onChange={(e) => setPlayer2Type(e.target.value)}
                disabled={isLoading}
              />
              AI
            </label>
          </div>
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