using Data;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenPop.Pop3;
using System.Text;

namespace Workers
{
    public class MailSQLLoggerWorker : BackgroundService
    {
        private readonly ILogger<MailSQLLoggerWorker> _logger;

        private const string LOGIN = "fortesttask.sarantsev@gmail.com";
        private const string PASSWORD = "uijzyiewpgxxezdu";

        private List<Letter> _letters = new List<Letter>();

        public MailSQLLoggerWorker(ILogger<MailSQLLoggerWorker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Run(() => 
                {
                    CheckInboxMails();
                    AddMailsIntoDB();
                });
                await Task.Delay(10_000, stoppingToken);
            }
        }

        private void AddMailsIntoDB()
        {
            using (var context = new MailContext())
            {
                foreach (var letter in _letters)
                {
                    var entity = context.Letters
                        .FirstOrDefault(x => x.Name == letter.Name && x.Date == letter.Date);
                    if (entity == null)
                    {
                        context.Add(letter);
                    }
                }
                context.SaveChanges();
            }
            
            _letters = new List<Letter>();
        }

        private void CheckInboxMails()
        {
            var client = new Pop3Client();
            client.Connect("pop.gmail.com", 995, true);
            client.Authenticate(LOGIN, PASSWORD);
            var count = client.GetMessageCount();
            for (int i = count; i >= 1; i--)
            {
                var message = client.GetMessage(i);
                var content = Encoding.UTF8.GetString(message.RawMessage);
                _letters.Add(new Letter
                {
                    Name = message.Headers.Subject,
                    Date = message.Headers.DateSent,
                    Receiver = LOGIN,
                    Sender = message.Headers.From.MailAddress.Address,
                    Content = content.Substring(content.IndexOf("Content-Type"))
                });
            }
            
        }
    }
}
