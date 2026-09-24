using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Ecliptari.UI
{
    public sealed class HudGradientGraphic : MaskableGraphic
    {
        public Color CorSuperior = HudTheme.PainelElevado;
        public Color CorInferior = HudTheme.Fundo;

        protected override void OnPopulateMesh(VertexHelper vertexHelper)
        {
            vertexHelper.Clear();
            Rect rect = rectTransform.rect;

            vertexHelper.AddVert(new Vector3(rect.xMin, rect.yMin), CorInferior, Vector2.zero);
            vertexHelper.AddVert(new Vector3(rect.xMin, rect.yMax), CorSuperior, Vector2.up);
            vertexHelper.AddVert(new Vector3(rect.xMax, rect.yMax), CorSuperior, Vector2.one);
            vertexHelper.AddVert(new Vector3(rect.xMax, rect.yMin), CorInferior, Vector2.right);
            vertexHelper.AddTriangle(0, 1, 2);
            vertexHelper.AddTriangle(0, 2, 3);
        }
    }

    public sealed class HudSmoothBar : MonoBehaviour
    {
        private RectTransform preenchimento;
        private Vector2 margemMinima;
        private Vector2 margemMaxima;
        private float valorAtual = 1f;
        private float valorAlvo = 1f;

        public void Configurar(RectTransform novoPreenchimento)
        {
            preenchimento = novoPreenchimento;
            margemMinima = preenchimento.offsetMin;
            margemMaxima = preenchimento.offsetMax;
            Aplicar(1f);
        }

        public void Definir(float valor, bool imediato = false)
        {
            valorAlvo = Mathf.Clamp01(valor);
            if (imediato)
            {
                valorAtual = valorAlvo;
                Aplicar(valorAtual);
            }
        }

        private void Update()
        {
            if (preenchimento == null || Mathf.Approximately(valorAtual, valorAlvo))
            {
                return;
            }

            valorAtual = Mathf.MoveTowards(valorAtual, valorAlvo, Time.unscaledDeltaTime * 1.8f);
            Aplicar(valorAtual);
        }

        private void Aplicar(float valor)
        {
            if (preenchimento == null)
            {
                return;
            }

            preenchimento.anchorMax = new Vector2(valor, 1f);
            preenchimento.offsetMin = margemMinima;
            preenchimento.offsetMax = margemMaxima;
        }
    }

    public sealed class HudHoverScale : MonoBehaviour,
        IPointerEnterHandler,
        IPointerExitHandler,
        IPointerDownHandler,
        IPointerUpHandler
    {
        private float escalaInteracao = 1f;
        private float multiplicadorExterno = 1f;

        private void Update()
        {
            Vector3 escalaAlvo = Vector3.one * escalaInteracao * multiplicadorExterno;
            transform.localScale = Vector3.Lerp(
                transform.localScale,
                escalaAlvo,
                1f - Mathf.Exp(-14f * Time.unscaledDeltaTime));
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            escalaInteracao = 1.045f;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            escalaInteracao = 1f;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            escalaInteracao = 0.96f;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            escalaInteracao = 1.045f;
        }

        public void DefinirMultiplicadorExterno(float valor)
        {
            multiplicadorExterno = Mathf.Max(0f, valor);
        }
    }

    public sealed class HudPulse : MonoBehaviour
    {
        public bool Ativo;
        public float Intensidade = 0.035f;
        public float Velocidade = 4f;
        private HudHoverScale escalaInterativa;

        private void Awake()
        {
            escalaInterativa = GetComponent<HudHoverScale>();
        }

        private void Update()
        {
            float pulso = Ativo
                ? 1f + ((Mathf.Sin(Time.unscaledTime * Velocidade) + 1f) * 0.5f * Intensidade)
                : 1f;
            if (escalaInterativa != null)
            {
                escalaInterativa.DefinirMultiplicadorExterno(pulso);
                return;
            }

            transform.localScale = Vector3.Lerp(
                transform.localScale,
                Vector3.one * pulso,
                1f - Mathf.Exp(-12f * Time.unscaledDeltaTime));
        }
    }
}
