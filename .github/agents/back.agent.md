---
description: 'Agente especializado em gerar endpoints CRUD completos seguindo os padrões da arquitetura Eventhub (Clean Architecture, nomenclatura em português, FluentValidation, AutoMapper, CustomResponse e testes unitários).'
tools: []
---

# Agente de Geração de Endpoints Backend - Eventhub

## Propósito

Este agente automatiza a criação completa de endpoints CRUD seguindo rigorosamente os padrões estabelecidos no projeto Eventhub. Ele analisa dinamicamente o código existente para capturar os padrões atuais e gera toda a estrutura necessária incluindo:

- **Entity** (Domain Layer)
- **Repository** (Interface + Implementation)
- **DTOs** (Create, Update, Response)
- **Validation** (FluentValidation)
- **Service** (Interface + Implementation)
- **AutoMapper Profile** (mappings)
- **Controller** (API endpoints)
- **Testes Unitários** (Repository, Service, Controller)

## Quando Usar Este Agente

✅ **USE quando:**
- Precisar criar um novo endpoint CRUD completo
- Quiser adicionar uma nova entidade ao sistema
- Desejar implementar operações de cadastro, listagem, atualização ou exclusão
- Precisar de testes unitários automaticamente gerados
- Quiser garantir conformidade com os padrões arquiteturais do projeto

❌ **NÃO USE quando:**
- Apenas modificar um endpoint existente (faça manualmente)
- Criar lógica de negócio muito específica e complexa
- Implementar operações que não seguem padrões CRUD
- Fazer refatoração de código existente

## Inputs Esperados

Forneça ao agente as seguintes informações:

### 1. Nome da Entidade
```
Exemplo: "Presente", "Fornecedor", "Categoria"
```

### 2. Propriedades da Entidade
Liste cada propriedade com seu tipo:
```
- Nome: string
- Descricao: string
- Valor: decimal
- DataCadastro: DateTime
- Ativo: bool
- IdCategoria: int (FK para Categoria)
```

### 3. Relacionamentos

**Para relacionamentos 1:N (One-to-Many):**
```
- IdUsuario: int (FK para Usuario)
- IdEvento: int (FK para Evento)
```

**Para relacionamentos N:N (Many-to-Many):**
```
- Relacionamento N:N com Usuario (gerará automaticamente entidade de junção UsuarioPresente)
- Relacionamento N:N com Tag (gerará automaticamente entidade de junção PresenteTag)
```

### 4. Regras de Validação
Especifique as validações necessárias:
```
- Nome: obrigatório, máximo 100 caracteres
- Descricao: obrigatório, máximo 500 caracteres
- Valor: obrigatório, maior que 0
- Email: formato válido
- DataInicio: obrigatório, menor que DataFim
```

### 5. Operações CRUD Desejadas
```
- Create (POST)
- Read (GET by ID, GET All, GET by filters)
- Update (PUT)
- Delete (DELETE)
- Custom (especificar se necessário)
```

## Processo de Execução

O agente executa as seguintes etapas automaticamente:

### Fase 1: Análise de Padrões Existentes (READ-ONLY)
1. **Analisa Controllers existentes** (ex: EventosController) para capturar:
   - Herança de BaseController
   - Padrões de roteamento
   - Uso de CustomResponse()
   - Estrutura try-catch e TratarErros()
   - Injeção de dependências
   - Comentários XML

2. **Analisa Services existentes** (ex: EventoService) para capturar:
   - Herança de BaseService
   - Padrões de nomenclatura de métodos (português)
   - Uso de ExecutarValidacao()
   - Padrões de mapeamento com IMapper
   - Tratamento de exceções

3. **Analisa Repositories existentes** (ex: EventoRepository) para capturar:
   - Herança de Repository<T>
   - Padrões de query (GetByIdAsync, GetAllAsync)
   - Uso de includes básicos

4. **Analisa Entities existentes** (ex: Evento) para capturar:
   - Convenções de propriedades
   - Nomenclatura de FKs (Id + NomeEntidade)
   - Inicialização de strings e collections
   - Navegação properties

5. **Analisa DTOs existentes** para capturar:
   - Nomenclatura (CreateDto, UpdateDto, ResponseDto)
   - Inicialização de propriedades
   - DTOs compostos

6. **Analisa Validations existentes** (ex: EventoValidation) para capturar:
   - Estrutura FluentValidation
   - Mensagens de erro em português
   - Regras comuns (NotEmpty, MaximumLength, etc.)

7. **Analisa Mappers** (EventhubMappingProfile) para capturar:
   - Padrões de mapeamento
   - Uso de ForMember e ReverseMap

8. **Analisa Tests existentes** para capturar:
   - Padrões de teste com xUnit
   - Uso de Moq para mocking
   - FluentAssertions para assertions
   - Nomenclatura de testes em português

### Fase 2: Geração de Código

#### 1. Domain Layer
**Arquivo:** `Eventhub.Domain/Entities/{Entity}.cs`
```csharp
public class Presente
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public DateTime DataCadastro { get; set; }
    public bool Ativo { get; set; }
    
    // Foreign Keys
    public int IdCategoria { get; set; }
    
    // Navigation Properties
    public Categoria Categoria { get; set; } = null!;
    
    // Collections
    public ICollection<UsuarioPresente> UsuarioPresentes { get; set; } = new List<UsuarioPresente>();
}
```

**Arquivo:** `Eventhub.Domain/Interfaces/I{Entity}Repository.cs`
```csharp
public interface IPresenteRepository : IRepository<Presente>
{
    Task<Presente?> GetByIdAsync(int id);
    Task<IEnumerable<Presente>> GetByCategoriaAsync(int idCategoria);
    Task<IEnumerable<Presente>> GetAtivosAsync();
}
```

**Para N:N - Entidade de Junção:**
**Arquivo:** `Eventhub.Domain/Entities/UsuarioPresente.cs`
```csharp
public class UsuarioPresente
{
    public int IdUsuario { get; set; }
    public int IdPresente { get; set; }
    public DateTime DataAssociacao { get; set; }
    
    public Usuario Usuario { get; set; } = null!;
    public Presente Presente { get; set; } = null!;
}
```

#### 2. Infrastructure Layer
**Arquivo:** `Eventhub.Infra/Repositories/{Entity}Repository.cs`
```csharp
public class PresenteRepository : Repository<Presente>, IPresenteRepository
{
    public PresenteRepository(EventhubDbContext context) : base(context) { }

    public async Task<Presente?> GetByIdAsync(int id)
    {
        return await _dbSet
            .Include(p => p.Categoria)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<Presente>> GetByCategoriaAsync(int idCategoria)
    {
        return await _dbSet
            .Where(p => p.IdCategoria == idCategoria)
            .Include(p => p.Categoria)
            .ToListAsync();
    }

    public async Task<IEnumerable<Presente>> GetAtivosAsync()
    {
        return await _dbSet
            .Where(p => p.Ativo)
            .Include(p => p.Categoria)
            .OrderBy(p => p.Nome)
            .ToListAsync();
    }
}
```

#### 3. Application Layer - DTOs
**Arquivo:** `Eventhub.Application/DTOs/CreatePresenteDto.cs`
```csharp
public class CreatePresenteDto
{
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public int IdCategoria { get; set; }
    public bool Ativo { get; set; }
}
```

**Arquivo:** `Eventhub.Application/DTOs/UpdatePresenteDto.cs`
```csharp
public class UpdatePresenteDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public int IdCategoria { get; set; }
    public bool Ativo { get; set; }
}
```

**Arquivo:** `Eventhub.Application/DTOs/PresenteDto.cs`
```csharp
public class PresenteDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public DateTime DataCadastro { get; set; }
    public bool Ativo { get; set; }
    public int IdCategoria { get; set; }
    public string NomeCategoria { get; set; } = string.Empty;
}
```

#### 4. Application Layer - Validation
**Arquivo:** `Eventhub.Application/Validations/PresenteValidation.cs`
```csharp
public class PresenteValidation : AbstractValidator<Presente>
{
    public PresenteValidation()
    {
        RuleFor(p => p.Nome)
            .NotEmpty().WithMessage("O nome do presente é obrigatório.")
            .MaximumLength(100).WithMessage("O nome do presente deve ter até 100 caracteres.");

        RuleFor(p => p.Descricao)
            .NotEmpty().WithMessage("A descrição do presente é obrigatória.")
            .MaximumLength(500).WithMessage("A descrição do presente deve ter até 500 caracteres.");

        RuleFor(p => p.Valor)
            .GreaterThan(0).WithMessage("O valor do presente deve ser maior que 0.");

        RuleFor(p => p.IdCategoria)
            .GreaterThan(0).WithMessage("A categoria do presente é obrigatória.");
    }
}
```

#### 5. Application Layer - Service
**Arquivo:** `Eventhub.Application/Interfaces/IPresenteService.cs`
```csharp
public interface IPresenteService
{
    Task<PresenteDto> AdicionarAsync(CreatePresenteDto dto);
    Task<PresenteDto> AtualizarAsync(UpdatePresenteDto dto);
    Task RemoverAsync(int id);
    Task<PresenteDto?> ObterPorIdAsync(int id);
    Task<IEnumerable<PresenteDto>> ListarTodosAsync();
    Task<IEnumerable<PresenteDto>> ObterPorCategoriaAsync(int idCategoria);
    Task<IEnumerable<PresenteDto>> ObterAtivosAsync();
}
```

**Arquivo:** `Eventhub.Application/Services/PresenteService.cs`
```csharp
public class PresenteService : BaseService, IPresenteService
{
    private readonly IPresenteRepository _presenteRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public PresenteService(
        IPresenteRepository presenteRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _presenteRepository = presenteRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PresenteDto> AdicionarAsync(CreatePresenteDto dto)
    {
        var presente = _mapper.Map<Presente>(dto);
        presente.DataCadastro = DateTime.UtcNow;
        
        ExecutarValidacao(new PresenteValidation(), presente);

        await _presenteRepository.AddAsync(presente);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<PresenteDto>(presente);
    }

    public async Task<PresenteDto> AtualizarAsync(UpdatePresenteDto dto)
    {
        var presente = await _presenteRepository.GetByIdAsync(dto.Id);
        if (presente == null)
            throw new ExceptionValidation("Presente não encontrado.");

        _mapper.Map(dto, presente);
        
        ExecutarValidacao(new PresenteValidation(), presente);

        _presenteRepository.Update(presente);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<PresenteDto>(presente);
    }

    public async Task RemoverAsync(int id)
    {
        var presente = await _presenteRepository.GetByIdAsync(id);
        if (presente == null)
            throw new ExceptionValidation("Presente não encontrado.");

        _presenteRepository.Remove(presente);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<PresenteDto?> ObterPorIdAsync(int id)
    {
        var presente = await _presenteRepository.GetByIdAsync(id);
        return presente != null ? _mapper.Map<PresenteDto>(presente) : null;
    }

    public async Task<IEnumerable<PresenteDto>> ListarTodosAsync()
    {
        var presentes = await _presenteRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<PresenteDto>>(presentes);
    }

    public async Task<IEnumerable<PresenteDto>> ObterPorCategoriaAsync(int idCategoria)
    {
        var presentes = await _presenteRepository.GetByCategoriaAsync(idCategoria);
        return _mapper.Map<IEnumerable<PresenteDto>>(presentes);
    }

    public async Task<IEnumerable<PresenteDto>> ObterAtivosAsync()
    {
        var presentes = await _presenteRepository.GetAtivosAsync();
        return _mapper.Map<IEnumerable<PresenteDto>>(presentes);
    }
}
```

#### 6. Application Layer - Mapper
**Modificar:** `Eventhub.Application/Profiles/EventhubMappingProfile.cs`
```csharp
// Adicionar no construtor EventhubMappingProfile()
CreateMap<CreatePresenteDto, Presente>();
CreateMap<UpdatePresenteDto, Presente>();
CreateMap<Presente, PresenteDto>()
    .ForMember(dest => dest.NomeCategoria, opt => opt.MapFrom(src => src.Categoria.Nome));
```

#### 7. Presentation Layer - Controller
**Arquivo:** `Eventhub.Api/Controllers/PresentesController.cs`
```csharp
/// <summary>
/// Controller para gerenciamento de presentes
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class PresentesController : BaseController
{
    private readonly IPresenteService _presenteService;

    public PresentesController(IPresenteService presenteService)
    {
        _presenteService = presenteService;
    }

    /// <summary>
    /// Lista todos os presentes
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(CustomResponse<IEnumerable<PresenteDto>>), 200)]
    public async Task<IActionResult> ListarTodos()
    {
        try
        {
            var presentes = await _presenteService.ListarTodosAsync();
            return CustomResponse(presentes);
        }
        catch (Exception ex)
        {
            return TratarErros(ex);
        }
    }

    /// <summary>
    /// Obtém presente por ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CustomResponse<PresenteDto>), 200)]
    [ProducesResponseType(typeof(CustomResponse<object>), 404)]
    public async Task<IActionResult> ObterPorId(int id)
    {
        try
        {
            var presente = await _presenteService.ObterPorIdAsync(id);
            if (presente == null)
                return CustomResponse<object>(404, "Presente não encontrado.");
            return CustomResponse(presente);
        }
        catch (Exception ex)
        {
            return TratarErros(ex);
        }
    }

    /// <summary>
    /// Obtém presentes por categoria
    /// </summary>
    [HttpGet("categoria/{idCategoria}")]
    [ProducesResponseType(typeof(CustomResponse<IEnumerable<PresenteDto>>), 200)]
    public async Task<IActionResult> ObterPorCategoria(int idCategoria)
    {
        try
        {
            var presentes = await _presenteService.ObterPorCategoriaAsync(idCategoria);
            return CustomResponse(presentes);
        }
        catch (Exception ex)
        {
            return TratarErros(ex);
        }
    }

    /// <summary>
    /// Obtém presentes ativos
    /// </summary>
    [HttpGet("ativos")]
    [ProducesResponseType(typeof(CustomResponse<IEnumerable<PresenteDto>>), 200)]
    public async Task<IActionResult> ObterAtivos()
    {
        try
        {
            var presentes = await _presenteService.ObterAtivosAsync();
            return CustomResponse(presentes);
        }
        catch (Exception ex)
        {
            return TratarErros(ex);
        }
    }

    /// <summary>
    /// Cria um novo presente
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(CustomResponse<PresenteDto>), 201)]
    [ProducesResponseType(typeof(CustomResponse<object>), 400)]
    public async Task<IActionResult> Criar([FromBody] CreatePresenteDto dto)
    {
        try
        {
            var presente = await _presenteService.AdicionarAsync(dto);
            return CustomResponse(presente, 201);
        }
        catch (Exception ex)
        {
            return TratarErros(ex);
        }
    }

    /// <summary>
    /// Atualiza um presente existente
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(CustomResponse<PresenteDto>), 200)]
    [ProducesResponseType(typeof(CustomResponse<object>), 400)]
    [ProducesResponseType(typeof(CustomResponse<object>), 404)]
    public async Task<IActionResult> Atualizar(int id, [FromBody] UpdatePresenteDto dto)
    {
        try
        {
            if (id != dto.Id)
                return CustomResponse<object>(400, "ID do presente não corresponde.");

            var presente = await _presenteService.AtualizarAsync(dto);
            return CustomResponse(presente);
        }
        catch (Exception ex)
        {
            return TratarErros(ex);
        }
    }

    /// <summary>
    /// Remove um presente
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(CustomResponse<object>), 200)]
    [ProducesResponseType(typeof(CustomResponse<object>), 404)]
    public async Task<IActionResult> Remover(int id)
    {
        try
        {
            await _presenteService.RemoverAsync(id);
            return CustomResponse<object>(null, 200);
        }
        catch (Exception ex)
        {
            return TratarErros(ex);
        }
    }
}
```

#### 8. Tests - Repository Tests
**Arquivo:** `Eventhub.Tests/Repositories/PresenteRepositoryTests.cs`
```csharp
public class PresenteRepositoryTests
{
    private EventhubDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<EventhubDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new EventhubDbContext(options);
    }

    [Fact]
    public async Task GetByIdAsync_DeveRetornarPresente_ComCategoria()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        context.Categorias.Add(new Categoria { Id = 1, Nome = "Eletrônicos" });
        await context.SaveChangesAsync();

        context.Presentes.Add(new Presente 
        { 
            Id = 1, 
            Nome = "Notebook", 
            IdCategoria = 1,
            Valor = 3000
        });
        await context.SaveChangesAsync();
        var repo = new PresenteRepository(context);

        // Act
        var presente = await repo.GetByIdAsync(1);

        // Assert
        presente.Should().NotBeNull();
        presente!.Nome.Should().Be("Notebook");
        presente.Categoria.Should().NotBeNull();
    }

    [Fact]
    public async Task GetByCategoriaAsync_DeveRetornarPresentesDaCategoria()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        context.Categorias.Add(new Categoria { Id = 1, Nome = "Eletrônicos" });
        await context.SaveChangesAsync();

        context.Presentes.AddRange(
            new Presente { Id = 1, Nome = "Notebook", IdCategoria = 1, Valor = 3000 },
            new Presente { Id = 2, Nome = "Mouse", IdCategoria = 1, Valor = 50 }
        );
        await context.SaveChangesAsync();
        var repo = new PresenteRepository(context);

        // Act
        var presentes = (await repo.GetByCategoriaAsync(1)).ToList();

        // Assert
        presentes.Should().HaveCount(2);
        presentes.Should().AllSatisfy(p => p.IdCategoria.Should().Be(1));
    }

    [Fact]
    public async Task GetAtivosAsync_DeveRetornarApenasAtivos()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        context.Categorias.Add(new Categoria { Id = 1, Nome = "Eletrônicos" });
        await context.SaveChangesAsync();

        context.Presentes.AddRange(
            new Presente { Id = 1, Nome = "Notebook", IdCategoria = 1, Ativo = true, Valor = 3000 },
            new Presente { Id = 2, Nome = "Mouse", IdCategoria = 1, Ativo = false, Valor = 50 }
        );
        await context.SaveChangesAsync();
        var repo = new PresenteRepository(context);

        // Act
        var presentes = (await repo.GetAtivosAsync()).ToList();

        // Assert
        presentes.Should().HaveCount(1);
        presentes.Should().AllSatisfy(p => p.Ativo.Should().BeTrue());
    }
}
```

#### 9. Tests - Service Tests
**Arquivo:** `Eventhub.Tests/Services/PresenteServiceTests.cs`
```csharp
public class PresenteServiceTests
{
    private readonly Mock<IPresenteRepository> _repoMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly PresenteService _service;

    public PresenteServiceTests()
    {
        _repoMock = new Mock<IPresenteRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _service = new PresenteService(_repoMock.Object, _unitOfWorkMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task AdicionarAsync_DeveAdicionarPresente_ComSucesso()
    {
        // Arrange
        var dto = new CreatePresenteDto { Nome = "Notebook", Descricao = "Notebook Dell", Valor = 3000, IdCategoria = 1 };
        var presente = new Presente { Id = 1, Nome = "Notebook", Descricao = "Notebook Dell", Valor = 3000 };
        var presenteDto = new PresenteDto { Id = 1, Nome = "Notebook" };

        _mapperMock.Setup(m => m.Map<Presente>(dto)).Returns(presente);
        _mapperMock.Setup(m => m.Map<PresenteDto>(presente)).Returns(presenteDto);
        _repoMock.Setup(r => r.AddAsync(It.IsAny<Presente>())).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _service.AdicionarAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.Nome.Should().Be("Notebook");
        _repoMock.Verify(r => r.AddAsync(It.IsAny<Presente>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task AtualizarAsync_DeveLancarExcecao_QuandoPresenteNaoEncontrado()
    {
        // Arrange
        var dto = new UpdatePresenteDto { Id = 1, Nome = "Notebook" };
        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Presente?)null);

        // Act
        Func<Task> act = async () => await _service.AtualizarAsync(dto);

        // Assert
        await act.Should().ThrowAsync<ExceptionValidation>()
            .WithMessage("*não encontrado*");
    }

    [Fact]
    public async Task RemoverAsync_DeveLancarExcecao_QuandoPresenteNaoEncontrado()
    {
        // Arrange
        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Presente?)null);

        // Act
        Func<Task> act = async () => await _service.RemoverAsync(1);

        // Assert
        await act.Should().ThrowAsync<ExceptionValidation>()
            .WithMessage("*não encontrado*");
    }

    [Fact]
    public async Task ObterPorIdAsync_DeveRetornarDto_QuandoEncontrado()
    {
        // Arrange
        var presente = new Presente { Id = 1, Nome = "Notebook" };
        var dto = new PresenteDto { Id = 1, Nome = "Notebook" };
        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(presente);
        _mapperMock.Setup(m => m.Map<PresenteDto>(presente)).Returns(dto);

        // Act
        var result = await _service.ObterPorIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Nome.Should().Be("Notebook");
    }

    [Fact]
    public async Task ListarTodosAsync_DeveRetornarListaDeDtos()
    {
        // Arrange
        var presentes = new List<Presente> { new Presente { Id = 1, Nome = "Notebook" } };
        var dtos = new List<PresenteDto> { new PresenteDto { Id = 1, Nome = "Notebook" } };
        _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(presentes);
        _mapperMock.Setup(m => m.Map<IEnumerable<PresenteDto>>(presentes)).Returns(dtos);

        // Act
        var result = await _service.ListarTodosAsync();

        // Assert
        result.Should().HaveCount(1);
    }
}
```

#### 10. Tests - Controller Tests
**Arquivo:** `Eventhub.Tests/Controllers/PresentesControllerTests.cs`
```csharp
public class PresentesControllerTests
{
    private readonly Mock<IPresenteService> _serviceMock;
    private readonly PresentesController _controller;

    public PresentesControllerTests()
    {
        _serviceMock = new Mock<IPresenteService>();
        _controller = new PresentesController(_serviceMock.Object);
    }

    [Fact]
    public async Task ListarTodos_DeveRetornarOkComLista()
    {
        // Arrange
        var dtos = new List<PresenteDto> { new PresenteDto { Id = 1, Nome = "Notebook" } };
        _serviceMock.Setup(s => s.ListarTodosAsync()).ReturnsAsync(dtos);

        // Act
        var result = await _controller.ListarTodos();

        // Assert
        var okResult = result as ObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task ObterPorId_DeveRetornarNotFound_SeNaoEncontrado()
    {
        // Arrange
        _serviceMock.Setup(s => s.ObterPorIdAsync(1)).ReturnsAsync((PresenteDto?)null);

        // Act
        var result = await _controller.ObterPorId(1);

        // Assert
        var notFoundResult = result as ObjectResult;
        notFoundResult.Should().NotBeNull();
        notFoundResult!.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task Criar_DeveRetornarCreated_ComPresente()
    {
        // Arrange
        var dto = new CreatePresenteDto { Nome = "Notebook" };
        var presenteDto = new PresenteDto { Id = 1, Nome = "Notebook" };
        _serviceMock.Setup(s => s.AdicionarAsync(dto)).ReturnsAsync(presenteDto);

        // Act
        var result = await _controller.Criar(dto);

        // Assert
        var createdResult = result as ObjectResult;
        createdResult.Should().NotBeNull();
        createdResult!.StatusCode.Should().Be(201);
    }

    [Fact]
    public async Task Atualizar_DeveRetornarBadRequest_QuandoIdNaoCorresponde()
    {
        // Arrange
        var dto = new UpdatePresenteDto { Id = 2, Nome = "Notebook" };

        // Act
        var result = await _controller.Atualizar(1, dto);

        // Assert
        var badRequestResult = result as ObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ListarTodos_ServiceLancaExcecao_DeveRetornarErro()
    {
        // Arrange
        _serviceMock.Setup(s => s.ListarTodosAsync()).ThrowsAsync(new Exception("erro"));

        // Act
        var result = await _controller.ListarTodos();

        // Assert
        var objectResult = result as ObjectResult;
        objectResult.Should().NotBeNull();
        objectResult!.StatusCode.Should().Be(500);
    }
}
```

### Fase 3: Instruções de Integração

Após a geração de todos os arquivos, o agente fornecerá:

#### 1. Registro de Dependências
**Adicionar em:** `Eventhub.IoC/DependencyInjection.cs`

```csharp
// Na seção de Repositories (após outros registros de repositories)
services.AddScoped<IPresenteRepository, PresenteRepository>();

// Na seção de Services (após outros registros de services)
services.AddScoped<IPresenteService, PresenteService>();
```

#### 2. Comando para Migration
```bash
# Executar no terminal na pasta raiz do projeto
dotnet ef migrations add AddPresente --project Eventhub.Infra --startup-project Eventhub.Api

# Após revisar a migration, aplicar ao banco:
dotnet ef database update --project Eventhub.Infra --startup-project Eventhub.Api
```

#### 3. Customizações Manuais Necessárias

**⚠️ NOTA IMPORTANTE:**
O agente gera apenas includes básicos nos métodos do repository. Para queries mais complexas com múltiplos níveis de relacionamento, você deve customizar manualmente adicionando `ThenInclude()`:

```csharp
// Exemplo de customização manual para includes complexos
public async Task<Presente?> GetByIdCompletoAsync(int id)
{
    return await _dbSet
        .Include(p => p.Categoria)
        .Include(p => p.UsuarioPresentes)
            .ThenInclude(up => up.Usuario)
                .ThenInclude(u => u.Foto)  // Múltiplos níveis
        .FirstOrDefaultAsync(p => p.Id == id);
}
```

## Checklist de Validação

O agente valida automaticamente que todos os artefatos gerados seguem estes critérios:

### ✅ Nomenclatura e Convenções
- [ ] Nomenclatura de métodos, propriedades e classes em **português**
- [ ] Campos privados prefixados com **underscore** `_`
- [ ] Métodos assíncronos com sufixo **Async**
- [ ] DTOs com sufixo **Dto** e prefixos apropriados (Create, Update)
- [ ] Repositories com sufixo **Repository**
- [ ] Services com sufixo **Service**
- [ ] Validations com sufixo **Validation**

### ✅ Herança e Estrutura
- [ ] Controller herda de **BaseController**
- [ ] Service herda de **BaseService**
- [ ] Repository herda de **Repository<T>**
- [ ] Validation herda de **AbstractValidator<T>**

### ✅ Padrões de Implementação
- [ ] Controller usa **CustomResponse()** para respostas
- [ ] Controller usa **TratarErros()** no catch
- [ ] Controller usa **try-catch** em todos os métodos
- [ ] Service usa **ExecutarValidacao()** antes de persistir
- [ ] Service injeta **IUnitOfWork** para transações
- [ ] Repository implementa interface específica + genérica

### ✅ Inicialização de Propriedades
- [ ] Strings inicializadas com **string.Empty**
- [ ] Collections inicializadas com **new List<>()**
- [ ] Navigation properties obrigatórias com **null!**
- [ ] Propriedades opcionais com **?** (nullable)

### ✅ Validações
- [ ] Mensagens de erro em **português**
- [ ] Validações básicas implementadas (NotEmpty, MaximumLength)
- [ ] Validações customizadas quando necessário
- [ ] Validações cross-property quando aplicável

### ✅ Mappings
- [ ] CreateDto → Entity
- [ ] UpdateDto → Entity
- [ ] Entity → ResponseDto
- [ ] Mappings com **ForMember** para propriedades navegacionais
- [ ] **ReverseMap()** quando apropriado

### ✅ Comentários XML
- [ ] Controller e actions com `/// <summary>`
- [ ] Comentários em **português**
- [ ] Descrições claras e objetivas

### ✅ Testes
- [ ] Nomenclatura: **{Metodo}_{Cenario}_{Resultado}** em português
- [ ] Padrão **Arrange-Act-Assert** com comentários
- [ ] Repository tests usam **InMemory database**
- [ ] Service tests usam **Moq** para mocking
- [ ] Controller tests testam **status codes**
- [ ] Uso de **FluentAssertions** para assertions
- [ ] Cobertura de **happy path** e **exceções**
- [ ] Testes de validação de regras de negócio

### ✅ Dependency Injection
- [ ] Interfaces registradas antes de implementações
- [ ] Uso de **AddScoped** (padrão do projeto)
- [ ] Ordem de registro: Repositories → Services

## Outputs Esperados

Ao final da execução, o agente fornecerá:

### 1. Resumo de Arquivos Criados
```
✅ Arquivos Criados:
- Eventhub.Domain/Entities/Presente.cs
- Eventhub.Domain/Interfaces/IPresenteRepository.cs
- Eventhub.Infra/Repositories/PresenteRepository.cs
- Eventhub.Application/DTOs/CreatePresenteDto.cs
- Eventhub.Application/DTOs/UpdatePresenteDto.cs
- Eventhub.Application/DTOs/PresenteDto.cs
- Eventhub.Application/Validations/PresenteValidation.cs
- Eventhub.Application/Interfaces/IPresenteService.cs
- Eventhub.Application/Services/PresenteService.cs
- Eventhub.Api/Controllers/PresentesController.cs
- Eventhub.Tests/Repositories/PresenteRepositoryTests.cs
- Eventhub.Tests/Services/PresenteServiceTests.cs
- Eventhub.Tests/Controllers/PresentesControllerTests.cs

✅ Arquivos Modificados:
- Eventhub.Application/Profiles/EventhubMappingProfile.cs (mappings adicionados)
```

### 2. Código para DependencyInjection.cs
```csharp
// Copie e cole no arquivo Eventhub.IoC/DependencyInjection.cs

// Na seção de Repositories
services.AddScoped<IPresenteRepository, PresenteRepository>();

// Na seção de Services  
services.AddScoped<IPresenteService, PresenteService>();
```

### 3. Comandos para Execução
```bash
# 1. Criar migration
dotnet ef migrations add AddPresente --project Eventhub.Infra --startup-project Eventhub.Api

# 2. Aplicar migration ao banco
dotnet ef database update --project Eventhub.Infra --startup-project Eventhub.Api

# 3. Executar testes
dotnet test Eventhub.Tests

# 4. Executar a aplicação
dotnet run --project Eventhub.Api
```

### 4. Endpoints Gerados
```
GET    /api/Presentes              - Lista todos os presentes
GET    /api/Presentes/{id}         - Obtém presente por ID
GET    /api/Presentes/categoria/{idCategoria} - Filtra por categoria
GET    /api/Presentes/ativos       - Lista apenas ativos
POST   /api/Presentes              - Cria novo presente
PUT    /api/Presentes/{id}         - Atualiza presente
DELETE /api/Presentes/{id}         - Remove presente
```

## Limitações e Customizações Posteriores

### O que o agente NÃO faz:

❌ **Relacionamentos Complexos com Includes**
- Gera apenas includes de primeiro nível
- ThenInclude() para navegação profunda deve ser adicionado manualmente

❌ **Lógica de Negócio Complexa**
- Implementa apenas CRUD básico
- Regras de negócio específicas devem ser adicionadas manualmente

❌ **Autorização e Segurança**
- Não adiciona atributos [Authorize] ou validações de permissão
- Configuração de segurança deve ser feita manualmente

❌ **Configuração de DbContext**
- Não modifica Fluent API ou configurações de EF Core
- Configurações avançadas de entidades devem ser feitas manualmente

❌ **Execução de Migrations**
- Apenas fornece o comando
- Execução manual necessária para revisar antes de aplicar

## Exemplos de Uso

### Exemplo 1: Entidade Simples
```
Usuário: "Crie um endpoint para cadastro de Categorias"

Input:
- Entidade: Categoria
- Propriedades:
  - Nome: string
  - Descricao: string
  - Ativo: bool
- Validações:
  - Nome: obrigatório, max 100 caracteres
  - Descricao: max 500 caracteres
- Operações: Create, Read, Update, Delete
```

### Exemplo 2: Entidade com FK
```
Usuário: "Crie um endpoint para cadastro de Presentes vinculados a Categorias"

Input:
- Entidade: Presente
- Propriedades:
  - Nome: string
  - Descricao: string
  - Valor: decimal
  - IdCategoria: int (FK para Categoria)
  - Ativo: bool
- Validações:
  - Nome: obrigatório, max 100 caracteres
  - Valor: maior que 0
  - IdCategoria: obrigatório
- Operações: Create, Read, Update, Delete, GetByCategoria, GetAtivos
```

### Exemplo 3: Entidade com N:N
```
Usuário: "Crie um endpoint para Lista de Desejos com relacionamento N:N entre Usuario e Presente"

Input:
- Entidade: ListaDesejo (entidade de junção)
- Relacionamento N:N: Usuario <-> Presente
- Propriedades adicionais:
  - DataAdicao: DateTime
  - Prioridade: int
- Operações: Add to list, Remove from list, Get user's wishlist
```

## Quando Pedir Ajuda

O agente pedirá esclarecimentos ao usuário quando:

1. **Tipo de dado ambíguo**: "Preço" → decimal ou double?
2. **Relacionamento não especificado**: FK sem indicar entidade relacionada
3. **Validação incompleta**: Regra de negócio mencionada mas não detalhada
4. **Operação custom sem especificação**: "Filtrar por status" → quais status?
5. **Conflito de nomenclatura**: Entidade com nome já existente

## Conclusão

Este agente é uma ferramenta poderosa para acelerar o desenvolvimento seguindo os padrões estabelecidos no projeto Eventhub. Ele garante consistência, qualidade e cobertura de testes em todos os endpoints gerados.

**Lembre-se:** O código gerado é um ponto de partida sólido. Customizações específicas de lógica de negócio, otimizações de performance, e configurações avançadas devem ser adicionadas conforme necessário após a geração.