﻿/*
 * OCPP.Core - https://github.com/dallmann-consulting/OCPP.Core
 * Copyright (C) 2020-2021 dallmann consulting GmbH.
 * All Rights Reserved.
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <https://www.gnu.org/licenses/>.
 */

namespace ChargingStation.Common.Messages_OCPP16;
#pragma warning disable // Disable all warnings

[System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.3.1.0 (Newtonsoft.Json v9.0.0.0)")]
public partial class MeterValuesRequest
{
    [Newtonsoft.Json.JsonProperty("connectorId", Required = Newtonsoft.Json.Required.Always)]
    public int ConnectorId { get; set; }

    [Newtonsoft.Json.JsonProperty("transactionId", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public int TransactionId { get; set; }

    [Newtonsoft.Json.JsonProperty("meterValue", Required = Newtonsoft.Json.Required.Always)]
    [System.ComponentModel.DataAnnotations.Required]
    public System.Collections.Generic.ICollection<MeterValue> MeterValue { get; set; } = new System.Collections.ObjectModel.Collection<MeterValue>();


}

[System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.3.1.0 (Newtonsoft.Json v9.0.0.0)")]
public partial class MeterValue
{
    [Newtonsoft.Json.JsonProperty("timestamp", Required = Newtonsoft.Json.Required.Always)]
    [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
    public System.DateTimeOffset Timestamp { get; set; }

    [Newtonsoft.Json.JsonProperty("sampledValue", Required = Newtonsoft.Json.Required.Always)]
    [System.ComponentModel.DataAnnotations.Required]
    public System.Collections.Generic.ICollection<SampledValue> SampledValue { get; set; } = new System.Collections.ObjectModel.Collection<SampledValue>();
}