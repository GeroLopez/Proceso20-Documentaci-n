using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Word = Microsoft.Office.Interop.Word; 
using System.Reflection;
using System.Globalization;
using System.Collections;

/*
 * Este formulario corresponde a la localizacion de sismos con el Hypo71.
 * El tipo de lectura se controla con la variable ond: 0 para la P, 1 para la S y 2 para la coda.
 * nutra, es el numero de canales totales. tminx, es el tiempo menor entre todas las trazas, tmini,
 * es el tiempo menor entre las trazas visibles o activas. durx, es la duracion totakl del sismo,
 * dura es la duracion de la ventana activa. inite es la profundidad inicial de iteracion, ite, es
 * el intervalo de iteracion en km. lasi y losi, las coordenadas del sismo. mejpun es la variable
 * que guarda la seleccion o mejor resultado (pun). mejite guarda el numero de dicha seleccion en 
 * la tabla de resultados; su valor es de -1 cuando no hay seleccion. imp es la variable con el
 * valor impulsivo o no. impu es dicho valor guardado para cada estacion. pola es la variable con
 * el valor de polaridad. pol es dicho valor para cada estacion.
 * klas variables que comienzan con fr1, son tomadas desde el formulario 1.
 */

namespace Proceso20
{
    public partial class Form2 : Form
    {
        Form1 fr1;
        const double Fei = 621355968000000000.0;
        const double Feisuds = 621355968000000000.0;
        const int Ma = 500;   // aqui debe colocarse el mismo valor que en el Form1!!.
        ushort nutra,sipro;
        short vol,refe=1,ond=0,peso=0,mejite=-1,numerrP=0,idfocmec=-1,totfocmec=0;
        int bx1, by1,xmli,cana,nuestmeca=0,vol_viejo=-1,espaciado=20;
        int[] mlxx = new int[3];
        int[] mlmm = new int[3];
        float inite=0,ite=0.5F,amp=1.0F;
        float itV = -1.0F, incV = -1.0F, itT = -1.0F, incT = -1.0F;
        double durx,dura,duraord,tminx,tmini,tminiord;
        double lasi, losi, facm, facmapa;
        double rumbo, buza, pitch;
        char imp = 'I';
        char[] impu = new char[Ma];
        char[] pol = new char[Ma];
        char[] pol2 = new char[Ma];
        string clas="",mejpun="";
        private Image myPicture = null;

        int[] ord;// variable que guarda los arribos en orden
        int[] lar = new int[Ma];// tamaño en muestras de cada traza
        int[] mx = new int[Ma]; // valor maximo en cuentas pico a pico
        int[] mn = new int[Ma]; // valor minimo en cuentas pico a pico
        int[] pro = new int[Ma]; // cero de la traza
        short[] by = new short[Ma]; // bytes por dato
        short[] ga = new short[Ma]; // ganancia de las cuentas
        short[][] pes = new short[Ma][];// peso de la lectura del arribo
        double[] ra = new double[Ma];// rata de muestreo
        double[][] fas = new double[Ma][];// lectura del arribo. la primera dimension corresponde a la 
        // estacion. la segunda al tipo de arribo (0:P,1:S,2:Coda).
        double[][] teo = new double[Ma][];// tiempo teorico.
        double[][] tim = new double[Ma][];// tiempo en formato suds de cada muestra.
        int[][] cu = new int[Ma][];// valor en cuentas de cada muestra.
        string[] est = new string[Ma];// nombre de la traza.
        char[] comp = new char[Ma];// tipo de componente si es el caso.
        char[] tar = new char[Ma];// letra identificadora de la tarjeta que capturo la traza.
        Boolean[] siEst = new Boolean[Ma];// variable que controla si una traza es activa o no.
        Boolean[] siEstMem = new Boolean[Ma];
        bool quita=false,hyp=false,grabas=true,verTeo=false,siteo=false,load=false,guia=false;
        bool oct=false,sioct=false,pola=true,fasauto=false,abso=false;
        bool HP71PC = false;
        float valHP71PC = 0.15F;
        string[] estml;// estas variables son para la magnitud local.
        int[] iniml, finml;
        string[] factml;
        char[] compml;
        double[] laestml;
        double[] loestml;
        double timlini, timlfin, timl1, timl2;
        short[] ordml;
        int[] nuestml = new int[3];
        short numl, idml;
        double[] ml;
        bool[] siboml;
        double ML = -10.0;//ver arriba
        string nomML="";
        Color colfondo, colinea, colotr1, colP, colS, colC;

        string[] estrot = new string[3];  // estaciones para rotacion en Radial y Transversal
        short[] idestrot = new short[3];
        int[][] nmrot = new int[3][];
        double[][] cuRT = new double[2][];
        double tiniRot, lafR, lofR, laeR, loeR; //tiempo inicial para rotacion de componentes y coordenadas del foco sismico y estacion seleccionada
        bool guiaRota=false,rota=false;
        byte ondRota = 0;

        //variable para conocer el numero y estaciones de la cabecera del modelo.
        bool[] siH;

        int[][] cf;  // variables para el filtro
        int[] mxF, mnF, proF;
        short M = 256;
        float Fc1 = 2.0F, Fc2 = 8.0F;
        char cfilt = '0';
        bool sifilt = false, yafilt = false, Punto = false;

        ToolTip tip = new ToolTip();
        Util util = new Util();


        public Form2(Form1 frmm1)
        {
            this.fr1 = frmm1;
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            const int Max = 498;
            int i,ii,iii,j,jjj,nmi,nmf,lari,tot,k,kk,tam;
            double tm, tx;
            string li="",ca="",ca2="",ca3="",s="";
            char[] delim = { ' ', '\t' };
            string[] pa = null;


            if (File.Exists(".\\h\\autopick.exe") && File.Exists(".\\h\\xtrhy71.exe")) boAutoPic.Visible = true;// estos dos programas son gratutos y permiten realizar las lecturas de P y Coda automaticamente.
            if (File.Exists(".\\pro\\estafases.txt")) boFasAuto.Visible = true;
            load = false;
            boRota.Visible = false;
            textBoxHPC.Visible = false;

            if (File.Exists(".\\pro\\inicio.txt"))
            {
                li = "";
                StreamReader pr = new StreamReader(".\\pro\\inicio.txt");
                while (li != null)
                {
                    try
                    {
                        li = pr.ReadLine();
                        if (li == null || li[0] == '*') break;
                        if (li.Length > 13)
                        {
                            if (string.Compare(li.Substring(0, 9), "ITERACION") == 0)
                            {
                                pa = li.Split(delim);
                                itV = float.Parse(pa[1]);
                                incV = float.Parse(pa[2]);
                                if (pa.Length > 3)
                                {
                                    itT = float.Parse(pa[3]);
                                    incT = float.Parse(pa[4]);
                                }
                                //MessageBox.Show("pa.len="+pa.Length.ToString());
                            }
                        }
                    }
                    catch
                    {
                    }
                }
                pr.Close();
            }
            li = "";
            
            colinea = fr1.colinea;// los valores de colores de estas variables se definen en el formulario 1.
            colfondo = fr1.colfondo;
            colotr1 = fr1.colotr1;
            colP = fr1.colP;
            colS = fr1.colS;
            colC = fr1.colC;
            panel1.BackColor = colfondo;// este es el panel principal, donde se despligan las trazas.
            panelEstHp71.Location = new Point(1,40);// panel en el que se hace la seleccion de las estaciones que se quieren visualizar o no.
            labelsis.Text = fr1.sismo.Substring(0,12);
            if (fr1.sismo[9] == 'X')
            {
                if (incT > 0)
                {
                    ite = incT;
                    inite = itT;
                }
                else ite = 1.5F;
            }
            else
            {
                if (incV > 0)
                {
                    ite = incV;
                    inite = itV;
                }
                else ite = 0.5F;
            }
            labelsis.Location = new Point(Width - 100, Height - 65);
            if (File.Exists("reviza.txt")) File.Delete("reviza.txt");
            if (File.Exists("locabas.txt")) File.Delete("locabas.txt");
            //for (i = 0; i < Ma; i++)
            for (i = 0; i < fr1.nutra; i++)
            {
                fas[i] = new double[3];
                fas[i][0]=0;
                fas[i][1]=0;
                fas[i][2]=0;
                teo[i] = new double[2];
                teo[i][0] = 0;
                teo[i][1] = 0;
                pes[i] = new short[2];
                pes[i][0] = 4;
                pes[i][1] = 4;
                pol[i] = ' ';
                impu[i] = 'I';
            }
            tminx = 0;
            durx = 0;
            tm = 0;
            tx = 0;
            vol = fr1.vol;
            for (i = 0; i <= fr1.nuvol; i++)
            {
                if (fr1.volcan[i][0] == fr1.sismo[9])
                {
                    vol = (short)(i);
                    break;
                }
            }
            clas = fr1.clas;
            for (i = 0; i <= fr1.nuvol; i++)
            {
                bvol[i] = new Button();
                bvol[i].Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
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
            for (i = 0; i < fr1.totestaloc; i++)
            {
                if (j < fr1.nuestaloc[i]) j = fr1.nuestaloc[i];
                if (j >= 16) break;
            }
            for (i = 0; i < j; i++)
            {
                best[i] = new Button();
                best[i].Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
                best[i].Location = new Point(panel1.Size.Width+2,i*22+1);
                best[i].Size = new Size(30, 22);
                best[i].Text = (i + 1).ToString();
                best[i].TabIndex = i+1;
                best[i].BackColor = Color.Peru;
                best[i].Font = new Font("Microsoft Sans Serif", 7);
                this.best[i].Click += new System.EventHandler(this.best_Click);
                this.Controls.Add(best[i]);
            }

            for (i = 0; i < 6; i++)
            {
                bpes[i] = new Button();
                bpes[i].Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
                bpes[i].Location = new Point(panel1.Size.Width + 3, i * 22 + 500);
                bpes[i].Size = new Size(25, 22);
                if (i < 5) bpes[i].Text = i.ToString();
                else bpes[i].Text = (i + 4).ToString();
                bpes[i].TabIndex = i;
                if (i == peso) bpes[i].BackColor = Color.MediumPurple;
                else bpes[i].BackColor = Color.White;
                bpes[i].Font = new Font("Microsoft Sans Serif", 7);
                this.bpes[i].Click += new System.EventHandler(this.bpes_Click);
                this.Controls.Add(bpes[i]);
            }

            nutra = 0;
            for (i = 0; i < fr1.nutra; i++)
            {
                if (fr1.siEst[i] == true)
                {
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
                    lari = nmf - nmi;
                    lar[nutra] = lari;
                    cu[nutra] = new int[lari];
                    tim[nutra] = new double[lari];
                    ra[nutra] = fr1.ra[i];
                    ga[nutra] = fr1.ga[i];
                    by[nutra] = fr1.by[i];
                    est[nutra] = fr1.est[i];
                    comp[nutra] = fr1.comp[i];
                    tar[nutra] = fr1.tar[i];
                    for (j = nmi; j < nmf; j++)
                    {
                        cu[nutra][j-nmi]=fr1.cu[i][j];
                        tim[nutra][j-nmi]=fr1.tim[i][j];
                    }
                    nutra += 1;
                }
            }
            for (i = 0; i < nutra; i++) siEst[i] = true;
            try
            {
                for (i = 0; i < nutra; i++)
                {
                    mx[i] = cu[i][0]; // ojo aqui a veces sale indice por fuera !!!
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
                    if (tm > tim[i][0]) tm = tim[i][0]; // aveces indices fuera de la matrix
                    if (tx < tim[i][lar[i] - 1]) tx = tim[i][lar[i] - 1];
                }
                tminx = tm;
                tmini = tminx;
                durx = tx - tm;
                dura = durx;
                for (i = 0; i < nutra; i++) pro[i] = (int)((mx[i] + mn[i]) / 2.0F);
                sipro = 0;
                siH = new bool[nutra];
                for (i = 0; i < nutra; i++) siH[i] = false;
                LeerInp();

                if (!Directory.Exists(".\\h")) Directory.CreateDirectory(".\\h");
                ca = fr1.rutbas + "\\h";
                DirectoryInfo dir = new DirectoryInfo(ca);
                FileInfo[] fcc = dir.GetFiles("?.mod");
                foreach (FileInfo f in fcc)
                {
                    ca2 = ca + "\\" + f.Name;
                    ca3 = ".\\h\\" + f.Name;
                    File.Copy(ca2, ca3, true);
                }

                /*lin = "/C copy " + fr1.rutbas + "\\h\\*.mod .\\h";
                util.Dos(lin,true);*/
                //load = true;
            }
            catch
            {
            }

            if (File.Exists("c:\\octave\\bin\\octave.exe") && File.Exists(".\\oct\\estaML.txt"))
            {
                li = "";
                numl = 0;
                StreamReader ml1 = new StreamReader(".\\oct\\estaML.txt");
                while (li != null)
                {
                    try
                    {
                        li = ml1.ReadLine();
                        if (li == null || li[0]=='*') break;
                        if (char.IsDigit(li[0]))
                        {
                            numl += 1;
                            if (numl >= Max) break;
                        }
                        else
                        {
                            numl = 0;
                            break;
                        }
                    }
                    catch
                    {
                    }
                }
                ml1.Close();
                if (numl == 0)
                {
                    oct = false;
                    sioct = false;
                }
                else
                {
                    ordml = new short[numl];
                    estml = new string[numl];
                    iniml = new int[numl];
                    finml = new int[numl];
                    factml = new string[numl];
                    compml = new char[numl];
                    laestml = new double[numl];
                    loestml = new double[numl];
                    i=(int)(numl/3.0);
                    siboml = new bool[i];

                    li = "";
                    i = 0;
                    StreamReader ml2 = new StreamReader(".\\oct\\estaML.txt");
                    while (li != null)
                    {
                        try
                        {
                            li = ml2.ReadLine();
                            if (li==null || li[0]=='*') break;
                            if (char.IsDigit(li[0]))
                            {
                                pa = li.Split(delim);
                                if (pa.Length >= 9)
                                {
                                    ordml[i] = short.Parse(pa[0]);
                                    estml[i] = pa[1]+" "+pa[2];
                                    factml[i] = pa[8];
                                    compml[i] = pa[3][0];
                                    if (pa.Length >= 12)
                                    {
                                        iniml[i] = int.Parse(pa[10]);
                                        finml[i] = int.Parse(pa[11]);
                                    }
                                    try
                                    {
                                        laestml[i] = double.Parse(pa[4]) + double.Parse(pa[5]) / 60.0;
                                        loestml[i++] = double.Parse(pa[6]) + double.Parse(pa[7]) / 60.0;
                                    }
                                    catch
                                    {
                                        MessageBox.Show("ERROR en .\\oct\\estaML.txt !!!!\n\nEs MUY Posible que las coordenadas de las estaciones\ntengan las letras de la latitud o longitud\n\nRecuerde que deben usarse numeros negativos\ntanto en los grados, como en los minutos, si la latitud\n es N o la longitud es W (LETRAS NO!!)\n\nRevise y corrija!!");
                                        ml2.Close();
                                        Close();
                                        return;
                                    }
                                    if (i >= Max) break;
                                }
                            }
                        }
                        catch
                        {
                        }
                    }
                    ml2.Close();
                    oct = true;
                    sioct = true;
                    j = ordml[numl-1]+1;
                    ml = new double[j];
                    ii = (ordml[i-1]+1)*3;
                    if (ii!=i)
                    {
                        MessageBox.Show("Ojo: Es posible que los números secuenciales del archivo estaML.txt esten erroneos!!, REVISE!!.\nMuy probablemente el programa se vuelva inestable y PIERDA SU TRABAJO.\n\n MEJOR REVISE POR FAVOR!!!");
                    }
                    for (i = 0; i < j; i++) ml[i]=-10.0;
                    if (ordml[numl - 1] > 20) kk = 30;
                    else kk = 40;                    
                    if (ordml[numl - 1] <= 39)
                    {
                        panelML.Size = new Size(100 + kk * (ordml[numl - 1]), 130);
                        tam = 7;
                    }
                    else
                    {
                        panelML.Size = new Size(100 + kk * 39, 130 * (int)(1 + ordml[numl - 1] / 39.0));
                        tam = 6;
                    }
                    iii = 0;
                    jjj = 0;
                    for (i = 0; i <= ordml[numl - 1]; i++)
                    {
                        caj[i] = new CheckBox();
                        this.caj[i].Anchor = ((AnchorStyles)((AnchorStyles.Top | AnchorStyles.Left)));
                        this.caj[i].AutoSize = true;
                        this.caj[i].CheckAlign = System.Drawing.ContentAlignment.BottomLeft;
                        this.caj[i].Location = new System.Drawing.Point(10 + iii * kk, jjj + 50);
                        this.caj[i].Size = new System.Drawing.Size(33, 17);
                        this.caj[i].TabIndex = i;
                        this.caj[i].UseVisualStyleBackColor = true;
                        this.caj[i].CheckedChanged += new System.EventHandler(this.caj_CheckedChanged);
                        this.panelML.Controls.Add(caj[i]);
                        labV[i] = new Label();
                        this.labV[i].Anchor = ((AnchorStyles)((AnchorStyles.Top | AnchorStyles.Right)));
                        this.labV[i].AutoSize = true;
                        this.labV[i].Location = new System.Drawing.Point(5 + iii * kk, jjj + 1);
                        this.labV[i].Size = new System.Drawing.Size(40, 17);
                        this.labV[i].TabIndex = i;
                        this.labV[i].Text = estml[i * 3].Substring(0, 1);
                        this.panelML.Controls.Add(labV[i]);
                        lab[i] = new Label();
                        this.lab[i].Anchor = ((AnchorStyles)((AnchorStyles.Top | AnchorStyles.Right)));
                        this.lab[i].AutoSize = true;
                        this.lab[i].Location = new System.Drawing.Point(1 + iii * kk, jjj + 15);
                        this.lab[i].Size = new System.Drawing.Size(40, 17);
                        this.lab[i].TabIndex = i;
                        this.lab[i].Text = estml[i * 3].Substring(2, 3);
                        this.panelML.Controls.Add(lab[i]);
                        labml[i] = new Label();
                        this.labml[i].Anchor = ((AnchorStyles)((AnchorStyles.Top | AnchorStyles.Right)));
                        this.labml[i].AutoSize = true;
                        this.labml[i].Location = new System.Drawing.Point(1 + iii * kk, jjj + 32);
                        this.labml[i].Size = new System.Drawing.Size(40, 17);
                        this.labml[i].TabIndex = i;
                        this.panelML.Controls.Add(labml[i]);

                        labml2[i] = new Label();
                        this.labml2[i].Anchor = ((AnchorStyles)((AnchorStyles.Top | AnchorStyles.Right)));
                        this.labml2[i].AutoSize = true;
                        this.labml2[i].Location = new System.Drawing.Point(iii * kk, jjj + 100);
                        this.labml2[i].Size = new System.Drawing.Size(44, 37);
                        this.labml2[i].TabIndex = i;
                        s = iniml[i * 3].ToString();
                        //MessageBox.Show("i=" + i.ToString() + " fin=" + finml[i].ToString() + " " + estml[i * 3]);
                        if (iniml[i * 3] > -1 && finml[i * 3] > -1)
                        {
                            if (iniml[i * 3] > 0)
                            {
                                ca = s.Substring(2, 2) + "/" + s.Substring(4, 2) + "/" + s.Substring(6, 2);
                                if (finml[i * 3] == 0) ca += "\nActual";
                                else if (finml[i * 3] > 0)
                                {
                                    s = finml[i * 3].ToString();
                                    ca += "\n" + s.Substring(2, 2) + "/" + s.Substring(4, 2) + "/" + s.Substring(6, 2);
                                }
                            }
                            else ca = "******";
                        }
                        else ca = "";
                        this.labml2[i].Text = ca;
                        //this.labml2[i].Font = new Font("Microsoft Sans Serif",6,FontStyle.Bold);
                        this.labml2[i].Font = new Font("Times New Roman",tam, FontStyle.Bold);
                        this.panelML.Controls.Add(labml2[i]);


                        boml[i] = new Button();
                        boml[i].Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
                        boml[i].Location = new Point(1 + iii * kk, jjj + 75);
                        boml[i].Size = new Size(30, 20);
                        boml[i].Text = "Tra";
                        boml[i].TabIndex = i;
                        boml[i].BackColor = Color.Peru;
                        boml[i].Font = new Font("Microsoft Sans Serif", 7);
                        this.boml[i].Click += new System.EventHandler(this.boml_Click);
                        this.panelML.Controls.Add(boml[i]);

                        iii += 1;
                        if (iii >= 39)
                        {
                            iii = 0;
                            jjj += 125;
                        }
                    }
                    i = panelML.Size.Width - 70;
                    labelMltot.Location = new Point(i, 55);
                }// else
            }
            else
            {
                oct = false;
                sioct = false;
            }
            if (File.Exists(".\\oct\\MLtodas.txt")) File.Delete(".\\oct\\MLtodas.txt");
            if (!File.Exists(".\\h\\9.mod") || fr1.ClAux.Substring(0, 2) == "??") fr1.ModAux = false;           
            if (fr1.ModAux == true && fr1.sismo.Substring(10, 2) == fr1.ClAux.Substring(0, 2))
            {
                label2.Text = "9.mod";
                bvol[vol].BackColor = Color.LimeGreen;
            }
            else label2.Text = fr1.volcan[vol][0] + ".mod";       

            panelmen.Size = new Size(150, 100);
            panelFocmec.Size = new Size(640, 480);
            boFocmec.BackColor = Color.Wheat;
            if (Height < 800)
            {
                panelpun.Size = new Size(panelpun.Width, (int)(Height / 2.5));
                panelmapa.Location = new Point(1, panelpun.Height + 20);
                panelmapa.Size = new Size(panelmapa.Width, (int)(Height / 2.5));
                espaciado = 12;
            }
            if (panelmapa.Size.Height + panelmapa.Location.Y > Size.Height)
            {
                i = panelpun.Location.X + panelpun.Size.Width + 10;
                panelmapa.Location = new Point(i, 10);
            }

            panelRotaRT.Size = new Size(Width - 370, Height - 150);
            panelRotaRT.Location = new Point(Width - (panelRotaRT.Width + 20), Height - (panelRotaRT.Height + 100));

            if (File.Exists(".\\oct\\ML.txt"))
            {
                StreamReader oo = new StreamReader(".\\oct\\ML.txt");
                li = oo.ReadLine();
                if (li[0] == '%')
                {
                    cBMLZ.Visible = true;
                    cBMLN.Visible = true;
                    cBMLE.Visible = true;
                    rdML1.Visible = true;
                    rdML2.Visible = true;
                    rdML3.Visible = true;
                }
                oo.Close();
            }

            load = true;
            panel1.Invalidate();
            return;
        }

        private void bvol_Click(object sender, EventArgs e)
        {
            short i,j;

            i = vol;
            Button bt = (Button)sender;
            vol=(short)(bt.TabIndex);
            siH = new bool[nutra];
            for (j = 0; j < nutra; j++) siH[j] = false;
            LeerInp();
            if (i == vol && fr1.ModAux == true && fr1.sismo.Substring(10, 2) == fr1.ClAux.Substring(0, 2))
            {
                vol_viejo = -1;
                label2.Text = "9.mod";
                bvol[vol].BackColor = Color.LimeGreen;
                return;
            }
            for (i = 0; i <= fr1.nuvol; i++)
            {
                if (i==vol)  bvol[i].BackColor = Color.Yellow;
                else bvol[i].BackColor = Color.LightYellow;
            }
            vol_viejo = vol;
            return;
        }

        private void boparam_Click(object sender, EventArgs e)
        {
            diag1frm2 di = new diag1frm2();

            di.Inite = inite.ToString();
            di.Interite = ite.ToString();
            if (di.ShowDialog() == DialogResult.OK)
            {
                inite = float.Parse(di.Inite);
                ite = float.Parse(di.Interite);
            }

            return;
        }

        private void best_Click(object sender, EventArgs e)
        {
            short i, j, k;

            if (fr1.totestaloc <= 0) return;
            Button bt = (Button)sender;

            for (j = 0; j < nutra; j++)
            {
                if (string.Compare(est[j].Substring(0, 4), "IRIG") == 0) siEst[j] = true;
                else siEst[j] = false;
            }
            for (i = 0; i < fr1.totestaloc; i++)
            {
                for (j = 0; j < nutra; j++)
                {
                    if (string.Compare(fr1.estaloc[i].Substring(0, 4), est[j].Substring(0, 4)) == 0)
                    {
                        if (fr1.nuestaloc[i] == bt.TabIndex) siEst[j] = true;
                        break;
                    }
                }
            }
            /*for (k = 0; k < fr1.totvolestaloc; k++)
            {
                for (i = 0; i < fr1.nuvol; i++)
                {
                    //MessageBox.Show("bt="+(bt.TabIndex-1).ToString()+"i="+i.ToString()+" totesloc="+fr1.totvolestaloc.ToString()+" nuv="+fr1.nuvol.ToString());
                    if (string.Compare(fr1.volestaloc[bt.TabIndex - 1].Substring(0, 4), fr1.volcan[i].Substring(0, 4)) == 0)
                    {
                        vol = i;
                        break;
                    }
                }
            }
            for (j = 0; j < fr1.nuvol; j++)
            {
                if (j == vol) bvol[j].BackColor = Color.Yellow;
                else bvol[j].BackColor = Color.LightYellow;
            }*/


            //Dibujo();
            if (sifilt == false) Dibujo(cu, mx, mn);
            else if (sifilt == true) Dibujo(cf, mxF, mnF);
            MarcasTi(panel1);
            if (panelEstHp71.Visible==true) util.EscribePanelEstaHP(panelEstHp71,nutra,est,siEst,pol);

            k = -1;
            for (i = 0; i < nutra; i++)
            {
                if (siEst[i] == false)
                {
                    k = 1;
                    break;
                }
            }
            if (k == 1) botodas.BackColor = Color.PaleVioletRed;
            else botodas.BackColor = Color.White;
            if (panelWada.Visible == true) panelWada.Invalidate();

            return;
        }

        private void bpes_Click(object sender, EventArgs e)
        {

            Button bt = (Button)sender;
           // MessageBox.Show("bt=" + bt.Text + " tabindex=" + bt.TabIndex.ToString());
            peso = (short)(bt.TabIndex);
            if (peso > 4) peso = 9;
            MenuPeso();

            return;
        }

        private void caj_CheckedChanged(object sender, EventArgs e)
        {
            short i;

            CheckBox chb = (CheckBox)sender;
            i = (short)(chb.TabIndex);
            boml[i].BackColor = Color.Peru;
            if (caj[chb.TabIndex].Checked == true && panelML.Visible == true)
            {

                //Calculo_ML(i);
                Calculo_ML();
                ML_total();
            }
            else
            {
                ml[i] = -10.0;
                labml[i].Text = string.Format("{0:0.0}", ml[i]);
                ML_total();
            }
            // MessageBox.Show(" ind="+chb.TabIndex.ToString()+" bool="+caj[chb.TabIndex].Checked.ToString());

            return;
        }

        private void boml_Click(object sender, EventArgs e)
        {
            int j;

            timlini = 0;
            Button bt = (Button)sender;
            j = (short)(bt.TabIndex);
            LeeMlAmp(j * 3);

            return;
        }

        void MenuPeso()
        {
            short i;

            for (i = 0; i < 6; i++)
            {
                if (bpes[i].Text == peso.ToString()) bpes[i].BackColor = Color.MediumPurple;
                else bpes[i].BackColor = Color.White;
            }

            return;
        }

        void Dibujo(int[][] val, int[] mxx, int[] mnn)
        {
            int xf, yf, i, j, jj, k, kk, tamlet, numtotra;
            int nmi, nmf, numu, proo;
            float x1, y1, iniy, ff, aabs = 1.0F, maa = 0;
            double fax, fay, fy, diff;
            Color col;
            Point[] dat;

            panel1.BackColor = colfondo;
            util.borra(panel1, colfondo);
            xf = panel1.Size.Width - 40;
            ff = (float)(xf);
            yf = panel1.Size.Height;
            numtotra = 0;
            numu = 0;
            for (i = 0; i < nutra; i++) if (siEst[i] == true) numtotra += 1;
            //MessageBox.Show("numtotra=" + numtotra.ToString());
            if (numtotra <= 0) return;
            fax = xf / dura;
            fay = yf / (double)(numtotra);
            tamlet = (int)(fay);
            if (tamlet > 10) tamlet = 10;

            Graphics dc = panel1.CreateGraphics();
            Pen lapiz = new Pen(colinea, 1);
            Pen lapiz2 = new Pen(Color.DarkOrange, 1);
            lapiz2.DashStyle = DashStyle.DashDot;

            if (abso == true)
            {
                j = -1;
                for (i = 0; i < nutra; i++)
                {
                    if (siEst[i] == true)
                    {
                        j = i;
                        break;
                    }
                }
                if (j > -1)
                {
                    maa = mxx[j] - mnn[j];
                    for (i = j + 1; i < nutra; i++) if (siEst[i] == true) if (maa < mxx[i] - mnn[i]) maa = mxx[i] - mnn[i];
                }
            }

            jj = 0;
            try
            {
                for (j = 0; j < nutra; j++)
                {
                    if (siEst[j] == true)
                    {
                        if (sifilt == false) proo = pro[j];
                        else proo = proF[j];
                        if (mxx[j] - proo != 0) fy = ((fay/2.0)/((mxx[j]-proo)));
                        else fy = 1.0;
                        iniy = (float)(jj * fay + fay / 2.0);
                        dc.DrawLine(lapiz2, 40.0F, iniy, ff, iniy);
                        if (siH[j] == true) col = colotr1;
                        else col = Color.Red;
                        SolidBrush broH = new SolidBrush(col);
                        dc.DrawString(est[j].Substring(0, 4), new Font("Times New Roman", tamlet, FontStyle.Bold), broH, 1, (float)(iniy - tamlet));
                        broH.Dispose();
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
                        if (sifilt == true && nmi < M) nmi = M;
                        nmf = (int)(((tmini + dura) - tim[j][0]) * ra[j]);
                        if (nmf > lar[j]) nmf = lar[j];
                        if (sifilt == true && nmf > lar[j] - M) nmf = lar[j] - M;
                        numu = nmf - nmi;
                        dat = new Point[numu];
                        if (abso == false) aabs = 1.0F;
                        else aabs = (mxx[j] - mnn[j]) / maa;

                        if (fr1.invertido[j] == false)
                        {
                            for (k = nmi; k < nmf; k++)
                            {
                                if (kk >= lar[j]) break;
                                diff = ((val[j][k] - proo) * aabs) * fy;
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
                                diff = ((proo - val[j][k]) * aabs) * fy;
                                y1 = (float)(iniy - amp * diff);
                                x1 = (float)(40.0 + (tim[j][k] - tmini) * fax);
                                dat[kk].Y = (int)y1;
                                dat[kk].X = (int)x1;
                                kk += 1;
                            }
                        }
                        dc.DrawLines(lapiz, dat);
                        jj += 1;
                    }// if siest.....
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

            util.borra(panel2,Color.DimGray);
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
                //pro = (int)(pro * amp);
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
                        dif = pro-(int)(cu[refe][k]);
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
            float x1,x2,ww;
            double fax;

            xf = panel2.Size.Width - 40;
            yf = panel2.Size.Height;
            fax = xf / durx;
            x1 = (float)(40.0 + (tmini - tminx) * fax);
            x2 = (float)(40.0 + ((tmini+dura) - tminx) * fax);
            ww=x2-x1;
        //    MessageBox.Show("x1="+x1.ToString()+" ww="+ww.ToString());
            Graphics dc = panel2.CreateGraphics();
            SolidBrush brocha = new SolidBrush(Color.LightBlue);
            dc.FillRectangle(brocha, x1, 0.0F, ww, 3.0F);
            brocha.Dispose();

            return;
        }

        private void panel2_MouseDown(object sender, MouseEventArgs e)
        {
            int xf, yf;
            double fax;

            if (e.X < 40 || dura==durx) return;
            xf = panel2.Size.Width - 40;
            yf = panel2.Size.Height;
            fax = xf / durx;
            tmini=tminx+((e.X-40)/fax);
            panel1.Invalidate();

            return;
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            if (load == false) return;
            if (sifilt == false) Dibujo(cu, mx, mn);
            else if (sifilt == true) Dibujo(cf, mxF, mnF);
            //Dibujo();
            DibujoRefe();
            if (tmini != tminx) BarraGuia();
            MarcasTi(panel1);
            if (mejite > -1)
            {
                if (File.Exists(".\\h\\buz.exe") && File.Exists("EGAVGA.BGI")) boBUZ.Visible = true;
                else
                {
                    if (File.Exists(".\\h\\buz.exe") && !File.Exists("EGAVGA.BGI") && File.Exists(".\\h\\EGAVGA.BGI"))
                    {
                        File.Copy(".\\h\\EGAVGA.BGI", ".\\EGAVGA.BGI");
                        boBUZ.Visible = true;
                    }
                    else boBUZ.Visible = false;
                }
                if (File.Exists(".\\h\\focmec.exe")) boFocmec.Visible = true;
                else boFocmec.Visible = false;
            }
            else
            {
                boBUZ.Visible = false;
                boFocmec.Visible = false;
            }
            if (panelWada.Visible == true) Wadati();

            return;
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            int i=-1,ii,j,jj,nues;
            float fay;

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
                fay = panel1.Size.Height / nues;
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
                if (e.Button == MouseButtons.Right)
                {                                        
                    for (j = 0; j < nutra; j++)
                    {
                        if (j == i) siEst[j] = true;
                        else siEst[j] = false;
                        panel1.Invalidate();
                    }
                    panelEstHp71.Visible = true;
                    i = util.EscribePanelEstaHP(panelEstHp71, nutra, est, siEst, pol);
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
            ushort nues, ii, jj, j, i, cont;
            int xf, yf, bx2, by2, nm1, nm2, tot, min, max, k;
            int mmx, mmn, id, dfcu, i1, j1, nuca;
            long tii1 = 0;
            double dif = 0, ti1, ti2, fax, fay, ttt, dd;
            string ss = "";

            if (e.X < 40) return;
            xf = panel1.Size.Width-40;
            yf = panel1.Size.Height;
            if (guia == true)
            {
                Graphics dc = panel1.CreateGraphics();
                Pen lapiz = new Pen(Color.Red, 1);
                dc.DrawLine(lapiz, e.X, 1, e.X, yf);
                lapiz.Dispose();
                guia = false;
                boGuia.BackColor = Color.White;
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

            if (fasauto == true)
            {
                Fases_Automaticas(ti2);
                fasauto = false;
                boFasAuto.BackColor = Color.Linen;
                return;
            }

            nuca = 0;
            for (i = 0; i < nutra; i++) if (siEst[i] == true) nuca += 1;
            fay = (double)(yf) / (double)(nuca);
            //fayy = (double)(panel1.Size.Height) / (double)(nuca); 
            j1 = 0;
            id = -1;
            k = (int)(e.Y / fay);
            for (i = 0; i < nutra; i++)
            {
                if (siEst[i] == true)
                {
                    if (j1 == k)
                    {
                        id = i;
                        break;
                    }
                    j1 += 1;
                }
            }
          /*  if (integrar == true)
            {
                tizi = ti1;
                tizf = ti2;
                tiniz = ti1;
                tifinz = ti2;
                Integracion((short)(id));
                return;
            }*/
            //else if (rota == true)
            if (rota == true)
            {
                if ((ti2 - ti1) < 5) return;
                ss = est[id].Substring(0, 3);
                ii = 0;
                idestrot[2] = -1;
                nmrot[0] = new int[2];
                nmrot[1] = new int[2];
                nmrot[2] = new int[2];
                for (i = 0; i < nutra; i++)
                {
                    if (est[i].Substring(0, 3) == ss)
                    {
                        if (est[i][3] == 'Z' || est[i][3] == 'N' || est[i][3] == 'E')
                        {
                            if (est[i][3] == 'Z')
                            {
                                jj = 0;
                                nmrot[0][0] = (int)((ti1 - tim[i][0]) * ra[i]);
                                nmrot[0][1] = (int)((ti2 - tim[i][0]) * ra[i]);
                            }
                            else if (est[i][3] == 'N')
                            {
                                jj = 1;
                                nmrot[1][0] = (int)((ti1 - tim[i][0]) * ra[i]);
                                nmrot[1][1] = (int)((ti2 - tim[i][0]) * ra[i]);
                            }
                            else
                            {
                                jj = 2;
                                nmrot[2][0] = (int)((ti1 - tim[i][0]) * ra[i]);
                                nmrot[2][1] = (int)((ti2 - tim[i][0]) * ra[i]);
                            }
                            estrot[jj] = est[i].Substring(0, 4);
                            idestrot[jj] = (short)(i);
                            ii += 1;
                            if (ii >= 3) break;
                        }
                    }
                }
                if (ii == 3)
                {
                    panelRotaRT.Visible = true;
                    tiniRot = ti1;
                    lafR = lasi;
                    lofR = losi;
                    panelRotaRT.Invalidate();
                    panelmapa.Visible = true;
                    util.Topo(panelmapa, fr1.volcan[vol], facm, fr1.latvol[vol], fr1.lonvol[vol], lafR, lofR, Color.Gray,
                        false, 0, 0, 0, 0);
                }
                return;
            }

            if (sipro == 1)
            {
                if (id < 0)
                {
                    sipro = 0;
                    boprom.BackColor = Color.White;
                    return;
                }
                tot = tim[id].Length;
                for (k = 0; k < tot; k++)
                {
                    if (tim[id][k] >= ti1) break;
                }
                nm1 = k;
                for (k = 0; k < tot; k++)
                {
                    if (tim[id][k] > ti2 || tim[id][k] <= 0) break;
                }
                nm2 = k - 1;
                if (sifilt == false)
                {
                    max = cu[id][nm1];
                    min = max;
                    for (k = nm1 + 1; k < nm2; k++)
                    {
                        if (max < cu[id][k]) max = cu[id][k];
                        else if (min > cu[id][k]) min = cu[id][k];
                    }
                    pro[id] = (int)((max + min) / 2.0F);
                }
                else
                {
                    max = cf[id][nm1];
                    min = max;
                    for (k = nm1 + 1; k < nm2; k++)
                    {
                        if (max < cf[id][k]) max = cf[id][k];
                        else if (min > cf[id][k]) min = cf[id][k];
                    }
                    proF[id] = (int)((max + min) / 2.0F);
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

                    if (id > -1)
                    {
                        i1 = (int)((ti1 - tim[id][0]) * ra[id]);
                        if (i < 0) i = 0;
                        j1 = (int)((ti2 - tim[id][0]) * ra[id]);
                        if (j1 > lar[id]) j1 = lar[id];
                        if (j1 > 0 && j1 > i1)
                        {
                            mmx = cu[id][i1];
                            mmn = mmx;
                            for (k = i1 + 1; k < j1; k++)
                            {
                                if (mmx < cu[id][k]) mmx = cu[id][k];
                                else if (mmn > cu[id][k]) mmn = cu[id][k];
                            }
                            dfcu = mmx - mmn;
                            ss += " " + dfcu.ToString() + " cue";
                            ss += "  ga:" + ga[id].ToString();
                        }
                    }
                }

                tip.IsBalloon = false;
                tip.ReshowDelay = 0;
                tip.Show(ss, panel1, e.X, e.Y - 15, 3000);
            }
            else
            {
                if (bx1 == bx2 || ti2 - ti1 < 0.5)
                {
                    //yahypo = false;
                    nues = 0;
                    for (j = 0; j < nutra; j++) if (siEst[j] == true) nues += 1;
                    //fay = panel1.Size.Height / nues;
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
                    if (quita == false)
                    {
                        if (ond > 0 && (fas[i][0] == 0 || ti1 < fas[i][0]))
                        {
                            if (fas[i][0] == 0) MessageBox.Show("No se ha leido la P !!!");
                            else if (ti1 < fas[i][0]) MessageBox.Show("Lectura MENOR que P ???");
                        }
                        else
                        {
                            if (ond == 0)
                            {
                                dd = ttt - (double)(ii);
                                if (pola == true)
                                {
                                    if (dd < 0.5) pol[i] = 'C';
                                    else pol[i] = 'D';
                                }
                                else
                                {
                                    pol[i] = ' ';
                                    pola = true;
                                    boPola.BackColor = Color.GreenYellow;
                                }

                                pol2[i] = pol[i];
                                impu[i] = imp;
                                if (imp == 'E')
                                {
                                    imp = 'I';
                                    boImp.Text = "I";
                                }
                                if (panelEstHp71.Visible == true) panelEstHp71.Invalidate();
                            }
                            MenuPeso();
                            fas[i][ond] = ti1;
                            if (ond < 2)
                            {
                                pes[i][ond] = peso;
                                if (panelWada.Visible == true) panelWada.Invalidate();
                            }
                            if (siH[i] == true) ss = est[i].Substring(0, 4) + " " + pol[i].ToString();
                            else ss = "NO EXISTE en la cabecera del Modelo!!";
                            tip.IsBalloon = false;
                            tip.ReshowDelay = 0;
                            tip.Show(ss, panel1, e.X + 5, e.Y - 15, 2000);
                            if (ond == 0)
                            {
                                peso = 0;
                            }
                            else if (ond == 1) peso = 2;
                        }
                    }
                    else
                    {
                        fas[i][ond] = 0;
                        if (ond < 2) pes[i][ond] = 4;
                        if (ond == 0)
                        {
                            fas[i][1] = 0;
                            fas[i][2] = 0;
                            pes[i][1] = 4;
                            pol[i] = ' ';
                        }
                        quita = false;
                        boQuita.BackColor = Color.White;
                    }
                    MarcasTi(panel1);
                    cont = 0;
                    for (k = 0; k < nutra; k++)
                    {
                        if (fas[k][0] > 0) cont += 1;
                        if (fas[k][1] > 0) cont += 1;
                        if (cont >= 3) break;
                    }
                    if (cont >= 3)
                    {
                        boHp71.Visible = true;
                        if (hyp == true) CorreHypo();
                    }
                    else boHp71.Visible = false;
                }
                else
                {
                    tmini = ti1;
                    dura = ti2 - ti1;
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
            CuentaS();
            if (panelPola.Visible == true) panelPola.Invalidate();

            return;
        }

        /*
        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            ushort nues,ii,jj,cont;
            int i,j,xf,yf,bx2,by2,nm1,nm2,tot,min,max,k,dfcu,idd,mmx,mmn;
            long tii1 = 0;
            double dif=0,ti1,ti2,fax,fay,ttt,dd;
            string ss = "";

            if (e.X < 40) return;
            xf = panel1.Size.Width - 40;
            yf = panel1.Size.Height;
            if (guia == true)
            {
                Graphics dc = panel1.CreateGraphics();
                Pen lapiz = new Pen(Color.Red,1);
                dc.DrawLine(lapiz, e.X, 1, e.X, yf);
                lapiz.Dispose();
                guia = false;
                boGuia.BackColor = Color.White;
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


            if (fasauto == true)
            {
                Fases_Automaticas(ti2);
                fasauto = false;
                boFasAuto.BackColor = Color.Linen;
                return;
            }           
            nues = 0;
            for (j = 0; j < nutra; j++) if (siEst[j] == true) nues += 1;
            fay = panel1.Size.Height / nues;
            ttt = e.Y / fay;
            ii = (ushort)(ttt);
            jj = 0;
            idd = -1;
            for (j = 0; j < nutra; j++)
            {
                if (siEst[j] == true)
                {
                    if (jj == ii)
                    {
                        idd = j;
                        break;
                    }
                    jj += 1;
                }
            }

            if (rota == true)
            {                
                ss = est[idd].Substring(0, 3);
                ii = 0;
                idestrot[2] = -1;
                nmrot[0] = new int[2];
                nmrot[1] = new int[2];
                nmrot[2] = new int[2];
                for (i = 0; i < nutra; i++)
                {
                    if (est[i].Substring(0, 3) == ss)
                    {
                        if (est[i][3] == 'Z' || est[i][3] == 'N' || est[i][3] == 'E')
                        {
                            if (est[i][3] == 'Z')
                            {
                                jj = 0;
                                nmrot[0][0] = (int)((ti1 - tim[i][0]) * ra[i]);
                                nmrot[0][1] = (int)((ti2 - tim[i][0]) * ra[i]);
                            }
                            else if (est[i][3] == 'N')
                            {
                                jj = 1;
                                nmrot[1][0] = (int)((ti1 - tim[i][0]) * ra[i]);
                                nmrot[1][1] = (int)((ti2 - tim[i][0]) * ra[i]);
                            }
                            else
                            {
                                jj = 2;
                                nmrot[2][0] = (int)((ti1 - tim[i][0]) * ra[i]);
                                nmrot[2][1] = (int)((ti2 - tim[i][0]) * ra[i]);
                            }
                            estrot[jj] = est[i].Substring(0, 4);
                            idestrot[jj] = (short)(i);
                            ii += 1;
                            //MessageBox.Show("ii=" + (ii-1).ToString() + " " + estrot[ii - 1]);
                            if (ii >= 3) break;
                        }
                    }
                }

                if (ii == 3)
                {
                    panelRotaRT.Visible = true;
                    tiniRot = ti1;
                    //RadialTransversal();
                    lafR = lasi;
                    lofR = losi;
                    panelRotaRT.Invalidate();
                    panelmapa.Visible = true;
                    util.Topo(panelmapa,fr1.volcan[vol],facm,fr1.latvol[vol],fr1.lonvol[vol],lafR,lofR,Color.Gray,
                        false, 0, 0, 0, 0);
                }
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

                    if (sifilt == false)
                    {
                        max = cu[id][nm1];
                        min = max;
                        for (k = nm1 + 1; k < nm2; k++)
                        {
                            if (max < cu[id][k]) max = cu[id][k];
                            else if (min > cu[id][k]) min = cu[id][k];
                        }
                        pro[id] = (int)((max + min) / 2.0F);
                    }
                    else
                    {
                        max = cf[id][nm1];
                        min = max;
                        for (k = nm1 + 1; k < nm2; k++)
                        {
                            if (max < cf[id][k]) max = cf[id][k];
                            else if (min > cf[id][k]) min = cf[id][k];
                        }
                        proF[id] = (int)((max + min) / 2.0F);
                    }

                   // max = cu[j][nm1];
                   // min = max;
                   // for (k = nm1 + 1; k < nm2; k++)
                   // {
                   //     if (max < cu[j][k]) max = cu[j][k];
                   //     else if (min > cu[j][k]) min = cu[j][k];
                   // }
                   // pro[j] = (int)((max + min) / 2.0F);
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
                    i = (int)((ti1 - tim[idd][0]) * ra[idd]);
                    if (i < 0) i = 0;
                    j = (int)((ti2 - tim[idd][0]) * ra[idd]);
                    if (j > lar[idd]) j = lar[idd];
                    if (j > 0 && j > i)
                    {
                        mmx = cu[idd][i];
                        mmn = mmx;
                        for (k = i + 1; k < j; k++)
                        {
                            if (mmx < cu[idd][k]) mmx = cu[idd][k];
                            else if (mmn > cu[idd][k]) mmn = cu[idd][k];
                        }
                        dfcu = mmx - mmn;
                        ss += " " + dfcu.ToString() + " cue";                        
                        if (fr1.fcnan[idd] > 0)
                        {
                            dif = Math.Abs((fr1.fcnan[idd] * dfcu) / 1000.0);
                            ss += string.Format("   {0:0.00}  mc/s", dif);
                        }
                    }
                }
                tip.IsBalloon = false;
                tip.ReshowDelay = 0;
                tip.Show(ss, panel1, e.X, e.Y - 15, 3000);
            }
            else
            {
                if (bx1 == bx2 || ti2-ti1<1.0)
                {
                    if (quita == false)
                    {
                        if (ond > 0 && (fas[idd][0]==0 || ti1<fas[idd][0]))
                        {
                            if (fas[idd][0] == 0) MessageBox.Show("No se ha leido la P !!!");
                            else if (ti1 < fas[idd][0]) MessageBox.Show("Lectura MENOR que P ???");
                        }
                        else
                        {
                           // if (OnFontChanged == 0)
                            if (ond == 0)
                            {
                                try
                                {
                                    dd = ttt - (double)(ii);
                                    if (pola == true)
                                    {
                                        if (dd < 0.5) pol[idd] = 'C';
                                        else pol[idd] = 'D';
                                    }
                                    else
                                    {
                                        pol[idd] = ' ';
                                        pola = true;
                                        boPola.BackColor = Color.GreenYellow;
                                    }
                                    pol2[idd] = pol[idd];
                                    impu[idd] = imp;
                                    if (imp == 'E')
                                    {
                                        imp = 'I';
                                        boImp.Text = "I";
                                    }
                                }
                                catch
                                {
                                }
                            }
                            MenuPeso();
                            fas[idd][ond] = ti1;
                            if (ond < 2)
                            {
                                pes[idd][ond] = peso;
                                if (panelWada.Visible == true) panelWada.Invalidate();
                            }
                            //ss=est[idd].Substring(0,4)+" "+pol[idd].ToString();
                            if (siH[idd] == true) ss = est[idd].Substring(0,4)+" "+pol[idd].ToString();
                            else ss = "NO EXISTE en la cabecera del Modelo!!";
                            tip.IsBalloon = false;
                            tip.ReshowDelay = 0;
                            tip.Show(ss,panel1,e.X+5,e.Y-15,2000);
                            if (ond == 0) peso = 0;
                            else if (ond == 1) peso = 2;
                        }
                    }
                    else
                    {
                        fas[idd][ond] = 0;
                        if (ond<2) pes[idd][ond] = 4;
                        if (ond == 0)
                        {
                            fas[idd][1] = 0;
                            fas[idd][2] = 0;
                            pes[idd][1] = 4;
                            pol[idd] = ' ';
                        }
                        quita = false;
                        boQuita.BackColor = Color.White;
                    }
                    MarcasTi(panel1);
                    cont = 0;
                    for (k = 0; k < nutra; k++)
                    {
                        if (fas[k][0] > 0 ) cont += 1;
                        if (fas[k][1] > 0) cont += 1;
                        if (cont >= 3) break;
                    }
                    if (cont >= 3)
                    {
                        boHp71.Visible = true;
                        if (hyp == true) CorreHypo();
                    }
                    else boHp71.Visible = false;
                }
                else
                {
                    tmini = ti1;
                    dura = ti2 - ti1;
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
            CuentaS();
            if (panelPola.Visible == true) panelPola.Invalidate();
                        
            return;
        }

*/

        void CuentaS()
        {
            int i, j, k;

            i = 0; j = 0;
            for (k = 0; k < nutra; k++)
            {
                if (fas[k][0] > 0)
                {
                    j += 1;
                    if (fas[k][1] > 0)
                    {
                        i += 1;
                    }
                }
            }
            if (i >= 3) boWada.Visible = true;
            else if (i < 3)
            {
                boWada.Visible = false;
                if (panelWada.Visible == true) panelWada.Visible = false;
            }
            if (j >= 3)
            {
                boOrd.Visible = true;
                if (panelOrd.Visible == true) panelOrd.Invalidate();
            }
            else if (j < 3)
            {
                boOrd.Visible = false;
                if (panelOrd.Visible == true) panelOrd.Visible = false;
            }


            return;
        }


        private void botodas_Click(object sender, EventArgs e)
        {
            int i;

            for (i = 0; i < nutra; i++) siEst[i] = true;
            panel1.Invalidate();
            botodas.BackColor = Color.White;
            if (panelEstHp71.Visible==true) util.EscribePanelEstaHP(panelEstHp71,nutra,est,siEst,pol);
            if (panelWada.Visible == true) panelWada.Invalidate();

            return;
        }

        private void boesta_Click(object sender, EventArgs e)
        {
            if (panelEstHp71.Visible == false)
            {
                panelEstHp71.Visible = true;
                panelEstHp71.BringToFront();
               // MessageBox.Show("ii1");
                util.EscribePanelEstaHP(panelEstHp71, nutra, est, siEst, pol);
            }
            else panelEstHp71.Visible = false;
        }

        private void panelEstHp71_MouseDown(object sender, MouseEventArgs e)
        {
            int i, j, k, val;

            if (panelEstHp71.Visible == false) return;

            i = e.Y / 10;
            //j = e.X / 35;
            j = e.X / 60;
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
            j = -1;
            for (i = 0; i < nutra; i++)
            {
                if (siEst[i] == false)
                {
                    j = 0;
                    break;
                }
            }
            bomemo.BackColor = Color.White;
            if (j == 0) bomemo.Visible = true;
            else
            {
                bomemo.Visible = false;
                bomemest.Visible = false;
            }
            panel1.Invalidate();
            panelEstHp71.Invalidate();

            return;
        }

        private void panelEstHp71_Paint(object sender, PaintEventArgs e)
        {
            int i;

            i = util.EscribePanelEstaHP(panelEstHp71, nutra, est, siEst, pol);
            if (i == 1) botodas.BackColor = Color.PaleVioletRed;
            else botodas.BackColor = Color.White;
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

        private void boP_Click(object sender, EventArgs e)
        {
            ond = 0;
            boP.BackColor = Color.Green;
            boS.BackColor = Color.White;
            boC.BackColor = Color.White;
            peso = 0;
            MenuPeso();
        }

        private void boS_Click(object sender, EventArgs e)
        {
            ond = 1;
            boP.BackColor = Color.White;
            boS.BackColor = Color.DeepSkyBlue;
            boC.BackColor = Color.White;
            peso = 2;
            MenuPeso();
        }

        private void boC_Click(object sender, EventArgs e)
        {
            ond = 2;
            boP.BackColor = Color.White;
            boS.BackColor = Color.White;
            boC.BackColor = Color.Red;
        }

        void MarcasTi(Panel panel)
        {
            ushort nues,j,jj,i;
            int xf;
            float x1,y1,ff;
            double fay,fax;
            Pen[] lap = new Pen[3];

            xf = panel.Size.Width - 40;
            nues = 0;
            for (j = 0; j < nutra; j++) if (siEst[j] == true) nues += 1;
            if (nues == 0) return;
            fay = panel.Size.Height / (double)(nues);
            fax = xf / dura;
            ff = (float)(fay / 3);

            Graphics dc = panel.CreateGraphics();
            lap[0] = new Pen(colP, 1);
            lap[1] = new Pen(colS, 1);
            lap[2] = new Pen(colC, 1);
            
            for (i = 0; i < 3; i++)
            {
                jj = 0;
                for (j = 0; j < nutra; j++)
                {
                    if (siEst[j] == true)
                    {
                        if (fas[j][i] > 0)
                        {
                            x1 = (float)(40.0 + (fas[j][i] - tmini) * fax);
                            y1 = (float)(jj * fay + fay / 2);
                            dc.DrawLine(lap[i], x1, y1 - ff, x1, y1 + ff);
                        }
                        jj += 1;
                    }
                }
            }
            lap[0].Dispose();
            lap[1].Dispose();
            lap[2].Dispose();

            if (verTeo == false) return;

            lap[0] = new Pen(Color.DarkSeaGreen, 1);
            lap[0].DashStyle = DashStyle.DashDot;
            lap[1] = new Pen(Color.DarkSlateBlue, 1);
            lap[1].DashStyle = DashStyle.DashDot;
            for (i = 0; i < 2; i++)
            {
                jj = 0;
                for (j = 0; j < nutra; j++)
                {
                    if (siEst[j] == true)
                    {
                        if (teo[j][i] > 0)
                        {
                            x1 = (float)(40.0 + (teo[j][i] - tmini) * fax);
                            y1 = (float)(jj * fay + fay / 2);
                            dc.DrawLine(lap[i], x1, y1 - ff, x1, y1 + ff);
                        }
                        jj += 1;
                    }
                }
            }
            lap[0].Dispose();
            lap[1].Dispose();                       
            //x1 = (float)(40.0 + (tim[j][k] - tmini) * fax);
            return;
        }

        private void boQuita_Click(object sender, EventArgs e)
        {
            if (quita == false)
            {
                quita = true;
                boQuita.BackColor = Color.Gold;
            }
            else
            {
                quita = false;
                boQuita.BackColor = Color.White;
            }
            return;
        }

        void CorreHypo()
        {
            // Rutina que llama al Hypo71 para localizar el sismo
            int j, i, jj, ii;
            int an, me, di, ho, mi;
            long ll;
            double sp, cod, itera, min = 0, fasminu = 0, dd = 0,dd1;
            string lin = "", linea = "", str = "", fe = "", si_s = "",ca="",ca2="",fe2="";
            string esp = "                                                                                                    ";
            bool cond = false;


            for (j = 0; j < nutra; j++)
            {
                if (fas[j][0] > 0)
                {
                    ll = (long)(Fei + fas[j][0] * 10000000.0);
                    DateTime fech = new DateTime(ll);
                    ca = string.Format("{0:HH}", fech);
                    if (ca2.Length < 1)
                    {
                        ca2 = ca;
                        min = fas[j][0];
                    }
                    else if (ca != ca2)
                    {
                        if (min > fas[j][0]) min = fas[j][0];
                        cond = true;
                        break;
                    }
                    if (min > fas[j][0]) min = fas[j][0];
                }
            }
            if (cond == true)
            {
                ll = (long)(Fei + min * 10000000.0);
                DateTime fech = new DateTime(ll);
                an = int.Parse(string.Format("{0:yyyy}", fech));
                me = int.Parse(string.Format("{0:MM}", fech));
                di = int.Parse(string.Format("{0:dd}", fech));
                ho = int.Parse(string.Format("{0:HH}", fech));
                mi = int.Parse(string.Format("{0:mm}", fech));
                DateTime fff = new DateTime(an, me, di, ho, mi, 0, 0);
                ll = fff.ToBinary();
                fasminu = ((double)(ll) - Feisuds) / 10000000.0;
                fe2 = string.Format("{0:yy}{0:MM}{0:dd}{0:HH}{0:mm}", fech);
            }
            panelmensa.Visible = true;
            util.Mensaje(panelmensa, "Corriendo HYPO71 ...",true);
            if (panelmapa.Visible == true) panelmapa.Visible = false;
            if (!File.Exists(".\\h\\data.inp"))
            {
                StreamWriter da = File.CreateText(".\\h\\data.inp");
                da.WriteLine(".\\h\\r.inp");
                da.WriteLine(".\\h\\r.prt");
                da.WriteLine(".\\h\\r.pun");
                da.Close();
            }
            if (File.Exists(".\\h\\r.pun")) File.Delete(".\\h\\r.pun");
            if (File.Exists(".\\h\\r.prt")) File.Delete(".\\h\\r.prt");
            if (File.Exists(".\\h\\pun.txt")) File.Delete(".\\h\\pun.txt");
            si_s = "00";
            for (j = 0; j < nutra; j++)
            {
                if (fas[j][0] > 0 && fas[j][1] > 0)
                {
                    si_s = "10";
                    break;
                }
            }

            if (vol_viejo == -1 && fr1.ModAux == true && fr1.sismo.Substring(10, 2) == fr1.ClAux.Substring(0, 2))
                lin = ".\\h\\9.mod";
            else lin = ".\\h\\" + fr1.volcan[vol][0] + ".mod";
            //lin = ".\\h\\" + fr1.volcan[vol][0] + ".mod";
            if (!File.Exists(lin))
            {
                MessageBox.Show("NO EXISTE el archivo de Volcan: " + lin);
                panelmensa.Visible = false;
                return;
            }
            File.Copy(lin, ".\\h\\r.inp", true);

            StreamWriter hp = File.AppendText(".\\h\\r.inp");

            itera = inite;
            for (i = 0; i < 21; i++)
            {
                for (j = 0; j < nutra; j++)
                {
                    if (fas[j][0] > 0)
                    {
                        if (fas[j][1] > 0) sp = fas[j][1] - fas[j][0];
                        else sp = 0;
                        if (fas[j][2] > 0) cod = fas[j][2] - fas[j][0];
                        else cod = 0;
                        if (cond == false)
                        {
                            ll = (long)(Fei + fas[j][0] * 10000000.0);
                            DateTime fech = new DateTime(ll);
                            fe = string.Format("{0:yy}{0:MM}{0:dd}{0:HH}{0:mm}{0:ss}.{0:ff}", fech);
                            ca = est[j].Substring(0, 4);
                            str = ca.Substring(0, 4) + impu[j].ToString() + "P" + pol[j].ToString();
                            str += pes[j][0].ToString() + " " + fe;
                        }
                        else
                        {
                            dd = fas[j][0] - fasminu;
                            ca = est[j].Substring(0, 4);
                            str = ca.Substring(0, 4) + impu[j].ToString() + "P" + pol[j].ToString();
                            str += pes[j][0].ToString() + " ";
                            str += fe2;
                            if (dd < 100) str += string.Format("{0:00.00}", dd);
                            else str += string.Format("{0:000.0}", dd);
                        }                        
                        if (sp > 0)
                        {
                            if (cond == false) sp += double.Parse(fe.Substring(10, 5));
                            else sp += dd;
                            if (sp < 100.0) str += "       " + string.Format("{0:00.00}", sp);
                            else str += "      " + string.Format("{0:000.0}", sp);
                            str += " S " + pes[j][1].ToString();
                        }
                        else str += "       00.00 S 4";
                        str += string.Format("{0,37:0.0}", cod);
                        jj = str.Length;
                        if (jj < 82)
                        {
                            ii = 82 - jj;
                            str += esp.Substring(0, ii);
                        }
                        hp.WriteLine(str);
                    }
                }
                //str = "        " + fr1.sismo.Substring(0, 8) + fr1.sismo.Substring(11, 1) + si_s;
                str = "                 " + si_s;
                //str += string.Format("{0,5:00.00}", itera);
                if (itera <= 0) dd1 = 0.01;  // provisional mientras se arregla la version nueva del Hypo71
                else dd1 = itera;
                if (HP71PC == false) str += string.Format("{0,5:00.00}",dd1);
                else
                {
                    if (itera < valHP71PC) str += string.Format("{0,5:00.00}", valHP71PC);
                    else str += string.Format("{0,5:00.00}",dd1);
                }
                jj = str.Length;
                if (jj < 82)
                {
                    ii = 82 - jj;
                    str += esp.Substring(0, ii);
                }
                hp.WriteLine(str);
                itera += ite;
            }

            hp.Close();

            //linea = "/C .\\h\\hp71 < .\\h\\data.inp > .\\h\\bas.txt";
            if (File.Exists(".\\h\\hp71pc.exe")) linea = "/C .\\h\\hp71pc ";
            else linea = "/C .\\h\\hp71 ";
            linea += "< .\\h\\data.inp > .\\h\\bas.txt";
            util.Dos(linea,true);
            mejite = -1;
            EscriPun();
            panelmensa.Visible = false;

            return;
        }

        private void boHp71_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (hyp == false)
                {
                    hyp = true;
                    boHp71.BackColor = Color.DarkOrange;
                }
                else
                {
                    hyp = false;
                    boHp71.BackColor = Color.Thistle;
                }
            }
            else
            {                
                boTeo.Visible = false;
                nomML = "";
                CorreHypo();
            }

            return;
        }

        void EscriPun()
        {
            int j=0, i, k, dif, ila=1, ilo=-1;
            int lag, log;
            double lam, lom;
            string linprt="",linpun="",linea="",lin1="",lin2="",lin3="",lin4="";
            string lin5="",lin6="",lin7="",ca="",pa="";
            bool data = false;

            if (!File.Exists(".\\h\\r.pun") || !File.Exists(".\\h\\r.prt"))
            {
                MessageBox.Show("Problemas en la 'corrida' del Hypo71 !!!");
                return;
            }
            //boManu.Visible = true;
            panelML.Visible = false;
            panelpun.Visible = true;            
            util.borra(panelpun, Color.AntiqueWhite);
            linea = "No hubo Iteracion !!!";
            Graphics dc = panelpun.CreateGraphics();
            Pen lapiz = new Pen(Color.Black, 1);
            Pen lapiz2 = new Pen(Color.BlueViolet, 1);
            SolidBrush brocha = new SolidBrush(Color.Blue);
            SolidBrush brocha2 = new SolidBrush(Color.DarkViolet);
            SolidBrush brocha3 = new SolidBrush(Color.Green);
            SolidBrush brocha4 = new SolidBrush(Color.DarkRed);
            SolidBrush brocha5 = new SolidBrush(Color.DarkOrange);
            SolidBrush brocha6 = new SolidBrush(Color.HotPink);
            SolidBrush brocha7 = new SolidBrush(Color.Brown);
            try
            {
                StreamReader prt = new StreamReader(".\\h\\r.prt");
                StreamReader pun = new StreamReader(".\\h\\r.pun");
                StreamWriter ww = new StreamWriter(".\\h\\pun.txt");
                linpun = pun.ReadLine();
                j = 20;
                i = 0;
                ca = "Prof    Mag   #fas     Gap        rms      erh        erz";
                dc.DrawString(ca, new Font("Times New Roman", 9), brocha, 140, 1);
                while (linprt != null)
                {
                    try
                    {
                        linprt = prt.ReadLine();
                        if (linprt == null) break;
                        if (string.Compare(linprt.Substring(5, 4), "ORIG") == 0) data = false;
                        else if (string.Compare(linprt.Substring(20, 3), "LAT") == 0 && string.Compare(linprt.Substring(29, 4), "LONG") == 0)
                        {
                            if (linprt[24] == 'N') ila = 1;
                            else ila = -1;
                            if (linprt[34] == 'E') ilo = 1;
                            else ilo = -1;
                            //MessageBox.Show("ila=" + ila.ToString() + " ilo=" + ilo.ToString());
                        }
                        else if (string.Compare(linprt.Substring(2, 3), "STN") == 0)
                        {
                            data = true;
                            ca = pun.ReadLine();
                            if (ca == null) break;
                            lag = ila * int.Parse(ca.Substring(18, 2)); // ojo, aqui se debe tomar solo los valores, pues el signo se lee si la localizacion en el Prt es N, S, E o W.
                            lam = ila * double.Parse(ca.Substring(21, 5));
                            log = ilo * int.Parse(ca.Substring(27, 3));
                            lom = ilo * double.Parse(ca.Substring(31, 5));
                            linpun = ca.Substring(0, 17);
                            pa = lag.ToString();
                            dif = 3 - pa.Length;
                            for (k = 0; k < dif; k++) linpun += " ";
                            linpun += pa;
                            pa = string.Format("{0:00.00}", lam);
                            dif = 6 - pa.Length;
                            for (k = 0; k < dif; k++) linpun += " ";
                            linpun += pa;
                            pa = log.ToString();
                            dif = 4 - pa.Length;
                            for (k = 0; k < dif; k++) linpun += " ";
                            linpun += pa;
                            pa = string.Format("{0:00.00}", lom);
                            dif = 6 - pa.Length;
                            for (k = 0; k < dif; k++) linpun += " ";
                            linpun += pa;
                            linpun += ca.Substring(36);
                            lin1 = linpun.Substring(17, 19);
                            lin2 = linpun.Substring(36, 7) + "   " + linpun.Substring(45, 5) + "   " + linpun.Substring(50, 3);
                            lin7 = linpun.Substring(54, 3);
                            lin3 = linpun.Substring(62, 5);
                            lin4 = linpun.Substring(67, 5);
                            lin5 = linpun.Substring(72, 5);
                            lin6 = linpun.Substring(77, 2);
                            ww.WriteLine(linpun);
                            dc.DrawString(lin1, new Font("Times New Roman", 10), brocha, 1, j);
                            dc.DrawString(lin2, new Font("Times New Roman", 10), brocha2, 130, j);
                            dc.DrawString(lin7, new Font("Times New Roman", 10), brocha7, 240, j);
                            dc.DrawString(lin3, new Font("Times New Roman", 10), brocha3, 280, j);
                            dc.DrawString(lin4, new Font("Times New Roman", 10), brocha4, 315, j);
                            dc.DrawString(lin5, new Font("Times New Roman", 10), brocha5, 355, j);
                            dc.DrawString(lin6, new Font("Times New Roman", 10), brocha6, 390, j);
                            /*if (itebas > -1.0F)
                            {
                                if ((float)(inite + (ite * i)) == itebas)
                                {
                                    Pen lapiz3 = new Pen(Color.Green, 2);
                                    dc.DrawRectangle(lapiz3, 4, j + 2, 360, 13);
                                    lapiz3.Dispose();
                                }
                            }*/
                            if (i == mejite)
                            {
                                dc.DrawRectangle(lapiz2, 1, j, 365, 16);
                            }
                            j += espaciado;
                            i += 1;
                        }
                        else if (string.Compare(linprt.Substring(5,7), "* INSUF") == 0)
                        {
                            if (data == false)
                            {
                                ww.WriteLine(linea);
                                dc.DrawString(linea, new Font("Times New Roman", 10), brocha, 1, j);
                                j += espaciado;
                                i += 1;
                            }
                            data = false;
                        }
                    }
                    catch
                    {
                        //MessageBox.Show("linprt="+linprt+" linpun="+linpun);
                    }
                }
                prt.Close();
                pun.Close();
                ww.Close();
            }
            catch
            {
                textBoxHPC.Visible = true;
                HP71PC = true;
            }
            lapiz.Dispose();
            lapiz2.Dispose();
            brocha.Dispose();
            brocha2.Dispose();
            brocha3.Dispose();
            brocha4.Dispose();
            brocha5.Dispose();
            brocha6.Dispose();
            brocha7.Dispose();

            if (j == 20)
            {
                panelpun.Visible = false;
                ArreglarModelo();
                MessageBox.Show("Formato Modelo Arreglado\nVuelva a correr el hp71pc");
                textBoxHPC.Visible = true;
                HP71PC = false;
                textBoxHPC.BackColor = Color.Red;
            }

            return;
        }

        /*void EscriPun()
        {
            int j=0,i,k,ila=1,ilo=-1,dif;
            int lag, log;
            double lam,lom;
            string linprt="",linpun="",linea="",lin1="",lin2="",lin3="",ca="",pa="";
            bool data = false;

            //mejite = -1;
            if (!File.Exists(".\\h\\r.pun") || !File.Exists(".\\h\\r.prt"))
            {
                MessageBox.Show("Problemas en la 'corrida' del Hypo71 !!!");
                return;
            }
            panelpun.Visible = true;
            panelpun.BringToFront();
            util.borra(panelpun, Color.AntiqueWhite);
            linea = "No hubo Iteracion !!!";
            Graphics dc = panelpun.CreateGraphics();
            Pen lapiz = new Pen(Color.Black, 1);
            Pen lapiz2 = new Pen(Color.BlueViolet, 1);
            SolidBrush brocha = new SolidBrush(Color.Blue);
            SolidBrush brocha2 = new SolidBrush(Color.DarkViolet);
            SolidBrush brocha3 = new SolidBrush(Color.DarkRed);
            try
            {
                StreamReader prt = new StreamReader(".\\h\\r.prt");
                StreamReader pun = new StreamReader(".\\h\\r.pun");
                StreamWriter ww = new StreamWriter(".\\h\\pun.txt");
                linpun = pun.ReadLine();
                j = 20;
                i = 0;
                while (linprt != null)
                {
                    try
                    {
                        linprt = prt.ReadLine();
                        if (linprt == null) break;
                        if (string.Compare(linprt.Substring(5, 4), "ORIG") == 0) data = false;
                        else if (string.Compare(linprt.Substring(20, 3), "LAT") == 0 && string.Compare(linprt.Substring(29, 4), "LONG") == 0)
                        {
                            if (linprt[24] == 'N') ila = 1;
                            else ila = -1;
                            if (linprt[34] == 'E') ilo = 1;
                            else ilo = -1;
                            // MessageBox.Show("ila=" + ila.ToString() + " ilo=" + ilo.ToString());
                        }
                        else if (string.Compare(linprt.Substring(2, 3), "STN") == 0)
                        {
                            data = true;
                            ca = pun.ReadLine();
                            if (ca == null) break;
                            //linpun = pun.ReadLine();
                            //if (linpun == null) break;
                            lag = ila * int.Parse(ca.Substring(17, 3));
                            lam = ila * double.Parse(ca.Substring(21, 5));
                            log = ilo * int.Parse(ca.Substring(27, 3));
                            lom = ilo * double.Parse(ca.Substring(31, 5));
                            linpun = ca.Substring(0, 17);
                            pa = lag.ToString();
                            dif = 3 - pa.Length;
                            for (k = 0; k < dif; k++) linpun += " ";
                            linpun += pa;
                            pa = string.Format("{0:00.00}", lam);
                            dif = 6 - pa.Length;
                            for (k = 0; k < dif; k++) linpun += " ";
                            linpun += pa;
                            pa = log.ToString();
                            dif = 4 - pa.Length;
                            for (k = 0; k < dif; k++) linpun += " ";
                            linpun += pa;
                            pa = string.Format("{0:00.00}", lom);
                            dif = 6 - pa.Length;
                            for (k = 0; k < dif; k++) linpun += " ";
                            linpun += pa;
                            linpun += ca.Substring(36);
                            //lin1 = linpun.Substring(1, 18);
                            lin1 = linpun.Substring(17, 19);
                            lin2 = linpun.Substring(36, 7) + "   " + linpun.Substring(45, 5) + "   " + linpun.Substring(50, 3);
                            lin3 = linpun.Substring(62, 17);
                            ww.WriteLine(linpun);
                            dc.DrawString(lin1, new Font("Times New Roman", 10), brocha, 1, j);
                            dc.DrawString(lin2, new Font("Times New Roman", 10), brocha2, 150, j);
                            dc.DrawString(lin3, new Font("Times New Roman", 10), brocha3, 270, j);
                            if (i == mejite)
                            {
                                dc.DrawRectangle(lapiz2, 1, j, 365, 16);
                            }
                            j += 20;
                            i += 1;
                        }
                        else if (string.Compare(linprt.Substring(5, 7), "* INSUF") == 0)
                        {
                            if (data == false)
                            {
                                ww.WriteLine(linea);
                                dc.DrawString(linea, new Font("Times New Roman", 10), brocha, 1, j);
                                j += 20;
                                i += 1;
                            }
                            data = false;
                        }
                    }
                    catch
                    {
                        //MessageBox.Show("ERROR  "+linea);
                    }
                }
                prt.Close();
                pun.Close();
                ww.Close();
            }
            catch
            {
                textBoxHPC.Visible = true;
                HP71PC = true;
            }
            lapiz.Dispose();
            lapiz2.Dispose();
            brocha.Dispose();
            brocha2.Dispose();
            brocha3.Dispose();


            if (j == 20)
            {
                panelpun.Visible = false;
                ArreglarModelo();
                MessageBox.Show("Formato Modelo Arreglado\nVuelva a correr el hp71pc");
                textBoxHPC.Visible = true;
                HP71PC = false;
                textBoxHPC.BackColor = Color.Red;
            }

            return;
        }*/

        void ArreglarModelo()
        {
            int i, j;
            string nom = "", ca = "", li = "", pa = "";
            string esp = "                                                                                                    ";

            panelmen.Visible = true;
            util.Mensaje(panelmen, "Arreglando\nFormato\nModelo....", false);
            nom = fr1.rutbas + "\\h\\" + fr1.volcan[vol][0] + ".mod";
            ca = ".\\h\\" + fr1.volcan[vol][0] + ".mod";
            if (File.Exists(ca)) File.Delete(ca);
            if (File.Exists(nom))
            {
                StreamReader ar = new StreamReader(nom);
                StreamWriter wr = File.CreateText(ca);
                while (li != null)
                {
                    try
                    {
                        li = ar.ReadLine();
                        if (li == null) break;
                        i = li.Length;
                        if (i < 82)
                        {
                            j = 82 - i;
                            pa = li + esp.Substring(0, j);
                            wr.WriteLine(pa);
                        }
                        else wr.WriteLine(li);
                    }
                    catch
                    {
                    }
                }
                ar.Close();
                wr.Close();
                File.Copy(ca, nom, true);
            }
            panelmen.Visible = false;
        }

        private void boImp_Click(object sender, EventArgs e)
        {
            if (imp == 'I') imp = 'E';
            else imp = 'I';
            boImp.Text = imp.ToString();
        }
       
        private void panelpun_MouseDown(object sender, MouseEventArgs e)
        {
            int iy,yf,i;
            double facmla, facmlo;
            //double lasi,losi,facm,facmapa;
            string lin="",pun="";

            siteo = false;
            if (e.Y < 20)
            {
                if (panelmapa.Visible == true) panelmapa.Visible = false;
                return;
            }
            boSiPun.Visible = true;
            facmapa = 1100.0 / 0.3;
            yf = panelpun.Size.Height - 20;
            iy = (e.Y - 20) / 20;
            
            StreamReader ar = new StreamReader(".\\h\\pun.txt");
            i = 0;
            while (lin != null)
            {
                try
                {
                    lin = ar.ReadLine();
                    if (lin == null) break;
                    if (i == iy)
                    {
                        pun = lin;
                        break;
                    }
                    i += 1;
                }
                catch
                {
                }
            }
            ar.Close();

            if (pun == "" || char.IsLetter(pun[0])) return;

            mejpun = pun;
            mejite = (short)(iy);
            EscriPun();
            boTeo.Visible = true;
            //boWada.Visible = true;
            Fijar();
            panelmapa.Visible = true;
            util.borra(panelmapa, Color.LavenderBlush);
            lasi = double.Parse(pun.Substring(17,3))+double.Parse(pun.Substring(20,6))/60;
            losi = double.Parse(pun.Substring(26,4)) + double.Parse(pun.Substring(30,6))/60;
            facmla=0.7*((0.5*panelmapa.Size.Height)/Math.Abs(lasi-fr1.latvol[vol]));
            facmlo=0.7*((0.5*panelmapa.Size.Height)/Math.Abs(losi-fr1.lonvol[vol]));
            facm = facmla;
            if (facm > facmlo) facm = facmlo;
            if (facm > facmapa) facm = facmapa;
          //  MessageBox.Show("lasi=" + lasi.ToString() + " losi=" + losi.ToString() + " lavol=" + fr1.latvol[vol].ToString() + " lovol=" + fr1.lonvol[vol].ToString());
            util.Topo(panelmapa,fr1.volcan[vol],facm,fr1.latvol[vol],fr1.lonvol[vol],
                 lasi,losi,Color.Gray,false,0,0,0,0);

            return;
        }

        void Fijar()
        {
            int j, jj, ii;
            int an, me, di, ho, mi;
            long ll;
            double fasP=0,fase,ps=0,pteo=0,dd=0,min=0,fasminu=0;
            string lin = "", str = "", fe = "", linea = "", ca="", ca2 = "", fe2 = "";
            string esp = "                                                                                                    ";
            bool cond = false;


            for (j = 0; j < nutra; j++)
            {
                if (fas[j][0] > 0)
                {
                    ll = (long)(Fei + fas[j][0] * 10000000.0);
                    DateTime fech = new DateTime(ll);
                    ca = string.Format("{0:HH}", fech);
                    if (ca2.Length < 1)
                    {
                        ca2 = ca;
                        min = fas[j][0];
                    }
                    else if (ca != ca2)
                    {
                        if (min > fas[j][0]) min = fas[j][0];
                        cond = true;
                        break;
                    }
                    if (min > fas[j][0]) min = fas[j][0];
                }
            }
            if (cond == true)
            {
                ll = (long)(Fei + min * 10000000.0);
                DateTime fech = new DateTime(ll);
                an = int.Parse(string.Format("{0:yyyy}", fech));
                me = int.Parse(string.Format("{0:MM}", fech));
                di = int.Parse(string.Format("{0:dd}", fech));
                ho = int.Parse(string.Format("{0:HH}", fech));
                mi = int.Parse(string.Format("{0:mm}", fech));
                DateTime fff = new DateTime(an, me, di, ho, mi, 0, 0);
                ll = fff.ToBinary();
                fasminu = ((double)(ll) - Feisuds) / 10000000.0;
                fe2 = string.Format("{0:yy}{0:MM}{0:dd}{0:HH}{0:mm}", fech);
            }
            for (j = 0; j < nutra; j++)
            {
                teo[j][0]=0;
                teo[j][1]=0;
            }
            if (!File.Exists(".\\h\\datafij.inp"))
            {
                StreamWriter da = File.CreateText(".\\h\\datafij.inp");
                da.WriteLine(".\\h\\f.inp");
                da.WriteLine(".\\h\\f.prt");
                da.WriteLine(".\\h\\f.pun");
                da.Close();
            }
            if (vol_viejo == -1 && fr1.ModAux == true && fr1.sismo.Substring(10, 2) == fr1.ClAux.Substring(0, 2))
                lin = ".\\h\\9.mod";
            else lin = ".\\h\\" + fr1.volcan[vol][0] + ".mod";
            //lin = ".\\h\\" + fr1.volcan[vol][0] + ".mod";
            if (!File.Exists(lin)) return;
            if (File.Exists(".\\h\\f.prt")) File.Delete(".\\h\\f.prt");

            for (j = 0; j < nutra; j++)
            {
                if (fas[j][0] > 0)
                {
                    fasP = fas[j][0];
                    break;
                }
            }

            File.Copy(lin, ".\\h\\f.inp", true);

            StreamWriter fj = File.AppendText(".\\h\\f.inp");

            for (j = 0; j < nutra; j++)
            {
                //if (fas[j][0] > 0)
                //{
                if (fas[j][0] > 0) fase = fas[j][0];
                else fase = fasP;
                if (cond == false)
                {
                    ll = (long)(Fei + fase * 10000000.0);
                    DateTime fech = new DateTime(ll);
                    fe = string.Format("{0:yy}{0:MM}{0:dd}{0:HH}{0:mm}{0:ss}.{0:ff}", fech);
                    str = est[j].Substring(0, 4) + impu[j].ToString() + "P" + pol[j].ToString();
                    str += pes[j][0].ToString() + " " + fe;
                }
                else
                {
                    dd = fase - fasminu;
                    ca = est[j].Substring(0, 4);
                    str = ca.Substring(0, 4) + impu[j].ToString() + "P" + pol[j].ToString();
                    str += pes[j][0].ToString() + " ";
                    str += fe2;
                    if (dd < 100) str += string.Format("{0:00.00}", dd);
                    else str += string.Format("{0:000.0}", dd);
                }
                jj = str.Length;
                if (jj < 82)
                {
                    ii = 82 - jj;
                    str += esp.Substring(0, ii);
                }
                fj.WriteLine(str);
            }
            str = string.Format("                 19");
            jj = str.Length;
            if (jj < 82)
            {
                ii = 82 - jj;
                str += esp.Substring(0, ii);
            }
            fj.WriteLine(str);
            str="  "+mejpun.Substring(9,2)+"."+mejpun.Substring(12,5)+"   ";
            str+=mejpun.Substring(18,2)+mejpun.Substring(21,5)+"   "+mejpun.Substring(28,2);
            str+=mejpun.Substring(31,5)+mejpun.Substring(38,5);
            jj = str.Length;
            if (jj < 82)
            {
                ii = 82 - jj;
                str += esp.Substring(0, ii);
            }
            fj.WriteLine(str);

            fj.Close();

            //linea = "/C .\\h\\hp71 < .\\h\\datafij.inp > .\\h\\bas.txt";
            if (File.Exists(".\\h\\hp71pc.exe")) linea = "/C .\\h\\hp71pc ";
            else linea = "/C .\\h\\hp71 ";
            linea += "< .\\h\\datafij.inp > .\\h\\bas.txt";
            util.Dos(linea,true);
            if (!File.Exists(".\\h\\f.prt")) return;

            StreamReader pr = new StreamReader(".\\h\\f.prt");
            lin = "";
            while (lin != null)
            {
                try
                {
                    lin = pr.ReadLine();
                    if (lin == null) break;
                    if (string.Compare(lin.Substring(17, 3), "POS") == 0)
                    {
                        lin = pr.ReadLine();
                        ps = double.Parse(lin.Substring(16,5));
                    }
                    else if (string.Compare(lin.Substring(2, 3), "STN") == 0)
                    {
                        if (ps == 0) break;
                        do
                        {
                            lin = pr.ReadLine();
                            for (j = 0; j < nutra; j++)
                            {
                                if (string.Compare(lin.Substring(1,4),est[j].Substring(0,4))==0)
                                {
                                    if (fas[j][0]>0) teo[j][0]=fas[j][0]-double.Parse(lin.Substring(53, 6));
                                    else teo[j][0] = fasP - double.Parse(lin.Substring(53, 6));
                                    pteo=double.Parse(lin.Substring(41,6))+double.Parse(lin.Substring(47,6));
                                    teo[j][1]=teo[j][0]+pteo*ps-pteo;
                                }
                            }
                        } while (lin != null);
                    }
                }
                catch
                {
                }
            }

            pr.Close();

            for (j = 0; j < nutra; j++)
            {
                if (teo[j][0] > 0)
                {
                    siteo = true;
                    break;
                }
            }

            return;
        }

        private void boSiPun_Click(object sender, EventArgs e)
        {
            int i,j;
            bool si = false;

            if (panelmapa.Visible == true) panelmapa.Visible = false;
            panelpun.Visible = false;
            boSiPun.Visible = false;
            boGraBas.Visible = true;
            boRota.Visible = true;
            if (File.Exists("c:\\reporte.doc")) boWord.Visible = true;
            if (verTeo == true && panelOrd.Visible == true) panelOrd.Invalidate();
            if (numl > 0)
            {
                for (i = 0; i <= ordml[numl - 1]; i++) ml[i] = -10.0;
                si = RevitraML();
                if (si == true)
                {
                    boctave.BackColor = Color.SteelBlue;
                    j = (int)(numl / 3.0);
                    for (i = 0; i < j; i++) boml[i].Visible = siboml[i];
                    //Calculo_ML(idml);
                    Calculo_ML();
                    ML_total();
                }
            }
            if (si == true) boctave.Visible = true;
            //MessageBox.Show("boctave.visible=" + boctave.Visible.ToString());
            bomapa.Visible = true;
            boPun.Visible = true;

            return;
        }

        private void boNoPun_Click(object sender, EventArgs e)
        {
            if (panelmapa.Visible == true) panelmapa.Visible = false;
            panelpun.Visible = false;
            mejpun = "";
            boSiPun.Visible = false;
            boGraBas.Visible = false;
            boWord.Visible = false;
            mejite = -1;
            bomapa.Visible = false;
            boPun.Visible = false;

            return;
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (grabas == true && mejpun != "")
            {
                GrabaBase();
            }
            else
            {
                if (File.Exists(nomML)) File.Delete(nomML);
            }
            bomapa.Visible = false;
            boPun.Visible = false;
            fr1.desactivado = false;
        }

        private void boBase_Click(object sender, EventArgs e)
        {
            if (grabas == true)
            {
                grabas = false;
                boBase.BackColor = Color.White;
                boGraBas.Visible = false;
            }
            else
            {
                grabas = true;
                boBase.BackColor = Color.YellowGreen;
                if (mejite > -1) boGraBas.Visible = true;
            }
            boGraFocmec.Visible = grabas;
        }

        void GrabaBuz()
        {
            int i, j, cont;
            string li = "", ca = "", fe = "";
            string[] lin = new string[Ma];

            if (!File.Exists(".\\h\\r.prt") || mejite < 0 || !File.Exists(".\\h\\buz.exe"))
            {
                return;
            }

            panelmen.Visible = true;
            util.Mensaje(panelmen, "Grabando datos BUZ.\n\nEspere por favor...", true);
            StreamReader ar = new StreamReader(".\\h\\r.prt");

            cont = 0;
            j = 0;
            while (li != null)
            {
                try
                {
                    li = "";
                    li = ar.ReadLine();
                    if (li == null) break;
                    if (string.Compare(li.Substring(114, 4), "SDXM") == 0)
                    {
                        if (cont == mejite)
                        {
                            for (i = 0; i < 3; i++) lin[j++] = ar.ReadLine();
                            do
                            {
                                li = ar.ReadLine();
                                if (li.Length < 60 || li[0] != ' ' || !char.IsLetter(li[1])) break;
                                lin[j++] = li;
                            } while (li.Length > 60 || li[0] == ' ' && char.IsLetter(li[1]));
                            break;
                        }
                        cont += 1;
                    }
                    else if (string.Compare(li.Substring(5, 3), "* I") == 0)
                    {
                        cont += 1;
                    }
                }
                catch
                {
                }
            }

            ar.Close();

            try
            {

                StreamWriter wr = File.CreateText(".\\h\\mec.txt");

                fe = lin[0].Substring(1, 6);
                ca = fe + "-" + lin[0].Substring(8, 2) + ":" + lin[0].Substring(10, 2);
                wr.WriteLine(ca);
                for (i = 3; i < j; i++)
                {

                    if (char.IsLetter(lin[i][22]))
                    {
                        ca = lin[i].Substring(1, 5) + fe + "  " + lin[i].Substring(12, 3) + "  " + lin[i].Substring(16, 3) + "   " + lin[i].Substring(22, 1);
                        wr.WriteLine(ca);
                    }
                }
                //for (i = 0; i < j; i++) wr.WriteLine(lin[i]);

                wr.Close();
            }
            catch
            {
                panelmen.Visible = false;
                return;
            }
            panelmen.Visible = false;

            return;
        }

        void GrabaMeca()
        {
            int i, j, cont, año, mes, dia;
            string li = "", nom = "", dir = "", ca = "";
            string[] lin = new string[Ma];

            if (!File.Exists(".\\h\\r.prt") || mejite < 0) return;

            panelmen.Visible = true;
            util.Mensaje(panelmen, "Grabando PRT.\n\nEspere por favor...", true);
            StreamReader ar = new StreamReader(".\\h\\r.prt");

            cont = 0;
            j = 0;
            while (li != null)
            {
                try
                {
                    li = "";
                    li = ar.ReadLine();
                    if (li == null) break;
                    if (string.Compare(li.Substring(114, 4), "SDXM") == 0)
                    {
                        if (cont == mejite)
                        {
                            lin[j++] = li;
                            for (i = 0; i < 3; i++) lin[j++] = ar.ReadLine();
                            do
                            {
                                li = ar.ReadLine();
                                if (li.Length < 60 || li[0] != ' ' || !char.IsLetter(li[1])) break;
                                lin[j++] = li;
                            } while (li.Length > 60 || li[0] == ' ' && char.IsLetter(li[1]));
                            break;
                        }
                        cont += 1;
                    }
                    else if (string.Compare(li.Substring(5, 3), "* I") == 0)
                    {
                        cont += 1;
                    }
                }
                catch
                {
                }
            }

            ar.Close();

            try
            {
                año = int.Parse(lin[1].Substring(1, 2));
                mes = int.Parse(lin[1].Substring(3, 2));
                dia = int.Parse(lin[1].Substring(5, 2));
                dir = fr1.rutbas + "\\mec\\" + string.Format("{0:00}\\", año) + string.Format("{0:00}\\", mes) + string.Format("{0:00}", dia);
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                for (i = 8; i <= 11; i++)
                {
                    if (lin[1][i] == ' ') ca += "0";
                    else ca += lin[1][i];
                }
                ca += "_";
                if (lin[1][13] == ' ') ca += "0";
                else ca += lin[1][13];
                ca += lin[1][14];
                nom = dir + "\\" + ca + "_" +fr1.sismo.Substring(0,8)+fr1.sismo.Substring(9,3) + ".txt";
                //MessageBox.Show("mecabas=" + mecabas + " nombas=" + nombas);
                //if (File.Exists(mecabas)) File.Delete(mecabas);

                StreamWriter wr = File.CreateText(nom);

                for (i = 0; i < j; i++) wr.WriteLine(lin[i]);

                wr.Close();
            }
            catch
            {
                panelmen.Visible = false;
                return;
            }
            panelmen.Visible = false;

            return;
        }

        void GrabaBase()
        {
            int i,j,nu,an,cood;
            long ll;
            double sp, cod, itera, suma, frac;
            char[] cc = new char[80];
            string[] lin;
            string linea="",str="",fe="",si_s="",nom="",ca="",fe1="",nom2="";

            if (!Directory.Exists(".\\mat")) Directory.CreateDirectory(".\\mat");
            nu=0;
            for (j = 0; j < nutra; j++) if (fas[j][0] > 0) nu += 1;
            lin = new string[nu+1];

            cc = mejpun.ToCharArray();
            for (i = 0; i < 6; i++) if (cc[i] == ' ') cc[i] = '0';
            for (i = 7; i < 10; i++) if (cc[i] == ' ') cc[i] = '0';
            for (i = 12; i < 16; i++) if (cc[i] == ' ') cc[i] = '0';
            linea = new string(cc);
            linea+=" "+string.Format("{0:00}",nu)+" "+fr1.sismo.Substring(0,12)+" ";
            //linea += fr1.volcan[vol][0] + " " + fr1.usu;
            if (vol_viejo==-1 && fr1.ModAux==true && fr1.sismo.Substring(10,2)==fr1.ClAux.Substring(0,2)) linea += "9";
            else linea+=fr1.volcan[vol][0];
            linea += " " + fr1.usu;
            if (ML > -10.0 && sioct == true)
            {
                if (ML>=0) linea += "  " + string.Format("{0:0.0}", ML);
                else linea += " " + string.Format("{0:0.0}", ML);
            }
            else linea += "     ";

            si_s = "00";
            for (j = 0; j < nutra; j++)
            {
                if (fas[j][0] > 0 && fas[j][1] > 0)
                {
                    si_s = "10";
                    break;
                }
            }
            i = 0;
            StreamWriter mat2 = File.CreateText(".\\mat\\tipocoda.m");
            mat2.WriteLine("tipoml='"+fr1.sismo.Substring(9,3)+"';");
            mat2.WriteLine();
            mat2.Write("codas='");
            for (j = 0; j < nutra; j++)
            {
                if (fas[j][0] > 0)
                {
                    if (fas[j][1] > 0) sp = fas[j][1] - fas[j][0];
                    else sp = 0;
                    if (fas[j][2] > 0) cod = fas[j][2] - fas[j][0];
                    else cod = 0;
                    cood = (int)(cod);
                    if (cood > 0) mat2.Write(est[j].Substring(0,4)+";"+string.Format("{0:00000}",cood)+";");
                    ll = (long)(Fei + fas[j][0] * 10000000.0);
                    DateTime fech = new DateTime(ll);
                    fe = string.Format("{0:yy}{0:MM}{0:dd}{0:HH}{0:mm}{0:ss}.{0:ff}", fech);
                    str = est[j].Substring(0, 4) + impu[j].ToString() + "P" + pol[j].ToString();
                    str += pes[j][0].ToString() + " " + fe;
                    if (sp > 0)
                    {
                        sp += double.Parse(fe.Substring(10, 5));
                        if (sp < 100.0) str += "       " + string.Format("{0:00.00}", sp);
                        else str += "      " + string.Format("{0:000.0}", sp);
                        str += " S " + pes[j][1].ToString();
                    }
                    else str += "       00.00 S 4";
                    str += string.Format("{0,37:0.0}", cod);
                    lin[i] = str;
                    i += 1;
                }
            }

            mat2.WriteLine("';");
            mat2.Close();
            itera = inite + ite * mejite;
            str = "        " + fr1.sismo.Substring(0, 8) + fr1.sismo.Substring(11, 1) + si_s;
            str += string.Format("{0,5:00.00}", itera);
            lin[i] = str;

            if (fr1.html == true) GrabaLocHtml(linea);
            nom = fr1.rutbas + "\\loc\\" + linea.Substring(0, 4) + ".ipn";
            nom2 = fr1.rutbas + "\\temp\\" + linea.Substring(0, 4) + ".ipn";

            if (File.Exists(nom)) File.Copy(nom, "localiza.txt", true);

            //StreamWriter hp = File.AppendText(nom);
            StreamWriter hp = File.AppendText("localiza.txt");
            hp.WriteLine(linea);
            for (i = 0; i <= nu; i++) hp.WriteLine(lin[i]);
            hp.Close();

            File.Copy("localiza.txt", nom, true);

            if (File.Exists(nom2))
            {
                FileInfo f1 = new FileInfo(nom);
                long l1 = f1.Length;
                FileInfo f2 = new FileInfo(nom2);
                long l2 = f2.Length;
                if (l1 > l2)
                {
                    File.Copy(nom, nom2, true);
                }
            }
            else
            {
                ca = fr1.rutbas + "\\temp";
                if (!Directory.Exists(ca)) Directory.CreateDirectory(ca);
                File.Copy(nom, nom2, true);
            }


            GrabaMeca();

            StreamWriter mat = File.CreateText(".\\mat\\coormagn.m");

            mat.Write("minlat="+linea.Substring(21,5));
            mat.Write("; minlon=" + linea.Substring(31, 5));
            mat.Write("; gralat=" + linea.Substring(18, 2));
            mat.Write("; gralon=" + linea.Substring(28,2));
            an=int.Parse(linea.Substring(0,2));
            if (an<85) an+=2000;
            else an+=1900;
            mat.Write(";fechaml='"+string.Format("{0:d4}",an)+"/"+linea.Substring(2,2)+"/"+linea.Substring(4,2)+"'; ");
            mat.Write("horalocml='"+linea.Substring(7,2)+":"+linea.Substring(9,2)+":"+linea.Substring(12,5)+"'; ");
            mat.Write("magdur='"+linea.Substring(46,4));
            mat.Write("'; nombretraza='"+linea.Substring(84,12)+"';");
            mat.Write("profundidad="+linea.Substring(37,6)+";");

            mat.Close();

            //MessageBox.Show("fr1=" + fr1.hor_rsn.ToString());
            if (fr1.hor_rsn != 10000)
            {
                if (!Directory.Exists(".\\rsn")) Directory.CreateDirectory(".\\rsn");
                if (fr1.hor_rsn == 0) suma = 0;
                else suma = fr1.hor_rsn * 36000000000.0;
                ca = "";
                for (j = 0; j < nutra; j++)
                {
                    if (fas[j][0] > 0)
                    {
                        ll = (long)(Fei + fas[j][0] * 10000000.0 + suma);
                        DateTime fech0 = new DateTime(ll);
                        ca = ".\\rsn\\" + string.Format("{0:yyyy}_{0:MM}{0:dd}_{0:HH}{0:mm}_{0:ss}.txt", fech0);
                        break;
                    }
                }
                if (ca == "") return;

                StreamWriter rsnc = File.CreateText(ca);
                for (j = 0; j < nutra; j++)
                {
                    if (fas[j][0] > 0)
                    {
                        if (fas[j][1] > 0) sp = fas[j][1] - fas[j][0];
                        else sp = 0;
                        if (fas[j][2] > 0) cod = fas[j][2] - fas[j][0];
                        else cod = 0;
                        ll = (long)(Fei + fas[j][0] * 10000000.0 + suma);
                        DateTime fech2 = new DateTime(ll);
                        //fe = string.Format("{0:yy}{0:MM}{0:dd}{0:HH}{0:mm}{0:ss}.{0:ffff}", fech);
                        fe = string.Format("{0:yy}{0:MM}{0:dd}{0:HH}{0:mm}", fech2);
                        frac = double.Parse(string.Format("{0:ss}.{0:ffff}", fech2));
                        fe1 = string.Format("{0:00.00}", frac);
                        str = est[j].Substring(0, 4) + impu[j].ToString() + "P" + pol[j].ToString();
                        //str += pes[j][0].ToString() + " " + fe;
                        str += pes[j][0].ToString() + " " + fe + fe1;
                        if (sp > 0)
                        {
                            //sp += double.Parse(fe.Substring(10, 5));
                            sp += double.Parse(fe1.Substring(0, 5));
                            if (sp < 100.0) str += "       " + string.Format("{0:00.00}", sp);
                            else str += "      " + string.Format("{0:000.0}", sp);
                            str += " S " + pes[j][1].ToString();
                        }
                        else str += "       00.00 S 4";
                        str += string.Format("{0,37:0.0}", cod);
                        rsnc.WriteLine(str);
                        i += 1;
                    }
                }
                rsnc.Close();
            }

            return;
        }

        void GrabaLocHtml(string lin)
        {
            int an;
            double lat, lon, z, mg;
            char cc = '"';
            string nom, carpeta, li = "", linea;
           
            carpeta = fr1.rutHtml + "\\" + fr1.carpHtml + "_" + fr1.sismo[9];
            if (!Directory.Exists(carpeta)) return;
            nom = carpeta + "\\sismo.xml";
            if (!File.Exists(nom)) return;

            an = int.Parse(lin.Substring(0, 2));
            if (an < 85) an += 2000;
            else an += 1900;
            lat = double.Parse(lin.Substring(17,3))+(double.Parse(lin.Substring(20,6))/60.0)+fr1.la84;
            lon = double.Parse(lin.Substring(26,4))+(double.Parse(lin.Substring(30,6))/60.0)+fr1.lo84;
            z = double.Parse(lin.Substring(36, 7));
            if (lin.Length > 105 && lin[105] == '.') mg = double.Parse(lin.Substring(102, 5));
            else if (lin[47] == '.') mg = double.Parse(lin.Substring(44, 6));
            else mg = -5.0;
            linea = "  <sismo fecha=" + cc + an.ToString() + "-" + lin.Substring(2, 2) + "-" + lin.Substring(4, 2) + cc + " ";
            linea += "hora=" + cc + lin.Substring(7, 2) + ":" + lin.Substring(9, 2) + ":" + lin.Substring(12, 2) + cc + " ";
            linea += "latitud=" + cc + string.Format("{0:0.000000}", lat) + cc + " longitud= " + cc;
            linea += string.Format("{0:0.000000}", lon);
            linea += cc + " profundidad=" + cc + string.Format("{0:0.00}", z) + cc;
            linea += " magnitud=" + cc + string.Format("{0:0.00}", mg) + cc + " />";

            StreamWriter wr = File.CreateText("arch.xtm");
            wr.WriteLine(" <xml id='xmldata' style='display:none;'>");
            wr.WriteLine("  <sismos>");
            StreamReader ar = new StreamReader(nom);
            li = ar.ReadLine();
            li = ar.ReadLine();
            while (li != null)
            {
                try
                {
                    li = ar.ReadLine();
                    if (li == null) break;
                    if (li.Length >= 10 && li[10] == '>')
                    {
                        wr.WriteLine(linea);
                        break;
                    }
                    else wr.WriteLine(li);
                }
                catch
                {
                }
            }
            ar.Close();
            wr.WriteLine("  </sismos>");
            wr.WriteLine("</xml>");
            wr.Close();

            File.Copy("arch.xtm", nom, true);


            return;
        }

        private void boGraBas_Click(object sender, EventArgs e)
        {
            int i,j;

            GrabaBase();
            for (i = 0; i < nutra; i++)
            {
                for (j = 0; j < 3; j++) fas[i][j] = 0;
                pes[i][0] = 4;
                pes[i][1] = 4;
                pol[i] = ' ';
                impu[i] = 'I';
            }
            ond = 0;
            boP.BackColor = Color.Green;
            boS.BackColor = Color.White;
            boC.BackColor = Color.White;
            peso = 0;
            MenuPeso();
            panelpun.Visible = false;
            panelmapa.Visible = false;
            boGraBas.Visible = false;
            boWord.Visible = false;
            tmini = tminx;
            dura = durx;
            mejite = -1;
            mejpun = "";

            panel1.Invalidate();
        }


        void Reviza()
        {
            int cont=0,ye,me,di,ho,mi,se,xf;
            long ll;
            float x1,w,h;
            double tmn,tmx,fax,dd;
            string nom="",fe="",lin="",linea="",pun="";
            bool si=false,sipun=false,siloc=false;


            tmn = tminx;
            tmx = tmn + durx;
            xf = panel1.Size.Width - 40;
            h=panel1.Size.Height-2;
            fax = xf / dura;

            if (!File.Exists("reviza.txt"))
            {
                ll = (long)(Fei + tminx * 10000000.0);
                DateTime fech = new DateTime(ll);
                fe = string.Format("{0:yy}{0:MM}{0:dd}{0:HH}{0:mm}{0:ss}.{0:ff}", fech);
                nom = fr1.rutbas + "\\loc\\" + fe.Substring(0, 4) + ".ipn";

                StreamWriter pr = File.CreateText("reviza.txt");
                StreamWriter pr2 = File.CreateText("locabas.txt");
                File.Copy(nom, "localoca.txt", true);
                StreamReader ar = new StreamReader("localoca.txt");

                while (lin != null)
                {
                    try
                    {
                        lin = ar.ReadLine();
                        if (lin == null) break;
                        if (char.IsLetter(lin[0]))
                        {
                            si = false;
                            linea = "";
                            do
                            {
                                ye = int.Parse(lin.Substring(9, 2));
                                if (ye < 80) ye += 2000;
                                else ye += 1900;
                                me = int.Parse(lin.Substring(11, 2));
                                di = int.Parse(lin.Substring(13, 2));
                                ho = int.Parse(lin.Substring(15, 2));
                                mi = int.Parse(lin.Substring(17, 2));
                                se = int.Parse(lin.Substring(19, 2));
                                DateTime fe2 = new DateTime(ye, me, di, ho, mi, se);
                                ll = fe2.ToBinary();
                                dd = ((double)(ll) - Feisuds) / 10000000.0;
                                if (dd > tmn && dd < tmx)
                                {
                                    if (sipun == true) pr2.WriteLine(pun);
                                    pr2.WriteLine(lin);
                                    siloc = true;
                                    sipun = false;
                                    linea += string.Format("{0,12:0}", dd);
                                    cont += 1;
                                    si = true;                                    
                                }
                                lin = ar.ReadLine();
                            } while (char.IsLetter(lin[0]));
                            if (si == true) pr.WriteLine(linea);
                        }
                        else if (char.IsDigit(lin[0]))
                        {
                            pun = lin;
                            sipun = true;
                            siloc = false;
                        }
                        else if (siloc == true)
                        {
                            pr2.WriteLine(lin);
                            siloc = false;
                        }
                    }
                    catch
                    {
                    }
                }

                ar.Close();
                pr.Close();
                pr2.Close();
            }// if !file.Exist....
            else cont = 10;
            if (cont == 0 && File.Exists("reviza.txt"))
            {
                File.Delete("reviza.txt");
                return;
            }

            Graphics dc = panel1.CreateGraphics();
            Pen lapiz = new Pen(Color.Gray,2);
            lin = "";
            StreamReader ar2 = new StreamReader("reviza.txt");
            while (lin != null)
            {
                try
                {
                    lin = ar2.ReadLine();
                    if (lin == null) break;
                    dd = double.Parse(lin.Substring(0,12));
                    if (dd > tmini && dd < (tmini+dura))
                    {
                        x1 = (float)(40.0 + (dd - tmini) * fax);
                        w = (float)(40.0 + 2.0 * fax);
                        dc.DrawRectangle(lapiz, x1, 1, w, h);
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

        private void boRevi_Click(object sender, EventArgs e)
        {
            Reviza();
            return;
        }

        private void boTeo_MouseDown(object sender, MouseEventArgs e)
        {

            if (siteo == false)
            {
                boTeo.Visible = false;
                verTeo = false;
                return;
            }
            if (verTeo == false)
            {
                boTeo.BackColor = Color.MediumPurple;
                verTeo = true;
            }
            else
            {
                boTeo.BackColor = Color.WhiteSmoke;
                verTeo = false;
            }
            panel1.Invalidate();
            if (panelOrd.Visible == true) panelOrd.Invalidate();

            return;
        }

        private void boWada_Click(object sender, EventArgs e)
        {

            if (panelWada.Visible == false)
            {
                panelWada.Visible = true;
                panelWada.BringToFront();
                boWada.BackColor = Color.Violet;
               // panel1.Invalidate();
            }
            else
            {
                panelWada.Visible = false;
                boWada.BackColor = Color.White;
            }

            return;
        }

        private void panelWada_MouseDown(object sender, MouseEventArgs e)
        {
            int i, j, tot, xf, yf;
            double facp, facs, maxwp, minwp, maxws, minws, difp, difs, wP, wS, ivp, ivs;
            double[] wp, ws;
            string[] west;
            char[] delim = { ' ', '\t' };
            string[] pa = null;
            ArrayList lista = new ArrayList();
            ToolTip tip = new ToolTip();

            lista = Wadati();
            tot = lista.Count;
            if (tot <= 0) return;

            west = new string[tot];
            wp = new double[tot];
            ws = new double[tot];
            for (i = 0; i < tot; i++)
            {
                pa = lista[i].ToString().Split(delim);
                west[i] = pa[0].Substring(0, 4);
                wp[i] = double.Parse(pa[1]);
                ws[i] = double.Parse(pa[2]);
            }
            minwp = wp[0];
            minws = ws[0]; ;
            maxwp = wp[0];
            maxws = ws[0];
            for (i = 1; i < tot; i++)
            {
                if (maxwp < wp[i]) maxwp = wp[i];
                else if (minwp > wp[i]) minwp = wp[i];
                if (maxws < ws[i]) maxws = ws[i];
                else if (minws > ws[i]) minws = ws[i];
            }
            difp = maxwp - minwp;
            if (difp <= 0) difp = 1.0;
            difs = maxws - minws;
            if (difs <= 0) difs = 1.0;

            xf = panelWada.Size.Width;
            yf = panelWada.Size.Height;
            facp = 0.7 * xf / difp;
            facs = 0.85 * yf / difs;

            wP = minwp + ((double)(e.X) - 5.0) / facp;
            wS = minws + ((double)(yf - e.Y) - 15.0) / facs;
            ivp = 5.0 / facp;
            ivs = 5.0 / facs;

            j = -1;
            for (i = 0; i < tot; i++)
            {
                difp = Math.Abs(wp[i] - wP);
                difs = Math.Abs(ws[i] - wS);
                //MessageBox.Show("difp="+difp.ToString()+" ivp="+ivp.ToString()+" difs="+difs.ToString()+" ivs="+ivs.ToString());              
                if (difp <= ivp && difs <= ivs)
                {
                    j = i;
                    break;
                }

            }
            if (j == -1) return;

            tip.IsBalloon = true;
            tip.InitialDelay = 0;
            tip.ReshowDelay = 0;
            tip.AutomaticDelay = 0;
            tip.SetToolTip(panelWada, west[j]);
        }

        private void panelWada_Paint(object sender, PaintEventArgs e)
        {
            Wadati();
            return;
        }

        ArrayList Wadati()
        {
            int i, j = 0, k, to, xf, yf, estilo = 0, menos;
            double minp = 0, difs = 0, difp = 0, facp = 0, facs = 0;
            double maxwp, maxws, minwp, minws;
            double Sx, Sy, Sxy, Sxx, M, Cor, dd, Rela, Interc, pm, spm, xx;
            float x1, y1, x2, y2;
            double[] wp, ws;
            string[] west;
            bool[] presente;
            Color color;
            Color[] col = new Color[26];
            string ca = "";
            ArrayList lista = new ArrayList();


            col[0] = Color.Red;
            col[1] = Color.Cyan;
            col[2] = Color.Gold;
            col[3] = Color.Chocolate;
            col[4] = Color.Fuchsia;
            col[5] = Color.Gray;
            col[6] = Color.LimeGreen;
            col[7] = Color.Orange;
            col[8] = Color.SteelBlue;
            col[9] = Color.Peru;
            col[10] = Color.DarkRed;
            col[11] = Color.Green;
            col[12] = Color.MediumAquamarine;
            col[13] = Color.Blue;
            col[14] = Color.Violet;
            col[15] = Color.GreenYellow;
            col[16] = Color.RosyBrown;
            col[17] = Color.Pink;
            col[18] = Color.Thistle;
            col[19] = Color.OrangeRed;
            col[20] = Color.Olive;
            col[21] = Color.RoyalBlue;
            col[22] = Color.MediumPurple;
            col[23] = Color.Goldenrod;
            col[24] = Color.IndianRed;
            col[25] = Color.LightGray;
            to = 0;
            for (i = 0; i < nutra; i++)
            {
                if (fas[i][0] > 0 && fas[i][1] > 0)
                {
                    if (to == 0) minp = fas[i][0];
                    else if (minp > fas[i][0]) minp = fas[i][0];
                    to += 1;
                }
            }
            if (to < 2) return (lista);
            wp = new double[to];
            ws = new double[to];
            west = new string[to];
            presente = new bool[to];
            j = 0;
            k = 0;
            for (i = 0; i < nutra; i++)
            {
                if (fas[i][0] > 0 && fas[i][1] > 0)
                {
                    wp[j] = fas[i][0] - minp;
                    ws[j] = fas[i][1] - fas[i][0];
                    west[j] = est[i];
                    if (siEst[i] == true) presente[j] = true;
                    else presente[j] = false;
                    j += 1;

                }
            }

            minwp = wp[0];
            minws = ws[0]; ;
            maxwp = wp[0];
            maxws = ws[0];
            for (i = 1; i < to; i++)
            {
                if (maxwp < wp[i]) maxwp = wp[i];
                else if (minwp > wp[i]) minwp = wp[i];
                if (maxws < ws[i]) maxws = ws[i];
                else if (minws > ws[i]) minws = ws[i];
            }
            difp = maxwp - minwp;
            if (difp <= 0) difp = 1.0;
            difs = maxws - minws;
            if (difs <= 0) difs = 1.0;

            Graphics dc = panelWada.CreateGraphics();
            SolidBrush brocha2 = new SolidBrush(Color.FloralWhite);
            xf = panelWada.Size.Width;
            yf = panelWada.Size.Height;
            dc.FillRectangle(brocha2, 0, 0, xf, yf);
            brocha2.Dispose();
            SolidBrush brocha3 = new SolidBrush(Color.Black);
            dc.DrawString("Ts-Tp", new Font("Times New Roman", 10), brocha3, 1, 1);
            dc.DrawString("Tp", new Font("Times New Roman", 10), brocha3, xf - 25, yf - 20);
            brocha3.Dispose();
            facp = 0.7 * xf / difp;
            facs = 0.85 * yf / difs;
            j = 0;
            menos = 45;
            k = 0;
            for (i = 0; i < to; i++)
            {
                ca = west[i].Substring(0, 4);
                if (presente[i] == true && i < 25) color = col[k++];
                else color = col[25];
                SolidBrush brocha = new SolidBrush(color);
                Pen lapiz = new Pen(color, 2);
                x1 = 5.0F + (float)((wp[i] - minwp) * facp);
                y1 = (yf - 15.0F) - (float)((ws[i] - minws) * facs);
                if (estilo == 0 || estilo == 2) dc.DrawRectangle(lapiz, x1, y1, 6, 6);
                else if (estilo == 1 || estilo == 3) dc.DrawEllipse(lapiz, x1, y1, 7, 7);
                if (estilo == 2) dc.FillRectangle(brocha, x1, y1, 6, 6);
                else if (estilo == 3) dc.FillEllipse(brocha, x1, y1, 7, 7);
                dc.DrawString(ca, new Font("Times New Roman", 9), brocha, xf - menos, 1 + i * 11);
                lapiz.Dispose();
                brocha.Dispose();
                j += 1;
                if (j > 25)
                {
                    j = 0;
                    estilo += 1;
                    if (estilo > 3) return (lista);
                }
                if (i > 56) menos = 100;
            }

            pm = 0;
            spm = 0;
            for (i = 0; i < to; i++)
            {
                pm += wp[i];
                spm += ws[i];
            }
            pm = pm / (double)(to);
            spm = spm / to;
            Sxy = 0;
            Sxx = 0;
            for (i = 0; i < to; i++)
            {
                Sxy += (wp[i] - pm) * (ws[i] - spm);
                Sxx += (wp[i] - pm) * (wp[i] - pm);
            }
            M = Sxy / Sxx;
            Rela = M + 1.0;
            dd = 0;
            for (i = 0; i < to; i++) dd += ((wp[i] - pm) * (wp[i] - pm));
            dd = dd / (double)(to - 1);
            Sx = Math.Sqrt(dd);
            dd = 0;
            for (i = 0; i < to; i++) dd += ((ws[i] - spm) * (ws[i] - spm));
            dd = dd / (double)(to - 1);
            Sy = Math.Sqrt(dd);
            dd = 0;
            for (i = 0; i < to; i++) dd += ((wp[i] - pm) / Sx) * ((ws[i] - spm) / Sy);
            Cor = (1.0 / (to - 1.0)) * dd;
            Interc = spm - M * pm;

            Pen lap2 = new Pen(Color.DarkOrange, 3);
            lap2.DashStyle = DashStyle.DashDot;
            SolidBrush br = new SolidBrush(Color.DarkOrange);

            xx = minwp;
            dd = (Rela - 1.0) * xx + Interc;
            x1 = 5.0F + (float)((xx - minwp) * facp);
            y1 = (yf - 15.0F) - (float)((dd - minws) * facs);
            xx = maxwp + 0.2 * (maxwp - minwp);
            dd = (Rela - 1.0) * xx + Interc;
            x2 = 5.0F + (float)((xx - minwp) * facp);
            y2 = (yf - 15.0F) - (float)((dd - minws) * facs);
            dc.DrawLine(lap2, x1, y1, x2, y2);
            ca = "Vp/Vs= " + string.Format("{0:0.000}", Rela);
            ca += "\n";
            ca += "Coef correl: " + string.Format("{0:0.000000}", Cor);
            dc.DrawString(ca, new Font("Lucida Console", 10, FontStyle.Bold), br, 10, 30);
            lap2.Dispose();
            br.Dispose();

            lista.Clear();
            for (i = 0; i < to; i++)
            {
                ca = west[i].Substring(0, 4) + " " + string.Format("{0:0.000000} {1:0.000000}", wp[i], ws[i]);
                lista.Add(ca);
            }

            return (lista);
        }      
               
        private void boWord_Click(object sender, EventArgs e)
        {            
            int   i,j,k,kk,nuvo,nucap,nutit,iniletra,cantletra;
            int   an, me, di, ho, mi, se;
            long  ll;
            double lasi,losi,dif,difla,diflo,min,mincap,fcpi,fcdislo,latitud;
            double grados, diflaa, difloo;
            double[] lavo,lovo,lacap,locap;
            string[] letvol,letcap;
            string[] lettit = new string[1];
            string ca="",lin="",levol="",lecap="",rosa="";
            char cala, calo;
            char[] delim = { ' ', '\t' };
            string[] pa = null;
            bool sirosa = false, sivolcan = false, sicap = false;
            DateTime fech;

            
            if (mejpun == "") return;

            if (!File.Exists(".\\rep\\volcan.txt")) return;
            fcpi = Math.PI / 180.0; 
            nuvo = 0;
            nutit = 0;            
            StreamReader ar1 = new StreamReader(".\\rep\\volcan.txt");
            while (lin != null)
            {
                try
                {
                    lin = ar1.ReadLine();
                    if (lin == null) break;
                    pa = lin.Split(delim);
                    if (char.IsDigit(pa[0][0])|| pa[0][0]=='-') nuvo += 1;
                    else if (char.IsLetter(pa[0][0])) nutit += 1;
                }
                catch
                {
                    break;
                }
            }
            ar1.Close();
            lavo = new double[nuvo];
            lovo = new double[nuvo];
            letvol=new string[nuvo];
            if (nutit > 0)
            {
                lettit = new string[nutit];
                lettit[0] = "";
            }
            lin = "";
            i = 0;
            j = 0;

            StreamReader ar = new StreamReader(".\\rep\\volcan.txt",Encoding.Default,true);
            while (lin != null)
            {
                try
                {
                    lin = ar.ReadLine();
                    if (lin == null) break;
                    pa = lin.Split(delim);
                    k = pa.Length;
                    if ((char.IsDigit(pa[0][0]) || pa[0][0]=='-') && k >= 2)
                    {
                        lavo[i] = double.Parse(pa[0]);
                        lovo[i] = double.Parse(pa[1]);
                        if (k > 2) for (kk = 2; kk < k; kk++) letvol[i] += pa[kk] + " ";
                        i += 1;
                    }
                    else if (char.IsLetter(pa[0][0])) lettit[j++] = lin;
                }
                catch
                {
                    break;
                }
            }
            ar.Close();

            if (!File.Exists(".\\rep\\capitales.txt")) return;

            lin = "";
            nucap = 0;
            StreamReader pr1 = new StreamReader(".\\rep\\capitales.txt",Encoding.Default,true);
            while (lin != null)
            {
                try
                {
                    lin = pr1.ReadLine();
                    if (lin == null) break;
                    pa = lin.Split(delim);
                    if (char.IsDigit(pa[0][0]) || pa[0][0]=='-') nucap += 1;
                }
                catch
                {
                    break;
                }
            }
            pr1.Close();
            lacap = new double[nucap];
            locap = new double[nucap];
            letcap = new string[nucap];
            lin = "";
            i = 0;
            StreamReader pr = new StreamReader(".\\rep\\capitales.txt",Encoding.Default,true);
            while (lin != null)
            {
                try
                {
                    lin = pr.ReadLine();
                    if (lin == null) break;
                    pa = lin.Split(delim);
                    k = pa.Length;
                    if (char.IsDigit(pa[0][0]) || pa[0][0] == '-')
                    {
                        lacap[i] = double.Parse(pa[0]);
                        locap[i] = double.Parse(pa[1]);
                        if (k > 2) for (kk = 2; kk < k; kk++) letcap[i] += pa[kk] + " ";
                    }
                    i += 1;
                }
                catch
                {
                    break;
                }
            }
            pr.Close();

            lasi = double.Parse(mejpun.Substring(17, 3)) + double.Parse(mejpun.Substring(20, 6)) / 60.0;
            losi = double.Parse(mejpun.Substring(27, 3)) + double.Parse(mejpun.Substring(30, 6)) / 60.0;
            lasi = double.Parse(mejpun.Substring(17, 3)) + double.Parse(mejpun.Substring(20, 6)) / 60.0;
            losi = double.Parse(mejpun.Substring(27, 3)) + double.Parse(mejpun.Substring(30, 6)) / 60.0;
            latitud = (lasi + lavo[0]) / 2.0;
            fcdislo = (Math.PI / 180.0) * Math.Cos(latitud * fcpi) * 6367.449;
            difla = (lasi - lavo[0]) * 110.9;
            diflo = (losi - lovo[0]) * fcdislo;
            diflaa = difla;
            difloo = diflo;

            min = Math.Sqrt(difla * difla + diflo * diflo);
            levol = letvol[0];
            j = 0;
            for (i = 1; i < nuvo; i++)
            {
                latitud = (lasi + lavo[i]) / 2.0;
                fcdislo = (Math.PI / 180.0) * Math.Cos(latitud * fcpi) * 6367.449;
                difla = (lasi - lavo[i]) * 110.9;
                diflo = (losi - lovo[i]) * fcdislo;
                dif = Math.Sqrt(difla * difla + diflo * diflo);
                if (dif < min)
                {
                    min = dif;
                    levol = letvol[i];
                    diflaa = difla;
                    difloo = diflo;
                    j = i;
                }
            }
            latitud = (lasi + lacap[0]) / 2.0;
            fcdislo = (Math.PI / 180.0) * Math.Cos(latitud * fcpi) * 6367.449;
            difla = (lasi - lacap[0]) * 110.9;
            diflo = (losi - locap[0]) * fcdislo;
            mincap = Math.Sqrt(difla * difla + diflo * diflo);
            //MessageBox.Show("lasi="+lasi.ToString()+" losi="+losi.ToString()+" lavo="+lavo[j].ToString()+" lovo0="+lovo[j].ToString()+" let="+letvol[j]);
            lecap = letcap[0];
            for (i = 1; i < nucap; i++)
            {
                latitud = (lasi + lacap[i]) / 2.0;
                fcdislo = (Math.PI / 180.0) * Math.Cos(latitud * fcpi) * 6367.449;
                difla = (lasi - lacap[i]) * 110.9;
                diflo = (losi - locap[i]) * fcdislo;
                dif = Math.Sqrt(difla * difla + diflo * diflo);
                if (dif < mincap)
                {
                    mincap = dif;
                    lecap = letcap[i];
                }
            }

            grados = Math.Asin(difloo / min) * (180.0 / Math.PI);
            if (grados >= 0)
            {
                if (diflaa < 0) grados = 180.0 - grados;
            }
            else
            {
                if (diflaa < 0) grados = 180.0 + Math.Abs(grados);
                else grados = 360.0 + grados;
            }   
            // MessageBox.Show("grados="+grados.ToString()+" difla="+difla.ToString()+" diflo="+diflo.ToString());
            if (grados < 22.5 || grados >= 337.5) rosa = "N";
            else if (grados >= 22.5 && grados < 67.5) rosa = "NE";
            else if (grados >= 67.5 && grados < 112.5) rosa = "E";
            else if (grados >= 112.5 && grados < 175.5) rosa = "SE";
            else if (grados >= 157.5 && grados < 202.5) rosa = "S";
            else if (grados >= 202.5 && grados < 247.5) rosa = "SW";
            else if (grados >= 247.5 && grados < 292.5) rosa = "W";
            else if (grados >= 292.5 && grados < 337.5) rosa = "NW";
            else rosa = "?";

            myPicture = CreatePicture();
            myPicture.Save("c:\\imagen.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);

            an = int.Parse(mejpun.Substring(0, 2));
            if (an < 85) an += 2000;
            else an += 1900;
            me = int.Parse(mejpun.Substring(2, 2));
            di = int.Parse(mejpun.Substring(4, 2));
            ho = int.Parse(mejpun.Substring(7, 2));
            mi = int.Parse(mejpun.Substring(9, 2));
            se = int.Parse(mejpun.Substring(12, 2));
            if (fr1.local != 0)
            {
                DateTime fech1 = new DateTime(an, me, di, ho, mi, se);
                ll = fech1.Ticks;
                ll += fr1.local * 36000000000;
                fech = new DateTime(ll);
            }
            else
            {
                fech = new DateTime(an, me, di, ho, mi, se);
            }
            object oMissing = System.Reflection.Missing.Value;
            object oEndOfDoc = "\\endofdoc"; // \endofdoc is a predefined bookmark 
            //Start Word and create a new document.
            Word._Application oWord;
            Word._Document oDoc;
            oWord = new Word.Application();
            oWord.Visible = true;
            object oTemplate = "c:\\reporte.doc";
            oDoc = oWord.Documents.Add(ref oTemplate, ref oMissing, ref oMissing, ref oMissing);
            //oDoc = oWord.Documents.Add(ref oMissing,ref oMissing,ref oMissing,ref oMissing);
            //Insert a paragraph at the beginning of the document.
            Word.Paragraph oPara1;
            oPara1 = oDoc.Content.Paragraphs.Add(ref oMissing);
            oPara1.Range.Text = " ";
            oPara1.Range.Font.Bold = 1;
            // oPara1.Format.SpaceAfter = 20;    //4 pt spacing after paragraph.
            oPara1.Format.SpaceAfter = 5;    //4 pt spacing after paragraph.
            oPara1.Range.InsertParagraphAfter();
            oPara1.Range.Text = ca;            
            oPara1.Range.Font.Bold = 1;
            //oPara1.Format.SpaceAfter = 50;    //4 pt spacing after paragraph.
            oPara1.Format.SpaceAfter = 5;    //4 pt spacing after paragraph.
            oPara1.Range.InsertParagraphAfter();

            if (File.Exists(".\\rep\\texto.txt"))
            {
                lin = "";
                StreamReader pr2 = new StreamReader(".\\rep\\texto.txt", Encoding.Default, true);
                while (lin != null)
                {
                    try
                    {
                        lin = pr2.ReadLine();
                        if (lin == null) break;
                        pa = lin.Split(delim);
                        k = pa.Length;
                        if (string.Compare("DIA", pa[0]) == 0)
                        {
                            for (i = 1; i < k; i++)
                            {
                                if (pa[i][0] != '_') ca += pa[i] + " ";
                                else ca += string.Format("{0:d}", fech) + " ";
                            }
                        }
                        else if (string.Compare("HORA", pa[0]) == 0)
                        {
                            for (i = 1; i < k; i++)
                            {
                                if (pa[i][0] != '_') ca += pa[i] + " ";
                                else ca += string.Format("{0:T}", fech) + " ";
                            }
                        }
                        else if (string.Compare("UBICACION", pa[0]) == 0)
                        {
                            j = 1;
                            sirosa = false;
                            sivolcan = false;
                            if (string.Compare("ROSA", pa[1]) == 0)
                            {
                                sirosa = true;
                                j += 1;
                            }
                            if (string.Compare("VOLCAN", pa[j]) == 0)
                            {
                                sivolcan = true;
                                j += 1;
                            }
                            //MessageBox.Show("sivolcan="+sivolcan.ToString()+" levol=" + levol);
                            kk = 0;
                            for (i = j; i < k; i++)
                            {
                                if (pa[i][0] != '_') ca += pa[i] + " ";
                                else
                                {
                                    if (kk == 0)
                                    {
                                        ca += string.Format("{0:0.0}", min) + " ";
                                        kk += 1;
                                    }
                                    else if (sirosa == true)
                                    {
                                        ca += rosa + " ";
                                        sirosa = false;
                                    }
                                    else if (sivolcan == true) ca += levol + " ";
                                }
                            }
                        }// ubicacion
                        else if (lin[0] == '*')
                        {
                            if (lin.Length > 3) ca += lin.Substring(2);
                            ca += "\n";
                        }
                        else if (string.Compare("TIEMPO ORIGEN", lin.Substring(0, 13)) == 0)
                        {
                            for (i = 2; i < k; i++)
                            {
                                if (pa[i][0] != '_') ca += pa[i] + " ";
                                else ca += string.Format("{0:T} ", fech);
                            }
                            ca += "\n";
                        }
                        else if (string.Compare("LATITUD", lin.Substring(0, 7)) == 0)
                        {
                            if (mejpun[17] == '-' || mejpun[18] == '-') cala = 'S';
                            else cala = 'N';
                            if (char.IsDigit(mejpun[18]))
                            {
                                iniletra = 18;
                                cantletra = 2;
                            }
                            else
                            {
                                iniletra = 19;
                                cantletra = 1;
                            }
                            for (i = 1; i < k; i++)
                            {
                                if (pa[i][0] != '_') ca += pa[i] + " ";
                                else ca += mejpun.Substring(iniletra, cantletra) + "° " + mejpun.Substring(21, 5) + "' " + cala;
                            }
                            ca += "\n";
                        }
                        else if (string.Compare("LONGITUD", lin.Substring(0, 8)) == 0)
                        {
                            if (mejpun[26] == '-' || mejpun[27] == '-' || mejpun[28] == '-') calo = 'W';
                            else calo = 'E';
                            if (char.IsDigit(mejpun[27]))
                            {
                                iniletra = 27;
                                cantletra = 3;
                            }
                            else if (char.IsDigit(mejpun[28]))
                            {
                                iniletra = 28;
                                cantletra = 2;
                            }
                            else
                            {
                                iniletra = 29;
                                cantletra = 1;
                            }
                            for (i = 1; i < k; i++)
                            {
                                if (pa[i][0] != '_') ca += pa[i] + " ";
                                else ca += mejpun.Substring(iniletra, cantletra) + "° " + mejpun.Substring(31, 5) + "' " + calo;
                            }
                            ca += "\n";
                        }
                        else if (string.Compare("PROFUNDIDAD", lin.Substring(0, 11)) == 0)
                        {
                            for (i = 1; i < k; i++)
                            {
                                if (pa[i][0] != '_') ca += pa[i] + " ";
                                else ca += mejpun.Substring(37,6) + " ";
                            }
                            ca += "\n";
                        }
                        else if (string.Compare("MAGNITUD", lin.Substring(0, 8)) == 0)
                        {
                            for (i = 1; i < k; i++)
                            {
                                if (pa[i][0] != '_') ca += pa[i] + " ";
                                else
                                {
                                    if (ML > -10.0) ca += string.Format("{0:0.0}", ML);
                                    else ca += mejpun.Substring(45,4) + " ";
                                }
                            }
                            ca += "\n";
                        }
                        else if (string.Compare("CERCANIA", lin.Substring(0, 8)) == 0)
                        {
                            sicap = true;
                            for (i = 1; i < k; i++)
                            {
                                if (pa[i][0] != '_') ca += pa[i] + " ";
                                else if (sicap == true)
                                {
                                    ca += lecap;
                                    sicap = false;
                                }
                                else ca += string.Format("{0:0.0} ", mincap);
                            }
                            ca += "\n";
                        }
                    }
                    catch
                    {
                        break;
                    }
                }
                pr2.Close();
            }
            else
            {
                ca = string.Format("El día {0:d} a las {0:T} hora Local se registró un sismo de Magnitud", fech);
                ca += " " + mejpun.Substring(45, 5) + " en la escala de Richter, con epicentro a ";
                ca += string.Format("{0:0.0}", min) + " km " + levol;
            }
            //MessageBox.Show("levol=" + levol);
            oPara1.Range.Text = ca;
            oPara1.Range.Font.Bold = 1;
            //oPara1.Format.SpaceAfter = 50;    //4 pt spacing after paragraph.
            oPara1.Format.SpaceAfter = 5;    //4 pt spacing after paragraph.
            oPara1.Range.InsertParagraphAfter();
            if (!File.Exists(".\\rep\\texto.txt"))
            {
                if (nutit > 0)
                {
                    oPara1.Range.Text = lettit[0];
                    oPara1.Range.Font.Bold = 1;
                    oPara1.Format.SpaceAfter = 5;    //24 pt spacing after paragraph.
                    oPara1.Range.InsertParagraphAfter();
                }
                ca = "Latitud: " + mejpun.Substring(17, 3) + "° " + mejpun.Substring(20, 6) + "'\n";
                ca += "Longitud: " + mejpun.Substring(26, 4) + "° " + mejpun.Substring(30, 6) + "'\n";
                ca += "Profundidad: " + mejpun.Substring(37, 6) + " Km\n";
                if (mejpun.Length >= 107) ca += "Magnitud: " + mejpun.Substring(103, 4) + " en la escala de Richter\n";
                else ca += "Magnitud: " + mejpun.Substring(46, 4) + " en la escala de Richter\n";
                ca += "Capital más cercana:  " + lecap + " a " + string.Format("{0:0.0} Km del epicentro", mincap);
                oPara1.Range.Text = ca;
                oPara1.Range.Font.Bold = 1;
                oPara1.Format.SpaceAfter = 5;    //4 pt spacing after paragraph.
                oPara1.Range.InsertParagraphAfter();
            }

            oDoc.InlineShapes.AddPicture("c:\\imagen.jpg", ref oMissing, ref oMissing, ref oMissing);
            return;
        }

        private Image CreatePicture()
        {
            PointF ulCorner1;
            PointF urCorner1;
            PointF llCorner1;
            int    inixim, iniyim, anchoim, altoim;
            float  alpha = 1.0F;
            double la1, la2, lo1, lo2, faclaim, facloim/*, fclo, fcpi*/;
            string ca2;
            Image Imagen = null;
            double latim1 = 0, latim2 = 0, lonim1 = 0, lonim2 = 0;

            int iniX, iniY, xf, yf, x1, y1, x2, y2, gra;
            float min;
            double f1, la, lo, lag1, lag2, log1, log2, fcpi, fclo;
            string linea = "", ca = "";
            string nombre;
            char[] delim = { ' ', '\t' };
            string[] pa = null;
            Color coltopo;
            GraphicsPath path;

            xf = 372;
            yf = 372;
            la = fr1.latvol[vol];
            lo = fr1.lonvol[vol];
            iniX = xf / 2;
            iniY = yf / 2;
            x1 = 0;
            y1 = 0;
            x2 = 0;
            y2 = 0;
            fcpi = Math.PI / 180.0;
            fclo = facm * ((Math.PI / 180.0) * Math.Cos(la * fcpi) * 6367.449) / 110.9;

            coltopo = Color.Black;
            Image canvas = new Bitmap(372, 372);
            Graphics dc = Graphics.FromImage(canvas);
            dc.Clear(Color.White);

            if (checkBoxRepIma.Checked == true)
            {
                ulCorner1 = new PointF(0.0F, 0.0F);
                urCorner1 = new PointF(canvas.Width, 0.0F);
                llCorner1 = new PointF(0.0F, canvas.Height);
                PointF[] destPara1 = { ulCorner1, urCorner1, llCorner1 };

                ca = ".\\ima\\" + fr1.volcan[vol].Substring(0, 1) + ".jpg";
                ca2 = ".\\ima\\" + fr1.volcan[0].Substring(0, 1) + ".txt";
                if (File.Exists(ca) && File.Exists(ca2))
                {
                    Imagen = Image.FromFile(ca);
                    linea = "";
                    StreamReader ar2 = new StreamReader(ca2);
                    try
                    {
                        linea = ar2.ReadLine();
                        pa = linea.Split(delim);
                        latim1 = Convert.ToDouble(pa[0]);
                        lonim1 = Convert.ToDouble(pa[1]);
                        linea = ar2.ReadLine();
                        pa = linea.Split(delim);
                        latim2 = Convert.ToDouble(pa[0]);
                        lonim2 = Convert.ToDouble(pa[1]);
                    }
                    catch
                    {
                        //MessageBox.Show("ERROR  "+linea);
                    }
                    ar2.Close();
                }
                else
                {
                    checkBoxRepIma.Checked = false;
                    //return;  //ojo aqui
                }
                if (checkBoxRepIma.Checked == true)
                {
                    try
                    {
                        lo1 = lo + (double)(1 - iniX) / fclo;
                        la1 = la + (double)(iniY - 1) / facm;
                        lo2 = lo + (double)(xf - iniX) / fclo;
                        la2 = la + (double)(iniY - yf) / facm;
                        faclaim = Imagen.Height / (latim1 - latim2);
                        facloim = Imagen.Width / (lonim1 - lonim2);
                        iniyim = (int)((latim1 - la1) * faclaim);
                        inixim = (int)((lonim1 - lo1) * facloim);
                        altoim = (int)((latim1 - la2) * faclaim) - iniyim;
                        anchoim = (int)((lonim1 - lo2) * facloim) - inixim;
                        GraphicsUnit units = GraphicsUnit.Pixel;
                        ImageAttributes imageAttr = new ImageAttributes();
                        ColorMatrix myColorMatrix = new ColorMatrix();
                        myColorMatrix.Matrix33 = alpha;
                        imageAttr.SetColorMatrix(myColorMatrix);
                        RectangleF srcRect = new RectangleF(inixim, iniyim, anchoim, altoim);
                        dc.DrawImage(Imagen, destPara1, srcRect, units, imageAttr);
                    }
                    catch
                    {
                    }
                }
            }

            path = new GraphicsPath();

            Pen lapiz = new Pen(coltopo, 1);
            Pen lapizVia = new Pen(coltopo, 2);
            Pen lapiz2 = new Pen(Color.Black, 1);
            Pen lapiz3 = new Pen(Color.Red, 2);
            SolidBrush brocha = new SolidBrush(coltopo);
            dc.DrawRectangle(lapiz2, 0, 0, xf - 1, yf - 1);

            if (checkBoxRepTop.Checked == true)
            {
                nombre = ".\\coor\\" + fr1.volcan[vol][0] + ".map";

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

                //lag = (double)(Math.Ceiling(la + iniY / facm));
                lag1 = la + iniY / facm;
                log1 = lo + (iniX - xf) / fclo;
                lag2 = la - iniY / facm;
                log2 = lo - (iniX - xf) / fclo;

                f1 = facm * (lag1 - la);
                y1 = iniY - (int)f1;
                gra = (int)(lag1);
                min = (float)((lag1 - (double)(gra)) * 60.0);
                ca = string.Format("{0:0}° {1:00}'", gra, min);
                //dc.DrawLine(lapiz, 1, y1, 15, y1);
                if (y1 < 5 && y1 >= 0)
                {
                    y1 = 5;
                    x1 = 1;
                }
                else if (y1 <= yf && y1 >= yf - 15)
                {
                    y1 = yf - 15;
                    x1 = xf - 35;
                }
                dc.DrawString(ca, new Font("Times New Roman", 10), brocha, x1, y1);

                f1 = facm * (lag2 - la);
                y2 = iniY - (int)f1;
                gra = (int)(lag2);
                min = (float)((lag2 - (double)(gra)) * 60.0);
                ca = string.Format("{0:0}° {1:00}'", gra, min);
                //dc.DrawLine(lapiz, xf-35, y2, xf, y2);
                if (y2 < 5 && y2 >= 0)
                {
                    y2 = 5;
                    x2 = 1;
                }
                else if (y2 <= yf && y2 >= yf - 15)
                {
                    y2 = yf - 15;
                    x2 = xf - 35;
                }
                dc.DrawString(ca, new Font("Times New Roman", 10), brocha, x2, y2);

                f1 = fclo * (log1 - lo);
                x1 = iniX + (int)f1;
                //dc.DrawLine(lapiz, x1, yf - 15, x1, yf);
                gra = (int)(log1);
                min = (float)((log1 - (double)(gra)) * 60.0);
                ca = string.Format("{0:0}° {1:00}'", gra, min);
                if (x1 < 5 && x1 >= 0)
                {
                    y1 = yf - 15;
                    x1 = 1;
                }
                else if (x1 <= xf && x1 >= xf - 15)
                {
                    y1 = 5;
                    x1 = xf - 45;
                }
                dc.DrawString(ca, new Font("Times New Roman", 10), brocha, x1, y1);

                f1 = fclo * (log2 - lo);
                x2 = iniX + (int)f1;
                //dc.DrawLine(lapiz, x2, 1, x2, 15);
                gra = (int)(log2);
                min = (float)((log2 - (double)(gra)) * 60.0);
                ca = string.Format("{0:0}° {1:00}'", gra, min);
                if (x2 < 5 && x2 >= 0)
                {
                    y2 = yf - 15;
                    x2 = 1;
                }
                else if (x2 <= xf && x2 >= xf - 15)
                {
                    y2 = 5;
                    x2 = xf - 450
                        ;
                }
                dc.DrawString(ca, new Font("Times New Roman", 10), brocha, x2, y2);
            }

            f1 = facm * (lasi - la);
            y1 = iniY - (int)f1;
            f1 = fclo * (losi - lo);
            x1 = iniX + (int)f1;
            dc.DrawRectangle(lapiz2, x1 - 6, y1 - 2, 11, 3);
            dc.DrawRectangle(lapiz2, x1 - 2, y1 - 6, 3, 11);
            dc.DrawLine(lapiz3, x1 + 5, y1, x1 - 5, y1);
            dc.DrawLine(lapiz3, x1, y1 - 5, x1, y1 + 5);

            brocha.Dispose();
            lapiz2.Dispose();
            lapiz.Dispose();
            lapizVia.Dispose();
            lapiz3.Dispose();
            // now the drawing is done, we can discard the artist object

            dc.Dispose();
            // return the picture

            return canvas;
        } 

        private Image CreatePicture2()
        {
            int iniX, iniY, xf, yf, x1, y1, x2, y2, gra;
            float min;
            double f1, la, lo, lag1, lag2, log1, log2, fcpi, fclo;
            string linea = "", ca = "";
            string nombre;
            char[] delim = { ' ', '\t' };
            string[] pa = null;
            Color coltopo;
            GraphicsPath path;


            coltopo = Color.Black;
            Image canvas = new Bitmap(372, 372);
            Graphics dc = Graphics.FromImage(canvas);
            dc.Clear(Color.White);

            path = new GraphicsPath();
            xf = 372;
            yf = 372;
            la = fr1.latvol[vol];
            lo = fr1.lonvol[vol];
            iniX = xf / 2;
            iniY = yf / 2;
            x1 = 0;
            y1 = 0;
            x2 = 0;
            y2 = 0;
            fcpi = Math.PI / 180.0;
            fclo = facm * ((Math.PI / 180.0) * Math.Cos(la * fcpi) * 6367.449) / 110.9;


            Pen lapiz = new Pen(coltopo, 1);
            Pen lapizVia = new Pen(coltopo, 2);
            Pen lapiz2 = new Pen(Color.Black, 1);
            Pen lapiz3 = new Pen(Color.Red, 2);
            SolidBrush brocha = new SolidBrush(coltopo);
            dc.DrawRectangle(lapiz2, 0, 0, xf - 1, yf - 1);

            nombre = ".\\coor\\" + fr1.volcan[vol][0] + ".map";

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

            //lag = (double)(Math.Ceiling(la + iniY / facm));
            lag1 = la + iniY / facm;
            log1 = lo + (iniX - xf) / fclo;
            lag2 = la - iniY / facm;
            log2 = lo - (iniX - xf) / fclo;

            f1 = facm * (lag1 - la);
            y1 = iniY - (int)f1;
            gra = (int)(lag1);
            min = (float)((lag1 - (double)(gra)) * 60.0);
            ca = string.Format("{0:0}° {1:00}'", gra, min);
            //dc.DrawLine(lapiz, 1, y1, 15, y1);
            if (y1 < 5 && y1 >= 0)
            {
                y1 = 5;
                x1 = 1;
            }
            else if (y1 <= yf && y1 >= yf - 15)
            {
                y1 = yf - 15;
                x1 = xf - 35;
            }
            dc.DrawString(ca, new Font("Times New Roman", 10), brocha, x1, y1);

            f1 = facm * (lag2 - la);
            y2 = iniY - (int)f1;
            gra = (int)(lag2);
            min = (float)((lag2 - (double)(gra)) * 60.0);
            ca = string.Format("{0:0}° {1:00}'", gra, min);
            //dc.DrawLine(lapiz, xf-35, y2, xf, y2);
            if (y2 < 5 && y2 >= 0)
            {
                y2 = 5;
                x2 = 1;
            }
            else if (y2 <= yf && y2 >= yf - 15)
            {
                y2 = yf - 15;
                x2 = xf - 35;
            }
            dc.DrawString(ca, new Font("Times New Roman", 10), brocha, x2, y2);

            f1 = fclo * (log1 - lo);
            x1 = iniX + (int)f1;
            //dc.DrawLine(lapiz, x1, yf - 15, x1, yf);
            gra = (int)(log1);
            min = (float)((log1 - (double)(gra)) * 60.0);
            ca = string.Format("{0:0}° {1:00}'", gra, min);
            if (x1 < 5 && x1 >= 0)
            {
                y1 = yf - 15;
                x1 = 1;
            }
            else if (x1 <= xf && x1 >= xf - 15)
            {
                y1 = 5;
                x1 = xf - 45;
            }
            dc.DrawString(ca, new Font("Times New Roman", 10), brocha, x1, y1);

            f1 = fclo * (log2 - lo);
            x2 = iniX + (int)f1;
            //dc.DrawLine(lapiz, x2, 1, x2, 15);
            gra = (int)(log2);
            min = (float)((log2 - (double)(gra)) * 60.0);
            ca = string.Format("{0:0}° {1:00}'", gra, min);
            if (x2 < 5 && x2 >= 0)
            {
                y2 = yf - 15;
                x2 = 1;
            }
            else if (x2 <= xf && x2 >= xf - 15)
            {
                y2 = 5;
                x2 = xf - 450
                    ;
            }
            dc.DrawString(ca, new Font("Times New Roman", 10), brocha, x2, y2);

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
            // now the drawing is done, we can discard the artist object
            dc.Dispose();
            // return the picture
            return canvas;
        }

        void Ordenar()
        {
            int i, j, inter, tem;
            int xf, yf, jj, k, kk, proo, dif = 0, tamlet;
            int nmi, nmf, numu;
            float x1, y1, iniy, ff;
            double fax, fay, fy, diff;
            Point[] dat;
            Pen[] lap = new Pen[3];

            cana = 0;
            for (i = 0; i < nutra; i++) if (fas[i][0] > 0) cana += 1;
            if (cana < 2) return;
            ord = new int[cana];
            j = 0;
            for (i = 0; i < nutra; i++) if (fas[i][0] > 0) ord[j++] = i;

            do
            {
                inter = 0;
                for (i = cana - 1; i > 0; --i)
                {
                    if (fas[ord[i - 1]][0] > fas[ord[i]][0])
                    {
                        tem = ord[i - 1];
                        ord[i - 1] = ord[i];
                        ord[i] = tem;
                        inter = 1;
                    }
                }
                for (i = 1; i < cana; ++i)
                {
                    if (fas[ord[i - 1]][0] > fas[ord[i]][0])
                    {
                        tem = ord[i - 1];
                        ord[i - 1] = ord[i];
                        ord[i] = tem;
                        inter = 1;
                    }
                }
            } while (inter == 1);

            // for (i = 0; i < cana; i++) ca+="i="+i.ToString()+" p="+fas[ord[i]][0].ToString();
            //util.borra(panelOrd, Color.Gainsboro);
            util.borra(panelOrd, Color.WhiteSmoke);
            xf = panelOrd.Size.Width;
            ff = (float)(xf);
            //yf = panelOrd.Size.Height - 40;
            yf = panelOrd.Size.Height - 10;
            numu = 0;
            fax = xf / duraord;
            fay = yf / cana;
            tamlet = (int)(fay);
            if (tamlet > 10) tamlet = 10;

            Graphics dc = panelOrd.CreateGraphics();
            Pen lapiz = new Pen(Color.Black, 1);
            Pen lapiz2 = new Pen(Color.DarkOrange, 1);
            lapiz2.DashStyle = DashStyle.DashDot;
            SolidBrush brocha = new SolidBrush(Color.Blue);
            SolidBrush broinv = new SolidBrush(Color.Gold);

            jj = 0;
            try
            {
                for (j = 0; j < cana; j++)
                {
                    // if (siEst[ord[j]] == true)
                    // {
                    proo = (int)((mx[ord[j]] + mn[ord[j]]) / 2.0F);
                    if (mx[ord[j]] - proo != 0) fy = ((fay / 2) / ((mx[ord[j]] - proo)));
                    else fy = 1.0;
                    proo = (int)(proo * amp);
                    iniy = (float)(jj * fay + fay / 2);
                    dc.DrawLine(lapiz2, 40.0F, iniy, ff, iniy);
                    dc.DrawString(est[ord[j]], new Font("Times New Roman", tamlet, FontStyle.Bold), brocha, 1, (float)(iniy - tamlet));
                    if (fr1.invertido[ord[j]] == true)
                    {
                        dc.FillEllipse(broinv, 1.0F, (float)(iniy - (tamlet + 2)), 5, 5);
                        dc.DrawEllipse(lapiz, 1.0F, (float)(iniy - (tamlet + 2)), 5, 5);
                    }
                    kk = 0;
                    nmi = (int)((tminiord - tim[ord[j]][0]) * ra[ord[j]]);
                    if (nmi < 0) nmi = 0;
                    nmf = (int)(((tminiord + duraord) - tim[ord[j]][0]) * ra[ord[j]]);
                    if (nmf > lar[ord[j]]) nmf = lar[ord[j]];
                    if (nmi >= nmf)
                    {
                        jj += 1;
                        continue;
                    }
                    numu = nmf - nmi;
                    dat = new Point[numu];
                    if (fr1.invertido[ord[j]] == false)
                    {
                        for (k = nmi; k < nmf; k++)
                        {
                            if (kk >= lar[ord[j]]) break;
                            dif = cu[ord[j]][k] - proo;
                            diff = dif * fy;
                            y1 = (float)(iniy - amp * diff);
                            x1 = (float)(40.0 + (tim[ord[j]][k] - tminiord) * fax);
                            dat[kk].Y = (int)y1;
                            dat[kk].X = (int)x1;
                            kk += 1;
                        }
                    }
                    else
                    {
                        for (k = nmi; k < nmf; k++)
                        {
                            if (kk >= lar[ord[j]]) break;
                            dif = proo - cu[ord[j]][k];
                            diff = dif * fy;
                            y1 = (float)(iniy - amp * diff);
                            x1 = (float)(40.0 + (tim[ord[j]][k] - tminiord) * fax);
                            dat[kk].Y = (int)y1;
                            dat[kk].X = (int)x1;
                            kk += 1;
                        }
                    }
                    dc.DrawLines(lapiz, dat);
                    jj += 1;
                    // }
                }
            }
            catch
            {
            }
            lapiz.Dispose();
            lapiz2.Dispose();
            brocha.Dispose();
            broinv.Dispose();

            ff = (float)(fay / 3.0);
            lap[0] = new Pen(Color.Green, 1);
            lap[1] = new Pen(Color.DeepSkyBlue, 1);
            lap[2] = new Pen(Color.Red, 1);

            for (i = 0; i < 3; i++)
            {
                jj = 0;
                for (j = 0; j < cana; j++)
                {
                    //if (siEst[ord[j]] == true)
                    // {
                    if (fas[ord[j]][i] > 0)
                    {
                        x1 = (float)(40.0 + (fas[ord[j]][i] - tminiord) * fax);
                        y1 = (float)(jj * fay + fay / 2);
                        dc.DrawLine(lap[i], x1, y1 - ff, x1, y1 + ff);
                    }
                    jj += 1;
                    //}
                }
            }
            lap[0].Dispose();
            lap[1].Dispose();
            lap[2].Dispose();

            if (verTeo == false) return;

            lap[0] = new Pen(Color.DarkSeaGreen, 1);
            lap[0].DashStyle = DashStyle.DashDot;
            lap[1] = new Pen(Color.DarkSlateBlue, 1);
            lap[1].DashStyle = DashStyle.DashDot;
            for (i = 0; i < 2; i++)
            {
                jj = 0;
                for (j = 0; j < cana; j++)
                {
                    //if (siEst[ord[j]] == true)
                    //{
                    if (teo[ord[j]][i] > 0)
                    {
                        x1 = (float)(40.0 + (teo[ord[j]][i] - tminiord) * fax);
                        y1 = (float)(jj * fay + fay / 2);
                        dc.DrawLine(lap[i], x1, y1 - ff, x1, y1 + ff);
                    }
                    jj += 1;
                    //}
                }
            }
            lap[0].Dispose();
            lap[1].Dispose();

            return;
        }       

        private void boOrd_Click(object sender, EventArgs e)
        {
            tminiord = tmini;
            duraord = dura;
            if (panelOrd.Visible == false)
            {
                panelOrd.Visible = true;
                boOrd.BackColor = Color.Chocolate;
                panelOrd.Invalidate();
            }
            else
            {
                panelOrd.Visible = false;
                boOrd.BackColor = Color.White;
            }
            return;
        }

        private void panelOrd_Paint(object sender, PaintEventArgs e)
        {
            Ordenar();
            if (panelWada.Visible == true) Wadati();
        }


        private void panelOrd_MouseDown(object sender, MouseEventArgs e)
        {
            ushort i, j, ii, jj;
            int xf, yf, oond;
            double fax, fay, ti1, ttt, dd;
            string ss = "";

            if (e.Button == MouseButtons.Right) oond = 2;
            else oond = ond;
            xf = panelOrd.Size.Width;
            yf = panelOrd.Size.Height - 10;
            fax = duraord / xf;
            ti1 = tminiord + ((e.X - 40) * fax);
            fay = yf / cana;
            ttt = e.Y / fay;
            ii = (ushort)(ttt);
            //ii = (ushort)(e.Y / fay);
            jj = 0;
            i = 0;
            for (j = 0; j < cana; j++)
            {
                if (jj == ii)
                {
                    i = j;
                    break;
                }
                jj += 1;
            }
            if (quita == false)
            {
                if (oond > 0 && (fas[ord[i]][0] == 0 || ti1 < fas[ord[i]][0]))
                {
                    if (fas[ord[i]][0] == 0) MessageBox.Show("No se ha leido la P !!!");
                    else if (ti1 < fas[ord[i]][0]) MessageBox.Show("Lectura MENOR que P ???");
                }
                else
                {
                    fas[ord[i]][oond] = ti1;                   
                    if (oond < 2) pes[ord[i]][oond] = peso;
                    dd = ttt - (double)(ii);
                    if (oond == 0)
                    {
                        if (dd < 0.5) pol[ord[i]] = 'C';
                        else pol[ord[i]] = 'D';
                    }
                    if (panelPola.Visible == true) panelPola.Invalidate();
                    impu[ord[i]] = imp;
                    if (imp == 'E')
                    {
                        imp = 'I';
                        boImp.Text = "I";
                    }
                    if (oond == 0) peso = 0;
                    else if (oond == 1) peso = 2;
                    MenuPeso();
                    ss = est[ord[i]].Substring(0, 4) + " " + pol[ord[i]].ToString();
                    tip.IsBalloon = false;
                    tip.ReshowDelay = 0;
                    tip.Show(ss, panelOrd, e.X + 5, e.Y - 15, 2000);
                    MarcasTi(panel1);
                }
            }
            CuentaS();
            panelOrd.Invalidate();

            return;
        }

        private void boGuia_Click(object sender, EventArgs e)
        {
            if (guia == false)
            {
                guia = true;
                boGuia.BackColor = Color.BlueViolet;
            }
            else
            {
                guia = false;
                boGuia.BackColor = Color.White;
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


        private void bordizq_Click(object sender, EventArgs e)
        {

            tminiord += 0.1 * duraord;
            if (tminiord+duraord>tminx+durx) tminiord -= 0.1*duraord;
            panelOrd.Invalidate();
            return;
        }

        private void border_Click(object sender, EventArgs e)
        {
            tminiord -= 0.1 * duraord;
            if (tminiord < tminx) tminiord = tminx;
            panelOrd.Invalidate();
            return;

        }

        private void bordagr_Click(object sender, EventArgs e)
        {
            duraord -= duraord * 0.1;
            panelOrd.Invalidate();
            return;
        }

        private void bordism_Click(object sender, EventArgs e)
        {
            double duur;

            duur=duraord;
            duraord += duraord * 0.1;
            if (tminiord + duraord > tminx + durx) duraord = duur;
            panelOrd.Invalidate();
            return;
        }

        private void bordini_Click(object sender, EventArgs e)
        {
            tminiord = tmini;
            duraord = dura;
            panelOrd.Invalidate();
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

        private void radbupro_CheckedChanged(object sender, EventArgs e)
        {
            ML_total();
        }

        private void radbumax_CheckedChanged(object sender, EventArgs e)
        {
            ML_total();
        }

        private void radbumin_CheckedChanged(object sender, EventArgs e)
        {
            ML_total();
        }

        void LeeMlAmp(int ni)
        {
            int i, j, ini;

            for (i = 0; i < 3; i++)
            {
                nuestml[i] = -1;
                for (j = 0; j < nutra; j++)
                {
                    if (char.IsLetter(est[j][0])) ini = 0;
                    else ini = 1;
                    if (string.Compare(est[j].Substring(ini, 4), estml[ni + i].Substring(2, 4)) == 0)
                    {
                        nuestml[i] = j;
                        break;
                    }
                }
                if (nuestml[i] == -1)
                {
                    MessageBox.Show("NO EXISTE " + estml[ni + i].Substring(2, 4));
                    idml = (short)(ni / 3.0);
                    boml[idml].Visible = false;
                    return;
                }
            }

            panelmltra.Visible = true;
            panelmltra.BringToFront();
            DibEstMl(nuestml, 0, 0, true);
            idml = (short)(ni / 3.0);
            caj[idml].Checked = true;

            return;
        }

        void DibEstMl(int[] nuest, double timl, double tfml, bool cond)
        {
            int i, j, k, xf, yf, ini, fin, proo, numu, kk;
            float iniy, x1, y1;
            double fax, fay, fy, tini = 0, tifin = 0, dd, dif, diff, ti1;
            string esta = "";
            Color col;
            Point[] dat;

            if (cond == true) util.borra(panelmltra, Color.Linen);
            if (timlini == 0)
            {
                for (i = 0; i < 3; i++)
                {
                    if (fas[nuest[i]][0] > 0 && fas[nuest[i]][2] > 0)
                    {
                        if (i == 0)
                        {
                            tini = fas[nuest[i]][0];
                            tifin = fas[nuest[i]][2];
                        }
                        else
                        {
                            if (tini < fas[nuest[i]][0]) tini = fas[nuest[i]][0];
                            if (tifin > fas[nuest[i]][2]) tifin = fas[nuest[i]][2];
                        }
                    }
                    else
                    {
                        tini = tim[nuest[i]][0];
                        tifin = tim[nuest[i]][lar[nuest[i]] - 1];
                    }
                    // MessageBox.Show(est[nuest[i]].Substring(0,4)+" ini="+tini.ToString()+" fin="+tifin.ToString());
                }
            }
            else
            {
                tini = timl;
                tifin = tfml;
            }
            if (tini == 0 || tifin == 0)
            {
                panelmltra.Visible = false;
                return;
            }

            if (cond == true)
            {
                timlini = tini;
                timlfin = tifin;
                ti1 = tini;
                dd = tifin - tini;
            }
            else
            {
                dd = timlfin - timlini;
                ti1 = timlini;
            }
            xf = panelmltra.Size.Width - 40;
            yf = panelmltra.Size.Height - 25;
            fax = xf / dd;
            fay = yf / 3.0;

            Graphics dc = panelmltra.CreateGraphics();
            if (cond == true) col = Color.Black;
            else col = Color.Orange;
            Pen lapiz = new Pen(col, 1);
            SolidBrush brocha = new SolidBrush(Color.Blue);

            for (i = 0; i < 3; i++)
            {
                ini = (int)((tini - tim[nuest[i]][0]) * ra[nuest[i]]);
                if (cond == true) ini -= 200;
                if (ini < 0) ini = 0;
                fin = (int)((tifin - tim[nuest[i]][0]) * ra[nuest[i]]);
                if (cond == true) fin += 200;
                if (fin > lar[nuest[i]] - 1) fin = lar[nuest[i]] - 1;
                if (cond == true)
                {
                    mlxx[i] = cu[nuest[i]][0];
                    mlmm[i] = mlxx[i];
                    for (j = ini; j < fin; j++)
                    {
                        if (mlxx[i] < cu[nuest[i]][j]) mlxx[i] = cu[nuest[i]][j];
                        else if (mlmm[i] > cu[nuest[i]][j]) mlmm[i] = cu[nuest[i]][j];
                    }
                }
                proo = (int)((mlxx[i] + mlmm[i]) / 2.0F);
                if (mlxx[i] - proo != 0) fy = ((fay / 2) / ((mlxx[i] - proo)));
                else fy = 1.0;
                iniy = 25 + (float)(i * fay + fay / 2);
                if (!char.IsLetterOrDigit(est[nuest[i]][4])) esta = est[nuest[i]].Substring(0, 4);
                else esta = est[nuest[i]];
                dc.DrawString(esta, new Font("Times New Roman", 10, FontStyle.Bold), brocha, 1, iniy);
                kk = 0;
                numu = fin - ini;
                dat = new Point[numu];
                for (k = ini; k < fin; k++)
                {
                    if (kk >= lar[nuest[i]]) break;
                    dif = pro[nuest[i]] - (int)(cu[nuest[i]][k]);
                    diff = dif * fy;
                    y1 = (float)(iniy - amp * diff);
                    x1 = (float)(40.0 + (tim[nuest[i]][k] - ti1) * fax);
                    dat[kk].Y = (int)y1;
                    dat[kk].X = (int)x1;
                    kk += 1;
                }
                dc.DrawLines(lapiz, dat);
            }

            lapiz.Dispose();
            brocha.Dispose();
        }

        private void panelmltra_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.X >= 40) xmli = e.X - 40;
            else xmli = -1;
        }

        private void panelmltra_MouseUp(object sender, MouseEventArgs e)
        {
            int j, xmlf, x1;
            double fax, fay, duu;

            if (e.X >= 40) xmlf = e.X - 40;
            else xmlf = -1;

            if (xmli > -1 && xmlf > -1)
            {
                if (xmli != xmlf)
                {
                    if (xmli > xmlf)
                    {
                        x1 = xmli;
                        xmli = xmlf;
                        xmlf = x1;
                    }
                    fay = (panelmltra.Size.Height - 25) / 3.0;
                    j = (int)((e.Y - 25) / fay);
                    duu = timlfin - timlini;
                    fax = duu / (panelmltra.Size.Width - 40);
                    timl1 = timlini + xmli * fax;
                    timl2 = timlini + xmlf * fax;
                    DibEstMl(nuestml, timlini, timlfin, true);
                    DibEstMl(nuestml, timl1, timl2, false);
                    botraml.Visible = true;
                }
            }

            return;
        }

        private void boml1_Click(object sender, EventArgs e)
        {
            double dif, tii = 0;

            dif = timlfin - timlini;
            tii = timlfin - dif / 3.0;
            if (tii > timlini) timlfin = tii;
            else return;
            DibEstMl(nuestml, timlini, timlfin, true);
        }

        private void boml2_Click(object sender, EventArgs e)
        {
            double dif;

            dif = timlfin - timlini;
            timlfin += dif / 3.0;
            DibEstMl(nuestml, timlini, timlfin, true);
        }

        private void boml3_Click(object sender, EventArgs e)
        {
            double dif;

            dif = timlfin - timlini;
            timlini += dif / 4.0;
            timlfin += dif / 4.0;
            DibEstMl(nuestml, timlini, timlfin, true);
        }

        private void boml4_Click(object sender, EventArgs e)
        {
            double dif;

            dif = timlfin - timlini;
            timlini -= dif / 4.0;
            timlfin -= dif / 4.0;
            DibEstMl(nuestml, timlini, timlfin, true);
        }

        private void bomlsal_Click(object sender, EventArgs e)
        {
            panelmltra.Visible = false;
            return;
        }

        private void botraml_Click(object sender, EventArgs e)
        {
            panelmen.Visible = true;
            util.Mensaje(panelmen, "Calculando ML...", false);
            botraml.BackColor = Color.BlueViolet;
            boml[idml].BackColor = Color.Red;
            Calculo_MLtra(idml);
            ML_total();
            panelmen.Visible = false;
        }

        bool Calculo_MLtra(short id)
        {
            short i, k = 0, m = 0, numm;
            int nmi, nmf, ii, jj, kk = 0, idd;
            double laf, lof, zz, dis, lla, llo;
            bool si;
            string nom = "", li = "";

            ML = -10.0;
            idd = id;
            if (File.Exists(".\\oct\\ini_ml.txt")) File.Delete(".\\oct\\ini_ml.txt");
            if (File.Exists(".\\oct\\maglocal.txt")) File.Delete(".\\oct\\maglocal.txt");
            if (oct == false)
            {
                boctave.Visible = false;
                return (false);
            }
            jj = id * 3;
            si = false;
            numm = -1;
            for (m = 0; m < 3; m++)
            {
                for (k = 0; k < nutra; k++)
                {
                    if (string.Compare(est[k].Substring(0, 4), estml[jj + m].Substring(2, 4)) == 0)
                    {
                        si = true;
                        numm = k;
                        break;
                    }
                    if (si == true) break;
                }
            }// for m...
            if (si == false && id != idml)
            {
                ml[id] = -10.0;
                labml[id].Text = "NO";
                caj[id].Checked = false;
                for (kk = 0; kk <= ordml[numl - 1]; kk++)
                {
                    if (kk != id)
                    {
                        jj = kk * 3;
                        numm = -1;
                        for (m = 0; m < 3; m++)
                        {
                            for (k = 0; k < nutra; k++)
                            {
                                if (string.Compare(est[k].Substring(0, 4), estml[jj + m].Substring(2, 4)) == 0)
                                {
                                    if (fas[k][0] > 0 && fas[k][2] > 0)
                                    {
                                        si = true;
                                        numm = k;
                                        idd = kk;
                                        caj[idd].Checked = true;
                                        //MessageBox.Show("numm=" + numm.ToString());
                                        break;
                                    }
                                }
                            }
                            if (si == true) break;
                        }// for mm...
                    }
                }  //for kk.ToString..
            }
            if (si == false)
            {
                ml[id] = -10.0;
                labml[id].Text = "NO";
                // boml[id].Visible = false;
                ML_total();
                return (si);
            }

            laf = double.Parse(mejpun.Substring(17, 3)) + double.Parse(mejpun.Substring(20, 6)) / 60.0;
            lof = double.Parse(mejpun.Substring(26, 4)) + double.Parse(mejpun.Substring(30, 6)) / 60.0;
            try
            {
                zz = double.Parse(mejpun.Substring(37, 6));
            }
            catch
            {
                zz = 0;
            }
            try
            {
                nmi = (int)((timl1 - tim[numm][0]) * ra[numm]);
                if (nmi < 0) nmi = 0;
                nmf = (int)((timl2 - tim[numm][0]) * ra[numm]);
                if (nmf >= lar[numm]) nmf = lar[numm] - 1;
                for (k = 0; k < nutra; k++)
                {
                    for (i = 0; i < 3; i++)
                    {
                        if (string.Compare(est[k].Substring(0, 4), estml[jj + i].Substring(2, 4)) == 0)
                        {
                            nom = estml[jj + i].Substring(2, 4);
                            StreamWriter wr = File.AppendText(".\\oct\\ini_ml.txt");
                            StreamWriter da = File.CreateText(nom);

                            for (ii = nmi; ii < nmf; ii++)
                            {
                                da.WriteLine(cu[k][ii]);
                            }

                            if (i == 0)
                            {
                                wr.WriteLine("fs=" + ra[k] + ";");
                                lla = Math.Abs(laf - laestml[jj + i]);
                                llo = Math.Abs(lof - loestml[jj + i]);
                                dis = 111.1 * Math.Sqrt(lla * lla + llo * llo);
                                if (checkBoxHypoML.Checked == true) dis = Math.Sqrt(dis*dis + zz*zz);     
                                wr.WriteLine("dist=" + dis.ToString() + ";");
                                if (cBMLZ.Checked == true) wr.WriteLine("siZ=true;");
                                else wr.WriteLine("siZ=false;");
                                if (cBMLN.Checked == true) wr.WriteLine("siN=true;");
                                else wr.WriteLine("siN=false;");
                                if (cBMLE.Checked == true) wr.WriteLine("siE=true;");
                                else wr.WriteLine("siE=false;");
                                if (rdML1.Checked == true) wr.WriteLine("radioboton=1;");
                                else if (rdML2.Checked == true) wr.WriteLine("radioboton=2;");
                                else if (rdML3.Checked == true) wr.WriteLine("radioboton=3;");
                            }
                            wr.WriteLine(compml[jj + i] + "=load " + estml[jj + i].Substring(2, 4) + ";");
                            wr.WriteLine("nom" + compml[jj + i] + "='" + estml[jj + i].Substring(2, 4) + "';");
                            wr.WriteLine(factml[jj + i]);

                            da.Close();
                            wr.Close();
                        }
                    }
                }
            }
            catch
            {
            }

            li = "/C c:\\octave\\bin\\octave.exe < .\\oct\\ML.txt";
            util.Dos(li,true);

            if (File.Exists(".\\oct\\maglocal.txt"))
            {
                li = "";
                StreamReader ar = new StreamReader(".\\oct\\maglocal.txt");
                while (li != null)
                {
                    try
                    {
                        li = ar.ReadLine();
                        if (li == null) break;
                        ml[idd] = double.Parse(li);
                    }
                    catch
                    {
                    }
                }
                ar.Close();
            }
            else
            {
                ml[id] = -10.0;
                boml[id].Visible = false;
            }
            labml[id].Text = string.Format("{0:0.0}", ml[id]);
            ML_total();
            li = nom.Substring(0, 3) + "Z";
            if (File.Exists(li)) File.Delete(li);
            li = nom.Substring(0, 3) + "N";
            if (File.Exists(li)) File.Delete(li);
            li = nom.Substring(0, 3) + "E";
            if (File.Exists(li)) File.Delete(li);
            botraml.BackColor = Color.White;

            return (si);
        }

        bool Calculo_ML()
        {
            short i, j, k = 0, m = 0, numm, id;
            int nmi, nmf, ii;
            double laf, lof, zz, dis, lla, llo;
            bool si;
            string nom = "", li = "";
            string[] nomide = new string[3];
            short[] idecan = new short[3];
            short[] idml = new short[3];

            if (File.Exists(".\\oct\\ini_ml.txt")) File.Delete(".\\oct\\ini_ml.txt");
            if (File.Exists(".\\oct\\maglocal.txt")) File.Delete(".\\oct\\maglocal.txt");
            if (oct == false)
            {
                boctave.Visible = false;
                return (false);
            }

            si = false;
            numm = -1;
            id = -1;           
            for (m = 0; m < numl; m++)
            {
                if (fr1.sismo[9] != 'X' && estml[m][0] != fr1.sismo[9]) continue;
                for (k = 0; k < nutra; k++)
                {
                    if (string.Compare(est[k].Substring(0,4), estml[m].Substring(2,4)) == 0)
                    {                        
                        if (iniml[m] > 0)
                        {
                            if (fr1.añoML < iniml[m]) continue;
                            else if (finml[m] > 0 && fr1.añoML > finml[m]) continue;
                        }
                        if (fas[k][0] > 0 && fas[k][2] > 0)
                        {
                            si = true;
                            numm = k;
                            id = ordml[m];
                            break;
                        }
                    }
                    if (si == true) break;
                }
            }// for m...           
            if (id == -1 || numm == -1) return (false);

            j = 0;
            for (i = 0; i < numl; i++)
            {
                if (ordml[i] == id)
                {
                    nomide[j] = estml[i].Substring(2, 4);
                    idml[j++] = i;
                    //MessageBox.Show("nom=" + nomide[j - 1] + " idml=" + idml[j - 1].ToString());
                }
            }
            if (j != 3) return (false);
            j = 0;
            for (i = 0; i < nutra; i++)
            {
                for (k = 0; k < 3; k++)
                {
                    if (est[i].Substring(0, 4) == nomide[k].Substring(0, 4))
                    {
                        idecan[j++] = i;
                    }
                }
            }
            if (j != 3) return (false);

            laf = double.Parse(mejpun.Substring(17, 3)) + double.Parse(mejpun.Substring(20, 6)) / 60.0;
            lof = double.Parse(mejpun.Substring(26, 4)) + double.Parse(mejpun.Substring(30, 6)) / 60.0;
            try
            {
                zz = double.Parse(mejpun.Substring(37, 6));
            }
            catch
            {
                zz = 0;
            }
            nmi = (int)((fas[numm][0] - tim[numm][0]) * ra[numm]) - 200;
            if (nmi < 0) nmi = 0;
            nmf = (int)((fas[numm][2] - tim[numm][0]) * ra[numm]);
            if (nmf >= lar[numm]) nmf = lar[numm] - 1;

            StreamWriter wr = File.AppendText(".\\oct\\ini_ml.txt");
            wr.WriteLine("fs=" + ra[numm] + ";");
            lla = Math.Abs(laf - laestml[idml[0]]);
            llo = Math.Abs(lof - loestml[idml[0]]);
            dis = 111.1 * Math.Sqrt(lla * lla + llo * llo);
            if (checkBoxHypoML.Checked == true) dis = Math.Sqrt(dis*dis + zz*zz);     
            wr.WriteLine("dist=" + dis.ToString() + ";");
            if (cBMLZ.Checked == true) wr.WriteLine("siZ=true;");
            else wr.WriteLine("siZ=false;");
            if (cBMLN.Checked == true) wr.WriteLine("siN=true;");
            else wr.WriteLine("siN=false;");
            if (cBMLE.Checked == true) wr.WriteLine("siE=true;");
            else wr.WriteLine("siE=false;");
            if (rdML1.Checked == true) wr.WriteLine("radioboton=1;");
            else if (rdML2.Checked == true) wr.WriteLine("radioboton=2;");
            else if (rdML3.Checked == true) wr.WriteLine("radioboton=3;");
            for (i = 0; i < 3; i++)
            {
                nom = est[idecan[i]].Substring(0, 4);
                if (File.Exists(nom)) File.Delete(nom);
                StreamWriter da = File.CreateText(nom);
                for (ii = nmi; ii < nmf; ii++)
                {
                    da.WriteLine(cu[idecan[i]][ii]);
                }
                da.Close();
                wr.WriteLine(compml[idml[i]] + "=load('" + estml[idml[i]].Substring(2, 4) + "');");
                wr.WriteLine("nom" + compml[idml[i]] + "='" + estml[idml[i]].Substring(2, 4) + "';");
                wr.WriteLine(factml[idml[i]]);
            }
            wr.Close();

            li = "/C c:\\octave\\bin\\octave.exe < .\\oct\\ML.txt";
            util.Dos(li, true);

            if (File.Exists(".\\oct\\maglocal.txt"))
            {
                li = "";
                StreamReader ar = new StreamReader(".\\oct\\maglocal.txt");
                while (li != null)
                {
                    try
                    {
                        li = ar.ReadLine();
                        if (li == null) break;
                        ml[id] = double.Parse(li);
                    }
                    catch
                    {
                    }
                }
                ar.Close();
            }
            else
            {
                ml[id] = -10.0;
                //boml[id].Visible = false;
            }
            labml[id].Text = string.Format("{0:0.0}", ml[id]);
            try
            {
                li = nom.Substring(0, 3) + "Z";
                if (File.Exists(li)) File.Delete(li);
                li = nom.Substring(0, 3) + "N";
                if (File.Exists(li)) File.Delete(li);
                li = nom.Substring(0, 3) + "E";
                if (File.Exists(li)) File.Delete(li);
            }
            catch
            {
            }

            return (si);
        }
          
        void ML_total()
        {
            int i, j, an, me, di, ho, mi, se;
            double mm;
            string ca = "", let = "";

            nomML = "";
            mm = 0;
            j = 0;
            if (radbupro.Checked == true)
            {
                for (i = 0; i <= ordml[numl - 1]; i++)
                {
                    if (ml[i] > -10.0)
                    {
                        mm += ml[i];
                        //MessageBox.Show("mm="+mm.ToString()+" ml="+ml[i].ToString()+" i="+i.ToString());
                        j += 1;
                    }
                }
                if (j == 0) ML = -10.0;
                else ML = mm / j;
            }
            else if (radbumax.Checked == true)
            {
                mm = -10.0;
                for (i = 0; i <= ordml[numl - 1]; i++)
                {
                    if (mm < ml[i]) mm = ml[i];
                }
                ML = mm;
            }
            else
            {
                mm = 100.0;
                for (i = 0; i <= ordml[numl - 1]; i++)
                {
                    if (mm > ml[i] && ml[i] > -10.0) mm = ml[i];
                }
                if (mm == 100.0) mm = -10.0;
                ML = mm;
            }

            labelMltot.Text = string.Format("ML={0:0.0}", ML);
            boctave.Text = "ML="+string.Format("{0:0.0}", ML);

            if (fr1.MagnitudLocal == false || grabas == false || ML <= -10.0) return;

            try
            {
                an = int.Parse(mejpun.Substring(0, 2));
                if (an < 85) an += 2000;
                else an += 1900;
                me = int.Parse(mejpun.Substring(2, 2));
                di = int.Parse(mejpun.Substring(4, 2));
                ho = int.Parse(mejpun.Substring(7, 2));
                mi = int.Parse(mejpun.Substring(9, 2));
                se = int.Parse(mejpun.Substring(12, 2));
                nomML = fr1.rutbas + string.Format("\\ML\\{0:0000}\\{1:00}", an, me);
                if (!Directory.Exists(nomML)) Directory.CreateDirectory(nomML);
                nomML += "\\" + string.Format("{0:000}{1:00}_{2:00}{3:00}{4:00}{5:00}_", an, me, di, ho, mi, se);
                nomML += fr1.sismo.Substring(0, 8) + fr1.sismo.Substring(9, 3) + ".txt";
                StreamWriter wr0 = File.CreateText(nomML);
                wr0.WriteLine(mejpun + " " + fr1.sismo);
                wr0.Close();
                if (File.Exists(".\\oct\\MLtodas.txt"))
                {
                    ca = "/C type  .\\oct\\MLtodas.txt >> " + nomML;
                    util.Dos(ca, true);
                    //File.Copy(".\\oct\\MLtodas.txt", "MLdetalle.txt", true);
                }
                StreamWriter wr = File.AppendText(nomML);
                wr.Write("ML=" + string.Format("{0:0.0}", ML) + " ");
                for (i = 0; i <= ordml[numl - 1]; i++)
                {
                    if (ml[i] > -10.0)
                    {
                        ca = estml[i * 3].Substring(2, 3) + " " + ml[i] + " ";
                        wr.Write(ca);
                    }
                }
                wr.WriteLine(let);
                wr.Close();
                //boArML.Visible = true;
            }
            catch
            {
            }

            return;
        }

        private void boctave_MouseDown(object sender, MouseEventArgs e)
        {

            if (e.Button == MouseButtons.Left)
            {
                if (sioct == true)
                {
                    sioct = false;
                    boctave.BackColor = Color.White;
                }
                else
                {
                    sioct = true;
                    boctave.BackColor = Color.SteelBlue;
                }
            }
            else
            {
                if (panelML.Visible == false) panelML.Visible = true;
                else panelML.Visible = false;
            }
        }

        bool RevitraML()
        {
            int i,k,ii,j,jj,kk,m;
            bool si = false;
            bool[] sii = new bool[3];

          
            k = (int)(numl / 3.0);
            //MessageBox.Show("numl=" + numl.ToString()+"k="+k.ToString());
            for (i = 0; i < k; i++) siboml[i] = false;

            for (ii = 0; ii < k; ii++)
            {
                for (i = 0; i < 3; i++) sii[i] = false;
                kk = ii * 3;
                for (jj = kk; jj < kk + 3; jj++)
                {
                    for (j = 0; j < nutra; j++)
                    {
                        //MessageBox.Show(est[j].Substring(0,4)+" "+estml[jj].Substring(2,4));
                        if (string.Compare(est[j].Substring(0,4),estml[jj].Substring(2,4)) == 0)
                        {
                            m = jj - kk;
                            sii[m] = true;
                            break;
                        }
                    }
                }
                if (sii[0] == true && sii[1] == true && sii[2] == true)
                {
                    si = true;
                    siboml[ii] = true;
                }
            }

            return(si);
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

        private void boCerrarMapa_Click(object sender, EventArgs e)
        {
            panelmapa.Visible = false;
            bomapa.BackColor = Color.White;
        }

        private void bomapa_Click(object sender, EventArgs e)
        {
            double facmla, facmlo;

            if (panelmapa.Visible == true)
            {
                panelmapa.Visible = false;
                bomapa.BackColor = Color.White;
            }
            else
            {
                panelmapa.Visible = true;
                bomapa.BackColor = Color.Gold;
                facmapa = 1100.0 / 0.3;
                util.borra(panelmapa, Color.LavenderBlush);
                //lasi = double.Parse(mejpun.Substring(18, 2)) + double.Parse(mejpun.Substring(21, 5)) / 60;
                //losi = double.Parse(mejpun.Substring(28, 2)) + double.Parse(mejpun.Substring(31, 5)) / 60;
                lasi = double.Parse(mejpun.Substring(17, 3)) + double.Parse(mejpun.Substring(20, 6)) / 60;
                losi = double.Parse(mejpun.Substring(26, 4)) + double.Parse(mejpun.Substring(30, 6)) / 60;
                facmla = 0.7 * ((0.5 * panelmapa.Size.Height) / Math.Abs(lasi - fr1.latvol[vol]));
                facmlo = 0.7 * ((0.5 * panelmapa.Size.Height) / Math.Abs(losi - fr1.lonvol[vol]));
                facm = facmla;
                if (facm > facmlo) facm = facmlo;
                if (facm > facmapa) facm = facmapa;
                util.Topo(panelmapa, fr1.volcan[vol], facm, fr1.latvol[vol], fr1.lonvol[vol],
                     lasi, losi, Color.Gray,false,0,0,0,0);
            }
        }

        private void boPun_Click(object sender, EventArgs e)
        {

            if (paneldatopun.Visible == false)
            {
                paneldatopun.Visible = true;
                Graphics dc = paneldatopun.CreateGraphics();
                SolidBrush brocha = new SolidBrush(Color.Black);
                dc.DrawString(mejpun, new Font("Times New Roman", 12, FontStyle.Bold), brocha, 10, 10);
                brocha.Dispose();
                boPun.BackColor = Color.DarkOrange;
            }
            else
            {
                paneldatopun.Visible = false;
                boPun.BackColor = Color.White;
                panel1.Invalidate();
            }
        }

        private void boAutoPic_MouseDown(object sender, MouseEventArgs e)
        {

            int i, k, an, me, di, ho, mi, se, ms, ini, nn;
            long ll;
            double seg, ti1;
            string ca = "", li = "";

            //MessageBox.Show(fr1.sis);
            if (!File.Exists(fr1.nomsud)) return;
            File.Copy(fr1.nomsud, fr1.sismo, true);
            if (File.Exists("automat.txt")) File.Delete("automat.txt");
            ca = "/C .\\h\\autopick /A " + fr1.sismo;
            util.Dos(ca,true);
            ca = "/C .\\h\\xtrhy71 " + fr1.sismo + " automat.txt";
            util.Dos(ca,true);

            if (!File.Exists("automat.txt")) return;
            panelmensa.Visible = true;
            util.Mensaje(panelmensa, "Arribos P Aoumaticos...", true);

            for (i = 0; i < nutra; i++)
            {
                fas[i][0] = 0;
                fas[i][1] = 0;
                fas[i][2] = 0;
                teo[i][0] = 0;
                teo[i][1] = 0;
                pes[i][0] = 4;
                pes[i][1] = 4;
                pol[i] = ' ';
                impu[i] = 'I';
            }
            nn = 0;
            StreamReader ar = new StreamReader("automat.txt");
            while (li != null)
            {
                try
                {
                    li = ar.ReadLine();
                    if (li == null) break;
                    if (li[7] != '4')
                    {
                        for (k = 0; k < nutra; k++)
                        {
                            if (char.IsLetterOrDigit(fr1.est[k][0])) ini = 0;
                            else ini = 1;
                            if (string.Compare(li.Substring(0, 4), fr1.est[k].Substring(ini, 4)) == 0 && li[7] != '4')
                            {
                                an = int.Parse(li.Substring(9, 2));
                                if (an > 85) an += 1900;
                                else an += 2000;
                                me = int.Parse(li.Substring(11, 2));
                                di = int.Parse(li.Substring(13, 2));
                                ho = int.Parse(li.Substring(15, 2));
                                mi = int.Parse(li.Substring(17, 2));
                                se = int.Parse(li.Substring(19, 2));
                                seg = double.Parse(li.Substring(21, 3));
                                ms = (int)(seg * 1000.0);
                                DateTime fe1 = new DateTime(an, me, di, ho, mi, se, ms);
                                ll = fe1.ToBinary();
                                fas[k][0] = ((double)(ll) - Feisuds) / 10000000.0;
                                pes[k][0] = short.Parse(li.Substring(7, 1));
                                pol[k] = li[6];
                                impu[k] = li[4];
                                nn += 1;
                                if (char.IsDigit(li[74]))
                                {
                                    ti1 = double.Parse(li.Substring(70, 5));
                                    if (ti1 > 0.5) fas[k][2] = fas[k][0] + ti1;
                                    //MessageBox.Show("CODA=" + fas[k][2].ToString());
                                }
                                break;
                            } // if compare....
                        }// for k....
                    }
                }
                catch
                {
                }
            }
            ar.Close();

            if (nn >= 3)
            {
                boHp71.Visible = true;
                boOrd.Visible = true;

                if (e.Button == MouseButtons.Right)
                {
                    panelpun.BringToFront();
                    boTeo.Visible = false;
                    bomapa.Visible = false;
                    boPun.Visible = false;
                    CorreHypo();
                }
            }
            else
            {
                boHp71.Visible = false;
                boOrd.Visible = false;
            }
            panelmensa.Visible = false;
            boAutoPic.BackColor = Color.OrangeRed;
            panel1.Invalidate();

            return;
        }

        private void cBMLZ_CheckedChanged(object sender, EventArgs e)
        {
            if (cBMLZ.Checked == false && cBMLN.Checked == false && cBMLE.Checked == false)
            {
                cBMLZ.Checked = true;
                return;
            }
        }

        private void cBMLN_CheckedChanged(object sender, EventArgs e)
        {
            if (cBMLZ.Checked == false && cBMLN.Checked == false && cBMLE.Checked == false)
            {
                cBMLN.Checked = true;
                return;
            }
        }

        private void cBMLE_CheckedChanged(object sender, EventArgs e)
        {
            if (cBMLZ.Checked == false && cBMLN.Checked == false && cBMLE.Checked == false)
            {
                cBMLE.Checked = true;
                //MessageBox.Show("hola");
                return;
            }
        }

        private void rdML1_CheckedChanged(object sender, EventArgs e)
        {// promedio amplitudes ML

        }

        private void rdML2_CheckedChanged(object sender, EventArgs e)
        {// Maxima amplitud ML

        }

        private void rdML3_CheckedChanged(object sender, EventArgs e)
        {// Amplitud Resultante ML

        }

        private void checkBoxHypoML_CheckedChanged(object sender, EventArgs e)
        {// Escoge entre distancia epicentral e hipocentral

        }

        private void boBUZ_MouseDown(object sender, MouseEventArgs e)
        {
            string linea = "";

            if (!File.Exists("EGAVGA.BGI"))
            {
                if (File.Exists(".\\h\\EGAVGA.BGI"))
                {
                    File.Copy(".\\h\\EGAVGA.BGI", ".\\EGAVGA.BGI");
                }
                else
                {
                    MessageBox.Show("Falta el archivo EGAVGA.BGI en el DIRECTORIO de TRABAJO");
                    return;
                }
            }
            if (File.Exists(".\\h\\mec.txt")) File.Delete(".\\h\\mec.txt");
            GrabaBuz();
            linea = "/C .\\h\\buz .\\h\\mec.txt";
            util.Dos(linea,false);
        }

        private void boMasVol_Click(object sender, EventArgs e)
        {
            if (panel3.Visible == false) panel3.Visible = true;
            else panel3.Visible = false;
        }

        private void boParamPun_Click(object sender, EventArgs e)
        {
            diag1frm2 di = new diag1frm2();

            di.Inite = inite.ToString();
            di.Interite = ite.ToString();
            if (di.ShowDialog() == DialogResult.OK)
            {
                inite = float.Parse(di.Inite);
                ite = float.Parse(di.Interite);
                panelpun.BringToFront();
                bomapa.Visible = false;
                boPun.Visible = false;
                boTeo.Visible = false;
                CorreHypo();
            }
        }

        bool GrabaFocMec(int numPerr)
        {
            int i, j, cont;
            string li = "", ca = "";
            string[] lin = new string[Ma];

            if (File.Exists(".\\h\\focmec.inp")) File.Delete(".\\h\\focmec.inp");
            if (File.Exists(".\\h\\focmec.out")) File.Delete(".\\h\\focmec.out");

            StreamWriter en = File.CreateText(".\\h\\entrada.txt");
            en.WriteLine(".\\h\\focmec.out");
            en.WriteLine(fr1.sismo + " Focmec");
            en.WriteLine(".\\h\\focmec.inp");
            en.WriteLine("Y");
            en.WriteLine("N");
            en.WriteLine(numPerr); // errores de P
            en.WriteLine("100");
            en.WriteLine("0");
            en.WriteLine("5");
            en.WriteLine("355");
            en.WriteLine("0");
            en.WriteLine("5");
            en.WriteLine("90");
            en.WriteLine("0");
            en.WriteLine("5");
            en.WriteLine("85");
            en.Close();

            StreamReader ar = new StreamReader(".\\h\\r.prt");

            cont = 0;
            j = 0;
            while (li != null)
            {
                try
                {
                    li = "";
                    li = ar.ReadLine();
                    if (li == null) break;
                    if (string.Compare(li.Substring(114, 4), "SDXM") == 0)
                    {
                        if (cont == mejite)
                        {
                            for (i = 0; i < 3; i++) lin[j++] = ar.ReadLine();
                            do
                            {
                                li = ar.ReadLine();
                                if (li.Length < 60 || li[0] != ' ' || !char.IsLetter(li[1])) break;
                                lin[j++] = li;
                            } while (li.Length > 60 || li[0] == ' ' && char.IsLetter(li[1]));
                            break;
                        }
                        cont += 1;
                    }
                    else if (string.Compare(li.Substring(5, 3), "* I") == 0)
                    {
                        cont += 1;
                    }
                }
                catch
                {
                }
            }

            ar.Close();

            nuestmeca = j - 3;
            if (nuestmeca < 5)
            {
                boFocmec.BackColor = Color.Red;
                if (panelFocmec.Visible == true) panelFocmec.Visible = false;
                return (false);
            }
            else if (nuestmeca == 5) boFocmec.BackColor = Color.Orange;

            try
            {
                StreamWriter wr = File.CreateText(".\\h\\focmec.inp");
                wr.WriteLine(fr1.sismo + " Hypo71.prt");
                for (i = 3; i < j; i++)
                {
                    ca = lin[i].Substring(1, 5) + " " + lin[i].Substring(12, 3) + ".00  " + lin[i].Substring(16, 3) + ".00" + lin[i].Substring(22, 1);
                    wr.WriteLine(ca);
                }

                wr.Close();
            }
            catch
            {
                panelmen.Visible = false;
                return (false);
            }

            return (true);
        }

        private void panelFocmec_Paint(object sender, PaintEventArgs e)
        {
            //DibFocmec();
            MecanismoFocMec();
        }

        void PloteoEstacionesFocMec()
        {
            int i, j, num, xf, yf, xc, yc, x, y;
            double fcP, az, ai, ra2, d1, d2, dis, rad;
            double[] azm, ain;
            char[] pol;
            string li = "";

            if (!File.Exists(".\\h\\focmec.inp")) return;

            StreamReader ar2 = new StreamReader(".\\h\\focmec.inp");
            li = ar2.ReadLine();
            num = 0;
            while (li != null)
            {
                try
                {
                    li = ar2.ReadLine();
                    if (li == null) break;
                    if (li[9] == '.') num += 1;
                }
                catch
                {
                }
            }
            ar2.Close();

            if (num == 0)
            {
                return; /// aqui controlar el boton ....
            }
            li = "";

            azm = new double[num];
            ain = new double[num];
            pol = new char[num];

            j = 0;
            StreamReader ar = new StreamReader(".\\h\\focmec.inp");
            li = ar.ReadLine();
            while (li != null)
            {
                try
                {
                    li = ar.ReadLine();
                    if (li == null) break;
                    if (li[9] == '.')
                    {
                        azm[j] = double.Parse(li.Substring(6, 6));
                        ain[j] = double.Parse(li.Substring(14, 6));
                        pol[j++] = li[20];
                    }
                }
                catch
                {
                }
            }
            ar.Close();

            fcP = Math.PI / 180.0;
            ra2 = Math.Sqrt(2.0);
            xf = panelFocmec.Size.Width - 40;
            yf = panelFocmec.Height - 20;
            //xc = (int)(xf / 3.0);
            xc = (int)(yf / 2.0);
            yc = (int)(yf / 2.0);

            j = (int)((yf / 1.3) / 2.0);
            rad = j * ra2;

            Graphics dc = panelFocmec.CreateGraphics();
            Pen lap = new Pen(Color.Black, 1);
            SolidBrush bro = new SolidBrush(Color.Blue);

            for (i = 0; i < num; i++)
            {
                az = azm[i];
                ai = 90.0 - ain[i];
                if (ai < 0)
                {
                    ai = Math.Abs(ai);
                    az += 180.0;
                    if (az > 360.0) az -= 360.0;
                }
                d1 = az * fcP;
                d2 = ai * fcP;
                dis = rad * Math.Sin((Math.PI / 4.0) - (d2 / 2.0));
                x = xc + (int)(dis * Math.Sin(d1));
                y = yc - (int)(dis * Math.Cos(d1));
                if (pol[i] == 'C' || pol[i] == 'U' || pol[i] == '+') dc.FillRectangle(bro, x - 4, y - 4, 4, 4);
                dc.DrawRectangle(lap, x - 4, y - 4, 4, 4);
            }

            lap.Dispose();
            bro.Dispose();

            return;
        }
        void DibFocmec(double strike, double dip, Color col)
        {
            int    j, k, xc, yc, xf, yf, tam, x, y;
            double r, az, delta, fcP, ra2;
            double dd, rad, dis, phi;
            Point[] dat;

            if (!File.Exists(".\\h\\focmec.out")) return;
            fcP = Math.PI / 180.0;
            ra2 = Math.Sqrt(2.0);
            xf = panelFocmec.Size.Width - 40;
            yf = panelFocmec.Height - 20;
            //xc = (int)(xf / 3.0);
            xc = (int)(yf / 2.0);
            yc = (int)(yf / 2.0);

            Graphics dc = panelFocmec.CreateGraphics();
            Pen lapiz = new Pen(Color.Black, 1);
            Pen lap = new Pen(col, 1);
            tam = (int)(yf / 1.3); // provi
            j = (int)(tam / 2.0);
            dc.DrawEllipse(lapiz, xc - j, yc - j, tam, tam);

            phi = dip * fcP;
            r = j;
            rad = r * ra2;
            az = strike * fcP;
            dat = new Point[181];
            for (k = 0; k <= 180; k++)
            {
                if (dip != 90.0)
                {
                    dd = k * fcP;
                    delta = Math.Atan(Math.Tan(phi) * Math.Cos((Math.PI / 2.0) - dd));//apparent dip
                    dis = rad * Math.Sin((Math.PI / 4.0) - (delta / 2.0));
                }
                else
                {
                    dd = 0;
                    dis = j * Math.Cos(k * fcP);
                }
                x = xc + (int)(dis * Math.Sin(az + dd));
                y = yc - (int)(dis * Math.Cos(az + dd));
                dat[k].Y = y;
                dat[k].X = x;
            }
            dc.DrawLines(lap, dat);

            lapiz.Dispose();
            lap.Dispose();

            return;
        }
        void LetreroFocMec(double[] strike, double[] dip, double[] rake, int num, int id)
        {
            int i, xf, yf, ini, x, y, cant;
            string ca = "";
            SolidBrush bb;

            xf = panelFocmec.Size.Width - 40;
            yf = panelFocmec.Height - 20;
            //ini = (int)(xf / 1.5);
            ini = yf + 50;
            cant = (int)((yf - 30) / 15.0);

            Graphics dc = panelFocmec.CreateGraphics();
            SolidBrush broid = new SolidBrush(Color.BlueViolet);
            //SolidBrush broid = new SolidBrush(Color.Green);
            SolidBrush bro = new SolidBrush(Color.LightGray);
            SolidBrush brr = new SolidBrush(Color.DarkSlateBlue);
            ca = string.Format("# Error P: {0:0}", numerrP);
            dc.DrawString(ca, new Font("Times New Roman", 10, FontStyle.Bold), brr, 350, 10);
            brr.Dispose();
            bb = bro;

            y = 25;
            x = ini;
            for (i = 0; i < num; i++)
            {
                if (i > 0 && Math.IEEERemainder(i, cant) == 0)
                {
                    y = 25;
                    x += 110;
                }
                ca = string.Format("{0:000.0} {1:00.0} {2:000.0}", strike[i], dip[i], rake[i]);
                if (i != id) bb = bro;
                else bb = broid;
                dc.DrawString(ca, new Font("Times New Roman", 9), bb, x, y);
                y += 15;

            }
            SolidBrush bro2 = new SolidBrush(Color.Green);
            ca = string.Format("AZM: {0:0.0}º", strike[id]);
            dc.DrawString(ca, new Font("Times New Roman", 12, FontStyle.Bold), bro2, 20, yf - 20);
            bro2.Dispose();
            SolidBrush bro3 = new SolidBrush(Color.Blue);
            ca = string.Format("DIP: {0:0.0}º", dip[id]);
            dc.DrawString(ca, new Font("Times New Roman", 12, FontStyle.Bold), bro3, 130, yf - 20);
            bro3.Dispose();
            SolidBrush bro4 = new SolidBrush(Color.DarkOrange);
            ca = string.Format("RAKE: {0:0.0}º", rake[id]);
            dc.DrawString(ca, new Font("Times New Roman", 12, FontStyle.Bold), bro4, 240, yf - 20);
            bro4.Dispose();

            bro.Dispose();
            broid.Dispose();

        }
        void MecanismoFocMec()  //stein
        {
            int i, j, k, num, cant, yf;
            double strikex, dipx, rakex, rak;
            double dd, dd1, dd2, fcP;
            double[] strike, dip, rake;
            string li = "";
            Color col;

            rumbo = -1.0;
            buza = -1.0;
            pitch = -1.0;
            if (!File.Exists(".\\h\\focmec.out")) return;
            FileInfo rr = new FileInfo(".\\h\\focmec.out");
            if (rr.Length <= 515) return;

            StreamReader ar2 = new StreamReader(".\\h\\focmec.out");
            for (i = 0; i < 10; i++)
            {
                li = ar2.ReadLine();
                if (li == null) break;
            }
            num = 0;
            while (li != null)
            {
                try
                {
                    li = ar2.ReadLine();
                    if (li == null) break;
                    if (li[5] == '.') num += 1;
                }
                catch
                {
                }
            }
            ar2.Close();

            if (num == 0)
            {
                totfocmec = 0;
                idfocmec = -1;
                return; /// aqui controlar el boton ....
            }
            totfocmec = (short)(num);
            yf = panelFocmec.Height - 20;
            cant = (int)((yf - 30) / 15.0);
            k = 640 + (int)((num / cant) * 120);
            panelFocmec.Size = new Size(k, 480);

            li = "";

            strike = new double[num];
            dip = new double[num];
            rake = new double[num];

            j = 0;
            StreamReader ar = new StreamReader(".\\h\\focmec.out");
            for (i = 0; i < 10; i++)
            {
                li = ar.ReadLine();
                if (li == null) break;
            }
            while (li != null)
            {
                try
                {
                    li = ar.ReadLine();
                    if (li == null) break;
                    if (li[5] == '.')
                    {
                        dip[j] = double.Parse(li.Substring(0, 8));
                        strike[j] = double.Parse(li.Substring(9, 7));
                        rake[j++] = double.Parse(li.Substring(17, 7));
                    }
                }
                catch
                {
                }
            }
            ar.Close();

            if (idfocmec == -1) idfocmec = (short)(num / 2.0);
            j = idfocmec;
            rumbo = strike[j];
            buza = dip[j];
            pitch = rake[j];
            for (i = 0; i < num; i++)
            {
                if (i != j) col = Color.Gainsboro;
                else continue;
                //else col = Color.BlueViolet;
                if (rake[i] > 0) rak = 180.0 - rake[i];
                else rak = Math.Abs(rake[i]);
                fcP = Math.PI / 180.0;
                dd1 = dip[i] * fcP;
                dd2 = rak * fcP;
                dipx = Math.Acos(Math.Sin(dd1) * Math.Sin(dd2));
                rakex = Math.Asin(Math.Cos(dd1) / Math.Sin(dipx));

                strikex = (strike[i] * fcP) - (Math.Asin(Math.Cos(dd2) / Math.Sin(dipx)));
                strikex += Math.PI;
                dd = rake[i] - (rakex / fcP);
                dd1 = strikex / fcP;
                dd2 = dipx / fcP;

                DibFocmec(strike[i], dip[i], col);
                DibFocmec(dd1, dd2, col);
            }
            col = Color.BlueViolet;
            if (rake[j] > 0) rak = 180.0 - rake[j];
            else rak = Math.Abs(rake[j]);
            fcP = Math.PI / 180.0;
            dd1 = dip[j] * fcP;
            dd2 = rak * fcP;
            dipx = Math.Acos(Math.Sin(dd1) * Math.Sin(dd2));
            rakex = Math.Asin(Math.Cos(dd1) / Math.Sin(dipx));

            strikex = (strike[j] * fcP) - (Math.Asin(Math.Cos(dd2) / Math.Sin(dipx)));
            strikex += Math.PI;
            dd = rake[j] - (rakex / fcP);
            dd1 = strikex / fcP;
            dd2 = dipx / fcP;

            DibFocmec(strike[j], dip[j], col);
            DibFocmec(dd1, dd2, col);

            PloteoEstacionesFocMec();
            LetreroFocMec(strike, dip, rake, num, j);

            return;
        }

        private void panelFocmec_MouseDown(object sender, MouseEventArgs e)
        {
            int i, j, nu, xf, yf, ini, cant;

            xf = panelFocmec.Size.Width - 40;
            yf = panelFocmec.Height - 20;

            ini = yf + 50;
            cant = (int)((yf - 30) / 15.0);

            if (e.X > ini)
            {
                i = (int)((e.Y - 25) / 15.0);
                if (i > cant) i = cant;
                j = (int)((e.X - ini) / 110.0);
                nu = i + j * cant;
                idfocmec = (short)(nu);
                if (idfocmec >= totfocmec) idfocmec = (short)(totfocmec - 1);
                panelFocmec.Invalidate();
                //MessageBox.Show("nu="+nu.ToString());
            }

        }

        private void boFocmec_MouseDown(object sender, MouseEventArgs e)
        {
            int k, maxPerr;
            string linea = "";
            bool res, si = false;

            panelFocmec.Visible = true;
            util.Mensaje(panelFocmec, "Calculando....", true);

            numerrP = -1;
            res = GrabaFocMec(0);
            if (res == false)
            {
                panelFocmec.Visible = false;
                return;
            }
            else
            {
                k = 0;
                //maxPerr = (int)(nuestmeca / 5.0);
                maxPerr = (int)(nuestmeca / 3.0);
                do
                {
                    linea = "/C .\\h\\focmec .\\h\\focmec.inp < .\\h\\entrada.txt > resFCM.txt";
                    util.Dos(linea, false);
                    FileInfo ar = new FileInfo(".\\h\\focmec.out");
                    if (ar.Length > 515)
                    {
                        si = true;
                        numerrP = (short)(k);
                        break;
                    }
                    k += 1;
                    if (k >= maxPerr) break;
                    GrabaFocMec(k);
                } while (k < maxPerr);
            }
            if (si == false)
            {
                boFocmec.BackColor = Color.Red;
                if (panelFocmec.Visible == true) panelFocmec.Visible = false;
                StreamWriter wr = File.AppendText("focmec.fff");
                wr.WriteLine(fr1.sismo + " num=" + numerrP.ToString() + " nuestmeca=" + nuestmeca.ToString());
                wr.Close();
                return;
            }
            util.borra(panelFocmec, Color.FloralWhite);
            // DibFocmec();
            panelFocmec.Invalidate();

            return;
        }

        private void boXFocmec_Click(object sender, EventArgs e)
        {
            panelFocmec.Visible = false;
        }

        private void boGraFocmec_Click(object sender, EventArgs e)
        {
            /*int an, me, di, ho, mi, se, ms;
            string ca = "", nom = "", carpe = "";

            if (grabas == false || mejite < 0) return;
            an = int.Parse(mejpun.Substring(0, 2));
            me = int.Parse(mejpun.Substring(2, 2));
            di = int.Parse(mejpun.Substring(4, 2));
            ho = int.Parse(mejpun.Substring(7, 2));
            mi = int.Parse(mejpun.Substring(9, 2));
            se = int.Parse(mejpun.Substring(12, 2));
            ms = int.Parse(mejpun.Substring(15, 2));

            carpe = fr1.rutbas + "\\mec\\";
            carpe += string.Format("{0:00}\\{1:00}\\{2:00}", an, me, di);
            if (!Directory.Exists(carpe)) Directory.CreateDirectory(carpe);
            nom = carpe + string.Format("\\{0:00}{1:00}_{2:00}", ho, mi, se);
            nom += "_" + fr1.sismo.Substring(0, 8) + fr1.sismo.Substring(9);
            ca = string.Format("_{0:000}_{1:00}_", rumbo, buza);
            if (buza < 0) ca += "N"; else ca += "P";
            ca += string.Format("{0:000}", Math.Abs(pitch));
            nom += ca + ".fcm";
            //if (File.Exists(nom)) MessageBox.Show("nom=" + nom);
            StreamWriter wr = File.CreateText(nom);
            ca = string.Format("{0:0.0} {1:0.0} {2:0.0}   {3:00} {4:000}   Strike Dip Rake   P_Errores_Aceptados  Total soluciones", rumbo, buza, pitch, numerrP, totfocmec);
            wr.WriteLine(ca);
            wr.WriteLine(mejpun);
            wr.Close();

            panelFocmec.Visible = false;

            return;*/
            GrabaMecanismo(true);
        }

        void GrabaMecanismo(bool cond)
        {
            int an, me, di, ho, mi, se, ms;
            string ca = "", nom = "", carpe = "", ca2 = "", pa = "";

            if (grabas == false || mejite < 0) return;
            an = int.Parse(mejpun.Substring(0, 2));
            me = int.Parse(mejpun.Substring(2, 2));
            di = int.Parse(mejpun.Substring(4, 2));
            ho = int.Parse(mejpun.Substring(7, 2));
            mi = int.Parse(mejpun.Substring(9, 2));
            se = int.Parse(mejpun.Substring(12, 2));
            ms = int.Parse(mejpun.Substring(15, 2));

            carpe = fr1.rutbas + "\\mec\\";
            carpe += string.Format("{0:00}\\{1:00}\\{2:00}", an, me, di);
            if (!Directory.Exists(carpe)) Directory.CreateDirectory(carpe);
            ca2 = string.Format("{0:00}{1:00}_{2:00}", ho, mi, se) + "_" + fr1.sismo.Substring(0, 8) + fr1.sismo.Substring(9);

            if (cond == false)
            {
                DialogResult result = MessageBox.Show("Esta opcion permite grabar VARIOS mecanismos\npara UN solo sismo Quiere continuar ??",
                                                "CONTINUAR ???", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.No) return;
            }

            if (cond == true)
            {
                DirectoryInfo dir = new DirectoryInfo(carpe);
                FileInfo[] fcc = dir.GetFiles(ca2 + "*.*");
                foreach (FileInfo f in fcc)
                {
                    pa = carpe + "\\" + f.Name;
                    if (File.Exists(pa)) File.Delete(pa);
                }
            }

            nom = carpe + "\\" + ca2;
            ca = string.Format("_{0:000}_{1:00}_", rumbo, buza);
            if (pitch < 0) ca += "N"; else ca += "P";
            ca += string.Format("{0:000}", Math.Abs(pitch));
            nom += ca + ".fcm";

            StreamWriter wr = File.CreateText("focmecdat.txt");
            ca = string.Format("{0:0.0} {1:0.0} {2:0.0}   {3:00} {4:000}   Strike Dip Rake   P_Errores_Aceptados  Total soluciones", rumbo, buza, pitch, numerrP, totfocmec);
            wr.WriteLine(ca);
            wr.WriteLine(mejpun);
            wr.Close();
            if (File.Exists("focmecdat.txt")) File.Copy("focmecdat.txt", nom, true);

            if (cond == true) panelFocmec.Visible = false;

            return;
        }

        private void boXpanelMl_Click(object sender, EventArgs e)
        {
            panelML.Visible = false;
        }

        void EscriPola()
        {
            int i, j, j1, j2, k, maxim;
            string ca = "";

            maxim = 55;
            j2 = 0;
            for (i = 0; i < Ma; i++) if (char.IsLetter(pol2[i])) j2 += 1;
            if (j2 <= maxim) j1 = 75;
            else
            {
                j1 = 170;
                j2 = maxim;
            }

            if (j2 == 0)
            {
                panelPola.Visible = false;
                return;
            }

            panelPola.Size = new Size(j1, 25 + j2 * 15);

            Graphics dc = panelPola.CreateGraphics();
            SolidBrush br = new SolidBrush(Color.PapayaWhip);
            dc.FillRectangle(br, 0, 0, panelPola.Width, panelPola.Height);
            br.Dispose();
            SolidBrush bro = new SolidBrush(Color.Black);
            SolidBrush bro2 = new SolidBrush(Color.Blue);

            k = 0;
            j = 0;
            j1 = 10;
            for (i = 0; i < nutra; i++)
            {
                if (char.IsLetter(pol2[i]))
                {
                    ca = est[i].Substring(0, 4);// +" " + pol[i];
                    dc.DrawString(ca, new Font("Times New Roman", 10), bro, j1, 21 + k * 15);
                    ca = pol[i].ToString();
                    dc.DrawString(ca, new Font("Times New Roman", 10, FontStyle.Bold), bro2, j1 + 40, 21 + k * 15);
                    k += 1;
                    j += 1;
                    if (j == maxim)
                    {
                        k = 0;
                        j1 = 90;
                    }
                    //MessageBox.Show(est[i] + " " + pol[i]);
                }
            }
            bro.Dispose();
            bro2.Dispose();

            return;
        }

        private void panelPola_MouseDown(object sender, MouseEventArgs e)
        {
            int i, j, k, num, maxim;

            maxim = 55;
            i = (int)(e.X / 75.0);
            j = (int)((e.Y - 20) / 15.0);
            num = j + i * maxim;

            k = 0;
            j = -1;
            for (i = 0; i < Ma; i++)
            {
                if (char.IsLetter(pol2[i]))
                {
                    if (k == num)
                    {
                        j = i;
                        break;
                    }
                    k += 1;
                }
            }

            try
            {
                if (char.IsLetter(pol[j])) pol[j] = ' ';
                else pol[j] = pol2[j];
            }
            catch
            {
                return;
            }
            EscriPola();

            return;
        }

        private void boPola_MouseDown(object sender, MouseEventArgs e)
        {

            if (e.Button == MouseButtons.Left)
            {
                if (pola == true)
                {
                    pola = false;
                    boPola.BackColor = Color.Red;
                }
                else
                {
                    pola = true;
                    boPola.BackColor = Color.GreenYellow;
                }
            }
            else
            {
                if (panelPola.Visible == false)
                {
                    panelPola.Visible = true;
                    EscriPola();
                }
                else panelPola.Visible = false;
            }
        }

        private void boPolaTra_Click(object sender, EventArgs e)
        {
            int i;

            for (i = 0; i < nutra; i++)
            {
                if (char.IsLetter(pol2[i])) siEst[i] = true;
                else siEst[i] = false;
            }
            panel1.Invalidate();
            botodas.BackColor = Color.PaleVioletRed;
        }

        private void boPolaX_Click(object sender, EventArgs e)
        {
            panelPola.Visible = false;
        }

        private void panelPola_Paint(object sender, PaintEventArgs e)
        {
            EscriPola();
        }

        private void boFasAuto_MouseDown(object sender, MouseEventArgs e)
        {
            if (fasauto == false)
            {
                fasauto = true;
                boFasAuto.BackColor = Color.ForestGreen;
            }
            else
            {
                fasauto = false;
                boFasAuto.BackColor = Color.Linen;
            }
        }

        void Fases_Automaticas(double tiini)
        {
            int i, j, nmi, xf, pro, mx, mn, mufin, ini = 0, umbral = 0, cont = 0, dif = 0;
            double fax;
            string li = "", esta = "";
            char[] delim = { ' ', '\t' };
            string[] pa = null;
            bool ya = false;

            xf = panel1.Size.Width - 40;
            fax = xf / dura;
            cont = 0;
            for (i = 0; i < fr1.nutra; i++) siEst[i] = false;
            StreamReader ar = new StreamReader(".\\pro\\estafases.txt");
            while (li != null)
            {
                try
                {
                    li = ar.ReadLine();
                    if (li == null || li[0] == '*') break;
                    pa = li.Split(delim);
                    esta = pa[0].Substring(0, 4);
                    umbral = int.Parse(pa[1]);
                    ini = 0;
                    for (i = 0; i < fr1.nutra; i++)
                    {
                        // if (siEst[i] == true)
                        // {
                        if (esta.Substring(0, 4) == est[i].Substring(0, 4))
                        {
                            mufin = (int)(10.0 * ra[i]);  //10 segundos
                            nmi = (int)((tiini - tim[i][0]) * ra[i]);
                            if (nmi - 20 < 0 || nmi >= lar[i] - mufin) break;
                            mx = cu[i][nmi - 40];
                            mn = mx;
                            for (j = nmi - 39; j <= nmi; j++)
                            {
                                if (mx < cu[i][j]) mx = cu[i][j];
                                else if (mn > cu[i][j]) mn = cu[i][j];
                            }
                            pro = (int)((mx + mn) / 2.0);
                            //MessageBox.Show(est[i].Substring(0,4)+" mx="+mx.ToString()+" mn="+mn.ToString()+" pro="+pro.ToString());
                            for (j = nmi; j < nmi + mufin; j++)
                            {
                                dif = cu[i][j] - cu[i][j - 1];
                                if (Math.Abs(cu[i][j] - pro) > 2.0 * Math.Abs(mx - pro))
                                {
                                    if (ya == false)
                                    {
                                        ya = true;
                                        ini = j;
                                    }
                                }
                                else if (Math.Abs(cu[i][j] - pro) <= 2.0 * Math.Abs(mx - pro))
                                {
                                    ya = false;
                                    ini = 0;
                                }
                                if (Math.Abs(cu[i][j] - pro) > umbral)
                                {
                                    fas[i][0] = tim[i][ini];
                                    pes[i][0] = 1;
                                    if (cu[i][j] > pro) pol[i] = 'C';
                                    else pol[i] = 'D';
                                    pol2[i] = pol[i];
                                    siEst[i] = true;
                                    cont += 1;
                                    break;
                                }
                            }
                            break;
                        } // if esta...
                        // } // if est[i]==true
                    } // for (i....                    
                    //if (ini == 0 && i<fr1.can) siEst[i] = false;
                    //MessageBox.Show(esta+" ini="+ini.ToString()+" siest="+siEst[i].ToString());
                }
                catch
                {
                }
            }
            ar.Close();

            if (cont >= 3)
            {
                boHp71.Visible = true;
                boOrd.Visible = true;
            }
            panel1.Invalidate();


            return;
        }

        private void bomemo_Click(object sender, EventArgs e)
        {
            int i;

            for (i = 0; i < nutra; i++) siEstMem[i] = siEst[i];
            bomemo.BackColor = Color.Tan;
            bomemest.Visible = true;
        }

        private void bomemest_Click(object sender, EventArgs e)
        {
            int i;

            for (i = 0; i < nutra; i++) siEst[i] = siEstMem[i];
            panel1.Invalidate();
            if (panelEstHp71.Visible == true) panelEstHp71.Visible = false;
        }

        private void boXmem_Click(object sender, EventArgs e)
        {
            panelEstHp71.Visible = false;
        }

        private void boRota_Click(object sender, EventArgs e)
        {

            if (rota == false)
            {
                rota = true;
                boRota.BackColor = Color.ForestGreen;
            }
            else
            {
                rota = false;
                boRota.BackColor = Color.PaleGoldenrod;
                if (panelRotaRT.Visible == true) panelRotaRT.Visible = false;
            }
        }

        private void boXRota_Click(object sender, EventArgs e)
        {
            panelRotaRT.Visible = false;
            panelmapa.Visible = false;
        }

        private void panelRotaRT_Paint(object sender, PaintEventArgs e)
        {
            RadialTransversal();
            DibujoRT(cuRT);
            DibujoZNE_RT();
            MarcasTiRota();
            if (panelmapa.Visible == false) return;
            Graphics dc = panelmapa.CreateGraphics();
            SolidBrush bro = new SolidBrush(Color.LavenderBlush);
            dc.FillRectangle(bro, 0, 0, panelmapa.Width, panelmapa.Height);
            bro.Dispose();
            util.Topo(panelmapa,fr1.volcan[vol],facm,fr1.latvol[vol],fr1.lonvol[vol],
                 lasi, losi, Color.Gray, true, lafR, lofR, laeR, loeR);
        }

        void RadialTransversal()
        {
            short ii;
            short[] nuH = new short[2];
            int i, k, gra;
            double min, dd, azi = 0, ang, fac, laa, loo, fac2;
            double[] laro = new double[3];
            double[] loro = new double[3];
            //double[][] cuRT = new double[2][];
            string li = "", nom = "";

            if (!File.Exists(".\\h\\X.mod"))
            {
                MessageBox.Show("NO existe .\\h\\X.mod");
                return;
            }

            try
            {
                nom = estrot[0].Substring(0, 3);
            }
            catch
            {
                return;
            }
            ii = 0;

            StreamReader ar = new StreamReader(".\\h\\X.mod");
            while (li != null)
            {
                try
                {
                    li = ar.ReadLine();
                    if (li == null) break;
                    if (li.Length < 22) continue;
                    if (li[0] == ' ' && li[1] == ' ' && char.IsLetter(li[2]))
                    {
                        if (nom == li.Substring(2, 3))
                        {
                            k = -1;
                            for (i = 0; i < 3; i++)
                            {
                                if (li.Substring(2, 4) == estrot[i])
                                {
                                    k = i;
                                    break;
                                }
                            }
                            if (k != -1)
                            {
                                gra = int.Parse(li.Substring(6, 2));
                                min = double.Parse(li.Substring(8, 5));
                                dd = (double)(gra) + min / 60.0;
                                if (li[13] == 'S') dd = -1.0 * dd;
                                laro[k] = dd;
                                gra = int.Parse(li.Substring(14, 3));
                                min = double.Parse(li.Substring(17, 5));
                                dd = (double)(gra) + min / 60.0;
                                if (li[22] == 'W') dd = -1.0 * dd;
                                loro[k] = dd;
                                ii += 1;
                            }
                        }
                        if (ii == 3) break;
                    }
                }
                catch { }
            }
            ar.Close();
            if (ii != 3) return;
            for (i = 1; i < 3; i++)
            {
                if (laro[i - 1] != laro[i] || loro[i - 1] != loro[i])
                {
                    MessageBox.Show("Componentes con DISTINTAS Coordenadas?");
                    return;
                }
            }

            laeR = laro[0];
            loeR = loro[0];
            laa = laro[0] - lafR;
            loo = loro[0] - lofR;
            fac2 = 180.0 / Math.PI;
            if (loro[0] != lofR)
            {
                ang = Math.Atan(Math.Abs(laa) / Math.Abs(loo)) * fac2;
                if (lafR >= laro[0] && lofR > loro[0]) azi = (float)(90.0 - ang);
                else if (laro[0] > lafR && loro[0] < lofR) azi = (float)(90.0 + ang);
                else if (laro[0] > lafR && loro[0] >= lofR) azi = (float)(270.0 - ang);
                else if (laro[0] < lafR && loro[0] >= lofR) azi = (float)(270.0 + ang);
            }
            else
            {
                if (lafR >= laro[0]) azi = 0;
                else azi = 180.0F;
            }

            fac = Math.PI / 180.0;
            ang = azi * fac;

            cuRT[0] = new double[nmrot[1][1] - nmrot[1][0]];
            cuRT[1] = new double[nmrot[2][1] - nmrot[2][0]];
            for (i = nmrot[1][0]; i < nmrot[1][1]; i++)
                cuRT[0][i - nmrot[1][0]] = cu[idestrot[1]][i] * Math.Cos(ang) + cu[idestrot[2]][i] * Math.Sin(ang);
            for (i = nmrot[2][0]; i < nmrot[2][1]; i++)
                cuRT[1][i - nmrot[2][0]] = -1.0 * cu[idestrot[1]][i] * Math.Sin(ang) + cu[idestrot[2]][i] * Math.Cos(ang);

            //DibujoRT(cuRT);

            //cuRT[0][i]=cuH[0][i]*Math.Cos(ang)+cuH[1][i]*Math.Sin(ang);
            //cuRT[1][i]=-1.0*cuH[0][i]*Math.Sin(ang)+cuH[1][i]*Math.Cos(ang);

            return;
        }

        void DibujoRT(double[][] cuRT)
        {

            int i, ii, jj, k, kk, xf, yf;
            int nmi, nmf, numu, pro;
            float iniy;
            double x1, y1, fax, fay, fy, dif, duracion, t1, t2, dife;
            double[] mxR = new double[2];
            double[] mnR = new double[2];
            double proR;
            string ca = "";
            Point[] dat;


            xf = panelRotaRT.Size.Width - 40;
            yf = panelRotaRT.Size.Height - 35;

            try
            {
                t1 = tim[idestrot[1]][nmrot[1][0]]; // idestrot[1] es la componente Norte. nmrot[0][0] es la muestra menor de la seleccion
                if (t1 > tim[idestrot[2]][nmrot[2][0]]) t1 = tim[idestrot[2]][nmrot[2][0]];
                t2 = tim[idestrot[1]][nmrot[1][1]];
                if (t2 < tim[idestrot[2]][nmrot[2][1]]) t2 = tim[idestrot[2]][nmrot[2][1]];
            }
            catch
            {
                return;
            }
            duracion = t2 - t1;
            fax = (float)(xf / duracion);
            fay = (float)(yf / 5.0);

            for (i = 0; i < 2; i++)
            {
                mxR[i] = cuRT[i][0];
                mnR[i] = mxR[i];
                for (ii = 1; ii < cuRT[i].Length; ii++)
                {
                    if (mxR[i] < cuRT[i][ii]) mxR[i] = cuRT[i][ii];
                    else if (mnR[i] > cuRT[i][ii]) mnR[i] = cuRT[i][ii];
                }
            }

            proR = (mxR[0] + mnR[0]) / 2.0;
            dife = mxR[0] - proR;
            proR = (mxR[1] + mnR[1]) / 2.0;
            if (dife < (mxR[1] - proR)) dife = mxR[1] - proR;
            fy = ((fay / 2) / dife);

            Graphics dc = panelRotaRT.CreateGraphics();
            //Pen lap = new Pen(Color.Black, 1);
            Pen lap = new Pen(Color.DarkBlue, 1);
            Pen lap2 = new Pen(Color.DarkOrange, 1);
            lap2.DashStyle = DashStyle.DashDot;
            SolidBrush bro = new SolidBrush(Color.Blue);

            jj = 0;
            for (i = 0; i < 2; i++)
            {
                try
                {
                    pro = (int)((mxR[i] + mnR[i]) / 2);
                    kk = 0;
                    iniy = (float)(25.0 + 3.0 * fay + (float)(jj * fay + fay / 2.0));
                    dc.DrawLine(lap2, 40, iniy, xf, iniy);
                    nmi = 0;
                    nmf = (int)(duracion * ra[idestrot[i + 1]]);
                    if (nmf >= cuRT[i].Length) nmf = cuRT[i].Length - 1;
                    numu = nmf - nmi;
                    if (numu < 2)
                    {
                        ca = "VACIO o muy pocas Muestras !!!";
                        dc.DrawString(ca, new Font("Times New Roman", 14, FontStyle.Bold), bro, 100, (float)(iniy - 10));
                    }
                    else
                    {
                        dat = new Point[numu];
                        for (k = nmi; k < nmf; k++)
                        {
                            if (kk >= numu) break;
                            dif = cuRT[jj][k] - pro;
                            y1 = iniy - dif * fy;
                            //x1 = 40.0F + (float)((tim[idestrot[i+1]][nmrot[i+1][0]+k]-tiniRot)*fax);
                            x1 = 40.0F + (float)((tim[idestrot[i + 1]][nmrot[i + 1][0] + k] - tim[idestrot[i + 1]][nmrot[i + 1][0]]) * fax);
                            dat[kk].Y = (int)(y1);
                            dat[kk].X = (int)(x1);
                            kk += 1;
                        }
                        dc.DrawLines(lap, dat);
                        if (i == 0) ca = "R";
                        else ca = "T";
                        dc.DrawString(ca, new Font("Times New Roman", 10), bro, 1, iniy - 5);
                    }
                    jj += 1;
                }
                catch
                {
                }
            }

            bro.Dispose();
            lap2.Dispose();
            lap.Dispose();


            return;
        }

        void DibujoZNE_RT()
        {

            int i, ii, jj, k, kk, xf, yf;
            int nmi, nmf, numu, pro;
            float iniy;
            double x1, y1, fax, fay, fy, dif, duracion, t1, t2, dife;
            double[] mxR = new double[3];
            double[] mnR = new double[3];
            double proR;
            string ca = "";
            Point[] dat;


            xf = panelRotaRT.Size.Width - 40;
            yf = panelRotaRT.Size.Height - 35;

            try
            {
                t1 = tim[idestrot[1]][nmrot[0][0]]; // idestrot[1] es la componente Norte. nmrot[0][0] es la muestra menor de la seleccion
                if (t1 > tim[idestrot[2]][nmrot[1][0]]) t1 = tim[idestrot[2]][nmrot[1][0]];
                t2 = tim[idestrot[1]][nmrot[0][1]];
                if (t2 < tim[idestrot[2]][nmrot[1][1]]) t2 = tim[idestrot[2]][nmrot[1][1]];
            }
            catch
            {
                return;
            }
            duracion = t2 - t1;
            fax = (float)(xf / duracion);
            fay = (float)(yf / 5.0);

            for (i = 0; i < 3; i++)
            {
                mxR[i] = cu[idestrot[i]][nmrot[i][0]];
                mnR[i] = mxR[i];
                for (ii = nmrot[i][0] + 1; ii < nmrot[i][1]; ii++)
                {
                    if (mxR[i] < cu[idestrot[i]][ii]) mxR[i] = cu[idestrot[i]][ii];
                    else if (mnR[i] > cu[idestrot[i]][ii]) mnR[i] = cu[idestrot[i]][ii];
                }
            }

            proR = (mxR[0] + mnR[0]) / 2.0;
            dife = mxR[0] - proR;
            proR = (mxR[1] + mnR[1]) / 2.0;
            if (dife < (mxR[1] - proR)) dife = mxR[1] - proR;
            proR = (mxR[2] + mnR[2]) / 2.0;
            if (dife < (mxR[2] - proR)) dife = mxR[2] - proR;
            fy = ((fay / 2) / dife);

            Graphics dc = panelRotaRT.CreateGraphics();
            //Pen lap = new Pen(Color.DarkGray, 1);
            Pen lap = new Pen(Color.Black, 1);
            Pen lap2 = new Pen(Color.DarkOrange, 1);
            lap2.DashStyle = DashStyle.DashDot;
            SolidBrush bro = new SolidBrush(Color.Blue);

            jj = 0;
            for (i = 0; i < 3; i++)
            {
                try
                {
                    pro = (int)((mxR[i] + mnR[i]) / 2);
                    kk = 0;
                    //iniy = (float)(25.0 + i * fay + (float)(jj * fay + fay / 2.0));
                    iniy = (float)(25.0 + (float)(jj * fay + fay / 2.0));
                    dc.DrawLine(lap2, 40, iniy, xf, iniy);
                    //nmi = 0;
                    nmi = nmrot[i][0];
                    //nmf = (int)(duracion * ra[idestrot[i]]);
                    nmf = nmrot[i][1];
                    if (nmf >= cu[idestrot[i]].Length) nmf = cu[idestrot[i]].Length - 1;
                    numu = nmf - nmi;
                    if (numu < 2)
                    {
                        ca = "VACIO o muy pocas Muestras !!!";
                        dc.DrawString(ca, new Font("Times New Roman", 14, FontStyle.Bold), bro, 100, (float)(iniy - 10));
                    }
                    else
                    {
                        dat = new Point[numu];
                        for (k = nmi; k < nmf; k++)
                        {
                            if (kk >= numu) break;
                            dif = cu[idestrot[i]][k] - pro;
                            y1 = iniy - dif * fy;
                            //x1 = 40.0F + (float)((tim[idestrot[i]][nmrot[i][0] + k] - tiniRot) * fax);
                            //x1 = 40.0F + (float)((tim[idestrot[i]][k] - tiniRot) * fax);
                            x1 = 40.0F + (float)((tim[idestrot[i]][k] - tim[idestrot[i]][nmi]) * fax);
                            dat[kk].Y = (int)(y1);
                            dat[kk].X = (int)(x1);
                            //MessageBox.Show("x1="+x1.ToString()+" y1="+y1.ToString());
                            kk += 1;
                        }
                        dc.DrawLines(lap, dat);
                        //if (i == 0) ca = "R";
                        //else ca = "T";
                        ca = estrot[i];
                        dc.DrawString(ca, new Font("Times New Roman", 10), bro, 1, iniy - 5);
                    }
                    jj += 1;
                }
                catch
                {
                }
            }

            bro.Dispose();
            lap2.Dispose();
            lap.Dispose();


            return;
        }

        void MarcasTiRota()
        {
            ushort i, j;
            int xf, nmi;
            float x1, y1, ff;
            double fay, fax, t1, t2, duracion;
            Pen[] lap = new Pen[3];

            if (panelRotaRT.Visible == false) return;
            xf = panelRotaRT.Size.Width - 40;

            try
            {
                t1 = tim[idestrot[1]][nmrot[0][0]]; // idestrot[1] es la componente Norte. nmrot[0][0] es la muestra menor de la seleccion
                if (t1 > tim[idestrot[2]][nmrot[1][0]]) t1 = tim[idestrot[2]][nmrot[1][0]];
                t2 = tim[idestrot[1]][nmrot[0][1]];
                if (t2 < tim[idestrot[2]][nmrot[1][1]]) t2 = tim[idestrot[2]][nmrot[1][1]];
            }
            catch
            {
                return;
            }
            duracion = t2 - t1;

            fay = panelRotaRT.Size.Height / 5;
            fax = xf / duracion;
            ff = (float)(fay / 3);

            Graphics dc = panelRotaRT.CreateGraphics();
            lap[0] = new Pen(colP, 1);
            lap[1] = new Pen(colS, 1);
            lap[2] = new Pen(colC, 1);

            for (j = 0; j < 3; j++)
            {
                for (i = 0; i < 3; i++)
                {
                    nmi = nmrot[i][0];
                    //if (i==1) MessageBox.Show("i="+i.ToString()+" idestrot="+idestrot[i].ToString()+" fase="+fas[idestrot[i]][i].ToString());
                    if (fas[idestrot[j]][i] <= 0) continue;
                    x1 = (float)(40.0 + (fas[idestrot[j]][i] - tim[idestrot[j]][nmi]) * fax);
                    y1 = (float)(j * fay + fay / 2);
                    dc.DrawLine(lap[i], x1, y1 - ff, x1, y1 + ff);
                }
            }
            lap[0].Dispose();
            lap[1].Dispose();
            lap[2].Dispose();

            return;
        }

        private void panelmapa_MouseDown(object sender, MouseEventArgs e)
        {
            int xf, yf, iniX, iniY;
            double fcpi, fclo, lat, lon;

            xf = panelmapa.Size.Width;
            yf = panelmapa.Size.Height;
            iniX = xf / 2;
            iniY = yf / 2;

            lat = fr1.latvol[vol];
            lon = fr1.lonvol[vol];
            fcpi = Math.PI / 180.0;
            fclo = facm * (fcpi * Math.Cos(lat * fcpi) * 6367.449) / 110.9;


            if (panelRotaRT.Visible == true)
            {
                lafR = lat + (iniY - e.Y) / facm;
                lofR = lon + (e.X - iniX) / fclo;
                panelRotaRT.Invalidate();
            }
        }

        private void boGuiRota_Click(object sender, EventArgs e)
        {

            if (guiaRota == false)
            {
                guiaRota = true;
                boGuiRota.BackColor = Color.BlueViolet;
                boPRota.Visible = false;
                boSRota.Visible = false;
                boCRota.Visible = false;
            }
            else
            {
                guiaRota = false;
                boGuiRota.BackColor = Color.PeachPuff;
                boPRota.Visible = true;
                boSRota.Visible = true;
                boCRota.Visible = true;
            }
        }

        private void boMasRota_MouseDown(object sender, MouseEventArgs e)
        {
            int i, dif;

            if (e.Button == MouseButtons.Left)
            {
                for (i = 0; i < 3; i++)
                {
                    dif = nmrot[i][1] - nmrot[i][0];
                    nmrot[i][0] += (int)(dif / 10.0);
                    nmrot[i][1] -= (int)(dif / 10.0);
                    panelRotaRT.Invalidate();
                }
            }
            else
            {
                for (i = 0; i < 3; i++)
                {
                    dif = nmrot[i][1] - nmrot[i][0];
                    nmrot[i][0] -= (int)(dif / 10.0);
                    nmrot[i][1] += (int)(dif / 10.0);
                    panelRotaRT.Invalidate();
                }
            }
        }

        private void boMenosRota_MouseDown(object sender, MouseEventArgs e)
        {
            int i, dif;

            if (e.Button == MouseButtons.Left)
            {
                for (i = 0; i < 3; i++)
                {
                    dif = nmrot[i][1] - nmrot[i][0];
                    nmrot[i][0] -= (int)(dif / 10.0);
                    nmrot[i][1] -= (int)(dif / 10.0);
                    panelRotaRT.Invalidate();
                }
            }
            else
            {
                for (i = 0; i < 3; i++)
                {
                    dif = nmrot[i][1] - nmrot[i][0];
                    nmrot[i][0] += (int)(dif / 10.0);
                    nmrot[i][1] += (int)(dif / 10.0);
                    panelRotaRT.Invalidate();
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            lafR = lasi;
            lofR = losi;
            panelRotaRT.Invalidate();
        }

        private void bofacmapRota_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                facm += 0.1 * facm;
            }
            else
            {
                facm -= 0.1 * facm;
            }
            panelRotaRT.Invalidate();
        }

        private void boPRota_Click(object sender, EventArgs e)
        {
            ondRota = 0;
            boPRota.BackColor = Color.ForestGreen;
            boSRota.BackColor = Color.NavajoWhite;
            boCRota.BackColor = Color.PapayaWhip;
        }

        private void boSRota_Click(object sender, EventArgs e)
        {
            ondRota = 1;
            boPRota.BackColor = Color.Bisque;
            boSRota.BackColor = Color.ForestGreen;
            boCRota.BackColor = Color.PapayaWhip;
        }

        private void boCRota_Click(object sender, EventArgs e)
        {
            ondRota = 2;
            boPRota.BackColor = Color.Bisque;
            boSRota.BackColor = Color.NavajoWhite;
            boCRota.BackColor = Color.ForestGreen;
        }

        private void panelRotaRT_MouseDown(object sender, MouseEventArgs e)
        {
            int    j, xf, yf, nmi;
            double t1, t2, fax, fay, duracion, timm;

            if (guiaRota == true)
            {
                Graphics dc = panelRotaRT.CreateGraphics();
                Pen lap = new Pen(Color.DarkGray, 1);
                dc.DrawLine(lap, e.X, 20, e.X, panelRotaRT.Height - 20);
                lap.Dispose();
                return;
            }
            xf = panelRotaRT.Size.Width - 40;
            yf = panelRotaRT.Size.Height - 35;
            t1 = tim[idestrot[1]][nmrot[0][0]]; // idestrot[1] es la componente Norte. nmrot[0][0] es la muestra menor de la seleccion
            if (t1 > tim[idestrot[2]][nmrot[1][0]]) t1 = tim[idestrot[2]][nmrot[1][0]];
            t2 = tim[idestrot[1]][nmrot[0][1]];
            if (t2 < tim[idestrot[2]][nmrot[1][1]]) t2 = tim[idestrot[2]][nmrot[1][1]];
            duracion = t2 - t1;
            fax = (float)(xf / duracion);
            fay = (float)(yf / 5.0);

            j = (int)((e.Y - 25.0) / fay);
            if (j > 2) return;
            nmi = nmrot[j][0];
            timm = (e.X - 40.0) / fax + tim[idestrot[j]][nmi];
            fas[idestrot[j]][ondRota] = timm;
            MarcasTiRota();        
        }

        private void textBoxHPC_TextChanged(object sender, EventArgs e)
        {
            float ff;

            try
            {
                ff = float.Parse(textBoxHPC.Text);
            }
            catch
            {
                return;
            }
            valHP71PC = ff;
        }

        private void textBoxHPC_DoubleClick(object sender, EventArgs e)
        {
            if (HP71PC == false)
            {
                HP71PC = true;
                textBoxHPC.BackColor = Color.Lime;
            }
            else
            {
                HP71PC = false;
                textBoxHPC.BackColor = Color.Orange;
            }
        }

        void LeerInp()
        {
            int i, j;
            string li = "", nom = "";
            ArrayList lis = new ArrayList();

            //nom = ".\\h\\" + fr1.Volcan[vol][0] + ".mod";
            //nom = ".\\h\\r.inp";
            if (vol_viejo == -1 && fr1.ModAux == true && fr1.sismo.Substring(10, 2) == fr1.ClAux.Substring(0, 2))
                nom = ".\\h\\9.mod";
            else nom = ".\\h\\" + fr1.volcan[vol][0] + ".mod";
            if (!File.Exists(nom)) return;

            lis.Clear();
            StreamReader ar = new StreamReader(nom);
            while (li != null)
            {
                try
                {
                    li = ar.ReadLine();
                    if (li == null) break;
                    if (li.Length < 20) continue;
                    if (li[10] == '.' && li[19] == '.') lis.Add(li.Substring(2, 4));
                }
                catch
                {
                }
            }
            ar.Close();
            if (lis.Count >= 300)
            {
                MessageBox.Show("Hay 300 o mas estaciones en " + fr1.volcan[vol][0] + ".mod\nEs posible que tenga problemas con el Hypo71!!.");
            }
            if (lis.Count < 3) return;

            siH = new bool[nutra];
            for (i = 0; i < nutra; i++) siH[i] = false;
            for (j = 0; j < lis.Count; j++)
            {
                for (i = 0; i < nutra; i++)
                {
                    if (string.Compare(lis[j].ToString().Substring(0, 4), est[i].Substring(0, 4)) == 0)
                    {
                        siH[i] = true;
                        break;
                    }
                }
            }

            return;
        }

        private void boFiltro_Click(object sender, EventArgs e)
        {
            cfilt = '0';
            if (panelFilt.Visible == false) panelFilt.Visible = true;
            else
            {
                panelFilt.Visible = false;
            }
            textBox1.BackColor = Color.White;
            textBox2.BackColor = Color.White;
            sifilt = false;
            boFilBaj.BackColor = Color.White;
            boFilAlt.BackColor = Color.White;
            boFilBan.BackColor = Color.White;

        }

        void AplicarFiltro()
        {
            int i, j;

            panelmen.Visible = true;
            util.Mensaje(panelmen, "Calculando Filtro ...", false);

            if (yafilt == false)
            {
                mxF = new int[fr1.nutra];
                mnF = new int[fr1.nutra];
                proF = new int[fr1.nutra];
                cf = new int[fr1.nutra][];
                for (i = 0; i < fr1.nutra; i++) cf[i] = new int[cu[i].Length];
                yafilt = true;
            }
            if (cfilt == '1') for (i = 0; i < fr1.nutra; i++) cf[i] = util.PasaBajos(cu[i], M, (float)(ra[i]), Fc1);
            else if (cfilt == '2') for (i = 0; i < fr1.nutra; i++) cf[i] = util.PasaAltos(cu[i], M, (float)(ra[i]), Fc1);
            else if (cfilt == '3') for (i = 0; i < fr1.nutra; i++) cf[i] = util.PasaBanda(cu[i], M, (float)(ra[i]), Fc1, Fc2);
            else
            {
                panelmen.Visible = false;
                return;
            }

            for (i = 0; i < fr1.nutra; i++)
            {
                mxF[i] = cf[i][M];
                mnF[i] = mxF[i];
                for (j = M + 1; j < cf[i].Length - M; j++)
                {
                    if (mxF[i] < cf[i][j]) mxF[i] = cf[i][j];
                    else if (mnF[i] > cf[i][j]) mnF[i] = cf[i][j];
                }
                proF[i] = (int)((mxF[i] + mnF[i]) / 2.0);
            }
            panelmen.Visible = false;
        }

        void FiltroBaja()
        {
            if (cfilt != '1') yafilt = false;
            cfilt = '1';
            if (sifilt == false)
            {
                sifilt = true;
                if (yafilt == false) AplicarFiltro();
                boFilBaj.BackColor = Color.Green;
                boFilAlt.BackColor = Color.White;
                boFilBan.BackColor = Color.White;
                textBox1.BackColor = Color.Orange;
                textBox2.BackColor = Color.White;
            }
            else
            {
                sifilt = false;
                boFilBaj.BackColor = Color.White;
                boFilAlt.BackColor = Color.White;
                boFilBan.BackColor = Color.White;
                textBox1.BackColor = Color.White;
                textBox2.BackColor = Color.White;
            }
            panel1.Invalidate();
        }
        private void boFilBaj_Click(object sender, EventArgs e)
        {
            FiltroBaja();
        }

        void FiltroAlta()
        {
            if (cfilt != '2') yafilt = false;
            cfilt = '2';
            if (sifilt == false)
            {
                sifilt = true;
                if (yafilt == false) AplicarFiltro();
                boFilBaj.BackColor = Color.White;
                boFilAlt.BackColor = Color.Green;
                boFilBan.BackColor = Color.White;
                textBox1.BackColor = Color.Orange;
                textBox2.BackColor = Color.White;
            }
            else
            {
                sifilt = false;
                boFilBaj.BackColor = Color.White;
                boFilAlt.BackColor = Color.White;
                boFilBan.BackColor = Color.White;
                textBox1.BackColor = Color.White;
                textBox2.BackColor = Color.White;
            }
            panel1.Invalidate();
        }
        private void boFilAlt_Click(object sender, EventArgs e)
        {
            FiltroAlta();
        }

        void FiltroBanda()
        {
            if (cfilt != '3') yafilt = false;
            cfilt = '3';
            if (sifilt == false)
            {
                sifilt = true;
                if (yafilt == false) AplicarFiltro();
                boFilBaj.BackColor = Color.White;
                boFilAlt.BackColor = Color.White;
                boFilBan.BackColor = Color.Green;
                textBox1.BackColor = Color.Orange;
                textBox2.BackColor = Color.Orange;
            }
            else
            {
                sifilt = false;
                boFilBaj.BackColor = Color.White;
                boFilAlt.BackColor = Color.White;
                boFilBan.BackColor = Color.White;
                textBox1.BackColor = Color.White;
                textBox2.BackColor = Color.White;
            }
            panel1.Invalidate();
        }
        private void boFilBan_Click(object sender, EventArgs e)
        {
            FiltroBanda();
        }



        private void boM_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) M = (short)(M * 2);
            else M = (short)(M / 2.0);
            if (M > 512) M = 128;
            else if (M < 128) M = 512;
            boM.Text = M.ToString();
            cfilt = '0';
            sifilt = false;
            yafilt = false;
            boFilBaj.BackColor = Color.White;
            boFilAlt.BackColor = Color.White;
            boFilBan.BackColor = Color.White;
            textBox1.BackColor = Color.White;
            textBox2.BackColor = Color.White;
        }


        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            float ff;

            try
            {
                ff = float.Parse(textBox1.Text);
            }
            catch
            {
                return;
            }
            Fc1 = ff;
            yafilt = false;
            cfilt = '0';
            textBox1.BackColor = Color.White;
            textBox2.BackColor = Color.White;
        }

        private void textBox1_Validated(object sender, EventArgs e)
        {
            textBox1.Text = string.Format("{0:00.00}", Fc1);
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            float ff;

            try
            {
                ff = float.Parse(textBox2.Text);
            }
            catch
            {
                return;
            }
            Fc2 = ff;
            yafilt = false;
            cfilt = '0';
            textBox1.BackColor = Color.White;
            textBox2.BackColor = Color.White;
        }

        private void textBox2_Validated(object sender, EventArgs e)
        {
            textBox2.Text = string.Format("{0:00.00}", Fc2);
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            float ff;

            cfilt = '0';
            if (e.KeyCode == Keys.Enter)
            {
                try
                {
                    ff = float.Parse(textBox1.Text);
                }
                catch
                {
                    return;
                }
                if (ff < Fc2) Fc1 = ff;
                textBox1.Text = string.Format("{0:00.00}", Fc1);
            }
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            float ff;

            cfilt = '0';
            if (e.KeyCode == Keys.Enter)
            {
                try
                {
                    ff = float.Parse(textBox2.Text);
                }
                catch
                {
                    return;
                }
                if (ff > Fc1) Fc2 = ff;
                textBox2.Text = string.Format("{0:00.00}", Fc2);
            }
        }
       


       

        
       

       

        

    }
}
