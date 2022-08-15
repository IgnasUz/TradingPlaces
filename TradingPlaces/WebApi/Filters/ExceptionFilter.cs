using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using TradingPlaces.WebApi.Exceptions;

namespace TradingPlaces.WebApi.Filters
{
    public class ExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext exceptionContext)
        {
            exceptionContext.Result = new ObjectResult(exceptionContext.Exception.Message)
            {
                StatusCode = GetStatusCode(exceptionContext.Exception)
            };

            base.OnException(exceptionContext);
        }

        private int GetStatusCode(Exception exception)
        {
            if (exception is InvalidStrategyException)
            {
                return StatusCodes.Status400BadRequest;
            }

            if (exception is StrategyNotFoundException)
            {
                return StatusCodes.Status404NotFound;
            }

            return StatusCodes.Status500InternalServerError;
        }
    }
}
