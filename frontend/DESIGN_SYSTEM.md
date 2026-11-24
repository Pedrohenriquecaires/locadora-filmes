# Design System — Locadora

Identidade visual dark-first inspirada em interfaces SaaS discretas: superfícies grafite, contraste confortável e azul como cor de ação. O tema claro permanece disponível pelo controle de tema do menu, usando um azul mais sóbrio e superfícies neutras.

## Fundamentos

- **Fonte:** Inter, com fallback para fontes de sistema.
- **Cor primária:** `#4778F5` (azul).
- **Plano de fundo:** `#0D1016`.
- **Superfície/card:** `#1B202A`.
- **Texto:** `#E7EAF0`.
- **Texto secundário:** `#929BAB`.
- **Bordas:** `#2C3441`.
- **Acento suave:** `#202E50`.
- **Raio padrão:** `0.7rem`.
- **Tema claro:** fundo `#F5F6F8`, superfícies brancas e azul de ação `#315FBA`.

## Direção visual

- Usar o azul principalmente em ações, estados ativos e pequenos detalhes.
- Manter fundos e superfícies em tons próximos de grafite, com bordas discretas.
- Priorizar hierarquia tipográfica, espaçamento e alinhamento em vez de adicionar cards a todos os elementos.
- Aplicar hover e movimento apenas como feedback: elevação curta, mudança de borda e transições rápidas.
- Respeitar `prefers-reduced-motion` e manter foco visível para navegação por teclado.

## Componentes

- Preferir componentes Spartan em novas implementações (`@spartan-ng/helm/*`).
- O Brain fornece comportamento e acessibilidade; o Helm fornece os componentes estilizados e customizáveis.
- Tokens globais são definidos em `src/styles.css`.
- Estilos específicos de página ficam junto da página; estilos globais são reservados a padrões compartilhados.

## Temas

O tema escuro é aplicado por padrão na primeira visita. A escolha do usuário é persistida em `localStorage`. O tema claro sobrescreve os mesmos tokens em `[data-tema='claro']`.
