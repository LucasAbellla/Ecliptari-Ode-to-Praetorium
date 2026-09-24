using System;
using System.Linq;
using NUnit.Framework;

namespace Ecliptari.Combate.Tests
{
    public sealed class MotorBatalhaTests
    {
        [Test]
        public void Iniciar_AbreAcaoParaOCombatenteMaisRapido()
        {
            MotorBatalha motor = CriarBatalhaPadrao();

            motor.Iniciar();

            Assert.That(motor.Fase, Is.EqualTo(FaseCombate.Acao));
            Assert.That(motor.AtorAtual.Id, Is.EqualTo("aliado"));
        }

        [Test]
        public void Habilidade_ConsomeRecursosEAplicaDano()
        {
            MotorBatalha motor = CriarBatalhaPadrao();
            motor.Iniciar();
            EstadoCombatente aliado = motor.ObterCombatente("aliado");
            EstadoCombatente inimigo = motor.ObterCombatente("inimigo");

            ResultadoComando resultado = motor.UsarHabilidade(
                aliado.Id,
                inimigo.Id,
                FixturesCombate.CriarAtaque());

            Assert.That(resultado.Executado, Is.True);
            Assert.That(motor.Fase, Is.EqualTo(FaseCombate.Resolucao));
            Assert.That(aliado.ApAtual, Is.EqualTo(3));
            Assert.That(aliado.MpAtual, Is.EqualTo(9));
            Assert.That(inimigo.HpAtual, Is.EqualTo(320));
        }

        [Test]
        public void ComandoInvalido_NaoAlteraEstadoNemConsomeRecursos()
        {
            MotorBatalha motor = CriarBatalhaPadrao();
            motor.Iniciar();
            EstadoCombatente aliado = motor.ObterCombatente("aliado");
            EstadoCombatente inimigo = motor.ObterCombatente("inimigo");
            var habilidadeCara = FixturesCombate.CriarAtaque(custoAp: 6);

            ResultadoComando resultado = motor.UsarHabilidade(
                aliado.Id,
                inimigo.Id,
                habilidadeCara);

            Assert.That(resultado.Executado, Is.False);
            Assert.That(resultado.Erro, Is.EqualTo(ErroComando.ApInsuficiente));
            Assert.That(aliado.ApAtual, Is.EqualTo(5));
            Assert.That(aliado.MpAtual, Is.EqualTo(10));
            Assert.That(inimigo.HpAtual, Is.EqualTo(500));
            Assert.That(motor.Fase, Is.EqualTo(FaseCombate.Acao));
        }

        [Test]
        public void NovoTurno_RegeneraApAteOLimite()
        {
            MotorBatalha motor = CriarBatalhaPadrao();
            motor.Iniciar();

            motor.UsarHabilidade("aliado", "inimigo", FixturesCombate.CriarAtaque(custoAp: 3));
            motor.ConcluirResolucao();
            Assert.That(motor.AtorAtual.Id, Is.EqualTo("inimigo"));

            motor.PassarTurno("inimigo");
            motor.ConcluirResolucao();

            Assert.That(motor.AtorAtual.Id, Is.EqualTo("aliado"));
            Assert.That(motor.ObterCombatente("aliado").ApAtual, Is.EqualTo(4));
        }

        [Test]
        public void DanoLetal_EncerraComVitoriaDepoisDaResolucao()
        {
            var aliado = FixturesCombate.CriarCombatente(
                "aliado", Equipe.Aliados, velocidade: 100, ataque: 100f);
            var inimigo = FixturesCombate.CriarCombatente(
                "inimigo", Equipe.Inimigos, velocidade: 80, hpMax: 50, defesa: 0f);
            var motor = new MotorBatalha(new[] { aliado, inimigo });
            motor.Iniciar();

            motor.UsarHabilidade(
                "aliado",
                "inimigo",
                FixturesCombate.CriarAtaque(poder: 1f));

            Assert.That(motor.Fase, Is.EqualTo(FaseCombate.Resolucao));
            Assert.That(motor.ObterCombatente("inimigo").EstaVivo, Is.False);

            motor.ConcluirResolucao();

            Assert.That(motor.Fase, Is.EqualTo(FaseCombate.Vitoria));
            Assert.That(motor.FoiEncerrada, Is.True);
            Assert.That(motor.AtorAtual, Is.Null);
        }

        [Test]
        public void Acao_ProduzEventosParaAApresentacao()
        {
            MotorBatalha motor = CriarBatalhaPadrao();
            motor.Iniciar();
            motor.DrenarEventos();

            motor.UsarHabilidade("aliado", "inimigo", FixturesCombate.CriarAtaque());
            var eventos = motor.DrenarEventos();

            Assert.That(eventos.Any(item => item.Tipo == TipoEventoCombate.HabilidadeUsada), Is.True);
            Assert.That(eventos.Any(item =>
                item.Tipo == TipoEventoCombate.DanoCausado && item.Valor == 180), Is.True);
            Assert.That(eventos.Any(item =>
                item.Tipo == TipoEventoCombate.FaseAlterada
                && item.Fase == FaseCombate.Resolucao), Is.True);
        }

        [Test]
        public void Construtor_RejeitaIdsDuplicados()
        {
            var primeiro = FixturesCombate.CriarCombatente("igual", Equipe.Aliados);
            var segundo = FixturesCombate.CriarCombatente("igual", Equipe.Inimigos);

            Assert.Throws<ArgumentException>(() =>
                new MotorBatalha(new[] { primeiro, segundo }));
        }

        [Test]
        public void DefinicaoCombatente_RejeitaEnumInvalido()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new DefinicaoCombatente(
                    "invalido",
                    "Inválido",
                    (Equipe)999,
                    Elemento.Nenhum,
                    TipoArma.Nenhuma,
                    TipagemPoder.Nulo,
                    100,
                    10,
                    5,
                    1,
                    100,
                    10f,
                    10f,
                    1f));
        }

        private static MotorBatalha CriarBatalhaPadrao()
        {
            var aliado = FixturesCombate.CriarCombatente(
                "aliado", Equipe.Aliados, velocidade: 100);
            var inimigo = FixturesCombate.CriarCombatente(
                "inimigo", Equipe.Inimigos, velocidade: 90);
            return new MotorBatalha(new[] { aliado, inimigo });
        }
    }
}
