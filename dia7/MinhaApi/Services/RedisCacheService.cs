using System.Text.Json;
using StackExchange.Redis;

namespace MinhaApi.Services
{
    /// <summary>
    /// Implementação do serviço de cache usando Redis
    /// </summary>
    public class RedisCacheService : ICacheService
    {
        private readonly IDatabase _database;
        private readonly ILogger<RedisCacheService> _logger;

        public RedisCacheService(IConnectionMultiplexer redis, ILogger<RedisCacheService> logger)
        {
            _database = redis.GetDatabase();
            _logger = logger;
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            try
            {
                var value = await _database.StringGetAsync(key);

                if (value.IsNullOrEmpty)
                    return default;

                return JsonSerializer.Deserialize<T>(value.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar chave {Key} do Redis", key);
                return default;
            }
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
        {
            try
            {
                var serializedValue = JsonSerializer.Serialize(value);

                if (expiration.HasValue)
                    await _database.StringSetAsync(key, serializedValue, expiration.Value);
                else
                    await _database.StringSetAsync(key, serializedValue);

                _logger.LogDebug("Chave {Key} salva no Redis com expiração de {Expiration}", key, expiration);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao salvar chave {Key} no Redis", key);
            }
        }

        public async Task RemoveAsync(string key)
        {
            try
            {
                await _database.KeyDeleteAsync(key);
                _logger.LogDebug("Chave {Key} removida do Redis", key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao remover chave {Key} do Redis", key);
            }
        }

        public async Task<bool> ExistsAsync(string key)
        {
            try
            {
                return await _database.KeyExistsAsync(key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar existência da chave {Key} no Redis", key);
                return false;
            }
        }
    }
}
