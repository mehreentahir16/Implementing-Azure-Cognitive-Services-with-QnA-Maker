using System.Collections.Generic;
using Microsoft.Bot.Schema;

namespace QnABot.Bots
{
    public static class Cards
    {
        public static SigninCard GetSigninCard(string[] QnA_Response)
        {
            var signinCard = new SigninCard
            {
                Text = QnA_Response[0],
                Buttons = new List<CardAction> { new CardAction(ActionTypes.Signin, "Sign-in", value: QnA_Response[1]) },
            };

            return signinCard;
        }

        public static HeroCard GetHeroCard(string[] QnA_Response)
        {
            var heroCard = new HeroCard
            {
                Title = QnA_Response[0],
                Subtitle = QnA_Response[1],
                Text = QnA_Response[2],
                Images = new List<CardImage> { new CardImage(QnA_Response[3]) },
                Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "Get Started", value: QnA_Response[4]) },
            };

            return heroCard;
        }

        public static VideoCard GetVideoCard(string[] QnA_Response)
        {
            var videoCard = new VideoCard
            {
                Title = QnA_Response[0],
                Subtitle = QnA_Response[1],
                Text = QnA_Response[2],
                Image = new ThumbnailUrl { Url = QnA_Response[3] },
                Media = new List<MediaUrl> { new MediaUrl() { Url = QnA_Response[4] } },
                Buttons = new List<CardAction> { new CardAction() { Title = "Learn More", Type = ActionTypes.OpenUrl, Value = QnA_Response[5], }, },
            };

            return videoCard; 
        }
    }
}
