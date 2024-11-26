using ApiConsola.Infrastructura.Data;
using ApiConsola.Interfaces;
using ApiConsola.Services.DTOs;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Storage.Internal;

namespace ApiConsola.Services
{
    public class TicketStatusService : ITicketStatusService
    {
        private readonly ISqlServerDbContext _connection;
        public TicketStatusService(ISqlServerDbContext connection)
        {
            _connection = connection;
        }

        public async Task<CreateResponse> CreatEstado(string NewEstado)
        {
            string sql = "INSERT INTO TicketStatus (Estado) VALUES (@Estado)";
            var response = await _connection.Database.GetDbConnection().ExecuteAsync(sql, new { Estado = NewEstado });

            return new CreateResponse { Success = response > 0, Message = "Estado creado exitósamente" };
        }

        public async Task<List<EstadosDTO>?> GetEstados()
        {
            string sql = "SELECT * FROM TicketStatus";
            var response = await _connection.Database.GetDbConnection().QueryAsync<EstadosDTO>(sql);
            if(response == null) return null;
            return response.ToList();
        }

        public async Task<CreateResponse> EditarEstado(EstadosDTO estado)
        {
            string sql = "UPDATE TicketStatus SET Estado = @Estado WHERE Id = @Id";
            var response = await _connection.Database.GetDbConnection().ExecuteAsync(sql, estado);

            return new CreateResponse { Success = response > 0, Message = "Estado actualizado exitósamente" };
        }
    }
}
