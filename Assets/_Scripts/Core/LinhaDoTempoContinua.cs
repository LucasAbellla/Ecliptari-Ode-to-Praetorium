using System;
using System.Collections.Generic;
using System.Linq;

namespace Ecliptari.Combate
{
    public sealed class LinhaDoTempoContinua
    {
        public const double BaseValorAcao = 10000d;
        private const double Tolerancia = 0.0000001d;

        private readonly List<EntradaLinhaTempo> entradas;

        public LinhaDoTempoContinua(IEnumerable<EstadoCombatente> combatentes)
        {
            if (combatentes == null)
            {
                throw new ArgumentNullException(nameof(combatentes));
            }

            entradas = combatentes
                .Select((combatente, indice) => new EntradaLinhaTempo(
                    combatente ?? throw new ArgumentException("A lista contém um combatente nulo.", nameof(combatentes)),
                    indice))
                .ToList();

            if (entradas.Count == 0)
            {
                throw new ArgumentException("A linha do tempo precisa de combatentes.", nameof(combatentes));
            }

            if (entradas.Select(entrada => entrada.Combatente.Id).Distinct().Count() != entradas.Count)
            {
                throw new ArgumentException("Os identificadores dos combatentes devem ser únicos.", nameof(combatentes));
            }
        }

        public EstadoCombatente SelecionarProximo()
        {
            EntradaLinhaTempo proxima = SelecionarEntrada(entradas);
            AvancarRelogio(entradas, proxima.ValorAcaoRestante);
            proxima.ValorAcaoRestante += CalcularIntervalo(proxima.Combatente.Definicao.Velocidade);
            return proxima.Combatente;
        }

        public IReadOnlyList<EstadoCombatente> PreverOrdem(int quantidade)
        {
            if (quantidade < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(quantidade));
            }

            var copia = entradas
                .Where(entrada => entrada.Combatente.EstaVivo)
                .Select(entrada => entrada.Copiar())
                .ToList();
            var ordem = new List<EstadoCombatente>(quantidade);

            for (int i = 0; i < quantidade && copia.Count > 0; i++)
            {
                EntradaLinhaTempo proxima = SelecionarEntrada(copia);
                AvancarRelogio(copia, proxima.ValorAcaoRestante);
                proxima.ValorAcaoRestante += CalcularIntervalo(proxima.Combatente.Definicao.Velocidade);
                ordem.Add(proxima.Combatente);
            }

            return ordem;
        }

        public double ObterValorAcaoRestante(string combatenteId)
        {
            EntradaLinhaTempo entrada = entradas.FirstOrDefault(item => item.Combatente.Id == combatenteId);
            if (entrada == null)
            {
                throw new KeyNotFoundException($"Combatente não encontrado: {combatenteId}");
            }

            return entrada.ValorAcaoRestante;
        }

        public static double CalcularIntervalo(int velocidade)
        {
            if (velocidade <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(velocidade));
            }

            return BaseValorAcao / velocidade;
        }

        private static EntradaLinhaTempo SelecionarEntrada(IReadOnlyCollection<EntradaLinhaTempo> origem)
        {
            EntradaLinhaTempo proxima = origem
                .Where(entrada => entrada.Combatente.EstaVivo)
                .OrderBy(entrada => entrada.ValorAcaoRestante)
                .ThenByDescending(entrada => entrada.Combatente.Definicao.Velocidade)
                .ThenBy(entrada => entrada.OrdemInicial)
                .FirstOrDefault();

            if (proxima == null)
            {
                throw new InvalidOperationException("Não existem combatentes vivos na linha do tempo.");
            }

            return proxima;
        }

        private static void AvancarRelogio(IEnumerable<EntradaLinhaTempo> origem, double valor)
        {
            foreach (EntradaLinhaTempo entrada in origem.Where(item => item.Combatente.EstaVivo))
            {
                entrada.ValorAcaoRestante -= valor;
                if (Math.Abs(entrada.ValorAcaoRestante) < Tolerancia)
                {
                    entrada.ValorAcaoRestante = 0d;
                }
            }
        }

        private sealed class EntradaLinhaTempo
        {
            public EstadoCombatente Combatente { get; }
            public int OrdemInicial { get; }
            public double ValorAcaoRestante { get; set; }

            public EntradaLinhaTempo(EstadoCombatente combatente, int ordemInicial)
            {
                Combatente = combatente;
                OrdemInicial = ordemInicial;
                ValorAcaoRestante = CalcularIntervalo(combatente.Definicao.Velocidade);
            }

            private EntradaLinhaTempo(
                EstadoCombatente combatente,
                int ordemInicial,
                double valorAcaoRestante)
            {
                Combatente = combatente;
                OrdemInicial = ordemInicial;
                ValorAcaoRestante = valorAcaoRestante;
            }

            public EntradaLinhaTempo Copiar()
            {
                return new EntradaLinhaTempo(Combatente, OrdemInicial, ValorAcaoRestante);
            }
        }
    }
}
