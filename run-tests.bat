@echo off
echo Running tests with coverage...

dotnet test tests\OrdersApi.Tests ^
  --settings coverage.runsettings ^
  --results-directory coverage

if %ERRORLEVEL% neq 0 (
    echo Tests failed.
    exit /b %ERRORLEVEL%
)

echo Generating HTML report...

reportgenerator ^
  -reports:coverage\**\coverage.cobertura.xml ^
  -targetdir:coverage\report ^
  -reporttypes:Html ^
  -classfilters:-Program

echo Opening report...
start coverage\report\index.html
