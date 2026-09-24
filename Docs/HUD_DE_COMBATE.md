# HUD de Combate

O protótipo possui uma interface de batalha funcional inspirada no ritmo e na
legibilidade de RPGs de turno modernos, com identidade visual própria de Ecliptari.
Ela é construída em tempo de execução na cena `SampleScene` e consome apenas o
estado e os eventos públicos do núcleo de combate.

## Identidade visual

- Fundo astral em azul muito escuro, com linhas e halo discretos.
- Ouro para molduras, títulos e informação de prioridade.
- Ciano para interação, seleção e elementos de navegação.
- Uma cor própria para cada elemento dos combatentes.
- Cartões em camadas, cantos recortados e alto contraste para leitura rápida.

O projeto não copia recursos gráficos, ícones ou composição exata de outro jogo.
A referência está no fluxo de informação: ordem de ação sempre visível, personagem
ativo destacado, comandos concentrados em um canto e feedback imediato no campo.

## Mapa da tela

| Região | Função |
| --- | --- |
| Cabeçalho | Nome do confronto, contagem de sobreviventes e reinício. |
| Coluna esquerda | Ator atual e previsão da linha do tempo. |
| Centro | Formação de quatro aliados contra três inimigos. |
| Faixa inferior | Retratos resumidos do grupo, HP, AP, MP e escudo. |
| Canto inferior direito | Duas habilidades do ator e comando de passar turno. |
| Lateral direita | Histórico curto dos eventos mais recentes. |

## Interação

1. Clique em um cartão válido para selecionar o alvo.
2. Use uma das duas habilidades exibidas; custos indisponíveis desativam o botão.
3. Use **Passar** para encerrar a ação sem consumir habilidade.
4. Os inimigos escolhem e executam ações automaticamente.
5. Ao terminar a luta, o painel de resultado permite reiniciar o confronto.

O alvo inicial é escolhido automaticamente para reduzir cliques. Habilidades de
autocura, suporte e área respeitam a regra de alvo do próprio domínio, então a
interface não precisa conhecer fórmulas ou alterar o estado diretamente.

## Feedback e animações

- Barras de HP, AP e MP interpolam até o novo valor.
- O cartão ativo pulsa e o alvo selecionado recebe destaque ciano.
- Cartões reagem a apontar, pressionar, dano e derrota.
- Dano, cura e escudo geram números flutuantes com cores distintas.
- Mudanças de turno usam uma faixa animada no centro da tela.
- Golpes geram flash e tremor leves, mantendo o estado do combate determinístico.
- Vitória e derrota aparecem somente depois da janela de resolução.

Todas as animações usam tempo não escalado. Assim, no futuro, a velocidade visual
pode ser configurada sem alterar a linha do tempo ou as regras da batalha.

## Arquitetura

| Arquivo | Responsabilidade |
| --- | --- |
| `BattleHudController.cs` | Orquestra entrada, atualização, IA e apresentação de eventos. |
| `HudFactory.cs` | Cria painéis, textos, botões e barras de forma consistente. |
| `HudComponents.cs` | Gradiente, barras suaves, hover e pulso reutilizáveis. |
| `HudTheme.cs` | Paleta e cores elementais centralizadas. |
| `Ecliptari.UI.asmdef` | Isola a camada visual do núcleo e do conteúdo. |

O HUD depende de `Ecliptari.Combate.Core` e `Ecliptari.Conteudo.Prototipo`. O núcleo
não conhece Unity, Canvas, TextMeshPro nem a interface. Novos front-ends, testes ou
uma futura apresentação 3D podem usar o mesmo motor.

## Pontos de extensão

- Adicionar ícones ou retratos: trocar o conteúdo visual criado em
  `CriarCartaoCampo` e `CriarCartaoEquipe`, mantendo os mesmos vínculos de estado.
- Adicionar mais habilidades: substituir os dois slots fixos por uma lista gerada a
  partir de `DefinicaoHabilidade`.
- Adicionar Suprema: criar um comando dedicado que observa o recurso e envia uma
  solicitação de corte ao motor, sem executar efeitos diretamente pela UI.
- Adicionar status e reações: projetar os novos eventos em selos junto às barras e
  usar a fila de eventos para sequenciar o feedback.
- Substituir o cenário abstrato: manter o Canvas em sobreposição e usar o campo
  central apenas como âncora para personagens do mundo.

## Execução

Abra `Assets/Scenes/SampleScene.unity` e pressione **Play**. O bootstrap
`BattleHudDemoBootstrap` cria a interface e desativa o Canvas legado chamado
`Canvas`. Ele só é ativado na `SampleScene`, evitando interferência em outras cenas.

