// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming

using System.Text.Json.Serialization;

namespace HomeCenter.Mqtt.Server;

public class JTSF
{
    [JsonPropertyName("0")]
    public JTSFValue F0 { get; set; } = new();
}
