namespace Ecliptari.Combate
{
    public enum Elemento
    {
        Nenhum = 0,
        Flama = 1,
        Aqua = 2,
        Terrae = 3,
        Eol = 4,
        Crelix = 5,
        Fulmen = 6,
        Lux = 7,
        Umbra = 8,
        Vitae = 9,
        Toxi = 10,
        Vis = 11
    }

    public enum TipoArma
    {
        Nenhuma = 0,
        Espada = 1,
        Lanca = 2,
        Arco = 3,
        Maca = 4,
        Tomo = 5,
        Catalisador = 6
    }

    public enum TipagemPoder
    {
        Nulo = 0,
        Latente = 1,
        Fagulha = 2,
        Gume = 3,
        Tomo = 4,
        Periplo = 5,
        Atroz = 6,
        Cetro = 7,
        Diadema = 8,
        Empireo = 9,
        Estrela = 10
    }

    public enum Equipe
    {
        Aliados = 0,
        Inimigos = 1
    }

    public enum FaseCombate
    {
        Preparacao = 0,
        Manutencao = 1,
        Acao = 2,
        Resolucao = 3,
        Vitoria = 4,
        Derrota = 5
    }

    public enum TipoAlvo
    {
        InimigoUnico = 0,
        AliadoUnico = 1,
        ProprioUsuario = 2,
        QualquerEntidade = 3,
        TodosInimigos = 4,
        TodosAliados = 5
    }

    public enum DestinoEfeito
    {
        AlvosSelecionados = 0,
        Usuario = 1
    }

    public enum ErroComando
    {
        Nenhum = 0,
        BatalhaNaoIniciada = 1,
        BatalhaEncerrada = 2,
        FaseInvalida = 3,
        AtorInvalido = 4,
        AlvoInvalido = 5,
        HabilidadeInvalida = 6,
        ApInsuficiente = 7,
        MpInsuficiente = 8
    }

    public enum TipoEventoCombate
    {
        BatalhaIniciada = 0,
        FaseAlterada = 1,
        TurnoIniciado = 2,
        ApRegenerado = 3,
        ApConsumido = 4,
        MpConsumido = 5,
        HabilidadeUsada = 6,
        DanoCausado = 7,
        TurnoPassado = 8,
        CombatenteDerrotado = 9,
        TurnoEncerrado = 10,
        BatalhaEncerrada = 11,
        CuraRecebida = 12,
        EscudoConcedido = 13,
        EscudoAbsorveu = 14
    }
}
