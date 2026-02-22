namespace MinhaApi.Services
{
    /// <summary>
    /// Interface para serviço de cache Redis
    /// </summary>
    public interface ICacheService
    {
        /// <summary>
        /// Obtém um valor do cache
        /// </summary>
        Task<T?> GetAsync<T>(string key);

        /// <summary>
        /// Define um valor no cache
        /// </summary>
        Task SetAsync<T>(string key, T value, TimeSpan? expiration = null);

        /// <summary>
        /// Remove um valor do cache
        /// </summary>
        Task RemoveAsync(string key);

        /// <summary>
        /// Verifica se uma chave existe no cache
        /// </summary>
        Task<bool> ExistsAsync(string key);
    }
}
