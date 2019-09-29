// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.AI.QnA;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using QnABot.Bots;


namespace Microsoft.BotBuilderSamples
{
    public class QnABot : ActivityHandler
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<QnABot> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public QnABot(IConfiguration configuration, ILogger<QnABot> logger, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var httpClient = _httpClientFactory.CreateClient();

            var qnaMaker = new QnAMaker(new QnAMakerEndpoint
            {
                KnowledgeBaseId = _configuration["QnAKnowledgebaseId"],
                EndpointKey = _configuration["QnAAuthKey"],
                Host = GetHostname()
            },
            null,
            httpClient);

            _logger.LogInformation("Calling QnA Maker");

            // The actual call to the QnA Maker service.
            var response = await qnaMaker.GetAnswersAsync(turnContext);

            //processing rich cards
            string[] QnA_Response = response[0].Answer.Split(';');
            int dataSize = QnA_Response.Length;

            if (response != null && response.Length > 0)
            {
                var attachments = new List<Attachment>();

                //respond to activity with interactive rich cards
                var InteractiveResponse = MessageFactory.Attachment(attachments);

                // since sign-in card has 2 parameters
                if (dataSize == 2)
                {
                    InteractiveResponse.Attachments.Add(Cards.GetSigninCard(QnA_Response).ToAttachment());
                    await turnContext.SendActivityAsync(InteractiveResponse, cancellationToken);
                }

                // since image card (Hero card) has 5 parameters
                else if (dataSize == 5)
                {
                    InteractiveResponse.Attachments.Add(Cards.GetHeroCard(QnA_Response).ToAttachment());
                    await turnContext.SendActivityAsync(InteractiveResponse, cancellationToken);
                }

                // since video card has 6 parameters

                else if (dataSize == 6)
                {
                    InteractiveResponse.Attachments.Add(Cards.GetVideoCard(QnA_Response).ToAttachment());
                    await turnContext.SendActivityAsync(InteractiveResponse, cancellationToken);
                }
                else
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text(response[0].Answer), cancellationToken);
                }
            }

            else
            {
                await turnContext.SendActivityAsync(MessageFactory.Text("Sorry! I don't know that. We'll get back to you soon."), cancellationToken);
            }
        }

        private string GetHostname()
        {
            var hostname = _configuration["QnAEndpointHostName"];
            if (!hostname.StartsWith("https://"))
            {
                hostname = string.Concat("https://", hostname);
            }

            if (!hostname.EndsWith("/qnamaker"))
            {
                hostname = string.Concat(hostname, "/qnamaker");
            }

            return hostname;
        }
    }
}
