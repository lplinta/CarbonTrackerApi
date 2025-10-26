# language: pt
Funcionalidade: Gestão de Edifícios
  Como um usuário autenticado
  Eu quero gerenciar informações de edifícios
  Para rastrear o consumo de carbono por localização

Cenário: Cadastro bem-sucedido de um novo edifício
    Dado que eu esteja autenticado como um usuário válido
    Dado que eu tenha os seguintes dados de edifício:
      | campo        | valor              |
      | nome         | Sede Central       |
      | endereco     | Av. Paulista, 1000 |
      | cidade       | Itarare            |
      | tipoEdificio | Casa               |
    Quando eu enviar a requisição POST para o endpoint "/Edificio"
    Então o status code da resposta deve ser 201
    E o corpo da resposta deve conter o campo "nome" com valor "Sede Central"

Cenário: Busca bem-sucedida de um edifício existente
    Dado que eu esteja autenticado como um usuário válido
    Dado que o edifício com ID 999 e Nome "Filial Norte" existe no banco de dados
    Quando eu enviar a requisição GET para o endpoint "/Edificio/999"
    Então o status code da resposta deve ser 200
    E o corpo da resposta deve conter o campo "nome" com valor "Filial Norte"
