﻿using System;

namespace SME.AE.Aplicacao.Comum.Config
{
    public static class VariaveisAmbiente
    {
        public static string JwtTokenSecret = Environment.GetEnvironmentVariable("SME_AE_JWT_TOKEN_SECRET");
    }
}