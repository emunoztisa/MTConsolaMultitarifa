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
using TestMdfEntityFramework.Controllers;
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
            llenaComboModos();
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

            config_varios config_modo = scv.getEntityByClave("MODO");
            for (int i = 0; i < cmbModoApp.Items.Count; i++)
            {
                if (cmbModoApp.Items[i].ToString() == config_modo.valor)
                {
                    cmbModoApp.SelectedValue = config_modo.valor;
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

        public void llenaComboModos()
        {
            List<string> list = getCatalogo_Modos_BaseLocal();
            foreach (var port in list)
            {
                cmbModoApp.Items.Add(port);
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
        private List<string> getCatalogo_Modos_BaseLocal()
        {
            ServiceOpcionesGenerales sog = new ServiceOpcionesGenerales();
            List<opciones_generales> list = sog.getEntitiesByAgrupador("MODO");
            List<string> list2 = new List<string>();
            for (int i = 0; i < list.Count; i++)
            {
                list2.Add(list[i].opcion_general);
            }
            return list2;
        }

        private void btnGuardarConfiguracion_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //CONFIGURACION DE PUERTO SERIE
                string fecha_actual = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                string port_name = cmbComPorts.Text;
                string baud_rate = cmbBausRate.Text;
                string data_bits = cmbDataBits.Text;
                string stop_bits = cmbStopBits.Text;
                string paridad = cmbParity.Text;
                string handshake = cmbHandShake.Text;

                string impresora = cmbImpresoras.Text;
                string tipo_tarifa = cmbTipoTarifa.Text;
                string numero_unidad = cmbUnidad.Text;
                string modo = cmbModoApp.Text;



                ServiceConfigVarios scv = new ServiceConfigVarios();
                
                //valor de unidad antes de ser cambiada
                config_varios cv_num_unidad_antes_cambio = scv.getEntityByClave("NUMERO_UNIDAD");
                string unidad_antes_cambio = cv_num_unidad_antes_cambio.valor;
                
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

                cv.clave = "MODO";
                cv.valor = modo;
                scv.updEntityByClave(cv);

                cv.clave = "NUMERO_UNIDAD";
                cv.valor = numero_unidad;
                scv.updEntityByClave(cv);

                

                #endregion

                #region Cambiar status en DB TISA
                /*
                 1 = DISPONIBLE PARA ASIGNAR A ALGUNA UNIDAD
                 2 = ASIGNADA YA A ALGUNA UNIDAD
                */

                // Para Cambiar de status a 1 la unidad que sera liberada en el catalogo de TISA.
                ServiceUnidades serv_unidades_cambio_status_1 = new ServiceUnidades();
                ct_unidades unidad_1 = serv_unidades_cambio_status_1.getEntityByName(unidad_antes_cambio);

                ct_unidades cu_1 = new ct_unidades();
                cu_1.pkUnidad = unidad_1.pkUnidad;
                cu_1.fkEmpresa = unidad_1.fkEmpresa;
                cu_1.fkCorredor = unidad_1.fkCorredor;
                cu_1.nombre = unidad_1.nombre;
                cu_1.noSerieAVL = unidad_1.noSerieAVL;
                cu_1.economico = unidad_1.economico;
                cu_1.capacidad = unidad_1.capacidad;
                cu_1.validador = unidad_1.validador;
                cu_1.status = 1;
                cu_1.created_at = unidad_1.created_at;
                cu_1.updated_at = fecha_actual;
                cu_1.deleted_at = unidad_1.deleted_at;

                UnidadesController uc_1 = new UnidadesController();
                uc_1.UpdateUnidad(cu_1);

                //Para Cambiar de status a 2 la unidad que sera asignada a la consola.
                ServiceUnidades serv_unidades_cambio_status_2 = new ServiceUnidades();
                ct_unidades unidad_2 = serv_unidades_cambio_status_2.getEntityByName(cv.valor);

                ct_unidades cu_2 = new ct_unidades();
                cu_2.pkUnidad = unidad_2.pkUnidad;
                cu_2.fkEmpresa = unidad_2.fkEmpresa;
                cu_2.fkCorredor = unidad_2.fkCorredor;
                cu_2.nombre = unidad_2.nombre;
                cu_2.noSerieAVL = unidad_2.noSerieAVL;
                cu_2.economico = unidad_2.economico;
                cu_2.capacidad = unidad_2.capacidad;
                cu_2.validador = unidad_2.validador;
                cu_2.status = 2;
                cu_2.created_at = unidad_2.created_at;
                cu_2.updated_at = fecha_actual;
                cu_2.deleted_at = unidad_2.deleted_at;

                UnidadesController uc_2 = new UnidadesController();
                uc_2.UpdateUnidad(cu_2);

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
