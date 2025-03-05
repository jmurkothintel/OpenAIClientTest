// See https://aka.ms/new-console-template for more information
using Azure.AI.OpenAI;
using OpenAI;
using OpenAI.Chat;
using System.ClientModel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.UserSecrets; // Add this using directive

Console.WriteLine("Hello, World!");

var builder = new ConfigurationBuilder()
    .AddUserSecrets<Program>() // This will now work
    .Build();

string azureDeploymentEndpoint = builder["AzureDeploymentEndpoint"];
string apikey = builder["ApiKey"];
string azuredeploymentname = builder["AzureDeploymentName"];

AzureOpenAIClient client = new AzureOpenAIClient(new Uri(azureDeploymentEndpoint), new ApiKeyCredential(apikey));

var chatClient = client.GetChatClient(azuredeploymentname);

List<ChatMessage> messages = new List<ChatMessage>();
messages.Add(new SystemChatMessage("You are an AI assistant that helps people find information."));
List<ChatMessageContentPart> messageContentParts = new List<ChatMessageContentPart>();

// Comment below line
messageContentParts.Add(ChatMessageContentPart.CreateImagePart(new Uri("https://th.bing.com/th/id/OIP.8n-niBlZ9IXGsxEylLaoVAHaFj?rs=1&pid=ImgDetMain"), ChatImageDetailLevel.Low));
// Comment below line
messageContentParts.Add(ChatMessageContentPart.CreateImagePart(new Uri("https://www.nps.gov/npgallery/GetAsset/603bd60e-6750-4655-81d1-364735790532/proxy/hires?"), ChatImageDetailLevel.Low));
messageContentParts.Add(ChatMessageContentPart.CreateTextPart("using the image help answer the questions"));
messageContentParts.Add(ChatMessageContentPart.CreateTextPart("Do both these images show the same object? If not describe 3 differences in bullet points"));
messages.Add(new UserChatMessage(messageContentParts));

//IF you comment the lines with the urls  usage starts showing up.
await foreach (StreamingChatCompletionUpdate message in chatClient.CompleteChatStreamingAsync(messages))
{
    if (message.Usage != null)
    {
        Console.WriteLine();
        Console.WriteLine("**********************************************************");
        Console.WriteLine($"Input Token Count {message.Usage.InputTokenCount}");
        Console.WriteLine($"Output Token Count {message.Usage.OutputTokenCount}");
        Console.WriteLine($"Total Token Count {message.Usage.TotalTokenCount}");
        Console.WriteLine("**********************************************************");
    }
    if (message.ContentUpdate.Count > 0)
    {
        Console.Write(message.ContentUpdate[0].Text);
    }
}
