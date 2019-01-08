using System;
using System.Collections.Generic;
using System.IO;

namespace ConsoleApp1
{
    class Program
    {
        static void Main()
        {
            List<ConsumerParameters> consumerParameters = new List<ConsumerParameters>();

            double phi0, h0, lambda0, vn0, vh0, ve0, psi0, theta0, gama0;

            #region Ввод начальных данных
            do
            {
                Console.WriteLine("Initial value of latitude:");
            } while (!Double.TryParse(Console.ReadLine(), out phi0));

            do
            {
                Console.WriteLine("Initial value of longitude:");
            } while (!Double.TryParse(Console.ReadLine(), out lambda0));

            do
            {
                Console.WriteLine("Initial value of height:");
            } while (!Double.TryParse(Console.ReadLine(), out h0));

            do
            {
                Console.WriteLine("Initial value of north velocity component:");
            } while (!Double.TryParse(Console.ReadLine(), out vn0));

            do
            {
                Console.WriteLine("Initial value of vertical velocity component:");
            } while (!Double.TryParse(Console.ReadLine(), out vh0));

            do
            {
                Console.WriteLine("Initial value of east velocity component:");
            } while (!Double.TryParse(Console.ReadLine(), out ve0));

            do
            {
                Console.WriteLine("Initial value of course angle:");
            } while (!Double.TryParse(Console.ReadLine(), out psi0));

            do
            {
                Console.WriteLine("Initial value of pitch angle:");
            } while (!Double.TryParse(Console.ReadLine(), out theta0));

            do
            {
                Console.WriteLine("Initial value of roll angle:");
            } while (!Double.TryParse(Console.ReadLine(), out gama0));
            #endregion

            ConsumerParameters initial = new ConsumerParameters(phi0, h0, lambda0, vn0, vh0, ve0, psi0, theta0, gama0);
            consumerParameters.Add(initial);

            FINS fins = new FINS(initial);

            double[] nTheta = new double[3];
            double[] nV = new double[3];
            double dt, tEx;

            #region Ввод начальных данных для циклической части алгоритма
            loopTheta:
            Console.WriteLine("Increment of the angle of apparent rotation (like \"x y z\"):");
            string s = Console.ReadLine();
            string[] prms = s.Split(' ');
            if (nTheta.Length != prms.Length)
            {
                Console.WriteLine("Incorrect data! Try again.");
                goto loopTheta;
            }
            for (int i = 0; i < nTheta.Length; i++)
                if (!Double.TryParse(prms[i], out nTheta[i]))
                {
                    Console.WriteLine("Incorrect data! Try again.");
                    goto loopTheta;
                }

            loopV:
            Console.WriteLine("Increment of apparent velocity vector (like \"x y z\"):");
            s = Console.ReadLine();
            prms = s.Split(' ');
            if (nV.Length != prms.Length)
            {
                Console.WriteLine("Incorrect data! Try again.");
                goto loopTheta;
            }
            for (int i = 0; i < nV.Length; i++)
                if (!Double.TryParse(prms[i], out nV[i]))
                {
                    Console.WriteLine("Incorrect data! Try again.");
                    goto loopV;
                }

            do
            {
                Console.WriteLine("Time between updates:");
            } while (!Double.TryParse(Console.ReadLine(), out dt));

            do
            {
                Console.WriteLine("Exit time:");
            } while (!Double.TryParse(Console.ReadLine(), out tEx));
            #endregion

            fins.Go(nTheta, nV, dt, tEx, consumerParameters);

            #region Вывод полученных данных в файл
            using (StreamWriter sw = new StreamWriter("D:\\result.txt"))
            {
                sw.WriteLine("Phi:");
                foreach (ConsumerParameters cp in consumerParameters)
                    sw.WriteLine(cp.Phi);
                sw.WriteLine();

                sw.WriteLine("Lambda:");
                foreach (ConsumerParameters cp in consumerParameters)
                    sw.WriteLine(cp.Lambda);
                sw.WriteLine();

                sw.WriteLine("V_n:");
                foreach (ConsumerParameters cp in consumerParameters)
                    sw.WriteLine(cp.V_n);
                sw.WriteLine();

                sw.WriteLine("V_h:");
                foreach (ConsumerParameters cp in consumerParameters)
                    sw.WriteLine(cp.V_h);
                sw.WriteLine();

                sw.WriteLine("V_e:");
                foreach (ConsumerParameters cp in consumerParameters)
                    sw.WriteLine(cp.V_e);
                sw.WriteLine();

                sw.WriteLine("Psi:");
                foreach (ConsumerParameters cp in consumerParameters)
                    sw.WriteLine(cp.Psi);
                sw.WriteLine();

                sw.WriteLine("Theta:");
                foreach (ConsumerParameters cp in consumerParameters)
                    sw.WriteLine(cp.Theta);
                sw.WriteLine();

                sw.WriteLine("Gama:");
                foreach (ConsumerParameters cp in consumerParameters)
                    sw.WriteLine(cp.Gama);
                sw.WriteLine();
            }
            #endregion

            Console.ReadLine();
        }
    }
}