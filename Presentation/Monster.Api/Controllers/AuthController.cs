﻿using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Monster.Application.Features.Auth.Command.Login;
using Monster.Application.Features.Auth.Command.Register;

namespace Monster.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator mediator;

        public AuthController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterCommandRequest request)
        {
            // CQRS komutunu tetikle ve işlemi başlat.
            await mediator.Send(request);

            // Başarı durumunda HTTP 201 Created yanıtı döndür.
            return StatusCode(StatusCodes.Status201Created);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginCommandRequest request)
        {
            // CQRS komutunu tetikle ve işlemi başlat responseye ata
            var response = await mediator.Send(request);

            // Başarı durumunda HTTP 200  responsede döndür
            return StatusCode(StatusCodes.Status200OK, response);
        }
    }
}