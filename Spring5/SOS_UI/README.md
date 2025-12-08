# SOS Game UI

A React-based user interface for the SOS game that connects to the SOS API backend.

## Features

- **Interactive Game Board**: Click to place S or O letters
- **Real-time Updates**: Game state syncs with the API
- **Game Statistics**: View scores, current player, and game status
- **Responsive Design**: Works on desktop and mobile devices
- **Simple & General Modes**: Support for both game modes
- **SOS Detection**: Visual feedback when SOS sequences are formed

## Getting Started

### Prerequisites
- Node.js (v14 or later)
- npm (v6 or later)
- SOS API running on https://localhost:7071

### Installation

1. Navigate to the UI directory:
   ```bash
   cd SOS_UI
   ```

2. Install dependencies:
   ```bash
   npm install
   ```

3. Start the development server:
   ```bash
   npm start
   ```

The app will open at http://localhost:3000 and proxy API calls to https://localhost:7071.

## Game Rules

### Simple Mode
- First player to form an SOS sequence wins immediately
- Game ends as soon as one SOS is found

### General Mode  
- Play continues until the board is full
- Player with the most SOS sequences wins
- Players get extra turns when they form an SOS

## How to Play

1. **Start a New Game**: Choose board size (3x3 to 10x10) and game mode
2. **Select Letter**: Choose S or O before making a move
3. **Make Moves**: Click empty cells to place your letter
4. **Form SOS**: Try to create sequences of S-O-S horizontally, vertically, or diagonally
5. **Win the Game**: Achieve victory conditions based on the selected game mode

## API Integration

The UI communicates with the SOS API using these endpoints:

- `POST /api/SOS/create` - Create new game
- `POST /api/SOS/move` - Make a move
- `GET /api/SOS/{gameId}` - Get game state
- `DELETE /api/SOS/{gameId}` - Delete game
- `GET /api/SOS/all` - Get all games

## Project Structure

```
src/
├── components/          # React components
│   ├── GameSetup.js     # Game creation form
│   ├── GameBoard.js     # Interactive game board
│   └── GameInfo.js      # Game status and scores
├── services/            # API integration
│   └── GameService.js   # API calls wrapper
├── App.js               # Main application component
└── index.js             # Application entry point
```

## Available Scripts

- `npm start` - Start development server
- `npm build` - Build for production
- `npm test` - Run tests
- `npm eject` - Eject from Create React App

## Technologies Used

- **React 18** - UI framework
- **CSS3** - Styling and animations
- **Fetch API** - HTTP client for API communication
- **Create React App** - Project setup and build tools