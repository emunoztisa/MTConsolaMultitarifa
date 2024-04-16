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
using System.Windows.Media.Animation;
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

        #region INICIALIZAR VARIBLES

        //SERIAL PORT
        System.IO.Ports.SerialPort puertoSerie1 = new System.IO.Ports.SerialPort();
        String[] listado_puerto = System.IO.Ports.SerialPort.GetPortNames();

        //BUFFERS SEND AND RECIEVED
        byte[] BufferSendData = new byte[80];
        byte[] RecievedDataGlobal = new byte[80];

        //POPUP OK
        private double left, top, right, bottom, centerX, centerY;
        private DoubleAnimation bottomToCenterAnimiation, topToCenterAnimation,
            leftToCenterAnimation, rightToCenterAnimation;
        private Storyboard bottomToCenterStoryboard, topToCenterStoryboard,
            leftToCenterStoryboard, rightToCenterStoryboard;

        #endregion
        public Configuracionv2()
        {
            InitializeComponent();
            configura_puerto_serial();
            llenaCombos();
            Task.WaitAll(new Task[] { Task.Delay(300) });

        }

        #region LLENA COMBOS

        public void llenaCombos()
        {
            llenaComboComPorts();
            llenaComboBaudRate();
            llenaComboDataBits();
            llenaComboStopBits();
            llenaComboParidad();
            llenaComboHandshake();

            llenaComboModos();
            llenaComboTipoTarifa();

            //llenaComboImpresoras();

            llenaComboEmpresas();
            //llenaComboCorredores();
            //llenaComboRutas();
            //llenaComboUnidades();

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

            //config_varios config_impresora = scv.getEntityByClave("IMPRESORA_DEFAULT");
            //for (int i = 0; i < cmbImpresoras.Items.Count; i++)
            //{
            //    if (cmbImpresoras.Items[i].ToString() == config_impresora.valor)
            //    {
            //        cmbImpresoras.SelectedValue = config_impresora.valor;
            //    }
            //}

           

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

            config_varios config_empresa = scv.getEntityByClave("EMPRESA");
            for (int i = 0; i < cmbEmpresa.Items.Count; i++)
            {
                if (cmbEmpresa.Items[i].ToString() == config_empresa.valor)
                {
                    cmbEmpresa.SelectedValue = config_empresa.valor;
                }
            }

            config_varios config_corredor = scv.getEntityByClave("CORREDOR");
            for (int i = 0; i < cmbCorredor.Items.Count; i++)
            {
                if (cmbCorredor.Items[i].ToString() == config_corredor.valor)
                {
                    cmbCorredor.SelectedValue = config_corredor.valor;
                }
            }

            config_varios config_ruta = scv.getEntityByClave("RUTA");
            for (int i = 0; i < cmbRuta.Items.Count; i++)
            {
                if (cmbRuta.Items[i].ToString() == config_ruta.valor)
                {
                    cmbRuta.SelectedValue = config_ruta.valor;
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

            #region obtener mensajes configurados en boleto
            string enc_linea_1 = ejecutar_commando_83_get_mensaje(0);
            string enc_linea_2 = ejecutar_commando_83_get_mensaje(1);
            string enc_linea_3 = ejecutar_commando_83_get_mensaje(2);

            txtEncabezadoLinea1.Text = enc_linea_1;
            txtEncabezadoLinea2.Text = enc_linea_2;
            txtEncabezadoLinea3.Text = enc_linea_3;

            string pie_linea_1 = ejecutar_commando_83_get_mensaje(3);
            string pie_linea_2 = ejecutar_commando_83_get_mensaje(4);
            string pie_linea_3 = ejecutar_commando_83_get_mensaje(5);

            txtPiePaginaLinea1.Text = pie_linea_1;
            txtPiePaginaLinea2.Text = pie_linea_2;
            txtPiePaginaLinea3.Text = pie_linea_3;
            #endregion

            #region obtener No Serie Alcancia
            string no_serie_alcancia = ejecutar_commando_88_obtener_serie_alcancia();
            lblNoSerieAlcancia.Text = no_serie_alcancia;
            #endregion

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

        //public void llenaComboImpresoras()
        //{
        //    List<string> list = getPrinterDevices();
        //    foreach (var imp in list)
        //    {
        //        cmbImpresoras.Items.Add(imp);
        //    }
        //}

        public void llenaComboEmpresas()
        {
            List<string> list = getCatalogo_Empresas_BaseLocal();
            foreach (var li in list)
            {
                cmbEmpresa.Items.Add(li);
            }
        }
        public void llenaComboCorredores()
        {
            List<string> list = getCatalogo_Corredores_BaseLocal();
            foreach (var li in list)
            {
                cmbCorredor.Items.Add(li);
            }
        }
        public void llenaComboRutas()
        {
            List<string> list = getCatalogo_Rutas_BaseLocal();
            foreach (var li in list)
            {
                cmbRuta.Items.Add(li);
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

        #endregion

        #region CATALOGOS

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

        private List<string> getCatalogo_Empresas_BaseLocal()
        {
            ServiceEmpresas serv = new ServiceEmpresas();
            List<ct_empresas> list = serv.getEntities();
            List<string> list2 = new List<string>();
            for (int i = 0; i < list.Count; i++)
            {
                list2.Add(list[i].nombre);
            }
            return list2;
        }
        private List<string> getCatalogo_Corredores_BaseLocal()
        {
            ServiceCorredores serv = new ServiceCorredores();
            List<ct_corredores> list = serv.getEntities();
            List<string> list2 = new List<string>();
            for (int i = 0; i < list.Count; i++)
            {
                list2.Add(list[i].nombre);
            }
            return list2;
        }
        private List<string> getCatalogo_Rutas_BaseLocal()
        {
            ServiceRutas serv = new ServiceRutas();
            List<ct_rutas> list = serv.getEntities();
            List<string> list2 = new List<string>();
            for (int i = 0; i < list.Count; i++)
            {
                list2.Add(list[i].nombre);
            }
            return list2;
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

        #endregion

        #region EVENTOS CONTROLES
        private void Configuracion_OnLoad(object sender, RoutedEventArgs e)
        {
            //MuestraValoresConfigSerial();

            //EVENTOS PARA POPUP OK
            SetPopupDlgCenter();
            InitializeAnimations();

            SeteaValoresEnCombosConValoresDBLocal();
        }
        private void Configuracionv2_OnUnload(object sender, RoutedEventArgs e)
        {
            close_serial_port();
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

                //string impresora = cmbImpresoras.Text;
                string modo = cmbModoApp.Text;
                string tipo_tarifa = cmbTipoTarifa.Text;

                string empresa = cmbEmpresa.Text;
                string corredor = cmbCorredor.Text;
                string ruta = cmbRuta.Text;
                string numero_unidad = cmbUnidad.Text;

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

                cv.clave = "MODO";
                cv.valor = modo;
                scv.updEntityByClave(cv);

                //cv.clave = "IMPRESORA_DEFAULT";
                //cv.valor = impresora;
                //scv.updEntityByClave(cv);

                cv.clave = "EMPRESA";
                cv.valor = empresa;
                scv.updEntityByClave(cv);

                cv.clave = "CORREDOR";
                cv.valor = corredor;
                scv.updEntityByClave(cv);

                cv.clave = "RUTA";
                cv.valor = ruta;
                scv.updEntityByClave(cv);
                ejecutar_commando_86_ruta(ruta);

                cv.clave = "NUMERO_UNIDAD";
                cv.valor = numero_unidad;
                scv.updEntityByClave(cv);
                ejecutar_commando_86_unidad(numero_unidad);



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

                #region Setear Mensajes en Boleto
                ejecutar_commando_82_set_mensaje(0, txtEncabezadoLinea1.Text);
                ejecutar_commando_82_set_mensaje(1, txtEncabezadoLinea2.Text);
                ejecutar_commando_82_set_mensaje(2, txtEncabezadoLinea3.Text);

                ejecutar_commando_82_set_mensaje(3, txtPiePaginaLinea1.Text);
                ejecutar_commando_82_set_mensaje(4, txtPiePaginaLinea2.Text);
                ejecutar_commando_82_set_mensaje(5, txtPiePaginaLinea3.Text);
                #endregion

                //MessageBox.Show("Configuracion Guardada con Exito", "INFO", MessageBoxButton.OK, MessageBoxImage.Information);

                txtMensajePopup.Text = "CONFIGURACION EXITOSA";
                mostrarPopupOk();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ATENCION", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
        //string nombre_combo_cambiado_actualmente = "";
        private void cmbEmpresa_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //nombre_combo_cambiado_actualmente = "cmbEmpresa";

            string nombreCombo = ((ComboBox)sender).Name;
            if (nombreCombo == "cmbEmpresa")
            {
                cmbCorredor.Items.Clear();

                var currentSelectedIndex = ((ComboBox)sender).SelectedIndex;
                string textSelected = ((ComboBox)sender).Items[currentSelectedIndex].ToString();
                
                //obtener el pkEmpresa de la seleccion en el combo empresa
                ServiceEmpresas serv_empresas = new ServiceEmpresas();
                ct_empresas obj_empresa_selected = serv_empresas.getEntityPorNombreEmpresa(textSelected);

                //obtener los corredores por fkEmpresa
                ServiceCorredores serv_corredores = new ServiceCorredores();
                List<ct_corredores> list_corredores = serv_corredores.getEntityPorFkEmpresa(obj_empresa_selected.pkEmpresa);

                List<string> list2 = new List<string>();
                for (int i = 0; i < list_corredores.Count; i++)
                {
                    list2.Add(list_corredores[i].nombre);
                }
                foreach (var li in list2)
                {
                    cmbCorredor.Items.Add(li);
                }
            }
        }
        private void cmbCorredor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                string nombreCombo = ((ComboBox)sender).Name;
                //if (nombreCombo == "cmbCorredor" && nombre_combo_cambiado_actualmente == "")
                //{
                    var currentSelectedIndex = ((ComboBox)sender).SelectedIndex;
                    string textSelected = ((ComboBox)sender).Items[currentSelectedIndex].ToString();


                    //obtener el pkCorredor de la seleccion en el combo corredor
                    ServiceCorredores serv_corredor = new ServiceCorredores();
                    ct_corredores obj_corredor_selected = serv_corredor.getEntityPorNombreCorredor(textSelected);

                    //obtener los rutas por fkCorredor
                    cmbRuta.Items.Clear();

                    ServiceRutas serv_rutas = new ServiceRutas();
                    List<ct_rutas> list_rutas = serv_rutas.getEntityPorFkCorredor(obj_corredor_selected.pkCorredor);

                    List<string> list2 = new List<string>();
                    for (int i = 0; i < list_rutas.Count; i++)
                    {
                        list2.Add(list_rutas[i].nombre);
                    }
                    foreach (var li in list2)
                    {
                        cmbRuta.Items.Add(li);
                    }

                    //obtener las unidades por fkCorredor
                    cmbUnidad.Items.Clear();

                    ServiceUnidades serv_unidades = new ServiceUnidades();
                    List<ct_unidades> list_unidades = serv_unidades.getEntityPorFkCorredor(obj_corredor_selected.pkCorredor);

                    List<string> list3 = new List<string>();
                    for (int i = 0; i < list_unidades.Count; i++)
                    {
                        list3.Add(list_unidades[i].nombre);
                    }
                    foreach (var li in list3)
                    {
                        cmbUnidad.Items.Add(li);
                    }
                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show("!!!!!", "ATENCION !!!");
            }
            
        }

        #endregion

        #region PUERTO SERIAL
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

                this.puertoSerie1 = new System.IO.Ports.SerialPort
                    ("" + cv_port_name.valor
                    , Convert.ToInt32(cv_baud_rate.valor)
                    , cv_paridad.valor == "NONE" ? System.IO.Ports.Parity.None : System.IO.Ports.Parity.Mark
                    , Convert.ToInt32(cv_data_bits.valor)
                    , Convert.ToInt32(cv_stop_bits.valor) == 1 ? System.IO.Ports.StopBits.One : System.IO.Ports.StopBits.None
                    );
                puertoSerie1.Handshake = cv_handshake.valor == "NONE" ? System.IO.Ports.Handshake.None : System.IO.Ports.Handshake.XOnXOff;
            }
            catch
            {
                MessageBox.Show("Verifique" + System.Environment.NewLine + "- Alimentación" + System.Environment.NewLine + "- Conexión del puerto", "Error de puerto COMM");
            }

        }
        private void open_serial_port()
        {
            if (puertoSerie1.IsOpen == false)
            {
                puertoSerie1.Open();
            }
        }
        private void close_serial_port()
        {
            if (puertoSerie1.IsOpen == true)
            {
                puertoSerie1.Close();
            }
        }
        private void MuestraValoresConfigSerial()
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

        #endregion

        #region COMANDOS
        void ClearBufferSendData()
        {
            for (int i = 0; i < BufferSendData.Length; i++)
            {
                BufferSendData[i] = 0;
            }
        }
        private void ejecutar_commando_86_ruta(string nombre_ruta)
        {
            const decimal ByteInicio = 1;
            const decimal AddressConsola = 1;
            const decimal AddressAlcancia = 2;
            const decimal Comando = 134;
            const decimal TipoConfig = 0; // RUTA

            const Int32 K_offsetDatos = 4;
            const Int32 K_CRCs = 2;

            const decimal CRC1 = 193;
            const decimal CRC2 = 194;

            int CantidadDatos = 16;
            CantidadDatos = nombre_ruta.Length + 2;

            ClearBufferSendData();

            BufferSendData[0] = decimal.ToByte(ByteInicio);
            BufferSendData[1] = decimal.ToByte(AddressAlcancia);
            BufferSendData[2] = decimal.ToByte(AddressConsola);
            BufferSendData[3] = decimal.ToByte(CantidadDatos);
            BufferSendData[4] = decimal.ToByte(Comando);
            BufferSendData[5] = decimal.ToByte(TipoConfig);

            int n = 6;
            for (int i = 0; i < nombre_ruta.Length; i++)
            {
                BufferSendData[n++] = (byte)nombre_ruta[i];
            }

            BufferSendData[K_offsetDatos + CantidadDatos] = decimal.ToByte(CRC1);
            BufferSendData[K_offsetDatos + CantidadDatos + 1] = decimal.ToByte(CRC2);

            open_serial_port();
            puertoSerie1.Write(BufferSendData, 0, K_offsetDatos + CantidadDatos + K_CRCs);

            Task.WaitAll(new Task[] { Task.Delay(200) });
            puertoSerie1.Read(RecievedDataGlobal, 0, 50);

            if (RecievedDataGlobal[5] == 0)
            {
                //MessageBox.Show("Se cambio la Ruta Correctamente", "INFO");
            }
            else
            {
                MessageBox.Show("Verifique la estructura del comando enviado", "Error en comando enviado");
            }
        }
        private void ejecutar_commando_86_unidad(string nombre_unidad)
        {
            const decimal ByteInicio = 1;
            const decimal AddressConsola = 1;
            const decimal AddressAlcancia = 2;
            const decimal Comando = 134;
            const decimal TipoConfig = 1; // UNIDAD

            const Int32 K_offsetDatos = 4;
            const Int32 K_CRCs = 2;

            const decimal CRC1 = 193;
            const decimal CRC2 = 194;

            int CantidadDatos = 16;
            CantidadDatos = nombre_unidad.Length + 2;

            ClearBufferSendData();

            BufferSendData[0] = decimal.ToByte(ByteInicio);
            BufferSendData[1] = decimal.ToByte(AddressAlcancia);
            BufferSendData[2] = decimal.ToByte(AddressConsola);
            BufferSendData[3] = decimal.ToByte(CantidadDatos);
            BufferSendData[4] = decimal.ToByte(Comando);
            BufferSendData[5] = decimal.ToByte(TipoConfig);

            int n = 6;
            for (int i = 0; i < nombre_unidad.Length; i++)
            {
                BufferSendData[n++] = (byte)nombre_unidad[i];
            }

            BufferSendData[K_offsetDatos + CantidadDatos] = decimal.ToByte(CRC1);
            BufferSendData[K_offsetDatos + CantidadDatos + 1] = decimal.ToByte(CRC2);

            open_serial_port();
            puertoSerie1.Write(BufferSendData, 0, K_offsetDatos + CantidadDatos + K_CRCs);

            Task.WaitAll(new Task[] { Task.Delay(200) });
            puertoSerie1.Read(RecievedDataGlobal, 0, 50);

            if (RecievedDataGlobal[5] == 0)
            {
                //MessageBox.Show("Se cambio la Unidad Correctamente", "INFO");
            }
            else
            {
                MessageBox.Show("Verifique la estructura del comando enviado", "Error en comando enviado");
            }
        }


        private string GetMensajeString(byte[] RecievedDataGlobal_local)
        {
            string empty = " ";
            string cadena = "";
            int n = 6;
            for (int i = 0; i < 32; i++)
            {
                //if((byte)RecievedDataGlobal_local[i] != 0 && (byte)RecievedDataGlobal_local[i] != 32 && (byte)RecievedDataGlobal_local[i] != (byte)empty[0])
                //{
                    char varTmp = (char)RecievedDataGlobal_local[n++];
                    cadena += varTmp.ToString();
                //}
               
            }

            //cadena = cadena.Substring(0, cadena.Length - 5);
            return cadena;
        }
        private void ejecutar_commando_82_set_mensaje(decimal num_linea, string mensaje)
        {
            const decimal ByteInicio = 1;
            const decimal AddressConsola = 1;
            const decimal AddressAlcancia = 2;
            const decimal Comando = 130;
            decimal numero_linea = num_linea; 

            const Int32 K_offsetDatos = 4;
            const Int32 K_CRCs = 2;

            const decimal CRC1 = 193;
            const decimal CRC2 = 194;

            int CantidadDatos = 0;
            CantidadDatos = 32 + 2;

            ClearBufferSendData();

            BufferSendData[0] = decimal.ToByte(ByteInicio);
            BufferSendData[1] = decimal.ToByte(AddressAlcancia);
            BufferSendData[2] = decimal.ToByte(AddressConsola);
            BufferSendData[3] = decimal.ToByte(CantidadDatos);
            BufferSendData[4] = decimal.ToByte(Comando);
            BufferSendData[5] = decimal.ToByte(numero_linea);

            int n = 6;
            for (int i = 0; i < 32; i++)
            {
                if((byte)mensaje[i] != 0)
                {
                    BufferSendData[n++] = (byte)mensaje[i];
                }
                else
                {
                    string empty = " ";
                    BufferSendData[n++] = (byte)empty[0];
                }
            }
            
            BufferSendData[K_offsetDatos + CantidadDatos] = decimal.ToByte(CRC1);
            BufferSendData[K_offsetDatos + CantidadDatos + 1] = decimal.ToByte(CRC2);

            open_serial_port();
            puertoSerie1.Write(BufferSendData, 0, K_offsetDatos + CantidadDatos + K_CRCs);

            Task.WaitAll(new Task[] { Task.Delay(100) });
            puertoSerie1.Read(RecievedDataGlobal, 0, 50);

            if (RecievedDataGlobal[5] == 0)
            {
                //MessageBox.Show("Se cambio la Ruta Correctamente", "INFO");
            }
            else
            {
                MessageBox.Show("Verifique la estructura del comando enviado", "Error en comando enviado");
            }
        }
        private string ejecutar_commando_83_get_mensaje(int num_linea)
        {
            string mensaje = "";
            try
            {
                const decimal ByteInicio = 1;
                const decimal AddressConsola = 1;
                const decimal AddressAlcancia = 2;
                const decimal Comando = 131;
                decimal numero_linea = num_linea;

                const Int32 K_offsetDatos = 4;
                const Int32 K_CRCs = 2;

                const decimal CRC1 = 193;
                const decimal CRC2 = 194;

                int CantidadDatos = 0;
                CantidadDatos = 2;

                ClearBufferSendData();

                BufferSendData[0] = decimal.ToByte(ByteInicio);
                BufferSendData[1] = decimal.ToByte(AddressAlcancia);
                BufferSendData[2] = decimal.ToByte(AddressConsola);
                BufferSendData[3] = decimal.ToByte(CantidadDatos);
                BufferSendData[4] = decimal.ToByte(Comando);
                BufferSendData[5] = decimal.ToByte(numero_linea);


                BufferSendData[K_offsetDatos + CantidadDatos] = decimal.ToByte(CRC1);
                BufferSendData[K_offsetDatos + CantidadDatos + 1] = decimal.ToByte(CRC2);

                open_serial_port();
                puertoSerie1.Write(BufferSendData, 0, K_offsetDatos + CantidadDatos + K_CRCs);

                Task.WaitAll(new Task[] { Task.Delay(100) });
                puertoSerie1.Read(RecievedDataGlobal, 0, 50);

                if (GetMensajeString(RecievedDataGlobal) != "")
                {
                    mensaje = GetMensajeString(RecievedDataGlobal);
                }
                else
                {
                    mensaje = "";
                }
            }
            catch (Exception ex)
            {
                mensaje = "";
                MessageBox.Show("Verifique la estructura del comando enviado", "Error en comando enviado");
            }
            
            return mensaje;
        }


        private string GetNoSerieAlcanciaString(byte[] RecievedDataGlobal_local)
        {
            string cadena = "";
            int n = 5;
            for (int i = 0; i < 16; i++)
            {
                char varTmp = (char)RecievedDataGlobal_local[n++];
                cadena += varTmp.ToString();
            }
            return cadena;
        }
        private string ejecutar_commando_88_obtener_serie_alcancia()
        {
            string no_serie_alcancia = "";

            const decimal ByteInicio = 1;
            const decimal AddressConsola = 1;
            const decimal AddressAlcancia = 2;
            const decimal Comando = 136;

            const Int32 K_offsetDatos = 4;
            const Int32 K_CRCs = 2;

            const decimal CRC1 = 193;
            const decimal CRC2 = 194;

            int CantidadDatos = 1;

            ClearBufferSendData();

            BufferSendData[0] = decimal.ToByte(ByteInicio);
            BufferSendData[1] = decimal.ToByte(AddressAlcancia);
            BufferSendData[2] = decimal.ToByte(AddressConsola);
            BufferSendData[3] = decimal.ToByte(CantidadDatos);
            BufferSendData[4] = decimal.ToByte(Comando);

            BufferSendData[K_offsetDatos + CantidadDatos] = decimal.ToByte(CRC1);
            BufferSendData[K_offsetDatos + CantidadDatos + 1] = decimal.ToByte(CRC2);

            open_serial_port();
            puertoSerie1.Write(BufferSendData, 0, K_offsetDatos + CantidadDatos + K_CRCs);

            Task.WaitAll(new Task[] { Task.Delay(100) });
            puertoSerie1.Read(RecievedDataGlobal, 0, 50);

            if (RecievedDataGlobal[5].ToString().Length >= 0)
            {
                no_serie_alcancia = GetNoSerieAlcanciaString(RecievedDataGlobal);
            }
            else
            {
                MessageBox.Show("Verifique la estructura del comando enviado", "Error en comando enviado");
            }
            return no_serie_alcancia;
        }
        #endregion

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
