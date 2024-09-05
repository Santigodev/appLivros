using appLivros;
using MySql.Data.MySqlClient;

namespace ControleAlunos
{
    class Program
    {
        private const string ConnectionString = "Server=localhost;Database=db_aulas_2024;Uid=shilton;Pwd=1234567;SslMode=none;";

        static void Main(string[] args)
        {
            while (true)
            {
                MostrarMenu();

                string opcao = Console.ReadLine();

                if (!ProcessarOpcaoMenu(opcao))
                {
                    break;
                }
            }
        }

        private static void MostrarMenu()
        {
            Console.WriteLine("1. Adicionar Aluno");
            Console.WriteLine("2. Listar Alunos");
            Console.WriteLine("3. Editar Aluno");
            Console.WriteLine("4. Excluir Aluno");
            Console.WriteLine("5. Listar Alunos por Curso");
            Console.WriteLine("6. Sair");
            Console.Write("Escolha uma opção: ");
        }

        private static bool ProcessarOpcaoMenu(string opcao)
        {
            switch (opcao)
            {
                case "1":
                    AdicionarAluno();
                    break;
                case "2":
                    ListarAlunos();
                    break;
                case "3":
                    EditarAluno();
                    break;
                case "4":
                    ExcluirAluno();
                    break;
                case "5":
                    ListarAlunosPorCurso();
                    break;
                case "6":
                    return false;
                default:
                    Console.WriteLine("Opção inválida!");
                    break;
            }
            return true;
        }

        private static void AdicionarAluno()
        {
            try
            {
                var aluno = ObterDadosAluno();

                using (var connection = new MySqlConnection(ConnectionString))
                {
                    connection.Open();
                    string query = "INSERT INTO alunos (Nome, Idade, Curso, DataMatricula) VALUES (@Nome, @Idade, @Curso, @DataMatricula)";
                    var cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@Nome", aluno.Nome);
                    cmd.Parameters.AddWithValue("@Idade", aluno.Idade);
                    cmd.Parameters.AddWithValue("@Curso", aluno.Curso);
                    cmd.Parameters.AddWithValue("@DataMatricula", aluno.DataMatricula);
                    cmd.ExecuteNonQuery();
                }
                Console.WriteLine("Aluno adicionado com sucesso!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao adicionar aluno: {ex.Message}");
            }
        }

        private static void ListarAlunos()
        {
            try
            {
                using (var connection = new MySqlConnection(ConnectionString))
                {
                    connection.Open();
                    string query = "SELECT Id, Nome, Idade, Curso, DataMatricula FROM alunos";
                    var cmd = new MySqlCommand(query, connection);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (!reader.HasRows)
                        {
                            Console.WriteLine("Nenhum aluno cadastrado.");
                            return;
                        }

                        while (reader.Read())
                        {
                            Console.WriteLine($"ID: {reader["Id"]}, Nome: {reader["Nome"]}, Idade: {reader["Idade"]}, Curso: {reader["Curso"]}, Data de Matrícula: {reader["DataMatricula"]}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao listar alunos: {ex.Message}");
            }
        }

        private static void EditarAluno()
        {
            try
            {
                int id = ObterIdAluno();

                using (var connection = new MySqlConnection(ConnectionString))
                {
                    connection.Open();
                    var alunoExistente = ObterAlunoPorId(connection, id);
                    if (alunoExistente == null)
                    {
                        Console.WriteLine("Aluno não encontrado.");
                        return;
                    }

                    var alunoAtualizado = AtualizarDadosAluno(alunoExistente);

                    string query = "UPDATE alunos SET Nome = @Nome, Idade = @Idade, Curso = @Curso, DataMatricula = @DataMatricula WHERE Id = @Id";
                    var cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@Nome", alunoAtualizado.Nome);
                    cmd.Parameters.AddWithValue("@Idade", alunoAtualizado.Idade);
                    cmd.Parameters.AddWithValue("@Curso", alunoAtualizado.Curso);
                    cmd.Parameters.AddWithValue("@DataMatricula", alunoAtualizado.DataMatricula);
                    cmd.Parameters.AddWithValue("@Id", alunoAtualizado.Id);
                    cmd.ExecuteNonQuery();

                    Console.WriteLine("Aluno editado com sucesso!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao editar aluno: {ex.Message}");
            }
        }

        private static void ExcluirAluno()
        {
            try
            {
                int id = ObterIdAluno();

                using (var connection = new MySqlConnection(ConnectionString))
                {
                    connection.Open();
                    string query = "DELETE FROM alunos WHERE Id = @Id";
                    var cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@Id", id);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        Console.WriteLine("Aluno excluído com sucesso!");
                    }
                    else
                    {
                        Console.WriteLine("Aluno não encontrado.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao excluir aluno: {ex.Message}");
            }
        }

        private static void ListarAlunosPorCurso()
        {
            try
            {
                Console.Write("Digite o curso para buscar: ");
                string curso = Console.ReadLine();

                using (var connection = new MySqlConnection(ConnectionString))
                {
                    connection.Open();
                    string query = "SELECT Id, Nome, Idade, Curso, DataMatricula FROM alunos WHERE Curso = @Curso";
                    var cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@Curso", curso);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (!reader.HasRows)
                        {
                            Console.WriteLine("Nenhum aluno encontrado.");
                            return;
                        }

                        while (reader.Read())
                        {
                            Console.WriteLine($"ID: {reader["Id"]}, Nome: {reader["Nome"]}, Idade: {reader["Idade"]}, Curso: {reader["Curso"]}, Data de Matrícula: {reader["DataMatricula"]}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar alunos: {ex.Message}");
            }
        }

        private static Aluno ObterDadosAluno()
        {
            Console.Write("Nome: ");
            string nome = Console.ReadLine();
            Console.Write("Idade: ");
            int idade = int.Parse(Console.ReadLine());
            Console.Write("Curso: ");
            string curso = Console.ReadLine();
            Console.Write("Data de Matrícula (YYYY-MM-DD): ");
            DateTime dataMatricula = DateTime.Parse(Console.ReadLine());

            return new Aluno { Nome = nome, Idade = idade, Curso = curso, DataMatricula = dataMatricula };
        }

        private static Aluno AtualizarDadosAluno(Aluno aluno)
        {
            Console.Write("Novo nome (deixe em branco para não alterar): ");
            string nome = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(nome)) aluno.Nome = nome;

            Console.Write("Nova idade (deixe em branco para não alterar): ");
            string idadeInput = Console.ReadLine();
            if (int.TryParse(idadeInput, out int idade)) aluno.Idade = idade;

            Console.Write("Novo curso (deixe em branco para não alterar): ");
            string curso = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(curso)) aluno.Curso = curso;

            Console.Write("Nova data de matrícula (deixe em branco para não alterar): ");
            string dataInput = Console.ReadLine();
            if (DateTime.TryParse(dataInput, out DateTime dataMatricula)) aluno.DataMatricula = dataMatricula;

            return aluno;
        }

        private static int ObterIdAluno()
        {
            Console.Write("ID do aluno: ");
            return int.Parse(Console.ReadLine());
        }

        private static Aluno ObterAlunoPorId(MySqlConnection connection, int id)
        {
            string query = "SELECT Id, Nome, Idade, Curso, DataMatricula FROM alunos WHERE Id = @Id";
            var cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Id", id);

            using (var reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    return new Aluno
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        Nome = reader["Nome"].ToString(),
                        Idade = Convert.ToInt32(reader["Idade"]),
                        Curso = reader["Curso"].ToString(),
                        DataMatricula = Convert.ToDateTime(reader["DataMatricula"])
                    };
                }
            }
            return null;
        }
    }


}
