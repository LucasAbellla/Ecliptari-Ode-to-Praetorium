namespace Ecliptari.Combate.Tests
{
    internal static class FixturesCombate
    {
        public static DefinicaoCombatente CriarCombatente(
            string id,
            Equipe equipe,
            int velocidade = 100,
            int hpMax = 500,
            int mpMax = 10,
            int apMax = 5,
            int apRegen = 2,
            float ataque = 100f,
            float defesa = 40f)
        {
            return new DefinicaoCombatente(
                id,
                id,
                equipe,
                Elemento.Nenhum,
                TipoArma.Nenhuma,
                TipagemPoder.Nulo,
                hpMax,
                mpMax,
                apMax,
                apRegen,
                velocidade,
                ataque,
                defesa,
                1f);
        }

        public static DefinicaoHabilidade CriarAtaque(
            int custoAp = 2,
            int custoMp = 1,
            float poder = 2f)
        {
            return new DefinicaoHabilidade(
                "ataque_teste",
                "Ataque de teste",
                custoAp,
                custoMp,
                poder,
                TipoAlvo.InimigoUnico);
        }
    }
}
