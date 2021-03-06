using MediatR;
using SME.AE.Aplicacao.Comandos.Notificacao.Criar;
using SME.AE.Aplicacao.Comandos.Notificacao.EnviarNotificacaoPorGrupo;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SME.AE.Dominio.Entidades;
using SME.AE.Aplicacao.Comum.Modelos;
using AutoMapper;
using SME.AE.Aplicacao.Comum.Enumeradores;
using FirebaseAdmin.Messaging;
using SME.AE.Comum.Utilitarios;

namespace SME.AE.Aplicacao
{
    public class CriarNotificacaoUseCase : ICriarNotificacaoUseCase
    {
        private readonly IMediator mediator;
        private readonly IMapper mapper;
        public List<Dictionary<String, String>> listaDicionario = new List<Dictionary<String, String>>();


        public CriarNotificacaoUseCase(IMediator mediator, IMapper mapper)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<NotificacaoSgpDto> Executar(NotificacaoSgpDto notificacao)
        {
            await mediator.Send(new CriarNotificacaoCommand(mapper.Map<Notificacao>(notificacao)));

            notificacao.InserirCategoria();

            await EnviarNotificacaoImediataAsync(notificacao);

            return notificacao;
        }

        private async Task EnviarNotificacaoImediataAsync(NotificacaoSgpDto notificacao)
        {
            var dataEnvio = TimeZoneInfo.ConvertTimeToUtc(notificacao.DataEnvio);
            var agora = TimeZoneInfo.ConvertTimeToUtc(DateTime.Now);

            if (dataEnvio > agora)
                return;

            notificacao.InserirCategoria();

            List<int> grupos = notificacao.ObterGrupoLista();
                        
            string bodyUTF8 = UtilString.EncodeUTF8("Voc� recebeu uma nova mensagem da SME. Clique aqui para visualizar os detalhes.");

            Dictionary<string, string> dicionarioNotificacao = new Dictionary<String, String>
            {
                ["Titulo"] = notificacao.Titulo,
                ["Mensagem"] = notificacao.Mensagem,
                ["categoriaNotificacao"] = notificacao.CategoriaNotificacao,
                ["Id"] = notificacao.Id.ToString(),
                ["CriadoEm"] = notificacao.CriadoEm.ToString("yyyy-MM-dd HH:mm:ss.ffffff"),
                ["click_action"] = "FLUTTER_NOTIFICATION_CLICK",
            };

            var Notificacao = new Notification
            {
                Title = notificacao.Titulo,
                Body = bodyUTF8,
            };

            await EnviarNotificacao(notificacao, grupos, dicionarioNotificacao, Notificacao);
        }       

        private async Task EnviarNotificacao(NotificacaoSgpDto notificacao, List<int> grupos, Dictionary<string, string> dicionarioNotificacao, Notification Notificacao)
        {
            switch (notificacao.TipoComunicado)
            {
                case TipoComunicado.SME:
                    await EnviarNotificacaoSME(grupos, dicionarioNotificacao, Notificacao);
                    break;
                case TipoComunicado.DRE:
                    await EnviarComunicadoDRE(notificacao, dicionarioNotificacao, Notificacao);
                    break;
                case TipoComunicado.UE:
                    await EnviarComunicadoUE(notificacao, dicionarioNotificacao, Notificacao);
                    break;
                case TipoComunicado.UEMOD:
                    await EnviarComunicadoUEModalidade(notificacao, grupos, dicionarioNotificacao, Notificacao);
                    break;
                case TipoComunicado.TURMA:
                    await EnviarComunicadoTurmas(notificacao, dicionarioNotificacao, Notificacao);
                    break;
                case TipoComunicado.ALUNO:
                    await EnviarComunicadoAlunos(notificacao, dicionarioNotificacao, Notificacao);
                    break;
                default:
                    break;
            }
        }

        private async Task EnviarComunicadoAlunos(NotificacaoSgpDto notificacao, Dictionary<string, string> dicionarioNotificacao, Notification Notificacao)
        {
            foreach (var aluno in notificacao.Alunos)
            {

                var data = new Dictionary<String, String>(dicionarioNotificacao);

                var topico = "ALU-" + aluno;

                data.Add("CodigoAluno", topico);
                data.Add("CodigoEOL", aluno);

                await mediator.Send(new EnviarNotificacaoPorGrupoCommand(MontaMensagem(topico, Notificacao, data)));
            }
        }

        private async Task EnviarComunicadoTurmas(NotificacaoSgpDto notificacao, Dictionary<string, string> dicionarioNotificacao, Notification Notificacao)
        {
            foreach (var turma in notificacao.Turmas)
            {
                var data = new Dictionary<String, String>(dicionarioNotificacao);
                var topico = "TUR-" + turma;
                data.Add("CodigoTurma", "TUR-" + turma);
                await mediator.Send(new EnviarNotificacaoPorGrupoCommand(MontaMensagem(topico, Notificacao, data)));
            }
        }

        private async Task EnviarComunicadoUEModalidade(NotificacaoSgpDto notificacao, List<int> grupos, Dictionary<string, string> dicionarioNotificacao, Notification Notificacao)
        {
            foreach (var grupo in grupos)
            {
                var data = new Dictionary<String, String>(dicionarioNotificacao);
                var topico = "UE-" + notificacao.CodigoUe + "-MOD-" + grupo;
                data.Add("CodigoUe", "UE-" + notificacao.CodigoUe);
                await mediator.Send(new EnviarNotificacaoPorGrupoCommand(MontaMensagem(topico, Notificacao, data)));
            }
        }

        private async Task EnviarComunicadoUE(NotificacaoSgpDto notificacao, Dictionary<string, string> dicionarioNotificacao, Notification Notificacao)
        {
            var data = new Dictionary<String, String>(dicionarioNotificacao);
            var topico = "UE-" + notificacao.CodigoUe;
            data.Add("CodigoUe", "UE-" + topico);

            await mediator.Send(new EnviarNotificacaoPorGrupoCommand(MontaMensagem(topico, Notificacao, data)));
        }

        private async Task EnviarComunicadoDRE(NotificacaoSgpDto notificacao, Dictionary<string, string> dicionarioNotificacao, Notification Notificacao)
        {
            var data = new Dictionary<String, String>(dicionarioNotificacao);
            var topico = "DRE-" + notificacao.CodigoDre;
            data.Add("CodigoDre", "DRE-" + topico);

            await mediator.Send(new EnviarNotificacaoPorGrupoCommand(MontaMensagem(topico, Notificacao, data)));
        }

        private async Task EnviarNotificacaoSME(List<int> grupos, Dictionary<string, string> dicionarioNotificacao, Notification Notificacao)
        {
            foreach (var grupo in grupos)
            {
                var data = dicionarioNotificacao;

                var topico = "Grupo-" + grupo.ToString();

                await mediator.Send(new EnviarNotificacaoPorGrupoCommand(MontaMensagem(topico, Notificacao, data)));

            }
        }

        private static Message MontaMensagem(string topico, Notification notificacao, Dictionary<string, string> data)
        {
            var Mensagem = new Message();
            Mensagem.Notification = notificacao;
            Mensagem.Data = data;
            Mensagem.Topic = topico;
            return Mensagem;
        }
    }
}