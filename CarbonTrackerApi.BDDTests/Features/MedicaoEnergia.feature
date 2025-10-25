# language: pt
Funcionalidade: Registro de medições de energia
Como usuário da API
Quero registrar medições de energia
Para que o sistema armazene os dados corretamente

    Cenário: Cadastro bem-sucedido de medição de energia
        Dado que o edifício com ID 1 existe no banco de dados
        Dado que o medidor de energia com ID 1 existe no banco de dados
        Dado que eu tenha os seguintes dados da medição:
          | campo            | valor                |
          | consumoValor     | 123,45               |
          | unidadeMedida    | kWh                  |
          | timestamp        | 2025-10-25T10:00:00Z |
          | medidorEnergiaId | 1                    |
        Quando eu enviar a requisição para o endpoint "/MedicaoEnergia"
        Então o status code da resposta deve ser 201
        E o corpo da resposta deve conter o campo "consumoValor" com valor "123,45"

    Cenário: Cadastro de medição com dados inválidos
        Dado que eu tenha os seguintes dados da medição:
          | campo            | valor                |
          | consumoValor     | -10                  |
          | unidadeMedida    | kWh                  |
          | timestamp        | 2025-10-25T10:00:00Z |
          | medidorEnergiaId | 1                    |
        Quando eu enviar a requisição para o endpoint "/MedicaoEnergia"
        Então o status code da resposta deve ser 400

    Cenário: Cadastro de medição com medidor inexistente
        Dado que eu tenha os seguintes dados da medição:
          | campo            | valor                |
          | consumoValor     | 50                   |
          | unidadeMedida    | kWh                  |
          | timestamp        | 2025-10-25T10:00:00Z |
          | medidorEnergiaId | 9999                 |
        Quando eu enviar a requisição para o endpoint "/MedicaoEnergia"
        Então o status code da resposta deve ser 404