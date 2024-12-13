# C# Graphing Calculator Documentation

## Project Overview

### Purpose
This project aims to allow users to quickly evaluate and examine the behavior of simple polynomials visually through the console.

### Features
- Allows the user to input multiple polynomials and display them in a single, easy-to-read graph.
- Users can add, edit, and delete the polynomials they wish to graph in an intuitive user interface.
- Saves the user’s graph if they are logged in, allowing them to continue working across multiple application sessions.
- Users can scroll through the graph to view areas farther from the origin, whether about the y-axis or the x-axis.
- Users can zoom in and out of the graph to adjust the size of the area they wish to view.
- Users can input a value of `x` to evaluate, where the resulting point will be plotted and highlighted in the graph. This works for all polynomials in the graph.
- Users can save a PNG file of their graph, displaying the graph in more detail.

### Target Audience
Students, educators, and any enthusiasts or professionals in STEM fields.

### Technology Stack
- **Language:** C#
- **IDE:** Visual Studio 2022

## Requirements

### Software Requirements
- **Operating System:** Windows 11 or later
- **Framework:** .NET SDK version 8.0 or higher
- **IDE:** Visual Studio 2022

### Installation Steps
1. Install an IDE like Visual Studio 2022.
2. Open the project folder in the IDE or terminal.
3. Build and run the application.

### System Requirements
- **Processor:** 1GHz or faster
- **Memory:** 2 GB of RAM
- **Disk Space:** 2 MB for application, plus additional space for save files

## File Handling Overview

### File Types and Purpose
- **`.txt` files** for storage
- **`.png` files** for graph screenshots

### File Operations
- **Read:** All users from the `userslist.txt` file at the start of the program.
- **Write:** Truncate the file to 0 bytes, then append each user’s name, password, and polynomials from their graph.

### Error Handling in File Operations
- If the file does not exist, create it.
- If the file cannot be opened, display the error message at the bottom of the screen.

## Code Structure

### Main Program Structure

#### Key Classes and Methods

**`class Term`**
- **Purpose:** Provides the structure for each term in a polynomial.
- **Methods:**
  - `ToString()`: Converts the term into a string.

**`class Polynomial`**
- **Purpose:** Represents polynomials as a list of terms.
- **Methods:**
  - `static CreatePolynomial(string)`: Converts a string into a polynomial object.
  - `static VerifyPolynomial(string)`: Verifies if a string can be converted into a Polynomial object.
  - `Evaluate(int x)`: Evaluates the polynomial at the value of `x`.
  - `ToString()`: Converts the polynomial into a string.

**`class Point`**
- **Purpose:** Represents a point in the graph.

**`class Graph`**
- **Purpose:** Handles graph visualization and control.
- **Methods:**
  - `Initialize()`: Plots the center point, x-axis, y-axis, and interval marks.
  - `PlotEquations(List<Polynomial>)`: Plots polynomials into the graph.
  - `InputX()`: Requests an x-value input from the user for evaluation.
  - `PlotX()`: Plots evaluated points into the graph.
  - `DisplayGraph()`: Displays the graph in the console.
  - `ControlGraph()`: Handles scrolling and zooming.
  - `ScreenshotGraph()`: Saves the graph as a PNG file using ScottPlot.

**`interface IAccountManager`**
- **Purpose:** Contains methods for account handling.
- **Methods:**
  - `DisplayMenu()`: Displays the main menu.
  - `ReadUser(string)`: Reads a user account.
  - `SaveUser()`: Saves the account details to `userslist.txt`.

**`abstract class Account`**
- **Purpose:** Handles account creation and editing.
- **Methods:**
  - `static DeleteUser(User)`: Deletes a user from `savedAccounts`.
  - `static SignIn()`: Logs in or switches accounts.
  - `static CreateAccount()`: Creates a new account.
  - `EditUser(User account)`: Edits account details.
  - `static LoadAllUsers()`: Loads accounts from `userslist.txt`.
  - `static SaveAllUsers()`: Saves accounts to `userslist.txt`.

**`class User`**
- **Purpose:** Handles user-specific operations.
- **Methods:**
  - `AddNewPolynomial(string)`: Adds a new polynomial.
  - `DeletePolynomial(Polynomial)`: Removes a polynomial.
  - `EditPolynomials()`: Manages polynomial operations.

**`class Admin`**
- **Purpose:** Handles admin-specific operations.
- **Methods:**
  - `DisplayUsers()`: Manages user accounts.

### Code Walkthrough

#### Displaying the Graph
- The graph is a 2D array of strings initialized using `Initialize()`. It plots axes, intervals, and center points.
- Polynomials are plotted with `PlotEquations(List<Polynomial>)`.
- `PlotX()` highlights evaluated points.
- The graph is displayed row-by-row in the console.
- A screenshot saves a detailed PNG file using ScottPlot.

#### File Handling
- `userslist.txt` is read at program start to populate `savedAccounts`.
- Changes to `savedAccounts` are saved back to `userslist.txt` after each iteration.

### Modularity and Reusability
- Classes implement interfaces to allow distinct behaviors for User and Admin accounts.
- Shared functionality is abstracted into reusable methods and interfaces.

## User Interface

### Design and Usability
- Options are displayed in a column with navigation using arrow keys.
- The graph uses colors to distinguish elements.
- Instructions are displayed for user guidance.

### Input/Output
- String inputs for usernames, passwords, and polynomials.
- Integer inputs for x-values.
- Key controls for navigation and graph manipulation.

### Error Messages
- Comprehensive error handling for invalid inputs and file operations.

## Challenges and Solutions

### Development Challenges
- Managing mutable string operations in a 2D graph representation.
- Handling unexpected behaviors in polynomial operations.

### Problem-Solving
- Used a 2D array of strings for graph representation.
- Improved polynomial handling with temporary object creation.

## Testing

### Test Cases
- Various input validation scenarios for usernames, passwords, polynomials, and x-values.
- File and directory manipulation tests.

### Results
- Program handles invalid inputs gracefully and prevents crashes.
- Robust input validation ensures accuracy in polynomial representation.

## Limitations
- Currently accepts only integer values for numeric inputs.
- Only supports polynomial functions.

## Future Enhancements

### Planned Features
- Graph presets.
- Compatibility with trigonometric functions.
- Support for fractions and floating-point inputs.

### Performance Improvements
- Optimize file saving operations.
- Improve code structure with advanced design patterns.
