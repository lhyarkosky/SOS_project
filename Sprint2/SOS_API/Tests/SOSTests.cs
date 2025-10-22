using NUnit.Framework;
using SOS_API.Controllers;
using SOS_API.Services;
using SOS_API.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using SOS_API.Models.GameStates;
using SOS_API.Models;
using System.Linq;

namespace SOS_API.Tests
{
    [TestFixture]
    public class SOSTests
    {
        private SOSController _controller = null!;
        private GameService _gameService = null!;

        [SetUp]
        public void SetUp()
        {
            _gameService = new GameService();
            _controller = new SOSController(_gameService);
        }

        [TearDown]
        public void TearDown()
        {
            // Clean up all games after each test to prevent interference between tests
            _gameService.CleanupOldGames(0);
        }


        // AC 1.1
        [Test]
        public void CreateGame_WithValidBoardDimension3x3_ReturnsSuccessResult()
        {
            var request = new CreateGameRequest { BoardSize = 3, GameMode = "Simple" };
            var result = _controller.CreateGame(request);
            Assert.IsInstanceOf<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            if (okResult != null)
            {
                Assert.IsNotNull(okResult.Value);
                Assert.AreEqual(200, okResult.StatusCode);
            }
            TearDown();
        }

        [Test]

        public void CreateGame_WithValidBoardDimension20x20_ReturnsSuccessResult()
        {
            var request = new CreateGameRequest { BoardSize = 20, GameMode = "Simple" };
            var result = _controller.CreateGame(request);
            Assert.IsInstanceOf<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            if (okResult != null)
            {
                Assert.IsNotNull(okResult.Value);
                Assert.AreEqual(200, okResult.StatusCode);
            }
            TearDown();
        }

        [Test]
        public void CreateGame_WithValidBoardDimensions_CreatesGameSuccessfully()
        {
            var request = new CreateGameRequest { BoardSize = 5, GameMode = "Simple" };
            var result = _controller.CreateGame(request);
            Assert.IsInstanceOf<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            if (okResult != null)
            {
                Assert.IsNotNull(okResult.Value);
                Assert.AreEqual(200, okResult.StatusCode);
            }
            TearDown();
        }


        // AC 1.2
        [Test]
        public void CreateGame_WithBoardSizeTooSmall_ThrowsException()
        {
            var request = new CreateGameRequest { BoardSize = 2, GameMode = "Simple" };
            var result = _controller.CreateGame(request);
            var statusResult = result.Result as ObjectResult;
            Assert.IsNotNull(statusResult);
            Assert.AreEqual(500, statusResult.StatusCode);
            Assert.IsTrue(statusResult.Value.ToString().Contains("Board size must be between 3 and 20"));
            TearDown();
        }

        [Test]
        public void CreateGame_WithBoardSizeTooLarge_ThrowsException()
        {
            var request = new CreateGameRequest { BoardSize = 21, GameMode = "Simple" };
            var result = _controller.CreateGame(request);
            var statusResult = result.Result as ObjectResult;
            Assert.IsNotNull(statusResult);
            Assert.AreEqual(500, statusResult.StatusCode);
            Assert.IsTrue(statusResult.Value.ToString().Contains("Board size must be between 3 and 20"));
            TearDown();
        }

        // AC 5.1 will be a UI test (im considering playwright)
        // AC 5.2
        [Test]
        public void CreateGame_WithSimpleGameMode_CreatesSimpleGameState()
        {
            var request = new CreateGameRequest { BoardSize = 5, GameMode = "Simple" };
            var gameState = _gameService.CreateGame(request.BoardSize, request.GameMode);
            Assert.IsInstanceOf<SimpleGameState>(gameState);
            TearDown();
        }

        // AC 5.3
        [Test]
        public void CreateGame_WithGeneralGameMode_CreatesGeneralGameState()
        {
            var request = new CreateGameRequest { BoardSize = 5, GameMode = "General" };
            var gameState = _gameService.CreateGame(request.BoardSize, request.GameMode);
            Assert.IsInstanceOf<GeneralGameState>(gameState);
            TearDown();
        }

        // AC 5.4 will be a UI test
        // AC 5.5
        [Test]
        public void CreateGame_WithInvalidGameModeText_ThrowsException()
        {
            var request = new CreateGameRequest { BoardSize = 5, GameMode = "InvalidMode" };
            var result = _controller.CreateGame(request);
            var statusResult = result.Result as ObjectResult;
            Assert.IsNotNull(statusResult);
            Assert.AreEqual(500, statusResult.StatusCode);
            Assert.IsTrue(statusResult.Value.ToString().Contains("Unknown game mode"));
            TearDown();
        }

        [Test]
        public void CreateGame_WithInvalidGameModeNumber_ThrowsException()
        {
            var request = new CreateGameRequest { BoardSize = 5, GameMode = "45" };
            var result = _controller.CreateGame(request);
            var statusResult = result.Result as ObjectResult;
            Assert.IsNotNull(statusResult);
            Assert.AreEqual(500, statusResult.StatusCode);
            Assert.IsTrue(statusResult.Value.ToString().Contains("Unknown game mode"));
            TearDown();
        }
        [Test]
        public void CreateGame_WithInvalidGameModeNone_ThrowsException()
        {
            var request = new CreateGameRequest { BoardSize = 5, GameMode = "" };
            var result = _controller.CreateGame(request);
            var statusResult = result.Result as ObjectResult;
            Assert.IsNotNull(statusResult);
            Assert.AreEqual(500, statusResult.StatusCode);
            Assert.IsTrue(statusResult.Value.ToString().Contains("Unknown game mode"));
            TearDown();
        }

        // AC 7.1 will be a UI test
        // AC 7.2
        [Test]
        public void CreateGame_WithInvalidBoardSizeAndGameMode_ThrowsException()
        {
            var request = new CreateGameRequest { BoardSize = 1, GameMode = "InvalidMode" };
            var result = _controller.CreateGame(request);
            var statusResult = result.Result as ObjectResult;
            Assert.IsNotNull(statusResult);
            if (statusResult != null)
            {
                Assert.AreEqual(500, statusResult.StatusCode);
                Assert.IsNotNull(statusResult.Value);
            }
            TearDown();
        }


        // AC 7.3
        [Test]
        public void CreateGame_WithValidSelections_ReturnsSuccess()
        {
            var request = new CreateGameRequest { BoardSize = 5, GameMode = "Simple" };
            var result = _controller.CreateGame(request);
            Assert.IsInstanceOf<OkObjectResult>(result.Result);
            TearDown();
        }

        // AC 8.1 & 8.3 will be a UI tests
        // AC 8.3 <valid move placement>
        [Test]
        public void MakeMove_WithSelectedLetterSOnEmptyCell_PlacesLetterSuccessfully()
        {
            var createRequest = new CreateGameRequest { BoardSize = 5, GameMode = "Simple" };
            var createResult = _controller.CreateGame(createRequest);
            var okResult = createResult.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            
            // retrieve the gameId
            var games = _gameService.GetAllGames();
            var gameId = games.First().GameId;
            
            var moveRequest = new MakeMoveRequest 
            { 
                GameId = gameId, 
                Row = 0, 
                Col = 0, 
                Letter = 'S' 
            };

            // Make a move on empty cell
            var moveResult = _controller.MakeMove(moveRequest);

            // Move should be successful
            Assert.IsInstanceOf<OkObjectResult>(moveResult.Result);
            var moveOkResult = moveResult.Result as OkObjectResult;
            Assert.IsNotNull(moveOkResult);
            if (moveOkResult != null)
            {
                Assert.AreEqual(200, moveOkResult.StatusCode);
            }
            
            // Verify letter placed on board
            var game = _gameService.GetGame(gameId);
            Assert.IsNotNull(game);
            if (game != null)
            {
                Assert.AreEqual('S', game.Board.GetCell(0, 0));
            }
        }

        [Test]
        public void MakeMove_WithSelectedLetterOOnEmptyCell_PlacesLetterSuccessfully()
        {
            var createRequest = new CreateGameRequest { BoardSize = 5, GameMode = "Simple" };
            var createResult = _controller.CreateGame(createRequest);
            var okResult = createResult.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            
            var games = _gameService.GetAllGames();
            var gameId = games.First().GameId;
            
            var moveRequest = new MakeMoveRequest 
            { 
                GameId = gameId, 
                Row = 1, 
                Col = 1, 
                Letter = 'O' 
            };

            // Make a move on an empty cell
            var moveResult = _controller.MakeMove(moveRequest);

            // Move should be successful
            Assert.IsInstanceOf<OkObjectResult>(moveResult.Result);
            var moveOkResult = moveResult.Result as OkObjectResult;
            Assert.IsNotNull(moveOkResult);
            if (moveOkResult != null)
            {
                Assert.AreEqual(200, moveOkResult.StatusCode);
            }
            
            // Verify the letter was placed on the board
            var game = _gameService.GetGame(gameId);
            Assert.IsNotNull(game);
            if (game != null)
            {
                Assert.AreEqual('O', game.Board.GetCell(1, 1));
            }
            TearDown();
        }


        // AC 8.4 <invalid move prevention>
        [Test]
        public void MakeMove_OnOccupiedCell_ThrowsInvalidOperationException()
        {
            var createRequest = new CreateGameRequest { BoardSize = 5, GameMode = "Simple" };
            var createResult = _controller.CreateGame(createRequest);
            var okResult = createResult.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            
            var games = _gameService.GetAllGames();
            var gameId = games.First().GameId;
            
            // First move - occupy cell (0,0)
            var firstMoveRequest = new MakeMoveRequest 
            { 
                GameId = gameId, 
                Row = 0, 
                Col = 0, 
                Letter = 'S' 
            };
            _controller.MakeMove(firstMoveRequest);
            
            // Second move - try to overwrite the same cell
            var secondMoveRequest = new MakeMoveRequest 
            { 
                GameId = gameId, 
                Row = 0, 
                Col = 0, 
                Letter = 'O' 
            };

            // Should return HTTP 500 with InvalidOperationException message
            var result = _controller.MakeMove(secondMoveRequest);
            var statusResult = result.Result as ObjectResult;
            Assert.IsNotNull(statusResult);
            if (statusResult != null)
            {
                Assert.AreEqual(500, statusResult.StatusCode);
                Assert.IsNotNull(statusResult.Value);
                if (statusResult.Value != null)
                {
                    Assert.IsTrue(statusResult.Value.ToString()!.Contains("Cell is already occupied"));
                }
            }
            
            // Verify the original letter is still there
            var game = _gameService.GetGame(gameId);
            Assert.IsNotNull(game);
            if (game != null)
            {
                Assert.AreEqual('S', game.Board.GetCell(0, 0));
            }
            TearDown();
        }
    }
}