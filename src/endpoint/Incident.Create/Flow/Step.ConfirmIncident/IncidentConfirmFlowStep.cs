using GarageGroup.Infra.Bot.Builder;
using Newtonsoft.Json;
using System.IO.Compression;
using System.IO;
using System.Text;
using System;
using System.Web;
using Microsoft.Extensions.Logging;

namespace GarageGroup.Internal.Support;

internal static class IncidentConfirmFlowStep
{
    internal static ChatFlow<IncidentCreateFlowState> ConfirmIncident(this ChatFlow<IncidentCreateFlowState> chatFlow)
        =>
        chatFlow.AwaitConfirmation(
            CreateIncidentConfirmationOption, GetWebAppData)
        .Forward(
            NextOrRestart)
        .ShowEntityCard(
            CreateIncidentCardOption)
        .SetTypingStatus();

    private static ConfirmationCardOption CreateIncidentConfirmationOption(IChatFlowContext<IncidentCreateFlowState> context)
    {
        var webAppUrl = context.FlowState.BuildWebAppUrl();
        context.Logger.LogInformation("WebAppUrl: {webAppUrl}", webAppUrl);

        return new(
           entity: context.InnerCreateIncidentCardOption(
               headerText: "Создать обращение?",
               skipStep: context.FlowState.WithoutConfirmation),
            buttons: new(
                confirmButtonText: "Создать",
                cancelButtonText: "Отменить",
                cancelText: "Создание инцидента было отменено")
            {
                TelegramWebApp = new(webAppUrl)
            });
    }

    private static EntityCardOption CreateIncidentCardOption(IChatFlowContext<IncidentCreateFlowState> context)
        =>
        context.InnerCreateIncidentCardOption("Изменение обращения", context.FlowState.WithoutConfirmation is false);

    private static EntityCardOption InnerCreateIncidentCardOption(
        this IChatFlowContext<IncidentCreateFlowState> context, string headerText, bool skipStep)
        =>
        new(
            headerText: headerText,
            fieldValues:
                [
                    new("Заголовок", context.FlowState.Title),
                    new("Клиент", context.FlowState.Customer?.Title),
                    new("Контакт", context.FlowState.Contact?.FullName ?? "--"),
                    new("Тип обращения", context.FlowState.CaseTypeTitle),
                    new("Приоритет", context.FlowState.PriorityTitle),
                    new("Ответственный", context.FlowState.Owner?.FullName),
                    new("Описание", context.FlowState.Description?.Value)
                ])
        {
            SkipStep = skipStep
        };

    private static ChatFlowJump<IncidentCreateFlowState> NextOrRestart(IChatFlowContext<IncidentCreateFlowState> context)
    {
        if (context.FlowState.Customer is null || context.FlowState.Contact is null || context.FlowState.Owner is null)
        {
            return ChatFlowJump.Restart(context.FlowState with
            {
                WithoutConfirmation = false
            });
        }

        return ChatFlowJump.Next(context.FlowState);
    }

    private static Result<IncidentCreateFlowState, BotFlowFailure> GetWebAppData(
        IChatFlowContext<IncidentCreateFlowState> context, string webAppData)
    {
        var support = JsonConvert.DeserializeObject<WebAppCreateSupportDataJson>(webAppData);

        return context.FlowState with
        {
            Title = support.Title,
            Customer = support.Customer,
            Contact = support.Contact,
            CaseTypeCode = support.CaseTypeCode,
            CaseTypeTitle = GetCaseTitle(support.CaseTypeCode),
            PriorityTitle = GetPriorityTitle(support.PriorityCode),
            Owner = support.Owner,
            Description = new(support.Description.OrEmpty()),
            WithoutConfirmation = true,
            IsNotFirstLaunch = true
        };
    }

    private static string BuildWebAppUrl(this IncidentCreateFlowState state)
    {
        var timesheet = new WebAppCreateSupportDataJson
        {
            Title = state.Title,
            Customer = state.Customer,
            Contact = state.Contact,
            CaseTypeCode = state.CaseTypeCode,
            PriorityCode = state.PriorityCode,
            Owner = state.Owner,
            Description = state.Description?.Value,
        };

        var data = timesheet.CompressDataJson();
        return $"{state.UrlWebApp}/updateSupportForm?data={HttpUtility.UrlEncode(data)}";
    }

    private static string CompressDataJson(this WebAppCreateSupportDataJson data)
    {
        var json = JsonConvert.SerializeObject(data);

        var buffer = Encoding.UTF8.GetBytes(json);
        var memoryStream = new MemoryStream();

        using (var zipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
        {
            zipStream.Write(buffer, 0, buffer.Length);
        }

        return Convert.ToBase64String(memoryStream.ToArray());
    }

    private static string GetCaseTitle(IncidentCaseTypeCode code)
        =>
        code switch
        {
            IncidentCaseTypeCode.Question => "Вопрос",
            IncidentCaseTypeCode.Problem => "Проблема",
            _ => "Запрос"
        };

    private static string GetPriorityTitle(IncidentPriorityCode code)
        =>
        code switch
        {
            IncidentPriorityCode.Hight => "Высокий",
            IncidentPriorityCode.Normal => "Обычный",
            _ => "Низкий"
        };
}