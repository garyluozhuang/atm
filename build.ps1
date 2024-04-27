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
git config --global user.email "zluo3@wpi.edu"
git config --global user.name "garyluozhuang"
cd docs
git add --all
git commit -m "Update documentation"
git push -u origin main






