# Arquitetura Técnica do Combate

## Objetivo

A arquitetura deve permitir um combate rápido na apresentação e rigoroso nas regras.
Animações podem se sobrepor visualmente, mas mudanças de estado precisam ser
determinísticas, registráveis e testáveis.

## Separação de responsabilidades

### Dados de conteúdo

`ScriptableObject` descreve personagens, habilidades, efeitos, inimigos e relíquias.
Esses objetos são configuração imutável durante uma batalha; não armazenam HP atual,
durações ou outros estados da partida.

### Estado de execução

Objetos comuns de C# mantêm o estado mutável da batalha:

- participantes e equipes;
- HP, MP, AP e Pontos de Suprema atuais;
- efeitos ativos e suas durações;
- posição na linha do tempo;
- invocações e vínculo com o invocador;
- fase e ação atualmente em resolução.

Esse estado não deve depender de componentes visuais, `Slider`, animações ou cenas.

### Regras e resolução

Um núcleo de regras recebe comandos e produz resultados. Exemplos de comandos:

- usar habilidade;
- passar turno;
- solicitar Suprema;
- selecionar alvo.

Exemplos de resultados:

- dano causado;
- recurso consumido ou regenerado;
- efeito aplicado, renovado ou removido;
- participante derrotado;
- turno iniciado ou concluído.

As regras validam custos e alvos antes de alterar o estado. Uma ação inválida não pode
consumir recursos parcialmente.

Habilidades são composições de efeitos reutilizáveis. O motor conhece o contrato de
resolução, mas não possui condicionais específicas para personagens. Um efeito recebe
um contexto seguro e pode causar dano, curar ou conceder escudo por meio dele. Novos
efeitos poderão ampliar esse contexto sem alterar o ciclo de turnos.

### Apresentação

Controladores Unity escutam os resultados e atualizam:

- sprites e animações;
- barras de HP, AP, MP e Suprema;
- números flutuantes e efeitos;
- linha do tempo;
- seleção de habilidades e alvos;
- som, câmera e vibração visual.

A interface nunca calcula dano nem decide a ordem dos turnos.

## Fluxo de uma ação

```text
Entrada do jogador/IA
        ↓
Validação do comando
        ↓
Reserva de custos e alvos
        ↓
Resolução determinística
        ↓
Sequência de eventos de combate
        ↓
Apresentação e animação
        ↓
Próximo ponto seguro / próxima fase
```

Essa sequência permite acelerar ou pular animações sem mudar o resultado da batalha.

## Máquina de estados sugerida

1. `PreparandoBatalha`
2. `InicioTurno`
3. `AguardandoComando`
4. `ResolvendoAcao`
5. `JanelaDeInterrupcao`
6. `FimTurno`
7. `Vitoria` ou `Derrota`

Supremas entram em uma fila própria. A entrada pode ser registrada em qualquer
momento permitido, mas a execução começa apenas num ponto seguro entre eventos
atômicos. Isso evita que uma interrupção aconteça no meio de uma alteração de HP ou
de uma remoção de entidade.

## Estrutura de pastas alvo

```text
Assets/_Scripts/
  Core/              Regras puras, comandos, resultados e cálculos
  Conteudo/          Catálogos determinísticos e conteúdo de protótipo
  Data/              ScriptableObjects e definições serializáveis
  Combat/            Estado e orquestração de batalha
  Abilities/         Efeitos reutilizáveis de habilidades
  StatusEffects/     Aflições, bônus e penalidades
  AI/                Escolha de ações de inimigos
  Presentation/      Pontes entre eventos e animações
  UI/                Controles e painéis
  Editor/            Ferramentas de autoria e validação
Assets/Tests/
  EditMode/          Regras, fórmulas, turnos e efeitos
  PlayMode/          Integração de cena e interface
```

As pastas serão criadas à medida que receberem código real, evitando estruturas vazias.

## Convenções iniciais

- Nomes de tipos e regras em português, acompanhando o código atual.
- Enums para Elemento, Tipo de Arma, Tipagem e Fase de Turno; strings ficam apenas em
  textos apresentados ao jogador.
- Valores de balanceamento são dados configuráveis ou constantes centralizadas.
- Uma batalha deve poder ser simulada sem carregar uma cena.
- Eventos de combate carregam os valores usados no cálculo, facilitando depuração.
- Coleções expostas pelo domínio são imutáveis ou cópias defensivas.
- Operações numéricas são validadas e limitadas para evitar estouros.
- Alvos em área são resolvidos pelo motor, não fornecidos livremente pela interface.
- Aleatoriedade usa uma fonte injetável com semente, permitindo repetir testes.
- Código de regra recebe testes EditMode antes de depender de animações.

## Migração do protótipo atual

`EntidadeBase` já cumpre o papel inicial de definição de personagem, mas usa strings
para elemento e arma. A migração para enums precisa preservar o asset de Rosalia.

`EntidadeBatalha` mistura estado de execução com interface por meio do `Slider`. Ele
continuará funcionando durante o primeiro passo; depois, o estado será extraído e o
componente se tornará uma ponte de apresentação. Essa migração incremental evita
quebrar a cena atual enquanto o núcleo é construído.

## Definição de pronto para sistemas de combate

Um sistema só é considerado pronto quando:

- sua regra está descrita em `GAME_DESIGN.md`;
- possui validação de dados inválidos;
- tem testes das regras principais e casos-limite;
- não depende da duração de animações;
- informa a interface por eventos;
- pode ser inspecionado durante a execução;
- não introduz erros ou avisos novos no Console da Unity.

