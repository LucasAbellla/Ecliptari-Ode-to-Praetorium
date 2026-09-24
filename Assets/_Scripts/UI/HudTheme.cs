using Ecliptari.Combate;
using UnityEngine;

namespace Ecliptari.UI
{
    public static class HudTheme
    {
        public static readonly Color Fundo = Cor("07101B");
        public static readonly Color FundoProfundo = Cor("03070E");
        public static readonly Color Painel = Cor("101C2C");
        public static readonly Color PainelElevado = Cor("182B3F");
        public static readonly Color PainelClaro = Cor("213A51");
        public static readonly Color Ouro = Cor("F2D58B");
        public static readonly Color OuroEscuro = Cor("8D7240");
        public static readonly Color Ciano = Cor("67E8F9");
        public static readonly Color Texto = Cor("EEF6FF");
        public static readonly Color TextoSecundario = Cor("8FA7BC");
        public static readonly Color Vida = Cor("58D59B");
        public static readonly Color Perigo = Cor("FF6577");
        public static readonly Color Escudo = Cor("72B7FF");
        public static readonly Color Mana = Cor("9B8CFF");
        public static readonly Color Sombra = new Color(0f, 0f, 0f, 0.55f);

        public static Color ParaElemento(Elemento elemento)
        {
            switch (elemento)
            {
                case Elemento.Flama:
                    return Cor("FF6B55");
                case Elemento.Aqua:
                    return Cor("4FC3F7");
                case Elemento.Terrae:
                    return Cor("D6A657");
                case Elemento.Eol:
                    return Cor("65E0C1");
                case Elemento.Crelix:
                    return Cor("8CD9FF");
                case Elemento.Fulmen:
                    return Cor("B989FF");
                case Elemento.Lux:
                    return Cor("FFE7A0");
                case Elemento.Umbra:
                    return Cor("9A76D8");
                case Elemento.Vitae:
                    return Cor("69D47C");
                case Elemento.Toxi:
                    return Cor("9BD95D");
                case Elemento.Vis:
                    return Cor("F185FF");
                default:
                    return TextoSecundario;
            }
        }

        public static Color ComAlpha(Color cor, float alpha)
        {
            cor.a = alpha;
            return cor;
        }

        private static Color Cor(string hexadecimal)
        {
            ColorUtility.TryParseHtmlString("#" + hexadecimal, out Color cor);
            return cor;
        }
    }
}
