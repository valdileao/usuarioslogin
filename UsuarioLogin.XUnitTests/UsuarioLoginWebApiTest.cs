using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Reflection;
using System.Threading.Tasks;
using UsuariosLogin.Models;
using Xunit;

namespace UsuariosLogin.XUnitTests
{
    public class UsuarioLoginWebApiTest
    {
        private readonly HttpClient _client;

        public UsuarioLoginWebApiTest()
        {
            var projectDir = GetProjectPath("", typeof(Startup).GetTypeInfo().Assembly);
            var server = new TestServer(new WebHostBuilder()
                .UseEnvironment("Development")
                .UseConfiguration(new ConfigurationBuilder()
                    .SetBasePath(projectDir)
                    .AddJsonFile("appsettings.json")
                    .Build()
                )
                .UseStartup<Startup>());
            _client = server.CreateClient();
        }

        [Fact]
        public async Task Registrar_usuario_passando_dados_obrigatorios_sucesso()
        {
            var request = RegistrarRequestHttpMessage(GetUsuarioMock());
            
            var response = await _client.SendAsync(request);

            Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);
        }


        [Fact]
        public async Task Registrar_usuario_email_ja_cadastrado_retornar_erro_existe()
        {
            var usuarioCriacao = GetUsuarioMock();
            usuarioCriacao.Email = "registra@asd.com";
            var requestCreate = RegistrarRequestHttpMessage(usuarioCriacao);
            
            var responseCreate = await _client.SendAsync(requestCreate);

            Assert.Equal(System.Net.HttpStatusCode.Created, responseCreate.StatusCode);

            var requestRegister = RegistrarRequestHttpMessage(usuarioCriacao);
            
            var response = await _client.SendAsync(requestRegister);
            var resultMessage = await response.Content.ReadAsStringAsync();
            ErrorDetails errorDetails = JsonConvert.DeserializeObject<ErrorDetails>(resultMessage);

            Assert.Equal("E-mail already exists", errorDetails.Message);
        }

        [Fact]
        public async Task Registrar_novo_usuario_com_campos_invalidos_retornando_mensagem_erro()
        {
            var usuarioTest = GetUsuarioMock();
            usuarioTest.Email = "aksdhj@asd.com";
            usuarioTest.LastName = null;
            var request = RegistrarRequestHttpMessage(usuarioTest);
            
            var response = await _client.SendAsync(request);
            var resultMessage = await response.Content.ReadAsStringAsync();
            ErrorDetails errorDetails = JsonConvert.DeserializeObject<ErrorDetails>(resultMessage);

            Assert.Equal("Missing fields", errorDetails.Message);
        }

        [Fact]
        public async Task Registrar_novo_usuario_com_campos_nao_preenchidos_retornando_mensagem_erro()
        {
            var usuarioTest = GetUsuarioMock();
            usuarioTest.Email = "aksdhj@asd.com";
            usuarioTest.Password = "hu";
            var request = RegistrarRequestHttpMessage(usuarioTest);
            
            var response = await _client.SendAsync(request);
            var resultMessage = await response.Content.ReadAsStringAsync();
            ErrorDetails errorDetails = JsonConvert.DeserializeObject<ErrorDetails>(resultMessage);

            Assert.Equal("Invalid fields", errorDetails.Message);

        }

        [Fact]
        public async Task Logar_usuario_campos_email_password_sucesso()
        {
            var request = LogarRequestHttpMessage(GetLoginEntity());
            
            var response = await _client.SendAsync(request);

            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Logar_usuario_campos_email_password_erro_mensagem_campos_invalidos()
        {
            var loginEntityTest = GetLoginEntity();
            loginEntityTest.Email = "asda@asdasd.com";
            var request = LogarRequestHttpMessage(loginEntityTest);
            
            var response = await _client.SendAsync(request);
            var resultMessage = await response.Content.ReadAsStringAsync();
            ErrorDetails errorDetails = JsonConvert.DeserializeObject<ErrorDetails>(resultMessage);

            Assert.Equal("Invalid e-mail or password", errorDetails.Message);
        }

        [Fact]
        public async Task Logar_usuario_campos_nao_preenchidos_erro_mensagem_campos_nao_existe()
        {
            var loginEntityTest = GetLoginEntity();
            loginEntityTest.Email = "aksdhj";
            var request = LogarRequestHttpMessage(loginEntityTest);
            
            var response = await _client.SendAsync(request);
            var resultMessage = await response.Content.ReadAsStringAsync();
            ErrorDetails errorDetails = JsonConvert.DeserializeObject<ErrorDetails>(resultMessage);

            Assert.Equal("Missing fields", errorDetails.Message);
        }

        [Fact]
        public async Task Resgatar_usuario_passando_token_retorno_usuario_sucesso()
        {
            var usuarioCriacao = GetUsuarioMock();
            usuarioCriacao.Email = "teste@teste.com";
            var requestCreate = RegistrarRequestHttpMessage(usuarioCriacao);
            var responseCreate = await _client.SendAsync(requestCreate);
            var resultMessage = await responseCreate.Content.ReadAsStringAsync();
            RegisterResponse signUpResponse = JsonConvert.DeserializeObject<RegisterResponse>(resultMessage);

            var request = RetrieveRequestHttpMessage(JwtBearerDefaults.AuthenticationScheme + " " + signUpResponse.Token);
            var response = await _client.SendAsync(request);

            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Resgatar_usuario_nao_passando_token_retorno_mensagem_erro_nao_autorizado()
        {
            var request = new HttpRequestMessage(new HttpMethod("GET"), "user/me");
            
            var response = await _client.SendAsync(request);
            var resultMessage = await response.Content.ReadAsStringAsync();
            
            ErrorDetails errorDetails = JsonConvert.DeserializeObject<ErrorDetails>(resultMessage);

            Assert.Equal("Unauthorized", errorDetails.Message);
        }

        [Fact]
        public async Task Resgatar_usuario_passando_token_expirado_retorno_mensagem_erro_nao_autorizado_sessao_invalida()
        {
            var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJoZWxsb0B3b3JsZC5jb20iLCJqdGkiOiJkZmMyMWM2MS05ZWJjLTRlMGQtODZhMS02YWY3OTZmZmI0NmYiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjRhZTliNjlhLWYzNTUtNGQ2MS1hMmI1LTYzNmUxODZlNDk5NCIsImV4cCI6MTYwMDk2NzQ3NX0.T7cpsZ65OA8jYVqkhpjo_E3NnexlRSeSnjvv-f36fKs";
            var request = RetrieveRequestHttpMessage(JwtBearerDefaults.AuthenticationScheme + " " + token);
            
            var response = await _client.SendAsync(request);
            var resultMessage = await response.Content.ReadAsStringAsync();
            
            ErrorDetails errorDetails = JsonConvert.DeserializeObject<ErrorDetails>(resultMessage);

            Assert.Equal("Unauthorized - invalid session", errorDetails.Message);
        }

        private RegisterEntity GetUsuarioMock()
        {
            var telefones = new List<EntityPhone>
            {
                new EntityPhone()
                {
                    Number = 18235186,
                    AreaCode = 213,
                    CountryCode = "+55"
                },

                new EntityPhone()
                {
                    Number = 789856756,
                    AreaCode = 213,
                    CountryCode = "+55"
                }
            };

            var novoUsuario = new RegisterEntity()
            {
                Name = "joao",
                LastName = "neves",
                Email = "joaoneves@asd.com",
                Password = "asd",
                Phones = telefones
            };

            return novoUsuario;
        }

        private LoginEntity GetLoginEntity()
        {
            return new LoginEntity
            {
                Email = "joaoneves@asd.com",
                Password = "asd"
            };
        }

        private HttpRequestMessage RegistrarRequestHttpMessage(RegisterEntity usuario)
        {
            return new HttpRequestMessage(new HttpMethod("POST"), "user/signup")
            {
                Headers = { { "X-Version", "1" } },
                Content = new ObjectContent<RegisterEntity>(usuario, new JsonMediaTypeFormatter(), "application/json")
            };
        }

        private HttpRequestMessage LogarRequestHttpMessage(LoginEntity usuario)
        {
            return new HttpRequestMessage(new HttpMethod("POST"), "user/signin")
            {
                Headers = { { "X-Version", "1" } },
                Content = new ObjectContent<LoginEntity>(usuario, new JsonMediaTypeFormatter(), "application/json")
            };
        }

        private HttpRequestMessage RetrieveRequestHttpMessage(string token)
        {
            return new HttpRequestMessage(new HttpMethod("GET"), "user/me")
            {
                Headers = { { "X-Version", "1" },
                    { HttpRequestHeader.Authorization.ToString(), token },
                }
            };
        }

        /// Ref: https://stackoverflow.com/a/52136848/3634867
        /// <summary>
        /// Gets the full path to the target project that we wish to test
        /// </summary>
        /// <param name="projectRelativePath">
        /// The parent directory of the target project.
        /// e.g. src, samples, test, or test/Websites
        /// </param>
        /// <param name="startupAssembly">The target project's assembly.</param>
        /// <returns>The full path to the target project.</returns>
        private static string GetProjectPath(string projectRelativePath, Assembly startupAssembly)
        {
            // Get name of the target project which we want to test
            var projectName = startupAssembly.GetName().Name;

            // Get currently executing test project path
            var applicationBasePath = System.AppContext.BaseDirectory;

            // Find the path to the target project
            var directoryInfo = new DirectoryInfo(applicationBasePath);
            do
            {
                directoryInfo = directoryInfo.Parent;

                var projectDirectoryInfo = new DirectoryInfo(Path.Combine(directoryInfo.FullName, projectRelativePath));
                if (projectDirectoryInfo.Exists)
                {
                    var projectFileInfo = new FileInfo(Path.Combine(projectDirectoryInfo.FullName, projectName, $"{projectName}.csproj"));
                    if (projectFileInfo.Exists)
                    {
                        return Path.Combine(projectDirectoryInfo.FullName, projectName);
                    }
                }
            }
            while (directoryInfo.Parent != null);

            throw new Exception($"Project root could not be located using the application root {applicationBasePath}.");
        }

    }
}
