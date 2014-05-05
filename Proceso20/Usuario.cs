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
    public partial class Usuario : Form
    {
        public Usuario()
        {
            InitializeComponent();
        }
        public string Usua
        {
            set { textBox1.Text = value; }
            get { return textBox1.Text; }
        }
    }
}
