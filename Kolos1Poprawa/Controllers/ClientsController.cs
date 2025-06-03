using Kolos1Poprawa.Models;
using Kolos1Poprawa.Services;
using Microsoft.AspNetCore.Mvc;

namespace Kolos1Poprawa.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly IClientService _clientService;
        
        public ClientsController(IClientService clientService)
        {
            _clientService = clientService;
        }
        
        [HttpGet("{id}")]
        public IActionResult GetClient(int id)
        {
            var clientTask = _clientService.GetClient(id);
            var client = clientTask.Result;
            
            if (client == null)
                return NotFound($"Client z id: {id} nie znaleziony");
            
            return Ok(client);
        }
        
        [HttpPost]
        public IActionResult CreateClient([FromBody] CreateClientDto request)
        {
            if (request is null || request.Client is null)
                return BadRequest("Błędne dane klienta");

            var createdTask = _clientService.CreateClient(request);
            int result = createdTask.Result;
            
            if (result < 0)
            {
                string errorMsg = result switch
                {
                    -1 => "Auto o podanym ID nie istnieje",
                    -2 => "Błąd przy insertowaniu danych klienta",
                    -3 => "Błąd przy insertwoaniu danych wyporzyczenia",
                    -4 => "DateFrom muszi byc wczesniejsza niż DateTo."
                };
                return BadRequest(errorMsg);
            }
            
            return CreatedAtAction(nameof(GetClient), new { id = result }, $"Stworzono klienta o id: {result}");
        }
    }
}


