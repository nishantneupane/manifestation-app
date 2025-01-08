using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using OpenAI;
using OpenAI.Chat;
using ManifestationApp.Models;

namespace ManifestationApp.Services
{
    public class OpenAIManifestationService : IManifestationService
    {
        private readonly string _apiKey;

        public OpenAIManifestationService(IConfiguration configuration)
        {
            // Read your OpenAI API key from config
            _apiKey = configuration["OpenAI:ApiKey"];
        }

        public async Task<string> GenerateManifestationAsync(UserGoal goal)
        {
            // 1. Create the parent client
            var openAiClient = new OpenAIClient(_apiKey);

            // 2. Get a ChatClient configured for your model (e.g., "gpt-3.5-turbo")
            var chatClient = openAiClient.GetChatClient("gpt-3.5-turbo");

            // 3. Build the conversation history/messages
            var messages = new List<ChatMessage>
            {
                new SystemChatMessage("You are a wise, supportive manifestation coach."),
                new UserChatMessage($@"The user has this goal: '{goal.GoalDescription}'.
                    Using a friendly, encouraging tone, please speak **directly** to the user:
                    1. Give them a concise, uplifting daily affirmation that boosts confidence and positivity.
                    2. Offer a small, practical action step they can do today that aligns with their goal.
                    3. Write it **as if** you are talking one-on-one with them, without bullet points or lists.

                    Make it short yet inspiring, and address them personally (using 'you' or 'your').
                    ")};

            // 4. Call the chat completion method (async)
            //    Optionally pass temperature, etc.
            ChatCompletion completion = await chatClient.CompleteChatAsync(
                messages
            );

            // 5. Check the result
            if (completion != null && completion.Content.Any())
            {
                return completion.Content[0].Text.Trim();
            }
            else
            {
                // Fallback
                return "Stay positive!";
            }
        }
    }
}
