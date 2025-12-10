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
            // best-effort: unable to determine AI/Player types from serialized game reliably
            setIsAiVsAi(false);
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

  const parseMovesFromText = (text) => {
    // Try JSON first. If it is a full package { game, moves } prefer that in the caller.
    try {
      const parsed = JSON.parse(text);
      return parsed; // caller will inspect whether this is array or object with {game,moves}
    } catch (e) {
      // not JSON, continue to parse as plain text
    }

    // Parse plain text lines produced by legacy export (e.g., "1. Player placed \"S\" at (row, col) at time")
    const lines = text.split(/\r?\n/).map(l => l.trim()).filter(Boolean);
    const moves = [];
    const lineRegex = /^\s*\d+\.\s*(.*?)\s+placed\s+"([SO])"\s+at\s+\((\d+)\s*,\s*(\d+)\)\s*(?:at\s*(.*))?$/i;

    for (const line of lines) {
      const m = line.match(lineRegex);
      if (m) {
        const player = m[1];
        const letter = m[2].toUpperCase();
        const row = parseInt(m[3], 10);
        const col = parseInt(m[4], 10);
        const time = m[5] || null;
        moves.push({
          player: player,
          isAI: false,
          move: { row, col, letter },
          sosFormed: false,
          newSequencesCount: 0,
          newSequences: [],
          message: time ? `Placed at ${time}` : 'Move'
        });
      } else {
        // Fallback: store the raw line as a move message
        moves.push({ player: 'Unknown', isAI: false, move: { row: 0, col: 0, letter: '' }, sosFormed: false, newSequencesCount: 0, newSequences: [], message: line });
      }
    }

    return moves;
  };

  const buildGameDataFromMoves = (moves) => {
    // Determine board size from max row/col
    let maxRow = 0, maxCol = 0;
    const playersSet = new Set();
    const finalBoard = {};
    const sequences = [];
    const scores = {};

    moves.forEach((m) => {
      const r = m.move?.row ?? 0;
      const c = m.move?.col ?? 0;
      const letter = m.move?.letter ?? '';
      if (typeof r === 'number') maxRow = Math.max(maxRow, r);
      if (typeof c === 'number') maxCol = Math.max(maxCol, c);
      if (m.player) playersSet.add(m.player);
      if (letter) finalBoard[`${r},${c}`] = letter;
      if (m.newSequences && Array.isArray(m.newSequences)) {
        m.newSequences.forEach(seq => {
          sequences.push(seq);
          const foundBy = seq.foundBy || (m.player ?? 'Unknown');
          scores[foundBy] = (scores[foundBy] || 0) + 1;
        });
      }
    });

    const players = Array.from(playersSet);
    // ensure scores keys include players
    players.forEach(p => { if (!(p in scores)) scores[p] = 0; });

    // determine winner
    let winner = null;
    const maxScore = Math.max(...Object.values(scores).concat(0));
    const winners = Object.entries(scores).filter(([_, s]) => s === maxScore).map(([p]) => p);
    if (winners.length === 1) winner = winners[0];

    return {
      board: { size: Math.max(3, Math.max(maxRow, maxCol) + 1), cells: finalBoard },
      completedSequences: sequences,
      scores,
      players,
      status: 'finished',
      winner
    };
  };

  const handleFileSelected = async (e) => {
    const file = e.target.files && e.target.files[0];
    if (!file) return;

    try {
      const text = await file.text();
      const parsed = parseMovesFromText(text);

      // If parsed is an object with { game, moves } prefer exact restore
      if (parsed && typeof parsed === 'object' && !Array.isArray(parsed) && parsed.game) {
        const pkg = parsed;
        setCurrentGame(null);
        setGameData(pkg.game);
        setMoves(pkg.moves || []);
        setIsAiVsAi(false);
        setShowSimulation(true);
        setError(null);
        return;
      }

      // If parsed is an array (moves) use legacy behavior
      const moves = Array.isArray(parsed) ? parsed : [];

      if (!moves || moves.length === 0) {
        setError('No moves found in uploaded file');
        return;
      }

      const gd = buildGameDataFromMoves(moves);
      setCurrentGame(null);
      setGameData(gd);
      setMoves(moves);
      setIsAiVsAi(false);
      setShowSimulation(true);
      setError(null);
    } catch (err) {
      console.error('Failed to read uploaded file', err);
      setError(`Failed to read uploaded file: ${err?.message || err}`);
    } finally {
      // nothing to reset here; the file input lives inside GameSetup and resets itself
    }
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

      {!currentGame && !showSimulation ? (
            <GameSetup 
              onGameCreated={handleGameCreated}
              onError={handleError}
              isLoading={isLoading}
              setIsLoading={setIsLoading}
              onReplayFileSelected={handleFileSelected}
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