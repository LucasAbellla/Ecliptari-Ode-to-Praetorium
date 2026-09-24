using System;

namespace Ecliptari.Combate
{
    public abstract class EfeitoHabilidade
    {
        public DestinoEfeito Destino { get; }

        protected EfeitoHabilidade(DestinoEfeito destino)
        {
            if (!Enum.IsDefined(typeof(DestinoEfeito), destino))
            {
                throw new ArgumentOutOfRangeException(nameof(destino));
            }

            Destino = destino;
        }

        public abstract void Resolver(ContextoEfeito contexto);

        protected static void ValidarMultiplicador(float valor, string parametro)
        {
            if (float.IsNaN(valor) || float.IsInfinity(valor) || valor < 0f)
            {
                throw new ArgumentOutOfRangeException(
                    parametro,
                    "O multiplicador deve ser finito e não negativo.");
            }
        }
    }

    public sealed class EfeitoDano : EfeitoHabilidade
    {
        public float Poder { get; }

        public EfeitoDano(
            float poder,
            DestinoEfeito destino = DestinoEfeito.AlvosSelecionados)
            : base(destino)
        {
            ValidarMultiplicador(poder, nameof(poder));
            Poder = poder;
        }

        public override void Resolver(ContextoEfeito contexto)
        {
            contexto.CausarDano(Poder);
        }
    }

    public sealed class EfeitoCura : EfeitoHabilidade
    {
        public float MultiplicadorAtaque { get; }
        public int ValorBase { get; }

        public EfeitoCura(
            float multiplicadorAtaque,
            int valorBase = 0,
            DestinoEfeito destino = DestinoEfeito.AlvosSelecionados)
            : base(destino)
        {
            ValidarMultiplicador(multiplicadorAtaque, nameof(multiplicadorAtaque));
            if (valorBase < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(valorBase));
            }

            MultiplicadorAtaque = multiplicadorAtaque;
            ValorBase = valorBase;
        }

        public override void Resolver(ContextoEfeito contexto)
        {
            contexto.Curar(MultiplicadorAtaque, ValorBase);
        }
    }

    public sealed class EfeitoEscudo : EfeitoHabilidade
    {
        public float MultiplicadorDefesa { get; }
        public int ValorBase { get; }

        public EfeitoEscudo(
            float multiplicadorDefesa,
            int valorBase = 0,
            DestinoEfeito destino = DestinoEfeito.AlvosSelecionados)
            : base(destino)
        {
            ValidarMultiplicador(multiplicadorDefesa, nameof(multiplicadorDefesa));
            if (valorBase < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(valorBase));
            }

            MultiplicadorDefesa = multiplicadorDefesa;
            ValorBase = valorBase;
        }

        public override void Resolver(ContextoEfeito contexto)
        {
            contexto.ConcederEscudo(MultiplicadorDefesa, ValorBase);
        }
    }

    public sealed class ContextoEfeito
    {
        private readonly Action<EventoCombate> registrarEvento;

        public EstadoCombatente Usuario { get; }
        public EstadoCombatente Alvo { get; }
        public string HabilidadeId { get; }

        internal ContextoEfeito(
            EstadoCombatente usuario,
            EstadoCombatente alvo,
            string habilidadeId,
            Action<EventoCombate> registrarEvento)
        {
            Usuario = usuario ?? throw new ArgumentNullException(nameof(usuario));
            Alvo = alvo ?? throw new ArgumentNullException(nameof(alvo));
            HabilidadeId = habilidadeId ?? throw new ArgumentNullException(nameof(habilidadeId));
            this.registrarEvento = registrarEvento
                ?? throw new ArgumentNullException(nameof(registrarEvento));
        }

        public ResultadoDano CausarDano(float poder)
        {
            int danoCalculado = CalculadoraDano.CalcularDanoDireto(
                Usuario.Definicao.Ataque,
                poder,
                Alvo.Definicao.Defesa);
            ResultadoDano resultado = Alvo.ReceberDano(danoCalculado);

            if (resultado.AbsorvidoPeloEscudo > 0)
            {
                registrarEvento(new EventoCombate(
                    TipoEventoCombate.EscudoAbsorveu,
                    Usuario.Id,
                    Alvo.Id,
                    HabilidadeId,
                    resultado.AbsorvidoPeloEscudo));
            }

            registrarEvento(new EventoCombate(
                TipoEventoCombate.DanoCausado,
                Usuario.Id,
                Alvo.Id,
                HabilidadeId,
                resultado.DanoNoHp));
            return resultado;
        }

        public int Curar(float multiplicadorAtaque, int valorBase = 0)
        {
            int valorCalculado = CalcularMagnitude(
                Usuario.Definicao.Ataque,
                multiplicadorAtaque,
                valorBase);
            int curaAplicada = Alvo.Curar(valorCalculado);
            registrarEvento(new EventoCombate(
                TipoEventoCombate.CuraRecebida,
                Usuario.Id,
                Alvo.Id,
                HabilidadeId,
                curaAplicada));
            return curaAplicada;
        }

        public int ConcederEscudo(float multiplicadorDefesa, int valorBase = 0)
        {
            int valorCalculado = CalcularMagnitude(
                Usuario.Definicao.Defesa,
                multiplicadorDefesa,
                valorBase);
            int escudoAplicado = Alvo.ConcederEscudo(valorCalculado);
            registrarEvento(new EventoCombate(
                TipoEventoCombate.EscudoConcedido,
                Usuario.Id,
                Alvo.Id,
                HabilidadeId,
                escudoAplicado));
            return escudoAplicado;
        }

        private static int CalcularMagnitude(float atributo, float multiplicador, int valorBase)
        {
            if (float.IsNaN(multiplicador) || float.IsInfinity(multiplicador) || multiplicador < 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(multiplicador));
            }

            if (valorBase < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(valorBase));
            }

            double total = ((double)atributo * multiplicador) + valorBase;
            if (total >= int.MaxValue)
            {
                return int.MaxValue;
            }

            return (int)Math.Round(total, MidpointRounding.AwayFromZero);
        }
    }
}
