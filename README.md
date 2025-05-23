# Teste de carga componente Signal-R

Bem-vindo ao guia de introdução para o projeto de testes de carga do [SignalR](https://learn.microsoft.com/aspnet/signalr/overview/getting-started/introduction-to-signalr) Neste documento, você encontrará informações essenciais para iniciar a jornada de testes de carga em aplicações que utilizam o [SignalR](https://learn.microsoft.com/aspnet/signalr/overview/getting-started/introduction-to-signalr) como tecnologia de comunicação em tempo real. O [SignalR](https://learn.microsoft.com/aspnet/signalr/overview/getting-started/introduction-to-signalr) desempenha um papel fundamental na criação de experiências interativas e dinâmicas, permitindo a troca contínua de informações entre clientes e servidor. No entanto, garantir que sua aplicação possa lidar com cargas crescentes e picos de tráfego é crucial para fornecer uma experiência de usuário confiável e responsiva. Este projeto visa fornecer diretrizes, ferramentas e exemplos para ajudá-lo a projetar, executar e analisar testes de carga abrangentes em sua implementação do [SignalR](https://learn.microsoft.com/aspnet/signalr/overview/getting-started/introduction-to-signalr).

## Componentes

O projeto de testes de carga do SignalR é construído em torno de três componentes essenciais, cada um desempenhando um papel crucial no ecossistema de comunicação em tempo real. A primeira peça-chave é a aplicação servidor (`./serverSinalR`), responsável por gerenciar a lógica de comunicação e coordenar as trocas de mensagens entre os clientes. A segunda parte vital é a aplicação receptora de eventos (`./clientSignalR`), que simula os clientes que recebem e processam as mensagens enviadas pelo servidor. Essa aplicação desempenha um papel fundamental na avaliação da capacidade de resposta e escalabilidade do sistema, permitindo medir como a infraestrutura lida com a carga de entrada. Por último, temos a aplicação emissor de mensagens (`./clientProducerSignalR`), que simula clientes que enviam mensagens para o servidor. Essa aplicação é crucial para avaliar a capacidade do sistema de lidar com um alto volume de mensagens enviadas a partir de múltiplos pontos de origem. Juntos, esses três componentes formam uma estrutura abrangente que nos permite realizar testes de carga realistas e detalhados no ambiente SignalR, identificando pontos fortes e áreas de melhoria na capacidade de comunicação em tempo real da sua aplicação.

## Pre-requisitos

- [.NET Core 6+](https://learn.microsoft.com/aspnet/signalr/overview/getting-started/introduction-to-signalr)

## Como testar

Para testar a aplicação, o ideal é realizar um processo de build de todos os componentes envolvidos em modo `Release`, também é preferível que as aplicações rodem em máquinas virtuais separadas, pois assim evitamos que elas disputem recursos entre si, garantindo uma melhor acurácia do teste.

### Servidor

Esse componente é o mais dispensável para realização deste teste, tendo em vista que o cliente já vai ter a sua própria implementação publicada em um ambiente de staging, porém, este componente ajuda a testar localmente. Para subir essa aplicação rode a sequência de comandos a partir do diretório raiz do projeto:

```dotnetcli

cd serverSignalR

dotnet restore

dotnet run -c Release

```

### Consumidor

Este componente é o responsável por simular os clientes que recebem as mensagens enviadas pelo servidor. A aplicação é um console, onde podemos passar os parâmetros:

- URL do servidor SignalR (obrigatório)
- Caminho onde será salvo o arquivo de resultado (obrigatório)
- `--clients` - Quantidade de clientes que serão simulados
- `--duration` - Duração do teste em segundos
- `--reconnect` - Se habilitado força a reconexao de clients a cada 10 segundos
- `--comment` - Comentário que será adicionado ao arquivo de resultado

Exemplo de execução:

```dotnetcli

cd clientSignalR

dotnet restore 

dotnet run -c Release --url http://localhost:5062/notificationhub --path C:\repos\loadtest-signalr\results\ --clients 500 --duration 100 --reconnect true --comment "Teste de carga"

```

### Produtor

Este componente é o responsável por simular os clientes que enviam as mensagens para o servidor. A aplicação é um console, onde podemos passar os parâmetros:

- URL do servidor SignalR (obrigatório)
- `--clients` - Quantidade de clientes que serão simulados
- `--duration` - Duração do teste em segundos
- `--mps` - Numero de mensagens por segundo, de cada cliente
- `--consumerClients` - Quantidade de clientes do lado do consumidor
- `--messageSize` - Tamanho da mensagem a ser enviada
- `--comment` - Comentário que será adicionado ao arquivo de resultado

Exemplo de execução:

```dotnetcli

cd clientProducerSignalR

dotnet restore

dotnet run --url http://localhost:5062/notificationhub --clients 500 --consumerClients 500 --duration 100 --mps 50000 --messageSize 1024 --comment "Teste de carga"

```

## Resultados

Os resultados dos testes serão salvos em arquivos `txt` no caminho especificado no parâmetro `--path` do consumidor. Cada linha representa um resultado de um teste, e cada coluna representa um atributo do resultado. Os atributos são:

- `Date` - Data e hora do teste
- `Clients` - Quantidade de clientes simulados
- `Duration` - Duração do teste em segundos
- `MessagesPerSecond` - Quantidade de mensagens enviadas por segundo
- `TotalMessages` - Quantidade de mensagens recebidas
- `Comment` - Comentário do teste
- `AverageLatency` - Media de latencia entre envio e recebimento da mensagem
- `Reconnect` - Indica se o teste foi realizado com reconexão dos clientes
