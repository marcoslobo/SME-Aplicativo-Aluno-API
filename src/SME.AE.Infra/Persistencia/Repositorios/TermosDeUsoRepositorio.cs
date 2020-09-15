﻿using Dapper;
using Sentry;
using SME.AE.Aplicacao.Comum.Config;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Dominio.Entidades;
using SME.AE.Infra.Persistencia.Consultas;
using System;
using System.Threading.Tasks;

namespace SME.AE.Infra.Persistencia.Repositorios
{
    public class TermosDeUsoRepositorio : BaseRepositorio<TermosDeUso>, ITermosDeUsoRepositorio
    {

        private const string ULTIMAVERSAOTERMOSDEUSO = "ultimaVersaoTermosDeUso";
        private const string TERMODEUSOPORID = "termoDeUsoPorId";
        private readonly ICacheRepositorio cacheRepositorio;

        public TermosDeUsoRepositorio(ICacheRepositorio cacheRepositorio) : base(ConnectionStrings.Conexao)
        {
            this.cacheRepositorio = cacheRepositorio;
        }

        public async Task<TermosDeUso> ObterUltimaVersao()
        {
            try
            {
                var ultimaVersaoTermosDeUsoCache = ObterUltimaVersaoCache();
                if (ultimaVersaoTermosDeUsoCache != null)
                    return ultimaVersaoTermosDeUsoCache;

                using var conexao = InstanciarConexao();
                conexao.Open();
                var termosDeUso = await conexao.QueryFirstAsync<TermosDeUso>(TermosDeUsoConsultas.ObterUltimaVersaoDosTermosDeUso);
                conexao.Close();

                var chaveUltimaVersaoTermosDeUso = $"{ULTIMAVERSAOTERMOSDEUSO}";
                await cacheRepositorio.SalvarAsync(chaveUltimaVersaoTermosDeUso, termosDeUso);
                return termosDeUso;
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                return null;
            }
        }

        async Task<long> ITermosDeUsoRepositorio.SalvarAsync(TermosDeUso termos)
        {
            return await base.SalvarAsync(termos);
        }

        public async Task<TermosDeUso> ObterPorId(long id)
        {
            try
            {
                var termoDeUsoPorId = ObterTermosDeUsoPorIdCache(id);
                if (termoDeUsoPorId != null)
                    return termoDeUsoPorId;

                using var conexao = InstanciarConexao();
                conexao.Open();
                var termosDeUso = await conexao.QueryFirstAsync<TermosDeUso>($"{TermosDeUsoConsultas.ObterTermosDeUso} WHERE Id = @TermoDeUsoId", new { id });
                conexao.Close();

                var chaveTermoDeUsoPorId = $"{TERMODEUSOPORID}";
                await cacheRepositorio.SalvarAsync(chaveTermoDeUsoPorId, termosDeUso);

                return termosDeUso;
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                return null;
            }
        }

        private TermosDeUso ObterUltimaVersaoCache()
=> cacheRepositorio.Obter<TermosDeUso>($"{ULTIMAVERSAOTERMOSDEUSO}");

        private TermosDeUso ObterTermosDeUsoPorIdCache(long id)
=> cacheRepositorio.Obter<TermosDeUso>($"{TERMODEUSOPORID}-{id}");
    }
}