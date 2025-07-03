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
    public partial class TripsUC : UserControl
    {
        public TripsUC()
        {
            InitializeComponent();
        }

        private void btnAddTrip_Click(object sender, EventArgs e)
        {
            NewTrip newTripForm = new NewTrip();
            newTripForm.ShowDialog();
        }
    }
}
