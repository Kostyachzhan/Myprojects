using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Cloo;
using Matrixes.Extensions;

namespace Matrixes
{
    public static class MatrixMultiply
    {
        private static bool isRegularMatrix(List<List<int>> matrix)
        {
            int count = matrix[0].Count;
            bool isRegular = true;
            foreach (var d in matrix)
            {
                if (d.Count != count)
                {
                    isRegular = false;
                }
            }
            return isRegular;
        }
        public static Tuple<List<List<int>>, TimeSpan> Multiply(List<List<int>> matrixOne, List<List<int>> matrixTwo)
        {
            if (!isRegularMatrix(matrixOne) || !isRegularMatrix(matrixTwo))
            {
                throw new ArgumentException("Non regular matrix detected. Rows size mismatch detected.");
            }
            if (matrixOne[0].Count != matrixTwo.Count)
            {
                throw new ArgumentException("Matrixes is not compatible. Columns count of first matrix is not equal to rows count of second matrix.");
            }
            List<List<int>> result = new List<List<int>>();
            for (int i = 0; i < matrixOne.Count; ++i)
            {
                result.Add(new List<int>());
                for (int j = 0; j < matrixTwo[0].Count; ++j)
                {
                    result[i].Add(0);
                }
            }
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            for (int i = 0; i < matrixOne.Count; ++i)
            {
                for (int j = 0; j < matrixTwo[0].Count; ++j)
                {
                    for (int k = 0; k < matrixTwo.Count; ++k)
                    {
                        result[i][j] += matrixOne[i][k] * matrixTwo[k][j];
                    }
                }
            }
            stopwatch.Stop();
            return new Tuple<List<List<int>>, TimeSpan>(result, stopwatch.Elapsed);
        }

        private static ComputePlatform GetGPU()
        {
            foreach (var platform in ComputePlatform.Platforms)
            {
                foreach (var device in platform.Devices)
                {
                    if (device.Type == ComputeDeviceTypes.Gpu)
                    {
                        return platform;
                    }
                }
            }
            return null;
        }

        public static Tuple<List<List<int>>, TimeSpan> MultiplyParallel(List<List<int>> matrixOne, List<List<int>> matrixTwo)
        {
            if (!isRegularMatrix(matrixOne) || !isRegularMatrix(matrixTwo))
            {
                throw new ArgumentException("Non regular matrix detected. Rows size mismatch detected.");
            }
            if (matrixOne[0].Count != matrixTwo.Count)
            {
                throw new ArgumentException("Matrixes is not compatible. Columns count of first matrix is not equal to rows count of second matrix.");
            }
            List<List<int>> result = new List<List<int>>();
            ComputePlatform platform = GetGPU();
            if (platform is null)
            {
                throw new PlatformNotSupportedException("Platform doesn't have a dedicated GPU. Run is impossible.");
            }
            ComputeContext context = new ComputeContext(ComputeDeviceTypes.Gpu, new ComputeContextPropertyList(platform), null, IntPtr.Zero);
            ComputeCommandQueue queue = new ComputeCommandQueue(context, context.Devices[0], ComputeCommandQueueFlags.None);
            ComputeProgram program = new ComputeProgram(context, CalculateKernel);
            program.Build(null, null, null, IntPtr.Zero);
            ComputeKernel kernel = program.CreateKernel("Multiply");

            List<ComputeBuffer<int>> rowsMatrixOne = matrixOne.TransformMatrixToComputerBuffersOfRows(context);
            List<ComputeBuffer<int>> columnsMatrixTwo = matrixTwo.TransformMatrixToComputerBuffersOfColumns(context);
            List<ComputeBuffer<int>> resultRowsMatrix = TwoDToOneDResult(matrixOne.Count, matrixTwo[0].Count, context);

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            for (int i = 0; i < resultRowsMatrix.Count; ++i)
            {
                for (int j = 0; j < resultRowsMatrix[i].Count; ++j)
                {
                    kernel.SetMemoryArgument(0, rowsMatrixOne[i]);
                    kernel.SetMemoryArgument(1, columnsMatrixTwo[j]);
                    kernel.SetMemoryArgument(2, resultRowsMatrix[i]);
                    kernel.SetValueArgument(3, matrixTwo.Count);
                    kernel.SetValueArgument(4, j);

                    queue.ExecuteTask(kernel, null);
                }
            }

            queue.Finish();
            stopwatch.Stop();

            for (int i = 0; i < resultRowsMatrix.Count; ++i)
            {
                int[] res = new int[resultRowsMatrix[i].Count];
                GCHandle gCHandle = GCHandle.Alloc(res, GCHandleType.Pinned);
                queue.Read<int>(resultRowsMatrix[i], true, 0, res.Length, gCHandle.AddrOfPinnedObject(), null);
                result.Add(new List<int>(res));
            }

            return new Tuple<List<List<int>>, TimeSpan>(result, stopwatch.Elapsed);
        }

        private static List<ComputeBuffer<int>> TwoDToOneDResult(int rows, int columns, ComputeContext context)
        {
            List<ComputeBuffer<int>> resultMatrix = new List<ComputeBuffer<int>>();
            for (int i = 0; i < rows; ++i)
            {
                resultMatrix.Add(new ComputeBuffer<int>(context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.UseHostPointer, new int[columns]));
            }
            return resultMatrix;
        }

        private static string CalculateKernel 
        {
            get
            {
                return @"
            kernel void Multiply(global int* m1, global int* m2, global int* m3, int size, int currIndex)
            {
                int sum = 0;
                for(int i = 0; i < size; i++)
                {
                    sum += m1[i] * m2[i];
                }
                m3[currIndex] = sum;
            }";
            }
        }
    }
}
