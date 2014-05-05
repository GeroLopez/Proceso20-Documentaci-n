using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Proceso20
{
    public partial class diag1 : Form
    {
        int di1 = 0, di2 = 0;

        public diag1()
        {
            InitializeComponent();
        }

        public string Fech1
        {
            set { textfe1.Text = value; }
            get { return textfe1.Text; }
        }

        public string Fech2
        {
            set { textfe2.Text = value; }
            get { return textfe2.Text; }
        }

        public string Dura
        {
            set { textdur.Text = value; }
            get { return textdur.Text; }
        }

        public string Usua
        {
            set { textusu.Text = value; }
            get { return textusu.Text; }
        }

        private void diag1_Load(object sender, EventArgs e)
        {

            int an, me, dia;
            string fe = "";

            fe = textfe1.Text;
            di1 = int.Parse(fe);
            an = int.Parse(fe.Substring(0, 4));
            me = int.Parse(fe.Substring(4, 2));
            dia = int.Parse(fe.Substring(6, 2));
            DateTime fech1 = new DateTime(an, me, dia);
            calen1.SetDate(fech1);
            fe = textfe2.Text;
            di2 = int.Parse(fe);
            an = int.Parse(fe.Substring(0, 4));
            me = int.Parse(fe.Substring(4, 2));
            dia = int.Parse(fe.Substring(6, 2));
            DateTime fech2 = new DateTime(an, me, dia);
            calen2.SetDate(fech2);

            return;
        }

        private void textfe1_TextChanged(object sender, EventArgs e)
        {
            int an, me, dia;
            string fe = "";
            DateTime fech;

            if (textfe1.Text.Length == 8)
            {
                fe = textfe1.Text;
                di1 = int.Parse(fe);
                an = int.Parse(fe.Substring(0, 4));
                me = int.Parse(fe.Substring(4, 2));
                dia = int.Parse(fe.Substring(6, 2));
                fech = new DateTime(an, me, dia);
               /* if (di2 - di1 > 7)
                {
                    ll = fech.Ticks + 6048000000000;
                    DateTime fechcal2 = new DateTime(ll);
                    textfe2.Text = string.Format("{0:yyyy}{0:MM}{0:dd}", fechcal2);
                    calen2.SetDate(fechcal2);
                }*/
                /*else*/ if (di2 - di1 < 0)
                {
                    textfe2.Text = textfe1.Text;
                }
                calen1.SetDate(fech);
            }
        }

        private void textfe2_TextChanged(object sender, EventArgs e)
        {
            int an, me, dia;
            string fe = "";
            DateTime fech;

            if (textfe2.Text.Length == 8)
            {
                fe = textfe2.Text;
                di2 = int.Parse(fe);
                an = int.Parse(fe.Substring(0, 4));
                me = int.Parse(fe.Substring(4, 2));
                dia = int.Parse(fe.Substring(6, 2));
                fech = new DateTime(an, me, dia);
               /* if (di2 - di1 > 7)
                {
                    ll = fech.Ticks - 6048000000000;
                    DateTime fechcal1 = new DateTime(ll);
                    textfe1.Text = string.Format("{0:yyyy}{0:MM}{0:dd}", fechcal1);
                    calen1.SetDate(fechcal1);
                }*/
                /*else*/ if (di2 - di1 < 0)
                {
                    textfe1.Text = textfe2.Text;
                }
                calen2.SetDate(fech);
            }
        }

        private void calen1_DateSelected(object sender, DateRangeEventArgs e)
        {
            string an, me, dia, fee;

            an = calen1.SelectionStart.Year.ToString();
            me = calen1.SelectionStart.Month.ToString("00");
            dia = calen1.SelectionStart.Day.ToString("00");
            fee = an + me + dia;
            di1 = int.Parse(fee);
            textfe1.Text = fee;
           /* if (di2 - di1 > 7)
            {
                DateTime fechcal = new DateTime(calen1.SelectionStart.Year, calen1.SelectionStart.Month, calen1.SelectionStart.Day);
                ll = fechcal.Ticks + 6048000000000;
                DateTime fechcal2 = new DateTime(ll);
                textfe2.Text = string.Format("{0:yyyy}{0:MM}{0:dd}", fechcal2);
            }*/
            /*else*/ if (di2 - di1 < 0)
            {
                textfe2.Text = textfe1.Text;
            }
            return;
        }

        private void calen2_DateSelected(object sender, DateRangeEventArgs e)
        {
            string an, me, dia, fee;

            an = calen2.SelectionStart.Year.ToString();
            me = calen2.SelectionStart.Month.ToString("00");
            dia = calen2.SelectionStart.Day.ToString("00");
            fee = an + me + dia;
            di2 = int.Parse(fee);
            textfe2.Text = fee;
          /*  if (di2 - di1 > 7)
            {
                DateTime fechcal = new DateTime(calen2.SelectionStart.Year, calen2.SelectionStart.Month, calen2.SelectionStart.Day);
                ll = fechcal.Ticks - 6048000000000;
                DateTime fechcal2 = new DateTime(ll);
                textfe1.Text = string.Format("{0:yyyy}{0:MM}{0:dd}", fechcal2);
            }*/
           /* else*/ if (di2 - di1 < 0)
            {
                textfe1.Text = textfe2.Text;
            }
            return;
        }


    }
}
