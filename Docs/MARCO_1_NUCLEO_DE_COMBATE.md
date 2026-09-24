# Marco 1 — Núcleo de Combate

O Marco 1 fornece uma batalha completa em memória, sem dependências visuais. Ele pode
ser executado e testado sem cena, câmera, animação ou interface.

## O que está implementado

- Dados imutáveis de combatentes e habilidades.
- Estado mutável de HP, MP e AP separado dos dados.
- Validação de atributos e identificadores.
- Linha do tempo contínua por Valor de Ação.
- Fases de Preparação, Manutenção, Ação e Resolução.
- Regeneração de AP no início do turno.
- Uso de habilidades compostas por efeitos, com custo de AP e MP.
- Cálculo e aplicação de dano direto.
- Cura, escudo e absorção de dano.
- Seleção de alvo único, próprio ou em área.
- Passagem voluntária de turno.
- Derrota de entidades e condições de vitória/derrota.
- Validação atômica: comandos inválidos não alteram o estado.
- Previsão de próximos turnos sem modificar a batalha real.
- Eventos de domínio para alimentar interface e animações.
- Testes EditMode das regras principais.

## Linha do tempo

Cada combatente recebe um intervalo de ação:

```text
intervalo = 10.000 / Velocidade
```

O relógio avança até o menor Valor de Ação restante. Quando um combatente age, seu
próximo intervalo é adicionado à linha. Por isso, Velocidade alta pode gerar ações
extras em vez de apenas definir a ordem inicial.

Empates são resolvidos por:

1. maior Velocidade;
2. ordem original de entrada na batalha.

Esses critérios tornam a simulação determinística e repetível.

## Ciclo do motor

```text
Iniciar
  → Manutenção automática
  → Ação
      → UsarHabilidade ou PassarTurno
  → Resolução
      → apresentação consome os eventos
      → ConcluirResolucao
  → próximo turno ou fim da batalha
```

O motor permanece em `Resolucao` até receber `ConcluirResolucao`. A apresentação pode
usar essa janela para exibir animações sem permitir um novo comando cedo demais.

## Exemplo mínimo

```csharp
var motor = new MotorBatalha(new[] { definicaoAliada, definicaoInimiga });
motor.Iniciar();

EstadoCombatente ator = motor.AtorAtual;
ResultadoComando resultado = motor.UsarHabilidade(
    ator.Id,
    "inimigo_01",
    ataqueBasico);

if (resultado.Executado)
{
    IReadOnlyList<EventoCombate> eventos = motor.DrenarEventos();
    // A camada visual apresenta os eventos em sequência.
    motor.ConcluirResolucao();
}
```

## Tipos centrais

| Tipo | Responsabilidade |
| --- | --- |
| `DefinicaoCombatente` | Atributos imutáveis de um participante. |
| `EstadoCombatente` | HP, MP e AP atuais. |
| `DefinicaoHabilidade` | Custos, poder e regra de alvo. |
| `LinhaDoTempoContinua` | Ordem, recorrência e previsão de turnos. |
| `CalculadoraDano` | Fórmulas centralizadas de dano direto e contínuo. |
| `MotorBatalha` | Validação, fases, resolução e fim da batalha. |
| `EventoCombate` | Saída consumida pela camada visual. |
| `ResultadoComando` | Sucesso ou motivo de recusa de uma ação. |

## Decisões do marco

- AP é individual e começa cheio.
- AP regenera no início do turno, limitado ao máximo.
- MP e Pontos de Suprema serão recursos separados.
- O dano direto é arredondado para o inteiro mais próximo e limitado a zero.
- Uma entidade derrotada é automaticamente ignorada pela linha do tempo.
- O fim da batalha acontece depois da janela de Resolução, permitindo apresentar o
  golpe final antes da tela de resultado.

## Limites intencionais

O Marco 1 não conecta Rosalia, `ScriptableObjects`, cenas ou interface ao novo motor.
Essa integração pertence ao Marco 2. Efeitos de status, reações, invocações e Supremas
também permanecem fora do núcleo atual, embora a arquitetura já possua pontos de
extensão para eles.

## Validação

Os 34 testes EditMode cobrem fórmula de dano, Valor de Ação, ações extras por
Velocidade, previsão sem mutação, recursos, regeneração, atomicidade de comandos,
eventos, fim de batalha, habilidades compostas, alvos em área, cura, escudo e a
interação das formações de protótipo. Todos os 34 foram executados com sucesso pelo
runtime Mono distribuído junto ao Unity.

O núcleo e a assembly de testes também foram compilados sem avisos com o compilador C#
distribuído junto ao Unity 6000.3.2f1. O Editor instalado na máquina exige uma licença
válida para executar a suíte pelo Test Runner gráfico ou em modo automatizado.

