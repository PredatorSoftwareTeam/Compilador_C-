using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Compilador
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            AnalizadorLexico GenerarLexico = new AnalizadorLexico(richTextBox1.Text);
            GenerarLexico.AnalizarLexico();

            var objSintactico = new Sintactico(GenerarLexico.listaToken);
            objSintactico.EjecutarSintactico(objSintactico.listaTokens);

            List<Error> listaErroresLexico = GenerarLexico.listaError;
            List<Error> listaErroresSintactico = objSintactico.listaError;
            List<Error> listaErrores = listaErroresLexico.Union(listaErroresSintactico).ToList();
            
            var lista = new BindingList<Token>(GenerarLexico.listaToken);
            lexicoDgv.DataSource = null;
            lexicoDgv.DataSource = lista;
            
            //List<Error> listaErroresLexico = GenerarLexico.listaError;
            erroresDgv.DataSource = null;
            erroresDgv.DataSource = listaErrores;
           
        }
    }
}
