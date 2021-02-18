using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matrixes
{
    class Program
    {
        static void Main(string[] args)
        {
            List<List<int>> matrixOne = new List<List<int>>();
            List<List<int>> matrixTwo = new List<List<int>>();
            Console.WriteLine("Initializing first matrix");
            Console.WriteLine("-------------------------------------------------");
            //Initialize(ref matrixOne);
            StaticInitialize(ref matrixOne, 100, 100);
            Console.WriteLine("-------------------------------------------------");
            Console.WriteLine("Done\n");
            Console.WriteLine("Printing first matrix");
            //Print(matrixOne);
            Console.WriteLine("-------------------------------------------------");
            Console.WriteLine("Press any key...");
            Console.ReadKey();
            Console.Clear();
            Console.WriteLine("Initializing second matrix");
            Console.WriteLine("-------------------------------------------------");
            //Initialize(ref matrixTwo);
            StaticInitialize(ref matrixTwo, 100, 100);
            Console.WriteLine("-------------------------------------------------");
            Console.WriteLine("Done\n");
            Console.WriteLine("Printing second matrix");
            //Print(matrixTwo);
            Console.WriteLine("-------------------------------------------------");
            Console.WriteLine("Press any key...");
            Console.ReadKey();
            Console.Clear();
            var tuple = MatrixMultiply.Multiply(matrixOne, matrixTwo);
            //Print(tuple.Item1);
            Console.WriteLine($"Non-parallel multiplication done in {tuple.Item2}\n");
            Console.WriteLine("Press any key...");
            Console.ReadKey();
            Console.Clear();
            var ptuple = MatrixMultiply.MultiplyParallel(matrixOne, matrixTwo);
            //Print(ptuple.Item1);
            Console.WriteLine($"Parallel multiplication done in {ptuple.Item2}\n");
            Console.WriteLine("Press any key...");
            Console.ReadKey();
            Console.Clear();
            Console.WriteLine($"Non-parallel multiplication done in {tuple.Item2} - Parallel multiplication done in {ptuple.Item2}");
            Console.WriteLine($"Difference in time {tuple.Item2-ptuple.Item2}");
            Console.ReadKey();
            Console.Clear();
        }

        private static void Print(List<List<int>> matrix)
        {
            foreach (var r in matrix)
            {
                foreach (var i in r)
                {
                    Console.Write($"{i}\t");
                }
                Console.Write("\n");
            }
        }

        private static void StaticInitialize(ref List<List<int>> matrix, int rows=10000, int columns=10000)
        {

            for (int i = 0; i < rows; ++i)
            {
                matrix.Add(new List<int>());
                for (int j = 0; j < columns; ++j)
                {
                    matrix[i].Add(-1);
                }
            }

            Random random = new Random((int)DateTime.UtcNow.Ticks);
            for (int i = 0; i < rows; ++i)
            {
                for (int j = 0; j < columns; ++j)
                {

                    int val = random.Next(10, 100000);
                    matrix[i][j] = val;
                }
            }
        }

        private static void Initialize(ref List<List<int>> matrix)
        {
            int rows = -1;
            string stringVal = string.Empty;
            bool isParsedCorrect = false;

            do
            {
                Console.WriteLine("Enter rows count: ");
                stringVal = Console.ReadLine();
                isParsedCorrect = int.TryParse(stringVal, out rows);
            }
            while (!isParsedCorrect);
            rows = int.Parse(stringVal);

            int columns = -1;
            stringVal = string.Empty;
            isParsedCorrect = false;
            do
            {
                Console.WriteLine("Enter columns count: ");
                stringVal = Console.ReadLine();
                isParsedCorrect = int.TryParse(stringVal, out columns);
            }
            while (!isParsedCorrect);
            columns = int.Parse(stringVal);

            for (int i = 0; i < rows; ++i)
            {
                matrix.Add(new List<int>());
                for (int j = 0; j < columns; ++j)
                {
                    matrix[i].Add(-1);
                }
            }

            for (int i = 0; i < rows; ++i)
            {
                for (int j = 0; j < columns; ++j)
                {
                    int val = -1;
                    stringVal = string.Empty;
                    isParsedCorrect = false;
                    do
                    {
                        Console.WriteLine($"Enter value for matrix[{i}][{j}]: ");
                        stringVal = Console.ReadLine();
                        isParsedCorrect = int.TryParse(stringVal, out val);
                    }
                    while (!isParsedCorrect);
                    val = int.Parse(stringVal);
                    matrix[i][j] = val;
                }
            }
        }
    }
}
