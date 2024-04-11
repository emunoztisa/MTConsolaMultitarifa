using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestMdfEntityFramework.Controller;
using TestMdfEntityFramework.Controllers;
using TestMdfEntityFramework.EntityServices;
using TestMdfEntityFramework.Responses;

namespace TestMdfEntityFramework.Clases
{
    public class SincronizacionTISA
    {
        Task tarea_sincroniza_boletos = new Task(SincronizaBoletos);
        Task tarea_sincroniza_boletos_detalle = new Task(SincronizaBoletosDetalles);
        Task tarea_sincroniza_boletos_y_boletos_detalle = new Task(SincronizaBoletosYBoletosDetalle);


        public void Task_Sincroniza_Boletos_START()
        {
            tarea_sincroniza_boletos.Start();
        }
        public void Task_Sincroniza_BoletosDetalle_START()
        {
            tarea_sincroniza_boletos_detalle.Start();
        }
        public void Task_Sincroniza_Boletos_y_BoletosDetalle_START()
        {
            tarea_sincroniza_boletos_y_boletos_detalle.Start();
        }


        public void Task_Sincroniza_Boletos_DISPOSE()
        {
            if (tarea_sincroniza_boletos.IsCanceled)
            {
                tarea_sincroniza_boletos.Dispose();
            }
        }
        public void Task_Sincroniza_BoletosDetalle_DISPOSE()
        {
            if (tarea_sincroniza_boletos_detalle.IsCanceled)
            {
                tarea_sincroniza_boletos_detalle.Dispose();
            }
        }
        public void Task_Sincroniza_Boletos_y_BoletosDetalle_DISPOSE()
        {
            if (tarea_sincroniza_boletos_y_boletos_detalle.IsCanceled)
            {
                tarea_sincroniza_boletos_y_boletos_detalle.Dispose();
            }
        }


        public static void SincronizaBoletos()
        {
            ServiceBoletos serv_boletos = new ServiceBoletos();
            List<sy_boletos> list = serv_boletos.getEntitiesByEnviados();

            BoletosController bc = new BoletosController();
            foreach (var item in list)
            {
                bc.InsertBoleto(item);
            }
        }
        public static void SincronizaBoletosDetalles()
        {
            ServiceBoletosDetalles serv_boletos_detalle = new ServiceBoletosDetalles();
            List<sy_boletos_detalle> list = serv_boletos_detalle.getEntitiesByEnviados();

            BoletosDetalleController bdc = new BoletosDetalleController();
            foreach (var item in list)
            {
                bdc.InsertBoletoDetalle(item);
            }
        }
        public static void SincronizaBoletosYBoletosDetalle()
        {
            ServiceBoletos serv_boletos = new ServiceBoletos();
            List<sy_boletos> list = serv_boletos.getEntitiesByEnviados();

            BoletosController bc = new BoletosController();
            foreach (var item in list)
            {
                ResBoletos_Insert resBoletoInseted = bc.InsertBoleto(item);
                if(resBoletoInseted.response == true && resBoletoInseted.status == 200)
                {
                    // TODO: Actualizar el pkBoletoTISA en la base de datos local
                    item.pkBoletoTISA = resBoletoInseted.data.pkBoleto;
                    item.enviado = 1;
                    item.confirmadoTISA = 1;
                    serv_boletos.updEntity(item);

                    //insertar detalle del boleto en TISA
                    ServiceBoletosDetalles serv_boletos_detalle = new ServiceBoletosDetalles();
                    List<sy_boletos_detalle> list_det = serv_boletos_detalle.getEntitiesByFkBoleto(item.pkBoleto);

                    BoletosDetalleController bdc = new BoletosDetalleController();
                    foreach (var item_det in list_det)
                    {
                        ResBoletosDetalle_Insert resBoletoDetalleInserted = bdc.InsertBoletoDetalle(item_det);
                        if(resBoletoDetalleInserted.response == true && resBoletoDetalleInserted.status == 200)
                        {
                            // TODO: Actualizar el pkBoleteDetalleTISA en la base de datos local.
                            item_det.pkBoletoDetalleTISA = resBoletoDetalleInserted.data.pkBoletoDetalle;
                            item_det.enviado = 1;
                            item_det.confirmadoTISA = 1;
                            serv_boletos_detalle.updEntity(item_det);
                        }
                    }
                }

            }
        }
        public static void SincronizaCortes()
        {
            ServiceCortes serv_cortes = new ServiceCortes();
            List<sy_cortes> list = serv_cortes.getEntitiesByEnviados();

            CortesController bc = new CortesController();
            foreach (var item in list)
            {
                ResCortes_Insert resCorteInserted = bc.InsertCorte(item);
                if (resCorteInserted.response == true && resCorteInserted.status == 200)
                {
                    // TODO: Actualizar el pkBoletoTISA en la base de datos local
                    item.pkCorteTISA = resCorteInserted.data.pkCorte;
                    item.enviado = 1;
                    item.confirmadoTISA = 1;
                    serv_cortes.updEntity(item);
                }
            }
        }


    }
}
