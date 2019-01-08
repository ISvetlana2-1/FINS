using System;

namespace ConsoleApp1
{
    class ConsumerParametersCalculation
    {
        ConsumerParameters parameters;

        public ConsumerParametersCalculation()
        {
            parameters = new ConsumerParameters();
        }

        public ConsumerParameters Calculate(double t, double[] r, double[] v, Quaternion n0, Quaternion l)
        {
            CoordinateCalculation(r, t);
            VelocityCalculation(t, v);
            AngleCalculation(t, n0, l);

            return parameters;
        }

        void CoordinateCalculation(double[] r, double t)
        {
            double d = Math.Sqrt(r[1] * r[1] + r[2] * r[2]);
            double lambda1, lambda2 = 0;

            if (d <= 10e-3)
            {
                parameters.Phi = (Math.PI * r[0]) / (2 * Math.Abs(r[0]));
                parameters.Lambda = 0;
                parameters.H = r[0] * Math.Sin(parameters.Phi) - MathExpansion.a
                    * Math.Sqrt(1 - MathExpansion.epsilon * MathExpansion.epsilon 
                    * Math.Sin(parameters.Phi) * Math.Sin(parameters.Phi));

                return;
            }

            lambda1 = Math.Asin(r[2] / d);

            if (r[1] >= 0)
                lambda2 = lambda1;
            else if (r[1] < 0)
                lambda2 = Math.Sign(r[2]) * Math.PI - lambda1;

            if (r[0] <= 10e-3)
            {
                parameters.Phi = 0;
                parameters.H = d - MathExpansion.a;
                parameters.Lambda = lambda2;

                return;
            }

            double rAux = Math.Sqrt(r[0] * r[0] + r[1] * r[1] + r[2] * r[2]);
            double c = Math.Asin(r[0] / rAux);
            double p = MathExpansion.epsilon * MathExpansion.epsilon * MathExpansion.a / (2 * rAux);

            double s1 = 0;

            double e = 0.000001;

            loopIter:
            double b = c + s1;
            double s2 = Math.Asin(p * Math.Sin(2 * b)
                / Math.Sqrt(1 - MathExpansion.epsilon * MathExpansion.epsilon * Math.Sin(b) * Math.Sin(b)));

            if (Math.Abs(s2 - s1) < e)
            {
                parameters.Phi = b;
                parameters.H = d * Math.Cos(parameters.Phi) + r[0] * Math.Sin(parameters.Phi) - MathExpansion.a
                    * Math.Sqrt(1 - MathExpansion.epsilon * MathExpansion.epsilon 
                    * Math.Sin(parameters.Phi) * Math.Sin(parameters.Phi));
            }
            else
            {
                s1 = s2;
                goto loopIter;
            }

            //Lambda = lambda2 - MathExpansion.om * t;                //?!
        }

        void VelocityCalculation(double t, double[] v)
        {
            Quaternion n = Quaternion.Proud(new Quaternion(Math.Cos((parameters.Lambda + MathExpansion.om * t) / 2),
                Math.Sin((parameters.Lambda + MathExpansion.om * t) / 2), 0, 0),
                new Quaternion(Math.Cos(parameters.Phi / 2), 0, 0, -Math.Sin(parameters.Phi / 2)));

            double[] v_xn = Quaternion.BasisTransform(n, v);

            double q = MathExpansion.a / Math.Sqrt(1 - MathExpansion.epsilon * MathExpansion.epsilon
                * Math.Sin(parameters.Phi) * Math.Sin(parameters.Phi));
            double v_z = MathExpansion.om * (q + parameters.H) * Math.Cos(parameters.Phi);

            parameters.V_n = v_xn[0];
            parameters.V_h = v_xn[1];
            parameters.V_e = v_xn[2] - v_z;
        }

        void AngleCalculation(double t, Quaternion n0, Quaternion l)
        {
            Quaternion n = Quaternion.Proud(new Quaternion(Math.Cos((parameters.Lambda + MathExpansion.om * t) / 2),
                Math.Sin((parameters.Lambda + MathExpansion.om * t) / 2), 0, 0),
                new Quaternion(Math.Cos(parameters.Phi / 2), 0, 0, -Math.Sin(parameters.Phi / 2)));

            Quaternion m = Quaternion.Proud(Quaternion.Proud(n.ConjugateQuaternion(), n0), l);

            double r1 = m.ScalarPart * m.ScalarPart + m.VectorPart[0] * m.VectorPart[0] 
                - m.VectorPart[1] * m.VectorPart[1] - m.VectorPart[2] * m.VectorPart[2];
            double r2 = m.ScalarPart * m.ScalarPart + m.VectorPart[1] * m.VectorPart[1] 
                - m.VectorPart[0] * m.VectorPart[0] - m.VectorPart[2] * m.VectorPart[2];

            parameters.Theta = Math.Asin(2 * m.ScalarPart * m.VectorPart[2] + 2 * m.VectorPart[1] * m.VectorPart[0]);
            parameters.Psi = Math.Sign(r1) * Math.Asin((-2 * m.ScalarPart * m.VectorPart[1] 
                + 2 * m.VectorPart[2] * m.VectorPart[0]) / Math.Cos(parameters.Theta)) 
                + Math.PI / 2 * (1 - Math.Sign(r1));
            parameters.Gama = Math.Sign(r2) * Math.Asin((2 * m.ScalarPart * m.VectorPart[0] 
                - 2 * m.VectorPart[2] * m.VectorPart[1]) / Math.Cos(parameters.Theta)) 
                + Math.PI / 2 * (1 - Math.Sign(r2));
        }
    }
}
