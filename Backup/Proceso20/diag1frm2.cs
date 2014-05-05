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
    public partial class diag1frm2 : Form
    {
        public diag1frm2()
        {
            InitializeComponent();
        }

        public string Inite
        {
            set { textinite.Text = value; }
            get { return textinite.Text; }
        }

        public string Interite
        {
            set { textite.Text = value; }
            get { return textite.Text; }
        }


    }
}
