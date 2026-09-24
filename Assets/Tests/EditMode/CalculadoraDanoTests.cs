using System;
using NUnit.Framework;

namespace Ecliptari.Combate.Tests
{
    public sealed class CalculadoraDanoTests
    {
        [Test]
        public void DanoDireto_UsaAtaquePoderEDefesa()
        {
            int dano = CalculadoraDano.CalcularDanoDireto(100f, 2f, 40f);

            Assert.That(dano, Is.EqualTo(180));
        }

        [Test]
        public void DanoDireto_NuncaFicaNegativo()
        {
            int dano = CalculadoraDano.CalcularDanoDireto(10f, 1f, 100f);

            Assert.That(dano, Is.Zero);
        }

        [Test]
        public void DanoContinuo_UsaQuinzePorCentoDoAtaque()
        {
            int dano = CalculadoraDano.CalcularDanoContinuo(110f);

            Assert.That(dano, Is.EqualTo(17));
        }

        [Test]
        public void Dano_RejeitaAtributoInvalido()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                CalculadoraDano.CalcularDanoDireto(float.NaN, 1f, 1f));
        }

        [Test]
        public void DanoMuitoAlto_EhLimitadoAoMaiorInteiro()
        {
            int dano = CalculadoraDano.CalcularDanoDireto(
                float.MaxValue,
                float.MaxValue,
                0f);

            Assert.That(dano, Is.EqualTo(int.MaxValue));
        }
    }
}
