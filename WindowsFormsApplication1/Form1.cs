using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string outputString;
            string query = this.inputTextBox.Text;
            double confidence = Convert.ToDouble(confidenceTextBox.Text);
            int support = Convert.ToInt32(supportTextBox.Text);

            string dbQueryRequest = Program.CreateRequest(query,confidence,support);
            outputString = Program.MakeRequest(dbQueryRequest);
            this.outputTextBox.Text = outputString;
        }

        private void inputTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void outputTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
