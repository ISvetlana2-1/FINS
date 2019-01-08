using System;

namespace ConsoleApp1
{
    class Quaternion
    {
        public double ScalarPart { get; private set; }        
        public double[] VectorPart { get; private set; }        

        public Quaternion()
        {
            ScalarPart = 0;
            VectorPart = new double[3];
            VectorPart[0] = 0;
            VectorPart[1] = 0;
            VectorPart[2] = 0;
        }

        public Quaternion(double l_0, double[] l)
        {
            ScalarPart = l_0;
            VectorPart = new double[3];
            VectorPart[0] = l[0];
            VectorPart[1] = l[1];
            VectorPart[2] = l[2];
        }

        public Quaternion(double l_0, double l_1, double l_2, double l_3)
        {
            ScalarPart = l_0;
            VectorPart = new double[3];
            VectorPart[0] = l_1;
            VectorPart[1] = l_2;
            VectorPart[2] = l_3;
        }

        public void Multiplication(double factor)
        {
            ScalarPart *= factor;

            for (int i = 0; i < 3; i++)
                VectorPart[i] *= factor;
        }
        
        public static Quaternion Proud(Quaternion l, Quaternion m)
        {
            Quaternion n = new Quaternion();

            n.ScalarPart = l.ScalarPart * m.ScalarPart - MathExpansion.ScalarProud(l.VectorPart, m.VectorPart);
            for (int i = 0; i < 3; i++)
                n.VectorPart[i] = l.ScalarPart * m.VectorPart[i] + m.ScalarPart * l.VectorPart[i] 
                    + MathExpansion.VectorProud(l.VectorPart, m.VectorPart)[i]; 

            return n;
        }

        public Quaternion ConjugateQuaternion() 
            => new Quaternion(ScalarPart, -VectorPart[0], -VectorPart[1], -VectorPart[2]);

        public static double[] BasisTransform(Quaternion l, double[] vec)
        {
            Quaternion r = Quaternion.Proud(Quaternion.Proud(l.ConjugateQuaternion(), new Quaternion(0, vec)), l);
            double[] result = { r.VectorPart[0], r.VectorPart[1], r.VectorPart[2] };
            return result;
        }

        public static double[] InverseBasisTransform(Quaternion l, double[] vec)
        {
            Quaternion r = Quaternion.Proud(Quaternion.Proud(l, new Quaternion(0, vec)), l.ConjugateQuaternion());
            double[] result = { r.VectorPart[0], r.VectorPart[1], r.VectorPart[2] };
            return result;
        }
    }   
}
