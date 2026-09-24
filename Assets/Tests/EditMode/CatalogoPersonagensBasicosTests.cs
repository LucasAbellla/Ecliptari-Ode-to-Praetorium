using System.Linq;
using Ecliptari.Conteudo;
using NUnit.Framework;

namespace Ecliptari.Combate.Tests
{
    public sealed class CatalogoPersonagensBasicosTests
    {
        [Test]
        public void Catalogo_PossuiEquipePadraoComIdsUnicos()
        {
            var personagens = CatalogoPersonagensBasicos.CriarEquipePadrao();

            Assert.That(personagens.Count, Is.EqualTo(RegrasFormacao.TamanhoPadraoEquipe));
            Assert.That(
                personagens.Select(item => item.Combatente.Id).Distinct().Count(),
                Is.EqualTo(RegrasFormacao.TamanhoPadraoEquipe));
        }

        [Test]
        public void Catalogo_NaoIncluiRosalia()
        {
            var personagens = CatalogoPersonagensBasicos.CriarEquipePadrao();

            Assert.That(
                personagens.Any(item =>
                    item.Combatente.Id.ToLowerInvariant().Contains("rosalia")
                    || item.Combatente.Nome.ToLowerInvariant().Contains("rosalia")),
                Is.False);
        }

        [Test]
        public void Equipe_CobreAtaqueDefesaSuporteEEspecialista()
        {
            var personagens = CatalogoPersonagensBasicos.CriarEquipePadrao();

            Assert.That(personagens.Any(item => item.Papel == PapelPersonagem.Atacante), Is.True);
            Assert.That(personagens.Any(item => item.Papel == PapelPersonagem.Defensor), Is.True);
            Assert.That(personagens.Any(item => item.Papel == PapelPersonagem.Suporte), Is.True);
            Assert.That(personagens.Any(item => item.Papel == PapelPersonagem.Especialista), Is.True);
        }

        [Test]
        public void CadaPersonagem_PossuiAtaqueEHabilidade()
        {
            var personagens = CatalogoPersonagensBasicos.CriarEquipePadrao();

            Assert.That(personagens.All(item => item.Habilidades.Count == 2), Is.True);
        }
    }
}
