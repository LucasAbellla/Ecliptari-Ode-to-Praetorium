using System.Collections.Generic;
using System.Linq;
using Ecliptari.Conteudo;
using NUnit.Framework;

namespace Ecliptari.Combate.Tests
{
    public sealed class InteracaoTrioBasicoTests
    {
        [Test]
        public void Trio_ProtegeRecebeDanoERecuperaVanguarda()
        {
            IReadOnlyList<PersonagemBasico> trio = CatalogoPersonagensBasicos.CriarTrio();
            PersonagemBasico vanguarda = trio.Single(item => item.Papel == PapelPersonagem.Atacante);
            PersonagemBasico guardiao = trio.Single(item => item.Papel == PapelPersonagem.Defensor);
            PersonagemBasico suporte = trio.Single(item => item.Papel == PapelPersonagem.Suporte);
            DefinicaoCombatente inimigo = FixturesCombate.CriarCombatente(
                "inimigo_teste",
                Equipe.Inimigos,
                velocidade: 80,
                hpMax: 2000,
                ataque: 130f,
                defesa: 50f);
            var ataqueInimigo = new DefinicaoHabilidade(
                "ataque_inimigo",
                "Ataque inimigo",
                custoAp: 0,
                custoMp: 0,
                poder: 1.5f,
                TipoAlvo.InimigoUnico);
            var motor = new MotorBatalha(trio
                .Select(item => item.Combatente)
                .Concat(new[] { inimigo }));
            motor.Iniciar();
            motor.DrenarEventos();

            UsarEConcluir(
                motor,
                vanguarda.Combatente.Id,
                inimigo.Id,
                vanguarda.ObterHabilidade(CatalogoPersonagensBasicos.IdAtaqueVanguarda));
            PassarEConcluir(motor, suporte.Combatente.Id);
            UsarEConcluir(
                motor,
                guardiao.Combatente.Id,
                vanguarda.Combatente.Id,
                guardiao.ObterHabilidade(CatalogoPersonagensBasicos.IdHabilidadeGuardiao));

            EstadoCombatente estadoVanguarda = motor.ObterCombatente(vanguarda.Combatente.Id);
            Assert.That(estadoVanguarda.EscudoAtual, Is.EqualTo(91));

            UsarEConcluir(motor, inimigo.Id, vanguarda.Combatente.Id, ataqueInimigo);
            Assert.That(estadoVanguarda.EscudoAtual, Is.Zero);
            Assert.That(estadoVanguarda.HpAtual, Is.EqualTo(823));

            UsarEConcluir(
                motor,
                vanguarda.Combatente.Id,
                inimigo.Id,
                vanguarda.ObterHabilidade(CatalogoPersonagensBasicos.IdAtaqueVanguarda));
            ResultadoComando cura = motor.UsarHabilidade(
                suporte.Combatente.Id,
                vanguarda.Combatente.Id,
                suporte.ObterHabilidade(CatalogoPersonagensBasicos.IdHabilidadeSuporte));

            Assert.That(cura.Executado, Is.True);
            Assert.That(estadoVanguarda.HpAtual, Is.EqualTo(vanguarda.Combatente.HpMax));

            var eventos = motor.DrenarEventos();
            Assert.That(eventos.Any(item =>
                item.Tipo == TipoEventoCombate.EscudoConcedido && item.Valor == 91), Is.True);
            Assert.That(eventos.Any(item =>
                item.Tipo == TipoEventoCombate.EscudoAbsorveu && item.Valor == 91), Is.True);
            Assert.That(eventos.Any(item =>
                item.Tipo == TipoEventoCombate.CuraRecebida && item.Valor == 77), Is.True);
        }

        private static void UsarEConcluir(
            MotorBatalha motor,
            string atorId,
            string alvoId,
            DefinicaoHabilidade habilidade)
        {
            Assert.That(motor.AtorAtual.Id, Is.EqualTo(atorId));
            Assert.That(motor.UsarHabilidade(atorId, alvoId, habilidade).Executado, Is.True);
            motor.ConcluirResolucao();
        }

        private static void PassarEConcluir(MotorBatalha motor, string atorId)
        {
            Assert.That(motor.AtorAtual.Id, Is.EqualTo(atorId));
            Assert.That(motor.PassarTurno(atorId).Executado, Is.True);
            motor.ConcluirResolucao();
        }
    }
}
