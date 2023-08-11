
class Program
{
    static void Main(string[] args)
    {
        // Create a 10x10 grid
        char[,] grid = new char[10, 10];

        // Amount of Ships 
        //1x Battleship (5 squares)
        //2x Destroyers(4 squares)

        int[] shipSizes = { 5,4,4 };

        // Place ships on the grid
        PlaceShips(grid, shipSizes);

        // Display the grid (without revealing ships' positions)
        DisplayGrid(grid, false);

        // Play the game
        PlayGame(grid, shipSizes.Length);
    }

    static void PlaceShips(char[,] grid, int[] shipSizes)
    {
        Random random = new();

        for (int i = 0; i < shipSizes.Length; i++)
        {
            PlaceShip(grid, random, shipSizes[i]);
        }
    }

    static void PlaceShip(char[,] grid, Random random, int shipSize)
    {
        int gridSize = grid.GetLength(0);
        bool placed = false;

        while (!placed)
        {
            int startX = random.Next(gridSize);
            int startY = random.Next(gridSize);
            bool isVertical = random.Next(2) == 0; // Randomly decide if the ship should be vertical or horizontal

            if (IsValidPlacement(grid, startX, startY, shipSize, isVertical))
            {
                PlaceShipOnGrid(grid, startX, startY, shipSize, isVertical);
                placed = true;
            }
        }
    }

    static bool IsValidPlacement(char[,] grid, int startX, int startY, int shipSize, bool isVertical)
    {
        int gridSize = grid.GetLength(0);

        if (isVertical)
        {
            if (startY + shipSize > gridSize)
            {
                return false; // Ship would go out of bounds
            }

            for (int y = startY; y < startY + shipSize; y++)
            {
                if (grid[startX, y] != '\0')
                {
                    return false; // Ship would overlap with another ship
                }
            }
        }
        else
        {
            if (startX + shipSize > gridSize)
            {
                return false; // Ship would go out of bounds
            }

            for (int x = startX; x < startX + shipSize; x++)
            {
                if (grid[x, startY] != '\0')
                {
                    return false; // Ship would overlap with another ship
                }
            }
        }

        return true;
    }

    static void PlaceShipOnGrid(char[,] grid, int startX, int startY, int shipSize, bool isVertical)
    {
        if (isVertical)
        {
            for (int y = startY; y < startY + shipSize; y++)
            {
                grid[startX, y] = 'S'; // 'S' represents a ship
            }
        }
        else
        {
            for (int x = startX; x < startX + shipSize; x++)
            {
                grid[x, startY] = 'S'; // 'S' represents a ship
            }
        }
    }

    static void DisplayGrid(char[,] grid, bool revealShips)
    {
        int gridSize = grid.GetLength(0);

        Console.WriteLine("  A B C D E F G H I J");
        Console.WriteLine("  -------------------");
        for (int y = 0; y < gridSize; y++)
        {
            Console.Write((y + 1).ToString().PadLeft(2) + "|");
            for (int x = 0; x < gridSize; x++)
            {
                char cell = grid[x, y];
                if (!revealShips && cell == 'S')
                {
                    cell = '\0'; // Hide ships' positions
                }
                Console.Write(cell == '\0' ? "  " : cell + " ");
            }
            Console.WriteLine();
        }
    }

    static void PlayGame(char[,] grid, int totalAmountOfShips)
    {
        int gridSize = grid.GetLength(0);
        int totalShips = totalAmountOfShips;
        int shotsFired = 0;
        int shipsSunk = 0;

        while (shipsSunk < totalShips)
        {
            Console.Write("Enter target (e.g., A5): ");
            string input = Console.ReadLine();

            if (input.Length != 2 && input.Length != 3)
            {
                Console.WriteLine("Invalid input. Please enter a valid target (e.g., A5).");
                continue;
            }

            char columnChar = Char.ToUpper(input[0]);
            int row;
            if (!int.TryParse(input[1].ToString() + (input.Length == 3 ? input[2].ToString() : ""), out row) || row < 1 || row > 10)
            {
                Console.WriteLine("Invalid input. Please enter a valid row (1-10).");
                continue;
            }

            int column = columnChar - 'A';
            int targetX = column;
            int targetY = row - 1;

            if (grid[targetX, targetY] == '\0')
            {
                Console.WriteLine("Miss!");
                grid[targetX, targetY] = 'M'; // 'M' represents a miss
            }
            else if (grid[targetX, targetY] == 'S')
            {
                Console.WriteLine("Hit!");
                grid[targetX, targetY] = 'H'; // 'H' represents a hit

                // Check if the ship is sunk
                if (IsShipSunk(grid, targetX, targetY))
                {
                    Console.WriteLine("Ship sunk!");
                    shipsSunk++;
                }
            }
            else if (grid[targetX, targetY] == 'H' || grid[targetX, targetY] == 'M')
            {
                Console.WriteLine("You already fired at this target.");
            }

            shotsFired++;
            DisplayGrid(grid, false);
        }

        Console.WriteLine("Congratulations! You sunk all the ships in " + shotsFired + " shots.");
    }

    static bool IsShipSunk(char[,] grid, int x, int y)
    {
        int gridSize = grid.GetLength(0);

        char shipType = grid[x, y]; // Get the ship type ('S' for battleship, 'D' for destroyer)
        int shipSize = (shipType == 'S') ? 5 : 4;

        if (shipType == 'S') // Only need to check if the battleship is sunk
        {
            if (x + shipSize < gridSize)
            {
                for (int i = 1; i < shipSize; i++)
                {
                    if (grid[x + i, y] == 'S')
                    {
                        return false; // Ship is not completely sunk
                    }
                }
            }
            else
            {
                for (int i = 1; i < shipSize; i++)
                {
                    if (grid[x - i, y] == 'S')
                    {
                        return false; // Ship is not completely sunk
                    }
                }
            }
        }
        else // For destroyers, check both horizontal and vertical positions
        {
            bool isSunkHorizontal = true;
            bool isSunkVertical = true;

            for (int i = 1; i < shipSize; i++)
            {
                if (x + i < gridSize && grid[x + i, y] == 'S')
                {
                    isSunkHorizontal = false;
                }

                if (y + i < gridSize && grid[x, y + i] == 'S')
                {
                    isSunkVertical = false;
                }
            }

            return isSunkHorizontal || isSunkVertical;
        }

        return true; // Ship is completely sunk
    }
}