rm -r "TestResults\*"

#
# advanced settings are explained in this section: https://github.com/coverlet-coverage/coverlet/blob/master/Documentation/VSTestIntegration.md#advanced-options-supported-via-runsettings
#
dotnet test --collect:"XPlat Code Coverage" --settings coverlet.runsettings
reportgenerator -reports:"TestResults\**\coverage.cobertura.xml" -targetdir:"coveragereport" -reporttypes:Html
Invoke-Item ".\coveragereport\index.html"
