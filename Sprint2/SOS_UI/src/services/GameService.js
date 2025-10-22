// API service for SOS game
const API_BASE = '/api/SOS';

class GameService {
  // Parse error response and return user-friendly message
  static async getErrorMessage(response, defaultMessage) {
    try {
      // First try to get as text
      const responseText = await response.text();
      
      // If it's just a plain string (no JSON structure), return it directly
      if (responseText && !responseText.trim().startsWith('{')) {
        return responseText;
      }
      
      // If it looks like JSON, try to parse it
      if (responseText) {
        try {
          const errorData = JSON.parse(responseText);
          
          // Check if it's a validation error response
          if (errorData.errors) {
            const validationErrors = [];
            for (const [field, messages] of Object.entries(errorData.errors)) {
              validationErrors.push(`${field}: ${messages.join(', ')}`);
            }
            return validationErrors.join('; ');
          } else if (errorData.title) {
            return errorData.title;
          }
        } catch (parseError) {
          // If JSON parsing fails but we have text, return the text
          return responseText;
        }
      }
    } catch (error) {
      // If everything fails, return default
    }

    return defaultMessage || `Request failed: ${response.status}`;
  }

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