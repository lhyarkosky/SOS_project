# Palindrome Checker

A full-stack web application that validates palindromes with a .NET backend API and React frontend.

## Table of Contents
- [Project Overview](#project-overview)
- [Project Structure](#project-structure)
- [Getting Started](#getting-started)
- [Architecture and Design Choices](#architecture-and-design-choices)
- [API Documentation](#api-documentation)
- [Testing](#testing)
- [Development Workflow](#development-workflow)
- [Troubleshooting](#troubleshooting)

## Project Overview

This application provides a web interface for checking if a text is a palindrome. It features:
- Case-insensitive palindrome validation
- Special character and number filtering
- Input validation for alphabetic characters
- Responsive web interface
- RESTful API design
- Comprehensive test coverage

## Project Structure

```
palindrome-checker/
├── PalindromeCheckerApi/           # Backend API
│   ├── BusinessLogic/              # Core business logic
│   │   └── TextValidator.cs        # Text validation implementation
│   ├── Controllers/                # API endpoints
│   │   └── PalindromeController.cs # Main controller
│   ├── Models/                     # Data models
│   │   └── ValidationResponse.cs   # API response model
│   ├── Repository/                 # Data access layer
│   │   └── Queries/               # SQL queries
│   │       └── example.sql        # SQL query templates
│   └── Tests/                      # Unit tests by component
│       └── BusinessLogic/          # Business logic tests
├── palindrome-checker-ui/          # Frontend React app
│   ├── public/                     # Static assets
│   └── src/                        # React source code
```

### Repository Layer Structure

The application includes a Repository layer designed for future database integration:

```
Repository/
└── Queries/
    └── example.sql    # Template for future database operations
```

This structure follows the Repository pattern and is designed to support:
- Game state persistence
- User data storage
- Historical palindrome checks
- Future feature expansions

## Getting Started

### Prerequisites
- .NET 8.0 SDK
- Node.js (v14 or later)
- npm (v6 or later)

### Running the Backend API

1. Navigate to the API directory:
   ```powershell
   cd PalindromeCheckerApi
   ```

2. Build and run the API:
   ```powershell
   dotnet run
   ```

The API will be available at:
- HTTPS: `https://localhost:7071`
- HTTP: `http://localhost:7070`

While Swagger is configured in the project, you can directly test the API using the React frontend or tools like Postman. The endpoint is:
```
GET https://localhost:7071/api/Palindrome/validate?text=yourtext
```

### Running the Frontend UI

1. Navigate to the UI directory:
   ```powershell
   cd palindrome-checker-ui
   ```

2. Install dependencies:
   ```powershell
   npm install
   ```

3. Start the development server:
   ```powershell
   npm start
   ```

The UI will automatically open in your default browser at `http://localhost:3000`.

## Architecture and Design Choices

### Backend Architecture

1. **Clean Architecture Pattern**
   - Separation of concerns with distinct layers
   - Business logic isolated in dedicated namespace
   - Repository pattern for data access
   - Easy to test and maintain

2. **Repository Layer Design**
   - Prepared for database integration
   - SQL queries isolated in dedicated files
   - Clear separation between data access and business logic
   - Structured for future game state persistence
   - Follows the repository pattern for data abstraction

2. **Static Utility Design**
   - `TextValidator` implemented as a static class
   - Stateless operations for text validation
   - Thread-safe implementation
   - Easy to scale horizontally

3. **RESTful API Design**
   - GET endpoint for validation (idempotent operation)
   - Query parameter for input text
   - Structured JSON responses
   - Proper HTTP status codes

4. **Input Validation**
   - Server-side validation for all inputs
   - Clear error messages
   - Null handling
   - Empty string handling

5. **CORS Configuration**
   - Configured for local development
   - Specific origin (http://localhost:3000) allowed
   - Required headers enabled

### Frontend Architecture

1. **React Components**
   - Functional components with hooks
   - Clean separation of concerns
   - Responsive design

2. **API Integration**
   - Axios for HTTP requests
   - Error handling
   - Loading states

## API Documentation

### Testing the API

You can test the API using:
1. The React frontend at `http://localhost:3000`
2. Direct HTTP requests to `https://localhost:7071/api/Palindrome/validate?text=yourtext`
3. Tools like Postman or cURL

### Endpoints

#### GET /api/Palindrome/validate
Validates if the input text is a palindrome.

**Parameters:**
- `text` (query string): The text to validate

**Response:**
```json
{
  "isValid": boolean,
  "message": string
}
```

**Example:**
```http
GET /api/Palindrome/validate?text=racecar
```

**Response:**
```json
{
  "isValid": true,
  "message": "This is a palindrome!"
}
```

## Testing

The application includes comprehensive unit tests for the backend business logic. Tests are organized by component type.

### Running Tests

```powershell
cd PalindromeCheckerApi
dotnet test
```

### Test Coverage
- Basic palindrome validation
- Case sensitivity
- Special characters and numbers
- Empty strings
- Null inputs
- Input validation

## Development Workflow

1. **Backend Changes**
   - Make changes in appropriate layer
   - Run tests: `dotnet test`
   - Start API: `dotnet run`
   - For database changes:
     - Add new queries in `Repository/Queries/`
     - Follow the repository pattern for data access
     - Keep SQL separate from C# code

2. **Frontend Changes**
   - Make changes in React components
   - Start development server: `npm start`
   - Changes hot-reload automatically

## Troubleshooting

1. **API Not Starting**
   - Check port availability (7071, 7070)
   - Verify .NET SDK installation
   - Check for running instances: `dotnet build --no-incremental`

2. **Frontend Issues**
   - Clear npm cache: `npm cache clean --force`
   - Reinstall dependencies: `rm -rf node_modules && npm install`
   - Check API URL in configuration

3. **CORS Issues**
   - Verify API is running on expected port
   - Check CORS configuration in Program.cs
   - Verify frontend URL matches CORS policy