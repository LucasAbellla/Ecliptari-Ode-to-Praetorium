using System;

namespace Ecliptari.Combate
{
    public sealed class EstadoCombatente
    {
        public DefinicaoCombatente Definicao { get; }
        public string Id => Definicao.Id;
        public bool EstaVivo => HpAtual > 0;
        public int HpAtual { get; private set; }
        public int MpAtual { get; private set; }
        public int ApAtual { get; private set; }
        public int EscudoAtual { get; private set; }

        public EstadoCombatente(DefinicaoCombatente definicao)
        {
            Definicao = definicao ?? throw new ArgumentNullException(nameof(definicao));
            HpAtual = definicao.HpMax;
            MpAtual = definicao.MpMax;
            ApAtual = definicao.ApMax;
            EscudoAtual = 0;
        }

        internal int RegenerarAp()
        {
            int anterior = ApAtual;
            long novoValor = (long)ApAtual + Definicao.ApRegen;
            ApAtual = novoValor >= Definicao.ApMax ? Definicao.ApMax : (int)novoValor;
            return ApAtual - anterior;
        }

        internal void ConsumirAp(int quantidade)
        {
            if (quantidade < 0 || quantidade > ApAtual)
            {
                throw new ArgumentOutOfRangeException(nameof(quantidade));
            }

            ApAtual -= quantidade;
        }

        internal void ConsumirMp(int quantidade)
        {
            if (quantidade < 0 || quantidade > MpAtual)
            {
                throw new ArgumentOutOfRangeException(nameof(quantidade));
            }

            MpAtual -= quantidade;
        }

        internal ResultadoDano ReceberDano(int quantidade)
        {
            if (quantidade < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(quantidade));
            }

            int absorvidoPeloEscudo = Math.Min(EscudoAtual, quantidade);
            EscudoAtual -= absorvidoPeloEscudo;

            int restante = quantidade - absorvidoPeloEscudo;
            int danoNoHp = Math.Min(HpAtual, restante);
            HpAtual -= danoNoHp;
            return new ResultadoDano(quantidade, absorvidoPeloEscudo, danoNoHp);
        }

        internal int Curar(int quantidade)
        {
            if (quantidade < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(quantidade));
            }

            if (!EstaVivo)
            {
                return 0;
            }

            int curaAplicada = Math.Min(Definicao.HpMax - HpAtual, quantidade);
            HpAtual += curaAplicada;
            return curaAplicada;
        }

        internal int ConcederEscudo(int quantidade)
        {
            if (quantidade < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(quantidade));
            }

            if (!EstaVivo)
            {
                return 0;
            }

            int anterior = EscudoAtual;
            long novoValor = (long)EscudoAtual + quantidade;
            EscudoAtual = novoValor > int.MaxValue ? int.MaxValue : (int)novoValor;
            return EscudoAtual - anterior;
        }
    }
}
