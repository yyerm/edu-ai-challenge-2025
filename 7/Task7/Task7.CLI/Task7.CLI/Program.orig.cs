//using System;
//using System.Collections.Generic;

//class Program
//{
//    private static int boardSize = 10;
//    private static int numShips = 3;
//    private static int shipLength = 3;

//    private static List<Ship> playerShips = new List<Ship>();
//    private static List<Ship> cpuShips = new List<Ship>();
//    private static int playerNumShips = numShips;
//    private static int cpuNumShips = numShips;

//    private static List<string> guesses = new List<string>();
//    private static List<string> cpuGuesses = new List<string>();
//    private static string cpuMode = "hunt";
//    private static List<string> cpuTargetQueue = new List<string>();

//    private static char[,] board;
//    private static char[,] playerBoard;

//    private static Random random = new Random();

//    public class Ship
//    {
//        public List<string> locations = new List<string>();
//        public List<string> hits = new List<string>();
//    }

//    static void Main(string[] args)
//    {
//        CreateBoard();

//        PlaceShipsRandomly(playerBoard, playerShips, playerNumShips);
//        PlaceShipsRandomly(board, cpuShips, cpuNumShips, false);

//        Console.WriteLine("\nLet's play Sea Battle!");
//        Console.WriteLine("Try to sink the " + cpuNumShips + " enemy ships.");
//        GameLoop();
//    }

//    static void CreateBoard()
//    {
//        board = new char[boardSize, boardSize];
//        playerBoard = new char[boardSize, boardSize];

//        for (int i = 0; i < boardSize; i++)
//        {
//            for (int j = 0; j < boardSize; j++)
//            {
//                board[i, j] = '~';
//                playerBoard[i, j] = '~';
//            }
//        }
//        Console.WriteLine("Boards created.");
//    }

//    static void PlaceShipsRandomly(char[,] targetBoard, List<Ship> shipsArray, int numberOfShips, bool isPlayerBoard = true)
//    {
//        int placedShips = 0;
//        while (placedShips < numberOfShips)
//        {
//            string orientation = random.NextDouble() < 0.5 ? "horizontal" : "vertical";
//            int startRow, startCol;
//            bool collision = false;

//            if (orientation == "horizontal")
//            {
//                startRow = (int)Math.Floor(random.NextDouble() * boardSize);
//                startCol = (int)Math.Floor(random.NextDouble() * (boardSize - shipLength + 1));
//            }
//            else
//            {
//                startRow = (int)Math.Floor(random.NextDouble() * (boardSize - shipLength + 1));
//                startCol = (int)Math.Floor(random.NextDouble() * boardSize);
//            }

//            List<string> tempLocations = new List<string>();
//            for (int i = 0; i < shipLength; i++)
//            {
//                int checkRow = startRow;
//                int checkCol = startCol;
//                if (orientation == "horizontal")
//                {
//                    checkCol += i;
//                }
//                else
//                {
//                    checkRow += i;
//                }
//                string locationStr = checkRow.ToString() + checkCol.ToString();
//                tempLocations.Add(locationStr);

//                if (checkRow >= boardSize || checkCol >= boardSize)
//                {
//                    collision = true;
//                    break;
//                }

//                if (targetBoard[checkRow, checkCol] != '~')
//                {
//                    collision = true;
//                    break;
//                }
//            }

//            if (!collision)
//            {
//                Ship newShip = new Ship();
//                for (int i = 0; i < shipLength; i++)
//                {
//                    int placeRow = startRow;
//                    int placeCol = startCol;
//                    if (orientation == "horizontal")
//                    {
//                        placeCol += i;
//                    }
//                    else
//                    {
//                        placeRow += i;
//                    }
//                    string locationStr = placeRow.ToString() + placeCol.ToString();
//                    newShip.locations.Add(locationStr);
//                    newShip.hits.Add("");

//                    if (isPlayerBoard)
//                    {
//                        targetBoard[placeRow, placeCol] = 'S';
//                    }
//                }
//                shipsArray.Add(newShip);
//                placedShips++;
//            }
//        }
//        Console.WriteLine(numberOfShips + " ships placed randomly for " + (isPlayerBoard ? "Player." : "CPU."));
//    }

//    static void PrintBoard()
//    {
//        Console.WriteLine("\n   --- OPPONENT BOARD ---          --- YOUR BOARD ---");
//        string header = "  ";
//        for (int h = 0; h < boardSize; h++) header += h + " ";
//        Console.WriteLine(header + "     " + header);

//        for (int i = 0; i < boardSize; i++)
//        {
//            string rowStr = i + " ";

//            for (int j = 0; j < boardSize; j++)
//            {
//                rowStr += board[i, j] + " ";
//            }
//            rowStr += "    " + i + " ";

//            for (int j = 0; j < boardSize; j++)
//            {
//                rowStr += playerBoard[i, j] + " ";
//            }
//            Console.WriteLine(rowStr);
//        }
//        Console.WriteLine("\n");
//    }

//    static bool ProcessPlayerGuess(string guess)
//    {
//        if (guess == null || guess.Length != 2)
//        {
//            Console.WriteLine("Oops, input must be exactly two digits (e.g., 00, 34, 98).");
//            return false;
//        }

//        int row, col;
//        if (!int.TryParse(guess[0].ToString(), out row) || !int.TryParse(guess[1].ToString(), out col))
//        {
//            Console.WriteLine("Oops, please enter valid row and column numbers between 0 and " + (boardSize - 1) + ".");
//            return false;
//        }

//        if (row < 0 || row >= boardSize || col < 0 || col >= boardSize)
//        {
//            Console.WriteLine("Oops, please enter valid row and column numbers between 0 and " + (boardSize - 1) + ".");
//            return false;
//        }

//        string formattedGuess = guess;

//        if (guesses.IndexOf(formattedGuess) != -1)
//        {
//            Console.WriteLine("You already guessed that location!");
//            return false;
//        }
//        guesses.Add(formattedGuess);

//        bool hit = false;

//        for (int i = 0; i < cpuShips.Count; i++)
//        {
//            Ship ship = cpuShips[i];
//            int index = ship.locations.IndexOf(formattedGuess);

//            if (index >= 0 && ship.hits[index] != "hit")
//            {
//                ship.hits[index] = "hit";
//                board[row, col] = 'X';
//                Console.WriteLine("PLAYER HIT!");
//                hit = true;

//                if (IsSunk(ship))
//                {
//                    Console.WriteLine("You sunk an enemy battleship!");
//                    cpuNumShips--;
//                }
//                break;
//            }
//            else if (index >= 0 && ship.hits[index] == "hit")
//            {
//                Console.WriteLine("You already hit that spot!");
//                hit = true;
//                break;
//            }
//        }

//        if (!hit)
//        {
//            board[row, col] = 'O';
//            Console.WriteLine("PLAYER MISS.");
//        }

//        return true;
//    }

//    static bool IsValidAndNewGuess(int row, int col, List<string> guessList)
//    {
//        if (row < 0 || row >= boardSize || col < 0 || col >= boardSize)
//        {
//            return false;
//        }
//        string guessStr = row.ToString() + col.ToString();
//        return guessList.IndexOf(guessStr) == -1;
//    }

//    static void CpuTurn()
//    {
//        Console.WriteLine("\n--- CPU's Turn ---");
//        int guessRow, guessCol;
//        string guessStr;
//        bool madeValidGuess = false;

//        while (!madeValidGuess)
//        {
//            if (cpuMode == "target" && cpuTargetQueue.Count > 0)
//            {
//                guessStr = cpuTargetQueue[0];
//                cpuTargetQueue.RemoveAt(0);
//                guessRow = int.Parse(guessStr[0].ToString());
//                guessCol = int.Parse(guessStr[1].ToString());
//                Console.WriteLine("CPU targets: " + guessStr);

//                if (cpuGuesses.IndexOf(guessStr) != -1)
//                {
//                    if (cpuTargetQueue.Count == 0) cpuMode = "hunt";
//                    continue;
//                }
//            }
//            else
//            {
//                cpuMode = "hunt";
//                guessRow = (int)Math.Floor(random.NextDouble() * boardSize);
//                guessCol = (int)Math.Floor(random.NextDouble() * boardSize);
//                guessStr = guessRow.ToString() + guessCol.ToString();

//                if (!IsValidAndNewGuess(guessRow, guessCol, cpuGuesses))
//                {
//                    continue;
//                }
//            }

//            madeValidGuess = true;
//            cpuGuesses.Add(guessStr);

//            bool hit = false;
//            for (int i = 0; i < playerShips.Count; i++)
//            {
//                Ship ship = playerShips[i];
//                int index = ship.locations.IndexOf(guessStr);

//                if (index >= 0)
//                {
//                    ship.hits[index] = "hit";
//                    playerBoard[guessRow, guessCol] = 'X';
//                    Console.WriteLine("CPU HIT at " + guessStr + "!");
//                    hit = true;

//                    if (IsSunk(ship))
//                    {
//                        Console.WriteLine("CPU sunk your battleship!");
//                        playerNumShips--;

//                        cpuMode = "hunt";
//                        cpuTargetQueue = new List<string>();
//                    }
//                    else
//                    {
//                        cpuMode = "target";
//                        var adjacent = new[]
//                        {
//                            new { r = guessRow - 1, c = guessCol },
//                            new { r = guessRow + 1, c = guessCol },
//                            new { r = guessRow, c = guessCol - 1 },
//                            new { r = guessRow, c = guessCol + 1 }
//                        };
//                        foreach (var adj in adjacent)
//                        {
//                            if (IsValidAndNewGuess(adj.r, adj.c, cpuGuesses))
//                            {
//                                string adjStr = adj.r.ToString() + adj.c.ToString();

//                                if (cpuTargetQueue.IndexOf(adjStr) == -1)
//                                {
//                                    cpuTargetQueue.Add(adjStr);
//                                }
//                            }
//                        }
//                    }
//                    break;
//                }
//            }

//            if (!hit)
//            {
//                playerBoard[guessRow, guessCol] = 'O';
//                Console.WriteLine("CPU MISS at " + guessStr + ".");

//                if (cpuMode == "target" && cpuTargetQueue.Count == 0)
//                {
//                    cpuMode = "hunt";
//                }
//            }
//        }
//    }

//    static bool IsSunk(Ship ship)
//    {
//        for (int i = 0; i < shipLength; i++)
//        {
//            if (ship.hits[i] != "hit")
//            {
//                return false;
//            }
//        }
//        return true;
//    }

//    static void GameLoop()
//    {
//        if (cpuNumShips == 0)
//        {
//            Console.WriteLine("\n*** CONGRATULATIONS! You sunk all enemy battleships! ***");
//            PrintBoard();
//            return;
//        }
//        if (playerNumShips == 0)
//        {
//            Console.WriteLine("\n*** GAME OVER! The CPU sunk all your battleships! ***");
//            PrintBoard();
//            return;
//        }

//        PrintBoard();
//        Console.Write("Enter your guess (e.g., 00): ");
//        string answer = Console.ReadLine();
//        bool playerGuessed = ProcessPlayerGuess(answer);

//        if (playerGuessed)
//        {
//            if (cpuNumShips == 0)
//            {
//                GameLoop();
//                return;
//            }

//            CpuTurn();

//            if (playerNumShips == 0)
//            {
//                GameLoop();
//                return;
//            }
//        }

//        GameLoop();
//    }
//}

