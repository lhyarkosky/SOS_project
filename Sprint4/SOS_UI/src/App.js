import React, { useState } from 'react';
import GameSetup from './components/GameSetup';
import GameBoard from './components/GameBoard';
import SimulateGame from './components/SimulateGame';
import GameInfo from './components/GameInfo';
import './App.css';

function App() {
  const [currentGame, setCurrentGame] = useState(null);
  const [gameData, setGameData] = useState(null);
  const [moves, setMoves] = useState([]);
  const [isAiVsAi, setIsAiVsAi] = useState(false);
  const [showSimulation, setShowSimulation] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState(null);

  const handleGameCreated = (gameData, gameConfig) => {
    const aiVsAi = gameConfig && gameConfig.player1Type === 'Computer' && gameConfig.player2Type === 'Computer';
    setIsAiVsAi(aiVsAi);
    setShowSimulation(aiVsAi); // Auto-show simulation for AI vs AI
    setCurrentGame(gameData.game);
    setGameData(gameData.game);
    setMoves(gameData.moves || []);
    setError(null);
  };

  const handleGameUpdate = (updatedGameData) => {
    setGameData(updatedGameData);
  };

  const handleNewGame = () => {
    setCurrentGame(null);
    setGameData(null);
    setMoves([]);
    setIsAiVsAi(false);
    setShowSimulation(false);
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
          {showSimulation || isAiVsAi ? (
            <SimulateGame 
              gameData={gameData}
              moves={moves}
              onNewGame={handleNewGame}
            />
          ) : (
            <GameBoard 
              game={currentGame}
              gameData={gameData}
              onGameUpdate={handleGameUpdate}
              onError={handleError}
              setIsLoading={setIsLoading}
              onShowSimulation={() => setShowSimulation(true)}
              isAiVsAi={isAiVsAi}
            />
          )}
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