# Melhorias visuais - Locadora

Esta versão mantém a estrutura Angular e os serviços existentes, com uma atualização visual geral.

## O que mudou

- Nova identidade visual com tipografia DM Sans / Space Grotesk, paleta violeta e superfícies mais refinadas.
- Barra de navegação redesenhada, com marca, links ativos, identificação do administrador e alternância de tema.
- Dashboard, cartões, filtros, formulários, cards de filmes, avisos e estados vazios com estilos renovados.
- Tabelas com espaçamento e cabeçalhos consistentes.
- Correção da coluna de ações: o `display: flex` agora fica em um contêiner interno, sem transformar a própria célula `td` em flexbox e quebrar as bordas/alinhamento.
- Tabelas adaptadas para cards em telas estreitas.
- Formulário de cliente alinhado ao padrão visual dos demais formulários.
- Toast de sucesso ao salvar cliente e filme.
- Histórico do cliente exibe “Em aberto” quando a devolução ainda não ocorreu.
- Título e idioma do documento atualizados para pt-BR.

## Como executar

Requer uma versão do Node.js compatível com Angular 22 (Node 22.22.3 ou superior dentro da linha 22, ou uma versão 24 compatível).

```bash
npm install
npm start
```

O projeto continua dependendo da API configurada nos arquivos `src/environments/`.
