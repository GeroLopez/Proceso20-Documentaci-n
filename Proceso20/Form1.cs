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
using System.Collections;
using System.Threading;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
//using EASendMail; //add EASendMail namespace


namespace Proceso20
{

    public partial class Form1 : Form
    {
        /// <summary>
        /// Número de trazas máximo.
        /// </summary>
        public const int Ma = 500;
        /// <summary>
        /// Permite convertir el tiempo en formato SUDS al tiempo del visual c#.
        /// </summary>
        public const double Fei = 621355968000000000.0;
        /// <summary>
        /// Permite convertir del tiempo visual c# al tiempo en formato SUDS.
        /// </summary>
        const double Feisuds = 621355968000000000.0;
        /// <summary>
        /// Permite convertir del tiempo en GCF a tiempo en SUDS.
        /// </summary>
        const double Feigcf = 627264000.0;
        /// <summary>
        /// Es el que indica en que panel se esta trabajando.
        /// 0 si esta trabajando en el panel principal.
        /// 1 si esta trabajando en el panel secundario.
        /// </summary>
        byte CualPanel = 0;
        /// <summary>
        /// Indica la cantidad de tarjetas en formato MUX.
        /// </summary>
        ushort numux = 1;
        /// <summary>
        /// Indica la cantidad de tarjetas en formato DMUX.
        /// </summary>
        ushort nudmx = 1;
        /// <summary>
        /// Indica la cantidad de tarjetas en formato GCF.
        /// </summary>
        ushort nugcf = 0;
        /// <summary>
        /// Indica si se ha seleccionado el promedio de la señal, el cual sirve como cero de referencia.
        /// 0 indica que no se ha seleccionado el promedio.
        /// 1 indica que se ha seleccionado el promedio.
        /// 2 es porque se ha buscado el promedio de un sector de traza, 
        /// el cual esta comprendido entre la muestra periodo1 y la periodo2.
        /// </summary>
        ushort sipro = 0;
        /// <summary>
        /// Se utiliza para verificar si los checkbox de las tarjetas MUX estan o no seleccionados.
        /// </summary>
        bool[] cajmux;
        /// <summary>
        /// Se utiliza para verificar si los checkbox de las tarjetas DMUX estan o no seleccionados.
        /// </summary>
        bool[] cajdmx;
        /// <summary>
        /// Se utiliza para verificar si los checkbox de las tarjetas GCF estan o no seleccionados.
        /// </summary>
        bool[] cajgcf;
        /// <summary>
        /// Se utiliza para verificar si los checkbox de las tarjetas YFILE estan o no seleccionados.
        /// </summary>
        bool[] cajyfile;
        /// <summary>
        /// Se utiliza para verificar si el checkbox de las tarjeta SEISAN está o no seleccionado.
        /// </summary>
        bool cajseis = false;
        /// <summary>
        /// Indica el movimiento de particulas para una porción de traza interpolada.
        /// </summary>
        bool mptintp = false;
        /// <summary>
        /// Valor por defecto de tiempo de las trazas a mostrar en la ventana.
        /// </summary>
        ushort totven = 60;
        /// <summary>
        /// Cantidad de lecturas de amplitud de la base.
        /// </summary>
        ushort contampl = 0;
        /// <summary>
        /// Indica el número de traza que se visualiza y se esta clasificando en el panel principal,
        /// este se modifica cada vez que se escoje una estación diferente. 
        /// </summary>
        ushort id = 1;
        /// <summary>
        /// Indica la estación que se tiene seleccionada y se esta clasificando en el panel secundario. 
        /// </summary>
        ushort ida = 1;
        /// <summary>
        /// Es la traza escogida del panel de clasificación usada para el espectro.
        /// </summary>
        ushort idc = 10000;
        /// <summary>
        /// Registra el espaciamiento entre lineas.
        /// </summary>
        ushort esp = 0;
        /// <summary>
        /// Almacena la cantidad de sismos clasificados en un lapso de tiempo seleccionado.
        /// </summary>
        ushort contarch = 0;
        /// <summary>
        /// Núnero de la estación a la que voy a leer la coda.
        /// </summary>
        short nucod;
        /// <summary>
        /// Núnero de la estación al que le voy a leer la amplitud.
        /// </summary>
        short nuampvar;
        /// <summary>
        /// 
        /// </summary>
        short nutarro = 0;
        /// <summary>
        /// 
        /// </summary>
        short totalbotoncajon;
        /// <summary>
        /// Sirve para dar un factor de dimensión al mapa.
        /// </summary>
        short factormapa = 0;
        /// <summary>
        /// Limita el espacio de dibujo del sismo para evitar que sea muy grande.
        /// </summary>
        short satur = 0;
        /// <summary>
        /// ES NUMERO DE MUESTRAS QUE EL USUARIO PUEDE ESCOJER PARA CALCULAR EL PROMEDIO
        /// </summary>
        short PROMEDIO = 0;
        /// <summary>
        /// Cantidad de pixels de la porción de traza la que se le calcula el espectro.
        /// </summary>
        short np = 1024;
        /// <summary>
        /// Posición y del espectro.
        /// </summary>
        short yesp = 0;
        /// <summary>
        /// Posición x del espectro.
        /// </summary>
        short xesp = 0;
        /// <summary>
        /// Variables para el cálculo del espectro.
        /// </summary>
        double t1esp;
        short yloc = -1;
        /// <summary>
        /// Se usa para condicionar la visualización del espectro de las trazas.
        /// </summary>
        bool VerEspectro = false;
        /// <summary>
        /// 
        /// </summary>
        bool moveresp = false;
        /// <summary>
        /// 
        /// </summary>
        bool movespcla = false;
        /// <summary>
        /// Maneja el tiempo en UT para los tiempos teoricos del Neic.
        /// </summary>
        short utNeic = 0;
        /// <summary>
        /// Es el tabindex inicial de las estaciones en formato GCF en el panel de Trajetas.
        /// </summary>
        short cajinigcf = 0;
        /// <summary>
        /// Posición en x inicial del arrastre del mouse. 
        /// </summary>
        int bxi;
        /// <summary>
        /// Posición en y inicial del arrastre del mouse. 
        /// </summary>
        int byi;
        /// <summary>
        /// Posición en x final del arrastre del mouse. 
        /// </summary>
        int bxf = 0;
        /// <summary>
        /// Posición en x inicial del lugar donde se va a poner la coda.
        /// </summary>
        int bcxi;
        /// <summary>
        /// Posición en y inicial del lugar donde se va a poner la coda.
        /// </summary>
        int bcyi;
        /// <summary>
        /// Posición en x inicial del lugar donde se va a poner la la P sel sismo a clasificar.
        /// </summary>
        int bpxi;
        /// <summary>
        /// Posición en y inicial del lugar donde se va a poner la la P sel sismo a clasificar.
        /// </summary>
        int bpyi;
        /// <summary>
        /// Posición en x donde inicia el intervalo de lectura de amplitud.
        /// </summary>
        int baxi;
        /// <summary>
        /// Posición en y donde inicia el intervalo de lectura de amplitud.
        /// </summary>
        int bayi;
        /// <summary>
        /// Posición en x donde inicia la lectura de huecos.
        /// </summary>
        int bhxi;
        /// <summary>
        /// Amplitud.
        /// </summary>
        int ampp;
        /// <summary>
        /// Periodo p1.
        /// </summary>
        int p1;
        /// <summary>
        /// Periodo p2.
        /// </summary>
        int p2;
        /// <summary>
        /// 
        /// </summary>
        int bloTremor = 0;
        /// <summary>
        /// Duración del tiempo del intervalo de traza sobre el cual se arrastró el mouse.
        /// </summary>
        float dur = 300.0F;
        /// <summary>
        /// Valor de la amplitud de visualización.
        /// </summary>
        float ampli = 1.0F;
        /// <summary>
        /// Tamaño de las pepas.
        /// </summary>
        float tam = 10.0F;
        /// <summary>
        /// Factor para el tamaño de la amplitud en el panelcoda. 
        /// </summary>
        float ampcod = 1.0F;
        /// <summary>
        /// Factor para el tamaño de la amplitud en el panelamp. 
        /// </summary>
        float ampamp = 1.0F;
        /// <summary>
        /// Factor para el tamaño de la amplitud en el panel de clasificación (panelcla). 
        /// </summary>
        float ampclas = 1.0F;
        /// <summary>
        /// 
        /// </summary>
        float periodo = 0F;
        /// <summary>
        /// Tiempo mínimo del que se registró una traza, o dicho de otra forma la lectura que empezó primero. 
        /// </summary>
        double timin;
        /// <summary>
        /// Es el mayor tiempo que hay de lectura.
        /// </summary>
        double timax;
        /// <summary>
        /// El tiempo mayor que se registro de una lectura.
        /// </summary>
        double timaxmin;
        /// <summary>
        /// Tiempo 1 de coda.
        /// </summary>
        double t1cod;
        /// <summary>
        /// Tiempo 2 de coda.
        /// </summary>
        double t2cod;
        /// <summary>
        /// Tiempo 1 de la amplitud.
        /// </summary>
        double t1amp;
        /// <summary>
        /// Tiempo 2 de la amplitud.
        /// </summary>
        double t2amp;
        /// <summary>
        /// Tiempo inicial del hueco.
        /// </summary>
        double t1hu;
        /// <summary>
        /// Tiempo final del hueco.
        /// </summary>
        double t2hu;
        /// <summary>
        /// Tiempo en formato SUDS del valor P en la clasificación de un sismo. 
        /// </summary>
        double Pti;
        /// <summary>
        /// Tiempo en formato SUDS del valor S en la clasificación de un sismo. 
        /// </summary>
        double Sti;
        /// <summary>
        /// Tiempo en formato SUDS del valor CODA en la clasificación de un sismo. 
        /// </summary>
        double Cti;
        /// <summary>
        /// Tiempo en formato SUDS del valor AMPLITUD en la clasificación de un sismo. 
        /// </summary>
        double Ati;
        /// <summary>
        /// Frecuencia de corte para trabajar el Octave en el panel principal. 
        /// </summary>
        double Fc;
        /// <summary>
        /// Frecuencia de corte para trabajar el Octave en el panel coda. 
        /// </summary>
        double Fccod;
        /// <summary>
        /// Tiempo inicial del tremor.
        /// </summary>
        double tinitremor = 0;
        /// <summary>
        /// Tiempo final del tremor.
        /// </summary>
        double tifintremor = 0;
        /// <summary>
        /// 
        /// </summary>
        double contremor = 0;
        /// <summary>
        /// 
        /// </summary>
        double incTremor = 120.0;
        /// <summary>
        /// Frecuencias de corte para filtro en el panel auxiliar.
        /// </summary>
        double Fcx1 = 2.0;
        /// <summary>
        /// Frecuencias de corte para filtro en el panel auxiliar.
        /// </summary>
        double Fcx2 = 8.0;
        /// <summary>
        /// Se utiliza como medida para el cáculo de filtros.
        /// </summary>
        short M = 256;
        /// <summary>
        /// Se utila para determinar que tipo de filtro usar dependiendo su valor, 1 = pasabajos, 2 = pasa altos, 3 =pasa banda.
        /// </summary>
        char cfilx = '0';
        /// <summary>
        /// 
        /// </summary>
        double factmm = -1.0;
        /// <summary>
        /// 
        /// </summary>
        double tigrabacion = 0;
        /// <summary>
        /// Es una variable usada en la rutina Vista.
        /// </summary>
        double durac = 60.0;
        /// <summary>
        /// Es una variable usada en la rutina Vista.
        /// </summary>
        double initic = 0;//durac e initic son variables usadas en la rutina Vista
        /// <summary>
        /// Polo del filtro calculado con el Octave en el panel principal.
        /// </summary>
        int polo;
        /// <summary>
        /// Polo del filtro calculado con el Octave en el panel coda.
        /// </summary>
        int polocod;
        /// <summary>
        /// 
        /// </summary>
        int idbox2ant = 1;
        /// <summary>
        /// Offset es la desviación del cero de la señal para el espectro, en otras palabras es el rago por encima y por debajo del valor tomado como cero de la señal.
        /// </summary>
        int offsetesp = 2;
        /// <summary>
        /// clasificación sola ahorro de tiempo en el metodo de clasificación.
        /// </summary>
        int clSola = -1;
        /// <summary>
        /// 
        /// </summary>     
        int incy = 0;
        /// <summary>
        /// cantidad de huecos en la señal
        /// </summary>
        int nuhueco = 0;
        /// <summary>
        /// Velocidad de la vista, para visualizar sismos grandes.
        /// </summary>
        int velo = 100000;
        /// <summary>
        /// Visuslización de las marcas de tiempo.
        /// </summary>
        bool marcati = true;
        /// <summary>
        /// Visuslización de todas las pepas.
        /// </summary>
        bool pepas = true;
        /// <summary>
        /// Visuslización pepas del volcan activado.
        /// </summary>
        bool pepasvol = false;
        /// <summary>
        /// True si el panel de clasificación ocupa toda la pantralla.
        /// </summary>
        bool zoomcla = false;
        /// <summary>
        /// Si es true pone * en las lecturas de las codas y la amplitud.
        /// </summary>
        bool refe = true;
        /// <summary>
        /// Controla si el boton archi esta activo.
        /// </summary>
        bool siArch = false;
        /// <summary>
        /// Indica si se quiere o no mostrar las lineas guia.
        /// </summary>
        bool guia = false;
        /// <summary>
        /// Indica que se a seccionado el punto P en el el panel de clasificación de sismos.
        /// </summary>
        bool Pcd = true;
        /// <summary>
        /// Indica que se a seccionado el punto S en el el panel de clasificación de sismos.
        /// </summary>
        bool Scd = false;
        /// <summary>
        /// Indica que se a seccionado el punto CODA en el el panel de clasificación de sismos.
        /// </summary>
        bool Ccd = false;
        /// <summary>
        /// Indica si se quiere o no mostrar los puntos de digitalización de una lectura.
        /// </summary>
        bool pto = false;
        /// <summary>
        /// Controla si se refresca el listbox de las clasificaciones.
        /// </summary>
        bool listabox = false;
        /// <summary>
        /// True cuando existen suds.
        /// </summary>
        bool estru30 = false;
        /// <summary>
        /// Si hay tarjetas YFILE.
        /// </summary>
        bool siyfi = false;
        /// <summary>
        /// Si hay tarjetas GCF.
        /// </summary>
        bool sigcf = false;
        /// <summary>
        /// Si hay tarjetas SEISAN.
        /// </summary>
        bool siseisan = false;
        /// <summary>
        /// 
        /// </summary>
        bool cargar = false;
        /// <summary>
        /// Indica si hay huecos en las lecturas.
        /// </summary>
        bool sihueco = false;
        /// <summary>
        /// Indica si hay o no lecturas.
        /// </summary>
        bool nolec = false;
        /// <summary>
        /// Tiene que ver con los circulos con los que se estima la localización, epicentro del sismo esta en vista.
        /// </summary>
        bool kilometro = true;
        /// <summary>
        /// 
        /// </summary>
        bool cambio = false;
        /// <summary>
        /// 
        /// </summary>
        bool leido = false;
        /// <summary>
        /// 
        /// </summary>
        bool disparo = false;
        /// <summary>
        /// Variable utilizada para la validación de guardado de detalles de Magnitud local en la base.
        /// </summary>
        public bool MagnitudLocal = false; //si True, guarda detalles de la ML en la Base
        /// <summary>
        /// 
        /// </summary>
        char leclec = ' ';
        /// <summary>
        /// Valor que representa la intencidad del color rojo de la pepa de un sismo ya clasificado.
        /// </summary>
        byte[] clR;
        /// <summary>
        /// Valor que representa la intencidad del color verde de la pepa de un sismo ya clasificado.
        /// </summary>
        byte[] clG;
        /// <summary>
        /// Valor que representa la intencidad del color azul de la pepa de un sismo ya clasificado.
        /// </summary>
        byte[] clB;
        /// <summary>
        /// Duración de los archivos MUX.
        /// </summary>     
        ushort[] durmux;
        /// <summary>
        /// Duración de los archivos DMUX.
        /// </summary>
        ushort[] durdmx;
        /// <summary>
        /// El valor que hay que sumar en los tiempos DMUX para convertirlo a la hora local.
        /// </summary>
        short[] utdmx;
        /// <summary>
        /// El valor que hay que sumar en los tiempos MUX para convertirlo a la hora local.
        /// </summary>
        short[] utmux;
        /// <summary>
        /// Si existe, se guarda tiempo adicional en segundos para la tarjeta en cuestion.
        /// </summary>
        double[] tiadimux; // si existe, se guarda tiempo adicional en segundos para la tarjeta en cuestion.

        int[] analog;

        string[] estanalog;
        /// <summary>
        /// Cantidad de estaciones analogas.
        /// </summary>
        short nuanalog = 0;
        /// <summary>
        /// 
        /// </summary>
        string[] rutmux;
        /// <summary>
        /// 
        /// </summary>
        string[] rutdmx;
        /// <summary>
        /// Extención de archivo de los formatos de las trazas en formato MUX.
        /// </summary>
        string[] extmux;
        /// <summary>
        /// Extención de archivo de los formatos de las trazas en formato DMUX.
        /// </summary>
        string[] extdmx;
        /// <summary>
        /// Contiene la ruta de donde encontrar el archivo YFILE.
        /// </summary>
        string archyfi = "";
        /// <summary>
        /// Contiene la ruta de donde encontrar el archivo GCF original.
        /// </summary>
        string archgcf = "";
        /// <summary>
        /// Contiene la ruta de donde encontrar el archivo SEISAN original.
        /// </summary>
        string archseis = "";
        /// <summary>
        /// Contiene la ruta de donde encontrar el archivo GCF original.
        /// </summary>
        string archgcfnorm = "";
        /// <summary>
        /// Contiene la ruta de donde encontrar el archivo GCF auxiliar.
        /// </summary>
        string archgcfaux = "";
        /// <summary>
        /// Tarjeta por defecto para disparos.
        /// </summary>
        char tardis = 'Z';// tarjeta por defecto para disparos
        /// <summary>
        /// Representa la cantidad de horas que se quieran añadir al tiempo del sismo cuando se utiliza la carga de sismos mediante el botón disparo.
        /// </summary>
        float UTdisp = 0;
        /// <summary>
        /// Nombres de las tarjetas YA sea J o T para el formato MUX.
        /// </summary>
        char[] tarmux;
        /// <summary>
        /// Nombres de las tarjetas YA sea J,T,Z para el formato DMUX.
        /// </summary>
        char[] tardmx;

        /// estas variables (sianmux....)permiten saber si se debe buscar las carpetas por año y/o mes y/o día.
        /// <summary>
        /// Vector que indica si hay un año definido para cada lectura del formato MUX.
        /// </summary>
        bool[] sianmux;
        /// <summary>
        /// Vector que indica si hay un año definido para cada lectura del formato DMUX.
        /// </summary>
        bool[] siandmx;
        /// <summary>
        /// Vector que indica si hay un mes definido para cada lectura del formato MUX.
        /// </summary>
        bool[] simesmux;
        /// <summary>
        /// Vector que indica si hay un mes definido para cada lectura del formato DMUX.
        /// </summary>
        bool[] simesdmx;
        /// <summary>
        /// Vector que indica si hay un día definido para cada lectura del formato MUX.
        /// </summary>
        bool[] sidiamux;
        /// <summary>
        /// Vector que indica si hay un día definido para cada lectura del formato DMUX.
        /// </summary>
        bool[] sidiadmx;
        /// <summary>
        /// Sirve para garantizar que cuando se haga la solicitud de carga en memoria de las trazas de determinada tarjeta
        /// (MUX) solo se lean las tarjetas que no estan en memoria en ese momento.
        /// </summary>
        bool[] yamux;
        /// <summary>
        /// Sirve para garantizar que cuando se haga la solicitud de carga en memoria de las trazas de determinada tarjeta
        /// (DMUX) solo se lean las tarjetas que no estan en memoria en ese momento.
        /// </summary>
        bool[] yadmx;
        /// <summary>
        /// Sirve para garantizar que cuando se haga la solicitud de carga en memoria de las trazas de determinada tarjeta
        /// (GCF) solo se lean las tarjetas que no estan en memoria en ese momento.
        /// </summary>
        bool[] yagcf;
        /// <summary>
        /// Almacena los valores que se encuentran en el archivo estacajon.
        /// </summary>
        string[] estarro;
        /// <summary>
        /// Es la fecha inicial que se escoje en el panel de selección de fechas.
        /// </summary>
        string fe1 = "";
        /// <summary>
        /// Es la fecha final que se escoje en el panel de selección de fechas.
        /// </summary>
        string fe2 = "";
        /// <summary>
        /// Sirve para el etiquetado de sismos con el fin de hacer una busqueda por esta etiqueta.
        /// </summary>
        string marca = "********";
        /// <summary>
        /// Nombre del sismo.
        /// </summary>
        public string sismo = "";
        //********** estos cuatro arreglos se utilizan para manejar la amplitud **********
        double[] valampl;
        bool[] siPampl;
        char[] letampl;
        char[] volampl;
        //********************************************************************************
        /// <summary>
        /// Tiempo inicial de duración del archivo clasificado.
        /// </summary>
        double[] tiar;
        /// <summary>
        /// Tiempos para la visualizacion de arribos.
        /// </summary>
        double[][] time;
        /// <summary>
        /// Duración del archivo clasificado. Archi
        /// </summary>
        ushort[] duar;
        /// <summary>
        /// Nombre archivo clasificado en la base.
        /// </summary>
        string[] nomar;
        /// <summary>
        /// Promedio para cada estación, el cero de la señal para cada traza.
        /// </summary>
        int[] promEst;
        /// <summary>
        /// Representa la fecha inicial seleccionada para mostrar trazas expresada en fromato C#.
        /// </summary>
        long ll1 = 0;
        /// <summary>
        /// Representa la fecha final seleccionada para mostrar trazas expresada en fromato C#.
        /// </summary>
        long ll2 = 0;
        /// <summary>
        /// Es la cantidad de trazas leidas.
        /// </summary>
        public ushort nutra = 0;
        /// <summary>
        /// posición de un volcan en el arreglo de volcanes.
        /// </summary>
        public short vol;
        /// <summary>
        /// Cantidad de clasificaciones disponibles.
        /// </summary>
        public short nucla;
        /// <summary>
        /// La cantidad de volcanes leidos.
        /// </summary>
        public short nuvol;
        /// <summary>
        /// Horas a agregar para convertir  tiempos a hora local.
        /// </summary>
        public short local = 0;
        /// <summary>
        /// Se usa para convertir el tiempo de las lecturas a tiempo UT.
        /// </summary>
        public int hor_rsn = 10000;
        /// <summary>
        /// Se usa para graficar cuando se hace analogico.
        /// </summary>
        public int CuentasAnalogico = 0;
        /// <summary>
        /// Se usa para el momento de guardar el sismo con su respectivo año.
        /// </summary>
        public long añoML;
        /// <summary>
        /// Variable para convertir coordenadas Internacional a WGS84.
        /// </summary>
        public double la84 = 0;
        /// <summary>
        /// Variable para convertir coordenadas Internacional a WGS84.
        /// </summary>
        public double lo84 = 0;
        /// <summary>
        /// Guarda la ruta de la base donde se guardan los archivos de sismos clasificados.
        /// </summary>
        public string rutbas = "";
        /// <summary>
        /// Es el nombre de usuario.
        /// </summary>
        public string usu = "";
        /// <summary>
        /// Se utiliza para especificar la clasificación del sismo cuando va a grabarse.
        /// </summary>
        public string clas = "__";
        /// <summary>
        /// Nombre del archivo de la traza ya clasificada que se va a guardar en la base.
        /// </summary>
        public string nomsud = "";
        /// <summary>
        /// Tipos de clasificaciones que se pueden asignar a un sismo.
        /// </summary>
        public string[] cl;
        /// <summary>
        /// Nombres de los volcanes.
        /// </summary>
        public string[] volcan;
        /// <summary>
        /// Guarda la latitud de cada volcán.
        /// </summary>
        public double[] latvol;
        /// <summary>
        /// Guarda la longitud de cada volcán.
        /// </summary>
        public double[] lonvol;
        /// <summary>
        /// 
        /// </summary>
        public string[] estaloc;
        /// <summary>
        /// 
        /// </summary>
        public string[] volestaloc;
        /// <summary>
        /// 
        /// </summary>
        public short[] nuestaloc;
        /// <summary>
        /// 
        /// </summary>
        public short totestaloc = 0;
        /// <summary>
        /// 
        /// </summary>
        public short totvolestaloc = 0;
        /// <summary>
        /// Tiempo de la traza justo cuando se comienza el arrastre para la selección de un periodo de tiempo.
        /// </summary>
        public double tie1;
        /// <summary>
        /// Tiempo de la traza justo cuando se termina el arrastre para la selección de un periodo de tiempo.
        /// </summary>
        public double tie2;
        /// <summary>
        /// Guarda los valores originales en bytes que ocupan los datos de las trazas con el fin de que cuando se guarden en la base
        /// puedan ser guardados en las variables apropiadas segun su tamaño.
        /// </summary>
        public short[] by = new short[Ma];
        /// <summary>
        /// Valor para amplificar digitalmente las cuentas de la traza, esto se usa cuando se quiere tener una traza mas amplia.
        /// Ganancia.
        /// </summary>
        public short[] ga = new short[Ma];
        /// <summary>
        /// Rata de muestreo.
        /// </summary>
        public double[] ra = new double[Ma]; //puede ser float
        /// <summary>
        /// Representa el tiempo de cada valor de cuenta. 
        /// </summary>
        public double[][] tim = new double[Ma][];
        /// <summary>
        /// Se almacena el valor de las cuentas con las que posteriormente se construye las señales de las trazas.
        /// </summary>
        public int[][] cu = new int[Ma][];
        /// <summary>
        /// Guarda los factores de conversión de las cuentas a nanometros/segundo.
        /// </summary>
        public double[] fcnan;
        /// <summary>
        /// factores para el método que calcula el desplazamiento reducido (esta descripción es minima).
        /// </summary>
        public double[] fcDR;
        /// <summary>
        /// Guarda el valor de latitud respectiva de cada estación.
        /// </summary>
        public double[] laD;
        /// <summary>
        /// Guarda el valor de longitud respectiva de cada estación.
        /// </summary>
        public double[] loD;
        /// <summary>
        /// 
        /// </summary>
        public string[] Unidad;
        /// <summary>
        /// Guarda la inicial de los volcanes asociados a cada estación o un * en caso de no tener asociada una estación.
        /// </summary>
        char[] VD;
        /// <summary>
        /// Contiene los datos de factores para convertir de cuentas a micrometros/seg (archivos fcms?.txt).
        /// </summary>
        ArrayList fclist = new ArrayList(); //contiene los datos de factores para convertir de cuentas a micrometros/seg (archivos fcms?.txt).
        /// <summary>
        /// Contiene los datos para convertir de cuentas a mm (archivos fcam?.txt).
        /// </summary>
        ArrayList fcmm = new ArrayList(); //contiene los datos para convertir de cuentas a mm (archivos fcam?.txt).
        /// <summary>
        /// Contiene la lista con los huecos encontrados en cada traza.
        /// </summary>
        ArrayList huecolist = new ArrayList();
        /// <summary>
        /// 
        /// </summary>
        int[] cf;
        /// <summary>
        /// 
        /// </summary>
        int[] cfx;
        /// <summary>
        /// Representa la traza de la estación seleccionada despues de la aplicación del filtro pasa altos.
        /// </summary>
        int[] cfD;
        /// <summary>
        /// Contiene los nombres de las estaciones.
        /// </summary>
        public string[] est = new string[Ma];
        /// <summary>
        /// 
        /// </summary>
        public char[] comp = new char[Ma];
        /// <summary>
        /// 
        /// </summary>
        public char[] tar = new char[Ma];
        /// <summary>
        /// Indica que estaciones están seleccionadas como activas.
        /// </summary>
        public Boolean[] siEst = new Boolean[Ma];
        /// <summary>
        /// Indica si hay o no rotos en la traza por cada estación.
        /// </summary>
        Boolean[] siRoto = new Boolean[Ma];
        /// <summary>
        /// 
        /// </summary>
        Boolean[] siTraslapo = new Boolean[Ma];
        /// <summary>
        /// 
        /// </summary>
        Boolean[] no30 = new Boolean[Ma];
        /// <summary>
        /// 
        /// </summary>
        string[] estanolec = new string[1];

        // archivos HTML
        public bool html = false;
        public string rutHtml = "";
        public string carpHtml = "";

        public bool ModAux = false; // variables Modelo Auxiliar
        public string ClAux = "??";

        short numbral;
        short[] valumbral;
        string[] estumbral;

        public Color colfondo;
        public Color colinea;
        public Color colotr1;
        public Color colP;
        public Color colS;
        public Color colC;
        /// <summary>
        /// Si es true indica que el Octave esta instalado, en caso de ser false quiere decir que no esta instalado o que no esta configurado correctamente.
        /// </summary>
        bool octa = false;
        /// <summary>
        /// The filt
        /// </summary>
        bool filt = false;
        /// <summary>
        /// The filtx
        /// </summary>
        bool filtx = false;
        bool calcfilt = false;
        bool tipofilt = true;
        bool octainterp = false;
        /// <summary>
        /// Indica que se cálculo la interpolación.
        /// </summary>
        bool interpol = false;
        bool filtcod = false;
        bool calcfiltcod = false;
        bool tipofiltcod = true;
        /// <summary>
        /// Controla la saturación en la visualización de la señal.
        /// </summary>
        bool satu = false;
        bool sihayclas = false;
        bool elimiclas = false;
        /// <summary>
        /// Es false si no existe ninguna lectura de trazas en memoria, true en caso contrario.
        /// </summary>
        bool estado = false;
        bool tremor = false;
        bool tremofin = false;
        bool seguir = false;
        bool analogico = false;
        bool promecod = false;
        /// <summary>
        /// Indica si la traza en el panelcoda se debe dibujar de forma analógica o no.
        /// </summary>
        bool analogcoda = false;
        /// <summary>
        /// Se utiliza para verificar si se despliega o no el panel vista donde se muestra un mapa.
        /// </summary>
        bool vista = false;
        bool pausa = false;
        bool stop = false;
        bool redibarribos = false;
        bool salto = false;
        bool hpvista = false;
        bool dimensionar = false;
        bool respuesta = false;
        bool DesRed = false;
        bool copiarMod = false;
        /// <summary>
        /// Se hace true cuando en el archivo inicio.txt se encuentra la palabra USUARIO.
        /// </summary>
        bool sidesactiva = false;
        /// <summary>
        /// Determina si el programa cargo satisfactoriamente.
        /// </summary>
        bool inicio = false;
        bool NoMostrar = false;
        bool CajonGcf = false;
        bool QuitarGcf = true;
        bool seisei = false;
        public bool desactivado = false;
        public bool MLVista = false;
        public bool MLsi = false;
        /// <summary>
        /// Determina si se dibuja o no invertida una traza.
        /// </summary>
        public Boolean[] invertido;

        bool vigilancia = false;
        string estvig = "";
        string rutaDisparo = "";

        float famap;
        float latitud;
        float longitud; // variables opcion Vista arribos.
        short[] idT1;
        short[] nuesvista;// numero de traza en la visualizacion de arribos.
        double tiniT; // tiempo inicial de trazas en la visualizacion de los arribos
        char Maparr = 'X'; // caracter con la primera letra del mapa y modelo para visualizacion de arribos

        int[][] va = new int[3][]; // variables para movimiento de particulas
        int[] mimpt = new int[3];
        /// <summary>
        /// Almacena los valores minimos de traza para el movimiento de particulas por cada componente en este orden
        /// componente N, E y Z.
        /// </summary>
        int[] mnmpt = new int[3];
        short velompt = 50;
        /// <summary>
        /// Valor identificador de la componente N.
        /// </summary>
        short N = -1;
        /// <summary>
        /// Valor identificador de la componente E.
        /// </summary>
        short E = -1;
        /// <summary>
        /// Valor identificador de la componente Z.
        /// </summary>
        short Z = -1;
        /// <summary>
        /// id de la traza para el movimiento de particulas.
        /// </summary>
        short idmpt = -1;
        short volmpt = -1;
        /// <summary>
        /// Duración del movimiento de particulas.
        /// </summary>
        float durmpt = 5.0F;
        /// <summary>
        /// 
        /// </summary>
        int muimpt = 0;
        /// <summary>
        /// 
        /// </summary>
        int muinimpt = 0;
        /// <summary>
        /// 
        /// </summary>
        int mufmpt = 0;
        /// <summary>
        /// Valor del cero de la señal en la componente N de una traza para el cálculo del movimiento de particulas.
        /// </summary>
        int ceroN = 0;
        /// <summary>
        /// Valor del cero de la señal en la componente E de una traza para el cálculo del movimiento de particulas.
        /// </summary>
        int ceroE = 0;
        /// <summary>
        /// Valor del cero de la señal en la componente Z de una traza para el cálculo del movimiento de particulas.
        /// </summary>
        int ceroZ = 0;
        //float ampmpt = 2.0F;
        double laE;
        double loE;
        double difmpt;
        double faympt;
        double timpt;
        bool pausmpt = false;
        /// <summary>
        /// Indica si se ha determinado el valor para el cero de la señal por componente para realizar el cálculo 
        /// del movimiento de particulas y el cálculo de la interpolación, en caso de que ya se haya determinado
        /// false en caso de que no.
        /// </summary>
        bool sicerompt = false;
        bool moverparti = false;
        bool puntompt = true;
        bool modX = false;
        bool loscajones = false;
        bool particula = false;
        bool yaInterp = false;

        double[] ArrbTeo; // teoricos ak135
        int tottab;
        bool siCajTeo = false;
        float deltAK135;
        float laNeic;
        float loNeic;
        float lafN;
        float lofN;
        float zzfN;
        float mgfN;
        string estdelt = "";
        string nomweb = "";
        bool siNeic = false;

        double[] vaesp;  // variables para el espectro
        double mxesp;
        double mnesp;
        bool silog = false;
        bool vacioesp = false;
        /// <summary>
        /// Factor para la rata de muestreo de la interpolación.
        /// </summary>
        byte facRaInterp = 5;  //variables de la Interpolacion;
        /// <summary>
        /// Corresponde a la muestra de la traza que indica el inicio de la interpolación.
        /// </summary>
        int ip1;
        /// <summary>
        /// Corresponde a la muestra de la traza que indica el final de la interpolación.
        /// </summary>
        int ip2;  // variables que guardan el numero de muestra del intervalo seleccionado
        int ipb1;
        int ipb2;
        int ixpb;
        int suma = 0;
        /// <summary>
        /// Indica el promedio de velocidad en el cálculo de desplazamiento reducido.
        /// </summary>
        int proVelDR = 0;
        float frInterp = 0.5F;
        double promInterp;
        double promDesplz;
        /// <summary>
        /// Guarda el valor de conversión a nanometros/segundo para la interpolación.
        /// </summary>
        double facNanInt = -1.0;
        /// <summary>
        /// Guarda los valores de interpolación (spl).
        /// </summary>
        int[] spl; // variable que guarda los valores de interpolacion(spl) y de integracion (dzp)
        /// <summary>
        /// Interpolación para movimiento de particulas.
        /// </summary>
        int[][] splintp = new int[3][];//interpolacion para movimiento de particulas
        /// <summary>
        /// Variable que guarda el tiempo de los datos interpolados.
        /// </summary>
        double[] timspl;/*,nnm*/  // variables que guardan el tiempo y traza integrada respectivamente.
        /// <summary>
        /// Guarda los valores de desplazamiento de cuentas.
        /// </summary>
        double[] dzp/*,nnm*/;  // variables que guardan el tiempo y traza integrada respectivamente.
        double[] timsplintp = new double[1];
        bool sei = true;
        bool asc = false;
        bool sud = false;
        /// <summary>
        /// Determina si se dibujan o no las guias en el panel de interpolación.
        /// </summary>
        bool guiainterp = false;
        /// <summary>
        /// Se utiliza para controlar si se calcula o no el espectro a una traza que se esta interpolando.
        /// </summary>
        bool especinterP = false;
        /// <summary>
        /// Determina si se realizó o no una interpolación.
        /// </summary>
        bool NoInterpol = false;
        /// <summary>
        /// Se utiliza para el cálculo de Desplazamiento Reducido DR sus posibles valores son:
        /// 0 indica que no se va a calcular el Desplazamiento Reducido.
        /// 1 indica que se va hacer el cálculo del Desplazamiento Reducido por el método 1 (revisar manual proceso20).
        /// 2 indica que se va hacer el cálculo del Desplazamiento Reducido por el método 2 (revisar manual proceso20).
        /// </summary>
        byte DR = 0;
        /// <summary>
        /// Registra la posición en x en pixeles del lugar donde se da click sobre el panelDR,
        /// este valor se tiene en cuenta para el cálculo del desplazamiento reducido en una traza.
        /// </summary>
        int xiDR;
        /// <summary>
        /// Registra la posición en y en pixeles del lugar donde se da click sobre el panelDR,
        /// este valor se tiene en cuenta para el cálculo del desplazamiento reducido en una traza. 
        /// </summary>
        int yiDR;
        /// <summary>
        /// Representa el cero de la señal de la traza especifica de la estación seleccionada, se utiliza en el cálculo de desplazamiento reducido.
        /// </summary>
        double promDR;
        /// <summary>
        /// Indica el momento desde el cual se empezó la selección de la porción de traza a la que se le calcularia el desplazamiento reducido.
        /// </summary>
        double tDR1;
        /// <summary>
        /// Indica el momento desde el cual terminó la selección de la porción de traza a la que se le calcularia el desplazamiento reducido.
        /// </summary>
        double tDR2;
        /// <summary>
        /// 
        /// </summary>
        double mxz;
        /// <summary>
        /// 
        /// </summary>
        double mnz;
        /// <summary>
        /// Indica el valor que se obtiene del cálculo del Desplazamiento Reducido (DR).
        /// </summary>
        double valDR = 0;
        /// <summary>
        /// Desplazamiento reducido en micrómetros.
        /// </summary>
        double microDR = 0;
        /// <summary>
        /// 
        /// </summary>
        double[] zDR;

        // variables para filtro en panel de Clasificación
        /// <summary>
        /// Almacena la traza que se está clasificando  después de  aplicarle un filtro (pasa altos, pasa bajos, pasa banda).
        /// </summary>
        int[][] cff;
        /// <summary>
        /// Almacena el valor MÁXIMO de cuenta de cada traza a la que se le ha aplicado un filtro.
        /// </summary>
        int[] mxF;// variable para el filtro
        /// <summary>
        /// Almacena el valor MÍNNIMO de cuenta de cada traza a la que se le ha aplicado un filtro.
        /// </summary>
        int[] mnF;// variable para el filtro
        //short MM = 256;
        /// <summary>
        /// Frecuencia de corte para los filtros del panel1 visualizados en el panel panelcladib.
        /// </summary>
        float Fc1 = 2.0F;
        float Fc2 = 8.0F;
        /// <summary>
        /// Es el indicador del tipo de filtro que se aplica en el panel de clasificación (panelcladib)
        /// sus valores cambian entre 0 y 3 de la siguiente forma:
        /// 0 en caso de que no se haya aplicado ningun filtro al panelcladib
        /// 1 en caso de que no se haya aplicado el filtro pasa bajos al panelcladib
        /// 2 en caso de que no se haya aplicado el filtro pasa altos al panelcladib
        /// 3 en caso de que no se haya aplicado el filtro pasa banda al panelcladib.
        /// </summary>
        char cfilt = '0';
        /// <summary>
        /// Indica si ya se solicito el cálculo de algún filtro, esta variable se utiliza con fines
        /// de controlar la apariencia de los botones con los que se solicitan los filtros.
        /// tomara el valor de true cuando se de click sobre alguno de los botones de filtros.
        /// false en caso de que se de click sobre un botón de filtro y el valor de sifilt sea true.
        ///
        /// </summary>
        bool sifilt = false;
        /// <summary>
        /// Indica si ya se solicito el cálculo de algún filtro, esta variable se utiliza con fines de 
        /// controlar la ejecución del método que calcula el filtro,
        /// se hace true justo despúes de calcular el filtro especificado,
        /// toma el valor false cuando se da click sobre alguno de los botones de filtros.
        /// 
        /// </summary>
        bool yafilt = false;

        Point[] Cladat = new Point[2];
        ToolTip tip = new ToolTip();
        Util util = new Util();
        Fourier four = new Fourier();
        protected PerformanceCounter ramCounter = new PerformanceCounter("Memory", "Available MBytes");
        /// <summary>
        /// Este formulario corresponde al menú principal, donde se puede escoger la fecha de interés y la estación deseada,
        /// para posteriormente clasificar los sismos y leer los parámetros sísmicos básicos. El Form2.cs corresponde al lector
        /// de arribos para la localización con el hypo71 y el form3.cs, al atenuador. Util.cs, contiene utilidades usadas en los 3 formularios.
        /// El tiempo en formato SUDS, corresponde al número de segundos, desde el 1 de enero de 1900 a las 0 horas.
        /// El tiempo del formato GCF (Guralp Compressed Format) corresponde al número de segundos desde el 1 de enero de 1970 a las 0 horas.
        /// El tiempo en visual c# corresponde al número de centenares de nanosegundos desde el año cero.
        /// En general el nombre dado a las variables es sugestivo. Se detallan algunas de las más importantes: 
        ///1.	Las variables numux y nudmx, corresponden al número de tarjetas con archivos en SUDS multiplexado y demultiplexado respectivamente (relacionadas en archivo inicio.txt). 
        ///2.	Totven, el total de la ventana de tiempo (60 segundos por defecto). contampl, el número de lecturas de amplitud. id: corresponde al número de traza o estación, visible actualmente. Por defecto es 1 ya que normalmente la traza 0 corresponde al código del tiempo (IRIG).
        ///3.	bxi, byi, etc. se usan como variables iniciales en algunos paneles cuando se hace la distinción entre la bajada del botón del ratón y su subida, como por ejemplo cuando se arrastra, donde ambas posiciones son distintas.
        ///4.	estru30 corresponde a la variable asociada con la estructura 30 del formato SUDS, la cual corresponde a la corrección del tiempo. Esta estructura es muy importante en los formatos SUDS multiplexados, ya que normalmente la tarjeta digitalizadora toma el tiempo del computador, el cual comúnmente está desfasado varios segundos con respecto al tiempo exacto.
        ///5.	clR, clG y clB, corresponden a los colores de las pepas en código RGB. Tarmux, tardmx, guardan la letra de las tarjetas digitalizadoras.
        ///6.	nutra: es el número de canales totales, contabilizando todas las tarjetas digitalizadoras.
        ///7.	nucla, nuvol, el número de clasificaciones y volcanes respectivamente. vol es el volcán actual.
        ///8.	El arreglo volcán, guarda las 4 letras de los volcanes (35 como máximo). El arreglo cl, guarda las 2 letras de las clasificaciones (25 clasificaciones como máximo).
        ///9.	Las variables que terminan en ...estaloc, están asociadas con los botones que asocian una estación a un cajón y dicho cajón a un volcán (archivo estaloc.txt).
        ///10.	tie1 y tie2, corresponden al tiempo inicial y final de las ventanas de los archivos clasificados.
        ///11.	Los arreglos: by (número de bytes de los datos); ra (rata de muestreo); ga (ganancia); tim (tiempo de cada traza); 
        ///12.	cu (datos en cuentas); est (nombre de las trazas); comp (componentes de las trazas); 
        ///13.	tar (letra que identifica a la tarjeta digitalizadora). fcnan: factor para conversión de cuentas a nanómetros/segundo.
        /// </summary>
        public Form1()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Rutina que inicializa las variables y revisa que los archivos de inicio esten correctos.
        /// Funciones que realiza:
        ///1.	Carga en memoria las variables de todo el programa.
        ///2.	Comprueba que en la configuración regional esté seleccionada la coma como separador decimal. 
        ///3.	Verifica que el Octave esté instalado.
        ///4.	Cuenta el número de tarjetas que hay por cada formato y los guarda en sus respectivas variables.
        ///5.	Verifica la existencia del archivo inicio.txt.
        ///6.	Verifica si los datos son multiplexados o demultiplexados en el archivo inicio.
        ///7.	Verifica la existencia de la base de datos. 
        ///8.	Identifica el número de volcanes y sus clasificaciones.
        ///9.	Crea los botones de los volcanes.
        ///10.	Crea los botones de las clasificaciones.
        ///11.	Verifica la existencia del archivo estacajon.txt.
        ///12.	Asocia estaciones por volcán a un botón determinado mediante el archivo estacajon.txt.
        ///13.	Verifica la existencia del archivo vigilancia.txt y manipula su contenido.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void Form1_Load(object sender, EventArgs e)
        { // Rutina que inicializa las variables y revisa que los archivos de inicio esten correctos.

            //MessageBox.Show(panelAmp.Location.ToString());
            short i, ii, j, jj, k, kk, iM, iD, sumb;
            int an, me, di, ho, mi, index;
            double dd;
            string lin = "", nom = "", li = "", ca = "", ca2 = "", ca3 = "";
            char[] delim = { ' ', '\t' };
            string[] pa = null;
            bool carpeta = true;

            try
            {
                Form1.ActiveForm.Text += "        Tarjetas Utilizadas: ";
            }
            catch
            {
            }
            // comprobacion que en la configuracion regional este el punto y no la coma.
            dd = 1000.001;
            li = dd.ToString();
            if (li[4] == ',')
            {
                NoMostrar = true;
                MessageBox.Show("OJO: La Configuracion Regional NO es la ADECUADA.\n\nVaya al PANEL DE CONTROL y Cambie como decimal, la COMA por PUNTO!!");
                Close();
                return;
            }

            numbral = 0;
            sumb = 0;
            Cladat[0].X = 0;
            Cladat[0].Y = 0;
            Cladat[1].X = 0;
            Cladat[1].Y = 0;

            // se comprueba si esta instalado el octave de acuerdo a las instrucciones del manual.
            // el octave es necesario para el filtraje de la señal y el calculo de la magnitud local.
            // se comprueba ademas que este funcionando el filtro correctamente:

            if (File.Exists("c:\\octave\\bin\\octave.exe") && Directory.Exists(".\\oct"))
            {
                octainterp = true;
                if (File.Exists("res.txt"))
                    File.Delete("res.txt");
                StreamWriter wr = File.AppendText("octa.txt");
                wr.WriteLine("[b,c]=butter_filtro(4,0.1);");
                wr.WriteLine("save res.txt b;");
                wr.Close();
                li = "/C c:\\octave\\bin\\octave.exe < octa.txt";
                util.Dos(li, true);
                if (File.Exists("res.txt"))
                {
                    octa = true;
                    panelfilcod.Visible = true;
                }
            }
            Fc = 1.0;
            Fccod = 1.0;
            polo = 4;
            polocod = 4;
            // por defecto el fondo es blanco y las lineas negras, pero puede ser cambiado en el archivo inicio.txt
            colinea = Color.Black;
            colfondo = Color.White;
            colotr1 = Color.Blue;
            colP = Color.Green;  // color arribo onda P
            colS = Color.DeepSkyBlue; // color arribo onda S
            colC = Color.Red; // color lectura de la Coda

            // En el archivo inicio.txt, esta la ruta de la base, asi como la ruta de las tarjetas digitalizadoras.
            if (!File.Exists(".\\pro\\inicio.txt"))
            {
                NoMostrar = true;
                MessageBox.Show("ERROR  NO existe .\\pro\\inicio.txt");
                Close();
                return;
            }

            //El archivo inicio.txt contiene en la primera línea, la ruta de la Base. En la segunda
            //línea, las horas que deben añadirse para convertir el tiempo de la Base a Hora Local.
            //Las demás líneas pueden encontrasen en cualquier orden. Además de las 2 primeras,
            //las líneas esenciales son aquellas que identifican los diferentes formatos sísmicos
            //(M, D, G, S Y) y los parámetros necesarios para leer correctamente los archivos.

            StreamReader ar = new StreamReader(".\\pro\\inicio.txt");

            numux = 0; nudmx = 0;
            while (lin != null)
            {
                try
                {
                    lin = ar.ReadLine();
                    if (lin == null || lin[0] == '*') break;
                    if (lin[0] == 'M' && lin[1] == ' ') numux += 1; // tarjeta SUDS multiplexado
                    if (lin[0] == 'D' && lin[1] == ' ') nudmx += 1; // tarjeta SUDS demultiplexado
                    if (lin[0] == 'G' && lin[1] == ' ') // tarjeta formato GCF
                    {
                        sigcf = true;
                        archgcfnorm = lin.Substring(2);
                    }
                    if (lin[0] == 'Y' && lin[1] == ' ')  //tarjeta formato Y-FILE
                    {
                        siyfi = true;
                        archyfi = lin.Substring(2);
                    }
                    if (lin[0] == 'S' && lin[1] == ' ')  //tarjeta formato SEISAN
                    {
                        pa = lin.Split(delim);
                        siseisan = true;
                        cajseis = true;
                        archseis = pa[1];
                    }
                    if (lin.Length >= 3 && string.Compare(lin.Substring(0, 3), "NEG") == 0) // si existe linea con la instruccion NEG, el fondo se vuelve negro y la linea blanca.
                    {
                        colfondo = Color.Black;
                        colinea = Color.White;
                        colotr1 = Color.Yellow;
                        colP = Color.LightGreen;
                        colS = Color.LightBlue;
                        colC = Color.Orange;
                    }
                    if (lin.Length >= 5 && string.Compare("HUECO", lin.Substring(0, 5)) == 0)
                    {
                        //verhueco = true;
                        boHueco.Visible = true;
                    }
                    if (lin.Length >= 13)
                        if (string.Compare("MAGNITUDLOCAL", lin.Substring(0, 13)) == 0)
                            MagnitudLocal = true;
                    if (lin.Length >= 12)
                        if (string.Compare("RUTA_DISPARO", lin.Substring(0, 12)) == 0)
                            rutaDisparo = lin.Substring(13);
                    // con esta linea se copian los arribos en la subcarpeta RSN, para uso de la RSNC.
                    if (lin.Length > 3 && string.Compare("RSN", lin.Substring(0, 3)) == 0)
                        hor_rsn = int.Parse(lin.Substring(4));
                    // esta linea permite tener la opcion de ver las trazas siempre con la misma referencia.
                    if (lin.Length >= 11 && string.Compare("ANALOGICO", lin.Substring(0, 9)) == 0)
                    {
                        pa = lin.Split(delim);
                        CuentasAnalogico = int.Parse(pa[1]);
                        if (CuentasAnalogico > 0)
                        {
                            boAnaloCoda.Visible = true;
                            analogcoda = true;
                            analogico = true;
                            boAnalogico.BackColor = Color.OrangeRed;
                            boSatu.BackColor = Color.DarkMagenta;
                            boNano.Visible = false;
                            satu = analogico;
                            if (pa.Length > 1)
                            {
                                ca = ".\\pro\\" + pa[2];
                                if (File.Exists(ca))
                                {
                                    nuanalog = 0;
                                    li = "";
                                    StreamReader rr = new StreamReader(ca);
                                    while (li != null)
                                    {
                                        try
                                        {
                                            li = rr.ReadLine();
                                            if (li == null) break;
                                            if (li.Length > 5) nuanalog += 1;
                                        }
                                        catch
                                        {
                                            break;
                                        }
                                    }
                                    rr.Close();
                                    if (nuanalog > 0)
                                    {
                                        estanalog = new string[nuanalog];
                                        analog = new int[nuanalog];
                                        li = "";
                                        i = 0;
                                        StreamReader pp = new StreamReader(ca);
                                        while (li != null)
                                        {
                                            try
                                            {
                                                li = pp.ReadLine();
                                                if (li == null) break;
                                                if (li.Length > 5)
                                                {
                                                    pa = li.Split(delim);
                                                    estanalog[i] = pa[0];
                                                    analog[i++] = int.Parse(pa[1]);
                                                }
                                            }
                                            catch
                                            {
                                                break;
                                            }
                                        }
                                        pp.Close();
                                    }
                                }
                            }
                        }
                    }
                    if (lin.Length >= 13 && string.Compare("UMBRAL", lin.Substring(0, 6)) == 0) sumb += 1;
                    if (lin.Length == 7 && string.Compare("USUARIO", lin.Substring(0, 7)) == 0) sidesactiva = true;
                    if (lin.Length >= 18 && string.Compare("MODELO_AUXILIAR", lin.Substring(0, 15)) == 0)
                    {
                        try
                        {
                            ClAux = lin.Substring(16, 2);
                            ModAux = true;
                        }
                        catch { }
                    }
                    if (lin.Length >= 5)
                    {
                        if (string.Compare("NEIC", lin.Substring(0, 4)) == 0)
                        {
                            try
                            {
                                pa = lin.Split(delim);
                                utNeic = short.Parse(pa[1]);
                                nomweb = pa[2];
                                if (!File.Exists(".\\coor\\mundo.txt"))
                                {
                                    NoMostrar = true;
                                    MessageBox.Show("Para la visualizacion del mapa con las localizaciones del NEIC\nHace falta el archivo MUNDO.TXT en la carpeta .\\coor  !!");
                                }
                                else siNeic = true;
                            }
                            catch
                            {
                                siNeic = false;
                            }
                            if (!File.Exists(".\\coor\\mundo.txt")) siNeic = false;
                        }
                        if (lin.Length > 8 && string.Compare("HTML", lin.Substring(0, 4)) == 0)
                        {
                            html = true;
                            pa = lin.Split(delim);
                            rutHtml = pa[1];
                            carpHtml = pa[2];
                        }
                        if (lin.Length > 6 && string.Compare(lin.Substring(0, 5), "WGS84") == 0)
                        {
                            try
                            {
                                pa = lin.Split(delim);
                                la84 = double.Parse(pa[1]);
                                lo84 = double.Parse(pa[2]);
                            }
                            catch
                            {
                                la84 = 0;
                                lo84 = 0;
                            }
                        }
                        if (lin.Length >= 10 && string.Compare("PROMEDIO", lin.Substring(0, 8)) == 0)
                        {
                            pa = lin.Split(delim);
                            PROMEDIO = short.Parse(pa[1]);
                        }
                        if (lin.Length >= 4 && string.Compare(lin.Substring(0, 4), "GAUX") == 0)
                        {
                            pa = lin.Split(delim);
                            archgcfaux = pa[1];
                        }
                    }
                }
                catch
                {
                    break;
                }
            }
            ar.Close();
            ///-------------------------------------------------------------------------------------------------------------------------------------------
            if (sigcf == false && archgcfaux.Length > 1)
                archgcfaux = "";

            if (sumb > 0)
            {
                estumbral = new string[sumb];
                valumbral = new short[sumb];
            }

            panel1.BackColor = colfondo;      // panel principal donde se muestra la traza de la estacion escogida
            panelcladib.BackColor = colfondo; // panel de clasificacion
            panelcoda.BackColor = colfondo;   // panel de la lectura de coda
            panelAmp.BackColor = colfondo;    // panel de la lectura de amplitud y periodo.            


            if (nudmx == 0 && numux == 0 && sigcf == false && siyfi == false && siseisan == false)
            {
                NoMostrar = true;
                MessageBox.Show("NO HAY Formatos sismicos en .\\pro\\inicio.txt");
                Close();
                return;
            }
            if (numux > 0)
            {
                rutmux = new string[numux]; durmux = new ushort[numux]; extmux = new string[numux];
                sianmux = new bool[numux]; simesmux = new bool[numux]; sidiamux = new bool[numux];
                tarmux = new char[numux]; utmux = new short[numux]; tiadimux = new double[numux];
                cajmux = new bool[numux]; yamux = new bool[numux]; /*yadmx = new bool[nudmx];*/
                for (i = 0; i < numux; i++)
                {
                    tiadimux[i] = 0;
                    cajmux[i] = true;
                }
            }
            if (nudmx > 0)
            {
                rutdmx = new string[nudmx]; durdmx = new ushort[nudmx]; extdmx = new string[nudmx];
                siandmx = new bool[nudmx]; simesdmx = new bool[nudmx]; sidiadmx = new bool[nudmx];
                cajdmx = new bool[nudmx]; yadmx = new bool[nudmx];
                for (i = 0; i < nudmx; i++) cajdmx[i] = true;
                tardmx = new char[nudmx]; utdmx = new short[nudmx];
            }
            if (sigcf == true)
            {
                archgcf = archgcfnorm;
                ValNugcf();
            }
            if (nugcf > 0)
            {
                yagcf = new bool[nugcf];
                for (i = 0; i < nugcf; i++) yagcf[i] = false;
            }

            iM = 0; iD = 0;
            lin = "";

            StreamReader ar2 = new StreamReader(".\\pro\\inicio.txt");
            rutbas = ar2.ReadLine();
            local = short.Parse(ar2.ReadLine());
            while (lin != null)
            {
                try
                {
                    lin = ar2.ReadLine();
                    if (lin == null) break;
                    if (lin[0] == 'M' && lin[1] == ' ') // SUD Multiplexado
                    {
                        pa = lin.Split(delim);
                        rutmux[iM] = pa[6];
                        utmux[iM] = short.Parse(pa[3]);
                        durmux[iM] = ushort.Parse(pa[4]);
                        extmux[iM] = pa[2];
                        tarmux[iM] = pa[5][0];
                        yamux[iM] = false;
                        if (pa[1][0] == 'A') sianmux[iM] = true; else sianmux[iM] = false; // si true, los datos estan en carpeta del año (4 cifras)
                        if (pa[1][1] == 'M') simesmux[iM] = true; else simesmux[iM] = false; // si true, los datos estan en carpeta de meses (2 cifras)
                        if (pa[1][2] == 'D') sidiamux[iM] = true; else sidiamux[iM] = false; // si true, los datos estan en carpetas de dias (2 cifras).
                        if (pa.Length > 7) tiadimux[iM] = double.Parse(pa[7]);
                        iM += 1;
                    }
                    else if (lin[0] == 'D' && lin[1] == ' ') // SUDS Demultiplexado
                    {
                        pa = lin.Split(delim);
                        rutdmx[iD] = pa[6];
                        utdmx[iD] = short.Parse(pa[3]);
                        durdmx[iD] = ushort.Parse(pa[4]);
                        extdmx[iD] = pa[2];
                        tardmx[iD] = pa[5][0];
                        yadmx[iD] = false;
                        if (pa[1][0] == 'A') siandmx[iD] = true; else sianmux[iD] = false; // si true, los datos estan en carpeta del año (4 cifras)
                        if (pa[1][1] == 'M') simesdmx[iD] = true; else simesmux[iD] = false; // si true, los datos estan en carpeta de meses (2 cifras)
                        if (pa[1][2] == 'D') sidiadmx[iD] = true; else sidiamux[iD] = false; // si true, los datos estan en carpetas de dias (2 cifras).
                        iD += 1;
                    }
                    else if (char.IsDigit(lin[0])) totven = ushort.Parse(lin);
                    else if (lin.Length >= 13 && string.Compare("UMBRAL", lin.Substring(0, 6)) == 0)
                    {
                        estumbral[numbral] = lin.Substring(7, 4);
                        valumbral[numbral++] = short.Parse(lin.Substring(12));
                    }
                }
                catch
                {
                    break;
                }
            }
            ar2.Close();

            try
            {
                if (iM > 0) for (i = 0; i < iM; i++) Form1.ActiveForm.Text += "  " + tarmux[i];
                if (iD > 0) for (i = 0; i < iD; i++) Form1.ActiveForm.Text += "  " + tardmx[i];
                if (sigcf == true) Form1.ActiveForm.Text += "   G";
                if (siyfi == true) Form1.ActiveForm.Text += "   Y";
            }
            catch
            {
            }

            try
            {
                if (!Directory.Exists(".\\pro")) Directory.CreateDirectory(".\\pro");
                ca = rutbas + "\\pro";
                DirectoryInfo dir = new DirectoryInfo(ca);
                FileInfo[] fcc = dir.GetFiles("fc???.txt");
                foreach (FileInfo f in fcc)
                {
                    ca2 = ca + "\\" + f.Name;
                    ca3 = ".\\pro\\" + f.Name;
                    File.Copy(ca2, ca3, true);
                }
            }
            catch
            {
                MessageBox.Show("NO EXISTE la Base: " + rutbas + " ??  Revise...");
                Close();
                return;
            }

            lin = "";
            nuvol = 0;
            nucla = 0;
            vol = 0;
            nom = rutbas + "\\pro\\clasificacion.txt";
            // En este archivo estan las clasificaciones y volcanes. Se lee primero el archivo para saber
            // cuantos volcanes (nuvol) y clasificaciones (nucla) existen.

            if (!File.Exists(nom))
            {
                NoMostrar = true;
                MessageBox.Show("NO EXISTE " + nom + "?  o Problemas de comunicacion??");
                Close();
                return;
            }
            StreamReader pr2 = new StreamReader(nom);
            while (lin != null)
            {
                try
                {
                    lin = pr2.ReadLine();
                    if (lin == null) break;
                    if (!char.IsLetterOrDigit(lin[0])) break;
                    else if (!char.IsLetterOrDigit(lin[2]) && nucla < 25) nucla += 1;
                    else if (char.IsLetterOrDigit(lin[3]) && nuvol < 35) nuvol += 1;
                }
                catch
                {
                    NoMostrar = true;
                    MessageBox.Show("Problemas en " + nom + " ??");
                    break;
                }
            }
            pr2.Close();
            if (nuvol == 0 || nucla == 0)
            {
                NoMostrar = true;
                MessageBox.Show("Problemas en " + nom + " ??");
                Close();
            }
            volcan = new string[nuvol + 1];
            latvol = new double[nuvol + 1];
            lonvol = new double[nuvol + 1];
            cl = new string[nucla];
            clR = new byte[nucla];// colores para las 'pepas'
            clG = new byte[nucla];
            clB = new byte[nucla];
            tie1 = 0;
            tie2 = 0;
            nuampvar = 0;
            lin = "";
            i = 0; j = 0;

            StreamReader prr = new StreamReader(nom);
            while (lin != null)
            {
                try
                {
                    lin = prr.ReadLine();
                    if (lin == null) break;

                    // se supone que tanto volcanes como clasificaciones, comienzan con una letra.
                    if (!char.IsLetterOrDigit(lin[0])) break;
                    else if (!char.IsLetterOrDigit(lin[2]) && j < 25) // colores de las pepas segun clasificacion.
                    {
                        pa = lin.Split(delim);
                        cl[j] = pa[0];
                        clR[j] = byte.Parse(pa[1]);
                        clG[j] = byte.Parse(pa[2]);
                        clB[j] = byte.Parse(pa[3]);
                        j += 1;
                    }
                    else if (char.IsLetterOrDigit(lin[3]) && i < 35) // nombres de los volcanes (4 letras) y las coordenadas del crater.
                    {
                        pa = lin.Split(delim);
                        volcan[i] = pa[0];
                        latvol[i] = double.Parse(pa[1]);
                        lonvol[i] = double.Parse(pa[2]);
                        i += 1;
                    }
                }
                catch
                {
                    //MessageBox.Show("lin222=" + lin);
                    break;
                }
            }
            prr.Close();

            if (i != nuvol || j != nucla)
            {
                NoMostrar = true;
                MessageBox.Show("Problemas en " + nom + " ???" + " i=" + i.ToString() + " j=" + j.ToString() + " nuvol=" + nuvol.ToString() + " nucla=" + nucla.ToString());
                Close();
                return;
            }
            carpeta = RevisarCarpetas();
            if (carpeta == false)
            {
                NoMostrar = true;
                ca = "Revise las clasificaciones -> clasificacion.txt de la Base\ny que existan en el LEC";
                ca += " y SUD de la Base!!\nRevise que las carpetas NO se hayan MOVIDO!!\n\nCONTINUAR ??";
                DialogResult result = MessageBox.Show(ca, "CONTINUAR ???", MessageBoxButtons.YesNo,
                       MessageBoxIcon.Question);
                if (result == DialogResult.No)
                {
                    Close();
                    return;
                }
            }
            volcan[i] = "XXXX"; // letras asociadas a los sismos tectonicos.
            latvol[i] = latvol[0]; // las coordenadas de los tectonicos, tienen por defecto las del primer volcan.
            lonvol[i] = lonvol[0];

            for (i = 0; i < nuvol; i++) // se crea dinamicamente los botones de los volcanes.
            {
                bvol[i] = new Button();
                bvol[i].Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
                if (i < 15) bvol[i].Location = new Point(i * 41, 0);
                else
                {
                    if (i < nuvol) bvol[i].Location = new Point((i - 15) * 41, 0);
                    else bvol[i].Location = new Point(15 * 41, 0);
                }
                bvol[i].Size = new Size(41, 20);
                bvol[i].Text = volcan[i].Substring(0, 4);
                bvol[i].TabIndex = i;
                if (i == vol) bvol[i].BackColor = Color.Yellow;
                else bvol[i].BackColor = Color.LightYellow;
                bvol[i].Font = new Font("Microsoft Sans Serif", 7);
                this.bvol[i].Click += new System.EventHandler(this.bvol_Click);
                if (i < 15 || i == nuvol) this.panelcla.Controls.Add(bvol[i]);
                else this.panel3.Controls.Add(bvol[i]);
            }
            if (nuvol > 14)
            {
                ii = (short)((nuvol - 15) * 41);
                panel3.Size = new Size(ii, 20);
                bMas = new Button();
                bMas.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
                bMas.Location = new Point(15 * 41, 0);
                bMas.Size = new Size(25, 20);
                bMas.BackColor = Color.Aquamarine;
                bMas.Text = "+";
                bMas.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);
                bMas.TabIndex = nuvol;
                this.bMas.Click += new System.EventHandler(this.bMas_Click);
                this.panelcla.Controls.Add(bMas);
            }
            for (i = 0; i < nucla; i++) // se crea dinamicamente los botones de las clasificaciones.
            {
                bcla[i] = new Button();
                bcla[i].Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
                bcla[i].Location = new Point(panelcla.Size.Width - 31, 25 + i * 20);
                bcla[i].Size = new Size(31, 21);
                bcla[i].Text = cl[i].Substring(0, 2);
                bcla[i].TabIndex = i;
                if (cl[i][2] == '+') bcla[i].BackColor = Color.LightPink;
                else bcla[i].BackColor = Color.LightCoral;
                bcla[i].Font = new Font("Microsoft Sans Serif", 7);
                this.bcla[i].Click += new System.EventHandler(this.bcla_Click);
                this.panelcla.Controls.Add(bcla[i]);
            }
            panelbotoncla.Size = new Size(24 * nucla, 20);

            if (!File.Exists(".\\pro\\estacajon.txt")) // archivo donde se asocia las estaciones a los cajones y estos a los volcanes.
            {
                NoMostrar = true;
                MessageBox.Show("El archivo ESTACAJON.TXT No Existe en el Directorio .\\pro !!\n" +
                    "\nEste archivo permite seleccionar el Volcan de acuerdo\na la estacion de trabajo y así minimizar errores !!");
            }
            else
            {
                lin = "";
                nutarro = 0;
                //estacajon.txt permite asociar determinadas estaciones a un volcan y a un
                //botón determinado del 1 al 15. Estos números deben ocuparse secuencialmente.

                StreamReader pr3 = new StreamReader(".\\pro\\estacajon.txt");
                while (lin != null)
                {
                    try
                    {
                        lin = pr3.ReadLine();
                        if (lin == null) break;
                        nutarro += 1;
                    }
                    catch
                    {
                        break;
                    }
                }
                pr3.Close();
                if (nutarro > 0)
                {
                    estarro = new string[nutarro];
                    lin = "";
                    jj = 0;
                    StreamReader pr4 = new StreamReader(".\\pro\\estacajon.txt");
                    while (lin != null)
                    {
                        try
                        {
                            lin = pr4.ReadLine();
                            estarro[jj] = lin;
                            jj += 1;
                            if (jj >= nutarro) break;
                        }
                        catch
                        {
                            break;
                        }
                    }
                    pr4.Close();
                }
            }
            if (File.Exists(".\\pro\\estacajon.txt"))
            {
                totestaloc = 0;
                totvolestaloc = 0;
                lin = "";
                StreamReader pr5 = new StreamReader(".\\pro\\estacajon.txt");

                while (lin != null)
                {
                    try
                    {
                        lin = pr5.ReadLine();
                        if (char.IsLetter(lin[0])) totestaloc += 1;
                        else if (char.IsDigit(lin[0]) && totvolestaloc <= 15) totvolestaloc += 1;
                    }
                    catch
                    {
                        break;
                    }
                }
                pr5.Close();

                estaloc = new string[totestaloc];
                nuestaloc = new short[totestaloc];
                volestaloc = new string[totvolestaloc];
                lin = "";
                j = 0;
                i = 0;
                StreamReader pr6 = new StreamReader(".\\pro\\estacajon.txt");
                while (lin != null)
                {
                    try
                    {
                        lin = pr6.ReadLine();
                        if (char.IsLetter(lin[0]) && j < totestaloc)
                        {
                            estaloc[j] = lin.Substring(0, 4);
                            nuestaloc[j] = short.Parse(lin.Substring(5));
                            j += 1;
                        }
                        else if (char.IsDigit(lin[0]))
                        {
                            volestaloc[i] = lin.Substring(2, 4);
                            i += 1;
                        }
                    }
                    catch
                    {
                        break;
                    }
                }
                pr6.Close();
                totalbotoncajon = 0;
                for (i = 0; i < totestaloc; i++)
                {
                    if (totalbotoncajon < nuestaloc[i]) totalbotoncajon = nuestaloc[i];
                    if (totalbotoncajon >= 16) break;
                }
                //Aqui se crea dinámicamente los botones de los cajones que asocian determinadas
                //estaciones a un determinado volcán.
                for (i = 0; i < totalbotoncajon; i++)
                {
                    best[i] = new Button();
                    best[i].Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
                    k = (short)(i * 27);
                    kk = 0;
                    best[i].Location = new Point(k, kk);
                    best[i].Size = new Size(29, 22);
                    best[i].Text = (i + 1).ToString();
                    best[i].TabIndex = i + 1;
                    best[i].BackColor = Color.Peru;
                    best[i].Font = new Font("Microsoft Sans Serif", 7);
                    this.best[i].MouseDown += new System.Windows.Forms.MouseEventHandler(this.best_MouseDown);
                    this.panelBotCla.Controls.Add(best[i]);
                }
            }
            else
            {
                NoMostrar = true;
                MessageBox.Show("No existe .\\pro\\estacajon.txt !!!\n\nEste archivo permite crear cajones para\nseleccionar estaciones y asociarlas a un Volcan");
            }

            //Existe un programa llamado Vigilancia.exe, el cual puede hacer llamado al Proceso.
            //Aqui se inicializa las variables de tal modo que se muestre por defecto la estacion
            //y el tiempo escogidos en dicho programa. Dicho programa crea en el directorio de
            //trabajo, el archivo vigilancia.txt

            if (File.Exists("vigilancia.txt"))
            {
                vigilancia = true;
                li = "";
                StreamReader le = new StreamReader("vigilancia.txt");
                try
                {
                    Facnano();//rutina que lee los factores de conversion de cuentas a nanómetros/segundo
                    Facmilimetro();
                    Form1.ActiveForm.Text += "          Ruta de la Base: " + rutbas;
                    DimensionarPanelTarjetas();

                    li = le.ReadLine();
                    pa = li.Split(delim);
                    an = int.Parse(pa[0]);
                    me = int.Parse(pa[1]);
                    di = int.Parse(pa[2]);
                    li = le.ReadLine();
                    pa = li.Split(delim);
                    ho = int.Parse(pa[0]);
                    mi = int.Parse(pa[1]);
                    DateTime fech1 = new DateTime(an, me, di);
                    ll1 = fech1.Ticks;
                    DateTime fech2 = new DateTime(an, me, di, 23, 59, 0);
                    ll2 = fech2.Ticks;
                    totven = 15;
                    index = ho * 60 + mi - 5;
                    LLenaBox(); // rutina que selecciona el tiempo y los sismos clasificados de la Base.               
                    estado = false;
                    try
                    {
                        estvig = le.ReadLine();
                        usu = le.ReadLine();
                    }
                    catch
                    {
                    }
                    listBox1.SelectedIndex = index;
                }
                catch
                {
                }
                le.Close();
            }
            else
            {
                fe1 = string.Format("{0:yyyy}{0:MM}{0:dd}", DateTime.Now);// la fecha por defecto es la actual.
                fe2 = string.Format("{0:yyyy}{0:MM}{0:dd}", DateTime.Now);
                NoMostrar = true;
                fecha();
            }
            if (usu == "") usu = "___"; // usu es la variable que guarda las 3 letras del usuario.

            i = (short)(Size.Height - 100);
            panelbotoncla.Location = new Point(459, i);
            i = (short)(Size.Width - Size.Width / 3.0);
            j = (short)(Size.Height - (panelcoda.Height + 150));
            panelcoda.Location = new Point(5, j);
            panelcoda.Size = new Size(i, panelcoda.Height);
            i = (short)(panelcoda.Size.Width - 460);
            panelbotinfcoda.Location = new Point(i, panelbotinfcoda.Location.Y);
            i = (short)(Size.Width - (panelAmp.Width + 50));
            i = (short)(panelcla.Location.X + (panelcla.Size.Width - (panelAmp.Size.Width + 50)));
            j = (short)(panelcla.Location.Y + 50);
            panelAmp.Size = new Size(800, panelAmp.Height);
            panelAmp.Location = new Point(150, j);
            panelcla.Location = new Point(0, 1);
            panelcla.Size = new Size(851, 568);
            if (panelcla.Size.Height > (Size.Height - 100))
            {
                i = (short)(Size.Height - 100);
                j = (short)(panelcla.Size.Width);
                panelcla.Size = new Size(j, i);
            }
            panelFFTzoom.Size = new Size((int)(panel1.Width / 1.3), 120);
            panelParti.Location = new Point(10, 85);
            panelParti.Size = new Size(800, 720);

            i = (short)((nuvol + 1) * 12);
            panelModVista.Size = new Size(24, i);

            panelInterP.Location = new Point(110, 30);
            i = (short)(Size.Width - 160);
            j = (short)(Size.Height / 2.8);
            panelInterP.Size = new Size(i, j);
            panelDesplazamiento.Size = new Size(i, (int)(j / 2.0));
            panelEspectros.Size = new Size(i, 300);
            j = (short)(panelInterP.Location.Y + panelInterP.Height + 5);
            panelDesplazamiento.Location = new Point(110, j);
            j += (short)(panelDesplazamiento.Height + 5);
            panelEspectros.Location = new Point(110, j);

            /* ----------------------Estas lineas podrian ser omitidas---------------------------*/
            Facnano();//rutina que lee los factores de conversion de cuentas a nanómetros/segundo
            Facmilimetro();
            /*------------------------------------------------------------------------------------*/
            try
            {
                Form1.ActiveForm.Text += "          Ruta de la Base: " + rutbas;
            }
            catch
            {
            }
            /* ----------------------Estas lineas podrian ser omitidas---------------------------*/
            DimensionarPanelTarjetas();
            /*------------------------------------------------------------------------------------*/
            inicio = true;

            labelMrc.Text = "";
            panelbotoncla.BackColor = Color.Red;

            return;
        }
        /// <summary>
        ///  Rutina que lee el número de trazas en formato GCF que hay en el archivo archigcf.txt y guarda ese número en la variable nugcf.
        /// </summary>
        void ValNugcf()
        {
            string lin, ca;

            lin = "";
            ca = ".\\pro\\" + archgcf;
            if (File.Exists(ca))
            {
                StreamReader pr = new StreamReader(ca);
                nugcf = 0;
                while (lin != null)
                {
                    try
                    {
                        lin = pr.ReadLine();
                        if (lin == null || lin[0] == '*') break;
                        nugcf += 1;
                    }
                    catch
                    {
                    }
                }
                pr.Close();
            }
            else MessageBox.Show("NO Existe " + ca);

            return;
        }
        /// <summary>
        /// Aqui se crean los cajones de estaciones, para que el usuario pueda escoger las
        /// estaciones a voluntad. Sobre todo útil en casos que se tenga problemas con algún
        /// computador o tarjeta y sea mejor deseleccionarla.
        /// </summary>
        void DimensionarPanelTarjetas()
        {
            int i = 0, ii, j = 0, jj = 0, k = 0, num, ngcf1 = 0, ngcf2 = 0;
            int numx = 0, igcf = 0;
            string nom = "", li = "";
            char[] delim = { ' ', '\t' };
            string[] pa = null;
            bool si = false;

            if (dimensionar == true) return;

            jj = 0;
            if (numux > 0)
            {
                cajmux = new bool[numux];
                i += 60;
                j = numux * 16;
                if (k < j) k = j;
                for (ii = 0; ii < numux; ii++)
                {

                    CheckBox cajon = new CheckBox();
                    cajon.Text = "Mux" + (ii + 1).ToString();
                    cajon.TabIndex = jj;
                    jj += 1;
                    cajon.Location = new Point(2, 5 + ii * 16);
                    cajon.Size = new Size(60, 16);
                    cajon.Checked = true;
                    cajon.CheckedChanged += new EventHandler(this.cajon_CheckedChanged);
                    this.panelTar.Controls.Add(cajon);
                    cajmux[ii] = true;
                }
            }
            if (nudmx > 0)
            {
                cajdmx = new bool[nudmx];
                i += 60;
                j = nudmx * 16;
                if (k < j) k = j;
                for (ii = 0; ii < nudmx; ii++)
                {
                    CheckBox cajon = new CheckBox();
                    cajon.Text = "Dmx" + (ii + 1).ToString();
                    cajon.TabIndex = jj;
                    jj += 1;
                    cajon.Location = new Point(2 + i - 60, 5 + ii * 16);
                    cajon.Size = new Size(60, 16);
                    cajon.Checked = true;
                    cajon.CheckedChanged += new EventHandler(this.cajon_CheckedChanged);
                    this.panelTar.Controls.Add(cajon);
                    cajdmx[ii] = true;
                }
            }
            if (sigcf == true)
            {
                i += 60;
                ngcf1 = 2 + i - 60;
                nom = ".\\pro\\" + archgcf;
                num = 0;
                if (!File.Exists(nom))
                {
                    NoMostrar = true;
                    MessageBox.Show("NO EXISTE " + nom + "\nPero en archivo inicio.txt aparece reportado!!");
                    return;
                }
                StreamReader ar00 = new StreamReader(nom);
                while (li != null)
                {
                    try
                    {
                        li = ar00.ReadLine();
                        if (li == null || li[0] == '*') break;
                        num += 1;
                    }
                    catch
                    {
                        break;
                    }
                }
                ar00.Close();

                numx = (int)(panel1.Height / 18.0);
                ngcf2 = 0;
                cajgcf = new bool[num];
                j = 0;
                ii = 0;
                igcf = 0;
                StreamReader ar0 = new StreamReader(nom);
                while (li != null)
                {
                    try
                    {
                        li = ar0.ReadLine();
                        if (li == null || li[0] == '*') break;
                        pa = li.Split(delim);
                        CheckBox cajon = new CheckBox();
                        cajon.Text = pa[3];
                        cajon.TabIndex = jj;
                        jj += 1;
                        cajon.Location = new Point(ngcf1, 5 + ii * 16);
                        cajon.Size = new Size(60, 16);
                        cajon.Checked = true;
                        cajon.CheckedChanged += new EventHandler(this.cajon_CheckedChanged);
                        cajon.MouseDown += new System.Windows.Forms.MouseEventHandler(this.cajon_MouseDown);
                        cajon.MouseMove += new System.Windows.Forms.MouseEventHandler(this.cajon_MouseMove);
                        this.panelTar.Controls.Add(cajon);
                        if (igcf < num) cajgcf[igcf++] = true;
                        j += 16;
                        ii += 1;
                        if (ii > numx)
                        {
                            ii = 0;
                            ngcf1 += 60;
                            i += 60;
                            k = numx * 17;
                            si = true;
                        }
                    }
                    catch
                    {
                        break;
                    }
                }
                ar0.Close();
                ngcf2 = 5 + num * 16;
                boGcfTar.Visible = true;
                if (si == false)
                {
                    if (k < j) k = j;
                    k += 18;
                }
            }
            if (siyfi == true)
            {
                i += 60;
                nom = ".\\pro\\" + archyfi;

                num = 0;
                StreamReader ar11 = new StreamReader(nom);
                while (li != null)
                {
                    try
                    {
                        li = ar11.ReadLine();
                        if (li == null || li[0] == '*') break;
                        num += 1;
                    }
                    catch
                    {
                        break;
                    }
                }
                ar11.Close();

                cajyfile = new bool[num];

                j = 0;
                ii = 0;
                StreamReader ar1 = new StreamReader(nom);
                while (li != null)
                {
                    try
                    {
                        li = ar1.ReadLine();
                        if (li == null || li[0] == '*') break;
                        pa = li.Split(delim);
                        CheckBox cajon = new CheckBox();
                        cajon.Text = pa[1];
                        cajon.TabIndex = jj;
                        jj += 1;
                        cajon.Location = new Point(2 + i - 60, 5 + ii * 16);
                        cajon.Size = new Size(60, 16);
                        cajon.Checked = true;
                        cajon.CheckedChanged += new EventHandler(this.cajon_CheckedChanged);
                        this.panelTar.Controls.Add(cajon);
                        if (ii < num) cajyfile[ii] = true;
                        j += 16;
                        ii += 1;
                    }
                    catch
                    {
                        break;
                    }
                }
                ar1.Close();
                if (k < j) k = j;
            }
            if (siseisan == true)
            {
                i += 60;
                ii = 0;
                CheckBox cajon = new CheckBox();
                cajon.Text = "Seis";
                cajon.Location = new Point(2, 5 + ii * 16);
                cajon.Size = new Size(60, 16);
                cajon.Checked = true;
                cajon.CheckedChanged += new EventHandler(this.cajon_CheckedChanged);
                this.panelTar.Controls.Add(cajon);
            }

            if (i < 120) i = 100;
            panelTar.Size = new Size(i, k + 40);
            if (archgcfaux != "")
            {
                panelBorTar.Visible = true;
                panelBorTar.BackColor = Color.GreenYellow;
                panelBorTar.Size = new Size(i, 50);
                panelBorTar.Location = new Point(0, panelTar.Height - 52);
                panelBorTar.BringToFront();
                boTarCarga.BringToFront();
                boGcfTar.BringToFront();
                labelPanelTar.BringToFront();
            }
            panelTar.Location = new Point(panel1.Width - (panelTar.Width + 2), panelTar.Location.Y);
            labelPanelTar.Location = new Point(5, panelTar.Size.Height - 15);
            dimensionar = true;

            panelDR.Size = new Size((int)(Width / 1.5), (int)(Height / 3.0));
            panelDR.Location = new Point(100, 100);

            return;
        }
        /// <summary>
        /// Marca o desmarca el checkBox que representa una tarjeta, y modifica el valor booleano del arreglo 
        /// que representa la tarjeta específica del checkBox.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void cajon_CheckedChanged(object sender, EventArgs e)
        {
            int i, j;
            bool si = false;

            CheckBox cc = (CheckBox)sender;
            j = 0;
            if (numux > 0)
            {
                j += numux;
                for (i = 0; i < numux; i++)
                {
                    if (cc.TabIndex == i)
                    {
                        cajmux[i] = cc.Checked;
                        return;
                    }
                }
            }
            if (nudmx > 0)
            {
                for (i = 0; i < nudmx; i++)
                {
                    if (cc.TabIndex == i + j)
                    {
                        cajdmx[i] = cc.Checked;
                        si = true;
                        break;
                    }
                }
                if (si == true)
                {
                    j += nudmx;
                    return;
                }
            }
            if (sigcf == true)
            {
                cajinigcf = (short)(j);
                for (i = 0; i < cajgcf.Length; i++)
                {
                    if (cc.TabIndex == i + j)
                    {
                        cajgcf[i] = cc.Checked;
                        si = true;
                        break;
                    }
                }
                if (si == true)
                {
                    j += cajgcf.Length;
                    return;
                }
            }
            if (siyfi == true)
            {
                for (i = 0; i < cajyfile.Length; i++)
                {
                    if (cc.TabIndex == i + j)
                    {
                        cajyfile[i] = cc.Checked;
                        si = true;
                        break;
                    }
                }
                if (si == true)
                {
                    j += cajyfile.Length;
                    return;
                }
            }
            if (siseisan == true)
            {
                if (cc.TabIndex == j)
                {
                    cajseis = cc.Checked;
                    return;
                }
            }
        }
        /// <summary>
        /// La idea con esta rutina y la siguiente, es que para las trazas en GCF cuando se active
        /// un cajón con el botón derecho del ratón y se suelte y enseguida se desplace el ratón por encima
        /// de los cajones, estos se vayan deseleccionando o seleccionando dependiendo si el botón
        /// inferior del panel en cuestión, se encuentre en rojo o verde, respectivamente.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void cajon_MouseDown(object sender, MouseEventArgs e)
        {//La idea con esta rutina y la siguiente, es que para las trazas en GCF cuando se active
            //un cajón con el botón derecho del ratón y se suelte y enseguida se desplace el ratón por encima
            //de los cajones, estos se vayan deseleccionando o seleccionando dependiendo si el botón
            //inferior del panel en cuestión, se encuentre en rojo o verde, respectivamente.

            NoMostrar = true;
            if (e.Button == MouseButtons.Right && CajonGcf == false)
                CajonGcf = true;
            else
                CajonGcf = false;
        }
        /// <summary>
        /// Cambia el valor del vector booleano cajgcf en una posición dependiente de la casilla donde
        /// se realizó el movimiento del mouse y el estado de verdad de la variable CajonGcf.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void cajon_MouseMove(object sender, MouseEventArgs e)
        {
            if (CajonGcf == false)
                return;
            NoMostrar = true;
            CheckBox cc = (CheckBox)sender;
            if (QuitarGcf == true)
                cc.Checked = false;
            else
                cc.Checked = true;
            try
            {
                cajgcf[cc.TabIndex - cajinigcf] = cc.Checked;
            }
            catch
            {
            }
            //MessageBox.Show("e.x=" + e.X.ToString());
            if (e.X <= 0 || e.X > 50)
            {
                CajonGcf = false;
            }
        }
        /// <summary>
        /// Este es botón inferior que permite deseleccionar o seleccionar estaciones
        /// en formato GCF.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void boGcfTar_MouseDown(object sender, MouseEventArgs e)
        {

            if (QuitarGcf == true)
            {
                QuitarGcf = false;
                boGcfTar.BackColor = Color.Green;
            }
            else
            {
                QuitarGcf = true;
                boGcfTar.BackColor = Color.Red;
            }
            CajonGcf = false;
        }
        /// <summary>
        ///  Esta rutina revisa que las carpetas de la base donde se guardan los datos, existan,
        ///  de tal modo que en caso contrario, el usuario pueda revisar si no se han movido. 
        /// </summary>
        /// <returns>true si todas las carpetas de la base existen, false en caso contrario.</returns>
        bool RevisarCarpetas()
        {
            /*
             * Esta rutina revisa que las carpetas de la base donde se guardan los datos, existan,
             * de tal modo que en caso contrario, el usuario pueda revisar si no se ha movido.
             */
            int i;
            string ca = "";

            ca = rutbas + "\\cla";
            if (!Directory.Exists(ca))
            {
                NoMostrar = true;
                MessageBox.Show("NO EXISTE " + ca);
                return (false);
            }
            ca = rutbas + "\\loc";
            if (!Directory.Exists(ca))
            {
                NoMostrar = true;
                MessageBox.Show("NO EXISTE " + ca);
                return (false);
            }
            ca = rutbas + "\\ate";
            if (!Directory.Exists(ca))
            {
                NoMostrar = true;
                MessageBox.Show("NO EXISTE " + ca);
                return (false);
            }
            ca = rutbas + "\\lec";
            if (!Directory.Exists(ca))
            {
                NoMostrar = true;
                MessageBox.Show("NO EXISTE " + ca);
                return (false);
            }
            ca = rutbas + "\\sud";
            if (!Directory.Exists(ca))
            {
                NoMostrar = true;
                MessageBox.Show("NO EXISTE " + ca);
                return (false);
            }

            for (i = 0; i < nucla; i++)
            {
                ca = rutbas + "\\lec\\" + cl[i].Substring(0, 2);
                if (!Directory.Exists(ca))
                {
                    NoMostrar = true;
                    MessageBox.Show("NO EXISTE " + ca);
                    return (false);
                }
            }
            for (i = 0; i < nucla; i++)
            {
                ca = rutbas + "\\sud\\" + cl[i].Substring(0, 2);
                if (!Directory.Exists(ca))
                {
                    NoMostrar = true;
                    MessageBox.Show("NO EXISTE " + ca);
                    return (false);
                }
            }

            return (true);
        }
        /// <summary>
        /// Este botón permite visualizar o esconder los botones de volcanes cuando su número sea
        /// mayor a 14.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void bMas_Click(object sender, EventArgs e)
        {//Este botón permite visualizar o esconder los botones de volcanes cuando su número sea
            //mayor a 14
            if (panel3.Visible == false)
                panel3.Visible = true;
            else
            {
                panel3.Visible = false;
                DibujoTrazas();
            }
        }
        /// <summary>
        /// Rutina que comprueba si se ha seleccionado un volcan.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void bvol_Click(object sender, EventArgs e)
        {// rutina que comprueba si se ha seleccionado un volcan.
            short i;
            Button bt = (Button)sender;
            for (i = 0; i < nuvol; i++)
            {
                if (bt.Text == volcan[i])
                {
                    vol = i;
                    bvol[i].BackColor = Color.Yellow;
                }
                else bvol[i].BackColor = Color.LightYellow;
            }
            if (vol >= 16)
                bMas.BackColor = Color.Yellow;
            else if (nuvol > 15)
                bMas.BackColor = Color.Aquamarine;
            return;
        }// bvol_Click
        /// <summary>
        /// Comprueba si se ha seleccionado una clasificacion y llama a la rutina que graba los datos.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void bcla_Click(object sender, EventArgs e)
        { // rutina que comprueba si se ha seleccionado una clasificacion y llama a la rutina que graba los datos.
            int i;

            Button bt = (Button)sender;
            clas = bt.Text + cl[bt.TabIndex][2];
            i = GrabaBase();
            if (i == 1)
            {
                boHypo71.Visible = true;
                boAten.Visible = true;
            }
            panelcla.Visible = false; // borra los paneles de clasificacion
            panelcoda.Visible = false;
            filtcod = false;
            calcfiltcod = false;
            bofilcod.BackColor = Color.White;
            bohzcod.BackColor = Color.White;
            bopolcod.BackColor = Color.White;
            radlowcod.BackColor = Color.White;
            radhicod.BackColor = Color.White;
            panelAmp.Visible = false;
            panel1.Invalidate(); // redibuja el panel principal.
            return;
        }
        /// <summary>
        /// Rutina que comprueba la activación de los botones de cajones asociados con las estaciones y volcanes.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void best_MouseDown(object sender, MouseEventArgs e)
        { // rutina que comprueba la activacion de los botones de cajones asociados con las estaciones y volcanes.
            short i, j, k;
            int n;

            if (totestaloc <= 0)
                return;
            Button bt = (Button)sender;

            for (j = 0; j < nutra; j++)
            {
                if (string.Compare(est[j].Substring(0, 4), "IRIG") == 0)
                    siEst[j] = true;// se asegura que si esta presente el codigo del tiempo, se tenga en cuenta.
                else
                    siEst[j] = false;
            }
            for (i = 0; i < totestaloc; i++)
            {
                for (j = 0; j < nutra; j++)
                {
                    if (string.Compare(estaloc[i].Substring(0, 4), est[j].Substring(0, 4)) == 0)
                    {
                        if (nuestaloc[i] == bt.TabIndex) siEst[j] = true;
                        break;
                    }
                }
            }
            for (k = 0; k < totvolestaloc; k++)
            {
                for (i = 0; i < nuvol; i++)
                {
                    if (string.Compare(volestaloc[bt.TabIndex - 1].Substring(0, 4), volcan[i].Substring(0, 4)) == 0)
                    {
                        vol = i;
                        break;
                    }
                }
            }
            for (j = 0; j < nuvol; j++)
            {
                if (j == vol)
                    bvol[j].BackColor = Color.Yellow;
                else
                    bvol[j].BackColor = Color.LightYellow;
            }
            loscajones = true;
            DibujoTrazas();// rutina que dibuja las trazas en el panel de clasificacion.
            i = (short)(util.EscribePanelEsta(panelEsta, nutra, est, siEst)); // rutina que escribe el nombre de las estaciones en el panel de Estaciones.
            if (panelcoda.Visible == true)
            {
                if (siEst[nucod] == false)
                {
                    panelcoda.Visible = false;
                    filtcod = false;
                    calcfiltcod = false;
                    bofilcod.BackColor = Color.White;
                    bohzcod.BackColor = Color.White;
                    bopolcod.BackColor = Color.White;
                    radlowcod.BackColor = Color.White;
                    radhicod.BackColor = Color.White;
                    panelAmp.Visible = false;
                }
                else
                    DibujoClascoda(); // rutina que pinta en color claro en el panel de clasificacion, la estacion y el sector seleccionado para la lectura de la coda.
            }

            k = -1;
            for (i = 0; i < nutra; i++)
            {
                if (siEst[i] == false)
                {
                    k = 1;
                    break;
                }
            }
            if (k == 1)
                boTodas.BackColor = Color.PaleVioletRed; // si no estan todas las estaciones presentes, se le avisa al usuario, pintando el boton TODAS, en color rozado.
            else
                boTodas.BackColor = Color.White;

            for (j = 0; j < totalbotoncajon; j++)
                best[j].BackColor = Color.Peru;
            n = bt.TabIndex - 1;
            best[n].BackColor = Color.Red;

            return;
        }
        /// <summary>
        ///  Rutina que se encarga de leer Todas las trazas de TODAS las tarjetas. Realmente
        ///  hace llamado a las rutinas respectivas, las cuales especificamente, leen el formato
        ///  sismico correspondiente.
        ///  Es importante recalcar la importancia de la estructura 30 en los sismos SUDS multiplexados,
        ///  ya que sino existe, los tiempos muy probablemente esten desfazados y se pueden tener
        ///  errores importantes (localizaciones por ejemplo) cuando se ensamblan con las trazas
        ///  de otros formatos o tarjetas.
        /// </summary>
        /// <param name="cond">Valor booleano.</param>
        /// <returns>Retorna 1 si el número de trazas es mayor a 0, en caso contrario retorna un 0.</returns>
        int lecturas(bool cond)
        {
            int i, j, jj, k, fe, fe1, fe2, num = 0;
            int largo;
            long lll;
            char[] delim = { ' ', '\t' };
            string[] pa = null;
            string esta = "", ca = "", estinv = "", li = "";

            NoMostrar = true;
            try
            {
                if (cond == true)
                {
                    if (nutra > 0)
                    {
                        for (i = 0; i < nutra; i++)
                        {
                            tim[i] = null;
                            cu[i] = null;
                        }
                    }
                    //En el listBox1 se guardan los minutos y clasificaciones. En el listBox2,
                    //se guardan los nombres de las Estaciones.
                    if (listBox2.Items.Count > 0)
                        listBox2.Items.Clear();
                    nutra = 0; // variable que lleva el conteo del total de trazas.
                    estru30 = false; // variable que controla si esta presente la estructura 30 en los SUDS Multiplexados.
                    for (j = 0; j < Ma; j++)
                    {
                        est[j] = "     ";
                        siRoto[j] = false;
                        no30[j] = false;
                        siTraslapo[j] = false;
                    }
                }// fin if del cond
                num = 0;
                if (numux > 0)
                {
                    for (i = 0; i < numux; i++)
                    {
                        if (cajmux[i] == false) continue;
                        try
                        {
                            k = LeeMux(i); // SUDS Multiplexados
                            if (k == 1)
                                num += k;//num++;
                            else if (k == -1)
                            {
                                panel2.Visible = false;
                                return (0); // provisional
                            }
                        }
                        catch
                        {
                            NoMostrar = true;
                            MessageBox.Show("Hubo un problema!!! Talvez hay 'huecos'. \n Mejor Revise o tome otro archivo inicial!!");
                            if (panel2.Visible == true)
                                panel2.Visible = false;
                        }
                    }
                }
                if (num > 0)
                {
                    largo = tim[0].Length;
                    timax = tim[0][largo - 1];
                    for (i = 1; i < nutra; i++)
                    {
                        largo = tim[i].Length;
                        if (timax < tim[i][largo - 1]) timax = tim[i][largo - 1];
                    }
                }
                else timax = 0;
                if (nudmx > 0) for (i = 0; i < nudmx; i++)
                    {
                        if (cajdmx[i] == false) continue;
                        try
                        {
                            LeeDmx(i); // SUDS Demultiplexados
                        }
                        catch
                        {
                            NoMostrar = true;
                            MessageBox.Show("Hubo un problema (DMX)!!! 'huecos?'. \n Mejor Revise o tome otro archivo inicial!!");
                        }
                    }
                if (sigcf == true)
                {

                    try
                    {
                        num = LeeGcf(); // trazas en formato GCF
                        if (num == -1)
                        {
                            panel2.Visible = false;
                            //return (0);// provisional
                        }
                    }
                    catch
                    {
                        NoMostrar = true;
                        MessageBox.Show("Hubo un problema (GCF)!!! 'huecos?'. \n Mejor Revise o tome otro archivo inicial!!");
                    }
                }
                if (siseisan == true)
                {
                    try
                    {
                        LeeSeisan(); // trazas en formato SEISAN
                    }
                    catch
                    {
                        NoMostrar = true;
                        MessageBox.Show("Hubo un problema (SEISAN)!!! 'huecos?'. \n Mejor Revise o tome otro archivo inicial!!");
                    }
                }
                if (siyfi == true)
                    LeeYfile(); // trazas en formato Y-FILE.

                if (nutra == 0)
                {
                    return (0);
                }
                timin = tim[0][0]; // En la variable timin se guarda el menor tiempo de TODAS las trazas.
                largo = tim[0].Length - 1;
                timaxmin = tim[0][largo];
                for (i = 1; i < nutra; i++)
                {
                    largo = tim[i].Length - 1;
                    if (timin > tim[i][0]) timin = tim[i][0];
                    if (timaxmin > tim[i][largo]) timaxmin = tim[i][largo];
                }

                for (i = 0; i < nutra; i++)
                {
                    if (est[i] == null)
                    {
                        NoMostrar = true;
                        MessageBox.Show("nula...  ii=" + i.ToString() + " est ii-1=" + est[i - 1] + " nutra=" + nutra.ToString());
                    }
                    else
                    {
                        listBox2.Items.AddRange(new object[] { est[i] }); // el listbox2 corresponde a las estaciones.
                        siEst[i] = true;
                    }
                }
            }
            catch
            {
                NoMostrar = true;
                MessageBox.Show("Hubo problemas!!!. \n Mejor Revise o tome otro archivo inicial!!");
            }

            lll = (long)(Fei + timin * 10000000.0); // se convierte el tiempo en SUDS a tiempo en visual c#
            DateTime fech = new DateTime(lll);
            fe = int.Parse(string.Format("{0:yyyy}{0:MM}{0:dd}", fech));
            fcnan = new double[nutra];
            Unidad = new string[nutra];
            fcDR = new double[nutra];
            laD = new double[nutra];
            loD = new double[nutra];
            VD = new char[nutra];

            //Aqui se va a leer los factores de conversión de cuentas a nanómetros/segundo, así
            //como los factores para el cálculo del Desplazamiento Reducido.
            for (j = 0; j < nutra; j++)
            {
                fcnan[j] = -1.0;
                Unidad[j] = "?";
                fcDR[j] = -1.0;
                laD[j] = 100.0;
                loD[j] = 1000.0;
                VD[j] = '*';
                // fclist contiene la lista de factores para convertir cuentas a nanómetros/segundo.
                for (jj = 0; jj < fclist.Count; jj++)
                {
                    ca = tar[j].ToString();
                    try
                    {
                        if (ca.Substring(0, 1) == fclist[jj].ToString().Substring(0, 1))
                        {
                            pa = fclist[jj].ToString().Split(delim);
                            esta = pa[1].Substring(0, 4);
                            fe1 = int.Parse(pa[3]);
                            fe2 = int.Parse(pa[4]);
                            if (string.Compare(esta.Substring(0, 4), est[j].Substring(0, 4)) == 0)
                            {
                                if (fe >= fe1 && (fe <= fe2 || fe2 == 0))
                                {
                                    try
                                    {
                                        fcnan[j] = double.Parse(pa[2]);
                                        if (pa.Length >= 10) Unidad[j] = pa[9];
                                        if (pa.Length >= 9) fcDR[j] = double.Parse(pa[8]);
                                        laD[j] = double.Parse(pa[5]);
                                        loD[j] = double.Parse(pa[6]);
                                        VD[j] = pa[7][0];
                                    }
                                    catch
                                    {
                                        //MessageBox.Show("ERROR " + fclist[jj].ToString());
                                    }
                                }
                            }
                        }
                    }
                    catch
                    {
                        fcnan[j] = -1.0;
                        fcDR[j] = -1.0;
                        //MessageBox.Show(" 14 ERROR");
                    }
                }
            }

            if (panel2.Visible == true)
                panel2.Visible = false;
            cargar = true;
            promEst = new int[nutra];// Variable que guarda el promedio para cada traza.
            invertido = new bool[nutra];// Variable que permite saber si la traza debe dibujarse al inverso en la vertical.
            for (i = 0; i < nutra; i++)
                invertido[i] = false;
            if (File.Exists(".\\pro\\invertido.txt"))
            {
                StreamReader ar = new StreamReader(".\\pro\\invertido.txt");
                while (li != null)
                {
                    try
                    {
                        li = ar.ReadLine();
                        if (li == null || li[0] == '*') break;
                        pa = li.Split(delim);
                        estinv = pa[0];
                        fe1 = int.Parse(pa[1]);
                        fe2 = int.Parse(pa[2]);
                        for (j = 0; j < nutra; j++)
                        {
                            if (string.Compare(estinv.Substring(0, 4), est[j].Substring(0, 4)) == 0)
                            {
                                if (fe >= fe1 && (fe <= fe2 || fe2 == 0))
                                {
                                    invertido[j] = true;
                                    //MessageBox.Show(est[j].Substring(0, 4) + " invertido=" + invertido[j].ToString());
                                }
                                break;
                            }
                        }// for j.....
                    }
                    catch
                    {
                    }
                }
                ar.Close();
            }
            labelPanelTar.Text = nutra.ToString() + " Trazas";
            PromediosIniciales();
            panel1.Invalidate();

            return (1);
        }
        /// <summary>
        /// Rutina que lee los factores para convertir cuentas a micrometros/segundo.
        /// Se leen los factores para todas las tarjetas y se colocan en una lista para
        /// usarse posteriormente en la rutina Lecturas().
        /// </summary>
        void Facnano()
        {
            // rutina que lee los factores para convertir cuentas a micrometros/segundo.
            // se leen los factores para todas las tarjetas y se colocan en una lista para
            // usarse posteriormente en la rutina Lecturas().
            char[] delim = { ' ', '\t' };
            string[] pa = null;
            string li = "", ca = "", ca2 = "";

            DirectoryInfo dir = new DirectoryInfo(".\\pro");
            FileInfo[] fcc = dir.GetFiles("fcms?.txt");
            foreach (FileInfo f in fcc)
            {
                li = "";
                ca = ".\\pro\\" + f.Name;
                StreamReader ar = new StreamReader(ca);
                while (li != null)
                {
                    try
                    {
                        li = ar.ReadLine();
                        if (li == null || li[0] == '*') break;
                        if (char.IsLetterOrDigit(li[0]))
                        {
                            pa = li.Split(delim);
                            if (pa.Length < 4)
                            {
                                NoMostrar = true;
                                MessageBox.Show("Faltan datos en " + f.Name + "??");
                                return;
                            }
                            ca2 = f.Name.Substring(4, 1) + " " + pa[0].Substring(0, 4) + " " + pa[1] + " " + pa[2] + " " + pa[3];
                            try
                            {
                                ca2 += " " + pa[4] + " " + pa[5] + " " + pa[6];
                                if (pa.Length >= 9)
                                {
                                    if (char.IsDigit(pa[9][0])) ca2 += " " + pa[9];
                                }
                                else ca2 += " 0";
                                if (pa.Length >= 10)
                                {
                                    if (char.IsLetter(pa[10][0])) ca2 += " " + pa[10];
                                }
                                else ca2 += " ?";
                            }
                            catch
                            {
                            }
                            //MessageBox.Show(ca2+"\npa.len="+pa.Length.ToString()+" pa10="+pa[10]+"\n"+li);
                            fclist.Add(ca2);
                        }
                    }
                    catch
                    {
                    }
                }
                ar.Close();
            }

            return;
        }
        /// <summary>
        /// Rutina que lee los factores para convertir cuentas a milimetros.
        /// Se leen los factores para todas las tarjetas. La idea original era convertir
        /// las amplitudes en cuentas a milímetros de amplitud de los sismogramas analógicos.
        /// Dichos sismogramas ya NO se usan para lecturas ni cálculos, por lo que estos factores ya NO tienen una utilidad.
        /// </summary>
        void Facmilimetro()
        {
            char[] delim = { ' ', '\t' };
            string[] pa = null;
            string li = "", ca = "", ca2 = "";
            DirectoryInfo dir = new DirectoryInfo(".\\pro");
            FileInfo[] fcc = dir.GetFiles("fcam?.txt");
            foreach (FileInfo f in fcc)
            {
                li = "";
                ca = ".\\pro\\" + f.Name;
                StreamReader ar = new StreamReader(ca);
                while (li != null)
                {
                    try
                    {
                        li = ar.ReadLine();
                        if (li == null || li[0] == '*') break;
                        if (char.IsLetterOrDigit(li[0]))
                        {
                            pa = li.Split(delim);
                            ca2 = f.Name.Substring(4, 1) + " " + pa[0].Substring(0, 4) + " " + pa[1] + " " + pa[2] + " " + pa[3];
                            fcmm.Add(ca2);
                        }
                    }
                    catch
                    {
                    }
                }
                ar.Close();
            }
            return;
        }
        /// <summary>
        /// ListBox1, contiene las fechas por minuto, que el usuario puede seleccionar para 
        /// visualizar la traza, hace false los vectores yamux[i], yadmx[i] y yagcf[i].
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int i;
            for (i = 0; i < numux; i++)
                yamux[i] = false;
            for (i = 0; i < nudmx; i++)
                yadmx[i] = false;
            for (i = 0; i < nugcf; i++)
                yagcf[i] = false;
            SeleccionarMinuto(true);
        }
        /// <summary>
        /// Configura el tamaño del listBox2 donde se muestran las estaciones,
        /// el ítem en el que va a tener seleccionado por defecto y hace visibles
        /// algunos botones necesarios para la clasificación de sismos.
        /// </summary>
        /// <param name="cond"></param>
        void SeleccionarMinuto(bool cond)
        {
            int i, j, k;

            NoMostrar = true;
            yaInterp = false;
            suma = 0;
            if (panelParti.Visible == true)
                panelParti.Visible = false;
            panelFFTzoom.Visible = false;
            panelInterP.Visible = false;
            panelDesplazamiento.Visible = false;
            panelEspectros.Visible = false;
            panelBarEspInterp.Visible = false;
            boEspInterP.Visible = false;

            if (nuhueco > 0)
            {
                if (huecolist.Count > 0)
                    huecolist.Clear();
                nuhueco = 0;
            }
            if (promecod == true)
                promecod = false;
            //util.borra(panel1,colfondo); // rutina que borra la pantalla.
            if (listabox == true)
            {
                listabox = false;
                return;
            }
            try
            {  // se reinicializan las variables
                cargar = false;
                listBox2.Items.Clear(); // caja que contiene la lista de estaciones.
                listBox2.Visible = false;
                sipro = 0;
                boprom.BackColor = Color.White;
                boaum.Visible = false;
                bosube.Visible = false;
                bobaja.Visible = false;
                calcfilt = false;
                filt = false;
                boFiltro.BackColor = Color.White;
                panelFiltro.BackColor = Color.White;
                panelFiltro.Visible = false;
                boInterp.Visible = false;
                bodis.Visible = false;
                boUno.Visible = false;
                boini.Visible = false;
                botim.Visible = false;
                bopep.Visible = false;
                boPepVol.Visible = false;
                boder.Visible = false;
                boizq.Visible = false;
                boprom.Visible = false;
                boAnalogico.Visible = false;
                boScilab.Visible = false;
                boInv.Visible = false;
                boEspe.Visible = false;
                boUnaCla.Visible = false;
                boDR.Visible = false;
                boTremor.Visible = false;
                boSatu.Visible = false;
                boVista.Visible = false;
                boNeic.Visible = false;
                i = 0;
                if (disparo == false)
                {
                    i = lecturas(cond); // lectura de trazas Aca agrega items a el listBox2
                }
                else
                    i = LecturaDisparo();
                if (i == 0)
                {
                    estado = false;
                    return;
                }
                // id, corresponde al numero de la traza que se visualiza.
                if (listBox2.Items.Count > 0 && id >= listBox2.Items.Count)
                    id = (ushort)(listBox2.Items.Count - 1);
                if (listBox2.Items.Count > 0 && ida >= listBox2.Items.Count)
                    ida = (ushort)(listBox2.Items.Count - 1);
                if (listBox2.Items.Count > 0)
                    listBox2.SelectedIndex = id;

                //Existe un programa llamado Vigilancia.exe, el cual puede hacer llamado al Proceso.
                //Dicho programa crea en el directorio de trabajo, el archivo vigilancia.txt
                if (vigilancia == true)
                {
                    vigilancia = false;
                    for (i = 0; i < listBox2.Items.Count; i++)
                    {
                        if (listBox2.Items[i].ToString().Substring(0, 4) == estvig.Substring(0, 4))
                        {
                            listBox2.SelectedIndex = i;
                            break;
                        }
                    }
                }
                k = nutra * 14;
                j = Size.Height - 50 - listBox2.Location.Y;
                if (k > j)
                    k = j;
                if (k < 60)
                    k = 60;
                listBox2.Size = new Size(65, k - 10);
                listBox2.Visible = true;
                if (octa == true)
                    panelFiltro.Visible = true;
                boInterp.Visible = true;
                bosube.Visible = true;
                bobaja.Visible = true;
                boaum.Visible = true;
                bodis.Visible = true;
                boUno.Visible = true;
                boini.Visible = true;
                botim.Visible = true;
                bopep.Visible = true;
                boPepVol.Visible = true;
                boder.Visible = true;
                boizq.Visible = true;
                boprom.Visible = true;
                if (CuentasAnalogico > 1)
                    boAnalogico.Visible = true;
                boInv.Visible = true;
                boEspe.Visible = true;
                boUnaCla.Visible = true;
                boDR.Visible = true;
                boTremor.Visible = true;
                boSatu.Visible = true;
                if (siNeic == true)
                    boNeic.Visible = true;
                if (File.Exists(".\\pro\\estavista.txt"))
                    boVista.Visible = true;
                sihayclas = Reviarch();
                boEliClas.Visible = false;
                estado = true;
                boRa1.Text = string.Format("{0:0.0}", ra[id] * 5.0);
                boRa2.Text = string.Format("{0:0.0}", ra[id] * 10.0);
                boRa3.Text = string.Format("{0:0.0}", ra[id] * 15.0);
            }
            catch
            {
                NoMostrar = true;
                MessageBox.Show("Problema Grabe!! Tome otro archivo inicial o un intervalo de tiempo mas corto!");
            }

            return;
        }
        /// <summary>
        /// Rutina que sirve para escoger el volcan automaticamente cuando se escoge una
        /// estacion en el panel principal. Para que esto funcione, la estacion escogida
        /// debe estar en el archivo ".\pro\estarro.txt" y estar asociada a un volcan especifico
        /// (la primera letra de su nombre, la cual NO se repite).
        /// </summary>
        void volcanTarro()
        {
            short i, voll = -1;
            string li = "", letr = "";

            if (!File.Exists(".\\pro\\estarro.txt")) return;
            StreamReader ar = new StreamReader(".\\pro\\estarro.txt");
            letr = "*";
            while (li != null)
            {
                try
                {
                    li = ar.ReadLine();
                    if (li == null) break;
                    if (li.Substring(0, 4) == est[id].Substring(0, 4))
                    {
                        letr = li.Substring(5);
                        break;
                    }
                }
                catch
                {
                }
            }
            ar.Close();

            if (letr[0] != '*')
            {
                for (i = 0; i < nuvol; i++)
                {
                    if (letr[0] == volcan[i][0])
                    {
                        voll = i;
                        break;
                    }
                }
            }
            if (voll > -1)
            {
                vol = voll;
                for (i = 0; i < nuvol; i++)
                {
                    if (i == vol) bvol[i].BackColor = Color.Yellow;
                    else bvol[i].BackColor = Color.LightYellow;
                }
            }
            if (vol >= 16) bMas.BackColor = Color.Yellow;
            else if (nuvol > 15) bMas.BackColor = Color.Aquamarine;

            return;
        }
        /// <summary>
        /// ListBox2, contiene las estaciones, este método se lanza cuando ocurre un cambio de selección en uno de los ítems del listBox2
        /// y esta encargado de configurar las variables necesarias para la clasificación de una traza al cambiar de estación.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            int i;
            yaInterp = false;
            suma = 0;
            valDR = 0;

            if (panelParti.Visible == true)
                panelParti.Visible = false;
            if (siNeic == true)
                boNeic.Visible = true;
            if (panelFFTzoom.Visible == true)
                panelFFTzoom.Visible = false;
            for (i = 0; i < totalbotoncajon; i++)
                best[i].BackColor = Color.Peru;
            boTodas.BackColor = Color.White;
            for (i = 0; i < nutra; i++)
                siEst[i] = true;
            if (panelcla.Visible == true)
                TrazasClas();
            if (promecod == true)
                promecod = false;
            if (nuhueco > 0)
            {
                if (huecolist.Count > 0)
                    huecolist.Clear();
                nuhueco = 0;
            }
            if (panelcoda.Visible == true)
            {
                listBox2.SelectedIndex = idbox2ant;
                return;
            }
            calcfilt = false;
            filt = false;
            // seguir es la variable asociada con la opcion de lectura de tremor continuo en varios 
            //archivos.
            if (seguir == false)
            {
                tremor = false;
                tinitremor = 0;
                boTremor.BackColor = Color.White;
            }
            boFiltro.BackColor = Color.White;
            panelFiltro.BackColor = Color.White;
            id = (ushort)(listBox2.SelectedIndex);
            if (id >= listBox2.Items.Count)
                id = (ushort)(listBox2.Items.Count - 1);
            idbox2ant = id;
            panelDR.Visible = false;
            if (fcnan[id] <= 0)
            {
                DR = 0;
                boDR.BackColor = Color.Gold;
            }
            //Si en inicio.txt hay una línea que contenga UMBRAL, la estacion y el valor de cuenta por encima
            //del cual se sobrepasa, aparece el botón Umbral que al activarlo muestra en color rojo sobre
            //la traza, el sector que sobrepasa el valor de Umbral.
            boUmbral.Visible = false;
            if (numbral > 0)
            {
                for (i = 0; i < numbral; i++)
                {
                    if (est[id].Substring(0, 4) == estumbral[i].Substring(0, 4))
                    {
                        boUmbral.Visible = true;
                        break;
                    }
                }
            }
            sipro = 0;
            boprom.BackColor = Color.White;
            volcanTarro();
            panel1.Invalidate();

            return;
        }
        /// <summary>
        /// Con esta rutina se puede mostrar en pantalla otra estación adicional en un panel
        /// inferior. En este caso se puede filtrar la señal sin utilizar el Octave. Para que
        /// funcione es necesario activar el nombre de la Estación con el botón derecho del ratón.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void listBox2_MouseDown(object sender, MouseEventArgs e)
        {
            int i;

            if (e.Button == MouseButtons.Left) return;
            if (panel1a.Visible == false)
            {
                panel1.BorderStyle = BorderStyle.FixedSingle;
                i = Height - 57;
                panel1a.Visible = true;
                panel1.Size = new Size(panel1.Size.Width, (int)(i / 2.0));
                panel1a.Size = new Size(panel1.Size.Width, (int)(i / 2.0));
                panel1a.Location = new Point(panel1.Location.X, panel1.Location.Y + panel1.Height);
                i = (short)(panel1a.Width / 2.0);
                panelbotFilX.Location = new Point(i + 50, panelbotFilX.Location.Y);
            }
            filtx = false;
            boFilBajX.BackColor = Color.White;
            boFilAltX.BackColor = Color.White;
            boFilBanX.BackColor = Color.White;
            cfilx = '0';
            i = (int)(e.Y / (double)(listBox2.ItemHeight)) + listBox2.TopIndex;
            if (i >= listBox2.Items.Count) i = listBox2.Items.Count - 1;

            try
            {
                ida = (ushort)(i);
                panel1a.Invalidate();
            }
            catch
            {
            }
        }
        /// <summary>
        /// Rutina que modifica el intervalo de fechas, en la caja de fechas por minuto (listBox1) 
        /// y busca sismos clasificados en la Base, dentro del intervalo de tiempo seleccionado.
        /// </summary>
        void LLenaBox()
        {
            ushort i = 0, j = 0, contdatos = 0;
            long ll;
            string fecha = "", cla = "___", lin = "";
            string[] listcla;
            long[] llist;

            if (disparo == true)
                return;
            listBox1.Items.Clear();

            contdatos = util.Leerbase(panel2, rutbas, ll1, ll2, cl, volcan);// rutina que chequea y cuenta los minutos que correspondan a sismos clasificados en la base.
            if (File.Exists("amplis.txt"))
                LeerAmplitud(); // esta rutina lee las amplitudes con el fin de colocar las pepas indicativas de dichas lecturas.

            if (!File.Exists("datos.txt"))
            {
                for (ll = ll1; ll <= ll2; ll += 600000000) //  600000000 en centenares de nanosegundos, corresponde a un minuto
                {
                    DateTime fech = new DateTime(ll);
                    fecha = string.Format("{0:yy}/{0:MM}/{0:dd} {0:HH}:{0:mm}", fech) + " " + cla;
                    listBox1.Items.AddRange(new object[] { fecha });
                }
            }
            else
            {
                lin = "";
                listcla = new string[contdatos];
                llist = new long[contdatos];
                //en datos.txt estan los datos de los sismos clasificados de la base, correspondientes
                //al o los dias seleccionados
                StreamReader pr = new StreamReader("datos.txt");
                while (lin != null)
                {
                    try
                    {
                        lin = pr.ReadLine();
                        if (lin == null) break;
                        listcla[i] = lin.Substring(25, 3);//lista de clasificacciones
                        llist[i++] = long.Parse(lin.Substring(34, 19));
                    }
                    catch
                    {
                    }
                }
                pr.Close();
                for (ll = ll1; ll <= ll2; ll += 600000000)
                {
                    cla = "___";
                    for (j = 0; j < contdatos; j++)
                    {
                        if (llist[j] >= ll && llist[j] <= ll + 600000000)
                        {
                            cla = listcla[j];
                        }
                        else if (llist[j] > ll + 600000000) break;
                    }
                    DateTime fech = new DateTime(ll);
                    fecha = string.Format("{0:yy}/{0:MM}/{0:dd} {0:HH}:{0:mm}", fech) + " " + cla;
                    listBox1.Items.AddRange(new object[] { fecha });
                }
            }
            //fecha es la variable string, que guarda la fecha y hora cada minuto y la clasificacion
            //si existe un sismo clasificado en ese minuto.

            return;
        }
        /// <summary>
        /// El archivo amplis.txt es creado por la rutina util.Leerbase() y contiene todas las
        /// lecturas de amplitud de la Base, realizadas en el intervalo de tiempo seleccionado, con
        /// el fin de colocar las 'pepas' indicativas de las clasificaciones, en el panel1, donde
        /// se visualiza la traza de la estacion activa.
        /// Las variables valampl, siPampl y letamp y volampson utilizadas por la rutina util.PonePepas().
        /// </summary>
        void LeerAmplitud()
        {
            int k;
            string lin = "";

            StreamReader pr = new StreamReader("amplis.txt");
            lin = "";
            contampl = 0;///contador de amplitud debe ser
            while (lin != null)
            {
                try
                {
                    lin = pr.ReadLine();
                    if (lin == null)
                        break;
                    contampl += 1;
                }
                catch
                {
                }
            }
            pr.Close();

            valampl = new double[contampl];
            siPampl = new bool[contampl];
            letampl = new char[contampl];
            volampl = new char[contampl];

            StreamReader pr1 = new StreamReader("amplis.txt");
            lin = "";
            k = 0;
            while (lin != null)
            {
                try
                {
                    lin = pr1.ReadLine();
                    if (lin == null) break;
                    try
                    {
                        valampl[k] = double.Parse(lin.Substring(103, 13));
                        if (valampl[k] == 0)
                        {
                            valampl[k] = double.Parse(lin.Substring(0, 13));
                            siPampl[k] = false;
                        }
                        else siPampl[k] = true;
                        letampl[k] = lin[23];
                        volampl[k] = lin[21];
                    }
                    catch
                    {
                        valampl[k] = 0;
                        siPampl[k] = false;
                        letampl[k] = ' ';
                        volampl[k] = ' ';
                    }
                    k += 1;
                    if (k >= contampl) break;
                }
                catch
                {
                }
            }
            pr1.Close();

            return;
        }
        /// <summary>
        /// Esta rutina lee las trazas en formato SUDS Multiplexado. Los datos de las cuentas vienen entonces por paquetes o bloques;
        /// se utiliza una variable provisional en un arreglo tridimensional (cuu). La primera dimensión corresponde al número de archivo;
        /// la segunda al número del bloque en el archivo y la tercera a la posición de la cuenta en el bloque.
        /// El tiempo y la rata de muestreo, se guardan en variables locales bidimensionales, ya que ambos valores vienen indicados
        /// solo al inicio de los bloques. Lo mismo para las variables bby (número de bytes de los datos);
        /// dmx (número de muestras por bloque para cada estación); compo (componente vertical, Este o Norte);
        /// gana (ganancia de la estación en la tarjeta).
        /// Una vez guardados los valores en estas variables, se comprueba si hay huecos o traslapos entre los archivos
        /// y finalmente se asignan los valores a las variables globales (cu, tim, ra, etc.).
        /// </summary>
        /// <param name="inu">Indica la tarjeta en formato MUX a leer.</param>
        /// <returns>1 en caso de que el método se ejecute exitosamente, 0 en caso contrario.</returns>
        int LeeMux(int inu)
        {
            int[] cana, blo;
            double[] tc, rc;
            int[][] dmx;
            int[][][] cuu;
            short[][] bby;
            double[][] tii, rat;
            string[][] esta;
            short[][] gana;
            char[][] compo;
            bool[] si30;
            string[] estt = new string[Ma];

            byte[] bb;

            ushort num, arch, cont, nuarch = 0, nublo = 0;
            short nulis;
            short estru;
            int larestru, lardat, numu, ininutra;
            int i, ii, j, jj, k, kk, ide, nuto, lara;
            int an, me, di, ho, mi, vacio, cuvacio, traslapo, inicio;
            int totalbloques = 0, numerobloque = 0;
            long ll0, lll, pos, lenght;
            double ut, tinicio, tfinal, facra, tie, dd1, dd2, dd = 0;
            double tiempo0 = 0, tiempotot = 0, tinicero = 0;
            double[] dift = new double[2];
            bool si = false, traslapomayor = false;
            string lis = "", nom = "", nomini = "", nomsii = "", ca = "", ca2 = "";
            char cc = ' ';


            if (yamux[inu] == true)
                return (0);
            ininutra = nutra;
            dift[0] = 0.0;
            dift[1] = 0.0;
            cont = 0;
            if (utmux[inu] != 0)
                ut = (double)(utmux[inu]) * 3600.0;
            else
                ut = 0;
            lis = listBox1.SelectedItem.ToString();
            nulis = (short)(listBox1.SelectedIndex);
            num = (ushort)(durmux[inu] / 60.0);
            an = int.Parse(lis.Substring(0, 2));
            if (an < 88) an += 2000;
            else an += 1900;
            me = int.Parse(lis.Substring(3, 2));
            di = int.Parse(lis.Substring(6, 2));
            ho = int.Parse(lis.Substring(9, 2));
            mi = int.Parse(lis.Substring(12, 2));
            DateTime fech1 = new DateTime(an, me, di, ho, mi, 0);
            ll0 = fech1.Ticks;
            tinicio = ((double)(ll0) - Feisuds) / 10000000.0;
            tfinal = tinicio + totven * 60.0;
            if (ut != 0) ll0 -= ((long)(ut * 10000000.0));

            // En la rutina siguiente, se busca desde y hacia atras de la seleccion del usuario, si
            // existe una traza para clasificar. Esta busqueda termina cuando se encuentra una traza o
            // cuando se supera el intervalo de tiempo de las trazas, el cual viene indicado en el
            // archivo inicio.txt (variable num). Este es el archivo inicial y a partir de allí, se
            // continúa leyendo archivos hasta que se supere la duración en segundos indicada en 
            // inicio.txt o 60 se gundos por defecto.
            for (i = 0; i <= num + 2; i++)
            {
                DateTime fech2 = new DateTime(ll0);
                nom = rutmux[inu];
                if (sianmux[inu] == true) nom += "\\" + string.Format("{0:yyyy}", fech2);
                if (simesmux[inu] == true) nom += "\\" + string.Format("{0:MM}", fech2);
                if (sidiamux[inu] == true) nom += "\\" + string.Format("{0:dd}", fech2);
                nomsii = string.Format("{0:MM}{0:dd}{0:HH}{0:mm}.", fech2) + extmux[inu];
                nom += "\\" + nomsii;
                panel2.Visible = true;
                j = inu + 1;
                ca = "Buscando la existencia de archivo...\nSi este aviso se demora, es muy probable\nque haya problemas de comunicacion!!\n    MUX   " + j.ToString();
                util.Mensaje(panel2, ca, true);
                if (File.Exists(nom))
                {
                    si = true;
                    panel2.Visible = false;
                    break;
                }
                panel2.Visible = false;
                ll0 -= 600000000;
                nulis -= 1;
            }
            if (si == false)
            {
                NoMostrar = true;
                MessageBox.Show("No hay Sismos Mux " + (inu + 1).ToString() + " !!!");
                return (0);
            }
            panel2.Visible = true;
            j = inu + 1;
            ca = "Adquiriendo Trazas\n    MUX   " + j.ToString();
            util.Mensaje(panel2, ca, false);
            if (nulis >= 0) ide = nulis;
            else ide = 0;

            nomini = nom;

            do
            {
                // Primero se mira cuantos archivos existen para la duracion total seleccionada, valor
                // que por defecto se indica en el archivo inicio.txt.
                if (File.Exists(nom)) nuarch += 1;
                ide += 1;
                if (ide == listBox1.Items.Count) break;
                lis = listBox1.Items[ide].ToString();
                an = int.Parse(lis.Substring(0, 2));
                if (an < 88) an += 2000;
                else an += 1900;
                me = int.Parse(lis.Substring(3, 2));
                di = int.Parse(lis.Substring(6, 2));
                ho = int.Parse(lis.Substring(9, 2));
                mi = int.Parse(lis.Substring(12, 2));
                DateTime fech3 = new DateTime(an, me, di, ho, mi, 0);
                ll0 = fech3.Ticks;
                if (ut != 0) ll0 -= ((long)(ut * 10000000.0));
                DateTime fech4 = new DateTime(ll0);
                nom = rutmux[inu];
                if (sianmux[inu] == true) nom += "\\" + string.Format("{0:yyyy}", fech4);
                if (simesmux[inu] == true) nom += "\\" + string.Format("{0:MM}", fech4);
                if (sidiamux[inu] == true) nom += "\\" + string.Format("{0:dd}", fech4);
                nom += "\\" + string.Format("{0:MM}{0:dd}{0:HH}{0:mm}.", fech4) + extmux[inu];
                cont += 1;
                if (cont > totven) break;
            } while (ide < listBox1.Items.Count);

            nom = nomini;
            ide = nulis;
            nom = nomini;
            arch = 0;
            cana = new int[nuarch];
            blo = new int[nuarch];
            tc = new double[nuarch];
            rc = new double[nuarch];
            si30 = new bool[nuarch];
            cuu = new int[nuarch][][];
            tii = new double[nuarch][];
            bby = new short[nuarch][];
            rat = new double[nuarch][];
            dmx = new int[nuarch][];
            esta = new string[nuarch][];
            gana = new short[nuarch][];
            compo = new char[nuarch][];

            for (i = 0; i < nuarch; i++)
            {
                tc[i] = 0; // variable que guarda la correcion de tiempo
                si30[i] = false;  // variable para saber si la estructura 30 o de corrección de tiempo existe.
            }

            do
            {
                if (File.Exists(nom))
                {
                    try
                    {
                        nublo = 0;
                        cana[arch] = 0;
                        si30[arch] = false;
                        tc[arch] = 0;
                        rc[arch] = 0;
                        FileInfo ar = new FileInfo(nom);
                        BinaryReader br0 = new BinaryReader(ar.OpenRead());

                        // aqui se mira cuantos bloques existen en cada archivo:
                        pos = 0;
                        lenght = br0.BaseStream.Length;
                        while (pos < lenght)
                        {
                            br0.ReadBytes(2);
                            estru = br0.ReadInt16();
                            if (estru < 1 || estru > 32) break;
                            larestru = br0.ReadInt32();
                            lardat = br0.ReadInt32();
                            if (estru == 5) cana[arch] += 1;
                            else if (estru == 6) nublo += 1;
                            br0.ReadBytes(larestru);
                            if (lardat > 0) br0.ReadBytes(lardat);
                            pos += 12 + larestru + lardat;
                        }
                        br0.Close();

                        cuu[arch] = new int[nublo][];
                        tii[arch] = new double[nublo];
                        bby[arch] = new short[nublo];
                        rat[arch] = new double[nublo];
                        dmx[arch] = new int[nublo];
                        esta[arch] = new string[cana[arch]];
                        gana[arch] = new short[cana[arch]];
                        compo[arch] = new char[cana[arch]];
                        blo[arch] = 0;
                        j = 0;

                        // aqui se leen los datos como tal:
                        BinaryReader br = new BinaryReader(ar.OpenRead());
                        pos = 0;
                        while (pos < lenght)
                        {
                            br.ReadBytes(2);
                            estru = br.ReadInt16();
                            if (estru < 1 || estru > 32) break;
                            larestru = br.ReadInt32();
                            lardat = br.ReadInt32();
                            if (estru == 5)
                            {
                                // en esta estructura se encuentra los valores de ganancia y componente
                                br.ReadBytes(4);
                                esta[arch][j] = Encoding.ASCII.GetString(br.ReadBytes(5));
                                compo[arch][j] = br.ReadChar();
                                br.ReadBytes(52);
                                gana[arch][j] = br.ReadInt16();
                                br.ReadBytes(larestru - 64);
                                j += 1;
                            }
                            else if (estru == 6)
                            { // Es la estructura que contiene los datos multiplexados de las trazas
                                br.ReadBytes(4);
                                tii[arch][blo[arch]] = br.ReadDouble() + ut;
                                if (tinicero == 0)
                                {
                                    tinicero = tii[arch][blo[arch]];

                                }
                                else if (tii[arch][blo[arch]] < tinicero)
                                {
                                    br.ReadBytes(larestru - 12);
                                    if (lardat > 0) br.ReadBytes(lardat);
                                    continue;
                                }
                                if (tiadimux[inu] != 0) tii[arch][blo[arch]] += tiadimux[inu];
                                if (arch == 0 && blo[arch] == 0) tiempo0 = tii[0][0];
                                br.ReadBytes(4);
                                rat[arch][blo[arch]] = (double)(br.ReadSingle());
                                cc = br.ReadChar();
                                if (cc == 's' || cc == 'q' || cc == 'i' || cc == 'u') bby[arch][blo[arch]] = 2;
                                else if (cc == 'l' || cc == '2') bby[arch][blo[arch]] = 4;
                                br.ReadBytes(7);
                                dmx[arch][blo[arch]] = br.ReadInt32();
                                tiempotot = tii[arch][blo[arch]] - tiempo0 + (dmx[arch][blo[arch]] / rat[arch][blo[arch]]);
                                if (larestru > 32) br.ReadBytes(larestru - 32);
                                numu = j * dmx[arch][blo[arch]];

                                kk = numu * bby[arch][blo[arch]];
                                bb = new byte[kk];
                                bb = br.ReadBytes(kk);
                                cuu[arch][blo[arch]] = new int[numu];
                                if (bby[arch][blo[arch]] == 2) for (i = 0; i < numu; i++) cuu[arch][blo[arch]][i] = BitConverter.ToInt16(bb, i * 2);
                                else for (i = 0; i < numu; i++) cuu[arch][blo[arch]][i] = BitConverter.ToInt32(bb, i * 4);
                                blo[arch] += 1;
                            }
                            else if (estru == 30)
                            { // Estructura que contiene la corrección del tiempo y de la rata de muestreo.                                
                                estru30 = true;
                                si30[arch] = true;
                                br.ReadBytes(12);
                                tc[arch] = br.ReadDouble(); // variable que guarda la correccion de tiempo
                                rc[arch] = br.ReadSingle(); // variable que guarda la correccion de la rata de muestreo
                                br.ReadBytes(9);
                                pos += 24;
                                if (Math.Abs(tc[arch]) >= 86400.0)
                                {
                                    tc[arch] = 0;
                                    si30[arch] = false;
                                }
                            }
                            else
                            {
                                br.ReadBytes(larestru);
                                if (lardat > 0) br.ReadBytes(lardat);
                            }
                            pos += 12 + larestru + lardat;
                        }
                        br.Close();
                        arch += 1;
                    }
                    catch
                    {
                    }
                }
                ide += 1;
                if (ide == listBox1.Items.Count || ide < 0) break;
                lis = listBox1.Items[ide].ToString();
                an = int.Parse(lis.Substring(0, 2));
                if (an < 88) an += 2000;
                else an += 1900;
                me = int.Parse(lis.Substring(3, 2));
                di = int.Parse(lis.Substring(6, 2));
                ho = int.Parse(lis.Substring(9, 2));
                mi = int.Parse(lis.Substring(12, 2));

                DateTime fech3 = new DateTime(an, me, di, ho, mi, 0);
                ll0 = fech3.Ticks;
                if (ut != 0) ll0 -= ((long)(ut * 10000000.0));
                DateTime fech4 = new DateTime(ll0);

                nom = rutmux[inu];
                if (sianmux[inu] == true) nom += "\\" + string.Format("{0:yyyy}", fech4);
                if (simesmux[inu] == true) nom += "\\" + string.Format("{0:MM}", fech4);
                if (sidiamux[inu] == true) nom += "\\" + string.Format("{0:dd}", fech4);
                nom += "\\" + string.Format("{0:MM}{0:dd}{0:HH}{0:mm}.", fech4) + extmux[inu];
                if (tiempotot > totven * 60.0) break;

            } while (ide < listBox1.Items.Count);

            if (arch > 0)
            {
                nuto = 0;
                for (i = 0; i < arch; i++)
                {
                    si = false;
                    for (j = 0; j < cana[i]; j++)
                    {
                        if (nuto > 0)
                        {
                            for (k = 0; k < nuto; k++)
                            {
                                if (string.Compare(estt[k].Substring(0, 4), esta[i][j].Substring(0, 4)) == 0 || esta[i][j].Substring(0, 4) == "XXXX")
                                {
                                    si = true;
                                    break;
                                }

                            }
                        }
                        if (si == false)
                        {
                            estt[nuto] = esta[i][j];
                            ga[nutra + nuto] = gana[i][j];
                            if (ga[nutra + nuto] <= 0) ga[nutra + nuto] = 1;
                            comp[nutra + nuto] = compo[i][j];
                            nuto += 1;
                        }
                        else si = false;
                    }
                }
                // en el codigo anterior, se miran todas las estaciones y se van asignado en su orden de aparicion
                // a la variable estt. nuto va llevando el numero de trazas. Esto se hace necesario, pues en el 
                // intervalo de tiempo seleccionado, se ha posido añadir o quitar una estacion de las tarjetas
                // digitalizadoras.

                // aqui se van a asignar los datos, estacion por estacion.             
                for (ii = 0; ii < nuto; ii++)
                {
                    lara = 0;
                    tie = 0;
                    facra = 0;
                    vacio = 0;
                    traslapo = 0;
                    dd2 = 0;

                    // aqui se va a buscar si existen vacios o traslapos, para una misma estacion. 
                    for (i = 0; i < arch; i++)
                    {
                        if (si30[i] == false)
                        {
                            no30[nutra] = true;
                            if (i > 0 && si30[i - 1] == true) tc[i] = tc[i - 1];
                            else if (i < arch - 1 && si30[i + 1] == true) tc[i] = tc[i + 1];
                        }
                        for (j = 0; j < cana[i]; j++)
                        {
                            if (estt[ii].Substring(0, 4) == esta[i][j].Substring(0, 4))
                            {
                                for (k = 0; k < blo[i]; k++)
                                {
                                    if (lara > 0)
                                    {
                                        dd1 = tii[i][k] + tc[i];
                                        if (dd1 - dd2 > (facra + 0.005) && dd2 > 0)
                                        {
                                            siRoto[nutra] = true;
                                            vacio += (int)((dd1 - dd2) * rat[i][k]);
                                        }
                                        else if (dd2 - dd1 > (facra + 0.005))
                                        {
                                            siTraslapo[nutra] = true;
                                            traslapo += (int)((dd2 - dd1) * rat[i][k]);
                                        }
                                    }
                                    lara += dmx[i][k];
                                    facra = 1.0 / (rat[i][k] + rc[i]);
                                    dd2 = tii[i][k] + tc[i] + facra * dmx[i][k];
                                }
                                break;
                            }
                        }
                    }
                    // asignacion de datos a las variables locales. En el caso que existan vacios, se asigna
                    //  el ultimo dato a todo el vacio. En el caso de traslapo, se desechan los datos repetidos.
                    if (lara > 0)
                    {
                        try
                        {
                            lara += vacio;
                            lara -= traslapo;
                            est[nutra] = estt[ii].Substring(0, 4) + " ";
                            try
                            {
                                cu[nutra] = new int[lara];
                            }
                            catch
                            {
                                NoMostrar = true;
                                ca = estt[ii].Substring(0, 4) + " (cu) Tal vez se esta quedando sin memoria??\nQuedan: " + ramCounter.NextValue().ToString() + " MegaBytes en memoria\nPuede salir y reducir minutos con Param o bien Reiniciar el Programa con un valor menor en minutos.\n\nOtra posibilidad es problemas con el tiempo de los archivos (UT y Hora Local?)";
                                DialogResult result = MessageBox.Show(ca,
                                                                "Salir ???", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                if (result == DialogResult.Yes)
                                {
                                    cuu = null;
                                    tii = null;
                                    bby = null;
                                    dmx = null;
                                    gana = null;
                                    compo = null;
                                    return (-1);
                                }
                            }
                            try
                            {
                                tim[nutra] = new double[lara];
                            }
                            catch
                            {
                                NoMostrar = true;
                                ca = estt[ii].Substring(0, 4) + " (tim) Tal vez se esta quedando sin memoria??\nQuedan: " + ramCounter.NextValue().ToString() + " MegaBytes en memoria\nPuede salir y reducir minutos con Param o bien Reiniciar el Programa con un valor menor en minutos.\n\nOtra posibilidad es problemas con el tiempo de los archivos (UT y Hora Local?)";
                                DialogResult result = MessageBox.Show(ca,
                                                                "Salir ???", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                if (result == DialogResult.Yes)
                                {
                                    cuu = null;
                                    tii = null;
                                    bby = null;
                                    dmx = null;
                                    gana = null;
                                    compo = null;
                                    return (-1);
                                }
                            }
                            ra[nutra] = rat[0][0] + rc[0];
                            siEst[nutra] = true;
                            tar[nutra] = tarmux[inu];
                            by[nutra] = bby[0][0];// se supone que es multiplexado y todos los datos tienen la misma caracteristica (rata, tipodato, etc!!

                            jj = 0;
                            dd2 = 0;
                            cuvacio = 0;
                            traslapomayor = false;
                            for (i = 0; i < arch; i++)
                            {
                                for (j = 0; j < cana[i]; j++)
                                {
                                    if (estt[ii].Substring(0, 4) == esta[i][j].Substring(0, 4))
                                    {
                                        for (k = 0; k < blo[i]; k++)
                                        {
                                            tie = tii[i][k] + tc[i];
                                            facra = 1.0 / (rat[i][k] + rc[i]);
                                            inicio = 0;
                                            if (tie - dd2 > (facra + 0.005) && dd2 > 0)
                                            {
                                                vacio = (int)((tie - dd2) * rat[i][k]);
                                                dd2 += facra;
                                                for (kk = 0; kk < vacio; kk++)
                                                {
                                                    tim[nutra][jj] = dd2 + kk * facra;
                                                    cu[nutra][jj] = cuvacio;
                                                    jj += 1;
                                                    if (jj >= lara) break;
                                                }
                                            }
                                            else if (dd2 - tie > (facra + 0.005))
                                            {
                                                traslapo = (int)((dd2 - tie) * rat[i][k]);
                                                if (traslapo > dmx[i][k] && traslapomayor == false)
                                                {
                                                    traslapomayor = true;
                                                    dd = traslapo / (double)(dmx[i][k]);
                                                    totalbloques = (int)(dd);
                                                    inicio = 1 + (int)((dd - (double)(totalbloques)) * dmx[i][k]);
                                                    numerobloque = 0;
                                                }
                                                else if (traslapomayor == false) inicio = traslapo;

                                            }
                                            if (traslapomayor == false)
                                            {
                                                if (jj < lara)
                                                {
                                                    for (kk = inicio; kk < dmx[i][k]; kk++)
                                                    {
                                                        tim[nutra][jj] = tie + kk * facra;
                                                        cu[nutra][jj] = cuu[i][k][j * dmx[i][k] + kk];
                                                        jj += 1;
                                                        if (jj >= lara) break;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                numerobloque += 1;
                                                if (numerobloque >= totalbloques)
                                                {
                                                    traslapomayor = false;
                                                    numerobloque = 0;
                                                }
                                            }
                                            facra = 1.0 / (rat[i][k] + rc[i]);
                                            dd2 = tii[i][k] + tc[i] + facra * dmx[i][k];
                                            cuvacio = cu[nutra][jj - 1];
                                        } // for k.....                              
                                        break;
                                    } //if estt[ii]......
                                }
                            }
                            NoMostrar = true;
                            nutra += 1;
                        }
                        catch
                        {
                            NoMostrar = true;
                            MessageBox.Show("problemas en " + estt[ii].Substring(0, 4) + " muestras=" + lara.ToString() + " ii=" + ii.ToString());
                        }
                    } // if lara>0
                } // ii
            }
            cuu = null;
            tii = null;
            bby = null;
            dmx = null;
            gana = null;
            compo = null;

            panel2.Visible = false;
            yamux[inu] = true;

            return (1);
        }
        /// <summary>
        /// Esta rutina lee los datos en formato SUDS Demultiplexado.
        /// Los datos de cuentas se guardan en una variable tridimensional (cuu),
        /// donde primeramente se asigna el número de archivo, luego el número de traza y por último los datos de cuentas.
        /// La mayoría de las demás variables son bidimensionales, donde se guarda primeramente el número de archivo y luego
        /// el valor de acuerdo al número de traza. Por último, se asignan los datos a las variables globales, estación por estación.
        /// En esencia la rutina es muy parecida a la anterior (SUDS Multiplexado), solo que aquí,
        /// los datos se desglosan completamente por estación.
        /// </summary>
        /// <param name="inu">Indica la tarjeta en formato MUX a leer.</param>
        void LeeDmx(int inu)
        {
            int[] cana, siete;
            double[] tc, rc;
            int[][] dmx;
            int[][][] cuu;
            short[][] bby;
            double[][] tii, rat;
            string[][] esta;
            short[][] gana;
            char[][] compo;
            bool[] si30;
            string[] estt = new string[Ma];
            ushort num, arch, cont, cont2, nuarch = 0, nusiete = 0;
            short nulis;
            short estru;
            int larestru, lardat;
            int i, ii, j, jj, k, kk, inicio, ide, nuto, lara;
            int an, me, di, ho, mi, vacio, cuvacio, traslapo;
            long ll0;
            double ut, tinicio, tfinal, facra, tie, dd1, dd2;
            bool si = false;
            string lis = "", nom = "", nomini = "", nomsii = "", ca = "";
            char cc = ' ';


            if (yadmx[inu] == true) return;
            cont = 0;
            if (utdmx[inu] != 0) ut = (double)(utdmx[inu]) * 3600.0;
            else ut = 0;
            lis = listBox1.SelectedItem.ToString();
            nulis = (short)(listBox1.SelectedIndex);
            num = (ushort)(durdmx[inu] / 60.0);
            an = int.Parse(lis.Substring(0, 2));
            if (an < 88) an += 2000;
            else an += 1900;
            me = int.Parse(lis.Substring(3, 2));
            di = int.Parse(lis.Substring(6, 2));
            ho = int.Parse(lis.Substring(9, 2));
            mi = int.Parse(lis.Substring(12, 2));
            DateTime fech1 = new DateTime(an, me, di, ho, mi, 0);
            ll0 = fech1.Ticks;
            tinicio = ((double)(ll0) - Feisuds) / 10000000.0;
            tfinal = tinicio + totven * 60.0;
            if (ut != 0) ll0 -= ((long)(ut * 10000000.0));

            //Basicamente aqui se tiene el mismo objetivo que en la rutina anterior, o sea se
            //busca el primer archivo y a partir de alli, se lee secuencialmente hasta que se
            //abarque el intervalo de tiempo indicado en inicio.tx o 60 segundos por defecto.
            for (i = 0; i <= num; i++)
            {
                DateTime fech2 = new DateTime(ll0);
                nom = rutdmx[inu];
                if (siandmx[inu] == true) nom += "\\" + string.Format("{0:yyyy}", fech2);
                if (simesdmx[inu] == true) nom += "\\" + string.Format("{0:MM}", fech2);
                if (sidiadmx[inu] == true) nom += "\\" + string.Format("{0:dd}", fech2);
                nomsii = string.Format("{0:MM}{0:dd}{0:HH}{0:mm}.", fech2) + extdmx[inu];
                nom += "\\" + nomsii;
                if (File.Exists(nom))
                {
                    si = true;
                    break;
                }
                ll0 -= 600000000;
                nulis -= 1;
            }
            if (si == false)
            {
                NoMostrar = true;
                MessageBox.Show("No hay Sismos DMX " + (inu + 1).ToString() + " !!!");
                return;
            }
            panel2.Visible = true;
            j = inu + 1;
            ca = "Adquiriendo Trazas\n    DMX   " + j.ToString();
            util.Mensaje(panel2, ca, false);
            if (nulis >= 0) ide = nulis;
            else ide = 0;

            nomini = nom;

            do
            {
                if (File.Exists(nom)) nuarch += 1;
                ide += 1;
                if (ide == listBox1.Items.Count) break;
                lis = listBox1.Items[ide].ToString();
                an = int.Parse(lis.Substring(0, 2));
                if (an < 88) an += 2000;
                else an += 1900;
                me = int.Parse(lis.Substring(3, 2));
                di = int.Parse(lis.Substring(6, 2));
                ho = int.Parse(lis.Substring(9, 2));
                mi = int.Parse(lis.Substring(12, 2));
                DateTime fech3 = new DateTime(an, me, di, ho, mi, 0);
                ll0 = fech3.Ticks;
                if (ut != 0) ll0 -= ((long)(ut * 10000000.0));
                DateTime fech4 = new DateTime(ll0);
                nom = rutdmx[inu];
                if (siandmx[inu] == true) nom += "\\" + string.Format("{0:yyyy}", fech4);
                if (simesdmx[inu] == true) nom += "\\" + string.Format("{0:MM}", fech4);
                if (sidiadmx[inu] == true) nom += "\\" + string.Format("{0:dd}", fech4);
                nom += "\\" + string.Format("{0:MM}{0:dd}{0:HH}{0:mm}.", fech4) + extdmx[inu];
                cont += 1;
                if (cont > totven) break;
            } while (ide < listBox1.Items.Count);

            nom = nomini;
            ide = nulis;
            nom = nomini;
            cont2 = 0;
            arch = 0;
            cana = new int[nuarch];
            siete = new int[nuarch];
            tc = new double[nuarch];
            rc = new double[nuarch];
            si30 = new bool[nuarch];
            cuu = new int[nuarch][][];
            tii = new double[nuarch][];
            bby = new short[nuarch][];
            rat = new double[nuarch][];
            dmx = new int[nuarch][];
            esta = new string[nuarch][];
            gana = new short[nuarch][];
            compo = new char[nuarch][];

            do
            {
                if (File.Exists(nom))
                {
                    try
                    {
                        nusiete = 0;
                        cana[arch] = 0;
                        si30[arch] = false;
                        tc[arch] = 0;
                        rc[arch] = 0;
                        //Se averigua cuantos canales existen.
                        FileInfo ar = new FileInfo(nom);
                        BinaryReader br0 = new BinaryReader(ar.OpenRead());
                        while (br0.PeekChar() != -1)
                        {
                            br0.ReadBytes(2);
                            estru = br0.ReadInt16();
                            if (estru < 1 || estru > 32) break;
                            larestru = br0.ReadInt32();
                            lardat = br0.ReadInt32();
                            if (estru == 7)
                            {
                                cana[arch] += 1;
                                nusiete += 1;
                            }
                            br0.ReadBytes(larestru);
                            if (lardat > 0) br0.ReadBytes(lardat);
                        }
                        br0.Close();
                        cuu[arch] = new int[nusiete][];
                        tii[arch] = new double[nusiete];
                        bby[arch] = new short[nusiete];
                        rat[arch] = new double[nusiete];
                        dmx[arch] = new int[nusiete];
                        esta[arch] = new string[cana[arch]];
                        gana[arch] = new short[cana[arch]];
                        compo[arch] = new char[cana[arch]];
                        siete[arch] = 0;
                        j = 0;

                        BinaryReader br = new BinaryReader(ar.OpenRead());
                        while (br.PeekChar() != -1)
                        {
                            br.ReadBytes(2);
                            estru = br.ReadInt16();
                            if (estru < 1 || estru > 32) break;
                            larestru = br.ReadInt32();
                            lardat = br.ReadInt32();
                            if (estru == 5)
                            {// Estructura que tiene la ganacia
                                br.ReadBytes(62);
                                gana[arch][j] = br.ReadInt16();
                                br.ReadBytes(larestru - 64);
                                j += 1;
                            }
                            else if (estru == 7)
                            {// Estructura que contiene los datos.
                                br.ReadBytes(4);
                                esta[arch][siete[arch]] = Encoding.ASCII.GetString(br.ReadBytes(5));
                                compo[arch][siete[arch]] = br.ReadChar();
                                br.ReadBytes(2);
                                tii[arch][siete[arch]] = br.ReadDouble() + ut;
                                br.ReadBytes(2);
                                cc = br.ReadChar();
                                if (cc == 's' || cc == 'q' || cc == 'i' || cc == 'u') bby[arch][siete[arch]] = 2;
                                else if (cc == 'l' || cc == '2') bby[arch][siete[arch]] = 4;
                                br.ReadBytes(5);
                                lara = br.ReadInt32();
                                rat[arch][siete[arch]] = (double)(br.ReadSingle());
                                br.ReadBytes(16);
                                dd1 = br.ReadDouble();
                                tii[arch][siete[arch]] += dd1;
                                dd2 = br.ReadSingle();
                                rat[arch][siete[arch]] += dd2;
                                if (larestru > 64) br.ReadBytes(larestru - 64);
                                cuu[arch][siete[arch]] = new int[lara];
                                if (lardat > 0)
                                {
                                    if (bby[arch][siete[arch]] == 4) for (i = 0; i < lara; i++) cuu[arch][siete[arch]][i] = br.ReadInt32();
                                    else for (i = 0; i < lara; i++) cuu[arch][siete[arch]][i] = (int)(br.ReadInt16());
                                }

                                siete[arch] += 1;
                            }
                            else if (estru == 30)
                            {// Estructura que contiene la correccion de tiempo.
                                si30[arch] = true;
                                br.ReadBytes(12);
                                tc[arch] = br.ReadDouble();
                                rc[arch] = br.ReadSingle();
                                br.ReadBytes(9);
                            }
                            else
                            {
                                br.ReadBytes(larestru);
                                if (lardat > 0) br.ReadBytes(lardat);
                            }
                        }
                        br.Close();
                        arch += 1;
                    }
                    catch
                    {
                    }
                }
                ide += 1;
                if (ide == listBox1.Items.Count || ide < 0) break;
                lis = listBox1.Items[ide].ToString();
                an = int.Parse(lis.Substring(0, 2));
                if (an < 88) an += 2000;
                else an += 1900;
                me = int.Parse(lis.Substring(3, 2));
                di = int.Parse(lis.Substring(6, 2));
                ho = int.Parse(lis.Substring(9, 2));
                mi = int.Parse(lis.Substring(12, 2));

                DateTime fech3 = new DateTime(an, me, di, ho, mi, 0);
                ll0 = fech3.Ticks;
                if (ut != 0) ll0 -= ((long)(ut * 10000000.0));
                DateTime fech4 = new DateTime(ll0);

                nom = rutdmx[inu];
                if (siandmx[inu] == true) nom += "\\" + string.Format("{0:yyyy}", fech4);
                if (simesdmx[inu] == true) nom += "\\" + string.Format("{0:MM}", fech4);
                if (sidiadmx[inu] == true) nom += "\\" + string.Format("{0:dd}", fech4);
                nom += "\\" + string.Format("{0:MM}{0:dd}{0:HH}{0:mm}.", fech4) + extdmx[inu];
                cont2 += 1;
                if (cont2 > totven) break;

            } while (ide < listBox1.Items.Count);

            // aqui se asigna secuencialmente el nombre de las estaciones a la variable estt. La variable nuto, 
            //lleva la cuenta de las trazas.

            if (arch > 0)
            {
                nuto = 0;
                for (i = 0; i < arch; i++)
                {
                    si = false;
                    for (j = 0; j < cana[i]; j++)
                    {
                        if (nuto > 0)
                        {
                            for (k = 0; k < nuto; k++)
                            {
                                if (string.Compare(estt[k].Substring(0, 4), esta[i][j].Substring(0, 4)) == 0 || esta[i][j].Substring(0, 4) == "XXXX")
                                {
                                    si = true;
                                    break;
                                }

                            }
                        }
                        if (si == false)
                        {
                            estt[nuto] = esta[i][j];
                            ga[nutra + nuto] = gana[i][j];
                            if (ga[nutra + nuto] <= 0) ga[nutra + nuto] = 1;
                            comp[nutra + nuto] = compo[i][j];
                            nuto += 1;
                        }
                        else si = false;
                    }
                }
                // aqui se van a asignar los datos a cada estacion sucesivamente.
                for (ii = 0; ii < nuto; ii++)
                {
                    lara = 0;
                    tie = 0;
                    facra = 0;
                    vacio = 0;
                    traslapo = 0;
                    dd2 = 0;
                    for (i = 0; i < arch; i++)
                    {
                        for (j = 0; j < cana[i]; j++)
                        {
                            if (estt[ii].Substring(0, 4) == esta[i][j].Substring(0, 4))
                            {
                                if (lara > 0)
                                {
                                    dd1 = tii[i][j] + tc[i];
                                    if (dd1 - dd2 > (facra + 0.005) && dd2 > 0)
                                    {
                                        siRoto[nutra] = true;
                                        vacio += (int)((dd1 - dd2) * rat[i][j]);
                                    }
                                    else if (dd2 - dd1 > (facra + 0.005) && dd2 > 0)
                                    {
                                        siTraslapo[nutra] = true;
                                        traslapo += (int)((dd2 - dd1) * rat[i][j]);
                                    }
                                }
                                lara += cuu[i][j].Length;
                                facra = 1.0 / (rat[i][j] + rc[i]);
                                dd2 = tii[i][j] + tc[i] + facra * cuu[i][j].Length;
                                break;
                            }
                        }
                    }

                    if (lara > 0)
                    {
                        lara += vacio;
                        lara -= traslapo;
                        est[nutra] = estt[ii].Substring(0, 4) + " ";
                        cu[nutra] = new int[lara];
                        tim[nutra] = new double[lara];
                        siEst[nutra] = true;
                        tar[nutra] = tardmx[inu];

                        jj = 0;
                        dd2 = 0;
                        cuvacio = 0;
                        for (i = 0; i < arch; i++)
                        {
                            for (j = 0; j < cana[i]; j++)
                            {
                                if (estt[ii].Substring(0, 4) == esta[i][j].Substring(0, 4))
                                {
                                    ra[nutra] = rat[i][j];
                                    by[nutra] = bby[i][j];
                                    tie = tii[i][j] + tc[i];
                                    facra = 1.0 / (rat[i][j] + rc[i]);
                                    inicio = 0;
                                    if (tie - dd2 > (facra + 0.005) && dd2 > 0)
                                    {
                                        vacio = (int)((tie - dd2) * rat[i][j]);
                                        dd2 += facra;
                                        for (kk = 0; kk < vacio; kk++)
                                        {
                                            tim[nutra][jj] = dd2 + kk * facra;
                                            cu[nutra][jj] = cuvacio;
                                            jj += 1;
                                            if (jj >= lara) break;
                                        }
                                    }
                                    else if (dd2 - tie > (facra + 0.005) && dd2 > 0)
                                    {
                                        traslapo = (int)((dd2 - tie) * rat[i][j]);
                                        inicio = traslapo;
                                    }
                                    for (kk = inicio; kk < cuu[i][j].Length; kk++)
                                    {
                                        tim[nutra][jj] = tie + kk * facra;
                                        cu[nutra][jj] = cuu[i][j][kk];
                                        jj += 1;
                                        if (jj >= lara) break;
                                    }
                                    facra = 1.0 / (rat[i][j] + rc[i]);
                                    dd2 = tii[i][j] + tc[i] + facra * cuu[i][j].Length;
                                    cuvacio = cu[nutra][jj - 1];
                                    break;
                                }
                            }
                        }
                        nutra += 1;
                    } // if lara>0
                } // ii
            }

            panel2.Visible = false;
            yadmx[inu] = true;

            return;
        }
        /// <summary>
        /// Rutina que adecua el nombre del archivo con ciertas caracteristicas (ver la rutina siguiente)
        /// </summary>
        /// <param name="sian">Si existe la carpeta del año es igual a true en caso contrario es false.</param>
        /// <param name="sime">Si existe la carpeta del mes es igual a true en caso contrario es false.</param>
        /// <param name="sidi">Si existe la carpeta del día es igual a true en caso contrario es false.</param>
        /// <param name="nomprincipal">El nombre principal de la traza.</param>
        /// <param name="nomext">La extensión de archive de la traza.</param>
        /// <param name="fech">La fecha que registra la traza.</param>
        /// <returns></returns>
        string Nombre10(bool sian, bool sime, bool sidi, string nomprincipal, string nomext, DateTime fech)
        {
            short numcara = 0;
            int j, k;
            string nom = "";

            for (j = 0; j < nomprincipal.Length; j++)
            {
                if (nomprincipal[j] == 'Y')
                {
                    numcara = 0;
                    k = j + 1;
                    do
                    {
                        numcara += 1;
                        if (nomprincipal[k++] != 'Y')
                            break;
                    } while (nomprincipal[k - 1] == 'Y');
                    j += numcara - 1;
                    if (numcara > 0)
                    {
                        if (numcara == 4)
                            nom += string.Format("{0:yyyy}", fech);
                        else if (numcara == 2)
                            nom += string.Format("{0:yy}", fech);
                    }
                }
                else if (nomprincipal[j] == 'M')
                {
                    j += 1;
                    nom += string.Format("{0:MM}", fech);
                }
                else if (nomprincipal[j] == 'D')
                {
                    j += 1;
                    nom += string.Format("{0:dd}", fech);
                }
                else if (nomprincipal[j] == 'H')
                {
                    j += 1;
                    nom += string.Format("{0:HH}", fech);
                }
                else if (nomprincipal[j] == 'm')
                {
                    j += 1;
                    nom += string.Format("{0:mm}", fech);
                }
                else nom += nomprincipal[j];
            }
            nom += "." + nomext;

            return (nom);
        }
        /// <summary>
        /// Rutina que lee el formato GCF de las Guralp. Debe tenerse en cuenta que en este formato, los datos se encuentran
        /// invertidos (little endian vs. big endian. El manual no lo dice), así que hay que invertir los datos.
        /// Normalmente el visual c# tiene la instrucción array.reverse() para invertir el arreglo.
        /// Aquí se usa la notación binaria tal y como se usaba en los programas en D.O.S. (C de la Borland, versión 3.1).
        /// (Se debe leer el manual del scream, para conocer este formato). Aquí a diferencia del formato SUDS, los datos se desglosan
        /// en  archivos individuales por traza, por lo que es necesario tener un archivo adicional,
        /// donde se encuentre el nombre de las carpetas y su ruta. La variable con los datos de cuentas, es tridimensional,
        /// donde primero se le asigna el número de archivo, luego el número de bloque en el archivo y por último los datos de las cuentas.
        /// La  mayoría de las demás variables son bidimensionales, donde se les asigna primero el número de archivo
        /// y el valor de acuerdo al número de bloque.
        /// </summary>
        /// <returns></returns>
        int LeeGcf()
        {
            int[][] rat;
            int[][][] cuu;
            double[][] tii;

            int[] cug = new int[1];
            double[] timg = new double[1];

            byte[] byy = new byte[1000];
            byte[] bb = new byte[4];
            sbyte sby;
            uint dias, segu, v1, v2, v3, v4;
            ushort num, cont, nuarch = 0, nublo = 0, blo, arch = 0;
            short nulis, ide, va;
            int cc, nr, i, ii, j, k, kk, an, me, di, ho, mi, total, bl, vacio, cuvacio, traslapo, inicio, fin;
            int mu, dai, da, daf;
            long ll0, pos, lenght;
            double ti0, facrat, dd, tinicio, tifinal, tiant;
            double tiempotot = 0, tiempo0 = 0;
            string li = "", lis = "", nom = "", ca = "", noesta = "";
            bool si = false, NoExiste = false;

            short[] largo;
            int[] utg, durg;
            char[] compg, targ;
            string[] nomcarp, nomgcf, nombas, nomprincipal, nomext;
            char[] delim = { ' ', '\t' };
            string[] pa = null;
            ///
            /// Resumen: si hay año true. 
            bool[] sian;
            bool[] sime;
            bool[] sidi;
            bool[] siho;
            bool[] simi;
            string nomarch = "";
            string nomini = "";


            nomarch = ".\\pro\\" + archgcf;
            nugcf = 0;
            StreamReader ar2 = new StreamReader(nomarch);
            while (li != null)
            {
                try
                {
                    li = ar2.ReadLine();
                    if (li == null || li[0] == '*') break;
                    nugcf += 1;
                }
                catch
                {
                    break;
                }
            }
            ar2.Close();
            if (nugcf == 0)
                return (0);
            if (nugcf != yagcf.Length)
                yagcf = new bool[nugcf];
            for (i = 0; i < nugcf; i++)
                yagcf[i] = false;
            utg = new int[nugcf];
            durg = new int[nugcf];
            compg = new char[nugcf];
            targ = new char[nugcf];
            nomcarp = new string[nugcf];
            nomgcf = new string[nugcf];
            nombas = new string[nugcf];
            nomprincipal = new string[nugcf];
            largo = new short[nugcf];
            nomext = new string[nugcf];
            sian = new bool[nugcf];
            sime = new bool[nugcf];
            sidi = new bool[nugcf];
            siho = new bool[nugcf];
            simi = new bool[nugcf];

            li = "";
            i = 0;
            ca = "";

            //Aqui se abre el archivo que contiene los parametros y los nombres de carpetas
            //de las diferentes trazas en formato GCF. Cada traza tiene un archivo GCF independiente.
            StreamReader arg = new StreamReader(nomarch);
            while (li != null)
            {
                try
                {
                    li = arg.ReadLine();
                    if (li == null || li[0] == '*')
                        break;
                    pa = li.Split(delim);
                    largo[i] = (short)(pa.Length);///tamaño del string
                    if (largo[i] >= 9 && largo[i] <= 10) ///largo[i] == 9 || largo[i] == 10
                    {
                        sian[i] = false;
                        sime[i] = false;
                        sidi[i] = false;
                        nomcarp[i] = pa[0];
                        if (pa[1][0] == 'A') sian[i] = true;
                        if (pa[1][1] == 'M') sime[i] = true;
                        if (pa[1][2] == 'D') sidi[i] = true;
                        nomgcf[i] = pa[2];
                        nombas[i] = pa[3];
                        compg[i] = pa[4][0];
                        targ[i] = pa[5][0];
                        utg[i] = 3600 * int.Parse(pa[6]);
                        durg[i] = int.Parse(pa[7]);
                        if (largo[i] == 9)
                            nomext[i] = pa[8];
                        else if (largo[i] == 10)
                        {
                            nomprincipal[i] = pa[8];
                            nomext[i] = pa[9];
                        }
                        i += 1;
                    }
                }
                catch
                {
                    break;
                }
            }
            arg.Close();

            tinicio = 0;
            // nugcf, guarda el numero de estaciones o carpetas de los archivos GCF.

            for (ii = 0; ii < nugcf; ii++)
            {
                try
                {
                    si = false;
                    if (cajgcf[ii] == false || yagcf[ii] == true) continue;
                    cont = 0;
                    tiempotot = 0;
                    lis = listBox1.SelectedItem.ToString();
                    nulis = (short)(listBox1.SelectedIndex);
                    num = (ushort)(durg[ii] / 60.0);
                    an = int.Parse(lis.Substring(0, 2));
                    if (an < 88) an += 2000;
                    else an += 1900;
                    me = int.Parse(lis.Substring(3, 2));
                    di = int.Parse(lis.Substring(6, 2));
                    ho = int.Parse(lis.Substring(9, 2));
                    mi = int.Parse(lis.Substring(12, 2));
                    DateTime fech1 = new DateTime(an, me, di, ho, mi, 0);
                    ll0 = fech1.Ticks;
                    tinicio = ((double)(ll0) - Feisuds) / 10000000.0;
                    tifinal = tinicio + totven * 60.0;
                    if (utg[ii] != 0) ll0 -= (long)(utg[ii] * 10000000.0);

                    //Aqui como en las rutinas anteriores, se busca el primer archivo, a partir
                    //del cual se leen archivos hasta completar el intervalo de tiempo determinado 
                    //en el arhivo que guarda la variable nomarch.
                    for (i = 0; i <= num; i++)
                    {
                        DateTime fech2 = new DateTime(ll0);
                        nom = nomcarp[ii];
                        if (sian[ii] == true) nom += "\\" + string.Format("{0:yyyy}", fech2);
                        if (sime[ii] == true) nom += "\\" + string.Format("{0:MM}", fech2);
                        if (sidi[ii] == true) nom += "\\" + string.Format("{0:dd}", fech2);
                        nom += "\\" + nomgcf[ii] + "\\";
                        if (largo[ii] == 9) nom += string.Format("{0:MM}{0:dd}{0:HH}{0:mm}.", fech2) + nomext[ii];
                        else if (largo[ii] == 10)
                            nom += Nombre10(sian[ii], sime[ii], sidi[ii], nomprincipal[ii], nomext[ii], fech2);
                        if (File.Exists(nom))
                        {
                            si = true;
                            break;
                        }
                        ll0 -= 600000000;
                        nulis -= 1;
                    }
                    if (si == false)
                    {
                        NoMostrar = true;
                        noesta += "No hay Sismos GCF " + nomgcf[ii] + "  " + nombas[ii] + " !!!\n";
                        NoExiste = true;
                    }
                    else
                    {
                        panel2.Visible = true;
                        j = (ushort)(ii + 1);
                        ca = "Adquiriendo Trazas\n    GCF   " + j.ToString() + " " + nomgcf[ii];
                        util.Mensaje(panel2, ca, false);

                        ide = nulis;
                        if (ide < 0) continue;
                        nuarch = 0;
                        nomini = nom;

                        do
                        {
                            if (File.Exists(nom)) nuarch += 1;
                            ide += 1;
                            if (ide == listBox1.Items.Count || ide < 0) break;
                            lis = listBox1.Items[ide].ToString();
                            an = int.Parse(lis.Substring(0, 2));
                            if (an < 88) an += 2000;
                            else an += 1900;
                            me = int.Parse(lis.Substring(3, 2));
                            di = int.Parse(lis.Substring(6, 2));
                            ho = int.Parse(lis.Substring(9, 2));
                            mi = int.Parse(lis.Substring(12, 2));

                            DateTime fech3 = new DateTime(an, me, di, ho, mi, 0);
                            ll0 = fech3.Ticks;
                            if (utg[ii] != 0) ll0 -= ((long)(utg[ii] * 10000000.0));
                            DateTime fech4 = new DateTime(ll0);
                            nom = nomcarp[ii];
                            if (sian[ii] == true) nom += "\\" + string.Format("{0:yyyy}", fech4);
                            if (sime[ii] == true) nom += "\\" + string.Format("{0:MM}", fech4);
                            if (sidi[ii] == true) nom += "\\" + string.Format("{0:dd}", fech4);

                            nom += "\\" + nomgcf[ii] + "\\";
                            if (largo[ii] == 9) nom += string.Format("{0:MM}{0:dd}{0:HH}{0:mm}.", fech4) + nomext[ii];
                            else if (largo[ii] == 10) nom += Nombre10(sian[ii],
                             sime[ii], sidi[ii], nomprincipal[ii], nomext[ii], fech4);

                            cont += 1;
                            if (cont > totven) break;
                        } while (ide < listBox1.Items.Count);

                        ide = nulis;
                        nom = nomini;

                        arch = 0;
                        cuu = new int[nuarch][][];
                        tii = new double[nuarch][];
                        rat = new int[nuarch][];

                        //Aqui comienza la lectura de los archivos. Primeramente se
                        //averigua el número de bloques y se almacena en nublo.
                        do
                        {
                            if (File.Exists(nom))
                            {
                                try
                                {
                                    nublo = 0;
                                    try
                                    {
                                        FileInfo ar = new FileInfo(nom);
                                        BinaryReader br0 = new BinaryReader(ar.OpenRead());
                                        pos = 0;
                                        lenght = br0.BaseStream.Length;
                                        while (pos < lenght)
                                        {
                                            br0.ReadBytes(1024);
                                            nublo += 1;
                                            pos += 1024;
                                        }
                                        br0.Close();
                                    }
                                    catch
                                    {
                                        //MessageBox.Show("error en lectura inicial GCF!!");
                                        //continue;
                                    }
                                    cuu[arch] = new int[nublo][];
                                    tii[arch] = new double[nublo];
                                    rat[arch] = new int[nublo];

                                    FileInfo arr = new FileInfo(nom);
                                    BinaryReader br = new BinaryReader(arr.OpenRead());
                                    pos = 0;
                                    blo = 0;
                                    lenght = br.BaseStream.Length;
                                    while (pos < lenght)
                                    {
                                        //br.ReadBytes(1024); 1024 son los bytes del bloque.
                                        br.ReadBytes(8);
                                        bb = br.ReadBytes(4);
                                        v1 = bb[0];
                                        v2 = bb[1];
                                        v3 = bb[2];
                                        v4 = bb[3];
                                        segu = v2 << 31;   // aqui se voltean los bytes
                                        segu = segu >> 23;
                                        segu = segu | v3;
                                        segu = segu << 8;
                                        segu = segu | v4;
                                        dias = v1 << 8;
                                        dias = dias | v2;
                                        dias = dias >> 1;
                                        dd = Feigcf + (double)(dias) * 86400.0 + (double)(segu);
                                        ti0 = dd + utg[ii];
                                        //Ver el manual del Scream para el formato de la fecha y hora.                                        

                                        tii[arch][blo] = ti0;
                                        if (arch == 0 && blo == 0) tiempo0 = ti0;
                                        br.ReadByte();
                                        rat[arch][blo] = br.ReadByte();
                                        cc = br.ReadByte(); //compression code
                                        nr = br.ReadByte(); // Number of Records
                                        mu = cc * nr;
                                        // ver el manual en Help->Contents->GCF Specifications del Scream.
                                        tiempotot = (ti0 - tiempo0) + (mu / rat[arch][blo]);

                                        //Aqui se lee el primer dato, su valor absoluto (Hay que voltiarlo).
                                        bb = br.ReadBytes(4);
                                        v1 = bb[0];
                                        v2 = bb[1];
                                        v3 = bb[2];
                                        v4 = bb[3];
                                        dai = (int)((v1 << 24) | (v2 << 16) | (v3 << 8) | v4);
                                        da = dai;

                                        cuu[arch][blo] = new int[mu];
                                        byy = br.ReadBytes(1000);

                                        // en la variable da, se guarda el valor de la cuenta. En las variables
                                        // sby y da, se guardan las diferencias.
                                        if (cc == 4)
                                        {
                                            for (j = 0; j < mu; j++)
                                            {
                                                sby = (sbyte)(byy[j]);
                                                da += (int)(sby);
                                                cuu[arch][blo][j] = da;
                                            }
                                        }
                                        else if (cc == 2)
                                        {
                                            for (j = 0; j < mu; j++)
                                            {
                                                v1 = byy[j * 2];
                                                v2 = byy[j * 2 + 1];
                                                va = (short)((v1 << 8) | v2);
                                                da += (int)(va);
                                                cuu[arch][blo][j] = da;
                                            }
                                        }
                                        else if (cc == 1)
                                        {
                                            for (j = 0; j < mu; j++)
                                            {
                                                v1 = byy[j * 4];
                                                v2 = byy[j * 4 + 1];
                                                v3 = byy[j * 4 + 2];
                                                v4 = byy[j * 4 + 3];
                                                i = (int)((v1 << 24) | (v2 << 16) | (v3 << 8) | v4);
                                                da += i;
                                                //da += (int)((v1 << 24) | (v2 << 16) | (v3 << 8) | v4);
                                                cuu[arch][blo][j] = da;
                                            }
                                        }
                                        blo += 1;

                                        if (nr * cc == 1000)
                                        {
                                            bb = br.ReadBytes(4);
                                            v1 = bb[0];
                                            v2 = bb[1];
                                            v3 = bb[2];
                                            v4 = bb[3];
                                            daf = (int)((v1 << 24) | (v2 << 16) | (v3 << 8) | v4);
                                        }
                                        else
                                        {
                                            br.ReadBytes(4);
                                        }
                                        pos += 1024;
                                    }// while
                                    br.Close();

                                    arch += 1;
                                }
                                catch
                                {
                                }
                            }
                            ide += 1;
                            if (ide == listBox1.Items.Count || ide < 0) break;
                            lis = listBox1.Items[ide].ToString();
                            an = int.Parse(lis.Substring(0, 2));
                            if (an < 88) an += 2000;
                            else an += 1900;
                            me = int.Parse(lis.Substring(3, 2));
                            di = int.Parse(lis.Substring(6, 2));
                            ho = int.Parse(lis.Substring(9, 2));
                            mi = int.Parse(lis.Substring(12, 2));

                            DateTime fech3 = new DateTime(an, me, di, ho, mi, 0);
                            ll0 = fech3.Ticks;
                            if (utg[ii] != 0) ll0 -= ((long)(utg[ii] * 10000000.0));
                            DateTime fech4 = new DateTime(ll0);
                            nom = nomcarp[ii];
                            if (sian[ii] == true) nom += "\\" + string.Format("{0:yyyy}", fech4);
                            if (sime[ii] == true) nom += "\\" + string.Format("{0:MM}", fech4);
                            if (sidi[ii] == true) nom += "\\" + string.Format("{0:dd}", fech4);
                            nom += "\\" + nomgcf[ii] + "\\";
                            if (largo[ii] == 9) nom += string.Format("{0:MM}{0:dd}{0:HH}{0:mm}.", fech4) + nomext[ii];
                            else if (largo[ii] == 10)
                                nom += Nombre10(sian[ii], sime[ii], sidi[ii], nomprincipal[ii], nomext[ii], fech4);

                            if (tiempotot > totven * 60.0) break;
                        } while (ide < listBox1.Items.Count);

                        total = 0;
                        for (i = 0; i < arch; i++)
                        {
                            j = cuu[i].Length;
                            for (k = 0; k < j; k++)
                            {
                                total += cuu[i][k].Length;
                            }
                        }
                        // En este formato no hay informacion de ganancia. Normalmente en este formato, viene los
                        // datos de Banda Ancha, los cuales pueden ser de 1, 2 o 4 bytes, pero en la base siempre 
                        // se guardan de 4 bytes, que es su valor maximo, ya que la estructura escogida para la
                        // Base, es el formato SUDS Demultiplexado (TODOS los datos para una traza, deben tener el
                        // mismo tipo de dato).
                        // 
                        if (total > 0)
                        {
                            est[nutra] = nombas[ii] + " ";
                            ga[nutra] = 1;
                            by[nutra] = 4;// bytes de los datos
                            tar[nutra] = targ[ii];
                            comp[nutra] = compg[ii];
                            ra[nutra] = rat[0][0];
                            facrat = 1.0 / ra[nutra];

                            // aqui se averigua si hay vacios o traslapos:
                            bl = 0;
                            tiant = tii[0][0];
                            vacio = 0;
                            traslapo = 0;
                            for (i = 0; i < arch; i++)
                            {
                                for (j = 0; j < cuu[i].Length; j++) // bloques
                                {
                                    facrat = 1.0 / rat[i][j];
                                    dd = tii[i][j];
                                    if (dd > (tiant + facrat * bl)) vacio += (int)((dd - (tiant + facrat * bl)) * ra[nutra]);
                                    else if ((tiant + facrat * bl) > dd) traslapo += (int)((tiant + facrat * bl) * ra[nutra] - dd);
                                    tiant = dd;
                                    bl = cuu[i][j].Length;
                                }
                            }
                            if (vacio > 0)
                            {
                                siRoto[nutra] = true;
                                total += vacio;
                            }
                            if (traslapo > 0)
                            {
                                siTraslapo[nutra] = true;
                                total -= traslapo;
                            }

                            try
                            {
                                cug = new int[total];
                            }
                            catch
                            {
                                NoMostrar = true;
                                ca = est[nutra].Substring(0, 4) + " (cug) Tal vez se esta quedando sin memoria??\nQuedan: " + ramCounter.NextValue().ToString() + " MegaBytes en memoria\nPuede salir y reducir minutos con Param o bien Reiniciar el Programa con un valor menor en minutos.";
                                DialogResult result = MessageBox.Show(ca,
                                                                "Salir ???", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                if (result == DialogResult.Yes)
                                {
                                    cuu = null;
                                    tii = null;
                                    cug = null;
                                    timg = null;
                                    byy = null;
                                    return (-1);
                                }
                            }
                            try
                            {
                                timg = new double[total];
                            }
                            catch
                            {
                                NoMostrar = true;
                                ca = est[nutra].Substring(0, 4) + " (timg) Tal vez se esta quedando sin memoria??\nQuedan: " + ramCounter.NextValue().ToString() + " MegaBytes en memoria\nPuede salir y reducir minutos con Param o bien Reiniciar el Programa con un valor menor en minutos.";
                                DialogResult result = MessageBox.Show(ca,
                                                                "Salir ???", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                if (result == DialogResult.Yes)
                                {
                                    cuu = null;
                                    tii = null;
                                    cug = null;
                                    timg = null;
                                    byy = null;
                                    return (-1);
                                }
                            }

                            try
                            {
                                kk = 0;
                                bl = 0;
                                tiant = tii[0][0];
                                vacio = 0;
                                for (i = 0; i < arch; i++)
                                {
                                    for (j = 0; j < cuu[i].Length; j++) // bloques
                                    {
                                        facrat = 1.0 / rat[i][j];
                                        dd = tii[i][j];
                                        inicio = 0;
                                        if (siRoto[nutra] == true && kk > 0 && dd > (timg[kk - 1] + facrat))
                                        {
                                            cuvacio = cug[kk - 1];
                                            vacio = (int)((dd - timg[kk - 1]) * ra[nutra]);
                                            tiant = timg[kk - 1] + facrat;
                                            for (k = 0; k < vacio; k++)
                                            {
                                                cug[kk] = cuvacio;
                                                timg[kk] = tiant + (facrat * k);
                                                kk += 1;
                                            }
                                        }
                                        if (siTraslapo[nutra] == true && kk > 0 && dd < (timg[kk - 1] + facrat))
                                        {
                                            traslapo = (int)((timg[kk - 1] - dd) * ra[nutra]);
                                            inicio = traslapo;
                                        }

                                        for (k = inicio; k < cuu[i][j].Length; k++)
                                        {
                                            cug[kk] = cuu[i][j][k];
                                            timg[kk] = dd + (facrat * k);
                                            kk += 1;
                                        }
                                    }
                                }
                            }
                            catch
                            {
                                continue;
                            }

                            inicio = (int)((tinicio - timg[0]) * ra[nutra]);
                            if (inicio < 0) inicio = 0;
                            if (timg[total - 1] - tifinal < 0) fin = total;
                            else fin = (int)(total - ((timg[total - 1] - tifinal) * ra[nutra]));
                            if (inicio >= fin) continue;
                            i = fin - inicio;
                            try
                            {
                                tim[nutra] = new double[i];
                            }
                            catch
                            {
                                NoMostrar = true;
                                ca = est[nutra].Substring(0, 4) + " (tim) Tal vez se esta quedando sin memoria??\nQuedan: " + ramCounter.NextValue().ToString() + " MegaBytes en memoria\nPuede salir y reducir minutos con Param o bien Reiniciar el Programa con un valor menor en minutos.";
                                DialogResult result = MessageBox.Show(ca,
                                                                "Salir ???", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                if (result == DialogResult.Yes)
                                {
                                    cuu = null;
                                    tii = null;
                                    cug = null;
                                    timg = null;
                                    byy = null;
                                    return (-1);
                                }
                            }
                            try
                            {
                                cu[nutra] = new int[i];
                            }
                            catch
                            {
                                NoMostrar = true;
                                ca = est[nutra].Substring(0, 4) + " (cu) Tal vez se esta quedando sin memoria??\nQuedan: " + ramCounter.NextValue().ToString() + " MegaBytes en memoria\nPuede salir y reducir minutos con Param o bien Reiniciar el Programa con un valor menor en minutos.";
                                DialogResult result = MessageBox.Show(ca,
                                                                "Salir ???", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                if (result == DialogResult.Yes)
                                {
                                    cuu = null;
                                    tii = null;
                                    cug = null;
                                    timg = null;
                                    byy = null;
                                    return (-1);
                                }
                            }
                            kk = 0;
                            // cu y tim son las variables generales para las cuentas y el tiempo.
                            for (j = inicio; j < fin; j++)
                            {
                                cu[nutra][kk] = cug[j];
                                tim[nutra][kk++] = timg[j];
                            }
                            nutra += 1;
                        }
                    }
                }
                catch
                {
                }
                yagcf[ii] = true;
            }// ii: el numero de estaciones GCF   

            panel2.Visible = false;

            if (NoExiste == true) MessageBox.Show(noesta);

            return (1);
        }
        /// <summary>
        /// Rutina que lee el formato SEISAN. La variable con los datos de cuentas, es tridimensional,
        /// donde primero se le asigna el número de archivo, luego el número de bloque en el archivo y por último los datos de las cuentas.
        /// La mayoría de las demás variables son bidimensionales, donde se les asigna primero el número de archivo y el
        /// valor de acuerdo al número de bloque.
        /// </summary>
        void LeeSeisan()
        {
            short[][] bby;
            int[][][] cuu;
            char[][][] cc;
            double[][] tii;
            double[][] rat;
            string[][] esta;
            string[] estt = new string[Ma];
            char[][] compo;
            short[] tipbyte;
            int[] cus;
            double[] tims;

            byte[] bit2 = new byte[2];
            byte[] bit4 = new byte[4];
            byte[] byy = new byte[1000];
            ushort num, cont, cont2, nuarch = 0, arch;
            short nulis, ide, nuseis;
            int i, ii, iii, j, k, kk, kkk, an, me, di, ho, mi, vacio, cuvacio;
            int an2, me2, di2, ho2, mi2, se2, ms2, nuto, nuletra;
            int adicion = 0;
            double seg;
            int traslapo, inicio, jj, fin = 0, totbyt = 0;
            int largo, lara;
            long ll0, ll;
            double tinicio, tifinal, tie, facra, dd1, dd2;
            string li = "", lis = "", nom = "", ca = "", ca2 = "", le = "";
            bool si = false;

            int[] uts, durs, pos, numu, canal;
            char[] tars;
            string[] nomcarp, nomext;
            char[] delim = { ' ', '\t' };
            string[] pa = null;
            bool[] sian, sime, sidi, siho, simi, UNIX;
            string nomarch = "", nomini = "";
            ArrayList listasei = new ArrayList();


            if (cajseis == false) return;
            if (File.Exists(".\\pro\\estaseisan.txt"))
            {
                StreamReader arsei = new StreamReader(".\\pro\\estaseisan.txt");
                while (li != null)
                {
                    try
                    {
                        li = arsei.ReadLine();
                        if (li == null) break;
                        if (char.IsLetterOrDigit(li[0]) && li.Length >= 9) listasei.Add(li);
                    }
                    catch
                    {
                        break;
                    }
                }
                arsei.Close();
            }
            li = "";
            nomarch = ".\\pro\\" + archseis;
            if (!File.Exists(nomarch))
            {
                NoMostrar = true;
                MessageBox.Show("NO EXISTE en PRO el archivo con los datos de las trazas en SEISAN!!");
                return;
            }
            nuseis = 0;
            StreamReader ar2 = new StreamReader(nomarch);
            while (li != null)
            {
                try
                {
                    li = ar2.ReadLine();
                    if (li == null || li[0] == '*') break;
                    nuseis += 1;
                }
                catch
                {
                    break;
                }
            }
            ar2.Close();
            if (nuseis == 0)
            {
                NoMostrar = true;
                MessageBox.Show("Archivo SEISAN vacio?");
                return;
            }
            uts = new int[nuseis];
            durs = new int[nuseis];
            tars = new char[nuseis];
            nomcarp = new string[nuseis];
            nomext = new string[nuseis];
            sian = new bool[nuseis];
            sime = new bool[nuseis];
            sidi = new bool[nuseis];
            siho = new bool[nuseis];
            simi = new bool[nuseis];
            UNIX = new bool[nuseis];
            for (i = 0; i < nuseis; i++) UNIX[i] = false;
            li = "";
            i = 0;
            StreamReader arg = new StreamReader(nomarch);
            while (li != null)
            {
                try
                {
                    li = arg.ReadLine();
                    if (li == null || li[0] == '*') break;
                    pa = li.Split(delim);
                    largo = pa.Length;
                    if (largo >= 6)
                    {
                        sian[i] = false;
                        sime[i] = false;
                        sidi[i] = false;
                        nomcarp[i] = pa[0];
                        if (pa[1][0] == 'A') sian[i] = true;
                        if (pa[1][1] == 'M') sime[i] = true;
                        if (pa[1][2] == 'D') sidi[i] = true;
                        tars[i] = pa[2][0];
                        uts[i] = 3600 * int.Parse(pa[3]);
                        durs[i] = int.Parse(pa[4]);
                        nomext[i] = pa[5];
                        if (largo == 7)
                        {
                            if (pa[6] == "UNIX") UNIX[i] = true;
                        }
                        i += 1;
                    }
                    else
                    {
                        NoMostrar = true;
                        MessageBox.Show("Revise " + nomarch + ". Tal vez hay espacios adicionales??");
                    }
                }
                catch
                {
                    break;
                }
            }
            arg.Close();

            tinicio = 0;
            // nuseis, guarda el numero de estaciones o carpetas de los archivos SEISAN.
            for (ii = 0; ii < nuseis; ii++)
            {
                //i = (ushort)(1 + (totven * 60.0) / durs[ii]);
                cont = 0;
                lis = listBox1.SelectedItem.ToString();
                nulis = (short)(listBox1.SelectedIndex);
                num = (ushort)(durs[ii] / 60.0);
                an = int.Parse(lis.Substring(0, 2));
                if (an < 88) an += 2000;
                else an += 1900;
                me = int.Parse(lis.Substring(3, 2));
                di = int.Parse(lis.Substring(6, 2));
                ho = int.Parse(lis.Substring(9, 2));
                mi = int.Parse(lis.Substring(12, 2));
                DateTime fech1 = new DateTime(an, me, di, ho, mi, 0);
                ll0 = fech1.Ticks;
                tinicio = ((double)(ll0) - Feisuds) / 10000000.0;
                tifinal = tinicio + totven * 60.0;
                if (uts[ii] != 0) ll0 -= (long)(uts[ii] * 10000000.0);

                for (i = 0; i <= num; i++)
                {
                    DateTime fech2 = new DateTime(ll0);
                    ca = nomcarp[ii];
                    if (sian[ii] == true) ca += "\\" + string.Format("{0:yyyy}", fech2);
                    if (sime[ii] == true) ca += "\\" + string.Format("{0:MM}", fech2);
                    if (sidi[ii] == true) ca += "\\" + string.Format("{0:dd}", fech2);
                    if (!Directory.Exists(ca)) continue;
                    DirectoryInfo dir = new DirectoryInfo(ca);
                    le = "*." + nomext[ii];
                    FileInfo[] nn = dir.GetFiles(le);
                    le = string.Format("{0:yyyy}-{0:MM}-{0:dd}-{0:HH}{0:mm}", fech2);
                    nom = "\0";
                    for (jj = 0; jj < nn.Length; jj++)
                    {
                        if (string.Compare(nn[jj].Name.Substring(0, 15), le.Substring(0, 15)) == 0)
                        {
                            nom = ca += "\\" + nn[jj].Name;
                            break;
                        }
                    }
                    if (File.Exists(nom))
                    {
                        //MessageBox.Show("SI EXISTE!!!");
                        si = true;
                        break;
                    }
                    //else MessageBox.Show(" ca="+ca+" le="+le+" largo="+nn.Length.ToString()+"\nnom="+nom);
                    ll0 -= 600000000;
                    nulis -= 1;
                }
                if (si == false)
                {
                    NoMostrar = true;
                    MessageBox.Show("No hay Sismos SEISAN " + (ii + 1).ToString() + " !!!");
                    // return;
                }
                else
                {
                    panel2.Visible = true;
                    j = (ushort)(ii + 1);
                    ca = "Adquiriendo Trazas\n    SEISAN   " + j.ToString();
                    util.Mensaje(panel2, ca, false);
                    ide = nulis;
                    if (ide < 0) continue;
                    nuarch = 0;
                    nomini = nom;
                    do
                    {
                        if (File.Exists(nom)) nuarch += 1;
                        ide += 1;
                        if (ide == listBox1.Items.Count || ide < 0) break;
                        lis = listBox1.Items[ide].ToString();
                        an = int.Parse(lis.Substring(0, 2));
                        if (an < 88) an += 2000;
                        else an += 1900;
                        me = int.Parse(lis.Substring(3, 2));
                        di = int.Parse(lis.Substring(6, 2));
                        ho = int.Parse(lis.Substring(9, 2));
                        mi = int.Parse(lis.Substring(12, 2));
                        DateTime fech3 = new DateTime(an, me, di, ho, mi, 0);
                        ll0 = fech3.Ticks;
                        if (uts[ii] != 0) ll0 -= ((long)(uts[ii] * 10000000.0));
                        DateTime fech4 = new DateTime(ll0);
                        ca = nomcarp[ii];
                        if (sian[ii] == true) ca += "\\" + string.Format("{0:yyyy}", fech4);
                        if (sime[ii] == true) ca += "\\" + string.Format("{0:MM}", fech4);
                        if (sidi[ii] == true) ca += "\\" + string.Format("{0:dd}", fech4);
                        DirectoryInfo dir = new DirectoryInfo(ca);
                        le = "*." + nomext[ii];
                        FileInfo[] nn = dir.GetFiles(le);
                        le = string.Format("{0:yyyy}-{0:MM}-{0:dd}-{0:HH}{0:mm}", fech4);
                        nom = "\0";
                        for (jj = 0; jj < nn.Length; jj++)
                        {
                            if (string.Compare(nn[jj].Name.Substring(0, 15), le.Substring(0, 15)) == 0)
                            {
                                nom = ca += "\\" + nn[jj].Name;
                                break;
                            }
                        }

                        cont += 1;
                        if (cont > totven) break;
                    } while (ide < listBox1.Items.Count);

                    ide = nulis;
                    nom = nomini;
                    cont2 = 0;
                    arch = 0;
                    cuu = new int[nuarch][][];
                    cc = new char[nuarch][][];
                    tii = new double[nuarch][];
                    rat = new double[nuarch][];
                    esta = new string[nuarch][];
                    compo = new char[nuarch][];
                    bby = new short[nuarch][];
                    numu = new int[1];
                    pos = new int[1];
                    tipbyte = new short[1];
                    canal = new int[nuarch];

                    do
                    {
                        if (File.Exists(nom))
                        {
                            try
                            {
                                FileInfo ar = new FileInfo(nom);
                                BinaryReader br0 = new BinaryReader(ar.OpenRead());
                                while (br0.PeekChar() != -1)
                                {
                                    br0.ReadBytes(34);
                                    //br0.ReadBytes(32); // reftek
                                    ca = Encoding.ASCII.GetString(br0.ReadBytes(3));
                                    canal[arch] = int.Parse(ca.Substring(0, 3));
                                    numu = new int[canal[arch]];
                                    tipbyte = new short[canal[arch]];
                                    if (canal[arch] > 12) fin = canal[arch];
                                    else fin = 12;
                                    br0.ReadBytes(143);
                                    do
                                    {
                                        ca = Encoding.ASCII.GetString(br0.ReadBytes(88));
                                        if (char.IsLetter(ca[0])) break;
                                    } while (!char.IsLetter(ca[0]));

                                    for (jj = 0; jj < canal[arch]; jj++)
                                    {
                                        numu[jj] = int.Parse(ca.Substring(43, 7));
                                        if (ca[76] == '4') tipbyte[jj] = 4;
                                        else tipbyte[jj] = 2;
                                        try
                                        {
                                            br0.ReadBytes(960);
                                        }
                                        catch
                                        {
                                        }
                                        totbyt = numu[jj] * tipbyte[jj];
                                        br0.ReadBytes(totbyt);
                                        if (jj == canal[arch] - 1) break;
                                        br0.ReadBytes(8);
                                        ca = Encoding.ASCII.GetString(br0.ReadBytes(88));
                                        le = ca.Substring(0, 6);
                                    }
                                    break;// provi
                                }// while
                                br0.Close();

                                cuu[arch] = new int[canal[arch]][];
                                cc[arch] = new char[canal[arch]][];
                                for (iii = 0; iii < canal[arch]; iii++)
                                {
                                    cuu[arch][iii] = new int[numu[iii]];
                                    cc[arch][iii] = new char[3];
                                }
                                tii[arch] = new double[canal[arch]];
                                rat[arch] = new double[canal[arch]];
                                esta[arch] = new string[canal[arch]];
                                compo[arch] = new char[canal[arch]];
                                bby[arch] = new short[canal[arch]];

                                BinaryReader br = new BinaryReader(ar.OpenRead());

                                try
                                {
                                    br.ReadBytes(1060);
                                    if (fin > 30)
                                    {
                                        adicion = 88 * (int)(Math.Ceiling((fin - 30.0) / 3.0));
                                        br.ReadBytes(adicion);
                                    }
                                }
                                catch
                                {
                                }

                                for (iii = 0; iii < canal[arch]; iii++)
                                {
                                    ca2 = Encoding.ASCII.GetString(br.ReadBytes(88));
                                    if (char.IsLetter(ca2[8])) nuletra = 8;
                                    else nuletra = 7;
                                    esta[arch][iii] = ca2.Substring(0, 3) + ca2.Substring(nuletra, 1);
                                    if (ca2[8] == 'N') compo[arch][iii] = 'n';
                                    else if (ca2[8] == 'E') compo[arch][iii] = 'e';
                                    else compo[arch][iii] = 'z';
                                    an2 = int.Parse(ca2.Substring(10, 2));
                                    if (ca2[9] == '1') an2 += 2000;
                                    else an2 += 1900;
                                    me2 = int.Parse(ca2.Substring(17, 2));
                                    di2 = int.Parse(ca2.Substring(20, 2));
                                    ho2 = int.Parse(ca2.Substring(23, 2));
                                    mi2 = int.Parse(ca2.Substring(26, 2));
                                    seg = double.Parse(ca2.Substring(29, 6));
                                    se2 = (int)(seg);
                                    ms2 = (int)((double)(seg - se2) * 1000.0);
                                    DateTime fee1 = new DateTime(an2, me2, di2, ho2, mi2, se2, ms2);
                                    ll = fee1.ToBinary();
                                    tii[arch][iii] = ((double)(ll) - Feisuds) / 10000000.0;
                                    rat[arch][iii] = double.Parse(ca2.Substring(36, 6));
                                    bby[arch][iii] = tipbyte[iii];
                                    try
                                    {
                                        br.ReadBytes(960);
                                    }
                                    catch
                                    {
                                    }
                                    if (tipbyte[iii] == 4)
                                    {
                                        if (UNIX[ii] == false)
                                        {
                                            for (kkk = 0; kkk < numu[iii]; kkk++)
                                                cuu[arch][iii][kkk] = br.ReadInt32();
                                        }
                                        else
                                        {
                                            for (kkk = 0; kkk < numu[iii]; kkk++)
                                            {
                                                bit4 = br.ReadBytes(4);
                                                Array.Reverse(bit4, 0, 4);
                                                cuu[arch][iii][kkk] = BitConverter.ToInt32(bit4, 0);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (UNIX[ii] == false)
                                        {
                                            for (kkk = 0; kkk < numu[iii]; kkk++)
                                                cuu[arch][iii][kkk] = (int)(br.ReadInt16());
                                        }
                                        else
                                        {
                                            for (kkk = 0; kkk < numu[iii]; kkk++)
                                            {
                                                bit2 = br.ReadBytes(2);
                                                Array.Reverse(bit2, 0, 2);
                                                cuu[arch][iii][kkk] = (int)(BitConverter.ToInt16(bit2, 0));
                                            }
                                        }
                                    }
                                    if (cuu[arch][iii][numu[iii] - 1] == 0)
                                    {
                                        int xxx = cuu[arch][iii][numu[iii] - 50];
                                        int mmm = xxx;
                                        for (kkk = numu[iii] - 49; kkk < numu[iii] - 1; kkk++)
                                        {
                                            if (xxx < cuu[arch][iii][kkk]) xxx = cuu[arch][iii][kkk];
                                            else if (mmm > cuu[arch][iii][kkk]) mmm = cuu[arch][iii][kkk];
                                        }
                                        int diff1 = cuu[arch][iii][numu[iii] - 1] - cuu[arch][iii][numu[iii] - 2];
                                        int diff2 = xxx - mmm;
                                        if (Math.Abs(Math.Abs(diff1) - Math.Abs(diff2)) > 60000.0) cuu[arch][iii][numu[iii] - 1] = cuu[arch][iii][numu[iii] - 2];
                                        //MessageBox.Show(esta[arch][iii]+" xxx="+xxx.ToString()+" mmm="+mmm.ToString()+" dif="+(xxx-mmm).ToString()+"    "+Math.Abs(Math.Abs(diff1)-Math.Abs(diff2)).ToString());
                                    }
                                    br.ReadBytes(8);
                                }

                                br.Close();
                                arch += 1;
                            }
                            catch
                            {
                                NoMostrar = true;
                                MessageBox.Show("*** ERROR!! ( quizas mas de 30 estaciones?? )");
                            }
                        }

                        //MessageBox.Show("arch-1="+arch.ToString()+" esta="+esta[arch-1][0]);

                        ide += 1;
                        if (ide == listBox1.Items.Count || ide < 0) break;
                        lis = listBox1.Items[ide].ToString();
                        an = int.Parse(lis.Substring(0, 2));
                        if (an < 88) an += 2000;
                        else an += 1900;
                        me = int.Parse(lis.Substring(3, 2));
                        di = int.Parse(lis.Substring(6, 2));
                        ho = int.Parse(lis.Substring(9, 2));
                        mi = int.Parse(lis.Substring(12, 2));
                        DateTime fech3 = new DateTime(an, me, di, ho, mi, 0);
                        ll0 = fech3.Ticks;
                        if (uts[ii] != 0) ll0 -= ((long)(uts[ii] * 10000000.0));
                        DateTime fech4 = new DateTime(ll0);
                        ca = nomcarp[ii];
                        if (sian[ii] == true) ca += "\\" + string.Format("{0:yyyy}", fech4);
                        if (sime[ii] == true) ca += "\\" + string.Format("{0:MM}", fech4);
                        if (sidi[ii] == true) ca += "\\" + string.Format("{0:dd}", fech4);
                        DirectoryInfo dir = new DirectoryInfo(ca);
                        le = "*." + nomext[ii];
                        FileInfo[] nn = dir.GetFiles(le);
                        le = string.Format("{0:yyyy}-{0:MM}-{0:dd}-{0:HH}{0:mm}", fech4);
                        nom = "\0";
                        for (jj = 0; jj < nn.Length; jj++)
                        {
                            if (string.Compare(nn[jj].Name.Substring(0, 15), le.Substring(0, 15)) == 0)
                            {
                                nom = ca += "\\" + nn[jj].Name;
                                break;
                            }
                        }
                        cont2 += 1;
                        if (cont2 > totven) break;
                    } while (ide < listBox1.Items.Count);

                    // En este formato no hay informacion de ganancia. 

                    // aqui se asigna secuencialmente el nombre de las estaciones a la variable estt. La variable nuto, 
                    //lleva la cuenta de las trazas.
                    // MessageBox.Show("arch="+arch.ToString());
                    if (arch > 0)
                    {
                        nuto = 0;
                        for (i = 0; i < arch; i++)
                        {
                            si = false;
                            for (j = 0; j < canal[i]; j++)
                            {
                                if (nuto > 0)
                                {
                                    for (k = 0; k < nuto; k++)
                                    {
                                        if (string.Compare(estt[k].Substring(0, 4), esta[i][j].Substring(0, 4)) == 0 || esta[i][j].Substring(0, 4) == "XXXX")
                                        {
                                            si = true;
                                            break;
                                        }
                                    }
                                }
                                if (si == false)
                                {
                                    estt[nuto] = esta[i][j];
                                    ga[nutra + nuto] = 1;
                                    comp[nutra + nuto] = compo[i][j];
                                    nuto += 1;
                                }
                                else si = false;
                            }
                        }

                        // aqui se van a asignar los datos a cada estacion sucesivamente.
                        for (iii = 0; iii < nuto; iii++)
                        {
                            lara = 0;
                            tie = 0;
                            facra = 0;
                            vacio = 0;
                            traslapo = 0;
                            dd2 = 0;
                            for (i = 0; i < arch; i++)
                            {
                                for (j = 0; j < canal[i]; j++)
                                {
                                    if (estt[iii].Substring(0, 4) == esta[i][j].Substring(0, 4))
                                    {
                                        if (lara > 0)
                                        {
                                            dd1 = tii[i][j];
                                            if (dd1 - dd2 > (facra + 0.005) && dd2 > 0)
                                            {
                                                siRoto[nutra] = true;
                                                vacio += (int)((dd1 - dd2) * rat[i][j]);
                                            }
                                            else if (dd2 - dd1 > (facra + 0.005) && dd2 > 0)
                                            {
                                                siTraslapo[nutra] = true;
                                                traslapo += (int)((dd2 - dd1) * rat[i][j]);
                                            }
                                        }
                                        lara += cuu[i][j].Length;
                                        facra = 1.0 / (rat[i][j]);
                                        dd2 = tii[i][j] + facra * cuu[i][j].Length;
                                        break;
                                    }
                                }
                            }// for i....

                            if (lara > 0)
                            {
                                lara += vacio;
                                lara -= traslapo;
                                si = false;
                                if (listasei.Count > 0)
                                {
                                    for (jj = 0; jj < listasei.Count; jj++)
                                    {
                                        if (listasei[jj].ToString().Substring(0, 4) == estt[iii].Substring(0, 4))
                                        {
                                            est[nutra] = listasei[jj].ToString().Substring(5, 4) + " ";
                                            si = true;
                                            break;
                                        }
                                    }
                                }
                                if (si == false) est[nutra] = estt[iii].Substring(0, 4) + " ";
                                cus = new int[lara];
                                tims = new double[lara];
                                siEst[nutra] = true;
                                tar[nutra] = tars[ii];
                                //by[nutra] = bby[0][0];// se supone que es multiplexado y todos los datos tienen la misma caracteristica (rata, tipodato, etc!!

                                jj = 0;
                                dd2 = 0;
                                cuvacio = 0;
                                for (i = 0; i < arch; i++)
                                {
                                    for (j = 0; j < canal[i]; j++)
                                    {
                                        //MessageBox.Show("i=" + i.ToString() + " j=" + j.ToString() + " estt=" + estt[iii] + "esta=" + esta[i][j]);
                                        if (estt[iii].Substring(0, 4) == esta[i][j].Substring(0, 4))
                                        {
                                            ra[nutra] = rat[i][j];// se puede mejorar!!
                                            by[nutra] = bby[i][j];
                                            tie = tii[i][j];
                                            facra = 1.0 / (rat[i][j]);
                                            inicio = 0;
                                            if (tie - dd2 > (facra + 0.005) && dd2 > 0)
                                            {
                                                vacio = (int)((tie - dd2) * rat[i][j]);
                                                dd2 += facra;
                                                for (kk = 0; kk < vacio; kk++)
                                                {
                                                    //tim[nutra][jj] = dd2 + kk * facra;
                                                    //cu[nutra][jj] = cuvacio;
                                                    tims[jj] = dd2 + kk * facra;
                                                    cus[jj] = cuvacio;
                                                    jj += 1;
                                                }
                                            }
                                            else if (dd2 - tie > (facra + 0.005) && dd2 > 0)
                                            {
                                                traslapo = (int)((dd2 - tie) * rat[i][j]);
                                                inicio = traslapo;
                                            }
                                            for (kk = inicio; kk < cuu[i][j].Length; kk++)
                                            {
                                                tims[jj] = tie + kk * facra;
                                                cus[jj] = cuu[i][j][kk];
                                                jj += 1;
                                            }
                                            facra = 1.0 / (rat[i][j]);
                                            dd2 = tii[i][j] + facra * cuu[i][j].Length;
                                            cuvacio = cus[jj - 1];
                                            break;
                                        }
                                    }
                                }
                                inicio = (int)((tinicio - tims[0]) * ra[nutra]);
                                if (inicio < 0) inicio = 0;
                                if (tims[lara - 1] - tifinal < 0) fin = lara;
                                else fin = (int)(lara - ((tims[lara - 1] - tifinal) * ra[nutra]));
                                if (inicio >= fin) continue;
                                i = fin - inicio;
                                tim[nutra] = new double[i];
                                cu[nutra] = new int[i];
                                kk = 0;
                                for (j = inicio; j < fin; j++)
                                {
                                    cu[nutra][kk] = cus[j];
                                    tim[nutra][kk++] = tims[j];
                                }
                                nutra += 1;
                            } // if lara>0
                        } // iii
                    }
                }
            }// ii: el numero de archivos SEISAN   

            panel2.Visible = false;

            return;
        }
        /// <summary>
        /// Este formato corresponde a las Oriones de la Kinemetrics. Los datos se encuentran desglozados
        /// por estaciones con carpetas diferentes. No se pudo contar con suficientes datos en este formato,
        /// por lo que se sigue la costumbre de la RSNC, donde los datos se encuentran por HORAS, comenzando
        /// en la hora exacta. En versiones posteriores debe mejorarse esta rutina.
        /// </summary>
        void LeeYfile() // provisional. se presume sismos por hora y comienzan en la hora exacta.
        {
            /*
             * Este formato corresponde a las Oriones de la Kinemetrics. Los datos se encuentran desglozados
             * por estaciones con carpetas diferentes. No se pudo contar con suficientes datos en este formato,
             * por lo que se sigue la costumbre de la RSNC, donde los datos se encuentran por HORAS, comenzando
             * en la hora exacta. En versiones posteriores debe mejorarse esta rutina.
            */
            float[] rat;
            int[][] cuu;
            double[][] tii;
            uint[] blo;

            byte[] byy = new byte[1000];
            ushort cuenta, num;
            short nulis;
            int i, ii, j, jj, an, me, di, ho, mi;
            int total;
            long ll0, ll1, ll;
            double facrat;
            string lis = "", nom = "", ca = "", nomca = "", nomest = "", loc = "", chan = "";

            ushort estru;
            short nuy;
            uint lar = 0, totmu = 0, inimu = 0, iii, k, kk;
            int larest, lardat, largo;
            float rata;
            double timp, initi, finti, fti, ti1, difti;
            string li = "", nomarch = "";
            int[] uty, dury;
            char[] compy, tary;
            string[] nomyfi, nombas, nom1, nom2, nomext;
            char[] delim = { ' ', '\t' };
            string[] pa = null;
            bool[] sian, sime, sidi, siho, simi;

            nomarch = ".\\pro\\" + archyfi;
            nuy = 0;
            StreamReader ar2 = new StreamReader(nomarch);
            while (li != null)
            {
                try
                {
                    li = ar2.ReadLine();
                    if (li == null || li[0] == '*') break;
                    nuy += 1;
                }
                catch
                {
                    break;
                }
            }
            ar2.Close();
            if (nuy == 0) return;
            uty = new int[nuy];
            dury = new int[nuy];
            compy = new char[nuy];
            tary = new char[nuy];
            nomyfi = new string[nuy];
            nombas = new string[nuy];
            nom1 = new string[nuy];
            nom2 = new string[nuy];
            nomext = new string[nuy];
            sian = new bool[nuy];
            sime = new bool[nuy];
            sidi = new bool[nuy];
            siho = new bool[nuy];
            simi = new bool[nuy];

            li = "";
            j = 0;
            StreamReader ar = new StreamReader(nomarch);
            while (li != null)
            {
                try
                {
                    li = ar.ReadLine();
                    if (li == null || li[0] == '*') break;
                    pa = li.Split(delim);
                    largo = pa.Length;
                    for (ii = 0; ii < largo; ii++)
                        nomyfi[j] = pa[0];
                    nombas[j] = pa[1];
                    compy[j] = pa[2][0];
                    tary[j] = pa[3][0];
                    uty[j] = 3600 * int.Parse(pa[4]);
                    dury[j] = int.Parse(pa[5]);
                    if (largo >= 9)
                    {
                        nom1[j] = pa[6];
                        if (pa[7][0] == 'A') sian[j] = true;
                        else sian[j] = false;
                        if (pa[7][1] == 'M') sime[j] = true;
                        else sime[j] = false;
                        if (pa[7][2] == 'D') sidi[j] = true;
                        else sidi[j] = false;
                        nom2[j] = pa[7];
                        nomext[j] = pa[8];
                    }
                    else
                    {
                        nom1[j] = "";
                        nom2[j] = "";
                        nomext[j] = pa[6];
                    }
                    j += 1;
                }
                catch
                {
                    break;
                }
            }
            ar.Close();

            panel2.Visible = true;
            for (ii = 0; ii < nuy; ii++)
            {
                if (cajyfile[ii] == false) continue;
                cuenta = (ushort)((totven * 60.0) / dury[ii]);
                total = totven * 60;

                lis = listBox1.SelectedItem.ToString();
                nulis = (short)(listBox1.SelectedIndex);
                an = int.Parse(lis.Substring(0, 2));
                if (an < 88) an += 2000;
                else an += 1900;
                me = int.Parse(lis.Substring(3, 2));
                di = int.Parse(lis.Substring(6, 2));
                ho = int.Parse(lis.Substring(9, 2));
                mi = 0;
                DateTime fech1 = new DateTime(an, me, di, ho, mi, 0);
                ll0 = fech1.Ticks;
                if (uty[ii] != 0) ll0 -= (long)(uty[ii] * 10000000.0);
                mi = int.Parse(lis.Substring(12, 2));
                DateTime fechll1 = new DateTime(an, me, di, ho, mi, 0);
                ll1 = fechll1.Ticks;
                initi = ((double)(ll1) - Feisuds) / 10000000.0;
                finti = initi + totven * 60.0;
                if (uty[ii] != 0) ll1 -= (long)(uty[ii] * 10000000.0);
                ll = dury[ii] - (long)((ll1 - ll0) / 10000000.0);
                num = 1;
                while (ll < total)
                {
                    ll += dury[ii];
                    num += 1;
                }
                rat = new float[num];
                cuu = new int[num][];
                tii = new double[num][];
                blo = new uint[num];
                blo[0] = 0;
                //MessageBox.Show("ll=" + ll.ToString()+" mi="+mi.ToString()+" total="+total.ToString()+" j="+j.ToString());
                for (j = 0; j < num; j++)
                {
                    DateTime fech2 = new DateTime(ll0);
                    nomca = nom1[ii];
                    if (sian[ii] == true) nomca += string.Format("{0:yyyy}", fech2);
                    if (sime[ii] == true) nomca += string.Format("{0:MM}", fech2);
                    if (sidi[ii] == true) nomca += string.Format("{0:dd}", fech2);
                    nomca += "." + string.Format("{0:HH}", fech2) + "0000";
                    nom = nomyfi[ii] + "\\" + nomca;
                    ll = dury[ii];
                    ll0 += (long)(ll * 10000000);
                    if (!File.Exists(nom))
                    {
                        ca = "NO EXISTE\n" + nomca;
                        util.Mensaje(panel2, ca, false);
                        blo[j] = 0;
                        rat[j] = 0;
                        continue;
                    }

                    FileInfo yf = new FileInfo(nom);
                    BinaryReader br = new BinaryReader(yf.OpenRead());

                    while (br.PeekChar() != -1)
                    {
                        br.ReadBytes(2);
                        estru = br.ReadUInt16();
                        larest = br.ReadInt32();
                        lardat = br.ReadInt32();
                        br.ReadInt32();
                        if (estru > 0)
                        {
                            if (estru == 1)
                            {
                                br.ReadBytes(8);
                                nomest = Encoding.ASCII.GetString(br.ReadBytes(5));
                                loc = Encoding.ASCII.GetString(br.ReadBytes(2));
                                chan = Encoding.ASCII.GetString(br.ReadBytes(3));
                                nomca = nomest + loc + chan;
                                br.ReadBytes(201);
                                //MessageBox.Show(nomca+"1 nomest="+nomest+" loc="+loc+" chan="+chan);
                            }
                            else if (estru == 3)
                            {
                                br.ReadBytes(40);
                                //br.ReadBytes(16);  
                                //sti = br.ReadDouble(); // sensibilidad del sensor
                                //fti = br.ReadDouble(); // frecuencia de medicion de la sensibilidad
                                //br.ReadBytes(8);
                                rat[j] = br.ReadSingle();
                                br.ReadBytes(84);
                            }
                            else if (estru == 5)
                            {
                                br.ReadBytes(16);
                                timp = br.ReadDouble() + (double)(uty[ii]);
                                fti = br.ReadDouble() + (double)(uty[ii]);
                                difti = initi - timp;
                                lar = br.ReadUInt32();
                                totmu = 0;
                                ti1 = 0;
                                inimu = 0;
                                if (timp > finti) break;
                                else if (timp < initi)
                                {
                                    ti1 = initi;
                                    inimu = (uint)((initi - timp) * rat[j]);
                                    if (fti < finti) totmu = (uint)((fti - initi) * rat[j]);
                                    else totmu = (uint)((finti - initi) * rat[j]);
                                    //else 
                                }
                                else if (timp >= initi)
                                {
                                    ti1 = timp;
                                    inimu = 0;
                                    if (fti < finti) totmu = (uint)((fti - timp) * rat[j]);
                                    else totmu = (uint)((finti - timp) * rat[j]);
                                }
                                blo[j] = totmu;
                                tii[j] = new double[totmu];
                                tii[j][0] = ti1;
                                cuu[j] = new int[totmu];
                                //ofs = br.ReadInt32();
                                br.ReadBytes(4);
                                br.ReadInt32();//provi
                                br.ReadInt32();//provi
                                //  amx = br.ReadInt32();
                                //  amn = br.ReadInt32();
                                //MessageBox.Show("amx=" + amx.ToString() + " amn=" + amn.ToString());
                                br.ReadBytes(16);
                            }
                            else if (estru == 7)
                            {
                                if (inimu > 0) br.ReadBytes((int)(inimu * 4));
                                for (iii = 0; iii < totmu; iii++)
                                {
                                    cuu[j][iii] = br.ReadInt32();
                                }
                                facrat = 1.0 / rat[j];
                                for (iii = 1; iii < totmu; iii++) tii[j][iii] = tii[j][iii - 1] + facrat;
                                break;
                            }
                            else
                            {
                                br.ReadBytes(larest);
                                //if (lardat > 0) br.ReadBytes(lardat);
                            }
                        } // if estru>0
                    } // while                      

                    br.Close();
                }  // for j....
                lar = 0;
                rata = 0;
                for (j = 0; j < num; j++)
                {
                    if (rat[j] > 0) rata = rat[j];
                    if (blo[j] > 0) lar += blo[j];
                }

                if (lar > 0 && rata > 0)
                {
                    facrat = 1.0 / rata;
                    est[nutra] = nombas[ii] + " ";
                    ga[nutra] = 1;
                    by[nutra] = 4;
                    tar[nutra] = tary[ii];
                    comp[nutra] = compy[ii];
                    ra[nutra] = rata; // ojo aqui se puede mejorar (comprobar que no haya cambio de rata)
                    cu[nutra] = new int[lar];
                    tim[nutra] = new double[lar];

                    k = 0;
                    for (i = 0; i < num; i++)
                    {
                        kk = blo[i];
                        if (blo[i] > 0)
                        {
                            for (jj = 0; jj < kk; jj++)
                            {
                                cu[nutra][k + jj] = cuu[i][jj];
                                tim[nutra][k + jj] = tii[i][jj];
                            }
                            k += kk;
                        }
                    }
                    nutra += 1;
                }
            }// for ii....

            panel2.Visible = false;
            return;
        }
        /// <summary>
        /// Calcula el promedio inicial por trazas sumando los primeros 100 valores en la matriz cu, 
        /// y guarda dichos promedios en el vector que almacena los promedios por estación.
        /// </summary>
        void PromediosIniciales()
        {
            int i, j;
            long suma;

            try
            {
                for (j = 0; j < nutra; j++)
                {
                    try
                    {
                        suma = 0;
                        for (i = 0; i < 100; i++)
                            suma += cu[j][i];
                        promEst[j] = (int)(suma / 100.0);
                    }
                    catch
                    {
                    }
                    //if (est[j].Substring(0, 4) == "GREZ") MessageBox.Show("Suma=" + suma.ToString() + " promEst=" + promEst[j].ToString()+" cu="+cu[j][50].ToString()+" "+est[j]);
                }
            }
            catch
            {
                MessageBox.Show("Error en Promedio Inicial!!");
            }
            return;
        }
        /// <summary>
        /// Rutina que dibuja toda la traza de la estación seleccionada y la cual sirve de 
        /// referencia para la clasificación, 'id' corresponde al numero de traza de la estación 
        /// seleccionada. La variable 'va' contiene los datos de las cuentas (aqui la traza puede 
        /// estar filtrada o no). La variable tim, contiene el valor del tiempo en formato SUDS.
        /// denom: variable que guarda el numero de lineas para graficar.
        /// </summary>
        /// <param name="pan">Panel donde se dibuja la traza, generalmente son los paneles panel1 y panel1a.</param>
        /// <param name="id">Corresponde al número de traza de la estación seleccionada.</param>
        /// <param name="va">Contiene los datos de las cuentas.</param>
        void Dibujo(Panel pan, ushort id, int[] va)
        {
            int i, xf, yf, ix, iy, jj, k, kk, ini, fin, pro, denom, max, min, mxx, mnn, dif = 0;
            int tota, inicio, final, mxp, mnp, cuentana;
            long ll;
            int[] numda;
            float fax, fay, fayy, fy, fsatu, iniy, x1 = 0, y1 = 0, diff, ff, fxsat, fmsat;
            double dd;
            string esta = "", ca = "", ss = "";
            ToolTip tip = new ToolTip();
            Color col, col2;
            Point[] dat;


            if (nutra == 0 || cargar == false)
                return;
            if (pan == panel1)
            {
                col = colotr1;
                col2 = colfondo;
            }
            else
            {
                col = Color.DarkGoldenrod;
                if (colfondo != Color.Black)
                    col2 = Color.Linen;
                else
                    col2 = Color.DarkGray;
            }
            mxp = 0;
            mnp = 0;
            if (!char.IsLetterOrDigit(est[id][1]) && !char.IsLetterOrDigit(est[id][2]))
                return;

            try
            {
                pan.BackColor = col2;
                util.borra(pan, col2);
                if (pan == panel1a)
                    tota = va.Length - M;
                else
                    tota = va.Length;
                if (tim[id][tota - 1] <= 0)
                {
                    do
                    {
                        tota -= 1;
                        if (tim[id][tota - 1] > 0)
                            break;
                    } while (tota > 0);
                }
                jj = 0;
                if (tota < 2)
                    return;
                dd = totven * 60.0;
                denom = (int)(Math.Abs(Math.Ceiling((tim[id][tota - 1] - timin) / dur)));
                if (denom <= 0)
                    denom = 1;

                // En el caso que sipro=2, es porque se ha buscado el promedio de un sector de traza, 
                // el cual esta comprendido entre la muestra p1 y la p2.
                if (sipro == 2)
                {
                    inicio = p1;
                    final = p2;
                    mxp = va[inicio];
                    mnp = mxp;
                    for (k = inicio + 1; k < final; k++)
                    {
                        if (mxp < va[k])
                            mxp = va[k];
                        else if (mnp > va[k])
                            mnp = va[k];
                    }
                }
                if (filt == true)
                    inicio = 250;
                else
                {
                    if (pan == panel1a)
                        inicio = M;
                    else
                        inicio = 1;
                }
                xf = pan.Size.Width;
                yf = pan.Size.Height;
                numda = new int[denom];
                for (k = 0; k < denom; k++)
                    numda[k] = 0;

                if (esp == 0)
                    fay = (float)((yf - 45.0F) / (double)(denom));
                else
                    fay = esp;
                if (analogico == false)
                    fayy = fay;// provi
                else
                {
                    cuentana = CuentasAnalogico;
                    if (nuanalog > 0)
                    {
                        for (i = 0; i < nuanalog; i++)
                        {
                            if (string.Compare(est[id], 0, estanalog[i], 0, 4) == 0)
                            {
                                cuentana = analog[i];
                                break;
                            }
                        }
                    }
                    fayy = (float)(100.0 / (cuentana * ga[id]));//provi
                }
                fax = xf / dur;
                max = va[inicio];
                min = max;
                for (k = inicio + 1; k < tota; k++)
                {
                    if (max < va[k])
                        max = va[k];
                    else if (min > va[k])
                        min = va[k];
                }
                for (k = inicio + 1; k < tota; k++)
                {
                    jj = (int)((tim[id][k] - timin) / dur);
                    if (jj > denom || jj < 0)
                        continue;

                    if (jj > -1)
                    {
                        if (jj >= denom)
                            break;
                        numda[jj] += 1;
                    }
                }
                if (sipro == 2)
                    pro = (int)((mxp + mnp) / 2.0);
                else
                {
                    if (PROMEDIO <= 0)
                        pro = (int)((max + min) / 2.0F);
                    else
                    {
                        mxx = va[0];
                        mnn = mxx;
                        for (i = 1; i < PROMEDIO; i++)
                        {
                            if (mxx < va[i])
                                mxx = va[i];
                            else if (mnn > va[i])
                                mnn = va[i];
                        }
                        pro = (int)((mxx + mnn) / 2.0);
                    }
                }
                //if (est[id].Substring(0, 4) == "GREZ") MessageBox.Show("sipro="+sipro.ToString()+" pro="+pro.ToString());
                if (analogico == false)
                {
                    if (max - pro != 0)
                        fy = ((fayy / 2) / ((max - pro)));
                    else
                        fy = 1;
                }
                else
                    fy = fayy;
                if (est[id].Length > 4 && !char.IsLetterOrDigit(est[id][4]))
                    esta = est[id].Substring(0, 4);
                else
                    esta = est[id];
                //promEst[id] = pro;

                if (max - pro != 0)
                    fsatu = ((fay / 2) / ((max - pro)));
                else
                    fsatu = 1;
                dif = max - pro;
                diff = dif * fsatu;
                fxsat = (float)(diff + satur * diff * 0.5);   // fxsat y fmsat, son variables para la opcion de saturacion.
                dif = min - pro;
                diff = dif * fsatu;
                fmsat = (float)(diff + satur * diff * 0.5);

                Graphics dc = pan.CreateGraphics();
                Pen lapiz = new Pen(colinea, 1);
                SolidBrush brocha = new SolidBrush(col);
                SolidBrush brocha2 = new SolidBrush(Color.OrangeRed);
                SolidBrush brocha3 = new SolidBrush(Color.Green);
                SolidBrush brocha4 = new SolidBrush(Color.Goldenrod);
                SolidBrush brocha5 = new SolidBrush(Color.Gray);

                kk = 0;
                iniy = 0;
                ll = (long)(Fei + timin * 10000000.0);
                DateTime fech = new DateTime(ll);
                ss = string.Format("{0:yyyy}/{0:MM}/{0:dd} {0:HH}:{0:mm}:{0:ss}", fech);
                dc.DrawString(ss, new Font("Times New Roman", 10), brocha2, 3, 3);
                ss = "(" + totven.ToString() + "' )";
                dc.DrawString(ss, new Font("Times New Roman", 10), brocha3, 115, 3);
                ss = "[" + esp.ToString() + "]";
                dc.DrawString(ss, new Font("Times New Roman", 10), brocha4, 145, 3);
                ss = "usuario: " + usu;
                dc.DrawString(ss, new Font("Times New Roman", 10), brocha5, 170, 3);
                ca = "";
                if (no30[id] == true)
                    ca = "## ";
                if (siTraslapo[id] == true)
                    ca += "!!   ";
                ca += esta + "   (" + tar[id] + ")";
                if (siRoto[id] == true)
                    ca += "   !! ";
                dc.DrawString(ca, new Font("Times New Roman", 12, FontStyle.Bold), brocha, xf / 2 - 50, 10);
                if (invertido[id] == true)
                {
                    Pen lapp = new Pen(Color.Black, 1);
                    SolidBrush broo = new SolidBrush(Color.Yellow);
                    dc.FillEllipse(broo, xf / 2 - 37, 15, 8, 8);
                    dc.DrawEllipse(lapp, xf / 2 - 37, 15, 8, 8);
                    lapp.Dispose();
                    broo.Dispose();
                }
                ca = string.Format("{0:0.000} m/s", ra[id]);
                dc.DrawString(ca, new Font("Times New Roman", 10), brocha5, xf - 100, 10);

                ini = inicio;

                try
                {
                    for (k = 0; k < denom; k++)
                    {
                        if (numda[k] > 0)
                        {
                            jj = 0;
                            dat = new Point[numda[k]];
                            fin = ini + numda[k];
                            if (fin > tota)
                                fin = tota;
                            iniy = incy + 45 + k * fay + fay / 2;
                            //Pen laaa = new Pen(Color.Red, 1);
                            //dc.DrawLine(laaa, 1, iniy, xf, iniy);
                            //laaa.Dispose();
                            if (satu == false)
                            {
                                if (invertido[id] == false)
                                {
                                    for (kk = ini; kk < fin; kk++)
                                    {
                                        //dif = va[kk] - pro;
                                        dif = va[kk] - promEst[id];
                                        diff = dif * fy;
                                        ff = ampli * diff;
                                        y1 = iniy - ff;
                                        x1 = (float)(((tim[id][kk] - timin) - k * dur) * fax);
                                        dat[jj].Y = (int)(y1);
                                        dat[jj].X = (int)x1;
                                        jj += 1;
                                    }
                                }
                                else
                                {
                                    for (kk = ini; kk < fin; kk++)
                                    {
                                        //dif = pro-va[kk];
                                        dif = promEst[id] - va[kk];
                                        diff = dif * fy;
                                        ff = ampli * diff;
                                        y1 = iniy - ff;
                                        x1 = (float)(((tim[id][kk] - timin) - k * dur) * fax);
                                        dat[jj].Y = (int)(y1);
                                        dat[jj].X = (int)x1;
                                        jj += 1;
                                    }
                                }
                            }
                            else
                            {
                                if (invertido[id] == false)
                                {
                                    for (kk = ini; kk < fin; kk++)
                                    {
                                        //dif = va[kk] - pro;
                                        dif = va[kk] - promEst[id];
                                        diff = dif * fy;
                                        ff = ampli * diff;
                                        if (ff > fxsat) ff = fxsat;
                                        else if (ff < fmsat) ff = fmsat;
                                        y1 = iniy - ff;
                                        x1 = (float)(((tim[id][kk] - timin) - k * dur) * fax);
                                        dat[jj].Y = (int)(y1);
                                        dat[jj].X = (int)x1;
                                        jj += 1;
                                    }
                                }
                                else
                                {
                                    for (kk = ini; kk < fin; kk++)
                                    {
                                        //dif = pro-va[kk];
                                        dif = promEst[id] - va[kk];
                                        diff = dif * fy;
                                        ff = ampli * diff;
                                        if (ff > fxsat) ff = fxsat;
                                        else if (ff < fmsat) ff = fmsat;
                                        y1 = iniy - ff;
                                        x1 = (float)(((tim[id][kk] - timin) - k * dur) * fax);
                                        dat[jj].Y = (int)(y1);
                                        dat[jj].X = (int)x1;
                                        jj += 1;
                                    }
                                }
                            }
                            dc.DrawLines(lapiz, dat);//Aca es donde grafica
                            ini += numda[k];
                        }
                    }
                }
                catch
                {
                }

                if (kk > 0 && iniy > 0 && iniy < yf)
                {
                    if (x1 < xf - 110)
                        ix = (int)(x1);
                    else
                        ix = xf - 110;
                    if (esp == 0)
                        iy = yf - 20;
                    else
                        iy = (int)(iniy + fay);
                    ll = (long)(Fei + tim[id][kk - 1] * 10000000.0);
                    DateTime fech2 = new DateTime(ll);
                    ss = string.Format("{0:yyyy}/{0:MM}/{0:dd} {0:HH}:{0:mm}:{0:ss}", fech2);
                    dc.DrawString(ss, new Font("Times New Roman", 10), brocha2, ix, iy);
                }

                lapiz.Dispose();
                brocha.Dispose();
                brocha2.Dispose();
                brocha3.Dispose();
                brocha4.Dispose();
                brocha5.Dispose();
                try
                {
                    if (marcati == true)
                        util.MarcaTiempo(pan, timin, tim[id], esp, dur, denom);
                }
                catch
                {
                }
                if (pepas == true || pepasvol == true)
                {
                    util.PonePepas(pan, timin, tim[id], esp, dur, contampl, valampl, clR, clG, clB,
                        letampl, cl, tam, nucla, siPampl, volampl, volcan[vol][0], pepasvol, nolec,
                           tigrabacion, leclec, denom);
                }
                // la variable siArch, es verdadera cuando se quiere visualizar la duración de los 
                // archivos clasificados en la traza de la estacion activa. Se visualiza con la rutina
                // VerArchi, que se encuentra en las utilidades (Util.cs)
                if (siArch == true)
                    util.VerArchi(pan, timin, tim[id], tiar, duar, esp, dur, contarch);
            }
            catch
            {
                NoMostrar = true;
                tip.IsBalloon = false;
                tip.ReshowDelay = 0;
                tip.Show("\nPROBLEMAS!!! Tome otro archivo inicial!\n ", panel1, 1000);
            }


            return;
        }
        /// <summary>
        /// Esta rutina adecua el panel de clasificacion y hace llamado a la rutina que
        /// Grafica las trazas: DibujoTrazas().
        /// </summary>
        void Clasificar()
        {
            int i, j;// xx, yy;
            double dd;
            //string ca = "";

            j = -1;
            for (i = 0; i < nutra; i++)
            {
                if (siEst[i] == false)
                {
                    j = 1;
                    break;
                }
            }
            if (j == 1)
                boTodas.BackColor = Color.PaleVioletRed;
            else
                boTodas.BackColor = Color.White;
            clas = "__";
            marca = "********";
            bomarca.BackColor = Color.White;
            bovar.Visible = false;
            boHypo71.Visible = false;
            boAten.Visible = false;
            bovar.Text = "Varias";
            if (File.Exists("c:\\scilab\\bin\\wscilex.exe") && File.Exists(".\\sci\\demux.sce"))
                boScilab.Visible = true;
            nuampvar = 0;
            // amplivarias.txt, es el archivo que lleva los datos cuando se realizan varias lecturas
            // de amplitud y coda.
            if (File.Exists("amplivarias.txt"))
                File.Delete("amplivarias.txt");
            if (Math.Abs(tie2 - tie1) < 0.1)
                return;
            // tie1 y tie2 corresponden a los extremos del
            // intervalo de tiempo seleccionado en el panel principal y con respecto al cual se
            // grafican las trazas en el panel de clasificacion.
            Pti = 0;
            Sti = 0;
            Cti = 0;
            Ati = 0;
            t1amp = 0;
            t2amp = 0;
            panelcla.Visible = true;
            if (tie1 > tie2)
            {
                dd = tie1;
                tie1 = tie2;
                tie2 = dd;
            }

            //if (File.Exists("datos.txt")) YaClasificados();    
            DibujoTrazas();// rutina que grafica las trazas de las estaciones que se van a clasificar.
            //xx = panelcla.Size.Width - 30;
            //yy = 1;
            //ca = usu.Substring(0, 3);
            util.Letreros(panelcla, panelcla.Size.Width - 30, 1, usu.Substring(0, 3), Color.LightYellow); // coloca el nombre del usuario

            return;
        }
        /// <summary>
        /// Rutina que comprueba si existen sismos ya clasificados para el inicio de la ventana
        /// seleccionada. Si existen, se lo comunica al usuario, con el fin que no exista 
        /// reemplazos indeseados, de las trazas de la Base.
        /// </summary>
        void YaClasificados()
        {
            /*
             * Rutina que comprueba si existen sismos ya clasificados para el inicio de la ventana
             * seleccionada. Si existen, se lo comunica al usuario, con el fin que no exista 
             * reemplazos indeseados, de las trazas de la Base.
            */
            int i, k;
            long ll;
            string pa = "", fee = "", ca = "";
            string[] ti = new string[25];

            ll = (long)(Fei + tie1 * 10000000.0); // se convierte el inicio del sector de clasificacion a tiempo en visual c#
            DateTime fech = new DateTime(ll);
            fee = string.Format("{0:MM}{0:dd}{0:HH}{0:mm}", fech);
            // En datos.txt, se guardan los datos esenciales de los archivos clasificados que existen en la Base, para el sector escogido al inicio de sesion.
            StreamReader ar = new StreamReader("datos.txt");
            pa = "";
            k = 0;
            while (pa != null)
            {
                try
                {
                    pa = ar.ReadLine();
                    if (pa == null) break;
                    if (string.Compare(pa.Substring(16, 8), fee.Substring(0, 8)) == 0)
                        ti[k++] = pa.Substring(16, 12);

                }
                catch
                {
                }
            }
            ar.Close();
            if (k > 0)
            {
                ca = "";
                for (i = 0; i < k; i++) ca += ti[i] + "\n";
                ca += "CLASIFICADOS!!";
                NoMostrar = true;
                MessageBox.Show(ca);
            }

            return;
        }
        /// <summary>
        /// Rutina que inicializa variables, cuando se cierra el panel de clasificación.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void bout_Click(object sender, EventArgs e)
        { // Rutina que inicializa variables, cuando se cierra el panel de clasificacion.
            //loscajones = false;
            if (panelcla.Visible == false) return;
            if (panel2.Visible == true) panel2.Visible = false;
            if (panelParti.Visible == true) panelParti.Visible = false;
            if (VerEspectro == true)
            {
                panelFFTzoom.Visible = false;
                panelBarEsp1.Visible = false;
                VerEspectro = false;
                moveresp = false;
                movespcla = false;
                boEspCla.BackColor = Color.White;
                boEspe.BackColor = Color.WhiteSmoke;
                yloc = -1;
            }

            Cladat[0].X = 0;
            Cladat[0].Y = 0;
            Cladat[1].X = 0;
            Cladat[1].Y = 0;
            panelcla.Visible = false;
            panelcoda.Visible = false;
            filtcod = false;
            calcfiltcod = false;
            bofilcod.BackColor = Color.White;
            bohzcod.BackColor = Color.White;
            bopolcod.BackColor = Color.White;
            radlowcod.BackColor = Color.White;
            radhicod.BackColor = Color.White;
            panelAmp.Visible = false;
            panelEsta.Visible = false;
            if (panelmarca.Visible == true)
                panelmarca.Visible = false;
            panel1.Invalidate();

            return;
        }
        /// <summary>
        /// Rutina que magnifica el panel de clasificacion a toda la pantalla.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void bozoom_Click(object sender, EventArgs e)
        {// Rutina que magnifica el panel de clasificacion a toda la pantalla.

            if (panelcla.Visible == false) return;

            if (panelcla.Size.Width < Size.Width - 10)
            {
                panelcla.Location = new Point(1, 1);
                panelcla.Size = new Size(Size.Width - 10, Size.Height - 30);
                zoomcla = true;
            }
            else
            {
                panelcla.Size = new Size(850, 568);
                zoomcla = false;
                panel1.Invalidate();
            }
            panelcla.BringToFront();
            Clasificar();
            if (panelEsta.Visible == true)
                util.EscribePanelEsta(panelEsta, nutra, est, siEst);
            return;
        }
        /// <summary>
        /// PanelClas superior  dererecha: Coloca el panel de clasificación en la parte superior derecha de la pantalla.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void bosude_Click(object sender, EventArgs e)
        {
            panelcla.Location = new Point(Size.Width - panelcla.Width, 1);
            panelcla.BringToFront();
            panel1.Invalidate();
            return;
        }
        /// <summary>
        /// PanelClas superior izquierda: Coloca el panel de clasificación en la parte superior izquierda de la pantalla.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void bosuiz_Click(object sender, EventArgs e)
        {// panelClas sup izq: Coloca el panel de clasificacion en la parte superior izquierda de la pantalla
            panelcla.Location = new Point(1, 1);
            panelcla.BringToFront();
            panel1.Invalidate();
            return;
        }
        /// <summary>
        ///  PanelClas inferiror dererecha: Coloca el panel de clasificación en la parte inferior derecha de la pantalla
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void boinde_Click(object sender, EventArgs e)
        {
            panelcla.Location = new Point(Size.Width - panelcla.Width, Size.Height - (panelcla.Height + 56));
            panelcla.BringToFront();
            panel1.Invalidate();
            return;
        }
        /// <summary>
        /// PanelClas inferior izquierda: Coloca el panel de clasificación en la parte inferior izquierda de la pantalla. 
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void boiniz_Click(object sender, EventArgs e)
        {// panelClas inf izq: Coloca el panel de clasificacion en la parte inferior izquierda de la pantalla            
            panelcla.Location = new Point(1, Size.Height - (panelcla.Height + 56));
            panelcla.BringToFront();
            panel1.Invalidate();
            return;
        }
        /// <summary>
        /// Botón de estacion de referencia.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void boref_Click(object sender, EventArgs e)
        {
            if (refe == true)
                refe = false;
            else
                refe = true;
            BotRefe();

            return;
        }
        /// <summary>
        /// Botón que marca el sismo a clasificar (ver manual).
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void bomarca_Click(object sender, EventArgs e)
        {
            if (panelmarca.Visible == false)
            {
                panelmarca.Visible = true;
                panelmarca.BringToFront();
                util.VerMarca(panelmarca, marca);
            }
            else
            {
                panelmarca.Visible = false;
                panel1.Invalidate();
                if (panelAmp.Visible == true) panelAmp.Invalidate();
            }
            return;
        }
        /// <summary>
        /// Panel donde aparecen en 8 columnas, los caracteres que sirven de marca a los sismos,
        /// marca, es la variable que guarda los 8 caracteres de marcacion de un sismo. Cuando el
        /// sismo no esta marcado, todos los caracteres son asteriscos.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void panelmarca_MouseDown(object sender, MouseEventArgs e)
        {
            int i, j, xf, yf;
            float fax, fay;

            if (panelmarca.Visible == false)
                return;
            util.VerMarca(panelmarca, marca);
            char[] marcachar = marca.ToCharArray();
            xf = panelmarca.Size.Width;
            yf = panelmarca.Size.Height - 20;
            fax = xf / 8;
            fay = yf / 42;
            i = (int)(e.X / fax);
            j = (int)((e.Y - 2) / fay);
            if (j == 0) marcachar[i] = '*';
            else marcachar[i] = (char)(j + 47);
            marca = new string(marcachar);
            util.VerMarca(panelmarca, marca);
            if (string.Compare(marca, "********") != 0) bomarca.BackColor = Color.IndianRed;
            else bomarca.BackColor = Color.White;

            return;
        }
        /// <summary>
        /// Desplaza el gráfico de las trazas en la ventana de clasificación hacia la izquierda.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void boclaizq_MouseDown(object sender, MouseEventArgs e)
        {
            int i, lar;
            double tii1, tii2;

            tii1 = tie1 + 5.0;
            tii2 = tie2 + 5.0;
            for (i = 0; i < nutra; i++)
            {
                lar = tim[i].Length;
                if (tii2 > tim[i][lar - 1]) return;
            }
            tie1 = tii1;
            tie2 = tii2;
            DibujoTrazas(); // dibuja las señales en el panel de clasificacion
            util.EscribePanelEsta(panelEsta, nutra, est, siEst);// si el panel con el nombre de estaciones esta visible, escribe el nombre de las estaciones.
            return;

        }
        /// <summary>
        /// Desplaza el gráfico de las trazas en la ventana de clasificación hacia la derecha.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void boclader_MouseDown(object sender, MouseEventArgs e)
        {
            int i;
            double tii1, tii2;

            tii1 = tie1 - 5.0;
            tii2 = tie2 - 5.0;
            for (i = 0; i < nutra; i++)
            {
                if (tii1 < tim[i][0]) return;
            }
            tie1 = tii1;
            tie2 = tii2;
            DibujoTrazas();
            util.EscribePanelEsta(panelEsta, nutra, est, siEst);
            return;

        }
        /// <summary>
        /// Disminuir ventana tiempo en Clasificar o si se quiere 'estira' la señal.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void bocladil_MouseDown(object sender, MouseEventArgs e)
        {
            int i;
            double tii;

            tii = tie2 - 5.0;
            if (tii - tie1 <= 0)
            {
                tii = tie2 - 0.5;
            }
            for (i = 0; i < nutra; i++)
            {
                //if (tii - tie1 < 20.0) return;
                if (tii - tie1 < 0.5) return;
            }
            tie2 = tii;
            DibujoTrazas();
            util.EscribePanelEsta(panelEsta, nutra, est, siEst);
            return;

        }
        /// <summary>
        /// Aumentar ventana tiempo en Clasificar, o si se quiere, 'comprime' la señal.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void boclaenc_MouseDown(object sender, MouseEventArgs e)
        {
            int i, lar;
            double tii;

            tii = tie2 + 5.0;
            for (i = 0; i < nutra; i++)
            {
                lar = tim[i].Length;
                if (tii >= tim[i][lar - 1]) return;
            }
            tie2 = tii;
            DibujoTrazas();
            util.EscribePanelEsta(panelEsta, nutra, est, siEst);

            return;

        }
        /// <summary>
        /// Selecciona todas las estaciones del sismo.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void boTodas_Click(object sender, EventArgs e)
        {
            int i;

            loscajones = false;
            for (i = 0; i < nutra; i++) siEst[i] = true;
            for (i = 0; i < totalbotoncajon; i++) best[i].BackColor = Color.Peru;
            boTodas.BackColor = Color.White;
            if (panelEsta.Visible == true)
                panelEsta.Visible = false;
            DibujoTrazas();
            if (panelcoda.Visible == true)
                DibujoClascoda();

            return;

        }
        /// <summary>
        /// Muestra o esconde el panel donde se escoge individualmente las estaciones.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void boEsta_Click(object sender, EventArgs e)
        {
            int i;
            if (panelEsta.Visible == false)
            {
                panelEsta.Visible = true;
                i = util.EscribePanelEsta(panelEsta, nutra, est, siEst);
                if (i == 1)
                    boTodas.BackColor = Color.PaleVioletRed;
                else
                    boTodas.BackColor = Color.White;
            }
            else
            {
                panelEsta.Visible = false;
                DibujoTrazas();// dibuja las trazas en el panel de clasificacion
            }
            if (panelcoda.Visible == true)
            {
                if (siEst[nucod] == false)
                {
                    panelcoda.Visible = false;
                    filtcod = false;
                    calcfiltcod = false;
                    bofilcod.BackColor = Color.White;
                    bohzcod.BackColor = Color.White;
                    bopolcod.BackColor = Color.White;
                    radlowcod.BackColor = Color.White;
                    radhicod.BackColor = Color.White;
                }
                else
                    DibujoClascoda();
            }
            return;
        }
        /// <summary>
        /// Selección individual de las estaciones.
        /// Cuando la traza de una estacion no esta visible, su nombre aparece en rojo.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void panelEsta_MouseDown(object sender, MouseEventArgs e)
        {
            int i, j, k, val;

            if (panelEsta.Visible == false)
                return;

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
            DibujoTrazas();
            i = util.EscribePanelEsta(panelEsta, nutra, est, siEst);// utilidad que escribe las estaciones en el panel de seleccion de estaciones
            if (i == 1) boTodas.BackColor = Color.PaleVioletRed;// si no estan seleccionadas todas las estaciones
            else boTodas.BackColor = Color.White;// si lo estan

            return;
        }
        /// <summary>
        /// Dibuja las trazas dependiendo el estado del botón boNano.
        /// </summary>
        void DibujoTrazas()
        {
            if (analogico == true || boNano.Text == "Tra")
            {
                TrazasClas();

            }
            else
                TrazasClasCuentas();

            return;
        }
        /// <summary>
        /// Se encarga de dibujar las trazas en el panel de clasificación según la porción de traza seleccionada, hace el dibujo semejante a la señal analógica.
        /// </summary>
        void TrazasClas()
        {
            int xf, yf, i, j = 0, jj, k, kk, lar, max, min, pro, dif = 0, tamlet, numtotra;
            int nmi = 0, nmf = 0;
            float x1, y1;
            double dura, fax, fay, fy, diff, iniy, fxsat, fmsat;
            string esta = "";
            bool[] siesta = new bool[nutra]; // indica que una traza esta en el rango de tiempo especificado
            bool error = false;
            string err = "Problemas en:\n";
            int[][] val;
            double[][] timm;
            Point[] dat;


            NoMostrar = true;
            val = new int[nutra][];
            timm = new double[nutra][];
            for (i = 0; i < nutra; i++)
            {
                lar = tim[i].Length; //La ultima columna
                if (siEst[i] == true && tim[i][lar - 1] <= tie1)
                {
                    siEst[i] = false;
                    err += est[i].Substring(0, 4) + "\n";
                    error = true;
                    val[i] = new int[1];
                    val[i][0] = 0;
                }
                else
                {
                    nmi = (int)((tie1 - tim[i][0]) * ra[i]);                 // muestra inferior del panel de clasificacion numero minutos inicial 
                    if (nmi < 0)
                        nmi = 0;
                    nmf = (int)((tie2 - tim[i][0]) * ra[i]);        // número minutos final
                    if (nmf > cu[i].Length)
                        nmf = cu[i].Length;
                    if (nmi > cu[i].Length || nmf < 0)
                    {
                        siesta[i] = false;
                    }
                    else
                    {
                        val[i] = new int[nmf - nmi]; // por cada traza almacena un vector con los valores de las cuentas 
                        timm[i] = new double[nmf - nmi];
                        k = 0;
                        if (sifilt == false)
                        {
                            for (j = nmi; j < nmf; j++)
                            {
                                val[i][k] = cu[i][j];
                                timm[i][k++] = tim[i][j];
                            }
                        }
                        else
                        {
                            try
                            {
                                if (nmi < 0)
                                    nmi = 0;
                                for (j = nmi; j < nmf; j++)
                                {
                                    //val[i][k] = cff[i][k]; // aqui sale el error
                                    val[i][k] = cff[i][j - nmi]; // aqui sale el error
                                    timm[i][k++] = tim[i][j];
                                }
                                //MessageBox.Show("val.len=" + val[i].Length.ToString());
                            }
                            catch
                            {
                                //MessageBox.Show("j="+j.ToString()+" i="+i.ToString()+" cff.len="+cff[i].Length.ToString());
                            }
                        }
                    }
                }
                siesta[i] = siEst[i];
            }//fin for-----------------------------------------------------------------------------------------------------------------------------------------------------
            panelcladib.BackColor = colfondo;
            panelcladib.Visible = false;
            panelcladib.Visible = true;
            xf = panelcladib.Size.Width - 40;
            yf = panelcladib.Size.Height;
            numtotra = 0; // seguramente es número total de trazas que hay en el rango de tiempo que se arrastró
            for (i = 0; i < nutra; i++)
            {
                if (siesta[i] == true)
                    numtotra += 1;
            }
            // MessageBox.Show("nutra="+nutra.ToString()+" numtotra="+numtotra.ToString()+" id="+id.ToString());
            dura = tie2 - tie1;          // duracion en segundos de la ventana. La duracion de todo el sismo se guarda en la variable durx.
            fax = xf / dura;             // factor en la horizontal
            fay = yf / (numtotra + 0.5); // factor en la vertical
            tamlet = (int)(fay);         // tamaño de las letras para el nombre de las estaciones
            if (tamlet > 10)
                tamlet = 10;

            Graphics dc = panelcladib.CreateGraphics();
            Pen lapiz = new Pen(colinea, 1);
            SolidBrush brocha = new SolidBrush(colotr1);
            jj = 0;
            try
            {
                for (j = 0; j < nutra; j++)
                {
                    if (siesta[j] == true)
                    {
                        try
                        {
                            if (val[j].Length > 0)
                            {
                                max = val[j][0];
                                min = max; // aqui se va a buscar el valor maximo de cuenta del intervalo de interes.
                                for (k = 1; k < val[j].Length; k++)
                                {
                                    if (max < val[j][k])
                                        max = val[j][k];
                                    else if (min > val[j][k])
                                        min = val[j][k];
                                }
                                if (analogico == false)
                                    pro = (int)((max + min) / 2.0F);
                                else
                                {
                                    pro = (int)((max + min) / 2.0F);
                                    max = pro + (int)(CuentasAnalogico / 2.0);
                                    min = pro - (int)(CuentasAnalogico / 2.0);
                                }
                                if (max - pro != 0)
                                    fy = ((fay / 2) / ((max - pro)));
                                else
                                    fy = 1.0;
                                iniy = 5 + jj * fay + fay / 2;
                                if (est[j].Length > 4 && !char.IsLetterOrDigit(est[j][4]))
                                    esta = est[j].Substring(0, 4);
                                else
                                    esta = est[j];
                                dc.DrawString(esta, new Font("Times New Roman", tamlet, FontStyle.Bold), brocha, 1, (float)(iniy - tamlet));
                                dat = new Point[val[j].Length];
                                kk = 0;
                                // los valores de la señal se guardan en un arreglo (dat) y se utiliza la funcion Drawlines, que es mucho mas rapida.
                                if (satu == false)
                                {
                                    if (invertido[j] == false)
                                    {
                                        for (k = 0; k < val[j].Length; k++)
                                        {
                                            dif = val[j][k] - pro;
                                            diff = dif * fy;
                                            y1 = (float)(iniy - diff);
                                            x1 = (float)(40.0 + (timm[j][k] - tie1) * fax);
                                            dat[kk].Y = (int)y1;
                                            dat[kk].X = (int)x1;
                                            kk += 1;
                                        }
                                    }
                                    else
                                    {
                                        for (k = 0; k < val[j].Length; k++)
                                        {
                                            dif = pro - val[j][k];
                                            diff = dif * fy;
                                            y1 = (float)(iniy - diff);
                                            x1 = (float)(40.0 + (timm[j][k] - tie1) * fax);
                                            dat[k].Y = (int)y1;
                                            dat[k].X = (int)x1;
                                        }
                                    }
                                }
                                else
                                {
                                    dif = max - pro;
                                    diff = dif * fy;
                                    fxsat = diff;   // fxsat y fmsat, son variables para la opcion de saturacion.
                                    dif = min - pro;
                                    diff = dif * fy;
                                    fmsat = diff;
                                    if (invertido[j] == false)
                                    {
                                        for (k = 0; k < val[j].Length; k++)
                                        {
                                            dif = val[j][k] - pro;
                                            diff = dif * fy;
                                            if (diff > fxsat)
                                                diff = fxsat;
                                            else if (diff < fmsat)
                                                diff = fmsat;
                                            y1 = (float)(iniy - diff);
                                            x1 = (float)(40.0 + (timm[j][k] - tie1) * fax);
                                            dat[k].Y = (int)y1;
                                            dat[k].X = (int)x1;
                                        }
                                    }
                                    else
                                    {
                                        for (k = 0; k < val[j].Length; k++)
                                        {
                                            dif = pro - val[j][k];
                                            diff = dif * fy;
                                            if (diff > fxsat)
                                                diff = fxsat;
                                            else if (diff < fmsat)
                                                diff = fmsat;
                                            y1 = (float)(iniy - diff);
                                            x1 = (float)(40.0 + (timm[j][k] - tie1) * fax);
                                            dat[k].Y = (int)y1;
                                            dat[k].X = (int)x1;
                                        }
                                    }
                                }
                                dc.DrawLines(lapiz, dat);
                            }// if lar....
                            else
                                siEst[j] = false;
                            jj += 1;
                        }//try
                        catch
                        {
                            continue;
                        }
                    }// if siesta.....
                }// for j....
            }
            catch
            {
                // MessageBox.Show("error " + est[j]);
            }
            lapiz.Dispose();
            brocha.Dispose();

            if (Cladat[1].X == 0 && Cladat[1].Y == 0)
                return;
            Graphics dc2 = panel1.CreateGraphics();
            Pen laap = new Pen(Color.Aquamarine, 1);
            i = (int)(Math.Abs(Cladat[1].X - Cladat[0].X));
            if (i < 4)
                i = 4;
            j = (int)(Math.Abs(Cladat[1].Y - Cladat[0].Y));
            if (j < 2)
                j = 2;
            dc2.DrawRectangle(laap, Cladat[0].X, Cladat[0].Y, i, j);
            laap.Dispose();

            if (error == true)
            {
                MessageBox.Show(err);
                TrazasClas();
            }

            return;
        }
        /// <summary>
        /// Se encarga de dibujar las trazas en el panel de clasificación según la porción de traza seleccionada,
        /// dependiendo del texto del botón boNano puede hacer el dibujo de las trazas de 2 formas distintas
        /// con base a la unidad de cuentas o en base a los nanómetros/seg.
        /// </summary>
        void TrazasClasCuentas()
        {// dibuja las trazas en el panel de clasificacion
            int xf, yf, i, j, jj, k, lar, max, min, pro, tamlet, numtotra;
            int nmi = 0, nmf = 0;
            float x1, y1;
            double dura, fax, fay, fy, diff, dif = 0, iniy, dd1, dd2, fxsat, fmsat;
            double maximo = 0;
            double[] factor = new double[nutra];
            double[] ganan = new double[nutra];
            string esta = "";
            bool[] siesta = new bool[nutra];
            bool si = false;
            int[][] val;
            double[][] timm;
            Point[] dat;

            for (i = 0; i < nutra; i++)
                siesta[i] = siEst[i];

            val = new int[nutra][];
            timm = new double[nutra][];
            for (i = 0; i < nutra; i++)
            {
                lar = tim[i].Length;
                if (siEst[i] == true && tim[i][lar - 1] <= tie1)
                {
                    /* siEst[i] = false;
                     err += est[i].Substring(0, 4) + "\n";
                     error = true;*/
                    val[i] = new int[1];
                    val[i][0] = 0;
                }
                else
                {
                    nmi = (int)((tie1 - tim[i][0]) * ra[i]);                 // muestra inferior del panel de clasificacion
                    if (nmi < 0)
                        nmi = 0;
                    nmf = (int)((tie2 - tim[i][0]) * ra[i]);
                    if (nmi > cu[i].Length || nmf < 0)
                    {
                        siesta[i] = false;
                    }
                    else
                    {
                        val[i] = new int[nmf - nmi];
                        timm[i] = new double[nmf - nmi];
                        k = 0;
                        if (sifilt == false)
                        {
                            for (j = nmi; j < nmf; j++)
                            {
                                val[i][k] = cu[i][j];
                                timm[i][k++] = tim[i][j];
                            }
                        }
                        else
                        {
                            for (j = nmi; j < nmf; j++)
                            {
                                val[i][k] = cff[i][k];
                                timm[i][k++] = tim[i][j];
                            }
                        }
                    }
                }
                siesta[i] = siEst[i];
            }

            if (boNano.Text == "Nan")
            {
                for (i = 0; i < nutra; i++)
                {
                    if (fcnan[i] <= 0)
                        siesta[i] = false;
                    factor[i] = fcnan[i];
                    ganan[i] = (double)(ga[i]);
                }
            }
            else
            {
                for (i = 0; i < nutra; i++)
                {
                    factor[i] = 1.0;
                    ganan[i] = 1.0;
                }
            }
            panelcladib.BackColor = colfondo;
            panelcladib.Visible = false;
            panelcladib.Visible = true;
            xf = panelcladib.Size.Width - 40;
            yf = panelcladib.Size.Height;
            numtotra = 0;
            for (i = 0; i < nutra; i++)
            {
                if (siesta[i] == true)
                    numtotra += 1;
            }
            // MessageBox.Show("nutra="+nutra.ToString()+" numtotra="+numtotra.ToString()+" id="+id.ToString());
            dura = tie2 - tie1;          // duracion en segundos de la ventana. La duracion de todo el sismo se guarda en la variable durx.
            fax = xf / dura;             // factor en la horizontal
            fay = yf / (numtotra + 0.5); // factor en la vertical
            tamlet = (int)(fay);         // tamaño de las letras para el nombre de las estaciones
            if (tamlet > 10) tamlet = 10;

            Graphics dc = panelcladib.CreateGraphics();
            Pen lapiz = new Pen(colinea, 1);
            SolidBrush brocha = new SolidBrush(colotr1);
            jj = 0;
            try
            {
                for (j = 0; j < nutra; j++)
                {
                    if (siesta[j] == true)
                    {
                        if (val[j].Length > 1)
                        {
                            max = val[j][0];
                            min = max; // aqui se va a buscar el valor maximo de cuenta del intervalo de interes.
                            for (k = 1; k < val[j].Length; k++)
                            {
                                if (max < val[j][k]) max = val[j][k];
                                else if (min > val[j][k]) min = val[j][k];
                            }
                            if (est[j].Substring(0, 4) != "IRIG")
                            {
                                if (si == true)
                                {
                                    if (maximo < factor[j] * (max - min) / (double)(ga[j])) maximo = factor[j] * (max - min) / (double)(ganan[j]);
                                }
                                else
                                {
                                    maximo = factor[j] * (max - min) / (double)(ganan[j]);
                                    si = true;
                                }
                            }// if est....
                        }// if lar
                    }// if siesta
                }// for j....

                for (j = 0; j < nutra; j++)
                {
                    if (siesta[j] == true)
                    {
                        if (val[j].Length > 0)
                        {
                            max = val[j][0];
                            min = max; // aqui se va a buscar el valor maximo de cuenta del intervalo de interes.                            
                            for (k = 1; k < val[j].Length; k++)
                            {
                                if (max < val[j][k]) max = val[j][k];
                                else if (min > val[j][k]) min = val[j][k];
                            }
                            //MessageBox.Show("maximo=" + maximo.ToString() + " dif=" + (max - min).ToString()+" gan="+ga[j].ToString()+" fac="+fcnan[j].ToString());
                            max = (int)(factor[j] * max / ganan[j]);
                            min = (int)(factor[j] * min / ganan[j]);
                            pro = (int)((max + min) / 2.0F);
                            if (est[j].Substring(0, 4) != "IRIG") max = (int)(pro + (maximo / 2.0));
                            //if (max - pro != 0) fy = ((fay / 2) / ((max - pro)));
                            if (max - pro != 0) fy = ((fay / 2) / ((max - pro)));
                            else fy = 1.0;
                            //pro = (int)(pro * ampclas);// ampclas guarda el valor 1.0. inicialmente se pretendia guardar el valor de amplificacion escogido por el usuario.
                            iniy = 5 + jj * fay + fay / 2;
                            if (!char.IsLetterOrDigit(est[j][4])) esta = est[j].Substring(0, 4);
                            else esta = est[j];
                            dc.DrawString(esta, new Font("Times New Roman", tamlet, FontStyle.Bold), brocha, 1, (float)(iniy - tamlet));
                            dat = new Point[val[j].Length];
                            // los valores de la señal se guardan en un arreglo (dat) y se utiliza la funcion Drawlines, que es mucho mas rapida.
                            if (satu == false)
                            {
                                if (invertido[j] == false)
                                {
                                    for (k = 0; k < val[j].Length; k++)
                                    {
                                        dif = factor[j] * val[j][k] / ganan[j] - pro;
                                        diff = dif * fy;
                                        y1 = (float)(iniy - diff);
                                        x1 = (float)(40.0 + (timm[j][k] - tie1) * fax);
                                        dat[k].Y = (int)y1;
                                        dat[k].X = (int)x1;
                                    }
                                }
                                else
                                {
                                    for (k = 0; k < val[j].Length; k++)
                                    {
                                        dif = pro - (factor[j] * val[j][k] / ganan[j]);
                                        diff = dif * fy;
                                        y1 = (float)(iniy - diff);
                                        x1 = (float)(40.0 + (timm[j][k] - tie1) * fax);
                                        dat[k].Y = (int)y1;
                                        dat[k].X = (int)x1;
                                    }
                                }
                            }
                            else
                            {
                                dif = max - pro;
                                diff = dif * fy;
                                fxsat = diff;   // fxsat y fmsat, son variables para la opcion de saturacion.
                                dif = min - pro;
                                diff = dif * fy;
                                fmsat = diff;
                                if (invertido[j] == false)
                                {
                                    for (k = 0; k < val[j].Length; k++)
                                    {
                                        dif = val[j][k] - pro;
                                        diff = dif * fy;
                                        if (diff > fxsat) diff = fxsat;
                                        else if (diff < fmsat) diff = fmsat;
                                        y1 = (float)(iniy - diff);
                                        x1 = (float)(40.0 + (timm[j][k] - tie1) * fax);
                                        dat[k].Y = (int)y1;
                                        dat[k].X = (int)x1;
                                    }
                                }
                                else
                                {
                                    for (k = 0; k < val[j].Length; k++)
                                    {
                                        dif = pro - val[j][k];
                                        diff = dif * fy;
                                        if (diff > fxsat) diff = fxsat;
                                        else if (diff < fmsat) diff = fmsat;
                                        y1 = (float)(iniy - diff);
                                        x1 = (float)(40.0 + (timm[j][k] - tie1) * fax);
                                        dat[k].Y = (int)y1;
                                        dat[k].X = (int)x1;
                                    }
                                }
                            }
                            dc.DrawLines(lapiz, dat);
                        }
                        else siEst[j] = false;
                        jj += 1;
                    }
                }

            }
            catch
            {
            }
            lapiz.Dispose();
            brocha.Dispose();

            if (Cladat[1].X == 0 && Cladat[1].Y == 0) return;
            Graphics dc2 = panel1.CreateGraphics();
            Pen laap = new Pen(Color.Aquamarine, 1);
            i = (int)(Math.Abs(Cladat[1].X - Cladat[0].X));
            if (i < 4) i = 4;
            j = (int)(Math.Abs(Cladat[1].Y - Cladat[0].Y));
            if (j < 2) j = 2;
            dc2.DrawRectangle(laap, Cladat[0].X, Cladat[0].Y, i, j);
            laap.Dispose();

            return;
        }
        /// <summary>
        /// Este método no se esta usando.
        /// </summary>
        void TrazasClasCuentas2()
        {// dibuja las trazas en el panel de clasificacion
            int xf, yf, i, j, jj, k, kk, lar, max, min, pro, tamlet, numtotra;
            int nmi = 0, nmf = 0, tot = 0;
            float x1, y1;
            double dura, fax, fay, fy, diff, dif = 0, iniy, dd1, dd2, fxsat, fmsat;
            double maximo = 0;
            double[] factor = new double[nutra];
            double[] ganan = new double[nutra];
            string esta = "";
            bool[] siesta = new bool[nutra];
            bool si = false;
            Point[] dat;

            for (i = 0; i < nutra; i++) siesta[i] = siEst[i];
            if (boNano.Text == "Nan")
            {
                for (i = 0; i < nutra; i++)
                {
                    if (fcnan[i] <= 0) siesta[i] = false;
                    factor[i] = fcnan[i];
                    ganan[i] = (double)(ga[i]);
                }
            }
            else
            {
                for (i = 0; i < nutra; i++)
                {
                    factor[i] = 1.0;
                    ganan[i] = 1.0;
                }
            }
            panelcladib.BackColor = colfondo;
            panelcladib.Visible = false;
            panelcladib.Visible = true;
            xf = panelcladib.Size.Width - 40;
            yf = panelcladib.Size.Height;
            numtotra = 0;
            for (i = 0; i < nutra; i++)
            {
                if (siesta[i] == true) numtotra += 1;
            }
            // MessageBox.Show("nutra="+nutra.ToString()+" numtotra="+numtotra.ToString()+" id="+id.ToString());
            dura = tie2 - tie1;          // duracion en segundos de la ventana. La duracion de todo el sismo se guarda en la variable durx.
            fax = xf / dura;             // factor en la horizontal
            fay = yf / (numtotra + 0.5); // factor en la vertical
            tamlet = (int)(fay);         // tamaño de las letras para el nombre de las estaciones
            if (tamlet > 10) tamlet = 10;

            Graphics dc = panelcladib.CreateGraphics();
            Pen lapiz = new Pen(colinea, 1);
            SolidBrush brocha = new SolidBrush(colotr1);
            jj = 0;
            try
            {
                for (j = 0; j < nutra; j++)
                {
                    tot = tim[j].Length;
                    lar = (int)((tim[j][tot - 1] - tim[j][0]) * ra[j]); // numero de muestras de la estacion en cuestion
                    dd1 = (tie1 - tim[j][0]) * ra[j];                 // muestra inferior del panel de clasificacion
                    dd2 = (tie2 - tim[j][0]) * ra[j];                 // muestra superior
                    if ((int)(dd1) > lar || dd1 < 0 || dd2 < 0)
                    {
                        siesta[j] = false;
                    }
                }
                for (j = 0; j < nutra; j++)
                {
                    if (siesta[j] == true)
                    {
                        for (k = 0; k < tim[j].Length; k++)
                        {
                            if (tim[j][k] >= tie1) break;// aqui se busca la muestra de la estacion que coincide con el tiempo inicial de la ventana de clasificacion
                        }
                        nmi = k;
                        for (k = nmi; k < tim[j].Length; k++)
                        {
                            if (tim[j][k] > tie2 || tim[j][k] <= 0) break; // aqui se busca la muestra de la estacion, que coincide con el tiempo final de la ventana de clasificacion.
                        }
                        nmf = k - 1;
                        lar = nmf - nmi;
                        // nmi: el numero de muestra inferior
                        // nmf: numero de muestra superior
                        if (lar > 0)
                        {
                            max = cu[j][nmi];
                            min = max; // aqui se va a buscar el valor maximo de cuenta del intervalo de interes.
                            for (k = nmi + 1; k < nmf; k++)
                            {
                                if (max < cu[j][k]) max = cu[j][k];
                                else if (min > cu[j][k]) min = cu[j][k];
                            }
                            if (est[j].Substring(0, 4) != "IRIG")
                            {
                                if (si == true)
                                {
                                    if (maximo < factor[j] * (max - min) / (double)(ga[j])) maximo = factor[j] * (max - min) / (double)(ganan[j]);
                                }
                                else
                                {
                                    maximo = factor[j] * (max - min) / (double)(ganan[j]);
                                    si = true;
                                }
                            }// if est....
                        }// if lar
                    }// if siesta
                }// for j....

                for (j = 0; j < nutra; j++)
                {
                    if (siesta[j] == true)
                    {
                        for (k = 0; k < tim[j].Length; k++)
                        {
                            if (tim[j][k] >= tie1) break;// aqui se busca la muestra de la estacion que coincide con el tiempo inicial de la ventana de clasificacion
                        }
                        nmi = k;
                        for (k = nmi; k < tim[j].Length; k++)
                        {
                            if (tim[j][k] > tie2 || tim[j][k] <= 0) break; // aqui se busca la muestra de la estacion, que coincide con el tiempo final de la ventana de clasificacion.
                        }
                        nmf = k - 1;
                        lar = nmf - nmi;
                        // nmi: el numero de muestra inferior
                        // nmf: numero de muestra superior
                        if (lar > 0)
                        {
                            max = cu[j][nmi];
                            min = max; // aqui se va a buscar el valor maximo de cuenta del intervalo de interes.                            
                            for (k = nmi + 1; k < nmf; k++)
                            {
                                if (max < cu[j][k]) max = cu[j][k];
                                else if (min > cu[j][k]) min = cu[j][k];
                            }
                            //MessageBox.Show("maximo=" + maximo.ToString() + " dif=" + (max - min).ToString()+" gan="+ga[j].ToString()+" fac="+fcnan[j].ToString());
                            max = (int)(factor[j] * max / ganan[j]);
                            min = (int)(factor[j] * min / ganan[j]);
                            pro = (int)((max + min) / 2.0F);
                            if (est[j].Substring(0, 4) != "IRIG") max = (int)(pro + (maximo / 2.0));
                            //if (max - pro != 0) fy = ((fay / 2) / ((max - pro)));
                            if (max - pro != 0) fy = ((fay / 2) / ((max - pro)));
                            else fy = 1.0;
                            //pro = (int)(pro * ampclas);// ampclas guarda el valor 1.0. inicialmente se pretendia guardar el valor de amplificacion escogido por el usuario.
                            iniy = 5 + jj * fay + fay / 2;
                            if (!char.IsLetterOrDigit(est[j][4])) esta = est[j].Substring(0, 4);
                            else esta = est[j];
                            dc.DrawString(esta, new Font("Times New Roman", tamlet, FontStyle.Bold), brocha, 1, (float)(iniy - tamlet));
                            dat = new Point[lar];
                            kk = 0;
                            // los valores de la señal se guardan en un arreglo (dat) y se utiliza la funcion Drawlines, que es mucho mas rapida.
                            if (satu == false)
                            {
                                if (invertido[j] == false)
                                {
                                    for (k = nmi; k < nmf; k++)
                                    {
                                        if (kk >= lar) break;
                                        dif = factor[j] * cu[j][k] / ganan[j] - pro;
                                        diff = dif * fy;
                                        y1 = (float)(iniy - diff);
                                        x1 = (float)(40.0 + (tim[j][k] - tie1) * fax);
                                        dat[kk].Y = (int)y1;
                                        dat[kk].X = (int)x1;
                                        kk += 1;
                                    }
                                }
                                else
                                {
                                    for (k = nmi; k < nmf; k++)
                                    {
                                        if (kk >= lar) break;
                                        dif = pro - (factor[j] * cu[j][k] / ganan[j]);
                                        diff = dif * fy;
                                        y1 = (float)(iniy - diff);
                                        x1 = (float)(40.0 + (tim[j][k] - tie1) * fax);
                                        dat[kk].Y = (int)y1;
                                        dat[kk].X = (int)x1;
                                        kk += 1;
                                    }
                                }
                            }
                            else
                            {
                                dif = max - pro;
                                diff = dif * fy;
                                fxsat = diff;   // fxsat y fmsat, son variables para la opcion de saturacion.
                                dif = min - pro;
                                diff = dif * fy;
                                fmsat = diff;
                                if (invertido[j] == false)
                                {
                                    for (k = nmi; k < nmf; k++)
                                    {
                                        if (kk >= lar) break;
                                        dif = cu[j][k] - pro;
                                        diff = dif * fy;
                                        if (diff > fxsat) diff = fxsat;
                                        else if (diff < fmsat) diff = fmsat;
                                        y1 = (float)(iniy - diff);
                                        x1 = (float)(40.0 + (tim[j][k] - tie1) * fax);
                                        dat[kk].Y = (int)y1;
                                        dat[kk].X = (int)x1;
                                        kk += 1;
                                    }
                                }
                                else
                                {
                                    for (k = nmi; k < nmf; k++)
                                    {
                                        if (kk >= lar) break;
                                        dif = pro - cu[j][k];
                                        diff = dif * fy;
                                        if (diff > fxsat) diff = fxsat;
                                        else if (diff < fmsat) diff = fmsat;
                                        y1 = (float)(iniy - diff);
                                        x1 = (float)(40.0 + (tim[j][k] - tie1) * fax);
                                        dat[kk].Y = (int)y1;
                                        dat[kk].X = (int)x1;
                                        kk += 1;
                                    }
                                }
                            }
                            dc.DrawLines(lapiz, dat);
                        }
                        else siEst[j] = false;
                        jj += 1;
                    }
                }

            }
            catch
            {
            }
            lapiz.Dispose();
            brocha.Dispose();

            if (Cladat[1].X == 0 && Cladat[1].Y == 0) return;
            Graphics dc2 = panel1.CreateGraphics();
            Pen laap = new Pen(Color.Aquamarine, 1);
            i = (int)(Math.Abs(Cladat[1].X - Cladat[0].X));
            if (i < 4) i = 4;
            j = (int)(Math.Abs(Cladat[1].Y - Cladat[0].Y));
            if (j < 2) j = 2;
            dc2.DrawRectangle(laap, Cladat[0].X, Cladat[0].Y, i, j);
            laap.Dispose();

            return;
        }
        // las rutinas siguientes, tienen que ver con el panel principal, el cual visualiza la señal de
        // la estacion activa:
        /// <summary>
        /// Controla el estado de la variable sipro la cual indica el promedio de la señal, además
        /// cambia el color del botón boprom dependiendo del estado de dicha variable.
        /// </summary>
        void Promedio()
        {// boton para seleccionar el promedio de la señal, el cual sirve como cero de referencia.
            if (boprom.Visible == false)
                return;
            if (sipro == 0)
            {
                sipro = 1;
                boprom.BackColor = Color.IndianRed;
            }
            else
            {
                sipro = 0;
                boprom.BackColor = Color.White;
                panel1.Invalidate();
            }
        }
        /// <summary>
        /// Lanza el método Promedio().
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void boprom_Click(object sender, EventArgs e)
        {
            Promedio();
        }
        /// <summary>
        /// Lanza el método Subir(MouseEventArgs e).
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento MouseEventArgs que se lanzó.</param>
        private void bosube_MouseDown(object sender, MouseEventArgs e)
        {// desplaza la señal hacia arriba.
            Subir(e);
        }
        /// <summary>
        /// Desplaza la señal hacia arriba.
        /// </summary>
        /// <param name="e">El evento MouseEventArgs que se lanzó.</param>
        void Subir(MouseEventArgs e)
        {
            if (e == null)
                incy -= 1;
            else
            {
                if (e.Button == MouseButtons.Left)
                    incy -= 1;
                else
                    incy -= 15;
            }
            panel1.Invalidate();
        }
        /// <summary>
        /// Lanza el método Bajar(MouseEventArgs e).
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento MouseEventArgs que se lanzó.</param>
        private void bobaja_MouseDown(object sender, MouseEventArgs e)
        {// desplaza la señal hacia abajo
            Bajar(e);
        }
        /// <summary>
        /// Desplaza la señal hacia abajo.
        /// </summary>
        /// <param name="e">El evento MouseEventArgs que se lanzó.</param>
        void Bajar(MouseEventArgs e)
        {
            if (e == null)
                incy += 1;
            else
            {
                if (e.Button == MouseButtons.Left) incy += 1;
                else
                    incy += 15;
            }
            panel1.Invalidate();
        }
        /// <summary>
        /// Aumenta el valor de la variable ampli, amplificando el tamaño de la señal en el panel principal.
        /// </summary>
        /// <param name="e">El evento MouseEventArgs que se lanzó.</param>
        void Aumentar(MouseEventArgs e)
        {
            if (e == null)
                ampli = 1.2F * ampli;
            else
            {
                if (e.Button == MouseButtons.Left)
                    ampli = 1.2F * ampli;
                else
                    ampli = 2.0F * ampli;
            }
            if (ampli != 1.0F)
                boUno.BackColor = Color.Tomato;
            else
                boUno.BackColor = Color.MistyRose;
            panel1.Invalidate();
            return;
        }
        /// <summary>
        /// Lanza el método Aumentar(MouseEventArgs e)
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento MouseEventArgs que se lanzó.</param>
        private void boaum_MouseDown(object sender, MouseEventArgs e)
        {// aumenta la amplitud de la señal
            Aumentar(e);
        }
        /// <summary>
        /// Disminuye el valor de la variable ampli, disminuyendo el tamaño de la señal en el panel principal.
        /// </summary>
        /// <param name="e">El evento MouseEventArgs que se lanzó.</param>
        void Disminuir(MouseEventArgs e)
        {
            if (e == null)
                ampli = 0.8F * ampli;
            else
            {
                if (e.Button == MouseButtons.Left)
                    ampli = 0.8F * ampli;
                else
                    ampli = 0.5F * ampli;
            }
            if (ampli != 1.0F)
                boUno.BackColor = Color.Tomato;
            else
                boUno.BackColor = Color.MistyRose;
            panel1.Invalidate();
        }
        /// <summary>
        /// Lanza el método Disminuir(MouseEventArgs e)
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento MouseEventArgs que se lanzó.</param>
        private void bodis_MouseDown(object sender, MouseEventArgs e)
        {// disminuye la amplitud de la señal
            Disminuir(e);
        }
        /// <summary>
        /// Lanza el Uno()
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento MouseEventArgs que se lanzó.</param>
        private void boUno_Click(object sender, EventArgs e)
        {// vuelve la amplitud de la señal a las condiciones iniciales.
            Uno();
        }
        /// <summary>
        /// Modifica el valor de la varible ampli haciendolo = 1.0f (condición inicial).
        /// </summary>
        void Uno()
        {
            ampli = 1.0F;
            boUno.BackColor = Color.MistyRose;
            panel1.Invalidate();
            return;
        }
        // ************************************
        /// <summary>
        /// Hace llamado a la rutina fecha(), la cual visualiza el panel para seleccionar la fecha inicial y final.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void butfech_Click(object sender, EventArgs e)
        {
            NoMostrar = true;
            yaInterp = false;
            if (panelParti.Visible == true)
                panelParti.Visible = false;
            suma = 0;
            fecha();

            return;
        }
        /// <summary>
        ///  Panel de dialogo para entrar la fecha inicial y final, asi como la duración de la traza 
        ///  dependiendo la estación escogida, Panel Principal.
        /// </summary>
        void fecha()
        {
            int an, me, dia, du, xx, yy;
            string ca = "", feant1 = "", feant2 = "";
            diag1 di = new diag1();

            di.Fech1 = fe1;
            di.Fech2 = fe2;
            feant1 = fe1;
            feant2 = fe2;
            du = totven;
            di.Dura = du.ToString();
            di.Usua = usu;
            if (di.ShowDialog() == DialogResult.OK)
            {
                fe1 = di.Fech1;
                fe2 = di.Fech2;
                usu = di.Usua;
                an = int.Parse(fe1.Substring(0, 4));
                me = int.Parse(fe1.Substring(4, 2));
                dia = int.Parse(fe1.Substring(6, 2));
                DateTime fech1 = new DateTime(an, me, dia);
                ll1 = fech1.Ticks;
                an = int.Parse(fe2.Substring(0, 4));
                me = int.Parse(fe2.Substring(4, 2));
                dia = int.Parse(fe2.Substring(6, 2));
                DateTime fech2 = new DateTime(an, me, dia, 23, 59, 0);
                ll2 = fech2.Ticks;
                totven = ushort.Parse(di.Dura);
                LLenaBox(); // rutina que selecciona el tiempo y los sismos clasificados de la Base.               
                estado = false;
            }
            else
            {
                if (inicio == true) return;
                boDisparo.Visible = true;
                textBoxUT.Visible = true;
                textBoxDisparo.Visible = true;
                boTar.Visible = false;
                butfech.Visible = false;
                butsigue.Visible = false;
                boUnaCla.Visible = false;
                boTremor.Visible = false;
                bofintrem.Visible = false;
                disparo = true;
            }
            if (feant1 != fe1 || feant2 != fe2)
                util.borra(panel1, colfondo);
            else
                panel1.Invalidate();
            if (panelcla.Visible == true)
            {
                xx = panelcla.Size.Width - 30;
                yy = 1;
                if (usu.Length == 3)
                    ca = usu.Substring(0, 3);
                else ca = "___";
                util.Letreros(panelcla, xx, yy, ca, Color.LightYellow);
                DibujoTrazas();
            }
            return;
        }
        /// <summary>
        /// Presta la funcionalidad donde se puede escoger la duración en minutos de las lineas de
        /// visualización, el espaciamiento entre lineas, la duración total de visualización
        /// y el tamaño de las pepas.
        /// </summary>
        void Parametros()
        {
            int i, totvenant;
            double ff, ff2;
            bool cond = false;
            diag2 di = new diag2();

            try
            {
                totvenant = totven;
                ff2 = tam;
                ff = dur / 60.0;
                di.Lin = ff.ToString();
                di.Ven = totven.ToString();
                di.Esp = esp.ToString();
                di.Tampepa = tam.ToString();
                if (di.ShowDialog() == DialogResult.OK)
                {
                    ff = float.Parse(di.Lin); // duracion en minutos de una linea
                    dur = (float)(ff * 60.0F);
                    totven = ushort.Parse(di.Ven); // duracion en segundos de la ventana
                    esp = ushort.Parse(di.Esp); // separacion en pixeles de las lineas
                    tam = float.Parse(di.Tampepa); // tamaño en pixeles de las pepas
                    cond = true;
                }
                if (cond == true)
                {
                    if (totven != totvenant)
                    {
                        i = listBox1.SelectedIndex;
                        if (i < 0)
                            return;
                        listBox1.SetSelected(i, true);
                    }
                    if (listBox2.Visible == true)
                        panel1.Invalidate();
                }
            }
            catch
            {
                tip.IsBalloon = true;
                tip.InitialDelay = 0;
                tip.ReshowDelay = 4000;
                tip.AutomaticDelay = 0;
                tip.SetToolTip(panel1, "Formato incorrecto?? Revise");
            }

            return;
        }
        /// <summary>
        /// Lanza el método Parametros().
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void butparam_Click(object sender, EventArgs e)
        {
            NoMostrar = true;
            Parametros();
        }
        /// <summary>
        /// Rutina que permite visualizar la continuación en el tiempo de la traza de la estación seleccionada, cambia el índice del listBox de las estaciones.
        /// </summary>
        /// <param name="cond">Si es true hace el cálculo de la estación siguiente, en caso sontrario usa el parámetro ii.</param>
        /// <param name="ii">Índice de la estación siguiente a la estación a la que se le esta leyendo la traza.</param>
        void Seguir(bool cond, int ii)
        {// Rutina que permite visualizar la continuacin en el tiempo de la traza de la estactión 
            // seleccionada
            int i;

            if (cond == false)
            {
                listBox1.Focus();
                if (listBox1.SelectedIndex < 0)
                    return;
                i = listBox1.SelectedIndex + totven - 1;
                if (i < listBox1.Items.Count)
                    listBox1.SelectedIndex = i;
            }
            else
            {
                listBox1.SelectedIndex = ii;
            }
            return;
        }
        /// <summary>
        /// Lanza el método Seguir(bool cond, int ii)
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void butsigue_MouseDown(object sender, MouseEventArgs e)
        {// se llama a la rutina anterior
            if (panelcla.Visible == true)
                panelcla.Visible = false;
            Seguir(false, 0);
        }
        /// <summary>
        /// Permite visualizar la extensión de tiempo de la traza de los archivos ya clasificados.
        /// </summary>
        /// <param name="e"></param>
        void Archivo(MouseEventArgs e)
        {// Boton que permite visualizar la extension en la traza de los archivos clasificados. Si se activa
            // con el boton derecho, se actualiza los datos de clasificación y lecturas.
            // se llama a la rutina (util.Verarchi) que visualiza los archivos ya clasificados.

            if (butarch.Visible == false)
                return;
            if (e == null)
            {
                siArch = true;
                util.Leerbase(panel2, rutbas, ll1, ll2, cl, volcan);
                // rutina que escribe en los archivos datos.txt y amplis.txt los datos respectivos en caso de que existan sismos clasificados
                //sihayclas = Reviarch(); // rutina que revisa si hay archivos clasificados y devuelve verdadero o falso
                util.VerArchi(panel1, timin, tim[id], tiar, duar, esp, dur, contarch); // dibuja un rectangulo correspondiente al intervalo de tiempo del sismo clasificado.
                butarch.BackColor = Color.Gold;
                return;
            }
            if (siArch == true)
            {
                if (e.Button == MouseButtons.Right) // permite actualizar los datos.
                {
                    util.Leerbase(panel2, rutbas, ll1, ll2, cl, volcan); // rutina que escribe en los archivos datos.txt y amplis.txt los datos respectivos en caso de que existan sismos clasificados
                    sihayclas = Reviarch(); // rutina que revisa si hay archivos clasificados y devuelve verdadero o falso
                    util.VerArchi(panel1, timin, tim[id], tiar, duar, esp, dur, contarch); // dibuja un rectangulo correspondiente al intervalo de tiempo del sismo clasificado.
                }
                else
                {
                    siArch = false;
                    butarch.BackColor = Color.White;
                }
                boEliClas.Visible = false;
            }
            else
            {
                siArch = true;
                butarch.BackColor = Color.Gold;
                if (e.Button == MouseButtons.Right)
                {
                    util.Leerbase(panel2, rutbas, ll1, ll2, cl, volcan);
                    sihayclas = Reviarch();
                }
                util.VerArchi(panel1, timin, tim[id], tiar, duar, esp, dur, contarch);
                if (sihayclas == true)
                    boEliClas.Visible = true;
            }

            return;
        }
        /// <summary>
        /// Lanza el método Archivo(MouseEventArgs e)
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void butarch_MouseDown(object sender, MouseEventArgs e)
        {
            Archivo(e);
        }
        /// <summary>
        /// Rutina que revisa los archivos de clasificación con el fin de averiguar los intervalos
        /// de tiempo que se encuentran clasificados. En Datos.txt se encuentran los tiempos
        /// en formato visual c#.
        /// </summary>
        /// <returns>Retorna true en caso de encontrar trazas ya clasificadas, false en caso de no encontrar trazas clasificadas.</returns>
        bool Reviarch()
        {// rutina que revisa los archivos de clasificacion con el fin de averiguar los intervalos
            // de tiempo que se encuentran clasificados. En Datos.txt se encuentran los tiempos
            // en formato visual c#
            ushort i;
            int to;
            double t1, t2, ti1, ti2;
            string lin = "";
            bool si = false;

            if (!File.Exists("datos.txt"))
            {
                siArch = false;
                butarch.BackColor = Color.White;
                return (false);
            }
            to = tim[id].Length;
            t1 = tim[id][0]; // tiempo inicial de la traza de la estacion activa.
            t2 = tim[id][to - 1]; // tiempo final de la traza activa.
            if (t2 < t1)
            {
                NoMostrar = true;
                MessageBox.Show("Tiempo Final estacion inferior a Inicio???");
                return (false);
            }
            StreamReader ar = new StreamReader("datos.txt"); // archivo en que se encuentra los datos de clasificaciones
            contarch = 0;
            while (lin != null)
            {
                try
                {
                    lin = ar.ReadLine();
                    if (lin == null)
                        break;
                    ti1 = (double.Parse(lin.Substring(34, 19)) - Feisuds) / 10000000.0;
                    ti2 = ti1 + double.Parse(lin.Substring(29, 5));
                    //MessageBox.Show("ti1="+ti1.ToString()+"ti2="+ti2.ToString());
                    if (ti1 >= t1 && ti2 <= t2)
                        contarch += 1;// si existe archivos clasificados en el intervalo visualizado de la traza activa.
                }
                catch
                {
                    break;
                }
            }
            ar.Close();
            if (contarch == 0)
            {
                siArch = false;
                butarch.BackColor = Color.White;
                butarch.Visible = false;
                return (false);
            }
            else
            {
                tiar = new double[contarch]; // guarda los tiempos iniciales de los sismos clasificados que esten dentro de la visualizacion de la traza activa.
                duar = new ushort[contarch]; // duraciones de los sismos clasificados.
                nomar = new string[contarch]; // nombres de los sismos clasificados.
                lin = "";
                StreamReader ar2 = new StreamReader("datos.txt");
                i = 0;
                while (lin != null)
                {
                    try
                    {
                        lin = ar2.ReadLine();
                        if (lin == null)
                            break;
                        ti1 = (double.Parse(lin.Substring(34, 19)) - Feisuds) / 10000000.0;
                        ti2 = ti1 + double.Parse(lin.Substring(29, 5));
                        if (ti1 >= t1 && ti2 <= t2)
                        {
                            tiar[i] = ti1;
                            duar[i] = ushort.Parse(lin.Substring(29, 5));
                            nomar[i] = lin.Substring(16, 12);
                            i += 1;
                            if (i > contarch)
                            {
                                NoMostrar = true;
                                MessageBox.Show("Problemas con contarch!!!");
                                butarch.Visible = false;
                                siArch = false;
                                butarch.BackColor = Color.White;
                                return (false);
                            }
                        }
                    }
                    catch
                    {
                        break;
                    }
                }
                ar2.Close();
            }
            if (contarch > 0)
                butarch.Visible = true;
            else
                butarch.Visible = false;
            siArch = false;
            butarch.BackColor = Color.White;
            si = butarch.Visible;

            return (si);
        }
        /// <summary>
        /// Se regresa al tiempo inicial por defecto.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void boini_Click(object sender, EventArgs e)
        {//se vuelve al tiempo tiempo inicial por defecto
            int i;
            timin = tim[id][0];
            for (i = 0; i < nutra; i++)
                if (timin > tim[i][0])
                    timin = tim[i][0];
            boini.BackColor = Color.PaleGoldenrod;
            panel1.Invalidate();
            return;
        }
        /// <summary>
        /// Oculta o hace visibles las marcas de tiempo en las trazas.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void botim_Click(object sender, EventArgs e)
        {// coloca o esconde las marcas de tiempo
            int denom;
            if (marcati == true)
            {
                marcati = false;
                botim.BackColor = Color.White;
                panel1.Invalidate();
            }
            else
            {
                marcati = true;
                botim.BackColor = Color.Orange;
                denom = (int)(Math.Abs(Math.Ceiling((tim[id][cu[id].Length - 1] - timin) / dur)));
                if (denom <= 0)
                    denom = 1;
                util.MarcaTiempo(panel1, timin, tim[id], esp, dur, denom);
            }
            return;
        }
        /// <summary>
        /// Oculta o hace visibles las pepas que indican donde se clasificó un sismo.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void bopep_MouseDown(object sender, MouseEventArgs e)
        {// coloca o esconde las Pepas si las hay
            int denom;

            if (e.Button == MouseButtons.Left)
            {
                if (pepas == true)
                {
                    pepas = false;
                    bopep.BackColor = Color.White;
                    boPepVol.BackColor = Color.White;
                    panel1.Invalidate();
                }
                else
                {
                    pepas = true;
                    bopep.BackColor = Color.YellowGreen;
                    pepasvol = false;
                }
                panel1.Invalidate();
            }
            else
            {
                LeerAmplitud();
                denom = (int)(Math.Abs(Math.Ceiling((tim[id][cu[id].Length - 1] - timin) / dur)));
                if (denom <= 0) denom = 1;
                util.PonePepas(panel1, timin, tim[id], esp, dur, contampl, valampl, clR, clG, clB,
                    letampl, cl, tam, nucla, siPampl, volampl, volcan[vol][0], pepasvol, nolec,
                    tigrabacion, leclec, denom);
            }

            return;
        }
        /// <summary>
        /// Hace solo visibles las pepas de las trazas clasificadas que pertenezcan al volcán activo en el momento.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void boPepVol_MouseDown(object sender, MouseEventArgs e)
        {// coloca solo las pepas correspondientes al volcan de la estacion activada
            if (pepasvol == false)
            {
                pepas = false;
                pepasvol = true;
                bopep.BackColor = Color.White;
                boPepVol.BackColor = Color.YellowGreen;
            }
            else
            {
                pepas = true;
                pepasvol = false;
                bopep.BackColor = Color.YellowGreen;
                boPepVol.BackColor = Color.White;
            }
            panel1.Invalidate();

            return;
        }
        /// <summary>
        /// Desplaza la traza a la derecha.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void boder_MouseDown(object sender, MouseEventArgs e)
        {// desplaza la traza a la derecha
            if (e.Button == MouseButtons.Left) timin -= 5.0;
            else timin -= 25.0;
            if (tim[id][0] - timin >= dur) timin = tim[id][0] - dur + 2.0;
            boini.BackColor = Color.Goldenrod;
            panel1.Invalidate();
            return;
        }
        /// <summary>
        /// Desplaza la traza a la izquierda.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void boizq_MouseDown(object sender, MouseEventArgs e)
        {// desplaza la traza a izquierda
            if (e.Button == MouseButtons.Left) timin += 5.0;
            else timin += 25.0;
            boini.BackColor = Color.Goldenrod;
            panel1.Invalidate();
            return;
        }
        /// <summary>
        /// Rutina que presenta un rectángulo indicando la duración de los archivos de "tremor",
        /// con el fin que el usuario realice las lecturas de amplitud, dentro de dicho rectángulo,
        /// el cual se va desplazando consecutivamente y automáticamente, una vez se realicen dichas lecturas.
        /// </summary>
        void Cuadro_Tremor()
        {
            int jj, xf, yf, b1, b2, lar;
            float fax, fay, h, w, y1, x1, x2, y2;
            double incre;

            if (tremor == false)
                return;

            if (tremofin == false)
                incre = incTremor; // tiempo de duracion de los archivos.
            else
                incre = tifintremor - tinitremor;
            if (incre <= 0)
            {
                //MessageBox.Show("incre=" + incre.ToString() + "???");
                return;
            }
            xf = panel1.Size.Width;
            yf = panel1.Size.Height;
            lar = tim[id].Length;
            jj = 1 + (int)((tim[id][lar - 1] - timin) / dur);
            if (esp == 0)
                fay = (yf - 45) / jj;
            else
                fay = esp;
            fax = xf / dur;

            Graphics dc = panel1.CreateGraphics();
            Pen lap = new Pen(Color.Orange, 2);

            b1 = (int)((tinitremor - timin) / dur);
            //MessageBox.Show("b1=" + b1.ToString()+" tinitremor="+tinitremor.ToString()+" timin="+timin.ToString());
            y1 = (float)(45.0 + b1 * fay + fay / 3.0);
            b2 = (int)(((tinitremor + incre) - timin) / dur);
            y2 = (float)(45.0 + b2 * fay + fay / 3.0);
            h = (float)(fay / 3.0);
            x1 = (float)(((tinitremor - timin) - b1 * dur) * fax);
            x2 = (float)((((tinitremor + incre) - timin) - b2 * dur) * fax);
            if (y1 != y2)
            {
                w = xf - x1;
                dc.DrawRectangle(lap, x1, y1, w, h);
                w = x2;
                dc.DrawRectangle(lap, -1, y2, w, h);
            }
            else
            {
                w = x2 - x1;
                dc.DrawRectangle(lap, x1, y1, w, h);
            }
            //if (y1 != y2) MessageBox.Show("x1="+x1.ToString()+" y1="+y1.ToString()+" x2="+x2.ToString()+" y2="+y2.ToString());

            //MessageBox.Show("x1=" + x1.ToString() + " y1=" + y1.ToString() + " x2=" + x2.ToString() + " y2=" + y2.ToString());
            lap.Dispose();

            return;
        }
        /// <summary>
        /// Se encarga de calcular los valores de las variables para el posterior cálculo del espectro y 
        /// posicionar la gráfica del mismo.
        /// </summary>
        /// <param name="pan">Panel principal donde se están mostrando las trazas, puede ser el panel principal panel1 o el panel secundario panel1a.</param>
        /// <param name="panelBar">Panel donde se grafica el espectro.</param>
        /// <param name="id">El id de la estación de la traza a la que se desea calcular el espectro.</param>
        /// <param name="e">Evento de mouse que se genera al dar click sobre el panel.</param>
        void CalcularEspectro(Panel pan, Panel panelBar, ushort id, MouseEventArgs e)
        {
            int xf, yf, jb, jj, j2;
            double t2, fax, fay;

            if (VerEspectro == false)
                return;
            if (estado == false)
                return; // la variable estado es false si no existe ninguna lectura de trazas en memoria.

            xf = pan.Size.Width;
            yf = pan.Size.Height;
            jb = tim[id].Length - 1; // tiempo de la ultima muestra
            if (tim[id][jb - 1] <= 0)
            {
                do
                {
                    jb -= 1;
                    if (tim[id][jb - 1] > 0)
                        break;
                } while (jb > 0);
                jb -= 1;
            }
            if (jb < 2)
                return;
            jj = 1 + (int)((tim[id][jb] - timin) / dur);
            if (esp == 0) // esp es la variable que guarda el espaciamiento entre lineas.
                fay = (yf - 45.0) / jj;
            else
                fay = esp;
            fax = dur / xf;
            j2 = (int)((e.Y - 45.0) / fay);
            t2 = (timin + e.X * fax + j2 * dur);
            if (t2 < tim[id][0])
                return;

            xesp = (short)(e.X);
            yesp = (short)(e.Y);
            t1esp = t2;
            Espectro(pan, panelBar, id, true);
        }
        /// <summary>
        /// Determina la posición inicial del espectro y el tiempo desde donde se empieza a calcular.
        /// </summary>
        /// <param name="panelBar">Panel que se usa para pasar como parámetro del método Espectro() que llama al final del método.</param>
        /// <param name="idc">Es la traza escogida del panel de clasificación usada para el espectro.</param>
        /// <param name="e">Es el evento que desencadena la ejecución del método, este parámetro es usado para determinar
        /// la posición donde se desencadeno el evento.</param>
        void CalcularEspectroCla(Panel panelBar, ushort idc, MouseEventArgs e)
        {
            int xf, yf, jb;
            double t2, fax;

            if (VerEspectro == false || e.X < 40)
                return;
            if (estado == false)
                return; // la variable estado es false si no existe ninguna lectura de trazas en memoria.

            xf = panelcladib.Width - 40;
            yf = panelcladib.Height;
            jb = tim[idc].Length - 1; // número de la penultima muestra de tiempo de la traza.
            if (tim[idc][jb - 1] <= 0)
            {
                do
                {
                    jb -= 1;
                    if (tim[idc][jb - 1] > 0)
                        break;
                } while (jb > 0);
                jb -= 1;
            }
            if (jb < 2)
                return;
            fax = (tie2 - tie1) / xf;
            t2 = (tie1 + (e.X - 40) * fax);
            if (t2 < tim[idc][0])
                return;
            xesp = (short)(e.X);
            yesp = (short)(e.Y);
            t1esp = t2;
            Espectro(panelcladib, panelBar, idc, true);
        }
        /// <summary>
        /// Lanza el método para calcular el espectro dentro del panel de clasificación secundario.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void panel1a_MouseDown(object sender, MouseEventArgs e)
        {
            CualPanel = 1;
            bxi = e.X;
            byi = e.Y;
            if (VerEspectro == true)
            {
                CalcularEspectro(panel1a, panelBarEsp1a, ida, e);
                moveresp = true;
            }
            textBox1.Text = string.Format("{0:00.00}", Fcx1);
            textBox2.Text = string.Format("{0:00.00}", Fcx2);
        }
        /// <summary>
        /// Lanza el método  CalcularEspectro(panel1a, panelBarEsp1a, ida, e).
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void panel1a_MouseMove(object sender, MouseEventArgs e)
        {
            if (moveresp == false)
                return;
            CalcularEspectro(panel1a, panelBarEsp1a, ida, e);
        }
        /// <summary>
        /// (panel1a) Aquí se busca saber si hay arrastre del mouse sobre la traza. Si se activa el botón izquierdo y se
        /// desplaza el mouse, se entra al panel de clasificación (si no se arrastra, no se hace nada
        /// por el momento). Con el botón derecho, se indica el tiempo, absoluto si no hay arrastre o
        /// el intervalo correspondiente al arrastre.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void panel1a_MouseUp(object sender, MouseEventArgs e)
        {
            int i, jj, jb, j1, j2, xf, yf, byf;
            double t1, t2, fax, fay;
            string ca = "";

            moveresp = false;

            if (estado == false)
                return; // la variable estado es false si no existe ninguna lectura de trazas en memoria.
            bxf = e.X;
            byf = e.Y;
            xf = panel1a.Size.Width;
            yf = panel1a.Size.Height;
            jb = tim[ida].Length - 1; // tiempo de la ultima muestra
            if (tim[ida][jb - 1] <= 0)
            {
                do
                {
                    jb -= 1;
                    if (tim[ida][jb - 1] > 0)
                        break;
                } while (jb > 0);
                jb -= 1;
            }
            if (jb < 2)
                return;
            jj = 1 + (int)((tim[ida][jb] - timin) / dur);
            if (esp == 0)
                fay = (yf - 45.0) / jj; // esp es la variable que guarda el espaciamiento entre lineas.
            else
                fay = esp;
            fax = dur / xf;
            j1 = (int)((byi - 45.0) / fay);
            t1 = timin + bxi * fax + j1 * dur;
            j2 = (int)((byf - 45.0) / fay);
            t2 = (timin + bxf * fax + j2 * dur);

            if (VerEspectro == true)
            {
                yesp = (short)(e.Y);
                xesp = (short)(e.X);
                moveresp = false;
                return;
            }
            tie1 = t1;
            tie2 = t2;
            if (sipro == 1)
            {
                sipro = 2;
                boprom.BackColor = Color.SpringGreen;
                p1 = (int)((t1 - tim[ida][0]) * ra[ida]);
                p2 = (int)((t2 - tim[ida][0]) * ra[ida]);
                if (p1 < 0)
                    p1 = 0;
                if (p2 <= 0)
                {
                    sipro = 1;
                    boprom.BackColor = Color.IndianRed;
                }
                else if (tremor == false)
                    panel1a.Invalidate();
                return;
            }
            //MessageBox.Show("tie1="+tie1.ToString()+" tie2="+tie2.ToString()+" clSola="+clSola.ToString());
            if (tie1 != tie2)
            {
                if (File.Exists("datos.txt"))
                    YaClasificados();
                if (clSola == -1)
                {
                    //MessageBox.Show("1");
                    if ((tie2 - tie1) < 10.0)
                    {
                        NoMostrar = true;
                        ca = "VENTANA MENOR DE 10 SEGUNDOS!!!\nNO se GRABA en la BASE sismos con\nDuracion Menor a 10 Segundos\n\nSALIR??";
                        DialogResult result = MessageBox.Show(ca,
                                                     "Duracion Menor de 10 Segundos", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.Yes)
                            return;
                    }
                    Cladat[0].X = bxi;
                    Cladat[0].Y = byi;
                    Cladat[1].X = e.X;
                    Cladat[1].Y = e.Y;
                    //MessageBox.Show("0.X="+Cladat[0].X.ToString()+" 0.Y="+Cladat[0].Y.ToString()+" 1.X="+Cladat[1].X.ToString()+" 1.Y="+Cladat[1].Y.ToString());
                    if (loscajones == false)
                        for (i = 0; i < nutra; i++)
                            siEst[i] = true;
                    panelcla.BringToFront();
                    Clasificar();
                }
            }
            else
            {
                Graphics dc1 = panel1.CreateGraphics();
                Graphics dc1a = panel1a.CreateGraphics();
                Pen lap = new Pen(Color.Orange, 2);
                Pen lap2 = new Pen(Color.Red, 2);
                dc1.DrawLine(lap, e.X, 0, e.X, panel1.Height);
                dc1a.DrawLine(lap, e.X, 0, e.X, panel1a.Height);
                dc1.DrawLine(lap2, e.X - 10, e.Y, e.X + 10, e.Y);
                dc1a.DrawLine(lap2, e.X - 10, e.Y, e.X + 10, e.Y);
                lap.Dispose();
            }
            return;
        }
        /// <summary>
        /// Lanza el método CalcularEspectro() para el panel de clasificación principal (panel1).
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (moveresp == false)
                return;
            CalcularEspectro(panel1, panelBarEsp1, id, e);
        }
        /// <summary>
        /// Lanza el método CalcularEspectro() en el panel1 e identifica el panel1 como el panel en el que
        /// se esta realizando la clasificación en ese momento.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {// el panel1 es donde se dibuja la traza activa.
            CualPanel = 0;
            bxi = e.X;
            byi = e.Y;
            if (VerEspectro == true)
            {
                CalcularEspectro(panel1, panelBarEsp1, id, e);
                moveresp = true;
            }
            /* else if (interpol == true)
             {
                 panelInterP.Visible = true;
             }*/
            return;
        }
        /// <summary>
        /// (panel1) Aquí se busca saber si hay arrastre del mouse sobre la traza. Si se activa el botón izquierdo y se
        /// desplaza el mouse, se entra al panel de clasificación (si no se arrastra, no se hace nada
        /// por el momento). Con el botón derecho, se indica el tiempo, absoluto si no hay arrastre o
        /// el intervalo correspondiente al arrastre.
        /// 
        /// (El nivel de detalle del proceso de este método en el archivo de contratos es bajo.)
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {

            int xf, yf, i, j, k, nuar, j1, j2, jb, jj, byf, dfcu, mmx, mmn;
            double fax, fay, fcra;
            long tii1 = 0;
            double dif = 0, t1, t2, ttt;
            string ss = "", lin = "", ee = "", ca = "";
            bool si = false;


            NoMostrar = true;
            moveresp = false;
            if (panel2.Visible == true)
            {
                panel2.Size = new Size(219, 74);
                panel2.Location = new Point(243, 254);
                panel2.Visible = false;
            }
            if (estado == false)
                return; // la variable estado es false si no existe ninguna lectura de trazas en memoria.
            bxf = e.X;
            byf = e.Y;
            xf = panel1.Size.Width;
            yf = panel1.Size.Height;
            jb = tim[id].Length - 1; // tiempo de la ultima muestra
            if (tim[id][jb - 1] <= 0)
            {
                do
                {
                    jb -= 1;
                    if (tim[id][jb - 1] > 0)
                        break;
                } while (jb > 0);
                jb -= 1;
            }
            if (jb < 2)
                return;
            jj = 1 + (int)((tim[id][jb] - timin) / dur);
            if (esp == 0)
                fay = (yf - 45.0) / jj; // esp es la variable que guarda el espaciamiento entre lineas.
            else
                fay = esp;
            fax = dur / xf;
            j1 = (int)((byi - 45.0) / fay); // se calcula cuantas lineas existen antes de la actual.
            t1 = timin + bxi * fax + j1 * dur; // se tiene en cuenta el tiempo de las lineas anteriores
            j2 = (int)((byf - 45.0) / fay);
            t2 = (timin + bxf * fax + j2 * dur);
            if (t2 < t1)
            {
                ttt = t1;
                t1 = t2;
                t2 = ttt;
            }
            // t1 es el tiempo inical del intervalo y t2 el tiempo final.
            tii1 = (long)(Fei + (t1 * 10000000.0));

            if (DR > 0)
            {
                if (fcnan[id] <= 0)
                {
                    ss = "NO hay Factor!..";
                    tip.IsBalloon = false;
                    tip.AutoPopDelay = 3000;
                    tip.SetToolTip(boDR, ss);
                    panelDR.Visible = false;
                    DR = 0;
                    boDR.BackColor = Color.Gold;
                    return;
                }
                tDR1 = t1;
                tDR2 = t2;
                if (checkBoxHz.Checked == true && DR == 1)
                    PromedioFiltrado();
                panelDR.Visible = true;
                panelDR.BringToFront();
                panelDR.Invalidate();
                return;
                /*if (DR == 1)
                {                    
                    if (checkBoxHz.Checked == true) PromedioFiltrado();
                    panelDR.Visible = true;
                    panelDR.BringToFront();
                    panelDR.Invalidate();
                    return;
                }
                else
                {                    
                    DibujoVelocidadDR(t1, t2);                    
                }*/
            }

            if (vista == true)
            {
                try
                {
                    splitContainer1.Visible = true;
                    splitContainer1.Location = new Point(1, 1);
                    splitContainer1.Size = new Size(Width - 100, Height - 100);
                    tie1 = t1;
                    backgroundWorker1.RunWorkerAsync();
                    return;
                }
                catch
                {
                    return;
                }
            }

            if (tremor == true)
            {
                if (tinitremor == 0)
                {
                    tinitremor = t1;
                    t1cod = tinitremor;
                    t2cod = tinitremor + incTremor;
                    bovar.Visible = false;
                    boaste.Visible = false;
                    boClaSola.Visible = true;
                }
                else
                {
                    fcra = 2.0 * 1.0 / ra[id];
                    if (t2 - t1 <= fcra)
                    {
                        if (tremofin == true)
                            tifintremor = t1;
                    }
                    else
                    {
                        nucod = (short)(id);
                        panelAmp.Visible = true;
                        labelMrc.Text = "";
                        t1cod = tinitremor;
                        t2cod = tinitremor + incTremor;
                        t1amp = t1;
                        t2amp = t2;
                        panelAmp.Invalidate();
                    }
                }
                Cuadro_Tremor();
                return;
            }

            if (e.Button == MouseButtons.Right)
            {
                if (panelcla.Visible == true)
                    return;
                if (bxi == bxf && byi == byf)
                {
                    DateTime fech1 = new DateTime(tii1);
                    ss = string.Format("{0:HH}:{0:mm}:{0:ss}.{0:ff}", fech1);
                }
                else
                {
                    dif = Math.Abs(t2 - t1);
                    ss = string.Format("{0:0.00}", dif) + " seg";
                    ss += string.Format(" ({0:0.00}')", dif / 60.0);
                    i = (int)((t1 - tim[id][0]) * ra[id]);
                    if (i < 0)
                        i = 0;
                    j = (int)((t2 - tim[id][0]) * ra[id]);
                    if (j > jb)
                        j = jb;
                    if (j > 0 && j > i)
                    {
                        mmx = cu[id][i];
                        mmn = mmx;
                        for (k = i + 1; k < j; k++)
                        {
                            if (mmx < cu[id][k])
                                mmx = cu[id][k];
                            else if (mmn > cu[id][k])
                                mmn = cu[id][k];
                        }
                        dfcu = mmx - mmn;
                        ss += " " + dfcu.ToString() + " cue";
                        if (fcnan[id] > 0)
                        {
                            //dif = Math.Abs((fcnan[id]*dfcu)/(1000.0*ga[id]));
                            dif = Math.Abs((fcnan[id] * dfcu) / (double)(ga[id]));
                            //ss += string.Format("   {0:0.00} mc/s    ga={1:0} fc={2:0.000000}", dif,ga[id],fcnan[id]);
                            ss += string.Format("   {0:0.00} ", dif) + Unidad[id];
                            ss += string.Format(" ga={0:0} fc={1:0.000000}", ga[id], fcnan[id]);
                        }
                    }
                }
                tip.IsBalloon = true;
                tip.InitialDelay = 0;
                tip.ReshowDelay = 0;
                tip.AutomaticDelay = 0;
                tip.SetToolTip(panel1, ss);
            }
            else
            {

                if (VerEspectro == true)
                {
                    yesp = (short)(e.Y);
                    xesp = (short)(e.X);
                    moveresp = false;
                    return;
                }
                else if (interpol == true)
                {
                    ip1 = (int)((t1 - tim[id][0]) * ra[id]);
                    ip2 = (int)((t2 - tim[id][0]) * ra[id]);
                    if (ip1 < 0)
                        ip1 = 0;
                    if (ip2 >= tim[id].Length)
                        ip2 = tim[id].Length - 1;
                    if (ip2 - ip1 <= 2)
                        return;
                    ipb1 = ip1; ipb2 = ip2;
                    si = CalculoInterpolacion(id);
                    if (si == true)
                    {
                        panelInterP.Visible = true;
                        panelInterP.Invalidate();
                    }
                    return;
                }
                tie1 = t1;
                tie2 = t2;
                if (sipro == 1)
                {
                    sipro = 2;
                    boprom.BackColor = Color.SpringGreen;
                    p1 = (int)((t1 - tim[id][0]) * ra[id]);
                    p2 = (int)((t2 - tim[id][0]) * ra[id]);
                    if (p1 < 0)
                        p1 = 0;
                    if (p2 <= 0)
                    {
                        sipro = 1;
                        boprom.BackColor = Color.IndianRed;
                    }
                    else if (tremor == false)
                        panel1.Invalidate();
                    return;
                }

                //MessageBox.Show("tie1="+tie1.ToString()+" tie2="+tie2.ToString()+" clSola="+clSola.ToString());
                if (tie1 != tie2)
                {
                    if (File.Exists("datos.txt"))
                        YaClasificados();
                    if (clSola == -1)
                    {
                        //MessageBox.Show("2");
                        if ((tie2 - tie1) < 10.0)
                        {
                            NoMostrar = true;
                            ca = "VENTANA MENOR DE 10 SEGUNDOS!!\nNO se GRABA en la BASE sismos con\nDuracion Menor a 10 Segundos\n\nSALIR??";
                            DialogResult result = MessageBox.Show(ca,
                                                         "Duracion Menor de 10 Segundos", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (result == DialogResult.Yes)
                                return;
                        }
                        Cladat[0].X = bxi;
                        Cladat[0].Y = byi;
                        Cladat[1].X = e.X;
                        Cladat[1].Y = e.Y;
                        //MessageBox.Show("0.X="+Cladat[0].X.ToString()+" 0.Y="+Cladat[0].Y.ToString()+" 1.X="+Cladat[1].X.ToString()+" 1.Y="+Cladat[1].Y.ToString());
                        if (loscajones == false)
                            for (i = 0; i < nutra; i++)
                                siEst[i] = true;
                        panelcla.BringToFront();
                        Clasificar();
                    }
                    else
                    {
                        nucod = (short)(id);
                        panelAmp.Visible = false;
                        Pti = 0;
                        Sti = 0;
                        Cti = 0;
                        Ati = 0;
                        periodo = 0;
                        t1cod = tie1;
                        t2cod = tie2;
                        BuscaCompCoda(); // rutina que busca si la traza actual corresponde a una componente de una estacion triaxial.
                        panelcoda.Visible = true;
                        panelcoda.Invalidate();
                    }
                }
                else if (siArch == true) // si la opcion de mostrar la duración de los archivos esta activa.
                {
                    for (i = 0; i < contarch; i++)
                    {
                        if (t1 > tiar[i] && t2 < (tiar[i] + duar[i]))
                        {
                            for (j = 0; j < nutra; j++)
                                siEst[j] = false;
                            StreamReader ar = new StreamReader("datos.txt");
                            lin = "";
                            while (lin != null)
                            {
                                try
                                {
                                    lin = ar.ReadLine();
                                    if (lin == null)
                                        break;
                                    ttt = (double.Parse(lin.Substring(34, 19)) - Feisuds) / 10000000.0;
                                    if (ttt == tiar[i] && string.Compare(nomar[i].Substring(0, 12), lin.Substring(16, 12)) == 0)
                                    {
                                        nuar = int.Parse(lin.Substring(53, 3));
                                        for (k = 0; k < nuar; k++)
                                        {
                                            sismo = nomar[i];
                                            ee = lin.Substring(57 + k * 5, 4);
                                            for (j = 0; j < nutra; j++)
                                            {
                                                if (string.Compare(ee.Substring(0, 4), est[j].Substring(0, 4)) == 0)
                                                {
                                                    siEst[j] = true;
                                                    break;
                                                }
                                            }// for j
                                        }// for k
                                        tie1 = ttt;
                                        tie2 = ttt + duar[i];
                                    }// if ttt
                                }
                                catch
                                {
                                }
                            }
                            ar.Close();
                            if (elimiclas == false)
                            { // se llama al programa clasificador si se activa dentro del cajon que muestra la duración del archivo, 
                                // siempre y cuando no se hayya activado el boton de eliminar la clasificacion.
                                Form2 frm2 = new Form2(this);
                                frm2.Show();
                                frm2.BringToFront();
                            }
                            else
                            { // aqui ya se ha activado el boton de eliminar archivo.
                                panel2.Visible = true;
                                ca = "Eliminando archivos....";
                                util.Mensaje(panel2, ca, false);
                                NoMostrar = true;
                                si = util.EliminaClasificacion(tie1, sismo, rutbas);
                                elimiclas = false;
                                boEliClas.BackColor = Color.White;
                                util.Leerbase(panel2, rutbas, ll1, ll2, cl, volcan);
                                if (File.Exists("amplis.txt"))
                                    LeerAmplitud();
                                Reviarch();
                                panel2.Visible = false;
                                panel1.Invalidate();
                            }
                            break;
                        }
                    }
                }
                else
                {
                    Graphics dc1 = panel1.CreateGraphics();
                    Pen lap = new Pen(Color.Orange, 2);
                    Pen lap2 = new Pen(Color.Red, 2);
                    dc1.DrawLine(lap, e.X, 0, e.X, panel1.Height);
                    if (panel1a.Visible == true)
                    {
                        Graphics dc1a = panel1a.CreateGraphics();
                        dc1a.DrawLine(lap, e.X, 0, e.X, panel1a.Height);
                        dc1.DrawLine(lap2, e.X - 10, e.Y, e.X + 10, e.Y);
                        dc1a.DrawLine(lap2, e.X - 10, e.Y, e.X + 10, e.Y);
                    }
                    lap.Dispose();
                    lap2.Dispose();
                }
            }


            return;
        }
        /// <summary>
        /// Es el encargado de filtrar con un pasa altos la traza de la estación actual y almacenar 
        /// la traza filtrada en el vector cfD, además determina cual es el cero de la señal de la 
        /// traza en ese momento seleccionada el cual guarda en la variable promDR que se utiliza en el cálculo del desplazamiento reducido. 
        /// </summary>
        void PromedioFiltrado()
        {
            int i, mxx, mnn, nmi, nmf;

            if (VD[id] == '*')
                return;
            nmi = (int)((tDR1 - tim[id][0]) * ra[id]);
            if (nmi < 0)
                nmi = 0;
            nmf = (int)((tDR2 - tim[id][0]) * ra[id]);
            if (nmf > cu[id].Length)
                nmf = cu[id].Length;
            //MessageBox.Show("nmi=" + nmi.ToString() + " nmf=" + nmf.ToString() + " <10=" + (nmf - nmi).ToString());
            if (nmf - nmi < 10)
                return;
            cfD = new int[cu[id].Length];
            cfD = util.PasaAltos(cu[id], M, (float)(ra[id]), 0.5);

            mxx = cfD[nmi];
            mnn = mxx;
            for (i = nmi + 1; i <= nmf; i++)
            {
                if (mxx < cfD[i]) mxx = cfD[i];
                else if (mnn > cfD[i])
                    mnn = cfD[i];
            }
            promDR = (int)((mxx + mnn) / 2.0);
        }
        /// <summary>
        /// Método que se ejecuta cuando se da click con el  mouse sobre el panel de clasificación 
        /// para desplegar el panel donde se muestra el espectro de una porción de la traza. 
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void panelcladib_MouseDown(object sender, MouseEventArgs e)
        {// panelcladib: panel de clasificacion.
            int numtotra, i, j, nuu, nuesp, yf;
            double fay;
            bool[] siesta = new bool[nutra];

            if (guia == true)
                return;
            bcxi = e.X;
            bcyi = e.Y;
            if (panelmarca.Visible == true)
                panelmarca.Visible = false;

            if (e.Button == MouseButtons.Left)
            {
                if (VerEspectro == true)
                {
                    for (i = 0; i < nutra; i++)
                    {
                        siesta[i] = siEst[i];
                        if (boNano.Text == "Nan" && fcnan[i] <= 0)
                            siesta[i] = false;
                    }
                    numtotra = 0;
                    for (i = 0; i < nutra; i++)
                        if (siesta[i] == true)
                            numtotra += 1;
                    yf = panelcladib.Size.Height;
                    fay = yf / (double)(numtotra);
                    nuu = (int)(e.Y / fay);
                    j = 0;
                    nuesp = -1;
                    for (i = 0; i < nutra; i++)
                    {
                        if (siesta[i] == true)
                        {
                            if (nuu == j)
                            {
                                nuesp = i;
                                break;
                            }
                            j += 1;
                        }
                    }
                    if (nuesp > -1)
                        idc = (ushort)(nuesp);
                    CalcularEspectroCla(panelBarEsp1, idc, e);
                    movespcla = true;
                }
            }
            else
            {
                VerEspectro = false;
                panelFFTzoom.Visible = false;
                moveresp = false;
                movespcla = false;
                boEspCla.BackColor = Color.White;
                boEspe.BackColor = Color.WhiteSmoke;
                //TrazasClas();
            }

            return;
        }
        /// <summary>
        /// Se busca saber si se ha arrastrado el mouse en el panel de clasificación,
        /// si se hace con el botón derecho (información sobre el tiempo o duración) o izquierdo (entra en el panel de coda).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void panelcladib_MouseUp(object sender, MouseEventArgs e)
        {
            int xf, yf, bcxf, bcyf, numtotra, i, j, nuu, nuu0, nue, k, jb, dfcu, mmx, mmn;
            long tii1 = 0;
            double dif = 0, ti1, ti2, fax, fay, dd;
            string ss = "";
            bool[] siesta = new bool[nutra];

            NoMostrar = true;
            movespcla = false;
            if (panelFFTzoom.Visible == true)
                return;
            bcxf = e.X;
            bcyf = e.Y;
            //MessageBox.Show("bcxf=" + bcxf.ToString() + " bcyf=" + bcyf.ToString());
            xf = panelcladib.Size.Width - 40;
            yf = panelcladib.Size.Height;
            if (guia == true)
            { // se dibuja una linea vertical que sirve de guia.
                Graphics dc = panelcladib.CreateGraphics();
                Pen lapiz = new Pen(Color.Red, 1);
                dc.DrawLine(lapiz, e.X, 1, e.X, yf);
                lapiz.Dispose();
                guia = false;
                boguiCla.BackColor = Color.White;
                return;
            }
            if ((int)(Math.Abs(e.X - bcxi)) < 5 && e.Button == MouseButtons.Left)
                return;
            fax = (tie2 - tie1) / xf;
            ti1 = tie1 + ((bcxi - 40) * fax);
            ti2 = tie1 + ((bcxf - 40) * fax);
            if (ti1 > ti2)
            {
                dd = ti1;
                ti1 = ti2;
                ti2 = dd;
            }
            tii1 = (long)(Fei + (ti1 * 10000000.0));
            for (i = 0; i < nutra; i++)
            {
                siesta[i] = siEst[i];
                if (boNano.Text == "Nan" && fcnan[i] <= 0)
                    siesta[i] = false;
            }
            numtotra = 0;
            for (i = 0; i < nutra; i++)
                if (siesta[i] == true)
                    numtotra += 1;
            //fay = yf / (numtotra + 0.5);
            fay = yf / (double)(numtotra);
            //nuu = (int)((e.Y+(fay/2.0))/fay);
            nuu = (int)(e.Y / fay);
            j = 0;
            nue = -1;
            for (i = 0; i < nutra; i++)
            {
                // if (siEst[i] == true)
                if (siesta[i] == true)
                {
                    if (nuu == j)
                    {
                        nue = i;
                        break;
                    }
                    j += 1;
                }
            }

            if (bcxf > 40)
            {
                if (e.Button == MouseButtons.Right)
                {

                    if (bcxi == bcxf/* && bcyi == bcyf*/)
                    {// se indica el tiempo absoluto
                        DateTime fech1 = new DateTime(tii1);
                        ss = string.Format("{0:HH}:{0:mm}:{0:ss}.{0:ff}", fech1);
                    }
                    else
                    {// se indica el intervalo de tiempo equivalente al sector de arrastre.
                        jb = tim[nue].Length - 1; // tiempo de la ultima muestra
                        dif = Math.Abs(ti2 - ti1);
                        ss = string.Format(est[nue].Substring(0, 4) + " {0:0.00}", dif) + " seg";
                        if (nue > -1)
                        {
                            i = (int)((ti1 - tim[nue][0]) * ra[nue]);
                            if (i < 0)
                                i = 0;
                            j = (int)((ti2 - tim[nue][0]) * ra[nue]);
                            if (j > jb)
                                j = jb;
                            if (j > 0 && j > i)
                            {
                                mmx = cu[nue][i];
                                mmn = mmx;
                                for (k = i + 1; k < j; k++)
                                {
                                    if (mmx < cu[nue][k])
                                        mmx = cu[nue][k];
                                    else if (mmn > cu[nue][k])
                                        mmn = cu[nue][k];
                                }
                                dfcu = mmx - mmn;
                                ss += " " + dfcu.ToString() + " cue";
                                if (fcnan[nue] > 0)
                                {
                                    dif = Math.Abs((fcnan[nue] * dfcu) / 1000.0);
                                    ss += string.Format("   {0:0.00}  mc/s", dif);
                                }
                            }
                        }

                    }
                    tip.IsBalloon = false;
                    tip.ReshowDelay = 0;
                    tip.Show(ss, panelcladib, e.X + 10, e.Y - 15, 3000);
                }
                else
                {// se visualiza el panel de coda o el de movimiento de particulas
                    if (moverparti == true)
                    {
                        panel2.Visible = false;
                        xf = panelcladib.Size.Width - 40;
                        //faxx = xf / dura;
                        //timpt = timin + (e.X - 40) / fax;
                        timpt = ti2;
                        idmpt = (short)(nue);
                        mptintp = false;
                        MovimientoParticula();
                        return;
                    }
                    filtcod = false;
                    calcfiltcod = false;
                    bofilcod.BackColor = Color.White;
                    bohzcod.BackColor = Color.White;
                    bopolcod.BackColor = Color.White;
                    radlowcod.BackColor = Color.White;
                    radhicod.BackColor = Color.White;
                    if (panelcoda.Visible == true)
                    {
                        DibujoTrazas();// dibuja las trazas en el panel de clasificacion.
                        panelAmp.Visible = false;
                        Pti = 0;
                        Sti = 0;
                        Cti = 0;
                        Ati = 0;
                        periodo = 0;
                    }
                    panelcoda.Visible = true;
                    t1cod = ti1;
                    t2cod = ti2;
                    nucod = (short)(nue);
                    if (nucod < 0)
                    {
                        panelcoda.Visible = false;
                        return;
                    }
                    BuscaCompCoda();         //rutina que busca si la traza hace parte de 3 componentes.                
                    panelcoda.Invalidate();
                    TrazasClas();
                    ChequeoFactormm();
                }
            }
            else
            {
                numtotra = 0;
                for (i = 0; i < nutra; i++)
                {
                    if (siesta[i] == true)
                        numtotra += 1;
                }
                fay = yf / (numtotra + 0.5);
                nuu = (int)(e.Y / fay);
                nuu0 = (int)(bcyi / fay);
                j = 0;
                for (i = 0; i < nutra; i++)
                {
                    if (siesta[i] == true)
                    {
                        if (j >= nuu0 && j <= nuu)
                        {
                            siEst[i] = false;
                            siesta[i] = false;
                            boTodas.BackColor = Color.PaleVioletRed;
                        }
                        j += 1;
                    }
                }
                DibujoTrazas();
                //MessageBox.Show("nuu0="+nuu0.ToString()+" nuu="+nuu.ToString());
            }

            return;
        }
        /// <summary>
        /// Lanza el método CalcularEspectroCla().
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void panelcladib_MouseMove(object sender, MouseEventArgs e)
        {
            if (movespcla == false)
                return;
            CalcularEspectroCla(panelBarEsp1, idc, e);
        }
        /// <summary>
        /// Dibuja la traza de la estación seleccionada, en el panel de Coda.
        /// </summary>
        /// <param name="va">Vector con las cuentas que forman una traza especifica.</param>
        /// <param name="idd">El id de la estación a la que se va asignar los valores P, S, C.</param>
        void Dibujocoda(int[] va, short idd)
        {
            int xf, yf, pro = 0, max, min, lar, kk, k, dif, tot;
            int i, nmi = 0, nmf = 0, minu, seg, frac, cuentana;
            float x1, y1, SP;
            long ll;
            double fax, fay, fayy, dura, diff, iniy, fy, fxsat, fmsat;
            string esta = "", ss = "";
            Point[] dat;

            // t1cod: tiempo inicial de la ventana de coda.
            // t2cod: tiempo final de la ventana de coda.
            if (t1cod == t2cod)
                return;
            panelcoda.BackColor = colfondo;

            try
            {
                if (panelmarca.Visible == true)
                    panelmarca.Visible = false;
                panelcoda.BringToFront();
                util.borra(panelcoda, colfondo);
                xf = panelcoda.Size.Width - 70;
                yf = panelcoda.Size.Height - 30;
                dura = t2cod - t1cod;
                fax = xf / dura;
                fay = 4.0 * yf / 5.0;

                Graphics dc = panelcoda.CreateGraphics();
                Pen lapiz = new Pen(colinea, 1);
                SolidBrush brocha = new SolidBrush(colotr1);
                Pen lap0 = new Pen(Color.Green, 1);
                Pen lap1 = new Pen(Color.Red, 3);
                Pen lap2 = new Pen(Color.DarkOrange, 2);
                dc.DrawRectangle(lapiz, 0, 0, panelcoda.Size.Width - 3, panelcoda.Size.Height - 3);

                //     ControlPaint.DrawBorder(dc,this.ClientRectangle,colinea,ButtonBorderStyle.Solid);               

                tot = tim[idd].Length;
                nmi = 0;
                nmf = tot;
                for (k = 0; k < tot; k++)
                {
                    if (tim[idd][k] >= t1cod)
                        break;
                }
                nmi = k;
                for (k = nmi; k < tot; k++)
                {
                    if (tim[idd][k] > t2cod || tim[idd][k] <= 0) break;
                }
                nmf = k - 1;
                lar = nmf - nmi;
                max = va[nmi];
                min = max;
                for (k = nmi + 1; k < nmf; k++)
                {
                    if (max < va[k])
                        max = va[k];
                    else if (min > va[k])
                        min = va[k];
                }

                //if (analogico == false && analogcoda == false)
                if (analogcoda == false)
                {
                    fayy = fay;
                    pro = (int)((max + min) / 2.0F);
                }
                else
                {
                    cuentana = CuentasAnalogico;
                    if (nuanalog > 0)
                    {
                        for (i = 0; i < nuanalog; i++)
                        {
                            if (string.Compare(est[idd], 0, estanalog[i], 0, 4) == 0)
                            {
                                cuentana = analog[i];
                                break;
                            }
                        }
                    }
                    pro = promEst[idd];
                    if (pro == 0)
                        pro = (int)((max + min) / 2.0F);
                    fayy = (float)(100.0 / (cuentana * ga[idd]));
                }
                //if (analogico == false && analogcoda==false)
                if (analogcoda == false)
                {
                    if (max - pro != 0)
                        fy = ((fayy / 2) / ((max - pro)));
                    else fy = 1;
                }
                else fy = fayy;

                //pro = (int)(pro * ampcod);
                iniy = panelcoda.Size.Height / 2;
                if (!char.IsLetterOrDigit(est[idd][4]))
                    esta = est[nucod].Substring(0, 4);
                else
                    esta = est[nucod];
                dc.DrawString(esta, new Font("Times New Roman", 8, FontStyle.Bold), brocha, 1, 10);
                dat = new Point[lar];
                kk = 0;
                if (satu == false)
                {
                    if (invertido[idd] == false)
                    {
                        for (k = nmi; k < nmf; k++)
                        {
                            if (kk >= lar) break;
                            dif = (int)(ampcod * va[k] - pro);
                            diff = dif * fy;
                            y1 = (float)(iniy - diff);
                            x1 = (float)(40.0 + (tim[idd][k] - t1cod) * fax);
                            dat[kk].Y = (int)y1;
                            dat[kk].X = (int)x1;
                            kk += 1;
                        }
                    }
                    else
                    {
                        for (k = nmi; k < nmf; k++)
                        {
                            if (kk >= lar) break;
                            //dif = pro-va[k];
                            dif = (int)(pro - (ampcod * va[k]));
                            diff = dif * fy;
                            y1 = (float)(iniy - diff);
                            x1 = (float)(40.0 + (tim[idd][k] - t1cod) * fax);
                            dat[kk].Y = (int)y1;
                            dat[kk].X = (int)x1;
                            kk += 1;
                        }
                    }
                }
                else
                {
                    dif = max - pro;
                    diff = dif * fy;
                    fxsat = diff;   // fxsat y fmsat, son variables para la opcion de saturacion.
                    dif = min - pro;
                    diff = dif * fy;
                    fmsat = diff;
                    if (invertido[idd] == false)
                    {
                        for (k = nmi; k < nmf; k++)
                        {
                            if (kk >= lar)
                                break;
                            dif = (int)(ampcod * va[k] - pro);
                            diff = dif * fy;
                            if (diff > fxsat)
                                diff = fxsat;
                            else if (diff < fmsat)
                                diff = fmsat;
                            y1 = (float)(iniy - diff);
                            x1 = (float)(40.0 + (tim[idd][k] - t1cod) * fax);
                            dat[kk].Y = (int)y1;
                            dat[kk].X = (int)x1;
                            kk += 1;
                        }
                    }
                    else
                    {
                        for (k = nmi; k < nmf; k++)
                        {
                            if (kk >= lar)
                                break;
                            dif = (int)(pro - (ampcod * va[k]));
                            diff = dif * fy;
                            if (diff > fxsat)
                                diff = fxsat;
                            else if (diff < fmsat)
                                diff = fmsat;
                            y1 = (float)(iniy - diff);
                            x1 = (float)(40.0 + (tim[idd][k] - t1cod) * fax);
                            dat[kk].Y = (int)y1;
                            dat[kk].X = (int)x1;
                            kk += 1;
                        }
                    }
                }
                dc.DrawLines(lapiz, dat);

                for (k = nmi; k < nmf; k++)
                {// marcas de tiempo en el la parte superior del panel de coda
                    ll = (long)(Fei + (tim[idd][k] * 10000000.0));
                    DateTime fech = new DateTime(ll);
                    frac = int.Parse(string.Format("{0:ff}", fech));
                    if (frac == 0)
                    {
                        x1 = (float)(40.0 + (tim[idd][k] - t1cod) * fax);
                        seg = int.Parse(string.Format("{0:ss}", fech));
                        if (seg == 0)
                        {
                            minu = int.Parse(string.Format("{0:mm}", fech));
                            if (minu == 0)
                                dc.DrawLine(lap1, x1, 1, x1, 14);
                            else
                                dc.DrawLine(lap2, x1, 1, x1, 10);
                        }
                        else dc.DrawLine(lap0, x1, 1, x1, 6);
                    }
                }

                lapiz.Dispose();
                lap1.Dispose();
                lap2.Dispose();
                if (Pti > 0)// tiempo de la P
                {
                    Pen lapizP = new Pen(colP, 1);
                    SolidBrush brochaP = new SolidBrush(colP);
                    x1 = (float)(40.0 + (Pti - t1cod) * fax);
                    dc.DrawLine(lapizP, x1, 30, x1, yf - 10);
                    ll = (long)(Fei + (Pti * 10000000.0));
                    DateTime fech = new DateTime(ll);
                    ss = string.Format("P: {0:HH}:{0:mm}:{0:ss}.{0:ff}", fech);
                    dc.DrawString(ss, new Font("Times New Roman", 9), brochaP, 70, 1);
                    lapizP.Dispose();
                    brochaP.Dispose();
                }
                if (Sti > 0)  // tiempo de la S
                {
                    Pen lapizS = new Pen(colS, 1);
                    x1 = (float)(40.0 + (Sti - t1cod) * fax);
                    dc.DrawLine(lapizS, x1, 30, x1, yf - 10);
                    if (Pti > 0)
                    {
                        SolidBrush brochaS = new SolidBrush(colS);
                        SP = (float)(Sti - Pti);
                        ss = string.Format("S-P: {0:0.00} seg", SP);
                        dc.DrawString(ss, new Font("Times New Roman", 9), brochaS, 200, 1);
                        brochaS.Dispose();
                    }
                    lapizS.Dispose();
                }
                if (Cti > 0) // tiempo de la coda.
                {
                    Pen lapizC = new Pen(colC, 1);
                    x1 = (float)(40.0 + (Cti - t1cod) * fax);
                    dc.DrawLine(lapizC, x1, 30, x1, yf - 10);
                    if (Pti > 0)
                    {
                        SolidBrush brochaC = new SolidBrush(colC);
                        SP = (float)(Cti - Pti);
                        ss = string.Format("Coda: {0:0.00} seg", SP);
                        dc.DrawString(ss, new Font("Times New Roman", 9), brochaC, 300, 1);
                        brochaC.Dispose();
                    }
                    lapizC.Dispose();
                }


                brocha.Dispose();
                DibujoClascoda();
            }
            catch
            {
            }

            return;
        }
        /// <summary>
        /// Dibuja en color naranja, en el panel de clasificación, el sector de coda seleccionado por el usuario.
        /// </summary>
        void DibujoClascoda()
        {
            int xf, yf, i, jj, k, kk, lar, max, min, pro, dif = 0, numtotra, nm1, nm2, tot;
            int[] nmi = new int[3];
            int[] nmf = new int[3];
            float x1, y1;
            double dura, fax, fay, fy, diff, iniy, fxsat, fmsat;
            Point[] dat;


            if (panelcoda.Visible == false)
                return;

            jj = -1;
            k = 0;
            for (i = 0; i < nutra; i++)
            {
                if (siEst[i] == true)
                {
                    if (i == nucod)
                    {
                        jj = k;
                        break;
                    }
                    k += 1;
                }
            }
            if (jj == -1)
                return;
            // MessageBox.Show("DibujoClasCoda jj=" + jj.ToString());

            xf = (panelcladib.Size.Width - 40);
            yf = panelcladib.Size.Height;
            numtotra = 0;
            for (i = 0; i < nutra; i++)
                if (siEst[i] == true)
                    numtotra += 1;
            dura = tie2 - tie1;
            fax = xf / dura;
            fay = yf / (numtotra + 0.5);

            tot = tim[nucod].Length;
            for (k = 0; k < tot; k++)
            {
                if (tim[nucod][k] >= tie1)
                    break;
            }
            nm1 = k;
            for (k = 0; k < tot; k++)
            {
                if (tim[nucod][k] > tie2 || tim[nucod][k] <= 0)
                    break;
            }
            nm2 = k - 1;
            max = cu[nucod][nm1];
            min = max;
            for (k = nm1 + 1; k < nm2; k++)
            {
                if (max < cu[nucod][k])
                    max = cu[nucod][k];
                else if (min > cu[nucod][k])
                    min = cu[nucod][k];
            }

            nmi[0] = nm1;
            nmf[2] = nm2;
            for (k = 0; k < tot; k++)
            {
                if (tim[nucod][k] >= t1cod)
                    break;
            }
            nmi[1] = k;
            nmf[0] = k;
            for (k = 0; k < tot; k++)
            {
                if (tim[nucod][k] >= t2cod)
                    break;
            }
            nmi[2] = k;
            nmf[1] = k;

            Graphics dc = panelcladib.CreateGraphics();
            Pen lapiz;

            try
            {
                for (i = 0; i < 3; i++)
                {
                    if (i != 1)
                        lapiz = new Pen(colinea, 1);
                    else
                        lapiz = new Pen(Color.Orange, 1);
                    lar = nmf[i] - nmi[i];
                    if (analogico == false)
                        pro = (int)((max + min) / 2.0F);
                    else
                    {
                        pro = promEst[id];
                        max = pro + (int)(CuentasAnalogico / 2.0);
                        min = pro - (int)(CuentasAnalogico / 2.0);
                    }

                    if (max - pro != 0)
                        fy = ((fay / 2) / ((max - pro)));
                    else
                        fy = 1;
                    //pro = (int)(pro * ampclas);
                    iniy = 5 + jj * fay + fay / 2;
                    dat = new Point[lar];
                    kk = 0;
                    if (satu == false)
                    {
                        if (invertido[nucod] == false)
                        {
                            for (k = nmi[i]; k < nmf[i]; k++)
                            {
                                if (kk >= lar)
                                    break;
                                //dif = pro - (int)(cu[nucod][k] * ampclas);
                                dif = (int)(cu[nucod][k] * ampclas) - pro;
                                diff = dif * fy;
                                y1 = (float)(iniy - diff);
                                x1 = (float)(40.0 + (tim[nucod][k] - tie1) * fax);
                                dat[kk].Y = (int)y1;
                                dat[kk].X = (int)x1;
                                kk += 1;
                            }
                        }
                        else
                        {
                            for (k = nmi[i]; k < nmf[i]; k++)
                            {
                                if (kk >= lar)
                                    break;
                                dif = pro - (int)(cu[nucod][k] * ampclas);
                                //dif = (int)(cu[nucod][k] * ampclas) - pro;
                                diff = dif * fy;
                                y1 = (float)(iniy - diff);
                                x1 = (float)(40.0 + (tim[nucod][k] - tie1) * fax);
                                dat[kk].Y = (int)y1;
                                dat[kk].X = (int)x1;
                                kk += 1;
                            }
                        }
                    }
                    else
                    {
                        dif = max - pro;
                        diff = dif * fy;
                        fxsat = diff;   // fxsat y fmsat, son variables para la opcion de saturacion.
                        dif = min - pro;
                        diff = dif * fy;
                        fmsat = diff;
                        if (invertido[nucod] == false)
                        {
                            for (k = nmi[i]; k < nmf[i]; k++)
                            {
                                if (kk >= lar)
                                    break;
                                //dif = (int)(cu[j][k] * ampclas) - pro;
                                dif = cu[nucod][k] - pro;
                                diff = dif * fy;
                                if (diff > fxsat)
                                    diff = fxsat;
                                else if (diff < fmsat)
                                    diff = fmsat;
                                y1 = (float)(iniy - diff);
                                x1 = (float)(40.0 + (tim[nucod][k] - tie1) * fax);
                                dat[kk].Y = (int)y1;
                                dat[kk].X = (int)x1;
                                kk += 1;
                            }
                        }
                        else
                        {
                            for (k = nmi[i]; k < nmf[i]; k++)
                            {
                                if (kk >= lar)
                                    break;
                                dif = pro - cu[nucod][k];
                                diff = dif * fy;
                                if (diff > fxsat)
                                    diff = fxsat;
                                else if (diff < fmsat)
                                    diff = fmsat;
                                y1 = (float)(iniy - diff);
                                x1 = (float)(40.0 + (tim[nucod][k] - tie1) * fax);
                                dat[kk].Y = (int)y1;
                                dat[kk].X = (int)x1;
                                kk += 1;
                            }
                        }
                    }
                    dc.DrawLines(lapiz, dat);
                    lapiz.Dispose();
                }/// fin del for
            }
            catch
            {
            }

            // MessageBox.Show("Fin dibujoClasCoda");
            return;
        }
        /// <summary>
        /// Indica que se va a asignar el valor de P por ende reinicia los valores de Pti, Sti y Cti,
        /// con este método se controla la asignación del tiempo de P en el panelcoda.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void boP_Click(object sender, EventArgs e)
        {// boton de lectura de P en el panel de coda
            Pti = 0;
            Sti = 0;
            Cti = 0;
            Pcd = true;
            Scd = false;
            Ccd = false;
            panelcoda.Invalidate();
            BotonesCoda();// rutina que muestra que lectura (P, S o Coda) esta activa. 
            return;
        }
        /// <summary>
        /// Indica que se va a asignar el valor de S por ende reinicia el valor de Sti,
        /// con este método se controla la asignación del tiempo de S en el panelcoda.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void boS_Click(object sender, EventArgs e)
        {// boton de lectura de S en el panel de coda
            Sti = 0;
            Pcd = false;
            Scd = true;
            Ccd = false;
            panelcoda.Invalidate();
            BotonesCoda();
            return;
        }
        /// <summary>
        /// Indica que se va a asignar el valor de C por ende reinicia el valor de Cti,
        /// con este método se controla la asignación del tiempo de C en el panelcoda.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void boC_Click(object sender, EventArgs e)
        {// boton de lectura de Coda en el panel de coda
            Cti = 0;
            Pcd = false;
            Scd = false;
            Ccd = true;
            panelcoda.Invalidate();
            BotonesCoda();
            return;
        }
        /// <summary>
        /// Controla el color con el que se dibujan los botones boP,boS y boC con el fin de indicar gráficamente 
        /// que ya se a asignado el valor de P, S o C.
        /// </summary>
        void BotonesCoda()
        {// muestra cual boton es el activo.
            boP.BackColor = Color.Lavender;
            boS.BackColor = Color.Lavender;
            boC.BackColor = Color.Lavender;
            if (Pcd == true)
                boP.BackColor = Color.MediumSeaGreen;
            else if (Scd == true)
                boS.BackColor = Color.MediumSeaGreen;
            else if (Ccd == true)
                boC.BackColor = Color.MediumSeaGreen;
        }
        /// <summary>
        /// Desplaza la traza en el panelcoda hacia la izquierda.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void boizqco_MouseDown(object sender, MouseEventArgs e)
        {
            double tii1, tii2;
            if (e.Button == MouseButtons.Left)
            {
                tii1 = t1cod + 2.0;
                tii2 = t2cod + 2.0;
            }
            else
            {
                tii1 = t1cod + 10.0;
                tii2 = t2cod + 10.0;
            }
            if (tii2 > tie2)
                return;
            t1cod = tii1;
            t2cod = tii2;
            if (t2cod > tie2)
                t2cod = tie2;
            panelcoda.Invalidate();
            return;
        }
        /// <summary>
        /// Desplaza la traza en el panelcoda hacia la derecha.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void boderco_MouseDown(object sender, MouseEventArgs e)
        {
            double tii1, tii2;

            if (e.Button == MouseButtons.Left)
            {
                tii1 = t1cod - 2.0;
                tii2 = t2cod - 2.0;
            }
            else
            {
                tii1 = t1cod - 10.0;
                tii2 = t2cod - 10.0;
            }
            if (tii1 < tie1) return;
            t1cod = tii1;
            t2cod = tii2;
            if (t2cod > tie2) t2cod = tie2;
            panelcoda.Invalidate();
            return;
        }
        /// <summary>
        /// Zoom positivo a la traza de coda.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void bodilco_MouseDown(object sender, MouseEventArgs e)
        {// Zoom positivo a la traza de coda.
            int lar;
            double tii;
            if (e.Button == MouseButtons.Left)
            {
                tii = t2cod - 2.0;
            }
            else
            {
                tii = t2cod - 10.0;
            }
            lar = tim[nucod].Length;
            if (tii - t1cod < 0.1)
                return;
            t2cod = tii;
            panelcoda.Invalidate();
            return;
        }
        /// <summary>
        /// Zoom negativo a la traza de coda.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void bocomco_MouseDown(object sender, MouseEventArgs e)
        {
            int lar;
            double tii;

            if (e.Button == MouseButtons.Left)
            {
                tii = t2cod + 2.0;
            }
            else
            {
                tii = t2cod + 10.0;
            }
            lar = tim[nucod].Length;
            if (tii >= tim[nucod][lar - 1]) return;
            t2cod = tii;
            if (t2cod > tie2) t2cod = tie2;
            panelcoda.Invalidate();
            return;
        }
        /// <summary>
        /// Cierra el panel de coda.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void boXcod_Click(object sender, EventArgs e)
        {// se cierra el panel de coda
            panelcoda.Visible = false;
            filtcod = false;
            calcfiltcod = false;
            bofilcod.BackColor = Color.White;
            bohzcod.BackColor = Color.White;
            bopolcod.BackColor = Color.White;
            radlowcod.BackColor = Color.White;
            radhicod.BackColor = Color.White;
            if (panelAmp.Visible == true) panelAmp.Visible = false;
            if (panelhueco.Visible == true) panelhueco.Visible = false;
            DibujoTrazas();
            if (zoomcla == false) panel1.Invalidate();
            return;

        }
        /// <summary>
        /// Detecta el punto donde se desea poner P en el panelcoda. 
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void panelcoda_MouseDown(object sender, MouseEventArgs e)
        {
            bpxi = e.X;
            bpyi = e.Y;
            if (panelmarca.Visible == true)
                panelmarca.Visible = false;
            return;
        }
        /// <summary>
        /// Busca saber si se arrastra o no el mouse en el panel de coda.
        /// Si se arrastra el mouse se entra en el panel para la lectura de la amplitud y frecuencia.
        /// Si no se arrastra y se pulsa el botón izquierdo, se hace las lecturas secuenciales de P, S y Coda.
        /// Si es con el botón derecho, siempre se lee la coda. Si el botón HUECO existe y está activado,
        /// las lecturas corresponden a los huecos para los cálculos de energía, de acuerdo al Observatorio de Pasto.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void panelcoda_MouseUp(object sender, MouseEventArgs e)
        {
            int bpxf, bpyf, i, xf, yf, i1, i2, mx, mn;
            double ti1, ti2, tii, fax, dura, tt1, tt2, dif;
            char[] delim = { ' ', '\t' };
            string[] pa = null;
            string ss = "";

            bpxf = e.X;
            bpyf = e.Y;
            xf = panelcoda.Size.Width - 70;
            yf = panelcoda.Size.Height - 30;
            dura = t2cod - t1cod;
            fax = dura / xf;
            ti1 = t1cod + ((bpxi - 40.0) * fax);
            ti2 = t1cod + ((bpxf - 40.0) * fax);
            if (Pcd == true)
                i = 0;
            else if (Scd == true)
                i = 1;
            else
                i = 2;

            if (e.Button == MouseButtons.Right && Math.Abs(bpxf - bpxi) >= 3)
            { // se indica el intervalo de tiempo equivalente al sector de arrastre.               
                dif = Math.Abs(ti2 - ti1);
                ss = string.Format("{0:0.00}", dif) + " seg";
                tip.IsBalloon = false;
                tip.ReshowDelay = 0;
                tip.Show(ss, panelcoda, e.X, e.Y - 15, 3000);
            }
            else // se efectuan la lectura del intervalo donde se va a leer la amplitud y el periodo, o bien se busca el 'cero' de las trazas o bien las lecturas de los 'huecos'
            {
                if (Math.Abs(bpxf - bpxi) < 3)
                {
                    if (panelhueco.Visible == false)
                    {
                        Pcd = false;
                        Scd = false;
                        Ccd = false;
                        if (e.Button == MouseButtons.Right)
                            i = 2; // si boton derecho se lee coda
                        if (i == 0)
                        {
                            Pti = ti1;
                            if (leido == true)
                            {
                                Cti = 0;
                                Sti = 0;
                            }
                            leido = false;
                        }
                        else if (i == 1)
                            Sti = ti1;
                        else
                            Cti = ti1;
                        if (Sti > 0 && Sti <= Pti)
                        {
                            NoMostrar = true;
                            MessageBox.Show("S MENOR que P?? Pti=?");
                            Sti = 0;
                            Scd = true;
                            return;
                        }
                        else if (Sti - Pti > 300.0)
                        {
                            NoMostrar = true;
                            DialogResult result = MessageBox.Show("S-P > 300 segundos, CONTINUAR??",
                                            "CONTINUAR ???", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (result == DialogResult.No)
                                return;
                        }
                        if (Cti > 0 && (Cti <= Pti || Cti <= Sti) && leido == false)
                        {
                            NoMostrar = true;
                            MessageBox.Show("CODA MENOR ??");
                            Cti = 0;
                            Ccd = true;
                            return;
                        }
                        if (i == 0)
                            Scd = true;
                        else if (i == 1)
                            Ccd = true;
                        else
                            Pcd = true;
                        BotonesCoda(); // rutina que muestra el boton activo
                        panelcoda.Invalidate();
                    }
                    else
                    {
                        if (nuhueco > 0)
                        {
                            for (i = 0; i < huecolist.Count; i++)
                            {
                                pa = huecolist[i].ToString().Split(delim);// los huecos se guardan en una lista de cadenas (string).
                                tt1 = double.Parse(pa[0]);
                                tt2 = double.Parse(pa[1]);
                                if (ti1 >= tt1 && ti1 <= tt2)
                                {
                                    huecolist.RemoveAt(i);
                                    nuhueco = huecolist.Count;
                                    //DibujarHuecosCoda();
                                    panelcoda.Invalidate();
                                    return;
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (ti1 > ti2)
                    {
                        tii = ti1;
                        ti1 = ti2;
                        ti2 = tii;
                    }
                    if (sihueco == false)
                    {
                        if (promecod == false)
                        {
                            panelAmp.Visible = true;
                            bool cond = panelAmp.Visible;///esto es mio
                            if (cond) MessageBox.Show("hola");///esto es mio
                            labelMrc.Text = "";
                            t1amp = ti1; // tiempo inicial y final en que se busca el valor del periodo
                            t2amp = ti2; // y la maxima amplitud pico a pico
                            panelAmp.Invalidate();
                        }
                        else // se busca el 'nivel cero' de las trazas
                        {
                            i1 = (int)((ti1 - tim[nucod][0]) * ra[nucod]);
                            i2 = (int)((ti2 - tim[nucod][0]) * ra[nucod]);
                            promecod = false;
                            boPromCoda.BackColor = Color.PaleGoldenrod;
                            if (i1 < 0) i1 = 0;
                            if (i2 <= 0)
                                return;
                            if (i2 >= cu[nucod].Length)
                                i2 = cu[nucod].Length - 1;
                            mx = cu[nucod][i1];
                            mn = mx;
                            for (i = i1 + 1; i <= i2; i++)
                            {
                                if (mx < cu[nucod][i]) mx = cu[nucod][i];
                                else if (mn > cu[nucod][i]) mn = cu[nucod][i];
                            }
                            promEst[nucod] = (int)((mx + mn) / 2.0);
                            panelcoda.Invalidate();
                            return;
                        }
                    }
                    else
                    {
                        panelhueco.Visible = true;
                        panelhueco.BringToFront();
                        t1hu = ti1;
                        t2hu = ti2;
                        panelhueco.Invalidate();
                    }
                }  //else
            } // else

            return;
        }
        /// <summary>
        /// Rutina que dibuja el sector de traza escogido en el panel de coda y donde se efectúa la
        /// lectura de la amplitud y el periodo.
        /// </summary>
        void DibAmpl()
        {
            int xf, yf, pro = 0, max, min, lar, kk, k, dif, tot = 0;
            int nmi = 0, nmf = 0;
            float x1, x2, y1;
            double fax, fay, dura, diff, iniy = 0, fy = 0;
            string ss = "";
            Point[] dat;

            if (t1amp == t2amp)
                return;
            if (panelmarca.Visible == true)
                panelmarca.Visible = false;
            panelAmp.BringToFront();
            util.borra(panelAmp, colfondo);
            xf = panelAmp.Size.Width - 10;
            yf = panelAmp.Size.Height;
            dura = t2amp - t1amp;
            fax = xf / dura;
            fay = 4.0 * yf / 5.0;

            Graphics dc = panelAmp.CreateGraphics();
            Pen lapiz = new Pen(colinea, 1);
            Pen lapiz2 = new Pen(Color.Orange, 1);
            SolidBrush brocha = new SolidBrush(colotr1);
            dc.DrawRectangle(lapiz, 0, 0, panelAmp.Size.Width - 3, panelAmp.Size.Height - 3);

            if (factmm <= 0) // en caso que no exista factor de conversion a milimetros, se le indica al usuario.
            {
                SolidBrush brocha0 = new SolidBrush(Color.Red);
                dc.DrawString("mm", new Font("Times New Roman", 12), brocha0, 1, 10);
                brocha0.Dispose();
            }
            if (fcnan[nucod] <= 0)// en caso que no exista factor de conversion a micrometros, se le indica al usuario.
            {
                SolidBrush brocha0 = new SolidBrush(Color.Red);
                dc.DrawString("mc", new Font("Times New Roman", 12), brocha0, 30, 10);
                brocha0.Dispose();
            }
            if (DesRed == true)// en caso que no exista factor de conversion a micrometros, se le indica al usuario.
            {
                SolidBrush brocha0 = new SolidBrush(Color.Orange);
                dc.DrawString("Dr", new Font("Lucida Console", 14, FontStyle.Bold), brocha0, 30, panelAmp.Height - 20);
                brocha0.Dispose();
            }

            try
            {

                tot = tim[nucod].Length; // se busca a que muestras corresponde el intervalo de la ventana.
                for (k = 0; k < tot; k++)
                {
                    if (tim[nucod][k] >= t1amp)
                        break;
                }
                nmi = k;
                for (k = nmi; k < tot; k++)
                {
                    if (tim[nucod][k] >= t2amp || tim[nucod][k] <= 0) break;
                }
                nmf = k;
                lar = nmf - nmi;
                if (filtcod == false)
                {
                    max = cu[nucod][nmi];
                    min = max;
                    for (k = nmi + 1; k <= nmf; k++) // se busca el valor maximo pico a pico
                    {
                        if (max < cu[nucod][k]) max = cu[nucod][k];
                        else if (min > cu[nucod][k]) min = cu[nucod][k];
                    }
                }
                else
                {
                    max = cf[nmi];
                    min = max;
                    for (k = nmi + 1; k <= nmf; k++) // se busca el valor maximo pico a pico
                    {
                        if (max < cf[k]) max = cf[k];
                        else if (min > cf[k]) min = cf[k];
                    }
                }
                pro = (int)((max + min) / 2.0F);
                if (max - pro != 0) fy = ((fay / 2) / ((max - pro)));
                else fy = 1;
                pro = (int)(pro * ampamp);
                iniy = panelAmp.Size.Height / 2;
                dat = new Point[lar];
                kk = 0;
                if (filtcod == false)
                {
                    if (invertido[nucod] == false)
                    {
                        for (k = nmi; k <= nmf; k++)
                        {
                            if (kk >= lar) break;
                            dif = (int)(cu[nucod][k] * ampamp) - pro;
                            diff = dif * fy;
                            y1 = (float)(iniy - diff);
                            x1 = (float)(10.0 + (tim[nucod][k] - t1amp) * fax);
                            if (pto == true)
                                dc.DrawEllipse(lapiz2, x1 - 3.0F, y1 - 3.0F, 6.0F, 6.0F);
                            dat[kk].Y = (int)y1;
                            dat[kk].X = (int)x1;
                            kk += 1;
                        }
                    }
                    else
                    {
                        for (k = nmi; k <= nmf; k++)
                        {
                            if (kk >= lar) break;
                            dif = pro - (int)(cu[nucod][k] * ampamp);
                            diff = dif * fy;
                            y1 = (float)(iniy - diff);
                            x1 = (float)(10.0 + (tim[nucod][k] - t1amp) * fax);
                            if (pto == true)
                                dc.DrawEllipse(lapiz2, x1 - 3.0F, y1 - 3.0F, 6.0F, 6.0F);
                            dat[kk].Y = (int)y1;
                            dat[kk].X = (int)x1;
                            kk += 1;
                        }
                    }
                }
                else
                {
                    if (invertido[nucod] == false)
                    {
                        for (k = nmi; k <= nmf; k++)
                        {
                            if (kk >= lar) break;
                            dif = (int)(cf[k] * ampamp) - pro;
                            diff = dif * fy;
                            y1 = (float)(iniy - diff);
                            x1 = (float)(10.0 + (tim[nucod][k] - t1amp) * fax);
                            if (pto == true) dc.DrawEllipse(lapiz2, x1 - 3.0F, y1 - 3.0F, 6.0F, 6.0F);
                            dat[kk].Y = (int)y1;
                            dat[kk].X = (int)x1;
                            kk += 1;
                        }
                    }
                    else
                    {
                        for (k = nmi; k <= nmf; k++)
                        {
                            if (kk >= lar) break;
                            dif = pro - (int)(cf[k] * ampamp);
                            diff = dif * fy;
                            y1 = (float)(iniy - diff);
                            x1 = (float)(10.0 + (tim[nucod][k] - t1amp) * fax);
                            if (pto == true) dc.DrawEllipse(lapiz2, x1 - 3.0F, y1 - 3.0F, 6.0F, 6.0F);
                            dat[kk].Y = (int)y1;
                            dat[kk].X = (int)x1;
                            kk += 1;
                        }
                    }
                }
                dc.DrawLines(lapiz, dat);
            }
            catch
            {
            }

            lapiz.Dispose();
            lapiz2.Dispose();
            // MessageBox.Show("ga=" + ga[nucod].ToString());

            //MessageBox.Show("Ati=" + Ati.ToString() + " periodo=" + periodo.ToString()+ " sigana="+siGana[nucod].ToString()+" nucod="+nucod.ToString());
            if (Ati > 0 && periodo > 0) // se visualiza en el panel de amplitud, el sector de lectura.
            {
                try
                {
                    ss = string.Format("{0:0.000} seg  {1:0.0} Hz", periodo, (1.0 / periodo)) + "  " + ampp.ToString() + " Cu  (" + ga[nucod].ToString() + ")";
                    Pen lapizA = new Pen(Color.Orange, 1);
                    Pen lapizAb = new Pen(Color.Green, 2);
                    dc.DrawString(ss, new Font("Times New Roman", 9), brocha, 1, 1);

                    for (k = 0; k < tot; k++)
                    {
                        if (tim[nucod][k] >= Ati) break;
                    }
                    nmi = k;
                    for (k = nmi; k < tot; k++)
                    {
                        if (tim[nucod][k] > Ati + periodo || tim[nucod][k] <= 0) break;
                    }
                    nmf = k;
                    lar = nmf - nmi;
                    dat = new Point[lar];
                    kk = 0;
                    if (filtcod == false)
                    {
                        if (invertido[nucod] == false)
                        {
                            for (k = nmi; k <= nmf; k++)
                            {
                                if (kk >= lar) break;
                                dif = (int)(cu[nucod][k] * ampamp) - pro;
                                diff = dif * fy;
                                y1 = (float)(iniy - diff);
                                x1 = (float)(10.0 + (tim[nucod][k] - t1amp) * fax);
                                dat[kk].Y = (int)y1;
                                dat[kk].X = (int)x1;
                                kk += 1;
                            }
                        }
                        else
                        {
                            for (k = nmi; k <= nmf; k++)
                            {
                                if (kk >= lar) break;
                                dif = pro - (int)(cu[nucod][k] * ampamp);
                                diff = dif * fy;
                                y1 = (float)(iniy - diff);
                                x1 = (float)(10.0 + (tim[nucod][k] - t1amp) * fax);
                                dat[kk].Y = (int)y1;
                                dat[kk].X = (int)x1;
                                kk += 1;
                            }
                        }
                    }
                    else
                    {
                        if (invertido[nucod] == false)
                        {
                            for (k = nmi; k <= nmf; k++)
                            {
                                if (kk >= lar) break;
                                dif = (int)(cf[k] * ampamp) - pro;
                                diff = dif * fy;
                                y1 = (float)(iniy - diff);
                                x1 = (float)(10.0 + (tim[nucod][k] - t1amp) * fax);
                                dat[kk].Y = (int)y1;
                                dat[kk].X = (int)x1;
                                kk += 1;
                            }
                        }
                        else
                        {
                            for (k = nmi; k <= nmf; k++)
                            {
                                if (kk >= lar) break;
                                dif = pro - (int)(cf[k] * ampamp);
                                diff = dif * fy;
                                y1 = (float)(iniy - diff);
                                x1 = (float)(10.0 + (tim[nucod][k] - t1amp) * fax);
                                dat[kk].Y = (int)y1;
                                dat[kk].X = (int)x1;
                                kk += 1;
                            }
                        }
                    }
                    dc.DrawLines(lapizA, dat);
                    x1 = (float)(10.0 + (Ati - t1amp) * fax);
                    x2 = (float)(10.0 + ((Ati + periodo) - t1amp) * fax);
                    dc.DrawLine(lapizAb, x1, yf - 5.0F, x2, yf - 5.0F);
                    lapizA.Dispose();
                    lapizAb.Dispose();
                }
                catch
                {
                }
            }

            brocha.Dispose();
            DibujoCodaAmp();

            return;
        }
        /// <summary>
        /// Reduce el tamaño de la lectura de la traza que se tiene en el panel de amplitud.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void bocomam_Click(object sender, EventArgs e)
        {// >< Ampl
            double tii, fac;

            fac = (t2amp - t1amp) / 3.0;
            tii = t2amp + fac;
            if (tii >= t2cod) return;
            t2amp = tii;
            panelAmp.Invalidate();
            return;
        }
        /// <summary>
        /// Amplifica el tamaño de la lectura de la traza que se tiene en el panel de amplitud.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void bodilam_Click(object sender, EventArgs e)
        {// <> Ampl
            double tii, fac;

            fac = (t2amp - t1amp) / 3.0;
            tii = t2amp - fac;
            if (tii - t1amp < 0.1) return;
            t2amp = tii;
            panelAmp.Invalidate();
            return;
        }
        /// <summary>
        /// Desplaza la lectura de la traza hacia la derecha en el panel de amplitud.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void boderam_Click(object sender, EventArgs e)
        {// >> Ampl
            double tii1, tii2, fac;

            fac = (t2amp - t1amp) / 3.0;
            tii1 = t1amp - fac;
            tii2 = t2amp - fac;
            if (tii1 < t1cod) return;
            t1amp = tii1;
            t2amp = tii2;
            panelAmp.Invalidate();
            return;
        }
        /// <summary>
        /// Desplaza la lectura de la traza hacia la izquierda en el panel de amplitud.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void boizqam_Click(object sender, EventArgs e)
        {// << Ampl
            double tii1, tii2, fac;

            fac = (t2amp - t1amp) / 3.0;
            tii1 = t1amp + fac;
            tii2 = t2amp + fac;
            //MessageBox.Show("t1amp="+t1amp.ToString()+" t1cod="+t1cod.ToString()+" tii1="+tii1.ToString()+" t2cod="+t2cod.ToString()+" t2amp="+t2amp.ToString()+" tii2="+tii2.ToString());
            if (tii2 > t2cod)
                return;
            t1amp = tii1;
            t2amp = tii2;
            panelAmp.Invalidate();
            return;
        }
        /// <summary>
        /// Modifica el valor de la variable pto que sirve para ver o no los puntos de digitalización.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void bopto_Click(object sender, EventArgs e)
        {// visualiza o no los puntos de digitalizacion.
            if (pto == false)
                pto = true;
            else
                pto = false;
            panelAmp.Invalidate();
            return;
        }
        /// <summary>
        /// Dibuja en el panel de coda, el sector correspondiente al periodo y amplitud leídas.
        /// </summary>
        void DibujoCodaAmp()
        {
            int xf, yf, i, k, kk, lar, max, min, pro, dif = 0, nm1, nm2, tot;
            int[] nmi = new int[3];
            int[] nmf = new int[3];
            float x1, y1;
            double dura, fax, fay, fy, diff, iniy, fxsat, fmsat;
            Point[] dat;


            if (panelAmp.Visible == false)
                return;

            xf = panelcoda.Size.Width - 70;
            yf = panelcoda.Size.Height - 30;
            dura = t2cod - t1cod;
            fax = xf / dura;
            fay = 4.0 * yf / 5.0;

            tot = tim[nucod].Length;
            for (k = 0; k < tot; k++)
            {
                if (tim[nucod][k] >= t1cod) break;
            }
            nm1 = k;
            for (k = nm1; k < tot; k++)
            {
                if (tim[nucod][k] > t2cod || tim[nucod][k] <= 0) break;
            }
            nm2 = k - 1;
            if (filtcod == false)
            {
                max = cu[nucod][nm1];
                min = max;
                for (k = nm1 + 1; k < nm2; k++)
                {
                    if (max < cu[nucod][k])
                        max = cu[nucod][k];
                    else if (min > cu[nucod][k])
                        min = cu[nucod][k];
                }
            }
            else
            {
                max = cf[nm1];
                min = max;
                for (k = nm1 + 1; k < nm2; k++)
                {
                    if (max < cf[k]) max = cf[k];
                    else if (min > cf[k]) min = cf[k];
                }
            }

            nmi[0] = nm1;
            nmf[2] = nm2;
            for (k = 0; k < tot; k++)
            {
                if (tim[nucod][k] >= t1amp) break;
            }
            nmi[1] = k;
            nmf[0] = k;
            for (k = 0; k < tot; k++)
            {
                if (tim[nucod][k] >= t2amp) break;
            }
            nmi[2] = k;
            nmf[1] = k;

            Graphics dc = panelcoda.CreateGraphics();
            Pen lapiz;

            try
            {
                for (i = 0; i < 3; i++)
                {
                    if (i != 1) lapiz = new Pen(colinea, 1);
                    else lapiz = new Pen(Color.Orange, 1);
                    lar = nmf[i] - nmi[i];
                    if (analogico == false && analogcoda == false)
                        pro = (int)((max + min) / 2.0F);
                    else
                    {
                        pro = promEst[nucod];
                        max = pro + (int)(CuentasAnalogico / 2.0);
                        min = pro - (int)(CuentasAnalogico / 2.0);
                    }
                    if (max - pro != 0)
                        fy = ((fay / 2) / ((max - pro)));
                    else
                        fy = 1;
                    //pro = (int)(pro * ampcod);
                    iniy = panelcoda.Size.Height / 2.0;
                    dat = new Point[lar];
                    kk = 0;
                    if (satu == false)
                    {
                        if (filtcod == false)
                        {
                            for (k = nmi[i]; k < nmf[i]; k++)
                            {
                                if (kk >= lar) break;
                                dif = cu[nucod][k] - pro;
                                diff = dif * fy;
                                y1 = (float)(iniy - diff);
                                x1 = (float)(40.0 + (tim[nucod][k] - t1cod) * fax);
                                dat[kk].Y = (int)y1;
                                dat[kk].X = (int)x1;
                                kk += 1;
                            }
                        }
                        else
                        {
                            for (k = nmi[i]; k < nmf[i]; k++)
                            {
                                if (kk >= lar) break;
                                dif = cf[k] - pro;
                                diff = dif * fy;
                                y1 = (float)(iniy - diff);
                                x1 = (float)(40.0 + (tim[nucod][k] - t1cod) * fax);
                                dat[kk].Y = (int)y1;
                                dat[kk].X = (int)x1;
                                kk += 1;
                            }
                        }
                    }
                    else
                    {
                        dif = max - pro;
                        diff = dif * fy;
                        fxsat = diff;   // fxsat y fmsat, son variables para la opcion de saturacion.
                        dif = min - pro;
                        diff = dif * fy;
                        fmsat = diff;
                        if (filtcod == false)
                        {
                            for (k = nmi[i]; k < nmf[i]; k++)
                            {
                                if (kk >= lar) break;
                                dif = cu[nucod][k] - pro;
                                diff = dif * fy;
                                if (diff > fxsat) diff = fxsat;
                                else if (diff < fmsat) diff = fmsat;
                                y1 = (float)(iniy - diff);
                                x1 = (float)(40.0 + (tim[nucod][k] - t1cod) * fax);
                                dat[kk].Y = (int)y1;
                                dat[kk].X = (int)x1;
                                kk += 1;
                            }
                        }
                        else
                        {
                            for (k = nmi[i]; k < nmf[i]; k++)
                            {
                                if (kk >= lar) break;
                                dif = cf[k] - pro;
                                diff = dif * fy;
                                if (diff > fxsat) diff = fxsat;
                                else if (diff < fmsat) diff = fmsat;
                                y1 = (float)(iniy - diff);
                                x1 = (float)(40.0 + (tim[nucod][k] - t1cod) * fax);
                                dat[kk].Y = (int)y1;
                                dat[kk].X = (int)x1;
                                kk += 1;
                            }
                        }
                    }
                    dc.DrawLines(lapiz, dat);
                    lapiz.Dispose();
                }
            }
            catch
            {
            }

            //MessageBox.Show("fin");
            return;
        }
        /// <summary>
        /// Inicio del intervalo de lectura en el panel de amplitud.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void panelAmp_MouseDown(object sender, MouseEventArgs e)
        {
            // inicio del intervalo de lectura en el panel de amplitud
            baxi = e.X;
            bayi = e.Y;
            return;
        }
        /// <summary>
        /// Marca el fin del intervalo de lectura del panel de amplitud. Dicho intervalo equivale al Periodo. 
        /// Se busca además el valor máximo de cuentas pico a pico.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void panelAmp_MouseUp(object sender, MouseEventArgs e)
        {
            int baxf, bayf, xf, yf, nmi, nmf, max, min, k, tot;
            double fax, facra, Vel, mmW, sp = 0, val = 8.0;
            double ti1, ti2, tii;
            // Para estimacion ML con S-P.
            float Mrc = -5.0F;

            labelMrc.Text = "";
            baxf = e.X;
            bayf = e.Y;
            xf = panelAmp.Size.Width - 10;
            yf = panelAmp.Size.Height;
            fax = (t2amp - t1amp) / xf;
            facra = 1.0 / ra[nucod];

            ti1 = t1amp + ((baxi - 10.0) * fax);
            ti2 = t1amp + ((baxf - 10.0) * fax);

            if (baxf == baxi)
                return;
            else
            {
                if (clSola > -1)
                    boClaSola.Visible = true;
                if (ti1 > ti2)
                {
                    tii = ti1;
                    ti1 = ti2;
                    ti2 = tii;
                }
                Ati = ti1; // tiempo inicial del intervalo de amplitud.
                //MessageBox.Show("ti1=" + ti1.ToString() + " ti2=" + ti2.ToString());
                if (Ati > 0 && (Ati < Pti || (Ati > Cti && Cti > 0)) && tremor == false)
                {
                    NoMostrar = true;
                    MessageBox.Show("Amplitud por fuera del intervalo P-CODA ???");
                    Ati = 0;
                    return;
                }
                if (ti2 - ti1 > facra)
                    periodo = (float)(ti2 - ti1);// intervalo de lectura.
                else
                    periodo = 0;

                tot = tim[nucod].Length;
                for (k = 0; k < tot; k++)
                {
                    if (tim[nucod][k] >= ti1)
                    {
                        break;
                    }
                }
                nmi = k;
                for (k = nmi; k < tot; k++)
                {
                    if (tim[nucod][k] > ti2 || tim[nucod][k] <= 0)
                    {
                        break;
                    }
                }
                nmf = k;

                if (filtcod == false)
                {
                    max = cu[nucod][nmi];
                    min = max;
                    for (k = nmi + 1; k <= nmf; k++)
                    {
                        if (max < cu[nucod][k])
                            max = cu[nucod][k];
                        else if (min > cu[nucod][k])
                            min = cu[nucod][k];
                    }
                }
                else
                {
                    max = cf[nmi];
                    min = max;
                    for (k = nmi + 1; k <= nmf; k++)
                    {
                        if (max < cf[k])
                            max = cf[k];
                        else if (min > cf[k])
                            min = cf[k];
                    }
                }
                ampp = max - min;
                if (ampp < 1)
                    ampp = 0;
                panelAmp.Invalidate();
                bovar.Visible = true;

                if (tremor == true && DesRed == true)
                {
                    ip1 = nmi;
                    ip2 = nmf;
                    NoInterpol = true;
                    CalculoInterpolacion(id);
                    IntegracionSpl(spl);
                }
                if (Sti > 0 && Pti > 0)
                    sp = Sti - Pti;
                Mrc = -5.0F;
                if (periodo > 0 && ampp > 0 && fcnan[nucod] > 0 && sp > 0)
                {
                    //if (sp < 5) val = 3.5;
                    //else val = 8.0;
                    Vel = (0.5 * (ampp / (double)(ga[nucod])) * fcnan[nucod]) / 1000.0;
                    //mmW = 2.8*(Vel*periodo/(2.0*Math.PI));
                    mmW = 1.4 * (Vel * periodo / Math.PI);
                    Mrc = (float)(Math.Log10(mmW) + 3.0 * Math.Log10(val * sp) - 2.92);
                    //MessageBox.Show("mrc=" + Mrc.ToString());// integrar la señal?
                    labelMrc.Text = "Mrc: " + string.Format("{0:0.0}", Mrc);
                }

            }

            return;
        }
        /// <summary>
        /// Rutina que lee el archivo donde se guardan los valores de las lecturas cuando 
        /// se realizan lecturas para varios sismos, y guarda dichas lecturas en la base.
        /// </summary>
        /// <param name="clasi">Clasificación con la que se guarda el sismo.</param>
        /// <param name="clas">Clasificación del sismo.</param>
        /// <param name="ss">Fecha formateada de la traza. ej: 20140318122045 año-mes-día-hora-minuto-segundo.</param>
        void variasamplitudes(string clasi, string clas, string ss)
        {
            int cont = 0, codd, ga, cu;
            float mm = 0, fre = 0, sp = 0, rata = 0, pe = 0;
            double A, P, C;
            string let = "", lin = "", ca = "", esta2 = "", sis = "", typ = "";


            StreamReader ar2 = new StreamReader("amplivarias.txt");
            do
            {
                P = 0; pe = 0; ga = 1; cu = 0;
                C = 0; rata = 0; sp = 0;
                lin = "";
                lin = ar2.ReadLine();
                if (lin == null) break;

                codd = int.Parse(lin.Substring(14, 6));  // la coda
                mm = float.Parse(lin.Substring(21, 10)); // mm en los analogicos si hay factor de conversion
                pe = float.Parse(lin.Substring(32, 5));  // periodo 
                sp = float.Parse(lin.Substring(38, 5));  // S-P
                esta2 = lin.Substring(44, 5);     // nombre de la estacion
                cu = int.Parse(lin.Substring(50, 9));  // cuentas pico a pico
                ga = int.Parse(lin.Substring(60, 2));  // ganacia de las cuentas
                A = double.Parse(lin.Substring(63, 13)); // tiempo de la lectura del inicio de la amplitud
                typ = lin.Substring(77, 1); // tipo de sensor (periodo corto o banda ancha)
                P = double.Parse(lin.Substring(79, 13)); // tiempo de la P
                C = double.Parse(lin.Substring(93, 13)); // tiempo del final de la coda
                rata = float.Parse(lin.Substring(107, 6)); // muestras/segundo
                sis = ss.Substring(4, 8) + "." + clasi.Substring(0, 3); // nombre del archivo
                let = ""; // variable que guarda la ruta donde se guardan los datos en la Base.
                let = rutbas + "\\lec\\" + clas.Substring(0, 2) + "\\" + clasi[0] + clasi[2] + ss.Substring(2, 6) + ".txt";
                if (pe >= 0.001) fre = 1.0F / pe;
                else fre = 0;
                if (fre > 99.99F)
                    fre = 99.99F;
                ca = "";// variable que guarda los valores que se van a grabar
                ca = string.Format("{0,13:0.00}", tie1) + " " + string.Format("{0,6:0}", codd) + " ";
                ca += clasi.Substring(0, 3) + " " + String.Format("{0,10:0.0}", mm) + " ";
                ca += string.Format("{0,5:0.00}", pe) + " " + string.Format("{0,5:0.00}", fre) + " ";
                ca += string.Format("{0,6:0.00}", sp) + " ";
                ca += esta2.Substring(0, 5) + " " + string.Format("{0,9:0}", cu) + " ";
                ca += string.Format("{0:00}", ga) + " " + sis + " " + string.Format("{0,13:0.00}", A) + " ";
                if (by[nucod] == 2)
                    ca += 'C' + " ";
                else
                    ca += 'B' + " ";
                ca += string.Format("{0,13:0.00}", P) + " " + string.Format("{0,13:0.00}", C) + " " + string.Format("{0,6:0.00}", ra[nucod]) + " " + usu.Substring(0, 3) + " " + tar[nucod];

                StreamWriter pr2 = File.AppendText(let);
                pr2.WriteLine(ca);
                pr2.Close();
                cont += 1;
            } while (cont < nuampvar);
            ar2.Close();

            return;
        }
        /// <summary>
        /// Boton que efectua las lecturas cuando hay varios sismos y las graba en el archivo amplivarias.txt,
        /// para ser posteriormente leidas y grabadas en la Base.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void bovar_Click(object sender, EventArgs e)
        {
            int codd;
            float sp;
            string ca = "";

            if (Pti == 0 || Ati == 0)
                return;

            if (Cti > 0)
                codd = (int)(Cti - Pti);
            else
                codd = (int)(tie2 - Pti);
            if (Sti > 0)
                sp = (float)(Sti - Pti);
            else
                sp = 0;
            //MessageBox.Show("Sti="+Sti.ToString()+" Pti="+Pti.ToString()+" sp="+sp.ToString());
            if (sp < 0)
                sp = 0;
            if (sp > 300)
            {
                NoMostrar = true;
                DialogResult result = MessageBox.Show("S-P > 300 segundos, CONTINUAR??",
                                "CONTINUAR ???", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.No) return;
            }
            StreamWriter pr = File.AppendText("amplivarias.txt");
            ca = string.Format("{0,13:0.00}", tie1) + "_" + string.Format("{0,6:0}", codd) + "_";
            ca += string.Format("{0,10:0.0}", ampp) + "_" + string.Format("{0,5:0.00}", periodo) + "_";
            ca += string.Format("{0,5:0.00}", sp) + "_";
            if (refe == true) ca += "*";
            else ca += " ";
            ca += est[nucod].Substring(0, 4) + "_";
            ca += string.Format("{0,9:0}", ampp) + "_" + string.Format("{0,2:0}", ga[nucod]) + "_";
            ca += string.Format("{0,13:0.00}", Ati) + " ";
            if (by[nucod] == 2) ca += "C";
            else ca += "B";
            ca += " " + string.Format("{0,13:0.00}", Pti) + " " + string.Format("{0,13:0.00}", Cti) + " ";
            ca += string.Format("{0,6:0.00}", ra[nucod]) + " " + usu.Substring(0, 3) + " " + tar[nucod];
            pr.WriteLine(ca);
            pr.Close();
            bovar.BackColor = Color.BlueViolet;
            nuampvar += 1;
            bovar.Text = nuampvar.ToString();
            //panelAmp.Invalidate();
            panelAmp.Visible = false;
            leido = true;
            TrazasClas();
            DibujoClascoda();

            return;
        }
        /// <summary>
        /// Revisa que se hayan grabado las lecturas y la traza en la Base.
        /// </summary>
        /// <param name="nomba">Ruta de la base.</param>
        /// <param name="sis">Nombre del archivo con el que se graba el sismo, ej "03181021.MLP"</param>
        /// <param name="lincla">Linea que se guarda en el archivo de sismos clasificados.</param>
        /// <param name="letlec">Ruta completa de donde se guarda el archivo.</param>
        /// <param name="dd">Fecha del sismo completa.</param>
        /// <param name="cc">Nombre del archivo con el que se graba el sismo, ej "03181021.MLP"</param>
        void RevisaGrabacion(string nomba, string sis, string lincla, string letlec, double dd, char cc)
        {
            int i, j;

            i = lincla.Length;
            j = letlec.Length;
            if (j == 0)
            {
                nolec = true;
                tigrabacion = dd;
                leclec = cc;
            }
            else
            {
                nolec = false;
                tigrabacion = 0;
                leclec = ' ';
            }
            //MessageBox.Show(" i=" + i.ToString() + " lincla=" + lincla+" j="+j.ToString()+" dd="+dd.ToString());
            if (!File.Exists(nomba))
            {
                NoMostrar = true;
                MessageBox.Show("OJO: NO se GRABO la traza " + sis + " en la BASE!!!");
            }

            return;
        }
        /// <summary>
        /// Rutina que Graba Todos los datos del sismo en la Base en su propio archivo y además graba
        /// en el archivo cla.txt el sismo como ya clasificado.
        /// </summary>
        /// <returns></returns>
        int GrabaBase()
        {
            int dur, i, nue, codd, cont = 0;
            long ll;
            float fre = 0, sp = 0;
            double dd, incre, tt1, tt2, mm = 0;
            string let = "", letcla = "", letlec = "", nom = "", ss = "", sis = "", fee = "", hoo = "", ca = "";
            string clasi = "", nomba = "", esp = "", esp2 = "", aste = "", lincla = "", li2 = "";
            char[] str2 = new char[4];
            char letiem = 'P';
            char[] delim = { ' ', '\t' };
            string[] pa = null;
            bool gralec = true, si = false;

            if (panelmarca.Visible == true)
                panelmarca.Visible = false;
            if ((tie2 - tie1) < 10.0 && tremor == false)
            {
                NoMostrar = true;
                MessageBox.Show("La duracion es menor de 10 segundos!!\nNO SE GRABA NADA EN LA BASE!!!");
                return (-1);
            }
            if (estru30 == true)
                letiem = 'P';
            else
                letiem = 'I';
            if (tremor == false)
            {
                if ((Pti == 0 || Ati == 0 || periodo == 0 || clas[1] == '_'))
                {
                    if (Pti == 0) let += "no P  ";
                    if (Ati == 0) let += "no Amp  ";
                    if (periodo == 0) let += "no Periodo  ";
                    if (clas[1] == '_') let += "No Clasi ";
                    let += "\n\n";
                    let += " SALIR???\n\n";
                    NoMostrar = true;
                    DialogResult result = MessageBox.Show(let, "", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                        return (-1);
                }
            }
            else
            {
                if (tremofin == false) incre = incTremor;
                else
                {
                    incre = tifintremor - tinitremor;
                }
                if (incre <= 0)
                {
                    NoMostrar = true;
                    MessageBox.Show("NO se ha leido el final del tremor?? (revise)");
                    return (0);
                }
                tie1 = tinitremor;
                tie2 = tinitremor + incre;
                Pti = tie1;
                Cti = tie2;
            }

            if (Pti == 0 && Ati == 0 && periodo == 0)
                gralec = false;
            else
                gralec = true;

            mm = 0;
            fre = 0;
            if (ga[nucod] <= 0)
                ga[nucod] = 1;
            if (factmm > 0)
                mm = ((double)(ampp) / (double)(ga[nucod])) * factmm;
            if ((int)(mm) == ampp && ampp > 0)
            {
                NoMostrar = true;
                MessageBox.Show("Los milimetros son iguales a las cuentas!!\nUtilice el Revisor para arreglar!!");
            }

            panelcla.Visible = false;
            panelcoda.Visible = false;
            bofilcod.BackColor = Color.White;
            panelAmp.Visible = false;
            panel2.Visible = true;
            panel2.BringToFront();
            util.Mensaje(panel2, "Grabando en la Base...\n  Espere por favor....", false);

            if (clas[2] == '+')
                clasi = volcan[vol][0] + clas.Substring(0, 2);
            else
            {
                clasi = 'X' + clas.Substring(0, 2);
            }
            nue = 0;
            for (i = 0; i < nutra; i++)
                if (siEst[i] == true)
                    nue += 1;
            if (nue == 0)
                return (-1);
            dur = (int)(tie2 - tie1);
            dd = Fei + tie1 * 10000000.0;
            ll = (long)(dd);
            DateTime fech = new DateTime(ll);
            ss = string.Format("{0:yyyy}{0:MM}{0:dd}{0:HH}{0:mm}{0:ss}", fech);
            nom = ss.Substring(2, 4) + ".txt";
            añoML = long.Parse(ss.Substring(0, 8));
            sis = ss.Substring(4, 8) + "." + clasi.Substring(0, 3);
            nomba = rutbas + "\\sud\\" + clasi.Substring(1, 2) + "\\" + ss.Substring(2, 2) + "\\" + sis;
            // MessageBox.Show("ss=" + ss + "\n" + nomba+"\nUTdisp="+UTdisp.ToString());
            if (File.Exists(nomba))
            {
                NoMostrar = true;
                FileInfo ff = new FileInfo(nomba);
                ca = "El sismo Existe en la Base. Tamaño=" + ((int)(ff.Length / 1000.0)).ToString() + " Kbytes. Salir??";
                DialogResult result = MessageBox.Show(ca, "", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    panel2.Visible = false;
                    return (-1);
                }
            }
            letcla = "";
            letcla = rutbas + "\\cla\\" + nom;
            fee = ss.Substring(2, 6) + " ";
            hoo = ss.Substring(8, 2) + ":" + ss.Substring(10, 2) + ":" + ss.Substring(12, 2) + " ";
            if (dur < 10000) esp = " ";
            else esp = "";
            if (nue < 100)
                esp2 = " ";
            else
                esp2 = "";
            //ca = string.Format("{0:0000}",dur)+esp+usu.Substring(0,3)+" "+string.Format("{0:00}", nue);
            ca = string.Format("{0:0000}", dur) + esp + usu.Substring(0, 3) + esp2 + string.Format("{0:00}", nue);
            lincla = fee + hoo + sis + " " + letiem + tar[nucod] + marca + " " + ca;

            // Datos Clasificacion
            si = false;
            panel2.Visible = true;
            cont = 0;
            util.Mensaje(panel2, "Copiando en CLA....", false);
            do
            {
                try
                {
                    if (!File.Exists(letcla))
                    {
                        StreamWriter wr2 = File.AppendText(letcla);
                        wr2.Write(lincla);
                        wr2.Close();
                        continue;
                    }
                    FileInfo ff1 = new FileInfo(letcla);
                    File.Copy(letcla, "cla.txt", true);
                    StreamWriter pr = File.AppendText("cla.txt");
                    pr.Write(lincla);
                    for (i = 0; i < nutra; i++)
                    {
                        if (siEst[i] == true)
                            pr.Write(" " + est[i].Substring(0, 4));
                    }
                    pr.WriteLine();
                    pr.Close();

                    FileInfo ff2 = new FileInfo("cla.txt");
                    if (ff2.Length >= ff1.Length)
                    {
                        File.Copy("cla.txt", letcla, true);
                        si = true;
                    }
                    try
                    {
                        if (File.Exists(letcla))
                        {
                            li2 = rutbas + "\\temp\\" + nom;
                            if (File.Exists(li2))
                            {
                                FileInfo f1 = new FileInfo("cla.txt");
                                long l1 = f1.Length;
                                FileInfo f2 = new FileInfo(li2);
                                long l2 = f2.Length;
                                if (l1 > l2)
                                {
                                    File.Copy(letcla, li2, true);
                                }
                            }
                            else
                            {
                                ca = rutbas + "\\temp";
                                if (!Directory.Exists(ca)) Directory.CreateDirectory(ca);
                                File.Copy("cla.txt", li2, true);
                            }
                        }
                    }
                    catch
                    {
                        MessageBox.Show("Error 1");
                    }
                }
                catch
                {
                    MessageBox.Show("Error 2");
                }
                cont += 1;
            } while (si == false && cont < 5000);
            panel2.Visible = false;
            if (cont >= 1000) MessageBox.Show("Es posible que NO se haya grabado en CLA!! REvise");

            // Datos Amplitud, coda, etc.
            if (nuampvar > 0)
                variasamplitudes(clasi, clas, ss);
            else if (gralec == true)
            {
                if (Cti > 0)
                    codd = (int)(Cti - Pti);
                //else codd = (int)(tie2 - Pti);
                else codd = 0;
                if (codd < 0) codd = 0;
                if (codd > 999999) codd = 0;
                letlec = "";
                letlec = rutbas + "\\lec\\" + clas.Substring(0, 2) + "\\" + clasi[0] + clasi[2] + ss.Substring(2, 6) + ".txt";
                if (periodo >= 0.001)
                {
                    fre = 1.0F / periodo;
                    if (fre > 99.99F) fre = 99.99F;
                }
                else
                {
                    periodo = 0;
                    fre = 0;
                }
                if (Sti > 0)
                {
                    sp = (float)(Sti - Pti);
                    if (sp < 0 || sp > 999.0) sp = 0;
                }
                else
                    sp = 0;
                if (tremor == true)
                {
                    if (contremor == 0)
                        aste = "*";
                    else
                        aste = "+";
                }
                else
                    aste = " ";
                if (factmm < 0)
                    factmm = 0;
                ca = string.Format("{0,13:0.00}", tie1) + " " + string.Format("{0,6:0}", codd) + " ";
                ca += clasi.Substring(0, 3) + aste;
                //MessageBox.Show("ampp="+ampp.ToString()+" ga="+ga[nucod].ToString()+" factmm="+factmm.ToString()+" mm="+mm.ToString());
                if (mm < 99999999.9)
                    ca += String.Format("{0,10:0.0}", mm) + " ";
                else
                    ca += string.Format("{0:E3}", mm) + " ";
                ca += string.Format("{0,5:0.00}", periodo) + " " + string.Format("{0,5:0.00}", fre) + " ";
                ca += string.Format("{0,6:0.00}", sp) + " ";
                if (refe == true)
                    ca += "*";
                else
                    ca += " ";
                ca += est[nucod].Substring(0, 4) + " " + string.Format("{0,9:0}", ampp) + " ";
                ca += string.Format("{0:00}", ga[nucod]) + " " + sis + " " + string.Format("{0,13:0.00}", Ati) + " ";
                if (by[nucod] == 2)
                    ca += 'C' + " ";
                else
                    ca += 'B' + " ";
                ca += string.Format("{0,13:0.00}", Pti) + " " + string.Format("{0,13:0.00}", Cti) + " ";
                ca += string.Format("{0,6:0.00}", ra[nucod]) + " " + usu.Substring(0, 3) + " " + tar[nucod];

                if (filtcod == true)
                {
                    if (tipofiltcod == false)
                        ca += " B ";
                    else
                        ca += " A ";
                    ca += string.Format("{0:00.00} {1:0}", Fccod, polocod);
                }

                try
                {
                    StreamWriter pr2 = File.AppendText(letlec);
                    pr2.WriteLine(ca);
                    pr2.Close();
                }
                catch
                {
                    ca = rutbas + "\\lec\\" + clas.Substring(0, 2);
                    if (!Directory.Exists(ca))
                    {
                        NoMostrar = true;
                        MessageBox.Show("NO EXISTE la carpeta " + ca + " en la BASE!!!!");
                    }
                }
            }

            // si Hay huecos
            if (nuhueco > 0 && huecolist.Count > 0)
            {
                ca = rutbas + "\\hue\\" + ss.Substring(2, 2);
                if (!Directory.Exists(ca))
                    Directory.CreateDirectory(ca);
                ca += "\\" + sis;
                str2 = est[nucod].Substring(0, 4).ToCharArray();
                FileInfo arhu = new FileInfo(ca);
                BinaryWriter brhu = new BinaryWriter(arhu.OpenWrite());

                brhu.Write(str2);
                brhu.Write((short)(nuhueco));
                for (i = 0; i < huecolist.Count; i++)
                {
                    pa = huecolist[i].ToString().Split(delim);
                    tt1 = double.Parse(pa[0]);
                    tt2 = double.Parse(pa[1]);
                    brhu.Write(tt1);
                    brhu.Write(tt2);
                }

                brhu.Close();
            }

            //Trazas en formato SUDS Demultiplexado
            GrabaSuds(0);
            listabox = true;
            i = listBox1.SelectedIndex;
            LLenaBox();
            try
            {
                listBox1.SelectedIndex = i;
            }
            catch
            {
                listBox1.SelectedIndex = 0;
            }
            panel2.Visible = false;
            butarch.Visible = true;

            RevisaGrabacion(nomba, sis, lincla, letlec, tie1, sis[11]);

            //boAnotacion.Visible = true;

            return (1);
        }
        /// <summary>
        /// Rutina que graba las trazas en la Base en formato SUDS Demultiplexado.
        /// </summary>
        /// <param name="bas">en la ejecución del programa se lo invoca con 3 parámetros diferentes (0, 1 y 2). 
        ///Cuando se graba con el parámetro = 0 es el formato por defecto.
        ///Cuando se graba con el parámetro = 1 es para ser leída por el PSW.
        ///Cuando se graba con el parámetro = 2 es para ser leída por el SCILAB.
        ///</param>
        void GrabaSuds(short bas)
        {
            int i, k, nmi, nmf, lar, tot;
            long ll;
            double dd;
            string nom = "", ss = "";
            char[] tag1 = { 'S', '6' };
            ushort tag2 = 5;
            int tag3 = 76, tag4 = 0, totmu = 0;
            char[] str1 = { 'I', 'N', 'G', 'E' };
            char[] str2 = new char[5];
            char[] str3 = new char[1];
            short str4 = 0, str5 = 0, str6 = 0;
            long str7 = 0, str8 = 0;
            float str9 = 0;
            char str10 = 's', str11 = 'n', str12 = 'n', str13 = 'v';
            short str14 = 0;
            char str15 = 'n', str16 = 'v';
            char str17 = 'l';//data type
            char str18 = 'g', str19 = 'n', str20 = 'g';
            float str21 = 0, str22 = 0, str23 = 0;
            short str24 = 0, str25 = 1;// str25 es la ganancia
            int str26;// tiempo
            float str27 = 0, str28 = 0;
            short sie2 = -5;
            char sie3 = 'l', sie4 = '?';
            short sie5 = 0, sie6 = 0;
            uint sie7;//numero de muestras
            float sie8;//rata
            float sie9 = 0, sie10 = 0, sie11 = 0;
            int sie12 = 0;
            double sie13 = 0.0001;
            float sie14 = 0;
            string dir = "", clasi = "";
            int[] cue = new int[10];

            sismo = "";
            //MessageBox.Show("bas=" + bas.ToString());
            if (bas == 0)
            {
                if (clas[2] == '+')
                    clasi = volcan[vol][0] + clas.Substring(0, 2);
                else
                    clasi = 'X' + clas.Substring(0, 2);
                dd = Fei + tie1 * 10000000.0;
                ll = (long)(dd);
                DateTime fech = new DateTime(ll);
                ss = string.Format("{0:yyyy}{0:MM}{0:dd}{0:HH}{0:mm}{0:ss}", fech);
                dir = rutbas + "\\sud\\" + clas.Substring(0, 2) + "\\" + ss.Substring(2, 2);
                //MessageBox.Show("dir=" + dir);
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                sismo = ss.Substring(4, 8) + "." + clasi.Substring(0, 3);
                nom = rutbas + "\\sud\\" + clas.Substring(0, 2) + "\\" + ss.Substring(2, 2) + "\\" + sismo;
                nomsud = nom;
            }
            else if (bas == 1)
                nom = "psw.dmx";
            else if (bas == 2)
                nom = "demux.dmx";

            //MessageBox.Show("nom=" + nom);
            FileInfo ar = new FileInfo(nom);
            BinaryWriter br = new BinaryWriter(ar.OpenWrite());

            if (estru30 == true)
                sie13 = 0.0001;
            else
                sie13 = -0.0001;
            for (i = 0; i < nutra; i++)
            {
                //MessageBox.Show("i="+i.ToString()+" siest="+siEst[i].ToString());                
                if (siEst[i] == true && (bas < 2 || (bas == 2 && i == id)))
                {
                    tag2 = 5;
                    tag3 = 76;
                    tag4 = 0;
                    br.Write(tag1);
                    br.Write(tag2);
                    br.Write(tag3);
                    br.Write(tag4);// hasta aqui Tag principal                    
                    //estructura 5
                    str2 = est[i].Substring(0, 5).ToCharArray();
                    str2[4] = '\0';
                    str3[0] = comp[i];// hasta aqui el statident del suds
                    if (ga[i] <= 0)
                        ga[i] = 1;
                    str25 = ga[i];
                    if (by[i] == 4)
                        str17 = 'l';
                    else
                        str17 = 'i';
                    str26 = (int)(tie1);
                    br.Write(str1);
                    br.Write(str2);
                    br.Write(str3);
                    br.Write(str4);
                    br.Write(str5);
                    br.Write(str6);
                    br.Write(str7);
                    br.Write(str8);
                    br.Write(str9);
                    br.Write(str10);
                    br.Write(str11);
                    br.Write(str12);
                    br.Write(str13);
                    br.Write(str14);
                    br.Write(str15);
                    br.Write(str16);
                    br.Write(str17);
                    br.Write(str18);
                    br.Write(str19);
                    br.Write(str20);
                    br.Write(str21);
                    br.Write(str22);
                    br.Write(str23);
                    br.Write(str24);
                    br.Write(str25);
                    br.Write(str26);
                    br.Write(str27);
                    br.Write(str28);

                    //tag estructura numero 7 del formato SUDS Demultiplexado
                    tag2 = 7;
                    tag3 = 64;
                    tot = tim[i].Length;
                    for (k = 0; k < tot; k++)
                    {
                        if (tim[i][k] >= tie1) break;
                    }
                    nmi = k;
                    for (k = nmi; k < tot; k++)
                    {
                        if (tim[i][k] > tie2 || tim[i][k] <= 0) break;
                    }
                    nmf = k - 1;
                    lar = nmf - nmi;
                    totmu = by[i] * lar;
                    //MessageBox.Show(est[i].Substring(0,4));
                    tag4 = totmu;
                    br.Write(tag1);
                    br.Write(tag2);
                    br.Write(tag3);
                    br.Write(tag4);
                    // estru 7
                    if (by[i] == 4)
                        sie3 = 'l';
                    else
                        sie3 = 'i';
                    sie7 = (uint)(lar);
                    sie8 = (float)(ra[i]);

                    //estructura 7
                    br.Write(str1);//statident
                    br.Write(str2);//statident
                    br.Write(str3);//sataident
                    br.Write(str4);//statident                          
                    br.Write(tim[i][nmi]);
                    br.Write(sie2);
                    br.Write(sie3);
                    sie4 = tar[i];
                    br.Write(sie4);
                    br.Write(sie5);
                    br.Write(sie6);
                    br.Write(sie7);
                    br.Write(sie8);
                    br.Write(sie9);
                    br.Write(sie10);
                    br.Write(sie11);
                    br.Write(sie12);
                    br.Write(sie13);
                    br.Write(sie14);
                    if (by[i] == 4)
                    {
                        for (k = nmi; k < nmf; k++) br.Write((int)(cu[i][k]));
                    }
                    else
                    {
                        for (k = nmi; k < nmf; k++) br.Write((short)(cu[i][k]));
                    }
                }// if siest
            }// for i....
            if (marca != "********") // variable que guarda los datos de las marcas de un sismo si existen.
            {
                tag2 = 20;
                tag3 = 8;
                tag4 = 0;
                br.Write(tag1);
                br.Write(tag2);
                br.Write(tag3);
                br.Write(tag4);
                for (i = 0; i < 8; i++) br.Write(marca[i]);
            }
            br.Flush();
            br.Close();

            return;
        }
        /// <summary>
        /// Hace llamado al programa PSW del sismologo Jaime Raigosa.
        /// Graba la última lectura en formato Demultiplexado en el archivo psw.dmx y la abre en el PSW.
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void boPSW_Click(object sender, EventArgs e)
        {
            string lin = "";
            NoMostrar = true;
            if (File.Exists("psw.dmx"))
                File.Delete("psw.dmx");
            GrabaSuds(1);// graba la traza en el directorio de trabajo, para leerla con el PSW.
            lin = "/C .\\psw\\psw.exe psw.dmx";// demux2.dmx " + nomj + " demux.dmx";
            util.Dos(lin, true);
            DibujoTrazas();
            return;
        }
        /// <summary>
        /// Llama al programa libre SCILAB.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void boScilab_Click(object sender, EventArgs e)
        {
            long ll;
            string lin = "", fe3 = "";

            NoMostrar = true;
            if (File.Exists("psw.dmx"))
                File.Delete("psw.dmx");
            GrabaSuds(2);// graba la traza en el directorio de trabajo, para leerla con el SCILAB.
            ll = (long)(Fei + tim[id][0] * 10000000.0);
            DateTime fech = new DateTime(ll);
            fe3 = string.Format("{0:yy}/{0:MM}/{0:dd}-{0:HH}:{0:mm}:{0:ss}", fech);
            if (File.Exists("fecha.txt"))
                File.Delete("fecha.txt");
            StreamWriter wr0 = File.AppendText("fecha.txt");
            wr0.WriteLine(fe3);
            wr0.Close();
            if (File.Exists("c:\\scilab.txt"))
                File.Delete("c:\\scilab.txt");
            StreamWriter wr = File.AppendText("c:\\scilab.txt");
            wr.WriteLine("exec('.\\sci\\demux.sce');");
            wr.Close();
            lin = "/C c:\\scilab\\bin\\wscilex.exe";
            util.Dos(lin, true);// comando que hace llamado al SCILAB. La rutina se encuentra en el formulario Util.cs

            DibujoTrazas();

            return;
        }
        /// <summary>
        /// Hace llamado al formulario2, el cual corresponde a las lecturas de arribos para la
        /// localización con el Hypo71.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void boHypo71_Click(object sender, EventArgs e)
        { // hace llamado al formulario2, el cual corresponde a las lecturas de arribos para la
            // localizacion con el Hypo71.
            Form2 frm2 = new Form2(this);
            frm2.Show();
            frm2.BringToFront();
        }
        /// <summary>
        /// Hace llamado al formulario 3, el cual efectúa las lecturas y estimación de la localización
        /// a partir de la curva de atenuación.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void boAten_Click(object sender, EventArgs e)
        {
            Form3 frm3 = new Form3(this);
            frm3.Show();
            frm3.BringToFront();
        }
        /// <summary>
        /// Permite colocar una linea vertical en el panel de Clasificación la cual sirve de guia.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void boguiCla_Click(object sender, EventArgs e)
        {// Permite colocar una linea vertical en el panel de Clasificación, una vez se active el raticho panel.            
            if (guia == false)
            {
                guia = true;
                boguiCla.BackColor = Color.BlueViolet;
            }
            else
            {
                guia = false;
                boguiCla.BackColor = Color.BlueViolet;
            }
            return;
        }
        /// <summary>
        /// Borra varios de los archivos utilizados al cerrar el programa.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            string ca;

            if (vista == true)
            {
                stop = true;
                splitContainer1.Visible = false;
                vista = false;
            }
            ca = "/C del *.txt";
            util.Dos(ca, true);
            ca = "/C del *.atn";
            util.Dos(ca, true);
            ca = "/C del *.ipn";
            util.Dos(ca, true);
            ca = "/C del *.amp";
            util.Dos(ca, true);
            return;
        }
        /// <summary>
        /// Invierte los colores del fondo.
        /// </summary>
        void Invertir()
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

            if (panelcla.Visible == true)
                DibujoTrazas();
            if (panelcoda.Visible == true)
                panelcoda.Invalidate();
            if (panelAmp.Visible == true)
                panelAmp.Invalidate();

            panel1.Invalidate();
            return;
        }
        /// <summary>
        /// Hace llamado a la rutina que invierte el color del fondo
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void boInv_Click(object sender, EventArgs e)
        {
            Invertir();
        }
        /// <summary>
        /// Repinta los paneles principales (panel1) y (panel1a).
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void panel1_Paint(object sender, PaintEventArgs e)
        {// repinta el panel principal.

            if (desactivado == true && NoMostrar == false)
            {
                Param();
                desactivado = false;
            }
            else
            {
                NoMostrar = false;
                desactivado = false;
            }
            if (panelValFFt.Visible == true)
                return;
            if (panelBarEsp1.Visible == true && moveresp == true)
                return;
            if (stop == true)
            {
                splitContainer1.Visible = false;
                boVista.BackColor = Color.MistyRose;
                stop = false;
                vista = false;
            }
            if (estado == false)
                return;
            try
            {
                if (id >= nutra && nutra > 0)
                    id = 1;
                if (filt == false)
                {
                    Dibujo(panel1, id, cu[id]);
                    if (panel1a.Visible == true)
                    {
                        if (filtx == false)
                            Dibujo(panel1a, ida, cu[ida]);
                        else
                            Dibujo(panel1a, ida, cfx);
                    }
                }
                else
                    Dibujo(panel1, id, cf);
                if (tremor == true && tinitremor > 0)
                    Cuadro_Tremor();
            }
            catch
            {
            }
            if (panelCajTeo.Visible == true)
                PonerTeorico();

            return;
        }
        /// <summary>
        /// Repinta el panel1a.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void panel1a_Paint(object sender, PaintEventArgs e)
        {
            if (panelValFFt.Visible == true)
                return;
            if (filtx == false)
                Dibujo(panel1a, ida, cu[ida]);
            else
                Dibujo(panel1a, ida, cfx);
        }
        /// <summary>
        /// Repinta el panelcoda.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void panelcoda_Paint(object sender, PaintEventArgs e)
        {
            if (nucod < 0)
                return;
            if (ampcod == 1.0F)
                boaumco.BackColor = Color.White;
            else
                boaumco.BackColor = Color.Brown;
            if (filtcod == false)
                Dibujocoda(cu[nucod], nucod);
            else
                Dibujocoda(cf, nucod);
            if (panelbotinfcoda.Visible == true)
            {
                bo1.BackColor = Color.White;
                bo2.BackColor = Color.White;
                bo3.BackColor = Color.White;
            }
            if (panelAmp.Visible == true) 
                DibujoCodaAmp();
            if (nuhueco > 0) 
                DibujarHuecosCoda();
        }
        /// <summary>
        /// Repinta el panel de amplitud.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void panelAmp_Paint(object sender, PaintEventArgs e)
        {
            DibAmpl();
        }
        /// <summary>
        /// Escoge la frecuencia de corte para el filtro del panel principal.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void boFc_MouseDown(object sender, MouseEventArgs e)
        {
            calcfilt = false;
            if (Fc > 0.5)
            {
                if (e.Button == MouseButtons.Left)
                    Fc += 0.5;
                else
                    Fc -= 0.5;
            }
            else
            {
                if (e.Button == MouseButtons.Left)
                    Fc += 0.05;
                else
                    Fc -= 0.05;
            }
            if (Fc <= 0.02)
                Fc = 0.02;
            else if (Fc >= ra[id] / 2.0)
                Fc = ra[id] / 2.0 - 0.5;
            if (Fc >= 0.1)
                boFc.Text = string.Format("{0:0.0Hz}", Fc);
            else
                boFc.Text = string.Format("{0:0.00Hz}", Fc);
            if (filt == true)
                CalculoFiltro(id);
        }
        /// <summary>
        /// Escoge el número de polos para el filtro del panel principal.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void boPolo_MouseDown(object sender, MouseEventArgs e)
        {
            calcfilt = false;
            if (e.Button == MouseButtons.Left)
                polo += 2;
            else
                polo -= 2;
            if (polo < 2)
                polo = 2;
            else if (polo > 8)
                polo = 8;
            boPolo.Text = string.Format("{0:0}", polo);
            if (filt == true)
                CalculoFiltro(id);
        }
        /// <summary>
        /// Llama al filtro en el panel principal (panel1).
        /// </summary>
        /// <param name="iid">id de la estación que se está clasificando.</param>
        void CalculoFiltro(int iid)
        {
            string ca = "";

            panel2.Visible = true;
            ca = "OCTAVE... Calculando Filtro";
            util.Mensaje(panel2, ca, false);
            filt = true;
            if (calcfilt == false)
            {
                FiltroOctave(iid, tipofilt, Fc, polo);
                calcfilt = true;
            }
            boFiltro.BackColor = Color.Orange;
            panelFiltro.BackColor = Color.LightGreen;
            panel2.Visible = false;

            return;
        }
        /// <summary>
        /// Llama al filtro en el panelcoda.
        /// </summary>
        /// <param name="iid">id de la estación que se esta clasificando.</param>
        void CalculoFiltroCoda(int iid)
        {// rutina que hace llamado al filtro del panel de coda.
            string ca = "";

            //panel2.Visible = true;
            ca = "OCTAVE... Calculando Filtro";
            util.Mensaje(panelcoda, ca, false);
            filtcod = true;
            if (calcfiltcod == false)
            {
                FiltroOctave(iid, tipofiltcod, Fccod, polocod);
                calcfiltcod = true;
            }
            bofilcod.BackColor = Color.Orange;
            panelfilcod.BackColor = Color.LightGreen;
            //panel2.Visible = false;

            return;
        }
        /// <summary>
        /// Llama a la rutina de filtro del panel principal.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void boFiltro_MouseDown(object sender, MouseEventArgs e)
        {
            // boton que llama a la rutina de filtro del panel principal.
            if (filt == false)
            {
                CalculoFiltro(id);
            }
            else
            {
                filt = false;
                boFiltro.BackColor = Color.White;
                panelFiltro.BackColor = Color.White;
            }
            panel1.Invalidate();

            return;
        }
        /// <summary>
        /// Rutina que adecua y hace llamado al Octave pra filtrar la señal.
        /// </summary>
        /// <param name="iid">id de la estación donde se va a calcular el filtro.</param>
        /// <param name="tipof">tipo de filtro a cálcular.</param>
        /// <param name="ffc">Frecuencia de corte que se usará para el filtro.</param>
        /// <param name="pol">Polo del filtro cálculado.</param>
        void FiltroOctave(int iid, bool tipof, double ffc, int pol)
        {
            int i, ii, j, k, lar;
            double wc, dd;
            string nom = "", li = "";

            wc = ffc / (ra[iid] / 2.0);
            if (File.Exists(".\\oct\\filtro.txt")) File.Delete(".\\oct\\filtro.txt");
            lar = cu[iid].Length;
            cf = new int[lar];

            if (File.Exists("res.txt")) File.Delete("res.txt");
            nom = est[iid].Substring(0, 4);
            StreamWriter wr = File.AppendText(".\\oct\\filtro.txt");
            StreamWriter da = File.CreateText(nom);

            for (ii = 0; ii < lar; ii++)
            {
                da.WriteLine(cu[iid][ii]);
            }
            wr.WriteLine("Z=load " + est[iid].Substring(0, 4) + ";");
            wr.WriteLine("n=" + pol.ToString() + ";");
            wr.WriteLine("wc=" + string.Format("{0:0.000}", wc));
            if (tipof == true) wr.WriteLine("[b,a]=butter_filtro(n,wc,'high');");
            else wr.WriteLine("[b,a]=butter_filtro(n,wc);");
            wr.WriteLine("yyyy=filter(b,a,Z);");
            wr.WriteLine("save res.txt yyyy");
            da.Close();
            wr.Close();
            //MessageBox.Show("fin escritura");

            li = "/C c:\\octave\\bin\\octave.exe < .\\oct\\filtro.txt";
            util.Dos(li, true);

            try
            {
                File.Delete(nom);
            }
            catch { }
            //MessageBox.Show("fin octave");

            if (File.Exists("res.txt"))
            {
                j = 0;
                StreamReader ar = new StreamReader("res.txt");
                li = "";
                for (k = 0; k < 5; k++) li = ar.ReadLine();
                while (li != null)
                {
                    try
                    {
                        li = ar.ReadLine();
                        if (li == null) break;
                        dd = double.Parse(li);
                        cf[j++] = (int)(dd + 0.5);
                    }
                    catch
                    {
                    }
                }
                ar.Close();
            }
            else
            {
                for (i = 0; i < lar; i++) cf[i] = 0;
                NoMostrar = true;
                MessageBox.Show("cero!!");
            }

            return;
        }
        /// <summary>
        /// Escoge el pasa bajo para el filtro del panel principal (panel1).
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void radioLow_CheckedChanged(object sender, EventArgs e)
        {// se escoge el pasa bajo para el filtro del panel principal
            tipofilt = false;
            if (filt == true)
            {
                calcfilt = false;
                CalculoFiltro(id);
                //panel1.Invalidate();
            }
        }
        /// <summary>
        /// Escoge el pasa alto para el filtro del panel principal (panel1).
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void radioHigh_CheckedChanged(object sender, EventArgs e)
        {// se escoge el pasa alto para el filtro del panel principal
            tipofilt = true;
            if (filt == true)
            {
                calcfilt = false;
                CalculoFiltro(id);
                //panel1.Invalidate();
            }
        }
        /// <summary>
        /// Hace el llamado a la rutina del filtro del panel de coda.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void bofilcod_MouseDown(object sender, MouseEventArgs e)
        {// boton que hace llamado a la rutina del filtro del panel de coda.
            if (filtcod == false)
            {
                CalculoFiltroCoda(nucod);
            }
            else
            {
                filtcod = false;
                calcfiltcod = false;
                bofilcod.BackColor = Color.White;
                bohzcod.BackColor = Color.White;
                bopolcod.BackColor = Color.White;
                radlowcod.BackColor = Color.White;
                radhicod.BackColor = Color.White;
            }
            panelcoda.Invalidate();

            return;
        }
        /// <summary>
        /// Escoge la frecuencia de corte del filtro del panel de coda.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void bohzcod_MouseDown(object sender, MouseEventArgs e)
        {
            // escoge la frecuencia de corte del filtro del panel de coda.
            calcfiltcod = false;
            if (e.Button == MouseButtons.Left) Fccod += 0.5;
            else Fccod -= 0.5;
            if (Fccod <= 0) Fccod = 0.5;
            else if (Fccod >= ra[nucod] / 2.0) Fccod = ra[nucod] / 2.0 - 0.5;
            bohzcod.Text = string.Format("{0:0.0Hz}", Fccod);
            if (filtcod == true) 
                CalculoFiltroCoda(nucod);
            panelcoda.Invalidate();
        }
        /// <summary>
        /// Escoge los polos del filtro del panel de coda.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void bopolcod_MouseDown(object sender, MouseEventArgs e)
        { // escoge los polos del filtro del panel de coda.
            calcfiltcod = false;
            if (e.Button == MouseButtons.Left) polocod += 2;
            else polocod -= 2;
            if (polocod < 2) polocod = 2;
            else if (polocod > 8) polocod = 8;
            bopolcod.Text = string.Format("{0:0}", polocod);
            if (filtcod == true) 
                CalculoFiltroCoda(nucod);
            panelcoda.Invalidate();
        }
        /// <summary>
        /// Escoge el pasa bajo para el filtro del panel de coda.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void radlowcod_CheckedChanged(object sender, EventArgs e)
        {// escoge el pasa bajo para el filtro del panel de coda
            tipofiltcod = false;
            if (filtcod == true)
            {
                calcfiltcod = false;
                CalculoFiltroCoda(nucod);
            }
            panelcoda.Invalidate();
        }
        /// <summary>
        /// Escoge el pasa alto para el filtro del panel de coda.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void radhicod_CheckedChanged(object sender, EventArgs e)
        {// escoge el pasa alto para el filtro del panel de coda
            tipofiltcod = true;
            if (filtcod == true)
            {
                calcfiltcod = false;
                CalculoFiltroCoda(nucod);
            }
            panelcoda.Invalidate();
        }
        /// <summary>
        /// Rutina que pretende ahorrar tiempo cuando se presenta un enjambre de sismos del mismo tipo.
        /// Al arrastrar en el panel principal, se hace llamado directamente al panel de coda para las
        /// lecturas de P, coda, etc.
        /// </summary>
        /// <param name="e">El evento MouseEventArgs que se lanzó.</param>
        void UnaClasificacion(MouseEventArgs e)
        {
            int ii, xx, yy, yf;
            bool derecho;

            if (e == null)
            {
                derecho = false;
            }
            else
            {
                if (e.Button == MouseButtons.Left) derecho = false;
                else derecho = true;
            }

            if (derecho == false)
            {
                if (panelbotoncla.Visible == false)
                {
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
                    yf = panel1.Size.Height - (yy + 25);
                    panelestaclauna.Size = new Size(xx, yy);
                    panelestaclauna.Location = new Point(150, yf);
                    panelestaclauna.Visible = true; // panel que muestra las estaciones, para que el usuario escoja o quite las que desee
                    yy = (totvolestaloc) * 18 + 1;
                    yf = panel1.Size.Height - (yy + 25);
                    panelvolclauna.Size = new Size(25, yy);
                    panelvolclauna.Location = new Point(90, yf);
                    panelvolclauna.Visible = true; // panel que muestra los volcanes para ecoger solo uno.
                    panelbotoncla.Visible = true; // panel con las clasificaciones para que el usuario escoja solo una.
                    panelestaclauna.Invalidate();
                    panelvolclauna.Invalidate();
                    DibujoCla();
                }
                else
                {
                    panelbotoncla.Visible = false;
                    panelestaclauna.Visible = false;
                    panelvolclauna.Visible = false;
                }
            }
            else
            {
                clSola = -1;
                boUnaCla.BackColor = Color.White;
                boUnaCla.Text = "1 Cla";
            }
        }
        /// <summary>
        /// Lanza a el método UnaClasificacion(e).
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void boUnaCla_MouseDown(object sender, MouseEventArgs e)
        {
            UnaClasificacion(e);
        }
        /// <summary>
        /// Lanza a el método EscribePanelestaClaUna que escribe los nombres de las estaciones en el panel de Una Clasificacion.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void panelestaclauna_Paint(object sender, PaintEventArgs e)
        {
            EscribePanelestaClaUna();// escribe los nombres de las estaciones en el panel de Una Clasificacion.
        }
        /// <summary>
        /// Lanza a el método EscribePanelvolClaUna() que escribe los volcanes en el panel de Una Clasificacion.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void panelvolclauna_Paint(object sender, PaintEventArgs e)
        {
            EscribePanelvolClaUna();// escribe los volcanes en el panel de Una Clasificacion.
        }
        /// <summary>
        /// Escribe los nombres de las estaciones en el panel de Una Clasificacion.
        /// </summary>
        void EscribePanelestaClaUna()
        {
            int i, j;

            Graphics dc = panelestaclauna.CreateGraphics();
            Pen lapiz = new Pen(Color.Black, 1);
            SolidBrush brocha = new SolidBrush(Color.Black);
            SolidBrush bro = new SolidBrush(Color.Orange);
            for (i = 0; i < nutra; i++)
            {
                j = 1 + i * 10;
                if (siEst[i] == true) dc.DrawString(est[i].Substring(0, 4), new Font("Times New Roman", 9), brocha, 1, j);
                else dc.DrawString(est[i].Substring(0, 4), new Font("Times New Roman", 9), bro, 1, j);
            }
            brocha.Dispose();
            bro.Dispose();
            lapiz.Dispose();

            return;
        }
        /// <summary>
        /// Escribe los volcanes en el panel de Una Clasificacion.
        /// </summary>
        void EscribePanelvolClaUna()
        {
            int i, j;

            Graphics dc2 = panelvolclauna.CreateGraphics();
            Pen lapiz2 = new Pen(Color.Black, 1);
            SolidBrush brocha2 = new SolidBrush(Color.Black);
            for (i = 0; i < totvolestaloc; i++)
            {
                j = 1 + i * 18;
                dc2.DrawString((i + 1).ToString(), new Font("Times New Roman", 9), brocha2, 1, j);
            }
            brocha2.Dispose();
            lapiz2.Dispose();

            return;
        }
        /// <summary>
        /// Panel con el nombre de todas las estaciones y donde el usuario puede escoger o quitar 
        /// estaciones a voluntad, cuyas trazas quedaran o no grabadas en la Base.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void panelestaclauna_MouseDown(object sender, MouseEventArgs e)
        {
            int nu;
            nu = e.Y / 10;
            if (siEst[nu] == false) siEst[nu] = true;
            else siEst[nu] = false;
            panelestaclauna.Invalidate();
            MessageBox.Show("yujuyyyyyyyyyyy");
        }
        /// <summary>
        /// Escoge un volcan en el panel de Una Clasificacion.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void panelvolclauna_MouseDown(object sender, MouseEventArgs e)
        {
            int i, k, j, nu;

            nu = e.Y / 18;
            for (i = 0; i < nutra; i++) siEst[i] = false;
            for (i = 0; i < totestaloc; i++)
            {
                for (j = 0; j < nutra; j++)
                {
                    if (string.Compare(estaloc[i].Substring(0, 4), est[j].Substring(0, 4)) == 0)
                    {
                        if (nuestaloc[i] == nu + 1) siEst[j] = true;
                        break;
                    }
                }
            }
            for (k = 0; k < totvolestaloc; k++)
            {
                for (i = 0; i < nuvol; i++)
                {
                    if (string.Compare(volestaloc[nu].Substring(0, 4), volcan[i].Substring(0, 4)) == 0)
                    {
                        vol = (short)(i);
                        break;
                    }
                }
            }
            panelestaclauna.Invalidate();
            panelvolclauna.Invalidate();

            return;
        }
        /// <summary>
        /// Escribe las clasificaciones en el panel de UnaClasificacion.
        /// </summary>
        void DibujoCla()
        {
            int i, j, j2, xf, yf, fx;

            xf = panelbotoncla.Size.Width;
            yf = panelbotoncla.Size.Height;
            fx = (int)(xf / (double)(nucla));

            j2 = 0;
            Graphics dc = panelbotoncla.CreateGraphics();
            Pen lapiz = new Pen(Color.Black, 1);
            SolidBrush brocha = new SolidBrush(Color.Black);
            SolidBrush bro = new SolidBrush(Color.Orange);
            SolidBrush bro2 = new SolidBrush(Color.LightGray);

            for (i = 0; i < nucla; i++)
            {
                j = fx + fx * i;
                if (clSola > -1 && i == clSola) dc.FillRectangle(bro, 1 + j2, 1, j - j2, yf);
                else dc.FillRectangle(bro2, 1 + j2, 1, j - j2, yf);
                dc.DrawLine(lapiz, j, 1, j, yf);
                dc.DrawString(cl[i].Substring(0, 2), new Font("Times New Roman", 9), brocha, 1 + i * fx, 1);
                j2 = j;
            }

            lapiz.Dispose();
            brocha.Dispose();
            bro.Dispose();
            bro2.Dispose();

            return;
        }
        /// <summary>
        /// Escoge la clasificación deseada por el usuario a la hora de clasificar un sismo.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void panelbotoncla_MouseDown(object sender, MouseEventArgs e)
        {// Escoge la clasificación deseada por el usuario.
            int i, xf, fx;

            xf = panelbotoncla.Size.Width;
            fx = (int)(xf / (double)(nucla));

            if (e.Button == MouseButtons.Left)
            {
                i = (int)(e.X / (double)(fx));
                if (i == clSola)
                {
                    clSola = -1;
                    if (tremor == false)
                    {
                        boUnaCla.BackColor = Color.White;
                        boUnaCla.Text = "1 Cla";
                    }
                }
                else
                {
                    clSola = i;
                    boClaSola.Text = cl[clSola].Substring(0, 2);
                    //boClaSola.Visible = true;
                    if (tremor == false)
                    {
                        boUnaCla.BackColor = Color.Orange;
                        boUnaCla.Text = "1 " + cl[clSola].Substring(0, 2);
                    }
                }
                DibujoCla();
                //MessageBox.Show("i=" + i.ToString());
            }
            else
            {
                clSola = -1;
                //boClaSola.Visible = false;
                if (tremor == false)
                {
                    boUnaCla.BackColor = Color.White;
                    boUnaCla.Text = "1 Cla";
                }
            }
            // MessageBox.Show("totestaloc=" + totestaloc.ToString()+" totvol"+totvolestaloc.ToString());
            panelbotoncla.Visible = false;
            panelestaclauna.Visible = false;
            panelvolclauna.Visible = false;

            return;
        }
        /// <summary>
        /// Botón que aparece en la parte inferior derecha del panel de Amplitud y al activarlo,
        /// graba los datos de lecturas en la Base.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void boClaSola_Click(object sender, EventArgs e)
        {
            int i, ii, j;
            long ll;
            double dd;  // provisional
            string ca = "", pa = "", fe = "";

            clas = cl[clSola];
            i = GrabaBase();

            if (tremor == true)// variable que permite la lectura continua de varios archivos, como si fuera una sola lectura.
            {
                if (i == 0) return;
                contremor += 1;
                ca = "Tremor " + contremor.ToString();
                boTremor.Text = ca;
                if (tremofin == false)
                {
                    dd = tinitremor + 2.0 * incTremor;
                    if (dd > timaxmin)
                    {
                        ll = (long)(Fei + tinitremor * 10000000.0);
                        DateTime fech = new DateTime(ll);
                        fe = string.Format("{0:yy}/{0:MM}/{0:dd} {0:HH}:{0:mm}", fech);
                        ii = listBox1.SelectedIndex;
                        do
                        {
                            pa = listBox1.Items[ii].ToString().Substring(0, 14);
                            j = string.Compare(pa, fe);
                            if (j >= 0) break;
                            ii += 1;
                        } while (ii < listBox1.Items.Count - 1);
                        seguir = true;
                        Seguir(true, ii);
                        seguir = false;
                    }
                    tinitremor += incTremor;
                }
                else
                {
                    contremor = 0;
                    tremor = false;
                    tremofin = false;
                    clSola = -1;
                    boFinTremor.BackColor = Color.White;
                    bofintrem.BackColor = Color.White;
                    bofintrem.Visible = false;
                    boFinTremor.Visible = false;
                    panelestaclauna.Visible = false;
                    panelvolclauna.Visible = false;
                    panelbotoncla.Visible = false;
                    panelAmp.Visible = false;
                    tinitremor = 0;
                    boTremor.BackColor = Color.White;
                    bovar.Visible = true;
                    boaste.Visible = true;
                    boClaSola.Visible = false;
                    if (panel2.Visible == true)
                    {
                        panel2.Size = new Size(219, 74);
                        panel2.Location = new Point(243, 254);
                        panel2.Visible = false;
                    }
                    ca = "Tremor";
                    boTremor.Text = ca;
                    return;
                }
                panelAmp.Visible = false;
                Cuadro_Tremor();
                return;
            }
            if (i == 1)
            {
                boHypo71.Visible = true;
                boAten.Visible = true;
            }
            panelcla.Visible = false;
            panelcoda.Visible = false;
            filtcod = false;
            calcfiltcod = false;
            bofilcod.BackColor = Color.White;
            bohzcod.BackColor = Color.White;
            bopolcod.BackColor = Color.White;
            radlowcod.BackColor = Color.White;
            radhicod.BackColor = Color.White;
            panelAmp.Visible = false;
            panel1.Invalidate();
            boClaSola.Visible = false;
            nuampvar = 0;
            Pti = 0;
            Ati = 0;
            Cti = 0;
            Sti = 0;
            bovar.Text = "varias";
            if (File.Exists("amplivarias.txt")) File.Delete("amplivarias.txt");
        }
        /// <summary>
        /// Regula el valor de la variable satur,con la cual se determina si se debe limitar o no el espacio de dibujo de las trazas.
        /// </summary>
        /// <param name="e">El evento que se lanzó.</param>
        void Saturacion(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (satu == false)
                {
                    satu = true;// variable que controla la saturacion en la visualizacion de la señal.
                    boSatu.BackColor = Color.DarkMagenta;
                    satur = 0;
                }
                else
                {
                    satu = false;
                    boSatu.BackColor = Color.White;
                }
            }
            else
            {
                satu = true;
                satur += 1;
                boSatu.BackColor = Color.DarkMagenta;
            }
            panel1.Invalidate();
            if (panelcoda.Visible == true)
                panelcoda.Invalidate();
        }
        /// <summary>
        /// Lanza el método Saturacion(MouseEventArgs e).
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void boSatu_MouseDown(object sender, MouseEventArgs e)
        {
            Saturacion(e);
        }
        /// <summary>
        /// Aumenta la amplitud en el panel de coda.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void boaumco_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                ampcod = (float)(1.5 * ampcod);
            else
                ampcod = (float)(0.6666 * ampcod);
            panelcoda.Invalidate();
        }
        /// <summary>
        /// Modifica el valor de la variable  ampcod haciendola igual a  1.0F.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void boinico_Click(object sender, EventArgs e)
        {
            ampcod = 1.0F;
            panelcoda.Invalidate();
        }
        /// <summary>
        /// Busca si la traza actual corresponde a una componente de una estación triaxial.
        /// Para que funcione, el nombre de las componentes debe terminar en Z, N y E.
        /// </summary>
        void BuscaCompCoda()
        {
            int i, cont;
            string nom;

            if (nucod < 0)
                return;
            cont = 0;
            nom = est[nucod].Substring(0, 4);
            bo1.Visible = false;
            bo2.Visible = false;
            bo3.Visible = false;
            bo1.BackColor = Color.White;
            bo2.BackColor = Color.White;
            bo3.BackColor = Color.White;
            bo1.Text = "    ";
            bo2.Text = "    ";
            bo3.Text = "    ";
            panelbotinfcoda.Visible = false;
            for (i = 0; i < nutra; i++)
            {
                if (string.Compare(nom.Substring(0, 3), est[i].Substring(0, 3)) == 0)
                {
                    if (est[i][3] == 'Z')
                    {
                        bo1.Text = est[i].Substring(0, 4);
                        bo1.Visible = true;
                        cont += 1;
                        if (cont >= 3) break;
                    }
                    else if (est[i][3] == 'N')
                    {
                        bo2.Text = est[i].Substring(0, 4);
                        bo2.Visible = true;
                        cont += 1;
                        if (cont >= 3) break;
                    }
                    else if (est[i][3] == 'E')
                    {
                        bo3.Text = est[i].Substring(0, 4);
                        bo3.Visible = true;
                        cont += 1;
                        if (cont >= 3) break;
                    }
                }
            }
            if (cont > 0)
                panelbotinfcoda.Visible = true;

            return;
        }
        // bo1, bo2 y bo3, corresponden a los botones de las componentes en el panel de coda.
        /// <summary>
        /// Este botón corresponde a la componente Z,
        /// este es visible en el panelcoda cuando se esta clasificando una traza que tenga la componente Z 
        /// y hace que se despliegue la traza de la componente Z de la estación que se esta clasificando.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void bo1_MouseDown(object sender, MouseEventArgs e)
        {
            short i;
            filtcod = false;
            calcfiltcod = false;
            bofilcod.BackColor = Color.White;
            bohzcod.BackColor = Color.White;
            bopolcod.BackColor = Color.White;
            radlowcod.BackColor = Color.White;
            radhicod.BackColor = Color.White;
            for (i = 0; i < nutra; i++)
            {
                if (string.Compare(bo1.Text.Substring(0, 4), est[i].Substring(0, 4)) == 0)
                {
                    Dibujocoda(cu[i], i);
                    bo1.BackColor = Color.Brown;
                    bo2.BackColor = Color.White;
                    bo3.BackColor = Color.White;
                    break;
                }
            }
            return;
        }
        /// <summary>
        /// Este botón corresponde a la componente N,
        /// este es visible en el panelcoda cuando se esta clasificando una traza que tenga la componente N 
        /// y hace que se despliegue la traza de la componente N de la estación que se esta clasificando.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void bo2_MouseDown(object sender, MouseEventArgs e)
        {
            short i;
            filtcod = false;
            calcfiltcod = false;
            bofilcod.BackColor = Color.White;
            bohzcod.BackColor = Color.White;
            bopolcod.BackColor = Color.White;
            radlowcod.BackColor = Color.White;
            radhicod.BackColor = Color.White;
            for (i = 0; i < nutra; i++)
            {
                if (string.Compare(bo2.Text.Substring(0, 4), est[i].Substring(0, 4)) == 0)
                {
                    Dibujocoda(cu[i], i);
                    bo2.BackColor = Color.Brown;
                    bo1.BackColor = Color.White;
                    bo3.BackColor = Color.White;
                    break;
                }
            }
            return;
        }
        /// <summary>
        /// Este botón corresponde a la componente E,
        /// este es visible en el panelcoda cuando se esta clasificando una traza que tenga la componente E 
        /// y hace que se despliegue la traza de la componente E de la estación que se esta clasificando.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void bo3_MouseDown(object sender, MouseEventArgs e)
        {
            short i;
            filtcod = false;
            calcfiltcod = false;
            bofilcod.BackColor = Color.White;
            bohzcod.BackColor = Color.White;
            bopolcod.BackColor = Color.White;
            radlowcod.BackColor = Color.White;
            radhicod.BackColor = Color.White;
            for (i = 0; i < nutra; i++)
            {
                if (string.Compare(bo3.Text.Substring(0, 4), est[i].Substring(0, 4)) == 0)
                {
                    Dibujocoda(cu[i], i);
                    bo3.BackColor = Color.Brown;
                    bo2.BackColor = Color.White;
                    bo1.BackColor = Color.White;
                    break;
                }
            }
            return;
        }
        /// <summary>
        /// Permite borrar archivos clasificados.
        /// Para ello se debe haber activado el botón Archi y verse los cajones de los archivos (ver el manual).
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void boEliClas_MouseDown(object sender, MouseEventArgs e)
        {
            if (elimiclas == false)
            {
                boEliClas.BackColor = Color.DodgerBlue;
                elimiclas = true;
            }
            else
            {
                boEliClas.BackColor = Color.White;
                elimiclas = false;
            }
        }
        /// <summary>
        /// Permite en el panel de amplitud, escoger si la lectura es de referencia o no.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void boaste_Click(object sender, EventArgs e)
        {// permite en el panel de amplitud, escoger si la lectura es de referencia o no.
            if (refe == true)
                refe = false;
            else
                refe = true;
            BotRefe();

            return;
        }
        /// <summary>
        /// Colorea los botones que escogen entre referencia o no, para que el usuario sepa en que estado se encuentran.
        /// </summary>
        void BotRefe()
        {// colorea los botones que escogen entre referencia o no, para que el usuario sepa en que estado se encuentran.
            if (refe == true)
            {
                boref.BackColor = Color.Gold;
                boaste.BackColor = Color.Tomato;
            }
            else
            {
                boref.BackColor = Color.White;
                boaste.BackColor = Color.White;
            }
        }
        /// <summary>
        /// Rutina que permite la lectura de datos de un sismo, repartido en varios archivos.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void boTremor_MouseDown(object sender, MouseEventArgs e)
        {
            int ii, xx, yy, yf;
            string ca = "";
            diagtrem di = new diagtrem();

            if (e.Button == MouseButtons.Left)
            {
                clSola = -1;
                if (tremor == false)
                {
                    tremor = true;
                    bofintrem.Visible = true;

                    ii = (nutra - 1) / 50;
                    xx = 42 + ii * 35;
                    yy = (nutra + 1) * 10 + 1;
                    yf = panel1.Size.Height - (yy + 25);
                    panelestaclauna.Size = new Size(xx, yy);
                    panelestaclauna.Location = new Point(150, yf);
                    panelestaclauna.Visible = true;
                    yy = (totvolestaloc) * 18 + 1;
                    yf = panel1.Size.Height - (yy + 25);
                    panelvolclauna.Size = new Size(25, yy);
                    panelvolclauna.Location = new Point(90, yf);
                    if (fcnan[id] > 0)
                    {
                        //checkBoxDr.Visible = true;
                        //checkBoxDr.Location = new Point(250,panel1.Height-50);
                    }
                    panelvolclauna.Visible = true;
                    panelbotoncla.Visible = true;
                    panelestaclauna.Invalidate();
                    panelvolclauna.Invalidate();
                    DibujoCla();
                    boFinTremor.Visible = true;
                    if (bloTremor == 0)
                    {
                        panel2.Size = new Size(230, 25);
                        panel2.Location = new Point(0, 0);
                        panel2.Visible = true;
                        ca = "Pique el Inicio del TREMOR";
                        util.Mensaje(panel2, ca, true);
                    }
                    boTremor.BackColor = Color.DeepPink;
                }
                else
                {
                    tremor = false;
                    bofintrem.Visible = false;
                    panelestaclauna.Visible = false;
                    panelvolclauna.Visible = false;
                    panelbotoncla.Visible = false;
                    panelAmp.Visible = false;
                    tinitremor = 0;
                    boTremor.BackColor = Color.White;
                    bovar.Visible = true;
                    boaste.Visible = true;
                    boClaSola.Visible = false;
                    boFinTremor.Visible = false;
                    boFinTremor.BackColor = Color.White;
                    if (panel2.Visible == true)
                    {
                        panel2.Size = new Size(219, 74);
                        panel2.Location = new Point(243, 254);
                        panel2.Visible = false;
                    }
                }
                panel1.Invalidate();
            }
            else
            {
                di.durtrem = incTremor.ToString();
                if (di.ShowDialog() == DialogResult.OK)
                {
                    incTremor = double.Parse(di.durtrem);
                }
            }
            return;
        }
        /// <summary>
        /// Esconde el panel de amplitud.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void boXamp_Click(object sender, EventArgs e)
        {
            panelAmp.Visible = false;
            TrazasClas();
        }
        /// <summary>
        /// Rutina que solicita mediante un mensaje en pantalla que se le indique el final del tremor en la traza, y la amplitud.
        /// </summary>
        void FinalTremor()
        {// rutina que avisa al programa el final de la lectura del sismo repartido en varios archivos.
            string ca = "";
            if (tremofin == false)
            {
                tremofin = true;
                boFinTremor.BackColor = Color.Red;
                bofintrem.BackColor = Color.Red;
                panel2.Size = new Size(280, 25);
                panel2.Location = new Point(0, 0);
                panel2.Visible = true;
                ca = "Pique el FINAL del TREMOR y luego la Amplitud";
                util.Mensaje(panel2, ca, true);
            }
            else
            {
                tremofin = false;
                boFinTremor.BackColor = Color.White;
                bofintrem.BackColor = Color.White;
                panel2.Visible = false;
            }

        }
        // boFinTremor y bofintrem, hacen lo mismo. el primero esta en el panel de amplitud y el segundo en el panel principal.
        /// <summary>
        /// Lanza el método que indica el fin del tremor.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void boFinTremor_Click(object sender, EventArgs e)
        {
            FinalTremor();
        }
        /// <summary>
        /// Lanza el método que indica el fin del tremor.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void bofintrem_Click(object sender, EventArgs e)
        {
            FinalTremor();
        }
        /// <summary>
        /// Permite la lectura de huecos.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void boHueco_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (sihueco == false)
                {
                    sihueco = true;
                    boHueco.BackColor = Color.BlueViolet;
                }
                else
                {
                    sihueco = false;
                    boHueco.BackColor = Color.White;
                    panelhueco.Visible = false;
                }
            }
            else
            {
                NoMostrar = true;
                DialogResult result = MessageBox.Show("Realmente Quiere Borrar TODOS los HUECOS ??",
                                "BORRAR HUECOS???", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.No) return;
                huecolist.Clear();
                nuhueco = 0;
                panelcoda.Invalidate();
            }
        }
        /// <summary>
        /// Realiza el dibujo de los huecos leidos.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        void DibHueco()
        {
            int xf, yf, pro = 0, max, min, lar, kk, k, dif, tot = 0;
            int nmi = 0, nmf = 0;
            float x1, y1;
            double fax, fay, dura, diff, iniy = 0, fy = 0;
            Point[] dat;

            if (t1hu == t2hu) return;
            if (panelmarca.Visible == true) panelmarca.Visible = false;
            panelhueco.BringToFront();
            util.borra(panelhueco, Color.Lavender);
            xf = panelhueco.Size.Width - 10;
            yf = panelhueco.Size.Height;
            dura = t2hu - t1hu;
            fax = xf / dura;
            fay = 4.0 * yf / 5.0;

            Graphics dc = panelhueco.CreateGraphics();
            Pen lapiz = new Pen(Color.Black, 1);
            Pen lapiz2 = new Pen(Color.Orange, 1);
            SolidBrush brocha = new SolidBrush(colotr1);
            dc.DrawRectangle(lapiz, 0, 0, panelhueco.Size.Width - 3, panelhueco.Size.Height - 3);

            try
            {

                tot = tim[nucod].Length;
                for (k = 0; k < tot; k++)
                {
                    if (tim[nucod][k] >= t1hu) break;
                }
                nmi = k;
                for (k = nmi; k < tot; k++)
                {
                    if (tim[nucod][k] > t2hu || tim[nucod][k] <= 0) break;
                }
                nmf = k - 1;
                lar = nmf - nmi;
                max = cu[nucod][nmi];
                min = max;
                for (k = nmi + 1; k <= nmf; k++)
                {
                    if (max < cu[nucod][k]) max = cu[nucod][k];
                    else if (min > cu[nucod][k]) min = cu[nucod][k];
                }
                pro = (int)((max + min) / 2.0F);
                if (max - pro != 0) fy = ((fay / 2) / ((max - pro)));
                else fy = 1;
                pro = (int)(pro * ampamp);
                iniy = panelhueco.Size.Height / 2;
                dat = new Point[lar];
                kk = 0;
                for (k = nmi; k <= nmf; k++)
                {
                    if (kk >= lar) break;
                    dif = (int)(cu[nucod][k] * ampamp) - pro;
                    diff = dif * fy;
                    y1 = (float)(iniy - diff);
                    x1 = (float)(10.0 + (tim[nucod][k] - t1hu) * fax);
                    dat[kk].Y = (int)y1;
                    dat[kk].X = (int)x1;
                    kk += 1;
                }
                dc.DrawLines(lapiz, dat);
            }
            catch
            {
            }

            lapiz.Dispose();
            lapiz2.Dispose();

            brocha.Dispose();

            DibujoCodaHueco();

            return;
        }
        /// <summary>
        /// Señala al usuario el sector del hueco en la señal del panel de coda.
        /// </summary>
        void DibujoCodaHueco()
        {
            int xf, yf, i, k, kk, lar, max, min, pro, dif = 0, nm1, nm2, tot;
            int[] nmi = new int[3];
            int[] nmf = new int[3];
            float x1, y1;
            double dura, fax, fay, fy, diff, iniy;
            Point[] dat;


            if (panelhueco.Visible == false) return;

            //MessageBox.Show("inicio");
            xf = panelcoda.Size.Width - 70;
            yf = panelcoda.Size.Height - 30;
            dura = t2cod - t1cod;
            fax = xf / dura;
            fay = 4.0 * yf / 5.0;

            tot = tim[nucod].Length;
            for (k = 0; k < tot; k++)
            {
                if (tim[nucod][k] >= t1cod) break;
            }
            nm1 = k;
            for (k = nm1; k < tot; k++)
            {
                if (tim[nucod][k] > t2cod || tim[nucod][k] <= 0) break;
            }
            nm2 = k - 1;
            max = cu[nucod][nm1];
            min = max;
            for (k = nm1 + 1; k < nm2; k++)
            {
                if (max < cu[nucod][k]) max = cu[nucod][k];
                else if (min > cu[nucod][k]) min = cu[nucod][k];
            }

            nmi[0] = nm1;
            nmf[2] = nm2;
            for (k = 0; k < tot; k++)
            {
                if (tim[nucod][k] >= t1hu) break;
            }
            nmi[1] = k;
            nmf[0] = k;
            for (k = 0; k < tot; k++)
            {
                if (tim[nucod][k] >= t2hu) break;
            }
            nmi[2] = k;
            nmf[1] = k;

            Graphics dc = panelcoda.CreateGraphics();
            Pen lapiz;

            try
            {
                for (i = 0; i < 3; i++)
                {
                    if (i != 1) lapiz = new Pen(colinea, 1);
                    else lapiz = new Pen(Color.BlueViolet, 1);
                    lar = nmf[i] - nmi[i];
                    pro = (int)((max + min) / 2.0);
                    if (max - pro != 0) fy = ((fay / 2) / ((max - pro)));
                    else fy = 1;
                    pro = (int)(pro * ampcod);
                    iniy = panelcoda.Size.Height / 2.0;
                    dat = new Point[lar];
                    kk = 0;
                    for (k = nmi[i]; k < nmf[i]; k++)
                    {
                        if (kk >= lar) break;
                        dif = (int)(cu[nucod][k] * ampcod) - pro;
                        diff = dif * fy;
                        y1 = (float)(iniy - diff);
                        x1 = (float)(40.0 + (tim[nucod][k] - t1cod) * fax);
                        dat[kk].Y = (int)y1;
                        dat[kk].X = (int)x1;
                        kk += 1;
                    }
                    dc.DrawLines(lapiz, dat);
                    lapiz.Dispose();
                }
            }
            catch
            {
            }

            return;
        }
        /// <summary>
        /// Redibuja el hueco.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void panelhueco_Paint(object sender, PaintEventArgs e)
        {// redibuja el hueco.
            DibHueco();
        }
        /// <summary>
        /// Desplaza la visualización del hueco hacia la izquierda.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void button1_Click(object sender, EventArgs e)
        {// izquierda hueco
            double tii1, tii2, fac;

            fac = (t2hu - t1hu) / 3.0;
            tii1 = t1hu + fac;
            tii2 = t2hu + fac;
            if (tii2 > t2cod) return;
            t1hu = tii1;
            t2hu = tii2;
            panelhueco.Invalidate();
            return;

        }
        /// <summary>
        /// Desplaza la visualización del hueco hacia la derecha.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void button2_Click(object sender, EventArgs e)
        {// derecha hueco
            double tii1, tii2, fac;

            fac = (t2hu - t1hu) / 3.0;
            tii1 = t1hu - fac;
            tii2 = t2hu - fac;
            if (tii1 < t1cod) return;
            t1hu = tii1;
            t2hu = tii2;
            panelhueco.Invalidate();
            return;

        }
        /// <summary>
        /// Ampliar hueco.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void button3_Click(object sender, EventArgs e)
        {// ampliar hueco
            double tii, fac;

            fac = (t2hu - t1hu) / 3.0;
            tii = t2hu - fac;
            if (tii - t1hu < 0.1) return;
            t2hu = tii;
            panelhueco.Invalidate();
            return;

        }
        /// <summary>
        /// Sirve para reducir el tamaño del hueco.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void button4_Click(object sender, EventArgs e)
        {// encoger hueco
            double tii, fac;

            fac = (t2hu - t1hu) / 3.0;
            tii = t2hu + fac;
            if (tii >= t2cod) return;
            t2hu = tii;
            panelhueco.Invalidate();
            return;
        }
        /// <summary>
        /// Inicio de la lectura del hueco en el panel de huecos
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void panelhueco_MouseDown(object sender, MouseEventArgs e)
        {// inicio de la lectura del hueco en el panel de huecos.
            bhxi = e.X;
        }
        /// <summary>
        /// Selanza cuando se termina la lectura del hueco, determina el valor final del hueco,
        /// y llama al método que dibuja la lista de huecos.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void panelhueco_MouseUp(object sender, MouseEventArgs e)
        {// final de la lectura del hueco en el panel de huecos.
            int bhxf, xx, xf;
            double fax;
            double ti1, ti2, tii;
            string ca = "";

            bhxf = e.X;
            xx = (int)(Math.Abs((double)(bhxf) - bhxi));
            if (xx < 3) return;
            if (bhxf < bhxi)
            {
                xx = bhxi;
                bhxi = bhxf;
                bhxf = xx;
            }

            xf = panelhueco.Size.Width - 10;
            fax = (t2hu - t1hu) / xf;

            ti1 = t1hu + ((bhxi - 10.0) * fax);
            ti2 = t1hu + ((bhxf - 10.0) * fax);

            if (ti1 > ti2)
            {
                tii = ti1;
                ti1 = ti2;
                ti2 = tii;
            }

            ca = string.Format("{0,14:0.0000} {1,14:0.0000}", ti1, ti2);
            huecolist.Add(ca);
            nuhueco = huecolist.Count;
            //MessageBox.Show("ca="+ca+" nuhueco="+nuhueco.ToString());

            panelhueco.Invalidate();
            DibujarHuecosCoda();

            return;
        }
        /// <summary>
        /// Señala al usuario graficamente donde estan los huecos leidos en el panel de coda.
        /// </summary>
        void DibujarHuecosCoda()
        {
            int i, xf, yf;
            float x1, x2;
            double fax, fay, dura, ti1, ti2;
            char[] delim = { ' ', '\t' };
            string[] pa = null;


            if (nuhueco == 0) return;

            xf = panelcoda.Size.Width - 70;
            yf = panelcoda.Size.Height - 30;
            dura = t2cod - t1cod;
            fax = xf / dura;
            fay = 4.0 * yf / 5.0;

            Graphics dc = panelcoda.CreateGraphics();
            //Pen lapiz = new Pen(colinea, 1);
            SolidBrush brocha = new SolidBrush(Color.PaleVioletRed);

            for (i = 0; i < nuhueco; i++)
            {
                pa = huecolist[i].ToString().Split(delim);
                ti1 = double.Parse(pa[0]);
                ti2 = double.Parse(pa[1]);
                x1 = (float)(40.0 + (ti1 - t1cod) * fax);
                x2 = (float)(40.0 + (ti2 - t1cod) * fax);
                dc.FillRectangle(brocha, x1, 10, x2 - x1, yf);
                //MessageBox.Show("ti1="+ti1.ToString()+" ti2="+ti2.ToString());
            }
            brocha.Dispose();

            return;
        }
        /// <summary>
        /// Se revisa que la estación tenga factor de conversion de cuentas a milimetros en los analogicos.
        /// </summary>
        void ChequeoFactormm()
        {
            int i, fe1, fe2, fe;
            long ll;
            char[] delim = { ' ', '\t' };
            string[] pa = null;
            string esta, ca;

            factmm = -1.0;
            ll = (long)(Fei + t1cod * 10000000.0); // se convierte el tiempo inicial de la ventana de coda en SUDS a tiempo en visual c#
            DateTime fech = new DateTime(ll);
            fe = int.Parse(string.Format("{0:yyyy}{0:MM}{0:dd}", fech));
            ca = tar[nucod].ToString();
            for (i = 0; i < fcmm.Count; i++)
            {
                try
                {
                    if (ca.Substring(0, 1) == fcmm[i].ToString().Substring(0, 1))
                    {
                        esta = fcmm[i].ToString().Substring(2, 4);
                        if (string.Compare(esta.Substring(0, 4), est[nucod].Substring(0, 4)) == 0)
                        {
                            pa = fcmm[i].ToString().Split(delim);
                            fe1 = int.Parse(pa[3]);
                            fe2 = int.Parse(pa[4]);
                            if (fe >= fe1 && (fe <= fe2 || fe2 == 0))
                            {
                                factmm = double.Parse(pa[2]);
                                break;
                            }
                        }
                    }
                }
                catch
                {
                    factmm = -1.0;
                }
            }

            return;
        }
        /// <summary>
        /// Rutina que grafica siempre con respecto al numero de cuentas
        /// que se indica en el archivo inicio.txt, simulando asi un registro con la misma ganancia.
        /// </summary>
        void SimulacionAnalogico()
        {
            if (analogico == false)
            {
                analogico = true;
                boAnalogico.BackColor = Color.OrangeRed;
                boSatu.BackColor = Color.DarkMagenta;
                boNano.Visible = false;
            }
            else
            {
                analogico = false;
                boAnalogico.BackColor = Color.White;
                boSatu.BackColor = Color.White;
                boNano.Visible = true;
                boNano.Text = "Tra";
            }
            satu = analogico;
            if (panelcladib.Visible == true)
                DibujoTrazas();
            if (panelcoda.Visible == true)
                panelcoda.Invalidate();
            panel1.Invalidate();

            return;
        }
        /// <summary>
        /// Lanza el método SimulacionAnalogico.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void boAnalogico_Click(object sender, EventArgs e)
        {
            SimulacionAnalogico();
        }
        /// <summary>
        /// Cambia el valor de verdad de la variable analogcoda que controla si se dibuja o no de 
        /// forma analogíca en el panelcoda.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void boAnaloCoda_Click(object sender, EventArgs e)
        {// analogico solo para el panel de coda.
            if (analogcoda == false)
            {
                analogcoda = true;
                boAnaloCoda.BackColor = Color.Orange;
            }
            else
            {
                analogcoda = false;
                boAnaloCoda.BackColor = Color.Moccasin;
            }
            panelcoda.Invalidate();
        }
        /// <summary>
        /// Busca el valor cero de la señal.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void boPromCoda_Click(object sender, EventArgs e)
        {// busca el valor 'cero' de la señal
            if (sihueco == true) return;
            if (promecod == false)
            {
                promecod = true;
                boPromCoda.BackColor = Color.DarkGoldenrod;
            }
            else
            {
                promecod = false;
                boPromCoda.BackColor = Color.PaleGoldenrod;
            }
            panelcoda.Invalidate();
            return;
        }
        /// <summary>
        ///  Botón en el panel de clasificación, que permite graficar con respecto a la maxima cuenta pico a pico,
        ///  micrometros pico a pico, o bien con respecto a la máxima amplitud de cada traza.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void boNano_Click(object sender, EventArgs e)
        {
            if (boNano.Text == "Tra")
            {
                boNano.Text = "Cue";
                boNano.BackColor = Color.Orange;
            }
            else if (boNano.Text == "Cue")
            {
                boNano.Text = "Nan";
                boNano.BackColor = Color.Green;
                // boNano.Font = new Font("Microsoft Sans Serif",7);
            }
            else if (boNano.Text == "Nan")
            {
                boNano.Text = "Tra";
                boNano.BackColor = Color.Lavender;
            }
            DibujoTrazas();
            return;
        }
        /// <summary>
        /// Busca aquellas amplitudes que sobre pasan el umbral indicado en el archivo inicio.txt.
        /// </summary>
        void CalculoUmbral()
        {
            if (numbral <= 0)
                return;

            Umbral();

            return;
        }
        /// <summary>
        /// Llama a la rutina CalculoUmbral().
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void boUmbral_Click(object sender, EventArgs e)
        {
            CalculoUmbral();
        }
        /// <summary>
        /// Indica graficamente al usuario aquellas amplitudes que sobrepasan cierto valor.
        /// </summary>
        void Umbral()
        {// indica al usuario aquellas amplitudes que sobrepasa cierto valor.
            int i, iu, j, k, kk, kkk, mxx, fin;
            int tota, xf, yf, denom, x1, y1;
            float fay, fax;

            iu = -1;
            for (i = 0; i < numbral; i++)
            {
                if (est[id].Substring(0, 4) == estumbral[i].Substring(0, 4))
                {
                    iu = i;
                    break;
                }
            }
            if (iu == -1)
            {
                boUmbral.Visible = false;
                return;
            }
            //MessageBox.Show("iu="+iu.ToString()+" "+estumbral[iu]+" "+valumbral[iu].ToString());

            kk = (int)(ra[id]);
            fin = tim[id].Length - kk;
            if (fin < 0) return;

            tota = tim[id].Length;
            if (tim[id][tota - 1] <= 0)
            {
                do
                {
                    tota -= 1;
                    if (tim[id][tota - 1] > 0) break;
                } while (tota > 0);
            }
            if (tota < 2) return;
            denom = (int)(Math.Abs(Math.Ceiling((tim[id][tota - 1] - timin) / dur)));
            if (denom <= 0) denom = 1;
            for (k = 0; k < tota; k++) if (tim[id][k] < timin) break;

            xf = panel1.Size.Width;
            yf = panel1.Size.Height;

            if (esp == 0)
                fay = (yf - 45.0F) / denom;
            else
                fay = esp;
            fax = xf / dur;

            Graphics dc = panel1.CreateGraphics();
            Pen lapiz = new Pen(Color.Red, 1);

            for (i = 1; i < fin; i++)
            {
                mxx = Math.Abs(cu[id][i] - cu[id][i - 1]);
                for (j = 2; j < kk; j++)
                {
                    if (mxx < Math.Abs(cu[id][i + j] - cu[id][i - 1])) mxx = Math.Abs(cu[id][i + j] - cu[id][i - 1]);
                }
                if (mxx >= valumbral[iu])
                {
                    kkk = (int)(Math.Floor((tim[id][i] - timin) / dur));
                    x1 = (int)(((tim[id][i] - timin) - kkk * dur) * fax);
                    y1 = (int)(incy + 45.0 + kkk * fay + fay / 2.0);
                    dc.DrawLine(lapiz, x1, y1 - 10, x1, y1 + 10);
                }
            }
            lapiz.Dispose();
            //MessageBox.Show("fin");

            return;
        }
        /// <summary>
        /// Llama a la rutina que permite ver los arribos secuencialmente.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void boVista_MouseDown(object sender, MouseEventArgs e)
        {

            if (File.Exists(".\\oct\\maglocalVista.txt")) File.Delete(".\\oct\\maglocalVista.txt");
            if (moverparti == true || panelParti.Visible == true) return;
            if (vista == false)
            {
                vista = true;
                boVista.BackColor = Color.DarkRed;
            }
            else
            {
                vista = false;
                boVista.BackColor = Color.MistyRose;
            }
        }
        /// <summary>
        /// Rutina que permite ver los arribos secuencialmente, aunque debe calibrase para aquellos sismos de interés.
        /// Esta parte realiza la búsqueda de los primeros arribos en las estaciones.
        /// </summary>
        /// <param name="idcan">Indice de las estaciones en la vista.</param>
        /// <param name="tii">Representa el momento en punto en la traza donde se inicio el arrastre para la clasificación -1.</param>
        /// <param name="time">Tiempo para la visualización de arribos.</param>
        /// <param name="cond">Condición que indica si en el archivo /oct/estaML.txt hay datos de meridiano y longitud para las estaciones.</param>
        /// <param name="siML">Condición que indica si el nombre de la estación esta en los archivos /oct/estaML.txt y /pro/estavista.txt.</param>
        void VerTrazasArribos(int[] idcan, double tii, double[][] time, bool cond, bool[] siML)
        {
            int i, ii, j, k, kk, m, nucan, xf, yf, mmx, mmn, mni, mnf, lar;
            int pro, iniy, tot, nt;
            float x1, y1;
            double fax, fay, fy, tiS;
            double[] orden;
            bool[] ya;
            string eesta = "";
            bool si = false;
            Color col;
            Point[] dat;


            //for (i = 0; i < siML.Length; i++) MessageBox.Show("i=" + i.ToString() + " siML=" + siML[i].ToString());
            tii += initic;
            xf = splitContainer1.Panel1.Width - 40;
            yf = splitContainer1.Panel1.Height - 20;
            nucan = idcan.Length;
            orden = new double[nucan];
            ya = new bool[nucan];
            fax = xf / durac;         // factor en la horizontal
            fay = yf / (nucan + 0.5); // factor en la vertical vertical

            for (i = 0; i < nucan; i++)
            {
                orden[i] = time[i][0];
                ya[i] = false;
            }
            Array.Sort(orden);
            Graphics dc = splitContainer1.Panel1.CreateGraphics();
            SolidBrush bro = new SolidBrush(Color.White);
            dc.FillRectangle(bro, 0, 0, splitContainer1.Panel1.Width, splitContainer1.Panel1.Height);
            bro.Dispose();
            Pen lapiz = new Pen(Color.Black, 1);
            Pen lap = new Pen(Color.Red, 1);
            Pen lapS = new Pen(Color.Cyan, 3);

            idT1 = new short[nucan];
            nuesvista = new short[nucan];
            tiniT = tii;
            nt = 0;
            for (i = 0; i < nucan; i++)
            {
                tiS = 0;
                k = -1;
                idT1[i] = -1;
                si = false;
                for (ii = 0; ii < nucan; ii++)
                {
                    if (time[ii][0] == orden[i] && ya[ii] == false)
                    {
                        si = siML[ii];
                        k = idcan[ii];
                        nuesvista[i] = (short)(k);
                        eesta = est[k];
                        idT1[i] = (short)(ii);
                        ya[ii] = true;
                        if (time[ii][1] > 0) tiS = time[ii][1];
                        break;
                    }
                }
                if (k == -1) continue;
                lar = cu[k].Length - 1;
                mni = (int)((tii - tim[k][0]) * ra[k]);
                mnf = (int)(((tii + durac) - tim[k][0]) * ra[k]);
                if (mni < 0) mni = 0;
                if (mnf > lar) mnf = lar;
                mmx = cu[k][mni];
                mmn = mmx;
                for (j = mni + 1; j < mnf; j++)
                {
                    if (mmx < cu[k][j]) mmx = cu[k][j];
                    else if (mmn > cu[k][j]) mmn = cu[k][j];
                }
                pro = (int)((mmx + mmn) / 2.0F);
                if (mmx - pro != 0) fy = ((fay / 2.0) / ((mmx - pro)));
                else fy = 1.0;
                tot = mnf - mni;
                dat = new Point[tot];
                iniy = (int)(5.0 + i * fay + fay / 2.0);
                kk = 0;
                for (m = mni; m < mnf; m++)
                {
                    y1 = (float)(iniy - (cu[k][m] - pro) * fy);
                    x1 = (float)(40.0 + (tim[k][m] - tii) * fax);
                    dat[kk].Y = (int)y1;
                    dat[kk].X = (int)x1;
                    kk += 1;
                }
                dc.DrawLines(lapiz, dat);
                if (orden[i] > 0)
                {
                    x1 = (float)(40.0 + (orden[i] - tii) * fax);
                    y1 = (float)(iniy);
                    dc.DrawLine(lap, x1, y1 - 8, x1, y1 + 8);
                    if (tiS > 0)
                    {
                        x1 = (float)(40.0 + (tiS - tii) * fax);
                        y1 = (float)(iniy);
                        dc.DrawLine(lapS, x1, y1 - 10, x1, y1 + 10);
                    }
                    //dc.DrawString(eesta, new Font("Times New Roman", 10), brocha, 1, (int)(iniy - 5));
                }
                if (orden[i] > 0) col = Color.OrangeRed;
                else col = Color.Gainsboro;
                SolidBrush brocha = new SolidBrush(col);
                dc.DrawString(eesta, new Font("Times New Roman", 10), brocha, 1, (int)(iniy - 5));
                brocha.Dispose();
                if (si == true)
                {
                    Pen lapM = new Pen(Color.GreenYellow, 3);
                    dc.DrawRectangle(lapM, 0, (int)(iniy - 5), 40, 16);
                    lapM.Dispose();
                }
                nt += 1;
            }

            lapiz.Dispose();
            lap.Dispose();
            lapS.Dispose();

            return;
        }
        /// <summary>
        /// Grafica los primeros arribos en la modalidad de mapa.
        /// </summary>
        void VerVista()
        {
            int i, ii, j, jj, k, kk, ktope, nuu, nmi, nmf, mmx = 0, mmn = 0, final, val;
            long lll;
            double laa, loo, ticero, tifin, ms, facmap;
            double lax, lam, lox, lom, dd, dif, ff;
            int[] tope, idcan, umbral, nivelbase;
            double[] la, lo;
            bool[] sii, siML;
            string[] facML;
            string li = "";
            char[] delim = { ' ', '\t' };
            string[] pa = null;
            string ca = "", lin = "", ca2 = "", ca3 = "";
            string esp = "                                                                                                    ";
            Color col = Color.Red;
            bool siMLvista = false;
            ArrayList LisVis = new ArrayList();


            if (!File.Exists(".\\pro\\estavista.txt"))
            {
                vista = false;
                boVista.Visible = false;
                NoMostrar = true;
                MessageBox.Show("NO se encuentra el archivo .\\pro\\estavista.tx!!\nVER el Manual");
                return;
            }
            MLVista = false;
            MLsi = false;
            if (File.Exists(".\\oct\\estaML.txt"))
            {
                StreamReader pr = new StreamReader(".\\oct\\estaML.txt");
                while (li != null)
                {
                    try
                    {
                        li = pr.ReadLine();
                        if (li == null || li[0] == '*') break;
                        if (li.Length > 10) LisVis.Add(li);
                    }
                    catch
                    {
                    }
                }
                pr.Close();
                li = "";
                if (LisVis.Count > 0) siMLvista = true;
            }
            if (File.Exists(".\\oct\\maglocal.txt"))
                File.Delete(".\\oct\\maglocal.txt");

            if (copiarMod == false)
            {
                if (!Directory.Exists(".\\h")) Directory.CreateDirectory(".\\h");
                ca = rutbas + "\\h";
                DirectoryInfo dir = new DirectoryInfo(ca);
                FileInfo[] fcc = dir.GetFiles("?.mod");
                foreach (FileInfo f in fcc)
                {
                    ca2 = ca + "\\" + f.Name;
                    ca3 = ".\\h\\" + f.Name;
                    File.Copy(ca2, ca3, true);
                }
                /*lin = "/C copy " + rutbas + "\\h\\*.mod .\\h";
                util.Dos(lin, true);*/
                copiarMod = true;
            }
            if (File.Exists(".\\h\\r.inp")) File.Delete(".\\h\\r.inp");
            if (File.Exists(".\\h\\r.pun")) File.Delete(".\\h\\r.pun");
            if (!File.Exists(".\\h\\data.inp"))
            {
                StreamWriter da = File.CreateText(".\\h\\data.inp");
                da.WriteLine(".\\h\\r.inp");
                da.WriteLine(".\\h\\r.prt");
                da.WriteLine(".\\h\\r.pun");
                da.Close();
            }
            nuu = 0;
            StreamReader ar = new StreamReader(".\\pro\\estavista.txt");
            //li = ar.ReadLine();
            while (li != null)//determina el numero de estaciones en le archivo
            {
                try
                {
                    li = ar.ReadLine();
                    if (li == null || li[0] == '*') break;
                    if (li.Length >= 6)
                    {
                        for (j = 0; j < nutra; j++)
                        {
                            if (li.Substring(0, 4) == est[j].Substring(0, 4))
                            {
                                nuu += 1;
                                break;
                            }
                        }
                    }
                }
                catch
                {
                }
            }
            ar.Close();
            if (nuu == 0)
            {
                vista = false;
                boVista.Visible = false;
                return;
            }

            tope = new int[nuu];
            idcan = new int[nuu];
            umbral = new int[nuu];
            nivelbase = new int[nuu];
            la = new double[nuu];
            lo = new double[nuu];
            sii = new bool[nuu];
            siML = new bool[nuu];
            facML = new string[nuu];
            time = new double[nuu][];
            for (i = 0; i < nuu; i++)
            {
                time[i] = new double[2];
                sii[i] = false;
                siML[i] = false;
                facML[i] = "";
            }
            nuu = 0;
            li = "";
            StreamReader ar2 = new StreamReader(".\\pro\\estavista.txt");
            while (li != null)
            {
                try
                {
                    li = ar2.ReadLine();
                    if (li == null || li[0] == '*') break;
                    if (li.Length > 15)
                    {
                        for (j = 0; j < nutra; j++)
                        {
                            if (li.Substring(0, 4) == est[j].Substring(0, 4))
                            {
                                try
                                {
                                    pa = li.Split(delim);
                                    tope[nuu] = int.Parse(pa[1]);
                                    idcan[nuu] = j;
                                    la[nuu] = double.Parse(pa[2]);
                                    lo[nuu] = double.Parse(pa[3]);
                                    if (siMLvista == true)
                                    {
                                        for (kk = 0; kk < LisVis.Count; kk++)
                                        {
                                            pa = LisVis[kk].ToString().Split(delim);
                                            //val = int.Parse(pa[11]);
                                            //if (val != 0) continue;
                                            //MessageBox.Show("pa=" + pa[2].Substring(0,4)+"  li="+li.Substring(0, 4));
                                            if (pa[2].Substring(0, 4) == li.Substring(0, 4))
                                            {
                                                // val = int.Parse(pa[11]);
                                                // if (val != 0) continue;
                                                siML[nuu] = true;
                                                facML[nuu] = pa[8];
                                                // MessageBox.Show("siML="+siML[nuu].ToString()+" nuu="+nuu.ToString());
                                                break;
                                            }
                                        }
                                    }
                                    nuu += 1;
                                    break;
                                }
                                catch
                                {
                                }
                            }
                        }
                    }
                }
                catch
                {
                }
            }
            ar2.Close();

            for (i = 0; i < nuu; i++)
            {
                time[i][0] = 0;
                time[i][1] = 0;
                sii[i] = false;
                k = idcan[i];
                nmi = (int)((tie1 - tim[k][0]) * ra[k]);
                if (nmi < 0) nmi = 0;
                ktope = (int)(200 / ra[k]);//provi
                nmf = (int)(((tie1 + ktope) - tim[k][0]) * ra[k]);
                umbral[i] = 0;
                nivelbase[i] = 0;
                if ((nmf - nmi) > 10)
                {
                    mmx = cu[k][nmi];
                    mmn = mmx;
                    for (j = nmi + 1; j < nmf; j++)
                    {
                        if (mmx < cu[k][j]) mmx = cu[k][j];
                        else if (mmn > cu[k][j]) mmn = cu[k][j];
                    }
                    umbral[i] = (int)(Math.Abs(mmx - mmn));
                    nivelbase[i] = (int)((mmx + mmn) / 2.0);
                }
            }

            lax = la[0];
            lam = lax;
            lox = lo[0];
            lom = lox;
            for (i = 1; i < nuu; i++)
            {
                if (lax < la[i]) lax = la[i];
                else if (lam > la[i]) lam = la[i];
                if (lox < lo[i]) lox = lo[i];
                else if (lom > lo[i]) lom = lo[i];
            }
            dd = lax - lam;
            if (dd < (lox - lom)) dd = lox - lom;
            dif = dd + dd / 5.0;
            facmap = (splitContainer1.Panel2.Width / dif) * 0.5;
            laa = (lax + lam) / 2.0;
            loo = (lox + lom) / 2.0;
            latitud = (float)(laa);
            longitud = (float)(loo);
            famap = (float)(facmap);
            util.TopoMapaArribos(splitContainer1.Panel2, facmap, laa, loo, Maparr);
            util.EstacionesArribos(splitContainer1.Panel2, facmap, laa, loo, la, lo);

            ticero = 0;
            tifin = 0;
            for (i = 0; i < nuu; i++)
            {
                time[i][0] = 0;
                time[i][1] = 0;
                k = idcan[i];
                nmi = (int)((tie1 - tim[k][0]) * ra[k]);
                if (nmi < 0) nmi = 0;
                final = nmi + (int)(60.0 * ra[k]);
                if (final >= tim[k].Length) final = tim[k].Length - 1;
                for (j = nmi; j < final; j++)
                {
                    if ((int)(Math.Abs(cu[k][j] - nivelbase[i])) >= tope[i])
                    {
                        time[i][0] = tim[k][j];
                        if (ticero == 0) ticero = time[i][0];
                        else if (ticero > time[i][0]) ticero = time[i][0];
                        if (tifin == 0) tifin = time[i][0];
                        else if (tifin < time[i][0] && time[i][0] > 0) tifin = time[i][0];
                        break;
                    }
                }
                if (time[i][0] == 0) sii[i] = true;
            }
            tifin = time[0][0];
            for (i = 1; i < nuu; i++) if (tifin < time[i][0]) tifin = time[i][0];
            ticero = tifin;
            for (i = 0; i < nuu; i++) if (ticero > time[i][0] && time[i][0] > 0) ticero = time[i][0];
            if (tifin - ticero < 1.0) tifin = ticero + 1.0;

            do
            {
                if (redibarribos == true)
                {
                    util.borra(splitContainer1.Panel1, colfondo);
                    redibarribos = false;
                }
                VerTrazasArribos(idcan, (tie1 - 1.0), time, siMLvista, siML);
                if (kilometro == true)
                {
                    Circulos(splitContainer1.Panel2, (short)(nuu), facmap, la, lo, laa, loo, idcan);
                }

                for (dd = ticero - 0.01; dd <= tifin + 0.01; dd += 0.01)
                {
                    for (i = 0; i < nuu; i++)
                    {
                        if (dd >= time[i][0] && time[i][0] > 0 && sii[i] == false)
                        {
                            util.UnaEstacionArribo(splitContainer1.Panel2, facmap, laa, loo, la[i], lo[i], col);
                            sii[i] = true;
                        }
                        if (stop == true)
                        {
                            break;
                        }
                        else if (pausa == true)
                        {
                            do
                            {
                                if (stop == true) break;
                            } while (pausa == true);
                        }
                        Thread.SpinWait(velo);
                        if (salto == true) break;
                    }
                    if (salto == true) break;
                }
                if (salto == true)
                {
                    util.borra(splitContainer1.Panel2, Color.White);
                    util.TopoMapaArribos(splitContainer1.Panel2, facmap, laa, loo, Maparr);
                    util.EstacionesArribos(splitContainer1.Panel2, facmap, laa, loo, la, lo);
                    salto = false;
                }
                if (col == Color.Red) col = Color.White;
                else col = Color.Red;
                for (i = 0; i < nuu; i++)
                {
                    if (time[i][0] > 0) sii[i] = false;
                    else sii[i] = true;
                }
                tifin = time[0][0];
                for (i = 1; i < nuu; i++) if (tifin < time[i][0]) tifin = time[i][0];
                ticero = tifin;
                for (i = 0; i < nuu; i++) if (ticero > time[i][0] && time[i][0] > 0) ticero = time[i][0];
                if (tifin - ticero < 1.0) tifin = ticero + 1.0;
                if (factormapa == 1)
                {
                    facmap += facmap * 0.3;
                }
                else if (factormapa == -1)
                {
                    facmap -= facmap * 0.3;
                }
                if (factormapa != 0)
                {
                    util.borra(splitContainer1.Panel2, Color.White);
                    util.TopoMapaArribos(splitContainer1.Panel2, facmap, laa, loo, Maparr);
                }
                factormapa = 0;

                if (hpvista == true)
                {
                    if (File.Exists(".\\h\\r.inp")) File.Delete(".\\h\\r.inp");
                    if (File.Exists(".\\h\\r.pun")) File.Delete(".\\h\\r.pun");
                    lin = ".\\h\\" + Maparr + ".mod";
                    if (!File.Exists(lin))
                    {
                        NoMostrar = true;
                        MessageBox.Show("NO EXISTE el archivo de Volcan: " + lin);
                        return;
                    }
                    File.Copy(lin, ".\\h\\r.inp", true);
                    StreamWriter hp = File.AppendText(".\\h\\r.inp");
                    for (j = 0; j <= 10; j++)
                    {
                        for (i = 0; i < nuu; i++)
                        {
                            if (time[i][0] > 0)
                            {
                                lll = (long)(time[i][0]);
                                ms = Math.Round((time[i][0] - lll) * 100.0);
                                ca = est[idcan[i]].Substring(0, 4) + "IP 0 ";
                                lll = (long)(Fei + time[i][0] * 10000000.0); // se convierte el tiempo en SUDS a tiempo en visual c#
                                DateTime fech = new DateTime(lll);
                                ca += string.Format("{0:yy}{0:MM}{0:dd}{0:HH}{0:mm}{0:ss}.{1:00}", fech, ms);
                                if (time[i][1] > time[i][0] && time[i][0] > 0)
                                {
                                    ff = double.Parse(ca.Substring(19, 5)) + (time[i][1] - time[i][0]);
                                    if (ff < 100.0) ca += string.Format("       {0:00.00} S 2", ff);
                                    else ca += string.Format("       {0:000.0} S 2", ff);
                                }
                                jj = ca.Length;
                                if (jj < 82)
                                {
                                    ii = 82 - jj;
                                    ca += esp.Substring(0, ii);
                                }
                                hp.WriteLine(ca);
                            }
                        }
                        ca = "                 10" + string.Format("{0:00}.00", j * 3 + 1);
                        jj = ca.Length;
                        if (jj < 82)
                        {
                            ii = 82 - jj;
                            ca += esp.Substring(0, ii);
                        }
                        hp.WriteLine(ca);
                    }
                    hp.Close();

                    if (File.Exists(".\\h\\hp71pc.exe")) lin = "/C .\\h\\hp71pc ";
                    else lin = "/C .\\h\\hp71 ";
                    lin += "< .\\h\\data.inp > .\\h\\bas.txt";
                    util.Dos(lin, true);
                    salto = true;
                    hpvista = false;
                    boHp71Vista.BackColor = Color.Moccasin;
                    MLVista = true;
                }// hpvista==true

                if (cambio == true)
                {
                    if (Maparr == 'X')
                    {
                        lax = la[0];
                        lam = lax;
                        lox = lo[0];
                        lom = lox;
                        for (i = 1; i < nuu; i++)
                        {
                            if (lax < la[i]) lax = la[i];
                            else if (lam > la[i]) lam = la[i];
                            if (lox < lo[i]) lox = lo[i];
                            else if (lom > lo[i]) lom = lo[i];
                        }
                        dd = lax - lam;
                        if (dd < (lox - lom)) dd = lox - lom;
                        dif = dd + dd / 5.0;
                        facmap = (splitContainer1.Panel2.Width / dif) * 0.5;
                        laa = (lax + lam) / 2.0;
                        loo = (lox + lom) / 2.0;
                    }
                    else
                    {
                        facmap = 1100.0 / 0.3;
                        j = 0;
                        for (i = 0; i < nuvol; i++)
                        {
                            if (volcan[i][0] == Maparr)
                            {
                                j = i;
                                break;
                            }
                        }
                        laa = latvol[j];
                        loo = lonvol[j];
                    }
                    cambio = false;
                }
            } while (stop == false);
            if (stop == true) panel1.Invalidate();

            return;
        }
        /// <summary>
        /// Activa la aparición del boMLVista en pantalla.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void boHp71Vista_MouseDown(object sender, MouseEventArgs e)
        {
            hpvista = true;
            boHp71Vista.BackColor = Color.Orange;
            salto = true;
            if (File.Exists(".\\oct\\MLvista.txt") && File.Exists(".\\oct\\estaML.txt")) boMLVista.Visible = true;
        }
        /// <summary>
        /// Cambia el estado de verdad de la variable MLsi, y borra el archivo
        /// con el cual se calcula la magnitud local del sismo en el Octave.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void boMLVista_MouseDown(object sender, MouseEventArgs e)
        {
            if (MLVista == false) return;
            if (MLsi == false)
            {
                MLsi = true;
                boMLVista.BackColor = Color.LimeGreen;
            }
            else
            {
                MLsi = false;
                boMLVista.BackColor = Color.White;
                if (File.Exists(".\\oct\\maglocalVista.txt"))
                    File.Delete(".\\oct\\maglocalVista.txt");
            }
            //MessageBox.Show("1");
        }
        /// <summary>
        /// Dibuja las iniciales de los volcanes en el mapa.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void boMapVista_MouseDown(object sender, MouseEventArgs e)
        {
            int i;
            string ss;
            Color col;

            ss = "Debe correr el Hypo para ver el Mapa";
            tip.IsBalloon = false;
            tip.InitialDelay = 0;
            tip.ReshowDelay = 0;
            tip.AutomaticDelay = 0;
            tip.Show(ss, splitContainer1.Panel2, e.X + 100, e.Y + 15, 3000);

            if (panelModVista.Visible == true)
            {
                panelModVista.Visible = false;
                return;
            }
            panelModVista.Visible = true;
            Graphics dc = panelModVista.CreateGraphics();

            for (i = 0; i <= nuvol; i++)
            {
                if (volcan[i][0] != Maparr)
                    col = Color.Gray;
                else
                    col = Color.Red;
                SolidBrush bro = new SolidBrush(col);
                dc.DrawString(volcan[i].Substring(0, 1), new Font("Times New Roman", 9), bro, 2, i * 11);
                bro.Dispose();
            }
        }
        /// <summary>
        /// Sirve para seleccionar la inicial del volcán del cual se quiere ver en el mapa.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void panelModVista_MouseDown(object sender, MouseEventArgs e)
        {
            int i;
            try
            {
                i = (int)(e.Y / 11.0);
                if (i > nuvol + 1 || i < 0)
                    return;
                Maparr = volcan[i][0];
                boMapVista.Text = "-" + volcan[i].Substring(0, 1) + "-";
                panelModVista.Visible = false;
                cambio = true;
            }
            catch
            {
            }
        }
        /// <summary>
        /// Si se da click con el botón  izquierdo la señal en el mapa se amplifica, en caso que el click sea con el botón derecho
        /// la señal se reduce.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void boTraVista_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                durac += durac * 0.2;
            else durac -= durac * 0.2;
            salto = true;
        }
        /// <summary>
        /// Si el click es con el botón izquierdo desplaza la gráfica de la traza hacia la izquierda,
        /// en caso de que sea el botón derecho desplaza la gráfica hacia la derecha.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void boDesTraVista_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                initic += durac * 0.05;
            else
                initic -= durac * 0.05;
            salto = true;
        }
        /// <summary>
        /// Gráfica un circulo en el panelVista tomando como centro el lugar del sismo.
        /// </summary>
        /// <param name="panel">Panel donde se grafica el mapa.</param>
        /// <param name="can">Cantidad de estaciones en el archivo estavista.txt.</param>
        /// <param name="facmap"></param>
        /// <param name="la">Arreglo con los valores de latitud de los sismos ordenados por orden de arribo.</param>
        /// <param name="lo">Arreglo con los valores de longitud de los sismos ordenados por orden de arribo.</param>
        /// <param name="laa">Promedio de latitudes.</param>
        /// <param name="loo">Promedio de longitudes.</param>
        /// <param name="idcan">Asocia la traza con el sismo con su estación respectiva en el archivo estavista.txt,
        ///  en otras palabras contiene el id de la estación a la que pertenece una traza con respecto al archivo estavista.</param>
        void Circulos(Panel panel, short can, double facmap, double[] la, double[] lo, double laa, double loo, int[] idcan)
        {
            int i, x1, y1, xf, yf, iniX, iniY, dif;
            double km = 0, fcpi, fclo, f1, kmm = 9999999999999.9;
            string ss = "";

            xf = panel.Size.Width;
            yf = panel.Size.Height;
            iniX = xf / 2;
            iniY = yf / 2;
            x1 = 0;
            y1 = 0;
            fcpi = Math.PI / 180.0;
            fclo = facmap * ((Math.PI / 180.0) * Math.Cos(laa * fcpi) * 6367.449) / 110.9;

            Graphics dc = panel.CreateGraphics();
            Pen lapiz = new Pen(Color.Orange, 1);
            Graphics dc2 = panelKm.CreateGraphics();
            SolidBrush bro2 = new SolidBrush(Color.SeaShell);
            dc2.FillRectangle(bro2, 0, 0, panelKm.Width, panelKm.Height);
            bro2.Dispose();


            for (i = 0; i < can; i++)
            {
                if (time[i][1] > 0)
                {
                    km = (time[i][1] - time[i][0]) * 8.0;
                    if (kmm > km)
                    {
                        kmm = km;
                        ss = est[idcan[i]].Substring(0, 4);
                    }
                    dif = (int)((km * facmap / 110.9) / 2.0);
                    f1 = facmap * (la[i] - laa);
                    y1 = iniY - (int)f1;
                    f1 = fclo * (lo[i] - loo);
                    x1 = iniX + (int)f1;
                    dc.DrawEllipse(lapiz, x1 - dif, y1 - dif, 2 * dif, 2 * dif);
                    //MessageBox.Show("i=" + i.ToString()+" S-9="+(time[i][1]-time[i][0]).ToString());
                }
            }
            lapiz.Dispose();

            if (kmm < 9999999999999.9)
            {
                ss += " " + string.Format("{0:0.00} Km", km);
                SolidBrush bro = new SolidBrush(Color.SeaShell);
                dc2.FillRectangle(bro, 0, 0, panelKm.Width, panelKm.Height);
                bro.Dispose();
                SolidBrush brocha = new SolidBrush(Color.Black);
                dc2.DrawString(ss, new Font("Times New Roman", 10), brocha, 3, 3);
                brocha.Dispose();
            }

            return;
        }
        /// <summary>
        /// Asocia letras del teclado con botones de los paneles.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            MouseEventArgs ee = new MouseEventArgs(MouseButtons.Left, 1, 1, 1, 0);

            if (disparo == true) return;

            if (e.KeyChar == '1') 
                Uno();
            else if (e.KeyChar == 'A' || e.KeyChar == 'a') Aumentar(null);
            else if (e.KeyChar == 'B' || e.KeyChar == 'b') Bajar(null);
            else if (e.KeyChar == 'D' || e.KeyChar == 'd') Disminuir(null);
            else if (e.KeyChar == 'C' || e.KeyChar == 'c')
            {
                if (panelcla.Visible == true) panelcla.Visible = false;
                Seguir(false, 0);
            }
            else if (e.KeyChar == 'F' || e.KeyChar == 'f')
            {
                NoMostrar = true;
                fecha();
            }
            else if (e.KeyChar == 'H' || e.KeyChar == 'h') Archivo(null);
            else if (e.KeyChar == 'I' || e.KeyChar == 'i') Invertir();
            else if (e.KeyChar == 'L' || e.KeyChar == 'l') UnaClasificacion(null);
            else if (e.KeyChar == 'M' || e.KeyChar == 'm') Promedio();
            else if (e.KeyChar == 'N' || e.KeyChar == 'n') SimulacionAnalogico();
            else if (e.KeyChar == 'P' || e.KeyChar == 'p')
            {
                NoMostrar = true;
                Parametros();
            }
            else if (e.KeyChar == 'R' || e.KeyChar == 'r') Saturacion(ee);
            else if (e.KeyChar == 'S' || e.KeyChar == 's') Subir(null);
            else if (e.KeyChar == 'U' || e.KeyChar == 'u') CalculoUmbral();
            else if (e.KeyChar == 'V' || e.KeyChar == 'v')
            {
                if (vista == false)
                {
                    vista = true;
                    boVista.BackColor = Color.DarkRed;
                }
                else
                {
                    vista = false;
                    boVista.BackColor = Color.MistyRose;
                }
            }
            //e.KeyChar = 'Ñ';
            return;
        }
        /// <summary>
        /// Esconde el panel de los primeros arribos.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void boMapX_Click(object sender, EventArgs e)
        {
            stop = true;
            boMapX.BackColor = Color.Coral;
            boVista.BackColor = Color.MistyRose;
            initic = 0;
        }
        /// <summary>
        /// Llama al método VerVista si la variable vista es true. 
        /// </summary>
        void Underground()
        {
            if (vista == true)
            {
                VerVista();
            }

            return;
        }
        /*  void Hilo()
          {
              MessageBox.Show("hola");
          }*/
        /// <summary>
        /// Rutina que corre en el 'fondo' del programa o sea un hilo ('Thread') de fondo.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            if (vista == true)
                Underground();
            else
            {
                if (mptintp == false)
                    MoverNEZ(va[0], va[1], va[2], e);
                else
                    MoverNEZinterp(va[0], va[1], va[2], e);
            }
        }
        /// <summary>
        /// Inicializa la visualizacion de los primeros arribos..
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void boPlay_Click(object sender, EventArgs e)
        {// 
            velo = 100000;
            salto = true;
        }
        /// <summary>
        /// Pausa o no la visualización de los primeros arribos.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void boPausa_Click(object sender, EventArgs e)
        {
            if (pausa == true)
            {
                pausa = false;
                boPausa.BackColor = Color.White;
            }
            else
            {
                pausa = true;
                boPausa.BackColor = Color.Gold;
            }
        }
        /// <summary>
        /// Desacelera la visualización de los primeros arribos.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void boLow_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) velo += 1000;
            else velo += 10000;
            salto = true;
        }
        /// <summary>
        /// Acelera la visualización de los primeros arribos.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void boFast_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) velo -= 1000;
            else velo -= 10000;
            if (velo < 1000) velo = 1000;
            salto = true;
        }
        /// <summary>
        /// Aumenta la dimensión del mapa.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void boMapAmp_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) factormapa = -1;
            else factormapa = 1;
            salto = true;
        }
        /// <summary>
        /// Inicio de la lectura de S-P en el panel de trazas de los primeros arribos.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void splitContainer1_Panel1_MouseDown(object sender, MouseEventArgs e)
        {
            bxi = e.X;
        }
        /// <summary>
        /// Final de la lectura de S-P en el panel de trazas de los primeros arribos.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void splitContainer1_Panel1_MouseUp(object sender, MouseEventArgs e)
        {
            int i, ii, xf, yf, tot, nucan, y1, mu1, mu2, muu, mxx, mnn;
            double fax, fay, tiempo, laf, lof, laesta, loesta;
            double lla, llo, dis;
            string ss = "", nom = "", li = "", factor = "";
            char[] delim = { ' ', '\t' };
            string[] pa = null;
            Color col;

            xf = splitContainer1.Panel1.Width - 40;
            yf = splitContainer1.Panel1.Height - 20;
            fax = xf / durac;             // factor en la horizontal    
            nucan = idT1.Length;
            fay = yf / (nucan + 0.5); // factor en la vertical
            i = (int)((e.Y - 5) / fay);
            //MessageBox.Show("i=" + i.ToString()+" nuesvista="+nuesvista[i].ToString() + " est=" + est[nuesvista[i]]);

            if (e.X == bxi)
            {
                if (idT1[i] > -1)
                {
                    y1 = (int)(5.0 + i * fay + fay / 2.0);
                    Graphics dc = splitContainer1.Panel1.CreateGraphics();
                    if (e.X >= 40)
                    {
                        tot = (int)(Math.Abs(e.X - 40));
                        tiempo = (tot / fax);
                        if (e.Button == MouseButtons.Left)
                        {
                            time[idT1[i]][0] = tiniT + tiempo;
                            col = Color.Green;
                        }
                        else
                        {
                            if (time[idT1[i]][1] > 0) time[idT1[i]][1] = 0;
                            else
                            {
                                if (time[idT1[i]][0] > 0)
                                {
                                    time[idT1[i]][1] = tiniT + tiempo;
                                    if (time[idT1[i]][1] < time[idT1[i]][0]) time[idT1[i]][1] = 0;
                                }
                            }
                            col = Color.BlueViolet;
                        }
                        Pen lap = new Pen(col, 3);
                        dc.DrawLine(lap, e.X, y1 - 8, e.X, y1 + 8);
                        lap.Dispose();
                    }
                    else
                    {
                        time[idT1[i]][0] = 0;
                        SolidBrush brocha = new SolidBrush(Color.Red);
                        dc.FillEllipse(brocha, 5, y1 - 4, 8, 8);
                        brocha.Dispose();
                    }
                    redibarribos = true;
                    if (e.Button == MouseButtons.Right) salto = true;
                }
            }
            else
            {
                if (MLsi == false)
                {
                    tot = (int)(Math.Abs(e.X - bxi));
                    tiempo = tot / fax;
                    ss = string.Format("{0:0.0}s", tiempo);
                    tip.IsBalloon = true;
                    tip.InitialDelay = 0;
                    tip.ReshowDelay = 0;
                    tip.AutomaticDelay = 0;
                    tip.SetToolTip(splitContainer1.Panel1, ss);
                }
                else
                {
                    if (File.Exists(".\\oct\\maglocalVista.txt")) File.Delete(".\\oct\\maglocalVista.txt");
                    tot = (int)(Math.Abs(bxi - 40));
                    tiempo = tiniT + (tot / fax);
                    mu1 = (int)((tiempo - tim[nuesvista[i]][0]) * ra[nuesvista[i]]);
                    tot = (int)(Math.Abs(e.X - 40));
                    tiempo = tiniT + (tot / fax);
                    mu2 = (int)((tiempo - tim[nuesvista[i]][0]) * ra[nuesvista[i]]);
                    if (mu1 > mu2)
                    {
                        muu = mu1;
                        mu1 = mu2;
                        mu2 = muu;
                    }
                    laesta = 100.0;
                    loesta = 0;
                    nom = est[nuesvista[i]].Substring(0, 4);
                    li = "";
                    StreamReader ar = new StreamReader(".\\oct\\estaML.txt");
                    while (li != null)
                    {
                        try
                        {
                            li = ar.ReadLine();
                            if (li == null) break;
                            pa = li.Split(delim);
                            if (nom.Substring(0, 4) == pa[2].Substring(0, 4))
                            {
                                laesta = double.Parse(pa[4]) + double.Parse(pa[5]) / 60.0;
                                loesta = double.Parse(pa[6]) + double.Parse(pa[7]) / 60.0;
                                factor = pa[8];
                                break;
                            }
                        }
                        catch
                        {
                        }
                    }
                    ar.Close();
                    if (laesta > 90.0) return;
                    li = "";
                    laf = 100.0;
                    lof = 0;
                    StreamReader ar2 = new StreamReader("locML.txt");
                    try
                    {
                        li = ar2.ReadLine();
                        pa = li.Split(delim);
                        laf = double.Parse(pa[0]);
                        lof = double.Parse(pa[1]);
                    }
                    catch
                    {
                    }
                    ar2.Close();
                    if (laf > 90.0) return;

                    StreamWriter wr = File.CreateText(".\\oct\\ini_ml.txt");
                    StreamWriter da = File.CreateText(nom);

                    for (ii = mu1; ii < mu2; ii++)
                    {
                        da.WriteLine(cu[nuesvista[i]][ii]);
                    }

                    wr.WriteLine("fs=" + ra[nuesvista[i]] + ";");
                    lla = Math.Abs(laf - laesta);
                    llo = Math.Abs(lof - loesta);
                    dis = 111.1 * Math.Sqrt(lla * lla + llo * llo);
                    wr.WriteLine("dist=" + dis.ToString() + ";");
                    wr.WriteLine("Z=load " + nom.Substring(0, 4) + ";");
                    wr.WriteLine("nomZ='" + nom.Substring(0, 4) + "';");
                    wr.WriteLine("gvz=" + factor.Substring(4));

                    da.Close();
                    wr.Close();

                    li = "/C c:\\octave\\bin\\octave.exe < .\\oct\\MLvista.txt";
                    util.Dos(li, true);
                    util.ColocarMLVista(splitContainer1.Panel2);
                }
            }
        }
        /// <summary>
        /// Llama al método TrazasClas() para dibujar las trazasdel panel de clasificación.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void boXfft_Click(object sender, EventArgs e)
        {
            panelFFTzoom.Visible = false;
            panelBarEsp1.Visible = false;
            if (panelcladib.Visible == true) TrazasClas();
            yloc = -1;
        }
        /// <summary>
        /// Se lanza cuando cambia el valor del checkBoxLogEsp, si CualPaneles igual a 0 o 1
        /// llama al método Espectro()
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void checkBoxLogEsp_CheckedChanged(object sender, EventArgs e)
        {
            Panel pan, panelBar;
            ushort idd;

            if (CualPanel == 0)
            {
                pan = panel1;
                panelBar = panelBarEsp1;
                if (panelcladib.Visible == false)
                    idd = id;
                else
                    idd = idc;
            }
            else if (CualPanel == 1)
            {
                pan = panel1a;
                panelBar = panelBarEsp1a;
                if (panelcladib.Visible == false)
                    idd = ida;
                else
                    idd = idc;
            }
            else
                return;
            Espectro(pan, panelBar, idd, false);
        }
        /// <summary>
        /// Activa o desactiva (segun el estado de la variable VerEspectro) la funcionalidad que permite ver el espectro de una traza.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void boEspe_MouseDown(object sender, MouseEventArgs e)
        {

            if (VerEspectro == false)
            {
                VerEspectro = true;
                boEspe.BackColor = Color.ForestGreen;
                if (interpol == true)
                {
                    interpol = false;
                    boInterp.BackColor = Color.WhiteSmoke;
                    panelInterP.Visible = false;
                }
            }
            else
            {
                VerEspectro = false;
                boEspe.BackColor = Color.WhiteSmoke;
                panelFFTzoom.Visible = false;
                panelBarEsp1.Visible = false;
                moveresp = false;
                movespcla = false;
                boEspCla.BackColor = Color.White;
            }
        }
        /// <summary>
        /// Con click sostenido sobre el panel que muestra el espectro se pueden ver los datos de la traza
        /// justo en la posición donde esta situado el mouse.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void panelFFTzoom_MouseDown(object sender, MouseEventArgs e)
        {
            int x, y, nn;
            Panel panel;

            if (silog == true)
                return;
            if (panelcladib.Visible == true)
                panel = panelcladib;
            else
                panel = panel1;
            nn = np;
            if (nn < 1024)
                nn = 1024;
            x = (int)(nn * 0.05);
            panelValFFt.Size = new Size(x, panelValFFt.Height);
            x = (int)(e.X + panel.Location.X + panelFFTzoom.Location.X - panelValFFt.Size.Width / 2.0);
            y = panel1.Location.Y + panelFFTzoom.Location.Y - panelValFFt.Height;
            panelValFFt.Location = new Point(x, y);
            panelValFFt.Visible = true;
            panelBarEsp.Visible = true;

            ZoomEspectro(e);
        }
        /// <summary>
        /// Oculta el panel que muestra la información del espectro y llama a al método especifico encargado de graficar el 
        /// espectro de la traza dependiendo del valor de la variable silog. 
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void panelFFTzoom_MouseUp(object sender, MouseEventArgs e)
        {
            if (silog == true)
                return;
            panelValFFt.Visible = false;
            panelBarEsp.Visible = false;
            if (silog == false)
                GraficaEspectro(id, vaesp, checkBoxFFT1.Checked, vacioesp);
            else
                GraficaEspectroLog(id, vaesp, vacioesp);
        }
        /// <summary>
        /// Actualiza la posición en la que se debe dibujar el panel 
        /// que muestra la información del espectro en un punto especifico.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void panelFFTzoom_MouseMove(object sender, MouseEventArgs e)
        {
            int x, y;

            if (silog == true)
                return;
            x = (int)(e.X + panel1.Location.X + panelFFTzoom.Location.X - panelValFFt.Size.Width / 2.0);
            y = panelFFTzoom.Location.Y - panelValFFt.Height;
            panelValFFt.Location = new Point(x, y);

            ZoomEspectro(e);

            return;
        }
        /// <summary>
        /// Determina los valores mayor y menor del espectro y con estos calcula cual debe ser el alto del panel,
        /// después tomando los valores del espectro calcula los puntos x,y en que debe dibujar cada punto y los dibuja, esto genera una gráfica.
        /// </summary>
        /// <param name="e">Se usa para determinar la posición en donde está ubicado el mouse.</param>
        void ZoomEspectro(MouseEventArgs e)
        {
            int i, j, j1, j2, jj, k, kk, xf, yf, x1, y1, yini;
            double ddx, mxx, mnn, fy, fx, fr;
            string ca = "";

            ddx = (panelFFTzoom.Width - 90.0) / vaesp.Length;
            i = (int)((e.X - 60.0) / ddx);
            k = (int)(np / 512.0);
            j1 = i - 4 * k;
            if (j1 < 0) j1 = 0;
            j2 = i + 4 * k;
            if (j2 > vaesp.Length) j2 = vaesp.Length - 1;

            Graphics dc = panelValFFt.CreateGraphics();
            SolidBrush bro = new SolidBrush(Color.PeachPuff);
            dc.FillRectangle(bro, 0, 0, panelValFFt.Width, panelValFFt.Height);
            bro.Dispose();
            Pen lap = new Pen(Color.Black, 1);
            Pen lap2 = new Pen(Color.Red, 1);
            SolidBrush bro2 = new SolidBrush(Color.Red);

            xf = panelValFFt.Width - 10;
            yf = panelValFFt.Height - 15;

            try
            {
                mxx = vaesp[j1];
                mnn = mxx;
                kk = j1;
                for (j = j1; j < j2; j++)
                {
                    if (mxx < vaesp[j])
                    {
                        mxx = vaesp[j];
                        kk = j;
                    }
                    else if (mnn > vaesp[j])
                        mnn = vaesp[j];
                }

                fx = xf / (double)(j2 - j1);
                fy = yf / (mxesp - mnesp);
                yini = panelValFFt.Height - 3;
                jj = (int)(panelValFFt.Width / 2.0) - 10;

                try
                {
                    for (j = j1; j < j2; j++)
                    {
                        x1 = (int)(5.0 + (j - j1) * fx);
                        y1 = yini - (int)((vaesp[j] - mnesp) * fy);
                        if (j == kk)
                        {
                            fr = kk * (ra[id] / (double)(np));
                            ca = string.Format("{0:0.00}", fr);
                            dc.DrawString(ca, new Font("Arial", 8), bro2, jj, 0);
                            dc.DrawLine(lap2, x1, yini, x1, y1);
                            continue;
                        }
                        dc.DrawLine(lap, x1, yini, x1, y1);
                    }

                }
                catch
                {
                }
            }
            catch
            {
            }
            lap.Dispose();
            lap2.Dispose();
            bro2.Dispose();

            fx = (panelFFTzoom.Width - 90) / (double)(vaesp.Length);
            x1 = (int)(60 + fx * j1);
            y1 = (int)(60 + fx * j2);
            panelBarEsp.Location = new Point(x1, panelBarEsp.Location.Y);
            panelBarEsp.Size = new Size((y1 - x1), panelBarEsp.Height);

            return;
        }
        /// <summary>
        /// Se encarga de calcular el espectro de una porción de traza y preparar el panel donde se muestra el resultado.
        /// </summary>
        /// <param name="pan">Panel en el que se está clasificando la traza actual, panel1 en caso de ser el panel principal,
        /// y panel1a en caso de ser el panel secundario.</param>
        /// <param name="panelBar">Se usa para dibujar una barra de ubicación.</param>
        /// <param name="id">Id de la traza que se está clasificando.</param>
        /// <param name="cond">Se usa para verificar si se modifica o no la localización del panel panelFFTzoom,
        /// si cond es true se cambia la posición del panel, si es false no. </param>
        void Espectro(Panel pan, Panel panelBar, ushort id, bool cond)
        {
            short py;
            int i, nu, nmi, anch, x, y, xf, yf, cual, denom, tota;
            int max, min, cero;
            double fax, fay, ff, facra;

            if (VerEspectro == false)
                return;

            tota = cu[id].Length; //ultimo valor de cuenta de la traza actual
            denom = (int)(Math.Abs(Math.Ceiling((tim[id][tota - 1] - timin) / dur)));
            if (denom <= 0)
                denom = 1;
            xf = pan.Size.Width;
            yf = pan.Size.Height;
            if (esp == 0)
                fay = (yf - 45.0F) / denom;
            else
                fay = esp;
            fax = xf / dur;
            ff = (yesp + 10) - (fay / 2.0);
            nu = (int)(ff / fay);
            py = (short)((nu + 2) * fay);
            if (py + panelFFTzoom.Size.Height > pan.Height)
                py = (short)((nu + 1) * fay - (panelFFTzoom.Size.Height + 1 * fay));
            if (cond == true)
                panelFFTzoom.Location = new Point(10, py);
            if (yloc != py)
            {
                if (yloc != -1)
                    TrazasClas();
                yloc = py;
            }

            if (panelFFTzoom.Visible == false)
                panelFFTzoom.Visible = true;
            panelFFTzoom.BringToFront();
            nmi = (int)((t1esp - tim[id][0]) * ra[id]);
            if (nmi + np >= cu[id].Length)
                nmi = cu[id].Length - (np + 1);
            if (nmi < 0)
                nmi = 0;

            vaesp = new double[np];

            max = cu[id][nmi];
            min = max;
            for (i = 1; i < np; i++)
            {
                if (max < cu[id][i + nmi])
                    max = cu[id][i + nmi];
                else if (min > cu[id][i + nmi])
                    min = cu[id][i + nmi];
            }
            cero = (int)((max + min) / 2.0);

            for (i = 0; i < np; i++)
                vaesp[i] = cu[id][i + nmi] - cero;

            vacioesp = false;
            facra = 1.0 / ra[id];
            for (i = nmi + 1; i < nmi + np; i++)
            {
                if (tim[id][i] - tim[id][i - 1] > (facra + facra * 0.5))
                {
                    vacioesp = true;
                    break;
                }
            }

            if (checkBoxFFT1.Checked == true)
            {
                vaesp = four.RealFFTAmpli(vaesp, true);// calculo del Espectro
                if (checkBoxLogEsp.Checked == true && checkBoxLogEsp.Visible == true)
                {
                    silog = true;
                    for (i = 0; i < vaesp.Length; i++)
                    {
                        if (vaesp[i] > 0)
                            vaesp[i] = Math.Log10(vaesp[i]);
                        else
                            vaesp[i] = -1000000.0;
                    }
                }
                else silog = false;
            }

            x = xesp;
            cual = (int)((yesp - 45) / fay);
            y = (int)((fay / 2.0) + fay * cual);
            anch = (int)((np / ra[id]) * fax);
            if (x + anch > pan.Size.Width)
                x = pan.Size.Width - anch;
            else if (x < 0)
                x = 0;

            // dibuja una barra de ubicacion.
            // Graphics dc = pan.CreateGraphics();
            panelBar.Size = new Size(anch, 2);
            panelBar.Visible = true;
            panelBar.Location = new Point(x, y);

            if (silog == false)
                GraficaEspectro(id, vaesp, checkBoxFFT1.Checked, vacioesp);
            else
                GraficaEspectroLog(id, vaesp, vacioesp);

            return;
        }
        /// <summary>
        /// Realiza el gráfico del espectro de una porción de traza, y lo despliega en el panel panelFFTzoom.
        /// </summary>
        /// <param name="id">Id de la traza que se está clasificando.</param>
        /// <param name="va">Los valores del espectro.</param>
        /// <param name="cond">Indica que el checkBoxFFT1 está seleccionado,
        /// por ende que se aplique la FFT (transformada rápida de Fourier) a la señal.</param>
        /// <param name="vacioesp">true si hay vacios en el espectro, false si el cálculo fue continuo.</param>
        void GraficaEspectro(int id, double[] va, bool cond, bool vacioesp)
        {
            int i, j, k, yini, x, y, fin, offset;
            double dd = 0, ddx, ddy;
            Color col;
            Point[] dat;

            if (cu[id].Length < np)
                return;

            if (cond == false)
                offset = 0;
            else
                offset = offsetesp;
            fin = va.Length;/// cantida de valores de espectro

            ddx = (panelFFTzoom.Width - 90) / (double)(va.Length);

            mxesp = va[offset];
            mnesp = mxesp;
            for (i = offset + 1; i < va.Length; i++)
            {
                if (mxesp < va[i])
                    mxesp = va[i];
                else if (mnesp > va[i])
                    mnesp = va[i];
            }
            j = (int)((mxesp + mnesp) / 2.0); //promedio entre el espectro máximo y el mínimo
            ddy = (panelFFTzoom.Height - 70) / (double)(mxesp - j);
            yini = (int)(panelFFTzoom.Height / 2.0);
            try
            {
                dat = new Point[fin - offset];
                for (i = offset; i < fin; i++)
                {
                    y = (int)(yini + (j - va[i]) * ddy);
                    dd += ddx;
                    x = 60 + (int)(dd + offset * ddx);
                    dat[i - offset].X = x;
                    dat[i - offset].Y = y;
                }
                Graphics dc = panelFFTzoom.CreateGraphics();
                Pen lapp = new Pen(Color.Black, 1);
                SolidBrush broo = new SolidBrush(Color.Lavender);
                SolidBrush bro = new SolidBrush(Color.Blue);
                dc.FillRectangle(broo, 0, 0, panelFFTzoom.Width, panelFFTzoom.Height);
                dc.DrawLines(lapp, dat);
                dc.DrawString(est[id].Substring(0, 4), new Font("Times New Roman", 8, FontStyle.Bold), bro, 0, panelFFTzoom.Height - 35);
                lapp.Dispose();
                broo.Dispose();

                if (cond == false)
                    return;

                //Pen lapHz = new Pen(Color.Magenta, 1);
                j = (int)(ra[id] / 2.0);
                ddx = (panelFFTzoom.Width - 90) / (double)(j);
                for (i = 0; i <= j; i++)
                {
                    x = (int)(60 + i * ddx);
                    col = Color.Magenta;
                    if (Math.IEEERemainder(i, 10) == 0)
                    {
                        col = Color.Blue;
                        k = 7;
                    }
                    else if (Math.IEEERemainder(i, 5) == 0)
                        k = 5;
                    else
                        k = 2;
                    Pen lapHz = new Pen(col, 1);
                    dc.DrawLine(lapHz, x, 0, x, k);
                    lapHz.Dispose();
                }
                //lapHz.Dispose();
                if (vacioesp == true)
                {
                    Pen la = new Pen(Color.Black, 1);
                    SolidBrush br = new SolidBrush(Color.Red);

                    dc.FillEllipse(br, 42, 3, 10, 10);
                    dc.DrawEllipse(la, 42, 3, 10, 10);

                    la.Dispose();
                    br.Dispose();
                }
            }
            catch
            {
                return;
            }

            return;
        }
        /// <summary>
        /// Realiza el gráfico del espectro de una porción de traza, y lo despliega en el panel panelFFTzoom,
        /// anexa un factor al cual le aplica logaritmo en base 10 y con este modifica la gráfica del espectro antes calculado.
        /// </summary>
        /// <param name="id">Id de la traza que se está clasificando.</param>
        /// <param name="va">Los valores del espectro.</param>
        /// <param name="vacioesp">true si hay vacios en el espectro, false si el cálculo fue continuo.</param>
        void GraficaEspectroLog(int id, double[] va, bool vacioesp)
        {
            int i, j, k, yini, x, y, fin, offset;
            double ddx, ddy, max, min;
            double mxfr, deltafr;
            double[] ffr = new double[va.Length];
            Point[] dat;
            Pen Lapiz;

            //if (lar[id] < np) return;
            if (cu[id].Length < np)
                return;

            mxfr = ra[id] / 2.0;
            deltafr = mxfr / (np / 2.0);
            ffr[0] = -1000000.00;
            for (i = 1; i < va.Length; i++)
                ffr[i] = Math.Log10(i * deltafr);
            offset = offsetesp;
            if (offset < 1)
                offset = 1;
            fin = va.Length;

            ddx = (panelFFTzoom.Width - 90) / (ffr[va.Length - 1] - ffr[1]);
            max = va[offset];
            min = max;
            for (i = offset + 1; i < fin; i++)
            {
                if (max < va[i])
                    max = va[i];
                else if (min > va[i])
                    min = va[i];
            }
            ddy = (panelFFTzoom.Height - 15) / (max - min);
            yini = (int)(5);
            try
            {
                dat = new Point[fin - offset];
                for (i = offset; i < fin; i++)
                {
                    y = (int)(yini + (max - va[i]) * ddy);
                    x = 50 + (int)((ffr[i] - ffr[1]) * ddx);
                    dat[i - offset].X = x;
                    dat[i - offset].Y = y;
                }
                Graphics dc = panelFFTzoom.CreateGraphics();
                Pen lapp = new Pen(Color.Black, 1);
                SolidBrush broo = new SolidBrush(Color.Lavender);
                dc.FillRectangle(broo, 0, 0, panelFFTzoom.Width, panelFFTzoom.Height);
                dc.DrawLines(lapp, dat);
                lapp.Dispose();
                broo.Dispose();

                Pen lapHz = new Pen(Color.Magenta, 1);
                Pen lapHz2 = new Pen(Color.Green, 1);
                j = (int)(ra[id] / 2.0);
                for (i = 1; i <= j; i++)
                {
                    x = (int)(50 + (Math.Log10(i) - ffr[1]) * ddx);
                    if (Math.IEEERemainder(i, 10) == 0) Lapiz = lapHz2;
                    else Lapiz = lapHz;
                    if (Math.IEEERemainder(i, 5) == 0)
                    {
                        if (Math.IEEERemainder(i, 10) == 0) k = 8;
                        else k = 5;
                    }
                    else k = 2;
                    dc.DrawLine(Lapiz, x, 0, x, k);
                }
                lapHz.Dispose();
                lapHz2.Dispose();
                if (vacioesp == true)
                {
                    Pen la = new Pen(Color.Black, 1);
                    SolidBrush br = new SolidBrush(Color.Red);

                    dc.FillEllipse(br, 42, 3, 10, 10);
                    dc.DrawEllipse(la, 42, 3, 10, 10);

                    la.Dispose();
                    br.Dispose();
                }
            }
            catch
            {
                return;
            }

            return;
        }
        /// <summary>
        /// Cambia el valor de la variable np a 512 y recalcula el espectro.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void radioFFT1_CheckedChanged(object sender, EventArgs e)
        {
            Panel pan, panelBar;
            ushort idd;

            if (CualPanel == 0)
            {
                pan = panel1;
                panelBar = panelBarEsp1;
                if (panelcladib.Visible == false) idd = id;
                else idd = idc;
            }
            else if (CualPanel == 1)
            {
                pan = panel1a;
                panelBar = panelBarEsp1a;
                if (panelcladib.Visible == false) idd = ida;
                else idd = idc;
            }
            else return;
            np = 512;
            Espectro(pan, panelBar, idd, false);
        }
        /// <summary>
        /// Cambia el valor de la variable np a 1024 y recalcula el espectro.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void radioFFT2_CheckedChanged(object sender, EventArgs e)
        {
            Panel pan, panelBar;
            ushort idd;

            if (CualPanel == 0)
            {
                pan = panel1;
                panelBar = panelBarEsp1;
                if (panelcladib.Visible == false) idd = id;
                else idd = idc;
            }
            else if (CualPanel == 1)
            {
                pan = panel1a;
                panelBar = panelBarEsp1a;
                if (panelcladib.Visible == false) idd = ida;
                else idd = idc;
            }
            else return;
            np = 1024;
            Espectro(pan, panelBar, idd, false);
        }
        /// <summary>
        /// Cambia el valor de la variable np a 2048 y recalcula el espectro.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void radioFFT3_CheckedChanged(object sender, EventArgs e)
        {
            Panel pan, panelBar;
            ushort idd;

            if (CualPanel == 0)
            {
                pan = panel1;
                panelBar = panelBarEsp1;
                if (panelcladib.Visible == false) idd = id;
                else idd = idc;
            }
            else if (CualPanel == 1)
            {
                pan = panel1a;
                panelBar = panelBarEsp1a;
                if (panelcladib.Visible == false) idd = ida;
                else idd = idc;
            }
            else return;
            np = 2048;
            Espectro(pan, panelBar, idd, false);
        }
        /// <summary>
        /// Activa o desactiva la función para calcular el espectro.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void checkBoxFFT1_CheckedChanged(object sender, EventArgs e)
        {
            Panel pan, panelBar;
            ushort idd;

            if (CualPanel == 0)
            {
                pan = panel1;
                panelBar = panelBarEsp1;
                if (panelcladib.Visible == false) idd = id;
                else idd = idc;
            }
            else if (CualPanel == 1)
            {
                pan = panel1a;
                panelBar = panelBarEsp1a;
                if (panelcladib.Visible == false) idd = ida;
                else idd = idc;
            }
            else return;
            Espectro(pan, panelBar, idd, false);
        }
        /// <summary>
        /// Cambia el valor de la variable np a 4096 y recalcula el espectro.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void radioFTT4_CheckedChanged(object sender, EventArgs e)
        {
            Panel pan, panelBar;
            ushort idd;

            if (CualPanel == 0)
            {
                pan = panel1;
                panelBar = panelBarEsp1;
                if (panelcladib.Visible == false) idd = id;
                else idd = idc;
            }
            else if (CualPanel == 1)
            {
                pan = panel1a;
                panelBar = panelBarEsp1a;
                if (panelcladib.Visible == false) idd = ida;
                else idd = idc;
            }
            else return;
            np = 4096;
            Espectro(pan, panelBar, idd, false);
        }
        /// <summary>
        /// Modifica el valor del offset que se le aplica al espectro.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void boOfsset_MouseDown(object sender, MouseEventArgs e)
        {
            Panel pan, panelBar;
            ushort idd;

            if (CualPanel == 0)
            {
                pan = panel1;
                panelBar = panelBarEsp1;
                if (panelcladib.Visible == false) idd = id;
                else idd = idc;
            }
            else if (CualPanel == 1)
            {
                pan = panel1a;
                panelBar = panelBarEsp1a;
                if (panelcladib.Visible == false) idd = ida;
                else idd = idc;
            }
            else return;

            if (e.Button == MouseButtons.Left) offsetesp += 1;
            else offsetesp -= 1;
            if (offsetesp < 0) offsetesp = 9;
            else if (offsetesp > 9) offsetesp = 0;
            boOfsset.Text = offsetesp.ToString();
            Espectro(pan, panelBar, idd, false);
        }
        /// <summary>
        /// Activa o desactiva la clasificación del sismo con movimiento de particulas al cambiar el valor de verdad de la variable moverparti.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void boParti_MouseDown(object sender, MouseEventArgs e)
        {
            if (vista == true || splitContainer1.Visible == true) return;
            if (moverparti == false)
            {
                moverparti = true;
                boParti.BackColor = Color.Orange;
                panel2.Visible = true;
                util.Mensaje(panel2, "Pique inicio en Traza\nde 3 Componentes", false);
            }
            else
            {
                moverparti = false;
                boParti.BackColor = Color.White;
                if (panelParti.Visible == true) panelParti.Visible = false;
                if (panel2.Visible == true) panel2.Visible = false;
            }
        }
        /// <summary>
        /// Cálcula el movimiento de partículas para una traza en específico y despliega el resultado en pantalla. 
        /// </summary>
        void MovimientoParticula()
        {
            int i, id;
            int k, grad, x, y, x2;
            double minu, la1, lo1;
            string nom = "", ca = "", pa = "", li = "";
            char cc;

            NoMostrar = true;
            id = idmpt;
            cc = est[id][3];
            if (cc != 'Z' && cc != 'N' && cc != 'E')
            {
                moverparti = false;
                boParti.BackColor = Color.White;
                if (panelParti.Visible == true) panelParti.Visible = false;
                if (panel2.Visible == true) panel2.Visible = false;
                return;
            }

            nom = est[id].Substring(0, 3);
            for (i = 0; i < nutra; i++)
            {
                if (est[i].Substring(0, 4) == nom.Substring(0, 3) + 'Z') Z = (short)(i);
                else if (est[i].Substring(0, 4) == nom.Substring(0, 3) + 'N') N = (short)(i);
                else if (est[i].Substring(0, 4) == nom.Substring(0, 3) + 'E') E = (short)(i);
            }

            if (Z == -1 || N == -1 || E == -1)
            {
                moverparti = false;
                boParti.BackColor = Color.White;
                if (panelParti.Visible == true) panelParti.Visible = false;
                if (panel2.Visible == true) panel2.Visible = false;
                return;
            }

            if (ra[N] != ra[E] || ra[N] != ra[Z] || ra[E] != ra[Z])
            {
                MessageBox.Show("Las Ratas de muestreo NO son Iguales!!");
                return;
            }

            panelParti.BringToFront();
            panelParti.Visible = true;

            if (File.Exists(".\\pro\\estarro.txt"))
            {
                k = -1;
                li = "";
                cc = ' ';
                StreamReader pr = new StreamReader(".\\pro\\estarro.txt");
                while (li != null)
                {
                    try
                    {
                        li = pr.ReadLine();
                        if (li == null) break;
                        if (li.Length >= 6)
                        {
                            if (li.Substring(0, 4) == est[Z].Substring(0, 4))
                            {
                                cc = li[5];
                                break;
                            }
                            else if (li.Substring(0, 4) == est[N].Substring(0, 4))
                            {
                                cc = li[5];
                                break;
                            }
                            else if (li.Substring(0, 4) == est[E].Substring(0, 4))
                            {
                                cc = li[5];
                                break;
                            }
                        }
                    }
                    catch
                    {
                    }
                }
                pr.Close();
                if (cc == ' ') k = vol;
                else
                {
                    k = vol;
                    for (i = 0; i <= nuvol; i++)
                    {
                        if (cc == volcan[i][0])
                        {
                            k = i;
                            break;
                        }
                    }
                }
            }
            else k = vol;

            ca = ".\\h\\" + volcan[k][0] + ".mod";
            if (!File.Exists(ca))
            {
                pa = rutbas + "\\h\\" + volcan[k][0] + ".mod";
                if (File.Exists(pa)) File.Copy(pa, ca, true);
                if (!File.Exists(ca))
                {
                    NoMostrar = true;
                    MessageBox.Show("no existe " + ca + "\nNecesario para situar la estacion en el mapa!!");
                    panelParti.Visible = false;
                    return;
                }
            }
            pa = ".\\coor\\" + volcan[k][0] + ".map";
            if (!File.Exists(pa))
            {
                NoMostrar = true;
                MessageBox.Show("no existe el mapa " + pa + " !!");
                panelParti.Visible = false;
                return;
            }

            StreamReader ar = new StreamReader(ca);
            laE = 500.0;
            loE = 500.0;
            li = "";
            while (li != null)
            {
                try
                {
                    li = ar.ReadLine();
                    if (li == null) break;
                    if (li.Substring(2, 3) == nom.Substring(0, 3) && (li[5] == 'Z' || li[5] == 'N' || li[5] == 'E'))
                    {
                        grad = int.Parse(li.Substring(6, 2));
                        minu = double.Parse(li.Substring(8, 5));
                        laE = (double)(grad) + minu / 60.0;
                        if (li[13] == 'S') laE = -1.0 * laE;
                        grad = int.Parse(li.Substring(14, 3));
                        minu = double.Parse(li.Substring(17, 5));
                        loE = (double)(grad) + minu / 60.0;
                        if (li[22] == 'W') loE = -1.0 * loE;
                    }
                }
                catch
                {
                }
            }
            ar.Close();

            if (laE == 500.0 || loE == 500.0)
            {
                NoMostrar = true;
                MessageBox.Show("NO se encuentra la Estacion en el Modelo!!");
                return;
            }

            la1 = laE - latvol[k];
            lo1 = loE - lonvol[k];
            difmpt = Math.Sqrt(la1 * la1 + lo1 * lo1);
            if (modX == false) volmpt = (short)(k);
            else volmpt = nuvol;
            NoMostrar = true;
            util.VerMapa(panelPartiEN, volcan[k][0], laE, loE, "", difmpt, laE, loE, Color.LightGray);
            if (mptintp == false)
                TrazaComponente();
            else
                TrazaComponenteInterp();

            ca = velompt.ToString() + " ms";
            Graphics dc = panelDatosMpt.CreateGraphics();
            SolidBrush bro = new SolidBrush(Color.Black);
            dc.DrawString(ca, new Font("Arial", 8), bro, 5, 3);

            Graphics dc2 = panelPartiNZ.CreateGraphics();
            Graphics dc3 = panelPartiEZ.CreateGraphics();
            Pen lap = new Pen(Color.Black, 1);
            Pen lap2 = new Pen(Color.Red, 1);
            x = (int)(panelPartiNZ.Width / 3.0);
            y = (int)(panelPartiNZ.Height / 2.0);
            x2 = x * 2;
            dc2.DrawLine(lap, x, y, x2, y);
            dc2.DrawString("N", new Font("Arial", 9, FontStyle.Bold), bro, panelPartiNZ.Width - 20, y - 6);
            dc2.DrawRectangle(lap2, (int)(panelPartiNZ.Width / 2.0) - 3, y - 3, 6, 6);
            x = (int)(panelPartiEZ.Width / 3.0);
            y = (int)(panelPartiEZ.Height / 2.0);
            x2 = x * 2;
            dc3.DrawLine(lap, x, y, x2, y);
            dc3.DrawString("E", new Font("Arial", 9, FontStyle.Bold), bro, panelPartiEZ.Width - 20, y - 6);
            dc3.DrawRectangle(lap2, (int)(panelPartiEZ.Width / 2.0) - 3, y - 3, 6, 6);
            lap.Dispose();
            bro.Dispose();

            return;
        }
        /// <summary>
        /// Este método es el encargado gestionar el cálculo de la interpolación a una porción de 
        /// traza para cada una de sus componentes E, N y Z (en caso de tenerlas), y de generar 
        /// la gráfica respectiva de esa interpolación en el panel que corresponde a cada componente. 
        /// </summary>
        void TrazaComponenteInterp()
        {
            int i, j, k, xf, yf, x, y, idt;
            int cero = 0;
            double mmxN, mmnN, mmxE, mmnE, mmxZ, mmnZ;
            int[] mi, mf, numu; //minutoInicial   minutoFinal  número de muestras
            double fax, fay, pico;
            double[] mx, mn; //valor máximo   valor minimo
            double[][] val = new double[3][];
            Panel[] panel = new Panel[3];
            Point[] dat;


            util.borra(panelPartiTraN, Color.White);
            util.borra(panelPartiTraE, Color.White);
            util.borra(panelPartiTraZ, Color.White);

            xf = panelPartiTraN.Size.Width - 50;
            yf = panelPartiTraN.Size.Height - 20;
            fax = xf / (double)(durmpt);

            mi = new int[3];
            mf = new int[3];
            numu = new int[3];
            mx = new double[3];
            mn = new double[3];

            for (i = 0; i < 3; i++)
            {
                val[i] = new double[1];
                if (i == 0)
                    idt = N;
                else if (i == 1)
                    idt = E;
                else
                    idt = Z;

                mi[i] = (int)((timpt - timspl[0]) * ra[idt] * facRaInterp) + suma;

                if (mi[i] < 0)
                    mi[i] = 0;
                mimpt[i] = mi[i];

                mf[i] = (int)(mi[i] + (durmpt * ra[idt] * facRaInterp));

                numu[i] = mf[i] - mi[i];
                if (numu[i] < 10) return;

                if (yaInterp == false)
                {
                    CalculoInterpolacion(idt);
                    splintp[i] = new int[spl.Length];
                    for (k = 0; k < spl.Length; k++) splintp[i][k] = spl[k];
                }
                val[i] = new double[numu[i]];
                timsplintp = new double[numu[i]];
                for (j = 0; j < numu[i]; j++)
                {
                    val[i][j] = (double)(splintp[i][mi[i] + j]);
                    timsplintp[j] = timspl[mi[i] + j];
                }


                mx[i] = val[i][0];
                mn[i] = mx[i];
                for (j = 1; j < numu[i]; j++)
                {
                    if (mx[i] < val[i][j])
                        mx[i] = val[i][j];
                    else if (mn[i] > val[i][j])
                        mn[i] = val[i][j];
                }/// encuentra los valores minimo y máximo de la porción de traza por componente
            }
            yaInterp = true;

            for (i = 0; i < 3; i++)
            {
                va[i] = new int[numu[i]];
                mnmpt[i] = (int)(mn[i]);
            }

            pico = mx[0] - mn[0];
            for (i = 1; i < 3; i++)
                if (pico < (mx[i] - mn[i]))
                    pico = mx[i] - mn[i];
            fay = ((double)(yf) / pico);
            faympt = fay;

            if (sicerompt == false)
            {
                mmxN = val[0][0];
                mmnN = mmxN;
                for (i = 1; i < 50; i++)
                {
                    if (mmxN < val[0][i]) mmxN = val[0][i];
                    else if (mmnN > val[0][i]) mmnN = val[0][i];
                }
                ceroN = (int)((mmxN + mmnN) / 2.0);

                mmxE = val[1][0];
                mmnE = mmxE;
                for (i = 1; i < 50; i++)
                {
                    if (mmxE < val[1][i]) mmxE = val[1][i];
                    else if (mmnE > val[1][i]) mmnE = val[1][i];
                }
                ceroE = (int)((mmxE + mmnE) / 2.0);

                mmxZ = val[2][0];
                mmnZ = mmxZ;
                for (i = 1; i < 50; i++)
                {
                    if (mmxZ < val[2][i]) mmxZ = val[2][i];
                    else if (mmnZ > val[2][i]) mmnZ = val[2][i];
                }
                ceroZ = (int)((mmxZ + mmnZ) / 2.0);
                sicerompt = true;
            }

            panel[0] = panelPartiTraN;
            panel[1] = panelPartiTraE;
            panel[2] = panelPartiTraZ;

            for (i = 0; i < 3; i++)
            {
                if (i == 0) idt = N;
                else if (i == 1) idt = E;
                else idt = Z;

                Graphics dc = panel[i].CreateGraphics();
                Pen lapiz = new Pen(Color.Black, 1);
                Pen lapRed = new Pen(Color.Red, 1);
                lapRed.DashStyle = DashStyle.DashDot;
                Pen lapViolet = new Pen(Color.Violet, 1);
                Pen lapBlueViolet = new Pen(Color.BlueViolet, 1);
                lapRed.DashStyle = DashStyle.DashDot;
                dat = new Point[numu[i]];
                k = 0;
                for (j = 0; j < numu[i]; j++)
                {
                    x = (int)(40.0 + (timsplintp[j] - timsplintp[0]) * fax);
                    y = (int)((5.0) + (double)(yf) - (val[i][j] - mn[i]) * fay);
                    va[i][k] = (int)(val[i][j]);
                    dat[k].Y = y;
                    dat[k++].X = x;
                }

                dc.DrawLines(lapiz, dat);
                lapiz.Dispose();

                SolidBrush bro = new SolidBrush(Color.Red);
                dc.DrawString(est[idt].Substring(0, 4), new Font("Times New Roman", 9, FontStyle.Bold), bro, 2, (int)(yf / 2.0));
                bro.Dispose();

                if (i == 0) cero = ceroN;
                else if (i == 1) cero = ceroE;
                else cero = ceroZ;
                x = 40;
                y = (int)((5.0) + yf - (cero - mn[i]) * fay);
                dc.DrawLine(lapRed, x, y, panel[i].Width - 10, y);

                x = (int)(40.0 + (timspl[mi[i] + muimpt] - timspl[mi[i]]) * fax);
                dc.DrawLine(lapViolet, x, 0, x, panel[i].Height);
                if (mufmpt > 0)
                {
                    x = (int)(40.0 + (timspl[mi[i] + mufmpt] - timspl[mi[i]]) * fax);
                    dc.DrawLine(lapBlueViolet, x, 0, x, panel[i].Height);
                }

                lapRed.Dispose();
                lapViolet.Dispose();
                lapBlueViolet.Dispose();
            }
            return;
        }
        /// <summary>
        /// Se encarga en determinar la porción de traza para cada una de sus componentes E, N y Z 
        /// (en caso de tenerlas) que se va a graficar en los paneles panelPartiTraN, panelPartiTraE 
        /// y panelPartiTraZ respectivamente.
        /// </summary>
        void TrazaComponente()
        {
            int i, j, k, xf, yf, x, y, idt, pico;
            int cero = 0;
            int mmxN, mmnN, mmxE, mmnE, mmxZ, mmnZ;
            int[] mi, mf, numu, mx, mn;
            double fax, fay;
            Panel[] panel = new Panel[3];
            Point[] dat;


            util.borra(panelPartiTraN, Color.White);
            util.borra(panelPartiTraE, Color.White);
            util.borra(panelPartiTraZ, Color.White);

            xf = panelPartiTraN.Size.Width - 50;
            yf = panelPartiTraN.Size.Height - 20;
            fax = xf / (double)(durmpt);

            mi = new int[3];
            mf = new int[3];
            numu = new int[3];
            mx = new int[3];
            mn = new int[3];

            for (i = 0; i < 3; i++)
            {
                if (i == 0)
                    idt = N;
                else if (i == 1)
                    idt = E;
                else
                    idt = Z;
                if (sifilt == false)
                {
                    mi[i] = (int)((timpt - tim[idt][0]) * ra[idt]) + suma;
                }
                else
                {
                    mi[i] = (int)((timpt - tie1) * ra[idt]) + suma;
                }
                if (mi[i] < 0)
                    mi[i] = 0;
                mimpt[i] = mi[i];
                mf[i] = (int)(mi[i] + (durmpt * ra[idt]));
                if (mf[i] > cu[idt].Length)
                    mf[i] = cu[idt].Length;
                numu[i] = mf[i] - mi[i];
                if (numu[i] < 10)
                    return;

                if (sifilt == false)
                {
                    mx[i] = cu[idt][mi[i]];
                    mn[i] = mx[i];
                    for (j = mi[i] + 1; j < mf[i]; j++)
                    {
                        if (mx[i] < cu[idt][j])
                            mx[i] = cu[idt][j];
                        else if (mn[i] > cu[idt][j])
                            mn[i] = cu[idt][j];
                    }
                }
                else
                {
                    mx[i] = cff[idt][mi[i]];
                    mn[i] = mx[i];
                    for (j = mi[i] + 1; j < mf[i]; j++)
                    {
                        if (mx[i] < cff[idt][j]) mx[i] = cff[idt][j];
                        else if (mn[i] > cff[idt][j]) mn[i] = cff[idt][j];
                    }
                }
            }

            for (i = 0; i < 3; i++)
            {
                va[i] = new int[numu[i]];
                mnmpt[i] = mn[i];
            }

            pico = mx[0] - mn[0];
            for (i = 1; i < 3; i++)
                if (pico < (mx[i] - mn[i]))
                    pico = mx[i] - mn[i];
            //fay = ampmpt * (yf / (double)(pico));
            fay = (yf / (double)(pico));
            faympt = fay;

            if (sicerompt == false)
            {
                if (sifilt == false)
                {
                    mmxN = cu[N][mi[0]];
                    mmnN = mmxN;
                    for (i = mi[0] + 1; i < mi[0] + 50; i++)
                    {
                        if (mmxN < cu[N][i])
                            mmxN = cu[N][i];
                        else if (mmnN > cu[N][i])
                            mmnN = cu[N][i];
                    }
                    ceroN = (int)((mmxN + mmnN) / 2.0);

                    mmxE = cu[E][mi[1]];
                    mmnE = mmxE;
                    for (i = mi[1] + 1; i < mi[1] + 50; i++)
                    {
                        if (mmxE < cu[E][i])
                            mmxE = cu[E][i];
                        else if (mmnE > cu[E][i])
                            mmnE = cu[E][i];
                    }
                    ceroE = (int)((mmxE + mmnE) / 2.0);

                    mmxZ = cu[Z][mi[1]];
                    mmnZ = mmxZ;
                    for (i = mi[1] + 1; i < mi[1] + 50; i++)
                    {
                        if (mmxZ < cu[Z][i]) mmxZ = cu[Z][i];
                        else if (mmnZ > cu[Z][i]) mmnZ = cu[Z][i];
                    }
                    ceroZ = (int)((mmxZ + mmnZ) / 2.0);
                }
                else
                {
                    mmxN = cff[N][mi[0]];
                    mmnN = mmxN;
                    for (i = mi[0] + 1; i < mi[0] + 50; i++)
                    {
                        if (mmxN < cff[N][i]) mmxN = cff[N][i];
                        else if (mmnN > cff[N][i]) mmnN = cff[N][i];
                    }
                    ceroN = (int)((mmxN + mmnN) / 2.0);

                    mmxE = cff[E][mi[1]];
                    mmnE = mmxE;
                    for (i = mi[1] + 1; i < mi[1] + 50; i++)
                    {
                        if (mmxE < cff[E][i]) mmxE = cff[E][i];
                        else if (mmnE > cff[E][i]) mmnE = cff[E][i];
                    }
                    ceroE = (int)((mmxE + mmnE) / 2.0);

                    mmxZ = cff[Z][mi[1]];
                    mmnZ = mmxZ;
                    for (i = mi[1] + 1; i < mi[1] + 50; i++)
                    {
                        if (mmxZ < cff[Z][i]) mmxZ = cff[Z][i];
                        else if (mmnZ > cff[Z][i]) mmnZ = cff[Z][i];
                    }
                    ceroZ = (int)((mmxZ + mmnZ) / 2.0);
                }

                sicerompt = true;
            }

            panel[0] = panelPartiTraN;
            panel[1] = panelPartiTraE;
            panel[2] = panelPartiTraZ;

            for (i = 0; i < 3; i++)
            {
                if (i == 0) idt = N;
                else if (i == 1) idt = E;
                else idt = Z;

                Graphics dc = panel[i].CreateGraphics();
                Pen lapiz = new Pen(Color.Black, 1);
                Pen lapRed = new Pen(Color.Red, 1);
                lapRed.DashStyle = DashStyle.DashDot;
                Pen lapViolet = new Pen(Color.Violet, 1);
                Pen lapBlueViolet = new Pen(Color.BlueViolet, 1);
                lapRed.DashStyle = DashStyle.DashDot;
                dat = new Point[numu[i]];
                k = 0;
                if (sifilt == false)
                {
                    for (j = mi[i]; j < mf[i]; j++)
                    {
                        x = (int)(40.0 + (tim[idt][j] - tim[idt][mi[i]]) * fax);
                        y = (int)((5.0) + yf - (cu[idt][j] - mn[i]) * fay);
                        va[i][k] = cu[idt][j];
                        dat[k].Y = y;
                        dat[k++].X = x;
                    }
                }
                else
                {
                    for (j = mi[i]; j < mf[i]; j++)
                    {
                        x = (int)(40.0 + (tim[idt][j] - tim[idt][mi[i]]) * fax);
                        y = (int)((5.0) + yf - (cff[idt][j] - mn[i]) * fay);
                        va[i][k] = cff[idt][j];
                        dat[k].Y = y;
                        dat[k++].X = x;
                    }
                }
                dc.DrawLines(lapiz, dat);
                lapiz.Dispose();

                SolidBrush bro = new SolidBrush(Color.Red);
                dc.DrawString(est[idt].Substring(0, 4), new Font("Times New Roman", 9, FontStyle.Bold), bro, 2, (int)(yf / 2.0));
                bro.Dispose();

                if (i == 0)
                    cero = ceroN;
                else if (i == 1)
                    cero = ceroE;
                else
                    cero = ceroZ;
                x = 40;
                y = (int)((5.0) + yf - (cero - mn[i]) * fay);
                dc.DrawLine(lapRed, x, y, panel[i].Width - 10, y);

                x = (int)(40.0 + (tim[idt][mi[i] + muimpt] - tim[idt][mi[i]]) * fax);
                dc.DrawLine(lapViolet, x, 0, x, panel[i].Height);
                if (mufmpt > 0)
                {
                    x = (int)(40.0 + (tim[idt][mi[i] + mufmpt] - tim[idt][mi[i]]) * fax);
                    dc.DrawLine(lapBlueViolet, x, 0, x, panel[i].Height);
                }

                lapRed.Dispose();
                lapViolet.Dispose();
                lapBlueViolet.Dispose();
            }

            return;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vN"></param>
        /// <param name="vE"></param>
        /// <param name="vZ"></param>
        /// <param name="e"></param>
        void MoverNEZ(int[] vN, int[] vE, int[] vZ, DoWorkEventArgs e)
        {
            int i, idd, j, k, mxN, mnN, mxE, mnE, mxZ, mnZ, yf;
            int mmxN, mmxE, mmxZ, mmxNE, mmxNZ, mmxEZ;
            int x = 0, y = 0, x1, y1, x2, y2, x3, y3, fin;
            int xx1 = 0, yy1 = 0, xx2 = 0, yy2 = 0, xx3 = 0, yy3 = 0;
            int[] valN, valE, valZ;
            double facNE, facNZ, facEZ, fac, fax;
            Panel[] panel = new Panel[3];

            if (vN.Length != vE.Length) return;

            yf = panelPartiTraN.Size.Height - 20;
            fax = (panelPartiTraN.Size.Width - 50.0) / durmpt;

            panel[0] = panelPartiTraN;
            panel[1] = panelPartiTraE;
            panel[2] = panelPartiTraZ;

            valN = new int[vN.Length];
            valE = new int[vN.Length];
            valZ = new int[vN.Length];

            if (sicerompt == false)
            {
                mxN = vN[0];
                mnN = mxN;
                for (i = 1; i < 50; i++)
                {
                    if (mxN < vN[i]) mxN = vN[i];
                    else if (mnN > vN[i]) mnN = vN[i];
                }
                ceroN = (int)((mxN + mnN) / 2.0);

                mxE = vE[0];
                mnE = mxE;
                for (i = 1; i < 50; i++)
                {
                    if (mxE < vE[i]) mxE = vE[i];
                    else if (mnE > vE[i]) mnE = vE[i];
                }
                ceroE = (int)((mxE + mnE) / 2.0);

                mxZ = vZ[0];
                mnZ = mxZ;
                for (i = 1; i < 50; i++)
                {
                    if (mxZ < vZ[i]) mxZ = vZ[i];
                    else if (mnZ > vZ[i]) mnZ = vZ[i];
                }
                ceroZ = (int)((mxZ + mnZ) / 2.0);
                sicerompt = true;
            }

            mmxN = vN[0] - ceroN;
            mmxE = vE[0] - ceroE;
            for (i = 0; i < vN.Length; i++)
            {
                valN[i] = vN[i] - ceroN;
                valE[i] = vE[i] - ceroE;
                if (mmxN < Math.Abs(valN[i])) mmxN = Math.Abs(valN[i]);
                if (mmxE < Math.Abs(valE[i])) mmxE = Math.Abs(valE[i]);
            }
            mmxNE = mmxN;
            if (mmxNE < mmxE) mmxNE = mmxE;
            facNE = (panelPartiEN.Width / 3.0) / (double)(mmxNE);

            mmxN = vN[0] - ceroN;
            mmxZ = vZ[0] - ceroZ;
            for (i = 0; i < vN.Length; i++)
            {
                valN[i] = vN[i] - ceroN;
                valZ[i] = vZ[i] - ceroZ;
                if (mmxN < Math.Abs(valN[i])) mmxN = Math.Abs(valN[i]);
                if (mmxZ < Math.Abs(valZ[i])) mmxZ = Math.Abs(valZ[i]);
            }
            mmxNZ = mmxN;
            if (mmxNZ < mmxZ) mmxNZ = mmxZ;
            facNZ = (panelPartiNZ.Width / 3.0) / (double)(mmxNZ);

            mmxE = vE[0] - ceroE;
            mmxZ = vZ[0] - ceroZ;
            for (i = 0; i < vE.Length; i++)
            {
                valE[i] = vE[i] - ceroE;
                valZ[i] = vZ[i] - ceroZ;
                if (mmxE < Math.Abs(valE[i])) mmxE = Math.Abs(valE[i]);
                if (mmxZ < Math.Abs(valZ[i])) mmxZ = Math.Abs(valZ[i]);
            }
            mmxEZ = mmxE;
            if (mmxEZ < mmxZ) mmxEZ = mmxZ;
            facEZ = (panelPartiEZ.Width / 3.0) / (double)(mmxEZ);

            //fac es el factor pra el dibujo del movimiento
            fac = facNE;
            if (fac > facNZ) fac = facNZ;
            if (fac > facEZ) fac = facEZ;

            Graphics dc = panelPartiEN.CreateGraphics();
            Graphics dc2 = panelPartiNZ.CreateGraphics();
            Graphics dc3 = panelPartiEZ.CreateGraphics();
            Pen lapiz = new Pen(Color.DarkRed, 1);
            Pen lap = new Pen(Color.Black, 1);
            Pen la2 = new Pen(Color.LightSeaGreen, 1);
            Pen laT = new Pen(Color.DarkOrange, 1);

            x1 = (int)(panelPartiEN.Width / 2.0) + (int)(valE[muimpt] * facNE);
            y1 = (int)(panelPartiEN.Height / 2.0) - (int)(valN[muimpt] * facNE);
            x2 = (int)(panelPartiNZ.Width / 2.0) + (int)(valN[muimpt] * fac);
            y2 = (int)(panelPartiNZ.Height / 2.0) - (int)(valZ[muimpt] * fac);
            x3 = (int)(panelPartiEZ.Width / 2.0) + (int)(valE[muimpt] * fac);
            y3 = (int)(panelPartiEZ.Height / 2.0) - (int)(valZ[muimpt] * fac);
            dc.DrawEllipse(la2, x1 - 2, y1 - 2, 4, 4);

            if (mufmpt > 0)
            {
                if (mufmpt > valN.Length) mufmpt = valN.Length;
                fin = mufmpt;
            }
            else fin = valN.Length;

            if (puntompt == true)
            {
                try
                {
                    for (i = muimpt; i < fin; i++)
                    {
                        dc.DrawEllipse(la2, x1 - 2, y1 - 2, 4, 4);
                        x = (int)(panelPartiEN.Width / 2.0) + (int)(valE[i] * fac);
                        y = (int)(panelPartiEN.Height / 2.0) - (int)(valN[i] * fac);
                        dc.DrawEllipse(lapiz, x - 2, y - 2, 4, 4);
                        x1 = x;
                        y1 = y;

                        dc2.DrawEllipse(la2, x2 - 2, y2 - 2, 4, 4);
                        x = (int)(panelPartiNZ.Width / 2.0) + (int)(valN[i] * fac);
                        y = (int)(panelPartiNZ.Height / 2.0) - (int)(valZ[i] * fac);
                        dc2.DrawEllipse(lapiz, x - 2, y - 2, 4, 4);
                        x2 = x;
                        y2 = y;

                        dc3.DrawEllipse(la2, x3 - 2, y3 - 2, 4, 4);
                        x = (int)(panelPartiEZ.Width / 2.0) + (int)(valE[i] * fac);
                        y = (int)(panelPartiEZ.Height / 2.0) - (int)(valZ[i] * fac);
                        dc3.DrawEllipse(lapiz, x - 2, y - 2, 4, 4);
                        x3 = x;
                        y3 = y;

                        try
                        {
                            if (sifilt == false)
                            {
                                for (j = 0; j < 3; j++)
                                {
                                    Graphics dcc = panel[j].CreateGraphics();
                                    if (j == 0) idd = N;
                                    else if (j == 1) idd = E;
                                    else idd = Z;
                                    k = i + mimpt[j];
                                    x = (int)(40.0 + (tim[idd][k] - tim[idd][mimpt[j]]) * fax);
                                    y = (int)((5.0) + yf - (cu[idd][k] - mnmpt[j]) * faympt);
                                    dcc.DrawEllipse(laT, x - 2, y - 2, 3, 3);
                                }
                            }
                            else
                            {
                                for (j = 0; j < 3; j++)
                                {
                                    Graphics dcc = panel[j].CreateGraphics();
                                    if (j == 0) idd = N;
                                    else if (j == 1) idd = E;
                                    else idd = Z;
                                    k = i + mimpt[j];
                                    x = (int)(40.0 + (tim[idd][k] - tim[idd][mimpt[j]]) * fax);
                                    y = (int)((5.0) + yf - (cff[idd][k] - mnmpt[j]) * faympt);
                                    dcc.DrawEllipse(laT, x - 2, y - 2, 3, 3);
                                }
                            }
                        }
                        catch
                        {
                            break;
                        }

                        System.Threading.Thread.Sleep(velompt);
                        if (pausmpt == true)
                        {
                            muimpt = i;
                            break;
                        }
                        if (backgroundWorker1.CancellationPending)
                        {
                            e.Cancel = true;
                            break;
                        }
                    }
                }
                catch
                {
                    return;
                }
            }
            else
            {
                for (i = muimpt; i < fin; i++)
                {
                    x = (int)(panelPartiEN.Width / 2.0) + (int)(valE[i] * fac);
                    y = (int)(panelPartiEN.Height / 2.0) - (int)(valN[i] * fac);
                    dc.DrawLine(lapiz, x, y, x1, y1);
                    if (i > muimpt) dc.DrawLine(la2, x1, y1, xx1, yy1);
                    xx1 = x1;
                    yy1 = y1;
                    x1 = x;
                    y1 = y;

                    x = (int)(panelPartiNZ.Width / 2.0) + (int)(valN[i] * fac);
                    y = (int)(panelPartiNZ.Height / 2.0) - (int)(valZ[i] * fac);
                    dc2.DrawLine(lapiz, x, y, x2, y2);
                    if (i > muimpt) dc2.DrawLine(la2, x2, y2, xx2, yy2);
                    xx2 = x2;
                    yy2 = y2;
                    x2 = x;
                    y2 = y;

                    x = (int)(panelPartiEZ.Width / 2.0) + (int)(valE[i] * fac);
                    y = (int)(panelPartiEZ.Height / 2.0) - (int)(valZ[i] * fac);
                    dc3.DrawLine(lapiz, x, y, x3, y3);
                    if (i > muimpt) dc3.DrawLine(la2, x3, y3, xx3, yy3);
                    xx3 = x3;
                    yy3 = y3;
                    x3 = x;
                    y3 = y;

                    if (sifilt == false)
                    {
                        for (j = 0; j < 3; j++)
                        {
                            Graphics dcc = panel[j].CreateGraphics();
                            if (j == 0) idd = N;
                            else if (j == 1) idd = E;
                            else idd = Z;
                            k = i + mimpt[j];
                            x = (int)(40.0 + (tim[idd][k] - tim[idd][mimpt[j]]) * fax);
                            y = (int)((5.0) + yf - (cu[idd][k] - mnmpt[j]) * faympt);
                            dcc.DrawEllipse(laT, x - 2, y - 2, 3, 3);
                        }
                    }
                    else
                    {
                        for (j = 0; j < 3; j++)
                        {
                            Graphics dcc = panel[j].CreateGraphics();
                            if (j == 0) idd = N;
                            else if (j == 1) idd = E;
                            else idd = Z;
                            k = i + mimpt[j];
                            x = (int)(40.0 + (tim[idd][k] - tim[idd][mimpt[j]]) * fax);
                            y = (int)((5.0) + yf - (cff[idd][k] - mnmpt[j]) * faympt);
                            dcc.DrawEllipse(laT, x - 2, y - 2, 3, 3);
                        }
                    }

                    System.Threading.Thread.Sleep(velompt);
                    if (pausmpt == true)
                    {
                        muimpt = i;
                        break;
                    }
                    if (backgroundWorker1.CancellationPending)
                    {
                        e.Cancel = true;
                        break;
                    }
                }
            }
            lapiz.Dispose();
            lap.Dispose();
            la2.Dispose();
            laT.Dispose();

            boStartMpt.BackColor = Color.MistyRose;

            return;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vN"></param>
        /// <param name="vE"></param>
        /// <param name="vZ"></param>
        /// <param name="e"></param>
        void MoverNEZinterp(int[] vN, int[] vE, int[] vZ, DoWorkEventArgs e)
        {
            int i, idd, j, k, mxN, mnN, mxE, mnE, mxZ, mnZ, yf;
            int mmxN, mmxE, mmxZ, mmxNE, mmxNZ, mmxEZ;
            int x = 0, y = 0, x1, y1, x2, y2, x3, y3, fin;
            int xx1 = 0, yy1 = 0, xx2 = 0, yy2 = 0, xx3 = 0, yy3 = 0;
            int[] valN, valE, valZ;
            double facNE, facNZ, facEZ, fac, fax;
            Panel[] panel = new Panel[3];

            if (vN.Length != vE.Length) return;

            yf = panelPartiTraN.Size.Height - 20;
            fax = (panelPartiTraN.Size.Width - 50.0) / durmpt;

            panel[0] = panelPartiTraN;
            panel[1] = panelPartiTraE;
            panel[2] = panelPartiTraZ;

            valN = new int[vN.Length];
            valE = new int[vN.Length];
            valZ = new int[vN.Length];

            if (sicerompt == false)
            {
                mxN = vN[0];
                mnN = mxN;
                for (i = 1; i < 50; i++)
                {
                    if (mxN < vN[i]) mxN = vN[i];
                    else if (mnN > vN[i]) mnN = vN[i];
                }
                ceroN = (int)((mxN + mnN) / 2.0);

                mxE = vE[0];
                mnE = mxE;
                for (i = 1; i < 50; i++)
                {
                    if (mxE < vE[i]) mxE = vE[i];
                    else if (mnE > vE[i]) mnE = vE[i];
                }
                ceroE = (int)((mxE + mnE) / 2.0);

                mxZ = vZ[0];
                mnZ = mxZ;
                for (i = 1; i < 50; i++)
                {
                    if (mxZ < vZ[i]) mxZ = vZ[i];
                    else if (mnZ > vZ[i]) mnZ = vZ[i];
                }
                ceroZ = (int)((mxZ + mnZ) / 2.0);
                sicerompt = true;
            }

            mmxN = vN[0] - ceroN;
            mmxE = vE[0] - ceroE;
            for (i = 0; i < vN.Length; i++)
            {
                valN[i] = vN[i] - ceroN;
                valE[i] = vE[i] - ceroE;
                if (mmxN < Math.Abs(valN[i])) mmxN = Math.Abs(valN[i]);
                if (mmxE < Math.Abs(valE[i])) mmxE = Math.Abs(valE[i]);
            }
            mmxNE = mmxN;
            if (mmxNE < mmxE) mmxNE = mmxE;
            facNE = (panelPartiEN.Width / 3.0) / (double)(mmxNE);

            mmxN = vN[0] - ceroN;
            mmxZ = vZ[0] - ceroZ;
            for (i = 0; i < vN.Length; i++)
            {
                valN[i] = vN[i] - ceroN;
                valZ[i] = vZ[i] - ceroZ;
                if (mmxN < Math.Abs(valN[i])) mmxN = Math.Abs(valN[i]);
                if (mmxZ < Math.Abs(valZ[i])) mmxZ = Math.Abs(valZ[i]);
            }
            mmxNZ = mmxN;
            if (mmxNZ < mmxZ) mmxNZ = mmxZ;
            facNZ = (panelPartiNZ.Width / 3.0) / (double)(mmxNZ);

            mmxE = vE[0] - ceroE;
            mmxZ = vZ[0] - ceroZ;
            for (i = 0; i < vE.Length; i++)
            {
                valE[i] = vE[i] - ceroE;
                valZ[i] = vZ[i] - ceroZ;
                if (mmxE < Math.Abs(valE[i])) mmxE = Math.Abs(valE[i]);
                if (mmxZ < Math.Abs(valZ[i])) mmxZ = Math.Abs(valZ[i]);
            }
            mmxEZ = mmxE;
            if (mmxEZ < mmxZ) mmxEZ = mmxZ;
            facEZ = (panelPartiEZ.Width / 3.0) / (double)(mmxEZ);

            //fac es el factor pra el dibujo del movimiento
            fac = facNE;
            if (fac > facNZ) fac = facNZ;
            if (fac > facEZ) fac = facEZ;

            Graphics dc = panelPartiEN.CreateGraphics();
            Graphics dc2 = panelPartiNZ.CreateGraphics();
            Graphics dc3 = panelPartiEZ.CreateGraphics();
            Pen lapiz = new Pen(Color.DarkRed, 1);
            Pen lap = new Pen(Color.Black, 1);
            Pen la2 = new Pen(Color.LightSeaGreen, 1);
            Pen laT = new Pen(Color.DarkOrange, 1);

            x1 = (int)(panelPartiEN.Width / 2.0) + (int)(valE[muimpt] * facNE);
            y1 = (int)(panelPartiEN.Height / 2.0) - (int)(valN[muimpt] * facNE);
            x2 = (int)(panelPartiNZ.Width / 2.0) + (int)(valN[muimpt] * fac);
            y2 = (int)(panelPartiNZ.Height / 2.0) - (int)(valZ[muimpt] * fac);
            x3 = (int)(panelPartiEZ.Width / 2.0) + (int)(valE[muimpt] * fac);
            y3 = (int)(panelPartiEZ.Height / 2.0) - (int)(valZ[muimpt] * fac);
            dc.DrawEllipse(la2, x1 - 2, y1 - 2, 4, 4);

            if (mufmpt > 0)
            {
                if (mufmpt > valN.Length) mufmpt = valN.Length;
                fin = mufmpt;
            }
            else fin = valN.Length;

            if (puntompt == true)
            {
                try
                {
                    for (i = muimpt; i < fin; i++)
                    {
                        dc.DrawEllipse(la2, x1 - 2, y1 - 2, 4, 4);
                        x = (int)(panelPartiEN.Width / 2.0) + (int)(valE[i] * fac);
                        y = (int)(panelPartiEN.Height / 2.0) - (int)(valN[i] * fac);
                        dc.DrawEllipse(lapiz, x - 2, y - 2, 4, 4);
                        x1 = x;
                        y1 = y;

                        dc2.DrawEllipse(la2, x2 - 2, y2 - 2, 4, 4);
                        x = (int)(panelPartiNZ.Width / 2.0) + (int)(valN[i] * fac);
                        y = (int)(panelPartiNZ.Height / 2.0) - (int)(valZ[i] * fac);
                        dc2.DrawEllipse(lapiz, x - 2, y - 2, 4, 4);
                        x2 = x;
                        y2 = y;

                        dc3.DrawEllipse(la2, x3 - 2, y3 - 2, 4, 4);
                        x = (int)(panelPartiEZ.Width / 2.0) + (int)(valE[i] * fac);
                        y = (int)(panelPartiEZ.Height / 2.0) - (int)(valZ[i] * fac);
                        dc3.DrawEllipse(lapiz, x - 2, y - 2, 4, 4);
                        x3 = x;
                        y3 = y;

                        try
                        {
                            for (j = 0; j < 3; j++)
                            {
                                Graphics dcc = panel[j].CreateGraphics();
                                if (j == 0) idd = N;
                                else if (j == 1) idd = E;
                                else idd = Z;
                                x = (int)(40.0 + (timsplintp[i] - timsplintp[0]) * fax);
                                y = (int)((5.0) + yf - (va[j][i] - mnmpt[j]) * faympt);
                                dcc.DrawEllipse(laT, x - 2, y - 2, 3, 3);
                            }
                        }
                        catch
                        {
                            //MessageBox.Show("**** x="+x.ToString()+" y="+y.ToString()+" mimpt="+mimpt[1].ToString()+" va.len="+va[0].Length.ToString()+" timspl.len="+timspl.Length+" muimpt="+muimpt.ToString()+" i="+i.ToString()+" valE.len="+valE.Length.ToString()+" vE.len="+vE.Length.ToString());
                            break;
                        }

                        System.Threading.Thread.Sleep(velompt);
                        if (pausmpt == true)
                        {
                            muimpt = i;
                            break;
                        }
                        if (backgroundWorker1.CancellationPending)
                        {
                            e.Cancel = true;
                            break;
                        }
                    }
                }
                catch
                {
                    return;
                }
            }
            else
            {
                for (i = muimpt; i < fin; i++)
                {
                    x = (int)(panelPartiEN.Width / 2.0) + (int)(valE[i] * fac);
                    y = (int)(panelPartiEN.Height / 2.0) - (int)(valN[i] * fac);
                    dc.DrawLine(lapiz, x, y, x1, y1);
                    if (i > muimpt) dc.DrawLine(la2, x1, y1, xx1, yy1);
                    xx1 = x1;
                    yy1 = y1;
                    x1 = x;
                    y1 = y;

                    x = (int)(panelPartiNZ.Width / 2.0) + (int)(valN[i] * fac);
                    y = (int)(panelPartiNZ.Height / 2.0) - (int)(valZ[i] * fac);
                    dc2.DrawLine(lapiz, x, y, x2, y2);
                    if (i > muimpt) dc2.DrawLine(la2, x2, y2, xx2, yy2);
                    xx2 = x2;
                    yy2 = y2;
                    x2 = x;
                    y2 = y;

                    x = (int)(panelPartiEZ.Width / 2.0) + (int)(valE[i] * fac);
                    y = (int)(panelPartiEZ.Height / 2.0) - (int)(valZ[i] * fac);
                    dc3.DrawLine(lapiz, x, y, x3, y3);
                    if (i > muimpt) dc3.DrawLine(la2, x3, y3, xx3, yy3);
                    xx3 = x3;
                    yy3 = y3;
                    x3 = x;
                    y3 = y;

                    try
                    {
                        for (j = 0; j < 3; j++)
                        {
                            Graphics dcc = panel[j].CreateGraphics();
                            if (j == 0) idd = N;
                            else if (j == 1) idd = E;
                            else idd = Z;
                            x = (int)(40.0 + (timsplintp[i] - timsplintp[0]) * fax);
                            y = (int)((5.0) + yf - (va[j][i] - mnmpt[j]) * faympt);
                            dcc.DrawEllipse(laT, x - 2, y - 2, 3, 3);
                        }
                    }
                    catch
                    {
                        //MessageBox.Show("**** x="+x.ToString()+" y="+y.ToString()+" mimpt="+mimpt[1].ToString()+" va.len="+va[0].Length.ToString()+" timspl.len="+timspl.Length+" muimpt="+muimpt.ToString()+" i="+i.ToString()+" valE.len="+valE.Length.ToString()+" vE.len="+vE.Length.ToString());
                        break;
                    }

                    System.Threading.Thread.Sleep(velompt);
                    if (pausmpt == true)
                    {
                        muimpt = i;
                        break;
                    }
                    if (backgroundWorker1.CancellationPending)
                    {
                        e.Cancel = true;
                        break;
                    }
                }
            }
            lapiz.Dispose();
            lap.Dispose();
            la2.Dispose();
            laT.Dispose();

            boStartMpt.BackColor = Color.MistyRose;

            return;
        }
        /// <summary>
        /// Es el que marca el inicio del cálculo de movimiento de partículas y desencadena la animación del mismo.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void boStartMpt_Click(object sender, EventArgs e)
        {
            int x, x2, y, k;

            if (modX == false) k = volmpt;
            else
                k = nuvol;
            NoMostrar = true;
            util.VerMapa(panelPartiEN, volcan[k][0], laE, loE, "", difmpt, laE, loE, Color.LightGray);
            if (mptintp == false)
                TrazaComponente();
            else
                TrazaComponenteInterp();
            Graphics dc2 = panelPartiNZ.CreateGraphics();
            Graphics dc3 = panelPartiEZ.CreateGraphics();
            Pen lap = new Pen(Color.Black, 1);
            Pen lap2 = new Pen(Color.Red, 1);
            SolidBrush bro = new SolidBrush(Color.White);
            dc2.FillRectangle(bro, 0, 0, panelPartiNZ.Width, panelPartiNZ.Height);
            dc3.FillRectangle(bro, 0, 0, panelPartiEZ.Width, panelPartiEZ.Height);
            bro.Dispose();
            SolidBrush bro2 = new SolidBrush(Color.Black);
            x = (int)(panelPartiNZ.Width / 3.0);
            y = (int)(panelPartiNZ.Height / 2.0);
            x2 = x * 2;
            dc2.DrawLine(lap, x, y, x2, y);
            dc2.DrawString("N", new Font("Arial", 9, FontStyle.Bold), bro2, panelPartiNZ.Width - 20, y - 6);
            dc2.DrawRectangle(lap2, (int)(panelPartiNZ.Width / 2.0) - 3, y - 3, 6, 6);
            x = (int)(panelPartiEZ.Width / 3.0);
            y = (int)(panelPartiEZ.Height / 2.0);
            x2 = x * 2;
            dc3.DrawLine(lap, x, y, x2, y);
            dc3.DrawString("E", new Font("Arial", 9, FontStyle.Bold), bro2, panelPartiEZ.Width - 20, y - 6);
            dc3.DrawRectangle(lap2, (int)(panelPartiEZ.Width / 2.0) - 3, y - 3, 6, 6);
            lap.Dispose();
            lap2.Dispose();
            bro2.Dispose();

            boStartMpt.BackColor = Color.DeepPink;

            try
            {
                backgroundWorker1.RunWorkerAsync();
            }
            catch
            {
                return;
            }
        }
        /// <summary>
        /// Detiene el cálculo de movimiento de partículas y la animación que lo describe.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void boStopMpt_Click(object sender, EventArgs e)
        {
            muimpt = muinimpt;
            backgroundWorker1.CancelAsync();
            pausmpt = false;
            boPausMpt.BackColor = Color.PaleGoldenrod;
        }
        /// <summary>
        /// Pausa el cálculo de movimiento de partículas, cuando se de click sobre el botón start se continua desde el punto donde se pauso,
        /// pero la animación borra la grafica del cálculo de los puntos anteriores.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void boPausMpt_Click(object sender, EventArgs e)
        {
            if (pausmpt == false)
            {
                pausmpt = true;
                boPausMpt.BackColor = Color.Red;
            }
            else
            {
                pausmpt = false;
                boPausMpt.BackColor = Color.PaleGoldenrod;
                try
                {
                    backgroundWorker1.RunWorkerAsync();
                }
                catch
                {
                    return;
                }
            }
        }
        /// <summary>
        /// Aumenta la amplitud utilizada en el cálculo de movimiento de partículas.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void boAumMpt_MouseDown(object sender, MouseEventArgs e)
        {
            string ca = "";

            if (e.Button == MouseButtons.Left)
            {
                velompt += 5;
            }
            else
            {
                velompt += 50;
            }
            ca = velompt.ToString() + " ms";
            Graphics dc = panelDatosMpt.CreateGraphics();
            SolidBrush br = new SolidBrush(panelDatosMpt.BackColor);
            dc.FillRectangle(br, 0, 0, panelDatosMpt.Width, panelDatosMpt.Height);
            br.Dispose();
            SolidBrush bro = new SolidBrush(Color.Black);
            dc.DrawString(ca, new Font("Arial", 9/*,FontStyle.Bold*/), bro, 5, 3);
            bro.Dispose();
        }
        /// <summary>
        /// Disminuye la amplitud utilizada en el cálculo de movimiento de partículas.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void boDisMpt_MouseDown(object sender, MouseEventArgs e)
        {
            string ca = "";

            if (e.Button == MouseButtons.Left)
            {
                velompt -= 5;
            }
            else
            {
                velompt -= 50;
            }
            if (velompt < 0) velompt = 0;
            ca = velompt.ToString() + " ms";
            Graphics dc = panelDatosMpt.CreateGraphics();
            SolidBrush br = new SolidBrush(panelDatosMpt.BackColor);
            dc.FillRectangle(br, 0, 0, panelDatosMpt.Width, panelDatosMpt.Height);
            br.Dispose();
            SolidBrush bro = new SolidBrush(Color.Black);
            dc.DrawString(ca, new Font("Arial", 9/*,FontStyle.Bold*/), bro, 5, 3);
            bro.Dispose();
        }
        /// <summary>
        /// Modifica el valor de la variable durmpt haciendola = 2.0F en caso de que el click sea con el botón izquierdo del mouse,
        /// o si es con el derecho y su valor actual es mayor de 2.0F, en caso de que se de el click con el botón derecho y que su
        /// valor sea menor que 2.0F y mayor que 0.5F le resta 0.5F y grafica las trazas involucradas en el movimiento de particulas.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void bo2Mpt_MouseDown(object sender, MouseEventArgs e)
        {
            string ca;

            if (e.Button == MouseButtons.Left) durmpt = 2.0F;
            else
            {
                if (durmpt > 2.0F)
                {
                    durmpt = 2.0F;
                    if (mptintp == false) 
                        TrazaComponente();
                    else
                        TrazaComponenteInterp();
                    return;
                }
                else if (durmpt > 0.5F) durmpt -= 0.5F;
                else if (durmpt <= 0.5F)
                {
                    durmpt -= 0.1F;
                    if (durmpt < 0.1F) durmpt = 0.1F;
                }
            }
            bo2Mpt.BackColor = Color.Plum;
            bo5Mpt.BackColor = Color.White;
            bo10Mpt.BackColor = Color.White;
            ca = string.Format("{0:0.0}", durmpt);
            bo2Mpt.Text = ca;
            if (mptintp == false) TrazaComponente();
            else TrazaComponenteInterp();
        }
        /// <summary>
        /// Modifica el valor de la variable durmpt haciendola = 5.0F.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void bo5Mpt_Click(object sender, EventArgs e)
        {
            durmpt = 5.0F;
            bo2Mpt.BackColor = Color.White;
            bo5Mpt.BackColor = Color.Plum;
            bo10Mpt.BackColor = Color.White;
            if (mptintp == false) TrazaComponente();
            else TrazaComponenteInterp();
        }
        /// <summary>
        /// Modifica el valor de la variable durmpt haciendola = 10.0F.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void bo10Mpt_Click(object sender, EventArgs e)
        {
            durmpt = 10.0F;
            bo2Mpt.BackColor = Color.White;
            bo5Mpt.BackColor = Color.White;
            bo10Mpt.BackColor = Color.Plum;
            if (mptintp == false)
                TrazaComponente();
            else
                TrazaComponenteInterp();
        }

        private void boMasMpt_MouseDown(object sender, MouseEventArgs e)
        {
            //if (e.Button == MouseButtons.Left) ampmpt += 0.1F;
            //else ampmpt = 1.0F;
        }
        private void boMenosMpt_MouseDown(object sender, MouseEventArgs e)
        {
            //if (e.Button == MouseButtons.Left) ampmpt -= 0.1F;
            //else ampmpt = 1.0F;
        }

        /// <summary>
        /// Hace que cuando se grafique la animación del movimiento de particulas,
        /// se grafique con puntos.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void boPtoMpt_Click(object sender, EventArgs e)
        {
            puntompt = true;
            boPtoMpt.BackColor = Color.ForestGreen;
            boLinMpt.BackColor = Color.PaleGoldenrod;
        }
        /// <summary>
        /// Hace que cuando se grafique la animación del movimiento de particulas,
        /// se grafique con lineas.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void boLinMpt_Click(object sender, EventArgs e)
        {
            puntompt = false;
            boPtoMpt.BackColor = Color.Gold;
            boLinMpt.BackColor = Color.ForestGreen;
        }
        /// <summary>
        /// Cierra el panel donde se muestra el movimiento de particulas.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void boXparti_Click(object sender, EventArgs e)
        {
            yaInterp = false;
            suma = 0;
            panelParti.Visible = false;
            moverparti = false;
            particula = false;
            boMptIntp.BackColor = Color.LavenderBlush;
            boParti.BackColor = Color.White;
            muimpt = muinimpt;
            mufmpt = 0;
            pausmpt = false;
            boPausMpt.BackColor = Color.PaleGoldenrod;
            if (panelInterP.Visible == false)
                Clasificar();
        }
        /// <summary>
        /// Desplaza hacia arriba la linea roja que cruza la traza de la componente N en la grafica de movimiento de particulas.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void bosubNmpt_MouseDown(object sender, MouseEventArgs e)
        {
            double dd;
            dd = (ceroN - mnmpt[0]) / (panelPartiTraN.Height / 2.0);
            if (e.Button == MouseButtons.Left) ceroN += (int)(dd);
            else ceroN += 5 * (int)(dd);
            if (mptintp == false) TrazaComponente();
            else TrazaComponenteInterp();
        }
        /// <summary>
        /// Desplaza hacia abajo la linea roja que cruza la traza de la componente N en la grafica de movimiento de particulas. 
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void bobajNmpt_MouseDown(object sender, MouseEventArgs e)
        {
            double dd;
            dd = (ceroN - mnmpt[0]) / (panelPartiTraN.Height / 2.0);
            if (e.Button == MouseButtons.Left) ceroN -= (int)(dd);
            else ceroN -= 5 * (int)(dd);
            if (mptintp == false) TrazaComponente();
            else TrazaComponenteInterp();
        }
        /// <summary>
        /// Desplaza hacia arriba la linea roja que cruza la traza de la componente E en la grafica de movimiento de particulas. 
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void bosubEmpt_MouseDown(object sender, MouseEventArgs e)
        {
            double dd;
            dd = (ceroE - mnmpt[1]) / (panelPartiTraE.Height / 2.0);
            if (e.Button == MouseButtons.Left) ceroE += (int)(dd);
            else ceroE += 5 * (int)(dd);
            if (mptintp == false) TrazaComponente();
            else TrazaComponenteInterp();
        }
        /// <summary>
        /// Desplaza hacia abajo la linea roja que cruza la traza de la componente E en la grafica de movimiento de particulas. 
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void bobajaEmpt_MouseDown(object sender, MouseEventArgs e)
        {
            double dd;
            dd = (ceroE - mnmpt[1]) / (panelPartiTraE.Height / 2.0);
            if (e.Button == MouseButtons.Left) ceroE -= (int)(dd);
            else ceroE -= 5 * (int)(dd);
            if (mptintp == false) TrazaComponente();
            else TrazaComponenteInterp();
        }
        /// <summary>
        /// Desplaza hacia arriba la linea roja que cruza la traza de la componente Z en la grafica de movimiento de particulas.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void bosubZmpt_MouseDown(object sender, MouseEventArgs e)
        {
            double dd;
            dd = (ceroZ - mnmpt[2]) / (panelPartiTraZ.Height / 2.0);
            if (e.Button == MouseButtons.Left) ceroZ += (int)(dd);
            else ceroZ += 5 * (int)(dd);
            if (mptintp == false) TrazaComponente();
            else TrazaComponenteInterp();
        }
        /// <summary>
        /// Desplaza hacia anbajo la linea roja que cruza la traza de la componente Z en la grafica de movimiento de particulas.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void bobajZmpt_MouseDown(object sender, MouseEventArgs e)
        {
            double dd;

            dd = (ceroZ - mnmpt[2]) / (panelPartiTraZ.Height / 2.0);
            if (e.Button == MouseButtons.Left) ceroZ -= (int)(dd);
            else ceroZ -= 5 * (int)(dd);
            if (mptintp == false) TrazaComponente();
            else TrazaComponenteInterp();
        }
        /// <summary>
        /// Panel donde se grafica la porción de traza de la componente N a la que se le calcula el movimiento de particulas.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void panelPartiTraN_MouseDown(object sender, MouseEventArgs e)
        {
            int xf;
            double fax, dd, fac;

            if (e.X < 40)
                return;
            if (mptintp == false)
                fac = 1.0;
            else
                fac = facRaInterp;
            xf = panelPartiTraN.Size.Width - 50;
            fax = (double)(durmpt) / (double)(xf);
            dd = (e.X - 40) * fax;
            if (dd > (double)(durmpt)) return;
            if (e.Button == MouseButtons.Left)
                muimpt = (int)(dd * ra[N] * fac);
            else
                mufmpt = (int)(dd * ra[N] * fac);
            if (mufmpt <= muimpt)
                mufmpt = 0;
            if (mptintp == false)
                TrazaComponente();
            else
                TrazaComponenteInterp();
        }
        /// <summary>
        /// Panel donde se grafica la porción de traza de la componente E a la que se le calcula el movimiento de particulas.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void panelPartiTraE_MouseDown(object sender, MouseEventArgs e)
        {
            int xf;
            double fax, dd, fac;

            if (e.X < 40) return;
            if (mptintp == false) fac = 1.0;
            else fac = facRaInterp;
            xf = panelPartiTraE.Size.Width - 50;
            fax = (double)(durmpt) / (double)(xf);
            dd = (e.X - 40) * fax;
            if (dd > (double)(durmpt)) return;
            if (e.Button == MouseButtons.Left) muimpt = (int)(dd * ra[E] * fac);
            else mufmpt = (int)(dd * ra[E] * fac);
            if (mufmpt <= muimpt) mufmpt = 0;
            if (mptintp == false) TrazaComponente();
            else TrazaComponenteInterp();
        }
        /// <summary>
        /// Panel donde se grafica la porción de traza de la componente Z a la que se le calcula el movimiento de particulas.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void panelPartiTraZ_MouseDown(object sender, MouseEventArgs e)
        {
            int xf;
            double fax, dd, fac;

            if (e.X < 40) return;
            if (mptintp == false) fac = 1.0;
            else fac = facRaInterp;
            xf = panelPartiTraZ.Size.Width - 50;
            fax = (double)(durmpt) / (double)(xf);
            dd = (e.X - 40) * fax;
            if (dd > (double)(durmpt)) return;
            if (e.Button == MouseButtons.Left) muimpt = (int)(dd * ra[Z] * fac);
            else mufmpt = (int)(dd * ra[Z] * fac);
            if (mufmpt <= muimpt) mufmpt = 0;
            if (mptintp == false) TrazaComponente();
            else TrazaComponenteInterp();
        }
        /// <summary>
        /// Grafica o esconde el mapa donde se dibuja el movimiento de particulas.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void boModX_Click(object sender, EventArgs e)
        {

            int k;

            if (modX == false)
            {
                modX = true;
                boModX.BackColor = Color.YellowGreen;
                k = nuvol;
            }
            else
            {
                modX = false;
                boModX.BackColor = Color.Wheat;
                k = volmpt;
            }
            NoMostrar = true;
            util.VerMapa(panelPartiEN, volcan[k][0], laE, loE, "", difmpt, laE, loE, Color.LightGray);
        }
        /// <summary>
        /// Este botón hace visible el panelMapaMundo en caso de que haya estaciones NEIC (que exista el archivo /pro/tab/estaciones.dat)
        /// y abre en el navegador web predeterminado la página http://earthquake.usgs.gov/earthquakes/recenteqsww/Quakes/quakes_all.php.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void boNeic_MouseDown(object sender, MouseEventArgs e)
        {
            if (nomweb == "") return;

            if (RevisarEstacionNeic() == false)
            {
                boNeic.Visible = false;
                return;
            }
            textBoxNeic.Text = "";
            deltAK135 = 0;
            panelMapaMundo.Visible = true;
            panelMapaMundo.BringToFront();
            panelMapaMundo.Invalidate();
            NoMostrar = true;
            System.Diagnostics.Process.Start(nomweb);
            return;
        }
        /// <summary>
        /// Verifica la existencia del archivo /pro/tab/estaciones.dat.
        /// </summary>
        /// <returns>Retorna true en caso de encontrarlo, false en caso contrario.</returns>
        bool RevisarEstacionNeic()
        {
            string li = "";
            bool res = false;

            if (!File.Exists(".\\tab\\estaciones.dat")) return (false);

            StreamReader ar = new StreamReader(".\\tab\\estaciones.dat");
            while (li != null)
            {
                try
                {
                    li = ar.ReadLine();
                    if (li == null) break;
                    if (li.Length > 5)
                    {
                        if (est[id].Substring(0, 4) == li.Substring(0, 4))
                        {
                            res = true;
                            break;
                        }
                    }
                }
                catch
                {
                    break;
                }
            }
            ar.Close();

            return (res);
        }
        /// <summary>
        /// El texbox textBoxNeic es el que aparece en el panelMapaMundo y es donde se ingresa el dato del sismo distante que 
        /// se quiere mostrar, con este dato se determina el mapa a dibujar en el panelMapaMundo con base a los datos de la
        /// página http://earthquake.usgs.gov/earthquakes/recenteqsww/Quakes/quakes_all.php.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void textBoxNeic_TextChanged(object sender, EventArgs e)
        {
            int i, j, jj, k, l, nu, an = 0, me = 0, di = 0, ho = 0, mi = 0, se = 0, ms = 0, mul = 0;
            double la = 500.0, lo = 500.0, zz = -100.0, mag = 100.0, delta = 0, delt = 0;
            double p1, p2, pp1, pp2, pp, dfz = 0, dfd, dl1, dl2, fac, tiori = 0;
            short z1 = -1, z2 = -1, nuz;
            char[] delim = { ' ', '\t' };
            string[] pa = null;
            double lae, loe;
            string li = "", li2 = "", ca = "";
            ArrayList lista = new ArrayList();

            // util.MapaMundo(panelMapaMundi);
            pa = textBoxNeic.Text.Split(delim);
            nu = 0;
            for (i = 0; i < pa.Length; i++) if (pa[i].Length > 0) nu += 1;
            j = 0;
            if (nu != 6 && nu != 8)
            {
                NoMostrar = true;
                MessageBox.Show("El formato NO es correcto!!");
                return;
            }
            try
            {
                if (nu == 8)
                {
                    for (i = 0; i < pa.Length; i++)
                    {
                        if (pa[i].Length > 0)
                        {
                            if (j == 0) an = int.Parse(pa[i]);
                            else if (j == 1) me = int.Parse(pa[i]);
                            else if (j == 2) di = int.Parse(pa[i]);
                            else if (j == 3)
                            {
                                ho = int.Parse(pa[i].Substring(0, 2));
                                mi = int.Parse(pa[i].Substring(2, 2));
                                se = int.Parse(pa[i].Substring(4, 2));
                            }
                            else if (j == 4) la = double.Parse(pa[i]);
                            else if (j == 5) lo = double.Parse(pa[i]);
                            else if (j == 6) zz = double.Parse(pa[i]);
                            else if (j == 7) mag = double.Parse(pa[i]);
                            j += 1;
                        }
                    }
                }
                else if (nu == 6)
                {
                    for (i = 0; i < pa.Length; i++)
                    {
                        if (pa[i].Length > 0)
                        {
                            if (j == 0)
                            {
                                if (pa[i].Length < 4) mul = 0;
                                else mul = 1;
                            }
                            //MessageBox.Show("j="+j.ToString()+" mul="+mul.ToString());
                            if (j == 0 + 5 * mul) mag = double.Parse(pa[i]);
                            else if (j == 1 - 1 * mul)
                            {
                                an = int.Parse(pa[i].Substring(0, 4));
                                me = int.Parse(pa[i].Substring(5, 2));
                                di = int.Parse(pa[i].Substring(8, 2));
                            }
                            else if (j == 2 - 1 * mul)
                            {
                                ho = int.Parse(pa[i].Substring(0, 2));
                                mi = int.Parse(pa[i].Substring(3, 2));
                                se = int.Parse(pa[i].Substring(6, 2));
                            }
                            else if (j == 3 - 1 * mul)
                            {
                                if (mul == 1)
                                {
                                    jj = pa[i].Length - 2;
                                    la = double.Parse(pa[i].Substring(0, jj));
                                    if (pa[i][jj + 1] == 'S') la = la * -1.0;
                                }
                                else la = double.Parse(pa[i]);
                            }
                            else if (j == 4 - 1 * mul)
                            {
                                if (mul == 1)
                                {
                                    jj = pa[i].Length - 2;
                                    lo = double.Parse(pa[i].Substring(0, jj));
                                    if (pa[i][jj + 1] == 'W') lo = lo * -1.0;
                                }
                                else lo = double.Parse(pa[i]);
                            }
                            else if (j == 5 - 1 * mul)
                            {
                                zz = double.Parse(pa[i]);
                            }
                            j += 1;
                        }
                    }
                }
            }
            catch
            {
                NoMostrar = true;
                MessageBox.Show("El formato NO es correcto!!");
                return;
            }

            if (mag == 100.0) mag = 5.0;
            lafN = (float)(la);
            lofN = (float)(lo);
            zzfN = (float)(zz);
            mgfN = (float)(mag);

            laNeic = -1000.0F;
            loNeic = -1000.0F;
            if (nu != 6 && nu != 8)
            {
                //util.SismoNEIC(panelMapaMundi, la, lo, zz, mag, laNeic, loNeic);
                return;
            }
            if (!File.Exists(".\\tab\\estaciones.dat") || !File.Exists(".\\tab\\P.txt")) return;
            if (siCajTeo == false)
                CrearCajonesTeoricos(panelCajTeo);

            DirectoryInfo dir = new DirectoryInfo(".\\tab");
            FileInfo[] fcc = dir.GetFiles("*.txt");
            j = fcc.Length + 2; // love + Rayleigh;
            tottab = j;
            ArrbTeo = new double[j + 2];
            for (k = 0; k < j + 2; k++) ArrbTeo[k] = -1.0;

            li = "";
            StreamReader ar0 = new StreamReader(".\\tab\\estaciones.dat");
            while (li != null)
            {
                try
                {
                    li = ar0.ReadLine();
                    if (li == null) break;
                    if (li.Length > 8) lista.Add(li);
                }
                catch
                {
                }
            }
            ar0.Close();

            DateTime fech1 = new DateTime(an, me, di, ho, mi, se, ms);
            tiori = ((double)(fech1.Ticks) - Feisuds) / 10000000.0 - utNeic * 3600.0;

            deltAK135 = 0;
            estdelt = "";

            for (j = 0; j < lista.Count; j++)
            {
                if (est[id].Substring(0, 4) == lista[j].ToString().Substring(0, 4))
                {
                    pa = lista[j].ToString().Split(delim);
                    lae = double.Parse(pa[1]);
                    loe = double.Parse(pa[2]);
                    delta = DeltaEstacion(lae, loe, la, lo);
                    if (deltAK135 == 0)
                    {
                        deltAK135 = (float)(delta);
                        estdelt = est[id];
                        laNeic = (float)(lae);
                        loNeic = (float)(loe);
                    }
                    k = 0;
                    DirectoryInfo dir2 = new DirectoryInfo(".\\tab");
                    FileInfo[] fcc2 = dir.GetFiles("*.txt");
                    foreach (FileInfo f in fcc2)
                    {
                        li = "";
                        li2 = "";
                        ca = ".\\tab\\" + f.Name;
                        if (!File.Exists(ca)) continue;
                        StreamReader pr = new StreamReader(ca);
                        try
                        {
                            li = pr.ReadLine();
                            pa = li.Split(delim);
                            nuz = -1;
                            for (l = 0; l < pa.Length; l++)
                            {
                                if (zz < double.Parse(pa[l]))
                                {
                                    z1 = short.Parse(pa[l - 1]);
                                    z2 = short.Parse(pa[l]);
                                    dfz = (double)(z2 - z1);
                                    nuz = (short)(l - 1);
                                    break;
                                }
                            }
                            if (nuz == -1)
                            {
                                k += 1;
                                continue;
                            }
                            //MessageBox.Show(f.Name+" z1="+z1.ToString()+" z2="+z2.ToString()+" zz="+zz.ToString()+" nuz="+nuz.ToString());
                        }
                        catch
                        {
                            k += 1;
                            continue;
                        }
                        while (li != null)
                        {
                            try
                            {
                                li = pr.ReadLine();
                                if (li == null) break;
                                delt = double.Parse(li.Substring(0, 3));
                                if (delt > delta)
                                {
                                    if (li2.Length == 0) break;
                                    pa = li2.Split(delim);
                                    dl1 = double.Parse(pa[0]);
                                    p1 = double.Parse(pa[nuz * 2 + 1]) * 60.0 + double.Parse(pa[nuz * 2 + 2]);
                                    p2 = double.Parse(pa[(nuz + 1) * 2 + 1]) * 60.0 + double.Parse(pa[(nuz + 1) * 2 + 2]);
                                    fac = (double)(zz - z1) / dfz;
                                    if (zz == z1) pp1 = p1;
                                    else pp1 = p1 + (p2 - p1) * fac;
                                    pa = li.Split(delim);
                                    dl2 = double.Parse(pa[0]);
                                    p1 = double.Parse(pa[nuz * 2 + 1]) * 60.0 + double.Parse(pa[nuz * 2 + 2]);
                                    p2 = double.Parse(pa[(nuz + 1) * 2 + 1]) * 60.0 + double.Parse(pa[(nuz + 1) * 2 + 2]);
                                    if (zz == z1) pp2 = p1;
                                    else pp2 = p1 + (p2 - p1) * fac;
                                    dfd = (double)(dl2 - dl1);
                                    fac = (delta - dl1) / dfd;
                                    if (delt == dl1) pp = pp1;
                                    else pp = pp1 + (pp2 - pp1) * fac;
                                    ArrbTeo[k] = tiori + pp;
                                    break;
                                }
                                li2 = li;
                            }
                            catch
                            {
                            }
                        }
                        pr.Close();
                        k += 1;
                    }// foreach
                    if (delta >= 20.0)
                    {
                        ArrbTeo[k] = tiori + 25.38 * delta; // Love
                        ArrbTeo[k + 1] = tiori + 28.17 * delta; // Rayleigh                    
                    }
                    else
                    {
                        ArrbTeo[k] = 0;
                        ArrbTeo[k + 1] = 0;
                    }
                    break;
                }
            }
            //util.SismoNEIC(panelMapaMundi, la, lo, zz, mag, laNeic, loNeic);
            panelCajTeo.Visible = true;
            panelCajTeo.BringToFront();
            ChequearCajones();
            PonerTeorico();
            panelMapaMundo.Invalidate();

            return;
        }
        /// <summary>
        /// Cierra el panelMapaMundo.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void boXmapamundo_Click(object sender, EventArgs e)
        {
            panelMapaMundo.Visible = false;
            if (panelCajTeo.Visible == true)
            {
                panelCajTeo.Visible = false;
                cbx.Initialize();
            }
            deltAK135 = 0;
        }
        /// <summary>
        /// Se encarga de dibujar al panelMapaMundo el cual esta situado en la esquina superior derecha de la pantalla.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void panelMapaMundo_Paint(object sender, PaintEventArgs e)
        {
            int an, me, di, ho, mi;
            long ll;
            string ca = "";

            //MessageBox.Show(listBox1.Items[listBox1.SelectedIndex].ToString());
            an = int.Parse(listBox1.Items[listBox1.SelectedIndex].ToString().Substring(0, 2));
            //string st = "14/07/24 7:10";
            //an = int.Parse(st.Substring(0, 2));
            if (an < 80) an += 2000;
            else an += 1900;
            me = int.Parse(listBox1.Items[listBox1.SelectedIndex].ToString().Substring(3, 2));
            di = int.Parse(listBox1.Items[listBox1.SelectedIndex].ToString().Substring(6, 2));
            ho = int.Parse(listBox1.Items[listBox1.SelectedIndex].ToString().Substring(9, 2));
            mi = int.Parse(listBox1.Items[listBox1.SelectedIndex].ToString().Substring(12, 2));

            DateTime fech = new DateTime(an, me, di, ho, mi, 0);
            ll = fech.Ticks;
            if (utNeic != 0) ll += (long)(utNeic * 3600.0 * 10000000.0);
            DateTime fecha = new DateTime(ll);
            ca = string.Format("{0:yyyy} {0:MM} {0:dd}    {0:HH}:{0:mm}", fecha);
            if (deltAK135 > 0) ca += "    " + string.Format("{0:0.0}º", deltAK135) + " " + estdelt.Substring(0, 4);
            Graphics dc = panelMapaMundo.CreateGraphics();
            SolidBrush bro = new SolidBrush(Color.Linen);
            dc.FillRectangle(bro, 0, 0, panelMapaMundo.Width, panelMapaMundo.Height);
            bro.Dispose();
            SolidBrush brocha = new SolidBrush(Color.Blue);
            dc.DrawString(ca, new Font("Arial", 9, FontStyle.Bold), brocha, 1, 20);
            brocha.Dispose();
            NoMostrar = true;
            util.MapaMundo(panelMapaMundi);
            util.SismoNEIC(panelMapaMundi, lafN, lofN, zzfN, mgfN, laNeic, loNeic);
        }
        /// <summary>
        /// Crea los checkbox que representan los archivos.txt alojados en la carpeta .\bin\Debug\tab,
        /// que se usan para presentar los tipos de arribo a ese sismo.
        /// </summary>
        /// <param name="panel">Panel donde se crean los checkBox, (panelCajTeo).</param>
        void CrearCajonesTeoricos(Panel panel)
        {
            int i, j, cont;

            siCajTeo = true;
            DirectoryInfo dir = new DirectoryInfo(".\\tab");
            FileInfo[] fcc = dir.GetFiles("*.txt");
            cont = fcc.Length + 2;
            panelCajTeo.Size = new Size(120, cont * 18 + 20);
            i = 0;
            foreach (FileInfo f in fcc)
            {
                cbx[i] = new CheckBox();
                this.cbx[i].Anchor = ((AnchorStyles)((AnchorStyles.Top | AnchorStyles.Left)));
                this.cbx[i].AutoSize = true;
                this.cbx[i].CheckAlign = System.Drawing.ContentAlignment.MiddleLeft;
                this.cbx[i].Location = new System.Drawing.Point(25, i * 18);
                this.cbx[i].Size = new System.Drawing.Size(33, 17);
                this.cbx[i].TabIndex = i;
                j = f.Name.Length - 4;
                this.cbx[i].Text = f.Name.Substring(0, j);
                this.cbx[i].UseVisualStyleBackColor = true;
                this.cbx[i].CheckedChanged += new System.EventHandler(this.cbx_CheckedChanged);
                this.panelCajTeo.Controls.Add(cbx[i++]);
            }
            for (j = 0; j < 2; j++)
            {
                cbx[i] = new CheckBox();
                this.cbx[i].Anchor = ((AnchorStyles)((AnchorStyles.Top | AnchorStyles.Left)));
                this.cbx[i].AutoSize = true;
                this.cbx[i].CheckAlign = System.Drawing.ContentAlignment.MiddleLeft;
                this.cbx[i].Location = new System.Drawing.Point(25, i * 18);
                this.cbx[i].Size = new System.Drawing.Size(33, 17);
                this.cbx[i].TabIndex = i;
                if (j == 0) this.cbx[i].Text = "LQ";
                else this.cbx[i].Text = "LR";
                this.cbx[i].UseVisualStyleBackColor = true;
                this.cbx[i].CheckedChanged += new System.EventHandler(this.cbx_CheckedChanged);
                this.panelCajTeo.Controls.Add(cbx[i++]);
            }

        }
        /// <summary>
        /// Se lanza cuando se cambia de estado a cualquiera de los checkBox cbx,
        /// y determina el checkbox especifico que tuvo este evento, si el inidice de dicho checkBox
        /// es diferente de -1 llama al método PonerTeorico().
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void cbx_CheckedChanged(object sender, EventArgs e)
        {
            int i, j;

            CheckBox chb = (CheckBox)sender;

            j = -1;
            for (i = 0; i < cbx.Length; i++)
            {
                if (chb.Text == cbx[i].Text)
                {
                    j = i;
                    break;
                }
            }
            if (j == -1)
                return;
            PonerTeorico();

            return;
        }
        /// <summary>
        /// Retorna un delta que calcula a partir de 2 coordenadas cada una de llas con su respectivo valor
        /// de latitud y longitud.
        /// </summary>
        /// <param name="la1">Latitud de la coordenada 1.</param>
        /// <param name="lo1">Longitud de la coordenada 1.</param>
        /// <param name="laf">Latitud de la coordenada 2.</param>
        /// <param name="lof">Longitud de la coordenada 2.</param>
        /// <returns></returns>
        double DeltaEstacion(double la1, double lo1, double laf, double lof)
        {
            double delta = 0, fac, fac2, phi1, phi2, difLon, res;

            fac = Math.PI / 180.0;
            fac2 = 180.0 / Math.PI;
            phi1 = (90.0 - la1) * fac;
            phi2 = (90.0 - laf) * fac;
            difLon = (Math.Abs(lo1 - lof)) * fac;
            res = Math.Acos(Math.Cos(phi1) * Math.Cos(phi2) + Math.Sin(phi1) * Math.Sin(phi2) * Math.Cos(difLon));
            delta = res * fac2;

            return (delta);
        }
        /// <summary>
        /// Está pendiente su documentación.
        /// </summary>
        void PonerTeorico()
        {
            int i, k, ancho;
            int xf, yf, jb, jj, b;
            long ll;
            float x1, y1;
            double fax, fay;
            double ti1, ti2, tii;
            Color colo;
            Color[] col;


            if (siCajTeo == false) return;

            col = new Color[24];

            for (i = 0; i < 24; i++) col[i] = Color.Gray;

            col[0] = Color.ForestGreen;
            col[1] = Color.GreenYellow;
            col[2] = Color.MediumSlateBlue;
            col[3] = Color.Fuchsia;
            col[4] = Color.IndianRed;
            col[5] = Color.Violet;
            col[6] = Color.SpringGreen;
            col[7] = Color.OrangeRed;
            col[8] = Color.Orange;
            col[9] = Color.Thistle;
            col[10] = Color.DarkGoldenrod;
            col[11] = Color.Blue;
            col[12] = Color.BurlyWood;
            col[13] = Color.RosyBrown;
            col[14] = Color.SandyBrown;
            col[15] = Color.SlateGray;
            col[16] = Color.Gray;
            col[17] = Color.Tan;
            col[18] = Color.DarkCyan;
            col[19] = Color.Maroon;
            col[20] = Color.Peru;
            col[21] = Color.DeepSkyBlue;
            col[22] = Color.MediumVioletRed;
            col[23] = Color.Magenta;
            xf = panel1.Size.Width;
            yf = panel1.Size.Height;

            jb = tim[id].Length - 1;
            jj = 1 + (int)((tim[id][jb] - timin) / dur);
            if (esp == 0) fay = (yf - 45.0) / jj;
            else fay = esp;
            fax = xf / dur;

            ll = (long)timin;
            ti1 = (double)(ll);
            for (i = 0; i <= 60; i++)
            {
                if (Math.IEEERemainder(ti1, 60.0) == 0) break;
                ti1 += 1.0;
            }
            ti2 = tim[id][jb];
            Graphics dc = panel1.CreateGraphics();
            Graphics dc0 = panelCajTeo.CreateGraphics();
            SolidBrush br = new SolidBrush(Color.Gainsboro);
            dc0.FillRectangle(br, 0, 0, panelCajTeo.Width, panelCajTeo.Height);
            br.Dispose();
            ancho = (int)(fay / 4.0);
            for (k = 0; k < tottab; k++)
            {
                if (ArrbTeo[k] <= 0) continue;
                if (cbx[k].Checked == true) colo = col[k];
                else colo = colfondo;
                Pen lap = new Pen(colo, 2);
                lap.DashStyle = DashStyle.Dash;
                tii = ArrbTeo[k];
                b = (int)((tii - timin) / dur);
                x1 = (float)(((tii - timin) - ((double)(b) * dur)) * fax);
                y1 = (float)(45.0 + b * fay + fay / 2);

                if (tii > 0)
                {
                    if (tii >= timin && tii <= tim[id][jb])
                    {
                        dc.DrawLine(lap, x1, y1 - ancho, x1, y1 + ancho);
                    }
                    else if (tii < timin)
                    {
                        SolidBrush br1 = new SolidBrush(colo);
                        dc0.FillEllipse(br1, 2, 2 + k * 18, 10, 10);
                        br1.Dispose();
                    }
                    else if (tii > tim[id][jb])
                    {
                        SolidBrush br2 = new SolidBrush(colo);
                        dc0.FillEllipse(br2, panelCajTeo.Width - 20, 2 + k * 18, 10, 10);
                        br2.Dispose();
                    }
                }
                lap.Dispose();
            }

            return;
        }
        /// <summary>
        /// Asigna los colores del texto y fondo de los checkBox cbx, además de virificar cuales están checked = true.
        /// </summary>
        void ChequearCajones()
        {
            int k;
            Color colo;
            Color[] col;

            if (siCajTeo == false) return;

            col = new Color[24];
            for (k = 0; k < tottab; k++)
            {
                cbx[k].ForeColor = Color.LightGray;
                cbx[k].Checked = false;
            }

            col[0] = Color.ForestGreen;
            col[1] = Color.GreenYellow;
            col[2] = Color.MediumSlateBlue;
            col[3] = Color.Fuchsia;
            col[4] = Color.IndianRed;
            col[5] = Color.Violet;
            col[6] = Color.SpringGreen;
            col[7] = Color.OrangeRed;
            col[8] = Color.Orange;
            col[9] = Color.Thistle;
            col[10] = Color.DarkGoldenrod;
            col[11] = Color.Blue;
            col[12] = Color.BurlyWood;
            col[13] = Color.RosyBrown;
            col[14] = Color.SandyBrown;
            col[15] = Color.SlateGray;
            col[16] = Color.Gray;
            col[17] = Color.Tan;
            col[18] = Color.DarkCyan;
            col[19] = Color.Maroon;
            col[20] = Color.Peru;
            col[21] = Color.DeepSkyBlue;
            col[22] = Color.MediumVioletRed;
            col[23] = Color.Magenta;

            for (k = 0; k < tottab; k++)
            {
                if (ArrbTeo[k] > 0)
                {
                    colo = col[k];
                    if (cbx[k].Checked == false)
                    {
                        cbx[k].Checked = true;
                        cbx[k].ForeColor = colo;
                    }
                }
            }
        }
        /// <summary>
        /// Cierra el panelCajTeo.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void boXNeic_Click(object sender, EventArgs e)
        {
            panelCajTeo.Visible = false;
            panelMapaMundo.Visible = false;
        }
        /// <summary>
        /// Actualiza el contenido gráfico del penel1 si el panel panelcladib está oculto, en caso contrario lanza el método TrazasClas().
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void boPaintEsp_Click(object sender, EventArgs e)
        {
            if (panelcladib.Visible == false)
                panel1.Invalidate();
            else
                TrazasClas();
        }
        /// <summary>
        /// Actualiza el estilo gráfico del Form1 cuando este cambia de tamaño.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            int i, j;

            if (estado == false || panel1a.Visible == false) return;
            panel1.BorderStyle = BorderStyle.FixedSingle;
            i = Height - 57;
            j = Width - 195;
            //panel1a.Visible = true;
            panel1.Size = new Size(j, (int)(i / 2.0));
            panel1a.Size = new Size(panel1.Width, (int)(i / 2.0));
            panel1a.Location = new Point(panel1.Location.X, panel1.Location.Y + panel1.Height);
            panel1.Invalidate();
            /*if (panel1a.Visible==true)*/
            panel1a.Invalidate();
        }
        /// <summary>
        /// Oculata el panel1a.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void boXpanel1a_Click(object sender, EventArgs e)
        {
            panel1.BorderStyle = BorderStyle.Fixed3D;
            panel1a.Visible = false;
            panel1.Size = new Size(panel1.Size.Width, Height - 57);
        }
        /// <summary>
        /// Es el botón de las tarjetas, muestra o esconde los paneles que indican la selección de tarjetas.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void boTar_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (panelTar.Visible == false)
                    panelTar.Visible = true;
                else
                    panelTar.Visible = false;
            }
            else if (archgcfaux.Length > 0)
            {
                if (panelTarAux.Visible == false)
                    panelTarAux.Visible = true;
                else panelTarAux.Visible = false;
            }
        }
        /// <summary>
        /// No hace nada.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void checkBox1_MouseDown(object sender, MouseEventArgs e)
        {
            CheckBox ch = (CheckBox)sender;
            //MessageBox.Show("hola ch="+ch.Text);
        }
        /// <summary>
        /// Modifica el número de muestras (la variable M) a usar para el filtro.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void boMx_MouseDown(object sender, MouseEventArgs e)
        {
            cfilx = '0';
            respuesta = false;
            if (e.Button == MouseButtons.Left)
                M = (short)(M * 2);
            else M = (short)(M / 2.0);
            if (M < 128) M = 1024;
            else if (M > 1024) M = 128;
            boMx.Text = M.ToString();
            filtx = false;
            boFilBajX.BackColor = Color.White;
            boFilAltX.BackColor = Color.White;
            boFilBanX.BackColor = Color.White;
            //boResFiltX.Visible = false;
        }
        /// <summary>
        /// Filtra bajos en el panel1a.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void boFilBajX_Click(object sender, EventArgs e)
        {
            if (filtx == true)
            {
                filtx = false;
                boFilBajX.BackColor = Color.White;
                textBox1.BackColor = Color.White;
                textBox2.BackColor = Color.White;
            }
            else
            {
                filtx = true;
                if (cfilx != '1')
                {
                    panel2.Visible = true;
                    util.Mensaje(panel2, "Calculando Filtro....", true);
                    cfx = new int[cu[ida].Length];
                    cfx = util.PasaBajos(cu[ida], M, (float)(ra[ida]), Fcx1);
                    cfilx = '1';
                }
                boFilBajX.BackColor = Color.Green;
                textBox1.BackColor = Color.Orange;
                textBox2.BackColor = Color.White;
            }
            boFilAltX.BackColor = Color.White;
            boFilBanX.BackColor = Color.White;
            boResFiltX.Visible = filtx;
            panel2.Visible = false;
            panel1a.Invalidate();
        }
        /// <summary>
        /// Filtra altos en el panel1a.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void boFilAltX_Click(object sender, EventArgs e)
        {
            if (filtx == true)
            {
                filtx = false;
                boFilAltX.BackColor = Color.White;
                textBox1.BackColor = Color.White;
                textBox2.BackColor = Color.White;
            }
            else
            {
                filtx = true;
                if (cfilx != '2')
                {
                    panel2.Visible = true;
                    cfilx = '2';
                    util.Mensaje(panel2, "Calculando Filtro....", true);
                    cfx = new int[cu[ida].Length];
                    cfx = util.PasaAltos(cu[ida], M, (float)(ra[ida]), Fcx1);
                }
                boFilAltX.BackColor = Color.Green;
                textBox1.BackColor = Color.Orange;
                textBox2.BackColor = Color.White;
            }
            boFilBajX.BackColor = Color.White;
            boFilBanX.BackColor = Color.White;
            panel2.Visible = false;
            boResFiltX.Visible = filtx;
            panel1a.Invalidate();
        }
        /// <summary>
        /// Filtra banda en el panel1a.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void boFilBanX_Click(object sender, EventArgs e)
        {

            if (filtx == true)
            {
                filtx = false;
                boFilBanX.BackColor = Color.White;
                textBox1.BackColor = Color.White;
                textBox2.BackColor = Color.White;
            }
            else
            {
                filtx = true;
                if (cfilx != '3')
                {
                    panel2.Visible = true;
                    cfilx = '3';
                    util.Mensaje(panel2, "Calculando Filtro....", true);
                    cfx = new int[cu[ida].Length];
                    cfx = util.PasaBanda(cu[ida], M, (float)(ra[ida]), Fcx1, Fcx2);
                }
                boFilBanX.BackColor = Color.Green;
                textBox1.BackColor = Color.Orange;
                textBox2.BackColor = Color.Orange;
            }
            boFilBajX.BackColor = Color.White;
            boFilAltX.BackColor = Color.White;
            panel2.Visible = false;
            boResFiltX.Visible = filtx;
            panel1a.Invalidate();
        }
        /// <summary>
        /// Setea el valor de la variable Fcx1 que determina la frecuencia de corte en el panel auxiliar.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            double dd;
            MessageBox.Show("Holaaaaaaaaaaaaaa");
            cfilx = '0';
            respuesta = false;
            if (e.KeyCode == Keys.Enter)
            {
                try
                {
                    dd = double.Parse(textBox1.Text);
                }
                catch
                {
                    return;
                }
                if (dd < Fcx2) Fcx1 = dd;
                textBox1.Text = string.Format("{0:00.00}", Fcx1);
            }
        }
        /// <summary>
        /// Setea el valor de la variable Fcx2 que determina la frecuencia de corte 2 en el panel auxiliar.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            double dd;

            cfilx = '0';
            respuesta = false;
            if (e.KeyCode == Keys.Enter)
            {
                try
                {
                    dd = double.Parse(textBox2.Text);
                }
                catch
                {
                    return;
                }
                if (dd > Fcx1) Fcx2 = dd;
                textBox2.Text = string.Format("{0:00.00}", Fcx2);
            }
        }
        /// <summary>
        /// Setea el valor de la variable Fcx1 que determina la frecuencia de corte en el panel auxiliar.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void textBox1_TextChanged(object sender, EventArgs e)
        {

            double dd;

            cfilx = '0';
            try
            {
                dd = double.Parse(textBox1.Text);
            }
            catch
            {
                return;
            }
            Fcx1 = dd;
        }
        /// <summary>
        /// Setea el valor de la variable Fcx2 que determina la frecuencia de corte 2 en el panel auxiliar.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            double dd;

            cfilx = '0';
            try
            {
                dd = double.Parse(textBox2.Text);
            }
            catch
            {
                return;
            }
            Fcx2 = dd;
        }
        /// <summary>
        /// Muestra en el textBox1 la frecuencia de corte 1 para el filtro del panel auxiliar.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void textBox1_Validated(object sender, EventArgs e)
        {
            textBox1.Text = string.Format("{0:00.00}", Fcx1);
        }
        /// <summary>
        /// Muestra en el textBox2 la frecuencia de corte 2 para el filtro del panel auxiliar.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void textBox2_Validated(object sender, EventArgs e)
        {
            textBox2.Text = string.Format("{0:00.00}", Fcx2);
        }
        /// <summary>
        /// Llama al método VerRespuestaFiltro().
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void boResFiltX_Click(object sender, EventArgs e)
        {
            VerRespuestaFiltro();
        }
        /// <summary>
        /// Dependiendo del valor de cfilx (1,2,3) determina qué tipo de filtro aplicar a la traza del panel1a,
        /// paso seguido determina los puntos a aplicar el filtro y grafica dichos puntos en el panel1a.
        /// </summary>
        void VerRespuestaFiltro()
        {
            int i, j, xf, yf, x1, x2, y1, y2, yini, xini;
            float fax, fx, fy;
            double mx, mn, suma;
            double[] h, esp;
            string ca = "";
            Point[] dat, dat2;


            if (cfilx == '0') return;
            if (respuesta == false) respuesta = true;
            else
            {
                respuesta = false;
                panel1a.Invalidate();
                return;
            }
            xf = panel1a.Size.Width - 100;
            yf = panel1a.Size.Height - 100;

            fax = (float)(xf / 2.0);
            fx = (float)(fax / M);

            h = new double[M];
            if (cfilx == '1') h = util.HBajo(M, (float)(ra[ida]), Fcx1);
            else if (cfilx == '2') h = util.HAlto(M, (float)(ra[ida]), Fcx1);
            else if (cfilx == '3') h = util.HBand(M, (float)(ra[ida]), Fcx1, Fcx2);
            else return;

            suma = 0;
            for (i = 0; i < h.Length; i++) suma += h[i];

            mx = h[0];
            mn = h[0];
            for (i = 1; i < M; i++)
            {
                if (mx < h[i]) mx = h[i];
                else if (mn > h[i]) mn = h[i];
            }

            fy = (float)(yf / (mx - mn));

            Graphics dc = panel1a.CreateGraphics();
            Pen lapiz = new Pen(Color.Black, 1);
            Pen lap = new Pen(Color.Orange, 1);
            lap.DashStyle = DashStyle.DashDot;
            SolidBrush bro = new SolidBrush(Color.White);
            dc.FillRectangle(bro, 0, 0, panel1a.Width, panel1a.Height);
            bro.Dispose();
            SolidBrush bro2 = new SolidBrush(Color.OrangeRed);

            xini = 30;
            yini = yf + 50;
            y1 = yini - (int)((0 - mn) * fy);
            dc.DrawLine(lap, xini, y1, (int)(xf / 2.0), y1);
            x1 = xini + (int)(fx * (M / 2.0));
            y1 = yini - (int)((mx - mn) * fy);
            dc.DrawLine(lap, x1, yini, x1, y1);
            ca = string.Format("{0:0}", (int)(M / 2.0));
            dc.DrawString(ca, new Font("Times New Roman", 10), bro2, x1 - 10, yini + 5);
            y1 = yini - (int)((mx - mn) * fy);
            ca = string.Format("{0:0.000000}", mx);
            dc.DrawString(ca, new Font("Times New Roman", 10), bro2, x1 - 20, y1 - 20);
            ca = string.Format("Suma Total={0:0.00}", suma);
            dc.DrawString(ca, new Font("Times New Roman", 10), bro2, xini, y1 - 20);
            x1 = (int)(fax) - 40;
            dc.DrawString("No. Muestra", new Font("Times New Roman", 10), bro2, x1 - 30, yini + 5);

            dat = new Point[M];

            for (i = 0; i < M; i++)
            {
                x1 = xini + (int)(fx * i);
                y1 = yini - (int)((h[i] - mn) * fy);
                dat[i].X = x1;
                dat[i].Y = y1;
            }
            dc.DrawLines(lapiz, dat);

            j = (int)(1 + M / 2.0);
            esp = new double[j];
            esp = four.RealFFTAmpli(h, true);

            mx = esp[0];
            mn = mx;
            for (i = 0; i < esp.Length; i++)
            {
                if (mx < esp[i]) mx = esp[i];
                else if (mn > esp[i]) mn = esp[i];
            }

            fx = (float)(2.0 * fx);
            fy = (float)(yf / (mx - mn));

            xini = (int)(fax + 80);
            x1 = xini;
            y1 = yini - (int)((0 - mn) * fy);
            dc.DrawLine(lap, x1, y1, x1 + (int)(xf / 2.0), y1);
            y2 = yini - (int)((mx - mn) * fy);
            dc.DrawLine(lap, x1, y1, x1, y2);

            dat2 = new Point[esp.Length];
            for (i = 0; i < esp.Length; i++)
            {
                x1 = xini + (int)(fx * i);
                y1 = yini - (int)((esp[i] - mn) * fy);
                dat2[i].X = x1;
                dat2[i].Y = y1;
            }
            dc.DrawLines(lapiz, dat2);

            lapiz.Dispose();

            fx = (float)((fax * 2.0) / ra[ida]);
            j = (int)(ra[ida] / 2.0);
            Pen lap1 = new Pen(Color.Red, 1);
            Pen lap2 = new Pen(Color.Blue, 1);
            Pen lap3 = new Pen(Color.Green, 1);

            y2 = yini - (int)((mx - mn) * fy);
            if (cfilx == '1')
            {
                x1 = xini + (int)(Fcx1 * fx);
                dc.DrawLine(lap, x1, yini, x1, y2);
            }
            else if (cfilx == '2')
            {
                x1 = xini + (int)(Fcx1 * fx);
                dc.DrawLine(lap, x1, yini, x1, y2);
            }
            else if (cfilx == '3')
            {
                x1 = xini + (int)(Fcx1 * fx);
                x2 = xini + (int)(Fcx2 * fx);
                dc.DrawLine(lap, x1, yini, x1, y2);
                dc.DrawLine(lap, x2, yini, x2, y2);
            }

            for (i = 1; i <= j; i++)
            {
                x1 = xini + (int)(i * fx);
                if (Math.IEEERemainder(i, 10) == 0) dc.DrawLine(lap3, x1, yini, x1, yini + 15);
                else if (Math.IEEERemainder(i, 5) == 0) dc.DrawLine(lap2, x1, yini, x1, yini + 8);
                else dc.DrawLine(lap1, x1, yini, x1, yini + 5);
            }

            x1 = xini - 30;
            ca = string.Format("{0:0.00}", mx);
            dc.DrawString(ca, new Font("Times New Roman", 10), bro2, x1, y2 - 10);
            ca = string.Format("{0:0.0} Hz", ra[ida] / 2.0);
            x1 = xf + 55;
            dc.DrawString(ca, new Font("Times New Roman", 10), bro2, x1, yini + 16);
            bro2.Dispose();

            lap1.Dispose();
            lap2.Dispose();
            lap3.Dispose();
            lap.Dispose();

            return;
        }
        /// <summary>
        /// Sirve para controlar el estado de la variable interpol y VerEspectro, ademas de desplegar u oclutar algúnos componentes
        /// dependiendo del botón con el que se le de click.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void boInterp_MouseDown(object sender, MouseEventArgs e)
        {

            boGraInterpol.BackColor = Color.Wheat;
            if (interpol == false)
            {
                interpol = true;
                if (e.Button == MouseButtons.Left)
                {
                    if (octainterp == true)
                    {
                        NoInterpol = false;
                        boRa1.Visible = true;
                        boRa2.Visible = true;
                        boRa3.Visible = true;
                        boInterp.BackColor = Color.BlueViolet;
                    }
                    else
                    {
                        NoInterpol = true;
                        boRa1.Visible = false;
                        boRa2.Visible = false;
                        boRa3.Visible = false;
                        boInterp.BackColor = Color.LightGreen;
                    }
                }
                else
                {
                    NoInterpol = true;
                    boRa1.Visible = false;
                    boRa2.Visible = false;
                    boRa3.Visible = false;
                    boInterp.BackColor = Color.LightGreen;
                }
                if (VerEspectro == true)
                {
                    VerEspectro = false;
                    boEspe.BackColor = Color.WhiteSmoke;
                    panelFFTzoom.Visible = false;
                    panelValFFt.Visible = false;
                }
            }
            else
            {
                interpol = false;
                NoInterpol = false;
                boInterp.BackColor = Color.WhiteSmoke;
            }
        }
        /// <summary>
        /// Cierra el panel panelInterP donde se nmuestra la interpolación de la porción de traza seleccionada.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void boXInterp_Click(object sender, EventArgs e)
        {
            MessageBox.Show(boXInterp.Text);
            suma = 0;
            yaInterp = false;
            suma = 0;
            NoMostrar = true;
            panelInterP.Visible = false;
            panelDesplazamiento.Visible = false;
            panelEspectros.Visible = false;
            panelBarEspInterp.Visible = false;
            boEspInterP.Visible = false;
            boGraInterpol.BackColor = Color.Wheat;
            if (panelParti.Visible == true)
                panelParti.Visible = false;
        }
        /// <summary>
        /// Realiza una interpolación en los datos de la traza identificada por el valor del parámetro idd utilizando el octave.
        /// </summary>
        /// <param name="idd">Indica el id de la estación que se está clasificando.</param>
        /// <returns>true en caso de que la interpolación se realice exitosamente, false en caso contrario.</returns>
        bool CalculoInterpolacion(int idd)
        {
            int i, j, mxx, mnn, tot1, tot2, cuenta;
            double dd, facra, fnrat, ini;
            string nom = "", li = "", nom3;

            facNanInt = -1.0;
            if (NoInterpol == false)
            {
                panel2.Visible = true;
                panel2.BringToFront();
                li = "Interpolando.....";
                util.Mensaje(panel2, li, false);
                nom = est[idd].Substring(0, 4);
                if (File.Exists(nom))
                    File.Delete(nom);
                if (File.Exists("spl.txt"))
                    File.Delete("spl.txt");
                StreamWriter da = File.CreateText(nom);
                for (i = ip1; i < ip2; i++)
                    da.WriteLine(cu[idd][i]);
                da.Close();
                StreamWriter wr = File.CreateText(".\\oct\\interpol.txt");
                wr.WriteLine("Val=load('" + nom + "');");
                dd = (ip2 - ip1) / ra[idd];
                facra = 1.0 / ra[idd];
                fnrat = 1.0 / (ra[idd] * facRaInterp);  // Variable que guarda el intervalo de tiempo entre datos interpolados
                ini = facra;
                wr.WriteLine("dur=" + dd.ToString() + ";");
                wr.WriteLine("X=[" + ini.ToString() + ":" + facra + ":" + dd + "];");
                wr.WriteLine("b=[" + ini.ToString() + ":" + fnrat.ToString() + ":" + dd + "];");
                wr.WriteLine("spl=interp1(X,Val,b,'spline');");
                wr.WriteLine("save(\"-ascii\",\"spl.txt\",\"spl\");");
                wr.Close();
                // los datos de interpolacion que saca el Octave se guardan en spl.txt

                li = "/C c:\\octave\\bin\\octave.exe < .\\oct\\interpol.txt";
                util.Dos(li, true);

                if (!File.Exists("spl.txt"))
                {
                    if (File.Exists(nom))
                        File.Delete(nom);
                    interpol = false;
                    panelInterP.Visible = false;
                    boInterp.BackColor = Color.WhiteSmoke;
                    panel2.Visible = false;
                    return (false);
                }

                string fileContent = File.ReadAllText("spl.txt");
                string[] palabras = fileContent.Split(new char[] { ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                spl = new int[palabras.Length];
                try
                {
                    for (int n = 0; n < palabras.Length; n++)
                        spl[n] = (int)(double.Parse(palabras[n])); // variable que guarda los datos interpolados.
                }
                catch
                {
                }
                timspl = new double[spl.Length]; // variable que guarda el tiempo de los datos interpolados.
                timspl[0] = tim[idd][ip1];// ip1 corresponde a la muestra de la traza que corresponde al inicio de la interpolacion.
                for (i = 1; i < timspl.Length; i++)
                    timspl[i] = timspl[i - 1] + fnrat;// fnrat es el tiempo entre datos interpolados consecutivos.
                nom3 = est[idd].Substring(0, 3);
                cuenta = 0;
                for (i = 0; i < nutra; i++)
                {
                    if (est[i].Substring(0, 4) == nom3.Substring(0, 3) + "Z")
                        cuenta += 1;
                    else if (est[i].Substring(0, 4) == nom3.Substring(0, 3) + "N")
                        cuenta += 1;
                    else if (est[i].Substring(0, 4) == nom3.Substring(0, 3) + "E")
                        cuenta += 1;
                }
                if (cuenta == 3)
                    boMptIntp.Visible = true;
                else
                    boMptIntp.Visible = false;

                boMptIntp.Visible = true;//This is my
            }
            else
            { //aqui la señal NO queda interpolada pero se utiliza la misma variable.
                spl = new int[ip2 - ip1];
                timspl = new double[ip2 - ip1];
                j = 0;
                for (i = ip1; i < ip2; i++)
                {
                    spl[j] = cu[idd][i]; // los datos NO se dividen por la Ganancia de la tarjeta.
                    timspl[j++] = tim[idd][i];
                }
            }

            if (checkBoxFiltAlta.Checked == true)
            {
                if (NoInterpol == false)
                    spl = util.PasaAltos(spl, 256, (float)(ra[idd] * facRaInterp), frInterp);
                else
                    spl = util.PasaAltos(spl, 256, (float)(ra[idd]), frInterp);
            }

            if (File.Exists(nom))
                File.Delete(nom);
            panel2.Visible = false;
            boGraInterpol.Visible = true;
            boIntegra.Visible = true;

            if (fcnan[idd] <= 0 || NoInterpol == true)
            {
                if (NoInterpol == true)
                    facNanInt = fcnan[idd];
                return (true);
            }

            mxx = cu[idd][ip1];
            mnn = mxx;
            for (i = ip1; i < ip2; i++)
            {
                if (mxx < cu[idd][i])
                    mxx = cu[idd][i];
                else if (mnn > cu[idd][i])
                    mnn = cu[idd][i];
            }
            tot1 = mxx - mnn;

            mxx = spl[0];
            mnn = mxx;
            for (i = 1; i < spl.Length; i++)
            {
                if (mxx < spl[i])
                    mxx = spl[i];
                else if (mnn > spl[i])
                    mnn = spl[i];
            }
            tot2 = mxx - mnn;

            dd = tot1 * fcnan[idd];
            facNanInt = dd / (double)(tot2);
            //MessageBox.Show("facnan="+fcnan[idd].ToString()+" facNanInt="+facNanInt.ToString()+" tot1="+tot1.ToString()+" tot2="+tot2.ToString());

            return (true);
        }
        /// <summary>
        /// Dibuja en el panel de interpolación (panelInterP) la representación de la traza interpolada.
        /// </summary>
        /// <param name="i1">Punto en la traza desde donde se inicia el cálculo de la interpolación.</param>
        /// <param name="i2">Punto en la traza desde donde se finaliza el cálculo de la interpolación.</param>
        void DibujoInterpolacion(int i1, int i2)
        {
            int i, j, xf, yf, mxx, mnn, pro;
            int x1, y1, yini;
            int ispl1, ispl2;  // valores inicial y final para la variable spl
            double fax, fay, fy;
            string ss = "";
            Point[] dat;

            xf = panelInterP.Width - 20;
            yf = panelInterP.Height - 20;

            mxx = cu[id][i1];
            mnn = mxx;
            for (i = i1 + 1; i <= i2; i++)
            {
                if (mxx < cu[id][i]) mxx = cu[id][i];
                else if (mnn > cu[id][i]) mnn = cu[id][i];
            }

            pro = (int)((mxx + mnn) / 2.0);

            fax = xf / (tim[id][i2] - tim[id][i1]);
            fay = yf / 2.0;
            fy = (fay / 2.5) / (double)(mxx - pro);

            Graphics dc = panelInterP.CreateGraphics();
            Pen lap = new Pen(Color.Black, 1);
            SolidBrush bro = new SolidBrush(Color.WhiteSmoke);
            dc.FillRectangle(bro, 0, 0, panelInterP.Width, panelInterP.Height);
            bro.Dispose();

            dat = new Point[i2 - i1];

            yini = 5 + (int)(fay / 2.0);
            j = 0;
            for (i = i1; i < i2; i++)
            {
                y1 = (int)(yini + (pro - cu[id][i]) * fy);
                x1 = 10 + (int)((tim[id][i] - tim[id][i1]) * fax);
                dat[j].Y = y1;
                dat[j].X = x1;
                j += 1;
            }
            dc.DrawLines(lap, dat);

            Graphics dc2 = panelBotInterpSup.CreateGraphics();
            SolidBrush brooo = new SolidBrush(Color.WhiteSmoke);
            dc.FillRectangle(brooo, 0, 0, panelBotInterpSup.Width, panelBotInterpSup.Height);
            brooo.Dispose();
            SolidBrush bro2 = new SolidBrush(Color.Green);
            ss = "Original dif=" + (mxx - mnn).ToString();
            dc2.DrawString(ss, new Font("Times New Roman", 10), bro2, 410, 2);

            ispl1 = 0;
            for (i = 0; i < spl.Length; i++)
            {
                if (timspl[i] >= tim[id][i1])
                {
                    ispl1 = i;
                    break;
                }
            }
            ispl2 = -1;
            for (i = ispl1; i < spl.Length; i++)
            {
                if (timspl[i] >= tim[id][i2])
                {
                    ispl2 = i;
                    break;
                }
            }
            if (ispl2 == -1) ispl2 = spl.Length - 1;
            //MessageBox.Show("ispl1="+ispl1.ToString()+" ispl2="+ispl2.ToString()+" spl.len="+spl.Length.ToString());

            mxx = spl[ispl1];
            mnn = mxx;
            for (i = ispl1 + 1; i < ispl2; i++)
            {
                if (mxx < spl[i]) mxx = spl[i];
                else if (mnn > spl[i]) mnn = spl[i];
            }

            pro = (int)((mxx + mnn) / 2.0);
            fy = (fay / 2.5) / (double)(mxx - pro);

            dat = new Point[ispl2 - ispl1];

            yini = 5 + (int)(fay + fay / 2.0);
            j = 0;
            for (i = ispl1; i < ispl2; i++)
            {
                y1 = (int)(yini + (pro - spl[i]) * fy);
                x1 = 10 + (int)((timspl[i] - timspl[ispl1]) * fax);
                dat[j].Y = y1;
                dat[j].X = x1;
                j += 1;
            }
            //Pen la = new Pen(Color.Blue, 1);
            dc.DrawLines(lap, dat);

            ss = "Interpol dif=" + (mxx - mnn).ToString();
            dc2.DrawString(ss, new Font("Times New Roman", 10), bro2, 550, 2);
            bro2.Dispose();

            SolidBrush broo = new SolidBrush(Color.Blue);
            ss = string.Format("Original {0:0.0} Muestras/segundo", ra[id]);
            dc2.DrawString(ss, new Font("Times New Roman", 10), broo, panelInterP.Width - 400, 2);
            if (NoInterpol == false) ss = string.Format("Interpolada {0:0.0} Muestras/segundo", ra[id] * facRaInterp);
            else ss = string.Format("Interpolada {0:0.0} Muestras/segundo", ra[id]);
            dc2.DrawString(ss, new Font("Times New Roman", 10), broo, panelInterP.Width - 400, panelInterP.Height - 25);

            broo.Dispose();
            lap.Dispose();

            if (panelDesplazamiento.Visible == true)
            {
                Pen lap2 = new Pen(Color.DarkOrange, 1);
                lap2.DashStyle = DashStyle.DashDot;
                y1 = (int)(yini + (pro - promInterp) * fy);
                dc.DrawLine(lap2, 10, y1, panelInterP.Width - 20, y1);
                lap2.Dispose();
            }

            return;
        }
        /// <summary>
        /// Modifica el valor de la variable facRaInterp haciendo la igual a 5, esta variable usa como factor 
        /// de rata de muestreo para la interpolación, además llama al método calcularInterpolacion con el id
        /// de la traza que se esta clasificando en el panel principal.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void boRa1_Click(object sender, EventArgs e)
        {
            bool si = false;
            NoMostrar = true;
            boGraInterpol.Visible = false;
            boIntegra.Visible = false;
            panelDesplazamiento.Visible = false;
            panelEspectros.Visible = false;
            panelBarEspInterp.Visible = false;
            boEspInterP.Visible = false;
            facRaInterp = 5;
            boRa1.BackColor = Color.Gold;
            boRa2.BackColor = Color.SeaShell;
            boRa3.BackColor = Color.SeaShell;
            si = CalculoInterpolacion(id);
            if (si == true)
            {
                panelInterP.Invalidate();
            }
        }
        /// <summary>
        /// Modifica el valor de la variable facRaInterp haciendo la igual a 10, esta variable usa como factor 
        /// de rata de muestreo para la interpolación, además llama al método calcularInterpolacion con el id
        /// de la traza que se esta clasificando en el panel principal.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void boRa2_Click(object sender, EventArgs e)
        {
            bool si = false;

            NoMostrar = true;
            boGraInterpol.Visible = false;
            boIntegra.Visible = false;
            panelDesplazamiento.Visible = false;
            panelEspectros.Visible = false;
            panelBarEspInterp.Visible = false;
            boEspInterP.Visible = false;
            facRaInterp = 10;
            boRa1.BackColor = Color.SeaShell;
            boRa2.BackColor = Color.Gold;
            boRa3.BackColor = Color.SeaShell;
            si = CalculoInterpolacion(id);
            if (si == true) panelInterP.Invalidate();
        }
        /// <summary>
        /// Modifica el valor de la variable facRaInterp haciendo la igual a 15, esta variable usa como factor 
        /// de rata de muestreo para la interpolación, además llama al método calcularInterpolacion con el id
        /// de la traza que se esta clasificando en el panel principal.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void boRa3_Click(object sender, EventArgs e)
        {
            bool si = false;

            NoMostrar = true;
            boGraInterpol.Visible = false;
            boIntegra.Visible = false;
            panelDesplazamiento.Visible = false;
            panelEspectros.Visible = false;
            panelBarEspInterp.Visible = false;
            boEspInterP.Visible = false;
            facRaInterp = 15;
            boRa1.BackColor = Color.SeaShell;
            boRa2.BackColor = Color.SeaShell;
            boRa3.BackColor = Color.Gold;
            si = CalculoInterpolacion(id);
            if (si == true) panelInterP.Invalidate();
        }
        /// <summary>
        /// Actualiza los gráficos del panel panelInterP.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void panelInterP_Paint(object sender, PaintEventArgs e)
        {
            DibujoInterpolacion(ipb1, ipb2);
        }
        /// <summary>
        /// Hace la variable ixpb = al x donde se efectuó el evento click.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void panelInterP_MouseDown(object sender, MouseEventArgs e)
        {
            ixpb = e.X;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void panelInterP_MouseUp(object sender, MouseEventArgs e)
        {
            int xf, yf, j, k, ini, fin, factorata, mui;
            double fax, tiempo, ttt, tiempo1;

            if (guiainterp == true)
            {
                DibujarGuiaIterp(e);
                return;
            }
            j = ipb1;
            xf = panelInterP.Width - 20;
            yf = panelInterP.Height - 20;

            if (NoInterpol == false)
                factorata = facRaInterp;
            else
                factorata = 1;
            ini = (ipb1 - ip1) * factorata;
            fin = (ipb2 - ip1) * factorata;
            if (fin > spl.Length)
                fin = spl.Length;
            ttt = (timspl[fin - 1] - timspl[ini]);
            fax = ttt / (double)(xf);
            tiempo = timspl[ini] + (ixpb - 10.0) * fax;
            // MessageBox.Show("ini="+ini.ToString()+" fin="+fin.ToString()+" factorata="+factorata.ToString()+" tiempo="+tiempo.ToString()+" ttt="+ttt.ToString());
            mui = (int)((tiempo - timspl[0]) * (ra[id] * factorata));
            tiempo1 = tiempo;

            if (especinterP == true)
            {
                k = 256;
                j = k;
                do
                {
                    k *= 2;
                    if (k > (spl.Length - mui))
                        break;
                    j = k;
                } while (k < (spl.Length - mui));
                if (j < 256)
                    return;
                panelEspectros.Visible = true;
                DosEspectros(mui, j);
                return;
            }
            panelBarEspInterp.Visible = false;
            fax = (tim[id][ipb2] - tim[id][j]) / (double)(xf);
            tiempo = tim[id][j] + (ixpb - 10.0) * fax;
            ipb1 += (int)((tiempo - tim[id][j]) * ra[id]);

            tiempo = tim[id][j] + (e.X - 10.0) * fax;
            ipb2 = j + (int)((tiempo - tim[id][j]) * ra[id]);
            if (ipb2 <= ipb1)
            {
                ipb1 = ip1;
                ipb2 = ip2;
                boIzqInterp.Visible = false;
                boderInterp.Visible = false;
            }
            else
            {
                boIzqInterp.Visible = true;
                boderInterp.Visible = true;
            }
            if (ipb2 > ip2)
                ipb2 = ip2;

            if (particula == true)
            {
                idmpt = (short)(id);
                timpt = tiempo1;
                if (e.Button == MouseButtons.Left)
                    mptintp = true;
                else
                    mptintp = false;
                MovimientoParticula();
            }

            panelInterP.Invalidate();
            if (panelDesplazamiento.Visible == true)
                panelDesplazamiento.Invalidate();

            return;
        }
        /// <summary>
        /// En el panel que muestra la interpolación desplaza la gráfica de la traza hacia la izquierda.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void boIzqInterp_MouseDown(object sender, MouseEventArgs e)
        {
            int i1, i2, num, nn;
            double dd;

            i1 = ipb1;
            i2 = ipb2;
            if (i2 >= ip2) return;
            dd = tim[id][i2] - tim[id][i1];
            num = (int)(ra[id] * dd * 0.2);

            i1 += num;
            i2 += num;
            if (i2 >= ip2)
            {
                nn = ipb2 - ipb1;
                i2 = ip2;
                i1 = ip2 - nn;
            }
            ipb1 = i1; ipb2 = i2;
            panelInterP.Invalidate();
            if (panelDesplazamiento.Visible == true) panelDesplazamiento.Invalidate();
        }
        /// <summary>
        /// En el panel que muestra la interpolación desplaza la gráfica de la traza hacia la derecha.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void boderInterp_MouseDown(object sender, MouseEventArgs e)
        {
            int i1, i2, num;
            double dd;

            i1 = ipb1;
            i2 = ipb2;
            dd = tim[id][i2] - tim[id][i1];
            num = (int)(ra[id] * dd * 0.2);

            i1 -= num;
            i2 -= num;
            if (i1 < ip1)
            {
                i2 += ip1 - i1;
                i1 += ip1 - i1;
            }
            ipb1 = i1; ipb2 = i2;
            panelInterP.Invalidate();
            if (panelDesplazamiento.Visible == true) panelDesplazamiento.Invalidate();
        }
        /// <summary>
        /// Obtiene el valor de verdad del checkBoxSeis y lanza el método ChequeoGraInterpol().
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void checkBoxSeis_CheckedChanged(object sender, EventArgs e)
        {
            sei = checkBoxSeis.Checked;
            ChequeoGraInterpol();
        }
        /// <summary>
        /// Obtiene el valor de verdad del checkBoxAscii y lanza el método ChequeoGraInterpol().
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void checkBoxAscii_CheckedChanged(object sender, EventArgs e)
        {
            asc = checkBoxAscii.Checked;
            ChequeoGraInterpol();
        }
        /// <summary>
        /// Obtiene el valor de verdad del checkBoxSuds y lanza el método ChequeoGraInterpol().
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void checkBoxSuds_CheckedChanged(object sender, EventArgs e)
        {
            sud = checkBoxSuds.Checked;
            ChequeoGraInterpol();
        }
        /// <summary>
        /// Despliega o esconde el botón boGraInterpol dependiendo de el estado de los 
        /// checkBox checkBoxSuds, checkBoxAscii y checkBoxSeis.
        /// </summary>
        void ChequeoGraInterpol()
        {
            if (sei == false && sud == false && asc == false)
                boGraInterpol.Visible = false;
            else
                boGraInterpol.Visible = true;

            return;
        }
        /// <summary>
        /// Llama los métodos para grabar la interpolación en la base en cada formato diferente (Seisan, ascii, Suds).
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void boGraInterpol_Click(object sender, EventArgs e)
        {
            long ll;
            string nom, nom2, f1, f2;

            if (listBox1.Items.Count == 0) return;

            ll = (long)(Fei + tim[id][ip1] * 10000000.0);
            DateTime fech = new DateTime(ll);
            f1 = string.Format("{0:yyyy}-{0:MM}-{0:dd}", fech); //fecha inicial de la traza interpolada 
            if (sei == true)
            {
                if (!Directory.Exists(".\\sei"))
                    Directory.CreateDirectory(".\\sei");
                f2 = string.Format("-{0:HH}{0:mm}-{0:ss}S.", fech) + "XXXX__001";
                nom = f1 + f2;
                GrabaSeisan(nom, false);
                if (panelDesplazamiento.Visible == true)
                {
                    f2 = string.Format("-{0:HH}{0:mm}-{0:ss}D.", fech) + "XXXX__001";
                    nom2 = f1 + f2;
                    GrabaSeisan(nom2, true);
                }
            }
            if (asc == true)
            {
                if (!Directory.Exists(".\\asc")) Directory.CreateDirectory(".\\asc");
                f2 = string.Format("_{0:HH}{0:mm}{0:ss}_" + est[id].Substring(0, 4) + ".txt", fech);
                nom = f1 + f2;
                GrabaAscii(nom, false);
                if (panelDesplazamiento.Visible == true)
                {
                    f2 = string.Format("_{0:HH}{0:mm}{0:ss}_" + est[id].Substring(0, 4) + "_D.txt", fech);
                    nom2 = f1 + f2;
                    GrabaAscii(nom2, true);
                }
            }
            if (sud == true)
            {
                if (!Directory.Exists(".\\sud")) Directory.CreateDirectory(".\\sud");
                f2 = string.Format("_{0:HH}{0:mm}{0:ss}.dmx", fech);
                nom = f1 + f2;
                GrabaSudsInterpol(nom, false);
                if (panelDesplazamiento.Visible == true)
                {
                    f2 = string.Format("_{0:HH}{0:mm}{0:ss}_D.dmx", fech);
                    nom2 = f1 + f2;
                    GrabaSudsInterpol(nom2, true);
                }
            }

            boGraInterpol.BackColor = Color.DarkOrange;

            return;
        }
        /// <summary>
        /// Graba en la base los datos de la interpolación en formato ascii.
        /// </summary>
        /// <param name="nom">Nombre con el que se desea guardar el archivo.</param>
        /// <param name="cond">Se utiliza para verificar si además de guardar los datos de la interpolación
        /// de velocidad de cuentas también se guardan los datos de desplazamiento de cuentas,
        /// esto último en caso de que cond sea true.</param>
        void GrabaAscii(string nom, bool cond)
        {
            int i;
            long ll;
            double dd, rata;
            string dir1, ca = "", ss = "";

            dir1 = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory(".\\asc");

            StreamWriter wr = File.CreateText(nom);

            wr.WriteLine(est[id].Substring(0, 4));
            ll = (long)(Fei + tim[id][ip1] * 10000000.0);
            DateTime fech = new DateTime(ll);
            ca = string.Format("{0:yyyy}/{0:MM}/{0:dd} {0:HH}:{0:mm}:{0:ss}.{0:ms}", fech);
            wr.WriteLine(ca);
            dd = tim[id][ip2] - tim[id][ip1];
            ca = string.Format("{0:0.00 seg}", dd);
            wr.WriteLine(ca);
            if (NoInterpol == false) rata = ra[id] * facRaInterp;
            else rata = ra[id];
            ca = string.Format("{0:0.00} Muestras/Segundo", rata);
            wr.WriteLine(ca);
            ca = string.Format("{0:0} Muestras", spl.Length);
            wr.WriteLine(ca);
            ca = string.Format("{0:0} Ganancia Digital", ga[id].ToString());
            wr.WriteLine(ca);

            if (cond == false)
            {
                ss = "Velocidad Cuentas";
                if (fcnan[id] > 0) ss += "  nanometros/seg";
                wr.WriteLine(ss);
                for (i = 0; i < spl.Length; i++)
                {
                    ss = string.Format("{0:000}", spl[i]);
                    if (fcnan[id] > 0) ss += string.Format("  {0:0.000000000}", spl[i] * (facNanInt / (double)(ga[id])));
                    wr.WriteLine(ss);
                }
            }
            else
            {
                ss = "Desplazamiento  Cuentas";
                if (fcnan[id] > 0) ss += "  Nanometros";
                wr.WriteLine(ss);
                for (i = 0; i < dzp.Length; i++)
                {
                    ss = string.Format("{0:000}", dzp[i]);
                    if (fcnan[id] > 0) ss += string.Format("  {0:0.000000000}", dzp[i] * (facNanInt / (double)(ga[id])));
                    wr.WriteLine(ss);
                }
            }

            wr.Close();

            Directory.SetCurrentDirectory(dir1);

            return;
        }
        /// <summary>
        /// Graba en la base los datos de la interpolación en formato suds extención .dmx.
        /// </summary>
        /// <param name="nom">Nombre con el que se desea guardar el archivo.</param>
        /// <param name="cond">Se utiliza para verificar si además de guardar los datos de la interpolación
        /// de velocidad de cuentas también se guardan los datos de desplazamiento de cuentas,
        /// esto último en caso de que cond sea true.</param>
        void GrabaSudsInterpol(string nom, bool cond)
        {
            string dir1;
            int k, lar;
            char[] tag1 = { 'S', '6' };
            ushort tag2 = 5;
            int tag3 = 76, tag4 = 0, totmu = 0;
            char[] str1 = { 'I', 'N', 'G', 'E' };
            char[] str2 = new char[5];
            char[] str3 = new char[1];
            short str4 = 0, str5 = 0, str6 = 0;
            long str7 = 0, str8 = 0;
            float str9 = 0;
            char str10 = 's', str11 = 'n', str12 = 'n', str13 = 'v';
            short str14 = 0;
            char str15 = 'n', str16 = 'v';
            char str17 = 'l';//data type
            char str18 = 'g', str19 = 'n', str20 = 'g';
            float str21 = 0, str22 = 0, str23 = 0;
            short str24 = 0, str25 = 1;// str25 es la ganancia
            int str26;// tiempo
            float str27 = 0, str28 = 0;
            short sie2 = -5;
            char sie3 = 'l', sie4 = '?';
            short sie5 = 0, sie6 = 0;
            uint sie7;//numero de muestras
            float sie8;//rata
            float sie9 = 0, sie10 = 0, sie11 = 0;
            int sie12 = 0;
            double sie13 = 0.0001;
            float sie14 = 0;

            dir1 = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory(".\\sud");

            FileInfo ar = new FileInfo(nom);
            BinaryWriter br = new BinaryWriter(ar.OpenWrite());

            tag2 = 5;
            tag3 = 76;
            tag4 = 0;
            br.Write(tag1);
            br.Write(tag2);
            br.Write(tag3);
            br.Write(tag4);// hasta aqui Tag principal                    
            //estructura 5
            str2 = est[id].Substring(0, 5).ToCharArray();
            str2[4] = '\0';
            str3[0] = comp[id];// hasta aqui el statident del suds
            if (ga[id] <= 0) ga[id] = 1;
            str25 = ga[id];
            if (by[id] == 4) str17 = 'l';
            else str17 = 'i';
            str26 = (int)(tim[id][ip1]);
            br.Write(str1);
            br.Write(str2);
            br.Write(str3);
            br.Write(str4);
            br.Write(str5);
            br.Write(str6);
            br.Write(str7);
            br.Write(str8);
            br.Write(str9);
            br.Write(str10);
            br.Write(str11);
            br.Write(str12);
            br.Write(str13);
            br.Write(str14);
            br.Write(str15);
            br.Write(str16);
            br.Write(str17);
            br.Write(str18);
            br.Write(str19);
            br.Write(str20);
            br.Write(str21);
            br.Write(str22);
            br.Write(str23);
            br.Write(str24);
            br.Write(str25);
            br.Write(str26);
            br.Write(str27);
            br.Write(str28);

            //tag estructura numero 7 del formato SUDS Demultiplexado
            tag2 = 7;
            tag3 = 64;

            lar = ip2 - ip1;
            totmu = by[id] * spl.Length;
            tag4 = totmu;
            br.Write(tag1);
            br.Write(tag2);
            br.Write(tag3);
            br.Write(tag4);
            // estru 7
            if (by[id] == 4 || cond == true) sie3 = 'l';
            else sie3 = 'i';
            sie7 = (uint)(lar);
            if (NoInterpol == false) sie8 = (float)(ra[id] * facRaInterp);
            else sie8 = (float)(ra[id]);
            br.Write(str1);//statident
            br.Write(str2);//statident
            br.Write(str3);//sataident
            br.Write(str4);//statident                          
            br.Write(tim[id][ip1]);
            br.Write(sie2);
            br.Write(sie3);
            sie4 = tar[id];
            br.Write(sie4);
            br.Write(sie5);
            br.Write(sie6);
            br.Write(sie7);
            br.Write(sie8);
            br.Write(sie9);
            br.Write(sie10);
            br.Write(sie11);
            br.Write(sie12);
            br.Write(sie13);
            br.Write(sie14);

            try
            {
                if (cond == false)
                {
                    if (by[id] == 4) for (k = 0; k < spl.Length; k++) br.Write((int)(spl[k]));
                    else for (k = 0; k < spl.Length; k++) br.Write((short)(spl[k]));
                }
                else
                {
                    if (dzp.Length > 0)
                        for (k = 0; k < dzp.Length; k++) br.Write((int)(dzp[k] * 1000.0));
                }
            }
            catch
            {
                if (cond == true)
                {
                    NoMostrar = true;
                    MessageBox.Show("Problemas (SUDS).. Probable desbordamiento...\nCuidado..El archivo debe estar malo");
                }
            }

            br.Flush();
            br.Close();

            Directory.SetCurrentDirectory(dir1);

            return;
        }
        /// <summary>
        /// Graba en la base los datos de la interpolación en formato Seisan.
        /// </summary>
        /// <param name="nomar">Nombre con el que se desea guardar el archivo.</param>
        /// <param name="cond">Se utiliza para verificar si además de guardar los datos de la interpolación
        /// de velocidad de cuentas también se guardan los datos de desplazamiento de cuentas,
        /// esto último en caso de que cond sea true.</param>
        void GrabaSeisan(string nomar, bool cond)
        {
            int i, j, k, año;
            long ll;
            double dif = 0, durac, rata;
            char[] car = { 'P', '\0', '\0', '\0' };
            char[] c1;
            char[] c2 = new char[994];
            char[] c3 = new char[200];
            string s1 = " Interpolacion                ";
            string s80 = "";
            string s2 = "", doy, an, me, di, ho, mi, se, tipaño, tipsen, tipcom, dir1;

            dir1 = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory(".\\sei");

            durac = tim[id][ip2] - tim[id][ip1];
            if (NoInterpol == false) rata = ra[id] * facRaInterp;
            else rata = ra[id];
            s80 = "P\0\0\0                                                                               ";

            ll = (long)(Fei + tim[id][ip1] * 10000000.0);
            DateTime fech = new DateTime(ll);
            doy = string.Format("{0:000}", fech.DayOfYear);

            an = string.Format("{0:yy}", fech);
            año = int.Parse(an);
            if (año > 84) tipaño = "0";
            else tipaño = "1";
            me = string.Format("{0:MM}", fech);
            di = string.Format("{0:dd}", fech);
            ho = string.Format("{0:HH}", fech);
            mi = string.Format("{0:mm}", fech);
            se = string.Format("{0:ss}.{0:fff}", fech);

            if (File.Exists(nomar))
                File.Delete(nomar);
            FileInfo ar = new FileInfo(nomar);
            BinaryWriter br = new BinaryWriter(ar.OpenWrite());

            br.Write(car);
            c1 = s1.ToCharArray();
            br.Write(c1);
            s2 = "001";
            s2 += tipaño + an.Substring(0, 2) + " " + doy + " ";
            c1 = s2.ToCharArray();
            br.Write(c1);
            s2 = me.Substring(0, 2) + " " + di.Substring(0, 2) + " " + ho.Substring(0, 2) + " ";
            c1 = s2.ToCharArray();
            br.Write(c1);
            s2 = mi.Substring(0, 2) + " " + se.Substring(0, 6) + " " + string.Format("{0,9:0.000}", durac) + "           ";
            c1 = s2.ToCharArray();
            br.Write(c1);
            br.Write(car);
            c1 = s80.ToCharArray();
            br.Write(c1);

            s2 = " P\0\0\0P\0\0\0 ";
            c1 = s2.ToCharArray();
            br.Write(c1);
            if (by[id] == 4 || cond == true) tipsen = "B";
            else tipsen = "S";
            if (est[id][3] == 'Z' || est[id][3] == 'N' || est[id][3] == 'E') tipcom = est[id].Substring(3, 1);
            else if (comp[id] == 'e') tipcom = "E";
            else if (comp[id] == 'n') tipcom = "N";
            else tipcom = "Z";
            s2 = est[id].Substring(0, 4) + tipsen + "H " + tipcom + " ";
            c1 = s2.ToCharArray();
            br.Write(c1);
            dif = 0;
            s2 = string.Format("{0,7:0.00}", dif) + " ";
            c1 = s2.ToCharArray();
            br.Write(c1);
            s2 = string.Format("{0,8:0.00}", durac) + " ";
            c1 = s2.ToCharArray();
            br.Write(c1);
            // es posible que aqui falte completar los 3.
            for (i = 1; i < 3; i++)
            {
                if (i % 3 == 0)
                {
                    s2 = " P\0\0\0P\0\0\0 ";
                    c1 = s2.ToCharArray();
                    br.Write(c1);
                }
                s2 = "         ";
                c1 = s2.ToCharArray();
                br.Write(c1);
                s2 = "        ";
                c1 = s2.ToCharArray();
                br.Write(c1);
                s2 = "         ";
                c1 = s2.ToCharArray();
                br.Write(c1);
            }
            //
            for (i = 1; i < 12; i++)
            {
                s2 = "";
                if (i < 10) s2 = " P\0\0\0P\0\0\0                                                                               ";
                else if (i == 10) s2 = " P\0\0\0\0\0\0\0";
                else break;
                c1 = s2.ToCharArray();
                br.Write(c1);
            }

            // aqui viene los datos de la traza
            ll = (long)(Fei + tim[id][ip1] * 10000000.0);
            DateTime fech2 = new DateTime(ll);
            an = string.Format("{0:yy}", fech2);
            año = int.Parse(an);
            if (año > 84) tipaño = "0";
            else tipaño = "1";
            me = string.Format("{0:MM}", fech2);
            di = string.Format("{0:dd}", fech2);
            ho = string.Format("{0:HH}", fech2);
            mi = string.Format("{0:mm}", fech2);
            se = string.Format("{0:ss}.{0:fff}", fech2);
            if (by[id] == 4 || cond == true) tipsen = "B";
            else tipsen = "S";
            if (est[id][3] == 'Z' || est[id][3] == 'N' || est[id][3] == 'E') tipcom = est[id].Substring(3, 1);
            else if (comp[id] == 'e') tipcom = "E";
            else if (comp[id] == 'n') tipcom = "N";
            else tipcom = "Z";
            s2 = est[id].Substring(0, 4) + " " + tipsen + "H" + tipcom + " " + tipaño;
            c1 = s2.ToCharArray();
            br.Write(c1);
            s2 = an.Substring(0, 2) + " " + doy + " ";
            c1 = s2.ToCharArray();
            br.Write(c1);
            s2 = me.Substring(0, 2) + " " + di.Substring(0, 2) + " " + ho.Substring(0, 2);
            c1 = s2.ToCharArray();
            br.Write(c1);
            s2 = " " + mi.Substring(0, 2) + " " + se.Substring(0, 6);
            //s2 = " 00" + "  0.000";
            c1 = s2.ToCharArray();
            br.Write(c1);
            s2 = " " + string.Format("{0,7:0.00} ", rata) + string.Format("{0:000000}", spl.Length);
            c1 = s2.ToCharArray();
            br.Write(c1);
            for (k = 0; k < 994; k++)
            {
                if (k == 26)
                {
                    if (by[id] == 2 && cond == false) c2[k] = '2';
                    else c2[k] = '4';
                }
                else if (k >= 990) c2[k] = '\0';
                else c2[k] = ' ';
            }
            br.Write(c2);

            try
            {
                if (cond == false)
                {
                    if (by[id] == 2) for (j = 0; j < spl.Length; j++) br.Write((short)(spl[j]));
                    else for (j = 0; j < spl.Length; j++) br.Write(spl[j]);
                }
                else
                {
                    if (dzp.Length > 0)
                        for (j = 0; j < dzp.Length; j++) br.Write((int)(dzp[j] * 1000.0));
                }
            }
            catch
            {
                if (cond == true)
                {
                    NoMostrar = true;
                    MessageBox.Show("Problemas (SEISAN).. Probable desbordamiento...\nCuidado..El archivo debe estar malo");
                }
            }

            br.Write(car);
            br.Write(car);
            br.Write(car);

            br.Close();
            Directory.SetCurrentDirectory(dir1);
            return;
        }
        /// <summary>
        /// Llama al método IntegracionSpl(spl), además esconde o despliega el panelDesplazamiento.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void boIntegra_MouseDown(object sender, MouseEventArgs e)
        {
            NoMostrar = true;
            if (panelDesplazamiento.Visible == false)
            {
                panelDesplazamiento.Visible = true;
                IntegracionSpl(spl);
                DibujoDesplazamiento(ipb1, ipb2);
                boEspInterP.Visible = true;
            }
            else
            {
                panelDesplazamiento.Visible = false;
                panelEspectros.Visible = false;
                panelBarEspInterp.Visible = false;
                boEspInterP.Visible = false;
            }
        }
        /// <summary>
        /// Calcula el promedio a partir de los datos suministrados como parámetros.
        /// </summary>
        /// <param name="canti">Valor utilizado para ser el denominador con el que se calcula el promedio.</param>
        /// <param name="dat">Datos que se suman para obtener el númerador con el que se calcula el promedio.</param>
        /// <param name="cond">Se utiliza para determinar la cantidad de datos del vector dat[] a sumar. </param>
        /// <returns></returns>
        double Promedio(int canti, double[] dat, bool cond)
        {
            int i, ini, fin;
            double prom, sum;

            if (cond == true)
            {
                ini = 0;
                fin = canti;
            }
            else
            {
                ini = dat.Length - canti;
                fin = dat.Length;
            }
            sum = 0;
            for (i = ini; i < fin; i++)
                sum += dat[i];
            prom = sum / (double)(canti);

            return (prom);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="spl">vector que almacenas los datos resultado de una interpolación.</param>
        void IntegracionSpl(int[] spl)
        {
            int i, lar, cont;
            double facra, dd, prom1, prom2, dif, inc;
            double[] val, sspl, val2;

            inc = 0.1;
            if (NoInterpol == false)
                dd = ra[id] * facRaInterp;
            else
                dd = ra[id];
            facra = 1.0 / dd;
            lar = spl.Length - 1;
            dzp = new double[lar];
            //nnm = new double[lar]; // provi nanometros
            val = new double[spl.Length];
            val2 = new double[spl.Length]; // provicional
            sspl = new double[spl.Length];

            for (i = 0; i < spl.Length; i++)
                sspl[i] = (double)(spl[i]);
            cont = 0;
            promInterp = Promedio(150, sspl, true);
            do
            {
                for (i = 0; i < spl.Length; i++)
                    val[i] = sspl[i] - promInterp;

                dzp[0] = 0;
                for (i = 1; i < lar - 1; i++)
                {
                    dzp[i] = dzp[i - 1] + ((val[i - 1] + val[i]) / 2.0) * facra;
                }
                prom1 = Promedio(100, dzp, true);
                prom2 = Promedio(100, dzp, false);
                dif = prom2 - prom1;
                cont += 1;
                if (dif > 1.0)
                    promInterp = promInterp + inc;
                else if (dif < -1.0)
                    promInterp = promInterp - inc;
                else break;
            } while (cont < 1000);
            promDesplz = prom1;
            panelInterP.Invalidate();

            return;
        }
        /// <summary>
        /// Realiza el gráfico de la porción de la traza que se ha interpolado con el fin de mostrar su desplazamiento.
        /// </summary>
        /// <param name="i1">Corresponde a la muestra de la traza que indica el inicio de la interpolación.</param>
        /// <param name="i2">Corresponde a la muestra de la traza que indica el final de la interpolación.</param>
        void DibujoDesplazamiento(int i1, int i2)//ipb1,ipb2
        {
            int i, j, xf, yf;
            int x1, y1, yini;
            int ispl1, ispl2;  // valores inicial y final para la variable spl
            double fax, fay, fy, mxx, mnn, pro;
            Point[] dat;

            xf = panelDesplazamiento.Width - 20;
            yf = panelDesplazamiento.Height - 20;

            ispl1 = 0;
            for (i = 0; i < spl.Length; i++)
            {
                if (timspl[i] >= tim[id][i1])
                {
                    ispl1 = i;
                    break;
                }
            }
            ispl2 = -1;
            for (i = ispl1; i < spl.Length; i++)
            {
                if (timspl[i] >= tim[id][i2])
                {
                    ispl2 = i;
                    break;
                }
            }
            if (ispl2 == -1)
                ispl2 = spl.Length - 1;

            mxx = dzp[ispl1];
            mnn = mxx;
            for (i = ispl1 + 1; i < ispl2; i++)
            {
                if (mxx < dzp[i])
                    mxx = dzp[i];
                else if (mnn > dzp[i])
                    mnn = dzp[i];
            }

            pro = (mxx + mnn) / 2.0;

            fax = xf / (tim[id][i2] - tim[id][i1]);
            fay = yf / 1.0;
            fy = (fay / 2.0) / (double)(mxx - pro);

            Graphics dc = panelDesplazamiento.CreateGraphics();
            Pen lap = new Pen(Color.DarkBlue, 1);
            SolidBrush bro = new SolidBrush(Color.SeaShell);
            dc.FillRectangle(bro, 0, 0, panelDesplazamiento.Width, panelDesplazamiento.Height);
            bro.Dispose();

            dat = new Point[ispl2 - ispl1];

            yini = 5 + (int)(fay / 2.0);
            j = 0;
            try
            {
                for (i = ispl1; i < ispl2; i++)
                {
                    y1 = (int)(yini + (pro - dzp[i]) * fy);
                    x1 = 10 + (int)((timspl[i] - timspl[ispl1]) * fax);
                    dat[j].Y = y1;
                    dat[j].X = x1;
                    j += 1;
                }
                dc.DrawLines(lap, dat);
            }
            catch
            {
            }
            lap.Dispose();
            return;
        }
        /// <summary>
        /// Llama al método CalculoInterpolacion(id) pasando como parámetro el id de la estación que tiene seleccionada,
        /// en caso de que el checkBoxFiltAlta este seleccionado en el método CalculoInterpolacion(id) se aplica un filtro
        /// pasa altos pasando como parámetro para este el factor de rata de muestreo para la interpolación * el factor 
        /// de rata de muestreo específico para la estación seleccionada; pero si el checkBoxFiltAlta no está seleccionado
        /// solo se pasa como parámetro al filtro pasa altos el valor de la rata de muestreo especifico de la estación.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void checkBoxFiltAlta_CheckedChanged(object sender, EventArgs e)
        {
            bool si = false;
            boGraInterpol.Visible = false;
            boIntegra.Visible = false;
            panelDesplazamiento.Visible = false;
            panelEspectros.Visible = false;
            panelBarEspInterp.Visible = false;
            si = CalculoInterpolacion(id);
            if (si == true)
                panelInterP.Invalidate();
        }
        /// <summary>
        /// Llama al método que dibuja el desplazamiento una traza.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void panelDesplazamiento_Paint(object sender, PaintEventArgs e)
        {
            DibujoDesplazamiento(ipb1, ipb2);
        }
        /// <summary>
        /// Dibuja una linea vertical sobre el panel de interpolación en el punto donde se da click.
        /// </summary>
        /// <param name="e">Evento que se lanza cuando da click sobre el panel de interpolación.</param>
        void DibujarGuiaIterp(MouseEventArgs e)
        {
            Graphics dc = panelDesplazamiento.CreateGraphics();
            Pen lap = new Pen(Color.Green, 1);
            dc.DrawLine(lap, e.X, 20, e.X, panelDesplazamiento.Height - 20);
            Graphics dc2 = panelInterP.CreateGraphics();
            Pen lap2 = new Pen(Color.Green, 1);
            dc2.DrawLine(lap, e.X, 20, e.X, panelInterP.Height - 20);
            lap.Dispose();
            lap2.Dispose();
        }
        /// <summary>
        /// Hace que la varible ixpb sea igual a la posición en x donde se dio ellcick.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void panelDesplazamiento_MouseDown(object sender, MouseEventArgs e)
        {
            ixpb = e.X;
        }
        /// <summary>
        /// Este método puede tener 3 funcionalidades diferentes que son:
        /// 1.	Graficar una línea vertical sobre el panel que sirve como guía en caso de que guiainterp sea true.
        /// 2.	Determinar los índices que indican el inicio y el fin del tramo de traza interpolado en la matriz 
        /// tim[][] en caso de que el click sea con el botón izquierdo.
        /// 3.	Graficar un ToolTip indicando el valor de las cuentas y posiciones en longitud y latitud en caso 
        /// de que el click sea con el botón derecho.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void panelDesplazamiento_MouseUp(object sender, MouseEventArgs e)
        {
            int i, xf, yf, j, j1, j2, ispl1, ispl2, ii1, ii2;
            double fax, tiempo, mxx, mnn, cuen, dd1;
            double fay, pro, fy, fcpi, fcdislo, disla = 0, dislo = 0, km, cm, Dr, frms;
            string ss = "";

            if (guiainterp == true)
            {
                DibujarGuiaIterp(e);
                return;
            }
            j = ipb1;
            ii1 = ipb1;
            ii2 = ipb2;
            xf = panelDesplazamiento.Width - 20;
            yf = panelDesplazamiento.Height - 20;
            fax = (tim[id][ipb2] - tim[id][j]) / (double)(xf);

            tiempo = tim[id][j] + (ixpb - 10.0) * fax;
            ii1 += (int)((tiempo - tim[id][j]) * ra[id]);

            tiempo = tim[id][j] + (e.X - 10.0) * fax;
            ii2 = j + (int)((tiempo - tim[id][j]) * ra[id]);
            if (ii2 <= ii1)
            {
                ii1 = ip1;
                ii2 = ip2;
            }
            if (ii2 > ip2)
                ii2 = ip2;

            if (e.Button == MouseButtons.Left)
            {
                ipb1 = ii1;
                ipb2 = ii2;
                panelInterP.Invalidate();
                if (panelDesplazamiento.Visible == true)
                    panelDesplazamiento.Invalidate();
            }
            else
            {
                fcpi = Math.PI / 180.0;
                fcdislo = fcpi * Math.Cos(latitud * fcpi) * 6367.449;
                frms = 1.0 / (2.0 * Math.Sqrt(2.0));
                //MessageBox.Show("frms=" + frms.ToString());
                if (VD[id] != '*')
                {
                    for (i = 0; i <= nuvol; i++)
                    {
                        if (volcan[i][0] == VD[id])
                        {
                            disla = Math.Abs(latvol[i] - laD[id]) * 110.9;//latitud
                            dislo = Math.Abs(lonvol[i] - loD[id]) * fcdislo;//longitud
                            break;
                        }
                    }
                }
                else
                    disla = -1.0;
                ispl1 = 0;
                for (i = 0; i < spl.Length; i++)
                {
                    if (timspl[i] >= tim[id][ii1])
                    {
                        ispl1 = i;
                        break;
                    }
                }
                ispl2 = -1;
                for (i = ispl1; i < spl.Length; i++)
                {
                    if (timspl[i] >= tim[id][ii2])
                    {
                        ispl2 = i;
                        break;
                    }
                }
                if (ispl2 == -1)
                    ispl2 = spl.Length - 1;

                j1 = ipb1 - ip1;
                j2 = ipb2 - ip1;
                mxx = dzp[ispl1];
                mnn = mxx;
                for (i = ispl1 + 1; i < ispl2; i++)
                {
                    if (mxx < dzp[i]) mxx = dzp[i];
                    else if (mnn > dzp[i]) mnn = dzp[i];
                }
                cuen = mxx - mnn;
                //dd1 = cuen*fcnan[id];// nanometros
                dd1 = cuen * (facNanInt / (double)(ga[id]));// nanometros
                //dd1 = cuen * facNanInt;// nanometros
                ss = "pp Cuentas=" + string.Format("{0:0.000000}\n", cuen);
                if (fcnan[id] > 0)
                {
                    ss += " Nanometros:" + string.Format("{0:0.000000}", dd1);
                    if (disla > 0)
                    {
                        km = Math.Sqrt(disla * disla + dislo * dislo);
                        cm = km * 100000.0;
                        Dr = (dd1 * 0.0000001) * frms * cm;
                        ss += string.Format("\nDr={0:0.00}cm2   (Km:{1:0.0}", Dr, km) + ")   " + VD[id].ToString();
                    }
                }
                tip.IsBalloon = true;
                tip.InitialDelay = 0;
                tip.ReshowDelay = 0;
                tip.AutomaticDelay = 0;
                tip.SetToolTip(panelDesplazamiento, ss);


                xf = panelDesplazamiento.Width - 20;
                yf = panelDesplazamiento.Height - 20;

                pro = (mxx + mnn) / 2.0;

                fax = xf / (tim[id][ipb2] - tim[id][ipb1]);
                fay = yf / 1.0;
                fy = (fay / 2.0) / (double)(mxx - pro);
                /*
                               Graphics dc = panelDesplazamiento.CreateGraphics();
                               Pen lap = new Pen(Color.Red, 1);
                               Pen lap2 = new Pen(Color.BlueViolet, 1);
                               SolidBrush bro = new SolidBrush(Color.BlueViolet);
                              // dc.FillRectangle(bro, 0, 0, panelDesplazamiento.Width, panelDesplazamiento.Height);
                              // bro.Dispose();

                               dat = new Point[ispl2 - ispl1];

                               yini = 5 + (int)(fay / 2.0);
                               j = 0;
                               try
                               {
                                   for (i = ispl1; i < ispl2; i++)
                                   {
                                       y1 = (int)(yini + (pro - dzp[i]) * fy);
                                       x1 = 10 + (int)((timspl[i] - timspl[ispl1]) * fax);
                                       dat[j].Y = y1;
                                       dat[j].X = x1;
                                       j += 1;
                                       dc.DrawEllipse(lap2, x1 - 2, y1 - 2, 4, 4);
                                       //MessageBox.Show("x1=" + x1.ToString() + " y1=" + y1.ToString());
                                   }
                                   dc.DrawLines(lap, dat);
                               }
                               catch
                               {
                               }

                               lap.Dispose();
                               bro.Dispose();
                               lap2.Dispose();*/

            }
        }
        /// <summary>
        /// Controla el valor de verdad de la variable guiainterp cambiando dicho valor
        /// cada vez que se hace click en este botón.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void boGuInterpol_Click(object sender, EventArgs e)
        {
            if (guiainterp == false)
            {
                guiainterp = true;
                boGuInterpol.BackColor = Color.BlueViolet;
            }
            else
            {
                guiainterp = false;
                boGuInterpol.BackColor = Color.AntiqueWhite;
            }
        }
        /// <summary>
        /// Controla el cambio de frecuencia que se usa en la interpolación dependiendo el botón del mouse con 
        /// el que se de click.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void boFrInterp_MouseDown(object sender, MouseEventArgs e)
        {
            bool si = false;
            if (e.Button == MouseButtons.Left)
                frInterp += 1.5F;
            else
                frInterp -= 3.0F;
            if (frInterp < 0.5F)
                frInterp = 0.5F;
            else if (frInterp > 30.0F)
                frInterp = 30.0F;
            boFrInterp.Text = string.Format("{0:0.0}", frInterp);
            if (panelDesplazamiento.Visible == true)
            {
                panelDesplazamiento.Visible = false;
                panelEspectros.Visible = false;
                panelBarEspInterp.Visible = false;
            }
            si = CalculoInterpolacion(id);
            if (si == true)
                panelInterP.Invalidate();
        }
        /// <summary>
        /// Cambia el estado de verdad de la variable especinterP la cual se usa para determinar si se 
        /// debe calcular o no el espectro a la traza en el panel de interpolación, graficamente este 
        /// cambio se nota porque el botón boEspInterP obriene un color diferente, verde cuando especinterP
        /// es true y rosa cuando especinterP es false.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void boEspInterP_MouseDown(object sender, MouseEventArgs e)
        {
            if (especinterP == false)
            {
                especinterP = true;
                boEspInterP.BackColor = Color.Green;
            }
            else
            {
                especinterP = false;
                boEspInterP.BackColor = Color.MistyRose;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nmi"></param>
        /// <param name="npp"></param>
        void DosEspectros(int nmi, int npp)
        {
            int i, j, k, nmf, x, y, xf, yf, iniy;
            int numdat;
            double fax, fay, fy, rata, facra, delta, frecmx, mx, dd;
            Color col;
            Point[] dat;

            nmf = nmi + npp;
            rata = ra[id] * facRaInterp;

            vaesp = new double[npp];
            for (i = 0; i < npp; i++)
                vaesp[i] = spl[i + nmi] - promInterp; // aqui a veces el programa se  vuela !!
            vaesp = four.RealFFTAmpli(vaesp, true);

            vacioesp = false;
            facra = 1.0 / rata;
            for (i = nmi + 1; i < nmi + npp; i++)
            {
                if (timspl[i] - timspl[i - 1] > (facra + facra * 0.5))
                {
                    vacioesp = true;
                    break;
                }
            }
            if (vacioesp == true) return;  // provisional

            xf = panelEspectros.Width - 20;
            yf = panelEspectros.Height - 20;
            fay = yf / 2.5;

            frecmx = ra[id] / 2.0;
            delta = rata / (double)(npp);
            numdat = (int)(frecmx / delta);
            fax = frecmx / (double)(xf);
            mx = vaesp[0];
            for (i = 1; i < numdat; i++)
            {
                if (mx < vaesp[i]) mx = vaesp[i];
            }
            if (mx <= 0) return;
            fy = fay / mx;

            Graphics dc = panelEspectros.CreateGraphics();
            SolidBrush bro = new SolidBrush(Color.Lavender);
            dc.FillRectangle(bro, 0, 0, panelEspectros.Width, panelEspectros.Height);
            bro.Dispose();
            Pen lap = new Pen(Color.Black, 1);

            dat = new Point[numdat - 1];

            dd = 10.0;
            fax = xf / (double)(numdat);
            iniy = (int)(yf / 2.2);
            j = 0;
            for (i = 1; i < numdat; i++)
            {
                x = (int)(dd);
                y = iniy - (int)(vaesp[i] * fy);
                dd += fax;
                dat[j].Y = y;
                dat[j++].X = x;
            }
            dc.DrawLines(lap, dat);

            vaesp = new double[npp];
            for (i = 0; i < npp; i++) vaesp[i] = dzp[i + nmi] - promDesplz; // ojo aqui a veces aborta el programa !!
            vaesp = four.RealFFTAmpli(vaesp, true);

            mx = vaesp[0];
            for (i = 1; i < numdat; i++)
            {
                if (mx < vaesp[i]) mx = vaesp[i];
            }
            if (mx <= 0) return;
            fy = fay / mx;

            dd = 10.0;
            iniy = (int)(yf);
            j = 0;
            for (i = 1; i < numdat; i++)
            {
                x = (int)(dd);
                y = iniy - (int)(vaesp[i] * fy);
                dd += fax;
                dat[j].Y = y;
                dat[j++].X = x;
            }
            Pen la = new Pen(Color.DarkBlue, 1);
            dc.DrawLines(la, dat);

            lap.Dispose();

            fax = (double)(xf) / frecmx;
            for (i = 0; i <= (int)(frecmx); i++)
            {
                dd = 10.0 + i * fax;
                x = (int)(dd);
                col = Color.Magenta;
                if (Math.IEEERemainder(i, 10) == 0)
                {
                    col = Color.Blue;
                    k = 7;
                }
                else if (Math.IEEERemainder(i, 5) == 0) k = 5;
                else k = 2;
                Pen lp = new Pen(col, 1);
                dc.DrawLine(lp, x, 0, x, k);

                lp.Dispose();
            }

            if (ipb1 != ip1 || ipb2 != ip2)
            {
                panelBarEspInterp.Visible = false;
                return;
            }
            // se dibuja la Barra indicativa del sector de traza para el calculo espectral
            xf = panelInterP.Width - 20;
            fax = xf / (double)(spl.Length);
            x = 10 + (int)(nmi * fax);
            y = 10 + (int)(nmf * fax);
            panelBarEspInterp.Visible = true;
            panelBarEspInterp.Location = new Point(x, panelBarEspInterp.Location.Y);
            panelBarEspInterp.Size = new Size((y - x), panelBarEspInterp.Height);

            return;
        }
/// <summary>
/// no hace nada
/// </summary>
/// <param name="sender"></param>
/// <param name="e"></param>
        private void panelEspectros_MouseDown(object sender, MouseEventArgs e)
        {

        }
        /// <summary>
        /// Grafica una linea vertical que sirve como guia en el panel.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void panelEspectros_MouseUp(object sender, MouseEventArgs e)
        {
            if (guiainterp == true)
            {
                Graphics dc = panelEspectros.CreateGraphics();
                Pen lap = new Pen(Color.BlueViolet, 1);
                dc.DrawLine(lap, e.X, 5, e.X, panelEspectros.Height - 5);
                lap.Dispose();
            }
        }
        /// <summary>
        /// no hace nada.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void boObserva_MouseDown(object sender, MouseEventArgs e)
        {
        }
        /// <summary>
        /// Sobre el mapa que se despliega después de marcar el botón vista y dar click sobre un sismo en el panel principal,
        /// es allí donde al momento de dar click sobre dicho mapa se despliega un tooTip mostrando la información de 
        /// localización (latitud y longitud) de ese punto del mapa. 
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void splitContainer1_Panel2_MouseDown(object sender, MouseEventArgs e)//PANEL DEL MAPA
        {
            int xf1, yf1, x1, y1, xf, yf, iniX, iniY, grala, gralo;
            double dif, laa2, loo2, laa, loo, km, laa1, loo1, minla, minlo, dd;
            double fcpi, fclo, disla, dislo, fcdislo;
            string ss = "";


            fcpi = Math.PI / 180.0;
            fcdislo = (Math.PI / 180.0) * Math.Cos(latitud * fcpi) * 6367.449;
            fclo = famap * ((Math.PI / 180.0) * Math.Cos(latitud * fcpi) * 6367.449) / 110.9;

            xf1 = e.X;
            yf1 = e.Y;
            x1 = xf1 - e.X;
            y1 = yf1 - e.Y;
            dif = Math.Sqrt(x1 * x1 + y1 * y1);
            xf = panel1.Size.Width;
            yf = panel1.Size.Height;
            iniX = xf / 2;
            iniY = yf / 2;
            laa1 = latitud + ((iniY - e.Y) / famap);
            loo1 = longitud + ((e.X - iniX) / fclo);
            laa2 = latitud + ((iniY - yf1) / famap);
            loo2 = longitud + ((xf1 - iniX) / fclo);
            if (laa1 > laa2)
            {
                dd = laa1;
                laa1 = laa2;
                laa2 = dd;
            }
            if (loo1 > loo2)
            {
                dd = loo1;
                loo1 = loo2;
                loo2 = dd;
            }
            //if (ss.Length == 0 || verSis == false)
            {
                grala = (int)(laa1);
                minla = (laa1 - grala) * 60.0;
                gralo = (int)(loo1);
                minlo = (loo1 - gralo) * 60.0;
                laa = laa1 - laa2;
                loo = loo1 - loo2;
                disla = laa * 110.9;
                dislo = loo * fcdislo;
                km = Math.Sqrt(disla * disla + dislo * dislo);
                if (dif < 5.0) ss = string.Format("{0:00}º {1:00.00}' ({4:0.000000})\n{2:00}º {3:00.00}' ({5:0.000000})", grala, minla, gralo, minlo, laa1, loo1);
                else ss = string.Format("{0:00.00} Km", km);
            }
            tip.IsBalloon = true;
            tip.InitialDelay = 0;
            tip.ReshowDelay = 0;
            tip.AutomaticDelay = 0;
            tip.SetToolTip(splitContainer1.Panel2, ss);
        }
        /// <summary>
        /// Este evento se lanza cuando se desactiva el Form1.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void Form1_Deactivate(object sender, EventArgs e)
        {
            if (inicio == false)
                return;
            if (sidesactiva == true)
                desactivado = true;
        }
        /// <summary>
        /// Asigna el valor a la varialbe usu (usuario) mediante la clase Usuario que despliega una interfaz gráfica
        /// que captura el nombre de usuario.
        /// </summary>
        void Param()
        {
            int i;

            Usuario di = new Usuario();

            di.Usua = usu;
            if (di.ShowDialog() == DialogResult.OK)
            {
                usu = di.Usua;
            }
            i = usu.Length;
            if (i != 3) usu = "___";

            return;
        }
        /// <summary>
        /// Despliega u oculta el panel panelFilt el cual muestra los botones para elegir el tipo de filtro a aplicar 
        /// a la porción de traza que se esta clasificando.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void boFIR_MouseDown(object sender, MouseEventArgs e)
        {

            //int yf, iniy;
            ///double fay;

            if (e.Button == MouseButtons.Left)
            {
                cfilt = '0';
                if (panelFilt.Visible == false)
                {
                    panelFilt.Visible = true;
                    panelFilt.BringToFront();
                    boFIR.BackColor = Color.Gold;
                }
                else
                {
                    sifilt = false;
                    panelFilt.Visible = false;
                    boFIR.BackColor = Color.White;
                    DibujoTrazas();
                }
                textBox1.BackColor = Color.White;
                textBox2.BackColor = Color.White;
                boFilBaj.BackColor = Color.White;
                boFilAlt.BackColor = Color.White;
                boFilBan.BackColor = Color.White;
                // paneltra.Invalidate();
                //if (panelFFTzoom.Visible == true) panelFFTzoom.Invalidate();
            }

        }
        /// <summary>
        /// Se encarga de aplicar un filtro a TODAS las trazas, dicho filtro depende del valor de la variable cfilt,
        /// si cfilt = 1 aplica pasa altos,
        /// si es 2 aplica pasa bajos,
        /// si es 3 aplica pasa banda,
        /// al final determina los valores máximo y mínimo de cuenta de cada traza después de pasar por el filtro y
        /// estos valores quedan almacenados en mxF[] y mnF[] respectivamente donde cada posición de estos vectores
        /// están asociados a la posición de su respectiva traza en la matriz cu[].
        /// </summary>
        void AplicarFiltro()
        {
            int i, j, k, nmi, nmf;
            int[][] ccf;

            panel2.Visible = true;
            util.Mensaje(panel2, "Calculando Filtro ...", false);

            if (yafilt == false)
            {
                mxF = new int[nutra];
                mnF = new int[nutra];
                cff = new int[nutra][];
                for (i = 0; i < nutra; i++)
                    cff[i] = new int[cu[i].Length];
                yafilt = true;
            }

            ccf = new int[nutra][];
            for (i = 0; i < nutra; i++)
            {
                nmi = (int)((tie1 - tim[i][0]) * ra[i]);                 // muestra inferior del panel de clasificacion
                nmf = (int)((tie2 - tim[i][0]) * ra[i]);                 // muestra superior
                if ((int)(nmf) > tim[i].Length || nmf < 0)
                {
                    ccf[i] = new int[1];
                    ccf[i][0] = 0;
                }
                else
                {
                    ccf[i] = new int[nmf - nmi];
                    k = 0;
                    for (j = nmi; j < nmf - 1; j++)
                    {
                        ccf[i][k++] = cu[i][j];
                    }
                }
            }

            if (cfilt == '1')
                for (i = 0; i < nutra; i++)
                    cff[i] = util.PasaBajos(ccf[i], M, (float)(ra[i]), Fc1);
            else if (cfilt == '2')
                for (i = 0; i < nutra; i++)
                    cff[i] = util.PasaAltos(ccf[i], M, (float)(ra[i]), Fc1);
            else if (cfilt == '3')
                for (i = 0; i < nutra; i++)
                    cff[i] = util.PasaBanda(ccf[i], M, (float)(ra[i]), Fc1, Fc2);
            else
            {
                panel2.Visible = false;
                return;
            }

            for (i = 0; i < nutra; i++)
            {
                mxF[i] = cff[i][M + 1];
                mnF[i] = mxF[i];
                for (j = M + 1; j < cff[i].Length - M; j++)
                {
                    if (mxF[i] < cff[i][j]) 
                        mxF[i] = cff[i][j];
                    else if (mnF[i] > cff[i][j]) 
                        mnF[i] = cff[i][j];
                }
            }
            panel2.Visible = false;
        }
        /// <summary>
        /// Modifica el valor de la variable M entre 128, 256 y 512, dicha variable se pasa como argumento
        /// a los métodos que calculan filtros, además segun su valor cambia el color de los  
        /// texbox1 y texbox2, cabe aclarar que dichos textbox no son los que se visualizan en el panelFilt
        /// donde si estan los botones de filtros, estos textbox son los que aparecen en el segundo panel que se usa para
        /// visualizar, trazas el panel panel1a.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void boM_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                M = (short)(M * 2);
            else
                M = (short)(M / 2.0);
            if (M > 512)
                M = 128;
            else if (M < 128)
                M = 512;
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
        /// <summary>
        /// Controla la solicitud de cálculo del filtro pasa bajos en el panelcladib y
        /// llama el método encargado de dibujar las trazas nuevas a partir de la aplicación del filtro.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void boFilBaj_Click(object sender, EventArgs e)
        {
            if (cfilt != '1')
                yafilt = false;
            cfilt = '1';
            if (sifilt == false)
            {
                sifilt = true;
                if (yafilt == false)
                    AplicarFiltro();
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
                textBox3.BackColor = Color.White;
                textBox4.BackColor = Color.White;
            }
            DibujoTrazas();
        }
        /// <summary>
        /// Controla la solicitud de cálculo del filtro pasa altos en el panelcladib y
        /// llama el método encargado de dibujar las trazas nuevas a partir de la aplicación del filtro.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void boFilAlt_Click(object sender, EventArgs e)
        {
            if (cfilt != '2') yafilt = false;
            cfilt = '2';
            if (sifilt == false)
            {
                sifilt = true;
                if (yafilt == false)
                    AplicarFiltro();
                boFilBaj.BackColor = Color.White;
                boFilAlt.BackColor = Color.Green;
                boFilBan.BackColor = Color.White;
                textBox3.BackColor = Color.Orange;
                textBox4.BackColor = Color.White;
            }
            else
            {
                sifilt = false;
                boFilBaj.BackColor = Color.White;
                boFilAlt.BackColor = Color.White;
                boFilBan.BackColor = Color.White;
                textBox3.BackColor = Color.White;
                textBox4.BackColor = Color.White;
            }
            DibujoTrazas();
        }
        /// <summary>
        /// Controla la solicitud de cálculo del filtro pasa banda en el panelcladib y
        /// llama el método encargado de dibujar las trazas nuevas a partir de la aplicación del filtro.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void boFilBan_Click(object sender, EventArgs e)
        {
            if (cfilt != '3')
                yafilt = false;
            cfilt = '3';
            if (sifilt == false)
            {
                sifilt = true;
                if (yafilt == false)
                    AplicarFiltro();
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
            DibujoTrazas();
        }
        /// <summary>
        /// Modifica el valor de la frecuencia de corte 1 Fc1 utilizada en los filtros 
        /// del panelcladib.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            float ff;

            try
            {
                ff = float.Parse(textBox3.Text);
            }
            catch
            {
                return;
            }
            Fc1 = ff;
            yafilt = false;
            cfilt = '0';
            textBox3.BackColor = Color.White;
            textBox4.BackColor = Color.White;
        }
        /// <summary>
        /// Verifica que el valor escrito en el textBox3 cumpla con el formato 
        /// que se necesita para el cálculo de los filtros.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void textBox3_Validated(object sender, EventArgs e)
        {
            textBox3.Text = string.Format("{0:00.00}", Fc1);
        }
        /// <summary>
        /// Modifica el valor de la frecuencia de corte 2 Fc2 utilizada en los filtros 
        /// del panelcladib.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            float ff;

            try
            {
                ff = float.Parse(textBox4.Text);
            }
            catch
            {
                return;
            }
            Fc2 = ff;
            yafilt = false;
            cfilt = '0';
            textBox3.BackColor = Color.White;
            textBox4.BackColor = Color.White;
        }
        /// <summary>
        /// Verifica que el valor escrito en el textBox4 cumpla con el formato 
        /// que se necesita para el cálculo de los filtros.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox4_Validated(object sender, EventArgs e)
        {
            textBox4.Text = string.Format("{0:00.00}", Fc2);
        }
        /// <summary>
        /// Asigna el valor encontrado en el textBox3 a la frcuencia de corte 1 Fc1.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void textBox3_KeyDown(object sender, KeyEventArgs e)
        {
            float ff;

            cfilt = '0';
            if (e.KeyCode == Keys.Enter)
            {
                try
                {
                    ff = float.Parse(textBox3.Text);
                }
                catch
                {
                    return;
                }
                if (ff < Fc2)
                    Fc1 = ff;
                textBox3.Text = string.Format("{0:00.00}", Fc1);
            }
        }
        /// <summary>
        /// Asigna el valor encontrado en el textBox4 a la frcuencia de corte 2 Fc2.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void textBox4_KeyDown(object sender, KeyEventArgs e)
        {
            float ff;

            cfilt = '0';
            if (e.KeyCode == Keys.Enter)
            {
                try
                {
                    ff = float.Parse(textBox4.Text);
                }
                catch
                {
                    return;
                }
                if (ff > Fc1) Fc2 = ff;
                textBox4.Text = string.Format("{0:00.00}", Fc2);
            }
        }
        /// <summary>
        /// El botón boMptIntp controla el valor de la variable particula con la cual se determina si se calcula o 
        /// no el movimiento de particulas a una porción de traza interpolada.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void boMptIntp_MouseDown(object sender, MouseEventArgs e)
        {
            suma = 0;
            yaInterp = false;
            if (panelParti.Visible == false)
            {
                particula = true;
                boMptIntp.BackColor = Color.Aquamarine;
                especinterP = false;
                boEspInterP.BackColor = Color.MistyRose;
            }
            else
            {
                panelParti.Visible = false;
                particula = false;
                boMptIntp.BackColor = Color.LavenderBlush;
            }
        }
        /// <summary>
        /// Desplaza la porción de traza que se ve en los paneles de cada componente(N, E, Z) 
        /// en el panel de movimieto de particulas haacia la derecha.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void boDerMpt_MouseDown(object sender, MouseEventArgs e)
        {
            int i = 0, mi = 0;
            double fac = 1.0, incr = 0.05;

            if (e.Button == MouseButtons.Left) incr = 0.05;
            else incr = 0.01;
            if (mptintp == true)
            {
                fac = facRaInterp;
                mi = (int)((timpt - timspl[0]) * ra[Z] * fac);
            }
            else
            {
                fac = 1.0;
                if (sifilt == false) mi = (int)((timpt - tim[Z][0]) * ra[Z]);
                else mi = (int)((timpt - tie1) * ra[Z]);
            }
            i = (int)(-1.0 * durmpt * incr * ra[Z] * fac) + suma;
            mi += i;
            if (mi < 0) return;
            suma = i;
            if (mptintp == false) TrazaComponente();
            else TrazaComponenteInterp();
        }
        /// <summary>
        /// Desplaza la porción de traza que se ve en los paneles de cada componente(N, E, Z) 
        /// en el panel de movimieto de particulas haacia la izquierda.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void boIzqMpt_MouseDown(object sender, MouseEventArgs e)
        {
            int i = 0;
            double fac = 1.0, incr = 0.05;

            if (e.Button == MouseButtons.Left) incr = 0.05;
            else incr = 0.01;
            if (mptintp == true)
            {
                fac = facRaInterp;
            }
            else
            {
                fac = 1.0;
            }
            i = (int)(durmpt * 0.05 * ra[Z] * fac) + suma;
            suma = i;
            if (mptintp == false)
                TrazaComponente();
            else
                TrazaComponenteInterp();
        }
        /// <summary>
        /// Controla la visualización del espectro en el panelcladib.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void boEspCla_MouseDown(object sender, MouseEventArgs e)
        {
            if (VerEspectro == false)
            {
                VerEspectro = true;
                boEspCla.BackColor = Color.ForestGreen;
            }
            else
            {
                VerEspectro = false;
                panelFFTzoom.Visible = false;
                moveresp = false;
                movespcla = false;
                boEspCla.BackColor = Color.White;
                boEspe.BackColor = Color.WhiteSmoke;
                if (panelcladib.Visible == true)
                    TrazasClas();
                yloc = -1;
            }
        }
        /// <summary>
        /// Se encarga de definir el tipo de método que se va a utilizar para el cálculo de desplazamiento reducido en una porción de traza,
        /// o en caso de que ya se haya calculado se indica que no se va a calcular más, esto lo hace modificando el valor de la variable DR entre:
        /// 0 (no se calcula)
        /// 1 (se utiliza el método 1)
        /// 2 (se utiliza el método 2).
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void boDR_MouseDown(object sender, MouseEventArgs e)
        {
            string ss = "";
            if (fcnan[id] <= 0)
            {
                ss = "NO hay Factor!..";
                tip.IsBalloon = false;
                tip.AutoPopDelay = 3000;
                tip.SetToolTip(boDR, ss);
                panelDR.Visible = false;
                DR = 0;
                boDR.BackColor = Color.Gold;
                return;
            }
            if (e.Button == MouseButtons.Left)
            {
                if (DR == 0)
                {
                    DR = 1;
                    boDR.BackColor = Color.Green;
                    ss = "Arrastre el sector de traza que desea Integrar";
                    tip.IsBalloon = false;
                    tip.AutoPopDelay = 3000;
                    tip.SetToolTip(boDR, ss);
                    promDR = promEst[id];
                }
                else
                {
                    panelDR.Visible = false;
                    DR = 0;
                    boDR.BackColor = Color.Gold;
                }
            }
            else
            {
                if (DR == 0)
                {
                    boDR.BackColor = Color.Magenta;
                    DR = 2;
                    ss = "Arrastre el sector de traza para calculo DR";
                    tip.IsBalloon = false;
                    tip.AutoPopDelay = 3000;
                    tip.SetToolTip(boDR, ss);
                    //nom = ".\\pro\\fcms" + tar[id] + ".txt";
                    //if (File.Exists(nom)) MessageBox.Show("est=" + est[id] + " tar=" + tar[id].ToString() + " nom=" + nom);
                }
                else
                {
                    panelDR.Visible = false;
                    DR = 0;
                    boDR.BackColor = Color.Gold;
                }
            }
        }
        /// <summary>
        /// Integra la señal que se está calculando desplazamiento reducido, la muestra en el panelDR,
        /// cálcula la distancia en kilómetros del volcán asociado a la estación y muestra los datos de desplazamiento reducido en el panelDR.
        /// En el manual de proceso20 el método UnaEstacionDesplazamiento es el que se describe como método uno para obtención de desplazamiento reducido.
        /// </summary>
        /// <param name="cuu">Arreglo que con los valores de cuentas de la traza que se esta visualizando.</param>
        void UnaEstacionDesplazamiento(int[] cuu)
        {
            int i, j, k, nmi, nmf, xf, yf, iniy;
            double facra, fax, fy, pro, fcpi, fcdislo, disla = 0, dislo = 0, km;
            double[] cDR;
            string ca;
            Point[] dat;

            try
            {
                panelDR.Visible = true;
                panelDR.BringToFront();
                nmi = (int)((tDR1 - tim[id][0]) * ra[id]);
                if (nmi < 0) 
                    nmi = 0;
                nmf = (int)((tDR2 - tim[id][0]) * ra[id]);
                if (nmf > cuu.Length) 
                    nmf = cuu.Length;
                if (nmf - nmi < 10) 
                    return;
                cDR = new double[nmf - nmi];
                zDR = new double[nmf - nmi];
                k = 0;
                for (i = nmi; i < nmf; i++)
                    cDR[k++] = (double)(cuu[i] - promDR);//valores de la cuenta menos el promedio
                facra = 1.0 / ra[id];
                zDR[0] = 0;
                for (i = 1; i < cDR.Length; i++) 
                    zDR[i] = zDR[i - 1] + ((cDR[i - 1] + cDR[i]) / 2.0) * facra; // integración de la señal
                //MessageBox.Show("id=" + id.ToString() + " nmi="+nmi.ToString()+" nmf="+nmf.ToString()+" prom="+promEst[id].ToString()+"   "+est[id]);
                xf = panelDR.Width - 80;
                yf = panelDR.Height - 30;
                fax = xf / (tim[id][nmf] - tim[id][nmi]);
                mxz = zDR[0];
                mnz = zDR[0];
                /// determina los valores máximo y mínimo de la señal integrada
                for (i = 0; i < zDR.Length; i++)
                {
                    if (mxz < zDR[i]) 
                        mxz = zDR[i];
                    else if (mnz > zDR[i]) 
                        mnz = zDR[i];
                }
                pro = (mxz + mnz) / 2.0;
                iniy = 10 + (int)((yf / 2.0));
                fy = (yf / 2.5) / (mxz - pro);

                Graphics dc = panelDR.CreateGraphics();
                SolidBrush brr = new SolidBrush(Color.Lavender);
                dc.FillRectangle(brr, 0, 0, panelDR.Width, panelDR.Height);
                brr.Dispose();
                Pen lap = new Pen(Color.DarkRed, 1);

                dat = new Point[zDR.Length];
                for (i = 0; i < zDR.Length; i++)
                {
                    dat[i].X = (int)(70 + (tim[id][nmi + i] - tim[id][nmi]) * fax);
                    dat[i].Y = (int)(iniy + (pro - zDR[i]) * fy);
                }
                dc.DrawLines(lap, dat);
                lap.Dispose();

                SolidBrush br = new SolidBrush(Color.Blue);
                dc.DrawString(est[id].Substring(0, 4), new Font("Times New Roman", 10, FontStyle.Bold), br, 2, (int)(yf / 2.0));

                if (VD[id] == '*') 
                    return;
                j = -1;
                for (i = 0; i < nuvol; i++)
                {
                    if (volcan[i][0] == VD[id])
                    {
                        j = i;
                        break;
                    }
                }
                if (j == -1) 
                    return;
                fcpi = Math.PI / 180.0;
                fcdislo = fcpi * Math.Cos(laD[id] * fcpi) * 6367.449;
                for (i = 0; i <= nuvol; i++)
                {
                    if (volcan[i][0] == VD[id])
                    {
                        disla = Math.Abs(latvol[i] - laD[id]) * 110.9;
                        dislo = Math.Abs(lonvol[i] - loD[id]) * fcdislo;
                        break;
                    }
                }
                km = Math.Sqrt(disla * disla + dislo * dislo);
                ca = volcan[j] + " " + string.Format("({0:0.0}km)", km);
                dc.DrawString(ca, new Font("Lucida Console", 12, FontStyle.Bold), br, 50, 2);
                br.Dispose();
                EscriDR();
            }
            catch
            {
            }

            return;
        }
        /// <summary>
        /// Determina el promedio de velocidad del desplazamiento reducido (proVelDR) el cual se utiliza
        /// en el cálculo del desplazamiento reducido. En el manual de proceso20 el método DibujoVelocidadDR
        /// es el que se describe como método 2 para obtención de desplazamiento reducido.
        /// </summary>
        void DibujoVelocidadDR()
        {
            int xf, yf;
            int i, k, nmi, nmf, mmx, mmn, iniy;
            float x1, y1;
            double fax, fy;
            Point[] dat;

            nmi = (int)((tDR1 - tim[id][0]) * ra[id]); //determina inicio del arrastre
            if (nmi < 0) nmi = 0;
            nmf = (int)((tDR2 - tim[id][0]) * ra[id]); //determina final del arrastre
            if (nmf > cu[id].Length) 
                nmf = cu[id].Length;
            if (nmf - nmi < 5)
                return;
            //facra = 1.0 / ra[id];
            xf = panelDR.Width - 80;
            yf = panelDR.Height - 30;
            fax = xf / (tim[id][nmf] - tim[id][nmi]); // ACA BOTA ERROR DE INDICE DE MATRIZ
            //determina los valores máximo y mínimo en el intervalo de traza arrastrado
            mmx = cu[id][nmi];
            mmn = mmx;
            for (i = nmi + 1; i < nmf; i++)
            {
                if (mmx < cu[id][i]) mmx = cu[id][i];
                else if (mmn > cu[id][i]) mmn = cu[id][i];
            }
            proVelDR = (int)((mmx + mmn) / 2.0);
            if (mmx - proVelDR != 0) 
                fy = ((yf / 2.5) / ((mmx - proVelDR)));
            else 
                fy = 1;
            iniy = (int)(yf / 2.0) + 15;

            Graphics dc = panelDR.CreateGraphics();
            SolidBrush brr = new SolidBrush(Color.NavajoWhite);
            dc.FillRectangle(brr, 0, 0, panelDR.Width, panelDR.Height);
            brr.Dispose();
            Pen lap = new Pen(Color.Red, 1);
            dat = new Point[nmf - nmi];
            k = 0;
            for (i = nmi; i < nmf; i++)
            {
                y1 = (float)(iniy - (cu[id][i] - proVelDR) * fy);
                x1 = (float)(70.0 + (tim[id][i] - tim[id][nmi]) * fax);
                dat[k].Y = (int)y1;
                dat[k++].X = (int)x1;
            }
            dc.DrawLines(lap, dat);
            lap.Dispose();
            return;
        }
        /// <summary>
        /// Es el encargado de la actualización gráfica del panelDR,dependiendo el valor de la variable
        /// DR (puede ser 1, 2, 0) aplica un método distinto para el calculo del desplazamiento reducido.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void panelDR_Paint(object sender, PaintEventArgs e)
        {
            if (DR == 1)
            {
                boMasDR.Visible = true;
                boMasmasDR.Visible = true;
                boMenosDR.Visible = true;
                boMenosmenosDR.Visible = true;
                checkBoxHz.Visible = true;
                panelDR.BackColor = Color.Lavender;
                if (checkBoxHz.Checked == true) 
                    UnaEstacionDesplazamiento(cfD);
                else 
                    UnaEstacionDesplazamiento(cu[id]);
            }
            else if (DR == 2)
            {
                boMasDR.Visible = false;
                boMasmasDR.Visible = false;
                boMenosDR.Visible = false;
                boMenosmenosDR.Visible = false;
                checkBoxHz.Visible = false;
                panelDR.BackColor = Color.NavajoWhite;
                DibujoVelocidadDR();
            }
        }
        /// <summary>
        /// En caso de que el click sea con el botón izquierdo del mouse resta 1 al valor de promDR,
        /// si el click es con el botón derecho se resta 10 a promDR.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void boMasDR_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) promDR -= 1.0;
            else promDR -= 10.0;
            panelDR.Invalidate();
        }
        /// <summary>
        /// En caso de que el click sea con el botón izquierdo del mouse agrega 1 al valor de promDR,
        /// si el click es con el botón derecho se agrega 10 a promDR.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void boMenosDR_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                promDR += 1.0;
            else promDR += 10.0;
            panelDR.Invalidate();
        }
        /// <summary>
        /// Esconde el panelDR y cambia el valor de valDR a 0. 
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void boXDR_Click(object sender, EventArgs e)
        {
            panelDR.Visible = false;
            valDR = 0;
        }
        /// <summary>
        /// Si se da click sobre el botón con el click izquierdo resta 50 al valor de promDR,
        /// si se da con el click derecho se resta 500 al valor de promDR.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void boMasmasDR_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) promDR -= 50.0;
            else promDR -= 500.0;
            panelDR.Invalidate();
        }
        /// <summary>
        /// Si se da click sobre el botón con el click izquierdo agrega 50 al valor de promDR,
        /// si se da con el click derecho se agrega 500 al valor de promDR.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void boMenosmenosDR_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                promDR += 50.0;
            else
                promDR += 500.0;
            panelDR.Invalidate();
        }
        /// <summary>
        /// Se lanza cuando se da click sobre el panel panelDR y asigna los valores a xiDR = x donde se dio el click,
        /// yiDR = y donde se dio el click.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void panelDR_MouseDown(object sender, MouseEventArgs e)
        {
            xiDR = e.X;
            yiDR = e.Y;
        }
        /// <summary>
        /// En este método básicamente realiza el cálculo del desplazamiento reducido por los dos métodos mencionados en el manual de proceso20,
        /// cuando DR==1 lo hace por el método 1, pero cuando DR== 2 lo hace por el método 2, cada uno de ellos tratando los datos de la traza de
        /// formas diferentes, finalmente dibuja el resultado en el panelDR.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void panelDR_MouseUp(object sender, MouseEventArgs e)
        {
            int i, k, xf, yf, nmi, nmf, ni, nf, iniy, mxx, mnn;
            double km, cm, disla = 0, dislo = 0, fcpi, fcdislo;
            double fax, fy, t1, t2, tt, max = 0, min = 0, pro, frms, fac;
            Point[] dat;
            int dfcu, ccu;
            double pper;
            //int j, jb, mmx, mmn;
            //double dcm, ff, fdr, fdr2, latE, lonE, latV, lonV;
            //double fcpi, fcdislo, disla, dislo;
            double dr1, dr2;
            string ca;
            //char[] delim = { ' ', '\t' };
            //string[] pa = null;
            //string ca, lin = "";


            if (VD[id] == '*') 
                return;
            try
            {
                if (DR == 1)
                {
                    if (checkBoxHz.Checked == true) 
                        UnaEstacionDesplazamiento(cfD);
                    else
                        UnaEstacionDesplazamiento(cu[id]);
                }
                fcpi = Math.PI / 180.0; //factor pi
                fcdislo = fcpi * Math.Cos(laD[id] * fcpi) * 6367.449; // factor  longitud
                for (i = 0; i <= nuvol; i++)
                {
                    if (volcan[i][0] == VD[id])
                    {
                        disla = Math.Abs(latvol[i] - laD[id]) * 110.9;
                        dislo = Math.Abs(lonvol[i] - loD[id]) * fcdislo;
                        break;
                    }
                }
                km = Math.Sqrt(disla * disla + dislo * dislo);
                cm = km * 100000.0;

                xf = panelDR.Width - 80;
                yf = panelDR.Height - 30;

                Graphics dc = panelDR.CreateGraphics();
                Pen lap = new Pen(Color.DarkOrange, 1);

                nmi = (int)((tDR1 - tim[id][0]) * ra[id]);
                if (nmi < 0) 
                    nmi = 0;
                nmf = (int)((tDR2 - tim[id][0]) * ra[id]);
                if (nmf > cu[id].Length) 
                    nmf = cu[id].Length;
                if (nmf - nmi < 5) 
                    return;
                fax = xf / (tim[id][nmf] - tim[id][nmi]);
                t1 = tim[id][nmi] + (xiDR - 70.0) / fax;
                t2 = tim[id][nmi] + (e.X - 70.0) / fax;
                if (t1 > t2)
                {
                    tt = t1;
                    t1 = t2;
                    t2 = tt;
                }
                ni = (int)((t1 - tim[id][nmi]) * ra[id]);
                nf = (int)((t2 - tim[id][nmi]) * ra[id]);

                mxx = cu[id][nmi];
                mnn = mxx;
                for (i = nmi + 1; i < nmf; i++)
                {
                    if (mxx < cu[id][i]) 
                        mxx = cu[id][i];
                    else if (mnn > cu[id][i]) 
                        mnn = cu[id][i];
                }
                dat = new Point[nf - ni];

                if (DR == 1)
                {
                    max = zDR[0];
                    min = max;
                    for (i = ni + 1; i < nf; i++)
                    {
                        if (max < zDR[i]) max = zDR[i];
                        else if (min > zDR[i]) min = zDR[i];
                    }
                    pro = (mxz + mnz) / 2.0;
                    iniy = 10 + (int)((yf / 2.0));
                    fy = (yf / 2.5) / (mxz - pro);
                    k = 0;
                    for (i = ni; i < nf; i++)
                    {
                        dat[k].X = (int)(70 + (tim[id][nmi + i] - tim[id][nmi]) * fax);
                        dat[k++].Y = (int)(iniy + (pro - zDR[i]) * fy);
                    }
                    dc.DrawLines(lap, dat);

                    frms = 1.0 / (2.0 * Math.Sqrt(2.0));
                    microDR = (((max - min) * fcnan[id]) / (double)(ga[id])) * 0.001;// conversion a micrometros
                    valDR =   (((max - min) * fcnan[id] / (double)(ga[id])) * 0.0000001) * frms * cm;
                    fac = cm / (4.0 * Math.PI * Math.Sqrt(2.0) * 10000000.0 / fcnan[id]);
                    EscriDR();
                }
                else if (DR == 2 && fcDR[id] > 0)
                {
                    max = cu[id][nmi + ni];
                    min = max;
                    for (i = nmi + ni + 1; i < nmi + nf; i++)
                    {
                        if (max < cu[id][i]) max = cu[id][i];
                        else if (min > cu[id][i]) min = cu[id][i];
                    }

                    iniy = 15 + (int)((yf / 2.0));
                    fy = (yf / 2.5) / (mxx - proVelDR);
                    k = 0;
                    for (i = ni; i < nf; i++)
                    {
                        dat[k].X = (int)(70 + (tim[id][nmi + i] - tim[id][nmi]) * fax);
                        dat[k++].Y = (int)(iniy + (proVelDR - cu[id][nmi + i]) * fy);
                    }
                    dc.DrawLines(lap, dat);
                    pper = tim[id][nmi + nf] - tim[id][nmi + ni];
                    ccu = (int)(max - min);
                    dfcu = (int)((max - min) / (double)(ga[id]));
                    dr1 = dfcu * pper * fcDR[id];
                    //dr1 = dfcu * pper * fdr;
                    //dr2 = dfcu * pper * fdr2;
                    SolidBrush br0 = new SolidBrush(Color.NavajoWhite);
                    dc.FillRectangle(br0, 0, 0, 600, 30);
                    br0.Dispose();
                    SolidBrush br = new SolidBrush(Color.Magenta);
                    //ca=string.Format("DR: {0:0.00}     fac= {1:0.000000000}  ",dr2,fdr2)+tar[id]+"  ff="+ff.ToString();
                    ca = string.Format("DR: {0:0.00} cm^2     Per: {1:0.00}s   cu: {2:0} ga: {3:0}     [fac= {4:0.000000000}  ", dr1, pper, ccu, ga[id], fcDR[id]) + tar[id] + "]";
                    dc.DrawString(ca, new Font("Times New Roman", 12, FontStyle.Bold), br, 30, 3);
                    br.Dispose();
                }
                lap.Dispose();
            }
            catch
            {
            }
        }
        /// <summary>
        /// Escribe el valor de desplazamiento reducido en cm cuadrados y en micrómetros en el panelDR (panel donde se muestra el desplazamiento reducido).
        /// </summary>
        void EscriDR()
        {
            string ca;

            if (valDR <= 0) 
                return;
            Graphics dc = panelDR.CreateGraphics();
            SolidBrush br0 = new SolidBrush(Color.Lavender);
            dc.FillRectangle(br0, 200, 0, 400, 20);
            br0.Dispose();
            SolidBrush br = new SolidBrush(Color.Green);
            ca = "DR= " + string.Format("{0:0.00} cm^2  ({1:0.000} um)", valDR, microDR);
            dc.DrawString(ca, new Font("Lucida Console", 16, FontStyle.Bold), br, 240, 2);
            br.Dispose();
        }
        /// <summary>
        /// Si el botón boTarCarga esta visible lo esconde, reasigna el indice seleccionado del listBox1 = 0
        /// y lanza el método SeleccionarMinuto().
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void boTarCarga_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                panelTar.Visible = false;
            if (listBox1.SelectedIndex < 0)
                listBox1.SelectedIndex = 0;
            SeleccionarMinuto(false);
        }
        /// <summary>
        /// Envia un email desde la dirección alvaropabloacevedo@gmail.com hacia observatoriosingeominas@gmail.com
        /// este método es una prueba del envio de correos desde la aplicación.
        /// </summary>
        void EnviarMensaje0()
        {
            try
            {
                string str_from_address = "alvaropabloacevedo@gmail.com";     //The From address (Email ID)    
                string str_name = "Test Mail";         //The Display Name  
                string str_to_address = "observatoriosingeominas@gmail.com";     //The To address (Email ID)

                //Create MailMessage Object
                MailMessage email_msg = new MailMessage();

                //Specifying From,Sender & Reply to address
                email_msg.From = new MailAddress(str_from_address, str_name);
                email_msg.Sender = new MailAddress(str_from_address, str_name);
                email_msg.ReplyTo = new MailAddress(str_from_address, str_name);

                //The To Email id
                email_msg.To.Add(str_to_address);

                email_msg.Subject = "My Subject";//Subject of email
                email_msg.Body = "This is the body of this message";
                email_msg.Priority = MailPriority.Normal;

                //Create an object for SmtpClient class
                SmtpClient mail_client = new SmtpClient();

                //Providing Credentials (Username & password)
                NetworkCredential network_cdr = new NetworkCredential();
                network_cdr.UserName = str_from_address;
                //network_cdr.Password = "xxxxx";
                //network_cdr.UserName = "observatoriosingeominas";
                network_cdr.Password = "naranjo123";

                mail_client.Host = "smtp.gmail.com"; //SMTP host    
                mail_client.UseDefaultCredentials = false;
                mail_client.Credentials = network_cdr;

                //Now Send the message
                mail_client.Send(email_msg);

                MessageBox.Show("Email Sent Successfully");

            }
            catch (Exception ex)
            {
                //Some error occured
                MessageBox.Show(ex.Message.ToString());
            }
        }
        /// <summary>
        /// Se lanza cuando cambia de estado el checkBox checkBoxHz, en caso de estar seleccionado lanza el método PromedioFiltrado(),
        /// si no esta seleccionado asigna el valor del promedio de la estación activa a la varible promDR.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void checkBoxHz_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxHz.Checked == true)
                PromedioFiltrado();
            else
                promDR = promEst[id];
            panelDR.Invalidate();
        }
        /// <summary>
        /// Esconde el botón boAnotación.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void boAnotacion_Click(object sender, EventArgs e)
        {
            boAnotacion.Visible = false;
        }
        /// <summary>
        /// Al activar el botón Disparo, se presenta un cuadro de diálogo,
        /// donde el usuario debe buscar la carpeta donde se encuentran los sismos de interés y aceptar.
        /// Si todo está bien, aparece a la derecha, el listado de sismos.
        /// Por el momento sólo se acepta el formato SEISAN (tipo Earthworm).
        /// Basta activar el sismo de intrés y continuar como usualmente se trabaja el Proceso.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void boDisparo_MouseDown(object sender, MouseEventArgs e)
        {

            ArrayList lista = new ArrayList();

            lista.Clear();
            listBox1.HorizontalScrollbar = true;

            FolderBrowserDialog d = new FolderBrowserDialog();

            d.ShowNewFolderButton = false;
            d.RootFolder = System.Environment.SpecialFolder.DesktopDirectory;
            d.SelectedPath = rutaDisparo;

            DialogResult response = d.ShowDialog();
            if (response == DialogResult.OK)
            {
                listBox1.Items.Clear();
                string[] files = Directory.GetFiles(d.SelectedPath);
                lista.AddRange(files);
                lista.Sort();
                listBox1.Items.AddRange(lista.ToArray());
                lista.Clear();
            }
        }
        /// <summary>
        /// Determina si el archivo es de tipo SEISAN o si es de tipo SUDS.
        /// </summary>
        /// <returns>el valor en ascci de la primera letra leida del archivo que contiene un sismo,
        /// P en caso de que el archivo sea de tipo SEISAN, o S en caso de que sea de tipo SUDS.</returns>
        int ChequeoArchivoSismo()
        {
            int i;
            char ch;
            string nom;

            nom = listBox1.SelectedItem.ToString();
            FileInfo ar = new FileInfo(nom);
            BinaryReader br = new BinaryReader(ar.OpenRead());
            ch = br.ReadChar();
            br.Close();
            i = (int)ch;
            MessageBox.Show("i=" + i.ToString());
            if (ch == 'P') MessageBox.Show("Seisan");
            else if (ch == 'S') MessageBox.Show("SUDS");
            return (i);
        }
        /// <summary>
        /// Se encarga de llamar al método que lee los archivos de tipo SEISAN, además actualiza el listBox2
        /// con las estaciones que leen SEISAN y configura variables de factores de conversión para posteriores
        /// manipulaciones a las trazas SEISAN.
        /// </summary>
        /// <returns>Si el archivo del sismo es de tipo SEISAN retorna el valor en ascci
        /// de la letra P (80), en caso contrario retorna 0. </returns>
        int LecturaDisparo()
        {
            int i, j, jj, nuu, fe, fe1, fe2;
            long largo, lll;
            char[] delim = { ' ', '\t' };
            string[] pa = null;
            string esta, ca, estinv, li;

            nuu = ChequeoArchivoSismo();
            if (nuu != 80)
            {
                MessageBox.Show("NO es Seisan. Por el momento NO se lee!");
                return (0);
            }
            nutra = 0;
            timin = 0;
            LeeSeisanUno();

            if (nutra == 0)
            {
                return (0);
            }
            timin = tim[0][0]; // tiempo menor
            largo = tim[0].Length - 1;
            timaxmin = tim[0][largo];
            for (i = 1; i < nutra; i++)
            {
                largo = tim[i].Length - 1;
                if (timin > tim[i][0])
                    timin = tim[i][0];
                if (timaxmin > tim[i][largo])
                    timaxmin = tim[i][largo];
            }

            for (i = 0; i < nutra; i++)
            {
                if (est[i] == null)
                {
                    NoMostrar = true;
                    MessageBox.Show("nula...  ii=" + i.ToString() + " est ii-1=" + est[i - 1] + " nutra=" + nutra.ToString());
                }
                else
                {
                    listBox2.Items.AddRange(new object[] { est[i] }); // el listbox2 corresponde a las estaciones.
                    siEst[i] = true;
                }
            }

            lll = (long)(Fei + timin * 10000000.0); // se convierte el tiempo en SUDS a tiempo en visual c#
            DateTime fech = new DateTime(lll);
            fe = int.Parse(string.Format("{0:yyyy}{0:MM}{0:dd}", fech));
            fcnan = new double[nutra];
            Unidad = new string[nutra];
            fcDR = new double[nutra];
            laD = new double[nutra];
            loD = new double[nutra];
            VD = new char[nutra];
            for (j = 0; j < nutra; j++)
            {
                fcnan[j] = -1.0;
                Unidad[j] = "?";
                fcDR[j] = -1.0;
                laD[j] = 100.0;
                loD[j] = 1000.0;
                VD[j] = '*';
                // fclist contiene la lista de factores para convertir cuentas a micrometros/segundo.
                for (jj = 0; jj < fclist.Count; jj++)
                {
                    //MessageBox.Show(fclist[jj].ToString());
                    ca = tar[j].ToString();
                    try
                    {
                        if (ca.Substring(0, 1) == fclist[jj].ToString().Substring(0, 1))
                        {
                            pa = fclist[jj].ToString().Split(delim);
                            esta = pa[1].Substring(0, 4);
                            fe1 = int.Parse(pa[3]);
                            fe2 = int.Parse(pa[4]);
                            if (string.Compare(esta.Substring(0, 4), est[j].Substring(0, 4)) == 0)
                            {
                                if (fe >= fe1 && (fe <= fe2 || fe2 == 0))
                                {
                                    try
                                    {
                                        fcnan[j] = double.Parse(pa[2]);
                                        if (pa.Length >= 10)
                                            Unidad[j] = pa[9];
                                        //MessageBox.Show(fclist[jj].ToString() + "\n" + pa[9]);
                                        if (pa.Length >= 9)
                                            fcDR[j] = double.Parse(pa[8]);
                                        laD[j] = double.Parse(pa[5]);
                                        loD[j] = double.Parse(pa[6]);
                                        VD[j] = pa[7][0];
                                    }
                                    catch
                                    {
                                        //MessageBox.Show("ERROR " + fclist[jj].ToString());
                                    }
                                }
                            }
                        }
                    }
                    catch
                    {
                        fcnan[j] = -1.0;
                        fcDR[j] = -1.0;
                        //MessageBox.Show(" 14 ERROR");
                    }
                }
            }

            if (panel2.Visible == true)
                panel2.Visible = false;
            cargar = true;
            promEst = new int[nutra];
            invertido = new bool[nutra];
            for (i = 0; i < nutra; i++)
                invertido[i] = false;
            li = "";
            if (File.Exists(".\\pro\\invertido.txt"))
            {
                StreamReader ar = new StreamReader(".\\pro\\invertido.txt");
                while (li != null)
                {
                    try
                    {
                        li = ar.ReadLine();
                        if (li == null || li[0] == '*') break;
                        pa = li.Split(delim);
                        estinv = pa[0];
                        fe1 = int.Parse(pa[1]);
                        fe2 = int.Parse(pa[2]);
                        for (j = 0; j < nutra; j++)
                        {
                            if (string.Compare(estinv.Substring(0, 4), est[j].Substring(0, 4)) == 0)
                            {
                                if (fe >= fe1 && (fe <= fe2 || fe2 == 0))
                                {
                                    invertido[j] = true;
                                    //MessageBox.Show(est[j].Substring(0, 4) + " invertido=" + invertido[j].ToString());
                                }
                                break;
                            }
                        }// for j.....
                    }
                    catch
                    {
                    }
                }
                ar.Close();
            }
            labelPanelTar.Text = nutra.ToString() + " Trazas";
            PromediosIniciales();
            panel1.Invalidate();

            return (nuu);
        }
        /// <summary>
        /// Se encarga de leer los archivos de sismos de tipo SEISAN y cargar en memoria los datos necesarios
        /// para su manipulación y clasificación, estos datos los guarda en las variables cu[][], tim[][], ra[], by[], nutra entre otras.
        /// ESTA PENDIENTE UNA REVISIÓN A FONDO DE SU FUNCIONAMIENTO.
        /// </summary>
        void LeeSeisanUno()
        {
            int i, iii, j, jj, k, kk, kkk, nucan = 0, fin = 0;
            int totbyt, adicion = 0, nuletra, vacio, cuvacio;
            int an2, me2, di2, ho2, mi2, se2, ms2, nuto;
            double seg, dt;
            int traslapo, inicio;
            int largo, lara;
            long ll0, ll;
            double tinicio = 0, tifinal = 0, tie, facra, dd1, dd2, dd;

            byte[] bit2 = new byte[2];
            byte[] bit4 = new byte[4];
            byte[] byy = new byte[1000];

            short[] tipbyte = new short[1];
            int[] numu = new int[1]; ;
            short[] bby = new short[1];
            int[][] cuu = new int[1][];
            char[][] cc;
            double[] tii = new double[1];
            double[] rat = new double[1]; ;
            string[] esta = new string[1]; ;
            string[] estt = new string[Ma];
            char[] compo = new char[1];
            int[] cus;
            double[] tims;

            bool si;
            string nom, ca, ca2;

            nom = listBox1.SelectedItem.ToString();
            if (!File.Exists(nom))
                return;

            try
            {
                FileInfo ar = new FileInfo(nom);
                BinaryReader br0 = new BinaryReader(ar.OpenRead());
                while (br0.PeekChar() != -1)
                {
                    br0.ReadBytes(34);
                    ca = Encoding.ASCII.GetString(br0.ReadBytes(3));
                    nucan = int.Parse(ca.Substring(0, 3));
                    numu = new int[nucan];
                    tipbyte = new short[nucan];
                    if (nucan > 12) fin = nucan;
                    else fin = 12;
                    br0.ReadBytes(143);
                    do
                    {
                        ca = Encoding.ASCII.GetString(br0.ReadBytes(88));
                        if (char.IsLetter(ca[0])) break;
                    } while (!char.IsLetter(ca[0]));

                    for (jj = 0; jj < nucan; jj++)
                    {
                        numu[jj] = int.Parse(ca.Substring(43, 7));
                        if (ca[76] == '4') tipbyte[jj] = 4;
                        else tipbyte[jj] = 2;
                        try
                        {
                            br0.ReadBytes(960);
                        }
                        catch
                        {
                        }
                        totbyt = numu[jj] * tipbyte[jj];
                        br0.ReadBytes(totbyt);
                        if (jj == nucan - 1) break;
                        br0.ReadBytes(8);
                        ca = Encoding.ASCII.GetString(br0.ReadBytes(88));
                        //le = ca.Substring(0, 6);
                    }
                    break;// provi
                }// while
                br0.Close();

                cuu = new int[nucan][];
                cc = new char[nucan][];
                for (iii = 0; iii < nucan; iii++)
                {
                    cuu[iii] = new int[numu[iii]];
                    cc[iii] = new char[3];
                }
                tii = new double[nucan];
                rat = new double[nucan];
                esta = new string[nucan];
                compo = new char[nucan];
                bby = new short[nucan];

                BinaryReader br = new BinaryReader(ar.OpenRead());

                try
                {
                    br.ReadBytes(1060);
                    if (fin > 30)
                    {
                        adicion = 88 * (int)(Math.Ceiling((fin - 30.0) / 3.0));
                        br.ReadBytes(adicion);
                    }
                }
                catch
                {
                }

                for (iii = 0; iii < nucan; iii++)
                {
                    ca2 = Encoding.ASCII.GetString(br.ReadBytes(88));
                    if (char.IsLetter(ca2[8]))
                        nuletra = 8;
                    else
                        nuletra = 7;
                    esta[iii] = ca2.Substring(0, 3) + ca2.Substring(nuletra, 1);
                    if (ca2[8] == 'N')
                        compo[iii] = 'n';
                    else if (ca2[8] == 'E')
                        compo[iii] = 'e';
                    else
                        compo[iii] = 'z';
                    an2 = int.Parse(ca2.Substring(10, 2));
                    if (ca2[9] == '1')
                        an2 += 2000;
                    else
                        an2 += 1900;
                    me2 = int.Parse(ca2.Substring(17, 2));
                    di2 = int.Parse(ca2.Substring(20, 2));
                    ho2 = int.Parse(ca2.Substring(23, 2));
                    mi2 = int.Parse(ca2.Substring(26, 2));
                    seg = double.Parse(ca2.Substring(29, 6));
                    se2 = (int)(seg);
                    ms2 = (int)((double)(seg - se2) * 1000.0);
                    DateTime fee1 = new DateTime(an2, me2, di2, ho2, mi2, se2, ms2);
                    ll = fee1.ToBinary();
                    tii[iii] = ((double)(ll) - Feisuds) / 10000000.0 + UTdisp * 3600.0;
                    if (tinicio == 0) tinicio = tii[iii];
                    else if (tinicio > tii[iii]) tinicio = tii[iii];
                    rat[iii] = double.Parse(ca2.Substring(36, 6));
                    if (rat[iii] <= 0)
                    {
                        MessageBox.Show("Rata de muestreo <= 0??");
                        br.Close();
                        return;
                    }
                    dt = 1.0 / rat[iii];
                    dd = tii[iii] + numu[iii] * dt;
                    if (tifinal < dd) tifinal = dd;

                    bby[iii] = tipbyte[iii];
                    try
                    {
                        br.ReadBytes(960);
                    }
                    catch
                    {
                    }
                    if (tipbyte[iii] == 4)
                    {
                        for (kkk = 0; kkk < numu[iii]; kkk++)
                            cuu[iii][kkk] = br.ReadInt32();
                    }
                    else
                    {
                        for (kkk = 0; kkk < numu[iii]; kkk++)
                            cuu[iii][kkk] = (int)(br.ReadInt16());
                    }
                    br.ReadBytes(8);
                }// for iii

                br.Close();
            }//try
            catch
            {
                NoMostrar = true;
                MessageBox.Show("*** ERROR!! ( quizas mas de 30 estaciones?? )");
            }

            nuto = 0;
            si = false;
            for (j = 0; j < nucan; j++)
            {
                if (nuto > 0)
                {
                    for (k = 0; k < nuto; k++)
                    {
                        if (string.Compare(estt[k].Substring(0, 4), esta[j].Substring(0, 4)) == 0 || esta[j].Substring(0, 4) == "XXXX")
                        {
                            si = true;
                            break;
                        }
                    }
                }
                if (si == false)
                {
                    estt[nuto] = esta[j];
                    ga[nutra + nuto] = 1;
                    comp[nutra + nuto] = compo[j];
                    nuto += 1;
                }
                else si = false;
            }

            // aqui se van a asignar los datos a cada estacion sucesivamente.
            for (iii = 0; iii < nuto; iii++)
            {
                lara = 0;
                tie = 0;
                facra = 0;
                vacio = 0;
                traslapo = 0;
                dd2 = 0;

                for (j = 0; j < nucan; j++)
                {
                    if (estt[iii].Substring(0, 4) == esta[j].Substring(0, 4))
                    {
                        if (lara > 0)
                        {
                            dd1 = tii[j];
                            if (dd1 - dd2 > (facra + 0.005) && dd2 > 0)
                            {
                                siRoto[nutra] = true;
                                vacio += (int)((dd1 - dd2) * rat[j]);
                            }
                            else if (dd2 - dd1 > (facra + 0.005) && dd2 > 0)
                            {
                                siTraslapo[nutra] = true;
                                traslapo += (int)((dd2 - dd1) * rat[j]);
                            }
                        }
                        lara += cuu[j].Length;
                        facra = 1.0 / (rat[j]);
                        dd2 = tii[j] + facra * cuu[j].Length;
                        break;
                    }
                }
                //}// for i....

                if (lara > 0)
                {
                    lara += vacio;
                    lara -= traslapo;
                    si = false;
                    /*if (listasei.Count > 0)
                    {
                        for (jj = 0; jj < listasei.Count; jj++)
                        {
                            if (listasei[jj].ToString().Substring(0, 4) == estt[iii].Substring(0, 4))
                            {
                                est[nutra] = listasei[jj].ToString().Substring(5, 4) + " ";
                                si = true;
                                break;
                            }
                        }
                    }*/
                    if (si == false)
                        est[nutra] = estt[iii].Substring(0, 4) + " ";
                    cus = new int[lara];
                    tims = new double[lara];
                    siEst[nutra] = true;
                    tar[nutra] = tardis;
                    //by[nutra] = bby[0][0];// se supone que es multiplexado y todos los datos tienen la misma caracteristica (rata, tipodato, etc!!

                    jj = 0;
                    dd2 = 0;
                    cuvacio = 0;
                    for (j = 0; j < nucan; j++)
                    {
                        //MessageBox.Show(" j=" + j.ToString() + " estt=" + estt[iii] + "esta=" + esta[j]);
                        if (estt[iii].Substring(0, 4) == esta[j].Substring(0, 4))
                        {
                            ra[nutra] = rat[j];// se puede mejorar!!
                            by[nutra] = bby[j];
                            tie = tii[j];
                            facra = 1.0 / (rat[j]);
                            inicio = 0;
                            if (tie - dd2 > (facra + 0.005) && dd2 > 0)
                            {
                                vacio = (int)((tie - dd2) * rat[j]);
                                dd2 += facra;
                                for (kk = 0; kk < vacio; kk++)
                                {
                                    tims[jj] = dd2 + kk * facra;
                                    cus[jj] = cuvacio;
                                    jj += 1;
                                }
                            }
                            else if (dd2 - tie > (facra + 0.005) && dd2 > 0)
                            {
                                traslapo = (int)((dd2 - tie) * rat[j]);
                                inicio = traslapo;
                            }
                            for (kk = inicio; kk < cuu[j].Length; kk++)
                            {
                                tims[jj] = tie + kk * facra;
                                cus[jj] = cuu[j][kk];
                                jj += 1;
                            }
                            facra = 1.0 / (rat[j]);
                            dd2 = tii[j] + facra * cuu[j].Length;
                            cuvacio = cus[jj - 1];
                            break;
                        }
                    }

                    inicio = (int)((tinicio - tims[0]) * ra[nutra]);
                    if (inicio < 0)
                        inicio = 0;
                    if (tims[lara - 1] - tifinal < 0)
                        fin = lara;
                    else
                        fin = (int)(lara - ((tims[lara - 1] - tifinal) * ra[nutra]));
                    //MessageBox.Show("inicio="+inicio.ToString()+" fin="+fin.ToString());
                    if (inicio >= fin)
                        continue;
                    i = fin - inicio;
                    tim[nutra] = new double[i];
                    cu[nutra] = new int[i];
                    kk = 0;
                    for (j = inicio; j < fin; j++)
                    {
                        cu[nutra][kk] = cus[j];
                        tim[nutra][kk++] = tims[j];
                    }
                    nutra += 1;
                } // if lara>0
            }

            return;
        }
        /// <summary>
        /// Cambia el texto del textBoxDisparo, esto es para seleccionar la letra identificadora de los archivos de factores.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void textBoxDisparo_TextChanged(object sender, EventArgs e)
        {
            try
            {
                tardis = textBoxDisparo.Text[0];
                textBoxDisparo.Text = tardis.ToString();
            }
            catch
            {
            }
        }
        /// <summary>
        /// El textBoxUT representa la cantidad de horas que se quieran añadir al tiempo del sismo, que se carga con el botón disparo
        /// de tal modo que sea concordante con la hora de la Base (UT, Hora Local).
        /// Este método modifica el valor de las horas a agregar.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void textBoxUT_TextChanged(object sender, EventArgs e)
        { 
            float ff;
            try
            {
                ff = float.Parse(textBoxUT.Text);
                UTdisp = ff;
                textBoxUT.BackColor = Color.White;
                SeleccionarMinuto(true);
                panel1.Invalidate();
            }
            catch
            {
                textBoxUT.BackColor = Color.Pink;
            }
        }
        /// <summary>
        /// Es para seleccionar la letra identificadora de los archivos de factores cuando se cargan sismos con el botón disparo.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void textBoxDisparo_MouseHover(object sender, EventArgs e)
        {
            ToolTip tip = new ToolTip();

            tip.IsBalloon = false;
            tip.ReshowDelay = 0;
            tip.Show("Letra de Tarjeta para Factores", panel1, 1200);
        }
        /// <summary>
        /// Da una indicación de que se debe escribir en el textBoxUT.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void textBoxUT_MouseHover(object sender, EventArgs e)
        {
            ToolTip tip = new ToolTip();

            tip.IsBalloon = false;
            tip.ReshowDelay = 0;
            tip.Show("Horas que se añaden al sismo", panel1, 1200);
        }
        /// <summary>
        /// Carga en memoria las trazas en formato GCF, dependiendo del color de este botón escoge si las carga
        /// desde el archivo original (archigcf.txt) o el auxiliar (archigcfaux.txt).
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void boTarAux_Click(object sender, EventArgs e)
        {
            int i;

            if (boTarAux.BackColor == Color.Red)
            {
                archgcf = archgcfaux;
                ValNugcf();
                boTarAux.BackColor = Color.Green;
            }
            else
            {
                archgcf = archgcfnorm;
                ValNugcf();
                boTarAux.BackColor = Color.Red;
            }
            if (nugcf > 0)
            {
                yagcf = new bool[nugcf];
                for (i = 0; i < nugcf; i++)
                    yagcf[i] = false;
            }
            dimensionar = false;
            DimensionarPanelTarjetas();
        }
        /// <summary>
        /// Esconde el panel panelTarAux.
        /// </summary>
        /// <param name="sender">El objeto que lanza el evento.</param>
        /// <param name="e">El evento que se lanzó.</param>
        private void boXAux_Click(object sender, EventArgs e)
        {
            panelTarAux.Visible = false;
        }

    }
}
