using MinhaApi.Dtos;

namespace MinhaApi.Services;

/// <summary>
/// Producer para enfileirar lotes no Redis Streams
/// </summary>
public interface ILoteQueueProducer
{
    Task<string> EnfileirarAsync(ProcessarLoteMessage mensagem);
}
