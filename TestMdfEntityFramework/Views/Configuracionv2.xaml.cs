using System;
using System.Collections.Generic;
using System.Drawing.Printing;
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
using TestMdfEntityFramework.Clases;
using TestMdfEntityFramework.EntityServices;

namespace TestMdfEntityFramework.Views
{
    /// <summary>
    /// Lógica de interacción para Configuracionv2.xaml
    /// </summary>
    public partial class Configuracionv2 : UserControl
    {

        public Configuracionv2()
        {
            InitializeComponent();
            llenaCombos();
            Task.WaitAll(new Task[] { Task.Delay(500) });

        }

        private void Configuracion_OnLoad(object sender, RoutedEventArgs e)
        {
            //MuestraValoresConfigSerial();

            
            SeteaValoresEnCombosConValoresDBLocal();
        }


        private void MuestraValoresConfigSerial ()
        {
            //ServiceConfigPort scp = new ServiceConfigPort();
            //config_port conf_port_activo = scp.getEntityByStatus(1);

            //if (conf_port_activo != null)
            //{
            //    string mensaje = "Los datos configurados actualmente para el puerto son: " + System.Environment.NewLine +
            //     System.Environment.NewLine +
            //    "port_name: " + conf_port_activo.port_name + System.Environment.NewLine +
            //    "baud_rate: " + conf_port_activo.baud_rate + System.Environment.NewLine +
            //    "data_bits: " + conf_port_activo.data_bits.ToString() + System.Environment.NewLine +
            //    "stop_bits: " + conf_port_activo.stop_bits.ToString() + System.Environment.NewLine +
            //    "parity: " + conf_port_activo.parity;

            //    MessageBox.Show(mensaje, "CONFIGURACION", MessageBoxButton.OK, MessageBoxImage.Information);
            //}
            //else
            //{
            //    MessageBox.Show("Aun no se configuran pararametros para el puerto serie", "CONFIGURACION", MessageBoxButton.OK, MessageBoxImage.Information);
            //}


        }

        public void llenaCombos()
        {
            llenaComboComPorts();
            llenaComboBaudRate();
            llenaComboDataBits();
            llenaComboStopBits();
            llenaComboParidad();
            llenaComboHandshake();
            llenaComboTipoTarifa();
            llenaComboImpresoras();
            llenaComboUnidades();
        }

        public void SeteaValoresEnCombosConValoresDBLocal()
        {
            ServiceConfigVarios scv = new ServiceConfigVarios();

            config_varios config_tipo_tarifa = scv.getEntityByClave("TIPO_TARIFA");
            for (int i = 0; i < cmbTipoTarifa.Items.Count; i++)
            {
                if (cmbTipoTarifa.Items[i].ToString() == config_tipo_tarifa.valor)
                {
                    cmbTipoTarifa.SelectedValue = config_tipo_tarifa.valor;
                }
            }

            config_varios config_impresora = scv.getEntityByClave("IMPRESORA_DEFAULT");
            for (int i = 0; i < cmbImpresoras.Items.Count; i++)
            {
                if (cmbImpresoras.Items[i].ToString() == config_impresora.valor)
                {
                    cmbImpresoras.SelectedValue = config_impresora.valor;
                }
            }

            config_varios config_numero_unidad = scv.getEntityByClave("NUMERO_UNIDAD");
            for (int i = 0; i < cmbUnidad.Items.Count; i++)
            {
                if (cmbUnidad.Items[i].ToString() == config_numero_unidad.valor)
                {
                    cmbUnidad.SelectedValue = config_numero_unidad.valor;
                }
            }
            //foreach (var item in cmbUnidad.Items)
            //{
            //    if( ((ComboBoxItem)item).Tag.ToString() == config_numero_unidad.valor)
            //    {
            //        cmbUnidad.SelectedItem = ((ComboBoxItem)item).Tag.ToString();
            //    }
            //}


            config_varios config_baud_rate = scv.getEntityByClave("BAUD_RATE");
            for (int i = 0; i < cmbBausRate.Items.Count; i++)
            {
                if (cmbBausRate.Items[i].ToString() == config_baud_rate.valor)
                {
                    cmbBausRate.SelectedValue = config_baud_rate.valor;
                }
            }

            config_varios config_paridad = scv.getEntityByClave("PARIDAD");
            for (int i = 0; i < cmbParity.Items.Count; i++)
            {
                if (cmbParity.Items[i].ToString() == config_paridad.valor)
                {
                    cmbParity.SelectedValue = config_paridad.valor;
                }
            }

            config_varios config_data_bits = scv.getEntityByClave("DATA_BITS");
            for (int i = 0; i < cmbDataBits.Items.Count; i++)
            {
                if (cmbDataBits.Items[i].ToString() == config_data_bits.valor)
                {
                    cmbDataBits.SelectedValue = config_data_bits.valor;
                }
            }

            config_varios config_stop_bits = scv.getEntityByClave("STOP_BITS");
            for (int i = 0; i < cmbStopBits.Items.Count; i++)
            {
                if (cmbStopBits.Items[i].ToString() == config_stop_bits.valor)
                {
                    cmbStopBits.SelectedValue = config_stop_bits.valor;
                }
            }

            config_varios config_handshake = scv.getEntityByClave("HANDSHAKE");
            for (int i = 0; i < cmbHandShake.Items.Count; i++)
            {
                if (cmbHandShake.Items[i].ToString() == config_handshake.valor)
                {
                    cmbHandShake.SelectedValue = config_handshake.valor;
                }
            }

            config_varios config_port_name = scv.getEntityByClave("PORT_NAME");
            for (int i = 0; i < cmbComPorts.Items.Count; i++)
            {
                if (cmbComPorts.Items[i].ToString() == config_port_name.valor)
                {
                    cmbComPorts.SelectedValue = config_port_name.valor;
                }
            }

        }

        public void llenaComboComPorts()
        {
            List<string> list = getCOMports();
            foreach (var port in list)
            {
                cmbComPorts.Items.Add(port);
            }
        }
        public void llenaComboBaudRate()
        {
            List<string> list = getCatalogo_BaudRate_BaseLocal();
            foreach (var port in list)
            {
                cmbBausRate.Items.Add(port);
            }
        }
        public void llenaComboDataBits()
        {
            List<string> list = getCatalogo_DataBits_BaseLocal();
            foreach (var port in list)
            {
                cmbDataBits.Items.Add(port);
            }
        }
        public void llenaComboStopBits()
        {
            List<string> list = getCatalogo_StopBits_BaseLocal();
            foreach (var port in list)
            {
                cmbStopBits.Items.Add(port);
            }
        }
        public void llenaComboParidad()
        {
            List<string> list = getCatalogo_Paridad_BaseLocal();
            foreach (var port in list)
            {
                cmbParity.Items.Add(port);
            }
        }
        public void llenaComboHandshake()
        {
            List<string> list = getCatalogo_Handshake_BaseLocal();
            foreach (var port in list)
            {
                cmbHandShake.Items.Add(port);
            }
        }
        
        public void llenaComboTipoTarifa()
        {
            List<string> list = getCatalogo_TipoTarifa_BaseLocal();
            foreach (var port in list)
            {
                cmbTipoTarifa.Items.Add(port);
            }
        }
        public void llenaComboImpresoras()
        {
            List<string> list = getPrinterDevices();
            foreach (var imp in list)
            {
                cmbImpresoras.Items.Add(imp);
            }
        }
        public void llenaComboUnidades()
        {
            List<string> list = getCatalogo_Unidades_BaseLocal();
            foreach (var li in list)
            {
                cmbUnidad.Items.Add(li);
            }
        }
        

        /// <summary>
        /// ////////////////////////////////////////
        /// </summary>
        public List<string> getCOMports()
        {
            List<string> listado_puertos = new List<string>();
            String[] arreglo_puertos = System.IO.Ports.SerialPort.GetPortNames();
            for (int i = 0; i < arreglo_puertos.Length; i++)
            {
                listado_puertos.Add(arreglo_puertos[i].ToString());
            }

            return listado_puertos;
        }
        private List<string> getCatalogo_BaudRate_BaseLocal()
        {
            ServiceOpcionesGenerales sog = new ServiceOpcionesGenerales();
            List<opciones_generales> list = sog.getEntitiesByAgrupador("BAUD_RATE");
            List<string> list2 = new List<string>();
            for (int i = 0; i < list.Count; i++)
            {
                list2.Add(list[i].opcion_general);
            }
            return list2;
        }
        private List<string> getCatalogo_Paridad_BaseLocal()
        {
            ServiceOpcionesGenerales sog = new ServiceOpcionesGenerales();
            List<opciones_generales> list = sog.getEntitiesByAgrupador("PARIDAD");
            List<string> list2 = new List<string>();
            for (int i = 0; i < list.Count; i++)
            {
                list2.Add(list[i].opcion_general);
            }
            return list2;
        }
        private List<string> getCatalogo_Handshake_BaseLocal()
        {
            ServiceOpcionesGenerales sog = new ServiceOpcionesGenerales();
            List<opciones_generales> list = sog.getEntitiesByAgrupador("HANDSHAKE");
            List<string> list2 = new List<string>();
            for (int i = 0; i < list.Count; i++)
            {
                list2.Add(list[i].opcion_general);
            }
            return list2;
        }
        private List<string> getCatalogo_DataBits_BaseLocal()
        {
            ServiceOpcionesGenerales sog = new ServiceOpcionesGenerales();
            List<opciones_generales> list = sog.getEntitiesByAgrupador("DATA_BITS");
            List<string> list2 = new List<string>();
            for (int i = 0; i < list.Count; i++)
            {
                list2.Add(list[i].opcion_general);
            }
            return list2;
        }
        private List<string> getCatalogo_StopBits_BaseLocal()
        {
            ServiceOpcionesGenerales sog = new ServiceOpcionesGenerales();
            List<opciones_generales> list = sog.getEntitiesByAgrupador("STOP_BITS");
            List<string> list2 = new List<string>();
            for (int i = 0; i < list.Count; i++)
            {
                list2.Add(list[i].opcion_general);
            }
            return list2;
        }
        private List<string> getCatalogo_TipoTarifa_BaseLocal()
        {
            ServiceOpcionesGenerales sog = new ServiceOpcionesGenerales();
            List<opciones_generales> list = sog.getEntitiesByAgrupador("TIPO_TARIFA");
            List<string> list2 = new List<string>();
            for (int i = 0; i < list.Count; i++)
            {
                list2.Add(list[i].opcion_general);
            }
            return list2;
        }
        public List<string> getPrinterDevices()
        {
            List<string> listado_impresoras = new List<string>();
            foreach (string printer in PrinterSettings.InstalledPrinters)
            {
                listado_impresoras.Add(printer);
            }
            return listado_impresoras;
        }
        private List<string> getCatalogo_Unidades_BaseLocal()
        {
            ServiceUnidades su = new ServiceUnidades();
            List<ct_unidades> list = su.getEntities();
            List<string> list2 = new List<string>();
            for (int i = 0; i < list.Count; i++)
            {
                list2.Add(list[i].nombre);
            }
            return list2;
        }

        private void btnGuardarConfiguracion_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //CONFIGURACION DE PUERTO SERIE
                string fecha_actual = DateTime.Now.ToShortDateString();

                string port_name = cmbComPorts.Text;
                string baud_rate = cmbBausRate.Text;
                string data_bits = cmbDataBits.Text;
                string stop_bits = cmbStopBits.Text;
                string paridad = cmbParity.Text;
                string handshake = cmbHandShake.Text;

                string impresora = cmbImpresoras.Text;
                string tipo_tarifa = cmbTipoTarifa.Text;
                string numero_unidad = cmbUnidad.Text;

                ServiceConfigVarios scv = new ServiceConfigVarios();
                config_varios cv = new config_varios();

                #region guardar config serial port

                cv.clave = "PORT_NAME";
                cv.valor = port_name;
                scv.updEntityByClave(cv);

                cv.clave = "BAUD_RATE";
                cv.valor = baud_rate;
                scv.updEntityByClave(cv);

                cv.clave = "DATA_BITS";
                cv.valor = data_bits;
                scv.updEntityByClave(cv);

                cv.clave = "STOP_BITS";
                cv.valor = stop_bits;
                scv.updEntityByClave(cv);

                cv.clave = "PARIDAD";
                cv.valor = paridad;
                scv.updEntityByClave(cv);

                cv.clave = "HANDSHAKE";
                cv.valor = handshake;
                scv.updEntityByClave(cv);

                #endregion

                #region guardar otras config varias

                cv.clave = "TIPO_TARIFA";
                cv.valor = tipo_tarifa;
                scv.updEntityByClave(cv);

                cv.clave = "IMPRESORA_DEFAULT";
                cv.valor = impresora;
                scv.updEntityByClave(cv);

                cv.clave = "NUMERO_UNIDAD";
                cv.valor = numero_unidad;
                scv.updEntityByClave(cv);

                #endregion


                #region antes
                /*
                ServiceConfigPort scp = new ServiceConfigPort();
                List<config_port> list = scp.getEntities();

                config_port cp = new config_port();
                cp.port_name = port_name;
                cp.baud_rate = baud_rate;
                cp.data_bits = Convert.ToInt32(data_bits);
                cp.stop_bits = Convert.ToInt32(stop_bits);
                cp.parity = paridad;
                cp.created_at = fecha_actual;
                cp.updated_at = fecha_actual;

                if (list.Count > 0)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (list[i].port_name == port_name)
                        {
                            cp.status = 1;
                            scp.updEntity(cp);
                        }
                        else
                        {
                            cp.status = 0;
                            scp.addEntity(cp);
                        }
                    }
                }

                //CONFIGURACION DE TIPO DE TARIFA
                string tipo_tarifa = cmbTipoTarifa.Text;
                ServiceTipoTarifa stt = new ServiceTipoTarifa();
                List<tipo_tarifa> list_tt = stt.getEntities();

                tipo_tarifa tt = new tipo_tarifa();
                tt.tipo_tarifa1 = tipo_tarifa;
                tt.created_at = Convert.ToDateTime(fecha_actual);
                tt.updated_at = Convert.ToDateTime(fecha_actual);

                if (list_tt.Count > 0)
                {
                    for (int i = 0; i < list_tt.Count; i++)
                    {
                        if (list_tt[i].tipo_tarifa1 == tipo_tarifa)
                        {
                            tt.status = 1;
                            scp.updEntity(cp);
                        }
                        else
                        {
                            tt.status = 0;
                            scp.addEntity(cp);
                        }
                    }
                }
                */
                #endregion



                MessageBox.Show("Configuracion Guardada con Exito", "INFO", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ATENCION", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }


    }
}
