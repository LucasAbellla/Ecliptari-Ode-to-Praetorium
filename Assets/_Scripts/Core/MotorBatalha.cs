using System;
using System.Collections.Generic;
using System.Linq;

namespace Ecliptari.Combate
{
    public sealed class MotorBatalha
    {
        private readonly Dictionary<string, EstadoCombatente> combatentes;
        private readonly LinhaDoTempoContinua linhaDoTempo;
        private readonly List<EventoCombate> eventos = new List<EventoCombate>();

        public FaseCombate Fase { get; private set; } = FaseCombate.Preparacao;
        public EstadoCombatente AtorAtual { get; private set; }
        public bool FoiIniciada { get; private set; }
        public bool FoiEncerrada => Fase == FaseCombate.Vitoria || Fase == FaseCombate.Derrota;
        public IReadOnlyCollection<EstadoCombatente> Combatentes => combatentes.Values;

        public MotorBatalha(IEnumerable<DefinicaoCombatente> definicoes)
        {
            if (definicoes == null)
            {
                throw new ArgumentNullException(nameof(definicoes));
            }

            List<EstadoCombatente> estados = definicoes
                .Select(definicao => new EstadoCombatente(
                    definicao ?? throw new ArgumentException("A lista contém uma definição nula.", nameof(definicoes))))
                .ToList();

            if (estados.Count < 2)
            {
                throw new ArgumentException("A batalha precisa de ao menos dois combatentes.", nameof(definicoes));
            }

            if (estados.Select(estado => estado.Id).Distinct().Count() != estados.Count)
            {
                throw new ArgumentException("Os identificadores dos combatentes devem ser únicos.", nameof(definicoes));
            }

            if (!estados.Any(estado => estado.Definicao.Equipe == Equipe.Aliados)
                || !estados.Any(estado => estado.Definicao.Equipe == Equipe.Inimigos))
            {
                throw new ArgumentException("A batalha precisa de ao menos um aliado e um inimigo.", nameof(definicoes));
            }

            combatentes = estados.ToDictionary(estado => estado.Id);
            linhaDoTempo = new LinhaDoTempoContinua(estados);
        }

        public void Iniciar()
        {
            if (FoiIniciada)
            {
                throw new InvalidOperationException("A batalha já foi iniciada.");
            }

            FoiIniciada = true;
            eventos.Add(new EventoCombate(TipoEventoCombate.BatalhaIniciada));
            PrepararProximoTurno();
        }

        public ResultadoComando UsarHabilidade(
            string atorId,
            string alvoId,
            DefinicaoHabilidade habilidade)
        {
            IEnumerable<string> alvos = alvoId == null
                ? Enumerable.Empty<string>()
                : new[] { alvoId };
            return UsarHabilidade(atorId, alvos, habilidade);
        }

        public ResultadoComando UsarHabilidade(
            string atorId,
            IEnumerable<string> alvoIds,
            DefinicaoHabilidade habilidade)
        {
            ResultadoComando validacaoBase = ValidarComandoDoAtor(atorId);
            if (!validacaoBase.Executado)
            {
                return validacaoBase;
            }

            if (habilidade == null)
            {
                return ResultadoComando.Falha(ErroComando.HabilidadeInvalida);
            }

            if (!TentarResolverAlvos(alvoIds, habilidade.TipoAlvo, out List<EstadoCombatente> alvos))
            {
                return ResultadoComando.Falha(ErroComando.AlvoInvalido);
            }

            if (AtorAtual.ApAtual < habilidade.CustoAp)
            {
                return ResultadoComando.Falha(ErroComando.ApInsuficiente);
            }

            if (AtorAtual.MpAtual < habilidade.CustoMp)
            {
                return ResultadoComando.Falha(ErroComando.MpInsuficiente);
            }

            AtorAtual.ConsumirAp(habilidade.CustoAp);
            AtorAtual.ConsumirMp(habilidade.CustoMp);
            eventos.Add(new EventoCombate(
                TipoEventoCombate.ApConsumido,
                AtorAtual.Id,
                valor: habilidade.CustoAp));
            eventos.Add(new EventoCombate(
                TipoEventoCombate.MpConsumido,
                AtorAtual.Id,
                valor: habilidade.CustoMp));
            eventos.Add(new EventoCombate(
                TipoEventoCombate.HabilidadeUsada,
                AtorAtual.Id,
                alvos.Count == 1 ? alvos[0].Id : null,
                habilidade.Id));

            AlterarFase(FaseCombate.Resolucao);

            var vivosAntes = new HashSet<string>(combatentes.Values
                .Where(combatente => combatente.EstaVivo)
                .Select(combatente => combatente.Id));

            foreach (EfeitoHabilidade efeito in habilidade.Efeitos)
            {
                IEnumerable<EstadoCombatente> destinos = efeito.Destino == DestinoEfeito.Usuario
                    ? new[] { AtorAtual }
                    : alvos;

                foreach (EstadoCombatente destino in destinos)
                {
                    var contexto = new ContextoEfeito(
                        AtorAtual,
                        destino,
                        habilidade.Id,
                        evento => eventos.Add(evento));
                    efeito.Resolver(contexto);
                }
            }

            foreach (EstadoCombatente derrotado in combatentes.Values.Where(combatente =>
                vivosAntes.Contains(combatente.Id) && !combatente.EstaVivo))
            {
                eventos.Add(new EventoCombate(
                    TipoEventoCombate.CombatenteDerrotado,
                    AtorAtual.Id,
                    derrotado.Id,
                    habilidade.Id));
            }

            return ResultadoComando.Sucesso();
        }

        public ResultadoComando PassarTurno(string atorId)
        {
            ResultadoComando validacao = ValidarComandoDoAtor(atorId);
            if (!validacao.Executado)
            {
                return validacao;
            }

            eventos.Add(new EventoCombate(TipoEventoCombate.TurnoPassado, AtorAtual.Id));
            AlterarFase(FaseCombate.Resolucao);
            return ResultadoComando.Sucesso();
        }

        public void ConcluirResolucao()
        {
            if (Fase != FaseCombate.Resolucao)
            {
                throw new InvalidOperationException("Não existe uma resolução de ação em andamento.");
            }

            string atorEncerrado = AtorAtual.Id;
            eventos.Add(new EventoCombate(TipoEventoCombate.TurnoEncerrado, atorEncerrado));

            if (VerificarEncerramento())
            {
                AtorAtual = null;
                return;
            }

            PrepararProximoTurno();
        }

        public EstadoCombatente ObterCombatente(string id)
        {
            if (!combatentes.TryGetValue(id ?? string.Empty, out EstadoCombatente combatente))
            {
                throw new KeyNotFoundException($"Combatente não encontrado: {id}");
            }

            return combatente;
        }

        public IReadOnlyList<EstadoCombatente> PreverProximosTurnos(int quantidade)
        {
            return linhaDoTempo.PreverOrdem(quantidade);
        }

        public IReadOnlyList<EventoCombate> DrenarEventos()
        {
            EventoCombate[] copia = eventos.ToArray();
            eventos.Clear();
            return copia;
        }

        private void PrepararProximoTurno()
        {
            AlterarFase(FaseCombate.Manutencao);
            AtorAtual = linhaDoTempo.SelecionarProximo();
            eventos.Add(new EventoCombate(TipoEventoCombate.TurnoIniciado, AtorAtual.Id));

            int apRegenerado = AtorAtual.RegenerarAp();
            if (apRegenerado > 0)
            {
                eventos.Add(new EventoCombate(
                    TipoEventoCombate.ApRegenerado,
                    AtorAtual.Id,
                    valor: apRegenerado));
            }

            AlterarFase(FaseCombate.Acao);
        }

        private ResultadoComando ValidarComandoDoAtor(string atorId)
        {
            if (!FoiIniciada)
            {
                return ResultadoComando.Falha(ErroComando.BatalhaNaoIniciada);
            }

            if (FoiEncerrada)
            {
                return ResultadoComando.Falha(ErroComando.BatalhaEncerrada);
            }

            if (Fase != FaseCombate.Acao)
            {
                return ResultadoComando.Falha(ErroComando.FaseInvalida);
            }

            if (AtorAtual == null || !AtorAtual.EstaVivo || AtorAtual.Id != atorId)
            {
                return ResultadoComando.Falha(ErroComando.AtorInvalido);
            }

            return ResultadoComando.Sucesso();
        }

        private bool VerificarEncerramento()
        {
            bool existemAliados = combatentes.Values.Any(combatente =>
                combatente.EstaVivo && combatente.Definicao.Equipe == Equipe.Aliados);
            bool existemInimigos = combatentes.Values.Any(combatente =>
                combatente.EstaVivo && combatente.Definicao.Equipe == Equipe.Inimigos);

            if (!existemAliados)
            {
                AlterarFase(FaseCombate.Derrota);
                eventos.Add(new EventoCombate(
                    TipoEventoCombate.BatalhaEncerrada,
                    referenciaId: Equipe.Inimigos.ToString(),
                    fase: Fase));
                return true;
            }

            if (!existemInimigos)
            {
                AlterarFase(FaseCombate.Vitoria);
                eventos.Add(new EventoCombate(
                    TipoEventoCombate.BatalhaEncerrada,
                    referenciaId: Equipe.Aliados.ToString(),
                    fase: Fase));
                return true;
            }

            return false;
        }

        private void AlterarFase(FaseCombate novaFase)
        {
            Fase = novaFase;
            eventos.Add(new EventoCombate(
                TipoEventoCombate.FaseAlterada,
                AtorAtual?.Id,
                fase: novaFase));
        }

        private static bool AlvoEhValido(
            EstadoCombatente ator,
            EstadoCombatente alvo,
            TipoAlvo tipoAlvo)
        {
            switch (tipoAlvo)
            {
                case TipoAlvo.InimigoUnico:
                    return ator.Definicao.Equipe != alvo.Definicao.Equipe;
                case TipoAlvo.AliadoUnico:
                    return ator.Definicao.Equipe == alvo.Definicao.Equipe;
                case TipoAlvo.ProprioUsuario:
                    return ator.Id == alvo.Id;
                case TipoAlvo.QualquerEntidade:
                    return true;
                default:
                    return false;
            }
        }

        private bool TentarResolverAlvos(
            IEnumerable<string> alvoIds,
            TipoAlvo tipoAlvo,
            out List<EstadoCombatente> alvos)
        {
            alvos = null;
            List<string> ids = (alvoIds ?? Enumerable.Empty<string>()).ToList();

            if (ids.Any(string.IsNullOrWhiteSpace) || ids.Distinct().Count() != ids.Count)
            {
                return false;
            }

            if (tipoAlvo == TipoAlvo.TodosInimigos || tipoAlvo == TipoAlvo.TodosAliados)
            {
                if (ids.Count != 0)
                {
                    return false;
                }

                bool buscarAliados = tipoAlvo == TipoAlvo.TodosAliados;
                alvos = combatentes.Values
                    .Where(combatente => combatente.EstaVivo)
                    .Where(combatente => buscarAliados
                        ? combatente.Definicao.Equipe == AtorAtual.Definicao.Equipe
                        : combatente.Definicao.Equipe != AtorAtual.Definicao.Equipe)
                    .ToList();
                return alvos.Count > 0;
            }

            if (tipoAlvo == TipoAlvo.ProprioUsuario && ids.Count == 0)
            {
                alvos = new List<EstadoCombatente> { AtorAtual };
                return true;
            }

            if (ids.Count != 1
                || !combatentes.TryGetValue(ids[0], out EstadoCombatente alvo)
                || !alvo.EstaVivo
                || !AlvoEhValido(AtorAtual, alvo, tipoAlvo))
            {
                return false;
            }

            alvos = new List<EstadoCombatente> { alvo };
            return true;
        }
    }
}
