using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TestMdfEntityFramework.EntityServices;

namespace TestMdfEntityFramework.Views
{
    /// <summary>
    /// Lógica de interacción para vwAsignaciones.xaml
    /// </summary>
    public partial class vwAsignaciones : UserControl
    {
        public vwAsignaciones()
        {
            InitializeComponent();
        }
        private void vwAsignaciones_OnLoad(object sender, RoutedEventArgs e)
        {
            LlenaComboAsignaciones();
            SetearAsignacionActivaEnControles();
        }

        private void SetearAsignacionActivaEnControles()
        {
            ServiceConfigVarios scv = new ServiceConfigVarios();
            config_varios cv = scv.getEntityByClave("ASIGNACION_ACTIVA");
            string asignacionActiva = cv.valor;

            ServiceAsignaciones sasign = new ServiceAsignaciones();
            sy_asignaciones asignacion_activa = sasign.getEntityByFolio(asignacionActiva);

            lblFolioAsignacion.Text = asignacion_activa.folio;
            lblDia.Text = asignacion_activa.fecha;
            lblHora.Text = asignacion_activa.hora;

            //setear ruta
            ServiceRutas sr = new ServiceRutas();
            ct_rutas ruta_actual = sr.getEntity(asignacion_activa.fkRuta);
            lblRuta.Text = ruta_actual != null ? ruta_actual.nombre : "";

            //setear unidad
            ServiceUnidades su = new ServiceUnidades();
            ct_unidades unidad_actual = su.getEntity(asignacion_activa.fkUnidad);
            lblUnidad.Text = unidad_actual != null ? unidad_actual.nombre : "";

            //setear operador
            ServiceOperadores so = new ServiceOperadores();
            ct_operadores operador_actual = so.getEntity(asignacion_activa.fkOperador);
            lblOperador.Text = operador_actual != null ? operador_actual.nombre : "";
        }
        private void btnGuardarAsignacion_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string update_at = DateTime.Now.ToString("yyyy-MM-dd");

                ServiceConfigVarios scv = new ServiceConfigVarios();
                config_varios cv = new config_varios();
                cv.clave = "ASIGNACION_ACTIVA";
                cv.valor = cmbAsignaciones.Text;
                cv.updated_at = update_at;
                scv.updEntityByClave(cv);

                MessageBox.Show("ASIGNACION ACTIVADA CON EXITO", "INFO", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        private void cmbAsignaciones_SeleccionChanged(object sender, SelectionChangedEventArgs e)
        {
            string currentSelectedTextFolio = cmbAsignaciones.SelectedValue.ToString().Trim();

            ServiceAsignaciones sasign = new ServiceAsignaciones();
            sy_asignaciones asignacion_selected = sasign.getEntityByFolio(currentSelectedTextFolio);

            lblFolioAsignacion.Text = asignacion_selected.folio;
            lblDia.Text = asignacion_selected.fecha;
            lblHora.Text = asignacion_selected.hora;

            //setear ruta
            ServiceRutas sr = new ServiceRutas();
            ct_rutas ruta_actual = sr.getEntity(asignacion_selected.fkRuta);
            lblRuta.Text = ruta_actual != null ? ruta_actual.nombre : "";

            //setear unidad
            ServiceUnidades su = new ServiceUnidades();
            ct_unidades unidad_actual = su.getEntity(asignacion_selected.fkUnidad);
            lblUnidad.Text = unidad_actual != null ? unidad_actual.nombre : "";

            //setear operador
            ServiceOperadores so = new ServiceOperadores();
            ct_operadores operador_actual = so.getEntity(asignacion_selected.fkOperador);
            lblOperador.Text = operador_actual != null ? operador_actual.nombre : "";

        }

        private void LlenaComboAsignaciones()
        {
            List<string> list = getAsignacionesDiaActual();
            foreach (var port in list)
            {
                cmbAsignaciones.Items.Add(port);
            }
        }
        private List<string> getAsignacionesDiaActual()
        {
            string day = DateTime.Now.ToString("yyyy-MM-dd");

            ServiceAsignaciones sasign = new ServiceAsignaciones();
            List<sy_asignaciones> list = sasign.getEntityByDay(day);
            List<string> list2 = new List<string>();
            for (int i = 0; i < list.Count; i++)
            {
                list2.Add(list[i].folio);
            }
            return list2;

        }

        
    }
}
