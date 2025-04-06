using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using TaskManager.CrossCutting.Exceptions;
using TaskManager.CrossCutting.Responses;


namespace TaskManager.CrossCutting.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }


        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exceção não tratada.");

                context.Response.ContentType = "application/json";

                var response = new ErrorResponse();
                var statusCode = (int)HttpStatusCode.InternalServerError;

                if (ex is NotFoundException)
                {
                    statusCode = (int)HttpStatusCode.NotFound;
                    response.Message = "Recurso não encontrado.";
                }
                else if (ex is BadRequestException)
                {
                    statusCode = (int)HttpStatusCode.BadRequest;
                    response.Message = "Requisição inválida.";
                }
                else
                {
                    statusCode = (int)HttpStatusCode.InternalServerError;
                    response.Message = "Erro interno do servidor.";
                }


                response.StatusCode = statusCode;
                response.Timestamp = DateTime.UtcNow;
                response.Detail = ex.Message;

                context.Response.StatusCode = statusCode;
                var json = JsonSerializer.Serialize(response);
                await context.Response.WriteAsync(json);
            }







        }
    }


}
