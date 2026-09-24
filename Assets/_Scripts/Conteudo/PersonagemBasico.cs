using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Ecliptari.Combate;

namespace Ecliptari.Conteudo
{
    public enum PapelPersonagem
    {
        Atacante = 0,
        Defensor = 1,
        Suporte = 2,
        Especialista = 3
    }

    public sealed class PersonagemBasico
    {
        private readonly DefinicaoHabilidade[] habilidades;
        private readonly ReadOnlyCollection<DefinicaoHabilidade> habilidadesSomenteLeitura;

        public DefinicaoCombatente Combatente { get; }
        public PapelPersonagem Papel { get; }
        public IReadOnlyList<DefinicaoHabilidade> Habilidades => habilidadesSomenteLeitura;

        public PersonagemBasico(
            DefinicaoCombatente combatente,
            PapelPersonagem papel,
            IEnumerable<DefinicaoHabilidade> habilidades)
        {
            Combatente = combatente ?? throw new ArgumentNullException(nameof(combatente));
            if (habilidades == null)
            {
                throw new ArgumentNullException(nameof(habilidades));
            }

            this.habilidades = habilidades.ToArray();
            if (this.habilidades.Length == 0 || this.habilidades.Any(item => item == null))
            {
                throw new ArgumentException(
                    "Um personagem precisa de ao menos uma habilidade válida.",
                    nameof(habilidades));
            }

            if (this.habilidades.Select(item => item.Id).Distinct().Count() != this.habilidades.Length)
            {
                throw new ArgumentException(
                    "Os identificadores das habilidades devem ser únicos por personagem.",
                    nameof(habilidades));
            }

            habilidadesSomenteLeitura = Array.AsReadOnly(this.habilidades);

            Papel = papel;
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
