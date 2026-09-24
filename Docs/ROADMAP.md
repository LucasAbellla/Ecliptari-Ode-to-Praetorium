# Roteiro de Desenvolvimento

O objetivo imediato não é construir todos os sistemas do RPG. É produzir uma batalha
vertical curta, divertida e representativa, que valide o ritmo e as decisões centrais.

## Marco 0 — Fundação

- [x] Consolidar as regras conhecidas.
- [x] Registrar lacunas sem preencher decisões criativas automaticamente.
- [x] Definir a separação entre dados, regras, estado e apresentação.
- [ ] Confirmar a versão oficial do Unity e atualizar a documentação.
- [ ] Definir as regras pendentes necessárias para o primeiro combate.

## Marco 1 — Núcleo de turno

- [x] Criar tipos seguros para Elemento, Arma, Tipagem e Fase.
- [x] Criar estado de participante independente de `MonoBehaviour`.
- [x] Implementar início, ação e fim de turno.
- [x] Implementar AP, MP, dano e derrota.
- [x] Implementar linha do tempo determinística.
- [x] Cobrir o núcleo com testes EditMode.

**Critério de saída:** uma batalha pode ser simulada por código até vitória ou derrota.

Implementação e uso: [Marco 1 — Núcleo de Combate](MARCO_1_NUCLEO_DE_COMBATE.md).

## Marco 1.1 — Interações fundamentais

- [x] Separar conteúdo de protótipo das regras centrais.
- [x] Criar uma formação padrão com quatro personagens não canônicos.
- [x] Criar três inimigos de papéis distintos.
- [x] Compor habilidades com efeitos reutilizáveis.
- [x] Implementar cura e escudo.
- [x] Implementar alvos em área.
- [x] Validar interações completas entre atacante, defensor e suporte.
- [x] Validar ataques coletivos aliados e inimigos.
- [x] Simular um confronto 4×3 completo até seu resultado.
- [x] Endurecer validações, imutabilidade e limites numéricos.

Detalhes: [Equipes de Protótipo](EQUIPES_DE_PROTOTIPO.md).

## Marco 2 — Primeiro combate controlável

- [x] Conectar a equipe padrão de quatro protótipos ao novo estado de batalha.
- [x] Criar um grupo de três inimigos de teste.
- [x] Permitir ataque básico, habilidade e passar turno.
- [x] Implementar seleção de alvo e painel de recursos.
- [x] Exibir ordem atual e próximas ações.
- [x] Criar IA básica previsível para inimigos.
- [x] Adicionar feedback visual e animações desacopladas das regras.

**Critério de saída:** o jogador conclui uma luta inteira na cena de protótipo.

Implementação e uso: [HUD de Combate](HUD_DE_COMBATE.md).

## Marco 3 — Ritmo e Supremas

- [ ] Implementar geração de Pontos de Suprema.
- [ ] Implementar solicitação fora de turno e fila de interrupções.
- [ ] Definir pontos seguros de corte nas sequências visuais.
- [ ] Adicionar antecipação, impacto, câmera e retorno visual.
- [ ] Permitir aceleração das animações sem afetar as regras.

**Critério de saída:** a Suprema pode cortar um turno sem corromper estado ou ordem.

## Marco 4 — Identidade elemental

- [ ] Implementar efeitos com duração e renovação.
- [ ] Implementar Queimadura e Envenenamento.
- [ ] Implementar invocação Vitae de Rosalia.
- [ ] Definir e implementar um pequeno conjunto de reações.
- [ ] Mostrar resíduos e reações de forma legível na interface.

**Critério de saída:** elementos alteram decisões e não apenas a cor do dano.

## Marco 5 — Vertical slice

- [ ] Criar um grupo pequeno de aliados e inimigos complementares.
- [ ] Balancear uma batalha de 3 a 6 minutos.
- [ ] Adicionar tutorial contextual curto.
- [ ] Refinar efeitos, áudio, câmera e transições.
- [ ] Realizar testes de ritmo, clareza e dificuldade.

**Critério de saída:** o protótipo comunica a identidade de Ecliptari sem explicação
externa e pode ser demonstrado do início ao fim.

## Fora do primeiro recorte

- Sistema completo de Relíquias.
- Progressão de raridades e economia.
- Todos os elementos e reações.
- Conteúdo cósmico e glitches de interface.
- Campanha, exploração e sistemas online.

Esses itens permanecem no projeto, mas só entram depois que o combate-base estiver
validado.

