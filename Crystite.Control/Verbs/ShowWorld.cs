//
//  SPDX-FileName: ShowWorld.cs
//  SPDX-FileCopyrightText: Copyright (c) Jarl Gullberg
//  SPDX-License-Identifier: AGPL-3.0-or-later
//

using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using CommandLine;
using Crystite.API.Abstractions;
using Crystite.Control.API;
using Crystite.Control.Verbs.Bases;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Remora.Results;

namespace Crystite.Control.Verbs;

/// <summary>
/// Shows a specific running world.
/// </summary>
[Verb("show-world", HelpText = "Shows a specific running world")]
public sealed class ShowWorld : WorldVerb
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ShowWorld"/> class.
    /// </summary>
    /// <inheritdoc cref="WorldVerb(string, string, ushort, string, bool)" path="/param" />
    [SuppressMessage("Documentation", "CS1573", Justification = "Copied from base class")]
    public ShowWorld(string? name, string? id, ushort port, string server, bool verbose)
        : base(name, id, port, server, verbose)
    {
    }

    /// <inheritdoc />
    public override async ValueTask<Result> ExecuteAsync(IServiceProvider services, CancellationToken ct = default)
    {
        var worldAPI = services.GetRequiredService<HeadlessWorldAPI>();
        var outputWriter = services.GetRequiredService<TextWriter>();
        var outputOptions = services.GetRequiredService<IOptionsMonitor<JsonSerializerOptions>>().Get("Crystite");

        IRestWorld? world;
        if (this.ID is null)
        {
            var getWorlds = await worldAPI.GetWorldsAsync(ct);
            if (!getWorlds.IsDefined(out var worlds))
            {
                return (Result)getWorlds;
            }

            world = worlds.FirstOrDefault(w => w.Name.Equals(this.Name, StringComparison.OrdinalIgnoreCase));
            if (world is null)
            {
                return new NotFoundError($"No world named \"{this.Name}\" found");
            }
        }
        else
        {
            var getWorld = await worldAPI.GetWorldAsync(this.ID, ct);
            if (!getWorld.IsDefined(out world))
            {
                return (Result)getWorld;
            }
        }

        if (this.Verbose)
        {
            await outputWriter.WriteLineAsync(JsonSerializer.Serialize(world, outputOptions));
        }
        else
        {
            var description = world.Description is null
                ? string.Empty
                : $"\t{world.Description}";

            await outputWriter.WriteLineAsync($"{world.Name}\t{world.Id}{description}");
        }

        return Result.FromSuccess();
    }
}
