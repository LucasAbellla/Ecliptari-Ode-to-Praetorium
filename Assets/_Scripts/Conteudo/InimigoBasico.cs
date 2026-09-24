using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Ecliptari.Combate;

namespace Ecliptari.Conteudo
{
    public enum PapelInimigo
    {
        Batedor = 0,
        Colosso = 1,
        Conjurador = 2
    }

    public sealed class InimigoBasico
    {
        private readonly DefinicaoHabilidade[] habilidades;
        private readonly ReadOnlyCollection<DefinicaoHabilidade> habilidadesSomenteLeitura;

        public DefinicaoCombatente Combatente { get; }
        public PapelInimigo Papel { get; }
        public IReadOnlyList<DefinicaoHabilidade> Habilidades => habilidadesSomenteLeitura;

        public InimigoBasico(
            DefinicaoCombatente combatente,
            PapelInimigo papel,
            IEnumerable<DefinicaoHabilidade> habilidades)
        {
            Combatente = combatente ?? throw new ArgumentNullException(nameof(combatente));
            if (combatente.Equipe != Equipe.Inimigos)
            {
                throw new ArgumentException("Um inimigo precisa pertencer à equipe inimiga.", nameof(combatente));
            }

            if (!Enum.IsDefined(typeof(PapelInimigo), papel))
            {
                throw new ArgumentOutOfRangeException(nameof(papel));
            }

            if (habilidades == null)
            {
                throw new ArgumentNullException(nameof(habilidades));
            }

            this.habilidades = habilidades.ToArray();
            if (this.habilidades.Length == 0 || this.habilidades.Any(item => item == null))
            {
                throw new ArgumentException(
                    "Um inimigo precisa de ao menos uma habilidade válida.",
                    nameof(habilidades));
            }

            if (this.habilidades.Select(item => item.Id).Distinct().Count() != this.habilidades.Length)
            {
                throw new ArgumentException(
                    "Os identificadores das habilidades devem ser únicos por inimigo.",
                    nameof(habilidades));
            }

            Papel = papel;
            habilidadesSomenteLeitura = Array.AsReadOnly(this.habilidades);
        }

        public DefinicaoHabilidade ObterHabilidade(string id)
        {
            DefinicaoHabilidade habilidade = habilidades.FirstOrDefault(item => item.Id == id);
            if (habilidade == null)
            {
                throw new KeyNotFoundException($"Habilidade não encontrada: {id}");
            }

            return habilidade;
        }
    }
}
