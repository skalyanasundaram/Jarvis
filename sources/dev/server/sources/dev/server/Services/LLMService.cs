using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Options;
using Azure.AI.OpenAI;
using Azure;
using System.Linq;
using System;
using OpenAI.Chat;

namespace Jarvis.Services;

/// <summary>
/// Service that interacts with Azure OpenAI via Foundry to perform chat completions.
/// </summary>
public class LLMService
{
    private readonly SecretClient secretClient;
    private readonly KeyVaultOptions keyVaultOptions;
    private readonly FoundryOptions foundryOptions;
    private AzureOpenAIClient? openAIClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="LLMService"/> class.
    /// </summary>
    /// <param name="secretClient">The Azure Key Vault secret client.</param>
    /// <param name="keyVaultOptions">The Key Vault configuration options.</param>
    /// <param name="foundryOptions">The Foundry configuration options.</param>
    public LLMService(SecretClient secretClient, IOptions<KeyVaultOptions> keyVaultOptions, IOptions<FoundryOptions> foundryOptions)
    {
        
        this.secretClient = secretClient;
        this.keyVaultOptions = keyVaultOptions.Value;
        this.foundryOptions = foundryOptions.Value;
    }

    /// <summary>
    /// Sends a chat completion request for the provided prompt.
    /// </summary>
    /// <param name="prompt">The user prompt to complete.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The model-generated content or an empty string when not found.</returns>
    public async Task<string> CompleteAsync(string prompt, CancellationToken cancellationToken = default)
    {
        var endpoint = this.foundryOptions.Endpoint?.TrimEnd('/') ?? "https://firsttestkalyan-resource.cognitiveservices.azure.com";
        var deployment = this.foundryOptions.DeploymentName ?? this.foundryOptions.Model ?? "gpt-5.2-chat";

        if (openAIClient is null)
        {
            if (string.IsNullOrEmpty(this.keyVaultOptions.KeyName))
                throw new InvalidOperationException("KeyName not configured in KeyVault options");

            var secret = await secretClient.GetSecretAsync(this.keyVaultOptions.KeyName, cancellationToken: cancellationToken);
            var apiKey = secret.Value.Value;
            openAIClient = new AzureOpenAIClient(new Uri(endpoint), new AzureKeyCredential(apiKey));
        }

        var options = new ChatCompletionOptions();

        var messages = new List<ChatMessage>
    {
        new SystemChatMessage("You are a helpful assistant."),
        new UserChatMessage(prompt)
    };

        var result = await openAIClient.GetChatClient(deployment).CompleteChatAsync(messages, options, cancellationToken);
        var content = result.Value.Content?.FirstOrDefault()?.Text;

        return content ?? string.Empty;
    }
}
