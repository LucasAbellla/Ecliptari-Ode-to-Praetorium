using System.Collections.Generic;
using Ecliptari.Combate;

namespace Ecliptari.Conteudo
{
    public static class CatalogoInimigosBasicos
    {
        public const string IdBatedor = "prototipo_batedor_umbra";
        public const string IdColosso = "prototipo_colosso_terrae";
        public const string IdConjurador = "prototipo_conjurador_eol";

        public const string IdAtaqueBatedor = "corte_velado";
        public const string IdHabilidadeBatedor = "assalto_sombrio";
        public const string IdAtaqueColosso = "punho_de_rocha";
        public const string IdHabilidadeColosso = "carga_fortificada";
        public const string IdAtaqueConjurador = "lamina_de_ar";
        public const string IdHabilidadeConjurador = "vendaval_hostil";

        public static IReadOnlyList<InimigoBasico> CriarGrupoDeTeste()
        {
            return new[]
            {
                CriarBatedor(),
                CriarColosso(),
                CriarConjurador()
            };
        }

        public static InimigoBasico CriarBatedor()
        {
            var combatente = new DefinicaoCombatente(
                IdBatedor,
                "Batedor Umbra",
                Equipe.Inimigos,
                Elemento.Umbra,
                TipoArma.Espada,
                TipagemPoder.Fagulha,
                hpMax: 620,
                mpMax: 45,
                apMax: 5,
                apRegen: 2,
                velocidade: 130,
                ataque: 92f,
                defesa: 35f,
                elementalizacao: 1f);

            return new InimigoBasico(
                combatente,
                PapelInimigo.Batedor,
                new[]
                {
                    new DefinicaoHabilidade(
                        IdAtaqueBatedor,
                        "Corte Velado",
                        custoAp: 0,
                        custoMp: 0,
                        poder: 0.9f,
                        TipoAlvo.InimigoUnico),
                    new DefinicaoHabilidade(
                        IdHabilidadeBatedor,
                        "Assalto Sombrio",
                        custoAp: 1,
                        custoMp: 8,
                        poder: 1.25f,
                        TipoAlvo.InimigoUnico)
                });
        }

        public static InimigoBasico CriarColosso()
        {
            var combatente = new DefinicaoCombatente(
                IdColosso,
                "Colosso Terrae",
                Equipe.Inimigos,
                Elemento.Terrae,
                TipoArma.Maca,
                TipagemPoder.Gume,
                hpMax: 1650,
                mpMax: 40,
                apMax: 6,
                apRegen: 2,
                velocidade: 70,
                ataque: 125f,
                defesa: 120f,
                elementalizacao: 1f);

            return new InimigoBasico(
                combatente,
                PapelInimigo.Colosso,
                new[]
                {
                    new DefinicaoHabilidade(
                        IdAtaqueColosso,
                        "Punho de Rocha",
                        custoAp: 0,
                        custoMp: 0,
                        poder: 1f,
                        TipoAlvo.InimigoUnico),
                    new DefinicaoHabilidade(
                        IdHabilidadeColosso,
                        "Carga Fortificada",
                        custoAp: 2,
                        custoMp: 8,
                        TipoAlvo.InimigoUnico,
                        new EfeitoHabilidade[]
                        {
                            new EfeitoDano(1.15f),
                            new EfeitoEscudo(
                                multiplicadorDefesa: 0.5f,
                                destino: DestinoEfeito.Usuario)
                        })
                });
        }

        public static InimigoBasico CriarConjurador()
        {
            var combatente = new DefinicaoCombatente(
                IdConjurador,
                "Conjurador Eol",
                Equipe.Inimigos,
                Elemento.Eol,
                TipoArma.Catalisador,
                TipagemPoder.Fagulha,
                hpMax: 840,
                mpMax: 75,
                apMax: 5,
                apRegen: 2,
                velocidade: 95,
                ataque: 102f,
                defesa: 45f,
                elementalizacao: 1.1f);

            return new InimigoBasico(
                combatente,
                PapelInimigo.Conjurador,
                new[]
                {
                    new DefinicaoHabilidade(
                        IdAtaqueConjurador,
                        "Lâmina de Ar",
                        custoAp: 0,
                        custoMp: 0,
                        poder: 0.8f,
                        TipoAlvo.InimigoUnico),
                    new DefinicaoHabilidade(
                        IdHabilidadeConjurador,
                        "Vendaval Hostil",
                        custoAp: 2,
                        custoMp: 12,
                        TipoAlvo.TodosInimigos,
                        new EfeitoHabilidade[] { new EfeitoDano(0.62f) })
                });
        }
    }
}
