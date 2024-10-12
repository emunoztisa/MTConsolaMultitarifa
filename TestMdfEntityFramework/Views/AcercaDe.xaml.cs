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
    /// Lógica de interacción para AcercaDe.xaml
    /// </summary>
    public partial class AcercaDe : UserControl
    {
        //SERIAL PORT
        System.IO.Ports.SerialPort puertoSerie1 = new System.IO.Ports.SerialPort();
        String[] listado_puerto = System.IO.Ports.SerialPort.GetPortNames();

        public AcercaDe()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (validaPuertoCOMConfigurado())
            {
                configura_puerto_serial();
            }
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            close_serial_port();
            //dispose_serial_port();
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
                //ServiceConfigVarios scv = new ServiceConfigVarios();
                //config_varios cv_port_name = scv.getEntityByClave("PORT_NAME");
                //config_varios cv_baud_rate = scv.getEntityByClave("BAUD_RATE");
                //config_varios cv_paridad = scv.getEntityByClave("PARIDAD");
                //config_varios cv_data_bits = scv.getEntityByClave("DATA_BITS");
                //config_varios cv_stop_bits = scv.getEntityByClave("STOP_BITS");
                //config_varios cv_handshake = scv.getEntityByClave("HANDSHAKE");

                ServiceConfigPuertos scp = new ServiceConfigPuertos();
                ct_config_puertos config_puerto = scp.getEntityByNombreDispositivo("ALCANCIA");
                if(config_puerto != null)
                {
                    string cv_port_name = config_puerto.port_name;
                    string cv_baud_rate = config_puerto.baud_rate;
                    string cv_paridad = config_puerto.paridad;
                    string cv_data_bits = config_puerto.data_bits;
                    string cv_stop_bits = config_puerto.stop_bits;
                    string cv_handshake = config_puerto.handshake;

                    if (cv_port_name != "")
                    {
                        this.puertoSerie1 = new System.IO.Ports.SerialPort
                        ("" + cv_port_name
                        , Convert.ToInt32(cv_baud_rate)
                        , cv_paridad == "NONE" ? System.IO.Ports.Parity.None : System.IO.Ports.Parity.Mark
                        , Convert.ToInt32(cv_data_bits)
                        , Convert.ToInt32(cv_stop_bits) == 1 ? System.IO.Ports.StopBits.One : System.IO.Ports.StopBits.None
                        );
                        puertoSerie1.Handshake = cv_handshake == "NONE" ? System.IO.Ports.Handshake.None : System.IO.Ports.Handshake.XOnXOff;

                        close_serial_port();
                        open_serial_port(); //EMD 2024-05-06
                    }
                }
                
            }
            catch
            {
                MessageBox.Show("Verifique" + System.Environment.NewLine + "- Alimentación" + System.Environment.NewLine + "- Conexión del puerto", "Error de puerto COMM");
            }
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


    }
}
