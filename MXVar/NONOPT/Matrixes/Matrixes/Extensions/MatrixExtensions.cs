using Cloo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matrixes.Extensions
{
    internal static class MatrixExtensions
    {
        public static List<List<T>> Transpose<T>(this List<List<T>> lists)
        {
            var longest = lists.Any() ? lists.Max(l => l.Count) : 0;
            List<List<T>> outer = new List<List<T>>(longest);
            for (int i = 0; i < longest; i++)
                outer.Add(new List<T>(lists.Count));
            for (int j = 0; j < lists.Count; j++)
                for (int i = 0; i < longest; i++)
                    outer[i].Add(lists[j].Count > i ? lists[j][i] : default(T));
            return outer;
        }

        public static List<ComputeBuffer<int>> TransformMatrixToComputerBuffersOfRows(this List<List<int>> matrix, ComputeContext context)
        {
            List<ComputeBuffer<int>> rowsMatrix = new List<ComputeBuffer<int>>();
            foreach (var mo in matrix)
            {
                int[] r = mo.ToArray();
                rowsMatrix.Add(new ComputeBuffer<int>(context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.UseHostPointer, r));
            }
            return rowsMatrix;
        }

        public static List<ComputeBuffer<int>> TransformMatrixToComputerBuffersOfColumns(this List<List<int>> matrix, ComputeContext context)
        {
            List<ComputeBuffer<int>> columnsMatrix = new List<ComputeBuffer<int>>();
            List<List<int>> columns = matrix.Transpose<int>();
            foreach (var mo in columns)
            {
                int[] r = mo.ToArray();
                columnsMatrix.Add(new ComputeBuffer<int>(context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.UseHostPointer, r));
            }
            return columnsMatrix;
        }
    }
}
