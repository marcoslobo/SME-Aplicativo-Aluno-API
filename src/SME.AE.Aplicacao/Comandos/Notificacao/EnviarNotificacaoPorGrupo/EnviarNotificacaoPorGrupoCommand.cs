﻿using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using MediatR;
using SME.AE.Aplicacao.Comum.Config;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comandos.Notificacao.EnviarNotificacaoPorGrupo
{
    public class EnviarNotificacaoPorGrupoCommand : IRequest<bool>
    {
        public SME.AE.Dominio.Entidades.Notificacao Notificacao { set; get; }

        public List<int> Grupos { get; set; }

        public EnviarNotificacaoPorGrupoCommand(SME.AE.Dominio.Entidades.Notificacao notificacao, List<int> grupos)
        {
            this.Notificacao = notificacao;
            this.Grupos = grupos;
        }
    }

    public class EnviarNotificacaoPorGrupoCommandHandler : IRequestHandler<EnviarNotificacaoPorGrupoCommand, bool>
    {

        public EnviarNotificacaoPorGrupoCommandHandler()
        {
        }

        public async Task<bool> Handle(EnviarNotificacaoPorGrupoCommand request, CancellationToken cancellationToken)
        {
            var firebaseCredential = GoogleCredential.FromJson(VariaveisAmbiente.FirebaseToken);

            FirebaseApp app = FirebaseApp.DefaultInstance;
            String resultado = "";

            if (app == null)
            {
                app = FirebaseApp.Create(new AppOptions()
                {
                    Credential = firebaseCredential,
                    ProjectId = VariaveisAmbiente.FirebaseProjectId
                });
            }

            foreach(var grupo in request.Grupos)
            {
                resultado = await FirebaseMessaging.DefaultInstance.SendAsync(new Message()
                {
                    Data = new Dictionary<String, String>
                    {
                        ["Titulo"] = request.Notificacao.Titulo,
                        ["Mensagem"] = request.Notificacao.Mensagem,
                        ["Grupo"] = request.Notificacao.Grupo,
                        ["Id"] = request.Notificacao.Id.ToString()
                    },
                    Notification = new Notification
                    {
                        Title = request.Notificacao.Titulo,
                        Body = "Você possui novos comunicados. Toque aqui para abrir no aplicativo.",
                    },
                    Topic = grupo.ToString()
                }).ConfigureAwait(true);
            }

            return resultado != null;
        }
    }
}