using PruebaChatbot.Models;
using System.Collections.Concurrent;
using System.Globalization;
using System.Net;

namespace PruebaChatbot
{
    public class BotService
    {
        private readonly HttpClient _http;
        private readonly ILogger<BotService> _logger;
        private const string Menu = "1) Consultar mis productos,\n2) Hablar con un asesor";

        private static readonly ConcurrentDictionary<string, string> Estados = new();

        public BotService(HttpClient http, ILogger<BotService> logger)
        {
            _http = http;
            _logger = logger;
        }

        public async Task ProcesamientoAsync(string from, string texto)
        {
            var estado = Estados.GetValueOrDefault(from, "INICIO");
            string respuesta = string.Empty;

            switch (estado)
            { 
                case "INICIO":
                    respuesta = $"Hola un gusto saludarte, que puedo hacer por ti,\n {Menu}";
                    estado = "MENU";
                    break;
                case "MENU" when texto == "1":
                    respuesta = "Por favor diguite el numero del cliente";
                    estado = "ESPERANDO_ID";
                    break;
                case "MENU" when texto == "2":
                    respuesta = "Te estaremos trasfieriendo con un asesion";
                    estado = "INICIO";
                    break;
                case "MENU":
                    respuesta = "Opcion no valida";
                    estado = "MENU";
                    break;
                case "ESPERANDO_ID":
                    (respuesta,estado) = await ConsultarInformacionExterna(texto);
                    break; 
            }

            Estados[from] = estado;
            _logger.LogInformation($"From:{from} Estado:{estado} Respuesta:{respuesta}");
        }

        public async Task<(string,string)> ConsultarInformacionExterna(string text)
        {
            try
            {
                //Validamos que el texto enviado sea un numero mayor a 0
                if (!int.TryParse(text, out var id) || id < 0)
                {
                    return ("El numero del cliente debe ser un número mayor a 0", "ESPERANDO_ID");
                }

                //validamos que el cliente exista
                var usuario = await _http.GetAsync($"users/{id}");
                if (usuario.StatusCode == HttpStatusCode.NotFound)
                {
                    return ($"El cliente con el id {id} no se encuentra en registrado en nuestro sistema", "ESPERANDO_ID");
                }
                usuario.EnsureSuccessStatusCode();
                var user = await usuario.Content.ReadFromJsonAsync<Usuario>();

                //Obtener los productos del cliente
                //validamos que el cliente exista
                var carts = await _http.GetFromJsonAsync<CartsRspuestas>($"carts/user/{id}");
                var productos = carts?.carts?.SelectMany(c => c.products).ToList();

                if(!productos.Any() || productos.Count == 0)
                {
                    return ($"El cliente con el id {id} no tiene productos registrados", "ESPERANDO_ID");
                }

                var lineas = productos.Select(p => $"{p.title} x{p.quantity} - {Dinero(p.price * p.quantity)}");
                var total = productos.Sum(p => p.price * p.quantity);

                return ($"Hola {user.firstName}, estos son tus productos: {string.Join("\n ", lineas)}\nTotal: {Dinero(total)}", "");
            }
            catch (Exception ex)
            {
                return ($"No se pudo consultar la informacion en este momento, intente mas tarde\n {Menu}", "MENU");
            }
            
        }

        private static string Dinero(decimal valor)
        {
            return valor.ToString("C", CultureInfo.CreateSpecificCulture("es-CO"));
        }
    }
}
