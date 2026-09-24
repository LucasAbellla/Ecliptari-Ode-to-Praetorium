using System.Linq;
using Ecliptari.Conteudo;
using NUnit.Framework;

namespace Ecliptari.Combate.Tests
{
    public sealed class CatalogoInimigosBasicosTests
    {
        [Test]
        public void Catalogo_PossuiTresInimigosComIdsUnicos()
        {
            var inimigos = CatalogoInimigosBasicos.CriarGrupoDeTeste();

            Assert.That(inimigos.Count, Is.EqualTo(3));
            Assert.That(
                inimigos.Select(item => item.Combatente.Id).Distinct().Count(),
                Is.EqualTo(3));
            Assert.That(inimigos.All(item => item.Combatente.Equipe == Equipe.Inimigos), Is.True);
        }

        [Test]
        public void Grupo_CobreTresPapeisDiferentes()
        {
            var inimigos = CatalogoInimigosBasicos.CriarGrupoDeTeste();

            Assert.That(inimigos.Any(item => item.Papel == PapelInimigo.Batedor), Is.True);
            Assert.That(inimigos.Any(item => item.Papel == PapelInimigo.Colosso), Is.True);
            Assert.That(inimigos.Any(item => item.Papel == PapelInimigo.Conjurador), Is.True);
        }

        [Test]
        public void Inimigos_PossuemAtaqueEHabilidadeEspecial()
        {
            var inimigos = CatalogoInimigosBasicos.CriarGrupoDeTeste();

            Assert.That(inimigos.All(item => item.Habilidades.Count == 2), Is.True);
            Assert.That(inimigos.All(item => item.Habilidades[0].CustoAp == 0), Is.True);
            Assert.That(inimigos.All(item => item.Habilidades[1].CustoAp > 0), Is.True);
        }
    }
}
