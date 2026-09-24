# Ecliptari: Ode to Praetorium — Fundamentos de Design

Este documento consolida as regras já definidas para o primeiro protótipo. Ele não
substitui a visão criativa do projeto: serve como fonte de verdade para implementação,
testes e balanceamento.

## Visão do jogo

**Ecliptari: Ode to Praetorium** é um RPG tático de turnos 2D no universo de
Nahvvatzal. O combate deve combinar decisões estratégicas, gerenciamento de recursos
e uma apresentação ágil, com pouca espera entre ações.

Os pilares do primeiro recorte são:

1. Linha do tempo governada por Velocidade.
2. Habilidades limitadas por AP e MP.
3. Interações elementais como principal camada estratégica.
4. Supremas capazes de interromper a ordem normal de ações.
5. Personagens definidos por dados, para facilitar criação e balanceamento.

Uma formação aliada padrão possui quatro integrantes. O motor pode executar formações
menores ou maiores quando uma batalha, invocação ou modo especial exigir.

## Recursos e atributos confirmados

| Atributo | Regra atual |
| --- | --- |
| HP | Ao chegar a zero, a entidade é derrotada. |
| MP | Recurso mágico consumido por determinadas habilidades. |
| AP | Recurso individual; habilidades possuem um custo e cada entidade regenera um valor fixo no início do próprio turno. |
| Velocidade | Determina a posição da entidade na linha do tempo. |
| Ataque | Participa do dano direto e serve como base para efeitos contínuos. |
| Defesa | Reduz o dano bruto recebido. |
| Elementalização | Multiplica a força de reações elementais complexas. |

Para o núcleo atual, AP é individual, começa cheio e é regenerado no início do turno.
MP e Pontos de Suprema são recursos distintos.

### Fórmulas iniciais

- Dano bruto: `(Ataque × Poder da habilidade) - (Defesa × 0,5)`.
- Dano contínuo: `15% do Ataque do aplicador` por ativação.
- O dano final nunca resulta em cura acidental por ficar negativo. No primeiro núcleo,
  ele é arredondado para o inteiro mais próximo e limitado a zero. Ainda será decidido
  no balanceamento se todo golpe válido deve causar ao menos 1 de dano.

Essas fórmulas são provisórias e devem permanecer centralizadas em um único módulo,
para que possam ser balanceadas sem alterar habilidades individualmente.

## Ciclo de turno

Cada turno possui três fases:

1. **Manutenção:** regenera AP e resolve efeitos de início de turno, como Queimadura
   e Envenenamento.
2. **Ação:** o controlador da entidade escolhe uma habilidade ou passa a vez.
3. **Resolução:** conclui consequências da ação e reduz a duração de efeitos.

Uma entidade derrotada deixa de receber turnos e não pode ser selecionada como alvo,
salvo por habilidades que explicitamente permitam ressuscitação.

### Linha do tempo contínua

A Velocidade produz um Valor de Ação recorrente por meio da fórmula
`10.000 / Velocidade`. O relógio avança até a próxima ação e, depois que a entidade
age, um novo intervalo é adicionado para ela. Entidades rápidas podem, portanto, agir
mais vezes que entidades lentas.

## Supremas e corte de turno

- Habilidades geram Pontos de Suprema.
- Ao completar a barra, a Suprema pode ser solicitada fora do turno do personagem.
- A Suprema interrompe temporariamente a ação corrente, resolve seu efeito e devolve
  o controle ao ponto anterior da linha do tempo.
- Para manter a batalha determinística, o comando pode ser recebido durante uma
  animação, mas sua resolução ocorre no próximo ponto seguro de interrupção.
- A regra de prioridade entre duas ou mais Supremas solicitadas em sequência ainda
  precisa ser definida.

## Elementos atualmente nomeados

| Elemento | Identidade mecânica inicial |
| --- | --- |
| Flama | Dano direto e Queimadura. |
| Aqua | Manipulação do próprio HP, cura e bônus condicionais. |
| Terrae | Defesa e resistência a controle. |
| Eol | Controle de grupo, campo e utilidade. |
| Crelix | Crítico, preservação e ataques concentrados. |
| Fulmen | Dano em área, ramificações e reações em cadeia. |
| Lux | Bônus, aprimoramento e purificação. |
| Umbra | Ocultação, penalidades, silêncio e confusão. |
| Vitae | Invocações que podem receber efeitos positivos e negativos. |
| Toxi | Dano contínuo e degradação. |
| Vis | Amplificação de dano e alteração de mecânicas. |

O material-fonte menciona **12 elementos**, mas nomeia 11. O décimo segundo não será
inventado na implementação. Ulrhtau, Inzurvitus e Gsoridon são conceitos cósmicos
futuros e permanecem fora do primeiro protótipo.

## Efeitos de status

- Efeitos possuem uma duração em turnos.
- Reaplicar o mesmo elemento renova a duração, sem somar turnos.
- Efeitos contínuos são resolvidos durante a Manutenção do alvo.
- Invocações de Vitae são entidades válidas para bônus e penalidades.
- Acúmulos de potência, múltiplos aplicadores e prioridade de limpeza ainda precisam
  de regras específicas.

## Progressão de poder

| Faixa | Nível | Tipagem |
| --- | ---: | --- |
| O Despertar | 0 | Nulo |
| O Despertar | 1 | Latente |
| O Despertar | 2 | Fagulha |
| Caminho do Combatente | 3 | Gume |
| Caminho do Combatente | 4 | Tomo |
| Caminho do Combatente | 5 | Périplo |
| A Ruptura | 6 | Atroz |
| A Ruptura | 7 | Cetro |
| A Ruptura | 8 | Diadema |
| A Ascensão | 9 | Empíreo |
| A Ascensão | 10 | Estrela |

`Tomo` aparece tanto como tipagem quanto como arma no material atual. Os dois
conceitos devem ser representados por tipos separados no código.

## Equipamentos

Armas atualmente previstas: Espada, Lança, Arco, Maça, Tomo e Catalisador.

Cada personagem possui seis espaços de Relíquia. Bônus de conjunto podem ser ativados
com 2, 4 e 6 peças. Relíquias têm raridade de 2 a 5 estrelas.

Conjuntos citados no material-fonte:

- Expansão Mental: Eol e Dispersão.
- Conclusão Imediata: Toxi, dano contínuo e redução de Defesa.
- Nascido do Crisol: Defesa e Escudo.
- Couraça: Aqua e HP; efeito de seis peças ainda incompleto.
- O Brinco Soberano: Crelix, crítico e fragmentos de Evig Kulde.
- Combatente de Ferro: Ataque e golpes básicos de armas físicas.

Relíquias ficam fora do primeiro combate vertical até que ataque, crítico, resistência,
escudo e modificadores tenham contratos estáveis.

## Decisões pendentes

Antes de cada sistema entrar em produção, as perguntas correspondentes devem ser
respondidas e incorporadas neste documento:

- Qual é o décimo segundo elemento?
- O piso definitivo deve permanecer em zero ou todo golpe válido causa ao menos 1?
- Como funcionam crítico, resistência elemental e escudos?
- Como são escolhidos alvo, área e prioridade de invocações?
- Como múltiplas Supremas simultâneas são ordenadas?
- Quais combinações e reações elementais existem e qual é sua fórmula?
- Qual é a condição de derrota de uma equipe com invocações ainda vivas?
- Qual é o efeito completo de seis peças da Couraça?

