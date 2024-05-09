using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TestMdfEntityFramework.Clases;
using TestMdfEntityFramework.EntityServices;

namespace TestMdfEntityFramework.Views
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            List<ClaseRepUsers> list_clase_rep_users = new List<ClaseRepUsers>();

            ServiceUsers serv_users = new ServiceUsers();
            List<users> list_users = serv_users.getEntities();

            if(list_users != null && list_users.Count > 0)
            {
                foreach (var it in list_users)
                {
                    list_clase_rep_users.Add(new ClaseRepUsers { usuario = it.user, password = it.contrasena });
                }
            }

            this.reportViewer1.LocalReport.ReportPath = "ReporteUsers.rdlc";
            ReportDataSource source = new ReportDataSource("", list_clase_rep_users);
            this.reportViewer1.LocalReport.DataSources.Clear();
            this.reportViewer1.LocalReport.DataSources.Add(source);

            reportViewer1.RefreshReport();
        }
    }
}
