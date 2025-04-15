﻿using System.Net.Http;
using System.Text.Json;
using MercadoPago.Client.Payment;
using MercadoPago.Client.Preference;
using MercadoPago.Http;
using MercadoPago.Resource.MerchantOrder;
using MercadoPago.Resource.Payment;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProgressusWebApi.Dtos.AuthDtos;
using ProgressusWebApi.Dtos.InventarioDtos;
using ProgressusWebApi.Dtos.MembresiaDtos;
using ProgressusWebApi.Dtos.MercadoPagoDtos;
using ProgressusWebApi.Models.MembresiaModels;
using ProgressusWebApi.Services.AuthServices.Interfaces;
using ProgressusWebApi.Services.InventarioServices.Interfaces;
using ProgressusWebApi.Services.MembresiaServices.Interfaces;
using ProgressusWebApi.Services.PedidosServices.Interfaces;
using WebApiMercadoPago.Services.Interface;

namespace ProgressusWebApi.Controllers.MembresiaControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AAMercadoPagoController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private const string MercadoPagoBaseUrl = "https://api.mercadopago.com/v1/payments/";
        private readonly ISolicitudDePagoService _solicituDePagoService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IInventarioService _inventarioService;
        private readonly IMercadoPagoService _mercadoPagoService;
        private readonly IPedidoService _pedidoService;

        public AAMercadoPagoController(
            IHttpClientFactory httpClientFactory,
            ISolicitudDePagoService solicitudDePagoService,
            UserManager<IdentityUser> userManager,
            IInventarioService inventarioService,
            IMercadoPagoService mp,
            IPedidoService pedido
        )
        {
            _httpClientFactory = httpClientFactory;
            _solicituDePagoService = solicitudDePagoService;
            _userManager = userManager;
            _inventarioService = inventarioService;
            _mercadoPagoService = mp;
            _pedidoService = pedido;
        }

        [HttpPost("Prueba")]
        public async Task<IActionResult> Prueba([FromBody] dynamic res)
        {
            /*
            if (res != null)
            {
                string email = res.payer.email;
                IdentityUser? usuario = await _userManager.FindByEmailAsync(email);
                SolicitudDePago? solicitud = await _solicituDePagoService.ObtenerSolicitudDePagoDeSocio(usuario.Id);
                if (res.status == "approved")
                {
                    await _solicituDePagoService.RegistrarPagoEnEfectivo(solicitud.Id);
                }
            }*/
            var idPayment = res?.id.ToString();
            return Ok("El id es: " + idPayment);
        }

        [HttpPost("ObtenerRequestMercadoPago")]
        public async Task<IActionResult> ObtenerRequestMercadoPago([FromBody] dynamic req)
        //Probar agregandole un [FromQuery]dynamic
        {
            try
            {
                var paymentId = req.data.id;
                // Crear cliente HTTP
                var httpClient = _httpClientFactory.CreateClient();

                // Configurar la solicitud
                var request = new HttpRequestMessage(
                    System.Net.Http.HttpMethod.Get,
                    $"{MercadoPagoBaseUrl}{paymentId}"
                );
                request.Headers.Add(
                    "Authorization",
                    "Bearer APP_USR-8561091671920007-062802-878c7cba462bc355cb6143df2a4634f3-1070425524"
                );

                // Realizar la solicitud
                HttpResponseMessage response = await httpClient.SendAsync(request);
                var res = response.Content.ReadAsStreamAsync().Result;
                var data = JsonSerializer.Deserialize<PaymentDto>(
                    res,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );
                // Validar la respuesta
                if (data.status == "approved")
                {
                    // Leer el contenido del response

                    var email = data.additional_info.payer.first_name.ToString();
                    var precio = data.additional_info.items[0].unit_price.ToString();

                    IdentityUser? usuario = await _userManager.FindByEmailAsync(email);
                    SolicitudDePago? solicitud =
                        await _solicituDePagoService.ObtenerSolicitudDePagoDeSocio(usuario.Id);
                    await _solicituDePagoService.RegistrarPagoEnEfectivo(solicitud.Id);
                    SolicitudDePago? solicitud2 =
                        await _solicituDePagoService.ObtenerSolicitudDePagoDeSocio(usuario.Id);

                    return Ok(solicitud2);
                }
                else
                {
                    Console.WriteLine($"Error en la solicitud: {response.StatusCode}");
                    return StatusCode(
                        (int)response.StatusCode,
                        "Error al procesar la solicitud en MercadoPago."
                    );
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        [HttpPost("NotificarPedidoCompletado/{pedidoId}")]
        public async Task<IActionResult> NotificarPedidoCompletado(
            [FromBody] NotificationDto req,
            string pedidoId
        )
        {
            try
            {
                if (req == null || string.IsNullOrEmpty(pedidoId) || req.data?.id == null)
                    return BadRequest();

                var paymentId = req!.data!.id;

                var estado = await _mercadoPagoService.ConsultarEstadoPreferencia(paymentId);

                if (estado == "approved")
                {
                    var registradoOK = await _pedidoService.RegistrarPago(pedidoId);
                    if (!registradoOK)
                        return BadRequest();

                    return Ok();
                }
                else
                {
                    Console.WriteLine($"Error en la solicitud: {estado}");
                    return Ok();
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
