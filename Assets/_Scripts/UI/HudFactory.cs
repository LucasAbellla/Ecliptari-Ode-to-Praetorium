using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Ecliptari.UI
{
    public static class HudFactory
    {
        private static Sprite spriteArredondado;

        public static RectTransform CriarRect(
            string nome,
            Transform pai,
            Vector2 ancoraMin,
            Vector2 ancoraMax,
            Vector2 pivot,
            Vector2 posicao,
            Vector2 tamanho)
        {
            var objeto = new GameObject(nome, typeof(RectTransform));
            objeto.layer = 5;
            RectTransform rect = objeto.GetComponent<RectTransform>();
            rect.SetParent(pai, false);
            rect.anchorMin = ancoraMin;
            rect.anchorMax = ancoraMax;
            rect.pivot = pivot;
            rect.anchoredPosition = posicao;
            rect.sizeDelta = tamanho;
            rect.localScale = Vector3.one;
            return rect;
        }

        public static Image CriarPainel(
            string nome,
            Transform pai,
            Vector2 ancoraMin,
            Vector2 ancoraMax,
            Vector2 pivot,
            Vector2 posicao,
            Vector2 tamanho,
            Color cor,
            bool recebeRaycast = false)
        {
            RectTransform rect = CriarRect(
                nome,
                pai,
                ancoraMin,
                ancoraMax,
                pivot,
                posicao,
                tamanho);
            Image imagem = rect.gameObject.AddComponent<Image>();
            imagem.color = cor;
            imagem.raycastTarget = recebeRaycast;
            imagem.sprite = ObterSpriteArredondado();
            if (imagem.sprite != null)
            {
                imagem.type = Image.Type.Sliced;
            }

            return imagem;
        }

        public static HudGradientGraphic CriarGradiente(
            string nome,
            Transform pai,
            Color superior,
            Color inferior)
        {
            RectTransform rect = CriarRect(
                nome,
                pai,
                Vector2.zero,
                Vector2.one,
                new Vector2(0.5f, 0.5f),
                Vector2.zero,
                Vector2.zero);
            var gradiente = rect.gameObject.AddComponent<HudGradientGraphic>();
            gradiente.CorSuperior = superior;
            gradiente.CorInferior = inferior;
            gradiente.raycastTarget = false;
            return gradiente;
        }

        public static TextMeshProUGUI CriarTexto(
            string nome,
            Transform pai,
            string conteudo,
            float tamanhoFonte,
            Color cor,
            TextAlignmentOptions alinhamento,
            Vector2 ancoraMin,
            Vector2 ancoraMax,
            Vector2 pivot,
            Vector2 posicao,
            Vector2 tamanho,
            FontStyles estilo = FontStyles.Normal)
        {
            RectTransform rect = CriarRect(
                nome,
                pai,
                ancoraMin,
                ancoraMax,
                pivot,
                posicao,
                tamanho);
            var texto = rect.gameObject.AddComponent<TextMeshProUGUI>();
            texto.text = conteudo;
            texto.fontSize = tamanhoFonte;
            texto.color = cor;
            texto.alignment = alinhamento;
            texto.fontStyle = estilo;
            texto.raycastTarget = false;
            texto.textWrappingMode = TextWrappingModes.NoWrap;
            texto.overflowMode = TextOverflowModes.Ellipsis;
            texto.characterSpacing = 1.2f;
            return texto;
        }

        public static Button CriarBotao(
            string nome,
            Transform pai,
            Vector2 posicao,
            Vector2 tamanho,
            Color corBase,
            out TextMeshProUGUI titulo,
            out TextMeshProUGUI subtitulo)
        {
            Image fundo = CriarPainel(
                nome,
                pai,
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                posicao,
                tamanho,
                corBase,
                true);
            Button botao = fundo.gameObject.AddComponent<Button>();
            botao.targetGraphic = fundo;
            ColorBlock cores = botao.colors;
            cores.normalColor = Color.white;
            cores.highlightedColor = new Color(1.12f, 1.12f, 1.12f, 1f);
            cores.pressedColor = new Color(0.82f, 0.82f, 0.82f, 1f);
            cores.disabledColor = new Color(0.35f, 0.38f, 0.42f, 0.65f);
            cores.colorMultiplier = 1f;
            cores.fadeDuration = 0.1f;
            botao.colors = cores;
            fundo.gameObject.AddComponent<HudHoverScale>();

            titulo = CriarTexto(
                "Titulo",
                fundo.transform,
                nome,
                21f,
                HudTheme.Texto,
                TextAlignmentOptions.BottomLeft,
                Vector2.zero,
                Vector2.one,
                Vector2.zero,
                new Vector2(18f, 25f),
                new Vector2(-36f, 46f),
                FontStyles.Bold);
            subtitulo = CriarTexto(
                "Custo",
                fundo.transform,
                string.Empty,
                13f,
                HudTheme.Ouro,
                TextAlignmentOptions.TopLeft,
                Vector2.zero,
                Vector2.one,
                Vector2.zero,
                new Vector2(18f, 8f),
                new Vector2(-36f, 22f));
            return botao;
        }

        public static HudSmoothBar CriarBarra(
            string nome,
            Transform pai,
            Vector2 posicao,
            Vector2 tamanho,
            Color corFundo,
            Color corPreenchimento)
        {
            Image fundo = CriarPainel(
                nome,
                pai,
                new Vector2(0f, 0.5f),
                new Vector2(0f, 0.5f),
                new Vector2(0f, 0.5f),
                posicao,
                tamanho,
                corFundo);
            Image preenchimento = CriarPainel(
                "Fill",
                fundo.transform,
                Vector2.zero,
                Vector2.one,
                new Vector2(0f, 0.5f),
                Vector2.zero,
                Vector2.zero,
                corPreenchimento);
            preenchimento.rectTransform.offsetMin = new Vector2(2f, 2f);
            preenchimento.rectTransform.offsetMax = new Vector2(-2f, -2f);
            var barra = fundo.gameObject.AddComponent<HudSmoothBar>();
            barra.Configurar(preenchimento.rectTransform);
            return barra;
        }

        public static Outline AdicionarContorno(GameObject objeto, Color cor, Vector2 distancia)
        {
            var contorno = objeto.AddComponent<Outline>();
            contorno.effectColor = cor;
            contorno.effectDistance = distancia;
            contorno.useGraphicAlpha = true;
            return contorno;
        }

        private static Sprite ObterSpriteArredondado()
        {
            if (spriteArredondado == null)
            {
                spriteArredondado = Resources.GetBuiltinResource<Sprite>("UI/Skin/UISprite.psd");
            }

            return spriteArredondado;
        }
    }
}
