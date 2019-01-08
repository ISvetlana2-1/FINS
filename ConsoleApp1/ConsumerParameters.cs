using System;

namespace ConsoleApp1
{
    class ConsumerParameters
    {
        public double Phi { get; set; }             //географическая широта (рад)
        public double H { get; set; }               //высота (м)
        public double Lambda { get; set; }          //долгота (рад)
        public double V_n { get; set; }             //северная,
        public double V_h { get; set; }             //вертикальная
        public double V_e { get; set; }             //и восточная составляющие скорости (м/с)
        public double Psi { get; set; }             //угол курса (рад)
        public double Theta { get; set; }           //угол тангажа (рад)
        public double Gama { get; set; }            //угол крена (рад)

        public ConsumerParameters() { }

        public ConsumerParameters(double phi, double h, double lambda, double v_n, double v_h, double v_e,
            double psi, double theta, double gama)
        {
            Phi = phi;
            H = h;
            Lambda = lambda;
            V_n = v_n;
            V_h = v_h;
            V_e = v_e;
            Psi = psi;
            Theta = theta;
            Gama = gama;
        }

        public ConsumerParameters(ConsumerParameters cp)
        {
            Phi = cp.Phi;
            H = cp.H;
            Lambda = cp.Lambda;
            V_n = cp.V_n;
            V_h = cp.V_h;
            V_e = cp.V_e;
            Psi = cp.Psi;
            Theta = cp.Theta;
            Gama = cp.Gama;
        }
    }
}
