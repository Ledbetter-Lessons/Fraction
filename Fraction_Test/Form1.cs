using System;
using System.Windows.Forms;

namespace Fraction_Test
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //each of the following declerations are equivelant
            Fraction f1 = "1/2";
            Fraction f2 = new Fraction(1 * 999999, 2 * 999999).Simplify();    // * 999999 to show simplify function
            Fraction f3 = 0.5; 


            rtb.Text =     "Fraction f1 = \"1/2\"                     = " + f1.ToString() + "    or decimal: " + f1.toDecimal() + "\n";
            rtb.AppendText("Fraction f2 = new Fraction(1, 2)    = " + f2.ToString() + "    or decimal: " + f1.toDecimal() + "\n");
            rtb.AppendText("Fraction f3 = 0.5                         = " + f3.ToString() + "\n" + "\n");

            rtb.AppendText("\n" + "Add fractions" + "\n");
            rtb.AppendText(f1.ToString() + " + " + f2.ToString() + " = " + (f1 + f2).ToString() + "\n");
            rtb.AppendText("\n" + "Subtract fractions" + "\n");
            rtb.AppendText(f1.ToString() + " - " + f2.ToString() + " = " + (f1 - f2).ToString() + "\n");
            rtb.AppendText("\n" + "Multiply fractions" + "\n");
            rtb.AppendText(f1.ToString() + " * " + f2.ToString() + " = " + (f1 * f2).ToString() + "\n");
            rtb.AppendText("\n" + "Divide fractions" + "\n");
            rtb.AppendText(f1.ToString() + " / " + f2.ToString() + " = " + (f1 / f2).ToString() + "\n");
            rtb.AppendText("\n" + "Raise fraction to a fraction power" + "\n");
            rtb.AppendText(f1.ToString() + " ^ " + f2.ToString() + " = " + (f1 ^ f2).ToString() + "    or decimal: " + (f1 ^ f2).toDecimal() + "\n");
            rtb.AppendText("\n" + "Find the Nth root of a fraction. (Nth root can be a fraction)" + "\n");
            rtb.AppendText(f1.ToString() + ".Root( " + f2.ToString() + ") = " + f1.Root(f2).ToString() + "    or decimal: " + f1.Root(f2).toDecimal() + "\n" + "\n");

            rtb.AppendText("f1 numerator is: " + f1.N + "\n");
            rtb.AppendText("f1 denominator is: " + f1.D + "\n");
        }
    }
}
