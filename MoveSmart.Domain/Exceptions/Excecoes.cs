namespace MoveSmart.Domain.Exceptions;

/// <summary>Violação de uma regra de negócio ou de validação do domínio (HTTP 400).</summary>
public class DominioException : Exception
{
    public DominioException(string mensagem) : base(mensagem) { }
}

/// <summary>Recurso inexistente (HTTP 404).</summary>
public class NaoEncontradoException : DominioException
{
    public NaoEncontradoException(string entidade, object chave)
        : base($"{entidade} '{chave}' não encontrado(a).") { }
}

/// <summary>Recurso duplicado ou em conflito (HTTP 409).</summary>
public class ConflitoException : DominioException
{
    public ConflitoException(string mensagem) : base(mensagem) { }
}
