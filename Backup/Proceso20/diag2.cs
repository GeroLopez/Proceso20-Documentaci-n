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
    public partial class diag2 : Form
    {
        public diag2()
        {
            InitializeComponent();
        }
        public string Lin
        {
            set { textBox1.Text = value; }
            get { return textBox1.Text; }
        }

        public string Ven
        {
            set { textBox2.Text = value; }
            get { return textBox2.Text; }
        }

        public string Esp
        {
            set { textBox3.Text = value; }
            get { return textBox3.Text; }
        }

        public string Tampepa
        {
            set { textBox4.Text = value; }
            get { return textBox4.Text; }
        }

    }
}
