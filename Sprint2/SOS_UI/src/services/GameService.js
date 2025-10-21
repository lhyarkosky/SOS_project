// API service for SOS game
const API_BASE = '/api/SOS';

class GameService {
  // Create a new game
  static async createGame(boardSize, gameMode) {
    const response = await fetch(`${API_BASE}/create`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({
        boardSize,
        gameMode
      })
    });

    if (!response.ok) {
      const errorData = await response.text();
      throw new Error(errorData || `Failed to create game: ${response.status}`);
    }

    return await response.json();
  }

  // Make a move in the game
  static async makeMove(gameId, row, col, letter) {
    const response = await fetch(`${API_BASE}/move`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({
        gameId,
        row,
        col,
        letter
      })
    });

    if (!response.ok) {
      const errorData = await response.text();
      throw new Error(errorData || `Move failed: ${response.status}`);
    }

    return await response.json();
  }

}

export default GameService;