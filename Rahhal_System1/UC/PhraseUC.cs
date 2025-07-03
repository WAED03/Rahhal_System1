using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Rahhal_System1
{
    public partial class PhraseUC : UserControl
    {
        public PhraseUC()
        {
            InitializeComponent();
        }

        private void btnAddCity_Click(object sender, EventArgs e)
        {
            NewWord newTripForm = new NewWord();
            newTripForm.ShowDialog();
        }
    }
}
