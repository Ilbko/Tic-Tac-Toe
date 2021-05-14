using System;
using System.Windows.Forms;
using Tic_Tac_Toe.Control;
using Tic_Tac_Toe.Model;

namespace Tic_Tac_Toe
{
    public partial class Form1 : Form
    {
        private Logic logic;
        public Form1()
        {
            this.Load += Form1_Load;
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            logic = new Logic(this);
            foreach(Button item in TicModel.buttons)
            {
                this.Controls.Add(item);
            }
        }

        private void Form1_SizeChanged(object sender, EventArgs e) => logic.Redraw(this.ClientSize);
    }
}
