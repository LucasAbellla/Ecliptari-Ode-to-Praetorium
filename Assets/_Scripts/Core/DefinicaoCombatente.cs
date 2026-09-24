using System;

namespace Ecliptari.Combate
{
    public sealed class DefinicaoCombatente
    {
        public string Id { get; }
        public string Nome { get; }
        public Equipe Equipe { get; }
        public Elemento Elemento { get; }
        public TipoArma TipoArma { get; }
        public TipagemPoder Tipagem { get; }
        public int HpMax { get; }
        public int MpMax { get; }
        public int ApMax { get; }
        public int ApRegen { get; }
        public int Velocidade { get; }
        public float Ataque { get; }
        public float Defesa { get; }
        public float Elementalizacao { get; }

        public DefinicaoCombatente(
            string id,
            string nome,
            Equipe equipe,
            Elemento elemento,
            TipoArma tipoArma,
            TipagemPoder tipagem,
            int hpMax,
            int mpMax,
            int apMax,
            int apRegen,
            int velocidade,
            float ataque,
            float defesa,
            float elementalizacao)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("O combatente precisa de um identificador.", nameof(id));
            }

            if (string.IsNullOrWhiteSpace(nome))
            {
                throw new ArgumentException("O combatente precisa de um nome.", nameof(nome));
            }

            if (!Enum.IsDefined(typeof(Equipe), equipe))
            {
                throw new ArgumentOutOfRangeException(nameof(equipe));
            }

            if (!Enum.IsDefined(typeof(Elemento), elemento))
            {
                throw new ArgumentOutOfRangeException(nameof(elemento));
            }

            if (!Enum.IsDefined(typeof(TipoArma), tipoArma))
            {
                throw new ArgumentOutOfRangeException(nameof(tipoArma));
            }

            if (!Enum.IsDefined(typeof(TipagemPoder), tipagem))
            {
                throw new ArgumentOutOfRangeException(nameof(tipagem));
            }

            ExigirPositivo(hpMax, nameof(hpMax));
            ExigirNaoNegativo(mpMax, nameof(mpMax));
            ExigirNaoNegativo(apMax, nameof(apMax));
            ExigirNaoNegativo(apRegen, nameof(apRegen));
            ExigirPositivo(velocidade, nameof(velocidade));
            ExigirNaoNegativo(ataque, nameof(ataque));
            ExigirNaoNegativo(defesa, nameof(defesa));
            ExigirNaoNegativo(elementalizacao, nameof(elementalizacao));

            Id = id;
            Nome = nome;
            Equipe = equipe;
            Elemento = elemento;
            TipoArma = tipoArma;
            Tipagem = tipagem;
            HpMax = hpMax;
            MpMax = mpMax;
            ApMax = apMax;
            ApRegen = apRegen;
            Velocidade = velocidade;
            Ataque = ataque;
            Defesa = defesa;
            Elementalizacao = elementalizacao;
        }

        private static void ExigirPositivo(int valor, string parametro)
        {
            if (valor <= 0)
            {
                throw new ArgumentOutOfRangeException(parametro, "O valor deve ser maior que zero.");
            }
        }

        private static void ExigirNaoNegativo(int valor, string parametro)
        {
            if (valor < 0)
            {
                throw new ArgumentOutOfRangeException(parametro, "O valor não pode ser negativo.");
            }
        }

        private static void ExigirNaoNegativo(float valor, string parametro)
        {
            if (float.IsNaN(valor) || float.IsInfinity(valor) || valor < 0f)
            {
                throw new ArgumentOutOfRangeException(parametro, "O valor deve ser finito e não negativo.");
            }
        }
    }
}
