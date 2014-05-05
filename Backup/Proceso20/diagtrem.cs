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
    public partial class diagtrem : Form
    {
        public diagtrem()
        {
            InitializeComponent();
        }
        public string durtrem
        {
            set { textBoxTrem.Text = value; }
            get { return textBoxTrem.Text; }
        }
    }
}
