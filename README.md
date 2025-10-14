# PartsTrader Client Tools

## Overview
Small ASP.NET Core (.NET 8) Web API implementing a single endpoint to return original + compatible parts for a given part number.  

## Endpoint
`GET /parts/compatible/{partNumber}`  
Returns:
- 200 OK + JSON array of `PartsContract` (3 stub entries)
- 200 OK + empty array if the part is excluded
- 400 Bad Request for invalid format

## Part number format
Pattern used (case-insensitive): `####-XXXX…`
- 4 digits
- Dash
- 4 or more alphanumeric characters (letters or digits)
- No spaces or special characters (underscore, percent, second dash, etc.)

Examples  
Valid: `1235-dcba`, `1234-9a0B `, ` 0001-XYZ123`  
Invalid: `123-abcd`, `12345-abcd`, `1234-abc`, `1234-ab d`, `1234-ab_c`

User input is trimmed then lower‑cased.

## Exclusions
`Data/Exclusions.json` - array of excluded objects with a `PartNumber` field.  
Intentional re-read of the file on each call is implemented for simplicity.  
Missing, invalid, or malformed file is treated as empty exclusions (logs error/warning, no exception is thrown).

## Stub implementation
`PartsApplicationService`:
- Splits `{partId}-{partCode}`
- Always returns 3 synthetic parts: original, `partId - 1`, `partId + 1` with suffixes `partCode`, `partCode + "a"`, `partCode + "b"`
- `Id` is a random `Guid` per item
- Throws `InvalidPartNumberException` if given invalid input
A real implementation would query a data source.


## Unit tests
Using xUnit + Moq

Controller:
- Invalid formats - returns 400 Bad request
- Excluded part - returns empty, service not called
- Valid non-excluded - service invoked
- Trimming + lowercasing - happens before service call

Service:
- Numeric variant generation - 3 items, adjusted IDs

ExclusionsProvider:
- Missing file - returns empty, logs error
- Invalid JSON - returns empty, logs warning/error
- Valid JSON - search within the file is case-insensitive
- File change - second call reflects new content

## Running the API
Run PartsTrader.ClientTools.Api to launch the Swagger UI (Development)

## Running tests
dotnet test