﻿
using System.Text.RegularExpressions;

namespace SME.AE.Aplicacao.Comum.Modelos.Usuario
{
    public class AlterarEmailCelularDto
    {
        public long Id { get; set; }
        public string Email { get; set; }
        public string Celular { get; set; }
        public bool AlterarSenha { get; set; } = false;

        public string CelularBanco
        {
            get
            {
                if (Celular == null)
                    return default;

                var apenasDigitos = new Regex(@"[^\d]");
                return apenasDigitos.Replace(Celular, "");
            }
        }
    }
}

