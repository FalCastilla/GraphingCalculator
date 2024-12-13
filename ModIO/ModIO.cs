using System;
using System.Text;

namespace ModIO
{
    internal class Option
    {
        private int start;
        private int left;
        private int top;

        public int Start
        {
            get { return start; }
            set { this.start = value; }
        }
        
        public int Left
        {
            get { return left; }
            set { left = value; }
        }

        public int Top
        {
            get { return top; }
            set { top = value; }
        }
    }
    public abstract class IO
    {
        private static int previousOption = 0;
        private static int selectedOption = 0;
        private static Option[] optionPos;
        public enum textColor
        {
            Default = -1,
            Black = 0,
            White = 15,
            Red = 9,
            Orange = 214,
            Green = 10,
            Yellow = 226,
            Blue = 12,
            Magenta = 13,
            Cyan = 14,
            BloodRed = 124,
            LightGray = 250,
            LightGray2 = 247,
            LightBlue = 39,
            NeonGreen = 46,
            DarkYellow = 136,
            Pink = 200,
            DirtyColorIDK = 252,
            DirtyYellow = 184
        }
        public static int SelectedOption
        {
            get { return selectedOption; }
        }
        public static ConsoleKey StartSelecting()
        {
            ConsoleKeyInfo keyPressed = Console.ReadKey(true);

            return keyPressed.Key;
        }

        public static void DisplayOptions(string heading, params string[] options)
        {
            optionPos = new Option[options.Length];
            int cursorRowStart = Console.CursorTop;
            int cursorCol = Console.CursorLeft;
            int cursorRowEnd;
            bool stopSelecting = false;

            if (selectedOption >= options.Length)
            {
                selectedOption = 0;
            }
            else
            {
                selectedOption = previousOption;
            }

            do
            {
                Console.WriteLine(heading);
                Console.WriteLine();

                for (int i = 0; i < options.Length; i++)
                {
                    string option = options[i];

                    if (optionPos[i] == null)
                    {
                        optionPos[i] = new Option();
                    }
                    if (option[0] == '<' && option[option.Length - 1] == '>')
                    {
                        Console.WriteLine("----------");
                        option = option.Replace("<", "");
                        option = option.Replace(">", "");
                    }
                    if (option[0] == '\n')
                    {
                        Console.WriteLine();
                        option = option.Replace("\n", "");
                    }

                    if (i == selectedOption)
                    {
                        Console.Write("[-->]");
                    }
                    else
                    {
                        Console.Write("[] ");
                    }

                    optionPos[i].Start = Console.CursorLeft;
                    Console.Write(" {0}     ", option);
                    optionPos[i].Left = Console.CursorLeft;
                    optionPos[i].Top = Console.CursorTop;
                    Console.WriteLine();
                }


                switch (StartSelecting())
                {
                    case ConsoleKey.UpArrow:
                        if (selectedOption > 0)
                        {
                            selectedOption--;
                        }
                        break;

                    case ConsoleKey.DownArrow:
                        if (selectedOption < options.Length - 1)
                        {
                            selectedOption++;
                        }
                        break;

                    case ConsoleKey.Enter:
                        previousOption = selectedOption;
                        stopSelecting = true;
                        break;
                    case ConsoleKey.Escape:
                        previousOption = selectedOption;
                        stopSelecting = true;
                        selectedOption = -1;
                        break;
                }
                cursorRowEnd = Console.CursorTop;
                Console.SetCursorPosition(cursorCol, cursorRowStart);
            } while (stopSelecting == false);
            Console.SetCursorPosition(cursorCol, cursorRowEnd);
        }

        public static void GoToOption(int option_no)
        {
            int x = optionPos[option_no].Left;
            int y = optionPos[option_no].Top;
            if (selectedOption == option_no)
            {
                x -= 2;
            }
            Console.SetCursorPosition(x, y);
        }

        internal static void GoToOption(int option_no, bool overwriteOption)
        {
            int x = optionPos[option_no].Start;
            int y = optionPos[option_no].Top;
            if (selectedOption == option_no)
            {
                x += 1;
            }
            Console.SetCursorPosition(x, y);
        }

        public static void ModifyOption(int option_no)
        {
            int option_start;

            GoToOption(option_no);
            option_start = Console.CursorLeft;
            Console.Write(new string(' ', Console.WindowWidth - option_start));
            IO.GoToOption(option_no);
            Console.Write("   : ");

        }
        public static void ModifyOption(int option_no, bool overwriteOption)
        {
            int option_start;

            GoToOption(option_no, true);
            option_start = Console.CursorLeft;
            Console.Write(new string(' ', Console.WindowWidth - option_start));
            IO.GoToOption(option_no, true);
        }

        public static void DisplayAtOption(int option_no, string display)
        {
            GoToOption(option_no);
            Console.WriteLine(display);
            Console.SetCursorPosition(0, 0);
        }

        public static void ResetSelect()
        {
            previousOption = 0;
            selectedOption = 0;
        }

        public static void ClearConsoleLine(int line)
        {
            Console.SetCursorPosition(0, line);
            int width = Console.WindowWidth;
            Console.Write(new string(' ', width));
            Console.SetCursorPosition(0, line);
        }

        public static void ClearAtOption(int optionNo)
        {
            ModifyOption(optionNo, true);
            Console.Write(new string(' ', Console.WindowWidth - optionPos[optionNo].Start));
            ModifyOption(optionNo, true);
        }

        public static void HandleException(Exception ex)
        {
            Console.SetCursorPosition(0, Console.WindowHeight - 1);
            Console.WriteLine(ex.Message);
            Console.SetCursorPosition(0, 0);
        }
        public static void HandleException(Exception ex, string line)
        {
            Console.SetCursorPosition(0, Console.WindowHeight - 2);
            Console.WriteLine(ex.Message);
            Console.WriteLine(line);
            Console.SetCursorPosition(0, 0);
        }

        public static string SetForeground(string line, textColor color)
        {
            int lineColor = (color == textColor.Default) ? 7 : (int)color;
            string coloredLine = $"\x1b[38;5;{lineColor}m{line}\x1b[38;5;7m";

            return coloredLine;
        }
        public static string SetBackground(string line, textColor color)
        {
            int lineColor = (color == textColor.Default) ? 0 : (int)color;
            string coloredLine = $"\x1b[48;5;{lineColor}m{line}\x1b[48;5;0m";

            return coloredLine;
        }
        public static string SetForeground(string line, textColor color, textColor endColor)
        {
            int lineColor = (color == textColor.Default) ? 7 : (int)color;
            string coloredLine = $"\x1b[38;5;{lineColor}m{line}\x1b[38;5;{(int)endColor}m";

            return coloredLine;
        }
        public static string SetBackground(string line, textColor color, textColor endColor)
        {
            int lineColor = (color == textColor.Default) ? 0 : (int)color;
            string coloredLine = $"\x1b[48;5;{lineColor}m{line}\x1b[48;5;{endColor}m";

            return coloredLine;
        }
        public static string ApplyColor(string line, textColor foregroundColor, textColor backgroundColor)
        {
            string coloredLine = SetForeground(line, foregroundColor);
            coloredLine = SetBackground(coloredLine, backgroundColor);

            return coloredLine;
        }

        public static void SbReplaceAt(StringBuilder sbLine, int index, string line)
        {
            for (int i = 0; i < line.Length; i++)
            {
                if (i + index < sbLine.Length)
                {
                    sbLine[i + index] = line[i];
                }
                else
                {
                    sbLine.Append(line[i]);
                }
            }
        }
    }

    class ModIOProgram
    {
        public static void Main(string[] args)
        {

        }
    }
}