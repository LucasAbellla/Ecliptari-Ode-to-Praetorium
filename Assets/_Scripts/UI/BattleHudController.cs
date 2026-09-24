using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Ecliptari.Combate;
using Ecliptari.Conteudo;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Ecliptari.UI
{
    [DisallowMultipleComponent]
    public sealed class BattleHudController : MonoBehaviour
    {
        private readonly Dictionary<string, List<CartaoCombatente>> cartoes =
            new Dictionary<string, List<CartaoCombatente>>();
        private readonly Dictionary<string, IReadOnlyList<DefinicaoHabilidade>> habilidades =
            new Dictionary<string, IReadOnlyList<DefinicaoHabilidade>>();
        private readonly Queue<string> historico = new Queue<string>();

        private MotorBatalha motor;
        private Canvas canvas;
        private RectTransform campoRaiz;
        private RectTransform linhaTempoRaiz;
        private RectTransform faixaTurno;
        private RectTransform painelResultado;
        private CanvasGroup faixaTurnoGrupo;
        private CanvasGroup flashGrupo;
        private TextMeshProUGUI tituloAtor;
        private TextMeshProUGUI detalheAtor;
        private TextMeshProUGUI textoAlvo;
        private TextMeshProUGUI textoHistorico;
        private TextMeshProUGUI textoResultado;
        private TextMeshProUGUI textoContagem;
        private readonly Button[] botoesHabilidade = new Button[2];
        private readonly TextMeshProUGUI[] titulosHabilidade = new TextMeshProUGUI[2];
        private readonly TextMeshProUGUI[] custosHabilidade = new TextMeshProUGUI[2];
        private Button botaoPassar;
        private string alvoSelecionadoId;
        private bool resolvendo;
        private Coroutine rotinaInimigo;

        private void Awake()
        {
            DesativarInterfaceAnterior();
            ConstruirInterface();
            CriarConfronto();
        }

        private void Start()
        {
            motor.Iniciar();
            motor.DrenarEventos();
            SelecionarAlvoPadrao();
            AtualizarTudo(imediato: true);
            MostrarFaixaDoTurno();
            PrepararControleDoTurno();
        }

        private void DesativarInterfaceAnterior()
        {
            foreach (Canvas canvasExistente in FindObjectsByType<Canvas>(FindObjectsSortMode.None))
            {
                if (canvasExistente.gameObject.name == "Canvas")
                {
                    canvasExistente.gameObject.SetActive(false);
                }
            }
        }

        private void CriarConfronto()
        {
            IReadOnlyList<PersonagemBasico> aliados = CatalogoPersonagensBasicos.CriarEquipePadrao();
            IReadOnlyList<InimigoBasico> inimigos = CatalogoInimigosBasicos.CriarGrupoDeTeste();

            foreach (PersonagemBasico aliado in aliados)
            {
                habilidades.Add(aliado.Combatente.Id, aliado.Habilidades);
            }

            foreach (InimigoBasico inimigo in inimigos)
            {
                habilidades.Add(inimigo.Combatente.Id, inimigo.Habilidades);
            }

            motor = new MotorBatalha(
                aliados.Select(item => item.Combatente)
                    .Concat(inimigos.Select(item => item.Combatente)));
            CriarFormacoesVisuais(aliados, inimigos);
        }

        private void ConstruirInterface()
        {
            GameObject canvasObjeto = new GameObject(
                "Ecliptari_HUD",
                typeof(RectTransform),
                typeof(Canvas),
                typeof(CanvasScaler),
                typeof(GraphicRaycaster));
            canvasObjeto.layer = 5;
            canvas = canvasObjeto.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 50;

            CanvasScaler scaler = canvasObjeto.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;

            RectTransform raiz = canvasObjeto.GetComponent<RectTransform>();
            HudFactory.CriarGradiente(
                "Fundo Astral",
                raiz,
                HudTheme.ComAlpha(HudTheme.PainelElevado, 0.97f),
                HudTheme.FundoProfundo);
            CriarOrnamentosDeFundo(raiz);
            CriarCabecalho(raiz);
            CriarLinhaDoTempo(raiz);
            CriarCampo(raiz);
            CriarPainelDeAcoes(raiz);
            CriarHistorico(raiz);
            CriarFaixaDeTurno(raiz);
            CriarFlash(raiz);
            CriarPainelDeResultado(raiz);
        }

        private void CriarOrnamentosDeFundo(RectTransform raiz)
        {
            Image halo = HudFactory.CriarPainel(
                "Halo Central",
                raiz,
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                new Vector2(70f, 30f),
                new Vector2(860f, 560f),
                HudTheme.ComAlpha(HudTheme.Ciano, 0.035f));
            halo.transform.localRotation = Quaternion.Euler(0f, 0f, -7f);
            HudFactory.AdicionarContorno(
                halo.gameObject,
                HudTheme.ComAlpha(HudTheme.Ciano, 0.1f),
                new Vector2(2f, -2f));

            for (int i = 0; i < 6; i++)
            {
                Image linha = HudFactory.CriarPainel(
                    "Linha Astral " + i,
                    raiz,
                    new Vector2(0.5f, 0.5f),
                    new Vector2(0.5f, 0.5f),
                    new Vector2(0.5f, 0.5f),
                    new Vector2(80f, -260f + (i * 104f)),
                    new Vector2(980f - (i * 55f), 1f),
                    HudTheme.ComAlpha(i % 2 == 0 ? HudTheme.Ciano : HudTheme.Ouro, 0.13f));
                linha.transform.localRotation = Quaternion.Euler(0f, 0f, -8f);
            }
        }

        private void CriarCabecalho(RectTransform raiz)
        {
            Image painel = HudFactory.CriarPainel(
                "Cabeçalho",
                raiz,
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 1f),
                new Vector2(0f, -24f),
                new Vector2(760f, 82f),
                HudTheme.ComAlpha(HudTheme.Painel, 0.95f));
            HudFactory.AdicionarContorno(
                painel.gameObject,
                HudTheme.ComAlpha(HudTheme.Ouro, 0.55f),
                new Vector2(1f, -1f));
            HudFactory.CriarPainel(
                "Acento",
                painel.transform,
                new Vector2(0.5f, 0f),
                new Vector2(0.5f, 0f),
                new Vector2(0.5f, 0f),
                Vector2.zero,
                new Vector2(520f, 3f),
                HudTheme.Ouro);
            HudFactory.CriarTexto(
                "Titulo",
                painel.transform,
                "ODE TO PRAETORIUM",
                27f,
                HudTheme.Texto,
                TextAlignmentOptions.Center,
                new Vector2(0f, 0.42f),
                Vector2.one,
                new Vector2(0.5f, 0.5f),
                Vector2.zero,
                Vector2.zero,
                FontStyles.Bold);
            HudFactory.CriarTexto(
                "Subtitulo",
                painel.transform,
                "SIMULAÇÃO TÁTICA  //  FORMAÇÃO 04 × AMEAÇAS 03",
                12f,
                HudTheme.Ouro,
                TextAlignmentOptions.Center,
                Vector2.zero,
                new Vector2(1f, 0.42f),
                new Vector2(0.5f, 0.5f),
                Vector2.zero,
                Vector2.zero);

            TextMeshProUGUI tituloReinicio;
            TextMeshProUGUI custoReinicio;
            Button reiniciar = HudFactory.CriarBotao(
                "Reiniciar",
                raiz,
                new Vector2(0f, 0f),
                new Vector2(148f, 46f),
                HudTheme.PainelElevado,
                out tituloReinicio,
                out custoReinicio);
            RectTransform reiniciarRect = reiniciar.GetComponent<RectTransform>();
            reiniciarRect.anchorMin = new Vector2(1f, 1f);
            reiniciarRect.anchorMax = new Vector2(1f, 1f);
            reiniciarRect.pivot = new Vector2(1f, 1f);
            reiniciarRect.anchoredPosition = new Vector2(-24f, -25f);
            tituloReinicio.text = "REINICIAR";
            tituloReinicio.alignment = TextAlignmentOptions.Center;
            tituloReinicio.rectTransform.offsetMin = Vector2.zero;
            tituloReinicio.rectTransform.offsetMax = Vector2.zero;
            custoReinicio.gameObject.SetActive(false);
            reiniciar.onClick.AddListener(ReiniciarCena);

            textoContagem = HudFactory.CriarTexto(
                "Contagem",
                raiz,
                "ALIADOS 04  //  INIMIGOS 03",
                13f,
                HudTheme.TextoSecundario,
                TextAlignmentOptions.TopRight,
                new Vector2(1f, 1f),
                new Vector2(1f, 1f),
                new Vector2(1f, 1f),
                new Vector2(-24f, -80f),
                new Vector2(310f, 24f));
        }

        private void CriarLinhaDoTempo(RectTransform raiz)
        {
            Image painel = HudFactory.CriarPainel(
                "Linha do Tempo",
                raiz,
                new Vector2(0f, 1f),
                new Vector2(0f, 1f),
                new Vector2(0f, 1f),
                new Vector2(24f, -24f),
                new Vector2(252f, 760f),
                HudTheme.ComAlpha(HudTheme.Painel, 0.93f));
            HudFactory.AdicionarContorno(
                painel.gameObject,
                HudTheme.ComAlpha(HudTheme.Ciano, 0.35f),
                new Vector2(1f, -1f));
            HudFactory.CriarTexto(
                "Rotulo",
                painel.transform,
                "ORDEM DE AÇÃO",
                15f,
                HudTheme.Ciano,
                TextAlignmentOptions.Center,
                new Vector2(0f, 1f),
                Vector2.one,
                new Vector2(0.5f, 1f),
                new Vector2(0f, -20f),
                new Vector2(-24f, 32f),
                FontStyles.Bold);
            linhaTempoRaiz = HudFactory.CriarRect(
                "Entradas",
                painel.transform,
                new Vector2(0f, 1f),
                Vector2.one,
                new Vector2(0.5f, 1f),
                new Vector2(0f, -62f),
                new Vector2(-18f, -78f));
        }

        private void CriarCampo(RectTransform raiz)
        {
            campoRaiz = HudFactory.CriarRect(
                "Campo de Batalha",
                raiz,
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                new Vector2(65f, 30f),
                new Vector2(1130f, 610f));
            HudFactory.CriarTexto(
                "Aliados",
                campoRaiz,
                "FORMAÇÃO",
                12f,
                HudTheme.Ciano,
                TextAlignmentOptions.Center,
                new Vector2(0f, 1f),
                new Vector2(0f, 1f),
                new Vector2(0f, 1f),
                new Vector2(110f, 8f),
                new Vector2(220f, 22f),
                FontStyles.Bold);
            HudFactory.CriarTexto(
                "Inimigos",
                campoRaiz,
                "AMEAÇAS",
                12f,
                HudTheme.Perigo,
                TextAlignmentOptions.Center,
                new Vector2(1f, 1f),
                new Vector2(1f, 1f),
                new Vector2(1f, 1f),
                new Vector2(-110f, 8f),
                new Vector2(220f, 22f),
                FontStyles.Bold);
            HudFactory.CriarTexto(
                "Versus",
                campoRaiz,
                "V S",
                17f,
                HudTheme.ComAlpha(HudTheme.Ouro, 0.65f),
                TextAlignmentOptions.Center,
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                Vector2.zero,
                new Vector2(70f, 32f),
                FontStyles.Bold);
        }

        private void CriarPainelDeAcoes(RectTransform raiz)
        {
            Image painel = HudFactory.CriarPainel(
                "Comandos",
                raiz,
                new Vector2(1f, 0f),
                new Vector2(1f, 0f),
                new Vector2(1f, 0f),
                new Vector2(-24f, 24f),
                new Vector2(650f, 220f),
                HudTheme.ComAlpha(HudTheme.Painel, 0.97f));
            HudFactory.AdicionarContorno(
                painel.gameObject,
                HudTheme.ComAlpha(HudTheme.Ouro, 0.45f),
                new Vector2(1f, 1f));

            tituloAtor = HudFactory.CriarTexto(
                "Ator",
                painel.transform,
                "—",
                23f,
                HudTheme.Texto,
                TextAlignmentOptions.TopLeft,
                new Vector2(0f, 1f),
                new Vector2(0f, 1f),
                new Vector2(0f, 1f),
                new Vector2(22f, -18f),
                new Vector2(270f, 34f),
                FontStyles.Bold);
            detalheAtor = HudFactory.CriarTexto(
                "Recursos",
                painel.transform,
                "—",
                13f,
                HudTheme.TextoSecundario,
                TextAlignmentOptions.TopLeft,
                new Vector2(0f, 1f),
                new Vector2(0f, 1f),
                new Vector2(0f, 1f),
                new Vector2(22f, -53f),
                new Vector2(330f, 25f));
            textoAlvo = HudFactory.CriarTexto(
                "Alvo",
                painel.transform,
                "ALVO: —",
                12f,
                HudTheme.Ouro,
                TextAlignmentOptions.TopRight,
                new Vector2(1f, 1f),
                new Vector2(1f, 1f),
                new Vector2(1f, 1f),
                new Vector2(-22f, -23f),
                new Vector2(280f, 25f),
                FontStyles.Bold);

            for (int i = 0; i < 2; i++)
            {
                TextMeshProUGUI titulo;
                TextMeshProUGUI custo;
                botoesHabilidade[i] = HudFactory.CriarBotao(
                    "Habilidade " + (i + 1),
                    painel.transform,
                    new Vector2(-135f + (i * 215f), -38f),
                    new Vector2(198f, 102f),
                    i == 0 ? HudTheme.PainelElevado : HudTheme.ComAlpha(HudTheme.OuroEscuro, 0.85f),
                    out titulo,
                    out custo);
                RectTransform rect = botoesHabilidade[i].GetComponent<RectTransform>();
                rect.anchorMin = new Vector2(0.5f, 0f);
                rect.anchorMax = new Vector2(0.5f, 0f);
                rect.pivot = new Vector2(0.5f, 0f);
                titulosHabilidade[i] = titulo;
                custosHabilidade[i] = custo;
            }

            TextMeshProUGUI tituloPassar;
            TextMeshProUGUI custoPassar;
            botaoPassar = HudFactory.CriarBotao(
                "Passar",
                painel.transform,
                new Vector2(276f, -38f),
                new Vector2(82f, 102f),
                HudTheme.PainelClaro,
                out tituloPassar,
                out custoPassar);
            RectTransform passarRect = botaoPassar.GetComponent<RectTransform>();
            passarRect.anchorMin = new Vector2(0.5f, 0f);
            passarRect.anchorMax = new Vector2(0.5f, 0f);
            passarRect.pivot = new Vector2(0.5f, 0f);
            tituloPassar.text = "PASSAR";
            tituloPassar.fontSize = 15f;
            tituloPassar.alignment = TextAlignmentOptions.Center;
            tituloPassar.rectTransform.offsetMin = Vector2.zero;
            tituloPassar.rectTransform.offsetMax = Vector2.zero;
            custoPassar.gameObject.SetActive(false);
            botaoPassar.onClick.AddListener(AoPassarTurno);
        }

        private void CriarHistorico(RectTransform raiz)
        {
            Image painel = HudFactory.CriarPainel(
                "Registro",
                raiz,
                new Vector2(1f, 1f),
                new Vector2(1f, 1f),
                new Vector2(1f, 1f),
                new Vector2(-24f, -116f),
                new Vector2(354f, 156f),
                HudTheme.ComAlpha(HudTheme.Painel, 0.78f));
            HudFactory.CriarTexto(
                "Rotulo",
                painel.transform,
                "REGISTRO DE COMBATE",
                11f,
                HudTheme.Ciano,
                TextAlignmentOptions.TopLeft,
                new Vector2(0f, 1f),
                Vector2.one,
                new Vector2(0f, 1f),
                new Vector2(15f, -10f),
                new Vector2(-30f, 20f),
                FontStyles.Bold);
            textoHistorico = HudFactory.CriarTexto(
                "Eventos",
                painel.transform,
                "Aguardando início da simulação...",
                12f,
                HudTheme.TextoSecundario,
                TextAlignmentOptions.TopLeft,
                Vector2.zero,
                Vector2.one,
                new Vector2(0f, 1f),
                new Vector2(15f, -35f),
                new Vector2(-30f, -46f));
            textoHistorico.textWrappingMode = TextWrappingModes.Normal;
            textoHistorico.overflowMode = TextOverflowModes.Truncate;
        }

        private void CriarFaixaDeTurno(RectTransform raiz)
        {
            Image faixa = HudFactory.CriarPainel(
                "Faixa de Turno",
                raiz,
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                new Vector2(0f, 182f),
                new Vector2(620f, 72f),
                HudTheme.ComAlpha(HudTheme.Painel, 0.96f));
            faixaTurno = faixa.rectTransform;
            faixaTurnoGrupo = faixa.gameObject.AddComponent<CanvasGroup>();
            HudFactory.AdicionarContorno(
                faixa.gameObject,
                HudTheme.ComAlpha(HudTheme.Ouro, 0.7f),
                new Vector2(1f, -1f));
            HudFactory.CriarTexto(
                "Texto",
                faixa.transform,
                "TURNO",
                23f,
                HudTheme.Texto,
                TextAlignmentOptions.Center,
                Vector2.zero,
                Vector2.one,
                new Vector2(0.5f, 0.5f),
                Vector2.zero,
                Vector2.zero,
                FontStyles.Bold).gameObject.name = "TextoTurno";
            faixa.gameObject.SetActive(false);
        }

        private void CriarFlash(RectTransform raiz)
        {
            Image flash = HudFactory.CriarPainel(
                "Flash",
                raiz,
                Vector2.zero,
                Vector2.one,
                new Vector2(0.5f, 0.5f),
                Vector2.zero,
                Vector2.zero,
                Color.white);
            flash.rectTransform.offsetMin = Vector2.zero;
            flash.rectTransform.offsetMax = Vector2.zero;
            flash.raycastTarget = false;
            flashGrupo = flash.gameObject.AddComponent<CanvasGroup>();
            flashGrupo.alpha = 0f;
        }

        private void CriarPainelDeResultado(RectTransform raiz)
        {
            Image painel = HudFactory.CriarPainel(
                "Resultado",
                raiz,
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                Vector2.zero,
                new Vector2(720f, 250f),
                HudTheme.ComAlpha(HudTheme.FundoProfundo, 0.98f),
                true);
            painelResultado = painel.rectTransform;
            HudFactory.AdicionarContorno(
                painel.gameObject,
                HudTheme.Ouro,
                new Vector2(2f, -2f));
            textoResultado = HudFactory.CriarTexto(
                "Texto Resultado",
                painel.transform,
                "VITÓRIA",
                42f,
                HudTheme.Ouro,
                TextAlignmentOptions.Center,
                Vector2.zero,
                Vector2.one,
                new Vector2(0.5f, 0.5f),
                new Vector2(0f, 28f),
                new Vector2(-60f, -70f),
                FontStyles.Bold);
            HudFactory.CriarTexto(
                "Instrucao",
                painel.transform,
                "Use REINICIAR para executar uma nova simulação.",
                14f,
                HudTheme.TextoSecundario,
                TextAlignmentOptions.Center,
                Vector2.zero,
                new Vector2(1f, 0.4f),
                new Vector2(0.5f, 0.5f),
                Vector2.zero,
                Vector2.zero);
            painel.gameObject.SetActive(false);
        }

        private void CriarFormacoesVisuais(
            IReadOnlyList<PersonagemBasico> aliados,
            IReadOnlyList<InimigoBasico> inimigos)
        {
            float[] posicoesAliadas = { 205f, 70f, -65f, -200f };
            float[] posicoesInimigas = { 145f, 0f, -145f };

            for (int i = 0; i < aliados.Count; i++)
            {
                CriarCartaoCampo(aliados[i].Combatente, new Vector2(-360f, posicoesAliadas[i]));
                CriarCartaoEquipe(aliados[i].Combatente, i);
            }

            for (int i = 0; i < inimigos.Count; i++)
            {
                CriarCartaoCampo(inimigos[i].Combatente, new Vector2(365f, posicoesInimigas[i]));
            }
        }

        private void CriarCartaoCampo(DefinicaoCombatente definicao, Vector2 posicao)
        {
            Color elemento = HudTheme.ParaElemento(definicao.Elemento);
            Image fundo = HudFactory.CriarPainel(
                definicao.Id,
                campoRaiz,
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                posicao,
                new Vector2(300f, 112f),
                HudTheme.ComAlpha(HudTheme.Painel, 0.96f),
                true);
            Outline contorno = HudFactory.AdicionarContorno(
                fundo.gameObject,
                HudTheme.ComAlpha(elemento, 0.45f),
                new Vector2(1f, -1f));
            var botao = fundo.gameObject.AddComponent<Button>();
            botao.targetGraphic = fundo;
            string idCapturado = definicao.Id;
            botao.onClick.AddListener(() => SelecionarAlvo(idCapturado));
            fundo.gameObject.AddComponent<HudHoverScale>();
            HudPulse pulso = fundo.gameObject.AddComponent<HudPulse>();

            HudFactory.CriarPainel(
                "Acento",
                fundo.transform,
                new Vector2(0f, 0f),
                new Vector2(0f, 1f),
                new Vector2(0f, 0.5f),
                new Vector2(4f, 0f),
                new Vector2(7f, -12f),
                elemento);
            Image selo = HudFactory.CriarPainel(
                "Selo",
                fundo.transform,
                new Vector2(0f, 0.5f),
                new Vector2(0f, 0.5f),
                new Vector2(0f, 0.5f),
                new Vector2(23f, 0f),
                new Vector2(66f, 66f),
                HudTheme.ComAlpha(elemento, 0.18f));
            HudFactory.AdicionarContorno(
                selo.gameObject,
                HudTheme.ComAlpha(elemento, 0.8f),
                new Vector2(1f, -1f));
            HudFactory.CriarTexto(
                "Sigla",
                selo.transform,
                definicao.Elemento.ToString().Substring(0, Math.Min(2, definicao.Elemento.ToString().Length)).ToUpperInvariant(),
                20f,
                elemento,
                TextAlignmentOptions.Center,
                Vector2.zero,
                Vector2.one,
                new Vector2(0.5f, 0.5f),
                Vector2.zero,
                Vector2.zero,
                FontStyles.Bold);
            TextMeshProUGUI nome = HudFactory.CriarTexto(
                "Nome",
                fundo.transform,
                definicao.Nome,
                16f,
                HudTheme.Texto,
                TextAlignmentOptions.TopLeft,
                new Vector2(0f, 1f),
                new Vector2(1f, 1f),
                new Vector2(0f, 1f),
                new Vector2(102f, -16f),
                new Vector2(-116f, 26f),
                FontStyles.Bold);
            HudSmoothBar vida = HudFactory.CriarBarra(
                "HP",
                fundo.transform,
                new Vector2(102f, -57f),
                new Vector2(174f, 12f),
                HudTheme.FundoProfundo,
                HudTheme.Vida);
            TextMeshProUGUI recursos = HudFactory.CriarTexto(
                "Recursos",
                fundo.transform,
                string.Empty,
                11f,
                HudTheme.TextoSecundario,
                TextAlignmentOptions.TopLeft,
                new Vector2(0f, 0f),
                new Vector2(1f, 0f),
                new Vector2(0f, 0f),
                new Vector2(102f, 14f),
                new Vector2(-116f, 22f));
            TextMeshProUGUI escudo = HudFactory.CriarTexto(
                "Escudo",
                fundo.transform,
                string.Empty,
                11f,
                HudTheme.Escudo,
                TextAlignmentOptions.BottomRight,
                Vector2.zero,
                Vector2.one,
                new Vector2(1f, 0f),
                new Vector2(-14f, 12f),
                new Vector2(88f, 20f),
                FontStyles.Bold);
            RegistrarCartao(new CartaoCombatente(
                definicao.Id,
                fundo.rectTransform,
                nome,
                recursos,
                escudo,
                vida,
                contorno,
                pulso,
                fundo.gameObject.AddComponent<CanvasGroup>()));
        }

        private void CriarCartaoEquipe(DefinicaoCombatente definicao, int indice)
        {
            Color elemento = HudTheme.ParaElemento(definicao.Elemento);
            Image fundo = HudFactory.CriarPainel(
                "Equipe " + definicao.Id,
                canvas.transform,
                new Vector2(0f, 0f),
                new Vector2(0f, 0f),
                new Vector2(0f, 0f),
                new Vector2(24f + (indice * 210f), 24f),
                new Vector2(196f, 108f),
                HudTheme.ComAlpha(HudTheme.Painel, 0.97f),
                true);
            Outline contorno = HudFactory.AdicionarContorno(
                fundo.gameObject,
                HudTheme.ComAlpha(elemento, 0.4f),
                new Vector2(1f, 1f));
            var botao = fundo.gameObject.AddComponent<Button>();
            botao.targetGraphic = fundo;
            string idCapturado = definicao.Id;
            botao.onClick.AddListener(() => SelecionarAlvo(idCapturado));
            fundo.gameObject.AddComponent<HudHoverScale>();
            HudPulse pulso = fundo.gameObject.AddComponent<HudPulse>();
            HudFactory.CriarTexto(
                "Indice",
                fundo.transform,
                "0" + (indice + 1),
                12f,
                elemento,
                TextAlignmentOptions.TopLeft,
                new Vector2(0f, 1f),
                new Vector2(0f, 1f),
                new Vector2(0f, 1f),
                new Vector2(12f, -9f),
                new Vector2(32f, 18f),
                FontStyles.Bold);
            TextMeshProUGUI nome = HudFactory.CriarTexto(
                "Nome",
                fundo.transform,
                definicao.Nome,
                13f,
                HudTheme.Texto,
                TextAlignmentOptions.TopLeft,
                new Vector2(0f, 1f),
                Vector2.one,
                new Vector2(0f, 1f),
                new Vector2(43f, -9f),
                new Vector2(-53f, 21f),
                FontStyles.Bold);
            HudSmoothBar vida = HudFactory.CriarBarra(
                "HP",
                fundo.transform,
                new Vector2(12f, -48f),
                new Vector2(172f, 10f),
                HudTheme.FundoProfundo,
                HudTheme.Vida);
            TextMeshProUGUI recursos = HudFactory.CriarTexto(
                "Recursos",
                fundo.transform,
                string.Empty,
                10f,
                HudTheme.TextoSecundario,
                TextAlignmentOptions.BottomLeft,
                Vector2.zero,
                Vector2.one,
                Vector2.zero,
                new Vector2(12f, 10f),
                new Vector2(-24f, 34f));
            TextMeshProUGUI escudo = HudFactory.CriarTexto(
                "Escudo",
                fundo.transform,
                string.Empty,
                10f,
                HudTheme.Escudo,
                TextAlignmentOptions.BottomRight,
                Vector2.zero,
                Vector2.one,
                new Vector2(1f, 0f),
                new Vector2(-12f, 10f),
                new Vector2(70f, 22f),
                FontStyles.Bold);
            RegistrarCartao(new CartaoCombatente(
                definicao.Id,
                fundo.rectTransform,
                nome,
                recursos,
                escudo,
                vida,
                contorno,
                pulso,
                fundo.gameObject.AddComponent<CanvasGroup>()));
        }

        private void RegistrarCartao(CartaoCombatente cartao)
        {
            if (!cartoes.TryGetValue(cartao.Id, out List<CartaoCombatente> lista))
            {
                lista = new List<CartaoCombatente>();
                cartoes.Add(cartao.Id, lista);
            }

            lista.Add(cartao);
        }

        private void AtualizarTudo(bool imediato = false)
        {
            foreach (EstadoCombatente estado in motor.Combatentes)
            {
                if (!cartoes.TryGetValue(estado.Id, out List<CartaoCombatente> lista))
                {
                    continue;
                }

                foreach (CartaoCombatente cartao in lista)
                {
                    cartao.Atualizar(
                        estado,
                        motor.AtorAtual != null && motor.AtorAtual.Id == estado.Id,
                        alvoSelecionadoId == estado.Id,
                        imediato);
                }
            }

            AtualizarLinhaDoTempo();
            AtualizarPainelDeAcoes();

            int aliadosVivos = motor.Combatentes.Count(item =>
                item.EstaVivo && item.Definicao.Equipe == Equipe.Aliados);
            int inimigosVivos = motor.Combatentes.Count(item =>
                item.EstaVivo && item.Definicao.Equipe == Equipe.Inimigos);
            textoContagem.text = $"ALIADOS {aliadosVivos:00}  //  INIMIGOS {inimigosVivos:00}";
        }

        private void AtualizarLinhaDoTempo()
        {
            foreach (Transform filho in linhaTempoRaiz)
            {
                Destroy(filho.gameObject);
            }

            var ordem = new List<EstadoCombatente>();
            if (motor.AtorAtual != null)
            {
                ordem.Add(motor.AtorAtual);
            }

            ordem.AddRange(motor.PreverProximosTurnos(9));
            for (int i = 0; i < Math.Min(10, ordem.Count); i++)
            {
                EstadoCombatente estado = ordem[i];
                bool atual = i == 0;
                Color elemento = HudTheme.ParaElemento(estado.Definicao.Elemento);
                Image item = HudFactory.CriarPainel(
                    "Turno " + i,
                    linhaTempoRaiz,
                    new Vector2(0f, 1f),
                    new Vector2(1f, 1f),
                    new Vector2(0.5f, 1f),
                    new Vector2(0f, -(i * 66f)),
                    new Vector2(-4f, 56f),
                    atual
                        ? HudTheme.ComAlpha(elemento, 0.25f)
                        : HudTheme.ComAlpha(HudTheme.PainelElevado, 0.84f));
                if (atual)
                {
                    HudFactory.AdicionarContorno(
                        item.gameObject,
                        HudTheme.ComAlpha(elemento, 0.9f),
                        new Vector2(1f, -1f));
                }

                HudFactory.CriarTexto(
                    "Ordem",
                    item.transform,
                    atual ? "AGORA" : i.ToString("00"),
                    atual ? 10f : 12f,
                    atual ? HudTheme.Ouro : HudTheme.TextoSecundario,
                    TextAlignmentOptions.Center,
                    Vector2.zero,
                    new Vector2(0f, 1f),
                    new Vector2(0f, 0.5f),
                    new Vector2(8f, 0f),
                    new Vector2(48f, 28f),
                    FontStyles.Bold);
                HudFactory.CriarTexto(
                    "Nome",
                    item.transform,
                    estado.Definicao.Nome,
                    atual ? 14f : 13f,
                    estado.EstaVivo ? HudTheme.Texto : HudTheme.TextoSecundario,
                    TextAlignmentOptions.MidlineLeft,
                    new Vector2(0f, 0f),
                    Vector2.one,
                    new Vector2(0f, 0.5f),
                    new Vector2(60f, 0f),
                    new Vector2(-96f, 0f),
                    atual ? FontStyles.Bold : FontStyles.Normal);
                HudFactory.CriarTexto(
                    "Elemento",
                    item.transform,
                    estado.Definicao.Elemento.ToString().ToUpperInvariant(),
                    9f,
                    elemento,
                    TextAlignmentOptions.Center,
                    new Vector2(1f, 0f),
                    Vector2.one,
                    new Vector2(1f, 0.5f),
                    new Vector2(-10f, 0f),
                    new Vector2(78f, 24f),
                    FontStyles.Bold);
            }
        }

        private void AtualizarPainelDeAcoes()
        {
            EstadoCombatente ator = motor.AtorAtual;
            if (ator == null)
            {
                return;
            }

            Color elemento = HudTheme.ParaElemento(ator.Definicao.Elemento);
            tituloAtor.text = ator.Definicao.Nome;
            tituloAtor.color = elemento;
            detalheAtor.text =
                $"{ator.Definicao.Elemento.ToString().ToUpperInvariant()}   "
                + $"HP {ator.HpAtual}/{ator.Definicao.HpMax}   "
                + $"AP {ator.ApAtual}/{ator.Definicao.ApMax}   "
                + $"MP {ator.MpAtual}/{ator.Definicao.MpMax}";

            EstadoCombatente alvo = ObterAlvoSelecionado();
            textoAlvo.text = alvo == null ? "ALVO: AUTOMÁTICO" : "ALVO: " + alvo.Definicao.Nome.ToUpperInvariant();

            IReadOnlyList<DefinicaoHabilidade> lista = habilidades[ator.Id];
            bool turnoJogador = ator.Definicao.Equipe == Equipe.Aliados
                && motor.Fase == FaseCombate.Acao
                && !resolvendo;

            for (int i = 0; i < botoesHabilidade.Length; i++)
            {
                Button botao = botoesHabilidade[i];
                botao.onClick.RemoveAllListeners();
                if (i >= lista.Count)
                {
                    botao.gameObject.SetActive(false);
                    continue;
                }

                botao.gameObject.SetActive(true);
                DefinicaoHabilidade habilidade = lista[i];
                titulosHabilidade[i].text = habilidade.Nome.ToUpperInvariant();
                custosHabilidade[i].text = habilidade.CustoAp == 0 && habilidade.CustoMp == 0
                    ? "BÁSICO  //  SEM CUSTO"
                    : $"AP {habilidade.CustoAp}   MP {habilidade.CustoMp}";
                bool podePagar = ator.ApAtual >= habilidade.CustoAp
                    && ator.MpAtual >= habilidade.CustoMp;
                botao.interactable = turnoJogador && podePagar;
                int indiceCapturado = i;
                botao.onClick.AddListener(() => AoUsarHabilidade(indiceCapturado));
            }

            botaoPassar.interactable = turnoJogador;
        }

        private void SelecionarAlvo(string id)
        {
            if (resolvendo || motor.FoiEncerrada)
            {
                return;
            }

            EstadoCombatente estado = motor.ObterCombatente(id);
            if (!estado.EstaVivo)
            {
                return;
            }

            alvoSelecionadoId = id;
            AtualizarTudo();
        }

        private void SelecionarAlvoPadrao()
        {
            EstadoCombatente alvo = motor.Combatentes
                .Where(item => item.EstaVivo && item.Definicao.Equipe == Equipe.Inimigos)
                .OrderBy(item => item.HpAtual)
                .ThenBy(item => item.Id, StringComparer.Ordinal)
                .FirstOrDefault();
            alvoSelecionadoId = alvo?.Id;
        }

        private EstadoCombatente ObterAlvoSelecionado()
        {
            if (string.IsNullOrEmpty(alvoSelecionadoId))
            {
                return null;
            }

            EstadoCombatente alvo = motor.ObterCombatente(alvoSelecionadoId);
            return alvo.EstaVivo ? alvo : null;
        }

        private void AoUsarHabilidade(int indice)
        {
            if (resolvendo || motor.AtorAtual == null || motor.FoiEncerrada)
            {
                return;
            }

            DefinicaoHabilidade habilidade = habilidades[motor.AtorAtual.Id][indice];
            ExecutarHabilidade(habilidade);
        }

        private void ExecutarHabilidade(DefinicaoHabilidade habilidade)
        {
            EstadoCombatente ator = motor.AtorAtual;
            IEnumerable<string> alvos = ResolverAlvos(ator, habilidade);
            ResultadoComando resultado = motor.UsarHabilidade(ator.Id, alvos, habilidade);
            if (!resultado.Executado)
            {
                AdicionarAoHistorico("COMANDO RECUSADO // " + resultado.Erro.ToString().ToUpperInvariant());
                StartCoroutine(Piscar(HudTheme.Perigo, 0.12f));
                return;
            }

            StartCoroutine(ResolverEventosDaAcao(habilidade.Nome));
        }

        private IEnumerable<string> ResolverAlvos(
            EstadoCombatente ator,
            DefinicaoHabilidade habilidade)
        {
            if (habilidade.TipoAlvo == TipoAlvo.TodosInimigos
                || habilidade.TipoAlvo == TipoAlvo.TodosAliados
                || habilidade.TipoAlvo == TipoAlvo.ProprioUsuario)
            {
                return Enumerable.Empty<string>();
            }

            bool aliado = habilidade.TipoAlvo == TipoAlvo.AliadoUnico;
            EstadoCombatente selecionado = ObterAlvoSelecionado();
            bool selecaoValida = selecionado != null
                && (habilidade.TipoAlvo == TipoAlvo.QualquerEntidade
                    || (aliado && selecionado.Definicao.Equipe == ator.Definicao.Equipe)
                    || (!aliado && selecionado.Definicao.Equipe != ator.Definicao.Equipe));
            if (!selecaoValida)
            {
                selecionado = motor.Combatentes
                    .Where(item => item.EstaVivo)
                    .Where(item => aliado
                        ? item.Definicao.Equipe == ator.Definicao.Equipe
                        : item.Definicao.Equipe != ator.Definicao.Equipe)
                    .OrderBy(item => item.HpAtual / (double)item.Definicao.HpMax)
                    .ThenBy(item => item.HpAtual)
                    .First();
            }

            alvoSelecionadoId = selecionado.Id;
            return new[] { selecionado.Id };
        }

        private void AoPassarTurno()
        {
            if (resolvendo || motor.AtorAtual == null)
            {
                return;
            }

            ResultadoComando resultado = motor.PassarTurno(motor.AtorAtual.Id);
            if (resultado.Executado)
            {
                StartCoroutine(ResolverEventosDaAcao("Turno passado"));
            }
        }

        private IEnumerator ResolverEventosDaAcao(string nomeAcao)
        {
            resolvendo = true;
            AtualizarTudo();
            AdicionarAoHistorico(motor.AtorAtual.Definicao.Nome + " // " + nomeAcao);
            IReadOnlyList<EventoCombate> eventos = motor.DrenarEventos();
            AtualizarTudo();

            foreach (EventoCombate evento in eventos)
            {
                yield return AnimarEvento(evento);
            }

            yield return new WaitForSecondsRealtime(0.22f);
            motor.ConcluirResolucao();
            motor.DrenarEventos();
            resolvendo = false;

            if (ObterAlvoSelecionado() == null)
            {
                SelecionarAlvoPadrao();
            }

            AtualizarTudo();
            if (motor.FoiEncerrada)
            {
                MostrarResultado();
                yield break;
            }

            MostrarFaixaDoTurno();
            PrepararControleDoTurno();
        }

        private IEnumerator AnimarEvento(EventoCombate evento)
        {
            switch (evento.Tipo)
            {
                case TipoEventoCombate.DanoCausado:
                    if (evento.Valor > 0)
                    {
                        CriarTextoFlutuante(evento.AlvoId, "-" + evento.Valor, HudTheme.Perigo);
                        AdicionarAoHistorico(NomeCurto(evento.AlvoId) + " sofreu " + evento.Valor + " de dano");
                        yield return AbalarCartao(evento.AlvoId);
                    }
                    break;
                case TipoEventoCombate.CuraRecebida:
                    CriarTextoFlutuante(evento.AlvoId, "+" + evento.Valor, HudTheme.Vida);
                    AdicionarAoHistorico(NomeCurto(evento.AlvoId) + " recuperou " + evento.Valor + " HP");
                    yield return new WaitForSecondsRealtime(0.16f);
                    break;
                case TipoEventoCombate.EscudoConcedido:
                    CriarTextoFlutuante(evento.AlvoId, "+" + evento.Valor + " ESC", HudTheme.Escudo);
                    AdicionarAoHistorico(NomeCurto(evento.AlvoId) + " recebeu escudo " + evento.Valor);
                    yield return new WaitForSecondsRealtime(0.16f);
                    break;
                case TipoEventoCombate.EscudoAbsorveu:
                    CriarTextoFlutuante(evento.AlvoId, "ABSORVEU " + evento.Valor, HudTheme.Escudo);
                    yield return Piscar(HudTheme.Escudo, 0.08f);
                    break;
                case TipoEventoCombate.CombatenteDerrotado:
                    CriarTextoFlutuante(evento.AlvoId, "DERROTADO", HudTheme.Ouro);
                    AdicionarAoHistorico(NomeCurto(evento.AlvoId) + " foi derrotado");
                    yield return Piscar(HudTheme.Perigo, 0.14f);
                    break;
            }

            AtualizarTudo();
        }

        private IEnumerator AbalarCartao(string id)
        {
            if (!cartoes.TryGetValue(id, out List<CartaoCombatente> lista) || lista.Count == 0)
            {
                yield break;
            }

            RectTransform rect = lista[0].Raiz;
            Vector2 origem = rect.anchoredPosition;
            for (int i = 0; i < 5; i++)
            {
                rect.anchoredPosition = origem + new Vector2(i % 2 == 0 ? -8f : 8f, 0f);
                yield return new WaitForSecondsRealtime(0.025f);
            }

            rect.anchoredPosition = origem;
        }

        private void CriarTextoFlutuante(string id, string conteudo, Color cor)
        {
            if (!cartoes.TryGetValue(id, out List<CartaoCombatente> lista) || lista.Count == 0)
            {
                return;
            }

            RectTransform referencia = lista[0].Raiz;
            TextMeshProUGUI texto = HudFactory.CriarTexto(
                "Feedback",
                campoRaiz,
                conteudo,
                22f,
                cor,
                TextAlignmentOptions.Center,
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                referencia.anchoredPosition + new Vector2(0f, 72f),
                new Vector2(220f, 40f),
                FontStyles.Bold);
            StartCoroutine(AnimarTextoFlutuante(texto));
        }

        private IEnumerator AnimarTextoFlutuante(TextMeshProUGUI texto)
        {
            RectTransform rect = texto.rectTransform;
            Vector2 inicio = rect.anchoredPosition;
            Color cor = texto.color;
            float tempo = 0f;
            while (tempo < 0.8f)
            {
                tempo += Time.unscaledDeltaTime;
                float progresso = Mathf.Clamp01(tempo / 0.8f);
                rect.anchoredPosition = inicio + new Vector2(0f, progresso * 54f);
                cor.a = 1f - Mathf.Pow(progresso, 2f);
                texto.color = cor;
                yield return null;
            }

            Destroy(texto.gameObject);
        }

        private IEnumerator Piscar(Color cor, float duracao)
        {
            Image imagem = flashGrupo.GetComponent<Image>();
            imagem.color = cor;
            flashGrupo.alpha = 0.18f;
            float tempo = 0f;
            while (tempo < duracao)
            {
                tempo += Time.unscaledDeltaTime;
                flashGrupo.alpha = Mathf.Lerp(0.18f, 0f, tempo / duracao);
                yield return null;
            }

            flashGrupo.alpha = 0f;
        }

        private void PrepararControleDoTurno()
        {
            if (motor.FoiEncerrada || motor.AtorAtual == null)
            {
                return;
            }

            if (motor.AtorAtual.Definicao.Equipe == Equipe.Inimigos)
            {
                if (rotinaInimigo != null)
                {
                    StopCoroutine(rotinaInimigo);
                }

                rotinaInimigo = StartCoroutine(ExecutarTurnoInimigo());
            }
        }

        private IEnumerator ExecutarTurnoInimigo()
        {
            resolvendo = true;
            AtualizarTudo();
            yield return new WaitForSecondsRealtime(0.7f);
            resolvendo = false;

            EstadoCombatente ator = motor.AtorAtual;
            IReadOnlyList<DefinicaoHabilidade> lista = habilidades[ator.Id];
            DefinicaoHabilidade especial = lista.Count > 1 ? lista[1] : null;
            DefinicaoHabilidade escolhida = especial != null
                && ator.ApAtual >= especial.CustoAp
                && ator.MpAtual >= especial.CustoMp
                    ? especial
                    : lista[0];
            ExecutarHabilidade(escolhida);
            rotinaInimigo = null;
        }

        private void MostrarFaixaDoTurno()
        {
            if (motor.AtorAtual == null)
            {
                return;
            }

            TextMeshProUGUI texto = faixaTurno.Find("TextoTurno").GetComponent<TextMeshProUGUI>();
            Color elemento = HudTheme.ParaElemento(motor.AtorAtual.Definicao.Elemento);
            texto.text = "TURNO DE " + motor.AtorAtual.Definicao.Nome.ToUpperInvariant();
            texto.color = elemento;
            StopCoroutine(nameof(AnimarFaixaTurno));
            StartCoroutine(nameof(AnimarFaixaTurno));
        }

        private IEnumerator AnimarFaixaTurno()
        {
            faixaTurno.gameObject.SetActive(true);
            faixaTurnoGrupo.alpha = 0f;
            Vector2 destino = new Vector2(0f, 182f);
            faixaTurno.anchoredPosition = destino + new Vector2(-70f, 0f);
            float tempo = 0f;
            while (tempo < 0.22f)
            {
                tempo += Time.unscaledDeltaTime;
                float t = 1f - Mathf.Pow(1f - Mathf.Clamp01(tempo / 0.22f), 3f);
                faixaTurnoGrupo.alpha = t;
                faixaTurno.anchoredPosition = Vector2.Lerp(destino + new Vector2(-70f, 0f), destino, t);
                yield return null;
            }

            yield return new WaitForSecondsRealtime(0.42f);
            tempo = 0f;
            while (tempo < 0.2f)
            {
                tempo += Time.unscaledDeltaTime;
                faixaTurnoGrupo.alpha = 1f - Mathf.Clamp01(tempo / 0.2f);
                yield return null;
            }

            faixaTurno.gameObject.SetActive(false);
        }

        private void MostrarResultado()
        {
            painelResultado.gameObject.SetActive(true);
            bool vitoria = motor.Fase == FaseCombate.Vitoria;
            textoResultado.text = vitoria ? "VITÓRIA" : "DERROTA";
            textoResultado.color = vitoria ? HudTheme.Ouro : HudTheme.Perigo;
            painelResultado.localScale = Vector3.one * 0.82f;
            StartCoroutine(AnimarResultado());
        }

        private IEnumerator AnimarResultado()
        {
            float tempo = 0f;
            while (tempo < 0.35f)
            {
                tempo += Time.unscaledDeltaTime;
                float t = 1f - Mathf.Pow(1f - Mathf.Clamp01(tempo / 0.35f), 3f);
                painelResultado.localScale = Vector3.one * Mathf.Lerp(0.82f, 1f, t);
                yield return null;
            }
        }

        private void AdicionarAoHistorico(string mensagem)
        {
            historico.Enqueue(mensagem);
            while (historico.Count > 5)
            {
                historico.Dequeue();
            }

            textoHistorico.text = string.Join("\n", historico);
        }

        private string NomeCurto(string id)
        {
            return motor.ObterCombatente(id).Definicao.Nome;
        }

        private void ReiniciarCena()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        private sealed class CartaoCombatente
        {
            public string Id { get; }
            public RectTransform Raiz { get; }
            private readonly TextMeshProUGUI nome;
            private readonly TextMeshProUGUI recursos;
            private readonly TextMeshProUGUI escudo;
            private readonly HudSmoothBar vida;
            private readonly Outline contorno;
            private readonly HudPulse pulso;
            private readonly CanvasGroup grupo;

            public CartaoCombatente(
                string id,
                RectTransform raiz,
                TextMeshProUGUI nome,
                TextMeshProUGUI recursos,
                TextMeshProUGUI escudo,
                HudSmoothBar vida,
                Outline contorno,
                HudPulse pulso,
                CanvasGroup grupo)
            {
                Id = id;
                Raiz = raiz;
                this.nome = nome;
                this.recursos = recursos;
                this.escudo = escudo;
                this.vida = vida;
                this.contorno = contorno;
                this.pulso = pulso;
                this.grupo = grupo;
            }

            public void Atualizar(
                EstadoCombatente estado,
                bool ativo,
                bool selecionado,
                bool imediato)
            {
                float proporcao = estado.Definicao.HpMax == 0
                    ? 0f
                    : estado.HpAtual / (float)estado.Definicao.HpMax;
                vida.Definir(proporcao, imediato);
                nome.text = estado.EstaVivo ? estado.Definicao.Nome : estado.Definicao.Nome + "  //  FORA";
                recursos.text =
                    $"HP {estado.HpAtual}/{estado.Definicao.HpMax}   "
                    + $"AP {estado.ApAtual}/{estado.Definicao.ApMax}";
                escudo.text = estado.EscudoAtual > 0 ? "ESC " + estado.EscudoAtual : string.Empty;
                grupo.alpha = estado.EstaVivo ? 1f : 0.34f;
                pulso.Ativo = ativo;
                Color elemento = HudTheme.ParaElemento(estado.Definicao.Elemento);
                contorno.effectColor = selecionado
                    ? HudTheme.Ouro
                    : ativo
                        ? elemento
                        : HudTheme.ComAlpha(elemento, 0.4f);
                contorno.effectDistance = selecionado ? new Vector2(2f, -2f) : new Vector2(1f, -1f);
            }
        }
    }

    public static class BattleHudDemoBootstrap
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void CriarHudNaCenaDeDemonstracao()
        {
            if (SceneManager.GetActiveScene().name != "SampleScene"
                || UnityEngine.Object.FindFirstObjectByType<BattleHudController>() != null)
            {
                return;
            }

            var objeto = new GameObject("Ecliptari HUD Demo");
            objeto.AddComponent<BattleHudController>();
        }
    }
}
