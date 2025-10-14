using System;
using System.Collections.Generic;

namespace PalindromeCheckerApi.Models
{
    public class SOS_Board
    {
        private Dictionary<(int row, int col), char> board;
        private int size;

        public int Size 
        { 
            get => size;
            private set 
            {
                if (value < 3 || value > 20)
                    throw new ArgumentOutOfRangeException(nameof(value), "Board size must be between 3 and 20");
                size = value;
            }
        }

      
        // Gets a copy of the internal board state for programmatic access, returns a defensive copy to prevent external modification of internal state.
    
        public Dictionary<(int row, int col), char> Board => new Dictionary<(int row, int col), char>(board);

        // Constructor
        public SOS_Board(int boardSize)
        {
            Size = boardSize;
            board = new Dictionary<(int row, int col), char>();
        }

        // used to set a value at a specific position
        public void SetCell(int row, int col, char value)
        {
            if (!IsValidPosition(row, col))
                throw new ArgumentOutOfRangeException("Invalid board position");

            if (value != 'S' && value != 'O')
                throw new ArgumentException("Value must be 'S' or 'O'");

            board[(row, col)] = value;
        }

        // to get the value at a specific position, included for future test cases
        public char? GetCell(int row, int col)
        {
            if (!IsValidPosition(row, col))
                throw new ArgumentOutOfRangeException("Invalid board position");

            return board.TryGetValue((row, col), out char value) ? value : null;
        }

        // using to check if a position is within bounds
        public bool IsValidPosition(int row, int col)
        {
            return row >= 0 && row < Size && col >= 0 && col < Size;
        }

        // Clear a specific cell, probably wont be used but included just in case
        public void ClearCell(int row, int col)
        {
            if (!IsValidPosition(row, col))
                throw new ArgumentOutOfRangeException("Invalid board position");

            board.Remove((row, col));
        }

        // Clear the entire board, also likely wont be used, but included just in case
        public void ClearBoard()
        {
            board.Clear();
        }

        // Check if a cell is empty
        public bool IsCellEmpty(int row, int col)
        {
            if (!IsValidPosition(row, col))
                return false;

            return !board.ContainsKey((row, col)); //looks weird, but is saying "if the board does not contain a value at this position, return true"
        }
    }
}