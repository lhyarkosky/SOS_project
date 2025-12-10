import React from 'react';
import './GameInfo.css';
import GameService from '../services/GameService';

const GameInfo = ({ gameData, onNewGame }) => {
  if (!gameData) return null;

  const handleExport = async () => {
    try {
      // Prefer persisted id in localStorage, fallback to current gameData.gameId
      const savedId = (() => {
        try { return localStorage.getItem('sos_game_id'); } catch { return null; }
      })();

      const gameId = savedId || gameData.gameId;
      if (!gameId) {
        window.alert('No game id available to export.');
        return;
      }

      // Fetch both the serialized game and its move history so we can export a full package
      const [moves, fullGame] = await Promise.all([
        GameService.getMoveHistory(gameId),
        // Use API to get the canonical serialized game when available
        (async () => {
          try {
            return await GameService.getGame(gameId);
          } catch (e) {
            // fallback to client-side gameData if server request fails
            return gameData;
          }
        })()
      ]);

      const payload = {
        game: fullGame || gameData,
        moves: moves || []
      };

      const textContent = JSON.stringify(payload, null, 2);
      const blob = new Blob([textContent], { type: 'application/json;charset=utf-8' });
      const url = URL.createObjectURL(blob);
      const a = document.createElement('a');
      a.href = url;
      const safeId = gameId.replace(/[^a-zA-Z0-9-_]/g, '_');
      a.download = `sos_game_${safeId}_full.json`;
      document.body.appendChild(a);
      a.click();
      a.remove();
      URL.revokeObjectURL(url);
    } catch (err) {
      console.error('Failed to export move history', err);
      window.alert(`Failed to export move history: ${err.message || err}`);
    }
  };

  return (
    <div className="game-info">
      <div className="info-header">
        <h3>Game Status</h3>
        <div className="info-header-actions">
          <button onClick={onNewGame} className="new-game-button">
            New Game
          </button>

          {gameData.status?.toLowerCase() === 'finished' && (
            <button onClick={handleExport} className="export-game-button">
              Export Game
            </button>
          )}
        </div>
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