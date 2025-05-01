using RabbitMQApp.Receivers;
using RabbitMQApp.Senders;
using RabbitMQApp.Services;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Aplicativo RabbitMQ - Escolha uma opção:");
        Console.WriteLine("1 - Iniciar Validation Service");
        Console.WriteLine("2 - Iniciar Receiver 1 (Frutas)");
        Console.WriteLine("3 - Iniciar Receiver 2 (Usuários)");
        Console.WriteLine("4 - Enviar mensagem de fruta");
        Console.WriteLine("5 - Enviar mensagem de usuário");

        var option = Console.ReadLine();

        switch (option)
        {
            case "1":
                ValidationService.Start();
                break;
            case "2":
                FruitReceiver.Start();
                break;
            case "3":
                UserReceiver.Start();
                break;
            case "4":
                SendFruitSample();
                break;
            case "5":
                SendUserSample();
                break;
            default:
                Console.WriteLine("Opção inválida!");
                break;
        }
    }

    static void SendFruitSample()
    {
        Console.WriteLine("Enviando exemplos de frutas da época...");

        FruitSender.SendFruitInfo("Manga", "Fruta tropical rica em vitamina A, típica do verão brasileiro.");
        Thread.Sleep(1000);

        FruitSender.SendFruitInfo("Jabuticaba", "Fruta nativa da Mata Atlântica, com casca roxa e polpa doce.");
        Thread.Sleep(1000);

        FruitSender.SendFruitInfo("Abacaxi", "Fruta tropical de sabor doce e ácido, rica em vitamina C.");
        Thread.Sleep(1000);

        FruitSender.Close();
        Console.WriteLine("Mensagens enviadas com sucesso!");
    }

    static void SendUserSample()
    {
        Console.WriteLine("Enviando exemplos de usuários...");

        UserSender.SendUserInfo(
            "João da Silva",
            "Rua das Flores, 123 - São Paulo, SP",
            "12.345.678-9",
            "12345678901",
            DateTime.Now.AddDays(-5)
        );
        Thread.Sleep(1000);

        UserSender.SendUserInfo(
            "Maria Oliveira",
            "Av. Paulista, 789 - São Paulo, SP",
            "98.765.432-1",
            "98765432101",
            DateTime.Now.AddDays(-2)
        );
        Thread.Sleep(1000);

        UserSender.SendUserInfo(
            "Pedro Santos",
            "Rua XV de Novembro, 456 - Curitiba, PR",
            "54.321.876-5",
            "54321876501",
            DateTime.Now.AddDays(-7)
        );
        Thread.Sleep(1000);

        UserSender.Close();
        Console.WriteLine("Mensagens enviadas com sucesso!");
    }
}