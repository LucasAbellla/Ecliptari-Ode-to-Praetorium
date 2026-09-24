using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Ecliptari.Combate
{
    public sealed class DefinicaoHabilidade
    {
        private readonly EfeitoHabilidade[] efeitos;
        private readonly ReadOnlyCollection<EfeitoHabilidade> efeitosSomenteLeitura;

        public string Id { get; }
        public string Nome { get; }
        public int CustoAp { get; }
        public int CustoMp { get; }
        public TipoAlvo TipoAlvo { get; }
        public IReadOnlyList<EfeitoHabilidade> Efeitos => efeitosSomenteLeitura;

        // Mantém compatibilidade com o primeiro protótipo e ferramentas de inspeção.
        public float Poder
        {
            get
            {
                EfeitoDano primeiroDano = efeitos.OfType<EfeitoDano>().FirstOrDefault();
                return primeiroDano?.Poder ?? 0f;
            }
        }

        public DefinicaoHabilidade(
            string id,
            string nome,
            int custoAp,
            int custoMp,
            float poder,
            TipoAlvo tipoAlvo)
            : this(
                id,
                nome,
                custoAp,
                custoMp,
                tipoAlvo,
                new EfeitoHabilidade[] { new EfeitoDano(poder) })
        {
        }

        public DefinicaoHabilidade(
            string id,
            string nome,
            int custoAp,
            int custoMp,
            TipoAlvo tipoAlvo,
            IEnumerable<EfeitoHabilidade> efeitos)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("A habilidade precisa de um identificador.", nameof(id));
            }

            if (string.IsNullOrWhiteSpace(nome))
            {
                throw new ArgumentException("A habilidade precisa de um nome.", nameof(nome));
            }

            if (custoAp < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(custoAp));
            }

            if (custoMp < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(custoMp));
            }

            if (!Enum.IsDefined(typeof(TipoAlvo), tipoAlvo))
            {
                throw new ArgumentOutOfRangeException(nameof(tipoAlvo));
            }

            if (efeitos == null)
            {
                throw new ArgumentNullException(nameof(efeitos));
            }

            this.efeitos = efeitos.ToArray();
            if (this.efeitos.Length == 0 || this.efeitos.Any(efeito => efeito == null))
            {
                throw new ArgumentException(
                    "A habilidade precisa de ao menos um efeito válido.",
                    nameof(efeitos));
            }

            efeitosSomenteLeitura = Array.AsReadOnly(this.efeitos);

            Id = id;
            Nome = nome;
            CustoAp = custoAp;
            CustoMp = custoMp;
            TipoAlvo = tipoAlvo;
        }
    }
}
