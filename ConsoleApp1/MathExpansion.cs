using System;

namespace ConsoleApp1
{
    static class MathExpansion
    {
        public const int a = 6378137;                           //большая полуось земного эллипсоида (м)
        public const double epsilon = 0.0818192;                //эксцентриситет (б/р)       
        public const double om = 7.292110e-5;                   //угловая скорость вращения Земли (рад/с)

        public static double[] Sum(double[] a, double[] b)
        {
            double[] result = new double[a.Length];

            for (int i = 0; i < result.Length; i++)
                result[i] = a[i] + b[i];

            return result;
        }

        public static double[] Multiplication(double sc, double[] vec)
        {
            double[] result = new double[vec.Length];

            for (int i = 0; i < result.Length; i++)
                result[i] = sc * vec[i];

            return result;
        }

        public static double[] Division(double[] vec, double sc)
        {
            double[] result = new double[vec.Length];

            for (int i = 0; i < result.Length; i++)
                result[i] = vec[i] / sc;

            return result;
        }

        public static double ScalarProud(double[] a, double[] b)
        {
            double result = 0;

            for (int i = 0; i < a.Length; i++)
                result += a[i] * b[i];

            return result;
        }

        public static double[] VectorProud(double[] a, double[] b)
        {
            double[] result = new double[3];

            result[0] = a[1] * b[2] - a[2] * b[1];
            result[1] = -(a[0] * b[2] - a[2] * b[0]);
            result[2] = a[0] * b[2] - a[1] * b[0];

            return result;
        }

        public static double Module(double[] vec)
        {
            double temp = 0;

            for (int i = 0; i < vec.Length; i++)
                temp += vec[i] * vec[i];

            return Math.Sqrt(temp);
        }
    }
}
