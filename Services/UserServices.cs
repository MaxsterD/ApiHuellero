using ApiConsola.Infrastructura.Data;
using ApiConsola.Interfaces;
using ApiConsola.Services.DTOs;
using Dapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Data.SqlTypes;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ApiConsola.Services
{
    public class UserServices: IUserServices
    {
        private readonly ISqlServerDbContext _sqlServerDbContext;
        private readonly IConfiguration _config;
        private static readonly RandomNumberGenerator rng = RandomNumberGenerator.Create();

        public UserServices (ISqlServerDbContext sqlServerDbContext, IConfiguration config)
        {
            _sqlServerDbContext = sqlServerDbContext;
            _config = config;
        }

        public async Task<SessionDTO?> Login(LoginDTO login)
        {
            var sql1 = "select top 1 Nombre,Email as Correo,Rol,PasswordSalt,PasswordHash from [User].Informacion where Email = @Email";
            var usuario = await _sqlServerDbContext.Database.GetDbConnection().QueryFirstOrDefaultAsync<LoginValidationDTO?>(sql1, new {Email = login.Email});

            if (usuario != null) 
            {
                var sal = Convert.FromBase64String(usuario.PasswordSalt);
                string passwordHash = GeneratePasswordHash(login.Password, Convert.FromBase64String(usuario.PasswordSalt));
     

                if (passwordHash == usuario.PasswordHash)
                {
                    SessionDTO newSession = new SessionDTO
                    {
                        Nombre = usuario.Nombre,
                        Correo = usuario.Correo,
                        Rol = usuario.Rol
                    };
                    var token = await GenerateToken(newSession);
                    newSession.Token = token;
                    return newSession;
                }
            }

            return null;
        }
        
        public async Task<CreateResponse> RegistrarUsuario(NewUsuarioDTO login)
        {
            CreateResponse response = new CreateResponse();
            if (await Validar(login))
            {
                byte[] saltBytes = GenerateSalt();

                string passwordHash = GeneratePasswordHash(login.Password, saltBytes);

                string sql = "INSERT INTO [User].Informacion (Nombre,Email,Rol,PasswordSalt,PasswordHash) VALUES (@Nombre,@Email,@Rol,@PasswordSalt,@PasswordHash)";
                var parametros = new { Nombre = login.Nombre, Email = login.Correo, Rol = login.Rol, PasswordSalt = Convert.ToBase64String(saltBytes), PasswordHash = passwordHash };

                int rowsAffected = await _sqlServerDbContext.Database.GetDbConnection().ExecuteAsync(sql, parametros);

                response.Success = rowsAffected > 0;
                response.Message = "Usuario creado con éxito";

                return response;
            }
            else
            {
                response.Success = false;
                response.Message = "Ya existe un usuario con ese email";

                return response;
            }
        }

        public async Task<CreateResponse> BuscarUsuario(string email)
        {
            CreateResponse response = new CreateResponse();

            var sql1 = "SELECT * FROM [User].Informacion WHERE email = @Email";
            var user = await _sqlServerDbContext.Database.GetDbConnection().QueryFirstOrDefaultAsync<UserResponse>(sql1, new { Email = email });

            if(user == null)
            {
                response.Success = false;
                response.Message = "No se encontró";
                return response;
            }

            response.Success = true;
            response.Message = "Se encontró";
            response.Data = user;
            return response;
        }

        public async Task<CreateResponse> ActualizarUsuario(UpdateData login)
        {
            CreateResponse response = new CreateResponse();
            if (!string.IsNullOrEmpty(login.Correo))
            {
                var sql1 = "SELECT * FROM [User].Informacion WHERE email = @Email";
                var user = await _sqlServerDbContext.Database.GetDbConnection().QueryFirstOrDefaultAsync<User>(sql1, new {Email = login.Correo});
                if (user != null)
                {
                    var Salt = "";
                    var Hash = "";

                    if(login.Password.Length > 0)
                    {
                        byte[] saltBytes = GenerateSalt();
                        string passwordHash = GeneratePasswordHash(login.Password, saltBytes);

                        Salt = Convert.ToBase64String(saltBytes);
                        Hash = passwordHash;
                    }
                    else
                    {
                        Salt = user.PasswordSalt;
                        Hash = user.PasswordHash;
                    }

                    var sqlUpdate = @"UPDATE [User].Informacion 
                                      SET Email = @Email,
                                          PasswordSalt = @PasswordSalt, 
                                          PasswordHash = @PasswordHash,
                                          Nombre = @Nombre,
                                          Rol = @Rol
                                      WHERE Id = @Id
                                        "
                    ;

                    var parametros = new { Nombre = login.Nombre, Id = user.Id, Email = login.Correo, PasswordHash = Hash, PasswordSalt = Salt, Rol = login.Rol };
                    var rowsAffected = await _sqlServerDbContext.Database.GetDbConnection().ExecuteAsync(sqlUpdate, parametros);

                    response.Success = rowsAffected > 0;
                    response.Message = "Usuario actualizado con éxito";

                    return response;
                }
                response.Success = false;
                response.Message = "No existe el usuario a actualizar";

                return response;

            }
            else
            {
                response.Success = false;
                response.Message = "Ya existe un usuario con ese email";

                return response;
            }
        }
        
        private async Task<bool> Validar(NewUsuarioDTO login)
        {
            var sql1 = $"select top 1 Nombre,Email as Correo,Rol,PasswordSalt,PasswordHash from [User].Informacion where Email = @Email";
            var usuario = await _sqlServerDbContext.Database.GetDbConnection().QueryFirstOrDefaultAsync<LoginValidationDTO?>(sql1, new { Email = login.Correo });

            // si el usuario con el correo suministrado ya existe
            return usuario == null;
        }
        
        private async Task<string> GenerateToken(SessionDTO session)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, session.Nombre),
                new Claim(ClaimTypes.Email, session.Correo)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("JWT:key").Value));
            var encryptionKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:EncryptionKey"]));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(30),
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature),
                EncryptingCredentials = new EncryptingCredentials(encryptionKey, SecurityAlgorithms.Aes256KW, SecurityAlgorithms.Aes256CbcHmacSha512)
            };

            try
            {
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var encryptedToken = tokenHandler.WriteToken(token);

                return encryptedToken;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }    

        }
        
        private byte[] GenerateSalt()
        {
            byte[] salt = new byte[64];            
            rng.GetBytes(salt);            
            return salt;
        }
       
        private string GeneratePasswordHash(string password, byte[] salt)
        {

            using (var hmac = new HMACSHA512(salt))
            {
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                byte[] hashBytes = hmac.ComputeHash(passwordBytes);
                return Convert.ToBase64String(hashBytes);
            }
        }

        private class User
        {
            public int Id { get; set; }
            public string PasswordSalt { get; set; }
            public string PasswordHash { get; set; }
        }

        public class UserResponse
        {
            public string Nombre { get; set; }
            public string Password { get; set; }
            public string Email { get; set; }
            public string Rol { get; set; }
        }

        
    }
}
