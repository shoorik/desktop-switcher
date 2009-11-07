using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DesktopSwitcher
{
    public partial class InputForm : Form
    {
        Form1 parent;

        public InputForm(Form1 up)
        {
            parent = up;
            InitializeComponent();
        }

        public void progFilter()
        {
            label.Text = "Enter the names of programs you wish to not change during.\n(separated by spaces)";
            inputBox.Text = parent.progfilter;
            okbutton.Click += new EventHandler(progFilterOK);
            this.Show();
            inputBox.Select(inputBox.Text.Length, 0);
        }

        private void progFilterOK(object sender, EventArgs e)
        {
            parent.progfilter = inputBox.Text;
            this.Hide();
        }

        private void inputBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return)
            {
                progFilterOK(sender, e);
                okbutton.Focus();
            }
            else if (e.KeyCode == Keys.Escape)
                this.Hide();

        }

        private void InputForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        private void InputForm_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.Hide();
        }

        private void okbutton_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.Hide();
        }


    }
}
