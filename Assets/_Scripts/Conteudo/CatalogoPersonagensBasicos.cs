using System.Collections.Generic;
using Ecliptari.Combate;

namespace Ecliptari.Conteudo
{
    public static class CatalogoPersonagensBasicos
    {
        public const string IdVanguarda = "prototipo_vanguarda_flama";
        public const string IdGuardiao = "prototipo_guardiao_terrae";
        public const string IdSuporte = "prototipo_adepta_lux";
        public const string IdEspecialista = "prototipo_arcanista_fulmen";

        public const string IdAtaqueVanguarda = "golpe_candente";
        public const string IdHabilidadeVanguarda = "investida_flamejante";
        public const string IdAtaqueGuardiao = "golpe_de_pedra";
        public const string IdHabilidadeGuardiao = "muralha_terrea";
        public const string IdAtaqueSuporte = "pulso_luminoso";
        public const string IdHabilidadeSuporte = "luz_restauradora";
        public const string IdAtaqueEspecialista = "centelha_fulmen";
        public const string IdHabilidadeEspecialista = "descarga_ramificada";

        public static IReadOnlyList<PersonagemBasico> CriarEquipePadrao()
        {
            return new[]
            {
                CriarVanguarda(),
                CriarGuardiao(),
                CriarSuporte(),
                CriarEspecialista()
            };
        }

        // Preservado para cenários focados nas três interações fundamentais.
        public static IReadOnlyList<PersonagemBasico> CriarTrio()
        {
            return new[]
            {
                CriarVanguarda(),
                CriarGuardiao(),
                CriarSuporte()
            };
        }

        public static PersonagemBasico CriarVanguarda()
        {
            var combatente = new DefinicaoCombatente(
                IdVanguarda,
                "Vanguarda Flama",
                Equipe.Aliados,
                Elemento.Flama,
                TipoArma.Espada,
                TipagemPoder.Fagulha,
                hpMax: 900,
                mpMax: 60,
                apMax: 5,
                apRegen: 2,
                velocidade: 120,
                ataque: 115f,
                defesa: 55f,
                elementalizacao: 1f);

            return new PersonagemBasico(
                combatente,
                PapelPersonagem.Atacante,
                new[]
                {
                    new DefinicaoHabilidade(
                        IdAtaqueVanguarda,
                        "Golpe Candente",
                        custoAp: 0,
                        custoMp: 0,
                        poder: 1f,
                        TipoAlvo.InimigoUnico),
                    new DefinicaoHabilidade(
                        IdHabilidadeVanguarda,
                        "Investida Flamejante",
                        custoAp: 2,
                        custoMp: 10,
                        poder: 1.75f,
                        TipoAlvo.InimigoUnico)
                });
        }

        public static PersonagemBasico CriarGuardiao()
        {
            var combatente = new DefinicaoCombatente(
                IdGuardiao,
                "Guardião Terrae",
                Equipe.Aliados,
                Elemento.Terrae,
                TipoArma.Maca,
                TipagemPoder.Fagulha,
                hpMax: 1250,
                mpMax: 50,
                apMax: 6,
                apRegen: 2,
                velocidade: 90,
                ataque: 75f,
                defesa: 110f,
                elementalizacao: 1f);

            return new PersonagemBasico(
                combatente,
                PapelPersonagem.Defensor,
                new[]
                {
                    new DefinicaoHabilidade(
                        IdAtaqueGuardiao,
                        "Golpe de Pedra",
                        custoAp: 0,
                        custoMp: 0,
                        poder: 0.8f,
                        TipoAlvo.InimigoUnico),
                    new DefinicaoHabilidade(
                        IdHabilidadeGuardiao,
                        "Muralha Terrae",
                        custoAp: 2,
                        custoMp: 8,
                        TipoAlvo.AliadoUnico,
                        new EfeitoHabilidade[]
                        {
                            new EfeitoEscudo(multiplicadorDefesa: 0.6f, valorBase: 25)
                        })
                });
        }

        public static PersonagemBasico CriarSuporte()
        {
            var combatente = new DefinicaoCombatente(
                IdSuporte,
                "Adepta Lux",
                Equipe.Aliados,
                Elemento.Lux,
                TipoArma.Catalisador,
                TipagemPoder.Fagulha,
                hpMax: 800,
                mpMax: 80,
                apMax: 5,
                apRegen: 2,
                velocidade: 105,
                ataque: 95f,
                defesa: 45f,
                elementalizacao: 1f);

            return new PersonagemBasico(
                combatente,
                PapelPersonagem.Suporte,
                new[]
                {
                    new DefinicaoHabilidade(
                        IdAtaqueSuporte,
                        "Pulso Luminoso",
                        custoAp: 0,
                        custoMp: 0,
                        poder: 0.75f,
                        TipoAlvo.InimigoUnico),
                    new DefinicaoHabilidade(
                        IdHabilidadeSuporte,
                        "Luz Restauradora",
                        custoAp: 2,
                        custoMp: 12,
                        TipoAlvo.AliadoUnico,
                        new EfeitoHabilidade[]
                        {
                            new EfeitoCura(multiplicadorAtaque: 0.9f, valorBase: 35)
                        })
                });
        }

        public static PersonagemBasico CriarEspecialista()
        {
            var combatente = new DefinicaoCombatente(
                IdEspecialista,
                "Arcanista Fulmen",
                Equipe.Aliados,
                Elemento.Fulmen,
                TipoArma.Tomo,
                TipagemPoder.Fagulha,
                hpMax: 760,
                mpMax: 90,
                apMax: 5,
                apRegen: 2,
                velocidade: 110,
                ataque: 105f,
                defesa: 40f,
                elementalizacao: 1.1f);

            return new PersonagemBasico(
                combatente,
                PapelPersonagem.Especialista,
                new[]
                {
                    new DefinicaoHabilidade(
                        IdAtaqueEspecialista,
                        "Centelha Fulmen",
                        custoAp: 0,
                        custoMp: 0,
                        poder: 0.9f,
                        TipoAlvo.InimigoUnico),
                    new DefinicaoHabilidade(
                        IdHabilidadeEspecialista,
                        "Descarga Ramificada",
                        custoAp: 2,
                        custoMp: 14,
                        TipoAlvo.TodosInimigos,
                        new EfeitoHabilidade[] { new EfeitoDano(0.72f) })
                });
        }
    }
}
