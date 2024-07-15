// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming

using System.Text.Json.Serialization;

namespace HomeCenter.Mqtt.Server;

public class JTSData
{
    [JsonPropertyName("ts")]
    public DateTime Ts { get; set; } = DateTime.Now;

    [JsonPropertyName("f")]
    public JTSF F { get; set; } = new();
}