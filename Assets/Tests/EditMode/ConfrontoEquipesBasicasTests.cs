using System;
using System.Collections.Generic;
using System.Linq;
using Ecliptari.Conteudo;
using NUnit.Framework;

namespace Ecliptari.Combate.Tests
{
    public sealed class ConfrontoEquipesBasicasTests
    {
        [Test]
        public void ArcanistaFulmen_AtingeOsTresInimigos()
        {
            var equipe = CatalogoPersonagensBasicos.CriarEquipePadrao();
            var inimigos = CatalogoInimigosBasicos.CriarGrupoDeTeste();
            PersonagemBasico arcanista = equipe.Single(item =>
                item.Papel == PapelPersonagem.Especialista);
            MotorBatalha motor = CriarConfronto(equipe, inimigos);
            motor.Iniciar();
            AvancarAte(motor, arcanista.Combatente.Id);

            ResultadoComando resultado = motor.UsarHabilidade(
                arcanista.Combatente.Id,
                Enumerable.Empty<string>(),
                arcanista.ObterHabilidade(CatalogoPersonagensBasicos.IdHabilidadeEspecialista));

            Assert.That(resultado.Executado, Is.True);
            Assert.That(inimigos.All(item =>
                motor.ObterCombatente(item.Combatente.Id).HpAtual < item.Combatente.HpMax), Is.True);
        }

        [Test]
        public void ConjuradorEol_AtingeOsQuatroAliados()
        {
            var equipe = CatalogoPersonagensBasicos.CriarEquipePadrao();
            var inimigos = CatalogoInimigosBasicos.CriarGrupoDeTeste();
            InimigoBasico conjurador = inimigos.Single(item =>
                item.Papel == PapelInimigo.Conjurador);
            MotorBatalha motor = CriarConfronto(equipe, inimigos);
            motor.Iniciar();
            AvancarAte(motor, conjurador.Combatente.Id);

            ResultadoComando resultado = motor.UsarHabilidade(
                conjurador.Combatente.Id,
                Enumerable.Empty<string>(),
                conjurador.ObterHabilidade(CatalogoInimigosBasicos.IdHabilidadeConjurador));

            Assert.That(resultado.Executado, Is.True);
            Assert.That(equipe.All(item =>
                motor.ObterCombatente(item.Combatente.Id).HpAtual < item.Combatente.HpMax), Is.True);
        }

        [Test]
        public void ColossoTerrae_AtacaESeProtegeNaMesmaAcao()
        {
            var equipe = CatalogoPersonagensBasicos.CriarEquipePadrao();
            var inimigos = CatalogoInimigosBasicos.CriarGrupoDeTeste();
            InimigoBasico colosso = inimigos.Single(item => item.Papel == PapelInimigo.Colosso);
            PersonagemBasico vanguarda = equipe.Single(item =>
                item.Papel == PapelPersonagem.Atacante);
            MotorBatalha motor = CriarConfronto(equipe, inimigos);
            motor.Iniciar();
            AvancarAte(motor, colosso.Combatente.Id);

            ResultadoComando resultado = motor.UsarHabilidade(
                colosso.Combatente.Id,
                vanguarda.Combatente.Id,
                colosso.ObterHabilidade(CatalogoInimigosBasicos.IdHabilidadeColosso));

            Assert.That(resultado.Executado, Is.True);
            Assert.That(
                motor.ObterCombatente(vanguarda.Combatente.Id).HpAtual,
                Is.LessThan(vanguarda.Combatente.HpMax));
            Assert.That(motor.ObterCombatente(colosso.Combatente.Id).EscudoAtual, Is.EqualTo(60));
        }

        [Test]
        public void ConfrontoCompleto_QuatroContraTresChegaAUmResultado()
        {
            var equipe = CatalogoPersonagensBasicos.CriarEquipePadrao();
            var inimigos = CatalogoInimigosBasicos.CriarGrupoDeTeste();
            var habilidades = equipe.ToDictionary(
                item => item.Combatente.Id,
                item => item.Habilidades);
            foreach (InimigoBasico inimigo in inimigos)
            {
                habilidades.Add(inimigo.Combatente.Id, inimigo.Habilidades);
            }

            MotorBatalha motor = CriarConfronto(equipe, inimigos);
            motor.Iniciar();
            int turnos = 0;

            while (!motor.FoiEncerrada && turnos < 300)
            {
                EstadoCombatente ator = motor.AtorAtual;
                DefinicaoHabilidade habilidade = EscolherHabilidade(
                    ator,
                    habilidades[ator.Id],
                    motor);
                IEnumerable<string> alvos = EscolherAlvos(ator, habilidade, motor);

                ResultadoComando resultado = motor.UsarHabilidade(
                    ator.Id,
                    alvos,
                    habilidade);
                Assert.That(resultado.Executado, Is.True);
                motor.ConcluirResolucao();
                turnos++;
            }

            Assert.That(motor.FoiEncerrada, Is.True);
            Assert.That(turnos, Is.LessThan(300));
            Assert.That(
                motor.Combatentes.Count(item => item.EstaVivo && item.Definicao.Equipe == Equipe.Aliados)
                <= RegrasFormacao.TamanhoPadraoEquipe,
                Is.True);
        }

        private static MotorBatalha CriarConfronto(
            IReadOnlyList<PersonagemBasico> equipe,
            IReadOnlyList<InimigoBasico> inimigos)
        {
            return new MotorBatalha(
                equipe.Select(item => item.Combatente)
                    .Concat(inimigos.Select(item => item.Combatente)));
        }

        private static void AvancarAte(MotorBatalha motor, string combatenteId)
        {
            int limite = 30;
            while (motor.AtorAtual.Id != combatenteId && limite-- > 0)
            {
                Assert.That(motor.PassarTurno(motor.AtorAtual.Id).Executado, Is.True);
                motor.ConcluirResolucao();
            }

            Assert.That(motor.AtorAtual.Id, Is.EqualTo(combatenteId));
        }

        private static DefinicaoHabilidade EscolherHabilidade(
            EstadoCombatente ator,
            IReadOnlyList<DefinicaoHabilidade> habilidades,
            MotorBatalha motor)
        {
            DefinicaoHabilidade especial = habilidades.Count > 1 ? habilidades[1] : null;
            bool podePagar = especial != null
                && ator.ApAtual >= especial.CustoAp
                && ator.MpAtual >= especial.CustoMp;

            if (podePagar
                && especial.Efeitos.Any(efeito => efeito is EfeitoCura)
                && !motor.Combatentes.Any(item =>
                    item.EstaVivo
                    && item.Definicao.Equipe == ator.Definicao.Equipe
                    && item.HpAtual < item.Definicao.HpMax))
            {
                podePagar = false;
            }

            return podePagar ? especial : habilidades[0];
        }

        private static IEnumerable<string> EscolherAlvos(
            EstadoCombatente ator,
            DefinicaoHabilidade habilidade,
            MotorBatalha motor)
        {
            if (habilidade.TipoAlvo == TipoAlvo.TodosInimigos
                || habilidade.TipoAlvo == TipoAlvo.TodosAliados
                || habilidade.TipoAlvo == TipoAlvo.ProprioUsuario)
            {
                return Enumerable.Empty<string>();
            }

            bool selecionarAliado = habilidade.TipoAlvo == TipoAlvo.AliadoUnico;
            EstadoCombatente alvo = motor.Combatentes
                .Where(item => item.EstaVivo)
                .Where(item => selecionarAliado
                    ? item.Definicao.Equipe == ator.Definicao.Equipe
                    : item.Definicao.Equipe != ator.Definicao.Equipe)
                .OrderBy(item => item.HpAtual / (double)item.Definicao.HpMax)
                .ThenBy(item => item.HpAtual)
                .ThenBy(item => item.Id, StringComparer.Ordinal)
                .First();
            return new[] { alvo.Id };
        }
    }
}
