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
    public partial class CitiesUC : UserControl
    {
        public CitiesUC()
        {
            InitializeComponent();
        }

        private void btnAddCity_Click(object sender, EventArgs e)
        {
            NewCity newTripForm = new NewCity();
            newTripForm.ShowDialog();
        }
    }
}
