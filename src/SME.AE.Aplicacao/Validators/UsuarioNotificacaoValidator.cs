﻿using FluentValidation;
using SME.AE.Aplicacao.Comum.Modelos.Entrada;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.AE.Aplicacao.Validators
{
    public class UsuarioNotificacaoValidator : AbstractValidator<UsuarioNotificacaoDto>
    {
        public UsuarioNotificacaoValidator()
        {
            RuleFor(x => x.cpfUsuario).Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("Deve ser informado o CPF");
            RuleFor(x => x.cpfUsuario).ValidarCpf().WithMessage("CPF com Formato Invalido").When(x => !string.IsNullOrWhiteSpace(x.cpfUsuario));

            RuleFor(x => x.NotificacaoId).NotEmpty().NotNull().WithMessage("Deve ser informado o Id da Notificação");

            RuleFor(x => x.MensagemVisualizada).NotNull().NotEmpty().WithMessage("Deve ser informada a propriedade Mensagem Visualizada");
        }
    }
}