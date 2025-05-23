﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Threading;
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
using TestMdfEntityFramework.Controllers;
using TestMdfEntityFramework.EntityServices;
using TestMdfEntityFramework.Responses;



namespace TestMdfEntityFramework.Views
{
    //Obtener la ubicacion y enviarla

    public partial class Mensajes : UserControl
    {
        string ASIGNACION_ACTIVA = "";
        long FK_ASIGNACION_ACTIVA = 0;
        string MODO_APP = "";
        string VOZ = "";

        //SERIAL PORT
        System.IO.Ports.SerialPort puertoSerie1 = new System.IO.Ports.SerialPort();
        String[] listado_puerto = System.IO.Ports.SerialPort.GetPortNames();

        //PARA REPRODUCIR TEXTO A VOZ
        SpeechSynthesizer synthesizer = new SpeechSynthesizer();
        List<VoiceInfo> vocesInfo = new List<VoiceInfo>();

        //POPUP OK
        private double left, top, right, bottom, centerX, centerY;
        private DoubleAnimation bottomToCenterAnimiation, topToCenterAnimation,
            leftToCenterAnimation, rightToCenterAnimation;
        private Storyboard bottomToCenterStoryboard, topToCenterStoryboard,
            leftToCenterStoryboard, rightToCenterStoryboard;

        //StackPanel pnl_teclado = new StackPanel();

        string[] alfabeto = { "1", "2", "3", "4", "5", "6"
                            , "7", "8", "9", "0"
                            , "Q", "W", "E", "R", "T", "Y"
                            , "U", "I", "O", "P", "A", "S"
                            , "D", "F", "G", "H", "J", "K", "L", "Del", "Z"
                            , "X", "C", "V", "N", "N", "Enter", "Espacio" };

        List<Border> removeKeyboardKey = new List<Border>();
        List<Border> chosenKeyboardKey = new List<Border>();

        public Mensajes()
        {
            InitializeComponent();

            //setting_up_teclado_pantalla();
        }

        private void setting_up_teclado_pantalla()
        {
            Grid.SetRow(panel_teclado, 6);
            Grid.SetRowSpan(panel_teclado, 3);
            Grid.SetColumn(panel_teclado, 1);
            Grid.SetColumnSpan(panel_teclado, 7);
        }

        
        private void hablar_texto(string texto)
        {
            double Volumen = 100;
            double Rate = .2;
            synthesizer.SelectVoice(VOZ);
            synthesizer.Volume = (int)Volumen;
            synthesizer.Rate = (int)Rate;
            synthesizer.Speak(texto);
        }

        private void runVirtualKeyboard()
        {
            for (int i = 0; i < alfabeto.Length; i++)
            {
                Border keyboardKey = new Border();
                keyboardKey.Width = 40;
                keyboardKey.Height = 40;
                keyboardKey.Margin = new Thickness(2, 3, 2, 3);
                keyboardKey.Background = Brushes.Black;
                keyboardKey.CornerRadius = new CornerRadius(5);
                keyboardKey.Tag = alfabeto[i];
                keyboardKey.Uid = i.ToString();
                keyboardKey.MouseLeftButtonDown += keyboardPressed;

                TextBlock teks = new TextBlock();
                teks.TextAlignment = TextAlignment.Center;
                teks.VerticalAlignment = VerticalAlignment.Center;
                teks.FontSize = 25;
                teks.Foreground = Brushes.White;
                teks.Text = alfabeto[i];

                keyboardKey.Child = teks;
                keyboardRow1.Children.Add(keyboardKey);
                removeKeyboardKey.Add(keyboardKey);
                chosenKeyboardKey.Add(keyboardKey);

                if(removeKeyboardKey.Count > 10)
                {
                    keyboardRow1.Children.Remove(keyboardKey);
                    keyboardRow2.Children.Add(keyboardKey);

                    if(removeKeyboardKey.Count > 20)
                    {
                        keyboardRow1.Children.Remove(keyboardKey);
                        keyboardRow2.Children.Remove(keyboardKey);
                        keyboardRow3.Children.Add(keyboardKey);

                        if(removeKeyboardKey.Count > 30)
                        {
                            keyboardKey.Background = Brushes.Blue;
                        }
                        if(removeKeyboardKey.Count > 30)
                        {
                            keyboardRow1.Children.Remove(keyboardKey);
                            keyboardRow2.Children.Remove(keyboardKey);
                            keyboardRow3.Children.Remove(keyboardKey);
                            keyboardRow4.Children.Add(keyboardKey);

                            if(removeKeyboardKey.Count > 38)
                            {
                                keyboardKey.Background = Brushes.Blue;
                                keyboardKey.Width = 130;

                            }
                        }
                    }
                }
            }
        }

        private async void keyboardPressed(object sender, MouseButtonEventArgs e)
        {
            Border btn = sender as Border;
            string i = (string)btn.Tag;
            string index = (string)btn.Uid;

            if (i.Equals("Del"))
            {
                try
                {
                    i = null;
                    string data = txtMensajes.Text.Substring(txtMensajes.Text.Length - 1);
                    txtMensajes.Text = txtMensajes.Text.Replace(data, null);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            if (i.Equals("Espacio"))
            {
                i = null;
                string data = txtMensajes.Text + " ";
                txtMensajes.Text = txtMensajes.Text.Replace(txtMensajes.Text, data);
            }

            txtMensajes.Text += i;

            chosenKeyboardKey[int.Parse(index)].Background = Brushes.White;
            await delay(100);
            chosenKeyboardKey[int.Parse(index)].Background = Brushes.Black;
            chosenKeyboardKey[29].Background = Brushes.Blue;
            chosenKeyboardKey[37].Background = Brushes.Blue;
        }
        async Task delay(int num)
        {
            await Task.Delay(num);
        }


        #region EVENTOS CONTROLES
        private void Mensajes_Load(object sender, RoutedEventArgs e)
        {
            if (validaPuertoCOMConfigurado())
            {
                configura_puerto_serial();
            }

            SetPopupDlgCenter();
            InitializeAnimations();


            ServiceConfigVarios serv_config_varios = new ServiceConfigVarios();
            config_varios cv_asign = serv_config_varios.getEntityByClave("ASIGNACION_ACTIVA");
            config_varios cv_modo = serv_config_varios.getEntityByClave("MODO");
            config_varios cv_voz = serv_config_varios.getEntityByClave("VOZ");

            ServiceAsignaciones serv_asign = new ServiceAsignaciones();
            sy_asignaciones asig = serv_asign.getEntityByFolio(cv_asign.valor);

            ASIGNACION_ACTIVA = cv_asign.valor;
            FK_ASIGNACION_ACTIVA = cv_asign.valor != "" ? asig.pkAsignacion : 0;
            MODO_APP = cv_modo.valor;
            VOZ = cv_voz.valor;

            reproducir_mensajes_no_reproducidos();
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

        private void reproducir_mensajes_no_reproducidos()
        {
            //Reproducir Mensajes Pendientes
            ServiceMensajes serv_mensajes = new ServiceMensajes();
            List<sy_mensajes> list_mensajes = obtener_mensaje_no_reproducidos();

            foreach (var item_msn in list_mensajes)
            {
                //Agregar el texto al listbox mensajes
                ListBoxItem lbi = new ListBoxItem();
                lbi.Content = item_msn.mensaje;
                lbi.Foreground = Brushes.DarkGray;
                listbox_mensajes.Items.Add(lbi);

                //reprodudir con voz el texto del mensaje recibido.
                hablar_texto(item_msn.mensaje);
                item_msn.reproducido = 1;
                serv_mensajes.updEntity(item_msn);

                Task.WaitAll(new Task[] { Task.Delay(300) });
            }
            

        }
        private List<sy_mensajes> obtener_mensaje_no_reproducidos()
        {
            List<sy_mensajes> list_msn = new List<sy_mensajes>();

            ServiceMensajes serv_mensajes = new ServiceMensajes();
            list_msn = serv_mensajes.getEntityNoReproducidos();

            return list_msn;
        }

        private void Mensajes_Unload(object sender, RoutedEventArgs e)
        {
            close_serial_port();
            //dispose_serial_port();
        }
        private void btnMostrarTeclado_Click(object sender, RoutedEventArgs e)
        {
            //PanelTeclado panel_teclado = new PanelTeclado();
            //panel_teclado.Visibility = Visibility.Visible;

            runVirtualKeyboard();
        }
        private void btnEnviarMensaje_Click(object sender, RoutedEventArgs e)
        {
            string mensaje = txtMensajes.Text;
            EnviarMensaje(mensaje);

            txtMensajePopup.Text = "MENSAJE ENVIADO";
            mostrarPopupOk();

            txtMensajes.Text = "";
        }
        private void btnEnviarAsistenciaMecanica_Click(object sender, RoutedEventArgs e)
        {
            string mensaje = "ASISTENCIA MECANICA REQUERIDA";
            EnviarMensaje(mensaje);

            txtMensajePopup.Text = "MENSAJE ENVIADO";
            mostrarPopupOk();
        }
        private void btnEnviarAsistenciaVial_Click(object sender, RoutedEventArgs e)
        {
            string mensaje = "ASISTENCIA VIAL REQUERIDA";
            EnviarMensaje(mensaje);

            txtMensajePopup.Text = "MENSAJE ENVIADO";
            mostrarPopupOk();
        }
        private void btnEnviarSOS_Click(object sender, RoutedEventArgs e)
        {
            string mensaje = "SOS EMERGENCIA - ATENCION URGENTE";
            EnviarMensaje(mensaje);

            txtMensajePopup.Text = "MENSAJE ENVIADO";
            mostrarPopupOk();
        }
        private void btnEnviarEmerganciaClinica_Click(object sender, RoutedEventArgs e)
        {
            string mensaje = "ATENCION MEDICA REQUERIDA CON URGENCIA";
            EnviarMensaje(mensaje);

            txtMensajePopup.Text = "MENSAJE ENVIADO";
            mostrarPopupOk();
        }

        private void EnviarMensaje(string mensaje)
        {
            // 1 = UNIDAD
            // 2 = TISA

            mensaje = mensaje.ToString().ToUpper();

            //Colocar el texto enviado en el listbox
            ListBoxItem item = new ListBoxItem();
            item.Content = mensaje;
            listbox_mensajes.Items.Add(item);


            string fecha_actual = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            ServiceMensajes serv_mensajes = new ServiceMensajes();
            sy_mensajes msn = new sy_mensajes();

            msn.pkMensaje = 0;
            msn.fkAsignacion = FK_ASIGNACION_ACTIVA;
            msn.fkStatus = 1;
            msn.mensaje = mensaje;
            msn.enviado = 0;
            msn.confirmadoTISA = 0;
            msn.modo = MODO_APP;
            msn.dispositivo_origen = 1;
            msn.dispositivo_destino = 2;
            msn.reproducido = 0;
            msn.created_at = fecha_actual;
            msn.updated_at = null;
            msn.deleted_at = null;

            serv_mensajes.addEntity(msn);
        }

       

        //private void Sincronizar_Mensajes()
        //{
        //    // 1 = UNIDAD
        //    // 2 = TISA

        //    string fecha_actual = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        //    MensajesController mensajes_controller = new MensajesController();

        //    ServiceMensajes serv_mensajes = new ServiceMensajes();
        //    List<sy_mensajes> list_mensaje = serv_mensajes.getEntityNoEnviados();

        //    foreach (var item in list_mensaje)
        //    {
        //        ResMensajes_Insert resMensajes_insert = mensajes_controller.InsertMensaje(item);
        //        if(resMensajes_insert.response == true && resMensajes_insert.status == 200)
        //        {
        //            item.confirmadoTISA = 1;
        //            serv_mensajes.updEntity(item);
        //        }
        //    }

        //}

        private void Hablar(string texto)
        {
            SpeechSynthesizer voz = new SpeechSynthesizer();
            voz.SetOutputToDefaultAudioDevice();
            voz.Speak(texto.ToString());
        }


        private void Btn_Click(object sender, EventArgs e)
        {
            Despintar();

            Button btn = sender as Button;
            btn.Background = Brushes.LightGray;
            txtMensajes.Text += btn.Content.Equals("ESPACIO") ? " " : btn.Content;

        }
        private void Despintar()
        {
            //foreach (var item in pnl_teclado.Children.Add()
            //{

            //}
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
