namespace Ecliptari.Combate
{
    public sealed class ResultadoComando
    {
        public bool Executado { get; }
        public ErroComando Erro { get; }

        private ResultadoComando(bool executado, ErroComando erro)
        {
            Executado = executado;
            Erro = erro;
        }

        public static ResultadoComando Sucesso()
        {
            return new ResultadoComando(true, ErroComando.Nenhum);
        }

        public static ResultadoComando Falha(ErroComando erro)
        {
            return new ResultadoComando(false, erro);
        }
    }
}
