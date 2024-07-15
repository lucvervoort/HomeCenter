// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming

using System.Text.Json.Serialization;

namespace HomeCenter.Mqtt.Server;

public class JTSRoot
{
    [JsonPropertyName("docType")]
    public string DocType { get; set; } = "jts";

    [JsonPropertyName("version")]
    public string Version { get; set; } = "1.0";

    [JsonPropertyName("header")]
    public JTSHeader Header { get; set; } = new JTSHeader();

    [JsonPropertyName("data")]
    public List<JTSData> Data { get; set; } = [];
}
