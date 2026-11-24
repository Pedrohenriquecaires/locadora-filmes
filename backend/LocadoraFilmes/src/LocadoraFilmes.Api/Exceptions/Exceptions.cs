namespace LocadoraFilmes.Api.Exceptions;

/// <summary>Recurso inexistente (HTTP 404).</summary>
public class RecursoNaoEncontradoException : Exception
{
    public RecursoNaoEncontradoException(string mensagem) : base(mensagem)
    {
    }
}

/// <summary>Conflito com o estado atual dos dados, ex.: duplicidade ou dependências (HTTP 409).</summary>
public class ConflitoException : Exception
{
    public ConflitoException(string mensagem) : base(mensagem)
    {
    }
}

/// <summary>Violação de regra de negócio (HTTP 422).</summary>
public class RegraNegocioException : Exception
{
    public RegraNegocioException(string mensagem) : base(mensagem)
    {
    }
}

/// <summary>Credenciais inválidas no login (HTTP 401).</summary>
public class CredenciaisInvalidasException : Exception
{
    public CredenciaisInvalidasException() : base("E-mail ou senha inválidos.")
    {
    }
}
