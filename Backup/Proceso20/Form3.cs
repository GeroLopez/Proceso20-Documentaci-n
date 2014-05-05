using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
//using Word = Microsoft.Office.Interop.Word;  // esta linea permite los reportes automaticos
using System.Reflection;
using System.Globalization;

namespace Proceso20
{
    public partial class Form3 : Form
    {
        Form1 fr1;
        const double Fei = 621355968000000000.0;
        const double Feisuds = 621355968000000000.0;
        const int Ma = 300;   // OJO: aqui debe colocarse el mismo valor que en el Form1!!.

        ushort nutra, sipro, nuvolestaloc, lec = 0;
        short vol, refe = 1;
        int bx1, by1, xam = -1, yam;
        float amp = 1.0F;
        double durx, dura, tminx, tmini, diff, lat, lot;
        double[] tle1;
        double[] tle2;
        char[] pol;
        //private Image myPicture = null;
        int[] lectu;
        byte[] clR;
        byte[] clG;
        byte[] clB;

        int[] lar;
        int[] mx;
        int[] mn;
        int[] pro;
        short[] by;
        short[] ga;
        double[] ra;
        double[][] tim;
        double[] sen;
        double[] laa;
        double[] loo;
        int[][] cu;
        string[] est;
        bool[] siFac;
        Boolean[] siEst;
        bool grabas = true, load = false, guia = false, grafoc = false, grapasto = false;
        bool quitar=false;
        string usu = "";
        Color col = Color.Red;
        Color colfondo, colinea, colotr1, colP, colS, colC;
        ToolTip tip = new ToolTip();
        Util util = new Util();

        public Form3(Form1 frmm1)
        {
            this.fr1 = frmm1;
            InitializeComponent();
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            int i, j, k, nmi, nmf, lari, tot;
            double tm, tx;
            string li = "";
            string caa = "";

            colinea = fr1.colinea;
            colfondo = fr1.colfondo;
            colotr1 = fr1.colotr1;
            colP = fr1.colP;
            colS = fr1.colS;
            colC = fr1.colC;
            panel1.BackColor = colfondo;
            usu = fr1.usu;
            load = false;
            diff = 0.01;          
            labelsis.Text = fr1.sismo.Substring(0,12);
            if (File.Exists("reviatn.txt")) File.Delete("reviatn.txt");

            tminx = 0;
            durx = 0;
            tm = 0;
            tx = 0;
            vol = 0;
            for (i = 0; i <= fr1.nuvol; i++)
            {
                if (fr1.volcan[i][0] == fr1.sismo[9])
                {
                    vol = (short)(i);
                    break;
                }
            }
            lat = fr1.latvol[vol];
            lot = fr1.lonvol[vol];
            for (i = 0; i <= fr1.nuvol; i++)
            {
                bvol[i] = new Button();
                bvol[i].Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
                // bvol[i].Location = new Point(i * 43, 0);
                if (i < 15) bvol[i].Location = new Point(i * 43, 0);
                else
                {
                    if (i < fr1.nuvol) bvol[i].Location = new Point((i - 15) * 43, 1);
                    else bvol[i].Location = new Point(15 * 43, 0);
                }
                bvol[i].Size = new Size(45, 20);
                bvol[i].Text = fr1.volcan[i];
                bvol[i].TabIndex = i;
                if (i == vol) bvol[i].BackColor = Color.Yellow;
                else bvol[i].BackColor = Color.LightYellow;
                bvol[i].Font = new Font("Microsoft Sans Serif", 7);
                this.bvol[i].Click += new System.EventHandler(this.bvol_Click);
                //this.Controls.Add(bvol[i]);
                if (i < 15) this.Controls.Add(bvol[i]);
                else
                {
                    if (i < fr1.nuvol) this.panel3.Controls.Add(bvol[i]);
                    else this.Controls.Add(bvol[i]);
                }
            }
            if (i >= 15)
            {
                panel3.Size = new Size((i - 17) * 45, 22);
                boMasVol.Visible = true;
            }


            j = 0;
            if (File.Exists(".\\pro\\estacajon.txt"))
            {
                StreamReader ar = new StreamReader(".\\pro\\estacajon.txt");
                while (li != null)
                {
                    try
                    {
                        li = ar.ReadLine();
                        if (li == null) break;
                        if (char.IsDigit(li[0]))
                        {
                            i = int.Parse(li.Substring(0, 2));
                            if (i > j) j = i;
                        }
                    }
                    catch
                    {
                    }
                }
                ar.Close();
            }
            nuvolestaloc = (ushort)(j);
            //MessageBox.Show("j=" + j.ToString());
            if (j > 0)
            {
                for (i = 0; i < j; i++)
                {
                    best[i] = new Button();
                    best[i].Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
                    //best[i].Location = new Point(93 + i * 29, 544);
                    best[i].Location = new Point(Size.Width - 38, i * 22 + 1);
                    best[i].Size = new Size(30, 22);
                    best[i].Text = (i + 1).ToString();
                    best[i].TabIndex = i + 1;
                    best[i].BackColor = Color.Peru;
                    best[i].Font = new Font("Microsoft Sans Serif", 7);
                    this.best[i].Click += new System.EventHandler(this.best_Click);
                    this.Controls.Add(best[i]);
                }
            }

            nutra = fr1.nutra;
            lectu = new int[nutra];
            lar = new int[nutra];
            mx = new int[nutra];
            mn = new int[nutra];
            pro = new int[nutra];
            ra = new double[nutra];
            tim = new double[nutra][];
            sen = new double[nutra];
            laa = new double[nutra];
            loo = new double[nutra];
            cu = new int[nutra][];
            siEst = new bool[nutra];
            siFac = new bool[nutra];
            by = new short[nutra];
            ga = new short[nutra];
            est = new string[nutra];
            clR = new byte[nutra];
            clG = new byte[nutra];
            clB = new byte[nutra];
            tle1 = new double[nutra];
            tle2 = new double[nutra];
            pol = new char[nutra];
            for (i = 0; i < nutra; i++)
            {
                lectu[i] = -1;
                tle1[i] = 0;
                tle2[i] = 0;
                tot = fr1.tim[i].Length;
                for (k = 0; k < tot; k++)
                {
                    if (fr1.tim[i][k] >= fr1.tie1) break;
                }
                nmi = k;
                for (k = nmi; k < tot; k++)
                {
                    if (fr1.tim[i][k] >= fr1.tie2) break;
                }
                nmf = k;
                // nmi = (int)((fr1.tie1 - fr1.tim[i][0]) * fr1.ra[i]);
                //nmf = (int)((fr1.tie2 - fr1.tim[i][0]) * fr1.ra[i]);
                lari = nmf - nmi;
                lar[i] = lari;
                cu[i] = new int[lari];
                tim[i] = new double[lari];
                ra[i] = fr1.ra[i];
                ga[i] = fr1.ga[i];
                by[i] = fr1.by[i];
                est[i] = fr1.est[i];                
                for (j = nmi; j < nmf; j++)
                {
                    cu[i][j - nmi] = fr1.cu[i][j];
                    tim[i][j - nmi] = fr1.tim[i][j];
                }              
            }
            for (i = 0; i < nutra; i++) siEst[i] = true;

            for (i = 0; i < nutra; i++)
            {
                mx[i] = cu[i][0];
                mn[i] = cu[i][0];
                for (j = 1; j < lar[i]; j++)
                {
                    if (mx[i] < cu[i][j]) mx[i] = cu[i][j];
                    else if (mn[i] > cu[i][j]) mn[i] = cu[i][j];
                }
            }
            tm = tim[0][0];
            tx = tm;
            for (i = 1; i < nutra; i++)
            {
                if (tm > tim[i][0]) tm = tim[i][0];
                if (tx < tim[i][lar[i] - 1]) tx = tim[i][lar[i] - 1];
            }
            tminx = tm;
            tmini = tminx;
            durx = tx - tm;
            dura = durx;
            for (i = 0; i < nutra; i++) pro[i] = (int)((mx[i] + mn[i]) / 2.0F);
            sipro = 0;

            load = true;
            li = fr1.rutbas + "\\pro\\atepasto.txt";
            if (File.Exists(li))
            {
                grapasto = true;
                bofoc.Text = "MATB";
            }
            else grapasto = false;

            LeeFactores();

            return;
        }

        private void bvol_Click(object sender, EventArgs e)
        {
            Button bt = (Button)sender;
            vol = (short)(bt.TabIndex);
            BotonVol();
            return;
        }
        void BotonVol()
        {
            int i;

            for (i = 0; i <= fr1.nuvol; i++)
            {
                if (i == vol) bvol[i].BackColor = Color.Yellow;
                else bvol[i].BackColor = Color.LightYellow;
            }
            return;
        }

        private void best_Click(object sender, EventArgs e)
        {
            int i, j, k;
            string li = "";

            if (nuvolestaloc <= 0) return;

            Button bt = (Button)sender;
            j = (short)(bt.TabIndex);
            //MessageBox.Show("j=" + j.ToString());
            for (i = 0; i < nutra; i++) siEst[i] = false;

            StreamReader ar = new StreamReader(".\\pro\\estacajon.txt");
            while (li != null)
            {
                try
                {
                    li = ar.ReadLine();
                    if (li == null) break;
                    if (char.IsLetter(li[0]))
                    {
                        k = int.Parse(li.Substring(5));
                        if (k == j)
                        {
                            for (i = 0; i < nutra; i++)
                            {
                                if (string.Compare(est[i].Substring(0, 4), li.Substring(0, 4)) == 0)
                                {
                                    siEst[i] = true;
                                    break;
                                }
                            }
                        }
                    }
                    else if (char.IsDigit(li[0]))
                    {
                        k = int.Parse(li.Substring(0, 2));
                        if (k == j)
                        {
                            for (i = 0; i < fr1.nuvol; i++)
                            {
                                if (fr1.volcan[i][0] == li[2])
                                {
                                    vol = (short)(i);
                                    break;
                                }
                            }
                        }
                    }
                }
                catch
                {
                }
            }
            ar.Close();
            BotonVol();
            panel1.Invalidate();
            if (panelfoc.Visible == true) VerAtenuacion(col);

            return;
        }

        void LeeFactores()
        {
            int i, j, fe1, fe2, fe, ii;
            long ll;
            double ff;
            char[] delim = { ' ', '\t' };
            string[] pa = null;
            string li = "", ca = "";

            ll = (long)(Fei + tmini * 10000000.0);
            DateTime fech = new DateTime(ll);
            fe = int.Parse(string.Format("{0:yyyy}{0:MM}{0:dd}", fech));

            for (i = 0; i < nutra; i++)
            {
                ca = ".\\pro\\fcat" + fr1.tar[i] + ".txt";                
                if (!File.Exists(ca))
                {                 
                    siFac[i] = false;
                }
                else
                {
                    li = "";
                    j = -1;
                    StreamReader ar = new StreamReader(ca);
                    while (li != null)
                    {
                        try
                        {
                            li = ar.ReadLine();
                            if (li == null) break;
                            if (string.Compare(est[i].Substring(0,4),li.Substring(0,4))==0)
                            {
                                pa = li.Split(delim);
                                fe1 = int.Parse(pa[10]);
                                fe2 = int.Parse(pa[11]);
                                ii = int.Parse(pa[1]);
                                ff = double.Parse(pa[2]);
                                laa[i] = (double)(ii) + ff / 60.0;
                                ii = int.Parse(pa[3]);
                                ff = double.Parse(pa[4]);
                                loo[i] = (double)(ii) + ff / 60.0;
                                if (fe >= fe1 && (fe <= fe2 || fe2 == 0))
                                {
                                    sen[i] = double.Parse(pa[5]);                                   
                                    if (sen[i] > -1.0)
                                    {
                                        siFac[i] = true;
                                        clR[i] = byte.Parse(pa[7]);
                                        clG[i] = byte.Parse(pa[8]);
                                        clB[i] = byte.Parse(pa[9]);
                                        j = 1;
                                    }
                                    else siFac[i] = false;
                                    break;
                                }
                            }
                        }
                        catch
                        {
                        }
                    }
                    ar.Close();
                    if (j == -1)
                    {
                        laa[i] = -500.0;
                        loo[i] = -500.0;
                        sen[i] = -1.0;
                        siFac[i] = false;
                    }
                }
            }
            for (i = 0; i < nutra; i++)
            {
                if (siFac[i] == false)
                {
                    clR[i] = 150;
                    clG[i] = 150;
                    clB[i] = 150;
                    sen[i] = -1.0;
                }
            }

            return;
        }

        void Dibujo()
        {
            int xf, yf, i, j, jj, k, kk, dif = 0, tamlet, numtotra;
            int nmi, nmf, numu, proo;
            float x1, y1, iniy, ff;
            double fax, fay, fy, diff;
            string esta = "";
            Color col;
            Point[] dat;


            util.borra(panel1,colfondo);
            //MessageBox.Show("rutbas=" + fr1.rutbas);
            xf = panel1.Size.Width - 40;
            ff = (float)(xf);
            yf = panel1.Size.Height;
            numtotra = 0;
            numu = 0;
            for (i = 0; i < nutra; i++) if (siEst[i] == true) numtotra += 1;
            if (numtotra == 0) return;
            fax = xf / dura;
            fay = yf/(double)(numtotra);
            tamlet = (int)(fay);
            if (tamlet > 10) tamlet = 10;

            Graphics dc = panel1.CreateGraphics();
            Pen lapiz = new Pen(colinea, 1);
            Pen lapiz2 = new Pen(Color.DarkOrange, 1);
            lapiz2.DashStyle = DashStyle.DashDot;            

            jj = 0;
            try
            {
                for (j = 0; j < nutra; j++)
                {
                    if (siEst[j] == true)
                    {
                        proo = (int)((mx[j] + mn[j]) / 2.0F);
                        if (mx[j] - proo != 0) fy = ((fay / 2) / ((mx[j] - proo)));
                        else fy = 1.0;
                        iniy = (float)(jj * fay + fay / 2);
                        dc.DrawLine(lapiz2, 40.0F, iniy, ff, iniy);
                        if (!char.IsLetterOrDigit(est[j][4])) esta = est[j].Substring(0, 4);
                        else esta = est[j];
                        if (sen[j] > 0) col = colotr1;
                        else col = Color.Red;                       
                        SolidBrush brocha = new SolidBrush(col);
                        dc.DrawString(esta, new Font("Times New Roman", tamlet, FontStyle.Bold), brocha, 1, (float)(iniy - tamlet));
                        brocha.Dispose();
                        if (fr1.invertido[j] == true)
                        {
                            Pen lapp = new Pen(Color.Black, 1);
                            SolidBrush broo = new SolidBrush(Color.Yellow);
                            dc.FillEllipse(broo, -1, (float)(iniy - tamlet), 5, 5);
                            dc.DrawEllipse(lapp, -1, (float)(iniy - tamlet), 5, 5);
                            lapp.Dispose();
                            broo.Dispose();
                        }
                        kk = 0;
                        nmi = (int)((tmini - tim[j][0]) * ra[j]);
                        if (nmi < 0) nmi = 0;
                        nmf = (int)(((tmini + dura) - tim[j][0]) * ra[j]);
                        if (nmf > lar[j]) nmf = lar[j];
                        numu = nmf - nmi;
                        dat = new Point[numu];
                        if (fr1.invertido[j] == false)
                        {
                            for (k = nmi; k < nmf; k++)
                            {
                                if (kk >= lar[j]) break;
                                //dif = pro[j] - (int)(cu[j][k]);
                                dif = (int)(cu[j][k]) - pro[j];
                                diff = dif * fy;
                                y1 = (float)(iniy - amp * diff);
                                x1 = (float)(40.0 + (tim[j][k] - tmini) * fax);
                                dat[kk].Y = (int)y1;
                                dat[kk].X = (int)x1;
                                kk += 1;
                            }
                        }
                        else
                        {
                            for (k = nmi; k < nmf; k++)
                            {
                                if (kk >= lar[j]) break;
                                dif = pro[j] - (int)(cu[j][k]);
                                //dif = (int)(cu[j][k]) - pro[j];
                                diff = dif * fy;
                                y1 = (float)(iniy - amp * diff);
                                x1 = (float)(40.0 + (tim[j][k] - tmini) * fax);
                                dat[kk].Y = (int)y1;
                                dat[kk].X = (int)x1;
                                kk += 1;
                            }
                        }
                        dc.DrawLines(lapiz, dat);
                        jj += 1;
                    }
                }

            }
            catch
            {
            }
            lapiz.Dispose();
            lapiz2.Dispose();

            return;
        }


        void DibujoRefe()
        {
            int xf, yf, jj, k, kk, pro, dif = 0, tamlet;
            int nmi, nmf;
            float x1, y1;
            double fax, fay, fy, diff, iniy;
            string esta = "";
            Point[] dat;

            util.borra(panel2, Color.DimGray);
            xf = panel2.Size.Width - 40;
            yf = panel2.Size.Height;
            fax = xf / durx;
            fay = yf;
            tamlet = 10;

            Graphics dc = panel2.CreateGraphics();
            Pen lapiz = new Pen(Color.Orange, 1);
            SolidBrush brocha = new SolidBrush(Color.SpringGreen);

            jj = 0;
            try
            {
                pro = (int)((mx[refe] + mn[refe]) / 2.0F);
                if (mx[refe] - pro != 0) fy = ((fay / 2) / ((mx[refe] - pro)));
                else fy = 1.0;
                iniy = fay / 2;
                if (!char.IsLetterOrDigit(est[refe][4])) esta = est[refe].Substring(0, 4);
                else esta = est[refe];
                dc.DrawString(esta, new Font("Times New Roman", tamlet, FontStyle.Bold), brocha, 1, (float)(iniy - tamlet));
                if (fr1.invertido[refe] == true)
                {
                    Pen lapp = new Pen(Color.Black, 1);
                    SolidBrush broo = new SolidBrush(Color.Yellow);
                    dc.FillEllipse(broo, -1, (float)(iniy - tamlet), 5, 5);
                    dc.DrawEllipse(lapp, -1, (float)(iniy - tamlet), 5, 5);
                    lapp.Dispose();
                    broo.Dispose();
                }
                dat = new Point[lar[refe]];
                kk = 0;
                nmi = 0;
                nmf = lar[refe];
                if (fr1.invertido[refe] == false)
                {
                    for (k = nmi; k < nmf; k++)
                    {
                        if (kk >= lar[refe]) break;
                        //dif = pro - (int)(cu[refe][k]);
                        dif = (int)(cu[refe][k]) - pro;
                        diff = dif * fy;
                        y1 = (float)(iniy - diff);
                        x1 = (float)(40.0 + (tim[refe][k] - tminx) * fax);
                        dat[kk].Y = (int)y1;
                        dat[kk].X = (int)x1;
                        kk += 1;
                    }
                }
                else
                {
                    for (k = nmi; k < nmf; k++)
                    {
                        if (kk >= lar[refe]) break;
                        //dif = pro - (int)(cu[refe][k]);
                        dif = (int)(cu[refe][k]) - pro;
                        diff = dif * fy;
                        y1 = (float)(iniy - diff);
                        x1 = (float)(40.0 + (tim[refe][k] - tminx) * fax);
                        dat[kk].Y = (int)y1;
                        dat[kk].X = (int)x1;
                        kk += 1;
                    }
                }
                dc.DrawLines(lapiz, dat);
                jj += 1;
            }
            catch
            {
            }
            lapiz.Dispose();
            brocha.Dispose();

            return;
        }
        void BarraGuia()
        {
            int xf, yf;
            float x1, x2, ww;
            double fax;

            xf = panel2.Size.Width - 40;
            yf = panel2.Size.Height;
            fax = xf / durx;
            x1 = (float)(40.0 + (tmini - tminx) * fax);
            x2 = (float)(40.0 + ((tmini + dura) - tminx) * fax);
            ww = x2 - x1;
            //    MessageBox.Show("x1="+x1.ToString()+" ww="+ww.ToString());
            Graphics dc = panel2.CreateGraphics();
            SolidBrush brocha = new SolidBrush(Color.LightBlue);
            dc.FillRectangle(brocha, x1, 0.0F, ww, 3.0F);
            brocha.Dispose();

            return;
        }


        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            if (load == false) return;
            Dibujo();
            DibujoRefe();
            VerLecturas();
            if (tmini != tminx) BarraGuia();
            return;
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            int i = -1, ii, j, jj, nues;
            double fay;

            if (guia == true) return;
            if (e.X >= 40)
            {
                bx1 = e.X;
                by1 = e.Y;
            }
            else
            {
                nues = 0;
                for (j = 0; j < nutra; j++) if (siEst[j] == true) nues += 1;
                fay = panel1.Size.Height/(double)(nues);
                ii = (int)(e.Y / fay);
                jj = 0;
                for (j = 0; j < nutra; j++)
                {
                    if (siEst[j] == true)
                    {
                        if (jj == ii)
                        {
                            i = j;
                            break;
                        }
                        jj += 1;
                    }
                }
                //MessageBox.Show("ii=" + ii.ToString()+" i="+i.ToString()+" "+est[i]);
                if (e.Button == MouseButtons.Right)
                {
                    for (j = 0; j < nutra; j++)
                    {
                        if (j == i) siEst[j] = true;
                        else siEst[j] = false;
                        panel1.Invalidate();
                    }
                    panelesta.Visible = true;
                    i = util.EscribePanelEsta(panelesta, nutra, est, siEst);
                }
                else
                {
                    if (i > -1)
                    {
                        refe = (short)(i);
                        DibujoRefe();
                        if (tmini != tminx) BarraGuia();
                    }
                }
            }

            return;
        }
        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            ushort nues, ii, jj, j, i;
            int xf, yf, bx2, by2, nm1, nm2, tot, min, max, k, id;
            long tii1 = 0;
            double dif = 0, ti1, ti2, fax, fay, ttt;
            string ss = "";

            if (e.X < 40) return;
            xf = panel1.Size.Width - 40;
            yf = panel1.Size.Height;
            if (guia == true)
            {
                Graphics dc = panel1.CreateGraphics();
                Pen lapiz = new Pen(Color.Red, 1);
                dc.DrawLine(lapiz, e.X, 1, e.X, yf);
                lapiz.Dispose();
                guia = false;
                boguia.BackColor = Color.White;
                return;
            }
            bx2 = e.X;
            by2 = e.Y;
            fax = dura / xf;
            ti1 = tmini + ((bx1 - 40) * fax);
            ti2 = tmini + ((bx2 - 40) * fax);
            if (ti1 > ti2)
            {
                ttt = ti1;
                ti1 = ti2;
                ti2 = ttt;
            }

            nues = 0;
            for (j = 0; j < nutra; j++) if (siEst[j] == true) nues += 1;
            fay = panel1.Size.Height/(double)(nues);
            ttt = e.Y / fay;
            ii = (ushort)(ttt);
            jj = 0;
            i = 0;
            id = -1;
            for (j = 0; j < nutra; j++)
            {
                if (siEst[j] == true)
                {
                    if (jj == ii)
                    {
                        id = j;
                        break;
                    }
                    jj += 1;
                }
            }
            if (id == -1) return;

            if (quitar == true)
            {
                lectu[id] = -1;
                quitar = false;
                boQuita.BackColor = Color.White;
                panel1.Invalidate();
                if (panelfocam.Visible == true) panelfocam.Invalidate();
                return;
            }

            if (sipro == 1)
            {
                sipro = 2;
                boprom.BackColor = Color.SpringGreen;
                for (j = 0; j < nutra; j++)
                {
                    tot = tim[j].Length;
                    for (k = 0; k < tot; k++)
                    {
                        if (tim[j][k] >= ti1) break;
                    }
                    nm1 = k;
                    for (k = 0; k < tot; k++)
                    {
                        if (tim[j][k] > ti2 || tim[j][k] <= 0) break;
                    }
                    nm2 = k - 1;
                    max = cu[j][nm1];
                    min = max;
                    for (k = nm1 + 1; k < nm2; k++)
                    {
                        if (max < cu[j][k]) max = cu[j][k];
                        else if (min > cu[j][k]) min = cu[j][k];
                    }
                    pro[j] = (int)((max + min) / 2.0F);
                }
                panel1.Invalidate();
                return;
            }

            tii1 = (long)(Fei + (ti1 * 10000000.0));

            if (e.Button == MouseButtons.Right)
            {

                if (bx1 == bx2)
                {
                    DateTime fech1 = new DateTime(tii1);
                    ss = string.Format("{0:HH}:{0:mm}:{0:ss}.{0:ff}", fech1);
                }
                else
                {
                    dif = Math.Abs(ti2 - ti1);
                    ss = string.Format("{0:0.00}", dif) + " seg";
                }
                tip.IsBalloon = false;
                tip.ReshowDelay = 0;
                tip.Show(ss, panel1, e.X, e.Y - 15, 3000);
            }
            else
            {
                if ((bx1 == bx2 || ti2 - ti1 < 1.0) && lec == 0)
                {
                    /* nues = 0;
                     for (j = 0; j < nutra; j++) if (siEst[j] == true) nues += 1;
                     fay = panel1.Size.Height /(double)(nues);
                     ttt = e.Y / fay;
                     ii = (ushort)(ttt);
                     //ii = (ushort)(e.Y / fay);
                     jj = 0;
                     i = 0;
                     for (j = 0; j < nutra; j++)
                     {
                         if (siEst[j] == true)
                         {
                             if (jj == ii)
                             {
                                 i = j;
                                 break;
                             }
                             jj += 1;
                         }
                     }
                     MessageBox.Show("i=" + i.ToString());*/
                }
                else
                {
                    if (lec == 0)
                    {
                        tmini = ti1;
                        dura = ti2 - ti1;
                    }
                    else if (lec == 1)
                    {
                        Lecturas(ti1, ti2, -1);
                    }
                    else if (lec == 2)
                    {
                        Lecturas(ti1, ti2, id);
                    }
                    if (panelfocam.Visible == true) panelfocam.Invalidate();
                }
                panel1.Invalidate();
            }

            if (dura != durx)
            {
                boizq.Visible = true;
                bodere.Visible = true;
                boamplia.Visible = true;
                boreduce.Visible = true;
                botodo.Visible = true;
            }
            else
            {
                boizq.Visible = false;
                bodere.Visible = false;
                boamplia.Visible = false;
                boreduce.Visible = false;
                botodo.Visible = false;
            }

            return;
        }

        void Lecturas(double t1, double t2, int nu)
        {
            int j, k, mx, mn, mi, mf;

            if (nu < 0)
            {
                for (j = 0; j < nutra; j++)
                {
                    if (siEst[j] == true)
                    {
                        tle1[j] = t1;
                        tle2[j] = t2;
                        mi = (int)((t1 - tim[j][0]) * ra[j]);
                        mf = (int)((t2 - tim[j][0]) * ra[j]);
                        mx = cu[j][mi];
                        mn = mx;
                        for (k = mi + 1; k < mf; k++)
                        {
                            if (mx < cu[j][k]) mx = cu[j][k];
                            else if (mn > cu[j][k]) mn = cu[j][k];
                        }
                        lectu[j] = mx - mn;
                    }
                    else
                    {
                        tle1[j] = 0;
                        tle2[j] = 0;
                    }
                }
            }
            else
            {
                tle1[nu] = t1;
                tle2[nu] = t2;
                mi = (int)((t1 - tim[nu][0]) * ra[nu]);
                mf = (int)((t2 - tim[nu][0]) * ra[nu]);
                mx = cu[nu][mi];
                mn = mx;
                for (k = mi + 1; k < mf; k++)
                {
                    if (mx < cu[nu][k]) mx = cu[nu][k];
                    else if (mn > cu[nu][k]) mn = cu[nu][k];
                }
                lectu[nu] = mx - mn;
            }

            Conteo();

            return;
        }

        void Conteo()
        {
            int i, cont;

            cont = 0;
            for (i = 0; i < nutra; i++)
            {
                if (lectu[i] > 0) cont += 1;
                if (cont > 3)
                {
                    bofoc.Visible = true;
                    break;
                }
            }
            return;
        }

        void VerLecturas()
        {
            int xf, yf, i, j, jj, k, kk, dif = 0, numtotra;
            int nmi, nmf, numu, proo;
            float x1, y1, iniy, ff;
            double fax, fay, fy, diff;
            Point[] dat;

            xf = panel1.Size.Width - 40;
            ff = (float)(xf);
            yf = panel1.Size.Height;
            numtotra = 0;
            numu = 0;
            for (i = 0; i < nutra; i++) if (siEst[i] == true) numtotra += 1;
            if (numtotra == 0) return;
            fax = xf / dura;
            fay = yf/(double)(numtotra);

            Graphics dc = panel1.CreateGraphics();
            Pen lapiz = new Pen(Color.DarkOrange, 1);

            jj = 0;
            try
            {
                for (j = 0; j < nutra; j++)
                {
                    if (siEst[j] == true)
                    {
                        if (lectu[j] > 0)
                        {
                            proo = (int)((mx[j] + mn[j]) / 2.0F);
                            if (mx[j] - proo != 0) fy = ((fay / 2) / ((mx[j] - proo)));
                            else fy = 1.0;
                            iniy = (float)(jj * fay + fay / 2);
                            kk = 0;
                            nmi = (int)((tle1[j] - tim[j][0]) * ra[j]);
                            if (nmi < 0) nmi = 0;
                            nmf = (int)((tle2[j] - tim[j][0]) * ra[j]);
                            if (nmf > lar[j]) nmf = lar[j];
                            numu = nmf - nmi;
                            dat = new Point[numu];
                            for (k = nmi; k < nmf; k++)
                            {
                                if (kk >= lar[j]) break;
                                dif = (int)(cu[j][k])-pro[j];
                                diff = dif * fy;
                                y1 = (float)(iniy - amp * diff);
                                x1 = (float)(40.0 + (tim[j][k] - tmini) * fax);
                                dat[kk].Y = (int)y1;
                                dat[kk].X = (int)x1;
                                kk += 1;
                            }
                            dc.DrawLines(lapiz, dat);
                        }
                        jj += 1;
                    }
                }

            }
            catch
            {
            }
            lapiz.Dispose();
           // MessageBox.Show("verlecturas fin");

            return;
        }

        private void botodas_Click(object sender, EventArgs e)
        {
            int i;

            for (i = 0; i < nutra; i++) siEst[i] = true;
            panel1.Invalidate();
            botodas.BackColor = Color.White;
            if (panelesta.Visible == true) util.EscribePanelEsta(panelesta, nutra, est, siEst);

            return;
        }

        private void boesta_Click(object sender, EventArgs e)
        {
            if (panelesta.Visible == false)
            {
                panelesta.Visible = true;
                panelesta.BringToFront();
                // MessageBox.Show("ii1");
                util.EscribePanelEsta(panelesta, nutra, est, siEst);
            }
            else panelesta.Visible = false;
        }

        private void boguia_Click(object sender, EventArgs e)
        {
            if (guia == false)
            {
                guia = true;
                boguia.BackColor = Color.BlueViolet;
            }
            else
            {
                guia = false;
                boguia.BackColor = Color.White;
            }
            return;
        }

        private void boprom_Click(object sender, EventArgs e)
        {
            int i;
            if (sipro == 0)
            {
                sipro = 1;
                boprom.BackColor = Color.IndianRed;
            }
            else
            {
                sipro = 0;
                boprom.BackColor = Color.White;
                for (i = 0; i < nutra; i++) pro[i] = (int)((mx[i] + mn[i]) / 2.0F);
                panel1.Invalidate();
            }
        }

        private void boWord_Click(object sender, EventArgs e)
        {

        }

        private void boGraBas_Click(object sender, EventArgs e)
        {

            GrabaBase();
            boGraBas.Visible = false;
            boWord.Visible = false;
            tmini = tminx;
            dura = durx;

            panel1.Invalidate();
        }

        private void boizq_Click(object sender, EventArgs e)
        {
            tmini += dura / 3;
            if (tmini < tminx) tmini = tminx;
            panel1.Invalidate();
        }

        private void bodere_Click(object sender, EventArgs e)
        {
            tmini -= dura / 3;
            panel1.Invalidate();
        }

        private void boamplia_Click(object sender, EventArgs e)
        {
            dura -= dura / 3;
            panel1.Invalidate();
        }

        private void boreduce_Click(object sender, EventArgs e)
        {
            dura += dura / 3;
            panel1.Invalidate();
        }
        private void botodo_Click(object sender, EventArgs e)
        {
            tmini = tminx;
            dura = durx;
            boizq.Visible = false;
            bodere.Visible = false;
            boamplia.Visible = false;
            boreduce.Visible = false;
            botodo.Visible = false;
            panel1.Invalidate();
            return;
        }

        private void panelesta_MouseDown(object sender, MouseEventArgs e)
        {
            int i, j, k, val;

            if (panelesta.Visible == false) return;

            i = e.Y / 10;
            j = e.X / 35;
            val = i + j * 50;
            if (e.Button == MouseButtons.Right)
            {
                for (k = 0; k < nutra; k++) siEst[k] = false;
                siEst[val] = true;
            }
            else
            {

                if (val >= nutra) return;
                if (siEst[val] == true) siEst[val] = false;
                else siEst[val] = true;
            }
            panel1.Invalidate();
            i = util.EscribePanelEsta(panelesta, nutra, est, siEst);
            if (i == 1) botodas.BackColor = Color.PaleVioletRed;
            else botodas.BackColor = Color.White;
            // MessageBox.Show("panelfoc=" + panelfoc.Visible.ToString());
            if (panelfoc.Visible == true) VerAtenuacion(col);

            return;
        }

        private void Form3_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (grabas == true)
            {
                GrabaBase();
            }
            fr1.desactivado = false;
        }

        private void bobase_Click(object sender, EventArgs e)
        {
            if (grabas == true)
            {
                grabas = false;
                bobase.BackColor = Color.White;
                boGraBas.Visible = false;
            }
            else
            {
                grabas = true;
                bobase.BackColor = Color.YellowGreen;
            }
        }

        void GrabaBase()
        {
            int i, j, nu, lagr, logr, iniX, iniY, xf, yf, dif, k;
            long ll;
            double timn=0,timx=0,dismx,ampmx,difla,diflo,am,di,lam,lom,facm,laf,lof;
            double[] amp = new double[Ma];
            double[] dis = new double[Ma];
            char cc = ' ';
            string ca="",fe="",fe2="",nom="",pa="";


            if (grabas == false || grafoc == false) return;
            j = -1;
            for (i = 0; i < fr1.nuvol; i++)
            {
                if (fr1.volcan[i][0] == fr1.sismo[9])
                {
                    j = i;
                    break;
                }
            }
            if (j == -1) return;
            xf = panelfoc.Size.Width;
            yf = panelfoc.Size.Height;
            iniX = xf / 2;
            iniY = yf / 2;
            facm = 0.3 * (140.0 / diff);
            laf = fr1.latvol[j] + (iniY - yam) / facm;
            lof = fr1.lonvol[j] + (xam-iniX) / facm;

            nu = 0;
            dismx = 0;
            ampmx = 0;
            for (i = 0; i < nutra; i++)
            {
                if (siFac[i] == true && siEst[i] == true && lectu[i] >= 0)
                {
                    if (nu == 0)
                    {
                        timn = tle1[i];
                        timx = timn;
                        cc = fr1.tar[i];
                    }
                    difla = laf - laa[i];
                    diflo = lof - loo[i];
                    dis[i] = 110.4 * Math.Sqrt(difla * difla + diflo * diflo);
                    if (dis[i] > dismx) dismx = dis[i];
                    amp[i] = (lectu[i] / ga[i]) * sen[i];
                    if (amp[i] > ampmx) ampmx = amp[i];
                    nu += 1;
                }
            }
            if (nu < 4) return;
            for (i = 0; i < nutra; i++)
            {
                if (siFac[i] == true && siEst[i] == true && lectu[i] >= 0)
                {
                    if (timx < tle2[i] && tle2[i]>0) timx = tle2[i];
                    if (timn > tle1[i] && tle1[i]>0) timn = tle1[i];
                }
            }

            ll = (long)(Fei + tminx * 10000000.0);
            DateTime fech = new DateTime(ll);
            fe = string.Format("{0:yy}{0:MM}{0:dd}{0:HH}{0:mm}{0:ss}.{0:ff}", fech);
            nom = fr1.rutbas + "\\ate\\" + fe.Substring(0, 4) + ".atn";

            StreamWriter at = File.AppendText(nom);

            ca = fr1.sismo.Substring(0,12) + " ";
            pa = string.Format("{0,6:0.0000}",laf);
            dif = 8 - pa.Length;
            for (k = 0; k < dif; k++) ca += " ";
            ca += pa+" ";
            pa = string.Format("{0,6:0.0000}", lof);
            dif = 9 - pa.Length;
            for (k = 0; k < dif; k++) ca += " ";
            ca += pa + "   3.00 ";            
           // ca += string.Format("{0:00.000} {1:00.000} {2:00} {3,13:0.00} {4,13:0.00} {5,13:0.00}", laf, lof, nu, tminx, timn, timx) + " ";
            ca += string.Format("{0:000} {1,13:0.00} {2,13:0.00} {3,13:0.00}", nu, tminx, timn, timx) + " ";
            i = usu.Length;
            if (i < 3) usu = "___";
            ca += usu.Substring(0, 3) + " " + cc;
            at.WriteLine(ca);
            ca = "";
            for (i = 0; i < nutra; i++)
            {
                if (siFac[i] == true && siEst[i] == true && lectu[i] >= 0)
                {
                    am = amp[i] / ampmx;
                    di = dis[i];
                    ca = est[i].Substring(0, 4) + " ";
                    ca += string.Format("{0,5:0.00} {1,5:0.00} {2,15:0.000} {3,14:0.000}", am, di, tle1[i], tle2[i]);
                    at.WriteLine(ca);
                }
            }
            at.Close();

            nom = fr1.rutbas + "\\ate\\" + fe.Substring(0, 4) + "ate.txt";
            ca = "";
            ll = (long)(Fei + timn * 10000000.0);
            DateTime fech2 = new DateTime(ll);
            fe2 = string.Format("{0:yy}{0:MM}{0:dd} {0:HH}{0:mm} {0:ss}.00", fech2);
            lagr = (int)(laf);
            logr = (int)(lof);
            lam = (laf - (double)(lagr)) * 60.0;
            lom = (lof - (double)(logr)) * 60.0;

            StreamWriter at2 = File.AppendText(nom);

            ca = fe2;

            pa = lagr.ToString();
            dif = 3 - pa.Length;
            for (k = 0; k < dif; k++) ca += " ";
            ca += pa;
            pa = string.Format("{0:00.00}", lam);
            dif = 6 - pa.Length;
            for (k = 0; k < dif; k++) ca += " ";
            ca += pa;
            pa = logr.ToString();
            dif = 4 - pa.Length;
            for (k = 0; k < dif; k++) ca += " ";
            ca += pa;
            pa = string.Format("{0:00.00}", lom);
            dif = 6 - pa.Length;
            for (k = 0; k < dif; k++) ca += " ";
            ca += pa;
            //ca = fe2 + " " + string.Format("{0:d2} {1,5:0.00}  {2:d2} {3,5:0.00}", lagr, lam, logr, lom);
            ca += "   3.00       " + string.Format("{0:d3}", nu);
            ca += "                         I9" + string.Format("{0:d3}", nu) + " " + fr1.sismo.Substring(0, 12);
            at2.WriteLine(ca);

            at2.Close();

            grafoc = false;
            for (i = 0; i < nutra; i++)
            {
                lectu[i] = -1;
                tle1[i] = 0;
                tle2[i] = 0;
            }
            panelfoc.Visible = false;
            panelfocam.Visible = false;
            lec = 0;
            boLecTo.BackColor = Color.White;
            boLecUn.BackColor = Color.White;

            return;
        }


        void Reviza()
        {
            int cont = 0, xf;
            long ll;
            float x1, w, h;
            double tmn, tmx, fax, dd1, dd2;
            string nom = "", fe = "", lin = "";
            bool si = false;


            tmn = tminx;
            tmx = tmn + durx;
            xf = panel1.Size.Width - 40;
            h = panel1.Size.Height - 2;
            fax = xf / dura;

            if (!File.Exists("reviatn.txt"))
            {
                ll = (long)(Fei + tminx * 10000000.0);
                DateTime fech = new DateTime(ll);
                fe = string.Format("{0:yy}{0:MM}{0:dd}{0:HH}{0:mm}{0:ss}.{0:ff}", fech);
                nom = fr1.rutbas + "\\ate\\" + fe.Substring(0, 4) + ".atn";

                StreamWriter pr = File.CreateText("reviatn.txt");
                StreamReader ar = new StreamReader(nom);

                while (lin != null)
                {
                    try
                    {
                        lin = ar.ReadLine();
                        if (lin == null) break;
                        if (char.IsDigit(lin[0]))
                        {
                            si = false;
                            dd1 = double.Parse(lin.Substring(44, 13));
                            dd2 = double.Parse(lin.Substring(58, 13));
                            if (dd1 >= tmn && dd2 <= tmx)
                            {
                                si = true;
                                pr.WriteLine(lin);
                                cont += 1;
                            }
                        }
                        else if (si == true)
                        {
                            pr.WriteLine(lin);
                            cont += 1;
                        }
                    }
                    catch
                    {
                    }
                }

                ar.Close();
                pr.Close();
            }// if !file.Exist....
            else cont = 10;
            if (cont == 0 && File.Exists("reviatn.txt"))
            {
                File.Delete("reviatn.txt");
                return;
            }

            Graphics dc = panel1.CreateGraphics();
            Pen lapiz = new Pen(Color.BlueViolet, 2);
            lin = "";
            StreamReader ar2 = new StreamReader("reviatn.txt");
            while (lin != null)
            {
                try
                {
                    lin = ar2.ReadLine();
                    if (lin == null) break;
                    if (char.IsDigit(lin[0]))
                    {
                        dd1 = double.Parse(lin.Substring(44, 13));
                        dd2 = double.Parse(lin.Substring(58, 13));
                        if (dd1 >= tmn && dd2 <= tmx)
                        {
                            x1 = (float)(40.0 + (dd1 - tmini) * fax);
                            w = (float)(40.0 + (dd2 - tmini) * fax) - x1;
                            dc.DrawRectangle(lapiz, x1, 1, w, h);
                        }
                    }
                }
                catch
                {
                }
            }
            ar2.Close();
            lapiz.Dispose();

            return;
        }

        private void boaum_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) amp += 0.1F * amp;
            else amp += 0.5F * amp;
            BotonesAmp();
            panel1.Invalidate();
        }

        private void bodism_MouseDown(object sender, MouseEventArgs e)
        {

            if (e.Button == MouseButtons.Left) amp -= 0.1F * amp;
            else amp -= 0.5F * amp;
            BotonesAmp();
            panel1.Invalidate();
        }

        private void boini_Click(object sender, EventArgs e)
        {
            amp = 1.0F;
            BotonesAmp();
            panel1.Invalidate();
        }

        void BotonesAmp()
        {

            if (amp == 1.0F)
            {
                boaum.BackColor = Color.White;
                bodism.BackColor = Color.White;
                boini.BackColor = Color.White;
            }
            else if (amp > 1.0F)
            {
                boaum.BackColor = Color.Tomato;
                bodism.BackColor = Color.White;
                boini.BackColor = Color.Purple;
            }
            else
            {
                boaum.BackColor = Color.White;
                bodism.BackColor = Color.Tomato;
                boini.BackColor = Color.Purple;
            }
            return;
        }

        private void boLecTo_Click(object sender, EventArgs e)
        {
            if (lec != 1)
            {
                lec = 1;
                boLecTo.BackColor = Color.DarkOrange;
                boLecUn.BackColor = Color.White;
            }
            else
            {
                lec = 0;
                boLecTo.BackColor = Color.White;
                boLecUn.BackColor = Color.White;
            }
            return;
        }

        private void boLecUn_Click(object sender, EventArgs e)
        {
            if (lec != 2)
            {
                lec = 2;
                boLecUn.BackColor = Color.SaddleBrown;
                boLecTo.BackColor = Color.White;
            }
            else
            {
                lec = 0;
                boLecUn.BackColor = Color.White;
                boLecTo.BackColor = Color.White;
            }
            return;
        }

        private void bofoc_Click(object sender, EventArgs e)
        {
            int i, j;

            if (grapasto == true)
            {
                util.GraMatlab(lat, lot, laa, loo, siFac, sen, nutra, siEst, lectu, ga, tle1, tle2, tminx, usu,
                     fr1.sismo, fr1.tar[0], est);
                return;
            }
            if (panelfoc.Visible == false)
            {
                j = -1;
                for (i = 0; i < fr1.nuvol; i++)
                {
                    if (fr1.volcan[i][0] == fr1.sismo[9])
                    {
                        j = i;
                        break;
                    }
                }
                if (j == -1) return;
                panelfoc.Visible = true;
                panelfocam.Visible = true;
                bofoc.BackColor = Color.PaleGoldenrod;
                col = Color.Red;
                VerAtenuacion(col);
            }
            else
            {
                panelfoc.Visible = false;
                panelfocam.Visible = false;
                bofoc.BackColor = Color.White;
            }

            return;
        }

        private void panelfoc_MouseDown(object sender, MouseEventArgs e)
        {

            boGraBas.Visible = false;
            boWord.Visible = false;
            grafoc = false;
            if (e.Button == MouseButtons.Left)
            {
                xam = e.X;
                yam = e.Y;
                col = Color.Red;
                VerAtenuacion(col);

            }
            else
            {
            }

            return;
        }

        private void panelfoc_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            boGraBas.Visible = true;
            boWord.Visible = true;
            col = Color.DarkOrange;
            VerAtenuacion(col);
            grafoc = true;
        }

        void VerAtenuacion(Color col)
        {
            int j, i;

            j = -1;
            for (i = 0; i < fr1.nuvol; i++)
            {
                if (fr1.volcan[i][0] == fr1.sismo[9])
                {
                    j = i;
                    break;
                }
            }
            if (j == -1) return;
            util.borra(panelfoc, Color.Lavender);
            util.borra(panelfocam, Color.LightSteelBlue);
            util.VerMapa(panelfoc, fr1.sismo[9], fr1.latvol[j], fr1.lonvol[j], "", diff, lat, lot, Color.Gray);
            util.Dibampl(panelfoc, panelfocam, lat, lot, fr1.latvol[j], fr1.lonvol[j], diff, nutra, siFac,
                 sen, laa, loo, clR, clG, clB, lectu, ga, xam, yam, siEst, col);

            return;
        }

        private void boaumfoc_Click(object sender, EventArgs e)
        {
            if (panelfoc.Visible == false) return;
            diff -= 0.3 * diff;
            VerAtenuacion(col);
        }

        private void bodisfoc_Click(object sender, EventArgs e)
        {
            if (panelfoc.Visible == false) return;
            diff += 0.3 * diff;
            VerAtenuacion(col);
        }

        private void panel2_MouseDown(object sender, MouseEventArgs e)
        {
            int xf, yf;
            double fax;

            if (e.X < 40 || dura == durx) return;
            xf = panel2.Size.Width - 40;
            yf = panel2.Size.Height;
            fax = xf / durx;

            tmini = tminx + ((e.X - 40) / fax);
            panel1.Invalidate();
        }

        private void boInv_Click(object sender, EventArgs e)
        {
            if (colfondo == Color.Black)
            {
                colinea = Color.Black;
                colfondo = Color.White;
                colotr1 = Color.Blue;
                colP = Color.Green;
                colS = Color.DeepSkyBlue;
                colC = Color.Red;
            }
            else
            {
                colfondo = Color.Black;
                colinea = Color.White;
                colotr1 = Color.Yellow;
                colP = Color.LightGreen;
                colS = Color.LightBlue;
                colC = Color.Orange;
            }
            panel1.Invalidate();
        }

        private void boMasVol_Click(object sender, EventArgs e)
        {
            if (panel3.Visible == false) panel3.Visible = true;
            else panel3.Visible = false;
        }

        private void boQuita_Click(object sender, EventArgs e)
        {
            if (quitar == false)
            {
                quitar = true;
                boQuita.BackColor = Color.GreenYellow;
            }
            else
            {
                quitar = false;
                boQuita.BackColor = Color.White;
            }
        }

        private void borevi_Click(object sender, EventArgs e)
        {
            Reviza();
            return;
        }

        private void panelfocam_Paint(object sender, PaintEventArgs e)
        {
            VerAtenuacion(col);
        }
    



    }
}
