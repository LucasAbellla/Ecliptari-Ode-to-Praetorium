using System;

namespace Ecliptari.Combate
{
    public sealed class ResultadoDano
    {
        public int DanoCalculado { get; }
        public int AbsorvidoPeloEscudo { get; }
        public int DanoNoHp { get; }
        public int DanoEfetivo => AbsorvidoPeloEscudo + DanoNoHp;

        public ResultadoDano(int danoCalculado, int absorvidoPeloEscudo, int danoNoHp)
        {
            if (danoCalculado < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(danoCalculado));
            }

            if (absorvidoPeloEscudo < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(absorvidoPeloEscudo));
            }

            if (danoNoHp < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(danoNoHp));
            }

            if ((long)absorvidoPeloEscudo + danoNoHp > danoCalculado)
            {
                throw new ArgumentException("O dano efetivo não pode exceder o dano calculado.");
            }

            DanoCalculado = danoCalculado;
            AbsorvidoPeloEscudo = absorvidoPeloEscudo;
            DanoNoHp = danoNoHp;
        }
    }
}
