import React, { useState } from 'react';
import GameService from '../services/GameService';
import './GameBoard.css';

const GameBoard = ({ game, gameData, onGameUpdate, onError, setIsLoading }) => {
  const [selectedLetter, setSelectedLetter] = useState('S');

  if (!gameData || !gameData.board) {
    return <div className="loading">Loading game board...</div>;
  }

  const boardSize = gameData.board.size;
  const cells = gameData.board.cells || {};

  const makeMove = async (row, col) => {
    if (gameData.status?.toLowerCase() !== 'inprogress') {
      onError('Game is not in progress');
      return;
    }

    // Check if cell is already occupied
    const cellKey = `${row},${col}`;
    if (cells[cellKey]) {
      onError('Cell is already occupied');
      return;
    }

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
        const errorMessage = await GameService.getErrorMessage(response, `Move failed: ${response.status}`);
        throw new Error(errorMessage);
      }

      const moveResult = await response.json();
      
      // Update the game data with the new state
      onGameUpdate(moveResult.game);

      // Show success message if SOS was formed
      if (moveResult.sosFormed) {
        // You could add a toast notification here
        console.log(`SOS formed: ${moveResult.message}`);
      }

    } catch (error) {
      console.error('Error making move:', error);
      onError(`Failed to make move: ${error.message}`);
    } finally {
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
    } else if (!isGameFinished) {
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
        grid.push(
          <div
            key={`${row}-${col}`}
            className={getCellClass(row, col)}
            onClick={() => makeMove(row, col)}
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

  return (
    <div className="game-board-container">
      <div className="board-header">
        <h3>Game Board</h3>
        
        {gameData.status?.toLowerCase() === 'inprogress' && (
          <div className="letter-selector">
            <span className="selector-label">Choose letter:</span>
            <div className="letter-buttons">
              <button
                className={`letter-button ${selectedLetter === 'S' ? 'selected' : ''}`}
                onClick={() => setSelectedLetter('S')}
              >
                S
              </button>
              <button
                className={`letter-button ${selectedLetter === 'O' ? 'selected' : ''}`}
                onClick={() => setSelectedLetter('O')}
              >
                O
              </button>
            </div>
          </div>
        )}
      </div>

      <div 
        className="game-board"
        style={{ 
          gridTemplateColumns: `repeat(${boardSize}, 1fr)`,
          gridTemplateRows: `repeat(${boardSize}, 1fr)`
        }}
      >
        {renderGrid()}
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