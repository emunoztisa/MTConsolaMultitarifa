using AForge.Video.DirectShow;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Speech.Synthesis;
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

        //PARA REPRODUCIR TEXTO A VOZ
        SpeechSynthesizer synthesizer = new SpeechSynthesizer();
        List<VoiceInfo> vocesInfo = new List<VoiceInfo>();

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

        //CAMARA
        private bool HayDispositivos;
        private FilterInfoCollection MisDispositivos;
        private VideoCaptureDevice MiWebCam;

        #endregion
        public Configuracionv2()
        {
            InitializeComponent();

            //if (validaPuertoCOMConfigurado())
            //{
            //    configura_puerto_serial();
            //}

            llenaCombos();
            Task.WaitAll(new Task[] { Task.Delay(200) });

        }
        private void Configuracion_OnLoad(object sender, RoutedEventArgs e)
        {
            //MuestraValoresConfigSerial();

            if (validaPuertoCOMConfigurado())
            {
                configura_puerto_serial();
            }

            //EVENTOS PARA POPUP OK
            SetPopupDlgCenter();
            InitializeAnimations();

            try
            {
                if (isValidComPortAndConnected())
                {
                    SeteaValoresEnCombosConValoresDBLocal();
                    SeteaValoresCheckboxConValoresDBLocal();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ATENCION !!!");
            }
        }
        private void Configuracionv2_OnUnload(object sender, RoutedEventArgs e)
        {
            close_serial_port();
            dispose_serial_port();
        }

        #region METODOS PARA SETEAR VALORES EN CONTROLES
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

            config_varios config_modo = scv.getEntityByClave("MODO");
            for (int i = 0; i < cmbModoApp.Items.Count; i++)
            {
                if (cmbModoApp.Items[i].ToString() == config_modo.valor)
                {
                    cmbModoApp.SelectedValue = config_modo.valor;
                }
            }

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

            config_varios config_voz = scv.getEntityByClave("VOZ");
            for (int i = 0; i < cmbVoces.Items.Count; i++)
            {
                if (cmbVoces.Items[i].ToString() == config_voz.valor)
                {
                    cmbVoces.SelectedValue = config_voz.valor;
                }
            }

            config_varios config_voz_activa = scv.getEntityByClave("VOZ_ACTIVA");
            for (int i = 0; i < cmbActivarVoz.Items.Count; i++)
            {
                if (cmbActivarVoz.Items[i].ToString() == config_voz_activa.valor)
                {
                    cmbActivarVoz.SelectedValue = config_voz_activa.valor;
                }
            }

            config_varios config_camara_default = scv.getEntityByClave("CAMARA_DEFAULT");
            for (int i = 0; i < cmbCamaras.Items.Count; i++)
            {
                if (cmbCamaras.Items[i].ToString() == config_camara_default.valor)
                {
                    cmbCamaras.SelectedValue = config_camara_default.valor;
                }
            }

            config_varios config_impresora_default = scv.getEntityByClave("IMPRESORA_DEFAULT");
            for (int i = 0; i < cmbImpresoras.Items.Count; i++)
            {
                if (cmbImpresoras.Items[i].ToString() == config_impresora_default.valor)
                {
                    cmbImpresoras.SelectedValue = config_impresora_default.valor;
                }
            }


            config_varios config_camara_activa = scv.getEntityByClave("CAMARA_ACTIVA");
            for (int i = 0; i < cmbActivarCamara.Items.Count; i++)
            {
                if (cmbActivarCamara.Items[i].ToString() == config_camara_activa.valor)
                {
                    cmbActivarCamara.SelectedValue = config_camara_activa.valor;
                }
            }

            config_varios config_impresora_activa = scv.getEntityByClave("IMPRESORA_ACTIVA");
            for (int i = 0; i < cmbActivarImpresora.Items.Count; i++)
            {
                if (cmbActivarImpresora.Items[i].ToString() == config_impresora_activa.valor)
                {
                    cmbActivarImpresora.SelectedValue = config_impresora_activa.valor;
                }
            }

            config_varios config_ubicacion_activa = scv.getEntityByClave("UBICACION_ACTIVA");
            for (int i = 0; i < cmbActivarUbicacion.Items.Count; i++)
            {
                if (cmbActivarUbicacion.Items[i].ToString() == config_ubicacion_activa.valor)
                {
                    cmbActivarUbicacion.SelectedValue = config_ubicacion_activa.valor;
                }
            }

            config_varios config_url_base = scv.getEntityByClave("BASE_URL");
            if (config_url_base != null)
            {
                txtUrlBase.Text = config_url_base.valor;
            }

            config_varios config_prefijo_folio_boleto = scv.getEntityByClave("PREFIJO_FOLIO_BOLETO");
            if (config_prefijo_folio_boleto != null)
            {
                txtPrefijoBoleto.Text = config_prefijo_folio_boleto.valor;
            }

            config_varios config_prefijo_folio_corte = scv.getEntityByClave("PREFIJO_FOLIO_CORTE");
            if (config_prefijo_folio_corte != null)
            {
                txtPrefijoCorte.Text = config_prefijo_folio_corte.valor;
            }

            config_varios config_cant_usuarios_perfil = scv.getEntityByClave("CANTIDAD_PERSONAS_PERFIL");
            for (int i = 0; i < cmbCantUsuariosPerfil.Items.Count; i++)
            {
                if (cmbCantUsuariosPerfil.Items[i].ToString() == config_cant_usuarios_perfil.valor)
                {
                    cmbCantUsuariosPerfil.SelectedValue = config_cant_usuarios_perfil.valor;
                }
            }

            config_varios config_logo_home = scv.getEntityByClave("LOGO_HOME");
            for (int i = 0; i < cmbImagenesSubidas.Items.Count; i++)
            {
                if (cmbImagenesSubidas.Items[i].ToString() == config_logo_home.valor)
                {
                    cmbImagenesSubidas.SelectedValue = config_logo_home.valor;
                }
            }


            if (validaPuertoCOMConfigurado())
            {
                if (validaDispositivoConectadoEnPuertoCOM())
                {
                    SetearValoresTextoEncabezadosYPiePagina();
                    SetearValoresLblNoSerieAlcancia();
                    SetearValoresTextoDisplayAlcancia();
                }
                else
                {
                    MessageBox.Show("Favor de Validar que este conectado el dispositivo al puerto COM");
                }

            }

        }
        private void SetearValoresTextoEncabezadosYPiePagina()
        {
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
        }
        private void SetearValoresLblNoSerieAlcancia()
        {
            #region obtener No Serie Alcancia
            string no_serie_alcancia = ejecutar_commando_88_obtener_serie_alcancia();
            lblNoSerieAlcancia.Text = no_serie_alcancia;
            #endregion
        }
        private void SetearValoresTextoDisplayAlcancia()
        {
            #region obtener el texto configurado para el display de alcancia

            string texto_display_alcancia = ejecutar_commando_85_get_texto_display_alcancia();
            txtTextoDisplayAlcancia.Text = texto_display_alcancia;

            #endregion
        }

        private void SeteaValoresCheckboxConValoresDBLocal()
        {
            ServiceConfigVarios serv_config_varios = new ServiceConfigVarios();
            
            config_varios cv_sinc_tarifas_montos_fijos = serv_config_varios.getEntityByClave("SINC_TISA_TARIFAS_MONTOS_FIJOS");
            if(cv_sinc_tarifas_montos_fijos != null)
            {
                if(cv_sinc_tarifas_montos_fijos.valor == "SI") 
                {
                    chkSinTISA_TarifasMontosFijos.IsChecked = true;
                }
                else if (cv_sinc_tarifas_montos_fijos.valor == "NO")
                {
                    chkSinTISA_TarifasMontosFijos.IsChecked = false;
                }
                else
                {
                    chkSinTISA_TarifasMontosFijos.IsChecked = false;
                }
            }

            config_varios cv_sinc_denominaciones = serv_config_varios.getEntityByClave("SINC_TISA_DENOMINACIONES");
            if (cv_sinc_denominaciones != null)
            {
                if (cv_sinc_denominaciones.valor == "SI")
                {
                    chkSinTISA_Denominaciones.IsChecked = true;
                }
                else if (cv_sinc_denominaciones.valor == "NO")
                {
                    chkSinTISA_Denominaciones.IsChecked = false;
                }
                else
                {
                    chkSinTISA_Denominaciones.IsChecked = false;
                }
            }

            config_varios cv_sinc_opciones_generales = serv_config_varios.getEntityByClave("SINC_TISA_OPCIONES_GENERALES");
            if (cv_sinc_opciones_generales != null)
            {
                if (cv_sinc_opciones_generales.valor == "SI")
                {
                    chkSinTISA_OpcionesGenerales.IsChecked = true;
                }
                else if (cv_sinc_opciones_generales.valor == "NO")
                {
                    chkSinTISA_OpcionesGenerales.IsChecked = false;
                }
                else
                {
                    chkSinTISA_OpcionesGenerales.IsChecked = false;
                }
            }

            config_varios cv_sinc_config_varios = serv_config_varios.getEntityByClave("SINC_TISA_CONFIG_VARIOS");
            if (cv_sinc_config_varios != null)
            {
                if (cv_sinc_config_varios.valor == "SI")
                {
                    chkSinTISA_ConfigVarios.IsChecked = true;
                }
                else if (cv_sinc_config_varios.valor == "NO")
                {
                    chkSinTISA_ConfigVarios.IsChecked = false;
                }
                else
                {
                    chkSinTISA_ConfigVarios.IsChecked = false;
                }
            }

        }

        #endregion

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

            LlenarVocesEnCombo();

            llenarComboActivarUbicacion();
            llenarComboActivarVoz();
            llenarComboCantUsuariosPerfiles();

            llenarPosicionesArregloDenominaciones();
            

            llenaComboImagenesSubidas();
            llenaComboImagenesDenominaciones();

            //llenaComboCamaras();
            llenaComboImpresoras();

            llenarComboActivarCamara();
            llenarComboActivarImpresora();

            llenarComboOrdenTarifasMontosFijos();

            llenarGridDenominaciones();
            llenarGridTarifasMontoFijo();
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
        private void llenarComboCantUsuariosPerfiles()
        {
            List<string> list = getCatalogo_CantUsuariosPerfil_BaseLocal();
            foreach (var item in list)
            {
                cmbCantUsuariosPerfil.Items.Add(item);
            }
        }
        private void llenarComboActivarVoz()
        {
            List<string> list = getCatalogo_ActivarVoz_BaseLocal();
            foreach (var item in list)
            {
                cmbActivarVoz.Items.Add(item);
            }
        }
        private void llenarComboActivarUbicacion()
        {
            List<string> list = getCatalogo_ActivarUbicacion_BaseLocal();
            foreach (var item in list)
            {
                cmbActivarUbicacion.Items.Add(item);
            }
        }
        private void LlenarVocesEnCombo()
        {
            foreach (InstalledVoice voice in synthesizer.GetInstalledVoices())
            {
                vocesInfo.Add(voice.VoiceInfo);
                cmbVoces.Items.Add(voice.VoiceInfo.Name);
            }
            cmbVoces.SelectedIndex = 0;
        }
        private void llenarGridDenominaciones()
        {
            dataGridDenominaciones.Items.Clear();

            List<ct_denominaciones> list = getCatalogo_Denominaciones_BaseLocal();
            foreach (var item in list)
            {
                dataGridDenominaciones.Items.Add(new ct_denominaciones 
                { 
                    posicion = item.posicion
                    , nombre = item.nombre
                    , valor = item.valor
                    , path_imagen = item.path_imagen
                    , status = item.status 
                });
            }
        }
        private void llenarPosicionesArregloDenominaciones()
        {
            for (int i = 0; i <= 6; i++)
            {
                cmbPosicionArreglo.Items.Add(i);
            }
            for (int i = 10; i <= 16; i++)
            {
                cmbPosicionArreglo.Items.Add(i);
            }
        }
        public void llenaComboImagenesSubidas()
        {
            List<string> list = getCatalogo_ImagenesSubidas_BaseLocal();
            foreach (var li in list)
            {
                cmbImagenesSubidas.Items.Add(li);
            }
        }

        public void llenaComboImagenesDenominaciones()
        {
            List<string> list = getCatalogo_ImagenesDenominaciones_BaseLocal();
            foreach (var li in list)
            {
                cmbImagenesDenominaciones.Items.Add(li);
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
        public void llenaComboCamaras()
        {
            List<string> list = getPrinterDevices();
            MisDispositivos = new FilterInfoCollection(FilterCategory.VideoInputDevice);

            if (MisDispositivos.Count > 0)
            {
                HayDispositivos = true;
                for (int i = 0; i < MisDispositivos.Count; i++)
                {
                    cmbCamaras.Items.Add(MisDispositivos[i].Name.ToString().Trim());
                }

                cmbCamaras.Text = MisDispositivos[0].Name.ToString();
            }
            else
            {
                HayDispositivos = false;
            }
        }
        private void llenarComboActivarCamara()
        {
            List<string> list = getCatalogo_ActivarCamara_BaseLocal();
            foreach (var item in list)
            {
                cmbActivarCamara.Items.Add(item);
            }
        }
        private void llenarComboActivarImpresora()
        {
            List<string> list = getCatalogo_ActivarImpresora_BaseLocal();
            foreach (var item in list)
            {
                cmbActivarImpresora.Items.Add(item);
            }
        }
        private void llenarComboOrdenTarifasMontosFijos()
        {
            for (int i = 1; i <= 6; i++)
            {
                cmbOrden_TarifaMontoFijo.Items.Add(i);
            }
        }
        private void llenarGridTarifasMontoFijo()
        {
            dataGridTarifasMontosFijos.Items.Clear();

            List<ct_tarifas_montos_fijos> list = getCatalogo_TarifasMontoFijo_BaseLocal();
            foreach (var item in list)
            {
                dataGridTarifasMontosFijos.Items.Add(new ct_tarifas_montos_fijos 
                { 
                    valor = item.valor
                    , texto = item.texto
                    , descripcion = item.descripcion
                    , orden = item.orden
                    , status = item.status 
                });
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

        private List<string> getCatalogo_CantUsuariosPerfil_BaseLocal()
        {
            ServiceOpcionesGenerales sog = new ServiceOpcionesGenerales();
            List<opciones_generales> list = sog.getEntitiesByAgrupador("CANT_MAX_USUARIOS_PERFIL");
            List<string> list2 = new List<string>();
            if (list != null && list.Count == 1)
            {
                int hasta = Convert.ToInt32(list[0].valor);

                for (int i = 1; i <= hasta; i++)
                {
                    list2.Add(i.ToString());
                }
            }

            return list2;
        }

        private List<string> getCatalogo_ActivarVoz_BaseLocal()
        {
            ServiceOpcionesGenerales sog = new ServiceOpcionesGenerales();
            List<opciones_generales> list = sog.getEntitiesByAgrupador("OPCION");
            List<string> list2 = new List<string>();
            for (int i = 0; i < list.Count; i++)
            {
                list2.Add(list[i].opcion_general);
            }
            return list2;
        }

        private List<string> getCatalogo_ActivarUbicacion_BaseLocal()
        {
            ServiceOpcionesGenerales sog = new ServiceOpcionesGenerales();
            List<opciones_generales> list = sog.getEntitiesByAgrupador("OPCION");
            List<string> list2 = new List<string>();
            for (int i = 0; i < list.Count; i++)
            {
                list2.Add(list[i].opcion_general);
            }
            return list2;
        }

        private List<ct_denominaciones> getCatalogo_Denominaciones_BaseLocal()
        {
            ServiceDenominaciones serv = new ServiceDenominaciones();
            List<ct_denominaciones> list = serv.getEntities();
            //List<string> list2 = new List<string>();
            //for (int i = 0; i < list.Count; i++)
            //{
            //    list2.Add(list[i].nombre);
            //}
            return list;
        }

        private List<string> getCatalogo_ImagenesSubidas_BaseLocal()
        {
            ServiceImagenesSubidas su = new ServiceImagenesSubidas();
            List<ct_imagenes_subidas> list = su.getEntities();
            List<string> list2 = new List<string>();
            for (int i = 0; i < list.Count; i++)
            {
                list2.Add(list[i].nombre);
            }
            return list2;
        }

        private List<string> getCatalogo_ImagenesDenominaciones_BaseLocal()
        {
            ServiceImagenesSubidas su = new ServiceImagenesSubidas();
            List<ct_imagenes_subidas> list = su.getEntities();
            List<string> list2 = new List<string>();
            for (int i = 0; i < list.Count; i++)
            {
                list2.Add(list[i].nombre);
            }
            return list2;
        }

        private List<string> getCatalogo_ActivarCamara_BaseLocal()
        {
            ServiceOpcionesGenerales sog = new ServiceOpcionesGenerales();
            List<opciones_generales> list = sog.getEntitiesByAgrupador("OPCION");
            List<string> list2 = new List<string>();
            for (int i = 0; i < list.Count; i++)
            {
                list2.Add(list[i].opcion_general);
            }
            return list2;
        }

        private List<string> getCatalogo_ActivarImpresora_BaseLocal()
        {
            ServiceOpcionesGenerales sog = new ServiceOpcionesGenerales();
            List<opciones_generales> list = sog.getEntitiesByAgrupador("OPCION");
            List<string> list2 = new List<string>();
            for (int i = 0; i < list.Count; i++)
            {
                list2.Add(list[i].opcion_general);
            }
            return list2;
        }

        private List<ct_tarifas_montos_fijos> getCatalogo_TarifasMontoFijo_BaseLocal()
        {
            ServiceTarifasMontosFijos serv = new ServiceTarifasMontosFijos();
            List<ct_tarifas_montos_fijos> list = serv.getEntities();
            return list;
        }

        #endregion

        #region VALIDACIONES
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
        private bool isValidComPortAndConnected()
        {
            bool isValid;
            try
            {
                open_serial_port();
                isValid = true;
            }
            catch (Exception ex)
            {
                isValid = false;
            }
            return isValid;
        }
        private bool validaDispositivoConectadoEnPuertoCOM()
        {
            bool hayDispositivoConectado = false;

            try
            {
                if (puertoSerie1.IsOpen == false)
                {
                    //close_serial_port();
                    //open_serial_port();
                    //puertoSerie1.Dispose();
                    
                    //puertoSerie1.Open();

                    hayDispositivoConectado = true;
                }
                else
                {
                    hayDispositivoConectado = true;
                }
            }
            catch (Exception ex)
            {
                hayDispositivoConectado = true;
                MessageBox.Show(ex.Message);
            }

            return hayDispositivoConectado;
        }

        #endregion

        #region BOTONES DE GUARDAR DE CADA PANTALLA EN CONFIGURACION

        private void btnGuardarConfiguracion_Click(object sender, RoutedEventArgs e)
        {
            try
            {


                txtMensajePopup.Text = "CONFIGURACION EXITOSA";
                mostrarPopupOk();

                //close_serial_port();
            }
            catch (Exception ex)
            {
                //close_serial_port();
                MessageBox.Show(ex.Message, "ATENCION", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
        private void btnGuardarConf_Puertos_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //CONFIGURACION DE PUERTO SERIE
                string port_name = cmbComPorts.Text;
                string baud_rate = cmbBausRate.Text;
                string data_bits = cmbDataBits.Text;
                string stop_bits = cmbStopBits.Text;
                string paridad = cmbParity.Text;
                string handshake = cmbHandShake.Text;

                ServiceConfigVarios scv = new ServiceConfigVarios();
                config_varios cv = new config_varios();

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



                txtMensajePopup.Text = "CONFIGURACION EXITOSA";
                mostrarPopupOk();

                //close_serial_port();
            }
            catch (Exception ex)
            {
                //close_serial_port();
                MessageBox.Show(ex.Message, "ATENCION", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            
        }
        private void btnGuardarConfBoletos_Click(object sender, RoutedEventArgs e)
        {
            // BOLETO
            string prefijo_folio_boleto = txtPrefijoBoleto.Text;
            string prefijo_folio_corte = txtPrefijoCorte.Text;

            #region Setear Mensajes en Boleto
            if (txtEncabezadoLinea1.Text.Length == 32 && txtEncabezadoLinea2.Text.Length == 32 && txtEncabezadoLinea3.Text.Length == 32 &&
                txtPiePaginaLinea1.Text.Length == 32 && txtPiePaginaLinea2.Text.Length == 32 && txtPiePaginaLinea3.Text.Length == 32)
            {
                ejecutar_commando_82_set_mensaje(0, txtEncabezadoLinea1.Text);
                ejecutar_commando_82_set_mensaje(1, txtEncabezadoLinea2.Text);
                ejecutar_commando_82_set_mensaje(2, txtEncabezadoLinea3.Text);

                ejecutar_commando_82_set_mensaje(3, txtPiePaginaLinea1.Text);
                ejecutar_commando_82_set_mensaje(4, txtPiePaginaLinea2.Text);
                ejecutar_commando_82_set_mensaje(5, txtPiePaginaLinea3.Text);
            }
            else
            {
                MessageBox.Show("Favor de llenar los mensajes del boleto en sus 32 caracteres, asi sea con espacios");
            }

            #endregion

            #region BOLETO

            ServiceConfigVarios serv_config_varios_boleto = new ServiceConfigVarios();

            config_varios conf_varios_boleto = new config_varios();

            conf_varios_boleto.clave = "PREFIJO_FOLIO_BOLETO";
            conf_varios_boleto.valor = prefijo_folio_boleto;
            serv_config_varios_boleto.updEntityByClave(conf_varios_boleto);

            conf_varios_boleto.clave = "PREFIJO_FOLIO_CORTE";
            conf_varios_boleto.valor = prefijo_folio_corte;
            serv_config_varios_boleto.updEntityByClave(conf_varios_boleto);

            #endregion

        }
        private void btnGuardarConf_Tarifa_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string tipo_tarifa = cmbTipoTarifa.Text;
                string cant_usuarios_perfil = cmbCantUsuariosPerfil.Text;
                string texto_display_alcancia = txtTextoDisplayAlcancia.Text;

                ServiceConfigVarios scv = new ServiceConfigVarios();
                config_varios cv = new config_varios();

                cv.clave = "TIPO_TARIFA";
                cv.valor = tipo_tarifa;
                scv.updEntityByClave(cv);

                ServiceConfigVarios serv_conf_varios_tarifa = new ServiceConfigVarios();

                config_varios conf_varios_tarifa = new config_varios();
                conf_varios_tarifa.clave = "CANTIDAD_PERSONAS_PERFIL";
                conf_varios_tarifa.valor = cant_usuarios_perfil;
                serv_conf_varios_tarifa.updEntityByClave(conf_varios_tarifa);

                config_varios conf_varios_tarifa_texto_display = new config_varios();
                conf_varios_tarifa_texto_display.clave = "TEXTO_DISPLAY_ALCANCIA";
                conf_varios_tarifa_texto_display.valor = texto_display_alcancia;
                serv_conf_varios_tarifa.updEntityByClave(conf_varios_tarifa_texto_display);

                // SE SETEA EN LA ALCANCIA EL MENSAJE A MOSTRAR EN DISPLAY
                ejecutar_commando_84_set_mensaje_pantalla_alcancia(conf_varios_tarifa_texto_display.valor);

                txtMensajePopup.Text = "CONFIGURACION EXITOSA";
                mostrarPopupOk();

                //close_serial_port();
            }
            catch (Exception ex)
            {
                //close_serial_port();
                MessageBox.Show(ex.Message, "ATENCION", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
        private void btnGuardarConf_Empresa_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string empresa = cmbEmpresa.Text;
                string corredor = cmbCorredor.Text;
                string ruta = cmbRuta.Text;
                string numero_unidad = cmbUnidad.Text;

                #region Cambiar status en DB TISA para la UNIDAD SELECCIONADA

                string fecha_actual = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                ServiceConfigVarios scv = new ServiceConfigVarios();

                //valor de unidad antes de ser cambiada
                config_varios cv_num_unidad_antes_cambio = scv.getEntityByClave("NUMERO_UNIDAD");
                string unidad_antes_cambio = cv_num_unidad_antes_cambio.valor;

                config_varios cv = new config_varios();


                /*
                 1 = DISPONIBLE PARA ASIGNAR A ALGUNA UNIDAD
                 2 = ASIGNADA YA A ALGUNA UNIDAD
                */

                // Para Cambiar de status a 1 la unidad que sera liberada en el catalogo de TISA.
                ServiceUnidades serv_unidades_cambio_status_1 = new ServiceUnidades();
                ct_unidades unidad_1 = serv_unidades_cambio_status_1.getEntityByName(unidad_antes_cambio);
                if (unidad_1 != null)
                {
                    ct_unidades cu_1 = new ct_unidades();
                    cu_1.pkUnidad = unidad_1.pkUnidad;
                    cu_1.fkEmpresa = unidad_1.fkEmpresa;
                    cu_1.fkCorredor = unidad_1.fkCorredor;
                    cu_1.numeracion = unidad_1.numeracion;
                    cu_1.nombre = unidad_1.nombre;
                    cu_1.noSerieAVL = unidad_1.noSerieAVL;
                    cu_1.capacidad = unidad_1.capacidad;
                    cu_1.validador = unidad_1.validador;
                    cu_1.status = 1;
                    cu_1.created_at = unidad_1.created_at;
                    cu_1.updated_at = fecha_actual;
                    cu_1.deleted_at = unidad_1.deleted_at;

                    UnidadesController uc_1 = new UnidadesController();
                    uc_1.UpdateUnidad(cu_1);
                }

                //Para Cambiar de status a 2 la unidad que sera asignada a la consola.
                ServiceUnidades serv_unidades_cambio_status_2 = new ServiceUnidades();
                ct_unidades unidad_2 = serv_unidades_cambio_status_2.getEntityByName(cv.valor);

                if (unidad_2 != null)
                {
                    ct_unidades cu_2 = new ct_unidades();
                    cu_2.pkUnidad = unidad_2.pkUnidad;
                    cu_2.fkEmpresa = unidad_2.fkEmpresa;
                    cu_2.fkCorredor = unidad_2.fkCorredor;
                    cu_2.numeracion = unidad_2.numeracion;
                    cu_2.nombre = unidad_2.nombre;
                    cu_2.noSerieAVL = unidad_2.noSerieAVL;
                    cu_2.capacidad = unidad_2.capacidad;
                    cu_2.validador = unidad_2.validador;
                    cu_2.status = 2;
                    cu_2.created_at = unidad_2.created_at;
                    cu_2.updated_at = fecha_actual;
                    cu_2.deleted_at = unidad_2.deleted_at;

                    UnidadesController uc_2 = new UnidadesController();
                    uc_2.UpdateUnidad(cu_2);
                }


                #endregion

                ServiceConfigVarios serv_cv = new ServiceConfigVarios();
                config_varios cv2 = new config_varios();

                cv2.clave = "EMPRESA";
                cv2.valor = empresa;
                serv_cv.updEntityByClave(cv2);

                cv2.clave = "CORREDOR";
                cv2.valor = corredor;
                serv_cv.updEntityByClave(cv2);

                cv2.clave = "RUTA";
                cv2.valor = ruta;
                serv_cv.updEntityByClave(cv2);

                ejecutar_commando_86_ruta(ruta);

                cv2.clave = "NUMERO_UNIDAD";
                cv2.valor = numero_unidad;
                serv_cv.updEntityByClave(cv2);

                ejecutar_commando_86_unidad(numero_unidad);



                txtMensajePopup.Text = "CONFIGURACION EXITOSA";
                mostrarPopupOk();

                //close_serial_port();
            }
            catch (Exception ex)
            {
                //close_serial_port();
                MessageBox.Show(ex.Message, "ATENCION", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            
        }
        private void btnGuardarConf_Multimedia_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int indice;
                indice = cmbVoces.SelectedIndex;
                string nombre = vocesInfo.ElementAt(indice).Name;
                string voz = nombre;

                string camara_default = cmbCamaras.Text;
                string impresora_default = cmbImpresoras.Text;

                string activar_voz = cmbActivarVoz.Text;
                string activar_camara = cmbActivarCamara.Text;
                string activar_impresora = cmbActivarImpresora.Text;

                ServiceConfigVarios serv_conf_varios_multimedia = new ServiceConfigVarios();
                config_varios conf_varios_multimedia = new config_varios();

                conf_varios_multimedia.clave = "VOZ";
                conf_varios_multimedia.valor = voz;
                serv_conf_varios_multimedia.updEntityByClave(conf_varios_multimedia);

                // VOZ ACTIVA
                conf_varios_multimedia.clave = "VOZ_ACTIVA";
                conf_varios_multimedia.valor = activar_voz;
                serv_conf_varios_multimedia.updEntityByClave(conf_varios_multimedia);

                // CAMARA ACTIVA
                conf_varios_multimedia.clave = "CAMARA_ACTIVA";
                conf_varios_multimedia.valor = activar_camara;
                serv_conf_varios_multimedia.updEntityByClave(conf_varios_multimedia);

                // IMPRESORA ACTIVA
                conf_varios_multimedia.clave = "IMPRESORA_ACTIVA";
                conf_varios_multimedia.valor = activar_impresora;
                serv_conf_varios_multimedia.updEntityByClave(conf_varios_multimedia);

                // CAMARA DEFAULT
                conf_varios_multimedia.clave = "CAMARA_DEFAULT";
                conf_varios_multimedia.valor = camara_default;
                serv_conf_varios_multimedia.updEntityByClave(conf_varios_multimedia);

                // IMPRESORA DEFAULT
                conf_varios_multimedia.clave = "IMPRESORA_DEFAULT";
                conf_varios_multimedia.valor = impresora_default;
                serv_conf_varios_multimedia.updEntityByClave(conf_varios_multimedia);


                txtMensajePopup.Text = "CONFIGURACION EXITOSA";
                mostrarPopupOk();

                //close_serial_port();
            }
            catch (Exception ex)
            {
                //close_serial_port();
                MessageBox.Show(ex.Message, "ATENCION", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
        private void btnGuardarConfiguracionDenominaciones_Click(object sender, RoutedEventArgs e)
        {
            #region DENOMINACIONES MONEDAS Y BILLETES

            try
            {
                ServiceDenominaciones serv_denominaciones = new ServiceDenominaciones();
                ct_denominaciones obj_denom = new ct_denominaciones();

                //Ciclo para actualizar cada posicion con su denominacion
                List<ct_denominaciones> list_denominaciones = serv_denominaciones.getEntities();
                for (int i = 0; i < list_denominaciones.Count; i++)
                {
                    decimal posicion_actual = list_denominaciones[i].posicion != null ? Convert.ToDecimal(list_denominaciones[i].posicion) : -1;
                    UInt32 valor_denominacion_centavos = (UInt32)(Convert.ToDecimal(list_denominaciones[i].valor) * 100);
                    if (posicion_actual != -1)
                    {
                        ejecutar_commando_80_set_denominacion(posicion_actual, valor_denominacion_centavos);
                    }
                }

                txtMensajePopup.Text = "CONFIGURACION EXITOSA";
                mostrarPopupOk();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }



            #endregion
        }
        private void btnGuardarConfiguracionUbicacion_Click(object sender, RoutedEventArgs e)
        {
            ServiceConfigVarios serv_config_varios = new ServiceConfigVarios();
            config_varios cv_ubicacion = new config_varios();

            // UBICACION
            string activar_ubicacion = cmbActivarUbicacion.Text;

            // UBICACION ACTIVA
            cv_ubicacion.clave = "UBICACION_ACTIVA";
            cv_ubicacion.valor = activar_ubicacion;
            serv_config_varios.updEntityByClave(cv_ubicacion);

            // MUESTRA POPUP DE CONFIRMACION DE QUE SE GUARDO EXITOSAMENTE.
            txtMensajePopup.Text = "CONFIGURACION EXITOSA";
            mostrarPopupOk();
        }
        private void btnGuardarConf_Mensajes_Click(object sender, RoutedEventArgs e)
        {

        }
        private void btnGuardarConf_Apariencia_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string logo_home = cmbImagenesSubidas.Text;

                ServiceConfigVarios serv_config_varios_apariencia = new ServiceConfigVarios();
                config_varios cv_apariencia_logo_home = new config_varios();
                cv_apariencia_logo_home.clave = "LOGO_HOME";
                cv_apariencia_logo_home.valor = logo_home;
                serv_config_varios_apariencia.updEntityByClave(cv_apariencia_logo_home);

                txtMensajePopup.Text = "CONFIGURACION EXITOSA";
                mostrarPopupOk();

                //close_serial_port();
            }
            catch (Exception ex)
            {
                //close_serial_port();
                MessageBox.Show(ex.Message, "ATENCION", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
        private void btnGuardarConf_Mantenimiento_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string base_url = txtUrlBase.Text;
                string modo = cmbModoApp.Text;

                ServiceConfigVarios serv_conf_varios_mantenimiento = new ServiceConfigVarios();
                config_varios conf_varios_mantenimiento = new config_varios();

                conf_varios_mantenimiento.clave = "BASE_URL";
                conf_varios_mantenimiento.valor = base_url;
                serv_conf_varios_mantenimiento.updEntityByClave(conf_varios_mantenimiento);

                conf_varios_mantenimiento.clave = "MODO";
                conf_varios_mantenimiento.valor = modo;
                serv_conf_varios_mantenimiento.updEntityByClave(conf_varios_mantenimiento);

                txtMensajePopup.Text = "CONFIGURACION EXITOSA";
                mostrarPopupOk();

                //close_serial_port();
            }
            catch (Exception ex)
            {
                //close_serial_port();
                MessageBox.Show(ex.Message, "ATENCION", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        #endregion

        #region EVENTOS CONTROLES

        private void btnAgregar_TarifaFija_Click(object sender, RoutedEventArgs e)
        {
            string valor_tarifa = txtValor_TarifaMontoFijo.Text.ToString().Trim();
            string texto_tarifa = txtTexto_TarifaMontoFijo.Text.ToString().Trim();
            string descripcion_tarifa = txtDescripcion_TarifaMontoFijo.Text.ToString().Trim();
            string orden_tarifa = cmbOrden_TarifaMontoFijo.Text.ToString().Trim();

            string fecha_actual = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            if (valor_tarifa != "" && texto_tarifa != "" && descripcion_tarifa != "" && orden_tarifa != "")
            {
                ServiceTarifasMontosFijos serv_tarifas_montos_fijos = new ServiceTarifasMontosFijos();
                ct_tarifas_montos_fijos obj = new ct_tarifas_montos_fijos();

                obj.valor = valor_tarifa;
                obj.texto = texto_tarifa;
                obj.descripcion = descripcion_tarifa;
                obj.orden = Convert.ToInt32(orden_tarifa);


                //se insertaran solo 6 tarifas para montos fijos
                ct_tarifas_montos_fijos tarifa_fija_orden = serv_tarifas_montos_fijos.getEntityByOrden(obj.orden);
                if (tarifa_fija_orden != null && tarifa_fija_orden.orden == obj.orden)
                {
                    obj.pkTarifaMontoFijo = tarifa_fija_orden.pkTarifaMontoFijo;
                    obj.status = 1;
                    obj.created_at = tarifa_fija_orden.created_at;
                    obj.updated_at = fecha_actual;

                    serv_tarifas_montos_fijos.updEntity(obj);
                }
                else
                {
                    obj.pkTarifaMontoFijo = tarifa_fija_orden.pkTarifaMontoFijo;
                    obj.status = 1;
                    obj.created_at = tarifa_fija_orden.created_at;
                    obj.updated_at = fecha_actual;

                    serv_tarifas_montos_fijos.addEntity(obj);
                }

                llenarGridTarifasMontoFijo();

                txtValor_TarifaMontoFijo.Text = "";
                txtTexto_TarifaMontoFijo.Text = "";
                txtDescripcion_TarifaMontoFijo.Text = "";
                cmbOrden_TarifaMontoFijo.Text = "";
            }
        }
        private void btnEliminar_TarifaFija_Click(object sender, RoutedEventArgs e)
        {
            ct_tarifas_montos_fijos row = (ct_tarifas_montos_fijos)dataGridTarifasMontosFijos.SelectedItems[0];
            //int orden = Convert.ToInt32(row.orden.ToString().Trim());

            ServiceTarifasMontosFijos serv_tarifas_montos_fijos = new ServiceTarifasMontosFijos();
            serv_tarifas_montos_fijos.delEntityByOrden(row);

            llenarGridTarifasMontoFijo();
        } 
        private void btnAgregar_Denominacion_Click(object sender, RoutedEventArgs e)
        {
            //Obtener objeto de imagenes subidas segun la seleccion de la imagen
            ServiceImagenesSubidas serv_img = new ServiceImagenesSubidas();
            ct_imagenes_subidas img_sub = serv_img.getEntityByName(cmbImagenesDenominaciones.Text.ToString().Trim());
            string filename_with_extension = System.IO.Path.GetFileName(img_sub.path_imagen);

            //extracion de valor de los controles y asignaciona a variables.
            string nombre_denominacion = txtNombreDenominacion.Text.ToString().Trim();
            string valor_denominacion = txtValorDenominacion.Text.ToString().Trim();
            string path_imagen_denominacion = @"C:\mt_con_database\IMAGENES_SUBIDAS\" + filename_with_extension;
            string posicion_tarifa = cmbPosicionArreglo.Text.ToString().Trim();

            string fecha_actual = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            if (nombre_denominacion != "" && valor_denominacion != "" && path_imagen_denominacion != "" && posicion_tarifa != "")
            {
                ServiceDenominaciones serv_denominaciones = new ServiceDenominaciones();
                ct_denominaciones obj = new ct_denominaciones();

                obj.nombre = nombre_denominacion;
                obj.valor = valor_denominacion;
                obj.path_imagen = path_imagen_denominacion;
                obj.posicion = Convert.ToInt32(posicion_tarifa);


                //se insertaran solo 6 tarifas para montos fijos
                ct_denominaciones denominacion = serv_denominaciones.getEntityByOrden(obj.posicion);
                if (denominacion != null && denominacion.posicion == obj.posicion)
                {
                    obj.pkDenominacion = denominacion.pkDenominacion;
                    obj.status = 1;
                    obj.created_at = denominacion.created_at;
                    obj.updated_at = fecha_actual;

                    serv_denominaciones.updEntity(obj);
                }
                else
                {
                    obj.pkDenominacion = denominacion.pkDenominacion;
                    obj.status = 1;
                    obj.created_at = denominacion.created_at;
                    obj.updated_at = fecha_actual;

                    serv_denominaciones.addEntity(obj);
                }

                llenarGridDenominaciones();

                txtNombreDenominacion.Text = "";
                txtValorDenominacion.Text = "";
                cmbImagenesDenominaciones.Text = "";
                cmbPosicionArreglo.Text = "";
            }
        }
        private void btnEliminar_Denominacion_Click(object sender, RoutedEventArgs e)
        {
            ct_denominaciones row = (ct_denominaciones)dataGridDenominaciones.SelectedItems[0];
            //int orden = Convert.ToInt32(row.orden.ToString().Trim());

            ServiceDenominaciones serv_denominaciones = new ServiceDenominaciones();
            serv_denominaciones.delEntityByOrden(row);

            llenarGridDenominaciones();
        }


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

        byte[] data; // Global
        BitmapImage bi = null; // Global
        private void btnExaminarImagen_Click(object sender, RoutedEventArgs e)
        {
            //Muestra el open dialog para seleccionar la imagen
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Title = "Examinar Imagen a Subir";
            dlg.Multiselect = false;
            dlg.Filter = "Image Files(*.jpg; *.jpeg; *.png; *.gif; *.bmp)|*.jpg; *.jpeg; *.png; *.gif; *.bmp";
            dlg.ShowDialog();

            if (dlg.FileName != "")
            {
                // Guarda la imagen en un arreglo de bytes para poder enviarla a la base de datos local
                FileStream fs = new FileStream(dlg.FileName, FileMode.Open, FileAccess.Read);
                //byte[] data = new byte[fs.Length];
                data = new byte[fs.Length];
                fs.Read(data, 0, System.Convert.ToInt32(fs.Length));
                fs.Close();

                // valida si hay algo en la imagen para insertarla en la base de datos local
                if (data != null)
                {
                    ImageSourceConverter imgs = new ImageSourceConverter();
                    imagebox.SetValue(Image.SourceProperty, imgs.
                    ConvertFromString(dlg.FileName.ToString()));
                }

                // crear un objeto de Bitmap para posteriormente al guardar se tome este objeto para guardar fisicamente en el disco la imagen
                bi = new BitmapImage();
                bi.BeginInit();
                bi.UriSource = new Uri(dlg.FileName, UriKind.RelativeOrAbsolute);
                bi.EndInit();
            }
        }
        private void btnAgregarImagen_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string path_imagenes_subidas = @"C:\mt_con_database\IMAGENES_SUBIDAS\";

                string strPath = imagebox.Source.ToString();
                string filename = System.IO.Path.GetFileName(strPath);

                ServiceImagenesSubidas serv_imagenes_subidas = new ServiceImagenesSubidas();
                ct_imagenes_subidas img_sub = new ct_imagenes_subidas();
                img_sub.nombre = txtNombreImagen.Text;
                img_sub.imagen = data;
                img_sub.path_imagen = path_imagenes_subidas + filename;
                img_sub.status = 1;

                if (img_sub.nombre != "" && img_sub.imagen != null)
                {
                    serv_imagenes_subidas.addEntity(img_sub);
                }

                //Para guardar la imagen en el directorio fisico
                //SaveFileDialog save = new SaveFileDialog();
                //save.Title = "Save picture as ";
                //save.Filter = "Image Files(*.jpg; *.jpeg; *.gif; *.bmp)|*.jpg; *.jpeg; *.gif; *.bmp";
                if (bi != null)
                {
                    //if (save.ShowDialog() == true)
                    //{
                    JpegBitmapEncoder jpg = new JpegBitmapEncoder();
                    jpg.Frames.Add(BitmapFrame.Create(bi));

                    string file_a_crear = path_imagenes_subidas + filename;


                    using (Stream stm = File.Create(file_a_crear))
                    {
                        jpg.Save(stm);
                    }

                }

                //Mostrar el popup para indicar que se guardo la imagen con exito.
                txtMensajePopup.Text = "Imagen Guardada";
                mostrarPopupOk();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


        }
        private void CopyAFile()
        {
            var source = new OpenFileDialog();
            if (source.ShowDialog().GetValueOrDefault())
            {
                var dest = new SaveFileDialog();
                if (dest.ShowDialog().GetValueOrDefault())
                {
                    File.Copy(source.FileName, dest.FileName);
                }
            }
        }

        private void chkSinTISA_TarifasMontosFijos_Checked(object sender, RoutedEventArgs e)
        {
            ServiceConfigVarios serv_config_varios = new ServiceConfigVarios();
            config_varios cv_sinc_tarifas_montos_fijos = new config_varios();

            if (chkSinTISA_TarifasMontosFijos.IsChecked == true)
            {
                cv_sinc_tarifas_montos_fijos.clave = "SINC_TISA_TARIFAS_MONTOS_FIJOS";
                cv_sinc_tarifas_montos_fijos.valor = "SI";
            }
            else
            {
                cv_sinc_tarifas_montos_fijos.clave = "SINC_TISA_TARIFAS_MONTOS_FIJOS";
                cv_sinc_tarifas_montos_fijos.valor = "NO";
            }

            serv_config_varios.updEntityByClave(cv_sinc_tarifas_montos_fijos);
        }
        private void chkSinTISA_Denominaciones_Checked(object sender, RoutedEventArgs e)
        {
            ServiceConfigVarios serv_config_varios = new ServiceConfigVarios();
            config_varios cv_sinc_denominaciones = new config_varios();

            if (chkSinTISA_Denominaciones.IsChecked == true)
            {
                cv_sinc_denominaciones.clave = "SINC_TISA_DENOMINACIONES";
                cv_sinc_denominaciones.valor = "SI";
            }
            else
            {
                cv_sinc_denominaciones.clave = "SINC_TISA_DENOMINACIONES";
                cv_sinc_denominaciones.valor = "NO";
            }

            serv_config_varios.updEntityByClave(cv_sinc_denominaciones);
        }
        private void chkSinTISA_OpcionesGenerales_Checked(object sender, RoutedEventArgs e)
        {
            ServiceConfigVarios serv_config_varios = new ServiceConfigVarios();
            config_varios cv_sinc_opciones_generales = new config_varios();

            if (chkSinTISA_OpcionesGenerales.IsChecked == true)
            {
                cv_sinc_opciones_generales.clave = "SINC_TISA_OPCIONES_GENERALES";
                cv_sinc_opciones_generales.valor = "SI";
            }
            else
            {
                cv_sinc_opciones_generales.clave = "SINC_TISA_OPCIONES_GENERALES";
                cv_sinc_opciones_generales.valor = "NO";
            }

            serv_config_varios.updEntityByClave(cv_sinc_opciones_generales);
        }
        private void chkSinTISA_ConfigVarios_Checked(object sender, RoutedEventArgs e)
        {
            ServiceConfigVarios serv_config_varios = new ServiceConfigVarios();
            config_varios cv_sinc_config_varios = new config_varios();

            if (chkSinTISA_ConfigVarios.IsChecked == true)
            {
                cv_sinc_config_varios.clave = "SINC_TISA_CONFIG_VARIOS";
                cv_sinc_config_varios.valor = "SI";
            }
            else
            {
                cv_sinc_config_varios.clave = "SINC_TISA_CONFIG_VARIOS";
                cv_sinc_config_varios.valor = "NO";
            }

            serv_config_varios.updEntityByClave(cv_sinc_config_varios);
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
        void ClearBufferRecievedDataGlobal()
        {
            for (int i = 0; i < RecievedDataGlobal.Length; i++)
            {
                RecievedDataGlobal[i] = 0;
            }
        }

        private void ejecutar_commando_86_ruta(string nombre_ruta)
        {
            //close_serial_port();

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

            //close_serial_port();
            //dispose_serial_port();

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
            //close_serial_port();

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

            //close_serial_port();
            //dispose_serial_port();

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
            for (int i = 0; i < mensaje.Length; i++)
            {
                if ((byte)mensaje[i] != 0)
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

            //close_serial_port();
            //dispose_serial_port();

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
        private void ejecutar_commando_80_set_denominacion(decimal num_posicion_denom, UInt32 valor_moneda_billete)
        {
            const decimal ByteInicio = 1;
            const decimal AddressConsola = 1;
            const decimal AddressAlcancia = 2;
            const decimal Comando = 128;
            decimal numero_posicion_denominacion = num_posicion_denom;

            const Int32 K_offsetDatos = 4;
            const Int32 K_CRCs = 2;

            const decimal CRC1 = 193;
            const decimal CRC2 = 194;

            int CantidadDatos = 0;
            CantidadDatos = 6;

            ClearBufferSendData();
            ClearBufferRecievedDataGlobal();

            BufferSendData[0] = decimal.ToByte(ByteInicio);
            BufferSendData[1] = decimal.ToByte(AddressAlcancia);
            BufferSendData[2] = decimal.ToByte(AddressConsola);
            BufferSendData[3] = decimal.ToByte(CantidadDatos);
            BufferSendData[4] = decimal.ToByte(Comando);

            byte Code_xVar = (byte)numero_posicion_denominacion;
            BufferSendData[5] = Code_xVar;

            BufferSendData[5] = (byte)numero_posicion_denominacion;

            int IndiceBuffer = 6;
            UInt32 Var32 = valor_moneda_billete;
            BufferSendData[IndiceBuffer] = (byte)Var32;
            BufferSendData[IndiceBuffer + 1] = (byte)(Var32 >> 8);
            BufferSendData[IndiceBuffer + 2] = (byte)(Var32 >> 16);
            BufferSendData[IndiceBuffer + 3] = (byte)(Var32 >> 24);

            BufferSendData[K_offsetDatos + CantidadDatos] = decimal.ToByte(CRC1);
            BufferSendData[K_offsetDatos + CantidadDatos + 1] = decimal.ToByte(CRC2);

            //close_serial_port();
            //dispose_serial_port();

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
                MessageBox.Show("Verifique la estructura del comando enviado", "Error en comando 80");
            }
        }
        private void ejecutar_commando_84_set_mensaje_pantalla_alcancia(string mensaje)
        {
            const decimal ByteInicio = 1;
            const decimal AddressConsola = 1;
            const decimal AddressAlcancia = 2;
            const decimal Comando = 132;


            const Int32 K_offsetDatos = 4;
            const Int32 K_CRCs = 2;

            const decimal CRC1 = 193;
            const decimal CRC2 = 194;

            int CantidadDatos = 0;
            CantidadDatos = 32 + 1;

            ClearBufferSendData();
            ClearBufferRecievedDataGlobal();

            BufferSendData[0] = decimal.ToByte(ByteInicio);
            BufferSendData[1] = decimal.ToByte(AddressAlcancia);
            BufferSendData[2] = decimal.ToByte(AddressConsola);
            BufferSendData[3] = decimal.ToByte(CantidadDatos);
            BufferSendData[4] = decimal.ToByte(Comando);

            int n = 5;
            for (int i = 0; i < mensaje.Length; i++)
            {
                if ((byte)mensaje[i] != 0)
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

            //close_serial_port();
            //dispose_serial_port();

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
                MessageBox.Show("Verifique la estructura del comando enviado", "Error en comando 84");
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

                //close_serial_port();
                //dispose_serial_port();

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
        private string ejecutar_commando_85_get_texto_display_alcancia()
        {
            string mensaje = "";
            try
            {
                const decimal ByteInicio = 1;
                const decimal AddressConsola = 1;
                const decimal AddressAlcancia = 2;
                const decimal Comando = 133;

                const Int32 K_offsetDatos = 4;
                const Int32 K_CRCs = 2;

                const decimal CRC1 = 193;
                const decimal CRC2 = 194;

                int CantidadDatos = 0;
                CantidadDatos = 1;

                ClearBufferSendData();
                ClearBufferRecievedDataGlobal();

                BufferSendData[0] = decimal.ToByte(ByteInicio);
                BufferSendData[1] = decimal.ToByte(AddressAlcancia);
                BufferSendData[2] = decimal.ToByte(AddressConsola);
                BufferSendData[3] = decimal.ToByte(CantidadDatos);
                BufferSendData[4] = decimal.ToByte(Comando);


                BufferSendData[K_offsetDatos + CantidadDatos] = decimal.ToByte(CRC1);
                BufferSendData[K_offsetDatos + CantidadDatos + 1] = decimal.ToByte(CRC2);

                //close_serial_port();
                //dispose_serial_port();

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

            //close_serial_port();
            //dispose_serial_port();

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
        private string GetMensajeString(byte[] RecievedDataGlobal_local)
        {
            string empty = " ";
            string cadena = "";
            int n = 5;
            for (int i = 0; i < ((RecievedDataGlobal_local[3]) - 1); i++)
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
