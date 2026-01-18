namespace Jarvis
{
    /// <summary>
    /// Configuration options for accessing Azure Key Vault.
    /// </summary>
    public class KeyVaultOptions
    {
        /// <summary>
        /// The name of the Azure Key Vault instance.
        /// </summary>
        public string? VaultName { get; set; }
        /// <summary>
        /// The name of the secret that contains the API key.
        /// </summary>
        public string? KeyName { get; set; }
    }
}
