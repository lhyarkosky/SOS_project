import React, { useState } from 'react';
import GameSetup from './components/GameSetup';
import GameBoard from './components/GameBoard';
import GameInfo from './components/GameInfo';
import './App.css';

function App() {
  const [currentGame, setCurrentGame] = useState(null);
  const [gameData, setGameData] = useState(null);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState(null);

  const handleGameCreated = (game) => {
    setCurrentGame(game);
    setGameData(game);
    setError(null);
  };

  const handleGameUpdate = (updatedGameData) => {
    setGameData(updatedGameData);
  };

  const handleNewGame = () => {
    setCurrentGame(null);
    setGameData(null);
    setError(null);
  };

  const handleError = (errorMessage) => {
    setError(errorMessage);
  };

  return (
    <div className="container">
      <div className="header">
        <h1>SOS Game</h1>
        <p>Create sequences of S-O-S to win points</p>
      </div>

      {error && (
        <div className="error-message">
          <p>{error}</p>
          <button onClick={() => setError(null)}>Dismiss</button>
        </div>
      )}

      {!currentGame ? (
        <GameSetup 
          onGameCreated={handleGameCreated}
          onError={handleError}
          isLoading={isLoading}
          setIsLoading={setIsLoading}
        />
      ) : (
        <div className="game-container">
          <GameInfo 
            gameData={gameData} 
            onNewGame={handleNewGame}
          />
          <GameBoard 
            game={currentGame}
            gameData={gameData}
            onGameUpdate={handleGameUpdate}
            onError={handleError}
            setIsLoading={setIsLoading}
          />
        </div>
      )}

      {isLoading && (
        <div className="loading-overlay">
          <div className="loading-spinner">Loading...</div>
        </div>
      )}
    </div>
  );
}

export default App;