# language: pt
Funcionalidade: Autenticação de Usuários
Verifica o comportamento da API de autenticação, incluindo login e registro.

Cenario: Login bem-sucedido com credenciais válidas
    Dado que existe um usuário válido com username "usuario1" e senha "senha123"
    Quando eu envio um POST para "/auth/login" com essas credenciais
    Entao a resposta deve ter status code 200
    E o corpo deve conter um token JWT

Cenario: Login com credenciais inválidas
    Dado que não existe um usuário com username "naoexiste"
    Quando eu envio um POST para "/auth/login" com username "naoexiste" e senha "errada"
    Entao a resposta deve ter status code 401
    E o corpo deve conter a mensagem "Credenciais inválidas."

Cenario: Registro de novo usuário com sucesso
    Dado que não existe um usuário com username "novoUsuario"
    Quando eu envio um POST para "/auth/register" com os dados válidos
    Entao a resposta deve ter status code 201
    E o corpo deve conter o username "novoUsuario"

Cenario: Tentativa de registro de usuário já existente
    Dado que já existe um usuário com username "usuarioExistente"
    Quando eu envio um POST de registro para "/auth/register" com username "usuarioExistente"
    Entao a resposta deve ter status code 409
    E o corpo deve conter a mensagem "Nome de usuário já existe."