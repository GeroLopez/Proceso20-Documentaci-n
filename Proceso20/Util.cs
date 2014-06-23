using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace Proceso20
{
    class Util
    {
        const int Ma = 300;
        const double Fei = 621355968000000000.0;
        /// <summary>
        /// Permite convertir del tiempo visual c# al tiempo en SUDS.
        /// </summary>
        const double Feisuds = 621355968000000000.0;  // permite convertir del tiempo visual c# al tiempo en SUDS
        /// <summary>
        /// Crea un proceso cmd para ejecutar líneas de comandos. 
        /// </summary>
        /// <param name="strCmdLine">Linea que se ejecuta en la consola de octave</param>
        /// <param name="cond">Valor que se utiliza para decidir si se despliega la ventana del proceso creado.
        /// True si no quiere mostrar la ventana, false si quiere desplegar la ventana.</param>
        public void Dos(string strCmdLine, bool cond)
        {
            Process p = new Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.Arguments = strCmdLine;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = false;
            p.StartInfo.CreateNoWindow = cond; // true si no se quiere mostrar ventana
            p.Start();
            p.WaitForExit();
            p.Close();
            return;
        }
        /// <summary>
        /// Pinta el interior del panel del color que se pasa como parámetro.
        /// </summary>
        /// <param name="panel">Panel a pintar.</param>
        /// <param name="col">Color con el que se desea rellenar el panel.</param>
        public void borra(Panel panel, Color col)
        {
            int xf, yf;

            xf = panel.Size.Width;
            yf = panel.Size.Height;
            Graphics dc = panel.CreateGraphics();
            Pen lapiz = new Pen(Color.Black, 1);
            //SolidBrush brocha = new SolidBrush(Color.White);
            SolidBrush brocha = new SolidBrush(col);
            dc.FillRectangle(brocha, 0, 0, xf, yf);
            brocha.Dispose();
            lapiz.Dispose();

            return;
        }

        public void Mensaje(Panel panel, string mensa, bool cond)
        {
            int xf, yf, x1, y1;

            xf = panel.Size.Width;
            yf = panel.Size.Height;
            if (cond == false)
            {
                x1 = 10;
                y1 = 20;
            }
            else
            {
                x1 = 5;
                y1 = 1;
            }
            Graphics dc = panel.CreateGraphics();
            SolidBrush brocha2 = new SolidBrush(Color.Gainsboro);
            dc.FillRectangle(brocha2, 0, 0, xf, yf);
            brocha2.Dispose();
            SolidBrush brocha = new SolidBrush(Color.Black);
            dc.DrawString(mensa, new Font("Times New Roman", 10), brocha, x1, y1);
            brocha.Dispose();

            return;
        }

        /// <summary>
        /// Revisa si a determinado minuto corresponde sismos clasificados y si tienen lectura de amplitud y de coda,
        /// en otras palabras chequea y cuenta los minutos que correspondan a sismos clasificados en la base,
        /// escribe en los archivos datos.txt y amplis.txt los datos respectivos en caso de que existan sismos clasificados.
        /// </summary>
        /// <param name="panel">Panel utilizado para informar que se está leyendo la base.</param>
        /// <param name="rutbas">Dirección de la base de datos.</param>
        /// <param name="ll1">Tiempo en formato C# del momento desde donde se deben mostrar las trazas (fecha inicial).</param>
        /// <param name="ll2">Tiempo en formato C# del momento hasta donde se deben mostrar las trazas (fecha final).</param>
        /// <param name="cl">Arreglo de strings que contiene el tipo de clasificaciones que hay guardados para los volcanes, VT, LP, TL etc.</param>
        /// <param name="volcan">Arreglo de strings que contiene los volcanes que se encuentran en la base, RUIZ, MACHIN etc.</param>
        /// <returns></returns>
        public ushort Leerbase(Panel panel, string rutbas, long ll1, long ll2, string[] cl, string[] volcan)
        {
            // rutina que chequea si a determinado minuto corresponde sismos clasificados y si 
            // tienen lectura de amplitud y de coda.
            ushort contdatos;
            uint contampl = 0;
            int i, j;
            int an, me, di, ho, mi, se;
            long ll, llsis;
            string nom = "", fee = "", li = "";


            panel.Visible = true;
            Mensaje(panel, "Leyendo de la Base...\n", false);
            if (File.Exists("datos2.txt"))
                File.Delete("datos2.txt"); // archivos de clasificacion ordenados
            if (File.Exists("datos.txt"))
                File.Delete("datos.txt");
            if (File.Exists("amplis2.txt"))
                File.Delete("amplis2.txt"); // archivos de amplitud ordenados
            if (File.Exists("amplis.txt"))
                File.Delete("amplis.txt");

            StreamWriter le = new StreamWriter("amplis2.txt");
            // 86400000000 corresponde a un dia en centenares de nanosegundos, que es la unidad de tiempo 
            // que maneja la vartiable ticks del visual C#

            contampl = 0;
            //MessageBox.Show(ll2 - (ll1 += 864000000000) + "");

            for (ll = ll1; ll <= ll2; ll += 864000000000)
            {
                DateTime fech = new DateTime(ll);
                fee = string.Format("{0:yy}{0:MM}{0:dd}", fech);
                for (i = 0; i < cl.Length; i++)
                {
                    if (cl[i][2] == '+')
                    {
                        for (j = 0; j < volcan.Length - 1; j++)
                        {
                            nom = rutbas + "\\lec\\" + cl[i].Substring(0, 2) + "\\" + volcan[j][0] + cl[i][1] + fee + ".txt";
                            //MessageBox.Show(nom);
                            if (!File.Exists(nom)) continue;
                            if (File.Exists("cla.txt"))
                                File.Delete("cla.txt");
                            File.Copy(nom, "cla.txt", true);
                            if (File.Exists("cla.txt"))
                            {
                                li = "";
                                StreamReader ar = new StreamReader("cla.txt");
                                while (li != null)
                                {
                                    try
                                    {
                                        li = ar.ReadLine();

                                        if (li == null) break;
                                        if (li[82] == '.')
                                        {
                                            le.WriteLine(li);
                                            contampl += 1;// lleva la cuenta del numero de lecturas de amplitud
                                        }
                                    }
                                    catch
                                    {
                                    }
                                } // while....
                                ar.Close();
                            }//if file.exist.....
                        }// for j....
                    }// if cl.....
                    else
                    {
                        nom = rutbas + "\\lec\\" + cl[i].Substring(0, 2) + "\\X" + cl[i][1] + fee + ".txt";
                        if (!File.Exists(nom)) continue;
                        if (File.Exists("cla.txt")) File.Delete("cla.txt");
                        File.Copy(nom, "cla.txt", true);
                        if (File.Exists("cla.txt"))
                        {
                            li = "";
                            StreamReader ar = new StreamReader("cla.txt");
                            while (li != null)
                            {
                                try
                                {
                                    li = ar.ReadLine();
                                    if (li == null) break;
                                    if (li[82] == '.')
                                    {
                                        le.WriteLine(li);
                                        contampl += 1;// lleva la cuenta del numero de lecturas de amplitud
                                    }
                                }
                                catch
                                {
                                }
                            } // while....
                            ar.Close();
                        }//if file.exist.....
                    }
                } // for i....
            }// for ll....

            le.Close();

            if (contampl > 0)
            {
                //li = "/C sort /+88 amplis2.txt > amplis.txt";
                li = "/C sort amplis2.txt > amplis.txt";
                Dos(li, true);
                // MessageBox.Show("contampli="+contampl.ToString()+" li="+li);
            }

            contdatos = 0;
            StreamWriter pr = new StreamWriter("datos2.txt");
            for (ll = ll1; ll <= ll2; ll += 864000000000)
            {
                if (File.Exists("cla.txt"))
                    File.Delete("cla.txt");
                DateTime fech2 = new DateTime(ll);
                nom = rutbas + "\\cla\\" + string.Format("{0:yy}{0:MM}.txt", fech2);
                if (File.Exists(nom))
                    File.Copy(nom, "cla.txt", true);
                else
                    continue;
                fee = string.Format("{0:yy}{0:MM}{0:dd}", fech2);
                if (File.Exists("cla.txt"))
                {
                    li = "";
                    StreamReader ar2 = new StreamReader("cla.txt");
                    while (li != null)
                    {
                        try
                        {
                            li = ar2.ReadLine();
                            if (li == null)
                                break;
                            if (string.Compare(fee, li.Substring(0, 6)) == 0)
                            {
                                an = int.Parse(li.Substring(0, 2));
                                if (an < 85) an += 2000;
                                else an += 1900;
                                me = int.Parse(li.Substring(2, 2));
                                di = int.Parse(li.Substring(4, 2));
                                ho = int.Parse(li.Substring(7, 2));
                                mi = int.Parse(li.Substring(10, 2));
                                se = int.Parse(li.Substring(13, 2));
                                DateTime fechsis = new DateTime(an, me, di, ho, mi, se);
                                llsis = fechsis.Ticks;
                                pr.WriteLine(li.Substring(0, 28) + " " + li.Substring(40, 4) + " " + llsis.ToString() + " " + li.Substring(48));
                                contdatos += 1;
                            }
                        }
                        catch
                        {
                        }
                    }
                    ar2.Close();
                }
            }
            pr.Close();

            if (contdatos > 0)
            {
                li = "/C sort /+35 datos2.txt > datos.txt";
                Dos(li, true);
            }
            else
            {
                if (File.Exists("datos.txt"))
                    File.Delete("datos.txt");
            }

            panel.Visible = false;

            return (contdatos);
        }
        /*  public ushort Leerbase2(Panel panel, string rutbas, long ll1, long ll2, string[] cl, string[] volcan)
          {
              // rutina que chequea si a determinado minuto corresponde sismos clasificados y si 
              // tienen lectura de amplitud y de coda.
              ushort contdatos;
              uint contampl = 0;
              int an, me, di, ho, mi, se;
              long ll, llsis;
              string nom = "", fee = "", lin = "", nomlec = "", linle = "";

              contdatos = 0;
              panel.Visible = true;
              Mensaje(panel, "Leyendo de la Base...\n", false);
              if (File.Exists("datos2.txt")) File.Delete("datos2.txt"); // archivos de clasificacion ordenados
              if (File.Exists("datos.txt")) File.Delete("datos.txt");
              if (File.Exists("amplis2.txt")) File.Delete("amplis2.txt"); // archivos de amplitud ordenados
              if (File.Exists("amplis.txt")) File.Delete("amplis.txt");

              StreamWriter pr = new StreamWriter("datos2.txt");
              StreamWriter le = new StreamWriter("amplis2.txt");
              // 86400000000 corresponde a un dia en centenares de nanosegundos, que es la unidad de tiempo 
              // que maneja la variable ticks del visual C#
              for (ll = ll1; ll <= ll2; ll += 864000000000)
              {
                  if (File.Exists("cla.txt")) File.Delete("cla.txt");
                  DateTime fech = new DateTime(ll);
                  nom = rutbas + "\\cla\\" + string.Format("{0:yy}{0:MM}.txt", fech);
                  if (File.Exists(nom)) File.Copy(nom, "cla.txt", true);
                  else continue;
                  fee = string.Format("{0:yy}{0:MM}{0:dd}", fech);
                  //if (File.Exists(nom))
                  if (File.Exists("cla.txt"))
                  {
                      lin = "";
                      StreamReader ar = new StreamReader("cla.txt");
                      while (lin != null)
                      {
                          try
                          {
                              lin = ar.ReadLine();
                              if (lin == null) break;
                              if (string.Compare(fee, lin.Substring(0, 6)) == 0)
                              {
                                  an = int.Parse(lin.Substring(0, 2));
                                  if (an < 85) an += 2000;
                                  else an += 1900;
                                  me = int.Parse(lin.Substring(2, 2));
                                  di = int.Parse(lin.Substring(4, 2));
                                  ho = int.Parse(lin.Substring(7, 2));
                                  mi = int.Parse(lin.Substring(10, 2));
                                  se = int.Parse(lin.Substring(13, 2));
                                  DateTime fechsis = new DateTime(an, me, di, ho, mi, se);
                                  if (File.Exists("lectu.txt")) File.Delete("lectu.txt");
                                  nomlec = rutbas + "\\lec\\" + lin.Substring(26, 2) + "\\" + lin.Substring(25, 1) + lin.Substring(27, 1) + lin.Substring(0, 6) + ".txt";
                                  if (File.Exists(nomlec))
                                  {
                                      llsis = fechsis.Ticks;
                                      pr.WriteLine(lin.Substring(0, 28) + " " + lin.Substring(40, 4) + " " + llsis.ToString() + " " + lin.Substring(48));
                                      File.Copy(nomlec, "lectu.txt", true);
                                  }
                                  linle = "";
                                  if (File.Exists("lectu.txt"))
                                  {
                                      StreamReader ler = new StreamReader("lectu.txt");
                                      while (linle != null)
                                      {
                                          try
                                          {
                                              linle = ler.ReadLine();
                                              if (linle == null) break;
                                              if (string.Compare(lin.Substring(16,12),linle.Substring(74,12)) == 0)
                                              {
                                                  le.WriteLine(linle);
                                                  contampl += 1;// lleva la cuenta del numero de lecturas de amplitud
                                              }
                                          }
                                          catch
                                          {
                                          }
                                      }
                                      ler.Close();
                                  }
                                  contdatos += 1; // lleva la cuenta del numero de clasificaciones.
                              }// stringcompare
                          }
                          catch
                          {
                              // break;
                          }
                      }
                      ar.Close();
                  }
              }
              panel.Visible = false;
              pr.Close();
              le.Close();
              if (contdatos > 0)
              {
                  lin = "/C sort /+35 datos2.txt > datos.txt";
                  Dos(lin, true);
              }
              else
              {
                  if (File.Exists("datos.txt")) File.Delete("datos.txt");
              }
              if (contampl > 0)
              {
                  lin = "/C sort /+88 amplis2.txt > amplis.txt";
                  Dos(lin, true);
              }

              return (contdatos);
          }*/
        /// <summary>
        /// Configura y dibuja el panel donde se puede generar la marca que se le asigna a un sismo para posteriores búsquedas.
        /// </summary>
        /// <param name="panel">Panel donde se muestran los caracteres para generar la marca.</param>
        /// <param name="marca">Etiqueta que se le da al sismo para futuras búsquedas.</param>
        public void VerMarca(Panel panel, string marca)
        {
            string cc = "";
            int xf, yf, i, j, ii, jj, k, kk, dif;
            float fax, fay;

            char[] marcachar = marca.ToCharArray();
            xf = panel.Size.Width;
            yf = panel.Size.Height - 20;
            dif = 90 - 48;
            fay = yf / dif;
            fax = xf / 8;
            j = 0; jj = 0;
            ii = 0;
            Graphics dc = panel.CreateGraphics();
            SolidBrush brocha = new SolidBrush(Color.Black);
            SolidBrush brocha2 = new SolidBrush(Color.NavajoWhite);
            Pen lapiz = new Pen(Color.Blue, 2);
            dc.FillRectangle(brocha2, 0, 0, xf, panel.Size.Height);
            for (ii = 0; ii < 8; ii++)
            {
                k = (int)(marcachar[ii]);
                if (k == 42) 
                    kk = 0;
                else
                    kk = k - 47;
                i = (int)(ii * fax);
                jj = (int)(kk * fay);
                dc.DrawRectangle(lapiz, i, jj, fax, fay);
            }

            for (i = 0; i < 8; i++)
            {
                ii = (int)(10 + i * fax);
                cc = char.ToString('*');
                dc.DrawString(cc, new Font("Times New Roman", 9, FontStyle.Bold), brocha, ii, 2);
                for (j = 0; j <= dif; j++)
                {
                    jj = (int)(fay + j * fay);
                    cc = char.ToString((char)(j + 48));
                    dc.DrawString(cc, new Font("Times New Roman", 9, FontStyle.Bold), brocha, ii, jj);
                }
            }

            brocha.Dispose();
            brocha2.Dispose();
            lapiz.Dispose();

            return;
        }
        /// <summary>
        /// Pinta el nombre de usuario que está trabajando en la clasificación de trazas.
        /// </summary>
        /// <param name="panel">Panel donde se dibuja el letrero.</param>
        /// <param name="x">Coordenada x del panel.</param>
        /// <param name="y">Coordenada y del panel.</param>
        /// <param name="let">Texto que se dibuja en el panel.</param>
        /// <param name="col">Color del texto que se dibuja.</param>
        public void Letreros(Panel panel, int x, int y, string let, Color col)
        {
            Graphics dc = panel.CreateGraphics();
            SolidBrush brocha = new SolidBrush(col);

            dc.DrawString(let, new Font("Times New Roman", 9), brocha, x, y);
            brocha.Dispose();

            return;
        }
        /// <summary>
        /// Calcula a que distancia se deben pintar las líneas zapotes que se pintan como marcas de tiempo y  pinta las lineas.
        /// </summary>
        /// <param name="panel">El panel donde se pintan las marcas.</param>
        /// <param name="timin">Tiempo mínimo del que se registró una traza.</param>
        /// <param name="tim">Representa el tiempo de cada valor de cuenta para una traza especifica.</param>
        /// <param name="esp">Es el espaciamiento entre líneas.</param>
        /// <param name="dur">Duración de tiempo del intervalo donde se arrastra el mouse.</param>
        /// <param name="denom">Cantidad de lineas que se dibujan.</param>
        public void MarcaTiempo(Panel panel, double timin, double[] tim, ushort esp, float dur, int denom)
        {
            int i;
            int xf, yf, jb, b;
            long ll;
            float x1, y1;
            double fax, fay;
            double ti1, ti2, tii;


            // MessageBox.Show("timin=" + timin.ToString());
            xf = panel.Size.Width;
            yf = panel.Size.Height;

            jb = tim.Length - 1;
            if (esp == 0) 
                fay = (yf - 45.0) / (double)(denom);
            else
                fay = esp;
            fax = xf / dur;

            ll = (long)timin;
            ti1 = (double)(ll);
            for (i = 0; i <= 60; i++)
            {
                if (Math.IEEERemainder(ti1, 60.0) == 0) break;
                ti1 += 1.0;
            }
            ti2 = tim[jb];
            Graphics dc = panel.CreateGraphics();
            Pen lapiz = new Pen(Color.Orange, 1);
            Pen lapiz2 = new Pen(Color.Red, 3);

            for (tii = ti1; tii <= ti2; tii += 60.0)
            {
                b = (int)((tii - timin) / (double)(dur));
                x1 = (float)(((tii - timin) - (b * dur)) * fax);
                y1 = (float)(45.0 + b * fay + fay / 2);
                if (Math.IEEERemainder(tii, 3600.0) == 0)
                {
                    dc.DrawLine(lapiz2, x1, y1, x1, y1 - 12);
                    dc.DrawLine(lapiz2, x1 + 6, y1, x1 + 6, y1 - 12);
                }
                else
                {
                    dc.DrawLine(lapiz, x1, y1, x1, y1 - 10);
                }
            }

            lapiz.Dispose();
            lapiz2.Dispose();

            return;
        }
        /// <summary>
        /// Dibuja las pepas que indican en donde se ha clasificado un sismo.
        /// </summary>
        /// <param name="panel">El panel donde se pintan las pepas.</param>
        /// <param name="timin">Tiempo mínimo del que se registró una traza.</param>
        /// <param name="tim">Representa el tiempo de cada valor de cuenta para una traza especifica.</param>
        /// <param name="esp">Es el espaciamiento entre líneas.</param>
        /// <param name="dur">Duración de tiempo del intervalo donde se arrastra el mouse.</param>
        /// <param name="contampl">Cantidad de lecturas de amplitud de la base.</param>
        /// <param name="valampl"></param>
        /// <param name="clR">Representa la intensidad del color rojo.</param>
        /// <param name="clG">Representa la intensidad del celor verde.</param>
        /// <param name="clB">Representa la intensidad del color azul.</param>
        /// <param name="letampl"></param>
        /// <param name="cl">Tipos de clasificaciones que se pueden asignar a un sismo.</param>
        /// <param name="tam">Tamaño de las pepas.</param>
        /// <param name="nucla">Cantidad de clasificaciones disponibles.</param>
        /// <param name="siPampl"></param>
        /// <param name="volampl"></param>
        /// <param name="voll">Primera inicial del volcán.</param>
        /// <param name="cond">Visualización de pepas del volcán activo.</param>
        /// <param name="nolec">Indica si hay o no lecturas.</param>
        /// <param name="tigrabacion"></param>
        /// <param name="leclec"></param>
        /// <param name="denom">Denominador que se tiene en cuenta a la hora de dibujar las líneas.</param>
        public void PonePepas(Panel panel, double timin, double[] tim, ushort esp, float dur, ushort contampl, double[] valampl, byte[] clR,
            byte[] clG, byte[] clB, char[] letampl, string[] cl, float tam, short nucla, bool[] siPampl, char[] volampl, char voll, bool cond,
                   bool nolec, double tigrabacion, char leclec, int denom)
        {
            int xf, yf, jb, j, b, kk, kkk, i;
            float x1, y1;
            double fax, fay;
            double tf;

            //if (contampl == 0) return;
            try
            {
                xf = panel.Size.Width;
                yf = panel.Size.Height;
                jb = tim.Length - 1;
                if (esp == 0) 
                    fay = (yf - 45.0) / (double)(denom);
                else 
                    fay = esp;
                fax = xf / dur;
                tf = tim[jb - 1];

                Graphics dc = panel.CreateGraphics();
                Pen lapiz = new Pen(Color.Black, 1);

                if (tam == 2.0F) 
                    tam = 1.9F;

                if (contampl > 0)
                {
                    for (j = 0; j < contampl; j++)
                    {
                        //MessageBox.Show("j="+j.ToString()+" valampl="+valampl[j].ToString()+" timin="+timin.ToString()+" tf="+tf.ToString()+" letampl="+letampl[j]);
                        if (valampl[j] >= timin && valampl[j] <= tf)
                        {
                            b = (int)((valampl[j] - timin) / dur);
                            y1 = (float)(45.0 + b * fay + fay / 2.0 - tam);
                            x1 = (float)(((valampl[j] - timin) - b * dur) * fax - tam);
                            kk = -1;
                            for (i = 0; i < nucla; i++)
                            {
                                if (letampl[j] == cl[i][1]) 
                                    kk = i;
                            }
                            if (cond == true && kk > -1)
                            {
                                kkk = kk;
                                kk = -1;
                                if (volampl[j] == voll) 
                                    kk = kkk;
                            }
                            if (kk > -1)
                            {
                                SolidBrush brocha = new SolidBrush(Color.FromArgb(clR[kk], clG[kk], clB[kk]));
                                if (siPampl[j] == true)
                                {
                                    dc.FillEllipse(brocha, x1, y1, tam, tam);
                                    dc.DrawEllipse(lapiz, x1, y1, tam, tam);
                                }
                                else
                                {
                                    dc.FillRectangle(brocha, x1, y1, tam, tam);
                                    dc.DrawRectangle(lapiz, x1, y1, tam, tam);
                                }
                                brocha.Dispose();
                            }
                        }
                    }// for j...
                    if (nolec == true)
                    {
                        b = (int)((tigrabacion - timin) / dur);
                        y1 = (float)(45.0 + b * fay + fay / 2.0 - tam);
                        x1 = (float)(((tigrabacion - timin) - b * dur) * fax - tam);
                        kk = -1;
                        for (i = 0; i < nucla; i++)
                        {
                            if (leclec == cl[i][1]) kk = i;
                        }
                        SolidBrush brocha = new SolidBrush(Color.FromArgb(clR[kk], clG[kk], clB[kk]));
                        dc.FillRectangle(brocha, x1, y1, tam, tam);
                        dc.DrawRectangle(lapiz, x1, y1, tam, tam);
                        brocha.Dispose();
                    }
                }
                else if (nolec == true)
                {
                    //MessageBox.Show("nolec==true");
                }
                lapiz.Dispose();
            }
            catch
            {
                StreamWriter wr = File.AppendText(".\\pro\\ERROR_en_Pepas.txt");
                wr.WriteLine("dura=" + dur.ToString() + " tam=" + tam.ToString());
                wr.Close();
            }

            return;
        }

        public int EscribePanelEstaHP(Panel panelEsta, ushort nutra, string[] est, bool[] siEst, char[] pol)
        {
            int i, ii, j, k, xx, yy, kk, xf, yf;
            string esta = "", ca = "";

            if (panelEsta.Visible == false) 
                return (-1);

            xf = panelEsta.Size.Width;
            yf = panelEsta.Size.Height;
            i = 10 * nutra;
            //MessageBox.Show("xf="+xf.ToString()+" yf="+yf.ToString()+" i="+i.ToString());            
            ii = (nutra - 1) / 50;
            xx = 55;
            if (nutra >= 50)
            {
                yy = 531;
                xx = xx * 2;
            }
            else yy = (nutra + 1) * 10 + 31;

            panelEsta.Size = new Size(xx, yy);

            Graphics dc = panelEsta.CreateGraphics();
            SolidBrush brocha = new SolidBrush(Color.Black);
            SolidBrush brocha2 = new SolidBrush(Color.Red);
            SolidBrush bro = new SolidBrush(Color.Blue);

            k = 0;
            kk = -1;
            //MessageBox.Show("ii=" + ii.ToString());
            for (j = 0; j <= ii; j++)
            {
                for (i = 0; i < 50; i++)
                {
                    esta = est[k];
                    ca = pol[k].ToString();
                    if (siEst[k] == true)
                    {
                        dc.DrawString(esta, new Font("Times New Roman", 9), brocha, 0 + j * 60, i * 10 - 3);
                        dc.DrawString(ca, new Font("Times New Roman", 9), bro, 40 + j * 60, i * 10 - 3);
                    }
                    else
                    {
                        dc.DrawString(esta, new Font("Times New Roman", 9), brocha2, 0 + j * 60, i * 10 - 3);
                        dc.DrawString(ca, new Font("Times New Roman", 9), bro, 40 + j * 60, i * 10 - 3);
                        kk = 1;
                    }
                    k += 1;
                    if (k >= nutra) break;
                    //MessageBox.Show("");
                    //dc.DrawString(li, new Font("Times New Roman", 9, FontStyle.Bold), brocha, 1, i * 10);
                }
            }
            brocha.Dispose();
            brocha2.Dispose();

            return (kk);
        }
        /// <summary>
        /// Dibuja los nombres de las estaciones en el panel panelEsta, si la estación esta activa en la clasificación que se está 
        /// realizando esta se pinta de color negro, sino se pinta de color rojo para indicar que no se tiene en cuenta 
        /// en la clasificación de ese sismo.
        /// </summary>
        /// <param name="panelEsta">Panel donde se grafican los nombres de las estaciones activas,
        /// este panel está dentro del panel de clasificación.</param>
        /// <param name="nutra">Es la cantidad de trazas leídas.</param>
        /// <param name="est">Contiene los nombres de las estaciones.</param>
        /// <param name="siEst">Indica que estaciones están seleccionadas como activas.</param>
        /// <returns>-1 en caso de que todas las estaciones estén activas.
        ///           1 en caso de que por lo menos 1 de las estaciones no este activa.
        ///</returns>
        public int EscribePanelEsta(Panel panelEsta, ushort nutra, string[] est, bool[] siEst)
        {
            int i, ii, j, k, xx, yy, kk;
            string esta = "";

            if (panelEsta.Visible == false)
                return (-1);

            ii = (nutra - 1) / 50;
            xx = 42 + ii * 35;
            if (nutra >= 50)
            {
                yy = 501;
            }
            else
            {
                yy = (nutra + 1) * 10 + 1;
            }

            panelEsta.Size = new Size(xx, yy);

            Graphics dc = panelEsta.CreateGraphics();
            SolidBrush brocha = new SolidBrush(Color.Black);
            SolidBrush brocha2 = new SolidBrush(Color.Red);

            k = 0;
            kk = -1;
            for (j = 0; j <= ii; j++)
            {
                for (i = 0; i < 50; i++)
                {
                    if (!char.IsLetterOrDigit(est[k][4]))
                        esta = est[k].Substring(0, 4);
                    else
                        esta = est[k];
                    if (siEst[k] == true)
                        //dc.DrawString(esta, new Font("Times New Roman", 9), brocha, 1 + j * 35, i * 10 - 3);
                        dc.DrawString(esta, new Font("Times New Roman", 8), brocha, 1 + j * 35, i * 10 - 3);
                    else
                    {
                        //dc.DrawString(esta, new Font("Times New Roman", 9), brocha2, 1 + j * 35, i * 10 - 3);
                        dc.DrawString(esta, new Font("Times New Roman", 8), brocha2, 1 + j * 35, i * 10 - 3);
                        kk = 1;
                    }
                    k += 1;
                    if (k >= nutra)
                        break;
                    //dc.DrawString(li, new Font("Times New Roman", 9, FontStyle.Bold), brocha, 1, i * 10);
                }
            }
            brocha.Dispose();
            brocha2.Dispose();

            // if (kk == 1) boTodas.BackColor = Color.PaleVioletRed;
            // else boTodas.BackColor = Color.White;

            return (kk);
        }



        public void Topo(Panel panel, string nomvol, double facm, double la, double lo,
            double lasi, double losi, Color coltopo, bool cond, double laR, double loR,
              double laER, double loER)
        {
            int iniX, iniY, xf, yf, x1, y1, x2, y2;
            double f1, fcpi, fclo;
            string linea = "";
            string nombre, ca = "";
            char[] delim = { ' ', '\t' };
            string[] pa = null;
            GraphicsPath path;


            path = new GraphicsPath();
            xf = panel.Size.Width;
            yf = panel.Size.Height;

            iniX = xf / 2;
            iniY = yf / 2;
            x1 = 0;
            y1 = 0;
            x2 = 0;
            y2 = 0;
            fcpi = Math.PI / 180.0;
            fclo = facm * ((Math.PI / 180.0) * Math.Cos(la * fcpi) * 6367.449) / 110.9;

            //Graphics dc = e.Graphics;
            Graphics dc = panel.CreateGraphics();
            Pen lapiz = new Pen(coltopo, 1);
            Pen lapizVia = new Pen(coltopo, 2);
            Pen lapiz2 = new Pen(Color.Black, 1);
            Pen lapiz3 = new Pen(Color.Red, 2);
            SolidBrush brocha = new SolidBrush(coltopo);

            nombre = ".\\coor\\" + nomvol[0] + ".map";
            if (!File.Exists(nombre))
            {
                ca = ".\\ima\\" + nomvol[0] + ".jpg";
                if (File.Exists(ca))
                {
                    PonerImagen(panel, nomvol, facm, la, lo, lasi, losi, fclo);
                }
                return;
            }

            StreamReader ar = new StreamReader(nombre);
            while (linea != null)
            {
                try
                {
                    linea = ar.ReadLine();
                    if (linea == null) break;
                    pa = linea.Split(delim);
                    f1 = facm * (Convert.ToDouble(pa[2]) - la);
                    y1 = iniY - (int)f1;
                    f1 = fclo * (Convert.ToDouble(pa[3]) - lo);
                    x1 = iniX + (int)f1;
                    if (linea[0] == 'C')
                    {
                        if (linea[1] == 'I')
                        {
                            x2 = x1;
                            y2 = y1;
                        }
                        else
                        {
                            dc.DrawLine(lapiz, x1, y1, x2, y2);
                            x2 = x1;
                            y2 = y1;
                        }
                    }
                    else if (linea[0] == 'M' && linea[1] != 'P')
                    {
                        if (linea[1] == 'I')
                        {

                            x2 = x1;
                            y2 = y1;
                        }
                        else if (linea[1] != 'F')
                        {
                            path.AddLine(x2, y2, x1, y1);
                            x2 = x1;
                            y2 = y1;
                        }
                        else if (linea[1] == 'F')
                        {
                            path.AddLine(x1, y1, x2, y2);
                            dc.FillPath(brocha, path);
                            dc.DrawPath(lapiz, path);
                            path.CloseAllFigures();
                        }
                    }
                    else if (linea[0] == 'E')
                    {
                        dc.FillRectangle(brocha, x1 - 3, y1 - 3, 6, 6);
                        dc.DrawRectangle(lapiz2, x1 - 3, y1 - 3, 6, 6);
                    }
                    else if (linea[1] == 'P')
                    {
                        dc.FillEllipse(brocha, x1, y1, 6, 6);
                        dc.DrawEllipse(lapiz, x1, y1, 6, 6);
                    }
                    else if (linea[0] == 'V')
                    {
                        if (linea[1] == 'I')
                        {
                            x2 = x1;
                            y2 = y1;
                        }
                        else
                        {
                            dc.DrawLine(lapizVia, x1, y1, x2, y2);
                            x2 = x1;
                            y2 = y1;
                        }
                    }
                    //MessageBox.Show(x1.ToString()+"  "+y1.ToString());
                }
                catch
                {
                    //MessageBox.Show("ERROR  "+linea);
                }
            }
            ar.Close();

            if (cond == true)
            {
                Pen lapiz4 = new Pen(Color.DarkOrange, 2);
                SolidBrush brochaR = new SolidBrush(Color.Gold);
                f1 = facm * (laR - la);
                y1 = iniY - (int)f1;
                f1 = fclo * (loR - lo);
                x1 = iniX + (int)f1;
                dc.DrawLine(lapiz4, x1 + 5, y1, x1 - 5, y1);
                dc.DrawLine(lapiz4, x1, y1 - 5, x1, y1 + 5);

                f1 = facm * (laER - la);
                y1 = iniY - (int)f1;
                f1 = fclo * (loER - lo);
                x1 = iniX + (int)f1;
                dc.FillRectangle(brochaR, x1 - 3, y1 - 3, 6, 6);
                dc.DrawRectangle(lapiz4, x1 - 3, y1 - 3, 6, 6);

                lapiz4.Dispose();
                brochaR.Dispose();
            }

            f1 = facm * (lasi - la);
            y1 = iniY - (int)f1;
            f1 = fclo * (losi - lo);
            x1 = iniX + (int)f1;
            dc.DrawLine(lapiz3, x1 + 5, y1, x1 - 5, y1);
            dc.DrawLine(lapiz3, x1, y1 - 5, x1, y1 + 5);

            brocha.Dispose();
            lapiz2.Dispose();
            lapiz.Dispose();
            lapizVia.Dispose();
            lapiz3.Dispose();

            return;
        } // topo

        public void TopoMapaArribos(Panel panel, double facm, double la, double lo,
             char map)
        {
            int iniX, iniY, xf, yf, x1, y1, y1x = 0, x2, y2;
            double f1, fcpi, fclo;
            string linea = "";
            string ca = "";
            char[] delim = { ' ', '\t' };
            string[] pa = null;
            GraphicsPath path;


            //ca = ".\\coor\\" + nombre;
            ca = ".\\coor\\" + map + ".map";
            if (!File.Exists(ca)) return;

            path = new GraphicsPath();
            xf = panel.Size.Width;
            yf = panel.Size.Height;

            iniX = xf / 2;
            iniY = yf / 2;
            x1 = 0;
            y1 = 0;
            x2 = 0;
            y2 = 0;
            fcpi = Math.PI / 180.0;
            fclo = facm * ((Math.PI / 180.0) * Math.Cos(la * fcpi) * 6367.449) / 110.9;

            Graphics dc = panel.CreateGraphics();
            Pen lapiz = new Pen(Color.Gray, 1);
            Pen lapizVia = new Pen(Color.Orange, 1);
            Pen lapizF = new Pen(Color.Red, 1);
            Pen lapizR = new Pen(Color.LightBlue, 3);
            SolidBrush brocha = new SolidBrush(Color.BlueViolet);
            SolidBrush bro = new SolidBrush(Color.Black);

            StreamReader ar = new StreamReader(ca);
            while (linea != null)
            {
                try
                {
                    linea = ar.ReadLine();
                    if (linea == null) break;
                    pa = linea.Split(delim);
                    f1 = facm * (Convert.ToDouble(pa[2]) - la);
                    y1 = iniY - (int)f1;
                    f1 = fclo * (Convert.ToDouble(pa[3]) - lo);
                    x1 = iniX + (int)f1;
                    if (linea[0] == 'C')
                    {
                        if (linea[1] == 'I')
                        {
                            x2 = x1;
                            y2 = y1;
                        }
                        else
                        {
                            dc.DrawLine(lapiz, x1, y1, x2, y2);
                            x2 = x1;
                            y2 = y1;
                        }
                    }
                    if (linea[0] == 'R')
                    {
                        if (linea[1] == 'I')
                        {
                            x2 = x1;
                            y2 = y1;
                        }
                        else
                        {
                            dc.DrawLine(lapizR, x1, y1, x2, y2);
                            x2 = x1;
                            y2 = y1;
                        }
                    }
                    else if (linea[0] == 'M' && linea[1] != 'P')
                    {
                        if (linea[1] == 'I')
                        {
                            x2 = x1;
                            y2 = y1;
                            y1x = y1;
                        }
                        else if (linea[1] != 'F')
                        {
                            path.AddLine(x2, y2, x1, y1);
                            x2 = x1;
                            y2 = y1;
                            if (y1 > y1x) y1x = y1;
                        }
                        else if (linea[1] == 'F')
                        {
                            path.AddLine(x1, y1, x2, y2);
                            dc.FillPath(brocha, path);
                            dc.DrawPath(lapiz, path);
                            path.CloseAllFigures();
                            if (pa.Length == 5) dc.DrawString(pa[4], new Font("Arial", 8, FontStyle.Regular), bro, x1 - 10, y1x + 5);
                        }
                    }
                    else if (linea[1] == 'P')
                    {
                        dc.FillEllipse(brocha, x1, y1, 6, 6);
                        dc.DrawEllipse(lapiz, x1, y1, 6, 6);
                        if (pa.Length == 5) dc.DrawString(pa[4], new Font("Arial", 8, FontStyle.Regular), bro, x1 - 10, y1 + 5);
                    }
                    else if (linea[0] == 'V')
                    {
                        if (linea[1] == 'I')
                        {
                            x2 = x1;
                            y2 = y1;
                        }
                        else
                        {
                            dc.DrawLine(lapizVia, x1, y1, x2, y2);
                            x2 = x1;
                            y2 = y1;
                        }
                    }
                    else if (linea[0] == 'F')
                    {
                        if (linea[1] == 'I')
                        {

                            x2 = x1;
                            y2 = y1;
                        }
                        else
                        {
                            dc.DrawLine(lapizF, x1, y1, x2, y2);
                            x2 = x1;
                            y2 = y1;
                        }
                    }
                }
                catch
                {
                    //MessageBox.Show("ERROR  "+linea);
                }
            }
            ar.Close();

            lapiz.Dispose();
            lapizVia.Dispose();
            lapizF.Dispose();
            brocha.Dispose();
            bro.Dispose();

            FocosArribos(panel, facm, la, lo, map);

            return;
        } // topoArribos

        public void EstacionesArribos(Panel panel, double facm, double la, double lo, double[] lae,
              double[] loe)
        {
            int i, j, iniX, iniY, xf, yf, x1, y1;
            double f1, fcpi, fclo;

            xf = panel.Size.Width;
            yf = panel.Size.Height;
            iniX = xf / 2;
            iniY = yf / 2;
            x1 = 0;
            y1 = 0;
            fcpi = Math.PI / 180.0;
            fclo = facm * ((Math.PI / 180.0) * Math.Cos(la * fcpi) * 6367.449) / 110.9;

            Graphics dc = panel.CreateGraphics();
            Pen lapiz = new Pen(Color.Black, 1);
            SolidBrush brocha = new SolidBrush(Color.LightGray);


            i = lae.Length;
            for (j = 0; j < i; j++)
            {
                f1 = facm * (lae[j] - la);
                y1 = iniY - (int)f1;
                f1 = fclo * (loe[j] - lo);
                x1 = iniX + (int)f1;
                dc.DrawRectangle(lapiz, x1 - 3, y1 - 3, 6, 6);
            }
            //brocha.Dispose();
            lapiz.Dispose();
            brocha.Dispose();

            return;
        }

        public void FocosArribos(Panel panel, double facm, double laa, double loo, char map)
        {
            int i, iniX, iniY, xf, yf, x1, y1, nu;
            double fcpi, fclo, laf, lof, Norte, Oeste, inc, zz, dd;
            string li = "";
            char[] delim = { ' ', '\t' };
            string[] pa = null;
            Color col;

            if (File.Exists("locML.txt")) File.Delete("locML.txt");
            if (!File.Exists(".\\h\\r.pun")) return;
            xf = panel.Size.Width;
            yf = panel.Size.Height;
            iniX = xf / 2;
            iniY = yf / 2;
            x1 = 0;
            y1 = 0;
            fcpi = Math.PI / 180.0;
            fclo = facm * ((Math.PI / 180.0) * Math.Cos(laa * fcpi) * 6367.449) / 110.9;
            if (map == 'X') inc = 15.0;
            else inc = 2.0;

            nu = MejorRms();

            Graphics dc = panel.CreateGraphics();
            Pen lap = new Pen(Color.Black, 1);

            StreamReader ar = new StreamReader(".\\h\\r.pun");
            li = ar.ReadLine();
            if (li[23] == 'N') Norte = 1.0;
            else Norte = -1.0;
            if (li[33] == 'W') Oeste = -1.0;
            else Oeste = 1.0;
            i = 0;
            while (li != null)
            {
                try
                {
                    li = ar.ReadLine();
                    if (li == null) break;
                    laf = Norte * ((double)(int.Parse(li.Substring(18, 2))) + (double.Parse(li.Substring(21, 5)) / 60.0));
                    lof = Oeste * ((double)(int.Parse(li.Substring(27, 3))) + (double.Parse(li.Substring(31, 5)) / 60.0));
                    zz = double.Parse(li.Substring(37, 6));
                    y1 = iniY - (int)(facm * (laf - laa));
                    x1 = iniX + (int)(fclo * (lof - loo));
                    if (zz < inc) col = Color.Yellow;
                    else if (zz < inc * 2.0) col = Color.Orange;
                    else if (zz < inc * 3.0) col = Color.Red;
                    else if (zz < inc * 4.0) col = Color.Green;
                    else if (zz < inc * 5.0) col = Color.Blue;
                    else col = Color.Magenta;
                    SolidBrush bro = new SolidBrush(col);
                    dc.FillEllipse(bro, x1 - 4, y1 - 4, 8, 8);
                    dc.DrawEllipse(lap, x1 - 4, y1 - 4, 8, 8);
                    if (i == nu)
                    {
                        StreamWriter wr = File.CreateText("locML.txt");
                        wr.WriteLine(laf.ToString() + " " + lof.ToString());
                        wr.Close();
                        dc.DrawRectangle(lap, x1 - 5, y1 - 5, 10, 10);
                        SolidBrush bro2 = new SolidBrush(Color.BlueViolet);
                        dc.DrawString(li, new Font("Lucida Console", 10, FontStyle.Bold), bro2, 113, panel.Height - 20);
                        bro2.Dispose();
                    }
                    bro.Dispose();
                }
                catch
                {
                }
                i += 1;
            }
            ar.Close();

            ColocarMLVista(panel);

            lap.Dispose();
        }

        public void ColocarMLVista(Panel panel)
        {
            double dd;
            string li = "";
            char[] delim = { ' ', '\t' };
            string[] pa = null;

            if (!File.Exists(".\\oct\\maglocalVista.txt")) return;
            Graphics dc = panel.CreateGraphics();
            Pen lap = new Pen(Color.Black, 1);
            StreamReader arr = new StreamReader(".\\oct\\maglocalVista.txt");
            try
            {
                li = arr.ReadLine();
                pa = li.Split(delim);
                dd = double.Parse(pa[1]);
            }
            catch
            {
                lap.Dispose();
                arr.Close();
                return;
            }
            arr.Close();
            li = pa[0] + string.Format(" ML= {0:0.0}", dd);
            SolidBrush bro4 = new SolidBrush(Color.White);
            dc.FillRectangle(bro4, 140, 40, 155, 24);
            bro4.Dispose();
            dc.DrawRectangle(lap, 140, 40, 155, 24);
            SolidBrush bro3 = new SolidBrush(Color.DarkRed);
            dc.DrawString(li, new Font("Lucida Console", 14, FontStyle.Bold), bro3, 142, 42);
            bro3.Dispose();

            return;
        }

        int MejorRms()
        {
            int i, nu = -1;
            double rms = 1000.0, dd;
            string li = "";

            if (!File.Exists(".\\h\\r.pun")) return (-1);
            StreamReader ar = new StreamReader(".\\h\\r.pun");
            li = ar.ReadLine();
            i = 0;
            while (li != null)
            {
                try
                {
                    li = ar.ReadLine();
                    if (li == null) break;
                    if (li[64] == '.')
                    {
                        dd = double.Parse(li.Substring(62, 5));
                        if (dd < rms)
                        {
                            rms = dd;
                            nu = i;
                        }
                    }
                    i += 1;
                }
                catch
                {
                }
            }

            ar.Close();

            return (nu);
        }

        public void UnaEstacionArribo(Panel panel, double facm, double la, double lo, double lae,
             double loe, Color col)
        {
            int iniX, iniY, xf, yf, x1, y1;
            double f1, fcpi, fclo;

            xf = panel.Size.Width;
            yf = panel.Size.Height;
            iniX = xf / 2;
            iniY = yf / 2;
            x1 = 0;
            y1 = 0;
            fcpi = Math.PI / 180.0;
            fclo = facm * ((Math.PI / 180.0) * Math.Cos(la * fcpi) * 6367.449) / 110.9;

            Graphics dc = panel.CreateGraphics();
            Pen lapiz = new Pen(Color.Black, 1);
            SolidBrush brocha = new SolidBrush(col);

            f1 = facm * (lae - la);
            y1 = iniY - (int)f1;
            f1 = fclo * (loe - lo);
            x1 = iniX + (int)f1;
            dc.FillRectangle(brocha, x1 - 3, y1 - 3, 6, 6);
            dc.DrawRectangle(lapiz, x1 - 3, y1 - 3, 6, 6);

            brocha.Dispose();
            lapiz.Dispose();

            return;
        }

        void PonerImagen(Panel panel, string nomvol, double facMapa,
            double la, double lo, double lasi, double losi, double fclo)
        {
            PointF ulCorner1 = new PointF(0.0F, 0.0F);
            PointF urCorner1 = new PointF(panel.Width, 0.0F);
            PointF llCorner1 = new PointF(0.0F, panel.Height);
            PointF[] destPara1 = { ulCorner1, urCorner1, llCorner1 };
            int xf, yf, x1, y1, iniX, iniY, inixim, iniyim, anchoim, altoim;
            double la1, la2, lo1, lo2, faclaim, facloim, f1;
            double latim1 = 0, latim2 = 0, lonim1 = 0, lonim2 = 0;
            string ca, ca2, linea;
            char[] delim = { ' ', '\t' };
            string[] pa = null;
            Image Imagen;

            ca = ".\\ima\\" + nomvol[0] + ".jpg";
            ca2 = ".\\ima\\" + nomvol[0] + ".txt";
            if (File.Exists(ca) && File.Exists(ca2))
            {
                Imagen = Image.FromFile(ca);
                linea = "";
                StreamReader ar2 = new StreamReader(ca2);
                try
                {
                    linea = ar2.ReadLine();
                    pa = linea.Split(delim);
                    latim1 = double.Parse(pa[0]);
                    lonim1 = double.Parse(pa[1]);
                    linea = ar2.ReadLine();
                    pa = linea.Split(delim);
                    latim2 = double.Parse(pa[0]);
                    lonim2 = double.Parse(pa[1]);
                }
                catch
                {
                    //MessageBox.Show("ERROR  "+linea);
                }
                ar2.Close();

                Graphics dc = panel.CreateGraphics();
                Pen lapiz = new Pen(Color.Red, 2);
                xf = panel.Size.Width;
                yf = panel.Size.Height;
                iniX = xf / 2;
                iniY = yf / 2;
                lo1 = lo + (double)(1 - iniX) / fclo;
                la1 = la + (double)(iniY - 1) / facMapa;
                lo2 = lo + (double)(xf - iniX) / fclo;
                la2 = la + (double)(iniY - yf) / facMapa;
                faclaim = Imagen.Height / (latim1 - latim2);
                facloim = Imagen.Width / (lonim1 - lonim2);
                iniyim = (int)((latim1 - la1) * faclaim);
                inixim = (int)((lonim1 - lo1) * facloim);
                altoim = (int)((latim1 - la2) * faclaim) - iniyim;
                anchoim = (int)((lonim1 - lo2) * facloim) - inixim;
                GraphicsUnit units = GraphicsUnit.Pixel;
                ImageAttributes imageAttr = new ImageAttributes();
                ColorMatrix myColorMatrix = new ColorMatrix();
                myColorMatrix.Matrix33 = 1.0F;
                imageAttr.SetColorMatrix(myColorMatrix);
                RectangleF srcRect = new RectangleF(inixim, iniyim, anchoim, altoim);
                dc.DrawImage(Imagen, destPara1, srcRect, units, imageAttr);

                f1 = facMapa * (lasi - la);
                y1 = iniY - (int)f1;
                f1 = fclo * (losi - lo);
                x1 = iniX + (int)f1;
                dc.DrawLine(lapiz, x1 + 5, y1, x1 - 5, y1);
                dc.DrawLine(lapiz, x1, y1 - 5, x1, y1 + 5);
                lapiz.Dispose();
            }

            return;
        }
        /// <summary>
        /// Dibuja un rectángulo sin relleno sobre la traza en el intervalo de tiempo correspondiente a un sismo clasificado.
        /// </summary>
        /// <param name="panel">Panel donde se dibuja el rectángulo.</param>
        /// <param name="timin">Tiempo mínimo del en que se registró una traza, o dicho de otra forma la lectura que empezó primero.</param>
        /// <param name="tim">Vector que almacena los valores de los tiempos de las cuentas de una traza especifica.</param>
        /// <param name="tiar">Tiempo inicial de la duración del archivo clasificado.</param>
        /// <param name="duar">Duración del archivo clasificado</param>
        /// <param name="esp">Valor del espectro.</param>
        /// <param name="dur">Duración del tiempo del intervalo de traza sobre el cual se arrastró el mouse.</param>
        /// <param name="contarch">Almacena la cantidad de sismos clasificados en un lapso de tiempo seleccionado.</param>
        public void VerArchi(Panel panel, double timin, double[] tim, double[] tiar, ushort[] duar, ushort esp, float dur, ushort contarch)
        {
            short cont = 0;
            int xf, yf, jj, j, b1, b2, lar;
            float x1, x2, y1, y2, w, h;
            double fax, fay;
            Pen[] lap = new Pen[3];


            if (contarch == 0)
                return;
            xf = panel.Size.Width;
            yf = panel.Size.Height;
            lar = tim.Length;
            jj = 1 + (int)((tim[lar - 1] - timin) / dur);
            if (esp == 0)
                fay = (yf - 45) / jj;
            else fay = esp;
            fax = xf / dur;

            Graphics dc = panel.CreateGraphics();
            lap[0] = new Pen(Color.Orange, 2);
            lap[1] = new Pen(Color.Orchid, 2);
            lap[2] = new Pen(Color.Green, 2);
            cont = 0;
            for (j = 0; j < contarch; j++)
            {
                b1 = (int)((tiar[j] - timin) / dur);
                y1 = (float)(45.0 + b1 * fay + fay / 3.0);
                b2 = (int)(((tiar[j] + duar[j]) - timin) / dur);
                y2 = (float)(45.0 + b2 * fay + fay / 3.0);
                h = (float)(fay / 3.0);
                x1 = (float)(((tiar[j] - timin) - b1 * dur) * fax);
                x2 = (float)((((tiar[j] + duar[j]) - timin) - b2 * dur) * fax);
                if (y1 != y2)
                {
                    w = xf - x1;
                    dc.DrawRectangle(lap[cont], x1, y1, w, h);
                    w = x2;
                    dc.DrawRectangle(lap[cont], -1, y2, w, h);
                }
                else
                {
                    w = x2 - x1;
                    dc.DrawRectangle(lap[cont], x1, y1, w, h);
                }
                //if (y1 != y2) MessageBox.Show("x1="+x1.ToString()+" y1="+y1.ToString()+" x2="+x2.ToString()+" y2="+y2.ToString());
                cont += 1;
                if (cont >= 3)
                    cont = 0;
            }

            lap[0].Dispose();
            lap[1].Dispose();
            lap[2].Dispose();
            return;
        }

        public void GraMatlab(double laff, double loff, double[] lae, double[] loe, bool[] siFac,
           double[] fac, int can, bool[] siest, int[] lectu, short[] ga, double[] tle1, double[] tle2,
           double tini, string usu, string sis, char tar, string[] est)
        {
            int i, numlec = 0;
            long ll;
            double dismx, ampmx, difla, diflo, tvenini, tvenfin, res, aam, ddi;
            double[] amp = new double[Ma];
            double[] dis = new double[Ma];
            string ca = "", fecha = "", resfe = "";

            i = usu.Length;
            if (i < 3) usu = "___";
            dismx = 0;
            ampmx = 0;
            numlec = 0;
            tvenini = 0;
            tvenfin = 0;
            for (i = 0; i < can; i++)
            {
                if (siFac[i] == true && siest[i] == true)
                {
                    difla = laff - lae[i];
                    diflo = loff - loe[i];
                    dis[i] = Math.Sqrt(difla * difla + diflo * diflo);
                    if (dis[i] > dismx) dismx = dis[i];
                    amp[i] = (lectu[i] / ga[i]) * fac[i];
                    if (amp[i] > ampmx) ampmx = amp[i];
                    numlec += 1;
                    if (tvenini == 0) tvenini = tle1[i];
                    else if (tvenini > tle1[i]) tvenini = tle1[i];
                    if (tvenfin == 0) tvenfin = tle2[i];
                    else if (tvenfin < tle2[i]) tvenfin = tle2[i];

                }
            }

            res = tvenini - (double)((long)(tvenini));
            resfe = string.Format("{0:0.00}", res);
            ll = (long)(Fei + tvenini * 10000000.0);
            DateTime fech = new DateTime(ll);
            fecha = string.Format("{0:yy}{0:MM}{0:dd} {0:HH}{0:mm} {0:ss}", fech) + resfe.Substring(1);

            StreamWriter wr1 = File.CreateText("mat1.txt");
            StreamWriter wr2 = File.CreateText("mat2.txt");

            ca = "nombre_traza='" + sis + "';";
            wr1.WriteLine(ca);
            ca = "numero_lecturas='" + numlec.ToString() + "';";
            wr1.WriteLine(ca);
            //ca = "tiempo_inicial_archivo_num='"+tini.ToString()+"';";
            ca = "tiempo_inicial_archivo_num='" + string.Format("{0:0000000000.00}", tini) + "';";
            wr1.WriteLine(ca);
            //ca = "tiempo_inicial_ventana_num='" + tvenini.ToString() + "';";
            ca = "tiempo_inicial_ventana_num='" + string.Format("{0:0000000000.00}", tvenini) + "';";
            wr1.WriteLine(ca);
            ca = "tiempo_inicial_ventana_fecha='" + fecha + "';";
            wr1.WriteLine(ca);
            ca = "tiempo_final_ventana_num='" + string.Format("{0:0000000000.00}", tvenfin) + "';";
            wr1.WriteLine(ca);
            ca = "estudiante='" + usu + "';";
            wr1.WriteLine(ca);
            ca = "sistema='" + tar + "';";
            wr1.WriteLine(ca);

            for (i = 0; i < can; i++)
            {
                //MessageBox.Show("i="+i.ToString()+" "+est[i].Substring(0,4)+" sifac="+siFac[i].ToString());
                if (siFac[i] == true && siest[i] == true)
                {
                    aam = amp[i] / ampmx;
                    if (aam > 0)
                    {
                        ddi = dis[i] / dismx;
                        ca = est[i].Substring(0, 4) + "=" + string.Format("{0:0.000000}", aam) + ";tini";
                        ca += est[i].Substring(0, 4) + "='" + string.Format("{0:0000000000.000}", tle1[i]) + "';tifin";
                        ca += est[i].Substring(0, 4) + "='" + string.Format("{0:0000000000.000}", tle2[i]) + "';";
                        wr2.WriteLine(ca);
                    }
                }
            }

            wr1.Close();
            wr2.Close();

            if (!Directory.Exists(".\\mat")) Directory.CreateDirectory(".\\mat");
            File.Copy("mat1.txt", ".\\mat\\VARSATE.M", true);
            ca = "/C sort /R /+6 mat2.txt >> .\\mat\\VARSATE.M";
            Dos(ca, true);

            return;
        }

        public void VerMapa(Panel panel, char nmvol, double la, double lo, string pun, double diff,
           double laat, double loot, Color col)
        {
            int iniX, iniY, xf, yf, x1, y1, x2, y2, i;
            double f1, laf, lof, dif, la1, lo1, facm, fcpi, fclo, ff;
            string linea = "";
            string nombre;
            char[] delim = { ' ', '\t' };
            string[] pa = null;
            GraphicsPath path;


            path = new GraphicsPath();
            xf = panel.Size.Width;
            yf = panel.Size.Height;
            laf = 0; lof = 0;

            iniX = xf / 2;
            iniY = yf / 2;
            x1 = 0;
            y1 = 0;
            x2 = 0;
            y2 = 0;

            if (pun != "")
            {
                i = int.Parse(pun.Substring(17, 3));
                f1 = double.Parse(pun.Substring(20, 6)) / 60.0;
                laf = (double)(i) + f1;
                i = int.Parse(pun.Substring(26, 4));
                f1 = double.Parse(pun.Substring(30, 6)) / 60.0;
                lof = (double)(i) + f1;
                la1 = laf - la;
                lo1 = lof - lo;
                dif = Math.Sqrt(la1 * la1 + lo1 * lo1);
            }
            else
            {
                dif = diff;
                laf = laat;
                lof = loot;
            }
            /*facm = 0.3 * (140.0 / dif);
            if (nmvol == 'X' && facm > 50) facm = 50.0;
            else if (facm > 2500.0 && pun != "") facm = 2500.0;*/
            if (pun == "" && (laat != 0 && loot != 0)) ff = 0.6;
            else ff = 0.3;
            facm = ff * (140.0 / dif);
            if (nmvol == 'X' && facm > 50)
            {
                if (laat != 0 && loot != 0) facm = 200.0;
                else facm = 50.0;
            }
            else if (facm > 2500.0 && pun != "") facm = 2500.0;
            fcpi = Math.PI / 180.0;
            fclo = facm * ((Math.PI / 180.0) * Math.Cos(la * fcpi) * 6367.449) / 110.9;

            Graphics dc = panel.CreateGraphics();
            SolidBrush broc = new SolidBrush(Color.White);
            dc.FillRectangle(broc, 0, 0, panel.Width, panel.Height);
            broc.Dispose();
            Pen lapiz = new Pen(col, 1);
            Pen lapizVia = new Pen(Color.Orange, 2);
            Pen lapiz2 = new Pen(Color.Black, 1);
            Pen lapfoc = new Pen(Color.Red, 2);
            SolidBrush brocha = new SolidBrush(Color.Gray);

            nombre = ".\\coor\\" + nmvol + ".map";
            if (!File.Exists(nombre))
            {
                MessageBox.Show("NO EXISTE " + nombre + " !!");
                return;
            }

            StreamReader ar = new StreamReader(nombre);
            while (linea != null)
            {
                try
                {
                    linea = ar.ReadLine();
                    if (linea == null) break;
                    pa = linea.Split(delim);
                    f1 = facm * (Convert.ToDouble(pa[2]) - la);
                    y1 = iniY - (int)f1;
                    f1 = fclo * (Convert.ToDouble(pa[3]) - lo);
                    x1 = iniX + (int)f1;
                    if (linea[0] == 'C')
                    {
                        if (linea[1] == 'I')
                        {
                            x2 = x1;
                            y2 = y1;
                        }
                        else
                        {
                            dc.DrawLine(lapiz, x1, y1, x2, y2);
                            x2 = x1;
                            y2 = y1;
                        }
                    }
                    else if (linea[0] == 'M' && linea[1] != 'P')
                    {
                        if (linea[1] == 'I')
                        {

                            x2 = x1;
                            y2 = y1;
                        }
                        else if (linea[1] != 'F')
                        {
                            path.AddLine(x2, y2, x1, y1);
                            x2 = x1;
                            y2 = y1;
                        }
                        else if (linea[1] == 'F')
                        {
                            path.AddLine(x1, y1, x2, y2);
                            dc.FillPath(brocha, path);
                            dc.DrawPath(lapiz, path);
                            path.CloseAllFigures();
                        }
                    }
                    else if (linea[0] == 'E' && pun != "")
                    {
                        dc.FillRectangle(brocha, x1 - 3, y1 - 3, 6, 6);
                        dc.DrawRectangle(lapiz2, x1 - 3, y1 - 3, 6, 6);
                    }
                    else if (linea[1] == 'P')
                    {
                        dc.FillEllipse(brocha, x1, y1, 6, 6);
                        dc.DrawEllipse(lapiz, x1, y1, 6, 6);
                    }
                    else if (linea[0] == 'V')
                    {
                        if (linea[1] == 'I')
                        {
                            x2 = x1;
                            y2 = y1;
                        }
                        else
                        {
                            dc.DrawLine(lapizVia, x1, y1, x2, y2);
                            x2 = x1;
                            y2 = y1;
                        }
                    }
                    //MessageBox.Show(x1.ToString()+"  "+y1.ToString());
                }
                catch
                {
                    //MessageBox.Show("ERROR  "+linea);
                }
            }
            ar.Close();

            if (pun != "" || (laf != 0 && lof != 0))
            {
                f1 = facm * (laf - la);
                y1 = iniY - (int)f1;
                f1 = fclo * (lof - lo);
                x1 = iniX + (int)f1;
                if (pun != "")
                {
                    dc.DrawLine(lapfoc, x1 - 5, y1, x1 + 5, y1);
                    dc.DrawLine(lapfoc, x1, y1 + 5, x1, y1 - 5);
                }
                else
                {
                    dc.DrawRectangle(lapfoc, x1 - 5, y1 - 5, 10, 10);
                }
            }

            brocha.Dispose();
            lapiz2.Dispose();
            lapiz.Dispose();
            lapizVia.Dispose();
            // MessageBox.Show("facm="+facm.ToString());

            return;
        }

        public void Dibampl(Panel panel, Panel panel2, double laff, double loff, double la, double lo,
           double diff, int can, bool[] siFac, double[] fac, double[] lae, double[] loe,
             byte[] clR, Byte[] clG, byte[] clB, int[] lectu, short[] ga, int xx, int yy, bool[] siest,
           Color col)
        {
            int iniX, iniY, xf, yf, x1, y1, i, k;
            double f1, facm, dismx, ampmx, difla, diflo, laf, lof;
            double[] amp = new double[Ma];
            double[] dis = new double[Ma];


            xf = panel.Size.Width;
            yf = panel.Size.Height;
            iniX = xf / 2;
            iniY = yf / 2;
            x1 = 0;
            y1 = 0;
            facm = 0.3 * (140.0 / diff);
            if (xx < 0)
            {
                laf = laff;
                lof = loff;
            }
            else
            {
                laf = la + (iniY - yy) / facm;
                //lof = lo + (iniX - xx) / facm;
                lof = lo + (xx - iniX) / facm;
            }

            Graphics dc = panel.CreateGraphics();
            Pen lapfoc = new Pen(col, 2);

            k = 0;
            for (i = 0; i < can; i++)
            {
                if (lae[i] > -500.0 && siest[i] == true && lectu[i] >= 0)
                {
                    Pen lap = new Pen(Color.FromArgb(clR[i], clG[i], clB[i]), 2);
                    f1 = facm * (lae[i] - la);
                    y1 = iniY - (int)f1;
                    f1 = facm * (loe[i] - lo);
                    //x1 = iniX - (int)f1;
                    x1 = iniX + (int)f1;
                    dc.DrawRectangle(lap, x1 - 3, y1 - 3, 6, 6);
                    lap.Dispose();
                    //MessageBox.Show("la=" + la.ToString() + " lae=" + lae[i].ToString() + " lo=" + lo.ToString() + " loe=" + loe[i].ToString());
                    k += 1;
                }
            }
            if (k < 3) return;
            f1 = facm * (laf - la);
            y1 = iniY - (int)f1;
            f1 = facm * (lof - lo);
            //x1 = iniX - (int)f1;
            x1 = iniX + (int)f1;
            dc.DrawLine(lapfoc, x1 - 5, y1, x1 + 5, y1);
            dc.DrawLine(lapfoc, x1, y1 + 5, x1, y1 - 5);
            lapfoc.Dispose();

            Graphics dc2 = panel2.CreateGraphics();
            xf = panel2.Size.Width - 20;
            yf = panel2.Size.Height - 20;

            dismx = 0;
            ampmx = 0;
            for (i = 0; i < can; i++)
            {
                if (siFac[i] == true && siest[i] == true && lectu[i] >= 0)
                {
                    difla = laf - lae[i];
                    diflo = lof - loe[i];
                    dis[i] = Math.Sqrt(difla * difla + diflo * diflo);
                    if (dis[i] > dismx) dismx = dis[i];
                    amp[i] = (lectu[i] / ga[i]) * fac[i];
                    if (amp[i] > ampmx) ampmx = amp[i];
                }
            }
            for (i = 0; i < can; i++)
            {
                if (siFac[i] == true && siest[i] == true && lectu[i] >= 0)
                {
                    Pen lap2 = new Pen(Color.FromArgb(clR[i], clG[i], clB[i]), 2);
                    x1 = 4 + (int)((dis[i] / dismx) * xf);
                    y1 = 4 + yf - (int)((amp[i] / ampmx) * yf);
                    dc2.DrawEllipse(lap2, x1 - 5, y1 - 5, 10, 10);
                    lap2.Dispose();
                }
            }

            return;
        }

        public bool EliminaClasificacion(double tii, string sis, string rutbas)
        {
            long ll;
            string fech = "", rusis = "", ruamp = "", ruloc = "", ruate1 = "", ruate2 = "", ruclas = "", ca = "";
            string li = "";
            bool sud = false, atn = false, ate = false, loc = false, amp = false, si = false, clas = false;


            ll = (long)(Fei + tii * 10000000.0);
            DateTime fee = new DateTime(ll);
            fech = string.Format("{0:yyyy}/{0:MM}/{0:dd} {0:HH}:{0:mm}:{0:ss}", fee);

            rusis = rutbas + "\\sud\\" + sis.Substring(10, 2) + "\\" + fech.Substring(2, 2) + "\\" + sis;
            ruclas = rutbas + "\\cla\\" + fech.Substring(2, 2) + fech.Substring(5, 2) + ".txt";
            ruamp = rutbas + "\\lec\\" + sis.Substring(10, 2) + "\\" + sis.Substring(9, 1) + sis.Substring(11, 1) + fech.Substring(2, 2) + fech.Substring(5, 2) + fech.Substring(8, 2) + ".txt";
            ruloc = rutbas + "\\loc\\" + fech.Substring(2, 2) + fech.Substring(5, 2) + ".ipn";
            ruate1 = rutbas + "\\ate\\" + fech.Substring(2, 2) + fech.Substring(5, 2) + ".atn";
            ruate2 = rutbas + "\\ate\\" + fech.Substring(2, 2) + fech.Substring(5, 2) + "ate.txt";
            //MessageBox.Show(sis + " sud=" + sud.ToString() + " amp=" + amp.ToString() + " loc=" + loc.ToString() + " atn=" + atn.ToString() + " ate=" + ate.ToString());
            if (File.Exists(rusis)) sud = true;
            else
            {
                ca = rutbas + "\\sud\\" + sis.Substring(10, 2) + "\\" + fech.Substring(2, 2);
                if (!Directory.Exists(ca)) MessageBox.Show("NO EXISTE " + ca + " !!!!");
            }
            if (File.Exists(ruclas))
            {
                StreamReader ar = new StreamReader(ruclas);
                StreamWriter le = new StreamWriter("class.txt");
                li = "";
                while (li != null)
                {
                    try
                    {
                        li = ar.ReadLine();
                        if (li == null) break;
                        if (string.Compare(li.Substring(16, 12), sis.Substring(0, 12)) != 0) le.WriteLine(li);
                        else clas = true;
                    }
                    catch
                    {
                    }
                }
                ar.Close();
                le.Close();
            }
            else
            {
                ca = rutbas + "\\cla";
                if (!Directory.Exists(ca)) MessageBox.Show("NO EXISTE " + ca + " !!!!");
            }
            if (File.Exists(ruamp))
            {
                StreamReader ar = new StreamReader(ruamp);
                StreamWriter le = new StreamWriter("ampp.txt");
                li = "";
                while (li != null)
                {
                    try
                    {
                        li = ar.ReadLine();
                        if (li == null) break;
                        if (string.Compare(li.Substring(74, 12), sis.Substring(0, 12)) != 0) le.WriteLine(li);
                        else amp = true;
                    }
                    catch
                    {
                    }
                }
                ar.Close();
                le.Close();
            }
            else
            {
                ca = rutbas + "\\lec\\" + sis.Substring(10, 2);
                if (!Directory.Exists(ca)) MessageBox.Show("NO EXISTE " + ca + " !!!!");
            }
            if (File.Exists(ruloc))
            {
                File.Copy(ruloc, "eliminaLoc.txt", true);
                //StreamReader ar = new StreamReader(ruloc);
                StreamReader ar = new StreamReader("eliminaLoc.txt");
                StreamWriter lc = new StreamWriter("locc.txt");
                li = "";
                while (li != null)
                {
                    try
                    {
                        li = ar.ReadLine();
                        if (li == null) break;
                        if (char.IsDigit(li[0]))
                        {
                            if (string.Compare(li.Substring(84, 12), sis.Substring(0, 12)) != 0) si = true;
                            else
                            {
                                loc = true;
                                si = false;
                            }
                        }
                        if (si == true) lc.WriteLine(li);
                    }
                    catch
                    {
                    }
                }
                ar.Close();
                lc.Close();
            }
            else
            {
                ca = rutbas + "\\loc";
                if (!Directory.Exists(ca)) MessageBox.Show("NO EXISTE " + ca + " !!!!");
            }
            if (File.Exists(ruate1))
            {
                StreamReader ar = new StreamReader(ruate1);
                StreamWriter lc = new StreamWriter("ate1.txt");
                li = "";
                while (li != null)
                {
                    try
                    {
                        li = ar.ReadLine();
                        if (li == null) break;
                        if (char.IsDigit(li[0]))
                        {
                            if (string.Compare(li.Substring(0, 12), sis.Substring(0, 12)) != 0) si = true;
                            else
                            {
                                atn = true;
                                si = false;
                            }
                        }
                        if (si == true) lc.WriteLine(li);
                    }
                    catch
                    {
                    }
                }
                ar.Close();
                lc.Close();
            }
            else
            {
                ca = rutbas + "\\ate";
                if (!Directory.Exists(ca)) MessageBox.Show("NO EXISTE " + ca + " !!!!");
            }
            if (File.Exists(ruate2))
            {
                StreamReader ar = new StreamReader(ruamp);
                StreamWriter le = new StreamWriter("ate2.txt");
                li = "";
                while (li != null)
                {
                    try
                    {
                        li = ar.ReadLine();
                        if (li == null) break;
                        if (string.Compare(li.Substring(84, 12), sis.Substring(0, 12)) != 0) le.WriteLine(li);
                        else ate = true;
                    }
                    catch
                    {
                    }
                }
                ar.Close();
                le.Close();
            }

            try
            {
                if (sud == true) File.Delete(rusis);
                if (clas == true) File.Copy("class.txt", ruclas, true);
                if (amp == true) File.Copy("ampp.txt", ruamp, true);
                if (loc == true) File.Copy("locc.txt", ruloc, true);
                if (atn == true) File.Copy("ate1.txt", ruate1, true);
                if (ate == true) File.Copy("ate2.txt", ruate2, true);
            }
            catch
            {
            }

            return (true);
        }

        public double[] Codigo_Irig_E(int[] cu, double timblo, double ct, double ra)
        {
            int i, ii, j, jj, k, kk, nublo, año, lar, segm, mmx, mmn, PROM;
            int mx, mn, bloque = 0, vez = 0;
            int ddd = 0, hhh = 0, mmm = 0, sss = 0;
            int[] muestra = new int[2];
            int[] irig;
            int[] prom = new int[5];
            long ll0, ll1, ll;
            double dd, ddA, ddB, facra, timirig, fcra;
            double[] dift = new double[3];
            string codigo = "";
            bool eureka = false, inicio = false;
            bool[] si = new bool[10];


            dift[0] = -1.0;
            dift[1] = -1.0;
            dift[2] = -1.0;
            muestra[0] = -1;
            muestra[1] = -1;
            vez = 0;
            lar = cu.Length;
            segm = (int)(lar / 5.0);
            for (i = 0; i < 5; i++)
            {
                mmx = cu[i * segm];
                mmn = mmx;
                for (j = 1 + i * segm; j < i * segm + segm; j++)
                {
                    if (mmx < cu[j]) mmx = cu[j];
                    else if (mmn > cu[j]) mmn = cu[j];
                }
                prom[i] = (int)((mmx + mmn) / 2.0);
            }
            k = prom[0];
            for (i = 1; i < 5; i++) k += prom[i];

            ll1 = (long)(Fei + timblo * 10000000.0);
            DateTime fech = new DateTime(ll1);
            año = int.Parse(string.Format("{0:yyyy}", fech));
            ll1 = (long)(ll1 / 10000000.0);
            DateTime fech1 = new DateTime(año, 1, 1, 0, 0, 0);
            ll0 = fech1.Ticks;
            timirig = ((double)(ll0) - Feisuds) / 10000000.0;

            mx = cu[0];
            mn = mx; // tot es lar
            for (i = 1; i < lar; i++)
            {
                if (cu[i] > mx) mx = cu[i];
                else if (cu[i] < mn) mn = cu[i];
            }
            PROM = (int)(k / 5.0);
            irig = new int[lar];
            nublo = 0;
            k = 0;
            for (j = 0; j < lar; j++)
            {
                if (cu[j] < PROM)
                {
                    irig[j] = 0;
                    k = 0;
                }
                else
                {
                    irig[j] = 1;
                    k += 1;
                    if (k == 8) nublo += 1;
                }
            }
            if (nublo == 0) return (dift);

            k = 0;
            kk = 0;
            jj = 0;
            ii = 0;
            facra = 1.0 / ra;
            ddA = 0;
            ddB = 0;

            for (j = 0; j < lar; j++)
            {
                if (irig[j] == 0)
                {
                    k += 1;
                    if (kk > 0)
                    {
                        dd = kk * facra;
                        if (dd > 0.07 && dd < 0.09)
                        {
                            if (ddA > 0.07 && ddA < 0.09) eureka = true;
                            else
                            {
                                eureka = false;
                                if (inicio == true)
                                {
                                    if (codigo.Length >= 8)
                                    {
                                        if (bloque == 0) // segundos
                                        {
                                            if (codigo[5] == '1') sss += 10;
                                            if (codigo[6] == '1') sss += 20;
                                            if (codigo[7] == '1') sss += 40;
                                        }
                                        else if (bloque == 1)  //minutos
                                        {
                                            if (codigo[0] == '1') mmm += 1;
                                            if (codigo[1] == '1') mmm += 2;
                                            if (codigo[2] == '1') mmm += 4;
                                            if (codigo[3] == '1') mmm += 8;
                                            if (codigo[5] == '1') mmm += 10;
                                            if (codigo[6] == '1') mmm += 20;
                                            if (codigo[7] == '1') mmm += 40;
                                        }
                                        else if (bloque == 2) // horas
                                        {
                                            if (codigo[0] == '1') hhh += 1;
                                            if (codigo[1] == '1') hhh += 2;
                                            if (codigo[2] == '1') hhh += 4;
                                            if (codigo[3] == '1') hhh += 8;
                                            if (codigo[5] == '1') hhh += 10;
                                            if (codigo[6] == '1') hhh += 20;
                                        }
                                        else if (bloque == 3) // dias1
                                        {
                                            if (codigo[0] == '1') ddd += 1;
                                            if (codigo[1] == '1') ddd += 2;
                                            if (codigo[2] == '1') ddd += 4;
                                            if (codigo[3] == '1') ddd += 8;
                                            if (codigo[5] == '1') ddd += 10;
                                            if (codigo[6] == '1') ddd += 20;
                                            if (codigo[7] == '1') ddd += 40;
                                            if (codigo[8] == '1') ddd += 80;
                                        }
                                        else if (bloque == 4) // dias2
                                        {
                                            if (codigo[0] == '1') ddd += 100;
                                            if (codigo[1] == '1') ddd += 200;
                                        }
                                        else if (bloque == 5)
                                        {
                                            if (vez == 0)
                                            {
                                                ll1 = (ddd - 1) * 86400 + hhh * 3600 + mmm * 60 + sss;
                                                timirig += (double)(ll1);
                                                dift[0] = timirig - (timblo + muestra[0] / ra);
                                            }
                                            else if (vez == 1)
                                            {
                                                ll = (ddd - 1) * 86400 + hhh * 3600 + mmm * 60 + sss;
                                                ll0 = ll - ll1;
                                                fcra = (double)(muestra[1] - muestra[0]) / (double)(ll0);
                                                dift[1] = fcra - ra;
                                                timirig += (double)(ll0);
                                                dift[2] = timirig - (timblo + muestra[1] / ra);
                                                break;
                                            }
                                            inicio = false;
                                            vez += 1;
                                            if (vez >= 2) break;
                                        }

                                        bloque += 1;
                                    }
                                    codigo = "";
                                }
                            }
                        }
                        else if (dd > 0.04 && dd < 0.06 && inicio == true) codigo += "1";
                        else if (dd > 0.01 && dd < 0.03 && inicio == true) codigo += "0";
                        ddA = dd;
                        jj = j;
                    }
                    kk = 0;
                }
                else if (irig[j] == 1)
                {
                    kk += 1;
                    if (k > 0)
                    {
                        dd = k * facra;
                        if (dd > 0 && dd < 0.03 && eureka == true)
                        {
                            muestra[vez] = ii;
                            inicio = true;
                            bloque = 0;
                            sss = 0;
                            mmm = 0;
                            hhh = 0;
                            ddd = 0;
                            codigo = "";
                        }
                        ddB = dd;
                        ii = j;
                    }
                    k = 0;
                }
            }

            return (dift);
        }

        public void MapaMundo(Panel panel)
        {
            int j = 0, k = 0, xf, yf, x1 = 0, y1 = 0, x2 = 0, y2 = 0;
            double lat = 0, lon = 0, faclon, facla, facpi;
            string lin = "";
            bool condi = false;

            if (!File.Exists(".\\coor\\mundo.txt")) return;
            xf = panel.Width;
            yf = panel.Height;
            facla = yf / 2.0;
            faclon = xf / 360.0;
            facpi = Math.PI / 180.0;
            StreamReader ar = new StreamReader(".\\coor\\mundo.txt");

            Graphics dc = panel.CreateGraphics();
            SolidBrush bro = new SolidBrush(Color.White);
            dc.FillRectangle(bro, 0, 0, panel.Width, panel.Height);
            bro.Dispose();
            Pen lapiz = new Pen(Color.Black, 1);

            while (lin != null)
            {
                try
                {
                    lin = ar.ReadLine();
                    if (lin == null) break;
                    if (lin[0] == '#') condi = false;
                    else
                    {
                        for (j = 0; j < lin.Length; j++) if (char.IsControl(lin[j])) break;
                        for (k = j + 1; k < lin.Length; k++) if (char.IsControl(lin[k])) break;
                        lon = double.Parse(lin.Substring(0, j - 1));
                        lat = double.Parse(lin.Substring(j, k - j));
                        x1 = (int)((xf / 2.0) + lon * faclon);
                        y1 = (int)((yf / 2.0) - Math.Sin(lat * facpi) * facla);
                        if (condi == true) dc.DrawLine(lapiz, x1, y1, x2, y2);
                        x2 = x1;
                        y2 = y1;
                        condi = true;
                    }
                }
                catch
                {
                    MessageBox.Show("ERROR: j=" + j.ToString() + " k=" + k.ToString() + " largo=" + lin.Length + " la=" + lat.ToString() + " lon=" + lon.ToString() + "  " + lin);
                }
            }

            lapiz.Dispose();
            ar.Close();

            return;
        }

        public void SismoNEIC(Panel panel, double lat, Double lon, double zz, double mag,
              float laNeic, float loNeic)
        {
            int xf, yf, x1 = 0, y1 = 0, tam = 0;
            double faclon, facla, facpi;
            Color col;

            xf = panel.Width;
            yf = panel.Height;
            facla = yf / 2.0;
            faclon = xf / 360.0;
            facpi = Math.PI / 180.0;

            Graphics dc = panel.CreateGraphics();
            Pen lapiz = new Pen(Color.Black, 1);

            tam = (int)(mag * 1.5) + 1;
            x1 = (int)((xf / 2.0) + lon * faclon - (tam / 2.0));
            y1 = (int)((yf / 2.0) - Math.Sin(lat * facpi) * facla - (tam / 2.0));

            if (zz < 33.0) col = Color.Yellow;
            else if (zz < 200.0) col = Color.Red;
            else if (zz < 400.0) col = Color.LightBlue;
            else col = Color.Violet;
            SolidBrush brocha = new SolidBrush(col);
            dc.FillEllipse(brocha, x1, y1, tam, tam);
            dc.DrawEllipse(lapiz, x1, y1, tam, tam);
            brocha.Dispose();

            if (laNeic > -1000.0F && loNeic > -1000.0F)
            {
                x1 = (int)((xf / 2.0) + loNeic * faclon);
                y1 = (int)((yf / 2.0) - Math.Sin(laNeic * facpi) * facla);
                SolidBrush bro = new SolidBrush(Color.Orange);
                dc.FillRectangle(bro, x1 - 3, y1 - 3, 6, 6);
                dc.DrawRectangle(lapiz, x1 - 3, y1 - 3, 6, 6);
                bro.Dispose();
            }

            return;
        }

        public double[] HBajo(short M, float rat, double Fc)
        {
            int i, j;
            double fpi = 2.0 * Math.PI;
            double fpi2 = 4.0 * Math.PI;
            double fac, ff, sum;
            double[] h;

            ff = Fc * (1.0 / rat);
            fac = fpi * ff;
            h = new double[M];
            j = (int)(M / 2.0);
            for (i = 0; i < j; i++) h[i] = Math.Sin(fac * (i - (M / 2.0))) / (i - (M / 2.0)); // muestras inferiores
            h[j] = fac;
            for (i = j + 1; i < M; i++) h[i] = Math.Sin(fac * (i - (M / 2.0))) / (i - (M / 2.0)); // muestras superiores
            for (i = 0; i < M; i++) h[i] = h[i] * (0.42 - 0.5 * Math.Cos(fpi * i / M) + 0.08 * Math.Cos(fpi2 * i / M));
            sum = 0;
            for (i = 0; i < M; i++) sum += h[i];
            for (i = 0; i < M; i++) h[i] = h[i] / sum;

            return (h);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="M"></param>
        /// <param name="rat"></param>
        /// <param name="Fc"></param>
        /// <returns></returns>
        public double[] HAlto(short M, float rat, double Fc)
        {
            int i, j;
            double[] h;

            h = new double[M];
            h = HBajo(M, rat, Fc);
            for (i = 0; i < M; i++) 
                h[i] = -h[i];
            j = (int)(M / 2.0);
            h[j] = h[j] + 1.0;

            return (h);
        }

        public double[] HBand(short M, float rat, double Fc1, double Fc2)
        {
            int i, j;
            double[] h1, h2, h;

            h = new double[M];
            h1 = new double[M];
            h2 = new double[M];

            h1 = HBajo(M, rat, Fc1);
            h2 = HAlto(M, rat, Fc2);

            for (i = 0; i < M; i++) h[i] = h1[i] + h2[i];
            for (i = 0; i < M; i++) h[i] = -h[i];
            j = (int)(M / 2.0);
            h[j] = h[j] + 1.0;

            return (h);
        }

        public int[] PasaBajos(int[] dat, short M, float rat, double Fc)
        {
            int i, j, k;
            double fpi = 2.0 * Math.PI;
            double fac;
            double dd, ff;
            int[] cf;
            double[] h;

            ff = Fc * (1.0 / rat);
            fac = fpi * ff;
            h = new double[M];
            cf = new int[dat.Length];
            for (i = 0; i < dat.Length; i++) cf[i] = 0;

            h = HBajo(M, rat, Fc);

            for (j = (int)(M / 2.0); j < (dat.Length - (int)(M / 2.0)); j++)
            {
                dd = 0;
                k = 0;
                for (i = (int)(-M / 2.0); i < (int)(M / 2.0); i++) dd += dat[j + i] * h[k++];
                cf[j] = (int)(dd);
            }

            return (cf);
        }
        /// <summary>
        /// Se utiliza para aplicar un filtro que atenue las frecuencias mas bajas de la señal.
        /// </summary>
        /// <param name="dat"></param>
        /// <param name="M"></param>
        /// <param name="rat"></param>
        /// <param name="Fc"></param>
        /// <returns></returns>
        public int[] PasaAltos(int[] dat, short M, float rat, double Fc)
        {
            int i, j, k;
            double dd;
            int[] cf;
            double[] h;

            h = new double[M];
            cf = new int[dat.Length];
            for (i = 0; i < dat.Length; i++) 
                cf[i] = 0;
            h = HAlto(M, rat, Fc);
            for (j = (int)(M / 2.0); j < (dat.Length - (int)(M / 2.0)); j++)
            {
                dd = 0;
                k = 0;
                for (i = (int)(-M / 2.0); i < (int)(M / 2.0); i++) dd += dat[j + i] * h[k++];
                cf[j] = (int)(dd);
            }

            return (cf);
        }

        public int[] PasaBanda(int[] dat, short M, float rat, double Fc1, double Fc2)
        {
            int i, j, k;
            double dd;
            int[] cf;
            double[] h;

            h = new double[M];
            cf = new int[dat.Length];
            for (i = 0; i < dat.Length; i++) cf[i] = 0;

            h = HBand(M, rat, Fc1, Fc2);

            for (j = (int)(M / 2.0); j < (dat.Length - (int)(M / 2.0)); j++)
            {
                dd = 0;
                k = 0;
                for (i = (int)(-M / 2.0); i < (int)(M / 2.0); i++) dd += dat[j + i] * h[k++];
                cf[j] = (int)(dd);
            }

            return (cf);
        }






    }
}
