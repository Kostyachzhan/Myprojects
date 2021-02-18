using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CoordsTranslator
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void move_Click(object sender, RoutedEventArgs e)
        {
            double npos_x = double.Parse(x_transition.Text);
            double npos_y = double.Parse(y_transition.Text);
            double npos_z = double.Parse(z_transition.Text);
            MeshGeometry3D geometry = (MeshGeometry3D)cube.Geometry;
            var points = geometry.Positions;
            var transition_matrix = GetTransitionMatrix(npos_x, npos_y, npos_z);

            Move(transition_matrix, points);
        }

        private double[,] GetTransitionMatrix(double x, double y, double z)
        {
            var transition_matrix = new double[4,4];
            for (int i = 0; i < 4; ++i)
            {
                for (int j = 0; j < 4; ++j)
                {
                    if (i < 3)
                    {
                        if (i == j)
                        {
                            transition_matrix[i, j] = 1;
                        }
                        else
                        {
                            transition_matrix[i, j] = 0;
                        }
                    }
                }
            }
            transition_matrix[3, 0] = x;
            transition_matrix[3, 1] = y;
            transition_matrix[3, 2] = z;
            transition_matrix[3, 3] = 1;

            return transition_matrix;
        }

        private void Move(double[,] transitionMatrix, Point3DCollection points)
        {
            for (int i = 0; i < points.Count; ++i)
            {
                var point_matrix = new double[1, 4] { { points[i].X, points[i].Y, points[i].Z, 1 } };
                var result = MultiplyMatrix(point_matrix, transitionMatrix);
                if(result == null)
                {
                    throw new NullReferenceException();
                }
                points[i] = new Point3D(result[0, 0], result[0, 1], result[0, 2]);
            }
        }

        private double[,] MultiplyMatrix(double[,] A, double[,] B)
        {
            int rA = A.GetLength(0);
            int cA = A.GetLength(1);
            int rB = B.GetLength(0);
            int cB = B.GetLength(1);
            double temp = 0;
            double[,] kHasil = new double[rA, cB];
            if (cA != rB)
            {
                return null;
            }
            else
            {
                for (int i = 0; i < rA; i++)
                {
                    for (int j = 0; j < cB; j++)
                    {
                        temp = 0;
                        for (int k = 0; k < cA; k++)
                        {
                            temp += A[i, k] * B[k, j];
                        }
                        kHasil[i, j] = temp;
                    }
                }
                return kHasil;
            }
        }
    }
}
