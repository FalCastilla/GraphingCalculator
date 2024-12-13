/*
 * FINAL PROJECT - CASTILLA
 * H2
 */

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;
using System.IO;
using ModIO;
using ScottPlot;
using System.Diagnostics;

namespace GraphingCalculator
{

    class Term
    {
        private int coefficient;
        private int exponent;

        public Term()
        {
            this.coefficient = 0;
            this.exponent = 0;
        }

        public Term(int coeff, int exp)
        {
            this.coefficient = coeff;
            this.exponent = exp;
        }

        public int Coefficient
        {
            get { return this.coefficient; }
            set { this.coefficient = value; }
        }
        public int Exponent
        {
            get { return exponent; }
            set { this.exponent = value; }
        }

        public override string ToString()
        {
            StringBuilder outputTerm = new StringBuilder();

            switch(this.coefficient)
            {
                case 0:
                    return "";
                case 1:
                case -1:
                    if (this.exponent == 0)
                    {
                        return $"{this.coefficient}";
                    }
                    else
                    {
                        if (this.coefficient < 0)
                        {
                            outputTerm.Append("-");
                        }
                        else
                        {
                            outputTerm.Append("");
                        }
                    }
                    break;
                default:
                    outputTerm.Append($"{this.coefficient}");
                    break;
            }
            if (this.exponent > 0)
            {
                outputTerm.Append("x");
            }

            if (this.exponent > 1)
            {
                outputTerm.Append($"^{this.exponent}");
            }

            return outputTerm.ToString();
        }

        public Term Absolute()
        {
            Term temp = new Term();

            temp.coefficient = Math.Abs(this.coefficient);
            temp.exponent = this.exponent;

            return temp;
        }
    }

    class Polynomial
    {
        private List<Term> terms = new List<Term>();
        private static Regex polynomialPattern = new Regex(@"([+-]?\s?\d*)x(?:\^(\d+))?|([+-]?\s?\d+)");
        public List<Term> Terms
        {
            get { return this.terms; }
            set { this.terms = value; }
        }

        public static Polynomial CreatePolynomial(string polynomial)
        {
            Polynomial newPolynomial = new Polynomial();
            MatchCollection matches = polynomialPattern.Matches(polynomial);
            string constantString = string.Empty;
            int constant, coefficient;

            foreach (Match match in matches)
            {
                coefficient = 0;
                Term newTerm = new Term();
                string coefficientString = match.Groups[1].Value;
                string exponentString = match.Groups[2].Value;
                constantString = match.Groups[3].Value;

                if (string.IsNullOrWhiteSpace(coefficientString) || coefficientString.Any(char.IsDigit) == false)
                {
                    coefficient = coefficientString.Contains('-') ? -1 : 1;
                }
                else
                {
                    coefficientString = coefficientString.Replace(" ", "");
                }

                if (Math.Abs(coefficient) != 1)
                {
                    coefficient = int.Parse(coefficientString);
                }
                newTerm.Coefficient = coefficient;
                newTerm.Exponent = (string.IsNullOrWhiteSpace(exponentString)) ? 1 : int.Parse(exponentString);

                if (string.IsNullOrWhiteSpace(constantString) == true)
                {
                    newPolynomial.Terms.Add(new Term(newTerm.Coefficient, newTerm.Exponent));
                }
            }
            if (string.IsNullOrWhiteSpace(constantString) || constantString.Any(char.IsDigit) == false)
            {
                coefficient = constantString.Contains('-') ? -1 : 1;
            }
            else
            {
                constantString = constantString.Replace(" ", "");
            }
            constant = (string.IsNullOrWhiteSpace(constantString)) ? 0 : int.Parse(constantString);
            newPolynomial.Terms.Add(new Term(constant, 0));

            return newPolynomial;
        }


        public static int VerifyPolynomial(string  polynomial)
        {
            // 1 for invalid, 0 for valid
            int result = 1;

            for (int i = 0; i < polynomial.Length; i++)
            {
                switch (polynomial[i])
                {
                    case 'x':
                    case '+':
                    case '-':
                    case '^':
                    case ' ':
                        result = 0;
                        break;
                    default:
                        if (char.IsDigit(polynomial[i]) == false)
                        {
                            return 1;
                        }
                        result = 0;
                        break;
                }
                if (polynomial[i] == 'x')
                {
                    if (i < polynomial.Length - 2)
                    {
                        result = 0;
                        switch (polynomial[i + 1])
                        {
                            case ' ':
                                if (polynomial[i + 2] == '+' || polynomial[i + 2] == '-')
                                {
                                    result = 0;
                                }
                                break;
                            case '+':
                            case '-':
                            case '^':
                                result = 0;
                                break;
                            default:
                                return 1;
                        }
                    }
                    else if (i < polynomial.Length - 1)
                    {
                        result = 1;
                    }
                    if (i > 0 && Char.IsDigit(polynomial[i - 1]) == false && polynomial[i - 1] != ' ' && polynomial[i - 1] != '-')
                    {
                        return 1;
                    }
                }
            }

            return result;
        }

        public double Evaluate(double x)
        {
            double total = 0;
            foreach (Term term in this.terms)
            {
                total += term.Coefficient * Math.Pow(x, term.Exponent);
            }

            return total;
        }
        public override string ToString()
        {
            StringBuilder sbPolynomial = new StringBuilder();
            bool firstTerm = true;
            foreach (Term term in this.terms)
            {
                if (firstTerm == true)
                {
                    sbPolynomial.Append(term.ToString());
                    firstTerm = false;
                }
                else if (string.IsNullOrEmpty(term.ToString()) == false)
                {
                    sbPolynomial.Append((term.Coefficient < 0) ? " - " : " + ");
                    sbPolynomial.Append(term.Absolute().ToString());
                }
            }

            return sbPolynomial.ToString();
        }
    }

    class Point
    {
        private double x_coord;
        private double y_coord;

        public Point(double x, double y)
        {
            this.x_coord = x;
            this.y_coord = y;
        }
        public Point()
        {
            this.x_coord = 0;
            this.y_coord = 0;
        }

        public double X_Coord
        {
            get { return x_coord; }
            set { x_coord = value; }
        }

        public double Y_Coord
        {
            get { return y_coord; }
            set { y_coord = value; }
        }
    }

    class Graph : IDirectoryHandler
    {
        private const int GRAPH_WIDTH = 101;
        private const int GRAPH_HEIGHT = 29;
        private const int GRAPH_LOCATION = 10;
        private int yInterval = 1;
        private double xInterval = 1;
        private int yOffset = 0;
        private int xOffset = 0;
        private int inputX, outputY;
        public bool beginPlotting = false;
        private Point centerPoint;
        public static bool graphIsSelected = false;
        private List<Polynomial> polynomialsUsed = new List<Polynomial>();
        Plot imageGraph = new();
        
        private IO.textColor[] lineColors =
        {
            IO.textColor.Blue,
            IO.textColor.Orange,
            IO.textColor.Green,
            IO.textColor.Red,
            IO.textColor.Magenta,
            IO.textColor.DarkYellow,
            IO.textColor.Pink,
            IO.textColor.DirtyColorIDK,
            IO.textColor.DirtyYellow,
            IO.textColor.Cyan,
        };


        public int WIDTH
        {
            get { return GRAPH_WIDTH; }
        }

        public int HEIGHT
        {
            get { return GRAPH_HEIGHT; }
        }

        public int GraphLocation
        {
            get { return GRAPH_LOCATION; }
        }

        private string[,] graphDisplay;

        public Graph()
        {
            
            this.Initialize();
        }
        public int DeleteDirectory(string filePath)
        {
            int exitCode = 0;
            try
            {

                if (Directory.Exists(filePath))
                {
                    Directory.Delete(filePath, recursive: true);
                }
                else
                {

                    exitCode = 1;
                }
            }
            catch (Exception ex)
            {
                exitCode = 1;
                Console.WriteLine($"Error: {ex.Message}");
            }

            return exitCode;
        }
        
        public void CreateDirectory(string path)
        {
            string directory = Path.GetDirectoryName(path);

            if (Directory.Exists(directory) == false)
            {
                Directory.CreateDirectory(directory);
            }
        }

        public void Initialize()
        {
            string xAxis = IO.ApplyColor("|", IO.textColor.Black, IO.textColor.White);
            string yAxis = IO.ApplyColor("-", IO.textColor.Black, IO.textColor.White);
            string center = IO.ApplyColor("+", IO.textColor.Black, IO.textColor.White);
            centerPoint = new Point(GRAPH_WIDTH / 2 + xOffset, GRAPH_HEIGHT / 2 + yOffset);
            graphDisplay = new string[GRAPH_HEIGHT, GRAPH_WIDTH];
            bool centerWithinXBounds = centerPoint.X_Coord > 0 && centerPoint.X_Coord < GRAPH_WIDTH;
            bool centerWithinYBounds = centerPoint.Y_Coord > 0 && centerPoint.Y_Coord < GRAPH_HEIGHT;

            for (int i = 0, yIntervalCount = (int)centerPoint.Y_Coord * yInterval; i < GRAPH_HEIGHT; i++, yIntervalCount -= yInterval)
            {
                string yIntervalMark = $"{yIntervalCount}";

                for (int j = 0, xIntervalCount = 0 - (int)centerPoint.X_Coord * (int)xInterval; j < GRAPH_WIDTH; j++, xIntervalCount += (int)xInterval)
                {
                    string xIntervalMark = $"{xIntervalCount}";

                    if (centerWithinYBounds)
                    {
                        graphDisplay[(int)centerPoint.Y_Coord, j] = yAxis;
                    }
                    if (centerWithinXBounds)
                    {
                        graphDisplay[i, (int)centerPoint.X_Coord] = xAxis;
                    }
                    if (centerWithinXBounds && centerWithinYBounds)
                    {
                        graphDisplay[(int)centerPoint.Y_Coord, (int)centerPoint.X_Coord] = center;
                    }
                    if (graphDisplay[i, j] == null)
                    {
                        graphDisplay[i, j] = IO.SetBackground(" ", IO.textColor.White);
                    }
                    if (i % 2 == 0 && j < yIntervalMark.Length)
                    {
                        int start = (int)centerPoint.X_Coord;
                        if (centerPoint.X_Coord < 0)
                        {
                            start = 0;
                        }
                        if (centerPoint.X_Coord + yIntervalMark.Length + 1 > GRAPH_WIDTH)
                        {
                            start = GRAPH_WIDTH - yIntervalMark.Length - 1;
                        }
                        string intervalDigit = IO.ApplyColor($"{yIntervalMark[j]}", IO.textColor.LightGray, IO.textColor.White);
                        graphDisplay[i, start + j + 1] = intervalDigit;
                    }
                    if (j % 5 == 0 && xIntervalMark.Length < GRAPH_WIDTH - j)
                    {
                        int start = (int)centerPoint.Y_Coord;
                        if (centerPoint.Y_Coord < 0)
                        {
                            start = 0;
                        }
                        if (centerPoint.Y_Coord + 2 > GRAPH_HEIGHT)
                        {
                            start = GRAPH_HEIGHT - 2;
                        }
                        for (int k = 0; k < xIntervalMark.Length; k++)
                        {
                            graphDisplay[start + 1, j + k] = IO.ApplyColor($"{xIntervalMark[k]}", IO.textColor.LightGray2, IO.textColor.White);
                        }
                    }
                        
                }
            }
        }
        public void DisplayGraph()
        {
            this.PlotEquations(User.CurrentUser.SavedPolynomials);
            try
            {
                PlotX();
            }
            catch (Exception e)
            {
                IO.HandleException(e);
            }
            Console.SetCursorPosition(0, GRAPH_LOCATION);
            for (int i = 0; i < GRAPH_HEIGHT; i++)
            {
                for (int j = 0; j < GRAPH_WIDTH; j++)
                {
                    Console.Write(graphDisplay[i, j]);
                }
                Console.WriteLine();
            }

        }

        public void PlotEquations(List<Polynomial> polynomials)
        {
            polynomialsUsed = polynomials;
            int noOfColors = lineColors.Length;
            IO.textColor currentColor;
            int i = 0;
            foreach (Polynomial polynomial in polynomials)
            {
                currentColor = lineColors[polynomials.IndexOf(polynomial) % noOfColors];
                string mark = IO.SetForeground("\x1b[1m*\x1b[0m", currentColor, IO.textColor.Black);
                mark = IO.SetBackground(mark, IO.textColor.White);
                for (double x = 0 - centerPoint.X_Coord * (int)xInterval; x < (GRAPH_WIDTH - centerPoint.X_Coord) * (int)xInterval - xInterval; x+= 0.01)
                {
                    int yValue = (int)Math.Round(polynomial.Evaluate(x));
                    //int yValue = (int)Math.Round(polynomial.Evaluate(x), MidpointRounding.AwayFromZero);
                    if (yValue < centerPoint.Y_Coord * yInterval && yValue > (centerPoint.Y_Coord - GRAPH_HEIGHT) * yInterval + yInterval)
                    {
                        try
                        {
                            graphDisplay[(int)centerPoint.Y_Coord - (yValue / yInterval), ((int)Math.Round(x) / (int)xInterval) + (int)centerPoint.X_Coord] = mark;
                        }
                        catch (Exception e)
                        {
                            IO.HandleException(e);
                            Console.WriteLine($"yValue = {yValue}, XCoord = {centerPoint.X_Coord}, interval = {xInterval}, x = {x}");
                        }
                    }
                }
                i++;
            }
        }

        private void ScreenshotGraph()
        {
            const int MAX_STRING_SIZE = 30;
            int leftLimit = 0 - (int)centerPoint.X_Coord * (int)xInterval;
            int rightLimit = (int)((GRAPH_WIDTH - centerPoint.X_Coord) * (int)xInterval - xInterval);
            int topLimit = (int)centerPoint.Y_Coord * yInterval;
            int bottomLimit = (int)(centerPoint.Y_Coord - GRAPH_HEIGHT) * yInterval + yInterval;
            int firstX, countX;
            bool showXAxis = false;
            bool showYAxis = false;
            int resWidth = 1920;
            int resHeight = 1080;
            string screenshotsPathRaw = $"Saved Graphs\\{User.SavedAccounts[User.LoggedAccount].Name}\\";
            StringBuilder screenshotsPath = new StringBuilder(screenshotsPathRaw);
            double[] xs;
            List<double> ys = new List<double>();
            imageGraph = new();

            if (polynomialsUsed.Count == 0)
            {
                Console.SetCursorPosition(0, GRAPH_LOCATION - 1);
                Console.WriteLine("Nothing to plot!");
                return;
            }

            for (int i = 0; i < polynomialsUsed.Count; i++)
            {
                ys.Clear();
                if (screenshotsPath.Length < MAX_STRING_SIZE)
                {
                    screenshotsPath.Append($"{((i == 0) ? "" : ", ")}{polynomialsUsed[i].ToString()}");
                }
                CreateDirectory(screenshotsPathRaw);

                try
                {
                    firstX = 0 - (int)centerPoint.X_Coord * (int)xInterval;
                    countX = GRAPH_WIDTH * (int)xInterval * 100;
                }
                catch (ArgumentOutOfRangeException)
                {
                    firstX = 0;
                    countX = 0;
                }
                
                xs = Generate.Consecutive(countX, 0.01 ,firstX);
                foreach (double x in xs)
                {
                    double yValue = polynomialsUsed[i].Evaluate(x);
                    ys.Add(yValue);
                    if (x == 0)
                    {
                        showXAxis = true;
                    }
                    if (rightLimit > 0 && leftLimit < 0)
                    {
                        showYAxis = true;
                    }
                }
                imageGraph.Axes.SetLimits
                    (
                        left: leftLimit,
                        right: rightLimit,
                        top: topLimit,
                        bottom: bottomLimit
                    );
                var plt = imageGraph.Add.SignalXY(xs, ys.ToArray());
                plt.LegendText = polynomialsUsed[i].ToString();
                plt.LineWidth = 5;

                if (beginPlotting == true)
                {
                    double yValue = polynomialsUsed[i].Evaluate(inputX);
                    imageGraph.Add.Marker(inputX, yValue, size: 20, color: new Color(69, 246, 56));
                    var text = imageGraph.Add.Text($"({inputX}, {yValue})", inputX, yValue);
                    text.LabelFontSize = 20;
                    text.LabelFontColor = new Color(0, 0, 0);
                    text.OffsetX = 2;
                    text.LabelBold = true;
                    text.LabelBorderColor = new Color(0, 0, 0);
                    text.LabelBackgroundColor = new Color(48, 233, 35);
                }
            }
            screenshotsPath.Append(".png");
            imageGraph.ShowLegend();
            imageGraph.Legend.FontSize = 48;
            imageGraph.Title("C# Graphing Calculator by Fal Castilla", size: 50);
            if (showYAxis == true)
            {
                imageGraph.Add.VerticalLine(0, color: new Color(0, 0, 0));
            }
            if (showXAxis == true)
            {
                imageGraph.Add.HorizontalLine(0, color: new Color(0, 0, 0));
            }
            string currentDirectory = Directory.GetCurrentDirectory();
            imageGraph.SavePng(screenshotsPath.ToString(), resWidth, resHeight);
            Console.SetCursorPosition(0, GRAPH_LOCATION - 1);
            Console.WriteLine($"Saved graph screenshot at {currentDirectory}\\{screenshotsPath.ToString().Substring(0, screenshotsPath.ToString().IndexOf('\\') + 1)}");
            OpenPNG(screenshotsPath.ToString());
        }

        private void OpenPNG(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = filePath,
                        UseShellExecute = true
                    });
                }
                else
                {
                    Console.SetCursorPosition(0, GRAPH_LOCATION - 2);
                    Console.WriteLine("File Not Found.");
                }
            }
            catch (Exception e)
            {
                IO.HandleException(e);
            }
            
        }

        public void ControlGraph()
        {
        const double X_INTERVAL_JUMP = (double)GRAPH_HEIGHT / GRAPH_WIDTH;
        const double MIN_X_INTERVAL = 1;
        const int MAX_INTERVAL = 20;
            const int MIN_INTERVAL = 1;
            ConsoleKey keyPressed = IO.StartSelecting();
            IO.ClearConsoleLine(GRAPH_LOCATION - 1);
            switch (keyPressed)
            {
                case ConsoleKey.OemComma:
                    if (yInterval > MIN_INTERVAL)
                    {
                        yInterval--;
                        if (xInterval - X_INTERVAL_JUMP < MIN_X_INTERVAL)
                        {
                            break;
                        }
                        xInterval -= X_INTERVAL_JUMP;
                    }
                    break;
                case ConsoleKey.OemPeriod:
                    if (yInterval < MAX_INTERVAL)
                    {
                        yInterval++;
                        xInterval += X_INTERVAL_JUMP;
                    }
                    break;
                case ConsoleKey.UpArrow:
                    yOffset++;
                    break;
                case ConsoleKey.DownArrow:
                    yOffset--;
                    break;
                case ConsoleKey.LeftArrow:
                    xOffset++;
                    break;
                case ConsoleKey.RightArrow:
                    xOffset--;
                    break;
                case ConsoleKey.Escape:
                    graphIsSelected = false;
                    beginPlotting = false;
                    break;
                case ConsoleKey.Enter:
                    InputX();
                    break;
                case ConsoleKey.Spacebar:
                    yOffset = 0;
                    xOffset = 0;
                    break;
                case ConsoleKey.Tab:
                    if (User.LoggedAccount == -1)
                    {
                        Console.SetCursorPosition(0, GRAPH_LOCATION - 1);
                        Console.WriteLine("Log in to screenshot and save your graphs!");
                        break;
                    }
                    ScreenshotGraph();
                    break;
            }
            this.Initialize();
        }

        private void InputX()
        {
            string input;

            
            do
            {
                Console.SetCursorPosition(GRAPH_WIDTH + 2, GRAPH_LOCATION);
                Console.Write("\x1b[0Kx = ");
                input = Console.ReadLine();
            } while (int.TryParse(input, out inputX) == false);

            beginPlotting = true;
        }

        private void PlotX()
        {
            IO.textColor currentColor;
            int noOfColors = lineColors.Length;
            StringBuilder output = new StringBuilder();
            if (beginPlotting == true)
            {
                Console.SetCursorPosition(GRAPH_WIDTH + 2, GRAPH_LOCATION + 1);
            }
            else
            {
                Console.SetCursorPosition(GRAPH_WIDTH + 2, GRAPH_LOCATION);
                Console.WriteLine("\x1b[0K");
            }

            foreach (Polynomial polynomial in polynomialsUsed)
            {
                currentColor = lineColors[polynomialsUsed.IndexOf(polynomial) % noOfColors];
                outputY = (int)polynomial.Evaluate(inputX);
                output.Append($"{polynomial.ToString()}");
                if (beginPlotting == true)
                {
                    output.Insert(0, $"{IO.SetForeground($"{outputY}", IO.textColor.NeonGreen, currentColor)} = ");
                    output.Replace("x", $"({inputX})");
                }
                else
                {
                    output.Insert(0, "y = ");
                }
                Console.SetCursorPosition(GRAPH_WIDTH + 2, Console.CursorTop);
                
                Console.WriteLine("\x1b[0K" + IO.SetForeground(output.ToString(), currentColor));
                output.Clear();
                if (beginPlotting == false)
                {
                    continue;
                }
                if (inputX > 0 - centerPoint.X_Coord * (int)xInterval
                    && inputX < (GRAPH_WIDTH - centerPoint.X_Coord) * (int)xInterval - xInterval
                    && outputY < centerPoint.Y_Coord * yInterval
                    && outputY > (centerPoint.Y_Coord - GRAPH_HEIGHT) * yInterval + yInterval)
                {
                    graphDisplay[(int)centerPoint.Y_Coord - (outputY / yInterval), (inputX / (int)xInterval) + (int)centerPoint.X_Coord] = IO.ApplyColor("@", IO.textColor.NeonGreen, IO.textColor.White);
                }
            }
        }
        
    }

    interface IAccountManager
    {
        public void LogIn();
        public void LogOut();
        public void DisplayMenu();
        public void ReadUser(string line);
        public void SaveUser();
    }

    interface IDirectoryHandler
    {
        public void CreateDirectory(string path);
        public int DeleteDirectory(string path);
    }

    abstract class Account : IAccountManager
    {
        protected string name;
        protected string password;
        private static List<User> savedAccounts = new List<User>();
        private static int loggedAccount = -1;
        private static User currentUser = new User();
        public static Admin adminAccount = new Admin();
        private const string usersListFile = "userslist.txt";
        protected static string adminUsername = "admin";
        protected static string adminPassword = "admin";

        public abstract void LogIn();
        public abstract void LogOut();
        public abstract void DisplayMenu();
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public string Password
        {
            get { return password; }
            set { password = value; }
        }
        public static List<User> SavedAccounts
        {
            get { return savedAccounts; }
        }
        public static User CurrentUser
        {
            get { return currentUser; }
            set { currentUser = value; }
        }
        public static int LoggedAccount
        {
            get { return loggedAccount; }
            set { loggedAccount = value; }
        }

        public static bool verifyUsername(string username)
        {
            bool usernameExists = false;
            foreach (User user in savedAccounts)
            {
                if (user.Name == username)
                {
                    Console.SetCursorPosition(0, 10);
                    Console.WriteLine("Username already exists!");
                    usernameExists = true;
                    break;
                }
            }
            return usernameExists;
        }
        protected static void DeleteUser(User userToDelete)
        {
            SavedAccounts.Remove(userToDelete);
        }
        public static void SignIn()
        {
            string newUsername = null;
            string newPassword = null;
            bool success = false;
            int accountIndex = -1;
            string menuHeader = "LOG IN";

            if (loggedAccount != -1)
            {
                menuHeader = "SWITCH ACCOUNTS";
            }

            Console.Clear();
            IO.ResetSelect();

            while (true)
            {
                Console.SetCursorPosition(0, 0);

                IO.DisplayOptions(menuHeader, "Enter username", "Enter password", "<Confirm>", "\nCancel");

                switch (IO.SelectedOption)
                {
                    case 0:
                        IO.ModifyOption(0);
                        newUsername = Console.ReadLine();

                        foreach (var account in savedAccounts)
                        {
                            if (account != null && account.name == newUsername)
                            {
                                accountIndex = savedAccounts.IndexOf(account);
                                break;
                            }
                        }
                        break;

                    case 1:
                        IO.ModifyOption(1);
                        newPassword = Console.ReadLine();
                        break;

                    case 2:
                        if (newUsername == adminUsername && newPassword == adminPassword)
                        {
                            accountIndex = -2;
                        }
                        if (accountIndex == -1 || (accountIndex != -2 && newPassword != savedAccounts[accountIndex].Password ))
                        {
                            Console.SetCursorPosition(0, 9);
                            Console.WriteLine("Invalid username or password!");
                        }
                        else
                        {
                            success = true;
                            for (int i = 0; i < savedAccounts.Count; i++)
                            {
                                savedAccounts[i].LogOut();
                            }
                        }
                        break;
                }
                if (IO.SelectedOption == 3 || (IO.SelectedOption == 2 && success == true) || IO.SelectedOption == -1)
                {
                    break;
                }
            }

            if (accountIndex == -2)
            {
                adminAccount.LogIn();
            }
            else if (accountIndex != -1)
            {
                savedAccounts[accountIndex].LogIn();
            }
        }
        public static void CreateAccount()
        {
            User newUser = new User();
            bool noName = true;
            bool noPass = true;
            Console.Clear();

            IO.ResetSelect();
            while (true)
            {
                for (int i = 3; i >= 0; i--)
                {
                    IO.ClearConsoleLine(i);
                }
                if (noName == false)
                {
                    IO.DisplayAtOption(0, $"    : {newUser.name}");
                }
                if(noPass == false)
                {
                    IO.DisplayAtOption(1, $"    : {newUser.password}");
                }

                IO.DisplayOptions("CREATE ACCOUNT", $"Enter username", $"Enter password", "<Confirm>", "\nCancel");

                switch (IO.SelectedOption)
                {
                    case 0:
                        IO.ModifyOption(0);
                        newUser.name = Console.ReadLine();
                        break;
                    case 1:
                        IO.ModifyOption(1);
                        newUser.password = Console.ReadLine();
                        break;
                    case 3:
                        newUser.name = string.Empty;
                        newUser.password = string.Empty;
                        break;
                }
                noName = string.IsNullOrEmpty(newUser.name);
                noPass = string.IsNullOrEmpty(newUser.password);

                if (IO.SelectedOption == -1)
                {
                    break;
                }

                if (IO.SelectedOption > 1)
                {
                    IO.ResetSelect();
                    bool usernameExists = verifyUsername(newUser.Name);
                    if (usernameExists == true)
                    {
                        usernameExists = false;
                        continue;
                    }
                    if (noName == true && noPass == true)
                    {
                        DeleteUser(newUser);
                    }
                    else if (noName == true || noPass == true)
                    {
                        Console.SetCursorPosition(0, 11);
                        Console.WriteLine("Please enter both username and password.");
                        continue;
                    }
                    else
                    {
                        savedAccounts.Add(newUser);
                        if (loggedAccount != -2)
                        {
                            newUser.LogIn();
                        }
                    }
                    break;
                }
            }
            Console.Clear();
        }
        public void ReadUser(string line)
        {
            if (string.IsNullOrEmpty(line) == true)
            {
                return;
            }

            Regex regexUserPattern = new Regex(@"(\w+), (\w+)");
            MatchCollection matches = regexUserPattern.Matches(line);

            foreach (Match match in matches)
            {
                string name = match.Groups[1].Value;
                string password = match.Groups[2].Value;

                savedAccounts.Add(new User(name, password));
            }
        }
        public void SaveUser()
        {
            try
            {
                using (FileStream usersList = new FileStream(usersListFile, FileMode.Append))
                using (StreamWriter writer = new StreamWriter(usersList))
                {
                    writer.WriteLine($"{this.Name}, {this.Password}");

                    if (this is User user)
                    {
                        foreach (var polynomial in user.SavedPolynomials)
                        {
                            writer.WriteLine(polynomial.ToString());
                        }
                    }
                }
            }
            catch (Exception e)
            {
                IO.HandleException(e);
            }
        }
        public static void LoadAllUsers()
        {
            User currentUser = new User();
            string line;
            try
            {
                using (FileStream usersList = new FileStream(usersListFile, FileMode.OpenOrCreate))
                using (StreamReader reader = new StreamReader(usersList))
                {
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (Polynomial.VerifyPolynomial(line) == 0)
                        {
                            currentUser.AddNewPolynomial(line);
                        }
                        currentUser.ReadUser(line);
                        currentUser = SavedAccounts[SavedAccounts.Count - 1];

                    }
                }
            }
            catch (Exception e)
            {
                IO.HandleException(e);
            }
        }
        public static void SaveAllUsers()
        {
            try
            {
                using (FileStream usersList = new FileStream(usersListFile, FileMode.Truncate))
                {
                }
            }
            catch (FileNotFoundException)
            {
                using(FileStream usersList = new FileStream(usersListFile, FileMode.Create))
                {
                }
            }
            catch (Exception e)
            {
                IO.HandleException(e);
            }

            foreach (User account in savedAccounts)
            {
                account.SaveUser();
            }
        }

    }

    class User : Account
    {

        private List<Polynomial> savedPolynomials = new List<Polynomial>();
        public User()
        {
            this.name = string.Empty;
            this.password = string.Empty;
        }

        public User(string name, string password)
        {
            this.name = name;
            this.password = password;
        }

        public List<Polynomial> SavedPolynomials
        {
            get { return savedPolynomials; }
        }

        public int AddNewPolynomial(string polynomialString)
        {
            int result = Polynomial.VerifyPolynomial(polynomialString);
            if (result == 0)
            {
                Polynomial newPolynomial = new Polynomial();
                newPolynomial = Polynomial.CreatePolynomial(polynomialString);
                savedPolynomials.Add(newPolynomial);
            }
            return result;
        }

        public void DeletePolynomial(Polynomial polynomialToDelete)
        {
            savedPolynomials.Remove(polynomialToDelete);
        }

        public void DeletePolynomial(int index)
        {
            savedPolynomials.RemoveAt(index);
        }

        public void EditPolynomials()
        {
            string newPolynomial = string.Empty;
            bool isValid = false;
            bool startDeleting = false;
            string menuHeader = "EDIT FUNCTIONS";
            StringBuilder sbPolynomial = new StringBuilder();
            List<string> editOptions = new List<string>();
            List<Polynomial> deleteList = new List<Polynomial>();
            int i;

            Console.Clear();
            while (true)
            {
                Console.SetCursorPosition(0, 0);
                //Add the polynomials as options
                for (i = 0; i < savedPolynomials.Count; i++)
                {
                    if (deleteList.Contains(savedPolynomials[i]))
                    {
                        sbPolynomial.AppendFormat(IO.SetForeground(savedPolynomials[i].ToString(), IO.textColor.BloodRed));
                    }
                    else
                    {
                        sbPolynomial.Append(savedPolynomials[i].ToString());
                    }
                    editOptions.Add(sbPolynomial.ToString());
                    sbPolynomial.Clear();
                }
                //Add every other option
                editOptions.Add("<Add New>");
                if (startDeleting == true)
                {
                    editOptions.Add("Confirm Delete");
                }
                else
                {
                    editOptions.Add("Delete");
                }
                editOptions.Add("\nBack");

                Console.Clear();
                IO.DisplayOptions(menuHeader, editOptions.ToArray());

                if (IO.SelectedOption != -1 && IO.SelectedOption <= savedPolynomials.Count)
                {
                    do
                    {
                        //If selected "Add New"
                        if (IO.SelectedOption == savedPolynomials.Count)
                        {
                            IO.ModifyOption(IO.SelectedOption);
                        }
                        //If selected a polynomial
                        else
                        {
                            if (startDeleting == true)
                            {
                                deleteList.Add(savedPolynomials[IO.SelectedOption]);
                                break;
                            }
                            else
                            {
                                IO.ModifyOption(IO.SelectedOption, true);
                            }
                        }

                        newPolynomial = Console.ReadLine();
                        if (Polynomial.VerifyPolynomial(newPolynomial) == 1)
                        {
                            Console.SetCursorPosition(0, editOptions.Count + 5);
                            Console.WriteLine("Invalid Polynomial!");
                            isValid = false;
                        }
                        else
                        {
                            isValid = true;
                        }
                    } while (isValid == false);

                    //If selected "Add New"
                    if (IO.SelectedOption == savedPolynomials.Count)
                    {
                        startDeleting = false;
                        deleteList.Clear();
                        AddNewPolynomial(newPolynomial);
                    }
                    //If selected a polynomial
                    else if (startDeleting == false)
                    {
                        savedPolynomials[IO.SelectedOption] = Polynomial.CreatePolynomial(newPolynomial);
                    }
                }
                //If selected "Delete" or "Confirm"
                else if (IO.SelectedOption == savedPolynomials.Count + 1)
                {
                    if (startDeleting == true)
                    {
                        savedPolynomials.RemoveAll(toDelete => deleteList.Contains(toDelete));
                        deleteList.Clear();
                        startDeleting = false;
                        IO.ResetSelect();
                    }
                    else
                    {
                        startDeleting = true;
                    }
                }
                else
                {
                    break;
                }

                editOptions.Clear();
            }
            IO.ResetSelect();
        }

        public override void LogIn()
        {
            LoggedAccount = SavedAccounts.IndexOf(this);
        }
        public override void LogOut()
        {
            LoggedAccount = -1;
        }

        public override void DisplayMenu()
        {
            string[] mainMenuOptions;
            string menuHeader;


            if (LoggedAccount == -1)
            {
                mainMenuOptions = new string[] { "Edit Functions", "Select Graph", "Log In", "Create New Account", "\n\x1b[1mExit\x1b[0m" };
                menuHeader = "WELCOME TO GRAPHING CALCULATOR (Log In to Save Your Graph)";
            }
            else
            {
                mainMenuOptions = new string[] { "Edit Functions", "Select Graph", "Switch Accounts", "My Account", "Log Out", "\n\x1b[1mExit\x1b[0m" };
                menuHeader = $"WELCOME TO GRAPHING CALCULATOR, {SavedAccounts[LoggedAccount].Name}!";
                CurrentUser = SavedAccounts[LoggedAccount];
            }

            IO.ClearConsoleLine(0);
            IO.DisplayOptions(menuHeader, mainMenuOptions);

            switch (IO.SelectedOption)
            {
                case 0:
                    CurrentUser.EditPolynomials();
                    break;
                case 1:
                    Graph.graphIsSelected = true;
                    break;
                case 2:
                    SignIn();
                    break;
                case 3:
                    if (LoggedAccount != -1)
                    {
                        EditUser(this);
                    }
                    else
                    {
                        CreateAccount();
                    }
                    break;
                case 4:
                    if (LoggedAccount != -1)
                    {
                        SavedAccounts[LoggedAccount].LogOut();
                    }
                    else
                    {
                        goto case 5;
                    }
                    break;
                case 5:
                    Console.Clear();
                    Console.WriteLine("See ya next time!");
                    Environment.Exit(0);
                    break;
            }
            Console.Clear();
        }

        protected void EditUser(User account)
        {
            string username = account.Name;
            string password = account.Password;
            string header = "Edit User";
            string[] options;
            bool exit = false;

            IO.ResetSelect();
            Console.Clear();
            while (exit == false)
            {
                options = new string[] { $"Edit username : {username}", $"Edit Password : {password}", "<Confirm>", "\nCancel" };

                IO.ClearConsoleLine(0);
                IO.DisplayOptions(header, options);

                switch (IO.SelectedOption)
                {
                    case 0:
                        string newUsername;
                        bool nameIsEmpty = true;
                        IO.ModifyOption(0, true);
                        Console.Write("Edit username : ");
                        newUsername = Console.ReadLine();
                        nameIsEmpty = string.IsNullOrEmpty(newUsername);

                        if (nameIsEmpty == false)
                        {
                            username = newUsername;
                        }
                        break;
                    case 1:
                        string newPassword;
                        bool passIsEmpty = true;
                        IO.ModifyOption(1, true);
                        Console.Write("Edit Password : ");
                        newPassword = Console.ReadLine();
                        passIsEmpty = string.IsNullOrEmpty(newPassword);

                        if (passIsEmpty == false)
                        {
                            password = newPassword;
                        }
                        break;
                    case 2:
                        bool userExists = false;
                        foreach (User user in User.SavedAccounts)
                        {
                            if (user.name == username && user != account)
                            {
                                Console.SetCursorPosition(0, 10);
                                Console.WriteLine("Username already exists!");
                                Console.SetCursorPosition(0, 0);
                                userExists = true;
                                break;
                            }
                        }
                        if (userExists)
                        {
                            break;
                        }
                        account.Name = username;
                        account.Password = password;
                        goto case 3;
                    case 3:
                        IO.ResetSelect();
                        exit = true;
                        break;
                    case -1:
                        goto case 3;
                }
            }
            Console.Clear();
        }

    }

    class Admin : Account, IDirectoryHandler
    {

        public override void DisplayMenu()
        {
            string savedGraphsPath = "Saved Graphs";
            string menuHeader = "GRAPHING CALCULATOR (ADMIN)";
            string[] menuOptions = { "View Accounts", "Clear Screenshots Folder" , "Log Out", "\nExit" };

            Console.SetCursorPosition(0, 0);
            IO.ResetSelect();
            IO.DisplayOptions(menuHeader, menuOptions);

            switch (IO.SelectedOption)
            {
                case 0:
                    DisplayUsers();
                    break;
                case 1:
                    if (DeleteDirectory(savedGraphsPath) == 0)
                    {
                        Console.SetCursorPosition(0, menuOptions.Length + 4);
                        Console.WriteLine("All screenshots deleted.");
                    }
                    break;


                case 2:
                    adminAccount.LogOut();
                    Console.Clear();
                    break;
                case 3:
                    Console.Clear();
                    Console.WriteLine("See ya next time!");
                    Environment.Exit(0);
                    break;
            }
        }
        public override void LogIn()
        {
            LoggedAccount = -2;
        }
        public override void LogOut()
        {
            LoggedAccount = -1;
        }

        private void DisplayUsers()
        {
            string header = "SAVED USERS";

            bool exit = false;
            bool startDeleting = false;
            List<string> usersList = new List<string>();

            Console.Clear();
            IO.ResetSelect();
            do
            {
                SaveAllUsers();

                foreach (var account in SavedAccounts)
                {
                    usersList.Add(account.Name);
                }
                usersList.Add("<Create Account>");
                if (startDeleting == false)
                {
                    usersList.Add("Delete All Accounts");
                }
                else
                {
                    usersList.Add("This cannot be undone. Continue to delete?");
                }
                usersList.Add("\nBack");

                Console.SetCursorPosition(0, 0);
                IO.DisplayOptions(header, usersList.ToArray());

                if (IO.SelectedOption != -1 && IO.SelectedOption < SavedAccounts.Count)
                {
                    startDeleting = false;
                    Console.Clear();
                    EditUser(SavedAccounts[IO.SelectedOption]);
                }
                else if (IO.SelectedOption == SavedAccounts.Count)
                {
                    startDeleting = false;
                    CreateAccount();
                }
                else if (IO.SelectedOption == SavedAccounts.Count + 1)
                {
                    if (startDeleting == false)
                    {
                        startDeleting = true;
                    }
                    else
                    {
                        SavedAccounts.Clear();
                        exit = true;
                    }
                }
                else
                {
                    exit = true;
                }
                usersList.Clear();
            } while (exit == false);
            Console.Clear();
        }
        protected void EditUser(User account)
        {
            string username = account.Name;
            string password = account.Password;
            string header = "Edit User";
            string userScreenshotsPath = $"Saved Graphs\\{account.Name}";
            string[] options;
            bool startDeleting = false;
            bool exit = false;

            IO.ResetSelect();
            Console.Clear();
            while (exit == false)
            {
                options = new string[] {
                    $"Edit username : {username}",
                    $"Edit Password : {password}",
                    "<Delete Account>",
                    "Delete Screenshots",
                    "Confirm",
                    "\nCancel"
                };
                if (startDeleting == true)
                {
                    options[2] = "<This cannot be undone. Continue?>";
                }

                IO.ClearConsoleLine(0);
                IO.DisplayOptions(header, options);

                if (IO.SelectedOption != 2 && startDeleting == true)
                {
                    startDeleting = false;
                    options[2] = "Delete Account>";
                }

                switch (IO.SelectedOption)
                {
                    case 0:
                        string newUsername;
                        bool nameIsEmpty = true;
                        IO.ModifyOption(0, true);
                        Console.Write("Edit username : ");
                        newUsername = Console.ReadLine();
                        nameIsEmpty = string.IsNullOrEmpty(newUsername);

                        if (nameIsEmpty == false)
                        {
                            username = newUsername;
                        }
                        break;
                    case 1:
                        string newPassword;
                        bool passIsEmpty = true;
                        IO.ModifyOption(1, true);
                        Console.Write("Edit Password : ");
                        newPassword = Console.ReadLine();
                        passIsEmpty = string.IsNullOrEmpty(newPassword);

                        if (passIsEmpty == false)
                        {
                            password = newPassword;
                        }
                        break;
                    case 2:
                        if (startDeleting == false)
                        {
                            startDeleting = true;
                        }
                        else
                        {
                            DeleteUser(account);
                            goto case 5;
                        }
                        break;
                    case 3:
                        if (this.DeleteDirectory(userScreenshotsPath) == 0)
                        {
                            Console.SetCursorPosition(0, options.Length + 5);
                            Console.WriteLine("Screenshots deleted.");
                        }
                        break;

                    case 4:
                        bool userExists = verifyUsername(username);
                        if (userExists)
                        {
                            break;
                        }
                        account.Name = username;
                        account.Password = password;
                        goto case 5;

                    case 5:
                        IO.ResetSelect();
                        exit = true;
                        break;
                    case -1:
                        goto case 5;
                }
            }
            Console.Clear();
        }

        public void CreateDirectory(string path)
        {
            string directory = Path.GetDirectoryName(path);

            if (Directory.Exists(directory) == false)
            {
                Directory.CreateDirectory(directory);
            }
        }

        public int DeleteDirectory(string filePath)
        {
            int exitCode = 0;
            try
            {

                if (Directory.Exists(filePath))
                {
                    Directory.Delete(filePath, recursive: true);
                    Directory.CreateDirectory(filePath);
                }
                else
                {

                    exitCode = 1;
                }
            }
            catch (Exception ex)
            {
                exitCode = 1;
                Console.WriteLine($"Error: {ex.Message}");
            }

            return exitCode;
        }
    }
    
    class GraphingCalculatorProgram
    {
        public static void Main(string[] args)
        {
            const int SIZE_ALLOWANCE = 10;
            Graph graph = new Graph();
            User guestUser = new User();
            bool previouslyLoggedIn = false;
            int heightCheck = 0; ;
            User.LoadAllUsers();

            while (true)
            {
                graph.Initialize();
                User.SaveAllUsers();

                while (Console.WindowHeight < graph.HEIGHT + SIZE_ALLOWANCE || Console.WindowWidth < graph.WIDTH + SIZE_ALLOWANCE)
                {
                    if (Console.WindowHeight != heightCheck)
                    {
                        string message = "Please maximize the window to display graph properly.";
                        Console.Clear();
                        try
                        {
                            Console.SetCursorPosition(Console.WindowWidth / 2 - (message.Length / 2), Console.WindowHeight / 2);
                        }
                        catch (ArgumentOutOfRangeException e)
                        {
                            IO.HandleException(e);
                        }
                        Console.WriteLine(message);
                        heightCheck = Console.WindowHeight;
                    }
                    else
                    {
                        Console.SetCursorPosition(0, 0);
                        Console.Write("");
                    }
                } 

                if (heightCheck != 0)
                {
                    Console.Clear();
                    IO.ResetSelect();
                    heightCheck = 0;
                }

                if (User.LoggedAccount == -1)
                {
                    if (previouslyLoggedIn == true)
                    {
                        guestUser = new User();
                        previouslyLoggedIn = false;
                    }
                    guestUser.LogIn();
                    User.CurrentUser = guestUser;
                }
                else if (User.LoggedAccount == -2)
                {
                    Account.adminAccount.DisplayMenu();
                    continue;
                }
                else
                {
                    previouslyLoggedIn = true;
                    User.CurrentUser = User.SavedAccounts[User.LoggedAccount];
                }
                graph.DisplayGraph();
                Console.SetCursorPosition(0, 0);
                if (Graph.graphIsSelected == true)
                {
                    Console.SetCursorPosition(0, 0);
                    Console.WriteLine("Press ESC to return" +
                        "\nMove graph with ARROW keys" +
                        "\nZoom out with >" +
                        "\nZoom in with <" +
                        "\nReset graph position with SPACEBAR" +
                        "\nScreenshot graph with TAB" +
                        "\nPress ENTER to plot x");
                    graph.ControlGraph();

                }
                else
                {
                    for (int i = 0; i < 5; i++)
                    {
                        IO.ClearConsoleLine(i);
                    }
                    IO.ResetSelect();
                    for (int i = 0; i < graph.GraphLocation; i++)
                    {
                        IO.ClearConsoleLine(i);
                    }
                    User.CurrentUser.DisplayMenu();
                }
            }
        }
    }
}

