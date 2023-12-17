//using Microsoft.AspNetCore.Mvc;
//using shoppingify_backend.Helpers.CustomExceptions;
//using System.Net;
//using System.Text.Json;
//using System;
//using Microsoft.AspNetCore.Diagnostics;

//namespace shoppingify_backend.Controllers
//{
//    [ApiController]
//    public class ErrorController : Controller
//    {
//        [HttpGet]
//        [Route("/Error")]
//        public IActionResult Error()
//        {
//            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
//            var exception = context?.Error; // Your exception
//            var code = exception switch
//            {
//                NotFoundException => NotFoundException.ErrorCode,
//                ForbiddenException => ForbiddenException.ErrorCode,
//                ValidationException => ValidationException.ErrorCode,
//                BadRequestException => BadRequestException.ErrorCode,
//                _ => HttpStatusCode.InternalServerError

//            };

//            Response.StatusCode = (int)code;
//            return Problem(detail: exception?.Message, statusCode: (int)code);
//        }
//    }
//}
