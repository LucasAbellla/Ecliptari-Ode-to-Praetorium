using NUnit.Framework;

namespace Ecliptari.Combate.Tests
{
    public sealed class LinhaDoTempoContinuaTests
    {
        [Test]
        public void CombatenteMaisRapido_AgePrimeiroEPodeAgirMaisVezes()
        {
            var lento = new EstadoCombatente(FixturesCombate.CriarCombatente(
                "lento", Equipe.Inimigos, velocidade: 100));
            var rapido = new EstadoCombatente(FixturesCombate.CriarCombatente(
                "rapido", Equipe.Aliados, velocidade: 200));
            var linha = new LinhaDoTempoContinua(new[] { lento, rapido });

            EstadoCombatente primeiro = linha.SelecionarProximo();
            EstadoCombatente segundo = linha.SelecionarProximo();
            EstadoCombatente terceiro = linha.SelecionarProximo();

            Assert.That(primeiro, Is.SameAs(rapido));
            Assert.That(segundo, Is.SameAs(rapido));
            Assert.That(terceiro, Is.SameAs(lento));
        }

        [Test]
        public void EmpateDeValorAcao_PriorizaMaiorVelocidade()
        {
            var lento = new EstadoCombatente(FixturesCombate.CriarCombatente(
                "lento", Equipe.Inimigos, velocidade: 100));
            var rapido = new EstadoCombatente(FixturesCombate.CriarCombatente(
                "rapido", Equipe.Aliados, velocidade: 200));
            var linha = new LinhaDoTempoContinua(new[] { lento, rapido });

            linha.SelecionarProximo();
            EstadoCombatente empatado = linha.SelecionarProximo();

            Assert.That(empatado, Is.SameAs(rapido));
        }

        [Test]
        public void Previsao_NaoAlteraORelogioReal()
        {
            var aliado = new EstadoCombatente(FixturesCombate.CriarCombatente(
                "aliado", Equipe.Aliados, velocidade: 120));
            var inimigo = new EstadoCombatente(FixturesCombate.CriarCombatente(
                "inimigo", Equipe.Inimigos, velocidade: 80));
            var linha = new LinhaDoTempoContinua(new[] { aliado, inimigo });

            var previsao = linha.PreverOrdem(4);
            EstadoCombatente real = linha.SelecionarProximo();

            Assert.That(previsao, Has.Count.EqualTo(4));
            Assert.That(real, Is.SameAs(previsao[0]));
        }

        [Test]
        public void Intervalo_UsaBaseDivididaPelaVelocidade()
        {
            Assert.That(LinhaDoTempoContinua.CalcularIntervalo(125), Is.EqualTo(80d));
        }
    }
}
