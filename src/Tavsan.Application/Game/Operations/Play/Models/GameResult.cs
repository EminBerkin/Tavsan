using Tavsan.Domain.Game.Models;
using Tavsan.Domain.Game.Enums;

namespace Tavsan.Application.Game.Operations.Play.Models;

public sealed record GameResult(GameStatus Status,
                                Position FinalPosition,
                                IReadOnlyList<StepResult> Steps,
                                int SkippedCommands);
