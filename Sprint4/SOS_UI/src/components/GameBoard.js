import React, { useState, useEffect} from 'react';
import GameService from '../services/GameService';
import './GameBoard.css';

const GameBoard = ({ game, gameData, onGameUpdate, onError, setIsLoading, onShowSimulation, isAiVsAi }) => {
  const [selectedLetter, setSelectedLetter] = useState('S');
  const [isProcessing, setIsProcessing] = useState(false);

  if (!gameData || !gameData.board) {
    return <div className="loading">Loading game board...</div>;
  }

  const boardSize = gameData.board.size;
  const cells = gameData.board.cells || {};

  const makeMove = async (row, col) => {
    if (isProcessing) return;
    setIsProcessing(true);
    setIsLoading(true);

    try {
      const response = await fetch('/api/SOS/move', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({
          gameId: gameData.gameId,
          row: row,
          col: col,
          letter: selectedLetter
        })
      });

      if (!response.ok) {
        const errorMessage = await GameService.getErrorMessage(response, response.status);
        onError(errorMessage);
        return;
      }

      const moveResult = await response.json();
      
      onGameUpdate(moveResult.game);
    } catch (error) {
      console.error('Error making move:', error);
      const errorMessage = await GameService.getErrorMessage(null, 'Network error occurred');
      onError(errorMessage);
    } finally {
      setIsProcessing(false);
      setIsLoading(false);
    }
  };

  const getCellContent = (row, col) => {
    const cellKey = `${row},${col}`;
    return cells[cellKey] || '';
  };

  const getCellClass = (row, col) => {
    const content = getCellContent(row, col);
    const isOccupied = content !== '';
    const isGameFinished = gameData.status?.toLowerCase() === 'finished';
    
    let className = 'cell';
    
    if (isOccupied) {
      className += ' occupied';
    } else if (!isGameFinished && !isProcessing) {
      className += ' clickable';
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
        const isGameFinished = gameData.status?.toLowerCase() === 'finished';
        grid.push(
          <div
            key={`${row}-${col}`}
            className={getCellClass(row, col)}
            onClick={isGameFinished || isProcessing ? undefined : () => makeMove(row, col)}
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

  // Get the center of a cell in percent (relative to board)
  const getCellCenterPercent = (row, col) => {
    const step = 100 / boardSize;
    return {
      x: (col + 0.5) * step,
      y: (row + 0.5) * step
    };
  };

  // Render SVG lines for completed sequences
  const renderSequenceLines = () => {
    const sequences = gameData.completedSequences;
    if (!sequences) return null;
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
        {sequences.map((seq, idx) => {
          if (!seq.positions || seq.positions.length < 2) return null;
          const start = getCellCenterPercent(seq.positions[0].row, seq.positions[0].col);
          const end = getCellCenterPercent(seq.positions[seq.positions.length - 1].row, seq.positions[seq.positions.length - 1].col);
          
          // Get color based on which player found the sequence
          const playerIndex = gameData.players?.indexOf(seq.foundBy) ?? 0;
          const color = playerIndex === 0 ? 'blue' : 'red';
          
          return (
            <line
              key={idx}
              x1={start.x}
              y1={start.y}
              x2={end.x}
              y2={end.y}
              stroke={color}
              strokeWidth={2}
              strokeLinecap="round"
              opacity={0.5}
            />
          );
        })}
      </svg>
    );
  };

  return (
    <div className="game-board-container">
      <div className="board-header">
        <h3>Game Board</h3>
        <div className="board-controls">
          {isAiVsAi && (
            <button 
              className="simulation-button"
              onClick={onShowSimulation}
            >
              Play Simulation
            </button>
          )}
          {gameData.status?.toLowerCase() === 'inprogress' && (
            <div className="letter-selector">
              <span className="selector-label">Choose letter:</span>
              <div className="letter-buttons">
                <button
                  className={`letter-button ${selectedLetter === 'S' ? 'selected' : ''}`}
                  onClick={() => setSelectedLetter('S')}
                  disabled={isProcessing}
                >
                  S
                </button>
                <button
                  className={`letter-button ${selectedLetter === 'O' ? 'selected' : ''}`}
                  onClick={() => setSelectedLetter('O')}
                  disabled={isProcessing}
                >
                  O
                </button>
              </div>
            </div>
          )}
        </div>
      </div>

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

      <div className="board-footer">
        <p className="instructions">
          {gameData.status?.toLowerCase() === 'inprogress' 
            ? `${gameData.currentPlayer}'s turn - Click a cell to place "${selectedLetter}"` 
            : 'Game finished!'
          }
        </p>
      </div>
    </div>
  );
};

export default GameBoard;