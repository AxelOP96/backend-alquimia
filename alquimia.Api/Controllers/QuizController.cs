﻿using alquimia.Services.Interfaces;
using alquimia.Services.Models;
using Microsoft.AspNetCore.Mvc;

namespace alquimia.Api.Controllers
{
    [ApiController]
    [Route("quiz")]
    public class QuizController : ControllerBase
    {
        private readonly IQuizService _quizService;

        public QuizController(IQuizService quizService)
        {
            _quizService = quizService;
        }
        [HttpGet("test")]
        public IActionResult Test() => Ok("Ruta activa");

        [HttpGet("questions")]
        public async Task<IActionResult> ObtenerPreguntas()
        {
            var preguntas = await _quizService.GetQuestionsAsync();
            return Ok(preguntas);
        }

        [HttpPost("respond")]
        public async Task<IActionResult> GuardarRespuestas([FromBody] List<AnswerDTO> respuestas)
        {
            await _quizService.SaveAnswersAsync(respuestas);
            return Ok(new { mensaje = "Respuestas registradas correctamente." });
        }

        [HttpPost("result")]
        public async Task<IActionResult> ObtenerResultado([FromBody] List<AnswerDTO> respuestas)
        {
            var resultado = await _quizService.GetResultAsync(respuestas);
            if (resultado == null)
                return NotFound("No se pudo calcular el resultado.");

            return Ok(resultado);
        }
    }
}