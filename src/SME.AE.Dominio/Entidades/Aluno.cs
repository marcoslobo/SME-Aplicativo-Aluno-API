﻿using System;
using System.Collections.Generic;
using System.Text;

namespace SME.AE.Dominio.Entidades
{
   public class Aluno
    {
        public int  CodigoEol { get; set; }
        public string Nome { get; set; }
        public string NomeSocial { get; set; }
        public string Escola { get; set; }
        public int CodigoTipoEscola { get; set; }
        public string DescricaoTipoEscola { get; set; }
        public string SiglaDre { get; set; }
        public string Turma { get; set; }
    }
}
