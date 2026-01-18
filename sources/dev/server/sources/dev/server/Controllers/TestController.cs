using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Jarvis.Services;

namespace Jarvis.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    /// <summary>
    /// Controller to test chat completion via the LLM service.
    /// </summary>
    public class TestController : ControllerBase
    {
        private readonly LLMService llmService;
        private readonly KeyVaultOptions options;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestController"/> class.
        /// </summary>
        /// <param name="llmService">The LLM service.</param>
        /// <param name="options">The key vault options.</param>
        public TestController(LLMService llmService, IOptions<KeyVaultOptions> options)
        {
            this.llmService = llmService;
            this.options = options.Value;
        }

        /// <summary>
        /// Executes a chat completion for a given prompt.
        /// </summary>
        /// <param name="prompt">The user prompt.</param>
        /// <returns>An action result containing the completion output.</returns>
        [HttpGet("chat")]
        public async Task<IActionResult> Chat([FromQuery] string prompt)
        {
            if (string.IsNullOrEmpty(prompt))
                return BadRequest("prompt is required");

            try
            {
                var result = await llmService.CompleteAsync(prompt);
                return Ok(new { result });
            }
            catch (Exception ex)
            {
                return Problem(ex.Message, statusCode: 500);
            }
        }
    }
}
