﻿namespace SME.AE.Infra.Persistencia.Consultas
{
    public static class UsuarioConsultas
    {
        internal static string ObterPorCpf = $@"
            SELECT pf,Celular,Nome,Email,UltimoLogin,PrimeiroAcesso,
                Excluido,Id,CriadoEm,CriadoPor,AlteradoEm,AlteradoPor,
                token_redefinicao, redefinicao, validade_token
            FROM Usuario
            WHERE Usuario.Cpf = @Cpf";

        internal static string ObterTodos = @"
            SELECT Cpf from Usuario";

        internal static string ObterCampos = @"Cpf,Celular,Nome,Email,UltimoLogin,PrimeiroAcesso,
                Excluido,Id,CriadoEm,CriadoPor,AlteradoEm,AlteradoPor,
                token_redefinicao, redefinicao, validade_token";

        //  AND Usuario.excluido = false";


        internal static string ObterTotalUsuariosComAcessoIncompleto = @"
        select 
            count(id) 
        from Usuario 
        where primeiroacesso = false 
            and email isnull 
            or celular isnull
        union
        select 
            count(id) 
        from Usuario 
        where primeiroacesso = true ";

        internal static string ObterTotalUsuariosValidos = @"
        select 
            count(id) 
        from Usuario 
        where primeiroacesso = true 
            and excluido = false 
            and email is not null 
            or celular is not null";

    }
}
