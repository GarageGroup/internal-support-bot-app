using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Web;
using GarageGroup.Infra.Telegram.Bot;
using Microsoft.Extensions.Logging;

namespace GarageGroup.Internal.Support;

using static IncidentCreateResource;

partial class IncidentCreateFlowStep
{
    internal static ChatFlow<IncidentCreateFlowState> ExpectIncidentConfirmationOrSkip(
        this ChatFlow<IncidentCreateFlowState> chatFlow)
        =>
        chatFlow.ExpectConfirmationOrSkip(
            CreateConfirmationOption)
        .Forward(
            NextOrRestart)
        .ShowEntityCardOrSkip(
            CreateIncidentCardOption);

    private static ConfirmationCardOption<IncidentCreateFlowState>? CreateConfirmationOption(
        IChatFlowContext<IncidentCreateFlowState> context)
    {
        if (context.FlowState.WithoutConfirmation)
        {
            return null;
        }

        return new()
        {
            Entity = context.InnerCreateIncidentCardOption(context.Localizer[ConfirmationHeaderText]),
            Keyboard = new(
                confirmButtonText: context.Localizer[ConfirmButton],
                cancelButtonText: context.Localizer[CancelButton],
                cancelText: context.Localizer[CancelText])
            {
                WebAppButton = context.BuildWebAppButton()
            }
        };
    }

    private static EntityCardOption? CreateIncidentCardOption(IChatFlowContext<IncidentCreateFlowState> context)
    {
        if (context.FlowState.WithoutConfirmation is false)
        {
            return null;
        }

        return context.InnerCreateIncidentCardOption(context.Localizer[ModificationHeaderText]);
    }

    private static EntityCardOption InnerCreateIncidentCardOption(
        this IChatFlowContext<IncidentCreateFlowState> context, string headerText)
    {
        FlatArray<KeyValuePair<string, string?>> fieldValues =
            [
                new(context.Localizer[TitleFieldName], context.FlowState.Title),
                new(context.Localizer[CustomerFieldName], context.FlowState.Customer?.Title),
                new(context.Localizer[ContactFieldName], (context.FlowState.Contact?.FullName).OrNullIfEmpty() ?? "--"),
                new(context.Localizer[CaseTypeFieldName], context.FlowState.CaseType?.Title),
                new(context.Localizer[PriorityFieldName], context.FlowState.Priority?.Title),
                new(context.Localizer[OwnerFieldName], context.FlowState.Owner?.FullName),
                new(context.Localizer[DescriptionFieldName], context.FlowState.Description)
            ];

        var annotationFileNames = context.GetAnnotationFileNames();
        if (string.IsNullOrEmpty(annotationFileNames) is false)
        {
            fieldValues = fieldValues.Concat([new(context.Localizer[AttachmentsFieldName], annotationFileNames)]);
        }

        return new(headerText)
        {
            FieldValues = fieldValues
        };
    }  

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

    private static CardWebAppButton<IncidentCreateFlowState>? BuildWebAppButton(this IChatFlowContext<IncidentCreateFlowState> context)
    {
        if (context.WebApp is null || context.FlowState.WithoutConfirmation)
        {
            return null;
        };

        var state = context.FlowState;

        var timesheet = new IncidentWebAppData
        {
            Title = state.Title,
            Customer = state.Customer is null ? null : new(state.Customer.Id, state.Customer.Title),
            Contact = state.Contact is null ? null : new(state.Contact.Id, state.Contact.FullName),
            CaseTypeCode = state.CaseType?.Code ?? default,
            PriorityCode = state.Priority?.Code ?? default,
            Owner = state.Owner is null ? null : new(state.Owner.Id, state.Owner.FullName),
            Description = state.Description,
        };

        var data = timesheet.CompressDataJson();

        var webAppUrl = context.WebApp.BuildUrl("updateSupportForm", [new("data", HttpUtility.UrlEncode(data))]);
        context.Logger.LogInformation("WebAppUrl: {webAppUrl}", webAppUrl);

        return new(
            buttonName: context.Localizer[EditButton],
            webAppUrl: webAppUrl,
            forward: context.ParseWebAppOrRepeat);
    }

    private static Result<IncidentCreateFlowState, ChatRepeatState> ParseWebAppOrRepeat(
        this IChatFlowContext<IncidentCreateFlowState> context, BotWebAppData webAppData)
    {
        if (string.IsNullOrWhiteSpace(webAppData.Data))
        {
            return default;
        }

        var incident = JsonSerializer.Deserialize<IncidentWebAppData>(webAppData.Data, SerializerOptions);
        if (incident is null)
        {
            return default;
        }

        context.Logger.LogInformation("WebAppCustomerId: {customerId}, ContactId: {contactId}", incident.Customer?.Id, incident.Contact?.Id);

        return context.FlowState with
        {
            Title = incident.Title,
            Customer = incident.Customer is null ? null : new(incident.Customer.Id, incident.Customer.Title),
            Contact = incident.Contact is null ? null : new(incident.Contact.Id, incident.Contact.FullName),
            CaseType = new(incident.CaseTypeCode, context.GetDisplayName(incident.CaseTypeCode)),
            Priority = new(incident.PriorityCode, context.GetDisplayName(incident.PriorityCode)),
            Owner = incident.Owner is null ? null : new(incident.Owner.Id, incident.Owner.FullName),
            Description = incident.Description,
            WithoutConfirmation = true,
            IsRepeated = true
        };
    }

    private static string CompressDataJson(this IncidentWebAppData data)
    {
        var json = JsonSerializer.Serialize(data, SerializerOptions);

        var buffer = Encoding.UTF8.GetBytes(json);
        var memoryStream = new MemoryStream();

        using (var zipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
        {
            zipStream.Write(buffer, 0, buffer.Length);
        }

        return Convert.ToBase64String(memoryStream.ToArray());
    }

    private static string GetAnnotationFileNames(this IChatFlowContext<IncidentCreateFlowState> context)
    {
        return string.Join(", ", context.FlowState.Pictures.AsEnumerable().Select(GetFileName));

        static string GetFileName(PictureState picture)
            =>
            picture.FileName;
    }
}