# language: pt
Funcionalidade: Gerenciamento de edifícios
Como usuário autenticado da API
Quero cadastrar, consultar e atualizar edifícios
Para que o sistema gerencie corretamente os registros de consumo de energia

    Cenário: Cadastro bem-sucedido de edifício
        Dado que eu possuo os seguintes dados do edifício:
          | campo        | valor            |
          | nome         | Edifício Central |
          | cidade       | São Paulo        |
          | endereco     | Rua tal          |
          | tipoEdificio | Prédio           |
        Quando eu enviar uma requisição POST para "edificio"
        Então o status code da resposta deve ser 201
        E o corpo deve conter o campo "nome" com valor "Edifício Central"

    Cenário: Consulta de edifício existente por ID
        Dado que existe um edifício cadastrado com ID 1
        Quando eu enviar uma requisição GET para "edificio/1"
        Então o status code da resposta deve ser 200
        E o corpo deve conter o campo "id" com valor 1

    Cenário: Tentativa de buscar edifício inexistente
        Dado que não existe um edifício com ID 999
        Quando eu enviar uma requisição GET para "edificio/999"
        Então o status code da resposta deve ser 404
        E o corpo deve conter a mensagem "não encontrado"

    Cenário: Atualização bem-sucedida de edifício
        Dado que existe um edifício cadastrado com ID 1
        E que eu possuo os seguintes novos dados do edifício:
          | campo        | valor               |
          | nome         | Edifício Atualizado |
          | cidade       | São Paulo           |
          | endereco     | Rua tal             |
          | tipoEdificio | Prédio              |
        Quando eu enviar uma requisição PUT para "edificio/1"
        Então o status code da resposta deve ser 200
        E o corpo deve conter o campo "nome" com valor "Edifício Atualizado"