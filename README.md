# Ecliptari: Ode to Praetorium

*História criada e desenvolvida por Lucas A. Abella Macedo*

![Unity](https://img.shields.io/badge/Unity-6.3-black?style=for-the-badge&logo=unity)
![C#](https://img.shields.io/badge/C%23-Programming-blue?style=for-the-badge&logo=c-sharp)
![Status](https://img.shields.io/badge/Status-Em%20Desenvolvimento-brightgreen?style=for-the-badge)

**Ecliptari** inicialmente é um RPG tático de turnos 2D focado em combates estratégicos e gerenciamento de recursos. Desenvolvido na Unity, o projeto apresenta uma interface simples e focada, onde o domínio da alquimia elemental dita a diferença entre a vitória e a derrota no mundo sombrio de Gionyyl.

## Documentação do projeto

* [Fundamentos de design](Docs/GAME_DESIGN.md)
* [Arquitetura técnica do combate](Docs/TECHNICAL_ARCHITECTURE.md)
* [Roteiro de desenvolvimento](Docs/ROADMAP.md)
* [Marco 1 — Núcleo de Combate](Docs/MARCO_1_NUCLEO_DE_COMBATE.md)
* [Equipes de Protótipo](Docs/EQUIPES_DE_PROTOTIPO.md)
* [HUD de Combate](Docs/HUD_DE_COMBATE.md)

## O Universo
O jogo se passa dentro da perspectiva Ecliptari de **Nahvvatzal**, um cenário com elementos de ficção, fantasia e horror cósmico. A fundação do mundo é regida por 11 deuses primordiais, os *Colossa-estelari*. Parte da magia e os encantamentos do jogo são fundamentados em um idioma próprio construído para a lore, o **Qoldjornesz**.

## Mecânicas Principais

* **Sistema de Turnos Dinâmico:** Uma linha do tempo baseada no atributo de *Velocidade* de cada entidade, dividida em Fases de Manutenção, Ação e Resolução.
* **Gerenciamento de AP (Action Points):** Habilidades não possuem "cooldowns" tradicionais, mas consomem AP, que é regenerado taticamente a cada turno.
* **A Alquimia (Elementalização):** Sistema complexo de reações onde elementos (*Flama*, *Aqua*, *Eol*, *Terrae*, *Crelix*, *Fulmen*, *Lux*, *Umbra*, *Toxi*, *Vitae* e *Vis*) interagem. A força dessas reações (Reacionárias e Combinatórias) é multiplicada pelo status de *Elementalização* do personagem.
* **Corte de Turno (Ultimates):** Os personagens acumulam energia para liberar habilidades Supremas que podem interromper o fluxo natural da linha do tempo a qualquer momento.
* **Sistema de Relíquias:** Equipamentos de 2 a 6 peças que alteram drasticamente o funcionamento do personagem (ex: o conjunto Hipótese Extensiva reduz a resistência elemental do inimigo atingido em 40%).

##  Arquitetura Técnica (Unity)

O projeto migrou de uma base inicial web (JS/Electron) para a Unity, adotando padrões de projeto robustos para facilitar a escalabilidade e a manutenção:

* **Separação de Lógica e Dados:** A arquitetura garante que a lógica de script do combate opere de forma independente do conteúdo de dados.
* **Conteúdo orientado a dados:** Combatentes, habilidades e efeitos são definições imutáveis separadas do estado. O protótipo usa catálogos C# e permite adicionar adaptadores para `ScriptableObjects` sem alterar o motor.
* **Interface Modular:** HUD construído com Canvas responsivo, isolando seleção, barras, linha do tempo, comandos e animações do `MotorBatalha`.
* **Pixel Perfect Rendering:** Configuração de câmera e importação de texturas otimizadas para manter a nitidez dos sprites Chibi e Pixel Art (`Point / No Filter`).

## Formação de Protótipo

A primeira batalha usa personagens não canônicos, preservando Rosalia para uma etapa
posterior de identidade elemental:

* **Vanguarda Flama:** dano direto e pressão ofensiva.
* **Guardião Terrae:** proteção e escudos para o grupo.
* **Adepta Lux:** cura e sustentação.
* **Arcanista Fulmen:** dano ramificado contra múltiplos alvos.

O grupo enfrenta Batedor Umbra, Colosso Terrae e Conjurador Eol, três inimigos com
ritmos e funções diferentes.

##  Como Executar o Projeto

### Pré-requisitos
* **Unity Editor:** O projeto está atualmente serializado na versão `6000.3.2f1`.
* **Git LFS:** O projeto utiliza o *Large File Storage* para versionar corretamente as artes e áudios. Certifique-se de ter o Git LFS instalado na sua máquina (`git lfs install`).
