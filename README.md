# CP2 ADVANCED BUSINESS DEVELOPMENT WITH .NET

- Luna Faustino Lima - RM552473
- Larissa Araújo Gama Alvarenga - RM96496

## Estrutura do Projeto

O projeto segue a arquitetura de mensageria com os seguintes componentes:

1. **Models**: Classes de domínio para representar as mensagens
   - `BaseMessage`: Classe base com timestamp
   - `FruitMessage`: Informações sobre frutas da época
   - `UserMessage`: Informações sobre usuários

2. **ValidationService**: Serviço responsável por validar as mensagens
   - Valida mensagens de frutas
   - Valida mensagens de usuários
   - Encaminha mensagens válidas para os receivers

3. **Senders**:
   - `FruitSender`: Envia informações sobre frutas para o ValidationService
   - `UserSender`: Envia informações sobre usuários para o ValidationService

4. **Receivers**:
   - `FruitReceiver`: Recebe mensagens validadas sobre frutas
   - `UserReceiver`: Recebe mensagens validadas sobre usuários

5. **Program**: Classe principal para execução da aplicação

## Configuração do RabbitMQ com Docker

1. Navegue até a pasta do projeto onde está o arquivo `docker-compose.yml`
2. Execute o comando para iniciar o container:

```
docker-compose up -d
```

3. Verifique se o container está rodando:

```
docker ps
```

4. Acesse a interface de gerenciamento do RabbitMQ:
   - Abra o navegador e acesse: http://localhost:15672/
   - Usuário: guest
   - Senha: guest

## Fluxo de Execução

### Preparação:

1. Execute o container do RabbitMQ usando Docker
2. Compile o projeto no Visual Studio 2022 (CTRL + Shift + B)
3. Entre na pasta do projeto e siga no caminho `bin\Debug\net8.0` para executar o aplicativo Console

### Ordem de Execução e exemplo de Teste:

Para testar o aplicativo, você deve iniciar os componentes nesta ordem:

1. Abra 5 instâncias do aplicativo console
2. Na primeira instância, selecione a opção 1 para iniciar o ValidationService
3. Na segunda instância, selecione a opção 2 para iniciar o Receiver1 (Frutas)
4. Na terceira instância, selecione a opção 3 para iniciar o Receiver2 (Usuários)
5. Na quarta instância, selecione a opção 4 para enviar os dados de frutas
6. Na quinta instância, selecione a opção 5 para enviar os dados de usuários
7. Então volte nas instâncias 2 e 3 para ver os dados recebidos

## Verificação no RabbitMQ Management

Após executar o teste, você pode verificar no painel de gerenciamento do RabbitMQ:

1. Acesse http://localhost:15672/ (usuário: guest, senha: guest)
2. Verifique as exchanges criadas: `fruit_exchange` e `user_exchange`
3. Verifique as filas criadas: `fruit_validation_queue`, `user_validation_queue`, `fruit_queue` e `user_queue`
4. Na aba "Queues and Streams", verifique as mensagens publicadas e entregues