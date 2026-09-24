# Equipes de Protótipo

Estas formações existem para validar o núcleo antes da introdução de personagens
canônicos. Nomes, números e habilidades são funcionais e substituíveis. **Rosalia não
faz parte destes catálogos.**

## Formação aliada padrão

Uma equipe padrão possui quatro integrantes. O motor aceita outras quantidades para
comportar tutoriais, chefes, invocações e modos especiais; o número quatro é uma regra
de formação, não uma limitação estrutural.

### Vanguarda Flama

Atacante rápido de alvo único: 900 HP, 60 MP, 5 AP, 120 de Velocidade, 115 de Ataque
e 55 de Defesa.

- **Golpe Candente:** poder 1,0, sem custo.
- **Investida Flamejante:** poder 1,75, custa 2 AP e 10 MP.

### Guardião Terrae

Defensor lento: 1.250 HP, 50 MP, 6 AP, 90 de Velocidade, 75 de Ataque e 110 de
Defesa.

- **Golpe de Pedra:** poder 0,8, sem custo.
- **Muralha Terrae:** concede escudo igual a `60% da Defesa + 25`; custa 2 AP e 8 MP.

### Adepta Lux

Suporte: 800 HP, 80 MP, 5 AP, 105 de Velocidade, 95 de Ataque e 45 de Defesa.

- **Pulso Luminoso:** poder 0,75, sem custo.
- **Luz Restauradora:** cura igual a `90% do Ataque + 35`; custa 2 AP e 12 MP.

### Arcanista Fulmen

Especialista em múltiplos alvos: 760 HP, 90 MP, 5 AP, 110 de Velocidade, 105 de
Ataque e 40 de Defesa.

- **Centelha Fulmen:** poder 0,9 contra um inimigo, sem custo.
- **Descarga Ramificada:** poder 0,72 contra todos os inimigos; custa 2 AP e 14 MP.

## Grupo inimigo de teste

### Batedor Umbra

Inimigo frágil e mais rápido do confronto: 620 HP, 130 de Velocidade, 92 de Ataque e
35 de Defesa.

- **Corte Velado:** ataque básico de poder 0,9.
- **Assalto Sombrio:** ataque de poder 1,25, custa 1 AP e 8 MP.

### Colosso Terrae

Inimigo lento e resistente: 1.650 HP, 70 de Velocidade, 125 de Ataque e 120 de
Defesa.

- **Punho de Rocha:** ataque básico de poder 1,0.
- **Carga Fortificada:** causa dano de poder 1,15 e concede ao próprio Colosso um
  escudo igual a 50% da Defesa; custa 2 AP e 8 MP.

### Conjurador Eol

Inimigo de pressão coletiva: 840 HP, 95 de Velocidade, 102 de Ataque e 45 de Defesa.

- **Lâmina de Ar:** ataque básico de poder 0,8.
- **Vendaval Hostil:** poder 0,62 contra os quatro aliados; custa 2 AP e 12 MP.

## Interações validadas

- Vanguarda, Guardião e Adepta executam o ciclo proteger, receber dano excedente e
  recuperar HP.
- Arcanista atinge três inimigos por uma única ação em área.
- Conjurador atinge quatro aliados sem receber uma lista manual de alvos.
- Colosso causa dano e aplica escudo próprio na mesma habilidade.
- Uma simulação determinística 4×3 escolhe habilidades e alvos até produzir vitória
  ou derrota, com limite de segurança contra batalhas infinitas.
- Entidades derrotadas deixam de agir e de ser selecionadas.

## Arquitetura das habilidades

Uma habilidade é formada por custo, regra de alvo e uma lista imutável de efeitos.
Cada efeito pode agir nos alvos selecionados ou no usuário. Dano, cura e escudo usam o
mesmo contrato de resolução, permitindo combinações sem condicionais específicas no
motor.

Os tipos de alvo atuais são inimigo único, aliado único, próprio usuário, qualquer
entidade, todos os inimigos e todos os aliados. Alvos em área são resolvidos pelo
motor, evitando listas incompletas ou manipuladas pela interface.

## Natureza dos catálogos

`CatalogoPersonagensBasicos` e `CatalogoInimigosBasicos` são conteúdo determinístico de
protótipo. Permanecem separados do núcleo permanente. No Marco 2, `ScriptableObjects`
poderão construir as mesmas definições sem alterar o motor, os efeitos ou a linha do
tempo.

