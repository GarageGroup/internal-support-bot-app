using System;
using GarageGroup.Infra.Telegram.Bot;

namespace GarageGroup.Internal.Support;

public interface IIncidentCreateCommand : IChatCommand<IncidentCreateCommandIn, Unit>, IChatCommandParser<IncidentCreateCommandIn>;