using System.Text.Json.Serialization;

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

        // Dont want to serialize the internal dictionary directly
        [JsonIgnore]
        public Dictionary<(int row, int col), char> Board => new Dictionary<(int row, int col), char>(board);

        // get/set JSON friendly dictionary representation
        [JsonPropertyName("Board")]
        public Dictionary<string, char> SerializableBoard
        {
            get
            {
                var result = new Dictionary<string, char>();
                foreach (var kvp in board)
                {
                    result[$"{kvp.Key.row},{kvp.Key.col}"] = kvp.Value;
                }
                return result;
            }
            set
            {
                board = new Dictionary<(int row, int col), char>();
                if (value != null)
                {
                    foreach (var kvp in value)
                    {
                        var parts = kvp.Key.Split(',');
                        if (parts.Length == 2 && 
                            int.TryParse(parts[0], out int row) && 
                            int.TryParse(parts[1], out int col))
                        {
                            board[(row, col)] = kvp.Value;
                        }
                    }
                }
            }
        }

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