﻿using System.Threading.Tasks;
using SME.AE.Dominio.Entidades;

namespace SME.AE.Aplicacao.Comum.Interfaces.Repositorios
{
    public interface IUsuarioNotificacaoRepositorio
    {
        public Task<UsuarioNotificacao> ObterPorId(long id);
        
        public Task<bool> Criar(UsuarioNotificacao notificacao);
        
        public Task<bool> Remover(Notificacao notificacao);

    }
}