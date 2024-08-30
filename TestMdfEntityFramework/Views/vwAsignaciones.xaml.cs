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
using System.Windows.Media.Animation;
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
        //SERIAL PORT
        System.IO.Ports.SerialPort puertoSerie1 = new System.IO.Ports.SerialPort();
        String[] listado_puerto = System.IO.Ports.SerialPort.GetPortNames();

        //POPUP OK
        private double left, top, right, bottom, centerX, centerY;
        private DoubleAnimation bottomToCenterAnimiation, topToCenterAnimation,
            leftToCenterAnimation, rightToCenterAnimation;
        private Storyboard bottomToCenterStoryboard, topToCenterStoryboard,
            leftToCenterStoryboard, rightToCenterStoryboard;
        public vwAsignaciones()
        {
            InitializeComponent();
        }
        private void vwAsignaciones_OnLoad(object sender, RoutedEventArgs e)
        {
            if (validaPuertoCOMConfigurado())
            {
                configura_puerto_serial();
            }

            //EVENTOS PARA POPUP OK
            SetPopupDlgCenter();
            InitializeAnimations();

            LlenaComboAsignaciones();
            SetearAsignacionActivaEnControles();
        }
        private void vwAsignaciones_Unloaded(object sender, RoutedEventArgs e)
        {
            close_serial_port();
            //dispose_serial_port();
        }

        

        private void SetearAsignacionActivaEnControles()
        {
            ServiceConfigVarios scv = new ServiceConfigVarios();
            config_varios cv = scv.getEntityByClave("ASIGNACION_ACTIVA");
            string asignacionActiva = cv.valor;

            ServiceAsignaciones serv_asign = new ServiceAsignaciones();
            sy_asignaciones obj_asignacion_activa = serv_asign.getEntityByFolio(asignacionActiva);
            
            if(obj_asignacion_activa != null)
            {
                lblFolioAsignacion.Text = obj_asignacion_activa.folio;
                lblDia.Text = obj_asignacion_activa.fecha;
                lblHora.Text = obj_asignacion_activa.hora;

                //setear ruta
                ServiceRutas sr = new ServiceRutas();
                ct_rutas ruta_actual = sr.getEntity(obj_asignacion_activa.fkRuta);
                lblRuta.Text = ruta_actual != null ? ruta_actual.nombre : "";

                //setear unidad
                ServiceUnidades su = new ServiceUnidades();
                ct_unidades unidad_actual = su.getEntity(obj_asignacion_activa.fkUnidad);
                lblUnidad.Text = unidad_actual != null ? unidad_actual.nombre : "";

                //setear operador
                ServiceOperadores so = new ServiceOperadores();
                ct_operadores operador_actual = so.getEntity(obj_asignacion_activa.fkOperador);
                lblOperador.Text = operador_actual != null ? operador_actual.nombre : "";
            }
            
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

                //ASIGNAR EL FK_ASIGNACION_ACTIVA a la variable del principal
                //OBTENER CONFIGURACIONES VARIAS DEL SISTEMA Y OPERACION ACTUAL
                ServiceConfigVarios serv_config_varios = new ServiceConfigVarios();
                config_varios cv_asign = serv_config_varios.getEntityByClave("ASIGNACION_ACTIVA");

                ServiceAsignaciones serv_asign = new ServiceAsignaciones();
                sy_asignaciones asig = serv_asign.getEntityByFolio(cv_asign.valor);

                Principal.FK_ASIGNACION_ACTIVA = cv_asign.valor != "" ? asig.pkAsignacion : 0;

                //MessageBox.Show("ASIGNACION ACTIVADA CON EXITO", "INFO", MessageBoxButton.OK, MessageBoxImage.Information);

                txtMensajePopup.Text = "ASIGNACION ACTIVADA";
                mostrarPopupOk();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }
        private bool validaPuertoCOMConfigurado()
        {
            bool isConfigured = false;

            ServiceConfigVarios serv_config_varios = new ServiceConfigVarios();
            config_varios cv_port_name = serv_config_varios.getEntityByClave("PORT_NAME");
            if (cv_port_name.valor != null && cv_port_name.valor != "")
            {
                isConfigured = true;
            }
            else
            {
                isConfigured = false;
            }

            return isConfigured;
        }
        public void configura_puerto_serial()
        {
            try
            {
                ServiceConfigVarios scv = new ServiceConfigVarios();
                config_varios cv_port_name = scv.getEntityByClave("PORT_NAME");
                config_varios cv_baud_rate = scv.getEntityByClave("BAUD_RATE");
                config_varios cv_paridad = scv.getEntityByClave("PARIDAD");
                config_varios cv_data_bits = scv.getEntityByClave("DATA_BITS");
                config_varios cv_stop_bits = scv.getEntityByClave("STOP_BITS");
                config_varios cv_handshake = scv.getEntityByClave("HANDSHAKE");

                if (cv_port_name.valor != "")
                {
                    //MessageBox.Show(cv_port_name.valor);

                    this.puertoSerie1 = new System.IO.Ports.SerialPort
                    ("" + cv_port_name.valor
                    , Convert.ToInt32(cv_baud_rate.valor)
                    , cv_paridad.valor == "NONE" ? System.IO.Ports.Parity.None : System.IO.Ports.Parity.Mark
                    , Convert.ToInt32(cv_data_bits.valor)
                    , Convert.ToInt32(cv_stop_bits.valor) == 1 ? System.IO.Ports.StopBits.One : System.IO.Ports.StopBits.None
                    );
                    puertoSerie1.Handshake = cv_handshake.valor == "NONE" ? System.IO.Ports.Handshake.None : System.IO.Ports.Handshake.XOnXOff;

                    close_serial_port();
                    //dispose_serial_port();

                    open_serial_port(); //EMD 2024-05-06
                }
            }
            catch
            {
                MessageBox.Show("Verifique" + System.Environment.NewLine + "- Alimentación" + System.Environment.NewLine + "- Conexión del puerto", "Error de puerto COMM");
            }

            //close_serial_port();

        }
        private void open_serial_port()
        {
            try
            {
                if (puertoSerie1.IsOpen == false)
                {
                    puertoSerie1.Open();
                }
            }
            catch (Exception ex)
            {
                //configura_puerto_serial();

            }
        }
        private void close_serial_port()
        {
            if (puertoSerie1.IsOpen == true)
            {
                puertoSerie1.Close();
            }
        }
        private void dispose_serial_port()
        {
            if (puertoSerie1.IsOpen == false)
            {
                puertoSerie1.Dispose();
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


        #region METODOS GRID POPUP
        private void ocultarPopupOk()
        {
            try
            {
                SetPopupDlgCenter();
                bottomToCenterAnimiation.From = bottom;
                bottomToCenterAnimiation.To = centerY;

                Canvas.SetTop(popupBd, bottom);

                popupGrid.Visibility = Visibility.Hidden;

                bottomToCenterStoryboard.Begin();
            }
            catch (Exception)
            {
                throw;
            }
        }
        private void mostrarPopupOk()
        {
            try
            {
                SetPopupDlgCenter();
                bottomToCenterAnimiation.From = bottom;
                bottomToCenterAnimiation.To = centerY;

                Canvas.SetTop(popupBd, bottom);

                popupGrid.Visibility = Visibility.Visible;

                bottomToCenterStoryboard.Begin();
            }
            catch (Exception)
            {
                throw;
            }
        }
        private void SetPopupDlgCenter()
        {
            try
            {
                left = -(popupBd.ActualWidth);
                top = -(popupBd.ActualHeight);
                right = (popupGrid.ActualWidth);
                bottom = (popupGrid.ActualHeight);

                centerX = (popupGrid.ActualWidth / 2) - popupBd.ActualWidth / 2;
                centerY = (popupGrid.ActualHeight / 2) - popupBd.ActualHeight / 2;

                Canvas.SetLeft(popupBd, centerX);
                Canvas.SetTop(popupBd, centerY);
            }
            catch (Exception)
            {

                throw;
            }
        }
        private void InitializeAnimations()
        {
            try
            {
                #region bottom to center animation
                bottomToCenterAnimiation = new DoubleAnimation()
                {
                    From = bottom,
                    To = centerY,
                    Duration = TimeSpan.FromMilliseconds(250),
                    FillBehavior = FillBehavior.Stop,
                };

                Storyboard.SetTarget(bottomToCenterAnimiation, popupBd);
                Storyboard.SetTargetProperty(bottomToCenterAnimiation, new PropertyPath(Canvas.TopProperty));

                bottomToCenterStoryboard = new Storyboard();
                bottomToCenterStoryboard.Children.Add(bottomToCenterAnimiation);

                bottomToCenterStoryboard.Completed += OnStoryboardCompleted;
                #endregion

                #region top to center animation 
                topToCenterAnimation = new DoubleAnimation()
                {
                    From = top,
                    To = centerY,
                    Duration = TimeSpan.FromMilliseconds(250),
                    FillBehavior = FillBehavior.Stop,
                };

                Storyboard.SetTarget(topToCenterAnimation, popupBd);
                Storyboard.SetTargetProperty(topToCenterAnimation, new PropertyPath(Canvas.TopProperty));

                topToCenterStoryboard = new Storyboard();
                topToCenterStoryboard.Children.Add(topToCenterAnimation);

                topToCenterStoryboard.Completed += OnStoryboardCompleted;
                #endregion

                #region left to center animation
                leftToCenterAnimation = new DoubleAnimation()
                {
                    From = left,
                    To = centerX,
                    Duration = TimeSpan.FromMilliseconds(250),
                    FillBehavior = FillBehavior.Stop,
                };

                Storyboard.SetTarget(leftToCenterAnimation, popupBd);
                Storyboard.SetTargetProperty(leftToCenterAnimation, new PropertyPath(Canvas.LeftProperty));

                leftToCenterStoryboard = new Storyboard();
                leftToCenterStoryboard.Children.Add(leftToCenterAnimation);

                leftToCenterStoryboard.Completed += OnStoryboardCompleted;
                #endregion

                #region right to center animation
                rightToCenterAnimation = new DoubleAnimation()
                {
                    From = right,
                    To = centerX,
                    Duration = TimeSpan.FromMilliseconds(250),
                    FillBehavior = FillBehavior.Stop,
                };

                Storyboard.SetTarget(rightToCenterAnimation, popupBd);
                Storyboard.SetTargetProperty(rightToCenterAnimation, new PropertyPath(Canvas.LeftProperty));

                rightToCenterStoryboard = new Storyboard();
                rightToCenterStoryboard.Children.Add(rightToCenterAnimation);

                rightToCenterStoryboard.Completed += OnStoryboardCompleted;

                #endregion
            }
            catch (Exception)
            {

                throw;
            }
        }
        private void OnStoryboardCompleted(object sender, EventArgs e)
        {
            Canvas.SetLeft(popupBd, centerX);
            Canvas.SetTop(popupBd, centerY);
        }
        private void popupGrid_LostFocus(object sender, RoutedEventArgs e)
        {
            ocultarPopupOk();
        }
        private void popupGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ocultarPopupOk();
        }
        private void popupGrid_TouchDown(object sender, TouchEventArgs e)
        {
            ocultarPopupOk();
        }
        #endregion


    }
}
