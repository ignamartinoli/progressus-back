using System.Net.Http;
using System.Text.Json;
using MercadoPago.Client.Preference;
using MercadoPago.Resource.Preference;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProgressusWebApi.Dtos.MercadoPagoDtos;
using ProgressusWebApi.Repositories.MembresiaRepositories.Interfaces;
using ProgressusWebApi.Services.InventarioServices.Interfaces;
using ProgressusWebApi.Services.MembresiaServices.Interfaces;
using WebApiMercadoPago.Repositories.Interface;

namespace ProgressusWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AA_MPController : ControllerBase
    {


        private readonly IHttpClientFactory _httpClientFactory;
        private const string MercadoPagoBaseUrl = "https://api.mercadopago.com/v1/payments/";
        private readonly ISolicitudDePagoService _solicituDePagoService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IInventarioService _inventarioService;

        public AA_MPController(IHttpClientFactory httpClientFactory, ISolicitudDePagoService solicitudDePagoService, UserManager<IdentityUser> userManager, IInventarioService inventarioService)
        {
            _httpClientFactory = httpClientFactory;
            _solicituDePagoService = solicitudDePagoService;
            _userManager = userManager;
            _inventarioService = inventarioService;

        }


        //Crear una PreferenceRequest
        [HttpPost("CrearSolicitudDePago")]
        public async Task<IActionResult> CrearSolicitudDePago(CompraMercadoPagoDto datosCompra)
        {
            PreferenceClient client = new PreferenceClient();

            var request = new PreferenceRequest
            {
                Items = datosCompra.ItemsCompra.Select(item => new PreferenceItemRequest
                {
                    Title = item.Nombre,
                    CurrencyId = "ARS",
                    PictureUrl = "https://www.mercadopago.com/org-img/MP3/home/logomp3.gif",
                    Description = "",
                    Quantity = item.Cantidad,
                    UnitPrice = item.Precio
                }).ToList(),
                Payer = new PreferencePayerRequest
                {
                    Name = datosCompra.NombreCliente,
                    Email = datosCompra.EmailCliente,
                },

                //Redirección del usuario según resultado del pago
                BackUrls = new PreferenceBackUrlsRequest
                {
                    Success = "https://pages-mp.vercel.app/success",
                    Failure = "https://pages-mp.vercel.app/failure",
                    Pending = "https://pages-mp.vercel.app/pending"
                },

                //URL para recibir la información del pago        
                NotificationUrl = "https://progressuscenter.azurewebsites.net/api/AA_MPController/RecibirPago",

                AutoReturn = "approved",
                PaymentMethods = new PreferencePaymentMethodsRequest
                {
                    //
                },

                //Expira en una hora
                Expires = true,
                ExpirationDateFrom = DateTime.Now,
                ExpirationDateTo = DateTime.Now.AddHours(1),
            };

            //Devuelve el init-point (link a donde redirigir al cliente para el pago)
            Preference result = await client.CreateAsync(request);
            return (IActionResult)result;

        }


        //Bool para determinar si el pago se concretó
        [HttpPost("RecibirPago")]
        public async Task<IActionResult> RecibirPago([FromBody] dynamic req)
        //Probar agregandole un [FromQuery]dynamic
        {

            try
            {

                var paymentId = req.data.id;
                // Crear cliente HTTP
                var httpClient = _httpClientFactory.CreateClient();


                // Configurar la solicitud
                var request = new HttpRequestMessage(System.Net.Http.HttpMethod.Get, $"{MercadoPagoBaseUrl}{paymentId}");
                request.Headers.Add("Authorization", "Bearer APP_USR-2278733141716614-062815-583c9779901a7bbf32c8e8a73971e44c-1878150528");

                // Realizar la solicitud
                HttpResponseMessage response = await httpClient.SendAsync(request);
                var res = response.Content.ReadAsStreamAsync().Result;
                var data = JsonSerializer.Deserialize<PaymentDto>(res, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                // Validar la respuesta
                if (data.status == "approved")
                {
                    // Leer el contenido del response

                    var email = data.additional_info.payer.first_name.ToString();
                    var precioTotal = data.additional_info.items.Sum(item => item.unit_price);

                    //COMPORTAMIENTO FINAL


                    return Ok();
                }
                else
                {
                    Console.WriteLine($"Error en la solicitud: {response.StatusCode}");
                    return StatusCode((int)response.StatusCode, "Error al procesar la solicitud en MercadoPago.");
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, "Error interno del servidor.");
            }
        }



    }
}
