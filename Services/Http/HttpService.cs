using ConsolaBlazor.Services.DTOs;
using ConsolaBlazor.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.VisualBasic;
using MudBlazor;
using System;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace ConsolaBlazor.Services.Http
{
    public class HttpService: IHttpService
    {
        private readonly HttpClient _httpClient;
        private readonly ISnackbar _snackbar;
        private readonly IConfiguration _configuration;

        public HttpService(HttpClient httpClient, ISnackbar snackbar, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _snackbar = snackbar;
            _configuration = configuration;
        }

        public async Task<List<T>?> FetchData<T>(string url)
        {
            try
            {
                if (string.IsNullOrEmpty(url))
                {
                    _snackbar.Add("La url es requerida", Severity.Warning, config => { config.HideIcon = true; });
                    return null;
                }

                var baseUrl = _configuration["UrlBackend"];
                var response = await _httpClient.GetAsync($"{baseUrl}/{url}");
                Console.WriteLine(response);
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadFromJsonAsync<List<T>>();

                    if (data != null)
                    {
                        return data;
                    }
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    var errorData = JsonSerializer.Deserialize<ApiResponseDTO>(errorResponse);

                    var Message = errorData?.Message ?? errorData?.Mensaje ?? string.Empty;

                    _snackbar.Add(Message, Severity.Warning, config => { config.HideIcon = true; });
                    return null;
                }

                _snackbar.Add("Ocurrió un error inesperado, explore la consola", Severity.Warning, config => { config.HideIcon = true; });
                return null;
            }
            catch (Exception ex)
            {
                _snackbar.Add($"Error en el servidor: {ex.Message}", Severity.Error, config => { config.HideIcon = true; });
                return null;
            }
        }

        public async Task<ApiResponseDTO> FetchDataById<T>(string url, int Id)
        {
            var responseApi = new ApiResponseDTO()
            {
                Success = false
            };

            try
            {
                if (string.IsNullOrEmpty(url))
                {
                    _snackbar.Add("La url es requerida", Severity.Warning, config => { config.HideIcon = true; });
                    return responseApi;
                }

                var baseUrl = _configuration["UrlBackend"];
                var response = await _httpClient.GetAsync($"{baseUrl}/{url}?Id={Id}");

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadFromJsonAsync<T>();

                    if (data != null)
                    {
                        responseApi.Success = true;
                        responseApi.Data = data;
                        return responseApi;
                    }
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    var errorData = JsonSerializer.Deserialize<ApiResponseDTO>(errorResponse);

                    responseApi.Message = errorData.Message;

                    _snackbar.Add(errorData?.Message, Severity.Warning, config => { config.HideIcon = true; });
                    return responseApi;
                }
                _snackbar.Add("Ocurrió un error inesperado, explore la consola", Severity.Warning, config => { config.HideIcon = true; });
                return responseApi;
            }
            catch (Exception ex)
            {
                _snackbar.Add($"Error en el servidor: {ex.Message}", Severity.Error, config => { config.HideIcon = true; });
                responseApi.Message = ex.Message;
                return responseApi;
            }
        }

        public async Task<ApiResponseDTO> CrearFn<T>(string Path, T Parametros)
        {
            var result = new ApiResponseDTO();

            try
            {
                if (string.IsNullOrEmpty(Path))
                {
                    _snackbar.Add("El path es requerido", Severity.Warning, config => { config.HideIcon = true; });
                    return result;
                }
                var baseUrl = _configuration["UrlBackend"];
                var response = await _httpClient.PostAsJsonAsync($"{baseUrl}/{Path}", Parametros);

                if (response.IsSuccessStatusCode)
                {
                    result.Success = true;
                    result.Message = "Creación exitosa";
                    _snackbar.Add(result.Message, Severity.Success, config => { config.HideIcon = true; });
                    return result;
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    var errorData = JsonSerializer.Deserialize<ApiResponseDTO>(errorResponse);

                    _snackbar.Add(errorData?.Message, Severity.Warning, config => { config.HideIcon = true; });

                    result.Success = false;
                    result.Message = errorData?.Message;
                    return result;
                }
                _snackbar.Add("Ocurrió un error inesperado, explore la consola", Severity.Warning, config => { config.HideIcon = true; });
                return result;

            }
            catch (Exception ex)
            {
                _snackbar.Add(ex.Message, Severity.Error, config => { config.HideIcon = true; });
                result.Success = false;
                result.Message = ex.Message;
                return result;
            }
        }

        public async Task<ApiResponseDTO> ActualizarFn<T>(string Path, T Parametros)
        {
            var result = new ApiResponseDTO()
            {
                Success = false,
                Message = string.Empty,
            };

            try
            {
                if (string.IsNullOrEmpty(Path))
                {
                    _snackbar.Add("El path es requerido", Severity.Warning, config => { config.HideIcon = true; });
                    return result;
                }
                var baseUrl = _configuration["UrlBackend"];
                var response = await _httpClient.PatchAsJsonAsync($"{baseUrl}/{Path}", Parametros);

                if (response.IsSuccessStatusCode)
                {
                    result.Success = true;
                    result.Message = "Actualización exitosa";
                    _snackbar.Add(result.Message, Severity.Success, config => { config.HideIcon = true; });
                    return result;
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    var errorData = JsonSerializer.Deserialize<ApiResponseDTO>(errorResponse);

                    _snackbar.Add(errorData?.Message, Severity.Warning, config => { config.HideIcon = true; });

                    result.Message = errorData?.Message;
                    return result;
                }
                _snackbar.Add("Ocurrió un error inesperado, explore la consola", Severity.Warning, config => { config.HideIcon = true; });
                return result;

            }
            catch (Exception ex)
            {
                _snackbar.Add(ex.Message, Severity.Error, config => { config.HideIcon = true; });
                result.Message = ex.Message;
                return result;
            }
        }

        public async Task<ApiResponseDTO> EliminarFn(string Path, int Id)
        {
            var result = new ApiResponseDTO()
            {
                Success = false,
                Message = string.Empty,
            };

            try
            {
                var baseUrl = _configuration["UrlBackend"];
                var response = await _httpClient.DeleteAsync($"{baseUrl}/{Path}?Id={Id}");

                if (response.IsSuccessStatusCode)
                {
                    result.Success = true;
                    result.Message = "Eliminado exitósamente";
                    _snackbar.Add(result.Message, Severity.Success, config => { config.HideIcon = true; });
                    return result;
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    var errorData = JsonSerializer.Deserialize<ApiResponseDTO>(errorResponse);

                    _snackbar.Add(errorData?.Message, Severity.Warning, config => { config.HideIcon = true; });
                    result.Message = errorData?.Message;
                    return result;
                }
                _snackbar.Add("Ocurrió un error inesperado, explore la consola", Severity.Warning, config => { config.HideIcon = true; });
                return result;
            }
            catch (Exception ex)
            {
                _snackbar.Add(ex.Message, Severity.Error, config => { config.HideIcon = true; });
                result.Message += ex.Message;
                return result;
            }
        }
    }
}
