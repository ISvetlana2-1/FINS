using System;
using System.Collections.Generic;

namespace ConsoleApp1
{
    class FINS
    {
        public ConsumerParameters input { get; private set; }     //входные данные
        public double T { get; private set; }                     //системное время (с)
        public double[] Omega { get; private set; }              //3-х мерный вектор средней на такте угловой скорости (м/с)  
        public double[] V { get; private set; }                   //3-х мерный вектор абсолютной скорости ПО в ГИСК (м/с)
        public double[] R_vec { get; private set; }              //радиус-вектор положения ПО в ГИСК (м)
        public Quaternion L { get; private set; }          //кватернион ориентации ПО относительно СИСК (б/р) соответсвенно
        readonly Quaternion n0;                         //кватернион ориентации СИСК относительно ГИСК (б/р)
        double q;                                           //радиус референц-эллипсоида в точке старта (м)
        double v_z;                                         //линейная скорость вращения Земли в точке старта (м/с)
        double r;                                           //расстояние от ПО до оси вращения Земли (м)
        double[] v;                                    //три проэкции вектора абсолютной скорости ПО на оси СИСК {Х0} (м/с)     

        public FINS(ConsumerParameters inputPars)
        {
            input = new ConsumerParameters(inputPars);
            T = 0;
            Omega = new double[3];
            q = MathExpansion.a / (Math.Sqrt(1 - MathExpansion.epsilon * MathExpansion.epsilon
                * Math.Sin(input.Phi) * Math.Sin(input.Phi)));
            v_z = MathExpansion.om * q * Math.Cos(input.Phi);
            r = (q + input.H) * Math.Cos(input.Phi);
            n0 = Quaternion.Proud(new Quaternion(Math.Cos(input.Lambda / 2), Math.Sin(input.Lambda / 2), 0, 0),
                new Quaternion(Math.Cos(input.Phi / 2), 0, 0, (-1) * Math.Sin(input.Phi / 2)));

            v = new double[3];
            v[0] = input.V_n;
            v[1] = input.V_h;
            v[2] = input.V_e + v_z;

            V = Quaternion.InverseBasisTransform(n0, v);

            R_vec = new double[3];
            R_vec[0] = ((1 - MathExpansion.epsilon * MathExpansion.epsilon) * q + input.H) * Math.Sin(input.Phi);
            R_vec[1] = r * Math.Cos(input.Lambda);
            R_vec[2] = r * Math.Sin(input.Lambda);

            L = Quaternion.Proud(new Quaternion(Math.Cos(input.Psi / 2), 0, -Math.Sin(input.Psi / 2), 0),
                new Quaternion(Math.Cos(input.Theta / 2), 0, 0, Math.Sin(input.Theta / 2)));
            L = Quaternion.Proud(L, new Quaternion(Math.Cos(input.Gama / 2), Math.Sin(input.Gama / 2), 0, 0));
        }

        public void Go(double[] nTheta, double[] nV, double nT, double tEx, List<ConsumerParameters> consParms)
        {
            double[] g = GravitationalAcceleratioVector(R_vec);

            double[] vScecified = MathExpansion.Sum(nTheta, 
                MathExpansion.Multiplication(1 / 2, MathExpansion.VectorProud(nTheta, nV)));

            double[] v1 = new double[3];            //3-х мерный вектор абсолютной скорости с предыдущего такта
            for (int i = 0; i < v1.Length; i++)
                v1[i] = V[i];

            double[] nTheta1 = MathExpansion.Multiplication(nT, Omega);
            Omega = MathExpansion.Division(nTheta, nT);

            Quaternion s = Quaternion.Proud(n0, L);

            double[] deltaV = MathExpansion.Sum(Quaternion.InverseBasisTransform(s, vScecified), 
                MathExpansion.Multiplication(nT, g));
            V = MathExpansion.Sum(V, deltaV);

            R_vec = MathExpansion.Sum(R_vec, MathExpansion.Multiplication(nT, 
                MathExpansion.Multiplication(1 / 2, MathExpansion.Sum(v1, V))));

            double dl_0 = 1 - MathExpansion.Module(nTheta) * MathExpansion.Module(nTheta) / 8;
            double[] dl = MathExpansion.Multiplication(1 / 2, nTheta);
            dl = MathExpansion.Sum(dl, MathExpansion.Multiplication(-MathExpansion.Module(nTheta) 
                * MathExpansion.Module(nTheta) / 48, nTheta));
            dl = MathExpansion.Sum(dl, MathExpansion.Multiplication(-1 / 24, MathExpansion.VectorProud(nTheta, nTheta1)));
            Quaternion deltaL = new Quaternion(dl_0, dl);

            deltaL.Multiplication(1 / Math.Sqrt(dl_0 * dl_0 + MathExpansion.ScalarProud(dl, dl)));
            L = Quaternion.Proud(L, deltaL);

            T += nT;

            ConsumerParameters result = new ConsumerParameters();
            ConsumerParametersCalculation cpCalc = new ConsumerParametersCalculation();
            result = cpCalc.Calculate(T, R_vec, V, n0, L);
            consParms.Add(result);

            if (T < tEx)
            Go(nTheta, nV, nT, tEx, consParms);
        }

        double[] GravitationalAcceleratioVector(double[] r)
        {
            const double m = 3.9860009e14;          //геоцентрическая гравитационная постоянная Земли
            const double c = -1082.63e-6;           //коэф. при второй зональной гармонике разложения геопотенциала в ряд
                                                    //по сферическим ф-циям
            double modR = Math.Sqrt(r[0] * r[0] + r[1] * r[1] + r[2] * r[2]);
            double f = 5 * r[2] / (modR * modR);

            double[] g = new double[3];
            g = MathExpansion.Multiplication(-m / (modR * modR * modR), r);

            double[] temp = { (3 - f) * r[0], (1 - f) * r[1], (1 - f) * r[2] };
            g = MathExpansion.Sum(g, MathExpansion.Multiplication((3 / 2) * c * m 
                * MathExpansion.a * MathExpansion.a / Math.Pow(modR, 5), temp));

            return g;
        }
    }
}
