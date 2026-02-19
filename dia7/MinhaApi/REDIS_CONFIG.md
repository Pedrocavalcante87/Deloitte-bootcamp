# 🔴 Guia de Configuração do Redis na MinhaApi

## 📋 O que foi feito

### 1️⃣ **appsettings.json e appsettings.Development.json**

Adicionada seção de configuração do Redis:

```json
"Redis": {
  "Configuration": "localhost:6379",      // Endereço do Redis
  "InstanceName": "MinhaApi:",            // Prefixo das chaves
  "Password": "redis123",                 // Senha do Redis
  "AbortOnConnectFail": false,            // Não aborta se falhar
  "ConnectTimeout": 5000,                 // Timeout de conexão (5s)
  "ConnectRetry": 3                       // 3 tentativas de reconexão
}
```

### 2️⃣ **Serviços Criados**

#### `ICacheService.cs` - Interface

Define o contrato para operações de cache:

- `GetAsync<T>` - Buscar valor
- `SetAsync<T>` - Salvar valor com expiração opcional
- `RemoveAsync` - Remover valor
- `ExistsAsync` - Verificar se existe

#### `RedisCacheService.cs` - Implementação

Implementa o cache usando Redis com:

- Serialização/deserialização JSON automática
- Logging de erros
- Tratamento de exceções

---

## 🚀 Como Usar

### **Passo 1: Instalar o pacote StackExchange.Redis**

```powershell
cd C:\Users\pdryn\Deloitte-bootcamp\dia7\MinhaApi
dotnet add package StackExchange.Redis
```

### **Passo 2: Registrar o Redis no Program.cs**

Adicione ANTES de `builder.Services.AddControllers()`:

```csharp
using StackExchange.Redis;
using MinhaApi.Services;

// ... código existente ...

// ========== REDIS ==========
var redisConfig = builder.Configuration.GetSection("Redis");
var redisConnection = ConnectionMultiplexer.Connect(new ConfigurationOptions
{
    EndPoints = { redisConfig["Configuration"]! },
    Password = redisConfig["Password"],
    AbortOnConnectFail = bool.Parse(redisConfig["AbortOnConnectFail"]!),
    ConnectTimeout = int.Parse(redisConfig["ConnectTimeout"]!),
    ConnectRetry = int.Parse(redisConfig["ConnectRetry"]!)
});

builder.Services.AddSingleton<IConnectionMultiplexer>(redisConnection);
builder.Services.AddScoped<ICacheService, RedisCacheService>();

builder.Services.AddControllers();
```

### **Passo 3: Usar no Controller**

Exemplo no `LotesMinerioController.cs`:

```csharp
using MinhaApi.Services;

public class LotesMinerioController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly ICacheService _cache;

    public LotesMinerioController(AppDbContext db, ICacheService cache)
    {
        _db = db;
        _cache = cache;
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        // Tenta buscar do cache primeiro
        var cacheKey = $"lote:{id}";
        var cached = await _cache.GetAsync<LoteMinerio>(cacheKey);

        if (cached != null)
            return Ok(cached); // Retorna do cache

        // Se não tiver no cache, busca do banco
        var entity = await _db.LotesMinerio.FirstOrDefaultAsync(x => x.Id == id);

        if (entity == null)
            return NotFound();

        // Salva no cache por 5 minutos
        await _cache.SetAsync(cacheKey, entity, TimeSpan.FromMinutes(5));

        return Ok(entity);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateLoteMinerioDto input)
    {
        // ... validações ...

        _db.SaveChanges();

        // Invalida o cache após atualizar
        await _cache.RemoveAsync($"lote:{id}");

        return NoContent();
    }
}
```

---

## 📊 Exemplos de Uso

### **1. Cache de Listagem**

```csharp
var cacheKey = "lotes:todos";
var cached = await _cache.GetAsync<List<LoteMinerio>>(cacheKey);

if (cached == null)
{
    cached = await _db.LotesMinerio.ToListAsync();
    await _cache.SetAsync(cacheKey, cached, TimeSpan.FromMinutes(10));
}

return Ok(cached);
```

### **2. Cache por Filtro**

```csharp
var cacheKey = $"lotes:status:{status}";
var cached = await _cache.GetAsync<List<LoteMinerio>>(cacheKey);
```

### **3. Invalidação de Cache**

```csharp
// Ao criar/atualizar/deletar
await _cache.RemoveAsync("lotes:todos");
await _cache.RemoveAsync($"lote:{id}");
```

---

## 🧪 Testando o Redis

### **Verificar conexão:**

```powershell
docker exec -it redis_estudos redis-cli
AUTH redis123
PING  # Deve retornar PONG
```

### **Ver chaves salvas:**

```redis
KEYS MinhaApi:*
GET "MinhaApi:lote:1"
```

### **Ver todas as chaves:**

```redis
KEYS *
```

### **Limpar cache:**

```redis
FLUSHDB  # Limpa banco atual
```

---

## 📝 Configurações Importantes

| Propriedade          | Descrição           | Padrão           |
| -------------------- | ------------------- | ---------------- |
| `Configuration`      | Host:Porta do Redis | `localhost:6379` |
| `InstanceName`       | Prefixo das chaves  | `MinhaApi:`      |
| `Password`           | Senha do Redis      | `redis123`       |
| `AbortOnConnectFail` | Abortar se falhar   | `false`          |
| `ConnectTimeout`     | Timeout (ms)        | `5000`           |
| `ConnectRetry`       | Tentativas          | `3`              |

---

## ✅ Benefícios

1. ⚡ **Performance** - Respostas 100x mais rápidas
2. 💰 **Economia** - Menos consultas ao banco
3. 🔄 **Escalabilidade** - Suporta mais requisições
4. 🎯 **Flexibilidade** - Cache configurable por endpoint

---

## 🎯 Próximos Passos

1. ✅ Instalar pacote `StackExchange.Redis`
2. ✅ Registrar Redis no `Program.cs`
3. ✅ Implementar cache nos endpoints
4. ✅ Testar com Insomnia
5. ✅ Monitorar performance

---

**Precisa de ajuda?** Me avise para configurar o `Program.cs` ou implementar cache em endpoints específicos! 🚀
