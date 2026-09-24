using System;

namespace Ecliptari.Combate
{
    public static class CalculadoraDano
    {
        public const float FatorDefesa = 0.5f;
        public const float FatorDanoContinuo = 0.15f;

        public static int CalcularDanoDireto(float ataque, float poder, float defesa)
        {
            ValidarAtributo(ataque, nameof(ataque));
            ValidarAtributo(poder, nameof(poder));
            ValidarAtributo(defesa, nameof(defesa));

            double danoBruto = ((double)ataque * poder) - ((double)defesa * FatorDefesa);
            return ArredondarSemNegativo(danoBruto);
        }

        public static int CalcularDanoContinuo(float ataqueDoAplicador)
        {
            ValidarAtributo(ataqueDoAplicador, nameof(ataqueDoAplicador));
            return ArredondarSemNegativo((double)ataqueDoAplicador * FatorDanoContinuo);
        }

        private static int ArredondarSemNegativo(double valor)
        {
            if (valor <= 0d)
            {
                return 0;
            }

            if (valor >= int.MaxValue)
            {
                return int.MaxValue;
            }

            return (int)Math.Round(valor, MidpointRounding.AwayFromZero);
        }

        private static void ValidarAtributo(float valor, string parametro)
        {
            if (float.IsNaN(valor) || float.IsInfinity(valor) || valor < 0f)
            {
                throw new ArgumentOutOfRangeException(parametro, "O atributo deve ser finito e não negativo.");
            }
        }
    }
}
