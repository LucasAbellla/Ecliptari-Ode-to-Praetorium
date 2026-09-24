using System.Linq;
using NUnit.Framework;

namespace Ecliptari.Combate.Tests
{
    public sealed class EfeitosHabilidadeTests
    {
        [Test]
        public void HabilidadeComposta_AplicaEfeitosEmDestinosDiferentes()
        {
            var aliado = FixturesCombate.CriarCombatente(
                "aliado", Equipe.Aliados, velocidade: 100, defesa: 100f);
            var inimigo = FixturesCombate.CriarCombatente(
                "inimigo", Equipe.Inimigos, velocidade: 80);
            var habilidade = new DefinicaoHabilidade(
                "ataque_protegido",
                "Ataque protegido",
                custoAp: 2,
                custoMp: 0,
                TipoAlvo.InimigoUnico,
                new EfeitoHabilidade[]
                {
                    new EfeitoDano(1f),
                    new EfeitoEscudo(0.5f, destino: DestinoEfeito.Usuario)
                });
            var motor = new MotorBatalha(new[] { aliado, inimigo });
            motor.Iniciar();

            motor.UsarHabilidade("aliado", "inimigo", habilidade);

            Assert.That(motor.ObterCombatente("inimigo").HpAtual, Is.LessThan(inimigo.HpMax));
            Assert.That(motor.ObterCombatente("aliado").EscudoAtual, Is.EqualTo(50));
        }

        [Test]
        public void AtaqueEmArea_ResolveTodosOsInimigosSemListaManualDeAlvos()
        {
            var aliado = FixturesCombate.CriarCombatente(
                "aliado", Equipe.Aliados, velocidade: 120);
            var inimigoA = FixturesCombate.CriarCombatente(
                "inimigo_a", Equipe.Inimigos, velocidade: 80);
            var inimigoB = FixturesCombate.CriarCombatente(
                "inimigo_b", Equipe.Inimigos, velocidade: 70);
            var habilidade = new DefinicaoHabilidade(
                "area",
                "Ataque em área",
                custoAp: 2,
                custoMp: 0,
                TipoAlvo.TodosInimigos,
                new EfeitoHabilidade[] { new EfeitoDano(1f) });
            var motor = new MotorBatalha(new[] { aliado, inimigoA, inimigoB });
            motor.Iniciar();

            ResultadoComando resultado = motor.UsarHabilidade(
                "aliado",
                Enumerable.Empty<string>(),
                habilidade);

            Assert.That(resultado.Executado, Is.True);
            Assert.That(motor.ObterCombatente("inimigo_a").HpAtual, Is.LessThan(inimigoA.HpMax));
            Assert.That(motor.ObterCombatente("inimigo_b").HpAtual, Is.LessThan(inimigoB.HpMax));
        }

        [Test]
        public void Escudo_AbsorveDanoAntesDoHp()
        {
            var defensor = FixturesCombate.CriarCombatente(
                "defensor", Equipe.Aliados, velocidade: 120, defesa: 100f);
            var inimigo = FixturesCombate.CriarCombatente(
                "inimigo", Equipe.Inimigos, velocidade: 100, ataque: 100f);
            var escudo = new DefinicaoHabilidade(
                "escudo",
                "Escudo",
                custoAp: 0,
                custoMp: 0,
                TipoAlvo.ProprioUsuario,
                new EfeitoHabilidade[] { new EfeitoEscudo(1f) });
            var ataque = FixturesCombate.CriarAtaque(custoAp: 0, custoMp: 0, poder: 2f);
            var motor = new MotorBatalha(new[] { defensor, inimigo });
            motor.Iniciar();
            motor.UsarHabilidade("defensor", alvoId: null, escudo);
            motor.ConcluirResolucao();

            motor.UsarHabilidade("inimigo", "defensor", ataque);

            Assert.That(motor.ObterCombatente("defensor").EscudoAtual, Is.Zero);
            Assert.That(motor.ObterCombatente("defensor").HpAtual, Is.EqualTo(450));
        }

        [Test]
        public void AtaqueEmArea_RejeitaAlvosManuaisSemConsumirRecursos()
        {
            var aliado = FixturesCombate.CriarCombatente(
                "aliado", Equipe.Aliados, velocidade: 120);
            var inimigo = FixturesCombate.CriarCombatente(
                "inimigo", Equipe.Inimigos, velocidade: 80);
            var habilidade = new DefinicaoHabilidade(
                "area",
                "Ataque em área",
                custoAp: 2,
                custoMp: 1,
                TipoAlvo.TodosInimigos,
                new EfeitoHabilidade[] { new EfeitoDano(1f) });
            var motor = new MotorBatalha(new[] { aliado, inimigo });
            motor.Iniciar();

            ResultadoComando resultado = motor.UsarHabilidade(
                "aliado",
                new[] { "inimigo" },
                habilidade);

            Assert.That(resultado.Executado, Is.False);
            Assert.That(resultado.Erro, Is.EqualTo(ErroComando.AlvoInvalido));
            Assert.That(motor.ObterCombatente("aliado").ApAtual, Is.EqualTo(aliado.ApMax));
            Assert.That(motor.ObterCombatente("aliado").MpAtual, Is.EqualTo(aliado.MpMax));
            Assert.That(motor.Fase, Is.EqualTo(FaseCombate.Acao));
        }

        [Test]
        public void DefinicaoHabilidade_CopiaAListaDeEfeitos()
        {
            var danoOriginal = new EfeitoDano(1f);
            EfeitoHabilidade[] origem = { danoOriginal };
            var habilidade = new DefinicaoHabilidade(
                "imutavel",
                "Imutável",
                custoAp: 0,
                custoMp: 0,
                TipoAlvo.InimigoUnico,
                origem);

            origem[0] = new EfeitoCura(1f);

            Assert.That(habilidade.Efeitos[0], Is.SameAs(danoOriginal));
        }
    }
}
