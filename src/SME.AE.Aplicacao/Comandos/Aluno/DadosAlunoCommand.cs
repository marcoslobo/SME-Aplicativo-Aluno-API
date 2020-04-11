﻿using FluentValidation.Results;
using MediatR;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comandos.Aluno
{
    public class DadosAlunoCommand : IRequest<RespostaApi>
    {
        public DadosAlunoCommand(string cpf)
        {
            Cpf = cpf;

        }

        public string Cpf { get; set; }

        public class DadosAlunoComandoHandler : IRequestHandler<DadosAlunoCommand, RespostaApi>
        {
            private readonly IAlunoRepositorio _repository;

            public DadosAlunoComandoHandler(IAlunoRepositorio repository)
            {
                _repository = repository;
            }
            public async Task<RespostaApi> Handle
             (DadosAlunoCommand request, CancellationToken cancellationToken)
            {

                var validator = new DadosAlunoUseCaseValidation();
                ValidationResult validacao = validator.Validate(request);
                if (!validacao.IsValid)
                    return RespostaApi.Falha(validacao.Errors);

                var resultado = await _repository.ObterDadosAlunos(request.Cpf);

                return RespostaApi.Sucesso(resultado);
            }

        }
    }
}