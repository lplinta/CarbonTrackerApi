# language: pt
Funcionalidade: Registro de medições de energia
Como administrador autenticado
Quero registrar medições de consumo de energia
Para que o sistema armazene os dados corretamente

    Cenário: Registrar medição com sucesso
        Dado que o serviço de medição está disponível
        E que o input de medição é válido
        Quando o usuário enviar uma requisição POST para o endpoint de medição
        Então o sistema deve retornar status 201
        E os dados da medição criada devem ser retornados

    Cenário: Input inválido retorna 400
        Dado que o input de medição é inválido
        Quando o usuário enviar uma requisição POST para o endpoint de medição
        Então o sistema deve retornar status 400

    Cenário: Serviço retorna null retorna 404
        Dado que o serviço retorna null ao adicionar a medição
        Quando o usuário enviar uma requisição POST para o endpoint de medição
        Então o sistema deve retornar status 404

    Cenário: Serviço lança InvalidOperationException retorna 404
        Dado que o serviço lança InvalidOperationException
        Quando o usuário enviar uma requisição POST para o endpoint de medição
        Então o sistema deve retornar status 404

    Cenário: Serviço lança ArgumentException retorna 400
        Dado que o serviço lança ArgumentException
        Quando o usuário enviar uma requisição POST para o endpoint de medição
        Então o sistema deve retornar status 400

    Cenário: Serviço lança Exception retorna 500
        Dado que o serviço lança uma exceção genérica
        Quando o usuário enviar uma requisição POST para o endpoint de medição
        Então o sistema deve retornar status 500