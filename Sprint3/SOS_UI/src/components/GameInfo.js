import React from 'react';
import './GameInfo.css';

const GameInfo = ({ gameData, onNewGame }) => {
  if (!gameData) return null;

  return (
    <div className="game-info">
      <div className="info-header">
        <h3>Game Status</h3>
        <button onClick={onNewGame} className="new-game-button">
          New Game
        </button>
      </div>

      <div className="info-section">
        <div className="info-item">
          <span className="label">Game ID:</span>
          <span className="value">{gameData.gameId?.substring(0, 8)}...</span>
        </div>

        <div className="info-item">
          <span className="label">Mode:</span>
          <span className="value">{gameData.mode}</span>
        </div>

        <div className="info-item">
          <span className="label">Board Size:</span>
          <span className="value">{gameData.board?.size} x {gameData.board?.size}</span>
        </div>

        <div className="info-item">
          <span className="label">Status:</span>
          <span className="value">{gameData.status}</span>
        </div>

        {gameData.status?.toLowerCase() === 'inprogress' && (
          <div className="info-item">
            <span className="label">Current Player:</span>
            <span className="value">{gameData.currentPlayer}</span>
          </div>
        )}

        {gameData.winner && (
          <div className="info-item winner">
            <span className="label">Winner:</span>
            <span className="value">{gameData.winner}</span>
          </div>
        )}
      </div>

      <div className="scores-section">
        <h4>Scores</h4>
        <div className="scores">
          {gameData.players && gameData.players.map((playerName, index) => (
            <div key={playerName} className="score-item">
              <span>{index === 0 ? '🔵' : '🔴'} {playerName}:</span>
              <span className="score">{gameData.scores?.[playerName] || 0}</span>
            </div>
          ))}
        </div>
      </div>

      {gameData.completedSequences && gameData.completedSequences.length > 0 && (
        <div className="sequences-section">
          <h4>SOS Sequences Found</h4>
          <div className="sequences-count">
            {gameData.completedSequences.length} sequence(s) completed
          </div>
        </div>
      )}
    </div>
  );
};

export default GameInfo;