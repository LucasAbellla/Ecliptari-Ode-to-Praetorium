namespace Ecliptari.Combate
{
    public sealed class EventoCombate
    {
        public TipoEventoCombate Tipo { get; }
        public string AtorId { get; }
        public string AlvoId { get; }
        public string ReferenciaId { get; }
        public int Valor { get; }
        public FaseCombate Fase { get; }

        public EventoCombate(
            TipoEventoCombate tipo,
            string atorId = null,
            string alvoId = null,
            string referenciaId = null,
            int valor = 0,
            FaseCombate fase = FaseCombate.Preparacao)
        {
            Tipo = tipo;
            AtorId = atorId;
            AlvoId = alvoId;
            ReferenciaId = referenciaId;
            Valor = valor;
            Fase = fase;
        }
    }
}
