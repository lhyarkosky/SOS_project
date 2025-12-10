import React, { useState, useEffect } from 'react';
import './SimulateGame.css';

const SimulateGame = ({ gameData, moves, onNewGame }) => {
  const [currentMoveIndex, setCurrentMoveIndex] = useState(0);
  const [isPlaying, setIsPlaying] = useState(false);
  const [currentBoard, setCurrentBoard] = useState({});
  const [currentSequences, setCurrentSequences] = useState([]);
  const [currentScores, setCurrentScores] = useState({});
  const [simulationSpeed, setSimulationSpeed] = useState(500); // ms
  const [showMoveMessage, setShowMoveMessage] = useState(false);

  const boardSize = gameData.board.size;
  const finalBoard = gameData.board.cells || {};
  const finalSequences = gameData.completedSequences || [];
  const finalScores = gameData.scores || {};

  useEffect(() => {
    // Initialize with empty board
    setCurrentBoard({});
    setCurrentSequences([]);
    setCurrentScores(Object.keys(finalScores).reduce((acc, player) => {
      acc[player] = 0;
      return acc;
    }, {}));
  }, [gameData, finalScores]);

  const playSimulation = () => {
    setIsPlaying(true);
    setCurrentMoveIndex(0);
  };

  const resetSimulation = () => {
    setIsPlaying(false);
    setCurrentMoveIndex(0); // was -1, now 0 for correct display
    setCurrentBoard({});
    setCurrentSequences([]);
    setCurrentScores(Object.keys(finalScores).reduce((acc, player) => {
      acc[player] = 0;
      return acc;
    }, {}));
  };

  const skipToEnd = () => {
    setCurrentMoveIndex(moves.length);
    setCurrentBoard(finalBoard);
    setCurrentSequences(finalSequences);
    setCurrentScores(finalScores);
    setIsPlaying(false);
  };

  useEffect(() => {
    if (isPlaying && currentMoveIndex >= 0 && currentMoveIndex < moves.length) {
      if (!showMoveMessage) {
        // Place the move on the board
        const move = moves[currentMoveIndex];
        const newBoard = { ...currentBoard };
        const cellKey = `${move.move.row},${move.move.col}`;
        newBoard[cellKey] = move.move.letter;

        const newSequences = [...currentSequences];
        const newScores = { ...currentScores };

        if (move.newSequences && move.newSequences.length > 0) {
          move.newSequences.forEach(seq => {
            newSequences.push(seq);
            const foundByName = typeof seq.foundBy === 'string' ? seq.foundBy : seq.foundBy?.name;
            if (foundByName) {
              newScores[foundByName] = (newScores[foundByName] || 0) + 1;
            }
          });
        }

        setCurrentBoard(newBoard);
        setCurrentSequences(newSequences);
        setCurrentScores(newScores);
        setShowMoveMessage(true);
      } else {
        // Show the move message after the move is placed
        const timer = setTimeout(() => {
          setCurrentMoveIndex(currentMoveIndex + 1);
          setShowMoveMessage(false);
        }, simulationSpeed);
        return () => clearTimeout(timer);
      }
    } else if (currentMoveIndex >= moves.length) {
      setIsPlaying(false);
      setShowMoveMessage(false);
    }
  }, [isPlaying, currentMoveIndex, moves, currentBoard, currentSequences, currentScores, simulationSpeed, showMoveMessage]);

  const getCellContent = (row, col) => {
    const cellKey = `${row},${col}`;
    return currentBoard[cellKey] || '';
  };

  const getCellClass = (row, col) => {
    const content = getCellContent(row, col);
    const isOccupied = content !== '';

    let className = 'cell';

    if (isOccupied) {
      className += ' occupied';
    }

    // Add letter-specific styling
    if (content === 'S') className += ' letter-s';
    if (content === 'O') className += ' letter-o';

    return className;
  };

  const renderGrid = () => {
    const grid = [];
    for (let row = 0; row < boardSize; row++) {
      for (let col = 0; col < boardSize; col++) {
        grid.push(
          <div
            key={`${row}-${col}`}
            className={getCellClass(row, col)}
            data-row={row}
            data-col={col}
          >
            <span className="cell-content">
              {getCellContent(row, col)}
            </span>
            <div className="cell-coordinates">
              {row},{col}
            </div>
          </div>
        );
      }
    }
    return grid;
  };

  const getCellCenterPercent = (row, col) => {
    const step = 100 / boardSize;
    return {
      x: (col + 0.5) * step,
      y: (row + 0.5) * step
    };
  };

  const renderSequenceLines = () => {
    if (!currentSequences || currentSequences.length === 0) return null;
    return (
      <svg
        className="sos-overlay"
        width="100%"
        height="100%"
        viewBox="0 0 100 100"
        style={{
          position: 'absolute',
          top: 0,
          left: 0,
          pointerEvents: 'none',
          zIndex: 2
        }}
        preserveAspectRatio="none"
      >
        {currentSequences.map((seq, idx) => {
          if (!seq.positions || seq.positions.length < 2) return null;
          const start = getCellCenterPercent(seq.positions[0].row, seq.positions[0].col);
          const end = getCellCenterPercent(seq.positions[seq.positions.length - 1].row, seq.positions[seq.positions.length - 1].col);

          // Get color based on which player found the sequence
          const foundByName = typeof seq.foundBy === 'string' ? seq.foundBy : seq.foundBy?.name;
          const playerIndex = gameData.players?.indexOf(foundByName) ?? 0;
          const color = playerIndex === 0 ? 'blue' : 'red';

          return (
            <line
              key={idx}
              x1={start.x}
              y1={start.y}
              x2={end.x}
              y2={end.y}
              stroke={color}
              strokeWidth={3}
              strokeLinecap="round"
              opacity={0.7}
            />
          );
        })}
      </svg>
    );
  };

  const currentMove = currentMoveIndex >= 0 && currentMoveIndex < moves.length ? moves[currentMoveIndex] : null;

  return (
    <div className="simulate-game-container">
      <div className="simulation-header">
        <h3>AI vs AI Game Simulation</h3>
        <div className="simulation-controls">
          <button onClick={playSimulation} disabled={isPlaying || currentMoveIndex >= moves.length}>
            {currentMoveIndex === -1 ? 'Start Simulation' : 'Resume'}
          </button>
          <button onClick={resetSimulation} disabled={isPlaying}>
            Reset
          </button>
          <button onClick={skipToEnd} disabled={isPlaying || currentMoveIndex >= moves.length}>
            Skip to End
          </button>
          <button onClick={onNewGame}>
            New Game
          </button>
        </div>
        <div className="simulation-speed">
          <label htmlFor="speed-slider">Simulation Speed: </label>
          <input
            id="speed-slider"
            type="range"
            min="100"
            max="2000"
            step="100"
            value={simulationSpeed}
            onChange={e => setSimulationSpeed(Number(e.target.value))}
            style={{ verticalAlign: 'middle' }}
          />
          <span>{simulationSpeed} ms</span>
        </div>
      </div>

      <div className="simulation-info">
        <div className="move-counter">
          Move: {currentMoveIndex} / {moves.length}
        </div>
        <div className="current-player">
          {currentMove ? `${currentMove.player} (${currentMove.isAI ? 'AI' : 'Human'})` : 'Game Start'}
        </div>
        <div className="scores">
          {Object.entries(currentScores).map(([player, score]) => (
            <div key={player} className="score">
              {player}: {score}
            </div>
          ))}
        </div>
      </div>

      {showMoveMessage && currentMove && (
        <div className="current-move-info">
          <p>
            <strong>{currentMove.message}</strong><br />
            {currentMove.player} placed "{currentMove.move.letter}" at ({currentMove.move.row}, {currentMove.move.col})
            {currentMove.sosFormed && ` - ${currentMove.newSequencesCount} SOS sequence(s) formed!`}
          </p>
        </div>
      )}

      <div
        className="game-board"
        style={{
          position: 'relative',
          gridTemplateColumns: `repeat(${boardSize}, 1fr)`,
          gridTemplateRows: `repeat(${boardSize}, 1fr)`
        }}
      >
        {renderGrid()}
        {renderSequenceLines()}
      </div>

      <div className="simulation-footer">
        {currentMoveIndex >= moves.length && (
          <div className="game-result">
            <h4>Game Finished!</h4>
            <p>Winner: {gameData.winner || 'Draw'}</p>
            <p>Final Scores: {Object.entries(finalScores).map(([player, score]) => `${player}: ${score}`).join(', ')}</p>
          </div>
        )}
      </div>
    </div>
  );
};

export default SimulateGame;