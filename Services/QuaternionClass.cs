using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRTest.Services
{
    public class QuaternionClass
    {
        public static double[,] QuaternionToMatrix(List<double> quat)
        {
            double w = quat[0], x = quat[1], y = quat[2], z = quat[3];
            return new double[,]
            {
            { 1 - 2*y*y - 2*z*z, 2*x*y - 2*z*w, 2*x*z + 2*y*w },
            { 2*x*y + 2*z*w, 1 - 2*x*x - 2*z*z, 2*y*z - 2*x*w },
            { 2*x*z - 2*y*w, 2*y*z + 2*x*w, 1 - 2*x*x - 2*y*y }
            };
        }

        public static double[,] MultiplyMatrices(double[,] a, double[,] b)
        {
            int n = a.GetLength(0);
            int m = a.GetLength(1);
            int p = b.GetLength(1);
            double[,] result = new double[n, p];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < p; j++)
                {
                    for (int k = 0; k < m; k++)
                    {
                        result[i, j] += a[i, k] * b[k, j];
                    }
                }
            }
            return result;
        }

        public static double[,] TransposeMatrix(double[,] matrix)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);
            double[,] transposed = new double[cols, rows];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    transposed[j, i] = matrix[i, j];
                }
            }
            return transposed;
        }

        public static double ExtractBendAngle(double[,] relativeRotationMatrix)
        {
            // Calculate the angle around the primary axis (e.g., x-axis)
            double angleX = Math.Atan2(relativeRotationMatrix[2, 1], relativeRotationMatrix[2, 2]);
            double angleY = Math.Atan2(-relativeRotationMatrix[2, 0], Math.Sqrt(relativeRotationMatrix[2, 1] * relativeRotationMatrix[2, 1] + relativeRotationMatrix[2, 2] * relativeRotationMatrix[2, 2]));
            double angleZ = Math.Atan2(relativeRotationMatrix[1, 0], relativeRotationMatrix[0, 0]);

            // Choose the relevant axis for the bend angle
            // Here, it's assumed that the bend is primarily around the Y-axis
            return angleY;
        }
    }





}
