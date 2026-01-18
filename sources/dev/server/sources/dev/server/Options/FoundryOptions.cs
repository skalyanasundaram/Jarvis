namespace Jarvis
{
    /// <summary>
    /// Configuration options for Azure Foundry endpoint and model selection.
    /// </summary>
    public class FoundryOptions
    {
        /// <summary>
        /// The base endpoint URL of the Azure Foundry resource.
        /// </summary>
        public string? Endpoint { get; set; }
        /// <summary>
        /// The default model name used when a deployment is not specified.
        /// </summary>
        public string? Model { get; set; }
        /// <summary>
        /// The deployment name of the model to target.
        /// </summary>
        public string? DeploymentName { get; set; }
    }
}
