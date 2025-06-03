using Kolos1Poprawa.Models;
using Microsoft.Data.SqlClient;
namespace Kolos1Poprawa.Services
{
    public class ClientService : IClientService
    {
        private readonly IConfiguration _configuration;
        public ClientService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<ClientDto?> GetClient(int id)
        {
            ClientDto? clientDto;
            string command = "SELECT ID, FirstName, LastName, Address FROM clients WHERE ID = @ClientId";

            using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("Default")))
            using (SqlCommand cmd = new SqlCommand(command, conn))
            {
                cmd.Parameters.AddWithValue("@ClientId", id);
                await conn.OpenAsync();
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        clientDto = new ClientDto
                        {
                            Id = reader.GetInt32(0),
                            FirstName = reader.GetString(1),
                            LastName = reader.GetString(2),
                            Address = reader.GetString(3),
                            Rentals = new List<RentalDto>()
                        };
                    }
                    else
                    {
                        return null;
                    }
                }
            }

            command =
              "SELECT cs.VIN, col.Name AS Color, m.Name AS Model, r.DateFrom, r.DateTo, r.TotalPrice " +
              "FROM car_rentals r " +
              "JOIN cars cs ON r.CarID = cs.ID " +
              "JOIN colors col ON cs.ColorID = col.ID " +
              "JOIN models m ON cs.ModelID = m.ID " +
              "WHERE r.ClientID = @ClientId";

            using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("Default")))
            using (SqlCommand cmd = new SqlCommand(command, conn))
            {
                cmd.Parameters.AddWithValue("@ClientId", id);
                await conn.OpenAsync();
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var rental = new RentalDto
                        {
                            Vin = reader.GetString(0),
                            Color = reader.GetString(1),
                            Model = reader.GetString(2),
                            DateFrom = reader.GetDateTime(3),
                            DateTo = reader.GetDateTime(4),
                            TotalPrice = reader.GetInt32(5)
                        };
                        clientDto.Rentals.Add(rental);
                    }
                }
            }

            return clientDto;
        }
        
        public async Task<int> CreateClient(CreateClientDto request)
        { if (request.DateFrom >= request.DateTo)
            {
                return -4;
            }
            string command = "SELECT PricePerDay FROM cars WHERE ID = @CarId";
            int pricePerDay;
            using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("Default")))
            using (SqlCommand cmd = new SqlCommand(command, conn))
            {
                cmd.Parameters.AddWithValue("@CarId", request.CarId);
                await conn.OpenAsync();
                var result = await cmd.ExecuteScalarAsync();
                if (result == null)
                {
                    return -1;
                }
                pricePerDay = Convert.ToInt32(result);
            }
            int days = (request.DateTo.Date - request.DateFrom.Date).Days;
            int totalPrice = days * pricePerDay;
            
            int newClientId;
            command = "INSERT INTO clients (FirstName, LastName, Address) OUTPUT INSERTED.ID VALUES (@FirstName, @LastName, @Address)";
            using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("Default")))
            using (SqlCommand cmd = new SqlCommand(command, conn))
            {
                cmd.Parameters.AddWithValue("@FirstName", request.Client.FirstName);
                cmd.Parameters.AddWithValue("@LastName", request.Client.LastName);
                cmd.Parameters.AddWithValue("@Address", request.Client.Address);
                await conn.OpenAsync();
                var scalar = await cmd.ExecuteScalarAsync();
                if (scalar == null)
                {
                    return -2;
                }
                newClientId = Convert.ToInt32(scalar);
            }
            
            command = "INSERT INTO car_rentals (ClientID, CarID, DateFrom, DateTo, TotalPrice) VALUES (@ClientID, @CarId, @DateFrom, @DateTo, @TotalPrice)";
            using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("Default")))
            using (SqlCommand cmd = new SqlCommand(command, conn))
            {
                cmd.Parameters.AddWithValue("@ClientID", newClientId);
                cmd.Parameters.AddWithValue("@CarId", request.CarId);
                cmd.Parameters.AddWithValue("@DateFrom", request.DateFrom);
                cmd.Parameters.AddWithValue("@DateTo", request.DateTo);
                cmd.Parameters.AddWithValue("@TotalPrice", totalPrice);
                await conn.OpenAsync();
                int rowsAffected = await cmd.ExecuteNonQueryAsync();
                if (rowsAffected <= 0)
                {
                    return -3;
                }
            }
            
            return newClientId;
        }
    }
}
