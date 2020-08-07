﻿using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.AE.Aplicacao.Consultas.ObterUsuarioPorTokenRedefinicao
{
    public class ObterUsuarioPorTokenRedefinicaoQueryValidator : AbstractValidator<ObterUsuarioPorTokenRedefinicaoQuery>
    {
        public ObterUsuarioPorTokenRedefinicaoQueryValidator()
        {
            RuleFor(x => x.Token).NotEmpty().WithMessage("O Token é Obrigátorio");
            RuleFor(x => x.Token).Length(8).When(x => !string.IsNullOrWhiteSpace(x.Token)).WithMessage("O Token possui tamanho inválido");
        }
    }
}