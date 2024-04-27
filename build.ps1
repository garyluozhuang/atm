# Format the code
dotnet format
# Build the code in release mode
dotnet build -c Release
# Build the code in debug mode
dotnet build -c Debug
# Run the unit test(s)
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
# Generate the documentation
doxygen Doxyfile
mkdir --parents _readthedocs/html/
cp --recursive docs/html/* _readthedocs/html/
