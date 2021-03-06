﻿using FluentValidation;
using SME.AE.Aplicacao.Comandos.Usuario.AlterarEmailCelular;
using SME.AE.Aplicacao.Validators;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.AE.Aplicacao.Comandos.Usuario.AlterarEmailCelular
{
    public class AlterarEmailCelularCommandValidator : AbstractValidator<AlterarEmailCelularCommand>
    {
        public AlterarEmailCelularCommandValidator()
        {
            RuleFor(x => x.AlterarEmailCelularDto).NotNull().WithMessage($"O Objeto AlterarEmailCelularDto é Obrigátorio").SetValidator(new AlterarEmailTelefoneValidator());            
        }
    }
}
