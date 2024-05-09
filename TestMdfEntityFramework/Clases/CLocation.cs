using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Device;
using System.Device.Location;
using TestMdfEntityFramework.Controllers;
using TestMdfEntityFramework.Responses;
using TestMdfEntityFramework.EntityServices;
using TestMdfEntityFramework.Utils;

namespace TestMdfEntityFramework.Clases
{
    public class CLocation
    {
        GeoCoordinateWatcher watcher;
        decimal[] arrLatLng = new decimal[2];

        string ASIGNACION_ACTIVA = "";
        long FK_ASIGNACION_ACTIVA = 0;
        string MODO_APP = "";

        public decimal latitud { get; set; }
        public decimal longitud { get; set; }

        int contador = 0;

        public CLocation()
        {
            init_variables();
        }

        public CLocation(decimal latitud, decimal longitud)
        {
            this.latitud = latitud;
            this.longitud = longitud;

            init_variables();
        }

        private void init_variables()
        {
            //OBTENER CONFIGURACIONES VARIAS DEL SISTEMA Y OPERACION ACTUAL
            ServiceConfigVarios serv_config_varios = new ServiceConfigVarios();
            config_varios cv_asign = serv_config_varios.getEntityByClave("ASIGNACION_ACTIVA");
            config_varios cv_modo = serv_config_varios.getEntityByClave("MODO");

            ServiceAsignaciones serv_asign = new ServiceAsignaciones();
            sy_asignaciones asig = serv_asign.getEntityByFolio(cv_asign.valor);

            ASIGNACION_ACTIVA = cv_asign.valor;
            FK_ASIGNACION_ACTIVA = asig.pkAsignacion;
            MODO_APP = cv_modo.valor;

            contador = 0;
        }

        public void GetLocationDataEvent()
        {
            this.watcher = new GeoCoordinateWatcher();
            this.watcher.PositionChanged += new EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>(watcher_PositionChanged);
            this.watcher.Start();
        }

        private void watcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            if(contador == 0)
            {
                latitud = (decimal)e.Position.Location.Latitude;
                longitud = (decimal)e.Position.Location.Longitude;
                this.watcher.Stop();

                Sincronizar_Ubicacion(latitud, longitud);

                contador++;
            }

            


            ////Enviar la Ubicacion a TISA, por medio del servicio
            //UbicacionController ubi_controller = new UbicacionController();
            //sy_ubicacion ubi = new sy_ubicacion();
            //ubi.fkAsignacion = FK_ASIGNACION_ACTIVA;
            //ubi.latitud = latitud;
            //ubi.longitud = longitud;
            //ubi.enviado = 1;
            //ubi.confirmadoTISA = 0;
            //ubi.modo = MODO_APP;
            //ResUbicacion res_ubicacion = ubi_controller.InsertUbicacion(ubi);

            ////Insertar la ubicacion en la dblocal en caso de que se haya insertado correctamente en TISA.
            //if (res_ubicacion.response == true && res_ubicacion.status == 200)
            //{
            //    ServiceUbicacion serv_ubicacion = new ServiceUbicacion();
            //    ubi.confirmadoTISA = 1;
            //    serv_ubicacion.addEntity(ubi);
            //}
        }

        private void Sincronizar_Ubicacion(decimal _lat, decimal _lng)
        {
            try
            {
                Comun comun = new Comun();
                if (comun.HayConexionInternet())
                {
                    //Enviar la Ubicacion a TISA, por medio del servicio
                    UbicacionController ubi_controller = new UbicacionController();
                    sy_ubicacion ubi = new sy_ubicacion();
                    ubi.fkAsignacion = FK_ASIGNACION_ACTIVA;
                    ubi.latitud = _lat;
                    ubi.longitud = _lng;
                    ubi.enviado = 1;
                    ubi.confirmadoTISA = 0;
                    ubi.modo = MODO_APP;
                    ResUbicacion res_ubicacion = ubi_controller.InsertUbicacion(ubi);

                    //Insertar la ubicacion en la dblocal en caso de que se haya insertado correctamente en TISA.
                    if (res_ubicacion.response == true && res_ubicacion.status == 200)
                    {
                        ServiceUbicacion serv_ubicacion = new ServiceUbicacion();
                        ubi.confirmadoTISA = 1;
                        serv_ubicacion.addEntity(ubi);
                    }
                }
            }
            catch (Exception ex)
            {


            }
        }



        private void watcher_PositionChanged_v2(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            PrintPosition(e.Position.Location.Latitude, e.Position.Location.Longitude);
            // Stop receiving updates after the first one.
            this.watcher.Stop();
        }

        private void PrintPosition(double Latitude, double Longitude)
        {
            Console.WriteLine("Latitude: {0}, Longitude {1}", Latitude, Longitude);
        }
    }
}
