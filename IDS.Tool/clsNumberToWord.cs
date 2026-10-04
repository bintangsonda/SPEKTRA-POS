using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Tool
{
    public class clsNumberToWord
    {
        public string NumToWordInd(double x, int type)
        {
            x = Convert.ToDouble(String.Format("{0:0.00}", x));
            double trillion;
            double billion;
            double million;
            double thousand;
            double unit;
            double cent;
            string baca = "";
            string ccy = "";

            switch (type)
            {
                case 1: ccy = "Rupiah "; break;
                case 2: ccy = "US Dollars "; break;
                case 3: ccy = "Yen "; break;
                default: ccy = " "; break;
            }


            //If x=0, read as 0
            if (x == 0)
                baca = UnderTwentyInd(0, 1);
            else
            {
                trillion = (int)Math.Floor((x * Math.Pow(0.001, 4)));
                billion = (int)Math.Floor((x - (trillion / Math.Pow(0.001, 4))) * Math.Pow(0.001, 3));
                million = (int)Math.Floor(((x - trillion / Math.Pow(0.001, 4)) - (billion * Math.Pow(1000, 3))) / Math.Pow(1000, 2));
                thousand = (int)Math.Floor(((x - trillion / Math.Pow(0.001, 4)) - (billion * Math.Pow(1000, 3)) - (million * Math.Pow(1000, 2))) / 1000);
                unit = (int)Math.Floor((x - trillion / Math.Pow(0.001, 4)) - (billion * Math.Pow(1000, 3)) - (million * Math.Pow(1000, 2)) - (thousand * 1000));
                double c = (Int64)Math.Floor(x);
                cent = (long)((x * 100) - (c * 100) + 0.5);
                if (trillion > 0)
                    baca = HundredInd(Convert.ToInt16(trillion), 5) + "Triliun ";
                if (billion > 0)
                    baca = baca + HundredInd(Convert.ToInt16(billion), 4) + "Milyar ";
                if (million > 0)
                    baca = baca + HundredInd(Convert.ToInt16(million), 3) + "Juta ";
                if (thousand > 0)
                    baca = baca + HundredInd(Convert.ToInt16(thousand), 2) + "Ribu ";
                if (unit > 0)
                    baca = baca + HundredInd(Convert.ToInt16(unit), 1) + ccy;
                else
                    baca = baca + ccy;
                if (cent > 0)
                    baca = baca + HundredInd(Convert.ToInt16(cent), 0) + "Sen ";
            }
            return baca;
        }

        private string HundredInd(int x, int pos)
        {
            int a100, a10, a1;
            string baca = "";
            a100 = (int)Math.Floor(x * 0.01);
            a10 = (int)Math.Floor((x - a100 * 100) * 0.1);
            a1 = (int)(x - a100 * 100 - a10 * 10);
            //Read Hundred Part
            if (a100 == 1)
                baca = "Seratus ";
            else
            {
                if (a100 > 0)
                {
                    baca = UnderTwentyInd(a100, 2) + "Ratus ";
                }
            }

            //Read Under Hundred Part
            if (a10 == 1)
                baca = baca + UnderTwentyInd(a10 * 10 + a1, 2);
            else
            {
                if (a10 > 0)
                    baca = baca + UnderTwentyInd(a10, 2) + "Puluh ";
                if (a1 > 0)
                {
                    if (pos == 2 && a100 == 0 && a10 == 0)
                        baca = baca + UnderTwentyInd(a1, 1);
                    else
                        baca = baca + UnderTwentyInd(a1, 2);
                }
            }
            return baca;
        }

        private string UnderTwentyInd(int x, int pos)
        {
            switch (x)
            {
                case 0: return "Nol";
                case 1:
                    if (pos == 2)
                        return "Satu ";
                    else
                        return "Se";
                case 2: return "Dua ";
                case 3: return "Tiga ";
                case 4: return "Empat ";
                case 5: return "Lima ";
                case 6: return "Enam ";
                case 7: return "Tujuh ";
                case 8: return "Delapan ";
                case 9: return "Sembilan ";
                case 10: return "Sepuluh ";
                case 11: return "Sebelas ";
                case 12: return "Dua Belas ";
                case 13: return "Tiga Belas ";
                case 14: return "Empat Belas ";
                case 15: return "Lima Belas ";
                case 16: return "Enam Belas ";
                case 17: return "Tujuh Belas ";
                case 18: return "Delapan Belas ";
                case 19: return "Sembilan Belas ";
                default: return " ";
            }
        }

    }
}
