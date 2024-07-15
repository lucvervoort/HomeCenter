// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming

using System.Text.Json.Serialization;

namespace HomeCenter.Mqtt.Server;

public class JTSHeader
{
    [JsonPropertyName("startTime")]
    public DateTime StartTime { get; set; } = DateTime.Now;

    [JsonPropertyName("endTime")]
    public DateTime EndTime { get; set; } = DateTime.Now;

    [JsonPropertyName("recordCount")]
    public int RecordCount { get; set; } = 0;

    [JsonPropertyName("columns")]
    public JTSHeaderColumns Columns { get; set; } = new JTSHeaderColumns();
}
