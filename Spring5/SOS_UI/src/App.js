import React, { useState, useEffect } from 'react';
import GameSetup from './components/GameSetup';
import GameBoard from './components/GameBoard';
import SimulateGame from './components/SimulateGame';
import GameInfo from './components/GameInfo';
import './App.css';
import GameService from './services/GameService';

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
    // create endpoint returns { game: <gameObj>, moves: [...] }
    const createdGame = gameData.game || gameData;
    setCurrentGame(createdGame);
    setGameData(createdGame);
    setMoves(gameData.moves || []);
    // Persist game id to localStorage for session persistence
    try {
      if (createdGame && createdGame.gameId) {
        localStorage.setItem('sos_game_id', createdGame.gameId);
      }
    } catch (e) {
      // ignore storage errors
      console.warn('Could not persist game id to localStorage', e);
    }
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
    try {
      localStorage.removeItem('sos_game_id');
    } catch (e) {
      console.warn('Could not remove game id from localStorage', e);
    }
  };

  const handleError = (errorMessage) => {
    setError(errorMessage);
  };

  // On mount, check localStorage for saved game id and try to restore
  useEffect(() => {
    const restore = async () => {
      try {
        const savedId = localStorage.getItem('sos_game_id');
        if (!savedId) return;
        setIsLoading(true);
        const fetched = await GameService.getGame(savedId);

        // fetched is the serialized game object
        if (fetched) {
          setCurrentGame(fetched);
          setGameData(fetched);
          setMoves([]);
          // best-effort: if both players are AI, show simulation
          const players = fetched.players || [];
          const aiVsAi = false; // unable to determine types from serialized game reliably
          setIsAiVsAi(aiVsAi);
        } else {
          // remove if server doesn't have it
          localStorage.removeItem('sos_game_id');
        }
      } catch (err) {
        console.warn('Failed to restore saved game:', err);
        try { localStorage.removeItem('sos_game_id'); } catch(_){}
      } finally {
        setIsLoading(false);
      }
    };

    restore();
  }, []);

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